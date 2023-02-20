// Decompiled with JetBrains decompiler
// Type: DuckGame.FloorWindow
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class FloorWindow : Window
    {
        public FloorWindow(float xpos, float ypos)
          : base(xpos, ypos)
        {
            angle = -((float)Math.PI / 2.0f);
            collisionSize = new Vec2(32f, 6f);
            collisionOffset = new Vec2(-16f, -2f);
            _editorName = "Floor Window";
            editorTooltip = "When you really want to see what's underneath your house.";
            _editorIcon = new Sprite("windowIconHorizontal");
            center = new Vec2(2f, 16f);
            editorOffset = new Vec2(8f, -6f);
            floor = true;
            UpdateHeight();
        }
    }
}
