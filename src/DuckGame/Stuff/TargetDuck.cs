// Decompiled with JetBrains decompiler
// Type: DuckGame.TargetDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Props", EditorItemType.Arcade)]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class TargetDuck : Duck, ISequenceItem
    {
        protected new SpriteMap _sprite;
        protected Sprite _base;
        protected Sprite _woodWing;
        protected bool _popup;
        protected float _upSpeed;
        protected bool _up;
        protected TargetStance _stance;
        public System.Type contains;
        public bool chestPlate;
        public bool helmet;
        private float _timeCount;
        public EditorProperty<float> time = new EditorProperty<float>(0f, max: 30f, minSpecial: "INF");
        public EditorProperty<float> autofire = new EditorProperty<float>(0f, max: 100f, minSpecial: "INF");
        public EditorProperty<bool> random = new EditorProperty<bool>(false);
        public EditorProperty<int> maxrandom = new EditorProperty<int>(1, min: 1f, max: 32f, increment: 1f);
        public EditorProperty<bool> dropgun = new EditorProperty<bool>(true);
        public EditorProperty<float> speediness = new EditorProperty<float>(1f, max: 2f, increment: 0.01f);
        private float _autoFireWait;
        private bool editorUpdate;
        protected float _waitFire = 1f;
        private Sprite _reticule;
        private int _stanceSetting;

        public override bool action => false;

        public TargetDuck(float xpos, float ypos, TargetStance stance)
          : base(xpos, ypos, null)
        {
            this._sprite = new SpriteMap("woodDuck", 32, 32);
            this._base = new Sprite("popupPad");
            this._woodWing = new Sprite("woodWing");
            this.graphic = _sprite;
            this.center = new Vec2(16f, 22f);
            this._stance = stance;
            this.UpdateCollision();
            this.physicsMaterial = PhysicsMaterial.Wood;
            this.thickness = 0.5f;
            this._hitPoints = this._maxHealth = 0.1f;
            this.editorOffset = new Vec2(0f, -4f);
            this.hugWalls = WallHug.Floor;
            this._canHaveChance = false;
            this.sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Target
            };
            this.speediness.value = 1f;
        }

        public override void OnDrawLayer(Layer pLayer)
        {
        }

        public override void Initialize()
        {
            this._profile = Profiles.EnvironmentProfile;
            this.InitProfile();
            this._sprite = new SpriteMap("woodDuck", 32, 32);
            this._base = new Sprite("popupPad");
            this._woodWing = new Sprite("woodWing");
            this.graphic = _sprite;
            if (!(Level.current is Editor))
            {
                if (this._stance != TargetStance.Fly)
                    this.scale = new Vec2(1f, 0f);
                else
                    this.scale = new Vec2(0f, 1f);
                ChallengeLevel.allTargetsShot = false;
                this._autoFireWait = this.autofire.value;
            }
            if ((double)(float)this.speediness == 0.0)
                this.speediness.value = 1f;
            this._waitFire = (float)this.speediness;
            this.UpdateCollision();
        }

        public void SpawnHoldObject()
        {
            this._autoFireWait = (float)this.autofire;
            if (!(this.contains != null) || !(Editor.CreateThing(this.contains) is Holdable thing))
                return;
            Level.Add(thing);
            this.GiveThing(thing);
        }

        public override void ReturnItemToWorld(Thing t)
        {
            Vec2 p1 = this.position + new Vec2(offDir * 3, 0f);
            Block block1 = Level.CheckLine<Block>(p1, p1 + new Vec2(16f, 0f));
            if (block1 != null && block1.solid && (double)t.right > (double)block1.left)
                t.right = block1.left;
            Block block2 = Level.CheckLine<Block>(p1, p1 - new Vec2(16f, 0f));
            if (block2 != null && block2.solid && (double)t.left < (double)block2.right)
                t.left = block2.right;
            Block block3 = Level.CheckLine<Block>(p1, p1 + new Vec2(0f, -16f));
            if (block3 != null && block3.solid && (double)t.top < (double)block3.bottom)
                t.top = block3.bottom;
            Block block4 = Level.CheckLine<Block>(p1, p1 + new Vec2(0f, 16f));
            if (block4 == null || !block4.solid || (double)t.bottom <= (double)block4.top)
                return;
            t.bottom = block4.top;
        }

        public void UpdateCollision()
        {
            if (Level.current is Editor || Level.current == null || this._up && this._popup)
            {
                this.crouch = false;
                this.sliding = false;
                if (this._stance == TargetStance.Stand)
                {
                    this._sprite.frame = 0;
                    this._collisionOffset = new Vec2(-6f, -24f);
                    this.collisionSize = new Vec2(12f, 24f);
                    this.hugWalls = WallHug.Floor;
                }
                else if (this._stance == TargetStance.StandArmed)
                {
                    this._sprite.frame = 1;
                    this._collisionOffset = new Vec2(-6f, -23f);
                    this.collisionSize = new Vec2(12f, 23f);
                    this.hugWalls = WallHug.Floor;
                }
                else if (this._stance == TargetStance.Crouch)
                {
                    this._sprite.frame = 2;
                    this._collisionOffset = new Vec2(-6f, -18f);
                    this.collisionSize = new Vec2(12f, 18f);
                    this.crouch = true;
                    this.hugWalls = WallHug.Floor;
                }
                else if (this._stance == TargetStance.Slide)
                {
                    this._sprite.frame = 3;
                    this._collisionOffset = new Vec2(-6f, -10f);
                    this.collisionSize = new Vec2(12f, 10f);
                    this.sliding = true;
                    this.hugWalls = WallHug.Floor;
                }
                else if (this._stance == TargetStance.Fly)
                {
                    this._sprite.frame = 4;
                    this._collisionOffset = new Vec2(-8f, -24f);
                    this.collisionSize = new Vec2(16f, 24f);
                    this.hugWalls = WallHug.Left | WallHug.Right;
                }
            }
            else
            {
                this.hugWalls = WallHug.Floor;
                if (this._stance == TargetStance.Stand)
                    this._sprite.frame = 0;
                else if (this._stance == TargetStance.StandArmed)
                    this._sprite.frame = 1;
                else if (this._stance == TargetStance.Crouch)
                    this._sprite.frame = 2;
                else if (this._stance == TargetStance.Slide)
                    this._sprite.frame = 3;
                else if (this._stance == TargetStance.Fly)
                {
                    this._sprite.frame = 4;
                    this.hugWalls = WallHug.Left | WallHug.Right;
                }
                this._collisionOffset = new Vec2(-6000f, 0f);
                this.collisionSize = new Vec2(2f, 2f);
            }
            this._collisionOffset.y += 10f;
            --this._collisionSize.y;
            this._featherVolume.collisionSize = new Vec2(this.collisionSize.x + 2f, this.collisionSize.y + 2f);
            this._featherVolume.collisionOffset = new Vec2(this.collisionOffset.x - 1f, this.collisionOffset.y - 1f);
        }

        public override void OnSequenceActivate() => this._popup = true;

        public void PopDown()
        {
            this._popup = false;
            if (this.holdObject != null)
            {
                Level.Remove(holdObject);
                this.holdObject = null;
            }
            foreach (Equipment equipment in this._equipment)
            {
                if (equipment != null)
                    Level.Remove(equipment);
            }
            this._equipment.Clear();
            this._sequence.Finished();
        }

        public override bool Kill(DestroyType type = null)
        {
            if (this._up && this._popup)
            {
                if (ChallengeLevel.running)
                    ++ChallengeLevel.targetsShot;
                if (this.holdObject is Gun && !(bool)this.dropgun)
                    (this.holdObject as Gun).ammo = 0;
                this.ThrowItem(false);
                foreach (Equipment t in this._equipment)
                {
                    if (t != null)
                    {
                        t.owner = null;
                        t.hSpeed = Rando.Float(2f) - 1f;
                        t.vSpeed = -Rando.Float(1.5f);
                        this.ReturnItemToWorld(t);
                        t.UnEquip();
                    }
                }
                SFX.Play("ting", Rando.Float(0.7f, 0.8f), Rando.Float(-0.2f, 0.2f));
                if (type is DTShot)
                    SFX.Play("targetRebound", Rando.Float(0.7f, 0.8f), Rando.Float(-0.2f, 0.2f));
                Vec2 vec2 = Vec2.Zero;
                if (type is DTShot)
                    vec2 = (type as DTShot).bullet.travelDirNormalized;
                for (int index = 0; index < 4; ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 20f + Rando.Float(16f));
                    woodDebris.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(3f) + Math.Sign(vec2.x) * 0.5);
                    woodDebris.vSpeed = -Rando.Float(1f);
                    Level.Add(woodDebris);
                }
                for (int index = 0; index < 2; ++index)
                    Level.Add(Feather.New(this.x, this.y - 16f, this.persona));
                this.PopDown();
            }
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos) => this._up && this._popup && base.Hit(bullet, hitPos);

        public override void ExitHit(Bullet bullet, Vec2 hitPos)
        {
            if (!this._up || this._popup)
                return;
            for (int index = 0; index < 2; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                woodDebris.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                woodDebris.vSpeed = (float)(-(double)bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                Level.Add(woodDebris);
            }
        }

        public override bool Hurt(float points)
        {
            if (!this._popup || _maxHealth == 0.0)
                return false;
            this._hitPoints -= points;
            return true;
        }

        public void GiveThing(Holdable h)
        {
            this.holdObject = h;
            this.holdObject.owner = this;
            this.holdObject.solid = false;
        }

        public override void UpdateSkeleton()
        {
            int num = 6;
            int frame = 0;
            if (this.sliding)
                frame = 12;
            else if (this.crouch)
                frame = 11;
            this._skeleton.head.position = this.Offset(DuckRig.GetHatPoint(frame) + new Vec2(0f, -num));
            this._skeleton.upperTorso.position = this.Offset(DuckRig.GetChestPoint(frame) + new Vec2(0f, -num));
            this._skeleton.lowerTorso.position = this.position + new Vec2(0f, 10 - num);
            if (this.sliding)
            {
                this._skeleton.head.orientation = Maths.DegToRad(90f);
                this._skeleton.upperTorso.orientation = Maths.DegToRad(90f);
            }
            else
            {
                this._skeleton.head.orientation = 0f;
                this._skeleton.upperTorso.orientation = 0f;
            }
        }

        public override void UpdateHoldPosition(bool updateLerp = true)
        {
            if (this.holdObject == null)
                return;
            this.holdOffY = 0f;
            this.armOffY = 0f;
            if ((!this._up || !this._popup) && !this.editorUpdate)
                return;
            this.holdObject.UpdateAction();
            this.holdObject.position = this.armPosition + this.holdObject.holdOffset + new Vec2(this.holdOffX, this.holdOffY) + new Vec2(2 * offDir, 0f);
            this.holdObject.offDir = this.offDir;
            if (this._sprite.currentAnimation == "slide")
            {
                --this.holdOffY;
                ++this.holdOffX;
            }
            else if (this.crouch)
            {
                if (this.holdObject != null)
                    this.armOffY += 4f;
            }
            else if (this.sliding && this.holdObject != null)
                this.armOffY += 6f;
            this.holdObject.position = this.HoldOffset(this.holdObject.holdOffset) + new Vec2(offDir * 3, 0f);
            this.holdObject.angle = this.holdObject.handAngle + this.holdAngleOff;
        }

        public override void DuckUpdate()
        {
        }

        public virtual void UpdateFire()
        {
            Gun holdObject = this.holdObject as Gun;
            float num = 300f;
            if (holdObject.ammoType != null)
                num = holdObject.ammoType.range;
            Vec2 vec2 = this.holdObject.Offset(new Vec2(num * this.holdObject.angleMul, 0f));
            if (_autoFireWait <= 0.0)
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)].Where<Thing>(d => !(d is TargetDuck)))
                {
                    if ((Collision.Line(this.holdObject.position + new Vec2(0f, -5f), vec2 + new Vec2(0f, -5f), duck.rectangle) || Collision.Line(this.holdObject.position + new Vec2(0f, 5f), vec2 + new Vec2(0f, 5f), duck.rectangle)) && Level.CheckLine<Block>(this.holdObject.position, duck.position) == null)
                    {
                        this._waitFire -= 0.03f;
                        break;
                    }
                }
            }
            bool flag = false;
            if (_autoFireWait > 0.0)
            {
                this._autoFireWait -= Maths.IncFrameTimer();
                if (_autoFireWait <= 0.0)
                    flag = true;
            }
            if (_waitFire <= 0.0 | flag)
            {
                holdObject.PressAction();
                this._waitFire = (float)this.speediness;
            }
            if (_waitFire >= (double)(float)this.speediness)
                return;
            this._waitFire += 0.01f;
        }

        public override void Update()
        {
            this.impacting.Clear();
            if (this._up && this._popup && this.holdObject is Gun)
                this.UpdateFire();
            this.UpdateCollision();
            this.UpdateSkeleton();
            if (_hitPoints <= 0.0)
                this.Destroy(new DTCrush(null));
            if (!this._up)
            {
                this._timeCount = 0f;
                if (this._popup)
                    this._upSpeed += 0.1f;
                if (this._stance != TargetStance.Fly)
                {
                    this.yscale += this._upSpeed;
                    if ((double)this.yscale < 1.0)
                        return;
                    this.yscale = 1f;
                    this._upSpeed = 0f;
                    this._up = true;
                    SFX.Play("grappleHook", 0.7f, Rando.Float(-0.2f, 0.2f));
                    Level.Add(SmallSmoke.New(this.x - 4f, this.y));
                    Level.Add(SmallSmoke.New(this.x + 4f, this.y));
                    this.SpawnHoldObject();
                    if (this.helmet)
                    {
                        Helmet e = new Helmet(this.x, this.y);
                        Level.Add(e);
                        this.Equip(e);
                    }
                    if (!this.chestPlate)
                        return;
                    ChestPlate e1 = new ChestPlate(this.x, this.y);
                    Level.Add(e1);
                    this.Equip(e1);
                }
                else
                {
                    this.xscale += this._upSpeed;
                    if ((double)this.xscale < 1.0)
                        return;
                    this.xscale = 1f;
                    this._upSpeed = 0f;
                    this._up = true;
                    SFX.Play("grappleHook", 0.7f, Rando.Float(-0.2f, 0.2f));
                    Level.Add(SmallSmoke.New(this.x - 4f, this.y));
                    Level.Add(SmallSmoke.New(this.x + 4f, this.y));
                    if (this.helmet)
                    {
                        Helmet e = new Helmet(this.x, this.y);
                        Level.Add(e);
                        this.Equip(e);
                    }
                    if (!this.chestPlate)
                        return;
                    ChestPlate e2 = new ChestPlate(this.x, this.y);
                    Level.Add(e2);
                    this.Equip(e2);
                }
            }
            else
            {
                this._timeCount += Maths.IncFrameTimer();
                if (this._popup && (double)this.time.value != 0.0 && _timeCount >= (double)this.time.value)
                {
                    SFX.Play("grappleHook", 0.2f, Rando.Float(-0.2f, 0.2f));
                    this.PopDown();
                }
                else
                {
                    if (!this._popup)
                        this._upSpeed += 0.1f;
                    if (this._stance != TargetStance.Fly)
                    {
                        this.yscale -= this._upSpeed;
                        if ((double)this.yscale >= 0.0)
                            return;
                        this.yscale = 0f;
                        this._upSpeed = 0f;
                        this._up = false;
                        SFX.Play("grappleHook", 0.2f, Rando.Float(-0.2f, 0.2f));
                        Level.Add(SmallSmoke.New(this.x - 4f, this.y));
                        Level.Add(SmallSmoke.New(this.x + 4f, this.y));
                        this._hitPoints = this._maxHealth = 0.1f;
                    }
                    else
                    {
                        this.xscale -= this._upSpeed;
                        if ((double)this.xscale >= 0.0)
                            return;
                        this.xscale = 0f;
                        this._upSpeed = 0f;
                        this._up = false;
                        SFX.Play("grappleHook", 0.2f, Rando.Float(-0.2f, 0.2f));
                        Level.Add(SmallSmoke.New(this.x - 4f, this.y));
                        Level.Add(SmallSmoke.New(this.x + 4f, this.y));
                        this._hitPoints = this._maxHealth = 0.1f;
                    }
                }
            }
        }

        public new void DrawIcon()
        {
            if (!this.ShouldDrawIcon() || !this._up || !ChallengeMode.showReticles)
                return;
            if (this._reticule == null)
                this._reticule = new Sprite("challenge/reticule");
            Vec2 position1 = this.position;
            if (this.ragdoll != null)
                position1 = this.ragdoll.part1.position;
            else if (this._trapped != null)
                position1 = this._trapped.position;
            if ((double)(position1 - Level.current.camera.position).length > (double)Level.current.camera.width * 2.0)
                return;
            float num = 14f;
            if (position1.x < (double)Level.current.camera.left + (double)num)
                position1.x = Level.current.camera.left + num;
            if (position1.x > (double)Level.current.camera.right - (double)num)
                position1.x = Level.current.camera.right - num;
            if (position1.y < (double)Level.current.camera.top + (double)num)
                position1.y = Level.current.camera.top + num;
            if (position1.y > (double)Level.current.camera.bottom - (double)num)
                position1.y = Level.current.camera.bottom - num;
            Vec2 position2 = Layer.HUD.camera.transformInverse(Level.current.camera.transform(position1));
            DuckGame.Graphics.DrawRect(position2 + new Vec2(-5f, -5f), position2 + new Vec2(5f, 5f), Color.Black, (Depth)0.8f);
            DuckGame.Graphics.DrawRect(position2 + new Vec2(-5f, -5f), position2 + new Vec2(5f, 5f), Color.White, (Depth)0.81f, false);
            DuckGame.Graphics.Draw(this._reticule.texture, position2, new Rectangle?(), Color.White, 0f, new Vec2(this._reticule.width / 2, this._reticule.height / 2), new Vec2(0.5f, 0.5f), SpriteEffects.None, (Depth)(0.9f + this.depth.span));
        }

        public override void Draw()
        {
            if (this._trapped != null)
                this.y = -10000f;
            if (this.graphic == null)
                return;
            this.graphic.flipH = this.offDir <= 0;
            this.graphic.scale = this.scale;
            if (Level.current is Editor)
            {
                this.graphic.center = this.center;
                this.graphic.position = this.position;
            }
            else if (this._stance != TargetStance.Fly)
            {
                this.graphic.center = this.center + new Vec2(0f, 10f);
                this.graphic.position = this.position + new Vec2(0f, 10f);
            }
            else
            {
                this.graphic.center = this.center + new Vec2(-12f, 10f);
                this.graphic.position = this.position + new Vec2(-12 * offDir, 10f);
            }
            this.graphic.depth = this.depth;
            this.graphic.alpha = this.alpha;
            this.graphic.angle = this.angle;
            this.graphic.Draw();
            if (!this._popup || !this._up)
                return;
            this.DrawHat();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("stanceSetting", stanceSetting);
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(this.contains));
            binaryClassChunk.AddProperty("chestPlate", chestPlate);
            binaryClassChunk.AddProperty("helmet", helmet);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.stanceSetting = node.GetProperty<int>("stanceSetting");
            this.contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            this.chestPlate = node.GetProperty<bool>("chestPlate");
            this.helmet = node.GetProperty<bool>("helmet");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("stanceSetting", Change.ToString(stanceSetting)));
            dxmlNode.Add(new DXMLNode("contains", this.contains != null ? contains.AssemblyQualifiedName : (object)""));
            dxmlNode.Add(new DXMLNode("chestPlate", Change.ToString(chestPlate)));
            dxmlNode.Add(new DXMLNode("helmet", Change.ToString(helmet)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode1 = node.Element("stanceSetting");
            if (dxmlNode1 != null)
                this.stanceSetting = Convert.ToInt32(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("contains");
            if (dxmlNode2 != null)
                this.contains = Editor.GetType(dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("chestPlate");
            if (dxmlNode3 != null)
                this.chestPlate = Convert.ToBoolean(dxmlNode3.Value);
            DXMLNode dxmlNode4 = node.Element("helmet");
            if (dxmlNode4 != null)
                this.helmet = Convert.ToBoolean(dxmlNode4.Value);
            return true;
        }

        public int stanceSetting
        {
            get => this._stanceSetting;
            set
            {
                this._stanceSetting = value;
                this._stance = (TargetStance)this._stanceSetting;
                this.UpdateCollision();
            }
        }

        public override string GetDetailsString()
        {
            string str = "NONE";
            if (this.contains != null)
                str = this.contains.Name;
            return base.GetDetailsString() + "Order: " + this.sequence.order.ToString() + "\nHolding: " + str;
        }

        public override void Netted(Net n)
        {
            base.Netted(n);
            this.y -= 10000f;
            this._trapped.infinite = true;
        }

        public override void EditorUpdate()
        {
            if (this.chestPlate && this.GetEquipment(typeof(ChestPlate)) == null)
                this.Equip(new ChestPlate(0f, 0f), false);
            else if (!this.chestPlate)
            {
                Equipment equipment = this.GetEquipment(typeof(ChestPlate));
                if (equipment != null)
                    this.Unequip(equipment);
            }
            if (this.helmet && this.GetEquipment(typeof(Helmet)) == null)
                this.Equip(new Helmet(0f, 0f), false);
            else if (!this.helmet)
            {
                Equipment equipment = this.GetEquipment(typeof(Helmet));
                if (equipment != null)
                    this.Unequip(equipment);
            }
            if (this.contains != null)
            {
                if (this.holdObject == null || this.holdObject.GetType() != this.contains)
                    this.GiveHoldable(Editor.CreateThing(this.contains) as Holdable);
            }
            else
                this.holdObject = null;
            foreach (Thing thing in this._equipment)
                thing.DoUpdate();
            if (this.holdObject != null)
            {
                this.editorUpdate = true;
                this.UpdateHoldPosition(true);
                this.holdObject.DoUpdate();
                this.editorUpdate = false;
            }
            base.EditorUpdate();
        }

        public override void EditorRender()
        {
            foreach (Thing thing in this._equipment)
                thing.DoDraw();
            if (this.holdObject != null)
            {
                this.holdObject.depth = (Depth)0.9f;
                this.holdObject.DoDraw();
            }
            base.EditorRender();
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextRadio("Stand", this.stanceSetting == 0, 0, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextRadio("Crouch", this.stanceSetting == 2, 2, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextRadio("Slide", this.stanceSetting == 3, 3, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextRadio("Fly", this.stanceSetting == 4, 4, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextCheckBox("Chest Plate", null, new FieldBinding(this, "chestPlate")));
            contextMenu.AddItem(new ContextCheckBox("Helmet", null, new FieldBinding(this, "helmet")));
            FieldBinding pBinding = new FieldBinding(this, "contains");
            EditorGroupMenu editorGroupMenu = new EditorGroupMenu(contextMenu);
            editorGroupMenu.InitializeTypelist(typeof(PhysicsObject), pBinding);
            editorGroupMenu.text = "Holding";
            contextMenu.AddItem(editorGroupMenu);
            return contextMenu;
        }
    }
}
