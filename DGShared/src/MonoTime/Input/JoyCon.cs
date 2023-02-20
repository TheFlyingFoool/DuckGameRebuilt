// Decompiled with JetBrains decompiler
// Type: DuckGame.JoyConBase
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public abstract class JoyConBase : AnalogGamePad
    {
        private Dictionary<int, string> _triggerNames = new Dictionary<int, string>()
    {
      {
        4096,
        "A"
      },
      {
        8192,
        "B"
      },
      {
        16384,
        "X"
      },
      {
        32768,
        "Y"
      },
      {
        16,
        Triggers.Start
      },
      {
        32,
        "BACK"
      },
      {
        4,
        Triggers.Left
      },
      {
        8,
        Triggers.Right
      },
      {
        1,
        Triggers.Up
      },
      {
        2,
        Triggers.Down
      },
      {
        2097152,
        "L{"
      },
      {
        1073741824,
        "L/"
      },
      {
        268435456,
        "L}"
      },
      {
        536870912,
        "L~"
      },
      {
        134217728,
        "R{"
      },
      {
        67108864,
        "R/"
      },
      {
        16777216,
        "R}"
      },
      {
        33554432,
        "R~"
      },
      {
        256,
        "LB"
      },
      {
        512,
        "RB"
      },
      {
        8388608,
        "LT"
      },
      {
        4194304,
        "RT"
      },
      {
        64,
        "LS"
      },
      {
        128,
        "RS"
      },
      {
        9999,
        "DPAD"
      },
      {
        9998,
        "DPAD"
      }
    };
        protected readonly Dictionary<int, Sprite> _imageMap;

        public override bool hasMotionAxis => true;

        public override float motionAxis => (float)Math.Abs(Math.Sin(SwitchSixAxis.GetAxis(_index) * Math.PI * 2.0));

        public override bool allowStartRemap => false;

        protected JoyConBase(
          int idx,
          string name,
          string productname,
          Dictionary<int, Sprite> image_map)
          : base(idx)
        {
            _name = name;
            _productName = productname;
            _imageMap = image_map;
            _productGUID = "";
        }

        public override Vec2 leftStick => new Vec2(_state.sticks.left.x, _state.sticks.left.y);

        protected Vec2 ReadRightStick() => new Vec2(_state.sticks.right.x, _state.sticks.right.y);

        public override Vec2 rightStick => Vec2.Zero;

        public abstract override bool isConnected { get; }

        public override Dictionary<int, string> GetTriggerNames() => _triggerNames;

        public override Sprite GetMapImage(int map)
        {
            Sprite mapImage;
            _imageMap.TryGetValue(map, out mapImage);
            return mapImage;
        }

        protected override PadState GetState(int index)
        {
            GamePadState state1 = FNAPlatform.GetGamePadState(index, GamePadDeadZone.Circular);
            PadState state2 = new PadState();
            if (state1.IsButtonDown(Buttons.DPadUp))
                state2.buttons |= PadButton.DPadUp;
            if (state1.IsButtonDown(Buttons.DPadDown))
                state2.buttons |= PadButton.DPadDown;
            if (state1.IsButtonDown(Buttons.DPadLeft))
                state2.buttons |= PadButton.DPadLeft;
            if (state1.IsButtonDown(Buttons.DPadRight))
                state2.buttons |= PadButton.DPadRight;
            if (state1.IsButtonDown(Buttons.Start))
                state2.buttons |= PadButton.Start;
            if (state1.IsButtonDown(Buttons.Back))
                state2.buttons |= PadButton.Back;
            if (state1.IsButtonDown(Buttons.LeftStick))
                state2.buttons |= PadButton.LeftStick;
            if (state1.IsButtonDown(Buttons.RightStick))
                state2.buttons |= PadButton.RightStick;
            if (state1.IsButtonDown(Buttons.LeftShoulder))
                state2.buttons |= PadButton.LeftShoulder;
            if (state1.IsButtonDown(Buttons.RightShoulder))
                state2.buttons |= PadButton.RightShoulder;
            if (state1.IsButtonDown(Buttons.BigButton))
                state2.buttons |= PadButton.BigButton;
            if (state1.IsButtonDown(Buttons.A))
                state2.buttons |= PadButton.A;
            if (state1.IsButtonDown(Buttons.B))
                state2.buttons |= PadButton.B;
            if (state1.IsButtonDown(Buttons.X))
                state2.buttons |= PadButton.X;
            if (state1.IsButtonDown(Buttons.Y))
                state2.buttons |= PadButton.Y;
            if (state1.IsButtonDown(Buttons.LeftThumbstickLeft))
                state2.buttons |= PadButton.LeftThumbstickLeft;
            if (state1.IsButtonDown(Buttons.RightTrigger))
                state2.buttons |= PadButton.RightTrigger;
            if (state1.IsButtonDown(Buttons.LeftTrigger))
                state2.buttons |= PadButton.LeftTrigger;
            if (state1.IsButtonDown(Buttons.RightThumbstickUp))
                state2.buttons |= PadButton.RightThumbstickUp;
            if (state1.IsButtonDown(Buttons.RightThumbstickDown))
                state2.buttons |= PadButton.RightThumbstickDown;
            if (state1.IsButtonDown(Buttons.RightThumbstickRight))
                state2.buttons |= PadButton.RightThumbstickRight;
            if (state1.IsButtonDown(Buttons.RightThumbstickLeft))
                state2.buttons |= PadButton.RightThumbstickLeft;
            if (state1.IsButtonDown(Buttons.LeftThumbstickUp))
                state2.buttons |= PadButton.LeftThumbstickUp;
            if (state1.IsButtonDown(Buttons.LeftThumbstickDown))
                state2.buttons |= PadButton.LeftThumbstickDown;
            if (state1.IsButtonDown(Buttons.LeftThumbstickRight))
                state2.buttons |= PadButton.LeftThumbstickRight;
            ref PadState.StickStates local1 = ref state2.sticks;
            GamePadThumbSticks thumbSticks = state1.ThumbSticks;
            Vec2 left = (Vec2)thumbSticks.Left;
            local1.left = left;
            ref PadState.StickStates local2 = ref state2.sticks;
            thumbSticks = state1.ThumbSticks;
            Vec2 right = (Vec2)thumbSticks.Right;
            local2.right = right;
            state2.triggers.left = state1.Triggers.Left;
            state2.triggers.right = state1.Triggers.Right;
            return state2;
        }
    }
}
