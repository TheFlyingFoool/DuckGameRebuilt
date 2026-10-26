using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DuckGame
{
    public enum ChatImageState
    {
        /// <summary>Another player offered the image, waiting for the user to click it (or for an automatic download).</summary>
        Offered,
        Downloading,
        Decoding,
        Ready,
        Failed,
        /// <summary>Not shown because of the image or moji settings.</summary>
        Hidden,
    }

    /// <summary>
    /// An image attached to a chat message, either one we're sending or one someone else sent.
    /// </summary>
    public sealed class ChatImageAttachment
    {
        public uint id;
        public Profile sender;
        /// <summary>Where to download the image from, null for our own images.</summary>
        public NetworkConnection source;
        public string name = "";
        public int width;
        public int height;
        public int byteSize;
        public ChatImageState state;
        public float progress;
        public Tex2D texture;
        public Sprite sprite;
        /// <summary>The image file as sent (PNG or JPEG), kept for saving and copying.</summary>
        public byte[] fileData;
        /// <summary>Bumped every time something that changes the chat layout happens.</summary>
        public int version;

        // download state
        internal byte[] buffer;
        internal bool[] received;
        internal int receivedCount;
        internal int chunkCount;
        internal int framesSinceData;

        public bool isLocal => source == null;

        public void SetState(ChatImageState pState)
        {
            if (state == pState)
                return;
            state = pState;
            version++;
        }
    }

    /// <summary>
    /// Sending, receiving, caching and drawing chat images.
    /// <para>
    /// Flow: the sender keeps the (re-encoded) file around and sends <see cref="NMChatImageOffer"/> to DGR players right
    /// before the chat message. Receivers download it with <see cref="NMChatImageRequest"/> either right away or when the
    /// image is clicked (see <see cref="DGRSettings.ChatImageMode"/>), and the sender streams it back in throttled
    /// <see cref="NMChatImageChunk"/>s so it doesn't choke the game's own traffic. Vanilla players just get a text placeholder.
    /// </para>
    /// </summary>
    public static class ChatImages
    {
        public const int ModeOff = 0;
        public const int ModeAsk = 1;
        public const int ModeAuto = 2;

        public const int ChunkSize = 512;
        public const int MaxSendDimension = 1024;
        public const int MaxSendBytes = 768 * 1024;
        public const int MaxReceiveBytes = 2 * 1024 * 1024;
        public const int MaxReceiveDimension = MaxSendDimension;
        private const int ChunksPerFrame = 10;
        /// <summary>Unacknowledged chunks allowed per upload, so images never pile up in front of the game's own reliable messages.</summary>
        private const int ChunkWindow = 24;
        private const int OfferLifetimeFrames = 60 * 5;
        private const int MaxUploadsPerConnection = 6;
        private const int MaxOutgoingImages = 32;
        private const int MaxLoadedImages = 48;
        private const int DownloadTimeoutFrames = 60 * 20;

        public static readonly string[] FileExtensions = { "png", "jpg", "jpeg", "gif", "bmp", "tga", "webp" };

        internal class Upload
        {
            public NetworkConnection connection;
            public uint id;
            public byte[] data;
            public int nextChunk;
            public int inFlight;
            public int generation;
        }

        private static readonly Dictionary<uint, byte[]> s_outgoing = new Dictionary<uint, byte[]>();
        private static readonly List<uint> s_outgoingOrder = new List<uint>();
        private static readonly Dictionary<uint, ChatImageAttachment> s_downloads = new Dictionary<uint, ChatImageAttachment>();
        private static readonly Dictionary<Profile, ChatImageAttachment> s_pendingOffers = new Dictionary<Profile, ChatImageAttachment>();
        private static readonly List<Upload> s_uploads = new List<Upload>();
        private static readonly List<ChatImageAttachment> s_loaded = new List<ChatImageAttachment>();
        private static readonly List<ChatImageAttachment> s_localImages = new List<ChatImageAttachment>();
        private static readonly Dictionary<Profile, int> s_pendingOfferFrames = new Dictionary<Profile, int>();
        private static int s_frame;
        private static readonly ConcurrentQueue<Action> s_mainThreadQueue = new ConcurrentQueue<Action>();
        private static readonly System.Random s_random = new System.Random();
        private static int s_uploadCursor;
        private static bool s_wasNetworkActive;

        /// <summary>The image attached to the message currently being typed.</summary>
        public static ChatImageAttachment pendingAttachment;
        /// <summary>True while a picked image is being loaded and re-encoded.</summary>
        public static bool preparingAttachment;
        /// <summary>Shown in the chat box when loading a picked image failed.</summary>
        public static string attachmentError;
        private static int s_attachmentErrorTimer;

        public static int mode => DGRSettings.ChatImageMode;

        public static bool sendingEnabled => mode != ModeOff;

        #region Permissions

        /// <summary>Whether we send this player the actual image (instead of a text placeholder).</summary>
        public static bool CanSendTo(Profile profile)
        {
            return profile != null && profile.isUsingRebuilt && profile.inSameRebuiltVersion;
        }

        /// <summary>Whether images from this player should be shown at all.</summary>
        public static bool AllowedFrom(Profile sender, NetworkConnection connection)
        {
            if (mode == ModeOff || Options.Data.mojiFilter == 0)
                return false;
            if (Options.Data.mojiFilter == 1 && connection != null && connection.data is User user && user.relationship != FriendRelationship.Friend)
                return false;
            return true;
        }

        #endregion

        #region Picking an image to send

        public static void OpenFilePicker()
        {
            if (!sendingEnabled || preparingAttachment)
                return;
            try
            {
                string pattern = string.Join(";", FileExtensions);
                FileDialogFna dialog = new FileDialogFna();
                dialog.ShowOpenFileDialog(OnFilePicked, window: MonoMain.instance.Window.Handle,
                    filters: FileDialogFna.GetDialogFileFilters("Images (" + pattern.Replace(";", ", ") + ")", pattern));
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
                SetAttachmentError("Couldn't open the file picker");
            }
        }

        // May be called from another thread by SDL
        private static void OnFilePicked(List<string> files)
        {
            string file = files?.FirstOrDefault();
            if (string.IsNullOrEmpty(file))
                return;
            s_mainThreadQueue.Enqueue(() => AttachFile(file));
        }

        public static void AttachFile(string path)
        {
            if (!sendingEnabled || preparingAttachment)
                return;
            preparingAttachment = true;
            attachmentError = null;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    FileInfo info = new FileInfo(path);
                    if (info.Length > 64L * 1024 * 1024)
                        throw new Exception("That file is way too big");
                    byte[] data = File.ReadAllBytes(path);
                    PrepareAttachment(data, Path.GetFileName(path));
                }
                catch (Exception e)
                {
                    s_mainThreadQueue.Enqueue(() =>
                    {
                        preparingAttachment = false;
                        SetAttachmentError(e.Message);
                    });
                }
            });
        }

        public static void AttachBytes(byte[] data, string name)
        {
            if (!sendingEnabled || preparingAttachment)
                return;
            preparingAttachment = true;
            attachmentError = null;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    PrepareAttachment(data, name);
                }
                catch (Exception e)
                {
                    s_mainThreadQueue.Enqueue(() =>
                    {
                        preparingAttachment = false;
                        SetAttachmentError(e.Message);
                    });
                }
            });
        }

        /// <summary>
        /// Tries to grab an image (or an image file copied in the file explorer) from the clipboard.
        /// </summary>
        /// <returns>True if the clipboard had an image that's now being attached.</returns>
        public static bool TryAttachFromClipboard()
        {
            if (!sendingEnabled || preparingAttachment || Program.isLinux)
                return false;
            byte[] imageData = null;
            string filePath = null;
            try
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        if (System.Windows.Forms.Clipboard.ContainsImage())
                        {
                            using (System.Drawing.Image image = System.Windows.Forms.Clipboard.GetImage())
                            {
                                if (image != null)
                                {
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                        imageData = stream.ToArray();
                                    }
                                }
                            }
                        }
                        else if (System.Windows.Forms.Clipboard.ContainsFileDropList())
                        {
                            foreach (string file in System.Windows.Forms.Clipboard.GetFileDropList())
                            {
                                string extension = Path.GetExtension(file).TrimStart('.').ToLowerInvariant();
                                if (FileExtensions.Contains(extension))
                                {
                                    filePath = file;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join(1000);
            }
            catch (Exception)
            {
                return false;
            }
            if (imageData != null)
            {
                AttachBytes(imageData, "");
                return true;
            }
            if (filePath != null)
            {
                AttachFile(filePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drops the image attached to the message being typed (freeing its texture, it was never sent).
        /// </summary>
        public static void DiscardPendingAttachment()
        {
            ChatImageAttachment attachment = pendingAttachment;
            pendingAttachment = null;
            if (attachment == null || s_localImages.Contains(attachment))
                return;
            attachment.sprite = null;
            attachment.texture?.Dispose();
            attachment.texture = null;
            s_outgoing.Remove(attachment.id);
            s_outgoingOrder.Remove(attachment.id);
        }

        /// <summary>
        /// Called once our own image was sent, it now lives in the chat until we leave.
        /// </summary>
        public static void OnSentLocal(ChatImageAttachment attachment)
        {
            if (attachment != null && !s_localImages.Contains(attachment))
                s_localImages.Add(attachment);
            if (pendingAttachment == attachment)
                pendingAttachment = null;
        }

        /// <summary>Runs something on the game thread at the start of the next frame.</summary>
        public static void RunOnMainThread(Action action) => s_mainThreadQueue.Enqueue(action);

        /// <summary>
        /// A file name (without extension) that's safe to save under: no folders, no invalid characters, not too long.
        /// </summary>
        public static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";
            name = name.Replace('\\', '/');
            name = name.Substring(name.LastIndexOf('/') + 1);
            int dot = name.LastIndexOf('.');
            if (dot > 0)
                name = name.Substring(0, dot);
            StringBuilder builder = new StringBuilder(name.Length);
            // Windows' rules, so names are safe for everyone no matter who sent them
            const string invalid = "<>:\"/\\|?*";
            foreach (char c in name)
            {
                if (c >= ' ' && c != (char)127 && invalid.IndexOf(c) < 0)
                    builder.Append(c);
            }
            string result = builder.ToString().Trim().Trim('.').Trim();
            if (result.Length > 64)
                result = result.Substring(0, 64).Trim();
            string upper = result.ToUpperInvariant();
            if (upper == "CON" || upper == "PRN" || upper == "AUX" || upper == "NUL" || upper.Length == 4 && (upper.StartsWith("COM") || upper.StartsWith("LPT")) && char.IsDigit(upper[3]))
                result = "_" + result;
            return result;
        }

        /// <summary>"png" or "jpg", from the file's signature.</summary>
        public static string GetFileExtension(byte[] data)
        {
            if (data != null && data.Length > 2 && data[0] == 0xFF && data[1] == 0xD8)
                return "jpg";
            return "png";
        }

        /// <summary>Shows a short error where the attached image would be (like when a dropped file can't be used).</summary>
        public static void ShowAttachmentError(string message) => SetAttachmentError(message);

        private static void SetAttachmentError(string message)
        {
            attachmentError = message;
            s_attachmentErrorTimer = 60 * 4;
        }

        // Runs on a worker thread
        private static void PrepareAttachment(byte[] data, string name)
        {
            byte[] encoded = EncodeForSending(data, out int width, out int height);
            Color[] pixels = DecodePixels(encoded, MaxSendDimension, out int decodedWidth, out int decodedHeight);
            s_mainThreadQueue.Enqueue(() =>
            {
                preparingAttachment = false;
                ChatImageAttachment attachment = new ChatImageAttachment
                {
                    id = NewImageId(),
                    sender = DuckNetwork.localProfile,
                    name = name ?? "",
                    width = width,
                    height = height,
                    byteSize = encoded.Length,
                    state = ChatImageState.Ready,
                };
                attachment.fileData = encoded;
                CreateTexture(attachment, pixels, decodedWidth, decodedHeight);
                StoreOutgoing(attachment.id, encoded);
                DiscardPendingAttachment();
                pendingAttachment = attachment;
                ChatBox.OnAttachmentChanged();
            });
        }

        private static uint NewImageId()
        {
            lock (s_random)
            {
                uint id;
                do
                {
                    id = (uint)s_random.Next(1, int.MaxValue) ^ ((uint)s_random.Next(0, 2) << 31);
                } while (id == 0 || s_outgoing.ContainsKey(id));
                return id;
            }
        }

        private static void StoreOutgoing(uint id, byte[] data)
        {
            s_outgoing[id] = data;
            s_outgoingOrder.Add(id);
            while (s_outgoingOrder.Count > MaxOutgoingImages)
            {
                s_outgoing.Remove(s_outgoingOrder[0]);
                s_outgoingOrder.RemoveAt(0);
            }
        }

        #endregion

        #region Encoding / decoding

        // GIF decoding is left out on purpose, ImageSharp 2.x has unpatched GIF decoder vulnerabilities and
        // only the first frame would be sent anyway
        private static readonly SixLabors.ImageSharp.Configuration s_sendConfiguration = new SixLabors.ImageSharp.Configuration(
            new PngConfigurationModule(),
            new JpegConfigurationModule(),
            new BmpConfigurationModule(),
            new WebpConfigurationModule(),
            new TgaConfigurationModule(),
            new TiffConfigurationModule(),
            new PbmConfigurationModule());

        // Received images come from other players, and EncodeForSending only ever produces PNG or JPEG
        private static readonly SixLabors.ImageSharp.Configuration s_receiveConfiguration = new SixLabors.ImageSharp.Configuration(
            new PngConfigurationModule(),
            new JpegConfigurationModule());

        /// <summary>
        /// Loads any supported image, keeps only the first frame, shrinks it until it fits the size limits and
        /// re-encodes it as PNG (or JPEG for big opaque images like photos).
        /// </summary>
        public static byte[] EncodeForSending(byte[] input, out int width, out int height)
        {
            IImageInfo info = Image.Identify(s_sendConfiguration, input, out _);
            if (info == null)
                throw new Exception("Not a supported image file");
            if ((long)info.Width * info.Height > 64L * 1024 * 1024)
                throw new Exception("That image is way too big");

            using (Image<Rgba32> loaded = Image.Load<Rgba32>(s_sendConfiguration, input))
            using (Image<Rgba32> image = loaded.Frames.Count > 1 ? loaded.Frames.CloneFrame(0) : loaded.Clone())
            {
                int maxDimension = MaxSendDimension;
                for (int attempt = 0; attempt < 8; attempt++)
                {
                    if (image.Width > maxDimension || image.Height > maxDimension)
                    {
                        int target = maxDimension;
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(target, target),
                            Mode = ResizeMode.Max,
                            Sampler = KnownResamplers.Bicubic,
                        }));
                    }

                    byte[] png = Save(image, false);
                    byte[] best = png;
                    if (png.Length > 256 * 1024 && IsOpaque(image))
                    {
                        byte[] jpeg = Save(image, true);
                        if (jpeg.Length < png.Length)
                            best = jpeg;
                    }
                    if (best.Length <= MaxSendBytes)
                    {
                        width = image.Width;
                        height = image.Height;
                        return best;
                    }
                    maxDimension = Math.Max(16, (int)(Math.Max(image.Width, image.Height) * 0.75f));
                }
            }
            throw new Exception("Couldn't make that image small enough to send");
        }

        private static byte[] Save(Image<Rgba32> image, bool jpeg)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (jpeg)
                    image.SaveAsJpeg(stream, new JpegEncoder { Quality = 85 });
                else
                    image.SaveAsPng(stream, new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression });
                return stream.ToArray();
            }
        }

        private static bool IsOpaque(Image<Rgba32> image)
        {
            Rgba32[] pixels = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].A != byte.MaxValue)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Decodes an image file into premultiplied colors ready for a texture.
        /// </summary>
        public static Color[] DecodePixels(byte[] data, int maxDimension, out int width, out int height)
        {
            IImageInfo info = Image.Identify(s_receiveConfiguration, data, out _);
            if (info == null)
                throw new Exception("Not a supported image file");
            if (info.Width <= 0 || info.Height <= 0 || info.Width > maxDimension || info.Height > maxDimension)
                throw new Exception("Image dimensions out of range");
            using (Image<Rgba32> image = Image.Load<Rgba32>(s_receiveConfiguration, data))
            {
                width = image.Width;
                height = image.Height;
                Rgba32[] pixels = new Rgba32[width * height];
                image.Frames.RootFrame.CopyPixelDataTo(pixels);
                Color[] colors = new Color[pixels.Length];
                for (int i = 0; i < pixels.Length; i++)
                {
                    Rgba32 p = pixels[i];
                    if (p.A == byte.MaxValue)
                        colors[i] = new Color(p.R, p.G, p.B, p.A);
                    else
                        colors[i] = new Color((byte)(p.R * p.A / 255), (byte)(p.G * p.A / 255), (byte)(p.B * p.A / 255), p.A);
                }
                return colors;
            }
        }

        private static void CreateTexture(ChatImageAttachment attachment, Color[] pixels, int width, int height)
        {
            Tex2D texture = new Tex2D(width, height);
            texture.SetData(pixels);
            attachment.texture = texture;
            attachment.sprite = new Sprite(texture);
            attachment.width = width;
            attachment.height = height;
            attachment.version++;

            if (!attachment.isLocal)
            {
                s_loaded.Add(attachment);
                while (s_loaded.Count > MaxLoadedImages)
                {
                    // Free the oldest image, it can be downloaded again by clicking it
                    ChatImageAttachment oldest = s_loaded[0];
                    s_loaded.RemoveAt(0);
                    oldest.sprite = null;
                    oldest.fileData = null;
                    oldest.texture?.Dispose();
                    oldest.texture = null;
                    oldest.SetState(ChatImageState.Offered);
                }
            }
        }

        #endregion

        #region Networking

        /// <summary>
        /// Remembers an image we're about to send so it can be requested.
        /// </summary>
        public static void SendOffer(ChatImageAttachment attachment, NetworkConnection connection)
        {
            Send.Message(new NMChatImageOffer(DuckNetwork.localProfile, attachment), connection);
        }

        public static void OnOffer(NMChatImageOffer offer)
        {
            if (offer.profile == null || offer.connection == null || offer.profile.connection != offer.connection)
                return;
            if (offer.byteSize <= 0 || offer.byteSize > MaxReceiveBytes || offer.width == 0 || offer.height == 0 || offer.width > MaxReceiveDimension || offer.height > MaxReceiveDimension)
                return;
            s_pendingOffers[offer.profile] = new ChatImageAttachment
            {
                id = offer.imageId,
                sender = offer.profile,
                source = offer.connection,
                width = offer.width,
                height = offer.height,
                byteSize = offer.byteSize,
                name = SanitizeFileName(offer.fileName),
                state = ChatImageState.Offered,
            };
            s_pendingOfferFrames[offer.profile] = s_frame;
        }

        /// <summary>
        /// Gets the image announced right before a chat message from this player, if any.
        /// </summary>
        public static ChatImageAttachment TakePendingOffer(Profile profile)
        {
            if (profile == null || !s_pendingOffers.TryGetValue(profile, out ChatImageAttachment attachment))
                return null;
            s_pendingOffers.Remove(profile);
            // The chat message is sent right after the offer, anything older than that belongs to a message that got lost
            if (s_pendingOfferFrames.TryGetValue(profile, out int frame) && s_frame - frame > OfferLifetimeFrames)
                return null;
            return attachment;
        }

        public static void ClearPendingOffer(Profile profile)
        {
            if (profile != null)
                s_pendingOffers.Remove(profile);
        }

        /// <summary>
        /// Called once a received image is attached to its chat message.
        /// </summary>
        public static void OnAttached(ChatImageAttachment attachment)
        {
            if (attachment == null || attachment.isLocal)
                return;
            if (!AllowedFrom(attachment.sender, attachment.source))
                attachment.SetState(ChatImageState.Hidden);
            else if (mode == ModeAuto)
                Request(attachment);
        }

        public static void Request(ChatImageAttachment attachment)
        {
            if (attachment == null || attachment.isLocal)
                return;
            if (attachment.state != ChatImageState.Offered && attachment.state != ChatImageState.Failed)
                return;
            if (attachment.source == null || attachment.source.status != ConnectionStatus.Connected || !Network.isActive)
            {
                attachment.SetState(ChatImageState.Failed);
                return;
            }
            attachment.buffer = null;
            attachment.received = null;
            attachment.receivedCount = 0;
            attachment.chunkCount = 0;
            attachment.progress = 0f;
            attachment.framesSinceData = 0;
            attachment.SetState(ChatImageState.Downloading);
            s_downloads[attachment.id] = attachment;
            Send.Message(new NMChatImageRequest(attachment.id), attachment.source);
        }

        public static void OnRequest(NetworkConnection connection, uint id)
        {
            if (connection == null)
                return;
            if (!s_outgoing.TryGetValue(id, out byte[] data))
            {
                Send.Message(new NMChatImageChunk(id, 0, 0, 0, new BitBuffer()), connection);
                return;
            }
            Upload existing = s_uploads.FirstOrDefault(x => x.connection == connection && x.id == id);
            if (existing != null)
            {
                // They gave up and asked again, start over (chunks still in flight from before are ignored by the window)
                existing.nextChunk = 0;
                existing.inFlight = 0;
                existing.generation++;
                return;
            }
            if (s_uploads.Count(x => x.connection == connection) >= MaxUploadsPerConnection)
            {
                Send.Message(new NMChatImageChunk(id, 0, 0, 0, new BitBuffer()), connection);
                return;
            }
            s_uploads.Add(new Upload { connection = connection, id = id, data = data });
        }

        public static void OnChunk(NetworkConnection connection, NMChatImageChunk chunk)
        {
            if (!s_downloads.TryGetValue(chunk.imageId, out ChatImageAttachment attachment) || attachment.source != connection)
                return;
            if (chunk.chunkCount == 0)
            {
                s_downloads.Remove(chunk.imageId);
                attachment.SetState(ChatImageState.Failed);
                return;
            }

            int expectedChunks = (chunk.totalBytes + ChunkSize - 1) / ChunkSize;
            if (chunk.totalBytes <= 0 || chunk.totalBytes > MaxReceiveBytes || chunk.totalBytes != attachment.byteSize || chunk.chunkCount != expectedChunks || chunk.chunkIndex >= chunk.chunkCount || chunk.data == null)
            {
                FailDownload(attachment);
                return;
            }
            if (attachment.buffer == null)
            {
                attachment.buffer = new byte[chunk.totalBytes];
                attachment.received = new bool[chunk.chunkCount];
                attachment.chunkCount = chunk.chunkCount;
            }
            else if (attachment.buffer.Length != chunk.totalBytes || attachment.chunkCount != chunk.chunkCount)
            {
                FailDownload(attachment);
                return;
            }

            int offset = chunk.chunkIndex * ChunkSize;
            int length = Math.Min(chunk.data.lengthInBytes, chunk.totalBytes - offset);
            if (length <= 0 || length > ChunkSize)
            {
                FailDownload(attachment);
                return;
            }
            Array.Copy(chunk.data.buffer, 0, attachment.buffer, offset, length);
            if (!attachment.received[chunk.chunkIndex])
            {
                attachment.received[chunk.chunkIndex] = true;
                attachment.receivedCount++;
            }
            attachment.framesSinceData = 0;
            attachment.progress = attachment.receivedCount / (float)attachment.chunkCount;

            if (attachment.receivedCount == attachment.chunkCount)
            {
                s_downloads.Remove(attachment.id);
                byte[] data = attachment.buffer;
                attachment.buffer = null;
                attachment.received = null;
                attachment.SetState(ChatImageState.Decoding);
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        Color[] pixels = DecodePixels(data, MaxReceiveDimension, out int width, out int height);
                        s_mainThreadQueue.Enqueue(() =>
                        {
                            attachment.fileData = data;
                            CreateTexture(attachment, pixels, width, height);
                            attachment.SetState(ChatImageState.Ready);
                        });
                    }
                    catch (Exception e)
                    {
                        s_mainThreadQueue.Enqueue(() =>
                        {
                            DevConsole.Log(DCSection.DuckNet, "|DGRED|Couldn't decode a chat image: " + e.Message);
                            attachment.SetState(ChatImageState.Failed);
                        });
                    }
                });
            }
        }

        private static void FailDownload(ChatImageAttachment attachment)
        {
            s_downloads.Remove(attachment.id);
            attachment.buffer = null;
            attachment.received = null;
            attachment.SetState(ChatImageState.Failed);
        }

        #endregion

        public static void Update()
        {
            s_frame++;
            while (s_mainThreadQueue.TryDequeue(out Action action))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    DevConsole.Log(e);
                }
            }

            if (s_attachmentErrorTimer > 0 && --s_attachmentErrorTimer == 0)
                attachmentError = null;

            bool networkActive = Network.isActive;
            if (!networkActive)
            {
                if (s_wasNetworkActive)
                    Reset();
                s_wasNetworkActive = false;
                return;
            }
            s_wasNetworkActive = true;

            // Uploads, round robin so one big image doesn't block everyone else. Each upload only has a few
            // unacknowledged chunks out at a time, so a slow connection can't build up a huge backlog.
            int budget = ChunksPerFrame;
            int idle = 0;
            while (budget > 0 && s_uploads.Count > 0 && idle < s_uploads.Count)
            {
                s_uploadCursor %= s_uploads.Count;
                Upload upload = s_uploads[s_uploadCursor];
                if (upload.connection == null || upload.connection.status != ConnectionStatus.Connected)
                {
                    s_uploads.RemoveAt(s_uploadCursor);
                    continue;
                }
                int chunkCount = (upload.data.Length + ChunkSize - 1) / ChunkSize;
                if (upload.nextChunk >= chunkCount)
                {
                    if (upload.inFlight <= 0)
                        s_uploads.RemoveAt(s_uploadCursor);
                    else
                        s_uploadCursor++;
                    idle++;
                    continue;
                }
                if (upload.inFlight >= ChunkWindow)
                {
                    s_uploadCursor++;
                    idle++;
                    continue;
                }
                idle = 0;
                int offset = upload.nextChunk * ChunkSize;
                int length = Math.Min(ChunkSize, upload.data.Length - offset);
                BitBuffer chunkData = new BitBuffer();
                chunkData.Write(upload.data, offset, length);
                NMChatImageChunk message = new NMChatImageChunk(upload.id, (ushort)upload.nextChunk, (ushort)chunkCount, upload.data.Length, chunkData);
                int generation = upload.generation;
                message.onAcknowledged = () =>
                {
                    if (upload.generation == generation && upload.inFlight > 0)
                        upload.inFlight--;
                };
                Send.Message(message, upload.connection);
                upload.nextChunk++;
                upload.inFlight++;
                budget--;
                s_uploadCursor++;
            }

            // Download timeouts
            if (s_downloads.Count > 0)
            {
                foreach (ChatImageAttachment attachment in s_downloads.Values.ToList())
                {
                    if (++attachment.framesSinceData > DownloadTimeoutFrames || attachment.source == null || attachment.source.status != ConnectionStatus.Connected)
                        FailDownload(attachment);
                }
            }
        }

        /// <summary>
        /// Frees the pictures of images shown in the chat, called when the chat gets cleared.
        /// Unlike <see cref="Reset"/> the network side stays, so others can still download images we sent.
        /// </summary>
        public static void ForgetShownImages()
        {
            foreach (ChatImageAttachment attachment in s_loaded)
            {
                attachment.sprite = null;
                attachment.fileData = null;
                attachment.texture?.Dispose();
                attachment.texture = null;
                attachment.SetState(ChatImageState.Offered);
            }
            s_loaded.Clear();
            foreach (ChatImageAttachment attachment in s_localImages)
            {
                attachment.sprite = null;
                attachment.texture?.Dispose();
                attachment.texture = null;
            }
            s_localImages.Clear();
        }

        /// <summary>
        /// Forgets everything network related, called when leaving a lobby.
        /// </summary>
        public static void Reset()
        {
            s_uploads.Clear();
            s_downloads.Clear();
            s_pendingOffers.Clear();
            s_pendingOfferFrames.Clear();
            s_outgoing.Clear();
            s_outgoingOrder.Clear();
            foreach (ChatImageAttachment attachment in s_loaded)
            {
                attachment.sprite = null;
                attachment.texture?.Dispose();
                attachment.texture = null;
            }
            s_loaded.Clear();
            foreach (ChatImageAttachment attachment in s_localImages)
            {
                attachment.sprite = null;
                attachment.texture?.Dispose();
                attachment.texture = null;
                attachment.SetState(ChatImageState.Failed);
            }
            s_localImages.Clear();
        }

        #region Drawing

        public static string GetLabel(ChatImageAttachment attachment)
        {
            switch (attachment.state)
            {
                case ChatImageState.Offered:
                    return "SHOW IMAGE (" + FormatSize(attachment.byteSize) + ")";
                case ChatImageState.Downloading:
                    return "LOADING IMAGE " + (int)(attachment.progress * 100f) + "%";
                case ChatImageState.Decoding:
                    return "LOADING IMAGE";
                case ChatImageState.Failed:
                    return attachment.isLocal ? "IMAGE UNAVAILABLE" : "IMAGE UNAVAILABLE (RETRY)";
                case ChatImageState.Hidden:
                    return "IMAGE HIDDEN";
                default:
                    return "IMAGE";
            }
        }

        /// <summary>Widest label a state can have, so the layout doesn't jitter while loading.</summary>
        public static string GetLabelTemplate(ChatImageAttachment attachment)
        {
            return attachment.state == ChatImageState.Downloading ? "LOADING IMAGE 100%" : GetLabel(attachment);
        }

        public static string FormatSize(int bytes)
        {
            if (bytes >= 1024 * 1024)
                return (bytes / (1024f * 1024f)).ToString("0.0") + " MB";
            return Math.Max(1, bytes / 1024) + " KB";
        }

        /// <summary>
        /// Settings can change after an image arrived: hidden images become loadable once allowed, and offered images
        /// get hidden when images or mojis get turned off.
        /// </summary>
        public static void Refresh(ChatImageAttachment attachment)
        {
            if (attachment == null || attachment.isLocal)
                return;
            bool allowed = AllowedFrom(attachment.sender, attachment.source);
            if (attachment.state == ChatImageState.Hidden && allowed)
            {
                attachment.SetState(ChatImageState.Offered);
                if (mode == ModeAuto)
                    Request(attachment);
            }
            else if (!allowed && (attachment.state == ChatImageState.Offered || attachment.state == ChatImageState.Failed))
                attachment.SetState(ChatImageState.Hidden);
        }

        /// <summary>
        /// What hovering an image in chat explains, mostly which setting is keeping it from showing up.
        /// </summary>
        public static string GetTooltip(ChatImageAttachment attachment)
        {
            switch (attachment.state)
            {
                case ChatImageState.Ready:
                    return "Click to view";
                case ChatImageState.Offered:
                    return "Click to view image (" + FormatSize(attachment.byteSize) + ")";
                case ChatImageState.Downloading:
                case ChatImageState.Decoding:
                    return "Loading image...";
                case ChatImageState.Failed:
                    return attachment.isLocal ? "This image isn't available anymore" : "Couldn't load the image, click to retry";
                case ChatImageState.Hidden:
                    if (mode == ModeOff)
                        return "Turn on Chat Images (Options > Rebuilt > Chat) to see images";
                    if (Options.Data.mojiFilter == 0)
                        return "Enable custom MOJIs (Options > Usability) to receive images";
                    if (Options.Data.mojiFilter == 1)
                        return "Only friends' images are shown, set custom MOJIs to ENABLED to see everyone's";
                    return "Click to view image";
                default:
                    return null;
            }
        }

        public static bool IsClickable(ChatImageAttachment attachment)
        {
            return attachment.state == ChatImageState.Ready && attachment.sprite != null
                || !attachment.isLocal && (attachment.state == ChatImageState.Offered || attachment.state == ChatImageState.Failed);
        }

        /// <summary>
        /// Gets the on screen size of an image glyph, either the image itself or a clickable label.
        /// </summary>
        public static Vec2 GetDisplaySize(ChatImageAttachment attachment, FancyBitmapFont font, float maxWidth, float maxHeight)
        {
            if (attachment.state == ChatImageState.Ready && attachment.sprite != null)
            {
                float scale = Math.Min(1f, Math.Min(maxWidth / attachment.width, maxHeight / attachment.height));
                return new Vec2(Math.Max(1f, attachment.width * scale), Math.Max(1f, attachment.height * scale));
            }
            float padding = 3f * font.scale.x;
            return new Vec2(font.GetWidth(GetLabelTemplate(attachment)) + padding * 2f, (font.characterHeight + 2) * font.scale.y + padding);
        }

        public static void DrawAttachment(ChatGlyph glyph, ChatTextLayout layout, Vec2 position, float alpha, float imageDepth, float textDepth, Color textColor)
        {
            ChatImageAttachment attachment = glyph.image;
            if (attachment == null)
                return;
            Refresh(attachment);
            if (attachment.state == ChatImageState.Ready && attachment.sprite != null)
            {
                Sprite sprite = attachment.sprite;
                sprite.scale = new Vec2(glyph.w / attachment.width, glyph.h / attachment.height);
                sprite.alpha = alpha;
                sprite.depth = ChatBox.D(imageDepth);
                Graphics.Draw(sprite, position.x, position.y, ChatBox.D(imageDepth));
                if (ChatBox.hoveredImage == attachment)
                    Graphics.DrawRect(position, position + new Vec2(glyph.w, glyph.h), Color.White * (0.12f * alpha), ChatBox.D(imageDepth + 0.01f));
                return;
            }

            FancyBitmapFont font = layout.font;
            float padding = 3f * font.scale.x;
            bool hovered = ChatBox.hoveredImage == attachment && IsClickable(attachment);
            Graphics.DrawRect(position, position + new Vec2(glyph.w, glyph.h), Color.Black * ((hovered ? 0.4f : 0.22f) * alpha), ChatBox.D(imageDepth));
            Color color = hovered ? Color.White : textColor;
            if (attachment.state == ChatImageState.Hidden)
                color = Color.Gray;
            font.Draw(GetLabel(attachment), position.x + padding, position.y + padding * 0.5f + font.scale.y, color * alpha, ChatBox.D(textDepth));
        }

        #endregion
    }
}
