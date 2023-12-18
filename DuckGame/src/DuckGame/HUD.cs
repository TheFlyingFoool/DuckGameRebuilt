using System;

namespace DuckGame
{
    public class HUD
    {
        private static HUDCore _core = new HUDCore();
        private const int CornerDisplayNextLineSeparation = 12;

        public static HUDCore core
        {
            get => _core;
            set => _core = value;
        }

        public static bool hide
        {
            get => _core._hide;
            set => _core._hide = value;
        }

        public static CornerDisplay FindDuplicateActiveCorner(
          HUDCorner corner,
          string text,
          bool allowStacking = false)
        {
            foreach (CornerDisplay cornerDisplay in _core._cornerDisplays)
            {
                if (cornerDisplay.corner == corner)
                {
                    if (cornerDisplay.text == text)
                    {
                        if (!cornerDisplay.closing)
                            return cornerDisplay;
                        break;
                    }
                    break;
                }
            }
            if (!allowStacking)
                CloseCorner(corner);
            return null;
        }

        public static CornerDisplay AddCornerMessage(HUDCorner corner, string text) => AddCornerMessage(corner, text, false);

        public static CornerDisplay AddCornerMessage(
          HUDCorner corner,
          string text,
          bool allowStacking)
        {
            CornerDisplay cornerDisplay1 = FindDuplicateActiveCorner(corner, text, allowStacking);
            if (cornerDisplay1 == null)
            {
                cornerDisplay1 = new CornerDisplay
                {
                    corner = corner,
                    text = text
                };
                _core._cornerDisplays.Add(cornerDisplay1);
            }
            if (!allowStacking)
            {
                foreach (CornerDisplay cornerDisplay2 in _core._cornerDisplays)
                {
                    if (cornerDisplay2.corner == corner && cornerDisplay2 != cornerDisplay1)
                        cornerDisplay2.closing = true;
                }
            }
            return cornerDisplay1;
        }

        public static CornerDisplay AddCornerControl(
          HUDCorner corner,
          string text,
          InputProfile pro)
        {
            return AddCornerControl(corner, text, pro, false);
        }

        public static CornerDisplay AddCornerControl(
          HUDCorner corner,
          string text,
          InputProfile pro = null,
          bool allowStacking = false)
        {
            CornerDisplay cornerDisplay = FindDuplicateActiveCorner(corner, text, allowStacking);
            if (cornerDisplay == null)
            {
                cornerDisplay = new CornerDisplay
                {
                    corner = corner,
                    text = text,
                    isControl = true,
                    profile = pro
                };
                _core._cornerDisplays.Add(cornerDisplay);
            }
            return cornerDisplay;
        }

        public static void AddInputChangeDisplay(string text)
        {
            _core._inputChangeDisplays.Clear();
            _core._inputChangeDisplays.Add(new CornerDisplay()
            {
                text = text,
                isControl = true,
                life = 3f
            });
        }

        public static void AddPlayerChangeDisplay(string text) => AddPlayerChangeDisplay(text, 4f);

        public static void AddPlayerChangeDisplay(string text, float life)
        {
            _core._playerChangeDisplays.Clear();
            _core._playerChangeDisplays.Add(new CornerDisplay()
            {
                text = text,
                isControl = true,
                life = life
            });
        }

        public static void AddCornerTimer(HUDCorner corner, string text, Timer timer) => _core._cornerDisplays.Add(new CornerDisplay()
        {
            corner = corner,
            text = text,
            timer = timer
        });

        public static void AddCornerCounter(
          HUDCorner corner,
          string text,
          FieldBinding counter,
          int max = 0,
          bool animateCount = false)
        {
            _core._cornerDisplays.Add(new CornerDisplay()
            {
                corner = corner,
                text = text,
                counter = counter,
                maxCount = max,
                animateCount = animateCount,
                curCount = (int)counter.value,
                realCount = (int)counter.value
            });
        }

