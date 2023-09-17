// Decompiled with JetBrains decompiler
// Type: DuckGame.Graphics
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AddedContent.Hyeve.PolyRender;

namespace DuckGame
{
    public class Graphics
    {
        public static List<GraphicsResource> objectsToDispose = new List<GraphicsResource>();
        public static bool disposingObjects = false;
        private static List<Action>[] _renderTasks = new List<Action>[2]
        {
          new List<Action>(),
          new List<Action>()
        };
        private static int _targetFlip = 0;
        private static int _currentStateIndex = 0;
        private static Vec2 _currentDrawOffset = Vec2.Zero;
        private static int _currentDrawIndex = 0;
        public static uint currentDepthSpan;
        public static int effectsLevel = 2;
        private static RenderTarget2D _screenTarget;
        public static bool drawing = false;
        private static bool _recordOnly = false;
        private static GraphicsDevice _base;
        public static GraphicsDeviceManager _manager;
        private static SpriteMap _passwordFont;
        public static BitmapFont _biosFont;
        private static BitmapFont _biosFontCaseSensitive;
        private static FancyBitmapFont _fancyBiosFont;
        private static MTSpriteBatch _defaultBatch;
        private static MTSpriteBatch _currentBatch;
        private static Layer _currentLayer;
        private static int _width;
        private static int _height;
        public static Sprite tounge;
        private static bool _frameFlipFlop = false;
        private static RenderTarget2D _screenCapture;
        private static Tex2D _blank;
        private static Tex2D _blank2;
        //private static float _depthBias = 0f;
        private static Matrix _projectionMatrix;
        public static float kSpanIncrement = 0.0001f;
        public static bool caseSensitiveStringDrawing = false; // obsolete from old DG before text sentivity
        public static Vec2 topLeft;
        public static Vec2 bottomRight;
        public static bool didCalc = false;
        public static Material material;
        public static Effect tempEffect;
        public static float snap = 4f;
        public static bool skipReplayRender = false;
        public static bool recordMetadata = false;
        public static bool doSnap = true;
        public static long frame;
        private static Dictionary<Tex2D, Dictionary<Vec3, Tex2D>> _recolorMap = new Dictionary<Tex2D, Dictionary<Vec3, Tex2D>>();
        private static float _baseDeviceWidth = 0f;
        private static float _baseDeviceHeight = 0f;
        private static RenderTarget2D _currentRenderTarget;
        public static RenderTarget2D _screenBufferTarget;
        private static RenderTarget2D _defaultRenderTarget;
        private static bool _settingScreenTarget;
        private static Rectangle _currentTargetSize;
        private static Viewport _oldViewport;
        public static Viewport? _screenViewport;
        private static Viewport _lastViewport;
        private static bool _lastViewportSet = false;
        private static Stack<Rectangle> _scissorStack = new Stack<Rectangle>();

        public static PolygonBatcher polyBatcher;

        public static void GarbageDisposal(bool pLevelTransition)
        {
            lock (objectsToDispose)
            {
                if (!pLevelTransition && objectsToDispose.Count <= 128)
                    return;
                disposingObjects = true;
                foreach (GraphicsResource graphicsResource in objectsToDispose)
                    graphicsResource.Dispose();
                objectsToDispose.Clear();
                disposingObjects = false;
            }
        }

        public static void GarbageDisposal() => GarbageDisposal(true);

        public void Transition(TransitionDirection pDirection, Level pTarget)
        {
        }

        public static void AddRenderTask(Action a) => _renderTasks[_targetFlip % 2].Add(a);

        public static void RunRenderTasks()
        {
            ++_targetFlip;
            foreach (Action action in _renderTasks[(_targetFlip + 1) % 2])
                action();
            _renderTasks[(_targetFlip + 1) % 2].Clear();
        }

        public static void FlashScreen()
        {
            if (!Options.Data.flashing)
                return;
            flashAdd = 1.3f;
            Layer.Game.darken = 1.3f;
            Layer.Blocks.darken = 1.3f;
            Layer.Foreground.darken = 1.3f;
            Layer.Background.darken = -1.3f;
        }

        public static int currentStateIndex
        {
            get => _currentStateIndex;
            set => _currentStateIndex = value;
        }

        public static Vec2 currentDrawOffset
        {
            get => _currentDrawOffset;
            set => _currentDrawOffset = value;
        }

        public static int currentDrawIndex
        {
            get => _currentDrawIndex;
            set => _currentDrawIndex = value;
        }

        public static int fps
        {
            get
            {
                return FPSCounter.GetFPS(0);
            }
        }

        public static RenderTarget2D screenTarget
        {
            get => _screenTarget;
            set => _screenTarget = value;
        }

        public static bool inFocus => MonoMain.framesBackInFocus > 4L;

        public static bool recordOnly
        {
            get => _recordOnly;
            set => _recordOnly = value;
        }

