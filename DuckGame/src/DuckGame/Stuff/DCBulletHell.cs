using System;

namespace DuckGame
{
    public class DCBulletHell : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            if (DGRSettings.ActualParticleMultiplier > 0)
            {
                float x = c.x;
                float ypos = c.y - 2f;
                Level.Add(new ExplosionPart(x, ypos));
                int num1 = 6;
                if (Graphics.effectsLevel < 2)
                    num1 = 3;
                for (int index = 0; index < num1; ++index)
                {
                    float deg = index * 60f + Rando.Float(-10f, 10f);
                    float num2 = Rando.Float(12f, 20f);
                    Level.Add(new ExplosionPart(x + (float)Math.Cos(Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin(Maths.DegToRad(deg)) * num2));
                }
            }
            if (server)
            {
                for (int index = 0; index < 18; ++index)
                {
                    float deg = index * 22.5f;
                    //double num3 = Rando.Float(8f, 14f); what -NiK0
                    Level.Add(new QuadLaserBullet(c.x, c.y, new Vec2((float)Math.Cos(Maths.DegToRad(deg)), (float)-Math.Sin(Maths.DegToRad(deg)))));
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(c.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
