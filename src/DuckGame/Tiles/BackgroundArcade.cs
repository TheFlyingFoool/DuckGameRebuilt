// Decompiled with JetBrains decompiler
// Type: DuckGame.BackgroundArcade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background")]
    public class BackgroundArcade : BackgroundTile
    {
        public BackgroundArcade(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("arcadeBackground", 16, 16, true);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "Arcade";
        }
    }
}
