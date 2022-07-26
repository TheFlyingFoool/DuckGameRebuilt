// Decompiled with JetBrains decompiler
// Type: DuckGame.SpawnerBall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class SpawnerBall : Thing
    {
        public System.Type contains;
        protected SpriteMap _sprite;
        private bool _secondBall;
        private float _wave;
        private bool _grow = true;
        private float _wave2;
        public float orbitDistance = 3f;
        public float desiredOrbitDistance = 3f;
        public float orbitHeight = 1f;
        public float desiredOrbitHeight = 1f;

        public SpawnerBall(float xpos, float ypos, bool secondBall)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("spawnerBall", 4, 4);
            this._sprite.frame = 1;
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(2f, 2f);
            this._sprite.center = new Vec2(2f, 2f);
            this.depth = (Depth)0.5f;
            this._secondBall = secondBall;
        }

        public override void Update()
        {
            this.orbitDistance = MathHelper.Lerp(this.orbitDistance, this.desiredOrbitDistance, 0.05f);
            this.orbitHeight = MathHelper.Lerp(this.orbitHeight, this.desiredOrbitHeight, 0.05f);
            this._wave += 0.08f;
            if ((double)this._wave > 6.28000020980835)
            {
                this._wave -= 6.28f;
                this._grow = !this._grow;
            }
            this._wave2 += 0.05f;
        }

        public override void Draw()
        {
            float num = (float)((Math.Sin((double)this._wave + 1.57000005245209) + 1.0) / 2.0 * 0.5);
            if (!this._secondBall)
            {
                this._sprite.scale = new Vec2(num + 0.6f, num + 0.6f);
                this._sprite.depth = (Depth)((double)this._sprite.scale.x > 0.800000011920929 ? 0.4f : -0.8f);
                Graphics.Draw((Sprite)this._sprite, this.x + (float)Math.Sin((double)this._wave) * this.orbitDistance, this.y - this.orbitHeight);
            }
            else
            {
                this._sprite.scale = new Vec2((float)(0.5 - (double)num + 0.600000023841858), (float)(0.5 - (double)num + 0.600000023841858));
                this._sprite.depth = (Depth)((double)this._sprite.scale.x > 0.800000011920929 ? 0.4f : -0.8f);
                Graphics.Draw((Sprite)this._sprite, this.x - (float)Math.Sin((double)this._wave) * this.orbitDistance, this.y - this.orbitHeight);
            }
        }
    }
}
