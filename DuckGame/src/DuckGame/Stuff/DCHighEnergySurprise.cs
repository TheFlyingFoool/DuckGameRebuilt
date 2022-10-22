// Decompiled with JetBrains decompiler
// Type: DuckGame.DCHighEnergySurprise
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class DCHighEnergySurprise : DeathCrateSetting
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
                EnergyScimitar energyScimitar1 = new EnergyScimitar(c.x, c.y - 8f);
                Level.Add(energyScimitar1);
                energyScimitar1.StartFlying(TileConnection.Left);
                EnergyScimitar energyScimitar2 = new EnergyScimitar(c.x, c.y - 8f);
                Level.Add(energyScimitar2);
                energyScimitar2.StartFlying(TileConnection.Right);
                EnergyScimitar energyScimitar3 = new EnergyScimitar(c.x, c.y - 8f);
                Level.Add(energyScimitar3);
                energyScimitar3.StartFlying(TileConnection.Up);
                EnergyScimitar energyScimitar4 = new EnergyScimitar(c.x, c.y - 8f);
                Level.Add(energyScimitar4);
                energyScimitar4.StartFlying(TileConnection.Down);
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(c.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
