using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Equipment")]
    public class SpikeHelm : Helmet
    {
        public StateBinding _pokedBinding = new StateBinding(nameof(poked));
        protected List<Type> _pokables = new List<Type>();
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
            _pickupSprite = new SpriteMap("spikehelm", 17, 22, 0);
            _sprite = new SpriteMap("spikehelmWorn", 17, 22);
            graphic = _pickupSprite;
            center = new Vec2(9f, 10f);
            _hasUnequippedCenter = true;
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 10f);
            _equippedCollisionOffset = new Vec2(-4f, -2f);
            _equippedCollisionSize = new Vec2(11f, 12f);
            _hasEquippedCollision = true;
            strappedOn = true;
            _sprite.center = new Vec2(8f, 10f);
            depth = (Depth)0.0001f;
            physicsMaterial = PhysicsMaterial.Metal;
            _isArmor = true;
            _equippedThickness = 3f;
            _pokables.Add(typeof(Crate));
            _pokables.Add(typeof(YellowBarrel));
            _pokables.Add(typeof(BlueBarrel));
            _pokables.Add(typeof(ExplosiveBarrel));
            _pokables.Add(typeof(LavaBarrel));
            _pokables.Add(typeof(CookedDuck));
            _pokables.Add(typeof(ECrate));
            _pokables.Add(typeof(TV));
            _pokables.Add(typeof(RagdollPart));
            _pokables.Add(typeof(Present));
            _pokables.Add(typeof(IceBlock));
            _pokables.Add(typeof(Desk));
            _pokables.Add(typeof(DeathCrate));
            SetEditorName("Pokeyhead");
            editorTooltip = "Looks sharp!";
        }

        public override void Crush(Thing pWith)
        {
            if (poked != null || _pokables.Contains(pWith.GetType()))
                return;
            crushed = true;
        }

        private Vec2 spikePoint => Offset(new Vec2(0f, -8f));

        private Vec2 spikeDir => OffsetLocal(new Vec2(0f, -8f)).normalized;

        public override bool action => poked == null && _owner != null && _owner.action;

        public override void Update()
        {
            if (poked == this) poked = null;
            if (isServerForObject && equippedDuck != null && poked == null && !crushed)
            {
                if (throwCooldown > 0)
                    --throwCooldown;
                if (equippedDuck.GetHeldByDuck() == null && _prevDuckOwner != null)
                {
                    throwCooldown = 20;
                    _filteredDuck = _prevDuckOwner;
                }
                _prevDuckOwner = equippedDuck.GetHeldByDuck();
                IEnumerable<MaterialThing> materialThings = Level.CheckRectAll<MaterialThing>(spikePoint + new Vec2(-2f, -2f), spikePoint + new Vec2(2f, 2f));
                Vec2 spikeDir = this.spikeDir;
                Vec2 velocity = equippedDuck.velocity;
                if (equippedDuck.ragdoll != null && equippedDuck.ragdoll.part1 != null)
                    velocity = equippedDuck.ragdoll.part1.velocity;
                else if (equippedDuck._trapped != null)
                    velocity = equippedDuck._trapped.velocity;
                foreach (MaterialThing materialThing1 in materialThings)
                {
                    Vec2 vec2 = velocity - materialThing1.velocity;
                    if (materialThing1 != this && materialThing1 != equippedDuck && materialThing1 != oldPoke && (materialThing1.velocity.length >= 0.5f || materialThing1 is IAmADuck) && Vec2.Dot(vec2.normalized, spikeDir) >= 0.65f && vec2.length >= 1.5f && _equippedDuck != null)
                    {
                        if (materialThing1 is IAmADuck)
                        {
                            if (materialThing1 is RagdollPart)
                            {
                                RagdollPart ragdollPart = materialThing1 as RagdollPart;
                                if (ragdollPart.doll != null && ragdollPart.doll.part1 != null && ragdollPart.doll.part2 != null && ragdollPart.doll.part3 != null)
                                {
                                    ragdollPart.doll.part2.hSpeed += equippedDuck.hSpeed * 0.75f;
                                    ragdollPart.doll.part2.vSpeed += equippedDuck.vSpeed * 0.75f;
                                    ragdollPart.doll.part1.hSpeed += equippedDuck.hSpeed * 0.75f;
                                    ragdollPart.doll.part1.vSpeed += equippedDuck.vSpeed * 0.75f;
                                    equippedDuck.clip.Add(ragdollPart.doll.part1);
                                    equippedDuck.clip.Add(ragdollPart.doll.part2);
                                    equippedDuck.clip.Add(ragdollPart.doll.part3);
                                }
                            }
                            MaterialThing materialThing2 = materialThing1;
                            if (materialThing2 != null)
                            {
                                if (!(materialThing1 is Duck) || !(materialThing1 as Duck).HasEquipment(typeof(Boots)) || (materialThing1 as Duck).sliding || spikeDir.y >= 0.5 || Math.Abs(spikeDir.x) >= 0.2f)
                                {
                                    Duck associatedDuck = Duck.GetAssociatedDuck(materialThing2);
                                    if ((associatedDuck == null || associatedDuck != _equippedDuck && (_equippedDuck == null || !_equippedDuck.IsOwnedBy(associatedDuck))) && (associatedDuck != _filteredDuck || throwCooldown <= 0))
                                    {
                                        Fondle(materialThing2, DuckNetwork.localConnection);
                                        materialThing2.Destroy(new DTImpale(this));
                                        continue;
                                    }
                                    continue;
                                }
                                continue;
                            }
                        }
                        if (_pokables.Contains(materialThing1.GetType()) && materialThing1 is PhysicsObject && materialThing1.owner == null)
                        {
                            materialThing1.owner = this;
                            poked = materialThing1 as PhysicsObject;
                            poked.enablePhysics = false;
                            _pokedOldDepth = poked.depth;
                            if (poked is Holdable)
                            {
                                (poked as Holdable)._hasOldDepth = true;
                                (poked as Holdable)._oldDepth = poked.depth;
                            }
                            if (materialThing1 is YellowBarrel)
                                (materialThing1 as YellowBarrel).MakeHole(spikePoint, spikeDir);
                            materialThing1.PlayCollideSound(ImpactedFrom.Top);
                            Fondle(poked);
                            prevRagdoll = equippedDuck.ragdoll;
                            break;
                        }
                    }
                }
            }
            prevPoke = spikePoint;
            if (_equippedDuck == null)
            {
                center = new Vec2(9f, 10f);
                depth = (Depth)0.0001f;
                collisionOffset = new Vec2(-6f, -4f);
                collisionSize = new Vec2(11f, 10f);
            }
            base.Update();
            if (oldPokeCooldown > 0)
            {
                oldPokeCooldown -= Maths.IncFrameTimer();
                if (oldPokeCooldown <= 0)
                    oldPoke = null;
            }
            if (poked == null || !isServerForObject)
                return;
            if (!poked.isServerForObject)
                Fondle(poked);
            poked.position = Offset(new Vec2(1f, -9f));
            poked.lastGrounded = DateTime.Now;
            poked.visible = false;
            poked.solid = false;
            poked.grounded = true;
            if (poked.removeFromLevel || poked.y < level.topLeft.y - 2000 || !poked.active)
            {
                ReleasePokedObject();
            }
            else
            {
                if (equippedDuck == null)
                    return;
                poked.hSpeed = duck.hSpeed;
                poked.vSpeed = duck.vSpeed;
                if (equippedDuck.ragdoll == null)
                    poked.solid = equippedDuck.velocity.length < 0.05f;
                if (equippedDuck.ragdoll != null && prevRagdoll == null)
                    ReleasePokedObject();
                prevRagdoll = equippedDuck.ragdoll;
            }
        }

        public void ReleasePokedObject()
        {
            if (poked != null)
            {
                poked.hSpeed = 0f;
                poked.vSpeed = -2f;
                poked.y += 8f;
                poked.owner = null;
                poked.enablePhysics = true;
                poked.depth = _pokedOldDepth;
                poked.visible = true;
                poked.solid = true;
                poked.grounded = false;
                poked.angle = 0f;
                oldPoke = poked;
                oldPokeCooldown = 0.5f;
            }
            poked = null;
        }

        public override void Draw()
        {
            int frame = _sprite.frame;
            _sprite.frame = crushed ? 1 : 0;
            _sprite.SkipIntraTick = SkipIntratick;
            (_pickupSprite as SpriteMap).frame = _sprite.frame;
            base.Draw();
            _sprite.frame = frame;
            (_pickupSprite as SpriteMap).frame = frame;
            if (poked == null)
                return;
            poked.position = Offset(new Vec2(1f, -9f));
            poked.depth = depth + 2;
            poked.angle = _sprite.angle;
            poked.Draw();
        }
    }
}
