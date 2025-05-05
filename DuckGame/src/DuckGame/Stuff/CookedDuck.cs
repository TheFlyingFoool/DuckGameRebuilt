using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class CookedDuck : Holdable, IPlatform
    {
        private List<SpriteMap> _flavourLines = new List<SpriteMap>();
        private int _timeHot = 3600;
        private float _hotAlpha = 1f;

        public override bool visible
        {
            get => base.visible;
            set => base.visible = value;
        }

        public CookedDuck(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("cookedDuck");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(12f, 11f);
            depth = -0.5f;
            thickness = 0.5f;
            weight = 5f;
            collideSounds.Add("rockHitGround2", ImpactedFrom.Bottom);
            collideSounds.Add("smallSplatLouder");
            for (int index = 0; index < 3; ++index)
            {
                SpriteMap spriteMap = new SpriteMap("barrelSmoke", 8, 8);
                spriteMap.AddAnimation("idle", 0.12f, true, 3, 4, 5, 6, 7, 8);
                spriteMap.SetAnimation("idle");
                spriteMap.frame = Rando.Int(5);
                spriteMap.center = new Vec2(1f, 8f);
                spriteMap.alpha = 0.5f;
                _flavourLines.Add(spriteMap);
            }
            holsterAngle = -90f;
        }

        protected override bool OnDestroy(DestroyType type = null) => false;

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
            {
                OnDestroy(new DTShot(bullet));
                velocity += bullet.travelDirNormalized * 0.7f;
                vSpeed -= 0.5f;
            }
            SFX.Play("smallSplat", Rando.Float(0.8f, 1f), Rando.Float(-0.2f, 0.2f));
            Level.Add(new WetEnterEffect(hitPos.x, hitPos.y, -bullet.travelDirNormalized, this));
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos) => Level.Add(new WetPierceEffect(exitPos.x, exitPos.y, bullet.travelDirNormalized, this));

        public override void Update()
        {
            --_timeHot;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (_hotAlpha <= 0)
                return;
            if (_timeHot <= 0 && MonoMain.UpdateLerpState)
                _hotAlpha -= 0.01f;
            float num = 0f;
            if (offDir < 0)
                num = -2f;
            for (int index = 0; index < 3; ++index)
            {
                _flavourLines[index].SkipIntraTick = SkipIntratick;
                _flavourLines[index].depth = depth;
                _flavourLines[index].color = Color.White * _hotAlpha;
                SpriteMap g = _flavourLines[index];
                Graphics.Draw(ref g, x - 4f + index * 4 + num, y - 3f);
            }
        }
    }
}
