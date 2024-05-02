using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class LaserBulletPurple : LaserBullet
    {
        public LaserBulletPurple(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            _thickness = type.bulletThickness;
            _beem = Content.Load<Texture2D>("laserBeamPurple");
        }
    }
}
