// Decompiled with JetBrains decompiler
// Type: DuckGame.ElectricalCharge
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class ElectricalCharge : Thing
    {
        private List<Vec2> _prevPositions = new List<Vec2>();
        private Vec2 _travelVec;

        public ElectricalCharge(float xpos, float ypos, int off, Thing own)
          : base(xpos, ypos)
        {
            this.offDir = (sbyte)off;
            this._travelVec = new Vec2(offDir * Rando.Float(6f, 10f), Rando.Float(-10f, 10f));
            this.owner = own;
        }

        public override void Update()
        {
            if (this._prevPositions.Count == 0)
                this._prevPositions.Insert(0, this.position);
            Vec2 position1 = this.position;
            this.position += this._travelVec;
            this._travelVec = new Vec2(offDir * Rando.Float(6f, 10f), Rando.Float(-10f, 10f));
            this._prevPositions.Insert(0, this.position);
            this.alpha -= 0.1f;
            if ((double)this.alpha < 0.0)
                Level.Remove(this);
            Vec2 position2 = this.position;
            foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(position1, position2))
            {
                if (amAduck is MaterialThing materialThing && amAduck != this.owner.owner)
                    materialThing.Zap(this.owner);
            }
            base.Update();
        }

        public override void Draw()
        {
            Vec2 p2 = Vec2.Zero;
            bool flag = false;
            float num = 1f;
            foreach (Vec2 prevPosition in this._prevPositions)
            {
                if (!flag)
                {
                    flag = true;
                    p2 = prevPosition;
                }
                else
                {
                    Graphics.DrawLine(prevPosition, p2, Colors.DGYellow * num, depth: ((Depth)0.9f));
                    num -= 0.25f;
                }
                if ((double)num <= 0.0)
                    break;
            }
            base.Draw();
        }
    }
}
