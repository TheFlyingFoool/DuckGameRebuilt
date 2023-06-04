// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterFall
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class WaterFall : Thing
    {
        public WaterFall(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("waterFall", 16, 32);
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-8f, -8f);
            layer = Layer.Foreground;
            depth = (Depth)0.9f;
            alpha = 0.8f;
        }

        public override void Draw()
        {
            (graphic as SpriteMap).frame = (int)(Graphics.frame / 3 % 4);
            graphic.flipH = offDir <= 0;
            base.Draw();
        }
    }
}
