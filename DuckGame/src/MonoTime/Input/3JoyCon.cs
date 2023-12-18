using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class SwitchJoyConRight : JoyConBase
    {
        private static readonly Dictionary<int, Sprite> _rightJoyImages = new Dictionary<int, Sprite>()
    {
      {
        4096,
        new Sprite("buttons/switch/right/oButton")
      },
      {
        8192,
        new Sprite("buttons/switch/right/aButton")
      },
      {
        16384,
        new Sprite("buttons/switch/right/uButton")
      },
      {
        32768,
        new Sprite("buttons/switch/right/yButton")
      },
      {
        16,
        new Sprite("buttons/switch/right/startButton")
      },
      {
        256,
        new Sprite("buttons/switch/right/leftBumper")
      },
      {
        512,
        new Sprite("buttons/switch/right/rightBumper")
      },
      {
        8388608,
        new Sprite("buttons/switch/right/leftTrigger")
      },
      {
        4194304,
        new Sprite("buttons/switch/right/rightTrigger")
      },
      {
        64,
        new Sprite("buttons/switch/dual/leftStick")
      },
      {
        9999,
        new Sprite("buttons/switch/right/leftStick")
      }
    };

        public override int numSticks => 1;

        public override int numTriggers => 0;

        public override bool allowDirectionalMapping => false;

        public SwitchJoyConRight(int idx)
          : base(idx, "Right Joy-Con", "Joy-Con (R)", _rightJoyImages)
        {
        }

        public extern override bool isConnected { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}
