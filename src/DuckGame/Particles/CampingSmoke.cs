// Decompiled with JetBrains decompiler
// Type: DuckGame.CampingSmoke
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CampingSmoke : Thing
    {
        private float _angleInc;
        private float _scaleInc;
        private float _fade;
        public Vec2 move;
        public Vec2 fly;
        private float _fastGrow;
        private float _shrinkSpeed;
        private Sprite _backgroundSmoke;

        public CampingSmoke(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.xscale = 0.3f + Rando.Float(0.2f);
            this.yscale = this.xscale;
            this.angle = Maths.DegToRad(Rando.Float(360f));
            this._fastGrow = 0.3f + Rando.Float(0.3f);
            this._angleInc = Maths.DegToRad(Rando.Float(2f) - 1f);
            this._scaleInc = 1f / 1000f + Rando.Float(1f / 1000f);
            this._fade = 0.0015f + Rando.Float(1f / 1000f);
            this.move.x = Rando.Float(0.2f) - 0.1f;
            this.move.y = Rando.Float(0.2f) - 0.1f;
            GraphicList graphicList = new GraphicList();
            Sprite graphic1 = new Sprite("smoke")
            {
                depth = (Depth)1f
            };
            graphic1.CenterOrigin();
            graphicList.Add(graphic1);
            Sprite graphic2 = new Sprite("smokeBack")
            {
                depth = -0.1f
            };
            graphic2.CenterOrigin();
            graphicList.Add(graphic2);
            this.graphic = graphicList;
            this.center = new Vec2(0.0f, 0.0f);
            this.depth = (Depth)1f;
            this._backgroundSmoke = new Sprite("smokeBack");
            this._shrinkSpeed = 0.01f + Rando.Float(0.005f);
        }

        public override void Update()
        {
            this.angle += this._angleInc;
            this.xscale += this._scaleInc;
            if (_fastGrow > 0.0)
            {
                this._fastGrow -= 0.05f;
                this.xscale += 0.03f;
            }
            if (fly.x > 0.00999999977648258 || fly.x < -0.00999999977648258)
            {
                this.x += this.fly.x;
                this.fly.x *= 0.9f;
            }
            if (fly.y > 0.00999999977648258 || fly.y < -0.00999999977648258)
            {
                this.y += this.fly.y;
                this.fly.y *= 0.9f;
            }
            this.yscale = this.xscale;
            this.x += this.move.x;
            this.y += this.move.y;
            if ((double)this.xscale < 0.25)
                this.alpha -= 0.01f;
            this.xscale -= this._shrinkSpeed;
            if ((double)this.xscale >= 0.0500000007450581)
                return;
            Level.Remove(this);
        }
    }
}
