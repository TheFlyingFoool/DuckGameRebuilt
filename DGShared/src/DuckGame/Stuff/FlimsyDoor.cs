// Decompiled with JetBrains decompiler
// Type: DuckGame.FlimsyDoor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Pyramid", EditorItemType.Pyramid)]
    public class FlimsyDoor : Door
    {
        public FlimsyDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _editorName = "Flimsy Door";
        }

        public override void Initialize()
        {
            secondaryFrame = true;
            _sprite = new SpriteMap("flimsyDoor", 32, 32);
            graphic = _sprite;
            colWide = 4f;
            base.Initialize();
        }
    }
}
