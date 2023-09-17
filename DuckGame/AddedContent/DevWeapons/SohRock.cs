using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
#if DEBUG
    [EditorGroup("Rebuilt|DEV")]
    [BaggedProperty("canSpawn", false)]
#endif
    public class SohRock : Rock
    {
        public SohRock(float xpos,float ypos) : base(xpos, ypos)
        {
            tapeable = false;
        }
        public override void Draw()
        {
            if (Graphics.currentRenderTarget != null)
            {
                Graphics.material = null;
            }
            base.Draw();
        }
        public override void Update()
        {
            if (isServerForObject)
            {
                IEnumerable<Thing> clips = Level.current.things[typeof(IPlatform)];
                foreach (Thing t in clips)
                {
                    if (t is MaterialThing mt) clip.Add(mt);
                }
                vSpeed += 0.1f;
            }
            _collisionSize = new Vec2(1280, 12);
            _collisionOffset = new Vec2(-640, -5f);
            base.Update();
        }
    }
}
