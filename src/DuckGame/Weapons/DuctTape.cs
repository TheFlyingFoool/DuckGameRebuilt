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
            _type = "gun";
            graphic = new Sprite("tape");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(10f, 10f);
            wearable = false;
            _editorName = "Tape";
            editorTooltip = "Taping things together is always a good time!";
        }

        public override void PressAction()
        {
            try
            {
                if (!isServerForObject)
                    return;
                Holdable holdable = Level.current.NearestThingFilter<Holdable>(position, t =>
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
                }, 16.0f);
                if (Distance(holdable) >= 16.0f)
                    return;
                Level.Add(SmallSmoke.New(position.x, position.y));
                Level.Add(SmallSmoke.New(position.x, position.y));
                SFX.PlaySynchronized("equip", 0.8f);
                TapedGun h = new TapedGun(0f, 0f);
                Thing.ExtraFondle(holdable, connection);
                h.gun1 = holdable;
                holdable.owner = duck;
                Level.Add(h);
                if (duck != null && held)
                {
                    duck.resetAction = true;
                    duck.GiveHoldable(h);
                }
                Level.Remove(this);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Duct Tape exception DuctTape.PressAction:");
                DevConsole.Log(DCSection.General, ex.ToString());
            }
        }
    }
}
