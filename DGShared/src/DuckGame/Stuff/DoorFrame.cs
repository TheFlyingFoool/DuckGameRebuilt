// Decompiled with JetBrains decompiler
// Type: DuckGame.DoorFrame
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DoorFrame : Thing
    {
        public DoorFrame(float xpos, float ypos, bool secondaryFrame)
          : base(xpos, ypos)
        {
            graphic = new Sprite(secondaryFrame ? "pyramidDoorFrame" : "doorFrame");
            center = new Vec2(5f, 26f);
            depth = -0.95f;
            _editorCanModify = false;
            shouldbeinupdateloop = false;
        }
    }
}
