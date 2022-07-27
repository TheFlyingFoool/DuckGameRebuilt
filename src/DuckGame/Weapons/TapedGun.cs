// Decompiled with JetBrains decompiler
// Type: DuckGame.TapedGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isFatal", false)]
    public class TapedGun : Gun
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
                if (this.gun1 != null && this.gun1.immobilizeOwner)
                    return true;
                return this.gun2 != null && this.gun2.immobilizeOwner;
            }
            set => this._immobilizeOwner = value;
        }

        public Holdable gun1
        {
            get => this._gun1;
            set
            {
                if (this._gun1 != null)
                {
                    this._gun1.owner = null;
                    this._gun1.enablePhysics = true;
                    this._gun1.tape = null;
                }
                this._gun1 = value;
                if (this._gun1 != null)
                {
                    this._gun1.owner = this;
                    this._gun1.enablePhysics = false;
                    this._gun1.tape = this;
                }
                this.UpdateGunOwners();
            }
        }

        public Holdable gun2
        {
            get => this._gun2;
            set
            {
                if (this._gun2 != null)
                {
                    this._gun2.owner = null;
                    this._gun2.enablePhysics = true;
                    this._gun2.tape = null;
                }
                this._gun2 = value;
                if (this._gun2 != null)
                {
                    this._gun2.owner = this;
                    this._gun2.enablePhysics = false;
                    this._gun2.tape = this;
                }
                this.UpdateGunOwners();
            }
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("gun1", this.gun1 != null ? this.gun1.Serialize() : (object)(BinaryClassChunk)null);
            binaryClassChunk.AddProperty("gun2", this.gun2 != null ? this.gun2.Serialize() : (object)(BinaryClassChunk)null);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            BinaryClassChunk property1 = node.GetProperty<BinaryClassChunk>("gun1");
            if (property1 != null)
            {
                this.gun1 = Thing.LoadThing(property1, false) as Holdable;
                if (this.gun1 != null)
                    this.gun1.tape = this;
            }
            BinaryClassChunk property2 = node.GetProperty<BinaryClassChunk>("gun2");
            if (property2 != null)
            {
                this.gun2 = Thing.LoadThing(property2, false) as Holdable;
                if (this.gun2 != null)
                    this.gun2.tape = this;
            }
            return base.Deserialize(node);
        }

        public override void Initialize()
        {
            if (this.gun1 != null)
                this.level.AddThing(gun1);
            if (this.gun2 != null)
                this.level.AddThing(gun2);
            base.Initialize();
        }

        public override bool action => !this.taping && base.action;

        public TapedGun(float xval, float yval)
          : base(xval, yval)
        {
            this._ammoType = new ATDefault();
            this.ammo = 99;
            this._type = "gun";
            this.graphic = new Sprite("tinyGun");
            this._barrelOffsetTL = new Vec2(20f, 15f);
            this._fireSound = "littleGun";
            this._kickForce = 0.0f;
            this._fireRumble = RumbleIntensity.Kick;
            this._tape = new Sprite("tapePiece");
            this._tape.CenterOrigin();
        }

        public void UpdateChildren()
        {
            if (this.gun2 != null)
                this.gun2.UpdateTaped(this);
            if (this.gun1 == null)
                return;
            this.gun1.UpdateTaped(this);
            if (!this.gun1.removeFromLevel && (this.taping || this.gun2 == null || !this.gun2.removeFromLevel))
                return;
            if (!this.gun1.removeFromLevel)
                this.gun1.owner = this.owner;
            if (this.gun2 != null && !this.gun2.removeFromLevel)
                this.gun2.owner = this.owner;
            this.gun1.tape = null;
            if (this.gun2 != null)
                this.gun2.tape = null;
            this.gun1 = null;
            this.gun2 = null;
            Level.Remove(this);
        }

        public void PreUpdatePositioning()
        {
            if (this.gun1 != null)
                this.gun1.PreUpdateTapedPositioning(this);
            if (this.gun2 == null)
                return;
            this.gun2.PreUpdateTapedPositioning(this);
        }

        public void UpdatePositioning()
        {
            this.ammo = 0;
            this.buoyancy = 0.0f;
            this.onlyFloatInLava = false;
            this.flammable = 0.0f;
            this.weight = 0.0f;
            this.heat = 0.0f;
            this.bouncy = 0.0f;
            this.kick = 0.0f;
            this.dontCrush = true;
            float num1 = this.gun1 is TapedGun || this.gun2 is TapedGun ? 4f : 3f;
            float num2 = 0.0f;
            if (this.gun1 != null)
            {
                num2 += this.gun1.height;
                this.gun1.tape = this;
            }
            if (this.gun2 != null)
            {
                num2 += this.gun2.height;
                this.taping = false;
                this.gun2.tape = this;
            }
            float y1 = num2 / 6f;
            float y2 = num2 - y1 * 2f;
            if (this.gun1 != null)
            {
                if (gun1.angleMul != 1.0)
                    this.angleMul = this.gun1.angleMul;
                if (this.gun1.addVerticalTapeOffset)
                    this.gun1.position = this.Offset(new Vec2(0.0f, y1) + this.gun1.tapedOffset);
                else
                    this.gun1.position = this.Offset(this.gun1.tapedOffset);
                this.gun1.depth = this.depth - 8;
                if (this.taping)
                {
                    this.gun1.angleDegrees = this.angleDegrees + 90 * offDir;
                    this.gun1.offDir = this.offDir;
                }
                else
                {
                    this.gun1.angleDegrees = this.angleDegrees - 180f;
                    this.gun1.offDir = (sbyte)-this.offDir;
                }
                this.gun1.grounded = this.grounded;
                this.gun1.hSpeed = this.hSpeed;
                this.gun1.vSpeed = this.vSpeed;
                this.gun1.lastGrounded = this.lastGrounded;
                this.gun1.clip = new HashSet<MaterialThing>(clip);
                if (this.gun1 is TapedGun)
                {
                    (this.gun1 as TapedGun).tapeDepth = this.tapeDepth + 1;
                    (this.gun1 as TapedGun).UpdatePositioning();
                }
                if (!this.gun1.removeFromLevel)
                {
                    this.ammo += this.gun1 is Gun ? (this.gun1 as Gun).ammo : 1;
                    this.buoyancy += this.gun1.buoyancy;
                    if (this.gun1.onlyFloatInLava)
                        this.onlyFloatInLava = true;
                    this.flammable += this.gun1.flammable;
                    this.weight = Math.Max(this.gun1.weight, this.weight);
                    this.heat += this.gun1.heat / 2f;
                    if (!this.gun1.dontCrush && (double)this.gun1.weight >= 5.0)
                        this.dontCrush = false;
                }
                this.gun1.UpdateTapedPositioning(this);
                if (!(this.gun1 is Gun))
                    this.gun1._extraOffset.y = (float)(1.0 - (Math.Sin((double)Maths.DegToRad(this.gun1.angleDegrees + 90f)) + 1.0) / 2.0) * (this.gun1.collisionOffset.y + this.gun1.collisionSize.y + this.gun1.collisionOffset.y);
                if (this.gun1 is Gun)
                    this.kick += (this.gun1 as Gun).kick;
                if ((double)this.gun1.bouncy > (double)this.bouncy)
                    this.bouncy = this.gun1.bouncy;
            }
            if (this.gun2 != null)
            {
                if (gun2.angleMul != 1.0)
                    this.angleMul = this.gun2.angleMul;
                if (this.gun2.addVerticalTapeOffset)
                    this.gun2.position = this.Offset(new Vec2(0.0f, -y1) + this.gun2.tapedOffset);
                else
                    this.gun2.position = this.Offset(this.gun2.tapedOffset);
                this.gun2.depth = this.depth - 4;
                this.gun2.angleDegrees = this.angleDegrees;
                this.gun2.offDir = this.offDir;
                this.gun2.grounded = this.grounded;
                this.gun2.hSpeed = this.hSpeed;
                this.gun2.vSpeed = this.vSpeed;
                this.gun2.lastGrounded = this.lastGrounded;
                this.gun2.clip = new HashSet<MaterialThing>(clip);
                if (this.gun2 is TapedGun)
                {
                    (this.gun2 as TapedGun).tapeDepth = this.tapeDepth + 1;
                    (this.gun2 as TapedGun).UpdatePositioning();
                }
                if (!this.gun2.removeFromLevel)
                {
                    this.ammo += this.gun2 is Gun ? (this.gun2 as Gun).ammo : 1;
                    this.buoyancy += this.gun2.buoyancy;
                    if (this.gun2.onlyFloatInLava)
                        this.onlyFloatInLava = true;
                    this.flammable += this.gun2.flammable;
                    this.weight = Math.Max(this.gun2.weight, this.weight);
                    this.heat += this.gun2.heat / 2f;
                    if (!this.gun2.dontCrush && (double)this.gun2.weight >= 5.0)
                        this.dontCrush = false;
                }
                this.gun2.UpdateTapedPositioning(this);
                if (!(this.gun2 is Gun))
                    this.gun2._extraOffset.y = (float)(1.0 - (Math.Sin((double)Maths.DegToRad(this.gun2.angleDegrees + 90f)) + 1.0) / 2.0) * (this.gun2.collisionOffset.y + this.gun2.collisionSize.y + this.gun2.collisionOffset.y);
                if (this.gun2 is Gun)
                    this.kick += (this.gun2 as Gun).kick;
                if ((double)this.gun2.bouncy > (double)this.bouncy)
                    this.bouncy = this.gun2.bouncy;
            }
            if (this.ammo > 100)
                this.ammo = 100;
            if ((double)this.weight > 8.0)
                this.weight = 8f;
            this.center = new Vec2(16f, 16f);
            if (this.gun1 != null && this.gun2 != null)
            {
                if (!this._firstCalc)
                    return;
                this.gun1._extraOffset.y = (float)(1.0 - (Math.Sin((double)Maths.DegToRad(this.gun1.angleDegrees + 90f)) + 1.0) / 2.0) * (this.gun1.collisionOffset.y + this.gun1.collisionSize.y + this.gun1.collisionOffset.y);
                this.gun2._extraOffset.y = (float)(1.0 - (Math.Sin((double)Maths.DegToRad(this.gun2.angleDegrees + 90f)) + 1.0) / 2.0) * (this.gun2.collisionOffset.y + this.gun2.collisionSize.y + this.gun2.collisionOffset.y);
                float num3 = Math.Min(this.gun1.top - this.gun1._extraOffset.y, this.gun2.top - this.gun2._extraOffset.y);
                float num4 = Math.Max(this.gun1.bottom - this.gun1._extraOffset.y, this.gun2.bottom - this.gun2._extraOffset.y);
                this.collisionOffset = new Vec2(-6f, -(this.y - num3));
                this.collisionSize = new Vec2(12f, num4 - num3);
                this._firstCalc = false;
            }
            else
            {
                this.collisionOffset = new Vec2(-6f, (float)-((double)y2 / 2.0));
                this.collisionSize = new Vec2(12f, y2);
            }
        }

        public override void Terminate()
        {
            if (this.gun1 != null)
                Level.Remove(gun1);
            if (this.gun2 != null)
                Level.Remove(gun2);
            base.Terminate();
        }

        public void UpdateSubActions(bool pAction)
        {
            this.UpdatePositioning();
            if (this.gun1 != null)
            {
                this.gun1.triggerAction = pAction;
                this.gun1.UpdateAction();
            }
            if (this.gun2 == null)
                return;
            this.gun2.triggerAction = pAction;
            this.gun2.UpdateAction();
        }

        private void UpdateGunOwners()
        {
            if (this.duck != null)
            {
                if (this.taping)
                    this.duck.resetAction = true;
                if (this.gun1 != null)
                    this.gun1.owner = duck;
                if (this.gun2 == null)
                    return;
                this.gun2.owner = duck;
            }
            else
            {
                if (this.gun1 != null)
                    this.gun1.owner = this;
                if (this.gun2 == null)
                    return;
                this.gun2.owner = this;
            }
        }

        public override void Update()
        {
            this.UpdateGunOwners();
            if (this.isServerForObject)
            {
                if (this.gun1 != null && !this.gun1.isServerForObject)
                    this.Fondle(gun1);
                if (this.gun2 != null && !this.gun2.isServerForObject)
                    this.Fondle(gun2);
            }
            try
            {
                if (this.isServerForObject)
                {
                    if (this.taping)
                    {
                        if (this.duck != null)
                        {
                            if (this.duck.inputProfile != null)
                            {
                                if (this.duck.inputProfile.Pressed("SHOOT"))
                                {
                                    Holdable holdable1 = Level.current.NearestThingFilter<Holdable>(this.position, t =>
                                   {
                                       if (t.owner == null && t != this && t != this.gun1)
                                       {
                                           switch (t)
                                           {
                                               case Equipment _:
                                               case RagdollPart _:
                                               case TapedGun _:
                                                   break;
                                               default:
                                                   if ((t as Holdable).tapeable)
                                                       return this.gun1 == null || this.gun1.CanTapeTo(t);
                                                   break;
                                           }
                                       }
                                       return false;
                                   });
                                    if ((double)this.Distance(holdable1) < 16.0)
                                    {
                                        Level.Add(SmallSmoke.New(this.position.x, this.position.y));
                                        Level.Add(SmallSmoke.New(this.position.x, this.position.y));
                                        SFX.PlaySynchronized("equip", 0.8f);
                                        Thing.ExtraFondle(holdable1, this.connection);
                                        this.gun2 = holdable1;
                                        this.gun2.owner = duck;
                                        this.taping = false;
                                        if (this.duck != null)
                                            this.duck.resetAction = true;
                                        Holdable holdable2 = this.gun1.BecomeTapedMonster(this);
                                        if (holdable2 != null)
                                        {
                                            Thing.Fondle(holdable2, DuckNetwork.localConnection);
                                            holdable2.position = this.position;
                                            Level.Add(holdable2);
                                            if (this.duck != null)
                                                this.duck.GiveHoldable(holdable2);
                                            Thing.Fondle(this, DuckNetwork.localConnection);
                                            Thing.Fondle(gun1, DuckNetwork.localConnection);
                                            Thing.Fondle(gun2, DuckNetwork.localConnection);
                                            Level.Remove(gun1);
                                            Level.Remove(gun2);
                                            Level.Remove(this);
                                        }
                                        else if (this.gun1.tapedIndexPreference >= 0 && this.gun1.tapedIndexPreference != 0)
                                        {
                                            Holdable gun2 = this.gun2;
                                            this.gun2 = this.gun1;
                                            this.gun1 = gun2;
                                        }
                                        else if (this.gun2.tapedIndexPreference >= 0)
                                        {
                                            if (this.gun2.tapedIndexPreference != 1)
                                            {
                                                Holdable gun1 = this.gun1;
                                                this.gun1 = this.gun2;
                                                this.gun2 = gun1;
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
            this.PreUpdatePositioning();
            base.Update();
            this.UpdatePositioning();
            this.UpdateChildren();
            this.DoFloat();
        }

        public override void Draw()
        {
            if (this.duck != null && this.taping)
                this.duck.resetAction = true;
            this.UpdatePositioning();
            this._tape.depth = this.depth + 16;
            this._tape.angleDegrees = this.angleDegrees;
            this._tape.flipH = this.offDir < 0;
            Vec2 vec2_1 = new Vec2(0.0f, this.bottom - this.top);
            if (this.gun2 != null)
            {
                Vec2 vec2_2 = this.gun2.Offset(new Vec2(0.0f, (float)-(collisionOffset.y / 2.0)));
                Graphics.Draw(this._tape, vec2_2.x, vec2_2.y);
            }
            else
                Graphics.Draw(this._tape, this.position.x, this.position.y);
            if (this.level != null && !Duck.renderingIcon)
                return;
            if (this.gun1 != null)
                this.gun1.Draw();
            if (this.gun2 == null)
                return;
            this.gun2.Draw();
        }

        public override void Burn(Vec2 firePosition, Thing litBy)
        {
            try
            {
                if (this.gun1 != null && gun1.flammable > 0.0)
                    this.gun1.Burn(firePosition, litBy);
                if (this.gun2 == null || gun2.flammable <= 0.0)
                    return;
                this.gun2.Burn(firePosition, litBy);
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
                if (this.gun1 != null)
                    this.gun1.DoHeatUp(val, location);
                if (this.gun2 == null)
                    return;
                this.gun2.DoHeatUp(val, location);
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
                if (this.gun1 != null)
                {
                    this.gun1.hSpeed = this.hSpeed;
                    this.gun1.vSpeed = this.vSpeed;
                    this.gun1.Impact(with, from, solidImpact);
                }
                if (this.gun2 != null)
                {
                    this.gun2.hSpeed = this.hSpeed;
                    this.gun2.vSpeed = this.vSpeed;
                    this.gun2.Impact(with, from, solidImpact);
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