        public static void ClearPlayerChangeDisplays() => _core._playerChangeDisplays.Clear();

        public static void CloseAllCorners()
        {
            foreach (CornerDisplay cornerDisplay in _core._cornerDisplays)
                cornerDisplay.closing = true;
        }
        public static void CloseAllCorners(bool notarcade)
        {
            foreach (CornerDisplay cornerDisplay in _core._cornerDisplays)
            {
                if (cornerDisplay.ischallenge)
                {
                    continue;
                }
                cornerDisplay.closing = true;
            }

        }

        public static void CloseCorner(HUDCorner corner)
        {
            foreach (CornerDisplay cornerDisplay in _core._cornerDisplays)
            {
                if (cornerDisplay.corner == corner)
                    cornerDisplay.closing = true;
            }
        }

        public static void CloseInputChangeDisplays()
        {
            foreach (CornerDisplay inputChangeDisplay in _core._inputChangeDisplays)
                inputChangeDisplay.closing = true;
        }

        public static void ClearCorners() => _core._cornerDisplays.Clear();

        public static void Update()
        {
            for (int index = 0; index < _core._inputChangeDisplays.Count; ++index)
            {
                CornerDisplay inputChangeDisplay = _core._inputChangeDisplays[index];
                if (inputChangeDisplay.closing)
                {
                    inputChangeDisplay.slide = Lerp.FloatSmooth(inputChangeDisplay.slide, -0.3f, 0.15f);
                    if (inputChangeDisplay.slide < -0.15f)
                    {
                        _core._inputChangeDisplays.RemoveAt(index);
                        --index;
                    }
                }
                else
                {
                    inputChangeDisplay.life -= Maths.IncFrameTimer();
                    inputChangeDisplay.slide = Lerp.FloatSmooth(inputChangeDisplay.slide, 1f, 0.15f, 1.2f);
                    if (inputChangeDisplay.life <= 0f)
                        inputChangeDisplay.closing = true;
                }
            }
            for (int index = 0; index < _core._playerChangeDisplays.Count; ++index)
            {
                CornerDisplay playerChangeDisplay = _core._playerChangeDisplays[index];
                if (playerChangeDisplay.closing)
                {
                    playerChangeDisplay.slide = Lerp.FloatSmooth(playerChangeDisplay.slide, -0.3f, 0.15f);
                    if (playerChangeDisplay.slide < -0.15f)
                    {
                        _core._playerChangeDisplays.RemoveAt(index);
                        --index;
                    }
                }
                else
                {
                    playerChangeDisplay.life -= Maths.IncFrameTimer();
                    playerChangeDisplay.slide = Lerp.FloatSmooth(playerChangeDisplay.slide, 1f, 0.15f, 1.2f);
                    if (playerChangeDisplay.life <= 0f)
                        playerChangeDisplay.closing = true;
                }
            }
            for (int index = 0; index < _core._cornerDisplays.Count; ++index)
            {
                CornerDisplay d = _core._cornerDisplays[index];
                if (d.closing)
                {
                    d.slide = Lerp.FloatSmooth(d.slide, -0.3f, 0.15f);
                    if (d.slide < -0.15f)
                    {
                        _core._cornerDisplays.RemoveAt(index);
                        --index;
                    }
                }
                else
                {
                    if (d.willDie)
                    {
                        d.life -= Maths.IncFrameTimer();
                        if (d.life <= 0f)
                            d.closing = true;
                    }
                    if (!_core._cornerDisplays.Exists(v => v.corner == d.corner && v.closing))
                    {
                        if (d.counter != null)
                        {
                            if (d.addCount != 0)
                            {
                                d.addCountWait -= 0.05f;
                                if (d.addCountWait <= 0f)
                                {
                                    d.addCountWait = 0.05f;
                                    if (d.addCount > 0)
                                    {
                                        --d.addCount;
                                        ++d.curCount;
                                    }
                                    else if (d.addCount < 0)
                                    {
                                        ++d.addCount;
                                        --d.curCount;
                                    }
                                    SFX.Play("tinyTick", 0.6f, 0.3f);
                                }
                            }
                            int num = (int)d.counter.value;
                            if (num != d.realCount)
                            {
                                if (d.animateCount)
                                {
                                    d.addCountWait = 1f;
                                    d.addCount = num - d.realCount;
                                    d.curCount = d.realCount;
                                    d.realCount = num;
                                }
                                else
                                {
                                    d.realCount = num;
                                    d.curCount = num;
                                }
                            }
                        }
                        if (d.timer != null && d.timer.maxTime.TotalSeconds != 0f && (int)(d.timer.maxTime - d.timer.elapsed).TotalSeconds == d.lowTimeTick)
                        {
                            --d.lowTimeTick;
                            SFX.Play("cameraBeep", 0.8f);
                        }
                        d.slide = Lerp.FloatSmooth(d.slide, 1f, 0.15f, 1.2f);
                    }
                }
            }
        }

