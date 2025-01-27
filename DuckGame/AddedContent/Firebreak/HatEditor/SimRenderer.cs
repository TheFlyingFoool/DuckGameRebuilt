using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.Design;

namespace DuckGame
{
    public class SimRenderer : Level
    {
        private Sprite _exportIcon;
        
        private Tex2D[] _animation;
        private int _animationIndex = 0;
        private int _timeToChangeAnimationFrame = 0;
        private string _teamName = String.Empty;
        public SimRenderer()
        {
            backgroundColor = Color.DarkSlateBlue;
        }

        public override void Initialize()
        {
            Add(new Block(0, camera.height * 0.75f, camera.width, camera.height * 0.25f));
            
            _exportIcon = new Sprite("exportIcon") {color = Color.GreenYellow};

            Dictionary<uint, Action<Duck>> timelineActions = new()
            {
                {30, duck =>
                    {
                        duck.AiInput.HoldDown(Triggers.Right);
                    }
                },
                {60, duck =>
                    {
                        duck.AiInput.Release(Triggers.Right);
                        duck.AiInput.HoldDown(Triggers.Left);
                    }
                },
                {61, duck =>
                    {
                        duck.AiInput.Press(Triggers.Shoot);
                        duck.AiInput.Press(Triggers.Jump);
                    }
                },
                {62, duck =>
                    {
                        duck.AiInput.Release(Triggers.Shoot);
                        duck.AiInput.HoldDown(Triggers.Jump);
                    }
                },
                {70, duck =>
                    {
                        duck.AiInput.Press(Triggers.Shoot);
                    }
                },
                {71, duck =>
                    {
                        duck.AiInput.Release(Triggers.Shoot);
                    }
                },
                {95, duck =>
                    {
                        duck.AiInput.Release(Triggers.Jump);
                    }
                },
            };

            Action<Duck> initialize = duck =>
            {
                duck.AiInput = new DuckAI();

                VirtualShotgun shotty = new(duck.x, duck.y);
                duck.GiveHoldable(shotty);
                Add(shotty);
                
                TeamHat hat = new(9999, 9999, Teams.all.ChooseRandom(), duck.profile);
                duck._equipment.Add(hat);
                _teamName = hat.team.name;
                hat.Equip(duck);
                Add(hat);
            };
            
            _animation = RenderLevelAnimation(new FeatherFashion.AnimationData(120, initialize, timelineActions), 320, 320, 1.5f);

            base.Initialize();
        }

        public static Tex2D[] RenderLevelAnimation(FeatherFashion.AnimationData animationData, int renderWidth = 144, int renderHeight = 96, float renderResolutionScale = 2f, Vec2 cameraOffset = default)
        {
            int framesToRender = animationData.Length;
            Action<Duck> initialize = animationData.Initialize;
            Dictionary<uint, Action<Duck>> timelineActions = animationData.Timeline;
            
            Tex2D[] animationFrames = new Tex2D[framesToRender];
            
            if (Thing._alphaTestEffect == null)
                Thing._alphaTestEffect = Content.Load<MTEffect>("Shaders/alphatest");

            HatPreviewLevel isolatedLevel = new();
            Level trueCurrentLevel = current;
            core.currentLevel = isolatedLevel;
            
            isolatedLevel.Initialize();
            isolatedLevel.DoUpdate();

            Duck levelDuck = (Duck) isolatedLevel.things[typeof(Duck)].First();
            initialize?.Invoke(levelDuck);

            isolatedLevel.DoUpdate();

            renderWidth = (int) (renderWidth * renderResolutionScale);
            renderHeight = (int) (renderHeight * renderResolutionScale);

            float camWidth = renderWidth / (2f * renderResolutionScale);
            float camHeight = renderHeight / (2f * renderResolutionScale);

            Camera cam = new((camWidth / -2) + cameraOffset.x, (camHeight / -2) + cameraOffset.y, camWidth, camHeight);
            isolatedLevel.camera = cam;

            RenderTarget2D previousRenderTarget = Graphics.currentRenderTarget;
            RenderTarget2D target = new(renderWidth, renderHeight, true);
            Graphics.SetRenderTarget(target);

            bool sfxEnabled = SFX.enabled;
            SFX.enabled = false;

            for (uint currentFrame = 0; currentFrame < framesToRender; currentFrame++)
            {
                if (timelineActions.TryGetValue(currentFrame, out Action<Duck> frameAction))
                    frameAction(levelDuck);

                isolatedLevel.DoUpdate();
                isolatedLevel.DoDraw();
                animationFrames[currentFrame] = target.ToTex2D();
                
                Graphics.frameFlipFlop ^= true;
            }

            SFX.enabled = sfxEnabled;

            Graphics.Clear(isolatedLevel.backgroundColor);
            if (previousRenderTarget == null || previousRenderTarget.IsDisposed)
                Graphics.SetRenderTarget(null);
            else Graphics.SetRenderTarget(previousRenderTarget);

            core.currentLevel.Terminate();
            core.currentLevel = trueCurrentLevel;

            return animationFrames;
        }
        public static Tex2D RenderRecorderator(int frames, Level isolatedLevel, int renderWidth = 144, int renderHeight = 96)
        {
            Level trueCurrentLevel = current;
            core.currentLevel = isolatedLevel;

            isolatedLevel.DoInitialize();
            isolatedLevel.DoUpdate();

            isolatedLevel.DoUpdate();

            isolatedLevel.CalculateBounds();

            float width = Math.Abs(isolatedLevel.topLeft.x - isolatedLevel.bottomRight.x);
            float height = width / 1.7777777777777777777777777777778f;
            Camera cam = new Camera(isolatedLevel.topLeft.x, isolatedLevel.topLeft.y, width, height);
            cam.width = width;
            cam.height = height;
            cam.skipUpdate = true;
            isolatedLevel.camera = cam;

            RenderTarget2D previousRenderTarget = Graphics.currentRenderTarget;
            

            bool sfxEnabled = SFX.enabled;
            SFX.enabled = false;

            Layer.Console.visible = false;
            Layer.HUD.visible = false;

            RenderTarget2D target = new RenderTarget2D(renderWidth, renderHeight, true); 
            for (int i = 0; i < frames; i++)
            {
                if (i == frames - 1)
                {
                    Graphics.SetRenderTarget(target);
                }
                isolatedLevel.DoUpdate();
                isolatedLevel.DoDraw();
            }
            Layer.HUD.visible = true;
            Layer.Console.visible = true;

            SFX.enabled = sfxEnabled;

            if (previousRenderTarget == null || previousRenderTarget.IsDisposed)
                Graphics.SetRenderTarget(null);
            else Graphics.SetRenderTarget(previousRenderTarget);

            core.currentLevel.Terminate();
            core.currentLevel = trueCurrentLevel;

            return target;
        }

