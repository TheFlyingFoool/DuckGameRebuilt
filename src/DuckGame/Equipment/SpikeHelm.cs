// Decompiled with JetBrains decompiler
// Type: DuckGame.SpikeHelm
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Equipment")]
    public class SpikeHelm : Helmet
    {
        public StateBinding _pokedBinding = new StateBinding(nameof(poked));
        protected List<System.Type> _pokables = new List<System.Type>();
        public PhysicsObject poked;
        public PhysicsObject oldPoke;
        public float oldPokeCooldown;
        private Depth _pokedOldDepth;
        private Duck _prevDuckOwner;
        private Duck _filteredDuck;
        private int throwCooldown;
        private Vec2 prevPoke;
        private Ragdoll prevRagdoll;

        public SpikeHelm(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new SpriteMap("spikehelm", 17, 22, 0);
            this._sprite = new SpriteMap("spikehelmWorn", 17, 22);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(9f, 10f);
            this._hasUnequippedCenter = true;
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(11f, 10f);
            this._equippedCollisionOffset = new Vec2(-4f, -2f);
            this._equippedCollisionSize = new Vec2(11f, 12f);
            this._hasEquippedCollision = true;
            this.strappedOn = true;
            this._sprite.center = new Vec2(8f, 10f);
            this.depth = (Depth)0.0001f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._isArmor = true;
            this._equippedThickness = 3f;
            this._pokables.Add(typeof(Crate));
            this._pokables.Add(typeof(YellowBarrel));
            this._pokables.Add(typeof(BlueBarrel));
            this._pokables.Add(typeof(ExplosiveBarrel));
            this._pokables.Add(typeof(LavaBarrel));
            this._pokables.Add(typeof(CookedDuck));
            this._pokables.Add(typeof(ECrate));
            this._pokables.Add(typeof(TV));
            this._pokables.Add(typeof(RagdollPart));
            this._pokables.Add(typeof(Present));
            this._pokables.Add(typeof(IceBlock));
            this._pokables.Add(typeof(Desk));
            this._pokables.Add(typeof(DeathCrate));
            this.SetEditorName("Pokeyhead");
            this.editorTooltip = "Looks sharp!";
        }

        public override void Crush(Thing pWith)
        {
            if (this.poked != null || this._pokables.Contains(pWith.GetType()))
                return;
            this.crushed = true;
        }

        private Vec2 spikePoint => this.Offset(new Vec2(0f, -8f));

        private Vec2 spikeDir => this.OffsetLocal(new Vec2(0f, -8f)).normalized;

        public override bool action => this.poked == null && this._owner != null && this._owner.action;

        public override void Update()
        {
            if (this.isServerForObject && this.equippedDuck != null && this.poked == null && !this.crushed)
            {
                if (this.throwCooldown > 0)
                    --this.throwCooldown;
                if (this.equippedDuck.GetHeldByDuck() == null && this._prevDuckOwner != null)
                {
                    this.throwCooldown = 20;
                    this._filteredDuck = this._prevDuckOwner;
                }
                this._prevDuckOwner = this.equippedDuck.GetHeldByDuck();
                IEnumerable<MaterialThing> materialThings = Level.CheckRectAll<MaterialThing>(this.spikePoint + new Vec2(-2f, -2f), this.spikePoint + new Vec2(2f, 2f));
                Vec2 spikeDir = this.spikeDir;
                Vec2 velocity = this.equippedDuck.velocity;
                if (this.equippedDuck.ragdoll != null && this.equippedDuck.ragdoll.part1 != null)
                    velocity = this.equippedDuck.ragdoll.part1.velocity;
                else if (this.equippedDuck._trapped != null)
                    velocity = this.equippedDuck._trapped.velocity;
                foreach (MaterialThing materialThing1 in materialThings)
                {
                    Vec2 vec2 = velocity - materialThing1.velocity;
                    if (materialThing1 != this && materialThing1 != this.equippedDuck && materialThing1 != this.oldPoke && (materialThing1.velocity.length >= 0.5 || materialThing1 is IAmADuck) && Vec2.Dot(vec2.normalized, spikeDir) >= 0.649999976158142 && vec2.length >= 1.5 && this._equippedDuck != null)
                    {
                        if (materialThing1 is IAmADuck)
                        {
                            if (materialThing1 is RagdollPart)
                            {
                                RagdollPart ragdollPart = materialThing1 as RagdollPart;
                                if (ragdollPart.doll != null && ragdollPart.doll.part1 != null && ragdollPart.doll.part2 != null && ragdollPart.doll.part3 != null)
                                {
                                    ragdollPart.doll.part2.hSpeed += this.equippedDuck.hSpeed * 0.75f;
                                    ragdollPart.doll.part2.vSpeed += this.equippedDuck.vSpeed * 0.75f;
                                    ragdollPart.doll.part1.hSpeed += this.equippedDuck.hSpeed * 0.75f;
                                    ragdollPart.doll.part1.vSpeed += this.equippedDuck.vSpeed * 0.75f;
                                    this.equippedDuck.clip.Add(ragdollPart.doll.part1);
                                    this.equippedDuck.clip.Add(ragdollPart.doll.part2);
                                    this.equippedDuck.clip.Add(ragdollPart.doll.part3);
                                }
                            }
                            MaterialThing materialThing2 = materialThing1;
                            if (materialThing2 != null)
                            {
                                if (!(materialThing1 is Duck) || !(materialThing1 as Duck).HasEquipment(typeof(Boots)) || (materialThing1 as Duck).sliding || spikeDir.y >= 0.5 || Math.Abs(spikeDir.x) >= 0.200000002980232)
                                {
                                    Duck associatedDuck = Duck.GetAssociatedDuck(materialThing2);
                                    if ((associatedDuck == null || associatedDuck != this._equippedDuck && (this._equippedDuck == null || !this._equippedDuck.IsOwnedBy(associatedDuck))) && (associatedDuck != this._filteredDuck || this.throwCooldown <= 0))
                                    {
                                        Thing.Fondle(materialThing2, DuckNetwork.localConnection);
                                        materialThing2.Destroy(new DTImpale(this));
                                        continue;
                                    }
                                    continue;
                                }
                                continue;
                            }
                        }
                        if (this._pokables.Contains(materialThing1.GetType()) && materialThing1 is PhysicsObject && materialThing1.owner == null)
                        {
                            materialThing1.owner = this;
                            this.poked = materialThing1 as PhysicsObject;
                            this.poked.enablePhysics = false;
                            this._pokedOldDepth = this.poked.depth;
                            if (this.poked is Holdable)
                            {
                                (this.poked as Holdable)._hasOldDepth = true;
                                (this.poked as Holdable)._oldDepth = this.poked.depth;
                            }
                            if (materialThing1 is YellowBarrel)
                                (materialThing1 as YellowBarrel).MakeHole(this.spikePoint, spikeDir);
                            materialThing1.PlayCollideSound(ImpactedFrom.Top);
                            this.Fondle(poked);
                            this.prevRagdoll = this.equippedDuck.ragdoll;
                            break;
                        }
                    }
                }
            }
            this.prevPoke = this.spikePoint;
            if (this._equippedDuck == null)
            {
                this.center = new Vec2(9f, 10f);
                this.depth = (Depth)0.0001f;
                this.collisionOffset = new Vec2(-6f, -4f);
                this.collisionSize = new Vec2(11f, 10f);
            }
            base.Update();
            if (oldPokeCooldown > 0.0)
            {
                this.oldPokeCooldown -= Maths.IncFrameTimer();
                if (oldPokeCooldown <= 0.0)
                    this.oldPoke = null;
            }
            if (this.poked == null || !this.isServerForObject)
                return;
            if (!this.poked.isServerForObject)
                this.Fondle(poked);
            this.poked.position = this.Offset(new Vec2(1f, -9f));
            this.poked.lastGrounded = DateTime.Now;
            this.poked.visible = false;
            this.poked.solid = false;
            this.poked.grounded = true;
            if (this.poked.removeFromLevel || this.poked.y < level.topLeft.y - 2000.0 || !this.poked.active)
            {
                this.ReleasePokedObject();
            }
            else
            {
                if (this.equippedDuck == null)
                    return;
                this.poked.hSpeed = this.duck.hSpeed;
                this.poked.vSpeed = this.duck.vSpeed;
                if (this.equippedDuck.ragdoll == null)
                    this.poked.solid = this.equippedDuck.velocity.length < 0.0500000007450581;
                if (this.equippedDuck.ragdoll != null && this.prevRagdoll == null)
                    this.ReleasePokedObject();
                this.prevRagdoll = this.equippedDuck.ragdoll;
            }
        }

        private void ReleasePokedObject()
        {
            if (this.poked != null)
            {
                this.poked.hSpeed = 0f;
                this.poked.vSpeed = -2f;
                this.poked.y += 8f;
                this.poked.owner = null;
                this.poked.enablePhysics = true;
                this.poked.depth = this._pokedOldDepth;
                this.poked.visible = true;
                this.poked.solid = true;
                this.poked.grounded = false;
                this.poked.angle = 0f;
                this.oldPoke = this.poked;
                this.oldPokeCooldown = 0.5f;
            }
            this.poked = null;
        }

        public override void Draw()
        {
            int frame = this._sprite.frame;
            this._sprite.frame = this.crushed ? 1 : 0;
            (this._pickupSprite as SpriteMap).frame = this._sprite.frame;
            base.Draw();
            this._sprite.frame = frame;
            (this._pickupSprite as SpriteMap).frame = frame;
            if (this.poked == null)
                return;
            this.poked.position = this.Offset(new Vec2(1f, -9f));
            this.poked.depth = this.depth + 2;
            this.poked.angle = this._sprite.angle;
            this.poked.Draw();
        }
    }
}
