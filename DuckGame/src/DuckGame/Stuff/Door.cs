// Decompiled with JetBrains decompiler
// Type: DuckGame.Door
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => _doorInstanceInternal;
            set => _doorInstanceInternal = value;
        }

        public override void SetTranslation(Vec2 translation)
        {
            if (_frame != null)
                _frame.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public override void EditorPropertyChanged(object property) => sequence.isValid = objective.value;

        public Door(float xpos, float ypos)
          : base(xpos, ypos)
        {
            shouldbegraphicculled = false;
            objective = new EditorProperty<bool>(false, this);
            _maxHealth = 50f;
            _hitPoints = 50f;
            _sprite = new SpriteMap("door", 32, 32);
            graphic = _sprite;
            center = new Vec2(16f, 25f);
            collisionSize = new Vec2(6f, 32f);
            collisionOffset = new Vec2(-3f, -25f);
            depth = -0.5f;
            _editorName = nameof(Door);
            thickness = 2f;
            _lock = new Sprite("lock");
            _lock.CenterOrigin();
            _impactThreshold = 0f;
            _key = new SpriteMap("keyInDoor", 16, 16)
            {
                center = new Vec2(2f, 8f)
            };
            _canFlip = false;
            physicsMaterial = PhysicsMaterial.Wood;
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            _placementCost += 6;
            _coll = new List<PhysicsObject>();
            editorTooltip = "Your basic door type door. Blocks some projectiles. If locked, needs a key to open.";
        }

        public override void Initialize()
        {
            sequence.isValid = objective.value;
            _lockDoor = locked;
            if (_lockDoor)
            {
                _sprite = new SpriteMap("lockDoor", 32, 32);
                graphic = _sprite;
                _lockedSprite = true;
            }
            else
            {
                _frame = new DoorFrame(x, y - 1f, secondaryFrame);
                Level.Add(_frame);
            }
        }

        public override void Terminate()
        {
            if (_hitPoints > 5.0 && !Network.isActive)
            {
                Level.Remove(_frame);
                _frame = null;
            }
            base.Terminate();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (_lockDoor || _destroyed || !isServerForObject)
                return false;
            _hitPoints = 0f;
            Level.Remove(this);
            if (sequence != null && sequence.isValid)
            {
                sequence.Finished();
                if (ChallengeLevel.running)
                    ++ChallengeLevel.goodiesGot;
            }
            DoorOffHinges t = null;
            if (Network.isActive)
            {
                if (_doorInstance != null)
                {
                    t = _doorInstance;
                    t.visible = true;
                    t.active = true;
                    t.solid = true;
                    Fondle(this, DuckNetwork.localConnection);
                    Fondle(t, DuckNetwork.localConnection);
                }
            }
            else
                t = new DoorOffHinges(x, y - 8f, secondaryFrame);
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
                    t.offDir = offDir;
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
                Fondle(this, DuckNetwork.localConnection);
            if (_hitPoints <= 0.0)
                return base.Hit(bullet, hitPos);
            hitPos -= bullet.travelDirNormalized;
            if (physicsMaterial == PhysicsMaterial.Wood)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                    woodDebris.hSpeed = (-bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f));
                    woodDebris.vSpeed = (-bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                    Level.Add(woodDebris);
                }
                SFX.Play("woodHit");
            }
            if (isServerForObject && bullet.isLocal)
            {
                _hitPoints -= damageMultiplier * 4f;
                ++damageMultiplier;
                if (_hitPoints <= 0f && !destroyed)
                    Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos += bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(exitPos.x, exitPos.y);
                woodDebris.hSpeed = (bullet.travelDirNormalized.x * 3f * (Rando.Float(1f) + 0.3f));
                woodDebris.vSpeed = (bullet.travelDirNormalized.y * 3f * (Rando.Float(1f) + 0.3f) - (Rando.Float(2f) - 1f));
                Level.Add(woodDebris);
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.isServerForObject && locked && with is Key)
                UnlockDoor(with as Key);
            base.OnSoftImpact(with, from);
        }

        public void UnlockDoor(Key with)
        {
            if (!locked || !with.isServerForObject)
                return;
            if (Network.isActive)
            {
                ExtraFondle(this, with.connection);
                Send.Message(new NMUnlockDoor(this));
                networkUnlockMessage = true;
            }
            locked = false;
            if (with.owner is Duck owner)
            {
                RumbleManager.AddRumbleEvent(owner.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                owner.ThrowItem();
            }
            Level.Remove(with);
            if (Network.isActive)
                return;
            DoUnlock(with.position);
        }

        public void DoUnlock(Vec2 keyPos)
        {
            SFX.Play("deedleBeep");
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                Level.Add(SmallSmoke.New(keyPos.x, keyPos.y));
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 3; ++index)
                    Level.Add(SmallSmoke.New(x + Rando.Float(-3f, 3f), y + Rando.Float(-3f, 3f)));
            }
            didUnlock = true;
        }

        public override void Update()
        {
            if (_doorInstance == null && Network.isActive && isServerForObject)
            {
                _doorInstance = new DoorOffHinges(x, y - 8f, secondaryFrame)
                {
                    active = false,
                    visible = false,
                    solid = false
                };
                Level.Add(_doorInstance);
            }
            if (!_lockDoor && locked)
            {
                _sprite = new SpriteMap("lockDoor", 32, 32);
                graphic = _sprite;
                _lockedSprite = true;
                _lockDoor = true;
            }
            if (networkUnlockMessage)
                locked = false;
            if (Network.isActive && !locked && prevLocked && !didUnlock)
                DoUnlock(position);
            prevLocked = locked;
            if (_lockDoor)
            {
                _hitPoints = 100f;
                physicsMaterial = PhysicsMaterial.Metal;
                thickness = 4f;
            }
            if (!_fucked && _hitPoints < _maxHealth / 2.0)
            {
                _sprite = new SpriteMap(secondaryFrame ? "flimsyDoorDamaged" : "doorFucked", 32, 32);
                graphic = _sprite;
                _fucked = true;
            }
            if (!_cornerInit)
            {
                _topLeft = topLeft;
                _topRight = topRight;
                _bottomLeft = bottomLeft;
                _bottomRight = bottomRight;
                _cornerInit = true;
            }
            base.Update();
            if (damageMultiplier > 1.0)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            _removeMines.Clear();
            foreach (KeyValuePair<Mine, float> mine in _mines)
            {
                if (mine.Value < 0f && _open > mine.Value || mine.Value >= 0f && _open < mine.Value)
                {
                    mine.Key.addWeight = 0f;
                    _removeMines.Add(mine.Key);
                }
                else
                    mine.Key.addWeight = 3f;
            }
            foreach (Mine removeMine in _removeMines)
                _mines.Remove(removeMine);
            bool flag1 = false;
            PhysicsObject t1 = null;
            if (_open < 0.9f && _open > -0.9f)
            {
                bool flag2 = false;
                Duck search2 = null;
                Thing thing = null;//Level.CheckRectFilter<Duck>(_topLeft - new Vec2(18f, 0f), _bottomRight + new Vec2(18f, 0f), d => !(d is TargetDuck));
                foreach (Duck d in Level.CheckRectAll<Duck>(_topLeft - new Vec2(32f, 0f), _bottomRight + new Vec2(32f, 0f)))
                {
                    if (!(d is TargetDuck))
                    {
                        if (Collision.Rect(_topLeft - new Vec2(18f, 0f), _bottomRight + new Vec2(18f, 0f), d))
                        {
                            thing = d;
                            break;
                        }
                        if (Math.Abs(d.hSpeed) > 4.0)
                        {
                            search2 = d;
                        }
                    }
                }
                if (thing == null)
                {
                    thing = search2;//Level.CheckRectFilter<Duck>(_topLeft - new Vec2(32f, 0f), _bottomRight + new Vec2(32f, 0f), d => !(d is TargetDuck) && Math.Abs(d.hSpeed) > 4.0);
                    flag2 = true;
                }
                if (thing != null)
                {
                    (thing as Duck).Fondle(this);
                    if (thing.x < x)
                    {
                        _coll.Clear();
                        Level.CheckRectAll(_topRight, _bottomRight + new Vec2(10f, 0f), _coll);
                        bool flag3 = true;
                        _jam = 1f;
                        foreach (PhysicsObject t2 in _coll)
                        {
                            if (!(t2 is TeamHat) && !(t2 is Duck) && t2.weight > 3.0 && t2.owner == null && (!(t2 is Holdable) || (t2 as Holdable).hoverSpawner == null))
                            {
                                if (t2 is RagdollPart)
                                {
                                    Fondle(t2);
                                    t2.hSpeed = 2f;
                                }
                                else
                                {
                                    float num = Maths.Clamp(((t2.left - _bottomRight.x) / 14f), 0f, 1f);
                                    if (num < 0.1f)
                                        num = 0.1f;
                                    if (_jam > num)
                                    {
                                        if (_open != 0f && t2 is Gun)
                                        {
                                            if (t2 is Mine key && !key.pin && !_mines.ContainsKey(key))
                                                _mines[key] = _open;
                                        }
                                        else
                                        {
                                            _jam = num;
                                            t1 = t2;
                                        }
                                    }
                                }
                            }
                        }
                        _coll.Clear();
                        if (locked)
                        {
                            _jam = 0.1f;
                            if (!_didJiggle)
                            {
                                _jiggle = 1f;
                                _didJiggle = true;
                            }
                        }
                        if (flag3)
                        {
                            if (flag2)
                                _openForce += 0.25f;
                            else
                                _openForce += 0.08f;
                        }
                    }
                    else
                    {
                        _coll.Clear();
                        Level.CheckRectAll(_topLeft - new Vec2(10f, 0f), _bottomLeft, _coll);
                        bool flag4 = true;
                        _jam = -1f;
                        foreach (PhysicsObject t3 in _coll)
                        {
                            if (!(t3 is TeamHat) && !(t3 is Duck) && t3.weight > 3f && t3.owner == null && (!(t3 is Holdable) || (t3 as Holdable).hoverSpawner == null))
                            {
                                if (t3 is RagdollPart)
                                {
                                    Fondle(t3);
                                    t3.hSpeed = -2f;
                                }
                                else
                                {
                                    float num = Maths.Clamp((t3.right - left) / 14f, -1f, 0f);
                                    if (num > -0.1f)
                                        num = -0.1f;
                                    if (_jam < num)
                                    {
                                        if (_open != 0f && t3 is Gun)
                                        {
                                            if (t3 is Mine key && !key.pin && !_mines.ContainsKey(key))
                                                _mines[key] = _open;
                                        }
                                        else
                                        {
                                            _jam = num;
                                            t1 = t3;
                                        }
                                    }
                                }
                            }
                        }
                        _coll.Clear();
                        if (locked)
                        {
                            _jam = -0.1f;
                            if (!_didJiggle)
                            {
                                _jiggle = 1f;
                                _didJiggle = true;
                            }
                        }
                        if (flag4)
                        {
                            if (flag2)
                                _openForce -= 0.25f;
                            else
                                _openForce -= 0.08f;
                        }
                    }
                }
                else
                    _didJiggle = false;
            }
            if (_open < -0.0 || _open > 0.0)
            {
                _coll.Clear();
                Level.CheckRectAll(_topLeft - new Vec2(18f, 0f), _bottomRight + new Vec2(18f, 0f), _coll);
                foreach (PhysicsObject t4 in _coll)
                {
                    if (!(t4 is TeamHat) && (t4 is Duck || !_jammed) && (!(t4 is Holdable) || t4 is Mine || (t4 as Holdable).canPickUp) && t4.solid)
                    {
                        if (!(t4 is Duck) && weight < 3.0)
                        {
                            if (_open < -0.0)
                            {
                                Fondle(t4);
                                t4.hSpeed = 3f;
                            }
                            else if (_open > 0.0)
                            {
                                Fondle(t4);
                                t4.hSpeed = -3f;
                            }
                        }
                        if (_open < -0.0 && t4 != null && (t4 is Duck || t4.right > _topLeft.x - 10.0 && t4.left < _topRight.x))
                            flag1 = true;
                        if (_open > 0.0 && t4 != null && (t4 is Duck || t4.left < _topRight.x + 10.0 && t4.right > _topLeft.x))
                            flag1 = true;
                    }
                }
            }

            _jiggle = Maths.CountDown(_jiggle, 0.08f);
            if (!flag1)
            {
                if (_openForce > 1f)
                    _openForce = 1f;
                if (_openForce < -1f)
                    _openForce = -1f;
                if (_openForce > 0.04f)
                    _openForce -= 0.04f;
                else if (_openForce < -0.04f)
                    _openForce += 0.04f;
                else if (_openForce > -0.06f && _openForce < 0.06f)
                    _openForce = 0f;
            }
            _open += _openForce;
            if (Math.Abs(_open) > 0.5f && !_opened)
            {
                _opened = true;
                SFX.Play("doorOpen", Rando.Float(0.8f, 0.9f), Rando.Float(-0.1f, 0.1f));
            }
            else if (Math.Abs(_open) < 0.1f && _opened)
            {
                _opened = false;
                SFX.Play("doorClose", Rando.Float(0.5f, 0.6f), Rando.Float(-0.1f, 0.1f));
            }
            if (_open > 1.0)
                _open = 1f;
            if (_open < -1.0)
                _open = -1f;
            if (_jam > 0f && _open > _jam)
            {
                if (!_jammed)
                {
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            SFX.PlaySynchronized("doorJam");
                    }
                    else
                        SFX.Play("doorJam");
                    _jammed = true;
                    if (t1 != null)
                    {
                        t1.hSpeed += 0.6f;
                        Fondle(t1);
                    }
                }
                _open = _jam;
                if (_openForce > 0.1f)
                    _openForce = 0.1f;
            }
            if (_jam < 0f && _open < _jam)
            {
                if (!_jammed)
                {
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            SFX.PlaySynchronized("doorJam");
                    }
                    else
                        SFX.Play("doorJam");
                    _jammed = true;
                    if (t1 != null)
                    {
                        t1.hSpeed -= 0.6f;
                        Fondle(t1);
                    }
                }
                _open = _jam;
                if (_openForce < -0.1f)
                    _openForce = -0.1f;
            }
            if (_open > 0f)
            {
                _sprite.flipH = false;
                _sprite.frame = (int)(_open * 15f);
            }
            else
            {
                _sprite.flipH = true;
                _sprite.frame = (int)(Math.Abs(_open) * 15f);
            }
            if (_sprite.frame > 9)
            {
                collisionSize = new Vec2(0f, 0f);
                solid = false;
                collisionOffset = new Vec2(0f, -999999f); // ok landon
                depth = -0.7f;
            }
            else
            {
                collisionSize = new Vec2(colWide, 32f);
                solid = true;
                collisionOffset = new Vec2((float)(-colWide / 2f), -24f);
                depth = -0.5f;
            }
            if (_hitPoints <= 0f && !_destroyed)
                Destroy(new DTImpact(this));
            if (_openForce == 0f)
                _open = Maths.LerpTowards(_open, 0f, 0.1f);
            if (_open == 0f)
                _jammed = false;
            float num1 = (_hitPoints / _maxHealth * 0.2f + 0.8f);
            _sprite.color = new Color(num1, num1, num1);
        }

        public override void Draw()
        {
            base.Draw();
            if (Level.current is Editor)
            {
                if (locked && !_lockedSprite)
                {
                    _sprite = new SpriteMap("lockDoor", 32, 32);
                    graphic = _sprite;
                    _lockedSprite = true;
                }
                else if (!locked && _lockedSprite)
                {
                    _sprite = new SpriteMap("door", 32, 32);
                    graphic = _sprite;
                    _lockedSprite = false;
                }
            }
            if (!_lockDoor || locked)
                return;
            _key.frame = _sprite.frame;
            if (_key.frame > 12)
                _key.depth = depth - 1;
            else
                _key.depth = depth + 1;
            _key.flipH = graphic.flipH;
            Graphics.Draw(_key, x + _open * 12f, y - 8f);
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
            locked = node.GetProperty<bool>("locked");
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
                locked = Convert.ToBoolean(dxmlNode.Value);
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
