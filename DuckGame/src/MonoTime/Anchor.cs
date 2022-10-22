// Decompiled with JetBrains decompiler
// Type: DuckGame.Anchor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Anchor
    {
        private Thing _thing;
        public Vec2 offset = Vec2.Zero;

        public Thing thing => _thing;

        public Vec2 position => _thing.anchorPosition + offset;

        public Anchor(Thing to) => _thing = to;

        public static implicit operator Anchor(Thing val) => new Anchor(val);

        public static implicit operator Thing(Anchor val) => val._thing;

        public static bool operator ==(Anchor c1, Thing c2)
        {
            if ((object)c1 != null)
                return c1._thing == c2;
            return c2 == null;
        }

        public static bool operator !=(Anchor c1, Thing c2)
        {
            if ((object)c1 != null)
                return c1._thing != c2;
            return c2 != null;
        }

        public bool Equals(Thing p) => p == _thing;

        public override bool Equals(object obj) => obj is Thing ? Equals(obj as Thing) : base.Equals(obj);

        public override int GetHashCode() => _thing.GetHashCode();
    }
}
