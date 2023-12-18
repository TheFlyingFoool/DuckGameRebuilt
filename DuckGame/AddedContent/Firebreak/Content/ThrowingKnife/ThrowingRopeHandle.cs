using System;

namespace DuckGame
{
    [ClientOnly]
    public sealed class ThrowingRopeHandle : Gun
    {
        private ThrowingKnife _head;
        public const float ROPE_MAX_LENGTH = 16 * 8;
        public bool Retracting;
        private byte _movingFrames;
        private static Tex2D s_laserTex = null;
        
        public ThrowingRopeHandle(ThrowingKnife head) : base(head.x, head.y)
        {
            s_laserTex ??= Content.Load<Tex2D>("pointerLaser");
            
            _head = head;

            graphic = new SpriteMap("quasik", 17, 7);
            ((SpriteMap)graphic).frame = 3;
            center = new Vec2(4, 2.5f);
            collisionOffset = new Vec2(-4, -2f);
            collisionSize = new Vec2(8, 5);
            _holdOffset = new Vec2(-2f, 2f);
            _barrelOffsetTL = new Vec2(6, 4f);

            canStore = false;
            ammo = 1;
        }
        
        public override bool CanTapeTo(Thing pThing)
        {
            return false;
        }

        public override void Draw()
        {
            if (removeFromLevel || inPipe || _head.inPipe)
                return;

            _movingFrames = (byte) ((_movingFrames + 1) % 255);
            Graphics.DrawDottedLine(barrelPosition, _head.position, new Color(147, 64, 221) * Rando.Float(0.75f, 0.85f), 1f, Rando.Float(2f, 22f), depth);
            if (Options.Data.fireGlow)
                // Graphics.DrawDottedLine(barrelPosition, _head.position, new Color(207, 176, 235) * 0.5f, 3f, Rando.Float(2f, 22f), depth);
                Graphics.DrawTexturedLine(s_laserTex, barrelPosition, _head.position, new Color(207, 176, 235) * 0.5f, 1, depth);
            
            base.Draw();
        }

        public override void Update()
        {
            if (removeFromLevel)
                goto Skip;
            
            for (int i = 0; i < 2; i++)
            {
                if (Rando.Int(0, 5) == 0)
                {
                    float lineAngle = (float) Math.Atan2(_head.y - y, _head.x - x);
                    Vec2 lineEnd = position + (Maths.AngleToVec(lineAngle) * Distance(_head) * new Vec2(1, -1));
                    Vec2 randomPos = Vec2.Lerp(position, lineEnd, Rando.Float(0, 1));
                    randomPos += Rando.Vec2(-4, 4, -4, 4);

                    Level.Add(ThrowingKnifeConnectionParticle.New(randomPos.x, randomPos.y, _head));
                }
            }
            
            float ropeLength = Vec2.Distance(position, _head.position);
            if (ropeLength > ROPE_MAX_LENGTH)
            {
                Vec2 appliedVelocity = Vec2.Normalize(_head.position - position);
                if (owner is PhysicsObject d)
                    d.velocity += appliedVelocity * 0.75f;
                else velocity += appliedVelocity / 4;
            }
            
            if (Retracting)
            {
                Vec2 pushback = Vec2.Normalize(position - _head.position);
                _head.velocity = pushback * Math.Max(8, Distance(_head) / 16);
            }
            else if (ropeLength > ROPE_MAX_LENGTH/* * 1.5f*/)
            {
                _head.velocity += Vec2.Normalize(position - _head.position) / 3;
            }

            Skip:
            base.Update();
        }

        public override void OnPressAction()
        {
            if (_head._framesSinceThrown < 2 || Retracting)
                return;

            if (_head.Stuck)
            {
                Vec2 forceDirection = Vec2.Normalize(_head.position - position);
                float force = Math.Max(Vec2.Distance(position, _head.position) / 16, 4);
                
                if (_head.StuckThing is Holdable prop)
                {
                    Vec2 forceAmplifier = new(0.75f, 1f);
                    prop.velocity -= forceDirection * force * forceAmplifier;
                }
                else if (_head.StuckThing is Door door && !door._lockDoor)
                {
                    door.offDir = (sbyte) (forceDirection.x < 0 ? 1 : -1);
                    door.Destroy(new DTImpact(_head));
                }
                else if (owner is Duck d)
                {
                    Vec2 forceAmplifier = new(1, 2);
                    d.velocity += forceDirection * force * forceAmplifier;
                }
                
                for (int i = 0; i < 8; i++)
                {
                    Level.Add(ThrowingKnifeConnectionParticle.New(_head.x, _head.y, this));
                }
            }
            
            Retracting = true;
            _head.Stuck = false;
        }
    }
}