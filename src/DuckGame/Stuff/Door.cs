// Decompiled with JetBrains decompiler
// Type: DuckGame.Door
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Doors")]
    public class Door : Block, IPlatform, IDontMove, ISequenceItem
    {
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints");
        public StateBinding _openBinding = new StateBinding(nameof(_open));
        public StateBinding _openForceBinding = new StateBinding(nameof(_openForce));
        public StateBinding _jiggleBinding = new StateBinding(nameof(_jiggle));
        public StateBinding _jamBinding = new StateBinding(nameof(_jam));
        public StateBinding _damageMultiplierBinding = new StateBinding(nameof(damageMultiplier));
        public StateBinding _doorInstanceBinding = new StateBinding(nameof(_doorInstance));
        public StateBinding _doorStateBinding = new DoorFlagBinding();
        private DoorOffHinges _doorInstanceInternal;
        public float damageMultiplier = 1f;
        protected SpriteMap _sprite;
        public bool landed = true;
        public bool locked;
        public bool _lockDoor;
        public float _open;
        public float _openForce;
        private Vec2 _topLeft;
        private Vec2 _topRight;
        private Vec2 _bottomLeft;
        private Vec2 _bottomRight;
        private bool _cornerInit;
        public bool _jammed;
        public float _jiggle;
        public bool _didJiggle;
        public new bool _initialized;
        public float colWide = 6f;
        public float _jam = 1f;
        private Dictionary<Mine, float> _mines = new Dictionary<Mine, float>();
        private Sprite _lock;
        private bool _opened;
        private SpriteMap _key;
        public DoorFrame _frame;
        private bool _fucked;
        private List<PhysicsObject> _coll;
        public EditorProperty<bool> objective;
        protected bool secondaryFrame;
        private bool _lockedSprite;
        public bool networkUnlockMessage;
        private bool didUnlock;
        private bool prevLocked;
        private List<Mine> _removeMines = new List<Mine>();

        public DoorOffHinges _doorInstance
        {
            get => this._doorInstanceInternal;
            set => this._doorInstanceInternal = value;
        }

        public override void SetTranslation(Vec2 translation)
        {
            if (this._frame != null)
                this._frame.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public override void EditorPropertyChanged(object property) => this.sequence.isValid = this.objective.value;

        public Door(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.objective = new EditorProperty<bool>(false, this);
            this._maxHealth = 50f;
            this._hitPoints = 50f;
            this._sprite = new SpriteMap("door", 32, 32);
            this.graphic = _sprite;
            this.center = new Vec2(16f, 25f);
            this.collisionSize = new Vec2(6f, 32f);
            this.collisionOffset = new Vec2(-3f, -25f);
            this.depth = - 0.5f;
            this._editorName = nameof(Door);
            this.thickness = 2f;
            this._lock = new Sprite("lock");
            this._lock.CenterOrigin();
            this._impactThreshold = 0.0f;
            this._key = new SpriteMap("keyInDoor", 16, 16)
            {
                center = new Vec2(2f, 8f)
            };
            this._canFlip = false;
            this.physicsMaterial = PhysicsMaterial.Wood;
            this.sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            this._placementCost += 6;
            this._coll = new List<PhysicsObject>();
            this.editorTooltip = "Your basic door type door. Blocks some projectiles. If locked, needs a key to open.";
        }

        public override void Initialize()
        {
            this.sequence.isValid = this.objective.value;
            this._lockDoor = this.locked;
            if (this._lockDoor)
            {
                this._sprite = new SpriteMap("lockDoor", 32, 32);
                this.graphic = _sprite;
                this._lockedSprite = true;
            }
            else
            {
                this._frame = new DoorFrame(this.x, this.y - 1f, this.secondaryFrame);
                Level.Add(_frame);
            }
        }

        public override void Terminate()
        {
            if (_hitPoints > 5.0 && !Network.isActive)
            {
                Level.Remove(_frame);
                this._frame = null;
            }
            base.Terminate();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this._lockDoor || this._destroyed || !this.isServerForObject)
                return false;
            this._hitPoints = 0.0f;
            Level.Remove(this);
            if (this.sequence != null && this.sequence.isValid)
            {
                this.sequence.Finished();
                if (ChallengeLevel.running)
                    ++ChallengeLevel.goodiesGot;
            }
            DoorOffHinges t = null;
            if (Network.isActive)
            {
                if (this._doorInstance != null)
                {
                    t = this._doorInstance;
                    t.visible = true;
                    t.active = true;
                    t.solid = true;
                    Thing.Fondle(this, DuckNetwork.localConnection);
                    Thing.Fondle(t, DuckNetwork.localConnection);
                }
            }
            else
                t = new DoorOffHinges(this.x, this.y - 8f, this.secondaryFrame);
            if (t != null)
            {
                if (type is DTShot dtShot && dtShot.bullet != null)
                {
                    t.hSpeed = dtShot.bullet.travelDirNormalized.x * 2f;
                    t.vSpeed = (float)(dtShot.bullet.travelDirNormalized.y * 2.0 - 1.0);
                    t.offDir = dtShot.bullet.travelDirNormalized.x > 0.0 ? (sbyte)1 : (sbyte)-1;
                }
                else
                {
                    t.hSpeed = offDir * 2f;
                    t.vSpeed = -2f;
                    t.offDir = this.offDir;
                }
                if (!Network.isActive)
                {
                    Level.Add(t);
                    t.MakeEffects();
                }
            }
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (_hitPoints <= 0.0)
                return base.Hit(bullet, hitPos);
            hitPos -= bullet.travelDirNormalized;
            if (this.physicsMaterial == PhysicsMaterial.Wood)
            {
                for (int index = 0; index < 1.0 + damageMultiplier / 2.0; ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                    woodDebris.hSpeed = (float)(-(double)bullet.travelDirNormalized.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                    woodDebris.vSpeed = (float)(-(double)bullet.travelDirNormalized.y * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929)) - Rando.Float(2f);
                    Level.Add(woodDebris);
                }
                SFX.Play("woodHit");
            }
            if (this.isServerForObject && bullet.isLocal)
            {
                this._hitPoints -= this.damageMultiplier * 4f;
                ++this.damageMultiplier;
                if (_hitPoints <= 0.0 && !this.destroyed)
                    this.Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos += bullet.travelDirNormalized;
            for (int index = 0; index < 1.0 + damageMultiplier / 2.0; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(exitPos.x, exitPos.y);
                woodDebris.hSpeed = (float)(bullet.travelDirNormalized.x * 3.0 * ((double)Rando.Float(1f) + 0.300000011920929));
                woodDebris.vSpeed = (float)(bullet.travelDirNormalized.y * 3.0 * ((double)Rando.Float(1f) + 0.300000011920929) - ((double)Rando.Float(2f) - 1.0));
                Level.Add(woodDebris);
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.isServerForObject && this.locked && with is Key)
                this.UnlockDoor(with as Key);
            base.OnSoftImpact(with, from);
        }

        public void UnlockDoor(Key with)
        {
            if (!this.locked || !with.isServerForObject)
                return;
            if (Network.isActive)
            {
                Thing.ExtraFondle(this, with.connection);
                Send.Message(new NMUnlockDoor(this));
                this.networkUnlockMessage = true;
            }
            this.locked = false;
            if (with.owner is Duck owner)
            {
                RumbleManager.AddRumbleEvent(owner.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                owner.ThrowItem();
            }
            Level.Remove(with);
            if (Network.isActive)
                return;
            this.DoUnlock(with.position);
        }

        public void DoUnlock(Vec2 keyPos)
        {
            SFX.Play("deedleBeep");
            Level.Add(SmallSmoke.New(keyPos.x, keyPos.y));
            for (int index = 0; index < 3; ++index)
                Level.Add(SmallSmoke.New(this.x + Rando.Float(-3f, 3f), this.y + Rando.Float(-3f, 3f)));
            this.didUnlock = true;
        }

        public override void Update()
        {
            if (this._doorInstance == null && Network.isActive && this.isServerForObject)
            {
                this._doorInstance = new DoorOffHinges(this.x, this.y - 8f, this.secondaryFrame)
                {
                    active = false,
                    visible = false,
                    solid = false
                };
                Level.Add(_doorInstance);
            }
            if (!this._lockDoor && this.locked)
            {
                this._sprite = new SpriteMap("lockDoor", 32, 32);
                this.graphic = _sprite;
                this._lockedSprite = true;
                this._lockDoor = true;
            }
            if (this.networkUnlockMessage)
                this.locked = false;
            if (Network.isActive && !this.locked && this.prevLocked && !this.didUnlock)
                this.DoUnlock(this.position);
            this.prevLocked = this.locked;
            if (this._lockDoor)
            {
                this._hitPoints = 100f;
                this.physicsMaterial = PhysicsMaterial.Metal;
                this.thickness = 4f;
            }
            if (!this._fucked && _hitPoints < _maxHealth / 2.0)
            {
                this._sprite = new SpriteMap(this.secondaryFrame ? "flimsyDoorDamaged" : "doorFucked", 32, 32);
                this.graphic = _sprite;
                this._fucked = true;
            }
            if (!this._cornerInit)
            {
                this._topLeft = this.topLeft;
                this._topRight = this.topRight;
                this._bottomLeft = this.bottomLeft;
                this._bottomRight = this.bottomRight;
                this._cornerInit = true;
            }
            base.Update();
            if (damageMultiplier > 1.0)
                this.damageMultiplier -= 0.2f;
            else
                this.damageMultiplier = 1f;
            this._removeMines.Clear();
            foreach (KeyValuePair<Mine, float> mine in this._mines)
            {
                if ((double)mine.Value < 0.0 && _open > (double)mine.Value || (double)mine.Value >= 0.0 && _open < (double)mine.Value)
                {
                    mine.Key.addWeight = 0.0f;
                    this._removeMines.Add(mine.Key);
                }
                else
                    mine.Key.addWeight = 3f;
            }
            foreach (Mine removeMine in this._removeMines)
                this._mines.Remove(removeMine);
            bool flag1 = false;
            PhysicsObject t1 = null;
            if (_open < 0.899999976158142 && _open > -0.899999976158142)
            {
                bool flag2 = false;
                Thing thing = Level.CheckRectFilter<Duck>(this._topLeft - new Vec2(18f, 0.0f), this._bottomRight + new Vec2(18f, 0.0f), d => !(d is TargetDuck));
                if (thing == null)
                {
                    thing = Level.CheckRectFilter<Duck>(this._topLeft - new Vec2(32f, 0.0f), this._bottomRight + new Vec2(32f, 0.0f), d => !(d is TargetDuck) && (double)Math.Abs(d.hSpeed) > 4.0);
                    flag2 = true;
                }
                if (thing != null)
                {
                    (thing as Duck).Fondle(this);
                    if ((double)thing.x < (double)this.x)
                    {
                        this._coll.Clear();
                        Level.CheckRectAll<PhysicsObject>(this._topRight, this._bottomRight + new Vec2(10f, 0.0f), this._coll);
                        bool flag3 = true;
                        this._jam = 1f;
                        foreach (PhysicsObject t2 in this._coll)
                        {
                            if (!(t2 is TeamHat) && !(t2 is Duck) && (double)t2.weight > 3.0 && t2.owner == null && (!(t2 is Holdable) || (t2 as Holdable).hoverSpawner == null))
                            {
                                if (t2 is RagdollPart)
                                {
                                    this.Fondle(t2);
                                    t2.hSpeed = 2f;
                                }
                                else
                                {
                                    float num = Maths.Clamp((float)(((double)t2.left - _bottomRight.x) / 14.0), 0.0f, 1f);
                                    if ((double)num < 0.100000001490116)
                                        num = 0.1f;
                                    if (_jam > (double)num)
                                    {
                                        if (_open != 0.0 && t2 is Gun)
                                        {
                                            if (t2 is Mine key && !key.pin && !this._mines.ContainsKey(key))
                                                this._mines[key] = this._open;
                                        }
                                        else
                                        {
                                            this._jam = num;
                                            t1 = t2;
                                        }
                                    }
                                }
                            }
                        }
                        this._coll.Clear();
                        if (this.locked)
                        {
                            this._jam = 0.1f;
                            if (!this._didJiggle)
                            {
                                this._jiggle = 1f;
                                this._didJiggle = true;
                            }
                        }
                        if (flag3)
                        {
                            if (flag2)
                                this._openForce += 0.25f;
                            else
                                this._openForce += 0.08f;
                        }
                    }
                    else
                    {
                        this._coll.Clear();
                        Level.CheckRectAll<PhysicsObject>(this._topLeft - new Vec2(10f, 0.0f), this._bottomLeft, this._coll);
                        bool flag4 = true;
                        this._jam = -1f;
                        foreach (PhysicsObject t3 in this._coll)
                        {
                            if (!(t3 is TeamHat) && !(t3 is Duck) && (double)t3.weight > 3.0 && t3.owner == null && (!(t3 is Holdable) || (t3 as Holdable).hoverSpawner == null))
                            {
                                if (t3 is RagdollPart)
                                {
                                    this.Fondle(t3);
                                    t3.hSpeed = -2f;
                                }
                                else
                                {
                                    float num = Maths.Clamp((float)(((double)t3.right - (double)this.left) / 14.0), -1f, 0.0f);
                                    if ((double)num > -0.100000001490116)
                                        num = -0.1f;
                                    if (_jam < (double)num)
                                    {
                                        if (_open != 0.0 && t3 is Gun)
                                        {
                                            if (t3 is Mine key && !key.pin && !this._mines.ContainsKey(key))
                                                this._mines[key] = this._open;
                                        }
                                        else
                                        {
                                            this._jam = num;
                                            t1 = t3;
                                        }
                                    }
                                }
                            }
                        }
                        this._coll.Clear();
                        if (this.locked)
                        {
                            this._jam = -0.1f;
                            if (!this._didJiggle)
                            {
                                this._jiggle = 1f;
                                this._didJiggle = true;
                            }
                        }
                        if (flag4)
                        {
                            if (flag2)
                                this._openForce -= 0.25f;
                            else
                                this._openForce -= 0.08f;
                        }
                    }
                }
                else
                    this._didJiggle = false;
            }
            this._coll.Clear();
            Level.CheckRectAll<PhysicsObject>(this._topLeft - new Vec2(18f, 0.0f), this._bottomRight + new Vec2(18f, 0.0f), this._coll);
            foreach (PhysicsObject t4 in this._coll)
            {
                if (!(t4 is TeamHat) && (t4 is Duck || !this._jammed) && (!(t4 is Holdable) || t4 is Mine || (t4 as Holdable).canPickUp) && t4.solid)
                {
                    if (!(t4 is Duck) && (double)this.weight < 3.0)
                    {
                        if (_open < -0.0)
                        {
                            this.Fondle(t4);
                            t4.hSpeed = 3f;
                        }
                        else if (_open > 0.0)
                        {
                            this.Fondle(t4);
                            t4.hSpeed = -3f;
                        }
                    }
                    if (_open < -0.0 && t4 != null && (t4 is Duck || (double)t4.right > _topLeft.x - 10.0 && (double)t4.left < _topRight.x))
                        flag1 = true;
                    if (_open > 0.0 && t4 != null && (t4 is Duck || (double)t4.left < _topRight.x + 10.0 && (double)t4.right > _topLeft.x))
                        flag1 = true;
                }
            }
            this._jiggle = Maths.CountDown(this._jiggle, 0.08f);
            if (!flag1)
            {
                if (_openForce > 1.0)
                    this._openForce = 1f;
                if (_openForce < -1.0)
                    this._openForce = -1f;
                if (_openForce > 0.0399999991059303)
                    this._openForce -= 0.04f;
                else if (_openForce < -0.0399999991059303)
                    this._openForce += 0.04f;
                else if (_openForce > -0.0599999986588955 && _openForce < 0.0599999986588955)
                    this._openForce = 0.0f;
            }
            this._open += this._openForce;
            if ((double)Math.Abs(this._open) > 0.5 && !this._opened)
            {
                this._opened = true;
                SFX.Play("doorOpen", Rando.Float(0.8f, 0.9f), Rando.Float(-0.1f, 0.1f));
            }
            else if ((double)Math.Abs(this._open) < 0.100000001490116 && this._opened)
            {
                this._opened = false;
                SFX.Play("doorClose", Rando.Float(0.5f, 0.6f), Rando.Float(-0.1f, 0.1f));
            }
            if (_open > 1.0)
                this._open = 1f;
            if (_open < -1.0)
                this._open = -1f;
            if (_jam > 0.0 && _open > (double)this._jam)
            {
                if (!this._jammed)
                {
                    if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            SFX.PlaySynchronized("doorJam");
                    }
                    else
                        SFX.Play("doorJam");
                    this._jammed = true;
                    if (t1 != null)
                    {
                        t1.hSpeed += 0.6f;
                        this.Fondle(t1);
                    }
                }
                this._open = this._jam;
                if (_openForce > 0.100000001490116)
                    this._openForce = 0.1f;
            }
            if (_jam < 0.0 && _open < (double)this._jam)
            {
                if (!this._jammed)
                {
                    if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            SFX.PlaySynchronized("doorJam");
                    }
                    else
                        SFX.Play("doorJam");
                    this._jammed = true;
                    if (t1 != null)
                    {
                        t1.hSpeed -= 0.6f;
                        this.Fondle(t1);
                    }
                }
                this._open = this._jam;
                if (_openForce < -0.100000001490116)
                    this._openForce = -0.1f;
            }
            if (_open > 0.0)
            {
                this._sprite.flipH = false;
                this._sprite.frame = (int)(_open * 15.0);
            }
            else
            {
                this._sprite.flipH = true;
                this._sprite.frame = (int)((double)Math.Abs(this._open) * 15.0);
            }
            if (this._sprite.frame > 9)
            {
                this.collisionSize = new Vec2(0.0f, 0.0f);
                this.solid = false;
                this.collisionOffset = new Vec2(0.0f, -999999f);
                this.depth = - 0.7f;
            }
            else
            {
                this.collisionSize = new Vec2(this.colWide, 32f);
                this.solid = true;
                this.collisionOffset = new Vec2((float)(-(double)this.colWide / 2.0), -24f);
                this.depth = - 0.5f;
            }
            if (_hitPoints <= 0.0 && !this._destroyed)
                this.Destroy(new DTImpact(this));
            if (_openForce == 0.0)
                this._open = Maths.LerpTowards(this._open, 0.0f, 0.1f);
            if (_open == 0.0)
                this._jammed = false;
            float num1 = (float)(_hitPoints / (double)this._maxHealth * 0.200000002980232 + 0.800000011920929);
            this._sprite.color = new Color(num1, num1, num1);
        }

        public override void Draw()
        {
            base.Draw();
            if (Level.current is Editor)
            {
                if (this.locked && !this._lockedSprite)
                {
                    this._sprite = new SpriteMap("lockDoor", 32, 32);
                    this.graphic = _sprite;
                    this._lockedSprite = true;
                }
                else if (!this.locked && this._lockedSprite)
                {
                    this._sprite = new SpriteMap("door", 32, 32);
                    this.graphic = _sprite;
                    this._lockedSprite = false;
                }
            }
            if (!this._lockDoor || this.locked)
                return;
            this._key.frame = this._sprite.frame;
            if (this._key.frame > 12)
                this._key.depth = this.depth - 1;
            else
                this._key.depth = this.depth + 1;
            this._key.flipH = this.graphic.flipH;
            Graphics.Draw(_key, this.x + this._open * 12f, this.y - 8f);
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("locked", locked);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.locked = node.GetProperty<bool>("locked");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("locked", Change.ToString(locked)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("locked");
            if (dxmlNode != null)
                this.locked = Convert.ToBoolean(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            ContextMenu contextMenu = base.GetContextMenu();
            contextMenu.AddItem(new ContextCheckBox("Locked", null, new FieldBinding(this, "locked")));
            return contextMenu;
        }
    }
}
