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
            _bouncy = 0.7f;
            gravMultiplier = 0.9f;
            frictionMult = 0.5f;
            base.Update();
        }

        public override void Draw()
        {
            tail.Add(position);
            if (tail.Count > 10)
                tail.RemoveAt(0);
            if (tail.Count > 1)
            {
                for (int index = 1; index < tail.Count; ++index)
                    Graphics.DrawLine(tail[index - 1], tail[index], Color.White * (index / (float)tail.Count) * 0.5f, depth: ((Depth)0.5f));
            }
            base.Draw();
        }
    }
}
