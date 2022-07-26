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

        public DuckBone upperTorso => this._upperTorso;

        public DuckBone head => this._head;

        public DuckBone lowerTorso => this._lowerTorso;

        public void Draw()
        {
            Graphics.DrawRect(this._upperTorso.position + new Vec2(-1f, -1f), this._upperTorso.position + new Vec2(1f, 1f), Color.LimeGreen * 0.9f, (Depth)0.8f);
            Graphics.DrawLine(this._upperTorso.position, this._upperTorso.position + Maths.AngleToVec(this._upperTorso.orientation) * 4f, Color.Yellow, depth: ((Depth)0.9f));
            Graphics.DrawRect(this._lowerTorso.position + new Vec2(-1f, -1f), this._lowerTorso.position + new Vec2(1f, 1f), Color.LimeGreen * 0.9f, (Depth)0.8f);
            Graphics.DrawLine(this._lowerTorso.position, this._lowerTorso.position + Maths.AngleToVec(this._lowerTorso.orientation) * 4f, Color.Yellow, depth: ((Depth)0.9f));
            Graphics.DrawRect(this._head.position + new Vec2(-1f, -1f), this._head.position + new Vec2(1f, 1f), Color.LimeGreen * 0.9f, (Depth)0.8f);
            Graphics.DrawLine(this._head.position, this._head.position + Maths.AngleToVec(this._head.orientation) * 4f, Color.Yellow, depth: ((Depth)0.9f));
        }
    }
}
