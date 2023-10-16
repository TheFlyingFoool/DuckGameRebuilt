using System.Linq;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Stuff")]
    public class BigFan : Block
    {
        public SpriteMap sprite;
        public BigFan(Vec2 position) : base(position.x, position.y)
        {
            sprite = new SpriteMap("BigFan", 32, 32);
            sprite.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            sprite.SetAnimation("idle");
            graphic = sprite;
            center = new Vec2(16, 16);
            collisionOffset = new Vec2(-16, -16);
            collisionSize = new Vec2(32, 13);
            editorOffset = new Vec2(8);
            editorTooltip = "Thats a pretty big fan, maybe even the biggest one.";
            _editorIcon = new Sprite("smallFan");
        }
        public EditorProperty<int> spin = new EditorProperty<int>(1, null, 1, 4, 1);
        public EditorProperty<float> strength = new EditorProperty<float>(0.5f, null, 0.1f, 1.5f, 0.01f);
        public override void EditorUpdate()
        {
            angleDegrees = 90 * (spin - 1);
            switch (spin)
            {
                case 1:
                    collisionOffset = new Vec2(-16, -16);
                    collisionSize = new Vec2(32, 13);
                    break;
                case 2:
                    collisionOffset = new Vec2(3, -16);
                    collisionSize = new Vec2(13, 32);
                    break;
                case 3:
                    collisionOffset = new Vec2(-16, 3);
                    collisionSize = new Vec2(32, 13);
                    break;
                case 4:
                    collisionOffset = new Vec2(-16, -16);
                    collisionSize = new Vec2(13, 32);
                    break;
            }
            base.EditorUpdate();
        }
        public override void Initialize()
        {
            angleDegrees = 90 * (spin - 1);
            switch (spin)
            {
                case 1:
                    collisionOffset = new Vec2(-16, -16);
                    collisionSize = new Vec2(32, 13);
                    break;
                case 2:
                    collisionOffset = new Vec2(3, -16);
                    collisionSize = new Vec2(13, 32);
                    break;
                case 3:
                    collisionOffset = new Vec2(-16, 3);
                    collisionSize = new Vec2(32, 13);
                    break;
                case 4:
                    collisionOffset = new Vec2(-16, -16);
                    collisionSize = new Vec2(13, 32);
                    break;
            }
            base.Initialize();
        }
        public float partTimer;
        public override void Update()
        {
            partTimer += 0.2f * DGRSettings.ActualParticleMultiplier;
            if (partTimer > 1)
            {
                sprite._speed = strength * 3;
                partTimer--;
                Vec2 vv = Offset(new Vec2(Rando.Float(-12, 12), -8));
                Level.Add(new WindSpec(vv, angle - 1.57f, strength + 0.5f) { blockIgnore = this });
            }
            Vec2 v = Maths.AngleToVec(-angle - 1.57f) * -strength;
            List<PhysicsObject> pos = Extensions.ThickCheckLineAll<PhysicsObject>(Offset(new Vec2(0, -8)), Offset(new Vec2(0, -100)), 24, 8).ToList();
            for (int i = 0; i < pos.Count; i++)
            {
                if (pos[i].isServerForObject && Level.CheckLine<Block>(position, pos[i].position, this) == null)
                {
                    pos[i].velocity += v;
                }
            }
            List<PhysicsParticle> pls = Extensions.ThickCheckLineAll<PhysicsParticle>(Offset(new Vec2(0, -8)), Offset(new Vec2(0, -100)), 24, 12).ToList();
            for (int i = 0; i < pls.Count; i++)
            {
                if (pls[i].isServerForObject && Level.CheckLine<Block>(position, pls[i].position, this) == null)
                {
                    pls[i].velocity += v;
                }
            }
            List<MaterialThing> mt = Level.CheckCircleAll<MaterialThing>(Offset(new Vec2(0, 8)), 8).ToList();
            for (int i = 0; i < mt.Count; i++)
            {
                if (mt[i].isServerForObject)
                {
                    mt[i].Destroy(new DTIncinerate(this));
                }
            }
            base.Update();
        }
    }
    public class WindSpec : Thing
    {
        public Block blockIgnore;
        public WindSpec(Vec2 pos, float ang, float strength = 1) : base(pos.x, pos.y)
        {
            rng = Rando.Float(0.02f, 0.05f);
            vec = Maths.AngleToVec(ang) * Rando.Float(2, 4) * strength;
            vec *= new Vec2(1, -1);
            lastpos = position;
        }
        private float rng;
        private Vec2 vec;
        public override void Update()
        {
            alpha -= rng;
            Block b = Level.CheckPoint<Block>(x, y, blockIgnore);
            if (b != null)
            {
                alpha = 0;
            }
            if (alpha <= 0)
            {
                y = 4235;
                Level.Remove(this);
                return;
            }
            lastpos = position;
            position += vec;
        }
        public Vec2 lastpos;
        public override void Draw()
        {
            Graphics.DrawLine(lastpos, position, Color.White * alpha, 1, 1);
        }
    }
}
