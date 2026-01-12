using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SDL2;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class XInputPad : AnalogGamePad
    {
        public Dictionary<int, string> _triggerNames = new Dictionary<int, string>()
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
            "WASD"
          }
        };
        private Dictionary<int, string> _triggerNamesPS = new Dictionary<int, string>()
    {
      {
        4096,
        "CROSS"
      },
      {
        8192,
        "CIRCLE"
      },
      {
        16384,
        "SQUARE"
      },
      {
        32768,
        "TRIANGLE"
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
        "L1"
      },
      {
        512,
        "R1"
      },
      {
        8388608,
        "L2"
      },
      {
        4194304,
        "R2"
      },
      {
        64,
        "L3"
      },
      {
        128,
        "R3"
      }
    };
        private Dictionary<int, string> _triggerNamesWii = new Dictionary<int, string>()
        {
          {
            4096,
            "B"
          },
          {
            8192,
            "A"
          },
          {
            16384,
            "1"
          },
          {
            32768,
            "2"
          },
          {
            16,
            "+"
          },
          {
            32,
            "-"
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
            2048,
            "HOME"
          },
          {
            9999,
            "DPAD"
          },
          {
            9998,
            "WASD"
          }
        };
        public Dictionary<int, string> _triggerNamesN64 = new Dictionary<int, string>()
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
            16,
            Triggers.Start
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
            "L"
          },
          {
            512,
            "R"
          },
          {
            8388608,
            "Z"
          },
          {
            64,
            "CS"
          },
          {
            128,
            "C"
          },
          {
            9999,
            "DPAD"
          },
          {
            9998,
            "WASD"
          }
        };
        public Dictionary<int, Sprite> _triggerImages = new Dictionary<int, Sprite>()
        {
          {
            4096,
            new Sprite("buttons/xbox/oButton")
          },
          {
            8192,
            new Sprite("buttons/xbox/aButton")
          },
          {
            16384,
            new Sprite("buttons/xbox/uButton")
          },
          {
            32768,
            new Sprite("buttons/xbox/yButton")
          },
          {
            16,
            new Sprite("buttons/xbox/startButton")
          },
          {
            32,
            new Sprite("buttons/xbox/selectButton")
          },
          {
            4,
            new Sprite("buttons/xbox/dPadLeft")
          },
          {
            8,
            new Sprite("buttons/xbox/dPadRight")
          },
          {
            1,
            new Sprite("buttons/xbox/dPadUp")
          },
          {
            2,
            new Sprite("buttons/xbox/dPadDown")
          },
          {
            256,
            new Sprite("buttons/xbox/leftBumper")
          },
          {
            512,
            new Sprite("buttons/xbox/rightBumper")
          },
          {
            8388608,
            new Sprite("buttons/xbox/leftTrigger")
          },
          {
            4194304,
            new Sprite("buttons/xbox/rightTrigger")
          },
          {
            64,
            new Sprite("buttons/xbox/leftStick")
          },
          {
            128,
            new Sprite("buttons/xbox/rightStick")
          },
          {
            9999,
            new Sprite("buttons/xbox/dPad")
          },
          {
            9998,
            new Sprite("buttons/xbox/dPad")
          }
        };
        private Dictionary<int, Sprite> _triggerImagesPS = new Dictionary<int, Sprite>()
        {
          {
            4096,
            new Sprite("buttons/ps4/x")
          },
          {
            8192,
            new Sprite("buttons/ps4/circle")
          },
          {
            16384,
            new Sprite("buttons/ps4/square")
          },
          {
            32768,
            new Sprite("buttons/ps4/triangle")
          },
          {
            16,
            new Sprite("buttons/ps4/startButton")
          },
          {
            32,
            new Sprite("buttons/ps4/startButton")
          },
          {
            4,
            new Sprite("buttons/ps4/dPadLeft")
          },
          {
            8,
            new Sprite("buttons/ps4/dPadRight")
          },
          {
            1,
            new Sprite("buttons/ps4/dPadUp")
          },
          {
            2,
            new Sprite("buttons/ps4/dPadDown")
          },
          {
            256,
            new Sprite("buttons/ps4/leftBumper")
          },
          {
            512,
            new Sprite("buttons/ps4/rightBumper")
          },
          {
            8388608,
            new Sprite("buttons/ps4/leftTrigger")
          },
          {
            4194304,
            new Sprite("buttons/ps4/rightTrigger")
          },
          {
            64,
            new Sprite("buttons/ps4/leftStick")
          },
          {
            128,
            new Sprite("buttons/ps4/rightStick")
          },
          {
            9999,
            new Sprite("buttons/ps4/dPad")
          },
          {
            9998,
            new Sprite("buttons/ps4/dPad")
          }
        };
        private Dictionary<int, Sprite> _triggerImagesWii = new Dictionary<int, Sprite>()
        {
          {
            4096,
            new Sprite("buttons/wii/bButton")
          },
          {
            8192,
            new Sprite("buttons/wii/aButton")
          },
          {
            16384,
            new Sprite("buttons/wii/1button")
          },
          {
            32768,
            new Sprite("buttons/wii/2button")
          },
          {
            16,
            new Sprite("buttons/wii/plusButton")
          },
          {
            32,
            new Sprite("buttons/wii/minusButton")
          },
          {
            4,
            new Sprite("buttons/wii/dPadLeft")
          },
          {
            8,
            new Sprite("buttons/wii/dPadRight")
          },
          {
            1,
            new Sprite("buttons/wii/dPadUp")
          },
          {
            2,
            new Sprite("buttons/wii/dPadDown")
          },
          {
            2048,
            new Sprite("buttons/wii/homeButton")
          },
          {
            9999,
            new Sprite("buttons/wii/dPad")
          },
          {
            9998,
            new Sprite("buttons/wii/dPad")
          }
        };
        private Dictionary<int, Sprite> _triggerImagesN64 = new Dictionary<int, Sprite>()
        {
          {
            4,
            new Sprite("buttons/n64/dPadLeft")
          },
          {
            8,
            new Sprite("buttons/n64/dPadRight")
          },
          {
            1,
            new Sprite("buttons/n64/dPadUp")
          },
          {
            2,
            new Sprite("buttons/n64/dPadDown")
          },
          {
            4096,
            new Sprite("buttons/n64/aButton")
          },
          {
            8192,
            new Sprite("buttons/n64/bButton")
          },
          {
            256,
            new Sprite("buttons/n64/leftBumper")
          },
          {
            512,
            new Sprite("buttons/n64/rightBumper")
          },
          {
            8388608,
            new Sprite("buttons/n64/zTrigger")
          },
          {
            16,
            new Sprite("buttons/n64/startButton")
          },
          {
            128,
            new Sprite("buttons/n64/cPad")
          },
          {
            64,
            new Sprite("buttons/n64/controlStick")
          },
          {
            9999,
            new Sprite("buttons/n64/dPad")
          },
          {
            9998,
            new Sprite("buttons/n64/dPad")
          }
        };
        private bool _connectedState;
        public int ControllerType = 0;
        public override bool isConnected => _connectedState;

        public override bool allowStartRemap => true;

        public override int numSticks => 2;

        public override int numTriggers => 2;
        public static PadButton[] PadButtons;
        static XInputPad()
        {
            PadButtons = (PadButton[])Enum.GetValues(typeof(PadButton));
        }
        public XInputPad(int idx)
          : base(idx)
        {
            _name = "xbox" + idx.ToString();
            _productName = "XBOX GAMEPAD";
            _productGUID = "";
        }
        public bool PlayStationController(int controllerType)
        {
            bool isPlayStationController = false;
            if (Program.SDL2)
            {
                SDL.SDL_GameControllerType SDLControllerType = (SDL.SDL_GameControllerType)ControllerType;
                if (SDLControllerType == SDL.SDL_GameControllerType.SDL_CONTROLLER_TYPE_PS3 || SDLControllerType == SDL.SDL_GameControllerType.SDL_CONTROLLER_TYPE_PS4 || SDLControllerType == SDL.SDL_GameControllerType.SDL_CONTROLLER_TYPE_PS5)
                {
                    isPlayStationController = true;
                }
            }
            else
            {
                SDL3.SDL.SDL_GamepadType SDLControllerType = (SDL3.SDL.SDL_GamepadType)ControllerType;
                if (SDLControllerType == SDL3.SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_PS3 || SDLControllerType == SDL3.SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_PS4 || SDLControllerType == SDL3.SDL.SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5)
                {
                    isPlayStationController = true;
                }
            }
            return isPlayStationController;
        }
        public override Dictionary<int, string> GetTriggerNames()
        {
            if (PlayStationController(ControllerType))
            {
                return _triggerNamesPS;
            }
            else if (_productName == "Nintendo Wii Remote")
            {
                return _triggerNamesWii;
            }
            else if (_productName == "Nintendo 64 Controller")
            {
                return _triggerNamesN64;
            }
            else
            {
                return _triggerNames;
            }
        }

        public override Sprite GetMapImage(int map)
        {
            Sprite mapImage;
            if (PlayStationController(ControllerType))
            {
                _triggerImagesPS.TryGetValue(map, out mapImage);
            }
            /* else if (SDLControllerType == SDL.SDL_GameControllerType.SDL_CONTROLLER_TYPE_NINTENDO_SWITCH_PRO) // 		SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO 
            {
                //you should do things here
            }*/
            else if (_productName == "Nintendo Wii Remote")
            {
                _triggerImagesWii.TryGetValue(map, out mapImage);
            }
            else if (_productName == "Nintendo 64 Controller")
            {
                //Console.WriteLine("TryGetValue: " + map);
                _triggerImagesN64.TryGetValue(map, out mapImage);
            }
            else
            {
                _triggerImages.TryGetValue(map, out mapImage);
            }
            return mapImage;
        }

        public void InitializeState() => GetState(index);

        protected override PadState GetState(int index)
        {
            GamePadState state1 = GamePad.GetState(index, GamePadDeadZone.Circular);
            if (_connectedState != state1.IsConnected && state1.IsConnected)
            {
                string productname = FNAPlatform.GameControllerNameForIndex(index);
                ControllerType = (int)FNAPlatform.GameControllerTypeForIndex(index);
                if (productname != null && productname != "")
                {
                    _productName = productname;
                }
                else if (ControllerType == 0) // TYPE_UNKNOWN
                {
                    _productName = GamePad.GetControllerName(index);
                }
            }
            PadState state2 = new PadState();
            foreach (PadButton button in PadButtons)
            {
                if (state1.IsButtonDown((Buttons)button))
                    state2.buttons |= button;
            }
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
            _connectedState = state1.IsConnected;
            return state2;
        }
    }
}
