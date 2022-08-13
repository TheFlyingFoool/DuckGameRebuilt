// Decompiled with JetBrains decompiler
// Type: DuckGame.TargetDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("woodDuck", 32, 32);
            _base = new Sprite("popupPad");
            _woodWing = new Sprite("woodWing");
            graphic = _sprite;
            center = new Vec2(16f, 22f);
            _stance = stance;
            UpdateCollision();
            physicsMaterial = PhysicsMaterial.Wood;
            thickness = 0.5f;
            _hitPoints = _maxHealth = 0.1f;
            editorOffset = new Vec2(0f, -4f);
            hugWalls = WallHug.Floor;
            _canHaveChance = false;
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Target
            };
            speediness.value = 1f;
        }

        public override void OnDrawLayer(Layer pLayer)
        {
        }

        public override void Initialize()
        {
            _profile = Profiles.EnvironmentProfile;
            InitProfile();
            _sprite = new SpriteMap("woodDuck", 32, 32);
            _base = new Sprite("popupPad");
            _woodWing = new Sprite("woodWing");
            graphic = _sprite;
            if (!(Level.current is Editor))
            {
                if (_stance != TargetStance.Fly)
                    scale = new Vec2(1f, 0f);
                else
                    scale = new Vec2(0f, 1f);
                ChallengeLevel.allTargetsShot = false;
                _autoFireWait = autofire.value;
            }
            if ((float)speediness == 0.0)
                speediness.value = 1f;
            _waitFire = (float)speediness;
            UpdateCollision();
        }

        public void SpawnHoldObject()
        {
            _autoFireWait = (float)autofire;
            if (!(contains != null) || !(Editor.CreateThing(contains) is Holdable thing))
                return;
            Level.Add(thing);
            GiveThing(thing);
        }

        public override void ReturnItemToWorld(Thing t)
        {
            Vec2 p1 = position + new Vec2(offDir * 3, 0f);
            Block block1 = Level.CheckLine<Block>(p1, p1 + new Vec2(16f, 0f));
            if (block1 != null && block1.solid && t.right > block1.left)
                t.right = block1.left;
            Block block2 = Level.CheckLine<Block>(p1, p1 - new Vec2(16f, 0f));
            if (block2 != null && block2.solid && t.left < block2.right)
                t.left = block2.right;
            Block block3 = Level.CheckLine<Block>(p1, p1 + new Vec2(0f, -16f));
            if (block3 != null && block3.solid && t.top < block3.bottom)
                t.top = block3.bottom;
            Block block4 = Level.CheckLine<Block>(p1, p1 + new Vec2(0f, 16f));
            if (block4 == null || !block4.solid || t.bottom <= block4.top)
                return;
            t.bottom = block4.top;
        }

        public void UpdateCollision()
        {
            if (Level.current is Editor || Level.current == null || _up && _popup)
            {
                crouch = false;
                sliding = false;
                if (_stance == TargetStance.Stand)
                {
                    _sprite.frame = 0;
                    _collisionOffset = new Vec2(-6f, -24f);
                    collisionSize = new Vec2(12f, 24f);
                    hugWalls = WallHug.Floor;
                }
                else if (_stance == TargetStance.StandArmed)
                {
                    _sprite.frame = 1;
                    _collisionOffset = new Vec2(-6f, -23f);
                    collisionSize = new Vec2(12f, 23f);
                    hugWalls = WallHug.Floor;
                }
                else if (_stance == TargetStance.Crouch)
                {
                    _sprite.frame = 2;
                    _collisionOffset = new Vec2(-6f, -18f);
                    collisionSize = new Vec2(12f, 18f);
                    crouch = true;
                    hugWalls = WallHug.Floor;
                }
                else if (_stance == TargetStance.Slide)
                {
                    _sprite.frame = 3;
                    _collisionOffset = new Vec2(-6f, -10f);
                    collisionSize = new Vec2(12f, 10f);
                    sliding = true;
                    hugWalls = WallHug.Floor;
                }
                else if (_stance == TargetStance.Fly)
                {
                    _sprite.frame = 4;
                    _collisionOffset = new Vec2(-8f, -24f);
                    collisionSize = new Vec2(16f, 24f);
                    hugWalls = WallHug.Left | WallHug.Right;
                }
            }
            else
            {
                hugWalls = WallHug.Floor;
                if (_stance == TargetStance.Stand)
                    _sprite.frame = 0;
                else if (_stance == TargetStance.StandArmed)
                    _sprite.frame = 1;
                else if (_stance == TargetStance.Crouch)
                    _sprite.frame = 2;
                else if (_stance == TargetStance.Slide)
                    _sprite.frame = 3;
                else if (_stance == TargetStance.Fly)
                {
                    _sprite.frame = 4;
                    hugWalls = WallHug.Left | WallHug.Right;
                }
                _collisionOffset = new Vec2(-6000f, 0f);
                collisionSize = new Vec2(2f, 2f);
            }
            _collisionOffset.y += 10f;
            --_collisionSize.y;
            _featherVolume.collisionSize = new Vec2(collisionSize.x + 2f, collisionSize.y + 2f);
            _featherVolume.collisionOffset = new Vec2(collisionOffset.x - 1f, collisionOffset.y - 1f);
        }

        public override void OnSequenceActivate() => _popup = true;

        public void PopDown()
        {
            _popup = false;
            if (holdObject != null)
            {
                Level.Remove(holdObject);
                holdObject = null;
            }
            foreach (Equipment equipment in _equipment)
            {
                if (equipment != null)
                    Level.Remove(equipment);
            }
            _equipment.Clear();
            _sequence.Finished();
        }

        public override bool Kill(DestroyType type = null)
        {
            if (_up && _popup)
            {
                if (ChallengeLevel.running)
                    ++ChallengeLevel.targetsShot;
                if (holdObject is Gun && !(bool)dropgun)
                    (holdObject as Gun).ammo = 0;
                ThrowItem(false);
                foreach (Equipment t in _equipment)
                {
                    if (t != null)
                    {
                        t.owner = null;
                        t.hSpeed = Rando.Float(2f) - 1f;
                        t.vSpeed = -Rando.Float(1.5f);
                        ReturnItemToWorld(t);
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
                    WoodDebris woodDebris = WoodDebris.New(x - 8f + Rando.Float(16f), y - 20f + Rando.Float(16f));
                    woodDebris.hSpeed = (float)((Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * Rando.Float(3f) + Math.Sign(vec2.x) * 0.5);
                    woodDebris.vSpeed = -Rando.Float(1f);
                    Level.Add(woodDebris);
                }
                for (int index = 0; index < 2; ++index)
                    Level.Add(Feather.New(x, y - 16f, persona));
                PopDown();
            }
            return false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos) => _up && _popup && base.Hit(bullet, hitPos);

        public override void ExitHit(Bullet bullet, Vec2 hitPos)
        {
            if (!_up || _popup)
                return;
            for (int index = 0; index < 2; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                woodDebris.hSpeed = (float)(-bullet.travelDirNormalized.x * 2.0 * (Rando.Float(1f) + 0.3f));
                woodDebris.vSpeed = (float)(-bullet.travelDirNormalized.y * 2.0 * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(woodDebris);
            }
        }

        public override bool Hurt(float points)
        {
            if (!_popup || _maxHealth == 0.0)
                return false;
            _hitPoints -= points;
            return true;
        }

        public void GiveThing(Holdable h)
        {
            holdObject = h;
            holdObject.owner = this;
            holdObject.solid = false;
        }

        public override void UpdateSkeleton()
        {
            int num = 6;
            int frame = 0;
            if (sliding)
                frame = 12;
            else if (crouch)
                frame = 11;
            _skeleton.head.position = Offset(DuckRig.GetHatPoint(frame) + new Vec2(0f, -num));
            _skeleton.upperTorso.position = Offset(DuckRig.GetChestPoint(frame) + new Vec2(0f, -num));
            _skeleton.lowerTorso.position = position + new Vec2(0f, 10 - num);
            if (sliding)
            {
                _skeleton.head.orientation = Maths.DegToRad(90f);
                _skeleton.upperTorso.orientation = Maths.DegToRad(90f);
            }
            else
            {
                _skeleton.head.orientation = 0f;
                _skeleton.upperTorso.orientation = 0f;
            }
        }

        public override void UpdateHoldPosition(bool updateLerp = true)
        {
            if (holdObject == null)
                return;
            holdOffY = 0f;
            armOffY = 0f;
            if ((!_up || !_popup) && !editorUpdate)
                return;
            holdObject.UpdateAction();
            holdObject.position = armPosition + holdObject.holdOffset + new Vec2(holdOffX, holdOffY) + new Vec2(2 * offDir, 0f);
            holdObject.offDir = offDir;
            if (_sprite.currentAnimation == "slide")
            {
                --holdOffY;
                ++holdOffX;
            }
            else if (crouch)
            {
                if (holdObject != null)
                    armOffY += 4f;
            }
            else if (sliding && holdObject != null)
                armOffY += 6f;
            holdObject.position = HoldOffset(holdObject.holdOffset) + new Vec2(offDir * 3, 0f);
            holdObject.angle = holdObject.handAngle + holdAngleOff;
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
                        _waitFire -= 0.03f;
                        break;
                    }
                }
            }
            bool flag = false;
            if (_autoFireWait > 0.0)
            {
                _autoFireWait -= Maths.IncFrameTimer();
                if (_autoFireWait <= 0.0)
                    flag = true;
            }
            if (_waitFire <= 0.0 | flag)
            {
                holdObject.PressAction();
                _waitFire = (float)speediness;
            }
            if (_waitFire >= (float)speediness)
                return;
            _waitFire += 0.01f;
        }

        public override void Update()
        {
            impacting.Clear();
            if (_up && _popup && holdObject is Gun)
                UpdateFire();
            UpdateCollision();
            UpdateSkeleton();
            if (_hitPoints <= 0.0)
                Destroy(new DTCrush(null));
            if (!_up)
            {
                _timeCount = 0f;
                if (_popup)
                    _upSpeed += 0.1f;
                if (_stance != TargetStance.Fly)
                {
                    yscale += _upSpeed;
                    if (yscale < 1.0)
                        return;
                    yscale = 1f;
                    _upSpeed = 0f;
                    _up = true;
                    SFX.Play("grappleHook", 0.7f, Rando.Float(-0.2f, 0.2f));
                    Level.Add(SmallSmoke.New(x - 4f, y));
                    Level.Add(SmallSmoke.New(x + 4f, y));
                    SpawnHoldObject();
                    if (helmet)
                    {
                        Helmet e = new Helmet(x, y);
                        Level.Add(e);
                        Equip(e);
                    }
                    if (!chestPlate)
                        return;
                    ChestPlate e1 = new ChestPlate(x, y);
                    Level.Add(e1);
                    Equip(e1);
                }
                else
                {
                    xscale += _upSpeed;
                    if (xscale < 1.0)
                        return;
                    xscale = 1f;
                    _upSpeed = 0f;
                    _up = true;
                    SFX.Play("grappleHook", 0.7f, Rando.Float(-0.2f, 0.2f));
                    Level.Add(SmallSmoke.New(x - 4f, y));
                    Level.Add(SmallSmoke.New(x + 4f, y));
                    if (helmet)
                    {
                        Helmet e = new Helmet(x, y);
                        Level.Add(e);
                        Equip(e);
                    }
                    if (!chestPlate)
                        return;
                    ChestPlate e2 = new ChestPlate(x, y);
                    Level.Add(e2);
                    Equip(e2);
                }
            }
            else
            {
                _timeCount += Maths.IncFrameTimer();
                if (_popup && time.value != 0.0 && _timeCount >= time.value)
                {
                    SFX.Play("grappleHook", 0.2f, Rando.Float(-0.2f, 0.2f));
                    PopDown();
                }
                else
                {
                    if (!_popup)
                        _upSpeed += 0.1f;
                    if (_stance != TargetStance.Fly)
                    {
                        yscale -= _upSpeed;
                        if (yscale >= 0.0)
                            return;
                        yscale = 0f;
                        _upSpeed = 0f;
                        _up = false;
                        SFX.Play("grappleHook", 0.2f, Rando.Float(-0.2f, 0.2f));
                        Level.Add(SmallSmoke.New(x - 4f, y));
                        Level.Add(SmallSmoke.New(x + 4f, y));
                        _hitPoints = _maxHealth = 0.1f;
                    }
                    else
                    {
                        xscale -= _upSpeed;
                        if (xscale >= 0.0)
                            return;
                        xscale = 0f;
                        _upSpeed = 0f;
                        _up = false;
                        SFX.Play("grappleHook", 0.2f, Rando.Float(-0.2f, 0.2f));
                        Level.Add(SmallSmoke.New(x - 4f, y));
                        Level.Add(SmallSmoke.New(x + 4f, y));
                        _hitPoints = _maxHealth = 0.1f;
                    }
                }
            }
        }

        public new void DrawIcon()
        {
            if (!ShouldDrawIcon() || !_up || !ChallengeMode.showReticles)
                return;
            if (_reticule == null)
                _reticule = new Sprite("challenge/reticule");
            Vec2 position1 = position;
            if (ragdoll != null)
                position1 = ragdoll.part1.position;
            else if (_trapped != null)
                position1 = _trapped.position;
            if ((position1 - Level.current.camera.position).length > Level.current.camera.width * 2.0)
                return;
            float num = 14f;
            if (position1.x < Level.current.camera.left + num)
                position1.x = Level.current.camera.left + num;
            if (position1.x > Level.current.camera.right - num)
                position1.x = Level.current.camera.right - num;
            if (position1.y < Level.current.camera.top + num)
                position1.y = Level.current.camera.top + num;
            if (position1.y > Level.current.camera.bottom - num)
                position1.y = Level.current.camera.bottom - num;
            Vec2 position2 = Layer.HUD.camera.transformInverse(Level.current.camera.transform(position1));
            DuckGame.Graphics.DrawRect(position2 + new Vec2(-5f, -5f), position2 + new Vec2(5f, 5f), Color.Black, (Depth)0.8f);
            DuckGame.Graphics.DrawRect(position2 + new Vec2(-5f, -5f), position2 + new Vec2(5f, 5f), Color.White, (Depth)0.81f, false);
            DuckGame.Graphics.Draw(_reticule.texture, position2, new Rectangle?(), Color.White, 0f, new Vec2(_reticule.width / 2, _reticule.height / 2), new Vec2(0.5f, 0.5f), SpriteEffects.None, (Depth)(0.9f + depth.span));
        }

        public override void Draw()
        {
            if (_trapped != null)
                y = -10000f;
            if (graphic == null)
                return;
            graphic.flipH = offDir <= 0;
            graphic.scale = scale;
            if (Level.current is Editor)
            {
                graphic.center = center;
                graphic.position = position;
            }
            else if (_stance != TargetStance.Fly)
            {
                graphic.center = center + new Vec2(0f, 10f);
                graphic.position = position + new Vec2(0f, 10f);
            }
            else
            {
                graphic.center = center + new Vec2(-12f, 10f);
                graphic.position = position + new Vec2(-12 * offDir, 10f);
            }
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            graphic.Draw();
            if (!_popup || !_up)
                return;
            DrawHat();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("stanceSetting", stanceSetting);
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            binaryClassChunk.AddProperty("chestPlate", chestPlate);
            binaryClassChunk.AddProperty("helmet", helmet);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            stanceSetting = node.GetProperty<int>("stanceSetting");
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            chestPlate = node.GetProperty<bool>("chestPlate");
            helmet = node.GetProperty<bool>("helmet");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("stanceSetting", Change.ToString(stanceSetting)));
            dxmlNode.Add(new DXMLNode("contains", contains != null ? contains.AssemblyQualifiedName : (object)""));
            dxmlNode.Add(new DXMLNode("chestPlate", Change.ToString(chestPlate)));
            dxmlNode.Add(new DXMLNode("helmet", Change.ToString(helmet)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode1 = node.Element("stanceSetting");
            if (dxmlNode1 != null)
                stanceSetting = Convert.ToInt32(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("contains");
            if (dxmlNode2 != null)
                contains = Editor.GetType(dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("chestPlate");
            if (dxmlNode3 != null)
                chestPlate = Convert.ToBoolean(dxmlNode3.Value);
            DXMLNode dxmlNode4 = node.Element("helmet");
            if (dxmlNode4 != null)
                helmet = Convert.ToBoolean(dxmlNode4.Value);
            return true;
        }

        public int stanceSetting
        {
            get => _stanceSetting;
            set
            {
                _stanceSetting = value;
                _stance = (TargetStance)_stanceSetting;
                UpdateCollision();
            }
        }

        public override string GetDetailsString()
        {
            string str = "NONE";
            if (contains != null)
                str = contains.Name;
            return base.GetDetailsString() + "Order: " + sequence.order.ToString() + "\nHolding: " + str;
        }

        public override void Netted(Net n)
        {
            base.Netted(n);
            y -= 10000f;
            _trapped.infinite = true;
        }

        public override void EditorUpdate()
        {
            if (chestPlate && GetEquipment(typeof(ChestPlate)) == null)
                Equip(new ChestPlate(0f, 0f), false);
            else if (!chestPlate)
            {
                Equipment equipment = GetEquipment(typeof(ChestPlate));
                if (equipment != null)
                    Unequip(equipment);
            }
            if (helmet && GetEquipment(typeof(Helmet)) == null)
                Equip(new Helmet(0f, 0f), false);
            else if (!helmet)
            {
                Equipment equipment = GetEquipment(typeof(Helmet));
                if (equipment != null)
                    Unequip(equipment);
            }
            if (contains != null)
            {
                if (holdObject == null || holdObject.GetType() != contains)
                    GiveHoldable(Editor.CreateThing(contains) as Holdable);
            }
            else
                holdObject = null;
            foreach (Thing thing in _equipment)
                thing.DoUpdate();
            if (holdObject != null)
            {
                editorUpdate = true;
                UpdateHoldPosition(true);
                holdObject.DoUpdate();
                editorUpdate = false;
            }
            base.EditorUpdate();
        }

        public override void EditorRender()
        {
            foreach (Thing thing in _equipment)
                thing.DoDraw();
            if (holdObject != null)
            {
                holdObject.depth = (Depth)0.9f;
                holdObject.DoDraw();
            }
            base.EditorRender();
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextRadio("Stand", stanceSetting == 0, 0, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextRadio("Crouch", stanceSetting == 2, 2, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextRadio("Slide", stanceSetting == 3, 3, null, new FieldBinding(this, "stanceSetting")));
            contextMenu.AddItem(new ContextRadio("Fly", stanceSetting == 4, 4, null, new FieldBinding(this, "stanceSetting")));
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
