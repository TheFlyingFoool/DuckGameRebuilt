using System;

namespace DuckGame
{
    public class DCNetCity : DeathCrateSetting
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
                for (int index = 0; index < 16; ++index)
                {
                    float deg = index * 22.5f + Rando.Float(-8f, 8f);
                    float num3 = Rando.Float(8f, 14f);
                    Net net = new Net(c.x, c.y, null)
                    {
                        hSpeed = (float)Math.Cos(Maths.DegToRad(deg)) * num3,
                        vSpeed = (float)-Math.Sin(Maths.DegToRad(deg)) * num3
                    };
                    Level.Add(net);
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(c.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
