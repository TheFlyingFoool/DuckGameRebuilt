// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterFallEdge
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class WaterFallEdge : Thing
    {
        public WaterFallEdge(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = (Sprite)new SpriteMap("waterFallEdge", 16, 16);
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(8f, 8f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.layer = Layer.Foreground;
            this.depth = (Depth)0.9f;
            this.alpha = 0.8f;
        }

        public override void Draw()
        {
            (this.graphic as SpriteMap).frame = (int)((double)Graphics.frame / 3.0 % 4.0);
            this.graphic.flipH = this.offDir <= (sbyte)0;
            base.Draw();
        }
    }
}
