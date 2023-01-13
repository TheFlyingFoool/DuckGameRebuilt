// Decompiled with JetBrains decompiler
// Type: DuckGame.Recording
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DuckGame
{
    public class Recording
    {
        private static int kNumFrames = 300;
        protected RecorderFrame[] _frames = new RecorderFrame[kNumFrames];
        protected int _frame;
        private int _startFrame;
        private int _endFrame;
        private bool _rolledOver;
        private float _highlightScore;
        private static FrameAnalytics _analytics = new FrameAnalytics();

        public int frame
        {
            get => _frame;
            set => _frame = value % kNumFrames;
        }

        public int startFrame => _startFrame;

        public int endFrame => _endFrame;

        public bool finished => _frame == _endFrame;

        public float highlightScore
        {
            get => _highlightScore;
            set => _highlightScore = value;
        }

        public Recording() => Initialize();

        public void Initialize()
        {
            for (int index = 0; index < _frames.Length; ++index)
                _frames[index].Initialize();
        }

        public void Reset()
        {
            _frame = 0;
            _startFrame = 0;
            _rolledOver = false;
            _highlightScore = 0f;
            _endFrame = 0;
        }

        public float GetFrameVelocity() => _frames[_frame].totalVelocity * 0.06f;

        public float GetFrameCoolness() => _frames[_frame].coolness;

        public int GetFrame(int f)
        {
            if (f < 0)
                f += kNumFrames - 1;
            else if (f >= kNumFrames)
                f -= kNumFrames;
            return f;
        }

        public float GetFrameAction() => _frames[_frame].actions;

        public float GetFrameBonus() => _frames[_frame].bonus;

        public float GetFrameTotal()
        {
            FrameAnalytics analytics = GetAnalytics(_analytics);
            return 0f + analytics.deaths + analytics.coolness + analytics.bonus + analytics.actions + analytics.totalVelocity;
        }

        public void Rewind() => _frame = _startFrame;

        public virtual void RenderFrame() => _frames[_frame].Render();

        public virtual void RenderFrame(float timeLag) => _frames[GetFrame(_frame - (int)(timeLag / Maths.IncFrameTimer()))].Render();

        public void UpdateFrame() => _frames[_frame].Update();

        public virtual void IncrementFrame(float speed = 1f) => _frame = (_frame + 1) % kNumFrames;

        public virtual void NextFrame()
        {
            ++_frame;
            if (_frame >= kNumFrames)
            {
                _rolledOver = true;
                _frame = 0;
            }
            _frames[_frame].Reset();
            _frames[_frame].actions += (byte)Math.Max(_frames[GetFrame(_frame - 1)].actions - 1, 0);
            _frames[_frame].bonus += (byte)Math.Max(_frames[GetFrame(_frame - 1)].bonus - 1, 0);
            _frames[_frame].coolness += (byte)Math.Max(_frames[GetFrame(_frame - 1)].coolness - 1, 0);
            _endFrame = _frame;
            if (!_rolledOver)
                return;
            _startFrame = (_frame + 1) % kNumFrames;
        }

        public bool StepForward()
        {
            _frame = (_frame + 1) % kNumFrames;
            return _frame == _startFrame;
        }

        public void LogVelocity(float velocity) => _frames[_frame].totalVelocity += velocity * Highlights.highlightRatingMultiplier;

        public void LogCoolness(int val) => _frames[_frame].coolness = Math.Max((byte)(_frames[_frame].coolness + (uint)(byte)(val * Highlights.highlightRatingMultiplier)), _frames[_frame].coolness);

        public void LogDeath() => _frames[_frame].deaths = Math.Max((byte)(_frames[_frame].deaths + (uint)(byte)(1.0 * Highlights.highlightRatingMultiplier)), _frames[_frame].deaths);

        public void LogAction(int num = 1) => _frames[_frame].actions = Math.Max((byte)(_frames[_frame].actions + (uint)(byte)(num * Highlights.highlightRatingMultiplier)), _frames[_frame].actions);

        public void LogBonus() => _frames[_frame].bonus = Math.Max((byte)(_frames[_frame].bonus + (uint)(byte)(1.0 * Highlights.highlightRatingMultiplier)), _frames[_frame].bonus);

        public void LogBackgroundColor(Color c) => _frames[_frame].backgroundColor = c;

        public void StateChange(
          SpriteSortMode sortModeVal,
          BlendState blendStateVal,
          SamplerState samplerStateVal,
          DepthStencilState depthStencilStateVal,
          RasterizerState rasterizerStateVal,
          MTEffect effectVal,
          Matrix cameraVal,
          Rectangle scissor)
        {
            _frames[_frame].StateChange(sortModeVal, blendStateVal, samplerStateVal, depthStencilStateVal, rasterizerStateVal, effectVal, cameraVal, scissor);
        }

        public void LogDraw(
          short textureVal,
          Vec2 topLeftVal,
          Vec2 bottomRightVal,
          float rotationVal,
          Color colorVal,
          short texXVal,
          short texYVal,
          short texWVal,
          short texHVal,
          float depthVal)
        {
            _frames[_frame].objects[_frames[_frame].currentObject].SetData(textureVal, topLeftVal, bottomRightVal, rotationVal, colorVal, texXVal, texYVal, texWVal, texHVal, depthVal);
            _frames[_frame].IncrementObject();
        }

        public void LogSound(string soundVal, float volumeVal, float pitchVal, float panVal) => _frames[_frame].sounds.Add(new RecorderSoundItem()
        {
            sound = soundVal,
            volume = volumeVal,
            pitch = pitchVal,
            pan = panVal
        });

        public FrameAnalytics GetAnalytics(FrameAnalytics f, int fr = -1)
        {
            fr = fr != -1 ? GetFrame(fr) : _frame;
            int kNumFrames = Recording.kNumFrames;
            int index1 = fr;
            float num1 = 0f;
            bool flag = false;
            for (int index2 = 0; index2 < kNumFrames; ++index2)
            {
                if (_frames[index1].deaths > 0)
                {
                    flag = true;
                    break;
                }
                num1 += 0.016f;
                ++index1;
                if (index1 >= Recording.kNumFrames)
                    index1 = 0;
                if (index1 == _startFrame)
                    break;
            }
            if (!flag)
                num1 = 99f;
            f.timeBeforeKill = num1;
            float num2 = (float)((1.0 - Maths.Clamp(f.timeBeforeKill, 0f, 3f) / 3.0) * 1.0 + 1.0);
            f.actions = _frames[fr].actions * (num2 * 0.03f);
            f.deaths = _frames[fr].deaths * num2;
            f.bonus = _frames[fr].bonus * (num2 * 0.08f);
            f.coolness = _frames[fr].coolness * (num2 * 0.1f);
            f.totalVelocity = _frames[fr].totalVelocity * (1f / 500f) * num2;
            return f;
        }
    }
}
