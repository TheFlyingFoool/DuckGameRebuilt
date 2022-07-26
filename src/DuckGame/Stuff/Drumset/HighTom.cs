// Decompiled with JetBrains decompiler
// Type: DuckGame.HighTom
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class HighTom : Drum
    {
        public HighTom(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("drumset/smallTom");
            this.center = new Vec2((float)(this.graphic.w / 2), (float)(this.graphic.h / 2));
            this._sound = "hiTom";
        }

        public override void Draw() => base.Draw();
    }
}
