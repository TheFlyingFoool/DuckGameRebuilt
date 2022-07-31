// Decompiled with JetBrains decompiler
// Type: DuckGame.AnalogGamePad
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public virtual float leftTrigger => Maths.NormalizeSection(this._state.triggers.left, 0.1f, 1f);

        public virtual float rightTrigger => Maths.NormalizeSection(this._state.triggers.right, 0.1f, 1f);

        public virtual Vec2 leftStick => new Vec2(this._state.sticks.left.x, this._state.sticks.left.y);

        public virtual Vec2 rightStick => new Vec2(this._state.sticks.right.x, this._state.sticks.right.y);

        public AnalogGamePad(int idx)
          : base(idx)
        {
            this.delayIndex = idx;
        }

        protected virtual PadState GetState(int index) => new PadState();

        public override void Rumble(float leftIntensity = 0f, float rightIntensity = 0f)
        {
            if (!this.isConnected)
                return;
            if (this._rumble == Vec2.Zero && ((double)leftIntensity != 0.0 || (double)rightIntensity != 0.0))
            {
                this.RumbleNow(leftIntensity, rightIntensity);
            }
            else
            {
                this._rumble = new Vec2(leftIntensity, rightIntensity);
                if (_rumble.x > (double)this._highestRumble.x)
                    this._highestRumble.x = this._rumble.x;
                if (_rumble.y <= (double)this._highestRumble.y)
                    return;
                this._highestRumble.y = this._rumble.y;
            }
        }

        private void RumbleNow(float pLeft, float pRight)
        {
            GamePad.SetVibration((PlayerIndex)this.index, pLeft, pRight);
            this._prevRumble = new Vec2(pLeft, pRight);
            this._rumble = this._prevRumble;
            this._highestRumble = Vec2.Zero;
        }

        public override void Update()
        {
            base.Update();
            if (this.isConnected)
            {
                --this._rumbleWait;
                if (this._rumbleWait <= 0)
                {
                    this._rumbleWait = 4;
                    if (this._rumble != this._prevRumble || _highestRumble.x > (double)this._rumble.x || _highestRumble.y > (double)this._rumble.y)
                        this.RumbleNow(this._highestRumble.x, this._highestRumble.y);
                }
            }
            if (this.delay)
            {
                this._states[this._curState] = this.GetState(this.delayIndex);
                this._curState = (this._curState + 1) % 256;
                ++this._numState;
                if (this._numState == 15)
                    this._realState = this._curState - 15;
                if (this._numState < 15)
                    return;
                this._statePrev = this._state;
                this._state = this._states[this._realState];
                this._realState = (this._realState + 1) % 256;
                if ((double)Rando.Float(1f) <= 0.5 || this._behind)
                    return;
                if (!this._ahead)
                {
                    this._realState = (this._realState + 1) % 256;
                    this._ahead = true;
                }
                else
                {
                    this._realState = (this._realState - 1) % 256;
                    if (this._realState < 0)
                        this._realState += 256;
                    this._ahead = false;
                }
            }
            else
            {
                this.startWasPressed = this._startPressed;
                this._startPressed = false;
                this._statePrev = this._state;
                this._state = this.GetState(this.index);
            }
        }

        public void StartPressed()
        {
            this._state = this._statePrev;
            this._startPressed = true;
        }

        public override bool MapPressed(int mapping, bool any = false)
        {
            PadButton butt = (PadButton)mapping;
            if (butt == PadButton.Start && this._startPressed)
                return true;
            return any ? (this._state.buttons & ~this._statePrev.buttons) != 0 : this._state.IsButtonDown(butt) && !this._statePrev.IsButtonDown(butt);
        }

        public override bool MapReleased(int mapping)
        {
            PadButton butt = (PadButton)mapping;
            return !this._state.IsButtonDown(butt) && this._statePrev.IsButtonDown(butt);
        }

        public override bool MapDown(int mapping, bool any = false)
        {
            if (any)
            {
                foreach (PadButton xboxButton in AnalogGamePad._xboxButtons)
                {
                    if (this._state.IsButtonDown(xboxButton))
                        return true;
                }
                return false;
            }
            return this._state.IsButtonDown((PadButton)mapping);
        }
    }
}
