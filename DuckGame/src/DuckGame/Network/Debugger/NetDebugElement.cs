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
