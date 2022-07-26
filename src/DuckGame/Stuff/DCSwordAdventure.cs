// Decompiled with JetBrains decompiler
// Type: DuckGame.DCSwordAdventure
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class DCSwordAdventure : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float x = c.x;
            float ypos = c.y - 2f;
            Level.Add((Thing)new ExplosionPart(x, ypos));
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add((Thing)new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2));
            }
            if (server)
            {
                for (int index = 0; index < 8; ++index)
                {
                    Sword sword = new Sword(c.x, c.y);
                    sword.hSpeed = (float)((double)((float)index / 7f) * 30.0 - 15.0) * Rando.Float(0.5f, 1f);
                    sword.vSpeed = Rando.Float(-10f, 10f);
                    sword._wasLifted = true;
                    sword._framesExisting = 16;
                    Level.Add((Thing)sword);
                }
                Level.Remove((Thing)c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(c.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
