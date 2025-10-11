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
            //if (Editor.clientonlycontent)
            //{nvm its ugly -Lucky
            //    graphic.color = Color.Red; //dan peer pressured me into this -Lucky
            //}
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
                if (DGRSettings.S_ParticleMultiplier != 0)
                {
                    Level.Add(SmallSmoke.New(position.x, position.y));
                    Level.Add(SmallSmoke.New(position.x, position.y));
                }
                SFX.PlaySynchronized("equip", 0.8f);
                TapedGun h = new TapedGun(0f, 0f);
                ExtraFondle(holdable, connection);
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
