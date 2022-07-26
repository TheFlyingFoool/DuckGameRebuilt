// Decompiled with JetBrains decompiler
// Type: DuckGame.ColorStar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ColorStar : PhysicsParticle, IFactory
    {
        private float maxSize;

        public ColorStar(float xpos, float ypos, Vec2 dir, Color pColor)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("colorStar");
            this.graphic.CenterOrigin();
            this.center = new Vec2((float)(this.graphic.width / 2), (float)(this.graphic.height / 2));
            this.xscale = this.yscale = 0.9f;
            this.hSpeed = dir.x;
            this.vSpeed = dir.y;
            this.maxSize = 0.1f;
            this.graphic.color = pColor;
            this._gravMult = 3f;
        }

        public override void Update()
        {
            this.xscale = Lerp.Float(this.xscale, this.maxSize, 0.04f);
            this.yscale = this.xscale;
            if ((double)this.xscale <= (double)this.maxSize)
                Level.Remove((Thing)this);
            base.Update();
        }
    }
}
