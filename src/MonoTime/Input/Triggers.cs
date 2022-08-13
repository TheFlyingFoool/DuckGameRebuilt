// Decompiled with JetBrains decompiler
// Type: DuckGame.Triggers
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
         0,
        "LEFT"
      },
      {
         1,
        "RIGHT"
      },
      {
         2,
        "UP"
      },
      {
         3,
        "DOWN"
      },
      {
         4,
        "JUMP"
      },
      {
         5,
        "QUACK"
      },
      {
         6,
        "SHOOT"
      },
      {
         7,
        "GRAB"
      },
      {
         8,
        "RAGDOLL"
      },
      {
         9,
        "STRAFE"
      },
      {
         10,
        "START"
      },
      {
         11,
        "SELECT"
      },
      {
         12,
        "CHAT"
      },
      {
         13,
        "LTRIGGER"
      },
      {
         14,
        "RTRIGGER"
      },
      {
         15,
        "LSTICK"
      },
      {
         16,
        "RSTICK"
      },
      {
         17,
        "ANY"
      },
      {
         18,
        "LBUMPER"
      },
      {
         19,
        "RBUMPER"
      },
      {
         20,
        "CANCEL"
      },
      {
         21,
        "LOPTION"
      },
      {
         22,
        "MENU1"
      },
      {
         23,
        "MENU2"
      },
      {
         24,
        "MENULEFT"
      },
      {
         25,
        "MENURIGHT"
      },
      {
         26,
        "MENUUP"
      },
      {
         27,
        "MENUDOWN"
      }
    };
        public static Dictionary<string, byte> toIndex = new Dictionary<string, byte>()
    {
      {
        "LEFT",
         0
      },
      {
        "RIGHT",
         1
      },
      {
        "UP",
         2
      },
      {
        "DOWN",
         3
      },
      {
        "JUMP",
         4
      },
      {
        "QUACK",
         5
      },
      {
        "SHOOT",
         6
      },
      {
        "GRAB",
         7
      },
      {
        "RAGDOLL",
         8
      },
      {
        "STRAFE",
         9
      },
      {
        "START",
         10
      },
      {
        "SELECT",
         11
      },
      {
        "CHAT",
         12
      },
      {
        "LTRIGGER",
         13
      },
      {
        "RTRIGGER",
         14
      },
      {
        "LSTICK",
         15
      },
      {
        "RSTICK",
         16
      },
      {
        "ANY",
         17
      },
      {
        "LBUMPER",
         18
      },
      {
        "RBUMPER",
         19
      },
      {
        "CANCEL",
         20
      },
      {
        "LOPTION",
         21
      },
      {
        "MENU1",
         22
      },
      {
        "MENU2",
         23
      },
      {
        "MENULEFT",
         24
      },
      {
        "MENURIGHT",
         25
      },
      {
        "MENUUP",
         26
      },
      {
        "MENUDOWN",
         27
      }
    };

        public static bool IsUITrigger(string val) => val == "START" || val == "CANCEL" || val == "SELECT" || val == "MENU1" || val == "MENU2" || val == "MENULEFT" || val == "MENURIGHT" || val == "MENUUP" || val == "MENUDOWN";

        public static bool IsBasicMovement(string val) => val == "LEFT" || val == "RIGHT";

        public static bool IsTrigger(string val) => val == "DPAD" || val == "WASD" || val == "LEFT" || val == "RIGHT" || val == "UP" || val == "DOWN" || val == "JUMP" || val == "QUACK" || val == "SHOOT" || val == "GRAB" || val == "RAGDOLL" || val == "STRAFE" || val == "START" || val == "SELECT" || val == "CHAT" || val == "LTRIGGER" || val == "RTRIGGER" || val == "LBUMPER" || val == "RBUMPER" || val == "LSTICK" || val == "RSTICK" || val == "CANCEL" || val == "LOPTION" || val == "MENU1" || val == "MENU2" || val == "MENULEFT" || val == "MENURIGHT" || val == "MENUUP" || val == "MENUDOWN";
    }
}
