// Decompiled with JetBrains decompiler
// Type: DuckGame.BlankDoor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class BlankDoor : Thing
    {
        private BitmapFont _fontSmall;

        public BlankDoor(float pX, float pY)
          : base(pX, pY, new Sprite("blank_door", Vec2.Zero))
        {
            this._fontSmall = new BitmapFont("smallBiosFont", 7, 6);
        }

        public override void Draw()
        {
            this._fontSmall.DrawOutline("DUCK", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, this.depth + 10);
            this._fontSmall.DrawOutline("GAME", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, this.depth + 10);
            base.Draw();
        }
    }
}
