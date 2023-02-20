// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public StateBinding _boxStateBinding = new ItemBoxFlagBinding();
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
            get => _containedObject;
            set => _containedObject = value;
        }

        public Type contains { get; set; }

        public bool canBounce => _canBounce;

        public ItemBox(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _sprite = new SpriteMap("itemBox", 16, 16);
            graphic = _sprite;
            layer = Layer.Foreground;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            depth = 0.5f;
            _canFlip = false;
            _placementCost += 4;
            editorTooltip = "Spawns a copy of the contained item any time it's used. Recharges after a short duration.";
        }

        public override void Initialize()
        {
            UpdateContainedObject();
            base.Initialize();
        }

        public void Pop()
        {
            Bounce();
            if (_hit)
                return;
            SpawnItem();
        }

        public void Bounce()
        {
            if (!_canBounce)
                return;
            bounceAmount = 8f;
            _canBounce = false;
            if (Network.isActive)
            {
                ++netDisarmIndex;
            }
            else
            {
                _aboveList = Level
                    .CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -4f), bottomRight + new Vec2(-1f, -12f))
                    .ToList();
                foreach (PhysicsObject above in _aboveList)
                {
                    if (above.grounded || above.vSpeed > 0.0 || above.vSpeed == 0.0)
                    {
                        Fondle(above);
                        above.y -= 2f;
                        above.vSpeed = -3f;
                        if (above is Duck pTarget)
                        {
                            if (!pTarget.isServerForObject)
                                Send.Message(new NMDisarmVertical(pTarget, -3f), pTarget.connection);
                            else
                                pTarget.Disarm(this);
                        }
                    }
                }
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (from != ImpactedFrom.Bottom || !with.isServerForObject)
                return;
            with.Fondle(this);
            if (with is Duck duck)
                RumbleManager.AddRumbleEvent(duck.profile,
                    new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            if (containedObject != null)
                with.Fondle(containedObject);
            Pop();
        }

        public virtual void UpdateCharging()
        {
            if (!isServerForObject)
                return;
            if (charging > 0)
            {
                ++chargeDelay;
                if (chargeDelay < 50)
                    return;
                charging -= 50;
                chargeDelay = 0;
            }
            else
            {
                chargeDelay = 0;
                charging = 0;
                _hit = false;
            }
        }

        public override void PrepareForHost()
        {
            UpdateContainedObject();
            if (containedObject != null)
                containedObject.PrepareForHost();
            base.PrepareForHost();
        }

        public virtual void UpdateContainedObject()
        {
            if (!Network.isActive || !isServerForObject && loadingLevel == null || containedObject != null)
                return;
            containedObject = GetSpawnItem();
            if (containedObject == null)
                return;
            containedObject.visible = false;
            containedObject.active = false;
            containedObject.position = position;
            Level.Add(containedObject);
        }

        public override void Update()
        {
            UpdateContainedObject();
            _aboveList.Clear();
            if (startY < -9999.0)
                startY = y;
            _sprite.frame = _hit ? 1 : 0;
            if (contains == null && containedObject == null && !(this is ItemBoxRandom))
                _sprite.frame = 1;
            if (netDisarmIndex != localNetDisarm)
            {
                localNetDisarm = netDisarmIndex;
                _aboveList = Level
                    .CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -4f), bottomRight + new Vec2(-1f, -12f))
                    .ToList();
                foreach (PhysicsObject above in _aboveList)
                {
                    if (isServerForObject && above.owner == null)
                        Fondle(above);
                    if (above.isServerForObject && (above.grounded || above.vSpeed > 0.0 || above.vSpeed == 0.0))
                    {
                        above.y -= 2f;
                        above.vSpeed = -3f;
                        if (above is Duck pTarget)
                        {
                            if (!pTarget.isServerForObject)
                                Send.Message(new NMDisarmVertical(pTarget, -3f), pTarget.connection);
                            else
                                pTarget.Disarm(this);
                        }
                    }
                }
            }

            UpdateCharging();
            if (bounceAmount > 0f)
                bounceAmount -= 0.8f;
            else
                bounceAmount = 0f;
            y -= bounceAmount;
            if (_canBounce)
                return;
            if (y < startY)
                y += 0.8f + Math.Abs(y - startY) * 0.4f;
            if (y > startY)
                y -= 0.8f - Math.Abs(y - startY) * 0.4f;
            if (Math.Abs(y - startY) >= 0.8f)
                return;
            _canBounce = true;
            y = startY;
        }

        public virtual PhysicsObject GetSpawnItem()
        {
            if (contains == null)
                return null;
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(contains);
            return !Network.isActive || bag.GetOrDefault("isOnlineCapable", true)
                ? Editor.CreateThing(contains) as PhysicsObject
                : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as
                    PhysicsObject;
        }

        public virtual void SpawnItem()
        {
            charging = 500;
            if (containContext == null && (!Network.isActive && contains == null && !(this is ItemBoxRandom) ||
                                           Network.isActive && containedObject == null))
                return;
            PhysicsObject t = containContext;
            if (t == null)
            {
                if (!Network.isActive)
                {
                    t = GetSpawnItem();
                }
                else
                {
                    if (containedObject == null)
                        return;
                    t = containedObject;
                    t.active = true;
                    t.visible = true;
                }
            }

            _hit = true;
            lastSpawnItem = t;
            if (t == null)
                return;
            foreach (PhysicsObject above in _aboveList)
                t.clip.Add(above);
            t.x = x;
            t.bottom = bottom;
            if (this is PurpleBlock)
                t.y -= 16f;
            else
                t.y -= 12f;
            t.vSpeed = -3.5f;
            t.clip.Add(this);
            if (t is Gun)
            {
                Gun gun = t as Gun;
                if (gun.CanSpin())
                    gun.angle = 3.14159f;
            }

            Block block1 = Level.CheckPoint<Block>(position + new Vec2(-16f, 0f));
            if (block1 != null)
                t.clip.Add(block1);
            Block block2 = Level.CheckPoint<Block>(position + new Vec2(16f, 0f));
            if (block2 != null)
                t.clip.Add(block2);
            if (!Network.isActive || this is PurpleBlock)
                Level.Add(t);
            if (!Network.isActive)
                SFX.Play("hitBox");
            else if (isServerForObject)
                NetSoundEffect.Play("itemBoxHit");
            Fondle(t, DuckNetwork.localConnection);
            containedObject = null;
        }

        public static List<Type> GetPhysicsObjects(EditorGroup group) => Editor.ThingTypes.Where(t =>
        {
            if (t.IsAbstract
                || !t.IsSubclassOf(typeof(PhysicsObject))
                || t.GetCustomAttributes(typeof(EditorGroupAttribute), false).Length == 0
                || (!Editor.clientonlycontent && t.IsDefined(typeof(ClientOnlyAttribute), false)))
                return false;
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(t);
            return bag.GetOrDefault("canSpawn", true)
                   && (!Network.isActive || !bag.GetOrDefault("noRandomSpawningOnline", false))
                   && (!Network.isActive || bag.GetOrDefault("isOnlineCapable", true))
                   && !bag.GetOrDefault("onlySpawnInDemo",
                       false); //(Main.isDemo || !bag.GetOrDefault("onlySpawnInDemo", false));
        }).ToList();

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("contains", contains != null ? contains.AssemblyQualifiedName : (object)""));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("contains");
            if (dxmlNode != null)
                contains = Editor.GetType(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            return contextMenu;
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (contains != null)
                str = contains.Name;
            return contains == null ? base.GetDetailsString() : base.GetDetailsString() + "Contains: " + str;
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (contains != null)
                text = contains.Name;
            Graphics.DrawString(text, position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2.0), -16f),
                Color.White, 0.9f);
        }
    }
}