// Decompiled with JetBrains decompiler
// Type: DuckGame.PopEffect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class PopEffect : Thing
    {
        private List<PopEffect.PopEffectPart> parts = new List<PopEffect.PopEffectPart>();
        private SpriteMap _sprite;

        public PopEffect(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("popLine", 16, 16);
            this.center = new Vec2(this._sprite.w / 2, this._sprite.h / 2);
            this.graphic = _sprite;
            int num1 = 8;
            for (int index = 0; index < num1; ++index)
            {
                float num2 = 360f / num1 * index;
                this.parts.Add(new PopEffect.PopEffectPart()
                {
                    scale = Rando.Float(0.6f, 1f),
                    rot = Maths.DegToRad(num2 + Rando.Float(-10f, 10f))
                });
            }
            this.scale = new Vec2(1.5f, 1.5f);
            this.depth = (Depth)0.85f;
        }

        public override void Update()
        {
            this.xscale -= 0.24f;
            this.yscale = this.xscale;
            if ((double)this.xscale >= 0.00999999977648258)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            foreach (PopEffect.PopEffectPart part in this.parts)
            {
                this._sprite.angle = part.rot;
                this._sprite.xscale = this._sprite.yscale = this.xscale * part.scale;
                this._sprite.center = new Vec2(this._sprite.w / 2, this._sprite.h / 2);
                this._sprite.alpha = 0.8f;
                Graphics.Draw(_sprite, this.x, this.y);
            }
            base.Draw();
        }

        public class PopEffectPart
        {
            public float scale;
            public float rot;
        }
    }
}