        public static GraphicsDevice device
        {
            get
            {
                // if (Thread.CurrentThread != MonoMain.mainThread && Thread.CurrentThread != MonoMain.initializeThread && Thread.CurrentThread != MonoMain.lazyLoadThread)
                // throw new Exception("accessing graphics device from thread other than main thread.");
                return _base;
            }
            set => _base = value;
        }

        public static MTSpriteBatch screen
        {
            get => _currentBatch;
            set
            {
                _currentBatch = value;
                if (_currentBatch != null)
                    return;
                _currentBatch = _defaultBatch;
            }
        }

        public static Layer currentLayer
        {
            get => _currentLayer;
            set => _currentLayer = value;
        }

        public static bool mouseVisible
        {
            get => MonoMain.instance.IsMouseVisible;
            set => MonoMain.instance.IsMouseVisible = value;
        }

        public static int width
        {
            get => !_screenViewport.HasValue ? device.Viewport.Width : _screenViewport.Value.Width;
            set => _width = value;
        }

        public static int height
        {
            get => !_screenViewport.HasValue ? device.Viewport.Height : _screenViewport.Value.Height;
            set => _height = value;
        }

        public static void SetSize(int w, int h)
        {
            _width = w;
            _height = h;
        }

        public static bool frameFlipFlop
        {
            get => _frameFlipFlop;
            set => _frameFlipFlop = value;
        }

        public static RenderTarget2D screenCapture
        {
            get => _screenCapture;
            set => _screenCapture = value;
        }

        private static float _fade
        {
            get => MonoMain.core._fade;
            set => MonoMain.core._fade = value;
        }

        public static float fade
        {
            get => _fade;
            set => _fade = value;
        }

        private static float _fadeAdd
        {
            get => MonoMain.core._fadeAdd;
            set => MonoMain.core._fadeAdd = value;
        }

        public static float fadeAdd
        {
            get => _fadeAdd;
            set => _fadeAdd = value;
        }

        public static float fadeAddRenderValue => !Options.Data.flashing ? 0f : _fadeAdd;

        private static float _flashAdd
        {
            get => MonoMain.core._flashAdd;
            set => MonoMain.core._flashAdd = value;
        }

        public static float flashAdd
        {
            get => _flashAdd;
            set => _flashAdd = value;
        }

        public static float flashAddRenderValue => !Options.Data.flashing ? 0f : _flashAdd;

        public static bool IsBlankTexture(Tex2D tex) => tex == _blank || tex == _blank2;

        public static Tex2D blankWhiteSquare => _blank;

        public static Matrix projectionMatrix => _projectionMatrix;

        public static void IncrementSpanAdjust()
        {
        }

        public static void ResetSpanAdjust() => Depth.ResetSpan();

        public static float AdjustDepth(Depth depth)
        {
            return 1f - ((depth.value + 1f) / 2f * (1f - Depth.kDepthSpanMax) + depth.span);//(depth.value + 1f) / 2f;//(depth.value + 1f) / 2f * (1f - Depth.kDepthSpanMax) + depth.span;
        }

        public static void ResetDepthBias()
        {
        }

        public static void DrawString(
          string text,
          Vec2 position,
          Color color,
          Depth depth = default(Depth),
          InputProfile pro = null,
          float scale = 1f)
        {
            _biosFont.scale = new Vec2(scale);
            _biosFont.Draw(text, position.x, position.y, color, depth, pro);
            _biosFont.scale = new Vec2(1f);
        }

        public static Texture2D Texture2DFromBase64String(string base64String, GraphicsDevice? graphicsDevice = null)
        {
            graphicsDevice ??= device;
            byte[] buffer = Convert.FromBase64String(base64String);
            return Texture2D.FromStream(graphicsDevice, new MemoryStream(buffer));
        }

        public static void DrawPassword(
          string text,
          Vec2 position,
          Color color,
          Depth depth = default(Depth),
          float scale = 1f)
        {
            for (int index = 0; index < text.Length; ++index)
            {
                if (text[index] == 'L')
                    _passwordFont.frame = 0;
                if (text[index] == 'R')
                    _passwordFont.frame = 1;
                if (text[index] == 'U')
                    _passwordFont.frame = 2;
                if (text[index] == 'D')
                    _passwordFont.frame = 3;
                _passwordFont.scale = new Vec2(scale);
                _passwordFont.color = color;
                _passwordFont.depth = depth;
                Draw(_passwordFont, position.x, position.y);
                position.x += 8f * scale;
            }
        }

        public static void DrawStringColoredSymbols(
          string text,
          Vec2 position,
          Color color,
          Depth depth = default(Depth),
          InputProfile pro = null,
          float scale = 1f)
        {
            _biosFont.scale = new Vec2(scale);
            _biosFont.Draw(text, position.x, position.y, color, depth, pro, true);
            _biosFont.scale = new Vec2(1f);
        }

