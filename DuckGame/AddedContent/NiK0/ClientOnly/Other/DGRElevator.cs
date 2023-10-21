using NAudio.MediaFoundation;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Stuff")]
    [BaggedProperty("canSpawn", false)]
    public class DGRElevator : PhysicsObject, IPlatform
    {
        private SpriteMap _sprite;

        public EditorProperty<float> maxSpeed = new EditorProperty<float>(6, null, 0, 6);

        public DGRElevator(float xpos, float ypos)
          : base(xpos, ypos)
        {
            thickness = -1000;
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
            friction = 1000; //dont move :^) -NiK0
            collideSounds.Add("rockHitGround2");
            _editorName = "Elevator";
            _editorIcon = new Sprite("elevatorIcon");
            vMax = 6;
            depth = -1;
            editorTooltip = "Elevators arent used often but in this virtual simulation they can be powered by nothing!";
        }

        public bool goingUp;
        public bool goingDown;

        public int tilGo;

        public EditorProperty<bool> spawnFloating = new EditorProperty<bool>(false);
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
                if (spawnFloating)
                {
                    gravMultiplier = 0;
                    grounded = false;
                    sleeping = false;
                }
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

            if (goingUp || goingDown)
            {
                foreach (PhysicsObject po in Level.CheckRectAll<PhysicsObject>(topLeft, bottomRight))
                {
                    if (po.isServerForObject && po != this)
                    {
                        po.y += vSpeed;
                        po.vSpeed += 0.1f;
                    }
                }
            }
            base.Update();
            if (goingUp)
            {
                if (Level.current is DGRDevHall ddv)
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 0, 0.01f);
                    if (Graphics.fade <= 0)
                    {
                        Level.current = new TitleScreen(ddv._duck.profile);
                    }
                }
                if (isServerForObject)
                {
                    if (y < Level.current.topLeft.y - 300)
                    {
                        goingDown = true;
                        goingUp = false;
                    }
                    if (vSpeed == 0)
                    {
                        goingUp = false;
                    }
                    vSpeed = Lerp.Float(vSpeed, -maxSpeed, 0.1f);
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
                    vSpeed = Lerp.Float(vSpeed, maxSpeed, 0.1f);
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
                if (!goTimer)
                {
                    IEnumerable<RCCar> rcs = Level.CheckRectAll<RCCar>(topLeft + new Vec2(0, 2), bottomRight);
                    foreach (RCCar rc in rcs)
                    {
                        if (rc.isServerForObject && rc.owner == null)
                        {
                            goTimer = true;
                            break;
                        }
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
            
        }
    }
}
