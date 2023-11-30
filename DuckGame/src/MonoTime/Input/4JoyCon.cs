using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class SwitchProController : JoyConBase
    {
        private static readonly Dictionary<int, Sprite> _proControllerImages = new Dictionary<int, Sprite>()
    {
      {
        4096,
        new Sprite("buttons/switch/pro/oButton")
      },
      {
        8192,
        new Sprite("buttons/switch/pro/aButton")
      },
      {
        16384,
        new Sprite("buttons/switch/pro/uButton")
      },
      {
        32768,
        new Sprite("buttons/switch/pro/yButton")
      },
      {
        16,
        new Sprite("buttons/switch/pro/startButton")
      },
      {
        32,
        new Sprite("buttons/switch/pro/selectButton")
      },
      {
        4,
        new Sprite("buttons/switch/pro/dPadLeft")
      },
      {
        8,
        new Sprite("buttons/switch/pro/dPadRight")
      },
      {
        1,
        new Sprite("buttons/switch/pro/dPadUp")
      },
      {
        2,
        new Sprite("buttons/switch/pro/dPadDown")
      },
      {
        256,
        new Sprite("buttons/switch/pro/leftBumper")
      },
      {
        512,
        new Sprite("buttons/switch/pro/rightBumper")
      },
      {
        8388608,
        new Sprite("buttons/switch/pro/leftTrigger")
      },
      {
        4194304,
        new Sprite("buttons/switch/pro/rightTrigger")
      },
      {
        64,
        new Sprite("buttons/switch/pro/leftStick")
      },
      {
        128,
        new Sprite("buttons/switch/pro/rightStick")
      },
      {
        9999,
        new Sprite("buttons/switch/pro/dPad")
      },
      {
        9998,
        new Sprite("buttons/switch/pro/dPad")
      }
    };

        public override int numSticks => 2;

        public override int numTriggers => 2;

        public SwitchProController(int idx)
          : base(idx, "Pro Controller", "Pro Controller", _proControllerImages)
        {
        }

        public extern override bool isConnected { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public override Vec2 rightStick => ReadRightStick();
    }
}
