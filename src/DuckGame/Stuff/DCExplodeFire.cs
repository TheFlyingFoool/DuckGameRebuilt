// Decompiled with JetBrains decompiler
// Type: DuckGame.DCExplodeFire
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class DCExplodeFire : DeathCrateSetting
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
                Level.Add(new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2));
            }
            if (server)
            {
                for (int index = 0; index < 16; ++index)
                    Level.Add(SmallFire.New(c.x - 6f + Rando.Float(12f), c.y - 8f + Rando.Float(4f), Rando.Float(12f) - 6f, 2f - Rando.Float(8.5f), firedFrom: c));
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(c.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
