// Decompiled with JetBrains decompiler
// Type: DuckGame.MindControlBolt
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class MindControlBolt : Thing
    {
        private bool _fade;
        private Duck _controlledDuck;

        public Duck controlledDuck => this._controlledDuck;

        public MindControlBolt(float xval, float yval, Duck control)
          : base(xval, yval)
        {
            this._controlledDuck = control;
            this.graphic = new Sprite("mindBolt");
            this.center = new Vec2(8f, 8f);
            this.scale = new Vec2(0.1f, 0.1f);
            this.alpha = 0.0f;
        }

        public override void Update()
        {
            Vec2 position = this._controlledDuck.position;
            if (this._controlledDuck.ragdoll != null)
                position = this._controlledDuck.ragdoll.part3.position;
            Vec2 vec2 = position - this.position;
            double length = (double)vec2.length;
            vec2.Normalize();
            this.angleDegrees = (float)(-(double)Maths.PointDirection(this.position, position) + 90.0);
            this.position = this.position + vec2 * 4f;
            this.xscale = this.yscale = Lerp.Float(this.xscale, 1f, 0.05f);
            if (length < 48.0 || this._controlledDuck.mindControl == null)
                this._fade = true;
            this.alpha = Lerp.Float(this.alpha, this._fade ? 0.0f : 1f, 0.1f);
            if ((double)this.alpha < 0.00999999977648258 && this._fade)
                Level.Remove((Thing)this);
            base.Update();
        }
    }
}