        public static void DrawStringOutline(
          string text,
          Vec2 position,
          Color color,
          Color outline,
          Depth depth = default(Depth),
          InputProfile pro = null,
          float scale = 1f)
        {
            _biosFont.scale = new Vec2(scale);
            _biosFont.DrawOutline(text, position, color, outline, depth);
            _biosFont.scale = new Vec2(1f);
        }

        public static float GetStringWidth(string text, bool thinButtons = false, float scale = 1f)
        {
            _biosFont.scale = new Vec2(scale);
            text = text.ToUpperInvariant();
            double width = _biosFont.GetWidth(text, thinButtons);
            _biosFont.scale = new Vec2(1f);
            return (float)width;
        }

        public static float GetStringHeight(string text) => text.Split('\n').Length * _biosFont.height;

        public static void DrawFancyString(
          string text,
          Vec2 position,
          Color color,
          Depth depth = default(Depth),
          float scale = 1f)
        {
            _fancyBiosFont.scale = new Vec2(scale);
            _fancyBiosFont.Draw(text, position.x, position.y, color, depth);
            _fancyBiosFont.scale = new Vec2(1f);
        }

        public static float GetFancyStringWidth(string text, bool thinButtons = false, float scale = 1f)
        {
            _fancyBiosFont.scale = new Vec2(scale);
            text = text.ToUpperInvariant();
            double width = _fancyBiosFont.GetWidth(text, thinButtons);
            _fancyBiosFont.scale = new Vec2(1f);
            return (float)width;
        }

        public static void DrawRecorderItem(ref RecorderFrameItem item) => _currentBatch.DrawRecorderItem(ref item);

        public static void DrawRecorderItemLerped(
          ref RecorderFrameItem item,
          ref RecorderFrameItem lerpTo,
          float dist)
        {
            RecorderFrameItem frame = item;
            frame.topLeft = Vec2.Lerp(item.topLeft, lerpTo.topLeft, dist);
            frame.bottomRight = Vec2.Lerp(item.bottomRight, lerpTo.bottomRight, dist);
            float num1 = item.rotation % 360f;
            float num2 = lerpTo.rotation % 360f;
            if (num1 > 180)
                num1 -= 360f;
            else if (num1 < -180)
                num1 += 360f;
            if (num2 > 180)
                num2 -= 360f;
            else if (num2 < -180)
                num2 += 360f;
            frame.rotation = MathHelper.Lerp(num1, num2, dist);
            frame.color = Color.Lerp(item.color, lerpTo.color, dist);
            _currentBatch.DrawRecorderItem(ref frame);
        }

        public static void Calc()
        {
            if (didCalc)
                return;
            didCalc = true;
            Viewport viewport = new Viewport(0, 0, 32, 32);
            Matrix result;
            Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 0f, -1f, out result);
            result.M41 += -0.5f * result.M11;
            result.M42 += -0.5f * result.M22;
            bottomRight = new Vec2(32f, 32f);
            bottomRight = Vec2.Transform(bottomRight, result);
            topLeft = new Vec2(0f, 0f);
            topLeft = Vec2.Transform(topLeft, result);
        }

        public static Queue<List<DrawCall>> drawCalls
        {
            get => Level.core.drawCalls;
            set => Level.core.drawCalls = value;
        }

        public static List<DrawCall> currentFrameCalls
        {
            get => Level.core.currentFrameCalls;
            set => Level.core.currentFrameCalls = value;
        }

        public static bool skipFrameLog
        {
            get => Level.core.skipFrameLog;
            set => Level.core.skipFrameLog = value;
        }

        public static void Draw(MTSpriteBatchItem item) => _currentBatch.DrawExistingBatchItem(item);

