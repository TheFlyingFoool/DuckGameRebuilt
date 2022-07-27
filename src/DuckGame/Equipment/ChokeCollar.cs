// Decompiled with JetBrains decompiler
// Type: DuckGame.ChokeCollar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ChokeCollar : Equipment
    {
        private SpriteMap _sprite;
        private Sprite _pickupSprite;
        protected WeightBall _ball;
        public EditorProperty<bool> mace = new EditorProperty<bool>(false);

        public WeightBall ball => this._ball;

        public ChokeCollar(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite("chokeCollar");
            this._sprite = new SpriteMap("chokeCollar", 8, 8);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -2f);
            this.collisionSize = new Vec2(8f, 4f);
            this._sprite.CenterOrigin();
            this.thickness = 0.1f;
            this.impactThreshold = 0.01f;
            this._equippedDepth = 5;
            this._wearOffset = new Vec2(6f, 15f);
            this.editorTooltip = "A heavy ball & chain that can be swung with great force. For fun!";
        }

        public override void Initialize()
        {
            base.Initialize();
            if (Level.current is Editor)
                return;
            this._ball = new WeightBall(this.x, this.y, this, this, this.mace.value);
            this.ReturnItemToWorld(_ball);
            Level.Add(_ball);
        }

        public override void Update()
        {
            this._ball.clip = this.clip;
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.collisionOffset = new Vec2(-6f, -6f);
                this.collisionSize = new Vec2(12f, 12f);
                this.center = new Vec2(8f, 8f);
                this.solid = false;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;
                this.graphic = _sprite;
                this._ball.SetAttach(this.owner as PhysicsObject);
            }
            else
            {
                this.collisionOffset = new Vec2(-4f, -2f);
                this.collisionSize = new Vec2(8f, 4f);
                this.center = new Vec2(this._pickupSprite.w / 2, this._pickupSprite.h / 2);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
                this._ball.SetAttach(this);
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove(this);
            base.Update();
        }
    }
}
