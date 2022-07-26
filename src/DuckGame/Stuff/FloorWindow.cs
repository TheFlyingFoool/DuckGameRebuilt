// Decompiled with JetBrains decompiler
// Type: DuckGame.FloorWindow
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class FloorWindow : Window
    {
        public FloorWindow(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.angle = -1.570796f;
            this.collisionSize = new Vec2(32f, 6f);
            this.collisionOffset = new Vec2(-16f, -2f);
            this._editorName = "Floor Window";
            this.editorTooltip = "When you really want to see what's underneath your house.";
            this._editorIcon = new Sprite("windowIconHorizontal");
            this.center = new Vec2(2f, 16f);
            this.editorOffset = new Vec2(8f, -6f);
            this.floor = true;
            this.UpdateHeight();
        }
    }
}
