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
        public const string DevConsoleTrigger = "CSOPEN";
        public const string Any = "ANY";

        public const string LeftBumper = "LBUMPER";
        public const string RightBumper = "RBUMPER";

        public const string KeyboardF = "KBDF";
        public static List<string> SimpleTriggerList = new List<string>()
        {
            Left,
            Right,
            Up,
            Down,
            Jump,
            Quack,
            Shoot,
            Grab,
            Ragdoll,
            Strafe,
            Start,
            Select,
            Cancel,
        };
        public static List<string> TriggerList = new List<string>()
        {
            Left,
            Right,
            Up,
            Down,
            Jump,
            Quack,
            Shoot,
            Grab,
            Ragdoll,
            Strafe,
            Start,
            Select,
            Chat,
            LeftTrigger,
            RightTrigger,
            LeftStick,
            RightStick,
            LeftBumper,
            RightBumper,
            Cancel,
            LeftOptionButton,
            Menu1,
            Menu2,
            MenuLeft,
            MenuRight,
            MenuUp,
            MenuDown
        };
        public static Dictionary<byte, string> fromIndex = new Dictionary<byte, string>()
        {
            {0, Left},
            {1, Right},
            {2, Up},
            {3, Down},
            {4, Jump},
            {5, Quack},
            {6, Shoot},
            {7, Grab},
            {8, Ragdoll},
            {9, Strafe},
            {10, Start},
            {11, Select},
            {12, Chat},
            {13, LeftTrigger},
            {14, RightTrigger},
            {15, LeftStick},
            {16, RightStick},
            {17, Any},
            {18, LeftBumper},
            {19, RightBumper},
            {20, Cancel},
            {21, LeftOptionButton},
            {22, Menu1},
            {23, Menu2},
            {24, MenuLeft},
            {25, MenuRight},
            {26, MenuUp},
            {27, MenuDown},
        };
        public static Dictionary<string, byte> toIndex = new Dictionary<string, byte>()
        {
            {Left, 0},
            {Right, 1},
            {Up, 2},
            {Down, 3},
            {Jump, 4},
            {Quack, 5},
            {Shoot, 6},
            {Grab, 7},
            {Ragdoll, 8},
            {Strafe, 9},
            { Start, 10},
            { Select, 11},
            { Chat, 12},
            { LeftTrigger, 13},
            { RightTrigger, 14},
            { LeftStick, 15},
            { RightStick, 16},
            { Any, 17},
            { LeftBumper, 18},
            { RightBumper, 19},
            { Cancel, 20 },
            { LeftOptionButton, 21 },
            { Menu1, 22 },
            { Menu2, 23 },
            { MenuLeft, 24 },
            { MenuRight, 25 },
            { MenuUp, 26 },
            { MenuDown, 27 },
        };

        public static bool IsUITrigger(string val)
        {
            if (val == Start ||
                val == Cancel ||
                val == Select ||
                val == Menu1 ||
                val == Menu2 ||
                val == MenuLeft ||
                val == MenuRight ||
                val == MenuUp ||
                val == MenuDown)
                return true;

            return false;
        }
        public static bool IsBasicMovement(string val) => val == Left || val == Right;

        public static bool IsTrigger(string val)
        {
            if (val == "DPAD" || val == "WASD" ||
               val == Left ||
               val == Right ||
               val == Up ||
               val == Down ||
               val == Jump ||
               val == Quack ||
               val == Shoot ||
               val == Grab ||
               val == Ragdoll ||
               val == Strafe ||
               val == Start ||
               val == Select ||
               val == Chat ||
               val == LeftTrigger ||
               val == RightTrigger ||
               val == LeftBumper ||
               val == RightBumper ||
               val == LeftStick ||
               val == RightStick ||
               val == Cancel ||
               val == LeftOptionButton ||
               val == Menu1 ||
               val == Menu2 ||
               val == MenuLeft ||
               val == MenuRight ||
               val == MenuUp ||
               val == MenuDown)
                return true;
            return false;
        }
    }
}
