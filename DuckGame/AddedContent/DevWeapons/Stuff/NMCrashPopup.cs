namespace DuckGame
{
    [ClientOnly]
    public class NMCrashPopup : NMEvent
    {
        public NMCrashPopup(Duck d)
        {
            duck = d;
        }
        public NMCrashPopup()
        {
        }
        public Duck duck;

        public override void Activate()
        {
            Vec2 v = duck.GetPos();
            v = Layer.HUD.camera.transformInverse(Layer.Game.camera.transform(v));
            Level.Add(new FakeCrashPopup(v.x + 8, v.y));
        }
    }
}
