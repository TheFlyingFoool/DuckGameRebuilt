using AddedContent.Firebreak;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public partial class FeatherFashion : Level
    {
        private static Vec2 _previousMousePositionScreen = Mouse.positionScreen;
        private static string? s_filePath = null;
        private static FileSystemWatcher s_fileWatcher = new();
        public static string? HatName => Path.GetFileNameWithoutExtension(FilePath);
        public static EditorInput InputMode = EditorInput.Gamepad;

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

        public static FFMainWindow MainWindow;
        
        [Marker.PostInitialize]
        public static void StaticInitialize()
        {
            FFIcons.Initialize();
            MetapixelEditor.Initialize();

            s_fileWatcher.Changed += OnFileChanged;
            s_fileWatcher.Renamed += OnFileChanged;

            MainWindow = new FFMainWindow();
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
            UpdateInputMode();
            
            if (Keyboard.Pressed(Keys.F5))
                MainWindow = new FFMainWindow();
            
            MainWindow.Update();

            base.Update();
        }

        private void UpdateInputMode()
        {
            if (Input.Pressed(Triggers.Any))
            {
                InputMode = EditorInput.Gamepad;
            }
            
            if (Mouse.left is InputState.Pressed ||
                Mouse.right is InputState.Pressed ||
                Mouse.middle is InputState.Pressed ||
                Mouse.positionScreen != _previousMousePositionScreen)
            {
                InputMode = EditorInput.Mouse;
            }

            _previousMousePositionScreen = Mouse.positionScreen;
        }

        public override void Draw()
        {
            MainWindow.Draw();

            if (Keyboard.Down(Keys.F6))
            {
                foreach (FFBoundary component in MainWindow.Components)
                {
                    bool important = component.Bounds.Contains(Mouse.positionScreen);
                    
                    string rectReadable = $"{component.Bounds.x:0},{component.Bounds.y:0} {component.Bounds.width:0}:{component.Bounds.height:0}";
                    if (important)
                        Graphics.DrawStringOutline(rectReadable, component.Bounds.tl + (0, -8), Color.Cyan, Color.Black, 3f, scale: 0.5f);
                    else
                        Graphics.DrawString(rectReadable, component.Bounds.tl + (0, -8), Color.Red, 2.4f, scale: 0.5f);
                        
                    Graphics.DrawDottedLine((component.Bounds.Center.x, component.Bounds.Top), (component.Bounds.Center.x, component.Bounds.Bottom), (important ? Color.Cyan : Color.Red), 0.5f, 1f, 2f);
                    Graphics.DrawDottedLine((component.Bounds.x, component.Bounds.Center.y), (component.Bounds.Right, component.Bounds.Center.y), (important ? Color.Cyan : Color.Red), 0.5f, 1f, 2f);
                }
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
            ClearBuffers();
            
            HatAnimationBuffer[0] = GetDataInRegion(hatData2D, new Rectangle(0, 0, 32, 32));
            HatAnimationBuffer[1] = GetDataInRegion(hatData2D, new Rectangle(32, 0, 32, 32));

            if (hasCape)
            {
                CapeFrameBuffer = GetDataInRegion(hatData2D, new Rectangle(64, 0, 32, 32));
            }

            if (hasRock)
            {
                RockFrameBuffer = GetDataInRegion(hatData2D, new Rectangle(0, 32, 24, 24));
            }
            
            if (hasParticles)
            {
                ParticleAnimationBuffer[0] = GetDataInRegion(hatData2D, new Rectangle(24, 32, 12, 12));
                ParticleAnimationBuffer[1] = GetDataInRegion(hatData2D, new Rectangle(36, 32, 12, 12));
                ParticleAnimationBuffer[2] = GetDataInRegion(hatData2D, new Rectangle(24, 44, 12, 12));
                ParticleAnimationBuffer[3] = GetDataInRegion(hatData2D, new Rectangle(36, 44, 12, 12));
            }
            
            if (hasMetapixels)
            {
                Color[] metapixelData = GetDataInRegion(hatData2D, new Rectangle(96, 0, w - 96, hasRock ? 56 : 32));
                Metapixels.AddRange(metapixelData.Where(x => x != default && MetapixelEditor.MetapixelInfoMap.ContainsKey(x.r)));
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
        
        public static void ClearBuffers()
        {
            Color[][] buffersToClear =
            {
                HatAnimationBuffer[0],
                HatAnimationBuffer[1],
                CapeFrameBuffer,
                RockFrameBuffer,
                ParticleAnimationBuffer[0],
                ParticleAnimationBuffer[1],
                ParticleAnimationBuffer[2],
                ParticleAnimationBuffer[3],
            };

            foreach (Color[] buffer in buffersToClear)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = default;
                }
            }
                
            DefaultRockBuffer.CopyTo(RockFrameBuffer, 0);
            Metapixels.Clear();
        }

        private static void GlobalActionImport(bool rightClick)
        {
            if (rightClick && FilePath is not null)
            {
                Texture2D texture = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, FilePath, true);
                
                if (texture is null)
                    return;

                if (texture.Width > 100 || texture.Height > 56)
                    throw new Exception("Image file too big to be a vanilla hat");

                LoadHat(texture);
                return;
            }
            
            Thread t = new(() =>
            {
                try
                {
                    OpenFileDialog dialog = new() {Filter = "PNG files (*.png)|*.png"};
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Texture2D texture = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, dialog.FileName, true);

                        if (texture.Width > 100 || texture.Height > 56)
                            throw new Exception("Image file too big to be a vanilla hat");

                        LoadHat(texture);
                        FilePath = dialog.FileName;
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                    $"Details:\n\n{ex.StackTrace}");
                }
                catch (Exception e)
                {
                    DevConsole.Log(e);
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private static void GlobalActionSave(bool alwaysNew = false)
        {
            if (FilePath is not null && !alwaysNew)
            {
                try
                {
                    using FileStream stream = File.OpenWrite(FilePath);
                    Tex2D fullHatTexture = GetFullHatTexture();
                    fullHatTexture.SaveAsPng(stream, fullHatTexture.w, fullHatTexture.h);
                    
                    HUD.AddPlayerChangeDisplay($"{HatName} saved!", 1f);
                }
                catch (Exception e)
                {
                    DevConsole.Log(e);
                    HUD.AddPlayerChangeDisplay("Save failed. Check console for error");
                    throw;
                }
                return;
            }
            else
            {
                Thread t = new(() =>
                {
                    try
                    {
                        SaveFileDialog dialog = new()
                        {
                            AddExtension = true,
                            DefaultExt = "png",
                            Filter = "PNG files (*.png)|*.png"
                        };
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            FilePath = dialog.FileName;
                            GlobalActionSave();
                        }
                    }
                    catch (SecurityException ex)
                    {
                        MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                                        $"Details:\n\n{ex.StackTrace}");
                    }
                    catch (Exception e)
                    {
                        DevConsole.Log(e);
                    }
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
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
    }
}