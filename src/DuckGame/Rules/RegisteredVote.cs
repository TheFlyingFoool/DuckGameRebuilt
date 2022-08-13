// Decompiled with JetBrains decompiler
// Type: DuckGame.RegisteredVote
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class RegisteredVote
    {
        public Profile who;
        public VoteType vote = VoteType.None;
        public float slide;
        public bool open = true;
        public bool doClose;
        public float wobble;
        public float wobbleInc;
        public Vec2 leftStick;
        public Vec2 rightStick;
    }
}
