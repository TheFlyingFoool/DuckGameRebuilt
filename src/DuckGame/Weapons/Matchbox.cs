// Decompiled with JetBrains decompiler
// Type: DuckGame.Matchbox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    [BaggedProperty("isOnlineCapable", true)]
    [BaggedProperty("isFatal", false)]
    public class Matchbox : Gun
    {
        public Matchbox(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 20;
            this._type = "gun";
            this.graphic = new Sprite("matchbox");
            this.center = new Vec2(8f, 14f);
            this.collisionOffset = new Vec2(-6f, -3f);
            this.collisionSize = new Vec2(12f, 5f);
            this._barrelOffsetTL = new Vec2(15f, 6f);
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 1f;
            this.editorTooltip = "A box full of fire sticks. Keep away from children.";
        }

        public override void Initialize() => base.Initialize();

        public override void Update() => base.Update();

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (this.isServerForObject && this.ammo > 0)
            {
                for (int index = 0; index < 5; ++index)
                    Level.Add(SmallFire.New(this.x - 6f + Rando.Float(12f), this.y - 8f + Rando.Float(4f), Rando.Float(4f) - 2f, (float)-(1.0 + (double)Rando.Float(2f)), firedFrom: this));
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                if (this.owner is Duck owner)
                    owner.ThrowItem();
                this.ammo = 0;
            }
            return true;
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                if (this.owner is Duck owner)
                {
                    --this.ammo;
                    SFX.Play("lightMatch", 0.5f, Rando.Float(0.2f) - 0.4f);
                    float num1 = 0.0f;
                    float num2 = 0.0f;
                    if (owner.inputProfile.Down("LEFT"))
                        --num1;
                    if (owner.inputProfile.Down("RIGHT"))
                        ++num1;
                    if (owner.inputProfile.Down("UP"))
                        --num2;
                    if (owner.inputProfile.Down("DOWN"))
                        ++num2;
                    if (this.receivingPress || !this.isServerForObject)
                        return;
                    if (owner.crouch)
                        Level.Add(SmallFire.New(this.x + offDir * 11f, this.y, 0.0f, 0.0f, firedFrom: this));
                    else
                        Level.Add(SmallFire.New(this.x + offDir * 11f, this.y, offDir * (1f + Rando.Float(0.3f)) + num1, -0.6f - Rando.Float(0.5f) + num2, firedFrom: this));
                }
                else
                    this.OnBurn(this.position, this);
            }
            else
                this.DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
