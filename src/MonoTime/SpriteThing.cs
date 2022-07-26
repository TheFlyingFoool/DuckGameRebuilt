// Decompiled with JetBrains decompiler
// Type: DuckGame.SpriteThing
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.collisionSize = new Vec2((float)spr.width, (float)spr.height);
            this.center = new Vec2((float)(spr.w / 2), (float)(spr.h / 2));
            this.collisionOffset = new Vec2((float)-(spr.w / 2), (float)-(spr.h / 2));
            this.color = Color.White;
        }

        public override void Draw()
        {
            this.graphic.flipH = this.flipHorizontal;
            this.graphic.color = this.color;
            base.Draw();
        }
    }
}
