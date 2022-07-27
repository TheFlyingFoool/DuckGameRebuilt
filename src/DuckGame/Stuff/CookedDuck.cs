// Decompiled with JetBrains decompiler
// Type: DuckGame.CookedDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

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
            this.graphic = new Sprite("cookedDuck");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 11f);
            this.depth = -0.5f;
            this.thickness = 0.5f;
            this.weight = 5f;
            this.collideSounds.Add("rockHitGround2", ImpactedFrom.Bottom);
            this.collideSounds.Add("smallSplatLouder");
            for (int index = 0; index < 3; ++index)
            {
                SpriteMap spriteMap = new SpriteMap("barrelSmoke", 8, 8);
                spriteMap.AddAnimation("idle", 0.12f, true, 3, 4, 5, 6, 7, 8);
                spriteMap.SetAnimation("idle");
                spriteMap.frame = Rando.Int(5);
                spriteMap.center = new Vec2(1f, 8f);
                spriteMap.alpha = 0.5f;
                this._flavourLines.Add(spriteMap);
            }
            this.holsterAngle = -90f;
        }

        protected override bool OnDestroy(DestroyType type = null) => false;

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
            {
                this.OnDestroy(new DTShot(bullet));
                this.velocity += bullet.travelDirNormalized * 0.7f;
                this.vSpeed -= 0.5f;
            }
            SFX.Play("smallSplat", Rando.Float(0.8f, 1f), Rando.Float(-0.2f, 0.2f));
            Level.Add(new WetEnterEffect(hitPos.x, hitPos.y, -bullet.travelDirNormalized, this));
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos) => Level.Add(new WetPierceEffect(exitPos.x, exitPos.y, bullet.travelDirNormalized, this));

        public override void Update()
        {
            --this._timeHot;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (_hotAlpha <= 0.0)
                return;
            if (this._timeHot <= 0)
                this._hotAlpha -= 0.01f;
            float num = 0.0f;
            if (this.offDir < 0)
                num = -2f;
            for (int index = 0; index < 3; ++index)
            {
                this._flavourLines[index].depth = this.depth;
                this._flavourLines[index].color = Color.White * this._hotAlpha;
                Graphics.Draw(this._flavourLines[index], this.x - 4f + index * 4 + num, this.y - 3f);
            }
        }
    }
}
