// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraBounds
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    public class CameraBounds : Thing
    {
        public EditorProperty<int> wide;
        public EditorProperty<int> high;

        public CameraBounds()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
            wide = new EditorProperty<int>(320, this, 60f, 1920f, 1f);
            high = new EditorProperty<int>(320, this, 60f, 1920f, 1f);
        }

        public override void Draw()
        {
            base.Draw();
            float num1 = wide.value;
            float num2 = high.value;
            Graphics.DrawRect(position + new Vec2((-num1 / 2f), (-num2 / 2f)), position + new Vec2(num1 / 2f, num2 / 2f), Color.Blue * 0.5f, (Depth)1f, false);
        }
    }
}
