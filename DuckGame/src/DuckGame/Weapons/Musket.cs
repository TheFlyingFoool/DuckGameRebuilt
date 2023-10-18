// Decompiled with JetBrains decompiler
// Type: DuckGame.Musket
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Rifles")]
    [BaggedProperty("isInDemo", true)]
    public class Musket : TampingWeapon
    {
        public Musket(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATShrapnel
            {
                range = 470f,
                rangeVariation = 70f,
                accuracy = 0.2f
            };
            _type = "gun";
            graphic = new Sprite("musket");
            center = new Vec2(19f, 5f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 7f);
            _barrelOffsetTL = new Vec2(38f, 3f);
            _fireSound = "shotgun";
            _kickForce = 2f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(3f, 0f);
            editorTooltip = "Old-timey rifle, takes approximately 150 years to reload.";
            _editorPreviewOffset.x -= 1;
            _editorPreviewWidth = 41;
        }

        public override void Update() => base.Update();

        public override void OnPressAction()
        {
            if (_tamped)
            {
                base.OnPressAction();
                int num = 0;
                for (int index = 0; index < 14 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 100000); ++index)
                {
                    MusketSmoke musketSmoke = new MusketSmoke(barrelPosition.x - 16f + Rando.Float(32f), barrelPosition.y - 16f + Rando.Float(32f))
                    {
                        depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                    };
                    if (num < 6)
                        musketSmoke.move -= barrelVector * Rando.Float(0.1f);
                    if (num > 5 && num < 10)
                        musketSmoke.fly += barrelVector * (2f + Rando.Float(7.8f));
                    Level.Add(musketSmoke);
                    ++num;
                }
                _tampInc = 0f;
                _tampTime = infinite.value ? 0.5f : 0f;
                _tamped = false;
            }
            else
            {
                if (_raised || !(this.owner is Duck owner) || !owner.grounded)
                    return;
                owner.immobilized = true;
                owner.sliding = false;
                _rotating = true;
            }
        }
    }
}
