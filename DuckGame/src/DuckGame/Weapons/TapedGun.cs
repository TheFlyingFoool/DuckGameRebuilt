using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isFatal", false)]
    public class TapedGun : Gun // There are alot of Ghost Sync Issue with these and other clients but i guess they will stay for now
    {
        public StateBinding _gun1Binding = new StateBinding(nameof(gun1));
        public StateBinding _gun2Binding = new StateBinding(nameof(gun2));
        private Holdable _gun1;
        private Holdable _gun2;
        private int tapeDepth;
        public bool taping = true;
        private Sprite _tape;
        private bool _firstCalc = true;

        public override bool immobilizeOwner
        {
            get
            {
                if (gun1 != null && gun1.immobilizeOwner)
                    return true;
                return gun2 != null && gun2.immobilizeOwner;
            }
            set => _immobilizeOwner = value;
        }

        public Holdable gun1
        {
            get => _gun1;
            set
            {
                if (_gun1 != null)
                {
                    _gun1.owner = null;
                    _gun1.enablePhysics = true;
                    _gun1.tape = null;
                }
                _gun1 = value;
                if (_gun1 != null)
                {
                    _gun1.owner = this;
                    _gun1.enablePhysics = false;
                    _gun1.tape = this;
                }
                UpdateGunOwners();
            }
        }

        public Holdable gun2
        {
            get => _gun2;
            set
            {
                if (_gun2 != null)
                {
                    _gun2.owner = null;
                    _gun2.enablePhysics = true;
                    _gun2.tape = null;
                }
                _gun2 = value;
                if (_gun2 != null)
                {
                    _gun2.owner = this;
                    _gun2.enablePhysics = false;
                    _gun2.tape = this;
                }
                UpdateGunOwners();
            }
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("gun1", gun1 != null ? gun1.Serialize() : (object)(BinaryClassChunk)null);
            binaryClassChunk.AddProperty("gun2", gun2 != null ? gun2.Serialize() : (object)(BinaryClassChunk)null);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            BinaryClassChunk property1 = node.GetProperty<BinaryClassChunk>("gun1");
            if (property1 != null)
            {
                gun1 = LoadThing(property1, false) as Holdable;
                if (gun1 != null)
                    gun1.tape = this;
            }
            BinaryClassChunk property2 = node.GetProperty<BinaryClassChunk>("gun2");
            if (property2 != null)
            {
                gun2 = LoadThing(property2, false) as Holdable;
                if (gun2 != null)
                    gun2.tape = this;
            }
            return base.Deserialize(node);
        }

        public override void Initialize()
        {
            if (gun1 != null)
                level.AddThing(gun1);
            if (gun2 != null)
                level.AddThing(gun2);
            base.Initialize();
        }

        public override bool action => !taping && base.action;

        public TapedGun(float xval, float yval)
          : base(xval, yval)
        {
            _ammoType = new ATDefault();
            ammo = 99;
            _type = "gun";
            graphic = new Sprite("tinyGun");
            _barrelOffsetTL = new Vec2(20f, 15f);
            _fireSound = "littleGun";
            _kickForce = 0f;
            _fireRumble = RumbleIntensity.Kick;
            _tape = new Sprite("tapePiece");
            _tape.CenterOrigin();
        }
        public void ReorderTapedGunsByPreference()
        {
            if (gun1 != null && gun2 != null)
            {
                //Swap indexes if the two guns prefer to be taped a certain way
                if (gun1.tapedIndexPreference >= 0 && gun1.tapedIndexPreference != 0)
                {
                    Holdable g2 = gun2;
                    gun2 = gun1;
                    gun1 = g2;
                }
                else if (gun2.tapedIndexPreference >= 0 && gun2.tapedIndexPreference != 1)
                {
                    Holdable g1 = gun1;
                    gun1 = gun2;
                    gun2 = g1;
                }
            }
        }
        public void UpdateChildren()
        {
            if (gun2 != null)
                gun2.UpdateTaped(this);
            if (gun1 == null)
                return;
            gun1.UpdateTaped(this);
            if (!gun1.removeFromLevel && (taping || gun2 == null || !gun2.removeFromLevel))
                return;
            if (!gun1.removeFromLevel)
                gun1.owner = owner;
            if (gun2 != null && !gun2.removeFromLevel)
                gun2.owner = owner;
            gun1.tape = null;
            if (gun2 != null)
                gun2.tape = null;
            gun1 = null;
            gun2 = null;
            Level.Remove(this);
        }

        public void PreUpdatePositioning()
        {
            if (gun1 != null)
                gun1.PreUpdateTapedPositioning(this);
            if (gun2 == null)
                return;
            gun2.PreUpdateTapedPositioning(this);
        }

        public void UpdatePositioning()
        {
            ammo = 0;
            buoyancy = 0f;
            onlyFloatInLava = false;
            flammable = 0f;
            weight = 0f;
            heat = 0f;
            bouncy = 0f;
            kick = 0f;
            dontCrush = true;
            float num1 = gun1 is TapedGun || gun2 is TapedGun ? 4f : 3f;
            float num2 = 0f;
            if (gun1 != null)
            {
                num2 += gun1.height;
                gun1.tape = this;
            }
            if (gun2 != null)
            {
                num2 += gun2.height;
                taping = false;
                gun2.tape = this;
            }
            float y1 = num2 / 6f;
            float y2 = num2 - y1 * 2f;
            if (gun1 != null)
            {
                if (gun1.angleMul != 1f)
                    angleMul = gun1.angleMul;
                if (gun1.addVerticalTapeOffset)
                    gun1.position = Offset(new Vec2(0f, y1) + gun1.tapedOffset);
                else
                    gun1.position = Offset(gun1.tapedOffset);
                gun1.depth = depth - 8;
                if (taping)
                {
                    gun1.angleDegrees = angleDegrees + 90 * offDir;
                    gun1.offDir = offDir;
                }
                else
                {
                    gun1.angleDegrees = angleDegrees - 180f;
                    gun1.offDir = (sbyte)-offDir;
                }
                gun1.grounded = grounded;
                gun1.hSpeed = hSpeed;
                gun1.vSpeed = vSpeed;
                gun1.lastGrounded = lastGrounded;
                gun1.clip = new HashSet<MaterialThing>(clip);
                if (gun1 is TapedGun && gun1 != this) //If a thing is taped to itself somehow its just gonna softlock
                {
                    (gun1 as TapedGun).tapeDepth = tapeDepth + 1;
                    (gun1 as TapedGun).UpdatePositioning();
                }
                if (!gun1.removeFromLevel)
                {
                    ammo += gun1 is Gun ? (gun1 as Gun).ammo : 1;
                    buoyancy += gun1.buoyancy;
                    if (gun1.onlyFloatInLava)
                        onlyFloatInLava = true;
                    flammable += gun1.flammable;
                    weight = Math.Max(gun1.weight, weight);
                    heat += gun1.heat / 2f;
                    if (!gun1.dontCrush && gun1.weight >= 5)
                        dontCrush = false;
                }
                gun1.UpdateTapedPositioning(this);
                if (!(gun1 is Gun))
                    gun1._extraOffset.y = (float)(1 - (Math.Sin(Maths.DegToRad(gun1.angleDegrees + 90f)) + 1) / 2) * (gun1.collisionOffset.y + gun1.collisionSize.y + gun1.collisionOffset.y);
                if (gun1 is Gun)
                    kick += (gun1 as Gun).kick;
                if (gun1.bouncy > bouncy)
                    bouncy = gun1.bouncy;
            }
            if (gun2 != null)
            {
                if (gun2.angleMul != 1)
                    angleMul = gun2.angleMul;
                if (gun2.addVerticalTapeOffset)
                    gun2.position = Offset(new Vec2(0f, -y1) + gun2.tapedOffset);
                else
                    gun2.position = Offset(gun2.tapedOffset);
                gun2.depth = depth - 4;
                gun2.angleDegrees = angleDegrees;
                gun2.offDir = offDir;
                gun2.grounded = grounded;
                gun2.hSpeed = hSpeed;
                gun2.vSpeed = vSpeed;
                gun2.lastGrounded = lastGrounded;
                gun2.clip = new HashSet<MaterialThing>(clip);
                if (gun2 is TapedGun && gun2 != this)
                {
                    (gun2 as TapedGun).tapeDepth = tapeDepth + 1;
                    (gun2 as TapedGun).UpdatePositioning();
                }
                if (!gun2.removeFromLevel)
                {
                    ammo += gun2 is Gun ? (gun2 as Gun).ammo : 1;
                    buoyancy += gun2.buoyancy;
                    if (gun2.onlyFloatInLava)
                        onlyFloatInLava = true;
                    flammable += gun2.flammable;
                    weight = Math.Max(gun2.weight, weight);
                    heat += gun2.heat / 2f;
                    if (!gun2.dontCrush && gun2.weight >= 5f)
                        dontCrush = false;
                }
                gun2.UpdateTapedPositioning(this);
                if (!(gun2 is Gun))
                    gun2._extraOffset.y = (float)(1 - (Math.Sin(Maths.DegToRad(gun2.angleDegrees + 90f)) + 1) / 2) * (gun2.collisionOffset.y + gun2.collisionSize.y + gun2.collisionOffset.y);
                if (gun2 is Gun)
                    kick += (gun2 as Gun).kick;
                if (gun2.bouncy > bouncy)
                    bouncy = gun2.bouncy;
            }
            if (ammo > 100)
                ammo = 100;
            if (weight > 8)
                weight = 8f;
            center = new Vec2(16f, 16f);
            if (gun1 != null && gun2 != null)
            {
                if (!_firstCalc)
                    return;
                gun1._extraOffset.y = (float)(1 - (Math.Sin(Maths.DegToRad(gun1.angleDegrees + 90f)) + 1) / 2) * (gun1.collisionOffset.y + gun1.collisionSize.y + gun1.collisionOffset.y);
                gun2._extraOffset.y = (float)(1 - (Math.Sin(Maths.DegToRad(gun2.angleDegrees + 90f)) + 1) / 2) * (gun2.collisionOffset.y + gun2.collisionSize.y + gun2.collisionOffset.y);
                float num3 = Math.Min(gun1.top - gun1._extraOffset.y, gun2.top - gun2._extraOffset.y);
                float num4 = Math.Max(gun1.bottom - gun1._extraOffset.y, gun2.bottom - gun2._extraOffset.y);
                collisionOffset = new Vec2(-6f, -(y - num3));
                collisionSize = new Vec2(12f, num4 - num3);
                _firstCalc = false;
            }
            else
            {
                collisionOffset = new Vec2(-6f, (float)-(y2 / 2));
                collisionSize = new Vec2(12f, y2);
            }
        }

        public override void Terminate()
        {
            if (gun1 != null)
                Level.Remove(gun1);
            if (gun2 != null)
                Level.Remove(gun2);
            base.Terminate();
        }

        public void UpdateSubActions(bool pAction)
        {
            UpdatePositioning();
            if (gun1 != null)
            {
                gun1.triggerAction = pAction;
                gun1.UpdateAction();
            }
            if (gun2 == null)
                return;
            gun2.triggerAction = pAction;
            gun2.UpdateAction();
        }

        private void UpdateGunOwners()
        {
            if (duck != null)
            {
                if (taping)
                    duck.resetAction = true;
                if (gun1 != null)
                    gun1.owner = duck;
                if (gun2 == null)
                    return;
                gun2.owner = duck;
            }
            else
            {
                if (gun1 != null)
                    gun1.owner = this;
                if (gun2 == null)
                    return;
                gun2.owner = this;
            }
        }

        public override void Update()
        {
            UpdateGunOwners();
            if (isServerForObject)
            {
                if (gun1 != null && !gun1.isServerForObject)
                    Fondle(gun1);
                if (gun2 != null && !gun2.isServerForObject)
                    Fondle(gun2);
            }
            try
            {
                if (isServerForObject)
                {
                    if (taping)
                    {
                        if (duck != null)
                        {
                            if (duck.inputProfile != null)
                            {
                                if (duck.inputProfile.Pressed(Triggers.Shoot))
                                {
                                    Holdable holdable1 = Level.current.NearestThingFilter<Holdable>(position, t =>
                                   {
                                       if (t.owner == null && t != this && t != gun1)
                                       {
                                           switch (t)
                                           {
                                               case Equipment _:
                                               case RagdollPart _:
                                               case TapedGun _:
                                                   break;
                                               default:
                                                   if ((t as Holdable).tapeable)
                                                       return gun1 == null || gun1.CanTapeTo(t);
                                                   break;
                                           }
                                       }
                                       return false;
                                   });
                                    if (Distance(holdable1) < 16)
                                    {
                                        if (DGRSettings.S_ParticleMultiplier != 0)
                                        {
                                            Level.Add(SmallSmoke.New(position.x, position.y));
                                            Level.Add(SmallSmoke.New(position.x, position.y));
                                        }
                                        SFX.PlaySynchronized("equip", 0.8f);
                                        ExtraFondle(holdable1, connection);
                                        gun2 = holdable1;
                                        gun2.owner = duck;
                                        taping = false;
                                        if (duck != null)
                                            duck.resetAction = true;
                                        Holdable holdable2 = gun1.BecomeTapedMonster(this);
                                        if (holdable2 != null)
                                        {
                                            Fondle(holdable2, DuckNetwork.localConnection);
                                            holdable2.position = position;
                                            Level.Add(holdable2);
                                            if (duck != null)
                                                duck.GiveHoldable(holdable2);
                                            Fondle(this, DuckNetwork.localConnection);
                                            Fondle(gun1, DuckNetwork.localConnection);
                                            Fondle(gun2, DuckNetwork.localConnection);
                                            Level.Remove(gun1);
                                            Level.Remove(gun2);
                                            Level.Remove(this);
                                        }
                                        else if (gun1.tapedIndexPreference >= 0 && gun1.tapedIndexPreference != 0)
                                        {
                                            Holdable gun2 = this.gun2;
                                            this.gun2 = gun1;
                                            gun1 = gun2;
                                        }
                                        else if (gun2.tapedIndexPreference >= 0)
                                        {
                                            if (gun2.tapedIndexPreference != 1)
                                            {
                                                Holdable gun1 = this.gun1;
                                                this.gun1 = gun2;
                                                gun2 = gun1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Duct Tape exception TapedGun.Update:");
                DevConsole.Log(DCSection.General, ex.ToString());
            }
            PreUpdatePositioning();
            base.Update();
            UpdatePositioning();
            UpdateChildren();
            DoFloat();
        }

        public override void Draw()
        {
            if (duck != null && taping)
                duck.resetAction = true;
            UpdatePositioning();
            _tape.depth = depth + 16;
            _tape.angleDegrees = angleDegrees;
            _tape.flipH = offDir < 0;
            //Vec2 vec2_1 = new Vec2(0f, bottom - top); what -NiK0
            if (gun2 != null)
            {
                Vec2 vec2_2 = gun2.Offset(new Vec2(0f, (float)-(collisionOffset.y / 2f)));
                Graphics.Draw(ref _tape, vec2_2.x, vec2_2.y);
            }
            else Graphics.Draw(ref _tape, position.x, position.y);
            if (level != null && !Duck.renderingIcon) return;
            if (gun1 != null) gun1.Draw();
            if (gun2 == null) return;
            gun2.Draw();
        }

        public override void Burn(Vec2 firePosition, Thing litBy)
        {
            try
            {
                if (gun1 != null && gun1.flammable > 0f) gun1.Burn(firePosition, litBy);
                if (gun2 == null || gun2.flammable <= 0f) return;
                gun2.Burn(firePosition, litBy);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Duct Tape exception TapedGun.Burn:");
                DevConsole.Log(DCSection.General, ex.ToString());
            }
        }

        public override void DoHeatUp(float val, Vec2 location)
        {
            try
            {
                if (gun1 != null) gun1.DoHeatUp(val, location);
                if (gun2 == null) return;
                gun2.DoHeatUp(val, location);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Duct Tape exception TapedGun.DoHeatUp:");
                DevConsole.Log(DCSection.General, ex.ToString());
            }
        }

        public override void OnPressAction() => base.OnPressAction();

        public override void Fire()
        {
        }

        public override void Impact(MaterialThing with, ImpactedFrom from, bool solidImpact)
        {
            try
            {
                if (gun1 != null)
                {
                    gun1.hSpeed = hSpeed;
                    gun1.vSpeed = vSpeed;
                    gun1.Impact(with, from, solidImpact);
                }
                if (gun2 != null)
                {
                    gun2.hSpeed = hSpeed;
                    gun2.vSpeed = vSpeed;
                    gun2.Impact(with, from, solidImpact);
                }
                base.Impact(with, from, solidImpact);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "Duct Tape exception TapedGun.Impact:");
                DevConsole.Log(DCSection.General, ex.ToString());
            }
        }
    }
}
