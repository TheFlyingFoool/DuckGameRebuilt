using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class SimRenderer : Level
    {
        private Sprite _exportIcon;
        
        private List<Tex2D> _animation;
        private int _animationIndex = 0;
        private int _timeToChangeAnimationFrame = 0;
        private string _teamName = String.Empty;
        public SimRenderer()
        {
            backgroundColor = Color.DarkSlateBlue;
            _animation = new List<Tex2D>();
        }

        public override void Initialize()
        {
            Add(new Block(0, camera.height * 0.75f, camera.width, camera.height * 0.25f));
            
            _exportIcon = new Sprite("exportIcon") {color = Color.GreenYellow};

            if (Thing._alphaTestEffect == null)
                Thing._alphaTestEffect = Content.Load<MTEffect>("Shaders/alphatest");

            Duck duck;
            HatPreviewLevel isolatedLevel = new();
            core.currentLevel = isolatedLevel;
            
            isolatedLevel.Initialize();
            isolatedLevel.DoUpdate();

            if (isolatedLevel.things.TryFirst(x => x is Duck, out Thing worldDuck))
            {
                duck = (Duck)worldDuck;
            }
            else throw new Exception("isolatedLevel contains no duck (?!)");

            isolatedLevel.backgroundColor = Color.SlateGray;
            # region AnimationActions
            DuckAI duckAi = new();
            duck.VirtualInput = duckAi;

            TeamHat hat = new(9999, 9999, Teams.all.Where(x => x.name.ToLower() == "devil").ChooseRandom(), duck.profile);
            duck._equipment.Add(hat);
            _teamName = hat.team.name;
            hat.Equip(duck);
            isolatedLevel.AddThing(hat);
            
            isolatedLevel.DoUpdate();
            
            const int imgWidth = 144 * 2;
            const int imgHeight = 96 * 2;

            List<Action?> animationActions = new()
            {
                () => duckAi.Press(Triggers.Right),
                () => duckAi.HoldDown(Triggers.Right),
                null,
                null,
                null,
                null,
                null,
                null,
                () => duckAi.Release(Triggers.Right),
                () => duckAi.Press(Triggers.Left),
                () => duckAi.HoldDown(Triggers.Left),
            };

            animationActions.AddRange(Enumerable.Repeat((Action) null, 20));
            animationActions.Add(() => duckAi.Release(Triggers.Left));
            animationActions.Add(() => duckAi.Press(Triggers.Down));
            animationActions.Add(() => duckAi.HoldDown(Triggers.Down));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 20));
            animationActions.Add(() => duckAi.Release(Triggers.Down));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 20));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 20));
            animationActions.Add(() => duckAi.Press(Triggers.Quack));
            animationActions.Add(() => duckAi.HoldDown(Triggers.Quack));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 2));
            animationActions.Add(() => duckAi.Release(Triggers.Quack));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 2));
            animationActions.Add(() => duckAi.Press(Triggers.Quack));
            animationActions.Add(() => duckAi.HoldDown(Triggers.Quack));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 16));
            animationActions.Add(() => duckAi.Release(Triggers.Quack));
            animationActions.AddRange(Enumerable.Repeat((Action) null, 50));
            #endregion

            Camera cam = new(0f, 0f, imgWidth / 4f, imgHeight / 4f);
            isolatedLevel.camera = cam;
            
            int i = 0;
            RenderTarget2D previousRenderTarget = Graphics.currentRenderTarget;
            RenderTarget2D target = new(imgWidth, imgHeight, true);
            Graphics.SetRenderTarget(target);
            bool sfxEnabled = SFX.enabled;
            SFX.enabled = false;
            foreach (Action? frameAction in animationActions)
            {
                frameAction?.Invoke();
                isolatedLevel.DoUpdate();

                cam.position = new Vec2(duck.x - (cam.width / 2f), duck.y - (cam.height / 2f));

                isolatedLevel.DoDraw();
                _animation.Add(target.ToTex2D());
            }
            SFX.enabled = sfxEnabled;
            Graphics.Clear(isolatedLevel.backgroundColor);
            if (previousRenderTarget == null || previousRenderTarget.IsDisposed)
                Graphics.SetRenderTarget(null);
            else Graphics.SetRenderTarget(previousRenderTarget);
            
            core.currentLevel.Terminate();
            core.currentLevel = this;

            base.Initialize();
        }

        public override void Update()
        {
            if (new Rectangle(_exportIcon.x, _exportIcon.y, _exportIcon.w, _exportIcon.h).Contains(Mouse.positionScreen))
            {
                _exportIcon.color = Color.White;

                if (Mouse.left == InputState.Pressed)
                    ExportGIF();
            }
            else _exportIcon.color = Color.GreenYellow;
            
            const int timeToChangeAnimationFrame = 0;
            
            if (_animation.Count > 0 && ++_timeToChangeAnimationFrame > timeToChangeAnimationFrame)
            {
                _animationIndex++;
                _animationIndex %= _animation.Count;

                if (timeToChangeAnimationFrame != 0)
                    _timeToChangeAnimationFrame %= timeToChangeAnimationFrame;
            }

            base.Update();
        }

        private void ExportGIF()
        {
            int imgWidth = _animation.First().w;
            int imgHeight = _animation.First().h;
            
            using Image<Rgba32> gif = new(imgWidth, imgHeight, SixLabors.ImageSharp.Color.SlateGray);
            
            gif.Metadata.GetGifMetadata().RepeatCount = 0;
            
            foreach (Tex2D frameTexture in _animation)
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
            
            gif.Mutate(x => x.Resize(imgWidth, imgHeight, new NearestNeighborResampler()));
            gif.SaveAsGif($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/duck.gif");
        }

        public override void Draw()
        {
            if (_animation.Count == 0)
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
            Graphics.DrawRect(new Rectangle(progressBarRect.x + (_animationIndex * progressBarRect.width * (1f / _animation.Count)), progressBarRect.y, progressBarRect.width * (1f / _animation.Count), progressBarRect.height), Color.GreenYellow, 1.9f);
            
            Graphics.DrawRect(new Rectangle(Mouse.xScreen - 0.5f, Mouse.yScreen - 0.5f, 1, 1), Color.White, 2f);
            
            base.Draw();
        }
    }
}