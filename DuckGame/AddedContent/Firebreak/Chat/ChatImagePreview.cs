using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace DuckGame
{
    /// <summary>
    /// The full screen view you get by clicking an image in chat: zoom (left/right click, mouse wheel), drag to move,
    /// and buttons to save, copy and close. Closes with escape or the menu key too.
    /// </summary>
    public static class ChatImagePreview
    {
        private const float MinZoom = 0.5f;
        private const float MaxZoom = 12f;
        private const float ZoomStep = 1.5f;
        private const float DragThreshold = 3f;
        /// <summary>
        /// After a zoom or drag, the image glides back so it's centered, or, when it's bigger than the screen, so it at least
        /// fills the screen without an empty gap. Set to false for free movement.
        /// </summary>
        private const bool KeepCentered = true;
        private const float SettleSpeed = 0.18f;

        private enum Button
        {
            None,
            Close,
            Copy,
            Save,
        }

        private static ChatImageAttachment s_image;
        private static float s_zoom = 1f;
        private static Vec2 s_pan;
        private static bool s_mouseHeld;
        private static bool s_dragging;
        private static Vec2 s_pressPosition;
        private static Vec2 s_panAtPress;
        private static Button s_hovered;
        private static string s_feedback;
        private static bool s_feedbackError;
        private static int s_feedbackFrames;
        private static int s_closedFrame = -999;
        private static int s_frame;

        public static bool isOpen => s_image != null;

        public static bool IsShowing(ChatImageAttachment image) => image != null && s_image == image;

        public static void Open(ChatImageAttachment image)
        {
            if (image?.sprite == null)
                return;
            s_image = image;
            s_zoom = 1f;
            s_pan = Vec2.Zero;
            s_mouseHeld = false;
            s_dragging = false;
            s_feedback = null;
        }

        public static void Close()
        {
            if (s_image != null)
                s_closedFrame = s_frame;
            s_image = null;
            s_mouseHeld = false;
            s_dragging = false;
        }

        /// <summary>
        /// Lets escape close the preview instead of the chat. Also true on the frame the preview was closed by the menu key,
        /// so the same key press doesn't close the chat as well.
        /// </summary>
        public static bool HandleEscape()
        {
            if (s_image != null)
            {
                Close();
                return true;
            }
            return s_closedFrame == s_frame;
        }

        private static void ShowFeedback(string message, bool error = false)
        {
            s_feedback = message;
            s_feedbackError = error;
            s_feedbackFrames = 60 * 3;
        }

        #region Geometry

        private static Vec2 screen => new Vec2(Layer.Console.width, Layer.Console.height);

        private static float fitScale
        {
            get
            {
                float scale = Math.Min(screen.x * 0.9f / s_image.width, screen.y * 0.8f / s_image.height);
                if (scale > 1f)
                    scale = (float)Math.Floor(scale);
                return Math.Max(scale, 0.01f);
            }
        }

        private static float scale => fitScale * s_zoom;

        private static Vec2 imageSize => new Vec2(s_image.width * scale, s_image.height * scale);

        private static Vec2 imagePosition => screen / 2f + s_pan - imageSize / 2f;

        private static float buttonSize => Math.Max(12f, (float)Math.Round(Layer.Console.height / 24f));

        private static Rectangle GetButtonRect(Button button)
        {
            float size = buttonSize;
            float margin = size * 0.4f;
            int slot = button == Button.Close ? 0 : button == Button.Copy ? 1 : 2;
            return new Rectangle(screen.x - margin - size - slot * (size + margin * 0.5f), margin, size, size);
        }

        private static void ClampPan()
        {
            // Keep at least a bit of the image on screen
            Vec2 size = imageSize;
            float limitX = Math.Max(0f, size.x / 2f + screen.x / 2f - 32f);
            float limitY = Math.Max(0f, size.y / 2f + screen.y / 2f - 32f);
            s_pan = new Vec2(Maths.Clamp(s_pan.x, -limitX, limitX), Maths.Clamp(s_pan.y, -limitY, limitY));
        }

        /// <summary>Eases the image towards the nearest centered (or screen filling) position.</summary>
        private static void Settle()
        {
            Vec2 half = imageSize / 2f;
            Vec2 screenHalf = screen / 2f;
            float SettleAxis(float pan, float imageHalf, float viewHalf)
            {
                if (imageHalf <= viewHalf)
                    return 0f;
                float limit = imageHalf - viewHalf;
                return Maths.Clamp(pan, -limit, limit);
            }
            Vec2 target = new Vec2(SettleAxis(s_pan.x, half.x, screenHalf.x), SettleAxis(s_pan.y, half.y, screenHalf.y));
            Vec2 difference = target - s_pan;
            if (difference.lengthSq < 0.01f)
                s_pan = target;
            else
                s_pan += difference * SettleSpeed;
        }

        private static void ZoomAt(Vec2 point, float factor)
        {
            float newZoom = Maths.Clamp(s_zoom * factor, MinZoom, MaxZoom);
            if (newZoom == s_zoom)
                return;
            // Keep the point under the cursor in place
            Vec2 center = screen / 2f + s_pan;
            Vec2 offset = point - center;
            float ratio = newZoom / s_zoom;
            s_zoom = newZoom;
            s_pan = point - offset * ratio - screen / 2f;
            ClampPan();
        }

        #endregion

        #region Update

        /// <summary>
        /// Keyboard handling, call every frame while the chat is open.
        /// </summary>
        public static void UpdateKeys()
        {
            s_frame++;
            if (s_feedbackFrames > 0 && --s_feedbackFrames == 0)
                s_feedback = null;
            if (s_image == null)
                return;
            if (s_image.sprite == null)
            {
                Close();
                return;
            }
            if (MenuKeyPressed())
            {
                Close();
                return;
            }
            if (KeepCentered && !s_dragging)
                Settle();
        }

        private static bool MenuKeyPressed()
        {
            // The usual input checks are blocked while typing, so look at the keys bound to the menu directly
            InputProfile profile = DuckNetwork.localProfile?.inputProfile;
            MultiMap<string, int> mappings = profile?.GetMappings(typeof(Keyboard));
            if (mappings == null || !mappings.TryGetValue(Triggers.Start, out List<int> keys))
                return false;
            foreach (int key in keys)
            {
                if (key < (int)Keys.MouseKeys && Keyboard.Pressed((Keys)key))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Mouse handling while the preview is open.
        /// </summary>
        /// <returns>The cursor frame to show.</returns>
        public static int UpdateMouse(Vec2 mouse)
        {
            if (s_image == null)
                return 0;
            bool leftPressed = Mouse.left == InputState.Pressed;
            bool leftDown = Mouse.left == InputState.Down || leftPressed;
            bool rightPressed = Mouse.right == InputState.Pressed;

            s_hovered = Button.None;
            if (!s_mouseHeld)
            {
                foreach (Button button in new[] { Button.Close, Button.Copy, Button.Save })
                {
                    if (GetButtonRect(button).Contains(mouse))
                        s_hovered = button;
                }
            }

            if (s_hovered != Button.None)
            {
                if (leftPressed)
                {
                    switch (s_hovered)
                    {
                        case Button.Close:
                            Close();
                            break;
                        case Button.Copy:
                            CopyImage();
                            break;
                        case Button.Save:
                            SaveImage();
                            break;
                    }
                }
                return 1;
            }

            // Left click zooms in, dragging moves the image around
            if (leftPressed)
            {
                s_mouseHeld = true;
                s_dragging = false;
                s_pressPosition = mouse;
                s_panAtPress = s_pan;
            }
            if (s_mouseHeld)
            {
                if (leftDown)
                {
                    if (!s_dragging && (mouse - s_pressPosition).length > DragThreshold)
                        s_dragging = true;
                    if (s_dragging)
                    {
                        s_pan = s_panAtPress + (mouse - s_pressPosition);
                        ClampPan();
                    }
                }
                else
                {
                    if (!s_dragging)
                        ZoomAt(mouse, ZoomStep);
                    s_mouseHeld = false;
                    s_dragging = false;
                }
            }

            if (rightPressed && !s_mouseHeld)
                ZoomAt(mouse, 1f / ZoomStep);

            if (Mouse.isScrolling && !s_mouseHeld)
                ZoomAt(mouse, Mouse.scroll < 0f ? 1.25f : 1f / 1.25f);

            return s_dragging ? 1 : 0;
        }

        #endregion

        #region Copy / save

        private static byte[] GetFileData(out string extension)
        {
            byte[] data = s_image.fileData;
            if (data == null && s_image.texture != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    s_image.texture.SaveAsPng(stream, s_image.width, s_image.height);
                    data = stream.ToArray();
                }
            }
            extension = ChatImages.GetFileExtension(data);
            return data;
        }

        private static void CopyImage()
        {
            byte[] data = GetFileData(out _);
            if (data == null)
            {
                ShowFeedback("COULDN'T COPY THE IMAGE", true);
                return;
            }
            if (Program.isLinux || Program.IsLinuxD)
            {
                ShowFeedback("COPYING IMAGES ISN'T SUPPORTED HERE, TRY SAVING IT", true);
                return;
            }
            Thread thread = new Thread(() =>
            {
                try
                {
                    using (MemoryStream input = new MemoryStream(data))
                    using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(input))
                    using (MemoryStream png = new MemoryStream())
                    {
                        bitmap.Save(png, System.Drawing.Imaging.ImageFormat.Png);
                        System.Windows.Forms.DataObject dataObject = new System.Windows.Forms.DataObject();
                        dataObject.SetImage(bitmap);
                        // Most image editors and browsers prefer this one, it keeps transparency
                        dataObject.SetData("PNG", false, new MemoryStream(png.ToArray()));
                        System.Windows.Forms.Clipboard.SetDataObject(dataObject, true, 10, 50);
                    }
                    ChatImages.RunOnMainThread(() => ShowFeedback("COPIED TO CLIPBOARD"));
                }
                catch (Exception e)
                {
                    ChatImages.RunOnMainThread(() =>
                    {
                        DevConsole.Log(e);
                        ShowFeedback("COULDN'T COPY THE IMAGE", true);
                    });
                }
            })
            {
                IsBackground = true,
                Name = "Chat image clipboard",
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void SaveImage()
        {
            byte[] data = GetFileData(out string extension);
            if (data == null)
            {
                ShowFeedback("COULDN'T SAVE THE IMAGE", true);
                return;
            }
            // Keep the name the image was sent with (the extension can differ, images get re-encoded before sending)
            string baseName = ChatImages.SanitizeFileName(s_image.name);
            if (baseName.Length == 0)
                baseName = "duckgame-chat-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            string folder = GetDownloadsFolder();
            string fileName = baseName + "." + extension;
            if (!string.IsNullOrEmpty(folder))
            {
                for (int i = 2; File.Exists(Path.Combine(folder, fileName)) && i < 1000; i++)
                    fileName = baseName + " (" + i + ")." + extension;
            }
            string defaultLocation = string.IsNullOrEmpty(folder) ? fileName : Path.Combine(folder, fileName);
            try
            {
                FileDialogFna dialog = new FileDialogFna();
                dialog.ShowSaveFileDialog(path =>
                {
                    // May come from another thread
                    if (string.IsNullOrEmpty(path))
                        return;
                    try
                    {
                        path = FileDialogFna.EnsureFileExtension(path, "." + extension);
                        File.WriteAllBytes(path, data);
                        ChatImages.RunOnMainThread(() => ShowFeedback("SAVED " + Path.GetFileName(path)));
                    }
                    catch (Exception e)
                    {
                        ChatImages.RunOnMainThread(() =>
                        {
                            DevConsole.Log(e);
                            ShowFeedback("COULDN'T SAVE THE IMAGE", true);
                        });
                    }
                }, window: MonoMain.instance.Window.Handle,
                filters: FileDialogFna.GetDialogFileFilters(extension.ToUpperInvariant() + " image", extension),
                default_location: defaultLocation);
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
                ShowFeedback("COULDN'T OPEN THE SAVE DIALOG", true);
            }
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

        private static readonly Guid FolderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        /// <summary>The user's Downloads folder (wherever they moved it to on Windows), or Pictures / nothing as a fallback.</summary>
        private static string GetDownloadsFolder()
        {
            if (!Program.isLinux && !Program.IsLinuxD)
            {
                try
                {
                    if (SHGetKnownFolderPath(FolderDownloads, 0, IntPtr.Zero, out IntPtr pointer) == 0)
                    {
                        string path = Marshal.PtrToStringUni(pointer);
                        Marshal.FreeCoTaskMem(pointer);
                        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                            return path;
                    }
                }
                catch (Exception)
                {
                }
            }
            try
            {
                string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string downloads = Path.Combine(home, "Downloads");
                if (Directory.Exists(downloads))
                    return downloads;
                string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (Directory.Exists(pictures))
                    return pictures;
            }
            catch (Exception)
            {
            }
            return null;
        }

        #endregion

        #region Drawing

        public static void Draw(FancyBitmapFont font)
        {
            if (s_image?.sprite == null)
            {
                Close();
                return;
            }
            Vec2 screenSize = screen;
            Graphics.DrawRect(Vec2.Zero, screenSize, Color.Black * 0.8f, ChatBox.D(0.5f));

            Vec2 size = imageSize;
            Vec2 position = imagePosition;
            Sprite sprite = s_image.sprite;
            sprite.scale = new Vec2(scale, scale);
            sprite.alpha = 1f;
            Graphics.Draw(sprite, position.x, position.y, ChatBox.D(0.6f));
            Graphics.DrawRect(position + new Vec2(-1f, -1f), position + size + new Vec2(1f, 1f), Color.White * 0.4f, ChatBox.D(0.55f), false, 1f);

            float lineHeight = (font.characterHeight + 2) * font.scale.y;

            // Info and hints along the bottom, on a dark strip so they're readable over the image
            string info = (s_image.sender != null ? s_image.sender.name + "  " : "") + s_image.width + "x" + s_image.height + "  " + (int)Math.Round(scale * 100f) + "%";
            string hint = "CLICK/WHEEL ZOOM  RIGHT CLICK ZOOM OUT  DRAG MOVE";
            float bottom = screenSize.y - 4f;
            Graphics.DrawRect(new Vec2(0f, bottom - lineHeight * 2f - 4f), new Vec2(screenSize.x, screenSize.y), Color.Black * 0.6f, ChatBox.D(0.7f));
            font.Draw(info, 6f, bottom - lineHeight * 2f, Color.White * 0.9f, ChatBox.D(1f));
            font.Draw(hint, 6f, bottom - lineHeight, Color.White * 0.5f, ChatBox.D(1f));
            if (s_feedback != null)
            {
                float width = font.GetWidth(s_feedback);
                font.Draw(s_feedback, screenSize.x - width - 6f, bottom - lineHeight, s_feedbackError ? Colors.DGRed : Colors.DGGreen, ChatBox.D(1f));
            }

            // Buttons
            DrawButton(Button.Save, font);
            DrawButton(Button.Copy, font);
            DrawButton(Button.Close, font);
        }

        private static void DrawButton(Button button, FancyBitmapFont font)
        {
            Rectangle rect = GetButtonRect(button);
            bool hovered = s_hovered == button;
            Vec2 tl = new Vec2(rect.x, rect.y);
            Vec2 br = new Vec2(rect.Right, rect.Bottom);
            Graphics.DrawRect(tl, br, (hovered ? Color.White : Color.Black) * 0.75f, ChatBox.D(0.8f));
            Graphics.DrawRect(tl, br, Color.White * 0.6f, ChatBox.D(0.85f), false, 1f);
            Color color = hovered ? Color.Black : Color.White;
            float pixel = Math.Max(1f, (float)Math.Round(rect.width / 14f));
            float s = rect.width;
            Depth depth = ChatBox.D(0.95f);
            switch (button)
            {
                case Button.Close:
                {
                    float inset = s * 0.28f;
                    Graphics.DrawLine(tl + new Vec2(inset, inset), br - new Vec2(inset, inset), color, pixel * 1.5f, depth);
                    Graphics.DrawLine(new Vec2(br.x - inset, tl.y + inset), new Vec2(tl.x + inset, br.y - inset), color, pixel * 1.5f, depth);
                    break;
                }
                case Button.Copy:
                {
                    // two overlapping pages
                    float page = s * 0.42f;
                    Vec2 back = tl + new Vec2(s * 0.22f, s * 0.2f);
                    Vec2 front = tl + new Vec2(s * 0.36f, s * 0.36f);
                    Graphics.DrawRect(back, back + new Vec2(page, page), color, depth, false, pixel);
                    Graphics.DrawRect(front, front + new Vec2(page, page), hovered ? Color.White : Color.Black, ChatBox.D(0.9f));
                    Graphics.DrawRect(front, front + new Vec2(page, page), color, depth, false, pixel);
                    break;
                }
                case Button.Save:
                {
                    // arrow pointing down into a tray
                    float midX = tl.x + s / 2f;
                    Graphics.DrawLine(new Vec2(midX, tl.y + s * 0.2f), new Vec2(midX, tl.y + s * 0.62f), color, pixel, depth);
                    Graphics.DrawLine(new Vec2(midX - s * 0.18f, tl.y + s * 0.45f), new Vec2(midX, tl.y + s * 0.64f), color, pixel, depth);
                    Graphics.DrawLine(new Vec2(midX + s * 0.18f, tl.y + s * 0.45f), new Vec2(midX, tl.y + s * 0.64f), color, pixel, depth);
                    Graphics.DrawLine(new Vec2(tl.x + s * 0.22f, tl.y + s * 0.62f), new Vec2(tl.x + s * 0.22f, tl.y + s * 0.78f), color, pixel, depth);
                    Graphics.DrawLine(new Vec2(tl.x + s * 0.22f, tl.y + s * 0.78f), new Vec2(tl.x + s * 0.78f, tl.y + s * 0.78f), color, pixel, depth);
                    Graphics.DrawLine(new Vec2(tl.x + s * 0.78f, tl.y + s * 0.78f), new Vec2(tl.x + s * 0.78f, tl.y + s * 0.62f), color, pixel, depth);
                    break;
                }
            }
            if (hovered)
            {
                string label = button == Button.Close ? "CLOSE" : button == Button.Copy ? "COPY" : "SAVE";
                float width = font.GetWidth(label);
                font.Draw(label, rect.x + (rect.width - width) / 2f, rect.Bottom + 3f, Color.White, ChatBox.D(1f));
            }
        }

        #endregion
    }
}
