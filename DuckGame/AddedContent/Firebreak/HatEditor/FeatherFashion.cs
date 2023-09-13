using System;

namespace DuckGame
{
    public partial class FeatherFashion : Level
    {
        public WorkMode CurrentWorkMode = WorkMode.Editor;
        
        private static uint s_framesUntilTooltip;
        private static string s_toolTipMessage = string.Empty;
        private static bool s_iconBeingHovered;
        private static string s_iconHoveredID = string.Empty;
        
        [PostInitialize]
        public static void StaticInitialize()
        {
            FFIcons.Initialize();
            LoadHat(Teams.all.Find(x => x.name.ToLower() == "devil"));
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
            
            base.Terminate();
        }

        public override void Update()
        {
            UpdateCursorTooltip();
            
            if (Keyboard.Pressed(Keys.Tab))
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
                CurrentWorkMode = (WorkMode) ((int)CurrentWorkMode % Enum.GetNames(typeof(WorkMode)).Length);
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
                return;
            }
            
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
                GetDataInRegion(hatData2D, new Rectangle(96, 0, w - 96, 56)).CopyTo(FFEditorPane.Metapixels, 0);
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

        public static void DrawCursor(Vec2 position)
        {
            Graphics.DrawRect(new Rectangle(position - new Vec2(0.5f), position + new Vec2(0.5f)), FFColors.Focus, 1.95f);
                
            if (s_framesUntilTooltip > 40)
                DrawCursorHoverTip(position, s_toolTipMessage);
        }

        private static void DrawCursorHoverTip(Vec2 position, string tip, float alpha = 1f)
        {
            const float fontSize = 0.6f;
                
            string segmentedTip = string.Join("\n", tip.SplitByLength(30));
            Vec2 textSize = Extensions.GetStringSize(segmentedTip, fontSize);
            Rectangle bgBox = new(position, position + textSize + new Vec2(2));

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