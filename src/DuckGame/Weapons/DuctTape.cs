// Decompiled with JetBrains decompiler
// Type: DuckGame.DuctTape
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isFatal", false)]
    public class DuctTape : Equipment
    {
        public DuctTape(float xval, float yval)
          : base(xval, yval)
        {
            this._type = "gun";
            this.graphic = new Sprite("tape");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(10f, 10f);
            this.wearable = false;
            this._editorName = "Tape";
            this.editorTooltip = "Taping things together is always a good time!";
        }

        public override void PressAction()
        {
            try
            {
                if (!this.isServerForObject)
                    return;
                Holdable holdable = Level.current.NearestThingFilter<Holdable>(this.position, (Predicate<Thing>)(t =>
               {
                   if (t.owner == null && t != this)
                   {
                       switch (t)
                       {
                           case Equipment _:
                           case RagdollPart _:
                           case TapedGun _:
                               break;
                           default:
                               return (t as Holdable).tapeable;
                       }
                   }
                   return false;
               }));
                if ((double)this.Distance((Thing)holdable) >= 16.0)
                    return;
                Level.Add((Thing)SmallSmoke.New(this.position.x, this.position.y));
                Level.Add((Thing)SmallSmoke.New(this.position.x, this.position.y));
                SFX.PlaySynchronized("equip", 0.8f);
                TapedGun h = new TapedGun(0.0f, 0.0f);
                Thing.ExtraFondle((Thing)holdable, this.connection);
                h.gun1 = holdable;
                holdable.owner = (Thing)this.duck;
                Level.Add((Thing)h);
                if (this.duck != null && this.held)
                {
                    this.duck.resetAction = true;
                    this.duck.GiveHoldable((Holdable)h);
                }
                Level.Remove((Thing)this);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Duct Tape exception DuctTape.PressAction:");
                DevConsole.Log(DCSection.General, ex.ToString());
            }
        }
    }
}