        public override void Update()
        {
            if (new Rectangle(_exportIcon.x, _exportIcon.y, _exportIcon.w, _exportIcon.h).Contains(Mouse.positionScreen))
            {
                _exportIcon.color = Color.White;

                if (Mouse.left == InputState.Pressed)
                    ExportGIF(_animation, $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/duck.gif");
            }
            else _exportIcon.color = Color.GreenYellow;
            
            const int timeToChangeAnimationFrame = 0;
            
            if (_animation.Length > 0 && ++_timeToChangeAnimationFrame > timeToChangeAnimationFrame)
            {
                _animationIndex++;
                _animationIndex %= _animation.Length;

                if (timeToChangeAnimationFrame != 0)
                    _timeToChangeAnimationFrame %= timeToChangeAnimationFrame;
            }

            if (Keyboard.Pressed(Keys.NumPad3))
                current = new SimRenderer();

            base.Update();
        }

        public static void ExportGIF(Tex2D[] animation, string toPath)
        {
            int imgWidth = animation.First().w;
            int imgHeight = animation.First().h;
            
            using Image<Rgba32> gif = new(imgWidth, imgHeight, SixLabors.ImageSharp.Color.SlateGray);
            
            gif.Metadata.GetGifMetadata().RepeatCount = 0;
            
            foreach (Tex2D frameTexture in animation)
            {
                using Image<Rgba32> frame = new(frameTexture.w, frameTexture.h);
            
                Color[] pixelData = frameTexture.GetData();
                for (int y = 0; y < frameTexture.h; y++)
                {
                    for (int x = 0; x < frameTexture.w; x++)
                    {
                        int i = y * frameTexture.w + x;
                        Color pixel = pixelData[i];
                        Rgba32 imageSharpPixel = new(pixel.r, pixel.g, pixel.b, pixel.a);
            
                        frame[x, y] = imageSharpPixel;
                    }
                }

                frame.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = 2;
                gif.Frames.AddFrame(frame.Frames.RootFrame);
            }
            
            gif.Frames.RemoveFrame(0);
            
            gif.SaveAsGif(toPath);
        }

        public override void Draw()
        {
            if (_animation.Length == 0)
                return;

            Tex2D frameTexture = _animation[_animationIndex];
            
            const float scale = 0.25f;
            float frameWidth = frameTexture.w * scale;
            float frameHeight = frameTexture.h * scale;
            
            Vec2 drawPos = new(camera.centerX - frameWidth / 2f, camera.centerY - frameHeight / 2f);
            
            Graphics.Draw(frameTexture, drawPos.x, drawPos.y, scale, scale);
            Graphics.DrawRect(new Rectangle(drawPos.x - 1, drawPos.y - 1, frameWidth + 2, frameHeight + 2), Color.GreenYellow, 1.9f, false, 1f);
            Graphics.DrawFancyString($"{_animationIndex + 1}", drawPos + Vec2.One, Color.GreenYellow, 2f);
            Graphics.DrawFancyString(_teamName.ToUpper(), drawPos - new Vec2(2, 10), Color.GreenYellow, 2f);
            
            // Graphics.DrawRect(new Rectangle(drawPos.x + frameTexture.width - 10 - 1, drawPos.y + 1, 10, 10), Color.DarkSlateBlue, 1.8f);
            Graphics.Draw(_exportIcon, drawPos.x + frameWidth - 8 - 1, drawPos.y + 1, 1.9f);

            Rectangle progressBarRect = new(drawPos.x + 1, drawPos.y - 1 + (frameHeight - 3), frameWidth - 2, 3);
            Graphics.DrawRect(progressBarRect, Color.DarkSlateBlue, 1.9f);
            Graphics.DrawRect(new Rectangle(progressBarRect.x + (_animationIndex * progressBarRect.width * (1f / _animation.Length)), progressBarRect.y, progressBarRect.width * (1f / _animation.Length), progressBarRect.height), Color.GreenYellow, 1.9f);
            
            Graphics.DrawRect(new Rectangle(Mouse.xScreen - 0.5f, Mouse.yScreen - 0.5f, 1, 1), Color.White, 2f);
            
            base.Draw();
        }
    }
}