
namespace DuckGame
{
    public class DuckPersonaColor
    {
        public Vec3 Color1;
        public Vec3 Color2;
        public Vec3 Color3;
        public DuckPersonaColor(Vec3 Col, Vec3 Col2, Vec3 Col3)
        {
            Color1 = Col;
            Color2 = Col2;
            Color3 = Col3;
        }
        public DuckPersonaColor(Vec3 varCol)
           : this(varCol, Vec3.Zero, Vec3.Zero)
        {
        }
    }
}
