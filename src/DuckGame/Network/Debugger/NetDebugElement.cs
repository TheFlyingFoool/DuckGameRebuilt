// Decompiled with JetBrains decompiler
// Type: DuckGame.NetDebugElement
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NetDebugElement
    {
        public float width;
        public NetDebugElement right;
        public Depth depth;
        public float indent;
        public float leading;
        protected NetDebugInterface _interface;
        protected string _name;

        public NetDebugElement(NetDebugInterface pInterface) => _interface = pInterface;

        public virtual bool DoDraw(Vec2 position, bool allowInput)
        {
            bool flag = Draw(position, allowInput);
            if (right != null)
                flag |= right.DoDraw(position + new Vec2(width, 0f), !flag);
            return flag;
        }

        protected virtual bool Draw(Vec2 position, bool allowInput) => false;
    }
}
