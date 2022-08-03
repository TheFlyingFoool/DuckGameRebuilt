// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckSkeleton
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DuckSkeleton
    {
        private DuckBone _upperTorso = new DuckBone();
        private DuckBone _head = new DuckBone();
        private DuckBone _lowerTorso = new DuckBone();

        public DuckBone upperTorso => _upperTorso;

        public DuckBone head => _head;

        public DuckBone lowerTorso => _lowerTorso;

        public void Draw()
        {
            Graphics.DrawRect(_upperTorso.position + new Vec2(-1f, -1f), _upperTorso.position + new Vec2(1f, 1f), Color.LimeGreen * 0.9f, (Depth)0.8f);
            Graphics.DrawLine(_upperTorso.position, _upperTorso.position + Maths.AngleToVec(_upperTorso.orientation) * 4f, Color.Yellow, depth: ((Depth)0.9f));
            Graphics.DrawRect(_lowerTorso.position + new Vec2(-1f, -1f), _lowerTorso.position + new Vec2(1f, 1f), Color.LimeGreen * 0.9f, (Depth)0.8f);
            Graphics.DrawLine(_lowerTorso.position, _lowerTorso.position + Maths.AngleToVec(_lowerTorso.orientation) * 4f, Color.Yellow, depth: ((Depth)0.9f));
            Graphics.DrawRect(_head.position + new Vec2(-1f, -1f), _head.position + new Vec2(1f, 1f), Color.LimeGreen * 0.9f, (Depth)0.8f);
            Graphics.DrawLine(_head.position, _head.position + Maths.AngleToVec(_head.orientation) * 4f, Color.Yellow, depth: ((Depth)0.9f));
        }
    }
}
