using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    //this probably shoudln't be visible on the editor lol -Lucky
    //[EditorGroup("Stuff|Doors", EditorItemType.Debug)]
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
                if (thing.Count() > 0)
                {
                    Duck duck = thing.First() as Duck;
                    if (duck.x < x - 5)
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