        public static void Draw(
          Tex2D texture,
          Vec2 position,
          Rectangle? sourceRectangle,
          Color color,
          float rotation,
          Vec2 origin,
          Vec2 scale,
          SpriteEffects effects,
          Depth depth = default(Depth))
        {
            if (texture.nativeObject is Microsoft.Xna.Framework.Graphics.RenderTarget2D)
            {
                if ((texture.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D).IsDisposed)
                    return;
                if (texture.textureIndex == 0)
                    Content.AssignTextureIndex(texture);
            }
            if (doSnap)
            {
                position.x = (float)Math.Round(position.x * snap) / snap;
                position.y = (float)Math.Round(position.y * snap) / snap;
            }
            if (effects == SpriteEffects.FlipHorizontally)
                origin.x = (sourceRectangle.HasValue ? sourceRectangle.Value.width : texture.w) - origin.x;
            float depth1 = AdjustDepth(depth);
            if (material != null)
                _currentBatch.DrawWithMaterial(texture, position, sourceRectangle, color, rotation, origin, scale, effects, depth1, material);
            else
                _currentBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, depth1);
        }

        public static void Draw(Sprite g, float x, float y)
        {
            g.x = x;
            g.y = y;
            g.Draw();
        }

        public static void Draw(Sprite g, float x, float y, Rectangle sourceRectangle)
        {
            g.x = x;
            g.y = y;
            g.Draw(sourceRectangle);
        }

        public static void Draw(Sprite g, float x, float y, Rectangle sourceRectangle, Vec2 scale)
        {
            g.x = x;
            g.y = y;
            g.scale = scale;
            g.Draw(sourceRectangle);
        }

        public static void Draw(Sprite g, float x, float y, Rectangle sourceRectangle, Depth depth)
        {
            g.x = x;
            g.y = y;
            g.depth = depth;
            g.Draw(sourceRectangle);
        }

        public static void Draw(Sprite g, float x, float y, Depth depth = default(Depth))
        {
            g.x = x;
            g.y = y;
            g.depth = depth;
            g.Draw();
        }

        public static void Draw(Sprite g, float x, float y, float scaleX, float scaleY)
        {
            g.x = x;
            g.y = y;
            g.xscale = scaleX;
            g.yscale = scaleY;
            g.Draw();
        }

        public static void Draw(
          Tex2D target,
          float x,
          float y,
          float xscale = 1f,
          float yscale = 1f,
          Depth depth = default(Depth))
        {
            Draw(target, new Vec2(x, y), new Rectangle?(), Color.White, 0f, Vec2.Zero, new Vec2(xscale, yscale), SpriteEffects.None, depth);
        }

        public static void Draw(
          SpriteMap g,
          int frame,
          float x,
          float y,
          float scaleX = 1f,
          float scaleY = 1f,
          bool maintainFrame = false)
        {
            g.x = x;
            g.y = y;
            g.xscale = scaleX;
            g.yscale = scaleY;
            int frame1 = g.frame;
            g.SetFrameWithoutReset(frame);
            g.Draw();
            if (!maintainFrame)
                return;
            g.SetFrameWithoutReset(frame1);
        }

        public static void DrawWithoutUpdate(
          SpriteMap g,
          float x,
          float y,
          float scaleX = 1f,
          float scaleY = 1f,
          bool maintainFrame = false)
        {
            g.x = x;
            g.y = y;
            g.xscale = scaleX;
            g.yscale = scaleY;
            g.DrawWithoutUpdate();
        }

        public static void DrawLine(Vec2 p1, Vec2 p2, Color col, float width = 1f, Depth depth = default(Depth))
        {
            ++currentDrawIndex;
            //p1 = new Vec2(p1.x, p1.y);
            //p2 = new Vec2(p2.x, p2.y);
            float rotation = (float)Math.Atan2(p2.y - p1.y, p2.x - p1.x);
            float length = (p1 - p2).length;
            Draw(_blank, p1, new Rectangle?(), col, rotation, new Vec2(0f, 0.5f), new Vec2(length, width), SpriteEffects.None, depth);
        }
        
        public static void DrawLine(Vec2 p, float lineLength, float angleDegrees, Color col, float width = 1f, Depth depth = default(Depth))
        {
            float angleRadians = Maths.DegToRad(angleDegrees);
            Vec2 lineEnd = new(lineLength * Maths.FastSin(angleRadians), lineLength * Maths.FastCos(angleRadians));
            
            DrawLine(p, p + lineEnd, col, width, depth);
        }

        public static void DrawDottedLine(
          Vec2 p1,
          Vec2 p2,
          Color col,
          float width = 1f,
          float dotLength = 8f,
          Depth depth = default(Depth))
        {
            ++currentDrawIndex;
            Vec2 vec2_1 = p1;
            Vec2 vec2_2 = p2 - p1;
            float length = vec2_2.length;
            int num = (int)(length / dotLength);
            vec2_2.Normalize();
            bool flag = false;
            for (int index = 0; index < num; ++index)
            {
                Vec2 vec2_3 = vec2_1 + vec2_2 * dotLength;
                if ((vec2_3 - p1).length > length)
                    vec2_3 = p2;
                if (!flag)
                    DrawLine(new Vec2(vec2_1.x, vec2_1.y), new Vec2(vec2_3.x, vec2_3.y), col, width, depth);
                flag = !flag;
                vec2_1 = vec2_3;
            }
        }

        public static void DrawCircle(
          Vec2 pos,
          float radius,
          Color col,
          float width = 1f,
          Depth depth = default(Depth),
          int iterations = 32)
        {
            Vec2 vec2_1 = Vec2.Zero;
            for (int index = 0; index < iterations; ++index)
            {
                float rad = Maths.DegToRad(360f / (iterations - 1) * index);
                Vec2 vec2_2 = new Vec2((float)Math.Cos(rad) * radius, (float)-Math.Sin(rad) * radius);
                if (index > 0)
                    DrawLine(pos + vec2_2, pos + vec2_1, col, width, depth);
                vec2_1 = vec2_2;
            }
        }

        public static void DrawTexturedLine(
          Tex2D texture,
          Vec2 p1,
          Vec2 p2,
          Color col,
          float width = 1f,
          Depth depth = default(Depth))
        {
            ++currentDrawIndex;
            if (texture.width > 1)
            {
                p1 = new Vec2(p1.x, p1.y);
                p2 = new Vec2(p2.x, p2.y);
                float rotation = (float)Math.Atan2(p2.y - p1.y, p2.x - p1.x);
                float x = (p1 - p2).length / texture.width;
                Draw(texture, p1, new Rectangle?(), col, rotation, new Vec2(0f, texture.height / 2), new Vec2(x, width), SpriteEffects.None, depth);
            }
            else
            {
                p1 = new Vec2(p1.x, p1.y);
                p2 = new Vec2(p2.x, p2.y);
                float rotation = (float)Math.Atan2(p2.y - p1.y, p2.x - p1.x);
                float length = (p1 - p2).length;
                Draw(texture, p1, new Rectangle?(), col, rotation, new Vec2(0f, texture.height / 2), new Vec2(length, width), SpriteEffects.None, depth);
            }
        }

        public static void DrawRect(
          Vec2 p1,
          Vec2 p2,
          Color col,
          Depth depth = default(Depth),
          bool filled = true,
          float borderWidth = 1f)
        {
            ++currentDrawIndex;
            if (filled)
            {
                Draw(_blank2, p1, new Rectangle?(), col, 0f, Vec2.Zero, new Vec2((float)-(p1.x - p2.x), (float)-(p1.y - p2.y)), SpriteEffects.None, depth);
            }
            else
            {
                float num = borderWidth / 2f;
                DrawLine(new Vec2(p1.x, p1.y + num), new Vec2(p2.x, p1.y + num), col, borderWidth, depth);
                DrawLine(new Vec2(p1.x + num, p1.y + borderWidth), new Vec2(p1.x + num, p2.y - borderWidth), col, borderWidth, depth);
                DrawLine(new Vec2(p2.x, p2.y - num), new Vec2(p1.x, p2.y - num), col, borderWidth, depth);
                DrawLine(new Vec2(p2.x - num, p2.y - borderWidth), new Vec2(p2.x - num, p1.y + borderWidth), col, borderWidth, depth);
            }
        }

        public static void DrawRect(
          Rectangle r,
          Color col,
          Depth depth = default(Depth),
          bool filled = true,
          float borderWidth = 1f)
        {
            ++currentDrawIndex;
            Vec2 position = new Vec2(r.Left, r.Top);
            Vec2 vec2 = new Vec2(r.Right, r.Bottom);
            if (filled)
            {
                Draw(_blank2, position, new Rectangle?(), col, 0f, Vec2.Zero, new Vec2((float)-(position.x - vec2.x), (float)-(position.y - vec2.y)), SpriteEffects.None, depth);
            }
            else
            {
                float num = borderWidth / 2f;
                DrawLine(new Vec2(position.x, position.y + num), new Vec2(vec2.x, position.y + num), col, borderWidth, depth);
                DrawLine(new Vec2(position.x + num, position.y + borderWidth), new Vec2(position.x + num, vec2.y - borderWidth), col, borderWidth, depth);
                DrawLine(new Vec2(vec2.x, vec2.y - num), new Vec2(position.x, vec2.y - num), col, borderWidth, depth);
                DrawLine(new Vec2(vec2.x - num, vec2.y - borderWidth), new Vec2(vec2.x - num, position.y + borderWidth), col, borderWidth, depth);
            }
        }
        
        public static void DrawOutlinedRect(Rectangle rect, Color col, Color outlineCol, Depth depth = default, float borderwidth = 1f)
        {
            DrawRect(rect, col, depth, true, 0);
            DrawRect(rect, outlineCol, depth.value + 0.05f, false, borderwidth);
        }

        public static void DrawDottedRect(
          Vec2 p1,
          Vec2 p2,
          Color col,
          Depth depth = default(Depth),
          float borderWidth = 1f,
          float dotLength = 8f)
        {
            ++currentDrawIndex;
            float num = borderWidth / 2f;
            DrawDottedLine(new Vec2(p1.x, p1.y + num), new Vec2(p2.x, p1.y + num), col, borderWidth, dotLength, depth);
            DrawDottedLine(new Vec2(p1.x + num, p1.y + borderWidth), new Vec2(p1.x + num, p2.y - borderWidth), col, borderWidth, dotLength, depth);
            DrawDottedLine(new Vec2(p2.x, p2.y - num), new Vec2(p1.x, p2.y - num), col, borderWidth, dotLength, depth);
            DrawDottedLine(new Vec2(p2.x - num, p2.y - borderWidth), new Vec2(p2.x - num, p1.y + borderWidth), col, borderWidth, dotLength, depth);
        }

        public static Tex2D Recolor(string sprite, Vec3 color) => RecolorOld(Content.Load<Tex2D>(sprite), color);

        public static Tex2D Recolor(Tex2D sprite, Vec3 color)
        {
            Dictionary<Vec3, Tex2D> dictionary;
            if (_recolorMap.TryGetValue(sprite, out dictionary))
            {
                Tex2D tex2D;
                if (dictionary.TryGetValue(color, out tex2D))
                    return tex2D;
            }
            else
                _recolorMap[sprite] = new Dictionary<Vec3, Tex2D>();
            Material material = new MaterialRecolor(new Vec3(color.x / byte.MaxValue, color.y / byte.MaxValue, color.z / byte.MaxValue));
            RenderTarget2D t = new RenderTarget2D(sprite.w, sprite.h);
            SetRenderTarget(t);
            Clear(Color.Transparent);
            material.Apply();
            screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, material.effect, Matrix.Identity);
            Draw(sprite, new Vec2(), new Rectangle?(), Color.White, 0f, new Vec2(), new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.5f);
            screen.End();
            device.SetRenderTarget(null);
            Tex2D tex2D1 = new Tex2D(sprite.w, sprite.h);
            tex2D1.SetData(t.GetData());
            tex2D1.AssignTextureName("RESKIN");
            t.Dispose();
            _recolorMap[sprite][color] = tex2D1;
            return tex2D1;
        }

        public static Tex2D RecolorOld(Tex2D sprite, Vec3 color)
        {
            Material material = new MaterialRecolor(new Vec3(color.x / byte.MaxValue, color.y / byte.MaxValue, color.z / byte.MaxValue));
            RenderTarget2D t = new RenderTarget2D(sprite.w, sprite.h);
            SetRenderTarget(t);
            Clear(Color.Transparent);
            material.Apply();
            screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, material.effect, Matrix.Identity);
            Draw(sprite, new Vec2(), new Rectangle?(), Color.White, 0f, new Vec2(), new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.5f);
            screen.End();
            device.SetRenderTarget(null);
            Tex2D tex2D = new Tex2D(sprite.w, sprite.h);
            tex2D.SetData(t.GetData());
            t.Dispose();
            return tex2D;
        }

        public static Tex2D RecolorNew(Tex2D sprite, Color color1, Color color2)
        {
            Color color3 = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
            Color color4 = new Color(157, 157, 157);
            Color[] data = sprite.GetData();
            for (int index = 0; index < data.Length; ++index)
            {
                if (data[index] == color3)
                    data[index] = color1;
                else if (data[index] == color4)
                    data[index] = color2;
            }
            Tex2D tex2D = new Tex2D(sprite.w, sprite.h);
            tex2D.SetData(data);
            return tex2D;
        }

        public static Tex2D RecolorM(Tex2D sprite, Color color1, Color color2)
        {
            Color color3 = new Color(195, 184, 172);
            Color color4 = new Color(163, 147, 128);
            Color[] data = sprite.GetData();
            for (int index = 0; index < data.Length; ++index)
            {
                if (data[index] == color3)
                    data[index] = color1;
                else if (data[index] == color4)
                    data[index] = color2;
            }
            Tex2D tex2D = new Tex2D(sprite.w, sprite.h);
            tex2D.SetData(data);
            return tex2D;
        }

        public static Tex2D RecolorM(Tex2D sprite, Color color1, Color color2, Color color3)
        {
            Color color9 = new Color(235, 137, 49); //-1341135  C
            Color color10 = new Color(247, 224, 90); // -532390
            Color color13 = new Color(164, 100, 34); // -6003678
            Color color14 = new Color(235, 137, 49); // -1341135 C
            Color[] data = sprite.GetData();
            for (int index = 0; index < data.Length; ++index)
            {
                switch (data[index].GetHashCode())
                {
                    case -6188414: //new Color(161, 146, 130); 
                        data[index] = color1;
                        break;
                    case -8359584: // new Color(128, 113, 96);
                        data[index] = color2;
                        break;
                    case -4213333: //new Color(191, 181, 171);
                        data[index] = color3;
                        break;
                    case -1287876: //new Color(236, 89, 60);
                        data[index] = color9;
                        break;
                    case -490653: //new Color(248, 131, 99);
                        data[index] = color10;
                        break;
                    case -2402273: // new Color(219, 88, 31);
                        data[index] = color13;
                        break;
                    case -1280964: //  new Color(236, 116, 60);
                        data[index] = color14;
                        break;
                }
            }
            Tex2D tex2D = new Tex2D(sprite.w, sprite.h);
            tex2D.SetData(data);
            return tex2D;
        }

        public static float baseDeviceWidth => _baseDeviceWidth;

        public static float baseDeviceHeight => _baseDeviceHeight;

        public static bool fixedAspect
        {
            get
            {
                if (Resolution.current.aspect > 1.8f)
                    return true;
                return !(Level.current is XMLLevel) && !(Level.current is Editor);
            }
        }

        public static float aspect => 9f / 16f;

        public static bool sixteenTen => aspect > 0.57f;

        public static float barSize => ((width * aspect - width * (9f / 16f)) / 2f);

        public static void InitializeBase(GraphicsDeviceManager m, int widthVal, int heightVal)
        {
            _manager = m;
            _width = widthVal;
            _baseDeviceWidth = _width;
            _height = heightVal;
            _baseDeviceHeight = _height;
        }

        public static void Initialize(GraphicsDevice d)
        {
            _base = d;
            _defaultBatch = new MTSpriteBatch(_base);
            screen = _defaultBatch;
            _blank = new Tex2D(1, 1);

            _blank.SetData(new Color[1]
            {
                    Color.White
            });

            _blank2 = new Tex2D(1, 1);
            _blank2.SetData(new Color[1]
            {
                    Color.White
            });
            _blank.Namebase = "_blanktex2d";
            _blank2.Namebase = "_blank2tex2d";
            Content.textures[_blank.Namebase] = _blank; //spriteatlas stuff
            Content.textures[_blank2.Namebase] = _blank2; //spriteatlas stuff
            _biosFont = new BitmapFont("biosFont", 8);
            _biosFontCaseSensitive = new BitmapFont("biosFontCaseSensitive", 8);
            _fancyBiosFont = new FancyBitmapFont("smallFont");
            _passwordFont = new SpriteMap("passwordFont", 8, 8);
            Viewport viewport = d.Viewport;
            double width = viewport.Width;
            viewport = d.Viewport;
            double height = viewport.Height;
            ref Matrix local = ref _projectionMatrix;
            Matrix.CreateOrthographicOffCenter(0f, (float)width, (float)height, 0f, 0f, 1f, out local);
            _projectionMatrix.M41 += -0.5f * _projectionMatrix.M11;
            _projectionMatrix.M42 += -0.5f * _projectionMatrix.M22;
            tounge = new Sprite("tounge");

            polyBatcher = new PolygonBatcher(_manager);
        }

        public static RenderTarget2D currentRenderTarget => _currentRenderTarget;

        public static RenderTarget2D defaultRenderTarget
        {
            get
            {
                if (_settingScreenTarget)
                    return null;
                return _defaultRenderTarget == null ? _screenBufferTarget : _defaultRenderTarget;
            }
            set => _defaultRenderTarget = value;
        }

        public static void SetRenderTargetToScreen()
        {
            _settingScreenTarget = true;
            SetRenderTarget(null);
            _settingScreenTarget = false;
        }

        public static void SetRenderTarget(RenderTarget2D t)
        {
            if (t != null && t.IsDisposed)
                return;
            if (t == null)
            {
                Microsoft.Xna.Framework.Graphics.RenderTarget2D renderTarget = defaultRenderTarget != null ? defaultRenderTarget.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D : null;
                if (renderTarget == null)
                {
                    _currentTargetSize.width = Resolution.current.x;
                    _currentTargetSize.height = Resolution.current.y;
                }
                else
                {
                    _currentTargetSize.width = renderTarget.Width;
                    _currentTargetSize.height = renderTarget.Height;
                }
                device.SetRenderTarget(renderTarget);
                if (!_settingScreenTarget && _defaultRenderTarget == null && !SettingForShader)
                    UpdateScreenViewport();
            }
            else
            {
                device.SetRenderTarget(t.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D);
                _currentTargetSize.width = t.width;
                _currentTargetSize.height = t.height;
            }
            _lastViewport = device.Viewport;
            _currentRenderTarget = t;
        }

        public static RenderTarget2D GetRenderTarget() => _currentRenderTarget;

        public static void SetFullViewport()
        {
            _oldViewport = device.Viewport;
            Internal_ViewportSet(new Viewport()
            {
                X = 0,
                Y = 0,
                Width = (int)_currentTargetSize.width,
                Height = (int)_currentTargetSize.height
            });
        }

        public static void RestoreOldViewport() => Internal_ViewportSet(_oldViewport);

        private static void Internal_ViewportSet(Viewport pViewport)
        {
            try
            {
                device.Viewport = pViewport;
            }
            catch (Exception)
            {
                DevConsole.Log("Error: Invalid Viewport (x = " + pViewport.X.ToString() + ", y = " + pViewport.Y.ToString() + ", w = " + pViewport.Width.ToString() + ", h = " + pViewport.Height.ToString() + ", minDepth = " + pViewport.MinDepth.ToString() + ", maxDepth = " + pViewport.MaxDepth.ToString() + ")");
            }
        }
        public static bool SettingForShader;
        public static void UpdateScreenViewport(bool pForceReset = false)
        {
            try
            {
                if (pForceReset || !_screenViewport.HasValue)
                {
                    Viewport viewport = new Viewport();
                    if (_currentTargetSize.aspect < 1.77f)
                    {
                        viewport.Width = (int)_currentTargetSize.width;
                        viewport.Height = Math.Min((int)Math.Round(_currentTargetSize.width / 1.77777f), (int)_currentTargetSize.height);
                    }
                    else
                    {
                        viewport.Height = (int)_currentTargetSize.height;
                        viewport.Width = Math.Min((int)Math.Round(_currentTargetSize.height * 1.77777f), (int)_currentTargetSize.width);
                    }
                    viewport.X = Math.Max((int)((_currentTargetSize.width - viewport.Width) / 2f), 0);
                    viewport.Y = Math.Max((int)((_currentTargetSize.height - viewport.Height) / 2f), 0);
                    viewport.MinDepth = 0f;
                    viewport.MaxDepth = 1f;
                    _screenViewport = new Viewport?(viewport);
                }
                Internal_ViewportSet(_screenViewport.Value);
            }
            catch (Exception)
            {
            }
            _lastViewport = device.Viewport;
        }

        public static void SetScreenTargetViewport()
        {
            Viewport pViewport = new Viewport();
            if (Resolution.adapterResolution.aspect < Resolution.current.aspect)
            {
                pViewport.Width = Resolution.adapterResolution.x;
                pViewport.Height = Math.Min((int)Math.Round(Resolution.adapterResolution.x / Resolution.current.aspect), Resolution.adapterResolution.y);
            }
            else
            {
                pViewport.Height = Resolution.adapterResolution.y;
                pViewport.Width = Math.Min((int)Math.Round(Resolution.adapterResolution.y * Resolution.current.aspect), Resolution.adapterResolution.x);
            }
            pViewport.X = Math.Max((Resolution.adapterResolution.x - pViewport.Width) / 2, 0);
            pViewport.Y = Math.Max((Resolution.adapterResolution.y - pViewport.Height) / 2, 0);
            pViewport.MinDepth = 0f;
            pViewport.MaxDepth = 1f;
            Internal_ViewportSet(pViewport);
        }

        public static Viewport viewport
        {
            get
            {
                if (device == null || device.IsDisposed)
                {
                    return _lastViewport;
                }
                return device.Viewport;
            }
            set
            {
                if (!_lastViewportSet)
                {
                    _lastViewport = value;
                    _lastViewportSet = true;
                }
                if (device.Viewport.Width != _lastViewport.Width || device.Viewport.Height != _lastViewport.Height)
                {
                    return;
                }
                Rectangle r = value.Bounds;
                if (_currentRenderTarget != null)
                {
                    ClipRectangle(r, new Rectangle(0f, 0f, _currentRenderTarget.width, _currentRenderTarget.height));
                }
                else
                {
                    ClipRectangle(r, device.PresentationParameters.Bounds);
                }
                value.X = (int)r.x;
                value.Y = (int)r.y;
                value.Width = (int)r.width;
                value.Height = (int)r.height;
                Internal_ViewportSet(value);
                _lastViewport = value;
            }
        }

        public static Rectangle GetScissorRectangle() => new Rectangle(device.ScissorRectangle.X, device.ScissorRectangle.Y, device.ScissorRectangle.Width, device.ScissorRectangle.Height);

        public static void SetScissorRectangle(Rectangle r)
        {
            float num = device.Viewport.Bounds.Width / (float)width;
            if (r.width < 0f || r.height < 0f)
                return;
            r.width *= num;
            r.height *= num;
            r.x *= num;
            r.y *= num;
            r.x += viewport.X;
            r.y += viewport.Y;
            device.ScissorRectangle = (Microsoft.Xna.Framework.Rectangle)ClipRectangle(r, (Rectangle)device.Viewport.Bounds);
        }

        public static void PushLayerScissor(Rectangle pRect)
        {
            if (screen != null)
                screen.FlushSettingScissor();
            _scissorStack.Push(pRect);
            float num1 = width / currentLayer.width;
            float num2 = height / currentLayer.height;
            pRect.x *= num1;
            pRect.y *= num2;
            pRect.width *= num1;
            pRect.height *= num2;
            SetScissorRectangle(pRect);
        }

        public static void PopLayerScissor()
        {
            if (screen != null)
                screen.FlushAndClearScissor();
            _scissorStack.Pop();
            if (_scissorStack.Count == 0)
                SetScissorRectangle(new Rectangle(0f, 0f, width, height));
            else
                SetScissorRectangle(_scissorStack.Peek());
        }

        public static Rectangle ClipRectangle(Rectangle r, Rectangle clipTo)
        {
            if (r.x > clipTo.Right)
                r.x = clipTo.Right - r.width;
            if (r.y > clipTo.Bottom)
                r.y = clipTo.Bottom - r.height;
            if (r.x < clipTo.Left)
                r.x = clipTo.Left;
            if (r.y < clipTo.Top)
                r.y = clipTo.Top;
            if (r.x < 0f)
                r.x = 0f;
            if (r.y < 0f)
                r.y = 0f;
            if (r.x + r.width > clipTo.x + clipTo.width)
                r.width = clipTo.Right - r.x;
            if (r.y + r.height > clipTo.y + clipTo.height)
                r.height = clipTo.Bottom - r.y;
            if (r.width < 0f)
                r.width = 0f;
            if (r.height < 0f)
                r.height = 0f;
            return r;
        }

        public static void Clear(Color c) => device.Clear((Microsoft.Xna.Framework.Color)c);

        public static void PushMarker(string s)
        {
        }

        public static void PopMarker()
        {
        }
    }
}
