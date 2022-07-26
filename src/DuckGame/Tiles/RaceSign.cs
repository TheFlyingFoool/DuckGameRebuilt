// Decompiled with JetBrains decompiler
// Type: DuckGame.RaceSign
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs")]
    public class RaceSign : Thing
    {
        private SpriteMap _sprite;

        public RaceSign(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("raceSign", 32, 32);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(16f, 24f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = - 0.5f;
            this._editorName = "Race Sign";
            this.hugWalls = WallHug.Floor;
        }

        public override void Draw()
        {
            this._sprite.frame = this.offDir > (sbyte)0 ? 1 : 0;
            base.Draw();
        }
    }
}
