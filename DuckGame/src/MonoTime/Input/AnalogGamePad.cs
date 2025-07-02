using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class AnalogGamePad : InputDevice
    {
        protected PadState _repeatState;
        protected PadState _state;
        protected PadState _statePrev;
        protected Queue<PadState> _delayBuffer = new Queue<PadState>();
        //private const int numStates = 256;
        private int _curState;
        private int _numState;
        private int _realState;
        private PadState[] _states = new PadState[256];
        private bool _startPressed;
        public bool delay;
        public int delayIndex;
        private bool _ahead;
        private bool _behind;
        protected static Array _xboxButtons = Enum.GetValues(typeof(PadButton));
        public static bool inputDelay = true;
        private Vec2 _rumble;
        private Vec2 _highestRumble = Vec2.Zero;
        private Vec2 _prevRumble;
        private int _rumbleWait;
        //private int _disableDelay;

        public bool startWasPressed { get; set; }

        public virtual float leftTrigger => Maths.NormalizeSection(_state.triggers.left, 0.1f, 1f);

        public virtual float rightTrigger => Maths.NormalizeSection(_state.triggers.right, 0.1f, 1f);

        public virtual Vec2 leftStick => new Vec2(_state.sticks.left.x, _state.sticks.left.y);

        public virtual Vec2 rightStick => new Vec2(_state.sticks.right.x, _state.sticks.right.y);

        public AnalogGamePad(int idx)
          : base(idx)
        {
            delayIndex = idx;
        }

        protected virtual PadState GetState(int index) => new PadState();

        public override void Rumble(float leftIntensity = 0f, float rightIntensity = 0f)
        {
            if (!isConnected)
                return;
            if (_rumble == Vec2.Zero && (leftIntensity != 0f || rightIntensity != 0f))
            {
                RumbleNow(leftIntensity, rightIntensity);
            }
            else
            {
                _rumble = new Vec2(leftIntensity, rightIntensity);
                if (_rumble.x > _highestRumble.x)
                    _highestRumble.x = _rumble.x;
                if (_rumble.y <= _highestRumble.y)
                    return;
                _highestRumble.y = _rumble.y;
            }
        }
        public override void SetLightBar(Color color)
        {
            float gamma = 2.2f;
            float r = (float)Math.Pow((color.r / 255f), gamma);
            float g = (float)Math.Pow(color.g / 255f, gamma);
            float b = (float)Math.Pow(color.b / 255f, gamma);
            var correctedColor = new Microsoft.Xna.Framework.Color(
                r,
                g,
                b,
                (float)(color.a / 255f)
            );
            GamePad.SetLightBarEXT(index, correctedColor);
        }

        private void RumbleNow(float pLeft, float pRight)
        {
            GamePad.SetVibration(index, pLeft, pRight);
            _prevRumble = new Vec2(pLeft, pRight);
            _rumble = _prevRumble;
            _highestRumble = Vec2.Zero;
        }

        public override void Update()
        {
            base.Update();
            if (isConnected)
            {
                --_rumbleWait;
                if (_rumbleWait <= 0)
                {
                    _rumbleWait = 4;
                    if (_rumble != _prevRumble || _highestRumble.x > _rumble.x || _highestRumble.y > _rumble.y)
                        RumbleNow(_highestRumble.x, _highestRumble.y);
                }
            }
            if (delay)
            {
                _states[_curState] = GetState(delayIndex);
                _curState = (_curState + 1) % 256;
                ++_numState;
                if (_numState == 15)
                    _realState = _curState - 15;
                if (_numState < 15)
                    return;
                _statePrev = _state;
                _state = _states[_realState];
                _realState = (_realState + 1) % 256;
                if (Rando.Float(1f) <= 0.5f || _behind)
                    return;
                if (!_ahead)
                {
                    _realState = (_realState + 1) % 256;
                    _ahead = true;
                }
                else
                {
                    _realState = (_realState - 1) % 256;
                    if (_realState < 0)
                        _realState += 256;
                    _ahead = false;
                }
            }
            else
            {
                startWasPressed = _startPressed;
                _startPressed = false;
                _statePrev = _state;
                _state = GetState(index);
            }
        }

        public void StartPressed()
        {
            _state = _statePrev;
            _startPressed = true;
        }

        public override bool MapPressed(int mapping, bool any = false)
        {
            PadButton butt = (PadButton)mapping;
            if (butt == PadButton.Start && _startPressed)
                return true;
            return any ? (_state.buttons & ~_statePrev.buttons) != 0 : _state.IsButtonDown(butt) && !_statePrev.IsButtonDown(butt);
        }

        public override bool MapReleased(int mapping)
        {
            PadButton butt = (PadButton)mapping;
            return !_state.IsButtonDown(butt) && _statePrev.IsButtonDown(butt);
        }

        public override bool MapDown(int mapping, bool any = false)
        {
            if (any)
            {
                foreach (PadButton xboxButton in _xboxButtons)
                {
                    if (_state.IsButtonDown(xboxButton))
                        return true;
                }
                return false;
            }
            return _state.IsButtonDown((PadButton)mapping);
        }
    }
}
