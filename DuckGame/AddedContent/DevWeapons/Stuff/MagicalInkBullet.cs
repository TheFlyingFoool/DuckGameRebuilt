namespace DuckGame
{
    [ClientOnly] //idk if this needs to be clientonly or not im just makin sure -Lucky
    public class MagicalInkBullet : Bullet
    {
        public MagicalInkBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = true,
          bool network = true)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
        }

        protected override void OnHit(bool destroyed)
        {
            if (!destroyed || !isLocal)
                return;
            
            level.AddThing(new MagicalInkSplat(x, y));
        }
    }
}
