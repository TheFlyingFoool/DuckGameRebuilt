// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class ItemBox : Block, IPathNodeBlocker, IContainAThing
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _containedObjectBinding = new StateBinding(nameof(containedObject));
        public StateBinding _boxStateBinding = (StateBinding)new ItemBoxFlagBinding();
        public StateBinding _chargingBinding = new StateBinding(nameof(charging), 9);
        public StateBinding _netDisarmIndexBinding = new StateBinding(nameof(netDisarmIndex));
        public byte netDisarmIndex;
        public byte localNetDisarm;
        public float bounceAmount;
        public bool _hit;
        public int charging;
        public float startY = -99999f;
        protected List<PhysicsObject> _aboveList = new List<PhysicsObject>();
        private PhysicsObject _containedObject;
        protected SpriteMap _sprite;
        public bool _canBounce = true;
        private int chargeDelay;
        public PhysicsObject lastSpawnItem;
        protected PhysicsObject containContext;

        public PhysicsObject containedObject
        {
            get => this._containedObject;
            set => this._containedObject = value;
        }

        public System.Type contains { get; set; }

        public bool canBounce => this._canBounce;

        public ItemBox(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("itemBox", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.layer = Layer.Foreground;
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.5f;
            this._canFlip = false;
            this._placementCost += 4;
            this.editorTooltip = "Spawns a copy of the contained item any time it's used. Recharges after a short duration.";
        }

        public override void Initialize()
        {
            this.UpdateContainedObject();
            base.Initialize();
        }

        public void Pop()
        {
            this.Bounce();
            if (this._hit)
                return;
            this.SpawnItem();
        }

        public void Bounce()
        {
            if (!this._canBounce)
                return;
            this.bounceAmount = 8f;
            this._canBounce = false;
            if (Network.isActive)
            {
                ++this.netDisarmIndex;
            }
            else
            {
                this._aboveList = Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(1f, -4f), this.bottomRight + new Vec2(-1f, -12f)).ToList<PhysicsObject>();
                foreach (PhysicsObject above in this._aboveList)
                {
                    if (above.grounded || (double)above.vSpeed > 0.0 || (double)above.vSpeed == 0.0)
                    {
                        this.Fondle((Thing)above);
                        above.y -= 2f;
                        above.vSpeed = -3f;
                        if (above is Duck pTarget)
                        {
                            if (!pTarget.isServerForObject)
                                Send.Message((NetMessage)new NMDisarmVertical(pTarget, -3f), pTarget.connection);
                            else
                                pTarget.Disarm((Thing)this);
                        }
                    }
                }
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from != ImpactedFrom.Bottom || !with.isServerForObject)
                return;
            with.Fondle((Thing)this);
            if (with is Duck duck)
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            if (this.containedObject != null)
                with.Fondle((Thing)this.containedObject);
            this.Pop();
        }

        public virtual void UpdateCharging()
        {
            if (!this.isServerForObject)
                return;
            if (this.charging > 0)
            {
                ++this.chargeDelay;
                if (this.chargeDelay < 50)
                    return;
                this.charging -= 50;
                this.chargeDelay = 0;
            }
            else
            {
                this.chargeDelay = 0;
                this.charging = 0;
                this._hit = false;
            }
        }

        public override void PrepareForHost()
        {
            this.UpdateContainedObject();
            if (this.containedObject != null)
                this.containedObject.PrepareForHost();
            base.PrepareForHost();
        }

        public virtual void UpdateContainedObject()
        {
            if (!Network.isActive || !this.isServerForObject && Thing.loadingLevel == null || this.containedObject != null)
                return;
            this.containedObject = this.GetSpawnItem();
            if (this.containedObject == null)
                return;
            this.containedObject.visible = false;
            this.containedObject.active = false;
            this.containedObject.position = this.position;
            Level.Add((Thing)this.containedObject);
        }

        public override void Update()
        {
            this.UpdateContainedObject();
            this._aboveList.Clear();
            if ((double)this.startY < -9999.0)
                this.startY = this.y;
            this._sprite.frame = this._hit ? 1 : 0;
            if (this.contains == (System.Type)null && this.containedObject == null && !(this is ItemBoxRandom))
                this._sprite.frame = 1;
            if ((int)this.netDisarmIndex != (int)this.localNetDisarm)
            {
                this.localNetDisarm = this.netDisarmIndex;
                this._aboveList = Level.CheckRectAll<PhysicsObject>(this.topLeft + new Vec2(1f, -4f), this.bottomRight + new Vec2(-1f, -12f)).ToList<PhysicsObject>();
                foreach (PhysicsObject above in this._aboveList)
                {
                    if (this.isServerForObject && above.owner == null)
                        this.Fondle((Thing)above);
                    if (above.isServerForObject && (above.grounded || (double)above.vSpeed > 0.0 || (double)above.vSpeed == 0.0))
                    {
                        above.y -= 2f;
                        above.vSpeed = -3f;
                        if (above is Duck pTarget)
                        {
                            if (!pTarget.isServerForObject)
                                Send.Message((NetMessage)new NMDisarmVertical(pTarget, -3f), pTarget.connection);
                            else
                                pTarget.Disarm((Thing)this);
                        }
                    }
                }
            }
            this.UpdateCharging();
            if ((double)this.bounceAmount > 0.0)
                this.bounceAmount -= 0.8f;
            else
                this.bounceAmount = 0.0f;
            this.y -= this.bounceAmount;
            if (this._canBounce)
                return;
            if ((double)this.y < (double)this.startY)
                this.y += (float)(0.800000011920929 + (double)Math.Abs(this.y - this.startY) * 0.400000005960464);
            if ((double)this.y > (double)this.startY)
                this.y -= (float)(0.800000011920929 - (double)Math.Abs(this.y - this.startY) * 0.400000005960464);
            if ((double)Math.Abs(this.y - this.startY) >= 0.800000011920929)
                return;
            this._canBounce = true;
            this.y = this.startY;
        }

        public virtual PhysicsObject GetSpawnItem()
        {
            if (this.contains == (System.Type)null)
                return (PhysicsObject)null;
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(this.contains);
            return !Network.isActive || bag.GetOrDefault("isOnlineCapable", true) ? Editor.CreateThing(this.contains) as PhysicsObject : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as PhysicsObject;
        }

        public virtual void SpawnItem()
        {
            this.charging = 500;
            if (this.containContext == null && (!Network.isActive && this.contains == (System.Type)null && !(this is ItemBoxRandom) || Network.isActive && this.containedObject == null))
                return;
            PhysicsObject t = this.containContext;
            if (t == null)
            {
                if (!Network.isActive)
                {
                    t = this.GetSpawnItem();
                }
                else
                {
                    if (this.containedObject == null)
                        return;
                    t = this.containedObject;
                    t.active = true;
                    t.visible = true;
                }
            }
            this._hit = true;
            this.lastSpawnItem = t;
            if (t == null)
                return;
            foreach (PhysicsObject above in this._aboveList)
                t.clip.Add((MaterialThing)above);
            t.x = this.x;
            t.bottom = this.bottom;
            t.y -= 12f;
            t.vSpeed = -3.5f;
            t.clip.Add((MaterialThing)this);
            if (t is Gun)
            {
                Gun gun = t as Gun;
                if (gun.CanSpin())
                    gun.angleDegrees = 180f;
            }
            Block block1 = Level.CheckPoint<Block>(this.position + new Vec2(-16f, 0.0f));
            if (block1 != null)
                t.clip.Add((MaterialThing)block1);
            Block block2 = Level.CheckPoint<Block>(this.position + new Vec2(16f, 0.0f));
            if (block2 != null)
                t.clip.Add((MaterialThing)block2);
            if (!Network.isActive || this is PurpleBlock)
                Level.Add((Thing)t);
            if (!Network.isActive)
                SFX.Play("hitBox");
            else if (this.isServerForObject)
                NetSoundEffect.Play("itemBoxHit");
            Thing.Fondle((Thing)t, DuckNetwork.localConnection);
            this.containedObject = (PhysicsObject)null;
        }

        public static List<System.Type> GetPhysicsObjects(EditorGroup group) => Editor.ThingTypes.Where<System.Type>((Func<System.Type, bool>)(t =>
       {
           if (t.IsAbstract || !t.IsSubclassOf(typeof(PhysicsObject)) || t.GetCustomAttributes(typeof(EditorGroupAttribute), false).Length == 0)
               return false;
           IReadOnlyPropertyBag bag = ContentProperties.GetBag(t);
           return bag.GetOrDefault("canSpawn", true) && (!Network.isActive || !bag.GetOrDefault("noRandomSpawningOnline", false)) && (!Network.isActive || bag.GetOrDefault("isOnlineCapable", true)) && (Main.isDemo || !bag.GetOrDefault("onlySpawnInDemo", false));
       })).ToList<System.Type>();

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", (object)Editor.SerializeTypeName(this.contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("contains", this.contains != (System.Type)null ? (object)this.contains.AssemblyQualifiedName : (object)""));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("contains");
            if (dxmlNode != null)
                this.contains = Editor.GetType(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding((object)this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            return (ContextMenu)contextMenu;
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (this.contains != (System.Type)null)
                str = this.contains.Name;
            return this.contains == (System.Type)null ? base.GetDetailsString() : base.GetDetailsString() + "Contains: " + str;
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (this.contains != (System.Type)null)
                text = this.contains.Name;
            Graphics.DrawString(text, this.position + new Vec2((float)(-(double)Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
        }
    }
}
