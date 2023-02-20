// Decompiled with JetBrains decompiler
// Type: DuckGame.BananaCluster
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("previewPriority", true)]
    public class BananaCluster : Gun
    {
        private SpriteMap _sprite;
        private int _ammoMax = 3;

        public BananaCluster(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 3;
            _ammoType = new ATShrapnel();
            _type = "gun";
            _sprite = new SpriteMap("banana", 16, 16)
            {
                frame = 4
            };
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(12f, 11f);
            physicsMaterial = PhysicsMaterial.Rubber;
            _holdOffset = new Vec2(0f, 2f);
            bouncy = 0.4f;
            friction = 0.05f;
            editorTooltip = "Need more than one banana? Have I got news for you...";
            isFatal = false;
        }

        public override void Update()
        {
            _sprite.frame = 4 + _ammoMax - ammo;
            if (ammo == 0 && owner != null)
            {
                if (this.owner is Duck owner)
                    owner.ThrowItem();
                Level.Remove(this);
            }
            if (owner == null && ammo == 1)
            {
                Banana banana = new Banana(x, y)
                {
                    hSpeed = hSpeed,
                    vSpeed = vSpeed
                };
                Level.Remove(this);
                Level.Add(banana);
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (ammo <= 0)
                return;
            --ammo;
            SFX.Play("smallSplat", pitch: Rando.Float(-0.6f, 0.6f));
            if (!(this.owner is Duck owner))
                return;
            float num1 = 0f;
            float num2 = 0f;
            if (owner.inputProfile.Down(Triggers.Left))
                num1 -= 3f;
            if (owner.inputProfile.Down(Triggers.Right))
                num1 += 3f;
            if (owner.inputProfile.Down(Triggers.Up))
                num2 -= 3f;
            if (owner.inputProfile.Down(Triggers.Down))
                num2 += 3f;
            if (!isServerForObject)
                return;
            Banana banana = new Banana(barrelPosition.x, barrelPosition.y);
            if (!owner.crouch)
            {
                banana.hSpeed = offDir * Rando.Float(3f, 3.5f) + num1;
                banana.vSpeed = num2 - 1.5f + Rando.Float(-0.5f, -1f);
            }
            banana.EatBanana();
            banana.clip.Add(owner);
            owner.clip.Add(banana);
            Level.Add(banana);
        }
    }
}
