using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public partial class FeatherFashion : Level
    {
        public WorkMode CurrentWorkMode = WorkMode.Editor;
        
        private static uint s_framesUntilTooltip;
        private static string s_toolTipMessage = string.Empty;
        private static bool s_iconBeingHovered;
        private static string s_iconHoveredID = string.Empty;
        public static string? FilePath = null;
        public static string? HatName => Path.GetFileNameWithoutExtension(FilePath);
        
        [PostInitialize]
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
                    return new MetapixelInfo((byte)attribute.index, attribute.name, attribute.description, x.FieldType);
                })
                .ToDictionary(x => x.Index, x => x);
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
                FFEditorPane.Metapixels.AddRange(metapixelData.Where(x => x != default));
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
                Texture2D texture = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, FilePath, true);

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
            //Graphics.DrawRect(new Rectangle(position - new Vec2(0.5f), position + new Vec2(0.5f)), FFColors.Focus, 1.95f);

            Graphics.DrawLine(position - new Vec2(4, 0), position + new Vec2(4, 0), FFColors.Focus, 1, 1.95f);
            Graphics.DrawLine(position - new Vec2(0, 4), position + new Vec2(0, 4), FFColors.Focus, 1, 1.95f);

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
    }
}