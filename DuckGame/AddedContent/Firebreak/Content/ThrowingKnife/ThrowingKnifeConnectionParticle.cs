using System;

namespace DuckGame
{
    public sealed class ThrowingKnifeConnectionParticle : Thing, IFactory
    {
        public const int MAX_PARTICLE_COUNT = 64;
        public static ThrowingKnifeConnectionParticle[] _sparks = new ThrowingKnifeConnectionParticle[MAX_PARTICLE_COUNT];
        public static int _lastActiveParticleIndex = 0;
        private Thing _target;
        private float life;

        public static ThrowingKnifeConnectionParticle New(float xpos, float ypos, Thing target)
        {
            ThrowingKnifeConnectionParticle throwingKnifeConnectionParticle;
            if (_sparks[_lastActiveParticleIndex] == null)
            {
                throwingKnifeConnectionParticle = new ThrowingKnifeConnectionParticle();
                _sparks[_lastActiveParticleIndex] = throwingKnifeConnectionParticle;
            }
            else
            {
                throwingKnifeConnectionParticle = _sparks[_lastActiveParticleIndex];
            }

            _lastActiveParticleIndex = (_lastActiveParticleIndex + 1) % MAX_PARTICLE_COUNT;
            throwingKnifeConnectionParticle.ResetProperties();
            throwingKnifeConnectionParticle.Init(xpos, ypos, target);
            throwingKnifeConnectionParticle.globalIndex = GetGlobalIndex();
            return throwingKnifeConnectionParticle;
        }

        private ThrowingKnifeConnectionParticle() { }

        private void Init(float xpos, float ypos, Thing target)
        {
            hSpeed = Rando.Float(-1f, 1f);
            vSpeed = Rando.Float(-1f, 1f);
            position.x = xpos;
            position.y = ypos;
            depth = 0.9f;
            life = 0.25f;
            _target = target;
            alpha = 1f;
        }

        public override void Update()
        {
            Vec2 vec2 = position - _target.position;
            float lengthSq = vec2.lengthSq;
            if (lengthSq < 128)
                alpha -= 0.08f;
            hSpeed = Lerp.Float(hSpeed, -vec2.x * 0.7f, 0.15f);
            vSpeed = Lerp.Float(vSpeed, -vec2.y * 0.7f, 0.15f);
            position.x += hSpeed;
            position.y += vSpeed;
            position.x = Lerp.Float(position.x, _target.x, 0.16f);
            position.y = Lerp.Float(position.y, _target.y, 0.16f);
            hSpeed *= Math.Min(1f, lengthSq / 128 + 0.25f);
            vSpeed *= Math.Min(1f, lengthSq / 128 + 0.25f);
            life -= 0.02f;
            if (life < 0)
                alpha -= 0.08f;
            if (alpha < 0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawLine(position, position + velocity.normalized * (velocity.length * 2f), new Color(147, 64, 221) * alpha, depth: depth);
    }
}