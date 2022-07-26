// Decompiled with JetBrains decompiler
// Type: DuckGame.DrumStick
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DrumStick : Thing
    {
        private float _startY;

        public DrumStick(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("drumset/drumStick");
            this.center = new Vec2((float)(this.graphic.w / 2), (float)(this.graphic.h / 2));
            this._startY = ypos;
            this.vSpeed = -3f;
        }

        public override void Update()
        {
            this.angle += 0.6f;
            this.y += this.vSpeed;
            this.vSpeed += 0.2f;
            if ((double)this.y <= (double)this._startY)
                return;
            Level.Remove((Thing)this);
        }
    }
}
