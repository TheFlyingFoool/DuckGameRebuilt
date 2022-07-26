// Decompiled with JetBrains decompiler
// Type: DuckGame.CannonGrenade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class CannonGrenade : Grenade
    {
        private List<Vec2> tail = new List<Vec2>();

        public CannonGrenade(float xval, float yval)
          : base(xval, yval)
        {
        }

        public override void Update()
        {
            this._bouncy = 0.7f;
            this.gravMultiplier = 0.9f;
            this.frictionMult = 0.5f;
            base.Update();
        }

        public override void Draw()
        {
            this.tail.Add(this.position);
            if (this.tail.Count > 10)
                this.tail.RemoveAt(0);
            if (this.tail.Count > 1)
            {
                for (int index = 1; index < this.tail.Count; ++index)
                    Graphics.DrawLine(this.tail[index - 1], this.tail[index], Color.White * ((float)index / (float)this.tail.Count) * 0.5f, depth: ((Depth)0.5f));
            }
            base.Draw();
        }
    }
}
