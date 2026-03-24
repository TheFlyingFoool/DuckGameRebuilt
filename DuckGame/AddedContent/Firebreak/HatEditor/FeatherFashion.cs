using AddedContent.Firebreak;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;

namespace DuckGame
{
    public partial class FeatherFashion : Level
    {
        public WorkMode CurrentWorkMode = WorkMode.Editor;
        private static uint s_framesUntilTooltip;
        private static string s_toolTipMessage = string.Empty;
        private static bool s_iconBeingHovered;
        private static string s_iconHoveredID = string.Empty;
        private static string? s_filePath = null;
        private static bool _isFileDialogOpen = false;
        private static string _hatFileFilterExtension = "png";
        private static string _hatFileFilterDescription = "PNG files (*.png)";
        private static FileSystemWatcher s_fileWatcher = new();
        private static SDL3.SDL.SDL_DialogFileFilter[] _hatFileFilter 
            => FileDialogFna.GetDialogFileFilters(_hatFileFilterDescription, _hatFileFilterExtension);

        public static string? HatName => Path.GetFileNameWithoutExtension(FilePath);

        public static string FilePath
        {
            get => s_filePath;
            set
            {
                s_filePath = value;
                s_fileWatcher.Path = Path.GetDirectoryName(value);
                s_fileWatcher.Filter = Path.GetFileName(value);
                
                s_fileWatcher.EnableRaisingEvents = File.Exists(value);
            }
        }

        [Marker.PostInitialize]
        public static void StaticInitialize()
        {
            FFIcons.Initialize();
            
            // don't ask.
            FFEditorPane.MetapixelInfo = typeof(Team.CustomHatMetadata)
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<Team.Metapixel>() is not null)
                .Select(x =>
                {
                    Team.Metapixel attribute = x.GetCustomAttribute<Team.Metapixel>();
                    bool dgr = x.GetCustomAttribute<DGRAttribute>() is not null;
                    bool synced = x.GetCustomAttribute<VanillaSyncedAttribute>() is not null;
                    
                    return new MetapixelInfo((byte)attribute.index, attribute.name, attribute.description, x.FieldType, dgr, synced);
                })
                .ToDictionary(x => x.Index, x => x);

            s_fileWatcher.Changed += OnFileChanged;
            s_fileWatcher.Renamed += OnFileChanged;

            MonoMain.OnGameExit += crashed =>
            {
                if (!crashed)
                    return;
                if (s_filePath != null)
                {
                    s_filePath = Path.GetDirectoryName(s_filePath) + "/" + Path.GetFileNameWithoutExtension(s_filePath) + "_autosave.png";
                    GlobalActionSave();
                }
            };
        }

        private static void OnFileChanged(object source, FileSystemEventArgs args)
        {
            FilePath = args.FullPath;
            GlobalActionImport(true);
        }

        public override void Initialize()
        {
            backgroundColor = FFColors.Background;
            
            // makes things not wonky
            Add(new Block(0, 0, 0, 0));
            Add(new Block(320, 180, 0, 0));
            
            Music.Play("othello7_fancyfeathers", true);

            base.Initialize();
        }

        public override void Terminate()
        {
            FFPreviewPane.DisposeCurrentAnimation();
            Options.ReloadHats();
            
            Music.Stop();

            if (current is TitleScreen)
                Music.Play("Title");
            
            base.Terminate();
        }

        public override void Update()
        {
            UpdateCursorTooltip();
            UpdateModeSelector();
            
            switch (CurrentWorkMode)
            {
                case WorkMode.Editor:
                    FFEditorPane.Update();
                    break;

                case WorkMode.Preview:
                    FFPreviewPane.Update();
                    break;

                default: throw new InvalidOperationException();
            }
            
            base.Update();
        }

        public override void Draw()
        {
            DrawCursor(Mouse.positionScreen);
            DrawModeSelector();
            Graphics.Draw(FFIcons.FFLogo_Beta, 2, 2, 0.5f);
            
            switch (CurrentWorkMode)
            {
                case WorkMode.Editor:
                    FFEditorPane.Draw();
                    break;

                case WorkMode.Preview:
                    FFPreviewPane.Draw();
                    break;

                default: throw new InvalidOperationException();
            }
            
            base.Draw();
        }

