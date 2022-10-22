// Decompiled with JetBrains decompiler
// Type: DuckGame.MetroidDoor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Doors", EditorItemType.Debug)]
    public class MetroidDoor : VerticalDoor
    {
        public Profile _arcadeProfile;
        //private RenderTarget2D _screenCapture;
        private bool _transitioning;

        public MetroidDoor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _editorName = "Arcadeexit Door";
        }

        public override void Update()
        {
            if (!_transitioning)
            {
                IEnumerable<Thing> thing = level.things[typeof(Duck)];
                if (thing.Count<Thing>() > 0)
                {
                    Duck duck = thing.First<Thing>() as Duck;
                    if (duck.x < x - 5.0)
                    {
                        duck.x -= 10f;
                        MonoMain.transitionDirection = TransitionDirection.Left;
                        MonoMain.transitionLevel = new TitleScreen(true, _arcadeProfile);
                    }
                }
            }
            base.Update();
        }

        public override void Draw() => base.Draw();
    }
}
