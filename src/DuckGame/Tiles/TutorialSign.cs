// Decompiled with JetBrains decompiler
// Type: DuckGame.TutorialSign
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class TutorialSign : Thing
    {
        public TutorialSign(float xpos, float ypos, string image, string name)
          : base(xpos, ypos)
        {
            if (image == null)
                return;
            this.graphic = new Sprite(image);
            this.center = new Vec2(this.graphic.w / 2, this.graphic.h / 2);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = -0.5f;
            this._editorName = name;
            this.layer = Layer.Background;
        }
    }
}
