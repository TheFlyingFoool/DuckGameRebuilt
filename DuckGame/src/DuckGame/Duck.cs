// Decompiled with JetBrains decompiler
// Type: DuckGame.Duck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Duck : PhysicsObject, IReflect, ITakeInput, IAmADuck, IDrawToDifferentLayers
    {
        public StateBinding _profileIndexBinding = new StateBinding(GhostPriority.Normal, nameof(netProfileIndex), 4);
        public StateBinding _disarmIndexBinding = new StateBinding(GhostPriority.Normal, nameof(disarmIndex), 4);
        public StateBinding _animationIndexBinding = new StateBinding(GhostPriority.Normal, nameof(netAnimationIndex), 4);
        public StateBinding _holdObjectBinding = new StateBinding(GhostPriority.High, nameof(holdObject));
        public StateBinding _ragdollBinding = new StateBinding(GhostPriority.High, nameof(ragdoll));
        public StateBinding _cookedBinding = new StateBinding(GhostPriority.High, nameof(_cooked));
        public StateBinding _quackBinding = new StateBinding(GhostPriority.High, nameof(quack), 5);
        public StateBinding _quackPitchBinding = new StateBinding(GhostPriority.High, nameof(quackPitch));
        public StateBinding _toungeBinding = new CompressedVec2Binding(GhostPriority.Normal, nameof(tounge), 1);
        public StateBinding _cameraPositionOverrideBinding = new CompressedVec2Binding(GhostPriority.Normal, "cameraPositionOverride");
        public StateBinding _trappedInstanceBinding = new StateBinding(nameof(_trappedInstance));
        public StateBinding _ragdollInstanceBinding = new StateBinding(nameof(_ragdollInstance));
        public StateBinding _cookedInstanceBinding = new StateBinding(nameof(_cookedInstance));
        public StateBinding _duckStateBinding = new DuckFlagBinding(GhostPriority.High);
        public StateBinding _netQuackBinding = new NetSoundBinding(GhostPriority.High, nameof(_netQuack));
        public StateBinding _netSwearBinding = new NetSoundBinding(nameof(_netSwear));
        public StateBinding _netScreamBinding = new NetSoundBinding(nameof(_netScream));
        public StateBinding _netJumpBinding = new NetSoundBinding(GhostPriority.Normal, nameof(_netJump));
        public StateBinding _netDisarmBinding = new NetSoundBinding(nameof(_netDisarm));
        public StateBinding _netTinyMotionBinding = new NetSoundBinding(nameof(_netTinyMotion));
        public StateBinding _conversionResistanceBinding = new StateBinding(nameof(conversionResistance), 8);
        public bool forceDead;
        public bool afk;
        private byte _quackPitch;
        public NetSoundEffect _netQuack = new NetSoundEffect(new string[1]
        {
            nameof (quack)
        });
        public NetSoundEffect _netJump = new NetSoundEffect(new string[1]
        {
            "jump"
        })
        {
            volume = 0.5f
        };
        public NetSoundEffect _netDisarm = new NetSoundEffect(new string[1]
        {
            "disarm"
        })
        {
            volume = 0.3f
        };
        public NetSoundEffect _netTinyMotion = new NetSoundEffect(new string[1]
        {
            "tinyMotion"
        });
        public NetSoundEffect _netSwear = new NetSoundEffect(new List<string>()
        {
            "cutOffQuack",
            "cutOffQuack2"
        }, new List<string>() { "quackBleep" })
        {
            pitchVariationLow = -0.05f,
            pitchVariationHigh = 0.05f
        };
        public NetSoundEffect _netScream = new NetSoundEffect(new string[3]
        {
            "quackYell01",
            "quackYell02",
            "quackYell03"
        });
        public PlusOne currentPlusOne;
        public byte disarmIndex = 9;
        public Vec2 _tounge;
        private int _netProfileIndex = -1;
        //private bool _netProfileInit;
        private bool _assignedIndex;
        private Sprite _shield;
        public const int BackpackDepth = -15;
        public const int BackWingDepth = -10;
        public const int ClothingDepth = 4;
        public const int HoldingDepth = 9;
        public const int WingDepth = 11;
        public int ctfTeamIndex;
        public bool forceMindControl;
        public SpriteMap _sprite;
        public SpriteMap _spriteArms;
        public SpriteMap _spriteQuack;
        public SpriteMap _spriteControlled;
        public InputProfile _mindControl;
        public Duck controlledBy;
        private bool _derpMindControl = true;
        protected DuckSkeleton _skeleton = new DuckSkeleton();
        public float slideBuildup;
        public int listenTime;
        public bool listening;
        public bool immobilized;
        public bool beammode;
        public bool jumping;
        public bool doThrow;
        public bool swinging;
        public bool holdObstructed;
        private bool _checkingTeam;
        private bool _checkingPersona;
        private bool _isGrabbedByMagnet;
        public bool isRockThrowDuck;
        public bool _hovering;
        private bool _closingEyes;
        public bool _canFire = true;
        public float crippleTimer;
        public float jumpCharge;
        protected float armOffY;
        protected float armOffX;
        protected float centerOffset;
        protected float holdOffX;
        protected float holdOffY;
        public float holdAngleOff;
        protected float aRadius = 4f;
        protected float dRadius = 4f;
        protected bool reverseThrow;
        protected float kick;
        public float unfocus = 1f;
        protected bool _isGhost;
        private bool _eyesClosed;
        //private SinWave _arrowBob = (SinWave)0.2f;
        private bool _remoteControl;
        private float _ghostTimer = 1f;
        private int _lives;
        private float _runMax = 3.1f;
        private bool _moveLock;
        private InputProfile _inputProfile = InputProfile.Get("SinglePlayer");
        private InputProfile _virtualInput;
        public InputProfile VirtualInput
        {
            get => _virtualInput;
            set => _virtualInput = value;
        }
        protected Profile _profile;
        protected Sprite _swirl;
        private float _swirlSpin;
        private bool _resetAction;
        protected SpriteMap _bionicArm;
        protected FeatherVolume _featherVolume;
        public List<Equipment> _equipment = new List<Equipment>();
        public int quack;
        public bool quackStart;
        private bool didHat;
        public TrappedDuck _trappedInstance;
        private Ragdoll _ins;
        public CookedDuck _cookedInstance;
        private SpriteMap _lagTurtle;
        private float _duckWidth = 1f;
        private float _duckHeight = 1f;
        //private string _collisionMode = "normal";
        private Profile _disarmedBy;
        private DateTime _disarmedAt = DateTime.Now;
        private DateTime _timeSinceFuneralPerformed = DateTime.MinValue;
        private DateTime _timeSinceDuckLayedToRest = DateTime.MinValue;
        private Thing _holdingAtDisarm;
        //private int _coolnessThisFrame;
        public Vec2 respawnPos;
        public float respawnTime;
        public Holdable _lastHoldItem;
        public byte _timeSinceThrow;
        public bool killingNet;
        public bool isKillMessage;
        private bool _killed;
        public Profile killedByProfile;
        public int framesSinceKilled;
        public Func<Duck, bool> KillOverride;
        public byte lastAppliedLifeChange;
        public bool invincible;
        public float killMultiplier;
        //private List<Thing> added = new List<Thing>();
        public DuckAI ai;
        private bool _crouchLock;
        public byte _flapFrame;
        public int _jumpValid;
        public int _groundValid;
        private TrappedDuck _trappedProp;
        public Profile trappedBy;
        private int breath = -1;
        protected bool _throwFondle = true;
        protected int tryGrabFrames;
        //private bool _heldLeft;
        //private bool _heldRight;
        private bool _updatedAnimation;
        private float disarmIndexCooldown;
        public int _disarmWait;
        public int _disarmDisable;
        public int _timeSinceChainKill;
        public int _iceWedging;
        public int framesSinceJump;
        //private ATShotgun shotty = new ATShotgun();
        public bool forceFire;
        public float jumpSpeed;
        public float removeBody = 1f;
        public int clientFrame;
        private bool _canWallJump;
        public bool localDuck = true;
        private Ragdoll _currentRagdoll;
        private int _wallJump;
        private bool _rightJump;
        private bool _leftJump;
        private int _atWallFrames;
        private bool leftWall;
        private bool rightWall;
        private bool atWall;
        private int _walkTime;
        private int _walkCount;
        private bool _nextTrigger;
        public bool grappleMul;
        public float grappleMultiplier = 1f;
        public bool onWall;
        private float maxrun;
        public bool pickedHat;
        public Vine _vine;
        public bool _double;
        public float _shieldCharge;
        public int skipPlatFrames;
        private bool didFireSlide;
        public static float JumpSpeed = -4.9f;
        private int _leftPressedFrame;
        private int _rightPressedFrame;
        public bool tvJumped;
        //private int bulletIndex;
        public int pipeOut;
        public int pipeBoost;
        public int slamWait;
        public float accelerationMultiplier = 1f;
        public bool strafing;
        private bool crouchCancel;
        private bool vineRelease;
        private Vec2 prevCamPosition;
        private Thing _followPart;
        private int wait;
        private float lastCalc;
        private bool firstCalc = true;
        private CoolnessPlus plus;
        private Ragdoll _prevRagdoll;
        private float _bubbleWait;
        private static int _framesSinceInput = 0;
        private Vec2 _lastGoodPosition;
        private byte _prevDisarmIndex = 6;
        public bool manualQuackPitch;
        //private int killedWait;
        private bool unfocused;
        private int _framesUnderwater;
        public int framesSinceRagdoll;
        public float verticalOffset;
        public int swordInvincibility;
        public bool fancyShoes;
        private bool _renderingDuck;
        public float _burnTime = 1f;
        public CookedDuck _cooked;
        private Sound _sizzle;
        private float _handHeat;
        private byte handSmokeWait;
        private Duck _converted;
        public int conversionResistance = 100;
        public bool isConversionMessage;
        private bool _gripped;
        public static bool renderingIcon = false;
        private Camera _iconCamera;
        private Rectangle _iconRect = new Rectangle(0f, 0f, 96f, 96f);
        public Vec2 tongueCheck = Vec2.Zero;
        private Vec2 _stickLerp;
        private Vec2 _stickSlowLerp;
        public bool localSpawnVisible = true;
        public bool enteringWalldoor;
        public bool exitingWalldoor;
        public bool autoExitDoor;
        public int autoExitDoorFrames;
        public DuckAI wallDoorAI;
        public WallDoor transportDoor;
        public float enterDoorSpeed;
        public float tilt;
        //private static Material kGhostMaterial;
        public int waitGhost;
        private ConnectionIndicators _indicators;
        private bool _protectedFromFire;
        public override bool destroyed => _destroyed || forceDead;

        public byte quackPitch
        {
            get => _quackPitch;
            set => _quackPitch = value;
        }

        public byte spriteFrame
        {
            get => _sprite == null ? (byte)0 : (byte)_sprite._frame;
            set
            {
                if (_sprite == null)
                    return;
                _sprite._frame = value;
            }
        }

        public byte spriteImageIndex
        {
            get => _sprite == null ? (byte)0 : (byte)_sprite._imageIndex;
            set
            {
                if (_sprite == null)
                    return;
                _sprite._imageIndex = value;
            }
        }

        public float spriteSpeed
        {
            get => _sprite == null ? 0f : _sprite._speed;
            set
            {
                if (_sprite == null)
                    return;
                _sprite._speed = value;
            }
        }

        public float spriteInc
        {
            get => _sprite == null ? 0f : _sprite._frameInc;
            set
            {
                if (_sprite == null)
                    return;
                _sprite._frameInc = value;
            }
        }

        public byte netAnimationIndex
        {
            get => _sprite == null ? (byte)0 : (byte)_sprite.animationIndex;
            set
            {
                if (_sprite == null || _sprite.animationIndex == value)
                    return;
                _sprite.animationIndex = value;
            }
        }

        public Vec2 tounge
        {
            get => (!Network.isActive || isServerForObject) && inputProfile != null ? inputProfile.rightStick : _tounge;
            set => _tounge = value;
        }

        public byte netProfileIndex
        {
            get => _netProfileIndex < 0 || _netProfileIndex > DG.MaxPlayers - 1 ? (byte)0 : (byte)_netProfileIndex;
            set
            {
                if (_netProfileIndex == value)
                    return;
                AssignNetProfileIndex(value);
            }
        }

        private void AssignNetProfileIndex(byte pIndex)
        {
            DevConsole.Log(DCSection.General, "Assigning net profile index (" + pIndex.ToString() + "\\" + Profiles.alllist.Count.ToString() + ")");
            _netProfileIndex = pIndex;
            Profile profile = Profiles.alllist[_netProfileIndex];
            if (Network.isClient && Network.inLobby)
                (Level.current as TeamSelect2).OpenDoor(_netProfileIndex, this);
            this.profile = profile;
            if (profile.team == null)
                profile.team = Teams.all[_netProfileIndex];
            InitProfile();
            //this._netProfileInit = true;
            _assignedIndex = true;
        }

        public Hat hat => GetEquipment(typeof(Hat)) as Hat;

        public InputProfile mindControl
        {
            get => _mindControl;
            set
            {
                if (value == null && _mindControl != null && profile != null && (profile.localPlayer || forceMindControl))
                {
                    if (holdObject != null)
                        Fondle(holdObject, DuckNetwork.localConnection);
                    foreach (Thing t in _equipment)
                        Fondle(t, DuckNetwork.localConnection);
                    Fondle(_ragdollInstance, DuckNetwork.localConnection);
                    Fondle(_cookedInstance, DuckNetwork.localConnection);
                    Fondle(_trappedInstance, DuckNetwork.localConnection);
                }
                _mindControl = value;
            }
        }

        public bool derpMindControl
        {
            get => _derpMindControl;
            set => _derpMindControl = value;
        }

        public DuckSkeleton skeleton
        {
            get
            {
                UpdateSkeleton();
                return _skeleton;
            }
        }

        public bool dead
        {
            get => destroyed;
            set => _destroyed = value;
        }

        public bool inNet => _trapped != null;

        public Team team
        {
            get
            {
                if (profile == null)
                    return null;
                if (_checkingTeam || _converted == null)
                    return profile.team;
                _checkingTeam = true;
                Team team = _converted.team;
                _checkingTeam = false;
                return team;
            }
        }

        public DuckPersona persona
        {
            get
            {
                if (profile == null)
                    return null;
                if (_checkingPersona || _converted == null)
                    return profile.persona;
                _checkingPersona = true;
                DuckPersona persona = _converted.persona;
                _checkingPersona = false;
                return persona;
            }
        }

        public bool isGrabbedByMagnet
        {
            get => _isGrabbedByMagnet;
            set
            {
                _isGrabbedByMagnet = value;
                if (value || !profile.localPlayer)
                    return;
                angle = 0f;
                immobilized = false;
                gripped = false;
                enablePhysics = true;
                visible = true;
                SetCollisionMode("normal");
                if (holdObject != null)
                    Fondle(holdObject, DuckNetwork.localConnection);
                foreach (Thing t in _equipment)
                    Fondle(t, DuckNetwork.localConnection);
                Fondle(_ragdollInstance, DuckNetwork.localConnection);
                Fondle(_cookedInstance, DuckNetwork.localConnection);
                Fondle(_trappedInstance, DuckNetwork.localConnection);
            }
        }

        public override bool CanBeControlled() => mindControl != null || isGrabbedByMagnet || listening || dead || wasSuperFondled > 0;

        public void CancelFlapping() => _hovering = false;

        public bool IsNetworkDuck() => !isRockThrowDuck && Network.isClient;

        public bool closingEyes
        {
            get => _closingEyes;
            set => _closingEyes = value;
        }

        public bool canFire
        {
            get => _canFire;
            set => _canFire = value;
        }

        public bool CanMove() => (holdObject == null || !holdObject.immobilizeOwner) && !immobilized && crippleTimer <= 0 && !inNet && !swinging && !dead && !listening && Level.current.simulatePhysics && !_closingEyes && ragdoll == null;

        public static Duck Get(int index)
        {
            foreach (Thing thing in Level.current.things[typeof(Duck)])
            {
                Duck duck = thing as Duck;
                if (Persona.Number(duck.profile.persona) == index)
                    return duck;
            }
            return null;
        }

        public bool isGhost
        {
            get => _isGhost;
            set => _isGhost = value;
        }

        public bool eyesClosed
        {
            get => _eyesClosed;
            set => _eyesClosed = value;
        }

        public bool remoteControl
        {
            get => _remoteControl;
            set => _remoteControl = value;
        }

        public override bool action => !_resetAction && (CanMove() || ragdoll != null && !dead && fancyShoes || _remoteControl || inPipe) && inputProfile.Down(Triggers.Shoot) && _canFire;

        public Vec2 armPosition => position + armOffset;

        public Vec2 armOffset
        {
            get
            {
                Vec2 vec2 = Vec2.Zero;
                if (gun != null)
                    vec2 = -gun.barrelVector * kick;
                return new Vec2(armOffX * xscale + vec2.x, armOffY * yscale + vec2.y);
            }
        }

        public Vec2 armPositionNoKick => position + armOffsetNoKick;

        public Vec2 armOffsetNoKick => new Vec2(armOffX * xscale, armOffY * yscale);

        public Vec2 HoldOffset(Vec2 pos)
        {
            Vec2 vec2 = pos + new Vec2(holdOffX, holdOffY);
            vec2 = vec2.Rotate(holdAngle, new Vec2(0f, 0f));
            return position + (vec2 + armOffset);
        }

        public float holdAngle => holdObject != null ? holdObject.handAngle + holdAngleOff : holdAngleOff;

        public static Duck GetAssociatedDuck(Thing pThing)
        {
            if (pThing == null)
                return null;
            if (pThing is Duck)
                return pThing as Duck;
            if (pThing.owner is Duck)
                return pThing.owner as Duck;
            switch (pThing)
            {
                case RagdollPart _ when (pThing as RagdollPart).doll != null:
                    return (pThing as RagdollPart).doll.captureDuck;
                case Ragdoll _:
                    return (pThing as Ragdoll).captureDuck;
                case TrappedDuck _:
                    return (pThing as TrappedDuck).captureDuck;
                default:
                    return null;
            }
        }

        public Duck GetHeldByDuck()
        {
            if (ragdoll != null)
            {
                if (ragdoll.part1 != null && ragdoll.part1.owner is Duck)
                    return ragdoll.part1.owner as Duck;
                if (ragdoll.part2 != null && ragdoll.part2.owner is Duck)
                    return ragdoll.part1.owner as Duck;
                if (ragdoll.part3 != null && ragdoll.part3.owner is Duck)
                    return ragdoll.part1.owner as Duck;
            }
            else if (_trapped != null && _trapped.owner is Duck)
                return _trapped.owner as Duck;
            return null;
        }

        public bool IsOwnedBy(Thing pThing) => pThing != null && (owner == pThing || _trapped != null && _trapped.owner == pThing || ragdoll != null && (ragdoll.part1 != null && ragdoll.part1.owner == pThing || ragdoll.part2 != null && ragdoll.part2.owner == pThing || ragdoll.part3 != null && ragdoll.part3.owner == pThing));

        public bool HoldingTaped(Holdable pObject) => holdObject is TapedGun && ((holdObject as TapedGun).gun1 == pObject || (holdObject as TapedGun).gun2 == pObject);

        public bool Held(Holdable pObject, bool ignorePowerHolster = false) => holdObject == pObject || holdObject is TapedGun && ((holdObject as TapedGun).gun1 == pObject || (holdObject as TapedGun).gun2 == pObject) || !ignorePowerHolster && GetEquipment(typeof(Holster)) is Holster equipment && equipment is PowerHolster && pObject == equipment.containedObject;

        public override Holdable holdObject
        {
            get => base.holdObject;
            set
            {
                if (value != holdObject && holdObject != null)
                {
                    if (holdObject.isServerForObject && holdObject.owner == this)
                        ThrowItem();
                    _lastHoldItem = holdObject;
                    _timeSinceThrow = 0;
                }
                base.holdObject = value;
            }
        }

        public int lives
        {
            get => _lives;
            set => _lives = value;
        }

        public float holdingWeight => holdObject == null ? 0f : holdObject.weight;

        public override float weight
        {
            get => (_weight + holdingWeight * 0.4f + (sliding || crouch ? 16f : 0f));
            set => _weight = value;
        }

        public float runMax
        {
            get => _runMax;
            set => _runMax = value;
        }

        public bool moveLock
        {
            get => _moveLock;
            set => _moveLock = value;
        }

        public InputProfile inputProfile
        {
            get
            {
                if (wallDoorAI != null)
                    return wallDoorAI;
                if (_mindControl != null)
                    return _mindControl;
                if (_virtualInput != null)
                    return _virtualInput;
                return _profile != null ? _profile.inputProfile : _inputProfile;
            }
        }

        public Profile profile
        {
            get => _profile;
            set
            {
                _profile = value;
                if (!Network.isActive || _profile == null)
                    return;
                if (_profile.localPlayer)
                {
                    Fondle(this, DuckNetwork.localConnection);
                }
                else
                {
                    if (_profile.connection == null)
                        return;
                    connection = _profile.connection;
                }
            }
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                if (Network.isServer && connection != null && connection.status == ConnectionStatus.Disconnected && Network.inGameLevel && !Network.isFakeActive)
                    Kill(new DTDisconnect(this));
                if (_profile != null)
                {
                    if (_profile.localPlayer && !CanBeControlled())
                    {
                        if (connection == DuckNetwork.localConnection)
                            return;
                        base.connection = DuckNetwork.localConnection;
                        authority += 5;
                    }
                    else
                        base.connection = value;
                }
                else
                    base.connection = value;
            }
        }

        public bool resetAction
        {
            get => _resetAction;
            set => _resetAction = value;
        }

        public virtual void InitProfile()
        {
            _profile.duck = this;
            _sprite = profile.persona.sprite.CloneMap();
            _spriteArms = profile.persona.armSprite.CloneMap();
            _spriteQuack = profile.persona.quackSprite.CloneMap();
            _spriteControlled = profile.persona.controlledSprite.CloneMap();
            _swirl = new Sprite("swirl");
            _swirl.CenterOrigin();
            _swirl.scale = new Vec2(0.75f, 0.75f);
            _bionicArm = new SpriteMap("bionicArm", 32, 32);
            _bionicArm.CenterOrigin();
            if (!didHat && (Network.isServer || RockScoreboard.initializingDucks))
            {
                if (profile.team != null && profile.team.hasHat)
                {
                    Hat e = new TeamHat(0f, 0f, team, profile);
                    if (RockScoreboard.initializingDucks)
                        e.IgnoreNetworkSync();
                    Level.Add(e);
                    Equip(e, false, true);
                }
                didHat = true;
            }
            graphic = _sprite;
        }

        public Ragdoll _ragdollInstance
        {
            get => _ins;
            set => _ins = value;
        }

        public Duck(float xval, float yval, Profile pro)
          : base(xval, yval)
        {
            shouldbegraphicculled = false;
            _featherVolume = new FeatherVolume(this)
            {
                anchor = (Anchor)this
            };
            duck = true;
            profile = pro;
            if (_profile == null)
                _profile = Profiles.EnvironmentProfile;
            if (profile != null)
                InitProfile();
            centerx = 16f;
            centery = 16f;
            friction = 0.25f;
            vMax = 8f;
            hMax = 12f;
            _lagTurtle = new SpriteMap("lagturtle", 16, 16);
            _lagTurtle.CenterOrigin();
            physicsMaterial = PhysicsMaterial.Duck;
            collideSounds.Add("land", ImpactedFrom.Bottom);
            _impactThreshold = 1.3f;
            _impactVolume = 0.4f;
            SetCollisionMode("normal");
            _shield = new Sprite("sheeld");
            _shield.CenterOrigin();
            flammable = 1f;
            thickness = 0.5f;
        }

        public override void Terminate()
        {
            if (Level.current.camera is FollowCam)
                (Level.current.camera as FollowCam).Remove(this);
            Level.Remove(_featherVolume);
            if (Network.isActive)
            {
                Level.Remove(_ragdollInstance);
                Level.Remove(_trappedInstance);
                Level.Remove(_cookedInstance);
            }
            foreach (Thing thing in _equipment.ToList())
                Level.Remove(thing);
        }

        public float duckWidth
        {
            get => _duckWidth;
            set
            {
                _duckWidth = value;
                xscale = _duckWidth;
            }
        }

        public float duckHeight
        {
            get => _duckHeight;
            set
            {
                _duckHeight = value;
                yscale = _duckHeight;
            }
        }

        public float duckSize
        {
            get => _duckHeight;
            set => duckWidth = duckHeight = value;
        }

        public void SetCollisionMode(string mode)
        {
            //this._collisionMode = mode;
            if (offDir > 0)
                _featherVolume.anchor.offset = new Vec2(0f, 0f);
            else
                _featherVolume.anchor.offset = new Vec2(1f, 0f);
            if (mode == "normal")
            {
                collisionSize = new Vec2(8f * duckWidth, 22f * duckHeight);
                collisionOffset = new Vec2(-4f * duckWidth, -7f * duckHeight);
                _featherVolume.collisionSize = new Vec2(12f * duckWidth, 26f * duckHeight);
                _featherVolume.collisionOffset = new Vec2(-6f * duckWidth, -9f * duckHeight);
            }
            else if (mode == "slide")
            {
                collisionSize = new Vec2(8f * duckWidth, 11f * duckHeight);
                collisionOffset = new Vec2(-4f * duckWidth, 4f * duckHeight);
                if (offDir > 0)
                {
                    _featherVolume.collisionSize = new Vec2(25f * duckWidth, 13f * duckHeight);
                    _featherVolume.collisionOffset = new Vec2(-13f * duckWidth, 3f * duckHeight);
                }
                else
                {
                    _featherVolume.collisionSize = new Vec2(25f * duckWidth, 13f * duckHeight);
                    _featherVolume.collisionOffset = new Vec2(-12f * duckWidth, 3f * duckHeight);
                }
            }
            else if (mode == "crouch")
            {
                collisionSize = new Vec2(8f * duckWidth, 16f * duckHeight);
                collisionOffset = new Vec2(-4f * duckWidth, -1f * duckHeight);
                _featherVolume.collisionSize = new Vec2(12f * duckWidth, 20f * duckHeight);
                _featherVolume.collisionOffset = new Vec2(-6f * duckWidth, -3f * duckHeight);
            }
            else if (mode == "netted")
            {
                collisionSize = new Vec2(16f * duckWidth, 17f * duckHeight);
                collisionOffset = new Vec2(-8f * duckWidth, -9f * duckHeight);
                _featherVolume.collisionSize = new Vec2(18f * duckWidth, 19f * duckHeight);
                _featherVolume.collisionOffset = new Vec2(-9f * duckWidth, -10f * duckHeight);
            }
            if (ragdoll == null)
                return;
            _featherVolume.collisionSize = new Vec2(12f * duckWidth, 12f * duckHeight);
            _featherVolume.collisionOffset = new Vec2(-6f * duckWidth, -6f * duckHeight);
        }

        public void KnockOffEquipment(Equipment e, bool ting = true, Bullet b = null)
        {
            if (!_equipment.Contains(e))
                return;
            if (isServerForObject)
                RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None));
            e.UnEquip();
            if (ting && !Network.isActive)
                SFX.Play("ting2");
            _equipment.Remove(e);
            e.Destroy(new DTImpact(null));
            e.solid = false;
            if (b != null)
            {
                e.hSpeed = b.travelDirNormalized.x;
                e.vSpeed = -2f;
                if (isServerForObject)
                {
                    hSpeed += b.travelDirNormalized.x * (b.ammo.impactPower + 1f);
                    vSpeed += b.travelDirNormalized.y * (b.ammo.impactPower + 1f);
                    --vSpeed;
                }
            }
            else
            {
                e.hSpeed = -offDir * 2f;
                e.vSpeed = -2f;
            }
            ReturnItemToWorld(e);
        }

        public override void ReturnItemToWorld(Thing t)
        {
            Vec2 position = this.position;
            if (sliding)
                position.y += 10f;
            else if (crouch)
                position.y += 8f;
            Block block1 = Level.CheckLine<Block>(position, position + new Vec2(16f, 0f));
            if (block1 != null && block1.solid && t.right > block1.left)
                t.right = block1.left;
            Block block2 = Level.CheckLine<Block>(position, position - new Vec2(16f, 0f));
            if (block2 != null && block2.solid && t.left < block2.right)
                t.left = block2.right;
            Block block3 = Level.CheckLine<Block>(position, position + new Vec2(0f, -16f));
            if (block3 != null && block3.solid && t.top < block3.bottom)
                t.top = block3.bottom;
            Block block4 = Level.CheckLine<Block>(position, position + new Vec2(0f, 16f));
            if (block4 == null || !block4.solid || t.bottom <= block4.top)
                return;
            t.bottom = block4.top;
        }

        public void Unequip(Equipment e, bool forceNetwork = false)
        {
            if (!(isServerForObject | forceNetwork) || e == null || !_equipment.Contains(e))
                return;
            Fondle(e);
            e.UnEquip();
            _equipment.Remove(e);
            ReturnItemToWorld(e);
        }

        public bool HasJumpModEquipment()
        {
            foreach (Equipment equipment in _equipment)
            {
                if (equipment.jumpMod)
                    return true;
            }
            return false;
        }

        public Equipment GetEquipment(Type t)
        {
            foreach (Equipment equipment in _equipment)
            {
                if (equipment.GetAllTypes().Contains(t))
                    return equipment;
            }
            return null;
        }

        public void Equip(Equipment e, bool makeSound = true, bool forceNetwork = false)
        {
            if (!(isServerForObject | forceNetwork))
                return;
            List<Type> allTypesFiltered = e.GetAllTypesFiltered(typeof(Equipment));
            if (allTypesFiltered.Contains(typeof(ITeleport)))
                allTypesFiltered.Remove(typeof(ITeleport));
            foreach (Type t in allTypesFiltered)
            {
                if (!t.IsInterface)
                {
                    Equipment equipment = GetEquipment(t);
                    if (equipment == null && e.GetType() == typeof(Jetpack))
                        equipment = GetEquipment(typeof(Grapple));
                    else if (equipment == null && e.GetType() == typeof(Grapple))
                        equipment = GetEquipment(typeof(Jetpack));
                    if (equipment != null)
                    {
                        _equipment.Remove(equipment);
                        Fondle(equipment);
                        equipment.vSpeed = -2f;
                        equipment.hSpeed = offDir * 3f;
                        equipment.UnEquip();
                        ReturnItemToWorld(equipment);
                    }
                }
            }
            if (e is TeamHat)
            {
                TeamHat teamHat = e as TeamHat;
                if (profile != null && teamHat.team != profile.team && !teamHat.hasBeenStolen)
                {
                    ++Global.data.hatsStolen;
                    teamHat.hasBeenStolen = true;
                }
            }
            Fondle(e);
            _equipment.Add(e);
            e.Equip(this);
            if (!makeSound)
                e._prevEquipped = true;
            else
                e.equipIndex += 1;
        }

        public List<Equipment> GetArmor()
        {
            List<Equipment> armor = new List<Equipment>();
            foreach (Equipment equipment in _equipment)
            {
                if (equipment.isArmor)
                    armor.Add(equipment);
            }
            return armor;
        }

        public bool ExtendsTo(Thing t)
        {
            if (ragdoll == null)
                return false;
            return t == ragdoll.part1 || t == ragdoll.part2 || t == ragdoll.part3;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_trapped != null || _trappedInstance != null && _trappedInstance.visible || ragdoll != null || _ragdollInstance != null && _ragdollInstance.visible)
                return false;
            if (bullet.isLocal && !HitArmor(bullet, hitPos))
            {
                Kill(new DTShot(bullet));
                SFX.Play("thwip", pitch: Rando.Float(-0.1f, 0.1f));
            }
            return base.Hit(bullet, hitPos);
        }

        public bool HitArmor(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
            {
                foreach (Equipment t in _equipment)
                {
                    if (!bullet._currentlyImpacting.Contains(t) && Collision.Point(hitPos, t))
                    {
                        bullet._currentlyImpacting.Add(t);
                        if (t.DoHit(bullet, hitPos))
                            return true;
                    }
                }
            }
            return false;
        }

        public override bool Destroy(DestroyType type = null) => Kill(type);

        public void AddCoolness(int amount)
        {
            if (Highlights.highlightRatingMultiplier == 0f)
                return;
            profile.stats.coolness += amount;
            //this._coolnessThisFrame += amount;
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogCoolness(Math.Abs(amount));
        }

        public bool WillAcceptLifeChange(byte pLifeChange)
        {
            if (lastAppliedLifeChange >= pLifeChange && Math.Abs(lastAppliedLifeChange - pLifeChange) <= 20)
                return false;
            lastAppliedLifeChange = pLifeChange;
            return true;
        }

        public virtual bool Kill(DestroyType type = null)
        {
            if (_killed || (!isKillMessage && invincible && !(type is DTFall) && !(type is DTPop)))
            {
                return true;
            }
            if (KillOverride != null && KillOverride(this))
            {
                return false;
            }
            forceDead = true;
            _killed = true;
            RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Short, RumbleType.Gameplay));
            int xpLogged = 10;
            if (type is DTFall)
            {
                Vec2 pos = GetEdgePos();
                Vec2 dir = (pos - GetPos()).normalized;
                for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 8; i++)
                {
                    Feather feather = Feather.New(pos.x - dir.x * 16f, pos.y - dir.y * 16f, persona);
                    feather.hSpeed += dir.x * 1f;
                    feather.vSpeed += dir.y * 1f;
                    Level.Add(feather);
                }
            }
            if (!GameMode.firstDead)
            {
                Party.AddDrink(profile, 1);
                if (Rando.Float(1f) > 0.8f)
                {
                    Party.AddRandomPerk(profile);
                }
                GameMode.firstDead = true;
            }
            if (Rando.Float(1f) > 0.97f)
            {
                Party.AddRandomPerk(profile);
                Party.AddDrink(profile, 1);
            }
            if (Recorder.currentRecording != null)
            {
                Recorder.currentRecording.LogDeath();
            }
            _destroyed = true;
            if (_isGhost)
            {
                return false;
            }
            swinging = false;
            Holster h = GetEquipment(typeof(Holster)) as Holster;
            foreach (Equipment e in _equipment)
            {
                if (e != null)
                {
                    e.sleeping = false;
                    e.owner = null;
                    if (!isKillMessage)
                    {
                        ExtraFondle(e, DuckNetwork.localConnection);
                    }
                    if (e is TeamHat th && th.team != null && th.team.metadata != null && th.team.metadata.FadeOnDeath.value)
                    {
                        th.UnEquip();
                        th._destroyed = true;
                    }
                    e.hSpeed = hSpeed - (1f + NetRand.Float(2f));
                    e.vSpeed = vSpeed - NetRand.Float(1.5f);
                    ReturnItemToWorld(e);
                    e.UnEquip();
                }
            }
            _equipment.Clear();
            if (TeamSelect2.QUACK3 && h != null)
            {
                Equip(h, false, false);
            }
            Profile killedBy = type.responsibleProfile;
            bool wasTrapped = false;
            if (_trapped != null)
            {
                if (type is DTFall || type is DTImpale)
                {
                    killedBy = trappedBy;
                    Duck d = _trapped.prevOwner as Duck;
                    if (d != null)
                    {
                        d.AddCoolness(1);
                    }
                }
                if ((type is DTFall || type is DTImpale) && trappedBy != null && trappedBy.localPlayer)
                {
                    Global.data.nettedDuckTossKills += 1;
                }
                if (!killingNet)
                {
                    killingNet = true;
                    _trapped.Destroy(type);
                }
                wasTrapped = true;
            }
            if (type is DTIncinerate)
            {
                xpLogged -= 3;
            }
            if (killedBy != null && killedBy.localPlayer)
            {
                killedByProfile = killedBy;
            }
            OnKill(type);
            Holdable prevHold = holdObject;
            if (!isKillMessage)
            {
                ThrowItem(false);
                if (prevHold != null)
                {
                    prevHold.hSpeed *= 0.3f;
                    if (type is DTImpale)
                    {
                        prevHold.vSpeed = (prevHold.hSpeed = 0f);
                    }
                    if (Network.isActive)
                    {
                        AuthorityFondle(prevHold, DuckNetwork.localConnection, 5);
                    }
                }
            }
            else if (profile != null && profile.connection == DuckNetwork.localConnection)
            {
                ThrowItem(false);
            }
            depth = 0.3f;
            if (killedBy != null)
            {
                if (killedBy == profile)
                {
                    ProfileStats stats = killedBy.stats;
                    int num = stats.suicides;
                    stats.suicides = num + 1;
                }
                else
                {
                    ProfileStats stats2 = killedBy.stats;
                    int num = stats2.kills;
                    stats2.kills = num + 1;
                }
            }
            if (Level.current is TeamSelect2 && isKillMessage)
            {
                ProfileBox2 box = Level.CheckPoint<ProfileBox2>(cameraPosition);
                if (box != null && box.duck == this)
                {
                    Profile profile = this.profile;
                    int num = profile.punished;
                    profile.punished = num + 1;
                }
            }
            if (!Network.isActive && !(Level.current is ChallengeLevel))
            {
                int myCoolness = 0;
                int yourCoolness = 0;
                Type weapon = type.killThingType;
                Thing weaponThing = type.thing;
                Bullet bullet = weaponThing as Bullet;
                if (bullet != null)
                {
                    if (bullet.travelTime > 0.5f)
                    {
                        yourCoolness++;
                    }
                    if (bullet.bulletDistance > 300f)
                    {
                        yourCoolness++;
                    }
                    if (bullet.didPenetrate)
                    {
                        yourCoolness++;
                    }
                    weaponThing = bullet.firedFrom;
                }
                Event.Log(new KillEvent(killedBy, profile, weapon));
                profile.stats.LogKill(killedBy);
                if (weapon != null)
                {
                    if (weapon == typeof(Mine))
                    {
                        myCoolness--;
                        xpLogged += 5;
                    }
                    if (weapon == typeof(HugeLaser))
                    {
                        myCoolness--;
                        if (killedBy != null)
                        {
                            yourCoolness++;
                        }
                    }
                    if (weapon == typeof(SuicidePistol))
                    {
                        myCoolness--;
                        if (killedBy != null && !killedBy.duck.dead)
                        {
                            yourCoolness++;
                        }
                    }
                }
                if (killedBy != null && killedBy.duck != null)
                {
                    if (killedBy == profile)
                    {
                        yourCoolness -= 2;
                        if (weapon == typeof(Grenade))
                        {
                            yourCoolness--;
                        }
                        if (prevHold == weaponThing)
                        {
                            yourCoolness--;
                        }
                        if (weapon == typeof(QuadLaser))
                        {
                            yourCoolness--;
                        }
                        Party.AddDrink(profile, 2);
                        if (Rando.Float(1f) > 0.9f)
                        {
                            Party.AddRandomPerk(profile);
                        }
                    }
                    else
                    {
                        yourCoolness++;
                        if (weapon == typeof(QuadLaser) && type is DTIncinerate)
                        {
                            QuadLaserBullet bb = (type as DTIncinerate).thing as QuadLaserBullet;
                            float mult = 1f + Math.Min(bb.timeAlive / 5f, 2f);
                            yourCoolness += (int)(1f * mult);
                        }
                        if ((DateTime.Now - killedBy.stats.lastKillTime).TotalSeconds < 2f) yourCoolness++;
                        if (bullet != null && Math.Abs(bullet.travelDirNormalized.y) > 0.3f) yourCoolness++;
                        killedBy.stats.lastKillTime = DateTime.Now;
                        if (weaponThing is Grenade)
                        {
                            yourCoolness++;
                            Grenade g = weaponThing as Grenade;
                            if (g.cookTimeOnThrow < 0.5f && g.cookThrower != null)
                            {
                                g.cookThrower.AddCoolness(1);
                            }
                        }
                        if (Math.Abs(killedBy.duck.hSpeed) + Math.Abs(killedBy.duck.vSpeed) + Math.Abs(hSpeed) + Math.Abs(vSpeed) > 20f) yourCoolness++;
                        if (_holdingAtDisarm != null && _disarmedBy == killedBy && (DateTime.Now - _disarmedAt).TotalSeconds < 3f)
                        {
                            if (killedBy.duck.holdObject == _holdingAtDisarm)
                            {
                                yourCoolness += 4;
                                myCoolness -= 2;
                            }
                            else yourCoolness++;
                        }
                        if (killedBy.duck.dead)
                        {
                            yourCoolness++;
                            killedBy.stats.killsFromTheGrave++;
                        }
                        if (type is DTShot && prevHold == null) killedBy.stats.unarmedDucksShot++;
                        else if (prevHold != null)
                        {
                            if (prevHold is PlasmaBlaster)
                            {
                                yourCoolness++;
                            }
                            else if (prevHold is Saxaphone || prevHold is Trombone || prevHold is DrumSet)
                            {
                                yourCoolness--;
                                myCoolness++;
                                Party.AddDrink(killedBy, 1);
                            }
                            else if (prevHold is Flower)
                            {
                                yourCoolness -= 2;
                                myCoolness += 2;
                                Party.AddDrink(killedBy, 1);
                            }
                        }
                        if (weapon != null)
                        {
                            if (weapon == typeof(SledgeHammer) || weapon == typeof(DuelingPistol)) yourCoolness++;
                            if (weaponThing is Sword && weaponThing.owner != null && (weaponThing as Sword).jabStance) yourCoolness++;
                        }
                        if (wasTrapped && type is DTFall) yourCoolness++;
                        if (type is DTCrush)
                        {
                            if (weaponThing is PhysicsObject)
                            {
                                double totalSeconds = (DateTime.Now - (weaponThing as PhysicsObject).lastGrounded).TotalSeconds;
                                yourCoolness += 1 + (int)Math.Floor((DateTime.Now - (weaponThing as PhysicsObject).lastGrounded).TotalSeconds * 6f);
                                if (Recorder.currentRecording != null)
                                {
                                    Recorder.currentRecording.LogAction(14);
                                }
                                Party.AddDrink(profile, 1);
                                if (Rando.Float(1f) > 0.8f)
                                {
                                    Party.AddRandomPerk(profile);
                                }
                            }
                            else
                            {
                                yourCoolness++;
                            }
                        }
                    }
                    if (killedBy.duck.team == team && killedBy != profile)
                    {
                        yourCoolness -= 2;
                        Party.AddDrink(killedBy, 1);
                    }
                    if ((DateTime.Now - _timeSinceDuckLayedToRest).TotalSeconds < 3f) yourCoolness--;
                    if ((DateTime.Now - _timeSinceFuneralPerformed).TotalSeconds < 3f) yourCoolness -= 2;
                }
                if (controlledBy != null && controlledBy.profile != null)
                {
                    controlledBy.profile.stats.coolness += Math.Abs(myCoolness);
                    if (myCoolness > 0)
                    {
                        myCoolness = 0;
                    }
                }
                yourCoolness++;
                myCoolness--;
                if (killedBy != null && killedBy.duck != null)
                {
                    yourCoolness *= (int)Math.Ceiling((1f + killedBy.duck.killMultiplier));
                    killedBy.duck.AddCoolness(yourCoolness);
                }
                AddCoolness(myCoolness);
                if (killedBy != null && killedBy.duck != null)
                {
                    killedBy.duck.killMultiplier += 1f;
                    if (TeamSelect2.KillsForPoints)
                    {
                        Profile realProfile = killedBy;
                        if (killedBy.duck.converted != null)
                        {
                            realProfile = killedBy.duck.converted.profile;
                        }
                        if (killedBy.team != profile.team)
                        {
                            SFX.Play("scoreDingShort", 0.9f, 0f, 0f, false);
                            if (killedBy.duck != null && killedBy.duck.currentPlusOne != null)
                            {
                                killedBy.duck.currentPlusOne.Pulse();
                            }
                            else
                            {
                                PlusOne plus = new PlusOne(0f, 0f, realProfile, false, true)
                                {
                                    _duck = killedBy.duck,
                                    anchor = killedBy.duck
                                };
                                plus.anchor.offset = new Vec2(0f, -16f);
                                if (killedBy.duck != null)
                                {
                                    killedBy.duck.currentPlusOne = plus;
                                }
                                Level.Add(plus);
                            }
                            realProfile.team.score++;
                            if (Teams.active.Count > 1 && Network.isActive && killedBy.connection == DuckNetwork.localConnection)
                            {
                                DuckNetwork.GiveXP("Ducks Despawned", 1, 1, 4, 25, 40, 9999999);
                            }
                            if (Network.isActive && Network.isServer)
                            {
                                Send.Message(new NMAssignKill(new List<Profile>
                                {
                                    killedBy
                                }, null));
                            }
                        }
                    }
                }
            }
            if (Highlights.highlightRatingMultiplier != 0f)
            {
                ProfileStats stats3 = profile.stats;
                int num = stats3.timesKilled;
                stats3.timesKilled = num + 1;
            }
            if (profile.connection == DuckNetwork.localConnection)
            {
                DuckNetwork.deaths++;
            }
            if (!isKillMessage)
            {
                if (profile.connection != DuckNetwork.localConnection)
                {
                    DuckNetwork.kills++;
                }
                if (TeamSelect2.Enabled("CORPSEBLOW", false))
                {
                    Grenade grenade = new Grenade(x, y)
                    {
                        hSpeed = hSpeed + Rando.Float(-2f, 2f),
                        vSpeed = vSpeed - Rando.Float(1f, 2.5f)
                    };
                    Level.Add(grenade);
                    grenade.PressAction();
                }
                SuperFondle(this, DuckNetwork.localConnection);
                if (_trappedInstance != null)
                {
                    SuperFondle(_trappedInstance, DuckNetwork.localConnection);
                }
                if (holdObject != null)
                {
                    SuperFondle(holdObject, DuckNetwork.localConnection);
                }
                if (y < -999f)
                {
                    Vec2 pos2 = position;
                    position = _lastGoodPosition;
                    GoRagdoll();
                    position = pos2;
                }
                else
                {
                    GoRagdoll();
                }
            }
            if (Network.isActive && ragdoll != null && !isKillMessage)
            {
                SuperFondle(ragdoll, DuckNetwork.localConnection);
            }
            if (Network.isActive && !isKillMessage)
            {
                lastAppliedLifeChange += 1;
                Send.Message(new NMKillDuck(profile.networkIndex, type is DTCrush, type is DTIncinerate, type is DTFall, lastAppliedLifeChange));
            }
            if (!(this is TargetDuck))
            {
                Global.Kill(this, type);
            }
            return true;
        }

        public override void Zap(Thing zapper)
        {
            GoRagdoll();
            if (ragdoll != null)
                ragdoll.Zap(zapper);
            base.Zap(zapper);
        }

        public override void Removed()
        {
            if (Network.isServer)
            {
                if (_ragdollInstance != null)
                {
                    Fondle(_ragdollInstance, DuckNetwork.localConnection);
                    Level.Remove(_ragdollInstance);
                }
                if (_trappedInstance != null)
                {
                    Fondle(_trappedInstance, DuckNetwork.localConnection);
                    Level.Remove(_trappedInstance);
                }
                if (_cookedInstance != null)
                {
                    Fondle(_cookedInstance, DuckNetwork.localConnection);
                    Level.Remove(_cookedInstance);
                }
            }
            base.Removed();
        }

        public void Disappear()
        {
            if (ragdoll != null)
            {
                position = ragdoll.position;
                if (Network.isActive)
                    ragdoll.Unragdoll();
                else
                    Level.Remove(ragdoll);
                vSpeed = -2f;
            }
            OnTeleport();
            y += 9999f;
        }

        public void Cook()
        {
            if (_cooked != null)
                return;
            if (ragdoll != null)
            {
                position = ragdoll.position;
                if (Network.isActive)
                    ragdoll.Unragdoll();
                else
                    Level.Remove(ragdoll);
                vSpeed = -2f;
            }
            if (Network.isActive)
            {
                _cooked = _cookedInstance;
                if (_cookedInstance != null)
                {
                    _cookedInstance.active = true;
                    _cookedInstance.visible = true;
                    _cookedInstance.solid = true;
                    _cookedInstance.enablePhysics = true;
                    _cookedInstance._sleeping = false;
                    _cookedInstance.x = x;
                    _cookedInstance.y = y;
                    _cookedInstance.owner = null;
                    ExtraFondle(_cookedInstance, DuckNetwork.localConnection);
                    ReturnItemToWorld(_cooked);
                    _cooked.vSpeed = vSpeed;
                    _cooked.hSpeed = hSpeed;
                }
            }
            else
            {
                _cooked = new CookedDuck(x, y);
                ReturnItemToWorld(_cooked);
                _cooked.vSpeed = vSpeed;
                _cooked.hSpeed = hSpeed;
                Level.Add(_cooked);
            }
            OnTeleport();
            SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
            y -= 25000f;
        }

        public void OnKill(DestroyType type = null)
        {
            if (!(type is DTPop))
            {
                SFX.Play("death");
                SFX.Play("pierce");
            }
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
                Level.Add(Feather.New(cameraPosition.x, cameraPosition.y, persona));
            if (!(Level.current is ChallengeLevel))
                Global.data.kills += 1;
            _remoteControl = false;
            switch (type)
            {
                case DTShot dtShot:
                    if (dtShot.bullet != null)
                    {
                        hSpeed = dtShot.bullet.travelDirNormalized.x * (dtShot.bullet.ammo.impactPower + 1f);
                        vSpeed = dtShot.bullet.travelDirNormalized.y * (dtShot.bullet.ammo.impactPower + 1f);
                    }
                    vSpeed -= 3f;
                    break;
                case DTIncinerate _:
                    Cook();
                    break;
                case DTPop _:
                    Disappear();
                    break;
            }
        }

        public bool crouchLock
        {
            get => _crouchLock;
            set => _crouchLock = value;
        }

        public TrappedDuck _trapped
        {
            get => _trappedProp;
            set => _trappedProp = value;
        }

        public virtual void Netted(Net n)
        {
            if (Network.isActive && (_trappedInstance == null || _trappedInstance.visible) || _trapped != null)
                return;
            if (Network.isActive)
            {
                _trapped = _trappedInstance;
                _trappedInstance.active = true;
                _trappedInstance.visible = true;
                _trappedInstance.solid = true;
                _trappedInstance.enablePhysics = true;
                _trappedInstance.x = x;
                _trappedInstance.y = y;
                _trappedInstance.owner = null;
                _trappedInstance.InitializeStuff();
                n.Fondle(_trappedInstance);
                n.Fondle(this);
                if (_trappedInstance._duckOwner != null)
                    RumbleManager.AddRumbleEvent(_trappedInstance._duckOwner.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            }
            else
            {
                RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                _trapped = new TrappedDuck(x, y, this);
                Level.Add(_trapped);
            }
            ReturnItemToWorld(_trapped);
            OnTeleport();
            if (holdObject != null)
                n.Fondle(holdObject);
            ThrowItem(false);
            Level.Remove(n);
            ++profile.stats.timesNetted;
            _trapped.clip.Add(this);
            _trapped.clip.Add(n);
            _trapped.hSpeed = hSpeed + n.hSpeed * 0.4f;
            _trapped.vSpeed = (float)(vSpeed + n.vSpeed - 1f);
            if (_trapped.hSpeed > 6f)
                _trapped.hSpeed = 6f;
            if (_trapped.hSpeed < -6f)
                _trapped.hSpeed = -6f;
            if (n.onFire)
                Burn(n.position, n);
            if (n.responsibleProfile == null || n.responsibleProfile.duck == null)
                return;
            trappedBy = n.responsibleProfile;
            n.responsibleProfile.duck.AddCoolness(1);
            Event.Log(new NettedEvent(n.responsibleProfile, profile));
        }

        public void Breath()
        {
            Vec2 vec2 = Offset(new Vec2(6f, 0f));
            if (ragdoll != null && ragdoll.part1 != null)
                vec2 = ragdoll.part1.Offset(new Vec2(6f, 0f));
            else if (_trapped != null)
                vec2 = _trapped.Offset(new Vec2(8f, -2f));
            Level.Add(BreathSmoke.New(vec2.x, vec2.y));
            Level.Add(BreathSmoke.New(vec2.x, vec2.y));
        }

        private void UpdateQuack()
        {
            if (dead || inputProfile == null || Level.current == null)
                return;
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                if (breath <= 0)
                {
                    if (breath == 0 && Level.current.cold && !underwater)
                        Breath();
                    breath = (int)(Rando.Int(70, 220) / DGRSettings.ActualParticleMultiplier);
                }
                --breath;
            }
            if (inputProfile.Pressed(Triggers.Quack))
            {
                float leftTrigger = inputProfile.leftTrigger;
                if (inputProfile.hasMotionAxis)
                    leftTrigger += inputProfile.motionAxis;
                Hat equipment = GetEquipment(typeof(Hat)) as Hat;
                if (equipment == null || equipment.quacks)
                {
                    if (Network.isActive)
                        _netQuack.Play(pit: leftTrigger);
                    else if (equipment != null)
                        equipment.Quack(1f, leftTrigger);
                    else
                        _netQuack.Play(pit: leftTrigger);
                }
                if (isServerForObject)
                    ++Global.data.quacks.valueInt;
                ++profile.stats.quacks;
                quack = 20;
            }
            if (!inputProfile.Down(Triggers.Quack))
                quack = Maths.CountDown(quack, 1, 0);
            if (!inputProfile.Released(Triggers.Quack))
                return;
            quack = 0;
        }

        public bool HasEquipment(Equipment t) => HasEquipment(t.GetType());

        public bool HasEquipment(Type t)
        {
            foreach (Thing thing in _equipment)
            {
                if (thing.GetAllTypesFiltered(typeof(Equipment)).Contains(t))
                    return true;
            }
            return false;
        }

        public void ObjectThrown(Holdable h)
        {
            h.enablePhysics = true;
            h.Thrown();
            h.owner = null;
            h.lastGrounded = DateTime.Now;
            h.solid = true;
            h.ReturnToWorld();
            ReturnItemToWorld(h);
        }

        public override float holdWeightMultiplier
        {
            get
            {
                float val2 = 1f;
                if (holdObject != null)
                    val2 = holdObject.weightMultiplier;
                if (holstered != null)
                    val2 = Math.Min(holstered.weightMultiplier, val2);
                return val2;
            }
        }

        public override float holdWeightMultiplierSmall => holdObject != null ? holdObject.weightMultiplierSmall : 1f;

        public virtual void ThrowItem(bool throwWithForce = true)
        {
            if (holdObject == null)
                return;
            if (_throwFondle)
                Fondle(holdObject);
            ObjectThrown(holdObject);
            holdObject.hSpeed = 0f;
            holdObject.vSpeed = 0f;
            holdObject.clip.Add(this);
            holdObstructed = false;
            if (holdObject is Mine && !(holdObject as Mine).pin && (!crouch || !grounded))
                (holdObject as Mine).Arm();
            if (!crouch)
            {
                float num1 = 1f;
                float num2 = 1f;
                if (inputProfile.Down(Triggers.Left) || inputProfile.Down(Triggers.Right))
                    num1 = 2.5f;
                if (num1 == 1f && inputProfile.Down(Triggers.Up))
                {
                    holdObject.vSpeed -= 5f * holdWeightMultiplier;
                }
                else
                {
                    float num3 = num1 * holdWeightMultiplier;
                    if (inputProfile.Down(Triggers.Up))
                        num2 = 2f;
                    float num4 = num2 * holdWeightMultiplier;
                    if (offDir > 0)
                        holdObject.hSpeed += 3f * num3;
                    else
                        holdObject.hSpeed -= 3f * num3;
                    if (reverseThrow)
                        holdObject.hSpeed = -holdObject.hSpeed;
                    holdObject.vSpeed -= 2f * num4;
                }
            }
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogAction(2);
            holdObject.hSpeed += 0.3f * offDir;
            holdObject.hSpeed *= holdObject.throwSpeedMultiplier;
            if (!throwWithForce)
                holdObject.hSpeed = holdObject.vSpeed = 0f;
            else if (Network.isActive)
            {
                if (isServerForObject)
                    _netTinyMotion.Play();
            }
            else
                SFX.Play("tinyMotion");
            _lastHoldItem = holdObject;
            _timeSinceThrow = 0;
            _holdObject = null;
        }

        public void GiveHoldable(Holdable h)
        {
            if (holdObject == h)
                return;
            if (holdObject != null)
                ThrowItem(false);
            if (h == null)
                return;
            if (profile.localPlayer)
            {
                if (h is RagdollPart)
                {
                    RagdollPart ragdollPart = h as RagdollPart;
                    if (ragdollPart.doll != null)
                    {
                        ragdollPart.doll.connection = connection;
                        Ragdoll doll = ragdollPart.doll;
                        doll.authority += 8;
                    }
                }
                else
                {
                    h.connection = connection;
                    Holdable holdable = h;
                    holdable.authority += 8;
                }
            }
            holdObject = h;
            holdObject.owner = this;
            holdObject.solid = false;
            h.hSpeed = 0f;
            h.vSpeed = 0f;
            h.enablePhysics = false;
            h._sleeping = false;
        }

        public virtual void TryGrab()
        {
            foreach (Holdable h in Level.CheckCircleAll<Holdable>(new Vec2(x, y + 4f), 18f).OrderBy(h => h, new CompareHoldablePriorities(this)))
            {
                if (h.owner == null && h.canPickUp && (h != _lastHoldItem || _timeSinceThrow >= 30) && h.active && h.visible && Level.CheckLine<Block>(position, h.position) == null)
                {
                    GiveHoldable(h);
                    if (holdObject.weight > 5f)
                    {
                        if (Rando.Float(1f) < 0.5)
                            PlaySFX("liftBarrel", pitch: Rando.Float(-0.1f, 0.2f));
                        else
                            PlaySFX("liftBarrel2", pitch: Rando.Float(-0.1f, 0.2f));
                        quack = 10;
                    }
                    else if (Network.isActive)
                    {
                        if (isServerForObject) _netTinyMotion.Play();
                    }
                    else SFX.Play("tinyMotion");
                    if (holdObject.disarmedFrom != this && (DateTime.Now - holdObject.disarmTime).TotalSeconds < 0.5)
                        AddCoolness(2);
                    tryGrabFrames = 0;
                    break;
                }
            }
        }

        private void UpdateThrow()
        {
            if (!isServerForObject)
                return;
            bool flag1 = false;
            if (CanMove())
            {
                if (HasEquipment(typeof(Holster)) && inputProfile.Down(Triggers.Up) && inputProfile.Pressed(Triggers.Grab) && (holdObject == null || holdObject.holsterable) && GetEquipment(typeof(Holster)) is Holster equipment && (!equipment.chained.value || equipment.containedObject == null))
                {
                    Holdable h = null;
                    bool flag2 = false;
                    if (equipment.containedObject != null)
                    {
                        h = equipment.containedObject;
                        equipment.SetContainedObject(null);
                        ObjectThrown(h);
                        flag2 = true;
                    }
                    if (holdObject != null)
                    {
                        if (holdObject is RagdollPart rp)
                        {
                            if (rp.doll != null && rp.doll.part3 != null)
                            {
                                holdObject.owner = null;
                                holdObject = rp.doll.part3;
                            }
                        }
                        holdObject.owner = this;
                        equipment.SetContainedObject(holdObject);
                        if (equipment.chained.value)
                        {
                            SFX.PlaySynchronized("equip");
                            for (int index = 0; index > DGRSettings.ActualParticleMultiplier * 3; ++index)
                                Level.Add(SmallSmoke.New(holdObject.x + Rando.Float(-3f, 3f), holdObject.y + Rando.Float(-3f, 3f)));
                        }
                        holdObject = null;
                        flag2 = true;
                    }
                    if (h != null)
                    {
                        GiveHoldable(h);
                        if (Network.isActive)
                        {
                            if (isServerForObject)
                                _netTinyMotion.Play();
                        }
                        else
                            SFX.Play("tinyMotion");
                    }
                    if (flag2)
                        return;
                }
                if (holdObject != null && inputProfile.Pressed(Triggers.Grab))
                    doThrow = true;
                if (!_isGhost && inputProfile.Pressed(Triggers.Grab) && holdObject == null)
                {
                    tryGrabFrames = 2;
                    TryGrab();
                }
            }
            if (flag1 || !doThrow || holdObject == null)
                return;
            Holdable holdObject1 = holdObject;
            doThrow = false;
            ThrowItem();
        }

        public void UpdateAnimation()
        {
            _updatedAnimation = true;
            if (_hovering)
            {
                ++_flapFrame;
                if (_flapFrame > 8)
                    _flapFrame = 0;
            }
            UpdateCurrentAnimation();
        }

        private void UpdateCurrentAnimation()
        {
            if (dead && _eyesClosed)
                _sprite.currentAnimation = "dead";
            else if (inNet)
                _sprite.currentAnimation = "netted";
            else if (listening)
                _sprite.currentAnimation = "listening";
            else if (crouch)
            {
                _sprite.currentAnimation = "crouch";
                if (!sliding)
                    return;
                _sprite.currentAnimation = "groundSlide";
            }
            else if (grounded)
            {
                if (hSpeed > 0f && !_gripped)
                {
                    _sprite.currentAnimation = "run";
                    if (strafing || Math.Sign(offDir) == Math.Sign(hSpeed))
                        return;
                    _sprite.currentAnimation = "slide";
                }
                else if (hSpeed < 0f && !_gripped)
                {
                    _sprite.currentAnimation = "run";
                    if (strafing || Math.Sign(offDir) == Math.Sign(hSpeed))
                        return;
                    _sprite.currentAnimation = "slide";
                }
                else
                    _sprite.currentAnimation = "idle";
            }
            else
            {
                _sprite.currentAnimation = "jump";
                _sprite.speed = 0f;
                if (vSpeed < 0f && !_hovering)
                    _sprite.frame = 0;
                else
                    _sprite.frame = 2;
            }
        }

        private void UpdateBurning()
        {
            burnSpeed = 0.005f;
            if (onFire && !dead)
            {
                if (_flameWait > 1f)
                    RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.Short));
                profile.stats.timeOnFire += Maths.IncFrameTimer();
                if (wallCollideLeft != null)
                {
                    offDir = 1;
                }
                else
                {
                    if (wallCollideRight == null)
                        return;
                    offDir = -1;
                }
            }
            else
            {
                if (onFire || dead)
                    return;
                burnt -= 0.005f;
                if (burnt >= 0f)
                    return;
                burnt = 0f;
            }
        }

        public override void Extinquish()
        {
            if (_trapped != null)
                _trapped.Extinquish();
            if (_ragdollInstance != null)
                _ragdollInstance.Extinguish();
            base.Extinquish();
        }

        public void ResetNonServerDeathState()
        {
            _isGhost = false;
            _killed = false;
            forceDead = false;
            unfocus = 1f;
            unfocused = false;
            active = true;
            solid = true;
            beammode = false;
            immobilized = false;
            gravMultiplier = 1f;
            if (!(Level.current is TeamSelect2) || (Level.current as TeamSelect2)._beam == null || (Level.current as TeamSelect2)._beam2 == null)
                return;
            (Level.current as TeamSelect2)._beam.RemoveDuck(this);
            (Level.current as TeamSelect2)._beam2.RemoveDuck(this);
        }

        public void Ressurect()
        {
            dead = false;
            if (ragdoll != null)
                ragdoll.Unragdoll();
            ResetNonServerDeathState();
            Regenerate();
            crouch = false;
            sliding = false;
            burnt = 0f;
            _onFire = false;
            hSpeed = 0f;
            vSpeed = 0f;
            if (Level.current != null && Level.current.camera is FollowCam)
                (Level.current.camera as FollowCam).Add(this);
            _cooked = null;
            ResurrectEffect(position);
            vSpeed = -3f;
            if (!Network.isActive || !isServerForObject)
                return;
            if (_cookedInstance != null)
                SuperFondle(_cookedInstance, DuckNetwork.localConnection);
                _cookedInstance.visible = false;
                _cookedInstance.active = false;
            ++lastAppliedLifeChange;
            Send.Message(new NMRessurect(position, this, lastAppliedLifeChange));
        }

        public static void ResurrectEffect(Vec2 pPosition)
        {
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                Level.Add(new CampingSmoke(pPosition.x - 5f + Rando.Float(10f), (float)(pPosition.y + 6f - 3f + Rando.Float(6f) - index * 1))
                {
                    move = {
            x = (Rando.Float(0.6f) - 0.3f),
            y = (Rando.Float(0.8f) - 1.8f)
          }
                });
        }
        /*
         * 2/8/2022
         * REMOVED VOID SINCE IT WAS COMPLETELY USELESS, USING A GHOSTPACK AN ITEM WHICH IS NEVER SPAWNED IN GAME AND IS COMPLETELY HIDDEN AND INVISIBLE
        private void UpdateGhostStatus()
        {
            GhostPack equipment = this.GetEquipment(typeof(GhostPack)) as GhostPack;
            if (equipment != null && !this._isGhost)
            {
                this._equipment.Remove(equipment);
                Level.Remove(equipment);
            }
            else if (equipment == null && this._isGhost)
            {
                GhostPack ghostPack = new GhostPack(0f, 0f);
                this._equipment.Add(ghostPack);
                ghostPack.Equip(this);
                Level.Add(ghostPack);
            }
            if (!this._isGhost)
                return;
            this._ghostTimer -= 23f / 1000f;
            if (_ghostTimer >= 0f)
                return;
            this._ghostTimer = 1f;
            this._isGhost = false;
            this.Ressurect();
        }*/

        public void Swear()
        {
            if (Network.isActive)
            {
                if (isServerForObject)
                    _netSwear.Play();
            }
            else
            {
                float num = 0f;
                if (profile.team != null && profile.team.name == "Sailors")
                    num += 0.1f;
                if (Rando.Float(1f) < 0.03f + profile.funslider * 0.045f + num)
                {
                    SFX.Play("quackBleep", 0.8f, Rando.Float(-0.05f, 0.05f));
                    Event.Log(new SwearingEvent(profile, profile));
                }
                else if (Rando.Float(1f) < 0.5f)
                    SFX.Play("cutOffQuack", pitch: Rando.Float(-0.05f, 0.05f));
                else
                    SFX.Play("cutOffQuack2", pitch: Rando.Float(-0.05f, 0.05f));
            }
            quack = 10;
        }

        public void Scream()
        {
            if (Network.isActive)
            {
                if (isServerForObject)
                    _netScream.Play();
            }
            else if (Rando.Float(1f) < 0.03f + profile.funslider * 0.045f)
            {
                SFX.Play("quackBleep", 0.9f);
                Event.Log(new SwearingEvent(profile, profile));
            }
            else if (Rando.Float(1f) < 0.5f)
                SFX.Play("quackYell03");
            else if (Rando.Float(1f) < 0.5f)
                SFX.Play("quackYell02");
            else
                SFX.Play("quackYell01");
            quack = 10;
        }

        public void Disarm(Thing disarmedBy)
        {
            if (!isServerForObject)
                return;
            if (holdObject != null && (!Network.isActive || disarmedBy != null && disarmedBy.isServerForObject))
                ++Global.data.disarms.valueInt;
            Profile responsibleProfile = disarmedBy?.responsibleProfile;
            if (responsibleProfile != null && holdObject != null)
            {
                disarmIndex = responsibleProfile.networkIndex;
                disarmIndexCooldown = 0.5f;
            }
            else
            {
                disarmIndex = 9;
                disarmIndexCooldown = 0.5f;
            }
            _disarmedBy = responsibleProfile;
            _disarmedAt = DateTime.Now;
            _holdingAtDisarm = holdObject;
            if (holdObject != null)
            {
                Fondle(holdObject);
                holdObject.disarmedFrom = this;
                holdObject.disarmTime = DateTime.Now;
                if (Network.isActive)
                {
                    if (isServerForObject)
                        _netDisarm.Play();
                }
                else
                    SFX.Play("disarm", 0.3f, Rando.Float(0.2f, 0.4f));
            }
            Event.Log(new DisarmEvent(responsibleProfile, profile));
            ThrowItem();
            Swear();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            Holdable hold = with as Holdable;
            if (_isGhost || with == null || (hold != null && hold.owner == this) || with is FeatherVolume || ((with == _lastHoldItem || (with.owner != null && with.owner == _lastHoldItem)) && _timeSinceThrow < 7) || with == _trapped || with == _trappedInstance || with is Dart || (with.owner != null && with.owner is SpikeHelm))
            {
                return;
            }
            if (with is IceWedge)
            {
                _iceWedging = 5;
            }
            if (with is RagdollPart)
            {
                RagdollPart part = with as RagdollPart;
                if (part != null && part.doll != null && part.doll.captureDuck != null && part.doll.captureDuck.killedByProfile == profile && part.doll.captureDuck.framesSinceKilled < 50)
                {
                    return;
                }
                if (part != null && part.doll != null && (part.doll.PartHeld() || (holdObject is Chainsaw && _timeSinceChainKill < 50)))
                {
                    return;
                }
                if (holdObject != null && holdObject is RagdollPart && part != null && part.doll != null && part.doll.holdingOwner == this)
                {
                    return;
                }
                if (ragdoll != null && (with == ragdoll.part1 || with == ragdoll.part2 || with == ragdoll.part3))
                {
                    return;
                }
                if (part.doll == null || part.doll.captureDuck == this)
                {
                    return;
                }
                if (_timeSinceThrow < 15 && part.doll != null && (part.doll.part1 == _lastHoldItem || part.doll.part2 == _lastHoldItem || part.doll.part3 == _lastHoldItem))
                {
                    return;
                }
            }
            if (!dead && !swinging && with is PhysicsObject && with.totalImpactPower > with.weightMultiplierInv * 2f)
            {
                if (with is Duck && with.weight >= 5f)
                {
                    Duck d = with as Duck;
                    bool bootsmash = d.HasEquipment(typeof(Boots)) && !d.sliding;
                    if (from == ImpactedFrom.Top && with.bottom - 5f < top && with.impactPowerV > 2f && bootsmash)
                    {
                        vSpeed = with.impactDirectionV * 0.5f;
                        with.vSpeed = -with.vSpeed * 0.7f;
                        d._groundValid = 7;
                        d.slamWait = 6;
                        if (with.isServerForObject)
                        {
                            RumbleManager.AddRumbleEvent(d.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                            MakeStars(position + new Vec2(0f, (crouch || ragdoll != null) ? -2 : -6), velocity);
                            if (Network.isActive)
                            {
                                Send.Message(new NMBonk(position + new Vec2(0f, (crouch || ragdoll != null) ? -2 : -6), velocity));
                            }
                        }
                        Helmet h = GetEquipment(typeof(Helmet)) as Helmet;
                        if (h != null)
                        {
                            SFX.Play("metalRebound", 1f, 0f, 0f, false);
                            RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                            h.Crush(d);
                            return;
                        }
                        if (with.isServerForObject)
                        {
                            StatBinding ducksCrushed = Global.data.ducksCrushed;
                            int valueInt = ducksCrushed.valueInt;
                            ducksCrushed.valueInt = valueInt + 1;
                            Kill(new DTCrush(with as PhysicsObject));
                            return;
                        }
                    }
                }
                else if (with.dontCrush)
                {
                    if (with.alpha > 0.99f && (from == ImpactedFrom.Left || from == ImpactedFrom.Right) && ((!Network.isActive && with.impactPowerH > 2.3f) || with.impactPowerH > 3f))
                    {
                        bool processDisarm = with.isServerForObject;
                        if (!processDisarm && Level.CheckLine<Block>(position, with.position) != null)
                        {
                            processDisarm = true;
                        }
                        if (processDisarm)
                        {
                            hSpeed = with.impactDirectionH * 0.5f;
                            if (!(with is EnergyScimitar))
                            {
                                with.hSpeed = -with.hSpeed * with.bouncy;
                            }
                            if (isServerForObject && (!Network.isActive || _disarmWait == 0) && _disarmDisable <= 0)
                            {
                                Disarm(with);
                                _disarmWait = 5;
                                RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                            }
                            if (!isServerForObject)
                            {
                                Send.Message(new NMDisarm(this, with.impactDirectionH * 0.5f), connection);
                                return;
                            }
                        }
                    }
                }
                else if (from == ImpactedFrom.Top && with.y < y && with.vSpeed > 0f && with.impactPowerV > 2f && with.weight >= 5f)
                {
                    if (with is PhysicsObject)
                    {
                        PhysicsObject wp = with as PhysicsObject;
                        if (wp.lastPosition.y + with.collisionOffset.y + with.collisionSize.y < top)
                        {
                            Helmet h2 = GetEquipment(typeof(Helmet)) as Helmet;
                            if (h2 != null && h2 is SpikeHelm && wp == (h2 as SpikeHelm).oldPoke)
                            {
                                return;
                            }
                            vSpeed = with.impactDirectionV * 0.5f;
                            with.vSpeed = -with.vSpeed * 0.5f;
                            if (with.isServerForObject)
                            {
                                MakeStars(position + new Vec2(0f, (crouch || ragdoll != null) ? -2 : -6), velocity);
                                if (Network.isActive)
                                {
                                    Send.Message(new NMBonk(position + new Vec2(0f, (crouch || ragdoll != null) ? -2 : -6), velocity));
                                }
                            }
                            if (h2 != null && ragdoll == null)
                            {
                                SFX.Play("metalRebound", 1f, 0f, 0f, false);
                                RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                                h2.Crush(wp);
                                return;
                            }
                            if (with.isServerForObject)
                            {
                                Kill(new DTCrush(with as PhysicsObject));
                                StatBinding ducksCrushed2 = Global.data.ducksCrushed;
                                int valueInt = ducksCrushed2.valueInt;
                                ducksCrushed2.valueInt = valueInt + 1;
                                return;
                            }
                        }
                    }
                }
                else if ((from == ImpactedFrom.Left || from == ImpactedFrom.Right) && ((!Network.isActive && with.impactPowerH > 2f) || with.impactPowerH > 3f))
                {
                    if ((holdObject is SledgeHammer && with is RagdollPart) || (holdObject is Sword && (holdObject as Sword).crouchStance && ((offDir < 0 && from == ImpactedFrom.Left) || (offDir > 0 && from == ImpactedFrom.Right))))
                    {
                        return;
                    }
                    bool processDisarm2 = with.isServerForObject;
                    if (!processDisarm2 && Level.CheckLine<Block>(position, with.position) != null)
                    {
                        processDisarm2 = true;
                    }
                    if (processDisarm2)
                    {
                        with.hSpeed = -with.hSpeed * with.bouncy;
                        if (with is TeamHat)
                        {
                            Swear();
                            return;
                        }
                        hSpeed = with.impactDirectionH * 0.5f;
                        if (isServerForObject && (!Network.isActive || _disarmWait == 0) && _disarmDisable <= 0)
                        {
                            Disarm(with);
                            RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                            _disarmWait = 5;
                        }
                        if (!isServerForObject)
                        {
                            Send.Message(new NMDisarm(this, with.impactDirectionH * 0.5f), connection);
                            return;
                        }
                    }
                }
                else if (!(with is TeamHat) && from == ImpactedFrom.Bottom && with.y > bottom && with.impactPowerV > 2f)
                {
                    vSpeed = with.impactDirectionV * 0.5f;
                    with.vSpeed = -with.vSpeed * 0.5f;
                }
            }
        }

        public override void OnTeleport()
        {
            if (holdObject != null)
                holdObject.OnTeleport();
            foreach (Thing thing in _equipment)
                thing.OnTeleport();
            if (_vine == null)
                return;
            _vine.Degrapple();
            _vine = null;
        }

        public void AdvanceServerTime(int frames)
        {
            while (frames > 0)
            {
                --frames;
                ++clientFrame;
                Update();
            }
        }

        public override void Initialize()
        {
            jumpSpeed = JumpSpeed;
            if (Level.current != null)
            {
                if (isServerForObject)
                {
                    foreach (Equipper equipper in Level.current.things[typeof(Equipper)])
                    {
                        if (equipper.radius.value == 0 || (position - equipper.position).length <= equipper.radius.value)
                        {
                            Thing containedInstance = equipper.GetContainedInstance(position);
                            if (containedInstance != null)
                            {
                                Level.Add(containedInstance);
                                if (containedInstance is Holdable)
                                    (containedInstance as Holdable).UpdateMaterial();
                                if ((bool)equipper.holstered || (bool)equipper.powerHolstered)
                                {
                                    Holster e = !(bool)equipper.powerHolstered ? new Holster(position.x, position.y) : new PowerHolster(position.x, position.y);
                                    Level.Add(e);
                                    e.SetContainedObject(containedInstance as Holdable);
                                    Equip(e);
                                    e.chained = equipper.holsterChained;
                                }
                                else if (containedInstance is Equipment && (containedInstance as Equipment).wearable)
                                {
                                    if (containedInstance is Hat && GetEquipment(typeof(Hat)) is Hat equipment)
                                    {
                                        Unequip(equipment);
                                        equipment.position = position;
                                        equipment.vSpeed = 0f;
                                        equipment.hSpeed = 0f;
                                    }
                                    Equip(containedInstance as Equipment);
                                }
                                else if (containedInstance is Holdable)
                                {
                                    if (holdObject != null)
                                    {
                                        Holdable holdObject = this.holdObject;
                                        ThrowItem(false);
                                        holdObject.position = position;
                                        holdObject.vSpeed = 0f;
                                        holdObject.hSpeed = 0f;
                                    }
                                    GiveHoldable(containedInstance as Holdable);
                                }
                            }
                        }
                    }
                }
                Level.Add(_featherVolume);
            }
            if (Network.isServer)
                _netProfileIndex = (byte)DuckNetwork.IndexOf(profile);
            if (Network.isActive)
                _netQuack.pitchBinding = new FieldBinding(this, "quackPitch");
            base.Initialize();
        }

        public override void NetworkUpdate()
        {
        }

        public Ragdoll ragdoll
        {
            get => _currentRagdoll;
            set
            {
                _currentRagdoll = value;
                if (_currentRagdoll == null)
                    return;
                _currentRagdoll._duck = this;
            }
        }

        public virtual void GoRagdoll()
        {
            if (Network.isActive && (_ragdollInstance == null || _ragdollInstance != null && _ragdollInstance.visible || _cookedInstance != null && _cookedInstance.visible) || ragdoll != null || _cooked != null)
                return;
            _hovering = false;
            float ypos = y + 4f;
            float degrees;
            if (sliding)
            {
                ypos += 6f;
                degrees = offDir >= 0 ? 0f : 180f;
            }
            else
                degrees = -90f;
            Vec2 v = new Vec2(_hSpeed, _vSpeed);
            hSpeed = 0f;
            vSpeed = 0f;
            if (Network.isActive)
            {
                ragdoll = _ragdollInstance;
                _ragdollInstance.active = true;
                _ragdollInstance.visible = true;
                _ragdollInstance.solid = true;
                _ragdollInstance.enablePhysics = true;
                _ragdollInstance.x = x;
                _ragdollInstance.y = y;
                _ragdollInstance.owner = null;
                _ragdollInstance.npi = netProfileIndex;
                _ragdollInstance.SortOutParts(x, ypos, this, sliding, degrees, offDir, v);
                Fondle(_ragdollInstance);
            }
            else
            {
                ragdoll = new Ragdoll(x, ypos, this, sliding, degrees, offDir, v);
                Level.Add(ragdoll);
                ragdoll.RunInit();
            }
            if (ragdoll == null)
                return;
            ragdoll.connection = connection;
            ragdoll.part1.connection = connection;
            ragdoll.part2.connection = connection;
            ragdoll.part3.connection = connection;
            if (!fancyShoes)
            {
                Hat hatt = (Hat) GetEquipment(typeof(Hat));
                if (hatt != null && !hatt.strappedOn && !(DGRSettings.StickyHats && hatt is TeamHat) && !(hatt is TeamHat th && th.team != null && th.team.metadata != null && th.team.metadata.StickyHat.value))
                {
                    Unequip(hatt);
                    hatt.hSpeed = hSpeed * 1.2f;
                    hatt.vSpeed = vSpeed - 2f;
                }
                ThrowItem(false);
            }
            OnTeleport();
            if (y > -4000f)
                y -= 5000f;
            sliding = false;
            crouch = false;
        }

        public virtual void UpdateSkeleton()
        {
            Vec2 position = this.position;
            if (_trapped != null)
            {
                x = _trapped.x;
                y = _trapped.y;
            }
            if (ragdoll != null)
            {
                if (ragdoll.part1 != null && ragdoll.part3 != null)
                {
                    _skeleton.upperTorso.position = ragdoll.part1.Offset(new Vec2(0f, 7f));
                    _skeleton.upperTorso.orientation = ragdoll.part1.offDir > 0 ? -ragdoll.part1.angle : ragdoll.part1.angle;
                    _skeleton.lowerTorso.position = ragdoll.part3.Offset(new Vec2(5f, 11f));
                    _skeleton.lowerTorso.orientation = (ragdoll.part3.offDir > 0 ? -ragdoll.part3.angle : ragdoll.part3.angle) + Maths.DegToRad(180f);
                    _skeleton.head.position = ragdoll.part1.Offset(new Vec2(-2f, -6f));
                    _skeleton.head.orientation = ragdoll.part1.offDir > 0 ? -ragdoll.part1.angle : ragdoll.part1.angle;
                }
            }
            else if (_sprite != null)
            {
                _skeleton.head.position = Offset(DuckRig.GetHatPoint(_sprite.imageIndex)) + new Vec2(0f, verticalOffset);
                _skeleton.upperTorso.position = Offset(DuckRig.GetChestPoint(_sprite.imageIndex)) + new Vec2(0f, verticalOffset);
                _skeleton.lowerTorso.position = this.position + new Vec2(0f, verticalOffset);
                if (sliding)
                {
                    _skeleton.head.orientation = Maths.DegToRad(90f);
                    _skeleton.upperTorso.orientation = Maths.DegToRad(90f);
                    _skeleton.lowerTorso.orientation = 0f;
                }
                else
                {
                    float num = offDir < 0 ? angle : -angle;
                    _skeleton.head.orientation = num;
                    _skeleton.upperTorso.orientation = num;
                    _skeleton.lowerTorso.orientation = num;
                }
                if (_trapped != null)
                {
                    _skeleton.head.orientation = 0f;
                    _skeleton.upperTorso.orientation = 0f;
                    _skeleton.lowerTorso.orientation = 0f;
                    _skeleton.head.position = Offset(new Vec2(-1f, -10f));
                    _skeleton.upperTorso.position = Offset(new Vec2(1f, 2f));
                    _skeleton.lowerTorso.position = Offset(new Vec2(0f, -8f));
                }
            }
            this.position = position;
        }

        public override float angle
        {
            get => _angle + tilt * 0.2f;
            set => _angle = value;
        }

        public PhysicsSnapshotDuckProperties GetProperties() => new PhysicsSnapshotDuckProperties()
        {
            jumping = jumping
        };

        public void SetProperties(PhysicsSnapshotDuckProperties dat) => jumping = dat.jumping;

        public Holdable holstered
        {
            get => GetEquipment(typeof(Holster)) is Holster equipment ? equipment.containedObject : null;
            set
            {
                if (!(GetEquipment(typeof(Holster)) is Holster equipment))
                    return;
                equipment.SetContainedObject(value);
            }
        }

        public Holdable skewered => GetEquipment(typeof(SpikeHelm)) is SpikeHelm equipment ? equipment.poked as Holdable : null;

        public TV tvHeld => this.holdObject is TV holdObject && !holdObject._ruined ? holdObject : null;

        public TV tvHolster
        {
            get
            {
                TV tv = null;
                if (GetEquipment(typeof(Holster)) is Holster equipment && equipment.containedObject is TV)
                    tv = equipment.containedObject as TV;
                return tv != null && !tv._ruined ? tv : null;
            }
        }

        public TV tvHat
        {
            get
            {
                TV tv = null;
                if (GetEquipment(typeof(SpikeHelm)) is SpikeHelm equipment && equipment.poked is TV)
                    tv = equipment.poked as TV;
                return tv != null && !tv._ruined ? tv : null;
            }
        }

        public bool HasTV() => tvHeld != null || tvHolster != null || tvHat != null;

        public int HasTVs(bool pChannel)
        {
            int num = 0;
            if (tvHeld != null && tvHeld.channel == pChannel)
                ++num;
            if (tvHolster != null && tvHolster.channel == pChannel)
                ++num;
            if (tvHat != null && tvHat.channel == pChannel)
                ++num;
            return num;
        }

        public bool CheckTVChannel(bool pChannel) => tvHeld != null && tvHeld.channel == pChannel || tvHolster != null && tvHolster.channel == pChannel || tvHat != null && tvHat.channel == pChannel;

        public bool CheckTVJump()
        {
            if (pipeOut > 0)
                return false;
            bool flag = false;
            if (tvHeld != null && tvHeld.channel && tvHeld.jumpReady)
            {
                tvHeld.jumpReady = false;
                flag = true;
            }
            else if (tvHolster != null && tvHolster.channel && tvHolster.jumpReady)
            {
                tvHolster.jumpReady = false;
                flag = true;
            }
            else if (tvHat != null && tvHat.channel && tvHat.jumpReady)
            {
                tvHat.jumpReady = false;
                flag = true;
            }
            if (DGRSettings.S_ParticleMultiplier != 0 && flag)
            {
                Level.Add(new ColorStar(x + hSpeed * 2f, y + 4f, new Vec2(-1.5f, -2.5f) + new Vec2((float)((hSpeed + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2f), Rando.Float(-0.5f, 0f)), new Color(237, 94, 238)));
                Level.Add(new ColorStar(x + hSpeed * 2f, y + 4f, new Vec2(-0.9f, -1.5f) + new Vec2((float)((hSpeed + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2f), Rando.Float(-0.5f, 0f)), new Color(49, 162, 242)));
                Level.Add(new ColorStar(x + hSpeed * 2f, y + 4f, new Vec2(0.9f, -1.5f) + new Vec2((float)((hSpeed + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2f), Rando.Float(-0.5f, 0f)), new Color(247, 224, 90)));
                Level.Add(new ColorStar(x + hSpeed * 2f, y + 4f, new Vec2(1.5f, -2.5f) + new Vec2((float)((hSpeed + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2f), Rando.Float(-0.5f, 0f)), new Color(192, 32, 45)));
            }
            return flag;
        }

        public void UpdateMove()
        {
            if (inputProfile == null)
                return;
            if (pipeOut > 0)
            {
                --pipeOut;
                if (pipeOut == 2 && !inputProfile.Down(Triggers.Jump))
                    pipeOut = 0;
                else
                    vSpeed -= 0.5f;
            }
            if (pipeBoost > 0)
                --pipeBoost;
            if (slamWait > 0)
                --slamWait;
            tvJumped = false;
            ++_timeSinceChainKill;
            weight = 5.3f;
            if (holdObject != null)
            {
                weight += Math.Max(0f, holdObject.weight - 5f);
                if (holdObject.destroyed)
                    ThrowItem();
            }
            if (isServerForObject)
                UpdateQuack();
            if (hat != null)
                hat.SetQuack(quack > 0 || _mindControl != null && _derpMindControl ? 1 : 0);
            if (_ragdollInstance != null && _ragdollInstance.part2 != null)
                _ragdollInstance.part2.UpdateLastReasonablePosition(position);
            if (inNet && _trapped != null)
            {
                x = _trapped.x;
                y = _trapped.y;
                if (_ragdollInstance != null && _ragdollInstance.part2 != null)
                    _ragdollInstance.part2.UpdateLastReasonablePosition(position);
                owner = _trapped;
                ThrowItem(false);
            }
            else
            {
                owner = null;
                skipPlatFrames = Maths.CountDown(skipPlatFrames, 1, 0);
                crippleTimer = Maths.CountDown(crippleTimer, 0.1f);
                if (inputProfile.Pressed(Triggers.Jump))
                {
                    _jumpValid = 4;
                    if (!grounded && crouch)
                        skipPlatFrames = 10;
                }
                else
                    _jumpValid = Maths.CountDown(_jumpValid, 1, 0);
                _skipPlatforms = false;
                if (inputProfile.Down(Triggers.Down) && skipPlatFrames > 0)
                    _skipPlatforms = true;
                bool flag1 = grounded;
                if (!flag1 && HasEquipment(typeof(ChokeCollar)))
                {
                    ChokeCollar equipment = GetEquipment(typeof(ChokeCollar)) as ChokeCollar;
                    if (equipment.ball.grounded && equipment.ball.bottom < top && vSpeed > -1f)
                        flag1 = true;
                }
                if (flag1)
                {
                    framesSinceJump = 0;
                    _groundValid = 7;
                    _hovering = false;
                    _double = false;
                }
                else
                {
                    _groundValid = Maths.CountDown(_groundValid, 1, 0);
                    ++framesSinceJump;
                }
                if (mindControl != null)
                    mindControl.UpdateExtraInput();
                //this._heldLeft = false;
                //this._heldRight = false;
                Block lockBlock = Level.CheckRect<Block>(new Vec2(x - 3f, y - 9f), new Vec2(x + 3f, y + 4f));
                _crouchLock = (crouch || sliding) && lockBlock != null && lockBlock.solid;
                float hAcc = 0.55f * holdWeightMultiplier * grappleMultiplier * accelerationMultiplier;
                maxrun = _runMax * holdWeightMultiplier;
                if (_isGhost)
                {
                    hAcc *= 1.4f;
                    maxrun *= 1.5f;
                }
                int num2 = HasTVs(false);
                for (int index = 0; index < num2; ++index)
                {
                    hAcc *= 1.4f;
                    maxrun *= 1.5f;
                }
                if (holdObject is EnergyScimitar && (holdObject as EnergyScimitar).dragSpeedBonus)
                {
                    if ((holdObject as EnergyScimitar)._spikeDrag)
                    {
                        hAcc *= 0.5f;
                        maxrun *= 0.5f;
                    }
                    else
                    {
                        hAcc *= 1.3f;
                        maxrun *= 1.35f;
                    }
                }
                if (specialFrictionMod > 0f)
                    hAcc *= Math.Min(specialFrictionMod * 2f, 1f);
                if (isServerForObject && isOffBottomOfLevel && !dead)
                {
                    if (ragdoll != null && ragdoll.part1 != null && ragdoll.part2 != null && ragdoll.part3 != null)
                    {
                        ragdoll.part1.y += 500f;
                        ragdoll.part2.y += 500f;
                        ragdoll.part3.y += 500f;
                    }
                    y += 500f;
                    Kill(new DTFall());
                    ++profile.stats.fallDeaths;
                }
                if (Network.isActive && ragdoll != null && ragdoll.connection != DuckNetwork.localConnection && ragdoll.TryingToControl() && !ragdoll.PartHeld())
                    Fondle(ragdoll);
                if (CanMove())
                {
                    if (!_grounded)
                        profile.stats.airTime += Maths.IncFrameTimer();
                    if (isServerForObject && !sliding && inputProfile.Pressed(Triggers.Up))
                    {
                        Desk t = Level.Nearest<Desk>(position, 22f);
                        if (t != null && Level.CheckLine<Block>(position, t.position) == null)
                        {
                            Fondle(t);
                            t.Flip(offDir < 0);
                        }
                    }
                    float leftMul;
                    if (inputProfile.Down(Triggers.Left))
                    {
                        leftMul = 1f;
                        if (_leftPressedFrame == 0)
                            _leftPressedFrame = (int)Graphics.frame;
                    }
                    else
                    {
                        leftMul = Maths.NormalizeSection(Math.Abs(Math.Min(inputProfile.leftStick.x, 0f)), 0.2f, 0.9f);
                        if (leftMul > 0.01f)
                        {
                            if (_leftPressedFrame == 0)
                                _leftPressedFrame = (int)Graphics.frame;
                        }
                        else
                            _leftPressedFrame = 0;
                    }
                    float rightMul;
                    if (inputProfile.Down(Triggers.Right))
                    {
                        rightMul = 1f;
                        if (_rightPressedFrame == 0)
                            _rightPressedFrame = (int)Graphics.frame;
                    }
                    else
                    {
                        rightMul = Maths.NormalizeSection(Math.Max(inputProfile.leftStick.x, 0f), 0.2f, 0.9f);
                        if (rightMul > 0.01f)
                        {
                            if (_rightPressedFrame == 0)
                                _rightPressedFrame = (int)Graphics.frame;
                        }
                        else
                            _rightPressedFrame = 0;
                    }
                    bool oldAngleCode = Options.Data.oldAngleCode;
                    if (!isServerForObject && inputProfile != null)
                        oldAngleCode = inputProfile.oldAngles;
                    if (leftMul < 0.01f && onFire && offDir == 1)
                        rightMul = 1f;
                    if (rightMul < 0.01f && onFire && offDir == -1)
                        leftMul = 1f;
                    if (grappleMul)
                    {
                        leftMul *= 1.5f;
                        rightMul *= 1.5f;
                    }
                    if (DevConsole.qwopMode && Level.current is GameLevel)
                    {
                        if (leftMul > 0f)
                            offDir = -1;
                        else if (rightMul > 0f)
                            offDir = 1;
                        if (_walkTime == 0)
                        {
                            rightMul = leftMul = 0f;
                        }
                        else
                        {
                            if (offDir < 0)
                                leftMul = 1f;
                            else
                                rightMul = 1f;
                            --_walkTime;
                        }
                        if (_walkCount > 0)
                            --_walkCount;
                        if (inputProfile.Pressed(Triggers.LeftTrigger))
                        {
                            if (_walkCount > 0 && _nextTrigger)
                            {
                                GoRagdoll();
                                _walkCount = 0;
                            }
                            else
                            {
                                _walkCount += 20;
                                if (DevConsole.rhythmMode && Level.current is GameLevel)
                                    _walkTime += 20;
                                else
                                    _walkTime += 8;
                                if (_walkTime > 20)
                                    _walkTime = 20;
                                if (_walkCount > 40)
                                    _walkCount = 40;
                                _nextTrigger = true;
                            }
                        }
                        else if (inputProfile.Pressed(Triggers.RightTrigger))
                        {
                            if (_walkCount > 0 && !_nextTrigger)
                            {
                                GoRagdoll();
                                _walkCount = 0;
                            }
                            else
                            {
                                _walkCount += 20;
                                if (DevConsole.rhythmMode && Level.current is GameLevel)
                                    _walkTime += 20;
                                else
                                    _walkTime += 8;
                                if (_walkTime > 20)
                                    _walkTime = 20;
                                if (_walkCount > 40)
                                    _walkCount = 40;
                                _nextTrigger = false;
                            }
                        }
                    }
                    bool nudging = _crouchLock && grounded && inputProfile.Pressed(Triggers.Any);
                    if (nudging && offDir == -1)
                    {
                        leftMul = 1f;
                        rightMul = 0f;
                    }
                    if (nudging && offDir == 1)
                    {
                        rightMul = 1f;
                        leftMul = 0f;
                    }
                    if (_leftJump)
                        leftMul = 0f;
                    else if (_rightJump)
                        rightMul = 0f;
                    strafing = false;
                    if (!_moveLock)
                    {
                        strafing = inputProfile.Down(Triggers.Strafe);
                        if (offdirLocked)
                        {
                            strafing = true;
                        }
                        if (leftMul > 0.01f && !crouch | nudging)
                        {
                            if (hSpeed > -maxrun * leftMul)
                            {
                                hSpeed -= hAcc;
                                if (hSpeed < -maxrun * leftMul)
                                    hSpeed = -maxrun * leftMul;
                            }
                            //this._heldLeft = true;
                            if (!strafing && !nudging && (oldAngleCode || _leftPressedFrame > _rightPressedFrame))
                                offDir = -1;
                        }
                        if (rightMul > 0.01f && !crouch | nudging)
                        {
                            if (hSpeed < maxrun * rightMul)
                            {
                                hSpeed += hAcc;
                                if (hSpeed > maxrun * rightMul)
                                    hSpeed = maxrun * rightMul;
                            }
                            //this._heldRight = true;
                            if (!strafing && !nudging && (oldAngleCode || _rightPressedFrame > _leftPressedFrame))
                                offDir = 1;
                        }
                        if (isServerForObject && strafing)
                            Global.data.strafeDistance.valueFloat += Math.Abs(hSpeed) * 0.00015f;
                        if (_atWallFrames > 0)
                        {
                            --_atWallFrames;
                        }
                        else
                        {
                            atWall = false;
                            leftWall = false;
                            rightWall = false;
                        }
                        _canWallJump = GetEquipment(typeof(WallBoots)) != null;
                        int wallJumpGiveFrames = 6;
                        if (!grounded && _canWallJump)
                        {
                            Block blockLeft = Level.CheckLine<Block>(topLeft + new Vec2(0f, 4f), bottomLeft + new Vec2(-3f, -4f));
                            Block blockRight = Level.CheckLine<Block>(topRight + new Vec2(3f, 4f), bottomRight + new Vec2(0f, -4f));
                            if (inputProfile.Down(Triggers.Left) && blockLeft != null && !blockLeft.clip.Contains(this))
                            {
                                atWall = true;
                                leftWall = true;
                                _atWallFrames = wallJumpGiveFrames;
                                if (!onWall)
                                {
                                    onWall = true;
                                    SFX.Play("wallTouch", pitch: Rando.Float(-0.1f, 0.1f));
                                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                                    {
                                        Feather feather1 = Feather.New(x + (!leftWall ? -4f : 4f) + Rando.Float(-1f, 1f), y + Rando.Float(-4f, 4f), persona);
                                        Feather feather2 = feather1;
                                        feather2.velocity *= 0.9f;
                                        if (leftWall)
                                            feather1.hSpeed = Rando.Float(-1f, 2f);
                                        else
                                            feather1.hSpeed = Rando.Float(-2f, 1f);
                                        feather1.vSpeed = Rando.Float(-2f, 1.5f);
                                        Level.Add(feather1);
                                    }
                                }
                            }
                            else if (inputProfile.Down(Triggers.Right) && blockRight != null && !blockRight.clip.Contains(this))
                            {
                                atWall = true;
                                rightWall = true;
                                _atWallFrames = wallJumpGiveFrames;
                                if (!onWall)
                                {
                                    onWall = true;
                                    SFX.Play("wallTouch", pitch: Rando.Float(-0.1f, 0.1f));
                                    for (int index = 0; index < 2; ++index)
                                    {
                                        Feather feather3 = Feather.New(x + (!leftWall ? -4f : 4f) + Rando.Float(-1f, 1f), y + Rando.Float(-4f, 4f), persona);
                                        feather3.vSpeed = Rando.Float(-2f, 1.5f);
                                        Feather feather4 = feather3;
                                        feather4.velocity *= 0.9f;
                                        if (leftWall)
                                            feather3.hSpeed = Rando.Float(-1f, 2f);
                                        else
                                            feather3.hSpeed = Rando.Float(-2f, 1f);
                                        Level.Add(feather3);
                                    }
                                }
                            }
                        }
                        if (onWall && _atWallFrames != wallJumpGiveFrames)
                        {
                            SFX.Play("wallLeave", pitch: Rando.Float(-0.1f, 0.1f));
                            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                            {
                                Feather feather5 = Feather.New(x + (!leftWall ? -4f : 4f) + Rando.Float(-1f, 1f), y + Rando.Float(-4f, 4f), persona);
                                feather5.vSpeed = Rando.Float(-2f, 1.5f);
                                Feather feather6 = feather5;
                                feather6.velocity *= 0.9f;
                                if (leftWall)
                                    feather5.hSpeed = Rando.Float(-1f, 2f);
                                else
                                    feather5.hSpeed = Rando.Float(-2f, 1f);
                                Level.Add(feather5);
                            }
                            onWall = false;
                        }
                        if ((leftWall || rightWall) && vSpeed > 1f && _atWallFrames == wallJumpGiveFrames)
                            vSpeed = 0.5f;
                        if (_wallJump > 0)
                            --_wallJump;
                        else
                            _rightJump = _leftJump = false;
                        bool canJump = _jumpValid > 0 && (_groundValid > 0 && !_crouchLock || atWall && _wallJump == 0 || doFloat);
                        if (_double && !HasJumpModEquipment() && !_hovering && inputProfile.Pressed(Triggers.Jump))
                        {
                            PhysicsRopeSection section = null;
                            if (_vine == null)
                                section = Level.Nearest<PhysicsRopeSection>(position, 18f);
                            if (section != null && (position - section.position).length < 18f)
                            {
                                _vine = section.rope.LatchOn(section, this);
                                _double = false;
                                canJump = false;
                                _groundValid = 0;
                            }
                        }
                        bool jumpDown = false;
                        if (canJump && Math.Abs(hSpeed) < 0.2f && inputProfile.Down(Triggers.Down) && Math.Abs(hSpeed) < 0.2f && inputProfile.Down(Triggers.Down))
                        {
                            foreach (IPlatform platform1 in Level.CheckLineAll<IPlatform>(bottomLeft + new Vec2(0.1f, 1f), bottomRight + new Vec2(-0.1f, 1f)))
                            {
                                if (platform1 is Block)
                                {
                                    canJump = true;
                                    break;
                                }
                                if (platform1 is MaterialThing materialThing)
                                {
                                    clip.Add(materialThing);
                                    foreach (IPlatform platform2 in Level.CheckPointAll<IPlatform>(materialThing.topLeft + new Vec2(-2f, 2f)))
                                    {
                                        if (platform2 != null && platform2 is MaterialThing && !(platform2 is Block))
                                            clip.Add(platform2 as MaterialThing);
                                    }
                                    foreach (IPlatform platform3 in Level.CheckPointAll<IPlatform>(materialThing.topRight + new Vec2(2f, 2f)))
                                    {
                                        if (platform3 != null && platform3 is MaterialThing && !(platform3 is Block))
                                            clip.Add(platform3 as MaterialThing);
                                    }
                                    canJump = false;
                                }
                            }
                            if (!canJump)
                            {
                                ++y;
                                vSpeed = 1f;
                                _groundValid = 0;
                                _hovering = false;
                                jumping = true;
                                jumpDown = true;
                            }
                        }
                        PhysicsRopeSection section1 = null;
                        if (_vine == null)
                        {
                            section1 = Level.Nearest<PhysicsRopeSection>(position, 19f);
                            if (section1 != null && (position - section1.position).length >= 18.0f)
                                section1 = null;
                        }
                        bool jet = false;
                        if (!jumpDown)
                        {
                            if (inputProfile.Pressed(Triggers.Jump))
                            {
                                if (HasEquipment(typeof(Jetpack)) && (_groundValid <= 0 || crouch || sliding))
                                {
                                    GetEquipment(typeof(Jetpack)).PressAction();
                                    jet = true;
                                }
                                if (!canJump && HasTV() && CheckTVChannel(true) && CheckTVJump() && section1 == null)
                                {
                                    _groundValid = 9999;
                                    canJump = true;
                                    tvJumped = true;
                                }
                            }
                            if (inputProfile.Down(Triggers.Jump) && HasEquipment(typeof(Jetpack)) && (_groundValid <= 0 || crouch || sliding))
                                jet = true;
                            if (inputProfile.Released(Triggers.Jump) && HasEquipment(typeof(Jetpack)))
                                GetEquipment(typeof(Jetpack)).ReleaseAction();
                            if (inputProfile.Pressed(Triggers.Jump) && HasEquipment(typeof(Grapple)) && !grounded && _jumpValid <= 0 && _groundValid <= 0)
                                jet = true;
                        }
                        bool canJump2 = canJump && !jet;
                        bool cancelJumping = false;
                        bool halfSpeed = false;
                        bool releasedVine = false;
                        if (!canJump2 && _vine != null && inputProfile.Released(Triggers.Jump))
                        {
                            _vine.Degrapple();
                            _vine = null;
                            if (!inputProfile.Down(Triggers.Down))
                            {
                                canJump2 = true;
                                cancelJumping = true;
                            }
                            if (!inputProfile.Down(Triggers.Up))
                                halfSpeed = true;
                            releasedVine = true;
                        }
                        if (canJump2)
                        {
                            if (atWall)
                            {
                                _wallJump = 8;
                                if (leftWall)
                                {
                                    hSpeed += 4f;
                                    _leftJump = true;
                                }
                                else if (rightWall)
                                {
                                    hSpeed -= 4f;
                                    _rightJump = true;
                                }
                                vSpeed = jumpSpeed;
                            }
                            else
                                vSpeed = jumpSpeed;
                            jumping = true;
                            sliding = false;
                            if (Network.isActive)
                            {
                                if (isServerForObject)
                                    _netJump.Play();
                            }
                            else
                                SFX.Play("jump", 0.5f);
                            _groundValid = 0;
                            _hovering = false;
                            _jumpValid = 0;
                            ++profile.stats.timesJumped;
                            if (Recorder.currentRecording != null)
                                Recorder.currentRecording.LogAction(6);
                        }
                        if (cancelJumping)
                        {
                            jumping = false;
                            if (halfSpeed && vSpeed < 0f)
                                vSpeed *= 0.7f;
                        }
                        if (inputProfile.Released(Triggers.Jump))
                        {
                            if (jumping)
                            {
                                jumping = false;
                                pipeOut = 0;
                                if (vSpeed < 0f)
                                    vSpeed *= 0.5f;
                            }
                            _hovering = false;
                        }
                        if (!canJump2 && !HasJumpModEquipment() && _groundValid <= 0)
                        {
                            bool flag11 = !crouch && holdingWeight <= 5f && (pipeOut <= 0 || vSpeed > -0.1f);
                            if (!_hovering && inputProfile.Pressed(Triggers.Jump))
                            {
                                if (section1 != null)
                                {
                                    _vine = section1.rope.LatchOn(section1, this);
                                    _double = false;
                                }
                                else if (_vine == null && flag11)
                                {
                                    _hovering = true;
                                    _flapFrame = 0;
                                }
                            }
                            if (flag11 && _hovering && vSpeed >= 0f)
                            {
                                if (vSpeed > 1f)
                                    vSpeed = 1f;
                                vSpeed -= 0.15f;
                            }
                        }
                        if (doFloat)
                            _hovering = false;
                        if (isServerForObject)
                        {
                            if (inputProfile.Down(Triggers.Down))
                            {
                                if (!grounded && HasTV())
                                {
                                    if (slamWait <= 0)
                                    {
                                        if (vSpeed < vMax)
                                            vSpeed += 0.6f;
                                        crouch = true;
                                    }
                                    else
                                        crouch = false;
                                }
                                else
                                    crouch = true;
                                if (!disableCrouch && !crouchCancel)
                                {
                                    if (grounded && Math.Abs(hSpeed) > 1f)
                                    {
                                        if (!sliding && slideBuildup < -0.3f)
                                        {
                                            slideBuildup = 0.4f;
                                            didFireSlide = true;
                                        }
                                        sliding = true;
                                    }
                                }
                                else
                                    crouch = false;
                            }
                            else
                            {
                                if (!_crouchLock)
                                {
                                    crouch = false;
                                    sliding = false;
                                }
                                crouchCancel = false;
                            }
                            if (!sliding)
                                didFireSlide = false;
                            if (slideBuildup > 0f || !sliding || !didFireSlide)
                            {
                                slideBuildup -= Maths.IncFrameTimer();
                                if (slideBuildup <= -0.6f)
                                    slideBuildup = -0.6f;
                            }
                        }
                        if (isServerForObject && !(holdObject is DrumSet) && !(holdObject is Trumpet) && inputProfile.Pressed(Triggers.Ragdoll) && !(Level.current is TitleScreen) && pipeOut <= 0)
                        {
                            framesSinceRagdoll = 0;
                            GoRagdoll();
                        }
                        if (isServerForObject && grounded && Math.Abs(vSpeed) + Math.Abs(hSpeed) < 0.5 && !_closingEyes && holdObject == null && inputProfile.Pressed(Triggers.Shoot))
                        {
                            Ragdoll t = Level.Nearest<Ragdoll>(position, 100.0f);
                            if (t != null && t.active && t.visible && t.captureDuck != null && t.captureDuck.dead && !t.captureDuck._eyesClosed && (t.part1.position - (position + new Vec2(0f, 8f))).length < 4f)
                            {
                                Level.Add(new EyeCloseWing(t.part1.angle < 0f ? x - 4f : x - 11f, y + 7f, t.part1.angle < 0f ? 1 : -1, _spriteArms, this, t.captureDuck));
                                if (Network.isActive)
                                    Send.Message(new NMEyeCloseWing(position, this, t.captureDuck));
                                _closingEyes = true;
                                ++profile.stats.respectGivenToDead;
                                AddCoolness(1);
                                _timeSinceDuckLayedToRest = DateTime.Now;
                                Flower flower = Level.Nearest<Flower>(position, 22f);
                                if (flower != null && (flower.position - position).length < 22f)
                                {
                                    Fondle(t);
                                    Fondle(t.captureDuck);
                                    if (Network.isActive)
                                        Send.Message(new NMFuneral(profile, t.captureDuck));
                                    t.captureDuck.LayToRest(profile);
                                    if (!Music.currentSong.Contains("MarchOfDuck"))
                                    {
                                        if (Network.isActive)
                                            Send.Message(new NMPlayMusic("MarchOfDuck"));
                                        Music.Play("MarchOfDuck", false);
                                    }
                                }
                            }
                        }
                        if (inputProfile.Released(Triggers.Jump) || vineRelease)
                            vineRelease = false;
                        if (releasedVine)
                            vineRelease = true;
                    }
                }
                disableCrouch = false;
            }
        }

        public override Vec2 cameraPosition
        {
            get
            {
                Vec2 zero = Vec2.Zero;
                Vec2 cameraPosition = ragdoll == null ? (_cooked == null ? (_trapped == null ? base.cameraPosition : _trapped.cameraPosition) : _cooked.cameraPosition) : ragdoll.cameraPosition;
                if ((cameraPositionOverride - position).length < 1000f)
                    cameraPositionOverride = Vec2.Zero;
                if (cameraPositionOverride != Vec2.Zero)
                    return cameraPositionOverride;
                if (cameraPosition.y < -1000f || cameraPosition == Vec2.Zero || cameraPosition.x < -5000f)
                    cameraPosition = prevCamPosition;
                else
                    prevCamPosition = cameraPosition;
                return cameraPosition;
            }
        }

        public override Vec2 anchorPosition => cameraPosition;

        public Thing followPart => _followPart == null ? this : _followPart;

        public bool offdirLocked;
        public bool underwater => doFloat && _curPuddle != null && top + 2 > _curPuddle.top;

        public void EmitBubbles(int num, float hVel)
        {
            if (!underwater)
                return;
            for (int index = 0; index < num; ++index)
                Level.Add(new TinyBubble(x + (offDir > 0 ? 6 : -6) * (sliding ? -1 : 1) + Rando.Float(-1f, 1f), top + 7f + Rando.Float(-1f, 1f), Rando.Float(hVel) * offDir, _curPuddle.top + 7f));
        }

        public void MakeStars()
        {
            if (DGRSettings.ActualParticleMultiplier == 0) return;
            Level.Add(new DizzyStar(x + offDir * -3, y - 9f, new Vec2(Rando.Float(-0.8f, -1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(x + offDir * -3, y - 9f, new Vec2(Rando.Float(-0.8f, -1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(x + offDir * -3, y - 9f, new Vec2(Rando.Float(0.8f, 1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(x + offDir * -3, y - 9f, new Vec2(Rando.Float(0.8f, 1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(x + offDir * -3, y - 9f, new Vec2(Rando.Float(-1.5f, 1.5f), Rando.Float(-0.5f, -1.1f))));
        }

        public static void MakeStars(Vec2 pPosition, Vec2 pVelocity)
        {
            if (DGRSettings.ActualParticleMultiplier == 0) return;
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(-1.7f, -1f) + new Vec2((float)((pVelocity.x + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2), Rando.Float(-0.5f, 0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(-0.7f, -0.5f) + new Vec2((float)((pVelocity.x + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2), Rando.Float(-0.5f, 0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(0.7f, -0.5f) + new Vec2((float)((pVelocity.x + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2), Rando.Float(-0.5f, 0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(1.7f, -1f) + new Vec2((float)((pVelocity.x + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2), Rando.Float(-0.5f, 0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(0f, -1.4f) + new Vec2((float)((pVelocity.x + Rando.Float(-0.5f, 0.5f)) * Rando.Float(0.6f, 0.9f) / 2), Rando.Float(-0.5f, 0f) - 1f), new Color(247, 224, 89)));
        }

        public override bool active
        {
            get => _active;
            set => _active = value;
        }
        public bool get_chatting() //public bool chatting => profile != null && profile.netData.Get<bool>(nameof(chatting));
        {
            return profile != null && profile.netData.Get<bool>("chatting");
            //oldchatting; // for old mods
        }
        public bool chatting;   // for old mods

        public virtual void DuckUpdate()
        {
        }

        public override void SpecialNetworkUpdate()
        {
        }

        public override void OnGhostObjectAdded()
        {
            if (!isServerForObject)
                return;
            if (_trappedInstance == null)
            {
                _trappedInstance = new TrappedDuck(x, y - 9999f, this)
                {
                    active = false,
                    visible = false,
                    authority = (NetIndex8)80
                };
                if (!GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_trappedInstance);
                Level.Add(_trappedInstance);
                Fondle(_trappedInstance);
            }
            if (_cookedInstance == null)
            {
                _cookedInstance = new CookedDuck(x, y - 9999f)
                {
                    active = false,
                    visible = false,
                    authority = (NetIndex8)80
                };
                if (!GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_cookedInstance);
                Level.Add(_cookedInstance);
                if (_profile.localPlayer)
                    Fondle(_cookedInstance);
            }
            if (_ragdollInstance != null)
                return;
            _ragdollInstance = new Ragdoll(x, y - 9999f, this, false, 0f, 0, Vec2.Zero)
            {
                npi = netProfileIndex
            };
            _ragdollInstance.RunInit();
            _ragdollInstance.active = false;
            _ragdollInstance.visible = false;
            _ragdollInstance.authority = (NetIndex8)80;
            Level.Add(_ragdollInstance);
            Fondle(_ragdollInstance);
        }

        private void RecoverServerControl()
        {
            Fondle(this, DuckNetwork.localConnection);
            Fondle(holdObject, DuckNetwork.localConnection);
            Fondle(_trappedInstance, DuckNetwork.localConnection);
            Fondle(_ragdollInstance, DuckNetwork.localConnection);
            foreach (Thing t in _equipment)
                Fondle(t, DuckNetwork.localConnection);
        }

        public override void Update()
        {
            if (Network.isActive && _trappedInstance != null && _trappedInstance.ghostObject != null && !_trappedInstance.ghostObject.IsInitialized())
                return;
            tilt = Lerp.FloatSmooth(tilt, 0f, 0.25f);
            verticalOffset = Lerp.FloatSmooth(verticalOffset, 0f, 0.25f);
            if (swordInvincibility > 0)
                --swordInvincibility;
            if ((ragdoll == null || ragdoll.tongueStuck == Vec2.Zero) && tongueCheck != Vec2.Zero && level.cold)
            {
                Block block = Level.CheckPoint<Block>(tongueCheck);
                if (block != null && block.physicsMaterial == PhysicsMaterial.Metal)
                {
                    GoRagdoll();
                    if (ragdoll != null)
                    {
                        ragdoll.tongueStuck = tongueCheck;
                        ragdoll.tongueStuckThing = block;
                        ragdoll.tongueShakes = 0;
                    }
                }
            }
            if (Network.isActive)
            {
                UpdateConnectionIndicators();
                if (_profile == Profiles.EnvironmentProfile && _netProfileIndex >= 0 && _netProfileIndex < DG.MaxPlayers)
                    AssignNetProfileIndex((byte)_netProfileIndex);
            }
            ++framesSinceRagdoll;
            if (killedByProfile != null)
                ++framesSinceKilled;
            int num = crouch ? 1 : 0;
            if (_sprite == null)
                return;
            fancyShoes = HasEquipment(typeof(FancyShoes));
            if (isServerForObject && inputProfile != null)
            {
                if (NetworkDebugger.enabled)
                {
                    if (inputProfile.CheckCode(Level.core.konamiCode) || inputProfile.CheckCode(Level.core.konamiCodeAlternate))
                    {
                        position = cameraPosition;
                        Presto();
                    }
                }
                else if (inputProfile.CheckCode(Input.konamiCode) || inputProfile.CheckCode(Input.konamiCodeAlternate))
                {
                    position = cameraPosition;
                    Presto();
                }
            }
            if (_disarmWait > 0)
                --_disarmWait;
            if (_disarmDisable > 0)
                --_disarmDisable;
            if (killMultiplier > 0f)
                killMultiplier -= 0.016f;
            else
                killMultiplier = 0f;
            if (isServerForObject && this.holdObject != null && this.holdObject.removeFromLevel)
                this.holdObject = null;
            if (Network.isActive)
            {
                if (isServerForObject)
                {
                    if (_assignedIndex)
                    {
                        _assignedIndex = false;
                        Fondle(this, DuckNetwork.localConnection);
                        if (this.holdObject != null)
                            PowerfulRuleBreakingFondle(this.holdObject, DuckNetwork.localConnection);
                        foreach (Equipment t in _equipment)
                        {
                            if (t != null)
                                PowerfulRuleBreakingFondle(t, DuckNetwork.localConnection);
                        }
                    }
                    if (inputProfile != null && !manualQuackPitch)
                    {
                        float leftTrigger = inputProfile.leftTrigger;
                        if (inputProfile.hasMotionAxis)
                            leftTrigger += inputProfile.motionAxis;
                        quackPitch = (byte)(leftTrigger * byte.MaxValue);
                    }
                    ++_framesSinceInput;
                    if (inputProfile != null && (inputProfile.Pressed("", true) || Level.current is RockScoreboard))
                    {
                        _framesSinceInput = 0;
                        afk = false;
                    }
                    if (_framesSinceInput > 1200)
                        afk = true;
                }
                else if (profile != null)
                {
                    if (disarmIndex != 9 && disarmIndex != _prevDisarmIndex && (_prevDisarmIndex == profile.networkIndex || _prevDisarmIndex == 9) && disarmIndex >= 0 && disarmIndex < 8 && DuckNetwork.profiles[disarmIndex].connection == DuckNetwork.localConnection)
                        ++Global.data.disarms.valueInt;
                    _prevDisarmIndex = disarmIndex;
                }
                if (isServerForObject)
                {
                    disarmIndexCooldown -= Maths.IncFrameTimer();
                    if (disarmIndexCooldown <= 0f && profile != null)
                    {
                        disarmIndexCooldown = 0f;
                        disarmIndex = profile.networkIndex;
                    }
                }
                if (y > -999f)
                    _lastGoodPosition = position;
                if (Network.isActive)
                {
                    if (_ragdollInstance != null)
                        _ragdollInstance.captureDuck = this;
                    if (ragdoll != null && ragdoll.isServerForObject)
                    {
                        if (_trapped != null && _trapped.y > -5000f)
                        {
                            if (Network.isActive)
                            {
                                ragdoll.active = false;
                                ragdoll.visible = false;
                                ragdoll.owner = null;
                                if (y > -1000f)
                                {
                                    ragdoll.y = -9999f;
                                    if (ragdoll.part1 != null)
                                        ragdoll.part1.y = -9999f;
                                    if (ragdoll.part2 != null)
                                        ragdoll.part2.y = -9999f;
                                    if (ragdoll.part3 != null)
                                        ragdoll.part3.y = -9999f;
                                }
                            }
                            else
                                Level.Remove(this);
                            ragdoll = null;
                        }
                        if (ragdoll != null)
                        {
                            if (ragdoll.y < -5000f)
                            {
                                ragdoll.position = cameraPosition;
                                if (ragdoll.part1 != null)
                                    ragdoll.part1.position = cameraPosition;
                                if (ragdoll.part2 != null)
                                    ragdoll.part2.position = cameraPosition;
                                if (ragdoll.part3 != null)
                                    ragdoll.part3.position = cameraPosition;
                            }
                            if (ragdoll.part1 != null && ragdoll.part1.owner != null && ragdoll.part1.owner.y < -5000f)
                                ragdoll.part1.owner = null;
                            if (ragdoll.part2 != null && ragdoll.part2.owner != null && ragdoll.part2.owner.y < -5000f)
                                ragdoll.part2.owner = null;
                            if (ragdoll.part3 != null && ragdoll.part3.owner != null && ragdoll.part3.owner.y < -5000f)
                                ragdoll.part3.owner = null;
                        }
                    }
                    if (_trapped != null && _trapped.y < -5000f && _trapped.isServerForObject)
                        _trapped.position = cameraPosition;
                    if (_cooked != null && _cooked.y < -5000f && _cooked.isServerForObject)
                        _cooked.position = cameraPosition;
                }
                if (_profile.localPlayer && !(this is RockThrowDuck) && isServerForObject)
                {
                    if (ragdoll == null && _trapped == null && _cooked == null && y < -5000f)
                        position = cameraPosition;
                    if (_ragdollInstance != null)
                    {
                        if (ragdoll == null && (_ragdollInstance.part1 != null && _ragdollInstance.part1.owner != null || _ragdollInstance.part2 != null && _ragdollInstance.part2.owner != null || _ragdollInstance.part3 != null && _ragdollInstance.part3.owner != null))
                        {
                            Thing owner1 = _ragdollInstance.part1.owner;
                            Thing owner2 = _ragdollInstance.part2.owner;
                            Thing owner3 = _ragdollInstance.part3.owner;
                            GoRagdoll();
                            if (owner1 != null)
                            {
                                _ragdollInstance.connection = owner1.connection;
                                _ragdollInstance.part1.owner = owner1;
                            }
                            if (owner2 != null)
                            {
                                _ragdollInstance.connection = owner2.connection;
                                _ragdollInstance.part2.owner = owner2;
                            }
                            if (owner3 != null)
                            {
                                _ragdollInstance.connection = owner3.connection;
                                _ragdollInstance.part3.owner = owner3;
                            }
                        }
                        if (_ragdollInstance.visible)
                        {
                            ragdoll = _ragdollInstance;
                        }
                        else
                        {
                            _ragdollInstance.visible = true;
                            _ragdollInstance.visible = false;
                            if (_ragdollInstance.part1 != null)
                                _ragdollInstance.part1.y = -9999f;
                            if (_ragdollInstance.part2 != null)
                                _ragdollInstance.part2.y = -9999f;
                            if (_ragdollInstance.part3 != null)
                                _ragdollInstance.part3.y = -9999f;
                            ragdoll = null;
                        }
                        if (_cookedInstance != null)
                        {
                            if (_cookedInstance.visible)
                            {
                                _cooked = _cookedInstance;
                                if (_ragdollInstance != null)
                                {
                                    _ragdollInstance.visible = false;
                                    _ragdollInstance.active = false;
                                    ragdoll = null;
                                }
                            }
                            else
                            {
                                _cooked = null;
                                _cookedInstance.y = -9999f;
                            }
                        }
                    }
                }
            }
            if (_profile.localPlayer && !(this is RockThrowDuck) && connection != DuckNetwork.localConnection && !CanBeControlled())
                RecoverServerControl();
            if (_trappedInstance != null)
            {
                if (_trappedInstance.visible || _trappedInstance.owner != null)
                {
                    _trapped = _trappedInstance;
                }
                else
                {
                    _trappedInstance.owner = null;
                    _trapped = null;
                    _trappedInstance.y = -9999f;
                }
            }
            if (profile != null && mindControl != null && Level.current is GameLevel)
                profile.stats.timeUnderMindControl += Maths.IncFrameTimer();
            if (underwater)
            {
                ++_framesUnderwater;
                if (_framesUnderwater >= 60)
                {
                    _framesUnderwater = 0;
                    ++Global.data.secondsUnderwater.valueInt;
                }
                if (DGRSettings.S_ParticleMultiplier != 0)
                {
                    _bubbleWait += Rando.Float(0.015f, 0.017f) * DGRSettings.ActualParticleMultiplier;
                    if (Rando.Float(1f) > 0.99f)
                        _bubbleWait += 0.5f;
                    if (_bubbleWait > 1f)
                    {
                        _bubbleWait = Rando.Float(0.2f) * DGRSettings.ActualParticleMultiplier;
                        EmitBubbles(1, 1f);
                    }
                }
            }
            if (!quackStart && quack > 0)
            {
                quackStart = true;
                EmitBubbles((int)(Rando.Int(3, 6) * DGRSettings.ActualParticleMultiplier), 1.2f);
                if (Level.current.cold && !underwater)
                    Breath();
            }
            if (quack <= 0)
                quackStart = false;
            ++wait;
            if (TeamSelect2.doCalc && wait > 10 && profile != null)
            {
                wait = 0;
                float profileScore = profile.endOfRoundStats.CalculateProfileScore();
                if (firstCalc)
                {
                    firstCalc = false;
                    lastCalc = profileScore;
                }
                if (Math.Abs(lastCalc - profileScore) > 0.005f)
                {
                    int c = (int)Math.Round((profileScore - lastCalc) / 0.005f);
                    if (plus == null || plus.removeFromLevel)
                    {
                        plus = new CoolnessPlus(x, y, this, c);
                        Level.Add(plus);
                    }
                    else
                        plus.change = c;
                }
                lastCalc = profileScore;
            }
            grappleMultiplier = !grappleMul ? 1f : 1.5f;
            ++_timeSinceThrow;
            if (_timeSinceThrow > 30)
                _timeSinceThrow = 30;
            if (_resetAction && !inputProfile.Down(Triggers.Shoot))
                _resetAction = false;
            if (_converted == null)
            {
                _sprite.texture = profile.persona.sprite.texture;
                _spriteArms.texture = profile.persona.armSprite.texture;
                _spriteQuack.texture = profile.persona.quackSprite.texture;
                _spriteControlled.texture = profile.persona.controlledSprite.texture;
            }
            else
            {
                _sprite.texture = _converted.profile.persona.sprite.texture;
                _spriteArms.texture = _converted.profile.persona.armSprite.texture;
                _spriteQuack.texture = _converted.profile.persona.quackSprite.texture;
                _spriteControlled.texture = _converted.profile.persona.controlledSprite.texture;
            }
            --listenTime;
            if (listenTime < 0)
                listenTime = 0;
            if (listening && listenTime <= 0)
                listening = false;
            if (isServerForObject && !listening)
            {
                ++conversionResistance;
                if (conversionResistance > 100)
                    conversionResistance = 100;
            }
            //this._coolnessThisFrame = 0;
            UpdateBurning();
            //this.UpdateGhostStatus(); -NiK0 Removed useless void
            if (dead)
            {
                immobilized = true;
                if (unfocus > 0f)
                    unfocus -= 0.015f;
                else if (!unfocused)
                {
                    if (!grounded && _lives > 0)
                    {
                        IEnumerable<Thing> thing = Level.current.things[typeof(SpawnPoint)];
                        position = thing.ElementAt(Rando.Int(thing.Count() - 1)).position;
                    }
                    if (profile != null && profile.localPlayer && Level.current is TeamSelect2)
                    {
                        foreach (ProfileBox2 profile in (Level.current as TeamSelect2)._profiles)
                        {
                            if (profile.duck == this)
                            {
                                UnstoppableFondle(this, DuckNetwork.localConnection);
                                Vec2 vec2 = profile.position + new Vec2(82f, 58f);
                                if (!profile.rightRoom)
                                    vec2 = profile.position + new Vec2(58f, 58f);
                                position = vec2;
                                if (_ragdollInstance != null)
                                {
                                    UnstoppableFondle(_ragdollInstance, DuckNetwork.localConnection);
                                    _ragdollInstance.position = new Vec2(vec2.x, vec2.y - 3f);
                                    _ragdollInstance.Unragdoll();
                                }
                                RecoverServerControl();
                                if (ragdoll != null)
                                    ragdoll.Unragdoll();
                                position = vec2;
                                SFX.PlaySynchronized("convert", 0.75f);
                                Ressurect();
                                if (Network.isActive && ghostObject != null)
                                    ghostObject.SuperDirtyStateMask();
                            }
                        }
                    }
                    else
                    {
                        Respawner respawner = Level.Nearest<Respawner>(position);
                        if (respawner != null && profile != null && profile.localPlayer)
                        {
                            if (ragdoll != null)
                                ragdoll.Unragdoll();
                            position = respawner.position + new Vec2(0f, -16f);
                            SFX.PlaySynchronized("respawn", 0.65f);
                            Ressurect();
                        }
                        else if (_lives > 0)
                        {
                            _lives--;
                            unfocus = 1f;
                            _isGhost = true;
                            Regenerate();
                            immobilized = false;
                            crouch = false;
                            sliding = false;
                        }
                        else
                        {
                            unfocus = -1f;
                            unfocused = true;
                            if (isServerForObject)
                                visible = false;
                            if (!Network.isActive)
                                active = false;
                            if (Level.current.camera is FollowCam && !(Level.current is ChallengeLevel))
                                (Level.current.camera as FollowCam).Remove(this);
                            y -= 100000f;
                        }
                    }
                }
                sliding = true;
                crouch = true;
            }
            else if (quack > 0)
                profile.stats.timeWithMouthOpen += Maths.IncFrameTimer();
            if (DevConsole.rhythmMode && Level.current is GameLevel && (inputProfile.Pressed(Triggers.Down) || inputProfile.Pressed(Triggers.Jump) || inputProfile.Pressed(Triggers.Shoot) || inputProfile.Pressed(Triggers.Quack) || inputProfile.Pressed(Triggers.Grab)) && !RhythmMode.inTime)
                GoRagdoll();
            _iceWedging--;
            if (_iceWedging < 0)
                _iceWedging = 0;
            UpdateMove();
            if (inputProfile == null)
                return;
            if (sliding && _iceWedging <= 0 && grounded && Level.CheckLine<Block>(position + new Vec2(-10f, 0f), position + new Vec2(10f, 0f)) != null)
            {
                foreach (IPlatform platform in Level.CheckPointAll<IPlatform>(new Vec2(position.x, bottom - 4f)))
                {
                    if (platform is Holdable)
                        sliding = false;
                }
            }
            if (ragdoll != null)
                ragdoll.UpdateUnragdolling();
            centerOffset = 8f;
            if (crouch)
                centerOffset = 24f;
            if (ragdoll == null && isServerForObject)
                base.Update();
            if (ragdoll == null && _prevRagdoll != null && DGRSettings.S_ParticleMultiplier != 0)
            {
                Level.Add(SmallSmoke.New(x - Rando.Float(2f, 5f), (float)(y + Rando.Float(-3f, 3f) + 16)));
                Level.Add(SmallSmoke.New(x + Rando.Float(2f, 5f), (float)(y + Rando.Float(-3f, 3f) + 16)));
                Level.Add(SmallSmoke.New(x, (float)(y + Rando.Float(-3f, 3f) + 16)));
            }
            _prevRagdoll = ragdoll;
            if (kick > 0f) kick -= 0.1f;
            else kick = 0f;
            _sprite.speed = 0.1f + Math.Abs(hSpeed) / maxrun * 0.1f;
            _sprite.flipH = offDir < 0;
            if (!swinging)
                UpdateAnimation();
            if (_trapped != null)
                SetCollisionMode("netted");
            else if (_sprite.currentAnimation == "run" || _sprite.currentAnimation == "jump" || _sprite.currentAnimation == "idle")
                SetCollisionMode("normal");
            else if (_sprite.currentAnimation == "slide")
                SetCollisionMode("normal");
            else if (_sprite.currentAnimation == "crouch" || _sprite.currentAnimation == "listening")
                SetCollisionMode("crouch");
            else if (_sprite.currentAnimation == "groundSlide" || _sprite.currentAnimation == "dead")
                SetCollisionMode("slide");
            Holdable holdObject = this.holdObject;
            if (this.holdObject != null && isServerForObject && (ragdoll == null || !fancyShoes) && !inPipe)
            {
                this.holdObject.isLocal = isLocal;
                this.holdObject.UpdateAction();
            }
            if (Network.isActive && this.holdObject != null && (this.holdObject.duck != this || !this.holdObject.active || !this.holdObject.visible || !this.holdObject.isServerForObject && !(this.holdObject is RagdollPart)) && isServerForObject)
                this.holdObject = null;
            if (tryGrabFrames > 0 && !inputProfile.Pressed(Triggers.Grab))
            {
                tryGrabFrames--;
                TryGrab();
                if (this.holdObject != null)
                    tryGrabFrames = 0;
            }
            else
                tryGrabFrames = 0;
            UpdateThrow();
            doThrow = false;
            reverseThrow = false;
            UpdateHoldPosition();
            if (!isServerForObject)
                base.Update();
            forceFire = false;
            foreach (Equipment equipment in _equipment)
                equipment.PositionOnOwner();
            _gripped = false;
        }

        public override Thing realObject => _trapped != null ? _trapped : this;

        public bool protectedFromFire
        {
            get
            {
                if (_protectedFromFire)
                {
                    return true;
                }
                if (holdObject != null && holdObject.heat < -0.05f || holstered != null && holstered.heat < -0.05f)
                    return true;
                return skewered != null && skewered.heat < -0.05f;
            }
            set
            {
                _protectedFromFire = value;
            }
        }
        public override void HeatUp(Vec2 location)
        {
            if (holdObject != null && holdObject.heat < -0.05f)
                holdObject.DoHeatUp(0.03f, location);
            else if (holstered != null && holstered.heat < -0.05f)
                holstered.DoHeatUp(0.03f, location);
            else if (skewered != null && skewered.heat < -0.05f)
                skewered.DoHeatUp(0.03f, location);
            base.HeatUp(location);
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (protectedFromFire)
                return false;
            _burnTime -= 0.02f;
            if (!onFire)
            {
                if (!dead)
                {
                    if (Network.isActive)
                        Scream();
                    else
                        SFX.Play("quackYell0" + Change.ToString(Rando.Int(2) + 1), pitch: (Rando.Float(0.3f) - 0.3f));
                    SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                    if (Rando.Float(1f) < 0.1f)
                        AddCoolness(-1);
                    Event.Log(new LitOnFireEvent(litBy?.responsibleProfile, profile));
                    ++profile.stats.timesLitOnFire;
                    if (Recorder.currentRecording != null)
                        Recorder.currentRecording.LogAction(9);
                    if (ragdoll == null)
                    {
                        for (int index = 0; index < 5; ++index)
                            Level.Add(SmallFire.New(Rando.Float(12f) - 6f, Rando.Float(16f) - 8f, 0f, 0f, stick: this));
                    }
                }
                onFire = true;
            }
            return true;
        }

        public virtual void UpdateHoldPosition(bool updateLerp = true)
        {
            if (_sprite == null || y < -8000f) return;
            armOffY = 6f;
            armOffX = -3f * offDir;
            if (holdObject != null)
            {
                armOffY = 6f;
                armOffX = -2f * offDir;
            }
            holdOffX = 6f;
            holdOffY = -3f;
            if (holdObject != null)
            {
                holdObject._sleeping = false;
                if (holdObject.owner != this) return;
                if (!onFire && holdObject.heat > 0.5f && holdObject.physicsMaterial == PhysicsMaterial.Metal)
                {
                    if (_sizzle == null) _sizzle = SFX.Play("sizzle", 0.6f, looped: true);
                    _handHeat += 0.016f;
                    if (_handHeat > 0.4f && DGRSettings.S_ParticleMultiplier != 0)
                    {
                        if (handSmokeWait <= 0)
                        {
                            Vec2 vec2 = new Vec2(armPosition.x + holdObject.handOffset.x * offDir, armPosition.y + holdObject.handOffset.y);
                            Level.Add(SmallSmoke.New(vec2.x, vec2.y, 0.8f, 1f));
                            handSmokeWait = 5;
                        }
                        handSmokeWait--;
                    }
                    if (_handHeat > 1.1f)
                    {
                        _sizzle.Stop();
                        Scream();
                        ThrowItem();
                        _handHeat = 0f;
                    }
                }
                else
                {
                    if (_sizzle != null)
                    {
                        _sizzle.Stop();
                        _sizzle = null;
                    }
                    _handHeat = 0f;
                }
                if (_sprite.currentAnimation == "run")
                {
                    if (_sprite.frame == 1) holdOffY++;
                    else if (_sprite.frame == 2)
                    {
                        holdOffY++;
                        holdOffX--;
                    }
                    else if (_sprite.frame == 3)
                    {
                        holdOffY++;
                        holdOffX -= 2f;
                    }
                    else if (_sprite.frame == 4)
                    {
                        holdOffY++;
                        holdOffX--;
                    }
                    else if (_sprite.frame == 5) holdOffY++;
                }
                else if (_sprite.currentAnimation == "jump")
                {
                    if (_sprite.frame == 0) holdOffY++;
                    else if (_sprite.frame == 2) holdOffY--;
                }
            }
            else
            {
                if (_sizzle != null)
                {
                    _sizzle.Stop();
                    _sizzle = null;
                }
                _handHeat = 0f;
            }
            holdOffX *= offDir;
            if (holdObject == null || ragdoll != null && fancyShoes) return;

            _spriteArms.angle = holdAngle;
            _bionicArm.angle = holdAngle;

            if (gun != null) kick = gun.kick * 5f;

            if (holdObject is DrumSet) position = holdObject.position + new Vec2(0f, -12f);
            else holdObject.position = armPositionNoKick + holdObject.holdOffset + new Vec2(holdOffX, holdOffY) + new Vec2(2 * offDir, 0f);

            holdObject.CheckIfHoldObstructed();
            if (HasEquipment(typeof(Holster)))
            {
                Holster equipment = GetEquipment(typeof(Holster)) as Holster;
                if (!equipment.chained.value || equipment.containedObject == null)
                {
                    if (!isServerForObject) holdObstructed = equipment.netRaise;
                    else if (holdObject != null && inputProfile.Down(Triggers.Up) && holdObject.holsterable) holdObstructed = true;
                }
            }
            if (!(holdObject is RagdollPart)) holdObject.offDir = offDir;
            if (_sprite.currentAnimation == "slide")
            {
                holdOffY--;
                holdOffX++;
            }
            else if (_sprite.currentAnimation == "crouch")
            {
                if (holdObject != null) armOffY += 4f;
            }
            else if ((_sprite.currentAnimation == "groundSlide" || _sprite.currentAnimation == "dead") && holdObject != null) armOffY += 6f;
            UpdateHoldLerp(updateLerp);
            if (!(holdObject is DrumSet))
            {
                holdObject.position = HoldOffset(holdObject.holdOffset);
                if (!(holdObject is RagdollPart)) holdObject.angle = holdObject.handAngle + holdAngleOff;
            }
            //why was this double ever assigned
            //double y = this.holdObject.y;
        }

        public void UpdateHoldLerp(bool updateLerp = false, bool instant = false)
        {
            if (Level.current is ReplayLevel)
            {
                float armOffY = 6;
                float armOffX = -3f * offDir;
                float holdOffX = 6;
                float holdOffY = -3;
                if (holdObject != null)
                {
                    armOffY = 6f;
                    armOffX = -2f * offDir;
                }
                if (holdObject != null)
                {
                    holdObject._sleeping = false;
                    if (holdObject.owner != this)
                    {
                        return;
                    }
                    if (spriteImageIndex == 1)
                    {
                        holdOffY += 1f;
                    }
                    else if (spriteImageIndex == 2)
                    {
                        holdOffY += 1f;
                        holdOffX -= 1f;
                    }
                    else if (spriteImageIndex == 3)
                    {
                        holdOffY += 1f;
                        holdOffX -= 2f;
                    }
                    else if (spriteImageIndex == 4)
                    {
                        holdOffY += 1f;
                        holdOffX -= 1f;
                    }
                    else if (spriteImageIndex == 5)
                    {
                        holdOffY += 1f;
                    }
                    else if (spriteImageIndex == 7)
                    {
                        holdOffY += 1f;
                    }
                    else if (spriteImageIndex == 9)
                    {
                        holdOffY -= 1f;
                    }
                }

                holdOffX *= offDir;
                if (holdObject != null)
                {
                    _spriteArms.angle = holdAngle;
                    if (holdObject is DrumSet)
                    {
                        position = holdObject.position + new Vec2(0f, -12f);
                    }
                    else
                    {
                        holdObject.position = armPositionNoKick +  holdObject.holdOffset + new Vec2(holdOffX, holdOffY) + new Vec2((float)(2 * offDir), 0f);
                    }
                    holdObject.CheckIfHoldObstructed();
                    if (HasEquipment(typeof(Holster)))
                    {
                        Holster h = GetEquipment(typeof(Holster)) as Holster;
                        if (!h.chained.value || h.containedObject == null)
                        {
                            if (!isServerForObject)
                            {
                                holdObstructed = h.netRaise;
                            }
                            else if (holdObject != null && inputProfile.Down("UP") && holdObject.holsterable)
                            {
                                holdObstructed = true;
                            }
                        }
                    }
                    if (!(holdObject is RagdollPart))
                    {
                        holdObject.offDir = offDir;
                    }
                    if (spriteImageIndex == 12 || spriteImageIndex == 13)
                    {
                        armOffY = 12;
                        holdOffY = -3;
                        holdOffX += 1f;
                    }
                    else if (spriteImageIndex == 11)
                    {
                        if (holdObject != null)
                        {
                            armOffY += 4f;
                        }
                    }
                    else if ((spriteImageIndex == 12 || spriteImageIndex == 13) && holdObject != null)
                    {
                        armOffY += 6f;
                    }
                    if (!(holdObject is DrumSet))
                    {
                        holdObject.position = HoldOffset(holdObject.holdOffset);
                        if (!(holdObject is RagdollPart))
                        {
                            holdObject.angle = holdObject.handAngle + holdAngleOff;
                        }
                    }
                }
                this.armOffX = armOffX;
                this.armOffY = armOffY;
                this.holdOffX = holdOffX;
                this.holdOffY = holdOffY;
                //Extensions.SetPrivateFieldValue(__instance, "armOffX", armOffX);
                //Extensions.SetPrivateFieldValue(__instance, "armOffY", armOffY);

                //Extensions.SetPrivateFieldValue(__instance, "holdOffX", holdOffX);
                //Extensions.SetPrivateFieldValue(__instance, "holdOffY", holdOffY);
                return;
            }
            if (holdObject.canRaise && (_hovering && holdObject.hoverRaise || holdObstructed || holdObject.keepRaised))
            {
                if (updateLerp)
                    holdAngleOff = Maths.LerpTowards(holdAngleOff, -((float)(Math.PI / 2f) * offDir) * holdObject.angleMul, instant ? 1.0f : (holdObject.raiseSpeed) * 2);
                holdObject.raised = true;
            }
            else
            {
                if (updateLerp) 
                    holdAngleOff = Maths.LerpTowards(holdAngleOff, 0f, instant ? 1f : (float)(holdObject.raiseSpeed * 2f * 2f));
                if (holdObject.raised)
                {
                    holdObject.raised = false;
                }
            }
        }

        public Duck converted => _converted;

        public void ConvertDuck(Duck to)
        {
            if (_converted != to && to != null && to.profile != null)
                ++to.profile.stats.conversions;
            RumbleManager.AddRumbleEvent(profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.Short));
            _converted = to;
            _spriteArms = to._spriteArms.CloneMap();
            _spriteControlled = to._spriteControlled.CloneMap();
            _spriteQuack = to._spriteQuack.CloneMap();
            _sprite = to._sprite.CloneMap();
            graphic = _sprite;
            if (!isConversionMessage)
            {
                Equipment equipment = GetEquipment(typeof(TeamHat));
                if (equipment != null)
                    Unequip(equipment);
                if (to.profile.team.hasHat)
                {
                    Hat e = new TeamHat(0f, 0f, to.profile.team, to.profile);
                    Level.Add(e);
                    Equip(e, false);
                }
            }
            for (int index = 0; index < 3 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 10000); ++index)
                Level.Add(new MusketSmoke(x - 5f + Rando.Float(10f), (float)(y + 6f - 3f + Rando.Float(6f) - index * 1))
                {
                    move = {
            x = (Rando.Float(0.4f) - 0.2f),
            y = (Rando.Float(0.4f) - 0.2f)
          }
                });
            listenTime = 0;
            listening = false;
            vSpeed -= 5f;
            SFX.Play("convert");
        }

        public void DoFuneralStuff()
        {
            if (ragdoll != null) position = ragdoll.position;
            for (int index = 0; index < 3 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 10000); ++index)
                Level.Add(new MusketSmoke(position.x - 5f + Rando.Float(10f), (float)(position.y + 6f - 3f + Rando.Float(6f) - index * 1f))
                {
                    move = {
                    x = Rando.Float(0.4f) - 0.2f,
                    y = Rando.Float(0.4f) - 0.2f
                  }
                });
            _timeSinceFuneralPerformed = DateTime.Now;
            SFX.Play("death");
            profile.stats.funeralsRecieved++;
        }

        public void LayToRest(Profile whoDid)
        {
            if (ragdoll != null) position = ragdoll.position;
            if (!isConversionMessage)
            {
                Tombstone tombstone = new Tombstone(position.x, position.y);
                Level.Add(tombstone);
                tombstone.vSpeed = -2.5f;
            }
            DoFuneralStuff();
            if (ragdoll != null)
            {
                ragdoll.y += 10000f;
                ragdoll.part1.y += 10000f;
                ragdoll.part2.y += 10000f;
                ragdoll.part3.y += 10000f;
            }
            y += 10000f;
            if (whoDid == null) return;
            ++whoDid.stats.funeralsPerformed;
            whoDid.duck.AddCoolness(2);
        }

        public bool gripped
        {
            get => _gripped;
            set => _gripped = value;
        }

        public void UpdateLerp()
        {
            if (lerpSpeed == 0f) return;
            lerpPosition += lerpVector * lerpSpeed;
        }

        public bool IsQuacking()
        {
            if (quack > 0 || _mindControl != null && _derpMindControl) return true;
            return ragdoll != null && ragdoll.tongueStuck != Vec2.Zero;
        }

        public void DrawHat()
        {
            if (hat == null) return;
            if (_sprite != null) hat.alpha = _sprite.alpha;
            hat.offDir = offDir;
            hat.depth = depth + hat.equippedDepth;
            hat.angle = angle;
            hat.Draw();
            if (!DevConsole.showCollision) return;
            hat.DrawCollision();
        }

        public Vec2 GetPos()
        {
            if (ragdoll != null && ragdoll.part1 != null) position = ragdoll.part1.position;
            else if (_trapped != null) position = _trapped.position;
            return position;
        }

        public Vec2 GetEdgePos()
        {
            Vec2 camPos = cameraPosition;
            float num = 14f;
            if (camPos.x < Level.current.camera.left + num) camPos.x = Level.current.camera.left + num;
            if (camPos.x > Level.current.camera.right - num) camPos.x = Level.current.camera.right - num;
            if (camPos.y < Level.current.camera.top + num) camPos.y = Level.current.camera.top + num;
            if (camPos.y > Level.current.camera.bottom - num) camPos.y = Level.current.camera.bottom - num;
            return camPos;
        }

        public bool ShouldDrawIcon()
        {
            Vec2 position = this.position;
            if (ragdoll != null)
            {
                if (ragdoll.part1 == null)
                    return false;
                position = ragdoll.part1.position;
            }
            else if (_trapped != null)
                position = _trapped.position;
            if (Network.isActive && _trapped != null && _trappedInstance != null && !_trappedInstance.visible)
                position = this.position;
            if (Network.isActive && ragdoll != null && _ragdollInstance != null && !_ragdollInstance.visible)
                position = this.position;
            if (_cooked != null)
                position = _cooked.position;
            if (Network.isActive && _cooked != null && _cookedInstance != null && !_cookedInstance.visible)
                position = this.position;
            if (position.x < level.camera.left - 1000f || position.y < -3000f)
                return false;
            float num = -6f;
            if (level != null && level.camera != null && !dead && !VirtualTransition.doingVirtualTransition)
            {
                switch (Level.current)
                {
                    case GameLevel _:
                    case ChallengeLevel _:
                        if (Level.current.simulatePhysics)
                            return position.x < level.camera.left + num || position.x > level.camera.right - num || position.y < level.camera.top + num || position.y > level.camera.bottom - num;
                        break;
                }
            }
            return false;
        }

        public void PrepareIconForFrame()
        {
            if (dead)
                return;
            RenderTarget2D iconMap = persona.iconMap;
            Viewport viewport = Graphics.viewport;
            RenderTarget2D renderTarget = Graphics.GetRenderTarget();
            Graphics.SetRenderTarget(iconMap);
            Graphics.viewport = new Viewport(0, 0, 96, 96);
            if (_iconCamera == null)
                _iconCamera = new Camera(0f, 0f, 48f, 48f);
            _iconCamera.center = position + new Vec2(0f, 2f);
            if (crouch)
                _iconCamera.centerY += 3f;
            if (sliding)
            {
                _iconCamera.centerY += 6f;
                _iconCamera.centerX -= offDir * 7;
            }
            if (ragdoll != null && ragdoll.part2 != null)
                _iconCamera.center = ragdoll.part2.position - ragdoll.part2.velocity;
            if (_trapped != null)
                _iconCamera.center = _trapped.position + new Vec2(0f, -5f);
            if (_cooked != null)
                _iconCamera.center = _cooked.position + new Vec2(0f, -5f);
            renderingIcon = true;
            _renderingDuck = true;
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, _iconCamera.getMatrix());
            Graphics.DrawRect(_iconCamera.rectangle, Colors.Transparent, (Depth)0.99f);
            Graphics.screen.End();
            Graphics.ResetSpanAdjust();
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, _iconCamera.getMatrix());
            if (_cooked != null && (!Network.isActive || _cookedInstance != null && _cookedInstance.visible))
                _cooked.Draw();
            else if (ragdoll != null && ragdoll.part1 != null && ragdoll.part3 != null)
            {
                ragdoll.part1.Draw();
                ragdoll.part3.Draw();
                foreach (Thing thing in _equipment)
                    thing.Draw();
            }
            else if (_trapped != null)
            {
                _trapped.Draw();
                foreach (Thing thing in _equipment)
                    thing.Draw();
            }
            else
                Draw();
            if (onFire)
            {
                foreach (SmallFire smallFire in Level.current.things[typeof(SmallFire)])
                {
                    if (smallFire.stick != null && (smallFire.stick == this || smallFire.stick == _trapped || ragdoll != null && (smallFire.stick == ragdoll.part1 || smallFire.stick == ragdoll.part2)))
                        smallFire.Draw();
                }
            }
            Graphics.screen.End();
            _renderingDuck = false;
            renderingIcon = false;
            Graphics.SetRenderTarget(renderTarget);
            Graphics.viewport = viewport;
        }

        public void DrawIcon()
        {
            if (dead || _iconCamera == null || _renderingDuck || !ShouldDrawIcon())
                return;
            Vec2 position = this.position;
            if (ragdoll != null)
                position = ragdoll.part1.position;
            else if (_trapped != null)
                position = _trapped.position;
            Vec2 p2 = position;
            //why?
            //float num1 = (float)(Level.current.camera.width / 320f * 0.5);
            //float num2 = 0.75f;
            float num3 = 16.5f;
            Vec2 vec2_1 = new Vec2(0f, 0f);
            if (position.x < Level.current.camera.left + num3)
            {
                vec2_1.x = Math.Abs(Level.current.camera.left - position.x);
                position.x = Level.current.camera.left + num3;
            }
            if (position.x > Level.current.camera.right - num3)
            {
                vec2_1.x = Math.Abs(Level.current.camera.right - position.x);
                position.x = Level.current.camera.right - num3;
            }
            if (position.y < Level.current.camera.top + num3)
            {
                vec2_1.y = Math.Abs(Level.current.camera.top - position.y);
                position.y = Level.current.camera.top + num3;
            }
            if (position.y > Level.current.camera.bottom - num3)
            {
                vec2_1.y = Math.Abs(Level.current.camera.bottom - position.y);
                position.y = Level.current.camera.bottom - num3;
            }
            Vec2 vec2_2 = vec2_1 * (3f / 1000f);
            float num4 = 0.75f - Math.Min(vec2_2.length, 1f) * 0.4f;
            Graphics.Draw(persona.iconMap, position, new Rectangle?(_iconRect), Color.White, 0f, new Vec2(48f, 48f), new Vec2(0.5f, 0.5f) * num4, SpriteEffects.None, (Depth)(0.9f + depth.span));
            int imageIndex = _sprite.imageIndex;
            _sprite.imageIndex = 21;
            float rad = Maths.DegToRad(Maths.PointDirection(position, p2));
            _sprite.depth = (Depth)0.8f;
            _sprite.angle = -rad;
            _sprite.flipH = false;
            _sprite.UpdateSpriteBox();
            _sprite.position = new Vec2(position.x + (float)Math.Cos(rad) * 12f, position.y - (float)Math.Sin(rad) * 12f);
            _sprite.DrawWithoutUpdate();
            _sprite.angle = 0f;
            _sprite.imageIndex = imageIndex;
            _sprite.UpdateSpriteBox();
        }

        public Color blendColor
        {
            set
            {
                _spriteArms.color = value;
                _spriteControlled.color = value;
                _spriteQuack.color = value;
                _sprite.color = value;
            }
        }

        public virtual void OnDrawLayer(Layer pLayer)
        {
            if (_sprite == null || !localSpawnVisible || !ShouldDrawIcon())
                return;
            if (pLayer == Layer.PreDrawLayer)
            {
                PrepareIconForFrame();
            }
            else
            {
                if (pLayer != Layer.Foreground)
                    return;
                DrawIcon();
            }
        }

        public bool cordHover;
        public override void Draw()
        {
            if (_sprite == null || !localSpawnVisible)
                return;
            if (inNet)
            {
                DrawHat();
            }
            else
            {
                if (DevConsole.showCollision)
                    Graphics.DrawRect(_featherVolume.rectangle, Color.LightGreen, (Depth)0.6f, false, 0.5f);
                int num1 = _renderingDuck ? 1 : 0;
                bool skipDraw = false;
                if (Network.isActive)
                {
                    if (_trappedInstance != null && _trappedInstance.visible)
                        skipDraw = true;
                    if (_ragdollInstance != null && _ragdollInstance.visible)
                        skipDraw = true;
                    if (_cookedInstance != null && _cookedInstance.visible)
                        skipDraw = true;
                }
                Depth depth = this.depth;
                if (!skipDraw)
                {
                    if (!_renderingDuck)
                    {
                        if (!_updatedAnimation)
                        {
                            UpdateAnimation();
                        }
                        _updatedAnimation = false;
                        _sprite.UpdateFrame(false);
                    }
                    _sprite.flipH = (offDir < 0);
                    if (enteringWalldoor)
                    {
                        base.depth = -0.55f;
                    }
                    _spriteArms.depth = base.depth + 11;
                    _bionicArm.depth = base.depth + 11;
                    _spriteQuack.alpha = (_spriteControlled.alpha = (_sprite.alpha = (_spriteArms.alpha = (_isGhost ? 0.5f : 1f) * alpha)));
                    _spriteQuack.flipH = (_spriteControlled.flipH = _sprite.flipH);
                    _spriteControlled.depth = base.depth;
                    _sprite.depth = base.depth;
                    _spriteQuack.depth = base.depth;
                    _sprite.angle = (_spriteQuack.angle = (_spriteControlled.angle = angle));
                    if (ragdoll != null && ragdoll.tongueStuck != Vec2.Zero)
                        quack = 10;
                    if (IsQuacking())
                    {
                        Vec2 tounge = this.tounge;
                        if (sliding)
                        {
                            if (tounge.y < 0f)
                                tounge.y = 0f;
                            if (offDir > 0)
                            {
                                if (tounge.x < -0.3f)
                                    tounge.x = -0.3f;
                                if (tounge.x > 0.4f)
                                    tounge.x = 0.4f;
                            }
                            else
                            {
                                if (tounge.x < -0.4f)
                                    tounge.x = -0.4f;
                                if (tounge.x > 0.3f)
                                    tounge.x = 0.3f;
                            }
                        }
                        else
                        {
                            if (offDir > 0 && tounge.x < 0f)
                                tounge.x = 0f;
                            if (offDir < 0 && tounge.x > 0f)
                                tounge.x = 0f;
                            if (tounge.y < -0.3f)
                                tounge.y = -0.3f;
                            if (tounge.y > 0.4f)
                                tounge.y = 0.4f;
                        }
                        _stickLerp = Lerp.Vec2Smooth(_stickLerp, tounge, 0.2f);
                        _stickSlowLerp = Lerp.Vec2Smooth(_stickSlowLerp, tounge, 0.1f);
                        Vec2 stickLerp = _stickLerp;
                        stickLerp.y *= -1f;
                        Vec2 stickSlowLerp = _stickSlowLerp;
                        stickSlowLerp.y *= -1f;
                        int num10 = 0;
                        double length = stickLerp.length;
                        if (length > 0.5)
                            num10 = 72;
                        Graphics.Draw(_mindControl == null || !_derpMindControl ? _spriteQuack : _spriteControlled, _sprite.imageIndex + num10, x, y + verticalOffset, xscale, yscale);
                        if (length > 0.05f)
                        {
                            Vec2 vec2_1 = position + new Vec2(0f, 1f);
                            if (sliding)
                            {
                                vec2_1.y += 9f;
                                vec2_1.x -= 4 * offDir;
                            }
                            else if (crouch)
                                vec2_1.y += 4f;
                            else if (!grounded)
                                vec2_1.y -= 2f;
                            List<Vec2> vec2List = Curve.Bezier(8, vec2_1, vec2_1 + stickSlowLerp * 6f, vec2_1 + stickLerp * 6f);
                            Vec2 vec2_2 = Vec2.Zero;
                            float num11 = 1f;
                            foreach (Vec2 p2 in vec2List)
                            {
                                if (vec2_2 != Vec2.Zero)
                                {
                                    Vec2 vec2_3 = vec2_2 - p2;
                                    Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2, new Color(223, 30, 30), 0.15f * num11, this.depth + 1);
                                    Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2 - vec2_3.normalized * 0.4f, Color.Black, 0.3f * num11, this.depth - 1);
                                }
                                num11 -= 0.1f;
                                vec2_2 = p2;
                                tongueCheck = p2;
                            }
                            if (_graphic != null)
                            {
                                _spriteQuack.position = position;
                                _spriteQuack.alpha = alpha;
                                _spriteQuack.angle = angle;
                                _spriteQuack.depth = this.depth + 2;
                                _spriteQuack.scale = scale;
                                _spriteQuack.center = center;
                                _spriteQuack.frame += 36;
                                _spriteQuack.Draw();
                                _spriteQuack.frame -= 36;
                            }
                        }
                        else
                            tongueCheck = Vec2.Zero;
                    }
                    else
                    {
                        Graphics.DrawWithoutUpdate(_sprite, x, y + verticalOffset, xscale, yscale);
                        _stickLerp = Vec2.Zero;
                        _stickSlowLerp = Vec2.Zero;
                    }
                }
                if (_renderingDuck)
                {
                    if (holdObject != null)
                        holdObject.Draw();
                    foreach (Thing thing in _equipment)
                        thing.Draw();
                }
                if (_mindControl != null && _derpMindControl || listening)
                {
                    _swirlSpin += 0.2f;
                    _swirl.angle = _swirlSpin;
                    Graphics.Draw(_swirl, x, y - 12f);
                }
                DrawHat();
                if (!skipDraw)
                {
                    Grapple equipment = GetEquipment(typeof(Grapple)) as Grapple;
                    bool flag2 = equipment != null;
                    int num12 = 0;
                    if (equipment != null && equipment.hookInGun)
                        num12 = 36;
                    _spriteArms.imageIndex = _sprite.imageIndex;
                    if (!inNet && !_gripped && !listening)
                    {
                        Vec2 vec2 = Vec2.Zero;
                        if (gun != null)
                            vec2 = -gun.barrelVector * kick;
                        float num13 = Math.Abs((float)((_flapFrame - 4f) / 4f)) - 0.1f;
                        if (!_hovering && !cordHover)
                            num13 = 0f;
                        _spriteArms._frameInc = 0f;
                        _spriteArms.flipH = _sprite.flipH;
                        if (holdObject != null && !holdObject.ignoreHands && !holdObject.hideRightWing)
                        {
                            _spriteArms.angle = holdAngle;
                            _bionicArm.angle = holdAngle;
                            if (!flag2)
                            {
                                bool flipH = _spriteArms.flipH;
                                if (holdObject.handFlip)
                                    _spriteArms.flipH = !_spriteArms.flipH;
                                Graphics.Draw(_spriteArms, _sprite.imageIndex + 18 + Maths.Int(action) * 18 * (holdObject.hasTrigger ? 1 : 0), armPosition.x + holdObject.handOffset.x * offDir, armPosition.y + holdObject.handOffset.y, _sprite.xscale, _sprite.yscale);
                                _spriteArms._frameInc = 0f;
                                _spriteArms.flipH = flipH;
                                if (_sprite.currentAnimation == "jump")
                                {
                                    _spriteArms.angle = 0f;
                                    _spriteArms.depth = this.depth + -10;
                                    Graphics.Draw(_spriteArms, _sprite.imageIndex + 5 + (int)Math.Round(num13 * 2f), (float)(x + vec2.x + 2 * offDir * xscale), (float)(y + vec2.y + armOffY * yscale), -_sprite.xscale, _sprite.yscale, true);
                                    _spriteArms.depth = this.depth + 11;
                                }
                            }
                            else
                            {
                                _bionicArm.flipH = _sprite.flipH;
                                if (holdObject.handFlip)
                                    _bionicArm.flipH = !_bionicArm.flipH;
                                Graphics.Draw(_bionicArm, _sprite.imageIndex + 18 + num12, armPosition.x + holdObject.handOffset.x * offDir, armPosition.y + holdObject.handOffset.y, _sprite.xscale, _sprite.yscale);
                            }
                        }
                        else if (!_closingEyes)
                        {
                            if (!flag2)
                            {
                                _spriteArms.angle = 0f;
                                if ((_sprite.currentAnimation == "jump" && _spriteArms.imageIndex == 9) || cordHover)
                                {
                                    int num14 = 2;
                                    if (HasEquipment(typeof(ChestPlate)))
                                        num14 = 3;
                                    if (holdObject == null || !holdObject.hideRightWing)
                                    {
                                        _spriteArms.depth = this.depth + 11;
                                        Graphics.Draw(_spriteArms, _spriteArms.imageIndex + 5 + (int)Math.Round(num13 * 2f), (float)(x + vec2.x - offDir * num14 * xscale), (float)(y + vec2.y + armOffY * yscale), _sprite.xscale, _sprite.yscale, true);
                                        _spriteArms.depth = this.depth + -10;
                                    }
                                    if (holdObject == null || !holdObject.hideLeftWing)
                                    {
                                        _spriteArms.imageIndex = 9;
                                        Graphics.Draw(_spriteArms, _spriteArms.imageIndex + 5 + (int)Math.Round(num13 * 2f), (float)(x + vec2.x + 2 * offDir * xscale), (float)(y + vec2.y + armOffY * yscale), -_sprite.xscale, _sprite.yscale, true);
                                        _spriteArms.depth = this.depth + 11;
                                    }
                                }
                                else if (holdObject == null || !holdObject.hideRightWing)
                                    Graphics.Draw(_spriteArms, _sprite.imageIndex, armPosition.x, armPosition.y, _sprite.xscale, _sprite.yscale);
                            }
                            else
                            {
                                _bionicArm.angle = 0f;
                                _bionicArm.flipH = _sprite.flipH;
                                Graphics.Draw(_bionicArm, _sprite.imageIndex + num12, armPosition.x, armPosition.y, _sprite.xscale, _sprite.yscale);
                            }
                        }
                    }
                }
                if (Network.isActive && !_renderingDuck)
                    DrawConnectionIndicators();
                Sprite graphic = this.graphic;
                this.graphic = null;
                base.Draw();
                this.graphic = graphic;
                if (!enteringWalldoor)
                    return;
                this.depth = depth;
            }
        }

        public void UpdateConnectionIndicators()
        {
            _indicators ??= new ConnectionIndicators { duck = this };
            _indicators.Update();
        }

        public void DrawConnectionIndicators()
        {
            _indicators ??= new ConnectionIndicators { duck = this };
            _indicators.Draw();
        }

        //private void DrawAIPath()
        //{
        //    if (this.ai == null)
        //        return;
        //    this.ai.Draw();
        //}

        [Flags]
        private enum ConnectionTrouble
        {
            Lag = 1,
            Loss = 2,
            AFK = 4,
            Chatting = 8,
            Disconnection = 16, // 0x00000010
            Spectator = 32, // 0x00000020
            Minimized = 64, // 0x00000040
            Paused = 128, // 0x00000080
            DevConsole = 256, // 0x00000100
        }

        private class ConnectionIndicators
        {
            public Duck duck;
            private List<Indicator> _indicators;
            private int numProblems;
            private static Sprite _rainbowGradient;

            public ConnectionIndicators()
            {
                List<Indicator> indicatorList = new List<Indicator>();
                SpriteMap pSprite1 = new SpriteMap("lagturtle", 16, 16, 3)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite1)
                {
                    problem = ConnectionTrouble.AFK
                });
                SpriteMap pSprite2 = new SpriteMap("lagturtle", 16, 16, 4)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite2)
                {
                    problem = ConnectionTrouble.Chatting
                });
                SpriteMap pSprite3 = new SpriteMap("lagturtle", 16, 16, 2)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite3)
                {
                    problem = ConnectionTrouble.Disconnection
                });
                SpriteMap pSprite4 = new SpriteMap("lagturtle", 16, 16, 0)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite4)
                {
                    problem = ConnectionTrouble.Lag
                });
                SpriteMap pSprite5 = new SpriteMap("lagturtle", 16, 16, 1)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite5)
                {
                    problem = ConnectionTrouble.Loss
                });
                SpriteMap pSprite6 = new SpriteMap("lagturtle", 16, 16, 8)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite6)
                {
                    problem = ConnectionTrouble.Minimized
                });
                SpriteMap pSprite7 = new SpriteMap("lagturtle", 16, 16, 7)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite7)
                {
                    problem = ConnectionTrouble.Spectator
                });
                SpriteMap pSprite8 = new SpriteMap("lagturtle", 16, 16, 9)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite8)
                {
                    problem = ConnectionTrouble.Paused
                });
                SpriteMap pSprite9 = new SpriteMap("lagturtle", 16, 16, 10)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Indicator(pSprite9)
                {
                    problem = ConnectionTrouble.DevConsole
                });
                _indicators = indicatorList;
                // ISSUE: explicit constructor call
                // base.\u002Ector(); wtf weird
                foreach (Indicator indicator in _indicators)
                    indicator.owner = this;
            }

            public void Update()
            {
                numProblems = 0;
                foreach (Indicator indicator in _indicators)
                {
                    indicator.Update();
                    if (indicator.visible)
                        ++numProblems;
                }
            }

            public void Draw()
            {
                if (duck.position.x < -4000f)
                    return;
                _rainbowGradient ??= new Sprite("rainbowGradient");
                if (numProblems <= 0)
                    return;
                float num1 = 37f;
                float num2 = (numProblems - 1) * num1;
                Vec2 vec2_1 = new Vec2(-1000f, -1000f);
                Vec2 pPos = duck.cameraPosition + new Vec2(0f, 6f);
                float num3 = numProblems / 5f;
                int num4 = 0;
                float num5 = -20f;
                bool flag = false;
                Vec2 vec2_2 = Vec2.Zero;
                foreach (Indicator indicator in _indicators)
                {
                    if (!indicator.visible)
                        continue;

                    double deg = -num2 / 2f + num4 * num1;
                    float x = (float)-(Math.Sin(Maths.DegToRad((float)deg)) * num5);
                    float y = (float)Math.Cos(Maths.DegToRad((float)deg)) * num5;
                    Vec2 pOffset = new Vec2(x, y);
                    indicator.Draw(pPos, pOffset);
                    if (flag)
                    {
                        pOffset = pPos + pOffset;
                        Vec2 normalized = (pOffset - vec2_2).normalized;
                        Graphics.DrawTexturedLine(_rainbowGradient.texture, vec2_2 - normalized, pOffset + normalized, Color.White * num3, (0.3f + 0.6f * num3), (Depth)0.85f);
                    }
                    flag = true;
                    vec2_2 = new Vec2(pPos.x + x, pPos.y + y);
                    ++num4;
                }
            }

            private class Indicator
            {
                public ConnectionTrouble problem;
                public SpriteMap sprite;
                public float bloop;
                public ConnectionIndicators owner;
                public float wait;
                public float activeLerp;
                public bool _prevActive;
                private Vec2 drawPos = Vec2.Zero;

                public bool noWait => problem
                    is ConnectionTrouble.Chatting
                    or ConnectionTrouble.AFK
                    or ConnectionTrouble.Minimized;

                public bool active
                {
                    get
                    {
                        if (owner.duck.connection == null || owner.duck.profile == null)
                            return false;
                        if (problem == ConnectionTrouble.Chatting)
                            return owner.duck.get_chatting();
                        if (problem == ConnectionTrouble.AFK)
                            return owner.duck.afk;
                        if (problem == ConnectionTrouble.Disconnection)
                            return owner.duck.connection != DuckNetwork.localConnection && owner.duck.connection.isExperiencingConnectionTrouble;
                        if (problem == ConnectionTrouble.Lag)
                            return owner.duck.connection != DuckNetwork.localConnection && owner.duck.connection.manager.ping > 0.25f;
                        if (problem == ConnectionTrouble.Loss)
                            return owner.duck.connection != DuckNetwork.localConnection && owner.duck.connection.manager.accumulatedLoss > 10;
                        if (problem == ConnectionTrouble.Minimized)
                            return !owner.duck.profile.netData.Get("gameInFocus", true);
                        if (problem == ConnectionTrouble.Paused)
                            return owner.duck.profile.netData.Get("gamePaused", false);
                        return problem == ConnectionTrouble.DevConsole && owner.duck.profile.netData.Get("consoleOpen", false);
                    }
                }

                public bool visible => activeLerp > 0f;

                public void Update()
                {
                    bool active = this.active;
                    if (active != _prevActive)
                    {
                        _prevActive = active;
                        if (active)
                            bloop = 1f;

                        if (problem
                            is ConnectionTrouble.Chatting
                            or ConnectionTrouble.Minimized
                            or ConnectionTrouble.Paused
                            or ConnectionTrouble.DevConsole
                            or ConnectionTrouble.AFK)
                            SFX.Play("rainpop", 0.65f, Rando.Float(-0.1f, 0.1f));
                    }
                    if (!active)
                    {
                        wait = Lerp.Float(wait, 0f, 0.03f);
                        if (noWait)
                            wait = 0f;
                        if (wait <= 0f)
                        {
                            if (sprite.currentAnimation != "pop")
                                sprite.SetAnimation("pop");
                            else if (sprite.finished)
                            {
                                sprite.SetAnimation("idle");
                                activeLerp = 0f;
                            }
                        }
                    }
                    else
                    {
                        sprite.SetAnimation("idle");
                        wait = 1f;
                        activeLerp = 1f;
                    }
                    bloop = Lerp.FloatSmooth(bloop, 0f, 0.21f);
                    if (bloop >= 0.1f)
                        return;
                    bloop = 0f;
                }

                public void Draw(Vec2 pPos, Vec2 pOffset)
                {
                    if ((drawPos - pOffset).length > 16f)
                        drawPos = pOffset;
                    drawPos = Lerp.Vec2Smooth(drawPos, pOffset, 0.4f);
                    sprite.scale = new Vec2((float)(1 + bloop * 0.6f), (float)(1 + bloop * 0.35f));
                    sprite.depth = (Depth)0.9f;
                    Graphics.Draw(sprite, pPos.x + drawPos.x, pPos.y + drawPos.y);
                }

                public Indicator(SpriteMap pSprite)
                {
                    sprite = pSprite;
                    sprite.AddAnimation("idle", 1f, true, sprite.frame);
                    sprite.AddAnimation("pop", 0.4f, false, 5, 6);
                }
            }
        }
    }
}
