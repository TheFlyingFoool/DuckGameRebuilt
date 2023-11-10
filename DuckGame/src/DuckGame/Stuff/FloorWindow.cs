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