        public static void LoadHat(Team team)
        {
            if (team != null && team.hat != null && team.hat.texture != null) LoadHat(team.hat.texture);
        }
        public static void LoadHat(Tex2D hatTexture)
        {
            if (hatTexture == null) return;
            int w = hatTexture.w;
            int h = hatTexture.h;

            if (w < 64 || h < 32)
                throw new Exception("hat.w < 64 || hat.h < 32");

            bool hasCape = w >= 96;
            bool hasRock = h >= 56;
            bool hasParticles = hasRock;
            bool hasMetapixels = w > 96;

            Color[,] hatData2D = hatTexture.GetData2D();
            FFEditorPane.ClearBuffers();
            
            FFEditorPane.HatAnimationBuffer[0] = GetDataInRegion(hatData2D, new Rectangle(0, 0, 32, 32));
            FFEditorPane.HatAnimationBuffer[1] = GetDataInRegion(hatData2D, new Rectangle(32, 0, 32, 32));

            if (hasCape)
            {
                FFEditorPane.CapeFrameBuffer = GetDataInRegion(hatData2D, new Rectangle(64, 0, 32, 32));
            }

            if (hasRock)
            {
                FFEditorPane.RockFrameBuffer = GetDataInRegion(hatData2D, new Rectangle(0, 32, 24, 24));
            }
            
            if (hasParticles)
            {
                FFEditorPane.ParticleAnimationBuffer[0] = GetDataInRegion(hatData2D, new Rectangle(24, 32, 12, 12));
                FFEditorPane.ParticleAnimationBuffer[1] = GetDataInRegion(hatData2D, new Rectangle(36, 32, 12, 12));
                FFEditorPane.ParticleAnimationBuffer[2] = GetDataInRegion(hatData2D, new Rectangle(24, 44, 12, 12));
                FFEditorPane.ParticleAnimationBuffer[3] = GetDataInRegion(hatData2D, new Rectangle(36, 44, 12, 12));
            }
            
            if (hasMetapixels)
            {
                Color[] metapixelData = GetDataInRegion(hatData2D, new Rectangle(96, 0, w - 96, hasRock ? 56 : 32));
                FFEditorPane.Metapixels.AddRange(metapixelData.Where(x => x != default && FFEditorPane.MetapixelInfo.ContainsKey(x.r)));
            }
        }

        private static Color[] GetDataInRegion(Color[,] fullData2D, Rectangle region)
        {
            int rx = (int) region.x;
            int ry = (int) region.y;
            int rw = (int) region.width;
            int rh = (int) region.height;

            int w = fullData2D.GetLength(0);
            int h = fullData2D.GetLength(1);

            Color[] regionData = new Color[rw * rh];

            for (int y = ry, i = 0; y < ry + rh; y++)
            {
                for (int x = rx; x < rx + rw; x++, i++)
                {
                    regionData[i] = fullData2D[x, y];
                }
            }

            return regionData;
        }
        
        public static void RegisterButtonHover(string ID, string tooltip)
        {
            s_iconHoveredID = ID;
            s_iconBeingHovered = true;
            s_toolTipMessage = tooltip;
        }

        private void UpdateModeSelector()
        {
            if (Keyboard.Pressed(Keys.Tab))
            {
                SwitchWorkMode();
                return;
            }
            
            (SpriteMap, WorkMode)[] modeSelectorIcons =
            {
                (FFIcons.GlobalActionSwitchEditor, WorkMode.Editor),
                (FFIcons.GlobalActionSwitchPreview, WorkMode.Preview),
            };

            (SpriteMap, Action<bool>)[] menuActionIcons =
            {
                (FFIcons.GlobalActionImport, GlobalActionImport),
                (FFIcons.GlobalActionSave, GlobalActionSave),
                (FFIcons.GlobalActionLeave, GlobalActionLeave),
            };

            for (int i = 0; i < modeSelectorIcons.Length; i++)
            {
                (SpriteMap icon, WorkMode mode) = modeSelectorIcons[i];
                
                if (CurrentWorkMode != mode && icon.frame == 1 && Mouse.left == InputState.Pressed)
                {
                    SwitchWorkMode();
                    break;
                }
            }

            for (int i = 0; i < menuActionIcons.Length; i++)
            {
                (SpriteMap icon, Action<bool> action) = menuActionIcons[i];

                if (icon.frame == 1 && (Mouse.left == InputState.Pressed || Mouse.right == InputState.Pressed))
                {
                    action(Mouse.right == InputState.Pressed);
                    break;
                }
            }
        }

