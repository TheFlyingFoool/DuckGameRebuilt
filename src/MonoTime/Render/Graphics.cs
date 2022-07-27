// Decompiled with JetBrains decompiler
// Type: DuckGame.Graphics
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        //private static float _depthBias = 0.0f;
        private static Matrix _projectionMatrix;
        public static float kSpanIncrement = 0.0001f;
        public static bool caseSensitiveStringDrawing = false;
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
        private static float _baseDeviceWidth = 0.0f;
        private static float _baseDeviceHeight = 0.0f;
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

        public static void GarbageDisposal(bool pLevelTransition)
        {
            lock (DuckGame.Graphics.objectsToDispose)
            {
                if (!pLevelTransition && DuckGame.Graphics.objectsToDispose.Count <= 128)
                    return;
                DuckGame.Graphics.disposingObjects = true;
                foreach (GraphicsResource graphicsResource in DuckGame.Graphics.objectsToDispose)
                    graphicsResource.Dispose();
                DuckGame.Graphics.objectsToDispose.Clear();
                DuckGame.Graphics.disposingObjects = false;
            }
        }

        public static void GarbageDisposal() => DuckGame.Graphics.GarbageDisposal(true);

        public void Transition(TransitionDirection pDirection, Level pTarget)
        {
        }

        public static void AddRenderTask(Action a) => DuckGame.Graphics._renderTasks[DuckGame.Graphics._targetFlip % 2].Add(a);

        public static void RunRenderTasks()
        {
            ++DuckGame.Graphics._targetFlip;
            foreach (Action action in DuckGame.Graphics._renderTasks[(DuckGame.Graphics._targetFlip + 1) % 2])
                action();
            DuckGame.Graphics._renderTasks[(DuckGame.Graphics._targetFlip + 1) % 2].Clear();
        }

        public static void FlashScreen()
        {
            if (!Options.Data.flashing)
                return;
            DuckGame.Graphics.flashAdd = 1.3f;
            Layer.Game.darken = 1.3f;
            Layer.Blocks.darken = 1.3f;
            Layer.Foreground.darken = 1.3f;
            Layer.Background.darken = -1.3f;
        }

        public static int currentStateIndex
        {
            get => DuckGame.Graphics._currentStateIndex;
            set => DuckGame.Graphics._currentStateIndex = value;
        }

        public static Vec2 currentDrawOffset
        {
            get => DuckGame.Graphics._currentDrawOffset;
            set => DuckGame.Graphics._currentDrawOffset = value;
        }

        public static int currentDrawIndex
        {
            get => DuckGame.Graphics._currentDrawIndex;
            set => DuckGame.Graphics._currentDrawIndex = value;
        }

        public static int fps => FPSCounter.GetFPS(0);

        public static RenderTarget2D screenTarget
        {
            get => DuckGame.Graphics._screenTarget;
            set => DuckGame.Graphics._screenTarget = value;
        }

        public static bool inFocus => MonoMain.framesBackInFocus > 4L;

        public static bool recordOnly
        {
            get => DuckGame.Graphics._recordOnly;
            set => DuckGame.Graphics._recordOnly = value;
        }

        public static GraphicsDevice device
        {
            get
            {
                if (Thread.CurrentThread != MonoMain.mainThread && Thread.CurrentThread != MonoMain.initializeThread && Thread.CurrentThread != MonoMain.lazyLoadThread)
                    throw new Exception("accessing graphics device from thread other than main thread.");
                return DuckGame.Graphics._base;
            }
            set => DuckGame.Graphics._base = value;
        }

        public static MTSpriteBatch screen
        {
            get => DuckGame.Graphics._currentBatch;
            set
            {
                DuckGame.Graphics._currentBatch = value;
                if (DuckGame.Graphics._currentBatch != null)
                    return;
                DuckGame.Graphics._currentBatch = DuckGame.Graphics._defaultBatch;
            }
        }

        public static Layer currentLayer
        {
            get => DuckGame.Graphics._currentLayer;
            set => DuckGame.Graphics._currentLayer = value;
        }

        public static bool mouseVisible
        {
            get => MonoMain.instance.IsMouseVisible;
            set => MonoMain.instance.IsMouseVisible = value;
        }

        public static int width
        {
            get => !DuckGame.Graphics._screenViewport.HasValue ? DuckGame.Graphics.device.Viewport.Width : DuckGame.Graphics._screenViewport.Value.Width;
            set => DuckGame.Graphics._width = value;
        }

        public static int height
        {
            get => !DuckGame.Graphics._screenViewport.HasValue ? DuckGame.Graphics.device.Viewport.Height : DuckGame.Graphics._screenViewport.Value.Height;
            set => DuckGame.Graphics._height = value;
        }

        public static void SetSize(int w, int h)
        {
            DuckGame.Graphics._width = w;
            DuckGame.Graphics._height = h;
        }

        public static bool frameFlipFlop
        {
            get => DuckGame.Graphics._frameFlipFlop;
            set => DuckGame.Graphics._frameFlipFlop = value;
        }

        public static RenderTarget2D screenCapture
        {
            get => DuckGame.Graphics._screenCapture;
            set => DuckGame.Graphics._screenCapture = value;
        }

        private static float _fade
        {
            get => MonoMain.core._fade;
            set => MonoMain.core._fade = value;
        }

        public static float fade
        {
            get => DuckGame.Graphics._fade;
            set => DuckGame.Graphics._fade = value;
        }

        private static float _fadeAdd
        {
            get => MonoMain.core._fadeAdd;
            set => MonoMain.core._fadeAdd = value;
        }

        public static float fadeAdd
        {
            get => DuckGame.Graphics._fadeAdd;
            set => DuckGame.Graphics._fadeAdd = value;
        }

        public static float fadeAddRenderValue => !Options.Data.flashing ? 0.0f : DuckGame.Graphics._fadeAdd;

        private static float _flashAdd
        {
            get => MonoMain.core._flashAdd;
            set => MonoMain.core._flashAdd = value;
        }

        public static float flashAdd
        {
            get => DuckGame.Graphics._flashAdd;
            set => DuckGame.Graphics._flashAdd = value;
        }

        public static float flashAddRenderValue => !Options.Data.flashing ? 0.0f : DuckGame.Graphics._flashAdd;

        public static bool IsBlankTexture(Tex2D tex) => tex == DuckGame.Graphics._blank || tex == DuckGame.Graphics._blank2;

        public static Tex2D blankWhiteSquare => DuckGame.Graphics._blank;

        public static Matrix projectionMatrix => DuckGame.Graphics._projectionMatrix;

        public static void IncrementSpanAdjust()
        {
        }

        public static void ResetSpanAdjust() => Depth.ResetSpan();

        public static float AdjustDepth(Depth depth) => 1f - ((float)((depth.value + 1.0) / 2.0 * (1.0 - Depth.kDepthSpanMax)) + depth.span);

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
            if (DuckGame.Graphics.caseSensitiveStringDrawing)
            {
                DuckGame.Graphics._biosFontCaseSensitive.scale = new Vec2(scale);
                DuckGame.Graphics._biosFontCaseSensitive.Draw(text, position.x, position.y, color, depth, pro);
                DuckGame.Graphics._biosFontCaseSensitive.scale = new Vec2(1f);
            }
            else
            {
                DuckGame.Graphics._biosFont.scale = new Vec2(scale);
                DuckGame.Graphics._biosFont.Draw(text, position.x, position.y, color, depth, pro);
                DuckGame.Graphics._biosFont.scale = new Vec2(1f);
            }
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
                    DuckGame.Graphics._passwordFont.frame = 0;
                if (text[index] == 'R')
                    DuckGame.Graphics._passwordFont.frame = 1;
                if (text[index] == 'U')
                    DuckGame.Graphics._passwordFont.frame = 2;
                if (text[index] == 'D')
                    DuckGame.Graphics._passwordFont.frame = 3;
                DuckGame.Graphics._passwordFont.scale = new Vec2(scale);
                DuckGame.Graphics._passwordFont.color = color;
                DuckGame.Graphics._passwordFont.depth = depth;
                DuckGame.Graphics.Draw(_passwordFont, position.x, position.y);
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
            if (DuckGame.Graphics.caseSensitiveStringDrawing)
            {
                DuckGame.Graphics._biosFontCaseSensitive.scale = new Vec2(scale);
                DuckGame.Graphics._biosFontCaseSensitive.Draw(text, position.x, position.y, color, depth, pro, true);
                DuckGame.Graphics._biosFontCaseSensitive.scale = new Vec2(1f);
            }
            else
            {
                DuckGame.Graphics._biosFont.scale = new Vec2(scale);
                DuckGame.Graphics._biosFont.Draw(text, position.x, position.y, color, depth, pro, true);
                DuckGame.Graphics._biosFont.scale = new Vec2(1f);
            }
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
            DuckGame.Graphics._biosFont.scale = new Vec2(scale);
            DuckGame.Graphics._biosFont.DrawOutline(text, position, color, outline, depth);
            DuckGame.Graphics._biosFont.scale = new Vec2(1f);
        }

        public static float GetStringWidth(string text, bool thinButtons = false, float scale = 1f)
        {
            DuckGame.Graphics._biosFont.scale = new Vec2(scale);
            text = text.ToUpperInvariant();
            double width = (double)DuckGame.Graphics._biosFont.GetWidth(text, thinButtons);
            DuckGame.Graphics._biosFont.scale = new Vec2(1f);
            return (float)width;
        }

        public static float GetStringHeight(string text) => text.Split('\n').Count<string>() * DuckGame.Graphics._biosFont.height;

        public static void DrawFancyString(
          string text,
          Vec2 position,
          Color color,
          Depth depth = default(Depth),
          float scale = 1f)
        {
            DuckGame.Graphics._fancyBiosFont.scale = new Vec2(scale);
            DuckGame.Graphics._fancyBiosFont.Draw(text, position.x, position.y, color, depth);
            DuckGame.Graphics._fancyBiosFont.scale = new Vec2(1f);
        }

        public static float GetFancyStringWidth(string text, bool thinButtons = false, float scale = 1f)
        {
            DuckGame.Graphics._fancyBiosFont.scale = new Vec2(scale);
            text = text.ToUpperInvariant();
            double width = (double)DuckGame.Graphics._fancyBiosFont.GetWidth(text, thinButtons);
            DuckGame.Graphics._fancyBiosFont.scale = new Vec2(1f);
            return (float)width;
        }

        public static void DrawRecorderItem(ref RecorderFrameItem item) => DuckGame.Graphics._currentBatch.DrawRecorderItem(ref item);

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
            if ((double)num1 > 180.0)
                num1 -= 360f;
            else if ((double)num1 < -180.0)
                num1 += 360f;
            if ((double)num2 > 180.0)
                num2 -= 360f;
            else if ((double)num2 < -180.0)
                num2 += 360f;
            frame.rotation = MathHelper.Lerp(num1, num2, dist);
            frame.color = Color.Lerp(item.color, lerpTo.color, dist);
            DuckGame.Graphics._currentBatch.DrawRecorderItem(ref frame);
        }

        public static void Calc()
        {
            if (DuckGame.Graphics.didCalc)
                return;
            DuckGame.Graphics.didCalc = true;
            Viewport viewport = new Viewport(0, 0, 32, 32);
            Matrix result;
            Matrix.CreateOrthographicOffCenter(0.0f, viewport.Width, viewport.Height, 0.0f, 0.0f, -1f, out result);
            result.M41 += -0.5f * result.M11;
            result.M42 += -0.5f * result.M22;
            DuckGame.Graphics.bottomRight = new Vec2(32f, 32f);
            DuckGame.Graphics.bottomRight = Vec2.Transform(DuckGame.Graphics.bottomRight, result);
            DuckGame.Graphics.topLeft = new Vec2(0.0f, 0.0f);
            DuckGame.Graphics.topLeft = Vec2.Transform(DuckGame.Graphics.topLeft, result);
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

        public static void Draw(MTSpriteBatchItem item) => DuckGame.Graphics._currentBatch.DrawExistingBatchItem(item);

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
            if (DuckGame.Graphics.doSnap)
            {
                position.x = (float)Math.Round(position.x * (double)DuckGame.Graphics.snap) / DuckGame.Graphics.snap;
                position.y = (float)Math.Round(position.y * (double)DuckGame.Graphics.snap) / DuckGame.Graphics.snap;
            }
            if (effects == SpriteEffects.FlipHorizontally)
                origin.x = (sourceRectangle.HasValue ? sourceRectangle.Value.width : texture.w) - origin.x;
            float depth1 = DuckGame.Graphics.AdjustDepth(depth);
            if (DuckGame.Graphics.material != null)
                DuckGame.Graphics._currentBatch.DrawWithMaterial(texture, position, sourceRectangle, color, rotation, origin, scale, effects, depth1, DuckGame.Graphics.material);
            else
                DuckGame.Graphics._currentBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, depth1);
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
            DuckGame.Graphics.Draw(target, new Vec2(x, y), new Rectangle?(), Color.White, 0.0f, Vec2.Zero, new Vec2(xscale, yscale), SpriteEffects.None, depth);
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
            ++DuckGame.Graphics.currentDrawIndex;
            p1 = new Vec2(p1.x, p1.y);
            p2 = new Vec2(p2.x, p2.y);
            float rotation = (float)Math.Atan2(p2.y - (double)p1.y, p2.x - (double)p1.x);
            float length = (p1 - p2).length;
            DuckGame.Graphics.Draw(DuckGame.Graphics._blank, p1, new Rectangle?(), col, rotation, new Vec2(0.0f, 0.5f), new Vec2(length, width), SpriteEffects.None, depth);
        }

        public static void DrawDottedLine(
          Vec2 p1,
          Vec2 p2,
          Color col,
          float width = 1f,
          float dotLength = 8f,
          Depth depth = default(Depth))
        {
            ++DuckGame.Graphics.currentDrawIndex;
            Vec2 vec2_1 = p1;
            Vec2 vec2_2 = p2 - p1;
            float length = vec2_2.length;
            int num = (int)((double)length / (double)dotLength);
            vec2_2.Normalize();
            bool flag = false;
            for (int index = 0; index < num; ++index)
            {
                Vec2 vec2_3 = vec2_1 + vec2_2 * dotLength;
                if ((double)(vec2_3 - p1).length > (double)length)
                    vec2_3 = p2;
                if (!flag)
                    DuckGame.Graphics.DrawLine(new Vec2(vec2_1.x, vec2_1.y), new Vec2(vec2_3.x, vec2_3.y), col, width, depth);
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
                Vec2 vec2_2 = new Vec2((float)Math.Cos((double)rad) * radius, (float)-Math.Sin((double)rad) * radius);
                if (index > 0)
                    DuckGame.Graphics.DrawLine(pos + vec2_2, pos + vec2_1, col, width, depth);
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
            ++DuckGame.Graphics.currentDrawIndex;
            if (texture.width > 1)
            {
                p1 = new Vec2(p1.x, p1.y);
                p2 = new Vec2(p2.x, p2.y);
                float rotation = (float)Math.Atan2(p2.y - (double)p1.y, p2.x - (double)p1.x);
                float x = (p1 - p2).length / texture.width;
                DuckGame.Graphics.Draw(texture, p1, new Rectangle?(), col, rotation, new Vec2(0.0f, texture.height / 2), new Vec2(x, width), SpriteEffects.None, depth);
            }
            else
            {
                p1 = new Vec2(p1.x, p1.y);
                p2 = new Vec2(p2.x, p2.y);
                float rotation = (float)Math.Atan2(p2.y - (double)p1.y, p2.x - (double)p1.x);
                float length = (p1 - p2).length;
                DuckGame.Graphics.Draw(texture, p1, new Rectangle?(), col, rotation, new Vec2(0.0f, texture.height / 2), new Vec2(length, width), SpriteEffects.None, depth);
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
            ++DuckGame.Graphics.currentDrawIndex;
            if (filled)
            {
                DuckGame.Graphics.Draw(DuckGame.Graphics._blank2, p1, new Rectangle?(), col, 0.0f, Vec2.Zero, new Vec2((float)-(p1.x - (double)p2.x), (float)-(p1.y - (double)p2.y)), SpriteEffects.None, depth);
            }
            else
            {
                float num = borderWidth / 2f;
                DuckGame.Graphics.DrawLine(new Vec2(p1.x, p1.y + num), new Vec2(p2.x, p1.y + num), col, borderWidth, depth);
                DuckGame.Graphics.DrawLine(new Vec2(p1.x + num, p1.y + borderWidth), new Vec2(p1.x + num, p2.y - borderWidth), col, borderWidth, depth);
                DuckGame.Graphics.DrawLine(new Vec2(p2.x, p2.y - num), new Vec2(p1.x, p2.y - num), col, borderWidth, depth);
                DuckGame.Graphics.DrawLine(new Vec2(p2.x - num, p2.y - borderWidth), new Vec2(p2.x - num, p1.y + borderWidth), col, borderWidth, depth);
            }
        }

        public static void DrawRect(
          Rectangle r,
          Color col,
          Depth depth = default(Depth),
          bool filled = true,
          float borderWidth = 1f)
        {
            ++DuckGame.Graphics.currentDrawIndex;
            Vec2 position = new Vec2(r.Left, r.Top);
            Vec2 vec2 = new Vec2(r.Right, r.Bottom);
            if (filled)
            {
                DuckGame.Graphics.Draw(DuckGame.Graphics._blank2, position, new Rectangle?(), col, 0.0f, Vec2.Zero, new Vec2((float)-(position.x - (double)vec2.x), (float)-(position.y - (double)vec2.y)), SpriteEffects.None, depth);
            }
            else
            {
                float num = borderWidth / 2f;
                DuckGame.Graphics.DrawLine(new Vec2(position.x, position.y + num), new Vec2(vec2.x, position.y + num), col, borderWidth, depth);
                DuckGame.Graphics.DrawLine(new Vec2(position.x + num, position.y + borderWidth), new Vec2(position.x + num, vec2.y - borderWidth), col, borderWidth, depth);
                DuckGame.Graphics.DrawLine(new Vec2(vec2.x, vec2.y - num), new Vec2(position.x, vec2.y - num), col, borderWidth, depth);
                DuckGame.Graphics.DrawLine(new Vec2(vec2.x - num, vec2.y - borderWidth), new Vec2(vec2.x - num, position.y + borderWidth), col, borderWidth, depth);
            }
        }

        public static void DrawDottedRect(
          Vec2 p1,
          Vec2 p2,
          Color col,
          Depth depth = default(Depth),
          float borderWidth = 1f,
          float dotLength = 8f)
        {
            ++DuckGame.Graphics.currentDrawIndex;
            float num = borderWidth / 2f;
            DuckGame.Graphics.DrawDottedLine(new Vec2(p1.x, p1.y + num), new Vec2(p2.x, p1.y + num), col, borderWidth, dotLength, depth);
            DuckGame.Graphics.DrawDottedLine(new Vec2(p1.x + num, p1.y + borderWidth), new Vec2(p1.x + num, p2.y - borderWidth), col, borderWidth, dotLength, depth);
            DuckGame.Graphics.DrawDottedLine(new Vec2(p2.x, p2.y - num), new Vec2(p1.x, p2.y - num), col, borderWidth, dotLength, depth);
            DuckGame.Graphics.DrawDottedLine(new Vec2(p2.x - num, p2.y - borderWidth), new Vec2(p2.x - num, p1.y + borderWidth), col, borderWidth, dotLength, depth);
        }

        public static Tex2D Recolor(string sprite, Vec3 color) => DuckGame.Graphics.RecolorOld(Content.Load<Tex2D>(sprite), color);

        public static Tex2D Recolor(Tex2D sprite, Vec3 color)
        {
            Dictionary<Vec3, Tex2D> dictionary;
            if (DuckGame.Graphics._recolorMap.TryGetValue(sprite, out dictionary))
            {
                Tex2D tex2D;
                if (dictionary.TryGetValue(color, out tex2D))
                    return tex2D;
            }
            else
                DuckGame.Graphics._recolorMap[sprite] = new Dictionary<Vec3, Tex2D>();
            Material material = new MaterialRecolor(new Vec3(color.x / byte.MaxValue, color.y / byte.MaxValue, color.z / byte.MaxValue));
            RenderTarget2D t = new RenderTarget2D(sprite.w, sprite.h);
            DuckGame.Graphics.SetRenderTarget(t);
            DuckGame.Graphics.Clear(new Color(0, 0, 0, 0));
            material.Apply();
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, material.effect, Matrix.Identity);
            DuckGame.Graphics.Draw(sprite, new Vec2(), new Rectangle?(), Color.White, 0.0f, new Vec2(), new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.5f);
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.device.SetRenderTarget(null);
            Tex2D tex2D1 = new Tex2D(sprite.w, sprite.h);
            tex2D1.SetData(t.GetData());
            tex2D1.AssignTextureName("RESKIN");
            t.Dispose();
            DuckGame.Graphics._recolorMap[sprite][color] = tex2D1;
            return tex2D1;
        }

        public static Tex2D RecolorOld(Tex2D sprite, Vec3 color)
        {
            Material material = new MaterialRecolor(new Vec3(color.x / byte.MaxValue, color.y / byte.MaxValue, color.z / byte.MaxValue));
            RenderTarget2D t = new RenderTarget2D(sprite.w, sprite.h);
            DuckGame.Graphics.SetRenderTarget(t);
            DuckGame.Graphics.Clear(new Color(0, 0, 0, 0));
            material.Apply();
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, material.effect, Matrix.Identity);
            DuckGame.Graphics.Draw(sprite, new Vec2(), new Rectangle?(), Color.White, 0.0f, new Vec2(), new Vec2(1f, 1f), SpriteEffects.None, (Depth)0.5f);
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.device.SetRenderTarget(null);
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
            Color color4 = new Color(161, 146, 130);
            Color color5 = new Color(128, 113, 96);
            Color color6 = new Color(191, 181, 171);
            Color color7 = new Color(236, 89, 60);
            Color color8 = new Color(248, 131, 99);
            Color color9 = new Color(235, 137, 49);
            Color color10 = new Color(247, 224, 90);
            Color color11 = new Color(219, 88, 31);
            Color color12 = new Color(236, 116, 60);
            Color color13 = new Color(164, 100, 34);
            Color color14 = new Color(235, 137, 49);
            Color[] data = sprite.GetData();
            for (int index = 0; index < data.Length; ++index)
            {
                if (data[index] == color4)
                    data[index] = color1;
                else if (data[index] == color5)
                    data[index] = color2;
                else if (data[index] == color6)
                    data[index] = color3;
                else if (data[index] == color7)
                    data[index] = color9;
                else if (data[index] == color8)
                    data[index] = color10;
                else if (data[index] == color11)
                    data[index] = color13;
                else if (data[index] == color12)
                    data[index] = color14;
            }
            Tex2D tex2D = new Tex2D(sprite.w, sprite.h);
            tex2D.SetData(data);
            return tex2D;
        }

        public static float baseDeviceWidth => DuckGame.Graphics._baseDeviceWidth;

        public static float baseDeviceHeight => DuckGame.Graphics._baseDeviceHeight;

        public static bool fixedAspect
        {
            get
            {
                if ((double)Resolution.current.aspect > 1.79999995231628)
                    return true;
                return !(Level.current is XMLLevel) && !(Level.current is Editor);
            }
        }

        public static float aspect => 9f / 16f;

        public static bool sixteenTen => (double)DuckGame.Graphics.aspect > 0.569999992847443;

        public static float barSize => (float)((width * (double)DuckGame.Graphics.aspect - width * (9.0 / 16.0)) / 2.0);

        public static void InitializeBase(GraphicsDeviceManager m, int widthVal, int heightVal)
        {
            DuckGame.Graphics._manager = m;
            DuckGame.Graphics._width = widthVal;
            DuckGame.Graphics._baseDeviceWidth = _width;
            DuckGame.Graphics._height = heightVal;
            DuckGame.Graphics._baseDeviceHeight = _height;
        }

        public static void Initialize(GraphicsDevice d)
        {
            DuckGame.Graphics._base = d;
            DuckGame.Graphics._defaultBatch = new MTSpriteBatch(DuckGame.Graphics._base);
            DuckGame.Graphics.screen = DuckGame.Graphics._defaultBatch;
            DuckGame.Graphics._blank = new Tex2D(1, 1);
            DuckGame.Graphics._blank.SetData(new Color[1]
            {
        Color.White
            });
            DuckGame.Graphics._blank2 = new Tex2D(1, 1);
            DuckGame.Graphics._blank2.SetData(new Color[1]
            {
        Color.White
            });
            DuckGame.Graphics._biosFont = new BitmapFont("biosFont", 8);
            DuckGame.Graphics._biosFontCaseSensitive = new BitmapFont("biosFontCaseSensitive", 8);
            DuckGame.Graphics._fancyBiosFont = new FancyBitmapFont("smallFont");
            DuckGame.Graphics._passwordFont = new SpriteMap("passwordFont", 8, 8);
            Viewport viewport = d.Viewport;
            double width = viewport.Width;
            viewport = d.Viewport;
            double height = viewport.Height;
            ref Matrix local = ref DuckGame.Graphics._projectionMatrix;
            Matrix.CreateOrthographicOffCenter(0.0f, (float)width, (float)height, 0.0f, 0.0f, 1f, out local);
            DuckGame.Graphics._projectionMatrix.M41 += -0.5f * DuckGame.Graphics._projectionMatrix.M11;
            DuckGame.Graphics._projectionMatrix.M42 += -0.5f * DuckGame.Graphics._projectionMatrix.M22;
            DuckGame.Graphics.tounge = new Sprite("tounge");
        }

        public static RenderTarget2D currentRenderTarget => DuckGame.Graphics._currentRenderTarget;

        public static RenderTarget2D defaultRenderTarget
        {
            get
            {
                if (DuckGame.Graphics._settingScreenTarget)
                    return null;
                return DuckGame.Graphics._defaultRenderTarget == null ? DuckGame.Graphics._screenBufferTarget : DuckGame.Graphics._defaultRenderTarget;
            }
            set => DuckGame.Graphics._defaultRenderTarget = value;
        }

        public static void SetRenderTargetToScreen()
        {
            DuckGame.Graphics._settingScreenTarget = true;
            DuckGame.Graphics.SetRenderTarget(null);
            DuckGame.Graphics._settingScreenTarget = false;
        }

        public static void SetRenderTarget(RenderTarget2D t)
        {
            if (t != null && t.IsDisposed)
                return;
            if (t == null)
            {
                Microsoft.Xna.Framework.Graphics.RenderTarget2D renderTarget = DuckGame.Graphics.defaultRenderTarget != null ? DuckGame.Graphics.defaultRenderTarget.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D : null;
                if (renderTarget == null)
                {
                    DuckGame.Graphics._currentTargetSize.width = Resolution.current.x;
                    DuckGame.Graphics._currentTargetSize.height = Resolution.current.y;
                }
                else
                {
                    DuckGame.Graphics._currentTargetSize.width = renderTarget.Width;
                    DuckGame.Graphics._currentTargetSize.height = renderTarget.Height;
                }
                DuckGame.Graphics.device.SetRenderTarget(renderTarget);
                if (!DuckGame.Graphics._settingScreenTarget && DuckGame.Graphics._defaultRenderTarget == null)
                    DuckGame.Graphics.UpdateScreenViewport();
            }
            else
            {
                DuckGame.Graphics.device.SetRenderTarget(t.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D);
                DuckGame.Graphics._currentTargetSize.width = t.width;
                DuckGame.Graphics._currentTargetSize.height = t.height;
            }
            DuckGame.Graphics._lastViewport = DuckGame.Graphics.device.Viewport;
            DuckGame.Graphics._currentRenderTarget = t;
        }

        public static RenderTarget2D GetRenderTarget() => DuckGame.Graphics._currentRenderTarget;

        public static void SetFullViewport()
        {
            DuckGame.Graphics._oldViewport = DuckGame.Graphics.device.Viewport;
            DuckGame.Graphics.Internal_ViewportSet(new Viewport()
            {
                X = 0,
                Y = 0,
                Width = (int)DuckGame.Graphics._currentTargetSize.width,
                Height = (int)DuckGame.Graphics._currentTargetSize.height
            });
        }

        public static void RestoreOldViewport() => DuckGame.Graphics.Internal_ViewportSet(DuckGame.Graphics._oldViewport);

        private static void Internal_ViewportSet(Viewport pViewport)
        {
            try
            {
                DuckGame.Graphics.device.Viewport = pViewport;
            }
            catch (Exception)
            {
                DevConsole.Log("Error: Invalid Viewport (x = " + pViewport.X.ToString() + ", y = " + pViewport.Y.ToString() + ", w = " + pViewport.Width.ToString() + ", h = " + pViewport.Height.ToString() + ", minDepth = " + pViewport.MinDepth.ToString() + ", maxDepth = " + pViewport.MaxDepth.ToString() + ")");
            }
        }

        public static void UpdateScreenViewport(bool pForceReset = false)
        {
            try
            {
                if (pForceReset || !DuckGame.Graphics._screenViewport.HasValue)
                {
                    Viewport viewport = new Viewport();
                    if ((double)DuckGame.Graphics._currentTargetSize.aspect < 1.76999998092651)
                    {
                        viewport.Width = (int)DuckGame.Graphics._currentTargetSize.width;
                        viewport.Height = Math.Min((int)Math.Round(_currentTargetSize.width / 1.77777004241943), (int)DuckGame.Graphics._currentTargetSize.height);
                    }
                    else
                    {
                        viewport.Height = (int)DuckGame.Graphics._currentTargetSize.height;
                        viewport.Width = Math.Min((int)Math.Round(_currentTargetSize.height * 1.77777004241943), (int)DuckGame.Graphics._currentTargetSize.width);
                    }
                    viewport.X = Math.Max((int)((_currentTargetSize.width - (double)viewport.Width) / 2.0), 0);
                    viewport.Y = Math.Max((int)((_currentTargetSize.height - (double)viewport.Height) / 2.0), 0);
                    viewport.MinDepth = 0.0f;
                    viewport.MaxDepth = 1f;
                    DuckGame.Graphics._screenViewport = new Viewport?(viewport);
                }
                DuckGame.Graphics.Internal_ViewportSet(DuckGame.Graphics._screenViewport.Value);
            }
            catch (Exception)
            {
            }
            DuckGame.Graphics._lastViewport = DuckGame.Graphics.device.Viewport;
        }

        public static void SetScreenTargetViewport()
        {
            Viewport pViewport = new Viewport();
            if ((double)Resolution.adapterResolution.aspect < (double)Resolution.current.aspect)
            {
                pViewport.Width = Resolution.adapterResolution.x;
                pViewport.Height = Math.Min((int)Math.Round(Resolution.adapterResolution.x / (double)Resolution.current.aspect), Resolution.adapterResolution.y);
            }
            else
            {
                pViewport.Height = Resolution.adapterResolution.y;
                pViewport.Width = Math.Min((int)Math.Round(Resolution.adapterResolution.y * (double)Resolution.current.aspect), Resolution.adapterResolution.x);
            }
            pViewport.X = Math.Max((Resolution.adapterResolution.x - pViewport.Width) / 2, 0);
            pViewport.Y = Math.Max((Resolution.adapterResolution.y - pViewport.Height) / 2, 0);
            pViewport.MinDepth = 0.0f;
            pViewport.MaxDepth = 1f;
            DuckGame.Graphics.Internal_ViewportSet(pViewport);
        }

        public static Viewport viewport
        {
            get => DuckGame.Graphics.device == null || DuckGame.Graphics.device.IsDisposed ? DuckGame.Graphics._lastViewport : DuckGame.Graphics.device.Viewport;
            set
            {
                if (!DuckGame.Graphics._lastViewportSet)
                {
                    DuckGame.Graphics._lastViewport = value;
                    DuckGame.Graphics._lastViewportSet = true;
                }
                Viewport viewport = DuckGame.Graphics.device.Viewport;
                if (viewport.Width != DuckGame.Graphics._lastViewport.Width)
                    return;
                viewport = DuckGame.Graphics.device.Viewport;
                if (viewport.Height != DuckGame.Graphics._lastViewport.Height)
                    return;
                Rectangle bounds = (Rectangle)value.Bounds;
                if (DuckGame.Graphics._currentRenderTarget != null)
                    DuckGame.Graphics.ClipRectangle(bounds, new Rectangle(0.0f, 0.0f, _currentRenderTarget.width, _currentRenderTarget.height));
                else
                    DuckGame.Graphics.ClipRectangle(bounds, (Rectangle)DuckGame.Graphics.device.PresentationParameters.Bounds);
                value.X = (int)bounds.x;
                value.Y = (int)bounds.y;
                value.Width = (int)bounds.width;
                value.Height = (int)bounds.height;
                DuckGame.Graphics.Internal_ViewportSet(value);
                DuckGame.Graphics._lastViewport = value;
            }
        }

        public static Rectangle GetScissorRectangle() => new Rectangle(device.ScissorRectangle.X, device.ScissorRectangle.Y, device.ScissorRectangle.Width, device.ScissorRectangle.Height);

        public static void SetScissorRectangle(Rectangle r)
        {
            float num = device.Viewport.Bounds.Width / (float)DuckGame.Graphics.width;
            if (r.width < 0.0 || r.height < 0.0)
                return;
            r.width *= num;
            r.height *= num;
            r.x *= num;
            r.y *= num;
            r.x += viewport.X;
            r.y += viewport.Y;
            DuckGame.Graphics.device.ScissorRectangle = (Microsoft.Xna.Framework.Rectangle)DuckGame.Graphics.ClipRectangle(r, (Rectangle)DuckGame.Graphics.device.Viewport.Bounds);
        }

        public static void PushLayerScissor(Rectangle pRect)
        {
            if (DuckGame.Graphics.screen != null)
                DuckGame.Graphics.screen.FlushSettingScissor();
            DuckGame.Graphics._scissorStack.Push(pRect);
            float num1 = width / DuckGame.Graphics.currentLayer.width;
            float num2 = height / DuckGame.Graphics.currentLayer.height;
            pRect.x *= num1;
            pRect.y *= num2;
            pRect.width *= num1;
            pRect.height *= num2;
            DuckGame.Graphics.SetScissorRectangle(pRect);
        }

        public static void PopLayerScissor()
        {
            if (DuckGame.Graphics.screen != null)
                DuckGame.Graphics.screen.FlushAndClearScissor();
            DuckGame.Graphics._scissorStack.Pop();
            if (DuckGame.Graphics._scissorStack.Count == 0)
                DuckGame.Graphics.SetScissorRectangle(new Rectangle(0.0f, 0.0f, width, height));
            else
                DuckGame.Graphics.SetScissorRectangle(DuckGame.Graphics._scissorStack.Peek());
        }

        public static Rectangle ClipRectangle(Rectangle r, Rectangle clipTo)
        {
            if (r.x > (double)clipTo.Right)
                r.x = clipTo.Right - r.width;
            if (r.y > (double)clipTo.Bottom)
                r.y = clipTo.Bottom - r.height;
            if (r.x < (double)clipTo.Left)
                r.x = clipTo.Left;
            if (r.y < (double)clipTo.Top)
                r.y = clipTo.Top;
            if (r.x < 0.0)
                r.x = 0.0f;
            if (r.y < 0.0)
                r.y = 0.0f;
            if (r.x + (double)r.width > clipTo.x + (double)clipTo.width)
                r.width = clipTo.Right - r.x;
            if (r.y + (double)r.height > clipTo.y + (double)clipTo.height)
                r.height = clipTo.Bottom - r.y;
            if (r.width < 0.0)
                r.width = 0.0f;
            if (r.height < 0.0)
                r.height = 0.0f;
            return r;
        }

        public static void Clear(Color c) => DuckGame.Graphics.device.Clear((Microsoft.Xna.Framework.Color)c);

        public static void PushMarker(string s)
        {
        }

        public static void PopMarker()
        {
        }
    }
}
