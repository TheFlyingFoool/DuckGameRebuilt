// Decompiled with JetBrains decompiler
// Type: DuckGame.SwitchJoyConDual
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class SwitchJoyConDual : JoyConBase
    {
        private static readonly Dictionary<int, Sprite> _dualJoyImages = new Dictionary<int, Sprite>()
    {
      {
        4096,
        new Sprite("buttons/switch/dual/oButton")
      },
      {
        8192,
        new Sprite("buttons/switch/dual/aButton")
      },
      {
        16384,
        new Sprite("buttons/switch/dual/uButton")
      },
      {
        32768,
        new Sprite("buttons/switch/dual/yButton")
      },
      {
        16,
        new Sprite("buttons/switch/dual/startButton")
      },
      {
        32,
        new Sprite("buttons/switch/dual/selectButton")
      },
      {
        4,
        new Sprite("buttons/switch/dual/dPadLeft")
      },
      {
        8,
        new Sprite("buttons/switch/dual/dPadRight")
      },
      {
        1,
        new Sprite("buttons/switch/dual/dPadUp")
      },
      {
        2,
        new Sprite("buttons/switch/dual/dPadDown")
      },
      {
        256,
        new Sprite("buttons/switch/dual/leftBumper")
      },
      {
        512,
        new Sprite("buttons/switch/dual/rightBumper")
      },
      {
        8388608,
        new Sprite("buttons/switch/dual/leftTrigger")
      },
      {
        4194304,
        new Sprite("buttons/switch/dual/rightTrigger")
      },
      {
        64,
        new Sprite("buttons/switch/dual/leftStick")
      },
      {
        128,
        new Sprite("buttons/switch/dual/rightStick")
      },
      {
        9999,
        new Sprite("buttons/switch/dual/dPad")
      },
      {
        9998,
        new Sprite("buttons/switch/dual/dPad")
      }
    };

        public override int numSticks => 2;

        public override int numTriggers => 2;

        public SwitchJoyConDual(int idx)
          : base(idx, "Dual Joy-Con", "Joy-Con", SwitchJoyConDual._dualJoyImages)
        {
        }

        public override extern bool isConnected { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public override Vec2 rightStick => ReadRightStick();
    }
}
