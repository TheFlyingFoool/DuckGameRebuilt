// Decompiled with JetBrains decompiler
// Type: DuckGame.Holster
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        private System.Type _contains;
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
                if (this.containedObject == null)
                    return;
                this.containedObject.connection = value;
            }
        }

        public bool netChained
        {
            get => this.chained.value;
            set => this.chained.value = value;
        }

        public Holdable containedObject
        {
            get => this._containedObject;
            set => this._containedObject = value;
        }

        public Holdable netContainedObject
        {
            get => this._containedObject;
            set => this._containedObject = value;
        }

        public Thing GetContainedInstance(Vec2 pos = default(Vec2))
        {
            if (this.contains == null)
                return null;
            object[] constructorParameters = Editor.GetConstructorParameters(this.contains);
            if (constructorParameters.Count<object>() > 1)
            {
                constructorParameters[0] = pos.x;
                constructorParameters[1] = pos.y;
            }
            PhysicsObject thing = Editor.CreateThing(this.contains, constructorParameters) as PhysicsObject;
            if (thing is Gun)
                (thing as Gun).infinite = this.infinite;
            return thing;
        }

        public void SetContainedObject(Holdable h)
        {
            if (this._containedObject != null)
            {
                this._containedObject.visible = true;
                this.Fondle(_containedObject);
                this._containedObject.owner = null;
                this._containedObject = null;
            }
            if (h == null)
                return;
            this._containedObject = h;
            h.lastGrounded = DateTime.Now;
            h.visible = false;
        }

        public virtual void EjectItem()
        {
            if (this.containedObject == null)
                return;
            SFX.PlaySynchronized("pelletgunBad", pitch: Rando.Float(0.1f, 0.1f));
            this.containedObject.hSpeed = -this.owner.offDir * 6f;
            this.containedObject.vSpeed = -1.5f;
            if (this.duck != null)
            {
                this.duck._lastHoldItem = this.containedObject;
                this.duck._timeSinceThrow = 0;
            }
            this.SetContainedObject(null);
        }

        public virtual void EjectItem(Vec2 pSpeed)
        {
            if (this.containedObject == null)
                return;
            SFX.PlaySynchronized("pelletgunBad", pitch: Rando.Float(0.1f, 0.1f));
            this.containedObject.hSpeed = pSpeed.x;
            this.containedObject.vSpeed = pSpeed.y;
            this.SetContainedObject(null);
        }

        public System.Type contains
        {
            get => this._contains;
            set
            {
                this._contains = value;
                if (Level.skipInitialize)
                    return;
                if (this._preview == null)
                    this._preview = new RenderTarget2D(32, 32);
                Thing containedInstance = this.GetContainedInstance();
                if (containedInstance == null)
                    return;
                this._previewSprite = containedInstance.GetEditorImage(32, 32, true, target: this._preview);
            }
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(this.contains));
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
            dxmlNode.Add(new DXMLNode("contains", this.contains != null ? contains.AssemblyQualifiedName : (object)""));
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
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.InitializeGroups(new EditorGroup(typeof(PhysicsObject)), radioBinding);
            return contextMenu;
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (this.contains != null)
                str = this.contains.Name;
            return this.contains == null ? base.GetDetailsString() : base.GetDetailsString() + "Contains: " + str;
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (this.contains != null)
                text = this.contains.Name;
            Graphics.DrawString(text, this.position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2.0), -16f), Color.White, (Depth)0.9f);
        }

        public Holster(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._chain = new Sprite("holsterChain")
            {
                center = new Vec2(3f, 3f)
            };
            this._lock = new Sprite("holsterLock")
            {
                center = new Vec2(3f, 2f)
            };
            this._sprite = new SpriteMap("holster", 12, 12);
            this._overPart = new SpriteMap("holster_over", 10, 3)
            {
                center = new Vec2(6f, -1f)
            };
            this._underPart = new SpriteMap("holster_under", 8, 9)
            {
                center = new Vec2(10f, 8f)
            };
            this.graphic = _sprite;
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(10f, 10f);
            this.center = new Vec2(6f, 6f);
            this.physicsMaterial = PhysicsMaterial.Wood;
            this._equippedDepth = 4;
            this._wearOffset = new Vec2(1f, 1f);
            this.editorTooltip = "Lets you carry around an additional item!";
        }

        protected override bool OnDestroy(DestroyType type = null) => this.owner == null && this.containedObject == null && base.OnDestroy(type);

        public override void Initialize()
        {
            if (!(Level.current is Editor) && this.GetContainedInstance(this.position) is Holdable containedInstance)
            {
                Level.Add(containedInstance);
                this.SetContainedObject(containedInstance);
                if (Network.isActive && Thing.loadingLevel != null && this._containedObject != null)
                    this._containedObject.PrepareForHost();
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this.destroyed)
                this.alpha -= 0.05f;
            if (this.alpha < 0.0)
                Level.Remove(this);
            if (this.isServerForObject)
            {
                this.netRaise = false;
                if (this._equippedDuck != null && this._equippedDuck.inputProfile != null && this._equippedDuck.inputProfile.Down("UP"))
                    this.netRaise = true;
                if (this.owner == null && this.equippedDuck == null)
                    this.angleDegrees = 0f;
                Vec2 vec2_1;
                if (this.containedObject != null)
                {
                    this.PositionContainedObject();
                    this._containedObject.HolsterUpdate(this);
                    this.weight = this.containedObject.weight;
                    if (this.duck != null)
                        this.containedObject.owner = duck;
                    else
                        this.containedObject.owner = this;
                    if (this.duck != null && this.duck.ragdoll != null)
                    {
                        this.containedObject.solid = false;
                        this.containedObject.grounded = false;
                    }
                    else
                    {
                        if (this.equippedDuck != null)
                        {
                            Holdable containedObject = this.containedObject;
                            vec2_1 = this.equippedDuck.velocity;
                            int num = vec2_1.length < 0.05f ? 1 : 0;
                            containedObject.solid = num != 0;
                        }
                        else
                        {
                            Holdable containedObject = this.containedObject;
                            vec2_1 = this.velocity;
                            int num = vec2_1.length < 0.05f ? 1 : 0;
                            containedObject.solid = num != 0;
                        }
                        this.containedObject.grounded = true;
                    }
                    if (!this.containedObject.isServerForObject && !(this.containedObject is IAmADuck))
                        this.Fondle(containedObject);
                    if (this.containedObject.removeFromLevel || this.containedObject.y < level.topLeft.y - 2000.0 || !this.containedObject.active || !this.containedObject.isServerForObject)
                        this.SetContainedObject(null);
                }
                if (this.containedObject is Gun && Level.CheckRect<FunBeam>(this.containedObject.position + new Vec2(-4f, -4f), this.containedObject.position + new Vec2(4f, 4f)) != null)
                    (this.containedObject as Gun).triggerAction = true;
                if (this.containedObject is RagdollPart && (this.containedObject as RagdollPart).doll != null && (this.containedObject as RagdollPart).doll.part1 != null && (this.containedObject as RagdollPart).doll.part2 != null && (this.containedObject as RagdollPart).doll.part3 != null)
                {
                    if ((this.containedObject as RagdollPart).doll.part1.x < (this.containedObject as RagdollPart).doll.part3.x - 4.0)
                        (this.containedObject as RagdollPart).doll.part1.x = (this.containedObject as RagdollPart).doll.part3.x - 4f;
                    if ((this.containedObject as RagdollPart).doll.part1.x > (this.containedObject as RagdollPart).doll.part3.x + 4.0)
                        (this.containedObject as RagdollPart).doll.part1.x = (this.containedObject as RagdollPart).doll.part3.x + 4f;
                    Vec2 vec2_2 = (this.containedObject as RagdollPart).doll.part3.position + new Vec2(0f, -11f);
                    Vec2 vec2_3 = (this.containedObject as RagdollPart).doll.part3.position + new Vec2(0f, -5f);
                    (this.containedObject as RagdollPart).doll.part1.x = Lerp.FloatSmooth((this.containedObject as RagdollPart).doll.part1.x, vec2_2.x, 0.5f);
                    (this.containedObject as RagdollPart).doll.part1.y = Lerp.FloatSmooth((this.containedObject as RagdollPart).doll.part1.y, vec2_2.y, 0.5f);
                    (this.containedObject as RagdollPart).doll.part2.x = Lerp.FloatSmooth((this.containedObject as RagdollPart).doll.part1.x, vec2_3.x, 0.5f);
                    (this.containedObject as RagdollPart).doll.part2.y = Lerp.FloatSmooth((this.containedObject as RagdollPart).doll.part1.y, vec2_3.y, 0.5f);
                    vec2_1 = vec2_2 - (this.containedObject as RagdollPart).doll.part3.position;
                    Vec2 normalized = vec2_1.normalized;
                    (this.containedObject as RagdollPart).doll.part1.vSpeed = normalized.y;
                    (this.containedObject as RagdollPart).doll.part2.vSpeed = normalized.y;
                    (this.containedObject as RagdollPart).doll.part1.hSpeed = normalized.x;
                    (this.containedObject as RagdollPart).doll.part2.hSpeed = normalized.x;
                    (this.containedObject as RagdollPart).doll.part1.vSpeed *= 0.8f;
                    (this.containedObject as RagdollPart).doll.part1.hSpeed *= 0.8f;
                    (this.containedObject as RagdollPart).doll.part2.vSpeed *= 0.8f;
                    (this.containedObject as RagdollPart).doll.part2.hSpeed *= 0.8f;
                }
                if (this.containedObject != null && !(this.containedObject is Equipment))
                {
                    this.containedObject.UpdateAction();
                    if (this.containedObject is TapedGun)
                        (this.containedObject as TapedGun).UpdateSubActions(this.containedObject.triggerAction);
                }
            }
            base.Update();
        }

        protected virtual void DrawParts()
        {
            if (this._equippedDuck == null)
                return;
            Depth depth = this.owner.depth;
            this._overPart.flipH = this.owner.offDir <= 0;
            this._overPart.angle = this.angle;
            this._overPart.alpha = this.alpha;
            this._overPart.scale = this.scale;
            this._overPart.depth = this.owner.depth + 5;
            Graphics.Draw(_overPart, this.x, this.y);
            this._underPart.flipH = this.owner.offDir <= 0;
            this._underPart.angle = this.angle;
            this._underPart.alpha = this.alpha;
            this._underPart.scale = this.scale;
            if (this._equippedDuck.ragdoll != null && this._equippedDuck.ragdoll.part2 != null)
                this._underPart.depth = this._equippedDuck.ragdoll.part2.depth + -11;
            else
                this._underPart.depth = this.owner.depth + -7;
            Graphics.Draw(_underPart, this.x, this.y);
        }

        private void PositionContainedObject()
        {
            if (this._equippedDuck != null)
            {
                this._containedObject.position = this.Offset(new Vec2(this.backOffset, -4f) + this.containedObject.holsterOffset);
                this._containedObject.depth = this.owner.depth + -14;
                this._containedObject.angleDegrees = (this.owner.offDir > 0 ? this.containedObject.holsterAngle : -this.containedObject.holsterAngle) + this.angleDegrees;
                this._containedObject.offDir = this.owner.offDir > 0 ? (sbyte)1 : (sbyte)-1;
                if (this.containedObject is RagdollPart)
                {
                    this._containedObject.position = this.Offset(new Vec2(this.backOffset, 0f));
                    this._containedObject.angleDegrees += this.owner.offDir > 0 ? 90f : -90f;
                    if (this.duck != null && this.duck.ragdoll == null)
                    {
                        this.afterDrawAngle = this._containedObject.angleDegrees;
                        this._containedObject.angleDegrees -= this.duck.hSpeed * 3f;
                    }
                }
                if (!(this.owner is Duck) || (this.owner as Duck).ragdoll == null)
                    return;
                RagdollPart part2 = (this.owner as Duck).ragdoll.part2;
                if (part2 == null)
                    return;
                this._containedObject.depth = part2.depth + -14;
            }
            else
            {
                this._containedObject.position = this.Offset(new Vec2(this.backOffset + 6f, -2f) + this.containedObject.holsterOffset);
                this._containedObject.depth = this.depth + -14;
                this._containedObject.angleDegrees = (this.offDir > 0 ? this.containedObject.holsterAngle : -this.containedObject.holsterAngle) + this.angleDegrees;
                this._containedObject.offDir = this.offDir > 0 ? (sbyte)1 : (sbyte)-1;
            }
        }

        public override void Draw()
        {
            if (Level.current is Editor && this._previewSprite != null)
            {
                this._previewSprite.depth = this.depth + 1;
                this._previewSprite.scale = new Vec2(0.5f, 0.5f);
                this._previewSprite.center = new Vec2(16f, 16f);
                Graphics.Draw(this._previewSprite, this.x, this.y);
            }
            if (this._equippedDuck != null)
                this.graphic = null;
            else
                this.graphic = _sprite;
            base.Draw();
            if (this._equippedDuck != null && this.duck == null)
                return;
            this.DrawParts();
            if (this._containedObject != null)
            {
                int offDir = this.offDir;
                this.PositionContainedObject();
                if (this.chained.value)
                {
                    float num = this._equippedDuck != null ? 0f : 8f;
                    this._chain.CenterOrigin();
                    this._chain.depth = this._underPart.depth + 1;
                    this._chain.angleDegrees = this.angleDegrees - 45 * this.offDir;
                    Vec2 vec2 = this.Offset(new Vec2(num - 11f, -3f));
                    Graphics.Draw(this._chain, vec2.x, vec2.y);
                    this._lock.angleDegrees = this._chainSway;
                    this._chainSwayVel -= ((this._lock.angleDegrees - (this.owner != null ? this.owner.hSpeed : this.hSpeed) * 10f) * 0.1f);
                    this._chainSwayVel *= 0.95f;
                    this._chainSway += this._chainSwayVel;
                    this._lock.depth = this._underPart.depth + 2;
                    this.Offset(new Vec2(num - 9f, -5f));
                    Graphics.Draw(this._lock, vec2.x, vec2.y);
                }
                if (!(this.containedObject is RagdollPart) || !Network.isActive)
                    this._containedObject.Draw();
                if (afterDrawAngle <= -99999f)
                    return;
                this._containedObject.angleDegrees = this.afterDrawAngle;
            }
            else
            {
                if (!this.chained.value)
                    return;
                this._chain.depth = this.depth + 1;
                Vec2 vec2 = this.Offset(new Vec2(-3f, -2f));
                if (this.equippedDuck != null)
                    vec2 = this.Offset(new Vec2(-9f, -2f));
                this._chain.center = new Vec2(3f, 3f);
                Graphics.Draw(this._chain, vec2.x, vec2.y);
                this.Offset(new Vec2(0f, -8f));
                this._chain.angleDegrees = 90f + this._chainSway;
                this._chainSwayVel -= ((this._chain.angleDegrees - (90f + (this.owner != null ? this.owner.hSpeed : this.hSpeed) * 10f)) * 0.1f);
                this._chainSwayVel *= 0.95f;
                this._chainSway += this._chainSwayVel;
            }
        }
    }
}
