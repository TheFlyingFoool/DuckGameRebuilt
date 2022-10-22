// Decompiled with JetBrains decompiler
// Type: DuckGame.SpriteThing
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class SpriteThing : Thing
    {
        public DuckPersona persona;
        public Color color;

        public SpriteThing(float xpos, float ypos, Sprite spr)
          : base(xpos, ypos, spr)
        {
            collisionSize = new Vec2(spr.width, spr.height);
            center = new Vec2(spr.w / 2, spr.h / 2);
            collisionOffset = new Vec2(-(spr.w / 2), -(spr.h / 2));
            color = Color.White;
        }

        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            graphic.color = color;
            base.Draw();
        }
    }
}
