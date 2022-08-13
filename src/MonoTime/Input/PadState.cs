// Decompiled with JetBrains decompiler
// Type: DuckGame.PadState
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    /// <summary>The state of a pad.</summary>
    public struct PadState
    {
        public PadButton buttons;
        public PadState.TriggerStates triggers;
        public PadState.StickStates sticks;

        public bool IsButtonDown(PadButton butt) => (buttons & butt) != 0;

        public bool IsButtonUp(PadButton butt) => (buttons & butt) == 0;

        public struct TriggerStates
        {
            public float left;
            public float right;
        }

        public struct StickStates
        {
            public Vec2 left;
            public Vec2 right;
        }
    }
}