        private void SwitchWorkMode()
        {
            switch (CurrentWorkMode)
            {
                case WorkMode.Editor:
                    FFEditorPane.OnSwitchOutOf();
                    break;

                case WorkMode.Preview:
                    FFPreviewPane.OnSwitchOutOf();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            CurrentWorkMode += 1;
            CurrentWorkMode = (WorkMode) ((int) CurrentWorkMode % Enum.GetNames(typeof(WorkMode)).Length);
            switch (CurrentWorkMode)
            {
                case WorkMode.Editor:
                    FFEditorPane.OnSwitch();
                    break;

                case WorkMode.Preview:
                    FFPreviewPane.OnSwitch();
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void DrawModeSelector()
        {
            (SpriteMap, WorkMode)[] modeSelectorIcons =
            {
                (FFIcons.GlobalActionSwitchEditor, WorkMode.Editor),
                (FFIcons.GlobalActionSwitchPreview, WorkMode.Preview),
            };

            SpriteMap[] menuActionIcons =
            {
                FFIcons.GlobalActionImport,
                FFIcons.GlobalActionSave,
                FFIcons.GlobalActionLeave,
            };

            for (int i = 0; i < modeSelectorIcons.Length; i++)
            {
                (SpriteMap icon, WorkMode mode) = modeSelectorIcons[i];
                
                Rectangle selectionBounds = new(camera.width - 50, 4 + (i * 12), 48, 11);

                if (CurrentWorkMode != mode)
                {
                    if (selectionBounds.Contains(Mouse.positionScreen))
                        icon.frame = 1;
                    else icon.frame = 0;
                }
                else icon.frame = 1;
                
                Graphics.Draw(icon, selectionBounds.x, selectionBounds.y, 1.3f);
            }

            float gapLineY = modeSelectorIcons.Length * 12 + 6f;
            Graphics.DrawLine(new Vec2(camera.width - 50, gapLineY), new Vec2(camera.width - 2, gapLineY), FFColors.PrimaryDim, 1f, 1.4f);
            
            for (int i = 0; i < menuActionIcons.Length; i++)
            {
                SpriteMap icon = menuActionIcons[i];
                
                Rectangle selectionBounds = new(camera.width - 50, gapLineY + 3 + (i * 12), 48, 11);

                if (selectionBounds.Contains(Mouse.positionScreen))
                    icon.frame = 1;
                else icon.frame = 0;
                
                Graphics.Draw(icon, selectionBounds.x, selectionBounds.y, 1.3f);
            }
        }

        private static void GlobalActionImport(bool rightClick)
        {
            if (rightClick && FilePath is not null)
            {
                LoadHatFile(FilePath);
                return;
            }
            //RunInMainThread(OpenHatFileDialog);
            OpenHatFileDialog();
        }

        private static void LoadHatFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }
            Texture2D texture = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, filePath, true);
            if (texture is null)
                return;

            if (texture.Width > 100 || texture.Height > 56)
                throw new Exception("Image file too big to be a vanilla hat");

            LoadHat(texture);
        }

        private static void OpenHatFileDialog()
        {
            if (_isFileDialogOpen) {
                return;
            }
            _isFileDialogOpen = true;
            try
            {
                var openDialog = new FileDialogFna();
                openDialog.ShowOpenFileDialog(OpenFileCallback, window: MonoMain.instance.Window.Handle, filters: _hatFileFilter);
            }
            catch (SecurityException ex)
            {
                MessageBoxFna.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                   $"Details:\n\n{ex.StackTrace}", icon: MessageBoxFna.Icon.Warning);
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
        }