        public static void DrawForeground()
        {
            if (DevConsole.debugOrigin)
            {
                Graphics.DrawLine(new Vec2(0f, -32f), new Vec2(0f, 32f), Color.Orange);
                Graphics.DrawLine(new Vec2(-32f, 0f), new Vec2(32f, 0f), Color.Orange);
                Graphics.DrawRect(new Vec2(-2f, -2f), new Vec2(2f, 2f), Color.Red);
            }
            if (!DevConsole.debugBounds || Level.current == null)
                return;
            Graphics.DrawLine(Level.current.topLeft, new Vec2(Level.current.bottomRight.x, Level.current.topLeft.y), Color.Green);
            Graphics.DrawLine(Level.current.topLeft, new Vec2(Level.current.topLeft.x, Level.current.bottomRight.y), Color.Green);
            Graphics.DrawLine(Level.current.bottomRight, new Vec2(Level.current.topLeft.x, Level.current.bottomRight.y), Color.Green);
            Graphics.DrawLine(Level.current.bottomRight, new Vec2(Level.current.bottomRight.x, Level.current.topLeft.y), Color.Green);
        }

        public static void Draw()
        {
            if (_core._hide)
                return;
            foreach (CornerDisplay inputChangeDisplay in _core._inputChangeDisplays)
            {
                Vec2 vec2_1 = new Vec2(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height);
                string text = inputChangeDisplay.text ?? "";
                float stringWidth = Graphics.GetStringWidth(text);
                float x = stringWidth;
                float stringHeight = Graphics.GetStringHeight(text);
                float num1 = stringHeight + 4f;
                float num2 = 0f;
                Vec2 vec2_2 = vec2_1;
                Vec2 vec2_3 = vec2_1;
                vec2_2.x -= stringWidth / 2f;
                vec2_3.x -= stringWidth / 2f;
                float num3 = Layer.HUD.camera.width / 32f + num1;
                Vec2 vec2_4 = Vec2.Zero;
                vec2_4 = new Vec2(0f, -num3);
                Graphics.DrawRect(vec2_3 + vec2_4 * inputChangeDisplay.slide, vec2_3 + new Vec2(x, num1 - 1f) + vec2_4 * inputChangeDisplay.slide, Color.Black, (Depth)0.95f);
                Graphics.DrawString(text, vec2_2 + new Vec2(((x - stringWidth) / 2f), ((num1 - stringHeight) / 2f) + num2) + vec2_4 * inputChangeDisplay.slide, Color.White, (Depth)0.97f, inputChangeDisplay.profile);
            }
            foreach (CornerDisplay playerChangeDisplay in _core._playerChangeDisplays)
            {
                Vec2 vec2_5 = new Vec2(Layer.HUD.camera.width / 2f, 0f);
                string text = playerChangeDisplay.text ?? "";
                float stringWidth = Graphics.GetStringWidth(text);
                float x = stringWidth;
                float stringHeight = Graphics.GetStringHeight(text);
                float num4 = stringHeight + 4f;
                float num5 = 0f;
                Vec2 vec2_6 = vec2_5;
                Vec2 vec2_7 = vec2_5;
                vec2_6.x -= stringWidth / 2f;
                vec2_7.x -= stringWidth / 2f;
                float y = Layer.HUD.camera.width / 32f + num4;
                Vec2 vec2_8 = Vec2.Zero;
                vec2_8 = new Vec2(0f, y);
                Graphics.DrawRect(vec2_7 + vec2_8 * playerChangeDisplay.slide, vec2_7 + new Vec2(x, num4 - 1f) + vec2_8 * playerChangeDisplay.slide, Color.Black, (Depth)0.95f);
                Graphics.DrawString(text, vec2_6 + new Vec2(((x - stringWidth) / 2f), ((num4 - stringHeight) / 2f) + num5) + vec2_8 * playerChangeDisplay.slide, Color.White, (Depth)0.97f, playerChangeDisplay.profile);
            }
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            int num11 = 0;
            foreach (CornerDisplay cornerDisplay in _core._cornerDisplays)
            {
                Vec2 vec2_9 = new Vec2(0f, 0f);
                switch (cornerDisplay.corner)
                {
                    case HUDCorner.TopLeft:
                        vec2_9 = new Vec2(0f, num6 * 12);
                        ++num6;
                        break;
                    case HUDCorner.TopRight:
                        vec2_9 = new Vec2(Layer.HUD.camera.width, num7 * 12);
                        ++num7;
                        break;
                    case HUDCorner.BottomLeft:
                        vec2_9 = new Vec2(0f, Layer.HUD.camera.height - num8 * 12);
                        ++num8;
                        break;
                    case HUDCorner.BottomRight:
                        vec2_9 = new Vec2(Layer.HUD.camera.width, Layer.HUD.camera.height - num9 * 12);
                        ++num9;
                        break;
                    case HUDCorner.BottomMiddle:
                        vec2_9 = new Vec2(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height - num10 * 12);
                        ++num10;
                        break;
                    case HUDCorner.TopMiddle:
                        vec2_9 = new Vec2(Layer.HUD.camera.width / 2f, num11 * 12);
                        ++num11;
                        break;
                }
                string text = cornerDisplay.text ?? "";
                bool flag = false;
                if (cornerDisplay.timer != null)
                {
                    if (cornerDisplay.timer.maxTime.TotalSeconds != 0f)
                    {
                        TimeSpan span = cornerDisplay.timer.maxTime - cornerDisplay.timer.elapsed;
                        text = text + cornerDisplay.text + MonoMain.TimeString(span, small: true);
                        if (span.TotalSeconds < 10f)
                            flag = true;
                    }
                    else
                        text = text + cornerDisplay.text + MonoMain.TimeString(cornerDisplay.timer.elapsed, small: true);
                }
                else if (cornerDisplay.counter != null && cornerDisplay.counter.value is int)
                {
                    int curCount = cornerDisplay.curCount;
                    if (cornerDisplay.addCount != 0)
                    {
                        text += Convert.ToString(curCount);
                        if (cornerDisplay.addCount > 0)
                            text = text + " |GREEN|+" + Convert.ToString(cornerDisplay.addCount);
                        else if (cornerDisplay.addCount < 0)
                            text = text + " |RED|" + Convert.ToString(cornerDisplay.addCount);
                    }
                    else
                        text = cornerDisplay.maxCount == 0 ? text + Convert.ToString(curCount) : text + Convert.ToString(curCount) + "/" + Convert.ToString(cornerDisplay.maxCount);
                }
                float stringWidth1 = Graphics.GetStringWidth(text);
                double stringWidth2 = Graphics.GetStringWidth(text, cornerDisplay.isControl);
                float num12 = stringWidth1 + 8f;
                float x = (float)(stringWidth2 + 8f);
                float stringHeight = Graphics.GetStringHeight(text);
                float num13 = stringHeight + 4f;
                Vec2 vec2_10 = vec2_9;
                Vec2 vec2_11 = vec2_9;
                if (cornerDisplay.corner == HUDCorner.TopRight || cornerDisplay.corner == HUDCorner.BottomRight)
                {
                    vec2_10.x -= num12 * cornerDisplay.slide;
                    vec2_11.x -= x * cornerDisplay.slide;
                }
                else if (cornerDisplay.corner == HUDCorner.TopLeft || cornerDisplay.corner == HUDCorner.BottomLeft)
                {
                    vec2_10.x -= num12 * (1f - cornerDisplay.slide);
                    vec2_11.x -= x * (1f - cornerDisplay.slide);
                    vec2_11.x += num12 - x;
                }
                if (cornerDisplay.corner == HUDCorner.BottomLeft || cornerDisplay.corner == HUDCorner.BottomRight || cornerDisplay.corner == HUDCorner.BottomMiddle)
                {
                    vec2_10.y -= num13;
                    vec2_11.y -= num13;
                }
                if (cornerDisplay.corner == HUDCorner.BottomMiddle || cornerDisplay.corner == HUDCorner.TopMiddle)
                {
                    vec2_10.x -= num12 / 2f;
                    vec2_11.x -= num12 / 2f;
                    vec2_11.x += num12 - x;
                }
                if (cornerDisplay.corner == HUDCorner.BottomMiddle)
                {
                    vec2_10.y += 24f * (1f - cornerDisplay.slide);
                    vec2_11.y += 24f * (1f - cornerDisplay.slide);
                }
                float num14 = Layer.HUD.camera.width / 32f;
                Vec2 vec2_12 = Vec2.Zero;
                if (cornerDisplay.corner == HUDCorner.TopLeft)
                    vec2_12 = new Vec2(num14, num14);
                else if (cornerDisplay.corner == HUDCorner.TopRight)
                    vec2_12 = new Vec2(-num14, num14);
                else if (cornerDisplay.corner == HUDCorner.BottomLeft)
                    vec2_12 = new Vec2(num14, -num14);
                else if (cornerDisplay.corner == HUDCorner.BottomRight)
                    vec2_12 = new Vec2(-num14, -num14);
                else if (cornerDisplay.corner == HUDCorner.BottomMiddle)
                    vec2_12 = new Vec2(0f, -num14);
                else if (cornerDisplay.corner == HUDCorner.TopMiddle)
                    vec2_12 = new Vec2(0f, num14);
                Graphics.DrawRect(vec2_11 + vec2_12 * cornerDisplay.slide, vec2_11 + new Vec2(x, num13 - 1f) + vec2_12 * cornerDisplay.slide, Color.Black, (Depth)0.95f);
                Graphics.DrawRect(vec2_11 + new Vec2(x, 1f) + vec2_12 * cornerDisplay.slide, vec2_11 + new Vec2(x + 1f, num13 - 2f) + vec2_12 * cornerDisplay.slide, Color.Black, (Depth)0.95f);
                Graphics.DrawRect(vec2_11 + new Vec2(0f, 1f) + vec2_12 * cornerDisplay.slide, vec2_11 + new Vec2(-1f, num13 - 2f) + vec2_12 * cornerDisplay.slide, Color.Black, (Depth)0.95f);
                Graphics.DrawString(text, vec2_10 + new Vec2((float)((num12 - stringWidth1) / 2f), (float)((num13 - stringHeight) / 2f)) + vec2_12 * cornerDisplay.slide, flag ? Color.Red : Color.White, (Depth)0.98f, cornerDisplay.profile);
            }
            if (!(Level.current is ChallengeLevel))
                return;
            foreach (TargetDuck targetDuck in Level.current.things[typeof(TargetDuck)])
                targetDuck.DrawIcon();
        }
    }
}
