// Decompiled with JetBrains decompiler
// Type: DuckGame.DCRomanCandle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class DCRomanCandle : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
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
            if (server)
            {
                for (int index = 0; index < 3; ++index)
                {
                    Thing thing;
                    if (index == 1)
                    {
                        thing = new FireExtinguisher(c.x, c.y);
                    }
                    else
                    {
                        thing = new RomanCandle(c.x, c.y);
                        (thing as Gun).PressAction();
                    }
                    float num3 = index / 2f;
                    thing.hSpeed = (float)(num3 * 30.0 - 15.0) * Rando.Float(0.5f, 1f);
                    thing.vSpeed = Rando.Float(-3f, -11f);
                    Level.Add(thing);
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(c.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
