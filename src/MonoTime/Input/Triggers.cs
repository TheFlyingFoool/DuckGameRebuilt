// Decompiled with JetBrains decompiler
// Type: DuckGame.Triggers
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Triggers
    {
        public const string Left = "LEFT";
        public const string Right = "RIGHT";
        public const string Up = "UP";
        public const string Down = "DOWN";
        public const string Jump = "JUMP";
        public const string Quack = "QUACK";
        public const string Shoot = "SHOOT";
        public const string Grab = "GRAB";
        public const string Ragdoll = "RAGDOLL";
        public const string Strafe = "STRAFE";
        public const string Start = "START";
        public const string Select = "SELECT";
        public const string Cancel = "CANCEL";
        public const string Menu1 = "MENU1";
        public const string Menu2 = "MENU2";
        public const string Chat = "CHAT";
        public const string MenuLeft = "MENULEFT";
        public const string MenuRight = "MENURIGHT";
        public const string MenuUp = "MENUUP";
        public const string MenuDown = "MENUDOWN";
        public const string LeftTrigger = "LTRIGGER";
        public const string LeftOptionButton = "LOPTION";
        public const string RightTrigger = "RTRIGGER";
        public const string LeftStick = "LSTICK";
        public const string RightStick = "RSTICK";
        public const string VoiceRegister = "VOICEREG";
        public const string Any = "ANY";
        public const string LeftBumper = "LBUMPER";
        public const string RightBumper = "RBUMPER";
        public const string KeyboardF = "KBDF";
        public static Dictionary<byte, string> fromIndex = new Dictionary<byte, string>()
    {
      {
        (byte) 0,
        "LEFT"
      },
      {
        (byte) 1,
        "RIGHT"
      },
      {
        (byte) 2,
        "UP"
      },
      {
        (byte) 3,
        "DOWN"
      },
      {
        (byte) 4,
        "JUMP"
      },
      {
        (byte) 5,
        "QUACK"
      },
      {
        (byte) 6,
        "SHOOT"
      },
      {
        (byte) 7,
        "GRAB"
      },
      {
        (byte) 8,
        "RAGDOLL"
      },
      {
        (byte) 9,
        "STRAFE"
      },
      {
        (byte) 10,
        "START"
      },
      {
        (byte) 11,
        "SELECT"
      },
      {
        (byte) 12,
        "CHAT"
      },
      {
        (byte) 13,
        "LTRIGGER"
      },
      {
        (byte) 14,
        "RTRIGGER"
      },
      {
        (byte) 15,
        "LSTICK"
      },
      {
        (byte) 16,
        "RSTICK"
      },
      {
        (byte) 17,
        "ANY"
      },
      {
        (byte) 18,
        "LBUMPER"
      },
      {
        (byte) 19,
        "RBUMPER"
      },
      {
        (byte) 20,
        "CANCEL"
      },
      {
        (byte) 21,
        "LOPTION"
      },
      {
        (byte) 22,
        "MENU1"
      },
      {
        (byte) 23,
        "MENU2"
      },
      {
        (byte) 24,
        "MENULEFT"
      },
      {
        (byte) 25,
        "MENURIGHT"
      },
      {
        (byte) 26,
        "MENUUP"
      },
      {
        (byte) 27,
        "MENUDOWN"
      }
    };
        public static Dictionary<string, byte> toIndex = new Dictionary<string, byte>()
    {
      {
        "LEFT",
        (byte) 0
      },
      {
        "RIGHT",
        (byte) 1
      },
      {
        "UP",
        (byte) 2
      },
      {
        "DOWN",
        (byte) 3
      },
      {
        "JUMP",
        (byte) 4
      },
      {
        "QUACK",
        (byte) 5
      },
      {
        "SHOOT",
        (byte) 6
      },
      {
        "GRAB",
        (byte) 7
      },
      {
        "RAGDOLL",
        (byte) 8
      },
      {
        "STRAFE",
        (byte) 9
      },
      {
        "START",
        (byte) 10
      },
      {
        "SELECT",
        (byte) 11
      },
      {
        "CHAT",
        (byte) 12
      },
      {
        "LTRIGGER",
        (byte) 13
      },
      {
        "RTRIGGER",
        (byte) 14
      },
      {
        "LSTICK",
        (byte) 15
      },
      {
        "RSTICK",
        (byte) 16
      },
      {
        "ANY",
        (byte) 17
      },
      {
        "LBUMPER",
        (byte) 18
      },
      {
        "RBUMPER",
        (byte) 19
      },
      {
        "CANCEL",
        (byte) 20
      },
      {
        "LOPTION",
        (byte) 21
      },
      {
        "MENU1",
        (byte) 22
      },
      {
        "MENU2",
        (byte) 23
      },
      {
        "MENULEFT",
        (byte) 24
      },
      {
        "MENURIGHT",
        (byte) 25
      },
      {
        "MENUUP",
        (byte) 26
      },
      {
        "MENUDOWN",
        (byte) 27
      }
    };

        public static bool IsUITrigger(string val) => val == "START" || val == "CANCEL" || val == "SELECT" || val == "MENU1" || val == "MENU2" || val == "MENULEFT" || val == "MENURIGHT" || val == "MENUUP" || val == "MENUDOWN";

        public static bool IsBasicMovement(string val) => val == "LEFT" || val == "RIGHT";

        public static bool IsTrigger(string val) => val == "DPAD" || val == "WASD" || val == "LEFT" || val == "RIGHT" || val == "UP" || val == "DOWN" || val == "JUMP" || val == "QUACK" || val == "SHOOT" || val == "GRAB" || val == "RAGDOLL" || val == "STRAFE" || val == "START" || val == "SELECT" || val == "CHAT" || val == "LTRIGGER" || val == "RTRIGGER" || val == "LBUMPER" || val == "RBUMPER" || val == "LSTICK" || val == "RSTICK" || val == "CANCEL" || val == "LOPTION" || val == "MENU1" || val == "MENU2" || val == "MENULEFT" || val == "MENURIGHT" || val == "MENUUP" || val == "MENUDOWN";
    }
}