        private static void OpenFileCallback(List<string> fileNames)
        {
            _isFileDialogOpen = false;
            var fileName = fileNames?.FirstOrDefault();
            if (string.IsNullOrEmpty(fileName)) {
                return; // Cancelled.
            }
            try
            {
                Console.WriteLine($"OpenFile from: {fileName}");
                Texture2D texture = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, fileName, true);
                if (texture.Width > 100 || texture.Height > 56) {
                    throw new Exception("Image file too big to be a vanilla hat");
                }
                LoadHat(texture);
                FilePath = fileName;
            } catch(Exception ex) {
                Console.WriteLine("Error opening file: " + ex);
                MessageBoxFna.Show($"Error opening file: {ex.Message}\n\nDetails:\n\n{ex.StackTrace}");
            }
        }

        private static void SaveFileCallback(string fileName)
        {
            _isFileDialogOpen = false;
            if (string.IsNullOrEmpty(fileName)) {
                return; // Cancelled
            }
            try
            {
                Console.WriteLine($"SaveFile to: {fileName}");
                var fileNameWithExtension = FileDialogFna.EnsureFileExtension(fileName, ".png");
                FilePath = fileNameWithExtension;
                GlobalActionSave();
            } catch (Exception ex) {
                Console.WriteLine("Error saving file: " + ex);
                MessageBoxFna.Show($"Error saving file: {ex.Message}\n\nDetails:\n\n{ex.StackTrace}");
            }
        }

        private static void GlobalActionSave(bool alwaysNew = false)
        {
            if (FilePath is not null && !alwaysNew)
            {
                SaveHatFile(FilePath);
                return;
            }
            else
            {
                //RunInMainThread(OpenSaveHatDialog);
                OpenSaveHatDialog();
            }
        }

        private static void OpenSaveHatDialog()
        {
            if (_isFileDialogOpen) {
                return;
            }
            _isFileDialogOpen = true;
            try
            {
                var saveDialog = new FileDialogFna();
                saveDialog.ShowSaveFileDialog(SaveFileCallback, null, window: MonoMain.instance.Window.Handle, _hatFileFilter);
            }
            catch (SecurityException ex)
            {
                MessageBoxFna.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                    $"Details:\n\n{ex.StackTrace}");
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
        }

        private static void SaveHatFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }
            try
            {
                using FileStream stream = File.OpenWrite(filePath);
                Tex2D fullHatTexture = FFEditorPane.FullHatTexture;
                fullHatTexture.SaveAsPng(stream, fullHatTexture.w, fullHatTexture.h);

                HUD.AddPlayerChangeDisplay($"{HatName} saved!", 1f);
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
                HUD.AddPlayerChangeDisplay("Save failed. Check console for error");
                throw;
            }
        }

        private static void GlobalActionLeave(bool _)
        {
            current = new TitleScreen();
        }

        public static void DrawCursor(Vec2 position)
        {
            float rotationValue = Maths.RadToDeg((Maths.FastSin((float) ((DateTime.Now - MonoMain.startTime).TotalSeconds % 1f * Math.PI * 2)) + 1) * 2f);

            Graphics.DrawLine(position, -0.5f, (rotationValue + 000) % 360f, FFColors.Focus, 1, 1.95f);
            Graphics.DrawLine(position, 0.5f, (rotationValue + 000) % 360f, FFColors.Focus, 1, 1.95f);
            
            Graphics.DrawLine(position, 4f, (rotationValue + 000) % 360f, FFColors.Focus, 1, 1.95f);
            Graphics.DrawLine(position, 4f, (rotationValue + 090) % 360f, FFColors.Focus, 1, 1.95f);
            Graphics.DrawLine(position, 4f, (rotationValue + 180) % 360f, FFColors.Focus, 1, 1.95f);
            Graphics.DrawLine(position, 4f, (rotationValue + 270) % 360f, FFColors.Focus, 1, 1.95f);

            if (s_framesUntilTooltip > 40)
                DrawCursorHoverTip(position, s_toolTipMessage);
        }

        private static void DrawCursorHoverTip(Vec2 position, string tip, float alpha = 1f)
        {
            const float fontSize = 0.6f;
                
            string segmentedTip = string.Join("\n", tip.SplitByLength(30));
            SizeF textSize = Extensions.GetStringSize(segmentedTip, fontSize);
            Rectangle bgBox = new(position, position + (Vec2) textSize + new Vec2(2));

            bool drawLeft = bgBox.Right > current.camera.width;
            bool drawUp = bgBox.Bottom > current.camera.height;

            if (drawLeft)
                bgBox.x -= bgBox.width;

            if (drawUp)
                bgBox.y -= bgBox.height;
                
            Graphics.DrawRect(bgBox, Color.Black * 0.6f * alpha, 1.975f);
            Graphics.DrawString(segmentedTip, bgBox.tl + Vec2.One, Color.White * alpha, 2f, null, fontSize);
        }
        
        private static void UpdateCursorTooltip()
        {
            if (s_iconBeingHovered)
            {
                s_iconBeingHovered = false;
                s_framesUntilTooltip++;
            }
            else
            {
                s_iconHoveredID = "";
                s_framesUntilTooltip = 0;
            }
        }

        private static Thread RunInMainThread(Action action)
        {
            Thread t = new(() =>
            {
                action.Invoke();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            return t;
        }
    }
}