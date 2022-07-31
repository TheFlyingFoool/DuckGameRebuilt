// Decompiled with JetBrains decompiler
// Type: DuckGame.Blunderbuss
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Shotguns")]
    public class Blunderbuss : TampingWeapon
    {
        public Blunderbuss(float xval, float yval)
          : base(xval, yval)
        {
            this.wideBarrel = true;
            this.ammo = 99;
            this._ammoType = new ATShrapnel();
            this._ammoType.range = 140f;
            this._ammoType.rangeVariation = 40f;
            this._ammoType.accuracy = 0.01f;
            this._numBulletsPerFire = 4;
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this.graphic = new Sprite("blunderbuss");
            this.center = new Vec2(19f, 5f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 7f);
            this._barrelOffsetTL = new Vec2(34f, 4f);
            this._fireSound = "shotgun";
            this._kickForce = 2f;
            this._fireRumble = RumbleIntensity.Light;
            this._holdOffset = new Vec2(4f, 1f);
            this.editorTooltip = "Old-timey shotgun, takes approximately 150 years to reload.";
        }

        public override void Update() => base.Update();

        public override void OnPressAction()
        {
            if (this._tamped)
            {
                base.OnPressAction();
                int num = 0;
                for (int index = 0; index < 14; ++index)
                {
                    MusketSmoke musketSmoke = new MusketSmoke(this.barrelPosition.x - 16f + Rando.Float(32f), this.barrelPosition.y - 16f + Rando.Float(32f))
                    {
                        depth = (Depth)(0.9f + index * (1f / 1000f))
                    };
                    if (num < 6)
                        musketSmoke.move -= this.barrelVector * Rando.Float(0.1f);
                    if (num > 5 && num < 10)
                        musketSmoke.fly += this.barrelVector * (2f + Rando.Float(7.8f));
                    Level.Add(musketSmoke);
                    ++num;
                }
                this._tampInc = 0f;
                this._tampTime = this.infinite.value ? 0.5f : 0f;
                this._tamped = false;
            }
            else
            {
                if (this._raised || !(this.owner is Duck owner) || !owner.grounded)
                    return;
                owner.immobilized = true;
                owner.sliding = false;
                this._rotating = true;
            }
        }
    }
}
