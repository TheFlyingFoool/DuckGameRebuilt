using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class SwitchJoyConLeft : JoyConBase
    {
        private static readonly Dictionary<int, Sprite> _leftJoyImages = new Dictionary<int, Sprite>()
    {
      {
        4096,
        new Sprite("buttons/switch/left/oButton")
      },
      {
        8192,
        new Sprite("buttons/switch/left/aButton")
      },
      {
        16384,
        new Sprite("buttons/switch/left/uButton")
      },
      {
        32768,
        new Sprite("buttons/switch/left/yButton")
      },
      {
        16,
        new Sprite("buttons/switch/left/startButton")
      },
      {
        256,
        new Sprite("buttons/switch/left/leftBumper")
      },
      {
        512,
        new Sprite("buttons/switch/left/rightBumper")
      },
      {
        8388608,
        new Sprite("buttons/switch/left/leftTrigger")
      },
      {
        4194304,
        new Sprite("buttons/switch/left/rightTrigger")
      },
      {
        64,
        new Sprite("buttons/switch/dual/leftStick")
      },
      {
        9999,
        new Sprite("buttons/switch/left/leftStick")
      }
    };

        public override int numSticks => 1;

        public override int numTriggers => 0;

        public override bool allowDirectionalMapping => false;

        public SwitchJoyConLeft(int idx)
          : base(idx, "Left Joy-Con", "Joy-Con (L)", _leftJoyImages)
        {
        }

        public extern override bool isConnected { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}
