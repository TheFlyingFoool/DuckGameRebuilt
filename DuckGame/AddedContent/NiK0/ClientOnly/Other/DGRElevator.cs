using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Stuff")]
    public class DGRElevator : PhysicsObject, IPlatform
    {
        private SpriteMap _sprite;

        public DGRElevator(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _skipPlatforms = true;
            _sprite = new SpriteMap("elevator", 32, 37);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(32, 29);
            editorOffset = new Vec2(0, 3);
            depth = -0.5f;
            thickness = 4f;
            weight = 7f;
            flammable = 0.3f;
            collideSounds.Add("rockHitGround2");
        }

        public bool goingUp;
        public bool goingDown;

        public int tilGo;

        public StateBinding _tilGoBinding = new StateBinding("tilGo");
        public StateBinding _goingUpBinding = new StateBinding("goingUp");
        public StateBinding _goingDownBinding = new StateBinding("goingDown");

        public InvisibleBlock ceiling;
        public InvisibleBlock side;
        public InvisibleBlock floor;
        public override void Update()
        {
            if (floor == null)
            {
                floor = new InvisibleBlock(0, 0, 32, 8);
                side = new InvisibleBlock(0, 0, 3, 37);
                ceiling = new InvisibleBlock(0, 0, 32, 3);
                Level.Add(floor);
                Level.Add(side);
                Level.Add(ceiling);
            }
            clip.Add(ceiling);
            clip.Add(side);
            clip.Add(floor);

            bool goTimer = false;
            IEnumerable<IAmADuck> iaads = Level.CheckRectAll<IAmADuck>(topLeft + new Vec2(0, 2), bottomRight);

            if (goingUp)
            {
                if (isServerForObject)
                {
                    if (vSpeed == 0)
                    {
                        goingUp = false;
                    }
                    vSpeed -= 0.1f;
                }
            }
            else if (goingDown)
            {
                if (isServerForObject)
                {
                    if (vSpeed == 0)
                    {
                        goingDown = false;
                    }
                    vSpeed += 0.1f;
                }
            }
            else
            {
                foreach (IAmADuck iaad in iaads)
                {
                    Duck d = Duck.GetAssociatedDuck((MaterialThing)iaad);
                    if (d != null && !d.dead && d.isServerForObject)
                    {
                        goTimer = true;
                        break;
                    }
                }
            }
            if (goTimer && ((!isServerForObject && tilGo == 0) || isServerForObject) && !(goingUp || goingDown))
            {
                Fondle(this, DuckNetwork.localConnection);
                tilGo++;
                if (tilGo > 60)
                {
                    if (grounded)
                    {
                        goingUp = true;
                        vSpeed = -0.1f;
                        gravMultiplier = 0;
                    }
                    else
                    {
                        goingDown = true;
                        vSpeed = 0.1f;
                        gravMultiplier = 0;
                    }
                }
            }
            else if (isServerForObject) tilGo = 0;
            base.Update();

            ceiling.position = topLeft;
            if (offDir == 1)
            {
                floor.position = Offset(new Vec2(-8, 21));
                side.position = Offset(new Vec2(21, -8));
            }
            else
            {
                floor.position = Offset(new Vec2(24, 21));
                side.position = Offset(new Vec2(24, -8));
            }

            if (goingUp || goingDown)
            {
                foreach (IAmADuck iaad in iaads)
                {
                    MaterialThing mt = (MaterialThing)iaad;
                    if (mt.isServerForObject)
                    {
                        mt.y += vSpeed;
                        mt.vSpeed -= 0.01f;
                    }
                }
            }
        }
    }
}
