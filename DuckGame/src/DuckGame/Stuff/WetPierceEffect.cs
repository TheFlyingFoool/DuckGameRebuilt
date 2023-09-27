// Decompiled with JetBrains decompiler
// Type: DuckGame.WetPierceEffect
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WetPierceEffect : Thing
    {
        private SpriteMap _sprite;

        public WetPierceEffect(float xpos, float ypos, Vec2 dir, Thing attach)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wetParticle", 16, 16);
            _sprite.AddAnimation("splash", 0.45f, false, 0, 1, 2, 3);
            _sprite.SetAnimation("splash");
            center = new Vec2(0f, 7f);
            graphic = _sprite;
            depth = (Depth)0.7f;
            alpha = 0.6f;
            angle = Maths.DegToRad(-Maths.PointDirection(Vec2.Zero, dir));
            anchor = new Anchor(attach)
            {
                offset = new Vec2(xpos, ypos) - attach.position
            };
        }

        public override void Update()
        {
            //since the lifetime of this particle is tied to its animation when its being culled the animation doesn't progress
            //so instead its just getting called here so it can delete properly -NiK0
            if (currentlyDrawing) _sprite.UpdateFrame();
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
