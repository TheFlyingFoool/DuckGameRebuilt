// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterFallEdgeTop
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class WaterFallEdgeTop : Thing
    {
        public WaterFallEdgeTop(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("waterFallEdgeTop", 16, 20);
            center = new Vec2(8f, 12f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-8f, -8f);
            layer = Layer.Foreground;
            depth = (Depth)0.9f;
            alpha = 0.8f;
        }

        public override void Draw()
        {
            (graphic as SpriteMap).frame = (int)(Graphics.frame / 3.0 % 4.0);
            graphic.flipH = offDir <= 0;
            base.Draw();
        }
    }
}
