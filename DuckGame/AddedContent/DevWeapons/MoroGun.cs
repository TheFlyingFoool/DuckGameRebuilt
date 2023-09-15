using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class MoroGun : Gun
    {
        public SpriteMap sprite;
        public MoroGun(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 3;
            weight = 9;

            sprite = new SpriteMap("MoroGun", 60, 35);
            graphic = sprite;
            center = new Vec2(30, 17.5f);
            _kickForce = 8f;
            wobble = new aWobbleMaterial(this, 0.2f);
            spawn = new MaterialDev(this, new Color(255, 255, 0));
            collisionSize = new Vec2(49, 13.5f);
            _collisionOffset = new Vec2(-21, -2);
            _holdOffset = new Vec2(0, -10);
        }
        public MaterialDev spawn;
        public float spawnSc;
        public aWobbleMaterial wobble;

        public override void Update()
        {
            base.Update();
        }
        public override void Draw()
        {
            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }

            sprite.imageIndex = 0;
            base.Draw();
            if (spawn.finished && level != null)  
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                sprite.imageIndex = 1;
                alpha = spawnSc;
                Graphics.material = wobble;
                depth -= 1;
                base.Draw();
                depth += 1;
                alpha = 1;
                Graphics.material = null;
            }
        }
    }
}
