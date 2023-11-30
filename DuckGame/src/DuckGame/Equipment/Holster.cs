using System;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isInDemo", true)]
    public class Holster : Equipment
    {
        public StateBinding _containedObjectBinding = new StateBinding(nameof(netContainedObject));
        public StateBinding _netRaiseBinding = new StateBinding(nameof(netRaise));
        public StateBinding _netChainedBinding = new StateBinding(nameof(netChained));
        public bool netRaise;
        protected SpriteMap _sprite;
        protected SpriteMap _overPart;
        protected SpriteMap _underPart;
        public EditorProperty<bool> infinite = new EditorProperty<bool>(false);
        public EditorProperty<bool> chained = new EditorProperty<bool>(false);
        private Holdable _containedObject;
        private RenderTarget2D _preview;
        private Sprite _previewSprite;
        private Type _contains;
        private Sprite _chain;
        private Sprite _lock;
        private Vec2 _prevPos = Vec2.Zero;
        private float afterDrawAngle = -999999f;
        private float _chainSway;
        private float _chainSwayVel;
        protected float backOffset = -8f;

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                base.connection = value;
                if (containedObject == null)
                    return;
                containedObject.connection = value;
            }
        }

        public bool netChained
        {
            get => chained.value;
            set => chained.value = value;
        }

        public Holdable containedObject
        {
            get => _containedObject;
            set => _containedObject = value;
        }

        public Holdable netContainedObject
        {
            get => _containedObject;
            set => _containedObject = value;
        }

        public Thing GetContainedInstance(Vec2 pos = default(Vec2))
        {
            if (contains == null)
                return null;
            object[] constructorParameters = Editor.GetConstructorParameters(contains);
            if (constructorParameters.Length > 1)
            {
                constructorParameters[0] = pos.x;
                constructorParameters[1] = pos.y;
            }
            PhysicsObject thing = Editor.CreateThing(contains, constructorParameters) as PhysicsObject;
            if (thing is Gun)
                (thing as Gun).infinite = infinite;
            return thing;
        }

        public void SetContainedObject(Holdable h)
        {
            if (_containedObject != null)
            {
                _containedObject.visible = true;
                Fondle(_containedObject);
                _containedObject.owner = null;
                _containedObject = null;
            }
            if (h == null)
                return;
            _containedObject = h;
            h.lastGrounded = DateTime.Now;
            h.visible = false;
        }

        public virtual void EjectItem()
        {
            if (containedObject == null)
                return;
            SFX.PlaySynchronized("pelletgunBad", pitch: Rando.Float(0.1f, 0.1f));
            containedObject.hSpeed = -owner.offDir * 6f;
            containedObject.vSpeed = -1.5f;
            if (duck != null)
            {
                duck._lastHoldItem = containedObject;
                duck._timeSinceThrow = 0;
            }
            SetContainedObject(null);
        }

        public virtual void EjectItem(Vec2 pSpeed)
        {
            if (containedObject == null)
                return;
            SFX.PlaySynchronized("pelletgunBad", pitch: Rando.Float(0.1f, 0.1f));
            containedObject.hSpeed = pSpeed.x;
            containedObject.vSpeed = pSpeed.y;
            SetContainedObject(null);
        }

        public Type contains
        {
            get => _contains;
            set
            {
                _contains = value;
                if (Level.skipInitialize)
                    return;
                if (_preview == null)
                    _preview = new RenderTarget2D(32, 32);
                Thing containedInstance = GetContainedInstance();
                if (containedInstance == null)
                    return;
                _previewSprite = containedInstance.GetEditorImage(32, 32, true, target: _preview);
            }
        }

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
            Graphics.DrawString(text, position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2), -16f), Color.White, (Depth)0.9f);
        }

        public Holster(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _chain = new Sprite("holsterChain")
            {
                center = new Vec2(3f, 3f)
            };
            _lock = new Sprite("holsterLock")
            {
                center = new Vec2(3f, 2f)
            };
            _sprite = new SpriteMap("holster", 12, 12);
            _overPart = new SpriteMap("holster_over", 10, 3)
            {
                center = new Vec2(6f, -1f)
            };
            _underPart = new SpriteMap("holster_under", 8, 9)
            {
                center = new Vec2(10f, 8f)
            };
            graphic = _sprite;
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(10f, 10f);
            center = new Vec2(6f, 6f);
            physicsMaterial = PhysicsMaterial.Wood;
            _equippedDepth = 4;
            _wearOffset = new Vec2(1f, 1f);
            editorTooltip = "Lets you carry around an additional item!";
        }

        protected override bool OnDestroy(DestroyType type = null) => owner == null && containedObject == null && base.OnDestroy(type);

        public override void Initialize()
        {
            if (!(Level.current is Editor) && GetContainedInstance(position) is Holdable containedInstance)
            {
                Level.Add(containedInstance);
                SetContainedObject(containedInstance);
                if (Network.isActive && loadingLevel != null && _containedObject != null)
                    _containedObject.PrepareForHost();
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (_equippedDuck != null && duck == null) return;
            if (destroyed) alpha -= 0.05f;
            if (alpha < 0f) Level.Remove(this);
            if (isServerForObject)
            {
                netRaise = false;
                if (_equippedDuck != null && _equippedDuck.inputProfile != null && _equippedDuck.inputProfile.Down(Triggers.Up))
                    netRaise = true;
                if (owner == null && equippedDuck == null)
                    angle = 0;
                Vec2 vec2_1;
                if (containedObject != null)
                {
                    PositionContainedObject();
                    _containedObject.HolsterUpdate(this);
                    weight = containedObject.weight;
                    if (duck != null)
                        containedObject.owner = duck;
                    else
                        containedObject.owner = this;
                    if (duck != null && duck.ragdoll != null)
                    {
                        containedObject.solid = false;
                        containedObject.grounded = false;
                    }
                    else
                    {
                        if (equippedDuck != null)
                        {
                            Holdable containedObject = this.containedObject;
                            vec2_1 = equippedDuck.velocity;
                            int num = vec2_1.length < 0.05f ? 1 : 0;
                            containedObject.solid = num != 0;
                        }
                        else
                        {
                            Holdable containedObject = this.containedObject;
                            vec2_1 = velocity;
                            int num = vec2_1.length < 0.05f ? 1 : 0;
                            containedObject.solid = num != 0;
                        }
                        containedObject.grounded = true;
                    }
                    if (!containedObject.isServerForObject && !(containedObject is IAmADuck))
                        Fondle(containedObject);
                    if (containedObject.removeFromLevel || containedObject.y < level.topLeft.y - 2000f || !containedObject.active || !containedObject.isServerForObject)
                        SetContainedObject(null);
                }
                if (containedObject is Gun && Level.CheckRect<FunBeam>(containedObject.position + new Vec2(-4f, -4f), containedObject.position + new Vec2(4f, 4f)) != null)
                    (containedObject as Gun).triggerAction = true;
                if (containedObject is RagdollPart && (containedObject as RagdollPart).doll != null && (containedObject as RagdollPart).doll.part1 != null && (containedObject as RagdollPart).doll.part2 != null && (containedObject as RagdollPart).doll.part3 != null)
                {
                    if ((containedObject as RagdollPart).doll.part1.x < (containedObject as RagdollPart).doll.part3.x - 4f)
                        (containedObject as RagdollPart).doll.part1.x = (containedObject as RagdollPart).doll.part3.x - 4f;
                    if ((containedObject as RagdollPart).doll.part1.x > (containedObject as RagdollPart).doll.part3.x + 4f)
                        (containedObject as RagdollPart).doll.part1.x = (containedObject as RagdollPart).doll.part3.x + 4f;
                    Vec2 vec2_2 = (containedObject as RagdollPart).doll.part3.position + new Vec2(0f, -11f);
                    Vec2 vec2_3 = (containedObject as RagdollPart).doll.part3.position + new Vec2(0f, -5f);
                    (containedObject as RagdollPart).doll.part1.x = Lerp.FloatSmooth((containedObject as RagdollPart).doll.part1.x, vec2_2.x, 0.5f);
                    (containedObject as RagdollPart).doll.part1.y = Lerp.FloatSmooth((containedObject as RagdollPart).doll.part1.y, vec2_2.y, 0.5f);
                    (containedObject as RagdollPart).doll.part2.x = Lerp.FloatSmooth((containedObject as RagdollPart).doll.part1.x, vec2_3.x, 0.5f);
                    (containedObject as RagdollPart).doll.part2.y = Lerp.FloatSmooth((containedObject as RagdollPart).doll.part1.y, vec2_3.y, 0.5f);
                    vec2_1 = vec2_2 - (containedObject as RagdollPart).doll.part3.position;
                    Vec2 normalized = vec2_1.normalized;
                    (containedObject as RagdollPart).doll.part1.vSpeed = normalized.y;
                    (containedObject as RagdollPart).doll.part2.vSpeed = normalized.y;
                    (containedObject as RagdollPart).doll.part1.hSpeed = normalized.x;
                    (containedObject as RagdollPart).doll.part2.hSpeed = normalized.x;
                    (containedObject as RagdollPart).doll.part1.vSpeed *= 0.8f;
                    (containedObject as RagdollPart).doll.part1.hSpeed *= 0.8f;
                    (containedObject as RagdollPart).doll.part2.vSpeed *= 0.8f;
                    (containedObject as RagdollPart).doll.part2.hSpeed *= 0.8f;
                }
                if (containedObject != null && !(containedObject is Equipment))
                {
                    containedObject.UpdateAction();
                    if (containedObject is TapedGun)
                        (containedObject as TapedGun).UpdateSubActions(containedObject.triggerAction);
                }
            }
            base.Update();
        }

        protected virtual void DrawParts()
        {
            if (_equippedDuck == null)
                return;
            Depth depth = owner.depth;
            _overPart.flipH = owner.offDir <= 0;
            _overPart.angle = angle;
            _overPart.alpha = alpha;
            _overPart.scale = scale;
            _overPart.depth = owner.depth + 5;
            Graphics.Draw(ref _overPart, x, y);
            _underPart.flipH = owner.offDir <= 0;
            _underPart.angle = angle;
            _underPart.alpha = alpha;
            _underPart.scale = scale;
            if (_equippedDuck.ragdoll != null && _equippedDuck.ragdoll.part2 != null)
                _underPart.depth = _equippedDuck.ragdoll.part2.depth + -11;
            else
                _underPart.depth = owner.depth + -7;
            Graphics.Draw(ref _underPart, x, y);
        }

        private void PositionContainedObject()
        {
            if (_equippedDuck != null)
            {
                _containedObject.position = Offset(new Vec2(backOffset, -4f) + containedObject.holsterOffset);
                _containedObject.depth = owner.depth + -14;
                _containedObject.angleDegrees = (owner.offDir > 0 ? containedObject.holsterAngle : -containedObject.holsterAngle) + angleDegrees;
                _containedObject.offDir = owner.offDir > 0 ? (sbyte)1 : (sbyte)-1;
                if (containedObject is RagdollPart)
                {
                    _containedObject.position = Offset(new Vec2(backOffset, 0f));
                    _containedObject.angleDegrees += owner.offDir > 0 ? 90f : -90f;
                    if (duck != null && duck.ragdoll == null)
                    {
                        afterDrawAngle = _containedObject.angleDegrees;
                        _containedObject.angleDegrees -= duck.hSpeed * 3f;
                    }
                }
                if (!(owner is Duck) || (owner as Duck).ragdoll == null)
                    return;
                RagdollPart part2 = (owner as Duck).ragdoll.part2;
                if (part2 == null)
                    return;
                _containedObject.depth = part2.depth + -14;
            }
            else
            {
                _containedObject.position = Offset(new Vec2(backOffset + 6f, -2f) + containedObject.holsterOffset);
                _containedObject.depth = depth + -14;
                _containedObject.angleDegrees = (offDir > 0 ? containedObject.holsterAngle : -containedObject.holsterAngle) + angleDegrees;
                _containedObject.offDir = offDir > 0 ? (sbyte)1 : (sbyte)-1;
            }
        }

        public override void Draw()
        {
            if (Level.current is Editor && _previewSprite != null)
            {
                _previewSprite.depth = depth + 1;
                _previewSprite.scale = new Vec2(0.5f, 0.5f);
                _previewSprite.center = new Vec2(16f, 16f);
                Graphics.Draw(ref _previewSprite, x, y);
            }
            if (_equippedDuck != null)
                graphic = null;
            else
                graphic = _sprite;
            base.Draw();
            if (_equippedDuck != null && duck == null)
                return;
            DrawParts();
            if (_containedObject != null)
            {
                int offDir = this.offDir;
                PositionContainedObject();
                if (chained.value)
                {
                    float num = _equippedDuck != null ? 0f : 8f;
                    _chain.CenterOrigin();
                    _chain.depth = _underPart.depth + 1;
                    _chain.angleDegrees = angleDegrees - 45 * this.offDir;
                    Vec2 vec2 = Offset(new Vec2(num - 11f, -3f));
                    Graphics.Draw(ref _chain, vec2.x, vec2.y);
                    _lock.angleDegrees = _chainSway;
                    _chainSwayVel -= ((_lock.angleDegrees - (owner != null ? owner.hSpeed : hSpeed) * 10f) * 0.1f);
                    _chainSwayVel *= 0.95f;
                    _chainSway += _chainSwayVel;
                    _lock.depth = _underPart.depth + 2;
                    Offset(new Vec2(num - 9f, -5f));
                    Graphics.Draw(ref _lock, vec2.x, vec2.y);
                }
                if (!(containedObject is RagdollPart) || !Network.isActive)
                    _containedObject.Draw();
                if (afterDrawAngle <= -99999f)
                    return;
                _containedObject.angleDegrees = afterDrawAngle;
            }
            else
            {
                if (!chained.value)
                    return;
                _chain.depth = depth + 1;
                Vec2 vec2 = Offset(new Vec2(-3f, -2f));
                if (equippedDuck != null)
                    vec2 = Offset(new Vec2(-9f, -2f));
                _chain.center = new Vec2(3f, 3f);
                Graphics.Draw(ref _chain, vec2.x, vec2.y);
                Offset(new Vec2(0f, -8f));
                _chain.angleDegrees = 90f + _chainSway;
                _chainSwayVel -= ((_chain.angleDegrees - (90f + (owner != null ? owner.hSpeed : hSpeed) * 10f)) * 0.1f);
                _chainSwayVel *= 0.95f;
                _chainSway += _chainSwayVel;
            }
        }
    }
}
