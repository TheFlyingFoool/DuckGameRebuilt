// Decompiled with JetBrains decompiler
// Type: DuckGame.GrenadeExplosion
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class GrenadeExplosion : Thing
    {
        private int _explodeFrames = -1;

        public GrenadeExplosion(float xpos, float ypos)
          : base(xpos, ypos)
        {
        }

        public override void Update()
        {
            if (this._explodeFrames < 0)
            {
                float x = this.x;
                float ypos = this.y - 2f;
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
                this._explodeFrames = 4;
            }
            else
            {
                --this._explodeFrames;
                if (this._explodeFrames != 0)
                    return;
                float x = this.x;
                float num3 = this.y - 2f;
                List<Bullet> varBullets = new List<Bullet>();
                for (int index = 0; index < 20; ++index)
                {
                    float num4 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                    ATPropExplosion type = new ATPropExplosion();
                    type.range = 60f + Rando.Float(18f);
                    Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num4)) * 6.0), num3 - (float)(Math.Sin((double)Maths.DegToRad(num4)) * 6.0), (AmmoType)type, num4);
                    bullet.firedFrom = (Thing)this;
                    varBullets.Add(bullet);
                    Level.Add((Thing)bullet);
                }
                if (Network.isActive)
                {
                    Send.Message((NetMessage)new NMExplodingProp(varBullets), NetMessagePriority.ReliableOrdered);
                    varBullets.Clear();
                }
                if (Options.Data.flashing)
                {
                    Graphics.flashAdd = 1.3f;
                    Layer.Game.darken = 1.3f;
                }
                foreach (Window ignore in Level.CheckCircleAll<Window>(this.position, 40f))
                {
                    if (Level.CheckLine<Block>(this.position, ignore.position, (Thing)ignore) == null)
                        ignore.Destroy((DestroyType)new DTImpact((Thing)this));
                }
                SFX.Play("explode");
                Level.Remove((Thing)this);
            }
        }
    }
}
