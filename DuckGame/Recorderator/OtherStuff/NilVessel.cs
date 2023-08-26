using System.Collections.Generic;

namespace DuckGame
{
    //if something has gone terribly wrong a nil vessel will appear in its place
    //it probably means we're overreading or forgot to read a part of the buffer and everything has gone to hell
    public class NilVessel : SomethingSomethingVessel
    {
        private string error;
        public bool display;
        public NilVessel(SomethingSync posSync, List<byte> changeDestroy, string reason) : base(null)
        {
            changeRemove = changeDestroy;
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
            if (posSync != null)
            {
                syncled["position"] = posSync;
            }
            error = reason;
            collisionSize = new Vec2(16, 16);
            _collisionOffset = new Vec2(-8, -8);
            center = new Vec2(8, 8);
        }
        public NilVessel(Vec2 pos, string reason) : base(null)
        {
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
            addVal("position", pos);
            error = reason;
            collisionSize = new Vec2(16, 16);
            _collisionOffset = new Vec2(-8, -8);
            center = new Vec2(8, 8);
        }
        public override void PlaybackUpdate()
        {
             position = (Vec2)valOf("position");
        }
        public override void Draw()
        {
            if (display)
            {
                Graphics.DrawString(error, position, Color.White, 1);
            }
            base.Draw();
        }
    }
}
