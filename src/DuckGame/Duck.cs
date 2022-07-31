// Decompiled with JetBrains decompiler
// Type: DuckGame.Duck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private bool _throwFondle = true;
        private int tryGrabFrames;
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
        private Rectangle _iconRect = new Rectangle(0.0f, 0.0f, 96f, 96f);
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
        private Duck.ConnectionIndicators _indicators;

        public override bool destroyed => this._destroyed || this.forceDead;

        public byte quackPitch
        {
            get => this._quackPitch;
            set => this._quackPitch = value;
        }

        public byte spriteFrame
        {
            get => this._sprite == null ? (byte)0 : (byte)this._sprite._frame;
            set
            {
                if (this._sprite == null)
                    return;
                this._sprite._frame = value;
            }
        }

        public byte spriteImageIndex
        {
            get => this._sprite == null ? (byte)0 : (byte)this._sprite._imageIndex;
            set
            {
                if (this._sprite == null)
                    return;
                this._sprite._imageIndex = value;
            }
        }

        public float spriteSpeed
        {
            get => this._sprite == null ? 0.0f : this._sprite._speed;
            set
            {
                if (this._sprite == null)
                    return;
                this._sprite._speed = value;
            }
        }

        public float spriteInc
        {
            get => this._sprite == null ? 0.0f : this._sprite._frameInc;
            set
            {
                if (this._sprite == null)
                    return;
                this._sprite._frameInc = value;
            }
        }

        public byte netAnimationIndex
        {
            get => this._sprite == null ? (byte)0 : (byte)this._sprite.animationIndex;
            set
            {
                if (this._sprite == null || this._sprite.animationIndex == value)
                    return;
                this._sprite.animationIndex = value;
            }
        }

        public Vec2 tounge
        {
            get => (!Network.isActive || this.isServerForObject) && this.inputProfile != null ? this.inputProfile.rightStick : this._tounge;
            set => this._tounge = value;
        }

        public byte netProfileIndex
        {
            get => this._netProfileIndex < 0 || this._netProfileIndex > DG.MaxPlayers - 1 ? (byte)0 : (byte)this._netProfileIndex;
            set
            {
                if (this._netProfileIndex == value)
                    return;
                this.AssignNetProfileIndex(value);
            }
        }

        private void AssignNetProfileIndex(byte pIndex)
        {
            DevConsole.Log(DCSection.General, "Assigning net profile index (" + pIndex.ToString() + "\\" + Profiles.all.Count<Profile>().ToString() + ")");
            this._netProfileIndex = pIndex;
            Profile profile = Profiles.all.ElementAt<Profile>(this._netProfileIndex);
            if (Network.isClient && Network.InLobby())
                (Level.current as TeamSelect2).OpenDoor(this._netProfileIndex, this);
            this.profile = profile;
            if (profile.team == null)
                profile.team = Teams.all[this._netProfileIndex];
            this.InitProfile();
            //this._netProfileInit = true;
            this._assignedIndex = true;
        }

        public Hat hat => this.GetEquipment(typeof(Hat)) as Hat;

        public InputProfile mindControl
        {
            get => this._mindControl;
            set
            {
                if (value == null && this._mindControl != null && this.profile != null && (this.profile.localPlayer || this.forceMindControl))
                {
                    if (this.holdObject != null)
                        Thing.Fondle(holdObject, DuckNetwork.localConnection);
                    foreach (Thing t in this._equipment)
                        Thing.Fondle(t, DuckNetwork.localConnection);
                    Thing.Fondle(_ragdollInstance, DuckNetwork.localConnection);
                    Thing.Fondle(_cookedInstance, DuckNetwork.localConnection);
                    Thing.Fondle(_trappedInstance, DuckNetwork.localConnection);
                }
                this._mindControl = value;
            }
        }

        public bool derpMindControl
        {
            get => this._derpMindControl;
            set => this._derpMindControl = value;
        }

        public DuckSkeleton skeleton
        {
            get
            {
                this.UpdateSkeleton();
                return this._skeleton;
            }
        }

        public bool dead
        {
            get => this.destroyed;
            set => this._destroyed = value;
        }

        public bool inNet => this._trapped != null;

        public Team team
        {
            get
            {
                if (this.profile == null)
                    return null;
                if (this._checkingTeam || this._converted == null)
                    return this.profile.team;
                this._checkingTeam = true;
                Team team = this._converted.team;
                this._checkingTeam = false;
                return team;
            }
        }

        public DuckPersona persona
        {
            get
            {
                if (this.profile == null)
                    return null;
                if (this._checkingPersona || this._converted == null)
                    return this.profile.persona;
                this._checkingPersona = true;
                DuckPersona persona = this._converted.persona;
                this._checkingPersona = false;
                return persona;
            }
        }

        public bool isGrabbedByMagnet
        {
            get => this._isGrabbedByMagnet;
            set
            {
                this._isGrabbedByMagnet = value;
                if (value || !this.profile.localPlayer)
                    return;
                this.angle = 0.0f;
                this.immobilized = false;
                this.gripped = false;
                this.enablePhysics = true;
                this.visible = true;
                this.SetCollisionMode("normal");
                if (this.holdObject != null)
                    Thing.Fondle(holdObject, DuckNetwork.localConnection);
                foreach (Thing t in this._equipment)
                    Thing.Fondle(t, DuckNetwork.localConnection);
                Thing.Fondle(_ragdollInstance, DuckNetwork.localConnection);
                Thing.Fondle(_cookedInstance, DuckNetwork.localConnection);
                Thing.Fondle(_trappedInstance, DuckNetwork.localConnection);
            }
        }

        public override bool CanBeControlled() => this.mindControl != null || this.isGrabbedByMagnet || this.listening || this.dead || this.wasSuperFondled > 0;

        public void CancelFlapping() => this._hovering = false;

        public bool IsNetworkDuck() => !this.isRockThrowDuck && Network.isClient;

        public bool closingEyes
        {
            get => this._closingEyes;
            set => this._closingEyes = value;
        }

        public bool canFire
        {
            get => this._canFire;
            set => this._canFire = value;
        }

        public bool CanMove() => (this.holdObject == null || !this.holdObject.immobilizeOwner) && !this.immobilized && crippleTimer <= 0.0 && !this.inNet && !this.swinging && !this.dead && !this.listening && Level.current.simulatePhysics && !this._closingEyes && this.ragdoll == null;

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
            get => this._isGhost;
            set => this._isGhost = value;
        }

        public bool eyesClosed
        {
            get => this._eyesClosed;
            set => this._eyesClosed = value;
        }

        public bool remoteControl
        {
            get => this._remoteControl;
            set => this._remoteControl = value;
        }

        public override bool action => !this._resetAction && (this.CanMove() || this.ragdoll != null && !this.dead && this.fancyShoes || this._remoteControl || this.inPipe) && this.inputProfile.Down("SHOOT") && this._canFire;

        public Vec2 armPosition => this.position + this.armOffset;

        public Vec2 armOffset
        {
            get
            {
                Vec2 vec2 = Vec2.Zero;
                if (this.gun != null)
                    vec2 = -this.gun.barrelVector * this.kick;
                return new Vec2(this.armOffX * this.xscale + vec2.x, this.armOffY * this.yscale + vec2.y);
            }
        }

        public Vec2 armPositionNoKick => this.position + this.armOffsetNoKick;

        public Vec2 armOffsetNoKick => new Vec2(this.armOffX * this.xscale, this.armOffY * this.yscale);

        public Vec2 HoldOffset(Vec2 pos)
        {
            Vec2 vec2 = pos + new Vec2(this.holdOffX, this.holdOffY);
            vec2 = vec2.Rotate(this.holdAngle, new Vec2(0.0f, 0.0f));
            return this.position + (vec2 + this.armOffset);
        }

        public float holdAngle => this.holdObject != null ? this.holdObject.handAngle + this.holdAngleOff : this.holdAngleOff;

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
            if (this.ragdoll != null)
            {
                if (this.ragdoll.part1 != null && this.ragdoll.part1.owner is Duck)
                    return this.ragdoll.part1.owner as Duck;
                if (this.ragdoll.part2 != null && this.ragdoll.part2.owner is Duck)
                    return this.ragdoll.part1.owner as Duck;
                if (this.ragdoll.part3 != null && this.ragdoll.part3.owner is Duck)
                    return this.ragdoll.part1.owner as Duck;
            }
            else if (this._trapped != null && this._trapped.owner is Duck)
                return this._trapped.owner as Duck;
            return null;
        }

        public bool IsOwnedBy(Thing pThing) => pThing != null && (this.owner == pThing || this._trapped != null && this._trapped.owner == pThing || this.ragdoll != null && (this.ragdoll.part1 != null && this.ragdoll.part1.owner == pThing || this.ragdoll.part2 != null && this.ragdoll.part2.owner == pThing || this.ragdoll.part3 != null && this.ragdoll.part3.owner == pThing));

        public bool HoldingTaped(Holdable pObject) => this.holdObject is TapedGun && ((this.holdObject as TapedGun).gun1 == pObject || (this.holdObject as TapedGun).gun2 == pObject);

        public bool Held(Holdable pObject, bool ignorePowerHolster = false) => this.holdObject == pObject || this.holdObject is TapedGun && ((this.holdObject as TapedGun).gun1 == pObject || (this.holdObject as TapedGun).gun2 == pObject) || !ignorePowerHolster && this.GetEquipment(typeof(Holster)) is Holster equipment && equipment is PowerHolster && pObject == equipment.containedObject;

        public override Holdable holdObject
        {
            get => base.holdObject;
            set
            {
                if (value != this.holdObject && this.holdObject != null)
                {
                    if (this.holdObject.isServerForObject && this.holdObject.owner == this)
                        this.ThrowItem();
                    this._lastHoldItem = this.holdObject;
                    this._timeSinceThrow = 0;
                }
                base.holdObject = value;
            }
        }

        public int lives
        {
            get => this._lives;
            set => this._lives = value;
        }

        public float holdingWeight => this.holdObject == null ? 0f : this.holdObject.weight;

        public override float weight
        {
            get => (_weight + this.holdingWeight * 0.4f + (this.sliding || this.crouch ? 16f : 0f));
            set => this._weight = value;
        }

        public float runMax
        {
            get => this._runMax;
            set => this._runMax = value;
        }

        public bool moveLock
        {
            get => this._moveLock;
            set => this._moveLock = value;
        }

        public InputProfile inputProfile
        {
            get
            {
                if (this.wallDoorAI != null)
                    return wallDoorAI;
                if (this._mindControl != null)
                    return this._mindControl;
                if (this._virtualInput != null)
                    return this._virtualInput;
                return this._profile != null ? this._profile.inputProfile : this._inputProfile;
            }
        }

        public Profile profile
        {
            get => this._profile;
            set
            {
                this._profile = value;
                if (!Network.isActive || this._profile == null)
                    return;
                if (this._profile.localPlayer)
                {
                    Thing.Fondle(this, DuckNetwork.localConnection);
                }
                else
                {
                    if (this._profile.connection == null)
                        return;
                    this.connection = this._profile.connection;
                }
            }
        }

        public override NetworkConnection connection
        {
            get => base.connection;
            set
            {
                if (Network.isServer && this.connection != null && this.connection.status == ConnectionStatus.Disconnected && Network.InGameLevel())
                    this.Kill(new DTDisconnect(this));
                if (this._profile != null)
                {
                    if (this._profile.localPlayer && !this.CanBeControlled())
                    {
                        if (this.connection == DuckNetwork.localConnection)
                            return;
                        base.connection = DuckNetwork.localConnection;
                        this.authority += 5;
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
            get => this._resetAction;
            set => this._resetAction = value;
        }

        public virtual void InitProfile()
        {
            this._profile.duck = this;
            this._sprite = this.profile.persona.sprite.CloneMap();
            this._spriteArms = this.profile.persona.armSprite.CloneMap();
            this._spriteQuack = this.profile.persona.quackSprite.CloneMap();
            this._spriteControlled = this.profile.persona.controlledSprite.CloneMap();
            this._swirl = new Sprite("swirl");
            this._swirl.CenterOrigin();
            this._swirl.scale = new Vec2(0.75f, 0.75f);
            this._bionicArm = new SpriteMap("bionicArm", 32, 32);
            this._bionicArm.CenterOrigin();
            if (!this.didHat && (Network.isServer || RockScoreboard.initializingDucks))
            {
                if (this.profile.team != null && this.profile.team.hasHat)
                {
                    Hat e = new TeamHat(0.0f, 0.0f, this.team, this.profile);
                    if (RockScoreboard.initializingDucks)
                        e.IgnoreNetworkSync();
                    Level.Add(e);
                    this.Equip(e, false, true);
                }
                this.didHat = true;
            }
            this.graphic = _sprite;
        }

        public Ragdoll _ragdollInstance
        {
            get => this._ins;
            set => this._ins = value;
        }

        public Duck(float xval, float yval, Profile pro)
          : base(xval, yval)
        {
            this._featherVolume = new FeatherVolume(this)
            {
                anchor = (Anchor)this
            };
            this.duck = true;
            this.profile = pro;
            if (this._profile == null)
                this._profile = Profiles.EnvironmentProfile;
            if (this.profile != null)
                this.InitProfile();
            this.centerx = 16f;
            this.centery = 16f;
            this.friction = 0.25f;
            this.vMax = 8f;
            this.hMax = 12f;
            this._lagTurtle = new SpriteMap("lagturtle", 16, 16);
            this._lagTurtle.CenterOrigin();
            this.physicsMaterial = PhysicsMaterial.Duck;
            this.collideSounds.Add("land", ImpactedFrom.Bottom);
            this._impactThreshold = 1.3f;
            this._impactVolume = 0.4f;
            this.SetCollisionMode("normal");
            this._shield = new Sprite("sheeld");
            this._shield.CenterOrigin();
            this.flammable = 1f;
            this.thickness = 0.5f;
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
            foreach (Thing thing in this._equipment.ToList<Equipment>())
                Level.Remove(thing);
        }

        public float duckWidth
        {
            get => this._duckWidth;
            set
            {
                this._duckWidth = value;
                this.xscale = this._duckWidth;
            }
        }

        public float duckHeight
        {
            get => this._duckHeight;
            set
            {
                this._duckHeight = value;
                this.yscale = this._duckHeight;
            }
        }

        public float duckSize
        {
            get => this._duckHeight;
            set => this.duckWidth = this.duckHeight = value;
        }

        public void SetCollisionMode(string mode)
        {
            //this._collisionMode = mode;
            if (this.offDir > 0)
                this._featherVolume.anchor.offset = new Vec2(0.0f, 0.0f);
            else
                this._featherVolume.anchor.offset = new Vec2(1f, 0.0f);
            if (mode == "normal")
            {
                this.collisionSize = new Vec2(8f * this.duckWidth, 22f * this.duckHeight);
                this.collisionOffset = new Vec2(-4f * this.duckWidth, -7f * this.duckHeight);
                this._featherVolume.collisionSize = new Vec2(12f * this.duckWidth, 26f * this.duckHeight);
                this._featherVolume.collisionOffset = new Vec2(-6f * this.duckWidth, -9f * this.duckHeight);
            }
            else if (mode == "slide")
            {
                this.collisionSize = new Vec2(8f * this.duckWidth, 11f * this.duckHeight);
                this.collisionOffset = new Vec2(-4f * this.duckWidth, 4f * this.duckHeight);
                if (this.offDir > 0)
                {
                    this._featherVolume.collisionSize = new Vec2(25f * this.duckWidth, 13f * this.duckHeight);
                    this._featherVolume.collisionOffset = new Vec2(-13f * this.duckWidth, 3f * this.duckHeight);
                }
                else
                {
                    this._featherVolume.collisionSize = new Vec2(25f * this.duckWidth, 13f * this.duckHeight);
                    this._featherVolume.collisionOffset = new Vec2(-12f * this.duckWidth, 3f * this.duckHeight);
                }
            }
            else if (mode == "crouch")
            {
                this.collisionSize = new Vec2(8f * this.duckWidth, 16f * this.duckHeight);
                this.collisionOffset = new Vec2(-4f * this.duckWidth, -1f * this.duckHeight);
                this._featherVolume.collisionSize = new Vec2(12f * this.duckWidth, 20f * this.duckHeight);
                this._featherVolume.collisionOffset = new Vec2(-6f * this.duckWidth, -3f * this.duckHeight);
            }
            else if (mode == "netted")
            {
                this.collisionSize = new Vec2(16f * this.duckWidth, 17f * this.duckHeight);
                this.collisionOffset = new Vec2(-8f * this.duckWidth, -9f * this.duckHeight);
                this._featherVolume.collisionSize = new Vec2(18f * this.duckWidth, 19f * this.duckHeight);
                this._featherVolume.collisionOffset = new Vec2(-9f * this.duckWidth, -10f * this.duckHeight);
            }
            if (this.ragdoll == null)
                return;
            this._featherVolume.collisionSize = new Vec2(12f * this.duckWidth, 12f * this.duckHeight);
            this._featherVolume.collisionOffset = new Vec2(-6f * this.duckWidth, -6f * this.duckHeight);
        }

        public void KnockOffEquipment(Equipment e, bool ting = true, Bullet b = null)
        {
            if (!this._equipment.Contains(e))
                return;
            if (this.isServerForObject)
                RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None));
            e.UnEquip();
            if (ting && !Network.isActive)
                SFX.Play("ting2");
            this._equipment.Remove(e);
            e.Destroy(new DTImpact(null));
            e.solid = false;
            if (b != null)
            {
                e.hSpeed = b.travelDirNormalized.x;
                e.vSpeed = -2f;
                if (this.isServerForObject)
                {
                    this.hSpeed += b.travelDirNormalized.x * (b.ammo.impactPower + 1f);
                    this.vSpeed += b.travelDirNormalized.y * (b.ammo.impactPower + 1f);
                    --this.vSpeed;
                }
            }
            else
            {
                e.hSpeed = -this.offDir * 2f;
                e.vSpeed = -2f;
            }
            this.ReturnItemToWorld(e);
        }

        public override void ReturnItemToWorld(Thing t)
        {
            Vec2 position = this.position;
            if (this.sliding)
                position.y += 10f;
            else if (this.crouch)
                position.y += 8f;
            Block block1 = Level.CheckLine<Block>(position, position + new Vec2(16f, 0.0f));
            if (block1 != null && block1.solid && (double)t.right > (double)block1.left)
                t.right = block1.left;
            Block block2 = Level.CheckLine<Block>(position, position - new Vec2(16f, 0.0f));
            if (block2 != null && block2.solid && (double)t.left < (double)block2.right)
                t.left = block2.right;
            Block block3 = Level.CheckLine<Block>(position, position + new Vec2(0.0f, -16f));
            if (block3 != null && block3.solid && (double)t.top < (double)block3.bottom)
                t.top = block3.bottom;
            Block block4 = Level.CheckLine<Block>(position, position + new Vec2(0.0f, 16f));
            if (block4 == null || !block4.solid || (double)t.bottom <= (double)block4.top)
                return;
            t.bottom = block4.top;
        }

        public void Unequip(Equipment e, bool forceNetwork = false)
        {
            if (!(this.isServerForObject | forceNetwork) || e == null || !this._equipment.Contains(e))
                return;
            this.Fondle(e);
            e.UnEquip();
            this._equipment.Remove(e);
            this.ReturnItemToWorld(e);
        }

        public bool HasJumpModEquipment()
        {
            foreach (Equipment equipment in this._equipment)
            {
                if (equipment.jumpMod)
                    return true;
            }
            return false;
        }

        public Equipment GetEquipment(System.Type t)
        {
            foreach (Equipment equipment in this._equipment)
            {
                if (equipment.GetAllTypes().Contains(t))
                    return equipment;
            }
            return null;
        }

        public void Equip(Equipment e, bool makeSound = true, bool forceNetwork = false)
        {
            if (!(this.isServerForObject | forceNetwork))
                return;
            List<System.Type> allTypesFiltered = e.GetAllTypesFiltered(typeof(Equipment));
            if (allTypesFiltered.Contains(typeof(ITeleport)))
                allTypesFiltered.Remove(typeof(ITeleport));
            foreach (System.Type t in allTypesFiltered)
            {
                if (!t.IsInterface)
                {
                    Equipment equipment = this.GetEquipment(t);
                    if (equipment == null && e.GetType() == typeof(Jetpack))
                        equipment = this.GetEquipment(typeof(Grapple));
                    else if (equipment == null && e.GetType() == typeof(Grapple))
                        equipment = this.GetEquipment(typeof(Jetpack));
                    if (equipment != null)
                    {
                        this._equipment.Remove(equipment);
                        this.Fondle(equipment);
                        equipment.vSpeed = -2f;
                        equipment.hSpeed = offDir * 3f;
                        equipment.UnEquip();
                        this.ReturnItemToWorld(equipment);
                    }
                }
            }
            if (e is TeamHat)
            {
                TeamHat teamHat = e as TeamHat;
                if (this.profile != null && teamHat.team != this.profile.team && !teamHat.hasBeenStolen)
                {
                    ++Global.data.hatsStolen;
                    teamHat.hasBeenStolen = true;
                }
            }
            this.Fondle(e);
            this._equipment.Add(e);
            e.Equip(this);
            if (!makeSound)
                e._prevEquipped = true;
            else
                e.equipIndex += 1;
        }

        public List<Equipment> GetArmor()
        {
            List<Equipment> armor = new List<Equipment>();
            foreach (Equipment equipment in this._equipment)
            {
                if (equipment.isArmor)
                    armor.Add(equipment);
            }
            return armor;
        }

        public bool ExtendsTo(Thing t)
        {
            if (this.ragdoll == null)
                return false;
            return t == this.ragdoll.part1 || t == this.ragdoll.part2 || t == this.ragdoll.part3;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._trapped != null || this._trappedInstance != null && this._trappedInstance.visible || this.ragdoll != null || this._ragdollInstance != null && this._ragdollInstance.visible)
                return false;
            if (bullet.isLocal && !this.HitArmor(bullet, hitPos))
            {
                this.Kill(new DTShot(bullet));
                SFX.Play("thwip", pitch: Rando.Float(-0.1f, 0.1f));
            }
            return base.Hit(bullet, hitPos);
        }

        public bool HitArmor(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
            {
                foreach (Equipment t in this._equipment)
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

        public override bool Destroy(DestroyType type = null) => this.Kill(type);

        public void AddCoolness(int amount)
        {
            if (Highlights.highlightRatingMultiplier == 0.0)
                return;
            this.profile.stats.coolness += amount;
            //this._coolnessThisFrame += amount;
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogCoolness(Math.Abs(amount));
        }

        public bool WillAcceptLifeChange(byte pLifeChange)
        {
            if (lastAppliedLifeChange >= pLifeChange && Math.Abs(lastAppliedLifeChange - pLifeChange) <= 20)
                return false;
            this.lastAppliedLifeChange = pLifeChange;
            return true;
        }

        public virtual bool Kill(DestroyType type = null)
        {
            if (this._killed || (!this.isKillMessage && this.invincible && !(type is DTFall) && !(type is DTPop)))
            {
                return true;
            }
            if (this.KillOverride != null && this.KillOverride(this))
            {
                return false;
            }
            this.forceDead = true;
            this._killed = true;
            RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Short, RumbleType.Gameplay));
            int xpLogged = 10;
            if (type is DTFall)
            {
                Vec2 pos = this.GetEdgePos();
                Vec2 dir = (pos - this.GetPos()).normalized;
                for (int i = 0; i < 8; i++)
                {
                    Feather feather = Feather.New(pos.x - dir.x * 16f, pos.y - dir.y * 16f, this.persona);
                    feather.hSpeed += dir.x * 1f;
                    feather.vSpeed += dir.y * 1f;
                    Level.Add(feather);
                }
            }
            if (!GameMode.firstDead)
            {
                Party.AddDrink(this.profile, 1);
                if (Rando.Float(1f) > 0.8f)
                {
                    Party.AddRandomPerk(this.profile);
                }
                GameMode.firstDead = true;
            }
            if (Rando.Float(1f) > 0.97f)
            {
                Party.AddRandomPerk(this.profile);
                Party.AddDrink(this.profile, 1);
            }
            if (Recorder.currentRecording != null)
            {
                Recorder.currentRecording.LogDeath();
            }
            base._destroyed = true;
            if (this._isGhost)
            {
                return false;
            }
            this.swinging = false;
            Holster h = this.GetEquipment(typeof(Holster)) as Holster;
            foreach (Equipment e in this._equipment)
            {
                if (e != null)
                {
                    e.sleeping = false;
                    e.owner = null;
                    if (!this.isKillMessage)
                    {
                        Thing.ExtraFondle(e, DuckNetwork.localConnection);
                    }
                    e.hSpeed = this.hSpeed - (1f + NetRand.Float(2f));
                    e.vSpeed = this.vSpeed - NetRand.Float(1.5f);
                    this.ReturnItemToWorld(e);
                    e.UnEquip();
                }
            }
            this._equipment.Clear();
            if (TeamSelect2.QUACK3 && h != null)
            {
                this.Equip(h, false, false);
            }
            Profile killedBy = type.responsibleProfile;
            bool wasTrapped = false;
            if (this._trapped != null)
            {
                if (type is DTFall || type is DTImpale)
                {
                    killedBy = this.trappedBy;
                    Duck d = this._trapped.prevOwner as Duck;
                    if (d != null)
                    {
                        d.AddCoolness(1);
                    }
                }
                if ((type is DTFall || type is DTImpale) && this.trappedBy != null && this.trappedBy.localPlayer)
                {
                    Global.data.nettedDuckTossKills += 1;
                }
                if (!this.killingNet)
                {
                    this.killingNet = true;
                    this._trapped.Destroy(type);
                }
                wasTrapped = true;
            }
            if (type is DTIncinerate)
            {
                xpLogged -= 3;
            }
            if (killedBy != null && killedBy.localPlayer)
            {
                this.killedByProfile = killedBy;
            }
            this.OnKill(type);
            Holdable prevHold = this.holdObject;
            if (!this.isKillMessage)
            {
                this.ThrowItem(false);
                if (prevHold != null)
                {
                    prevHold.hSpeed *= 0.3f;
                    if (type is DTImpale)
                    {
                        prevHold.vSpeed = (prevHold.hSpeed = 0f);
                    }
                    if (Network.isActive)
                    {
                        Thing.AuthorityFondle(prevHold, DuckNetwork.localConnection, 5);
                    }
                }
            }
            else if (this.profile != null && this.profile.connection == DuckNetwork.localConnection)
            {
                this.ThrowItem(false);
            }
            base.depth = 0.3f;
            if (killedBy != null)
            {
                if (killedBy == this.profile)
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
            if (Level.current is TeamSelect2 && this.isKillMessage)
            {
                ProfileBox2 box = Level.CheckPoint<ProfileBox2>(this.cameraPosition);
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
                Event.Log(new KillEvent(killedBy, this.profile, weapon));
                this.profile.stats.LogKill(killedBy);
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
                    if (killedBy == this.profile)
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
                        Party.AddDrink(this.profile, 2);
                        if (Rando.Float(1f) > 0.9f)
                        {
                            Party.AddRandomPerk(this.profile);
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
                        if ((DateTime.Now - killedBy.stats.lastKillTime).TotalSeconds < 2.0)
                        {
                            yourCoolness++;
                        }
                        if (bullet != null && Math.Abs(bullet.travelDirNormalized.y) > 0.3f)
                        {
                            yourCoolness++;
                        }
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
                        if (Math.Abs(killedBy.duck.hSpeed) + Math.Abs(killedBy.duck.vSpeed) + Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed) > 20f)
                        {
                            yourCoolness++;
                        }
                        if (this._holdingAtDisarm != null && this._disarmedBy == killedBy && (DateTime.Now - this._disarmedAt).TotalSeconds < 3.0)
                        {
                            if (killedBy.duck.holdObject == this._holdingAtDisarm)
                            {
                                yourCoolness += 4;
                                myCoolness -= 2;
                            }
                            else
                            {
                                yourCoolness++;
                            }
                        }
                        if (killedBy.duck.dead)
                        {
                            yourCoolness++;
                            killedBy.stats.killsFromTheGrave++;
                        }
                        if (type is DTShot && prevHold == null)
                        {
                            killedBy.stats.unarmedDucksShot++;
                        }
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
                            if (weapon == typeof(SledgeHammer) || weapon == typeof(DuelingPistol))
                            {
                                yourCoolness++;
                            }
                            if (weaponThing is Sword && weaponThing.owner != null && (weaponThing as Sword).jabStance)
                            {
                                yourCoolness++;
                            }
                        }
                        if (wasTrapped && type is DTFall)
                        {
                            yourCoolness++;
                        }
                        if (type is DTCrush)
                        {
                            if (weaponThing is PhysicsObject)
                            {
                                double totalSeconds = (DateTime.Now - (weaponThing as PhysicsObject).lastGrounded).TotalSeconds;
                                yourCoolness += 1 + (int)Math.Floor((DateTime.Now - (weaponThing as PhysicsObject).lastGrounded).TotalSeconds * 6.0);
                                if (Recorder.currentRecording != null)
                                {
                                    Recorder.currentRecording.LogAction(14);
                                }
                                Party.AddDrink(this.profile, 1);
                                if (Rando.Float(1f) > 0.8f)
                                {
                                    Party.AddRandomPerk(this.profile);
                                }
                            }
                            else
                            {
                                yourCoolness++;
                            }
                        }
                    }
                    if (killedBy.duck.team == this.team && killedBy != this.profile)
                    {
                        yourCoolness -= 2;
                        Party.AddDrink(killedBy, 1);
                    }
                    if ((DateTime.Now - this._timeSinceDuckLayedToRest).TotalSeconds < 3.0)
                    {
                        yourCoolness--;
                    }
                    if ((DateTime.Now - this._timeSinceFuneralPerformed).TotalSeconds < 3.0)
                    {
                        yourCoolness -= 2;
                    }
                }
                if (this.controlledBy != null && this.controlledBy.profile != null)
                {
                    this.controlledBy.profile.stats.coolness += Math.Abs(myCoolness);
                    if (myCoolness > 0)
                    {
                        myCoolness = 0;
                    }
                }
                yourCoolness++;
                myCoolness--;
                if (killedBy != null && killedBy.duck != null)
                {
                    yourCoolness *= (int)Math.Ceiling((double)(1f + killedBy.duck.killMultiplier));
                    killedBy.duck.AddCoolness(yourCoolness);
                }
                this.AddCoolness(myCoolness);
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
                        if (killedBy.team != this.profile.team)
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
                                    _duck = killedBy.duck
                                };
                                plus.anchor = killedBy.duck;
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
                ProfileStats stats3 = this.profile.stats;
                int num = stats3.timesKilled;
                stats3.timesKilled = num + 1;
            }
            if (this.profile.connection == DuckNetwork.localConnection)
            {
                DuckNetwork.deaths++;
            }
            if (!this.isKillMessage)
            {
                if (this.profile.connection != DuckNetwork.localConnection)
                {
                    DuckNetwork.kills++;
                }
                if (TeamSelect2.Enabled("CORPSEBLOW", false))
                {
                    Grenade grenade = new Grenade(base.x, base.y)
                    {
                        hSpeed = this.hSpeed + Rando.Float(-2f, 2f),
                        vSpeed = this.vSpeed - Rando.Float(1f, 2.5f)
                    };
                    Level.Add(grenade);
                    grenade.PressAction();
                }
                Thing.SuperFondle(this, DuckNetwork.localConnection);
                if (this._trappedInstance != null)
                {
                    Thing.SuperFondle(this._trappedInstance, DuckNetwork.localConnection);
                }
                if (this.holdObject != null)
                {
                    Thing.SuperFondle(this.holdObject, DuckNetwork.localConnection);
                }
                if (base.y < -999f)
                {
                    Vec2 pos2 = this.position;
                    this.position = this._lastGoodPosition;
                    this.GoRagdoll();
                    this.position = pos2;
                }
                else
                {
                    this.GoRagdoll();
                }
            }
            if (Network.isActive && this.ragdoll != null && !this.isKillMessage)
            {
                Thing.SuperFondle(this.ragdoll, DuckNetwork.localConnection);
            }
            if (Network.isActive && !this.isKillMessage)
            {
                this.lastAppliedLifeChange += 1;
                Send.Message(new NMKillDuck(this.profile.networkIndex, type is DTCrush, type is DTIncinerate, type is DTFall, this.lastAppliedLifeChange));
            }
            if (!(this is TargetDuck))
            {
                Global.Kill(this, type);
            }
            return true;
        }

        public override void Zap(Thing zapper)
        {
            this.GoRagdoll();
            if (this.ragdoll != null)
                this.ragdoll.Zap(zapper);
            base.Zap(zapper);
        }

        public override void Removed()
        {
            if (Network.isServer)
            {
                if (this._ragdollInstance != null)
                {
                    Thing.Fondle(_ragdollInstance, DuckNetwork.localConnection);
                    Level.Remove(_ragdollInstance);
                }
                if (this._trappedInstance != null)
                {
                    Thing.Fondle(_trappedInstance, DuckNetwork.localConnection);
                    Level.Remove(_trappedInstance);
                }
                if (this._cookedInstance != null)
                {
                    Thing.Fondle(_cookedInstance, DuckNetwork.localConnection);
                    Level.Remove(_cookedInstance);
                }
            }
            base.Removed();
        }

        public void Disappear()
        {
            if (this.ragdoll != null)
            {
                this.position = this.ragdoll.position;
                if (Network.isActive)
                    this.ragdoll.Unragdoll();
                else
                    Level.Remove(ragdoll);
                this.vSpeed = -2f;
            }
            this.OnTeleport();
            this.y += 9999f;
        }

        public void Cook()
        {
            if (this._cooked != null)
                return;
            if (this.ragdoll != null)
            {
                this.position = this.ragdoll.position;
                if (Network.isActive)
                    this.ragdoll.Unragdoll();
                else
                    Level.Remove(ragdoll);
                this.vSpeed = -2f;
            }
            if (Network.isActive)
            {
                this._cooked = this._cookedInstance;
                if (this._cookedInstance != null)
                {
                    this._cookedInstance.active = true;
                    this._cookedInstance.visible = true;
                    this._cookedInstance.solid = true;
                    this._cookedInstance.enablePhysics = true;
                    this._cookedInstance._sleeping = false;
                    this._cookedInstance.x = this.x;
                    this._cookedInstance.y = this.y;
                    this._cookedInstance.owner = null;
                    Thing.ExtraFondle(_cookedInstance, DuckNetwork.localConnection);
                    this.ReturnItemToWorld(_cooked);
                    this._cooked.vSpeed = this.vSpeed;
                    this._cooked.hSpeed = this.hSpeed;
                }
            }
            else
            {
                this._cooked = new CookedDuck(this.x, this.y);
                this.ReturnItemToWorld(_cooked);
                this._cooked.vSpeed = this.vSpeed;
                this._cooked.hSpeed = this.hSpeed;
                Level.Add(_cooked);
            }
            this.OnTeleport();
            SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
            this.y -= 25000f;
        }

        public void OnKill(DestroyType type = null)
        {
            if (!(type is DTPop))
            {
                SFX.Play("death");
                SFX.Play("pierce");
            }
            for (int index = 0; index < 8; ++index)
                Level.Add(Feather.New(this.cameraPosition.x, this.cameraPosition.y, this.persona));
            if (!(Level.current is ChallengeLevel))
                Global.data.kills += 1;
            this._remoteControl = false;
            switch (type)
            {
                case DTShot dtShot:
                    if (dtShot.bullet != null)
                    {
                        this.hSpeed = dtShot.bullet.travelDirNormalized.x * (dtShot.bullet.ammo.impactPower + 1f);
                        this.vSpeed = dtShot.bullet.travelDirNormalized.y * (dtShot.bullet.ammo.impactPower + 1f);
                    }
                    this.vSpeed -= 3f;
                    break;
                case DTIncinerate _:
                    this.Cook();
                    break;
                case DTPop _:
                    this.Disappear();
                    break;
            }
        }

        public bool crouchLock
        {
            get => this._crouchLock;
            set => this._crouchLock = value;
        }

        public TrappedDuck _trapped
        {
            get => this._trappedProp;
            set => this._trappedProp = value;
        }

        public virtual void Netted(Net n)
        {
            if (Network.isActive && (this._trappedInstance == null || this._trappedInstance.visible) || this._trapped != null)
                return;
            if (Network.isActive)
            {
                this._trapped = this._trappedInstance;
                this._trappedInstance.active = true;
                this._trappedInstance.visible = true;
                this._trappedInstance.solid = true;
                this._trappedInstance.enablePhysics = true;
                this._trappedInstance.x = this.x;
                this._trappedInstance.y = this.y;
                this._trappedInstance.owner = null;
                this._trappedInstance.InitializeStuff();
                n.Fondle(_trappedInstance);
                n.Fondle(this);
                if (this._trappedInstance._duckOwner != null)
                    RumbleManager.AddRumbleEvent(this._trappedInstance._duckOwner.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            }
            else
            {
                RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                this._trapped = new TrappedDuck(this.x, this.y, this);
                Level.Add(_trapped);
            }
            this.ReturnItemToWorld(_trapped);
            this.OnTeleport();
            if (this.holdObject != null)
                n.Fondle(holdObject);
            this.ThrowItem(false);
            Level.Remove(n);
            ++this.profile.stats.timesNetted;
            this._trapped.clip.Add(this);
            this._trapped.clip.Add(n);
            this._trapped.hSpeed = this.hSpeed + n.hSpeed * 0.4f;
            this._trapped.vSpeed = (float)((double)this.vSpeed + (double)n.vSpeed - 1.0);
            if ((double)this._trapped.hSpeed > 6.0)
                this._trapped.hSpeed = 6f;
            if ((double)this._trapped.hSpeed < -6.0)
                this._trapped.hSpeed = -6f;
            if (n.onFire)
                this.Burn(n.position, n);
            if (n.responsibleProfile == null || n.responsibleProfile.duck == null)
                return;
            this.trappedBy = n.responsibleProfile;
            n.responsibleProfile.duck.AddCoolness(1);
            Event.Log(new NettedEvent(n.responsibleProfile, this.profile));
        }

        public void Breath()
        {
            Vec2 vec2 = this.Offset(new Vec2(6f, 0.0f));
            if (this.ragdoll != null && this.ragdoll.part1 != null)
                vec2 = this.ragdoll.part1.Offset(new Vec2(6f, 0.0f));
            else if (this._trapped != null)
                vec2 = this._trapped.Offset(new Vec2(8f, -2f));
            Level.Add(BreathSmoke.New(vec2.x, vec2.y));
            Level.Add(BreathSmoke.New(vec2.x, vec2.y));
        }

        private void UpdateQuack()
        {
            if (this.dead || this.inputProfile == null || Level.current == null)
                return;
            if (this.breath <= 0)
            {
                if (this.breath == 0 && Level.current.cold && !this.underwater)
                    this.Breath();
                this.breath = Rando.Int(70, 220);
            }
            --this.breath;
            if (this.inputProfile.Pressed("QUACK"))
            {
                float leftTrigger = this.inputProfile.leftTrigger;
                if (this.inputProfile.hasMotionAxis)
                    leftTrigger += this.inputProfile.motionAxis;
                Hat equipment = this.GetEquipment(typeof(Hat)) as Hat;
                if (equipment == null || equipment.quacks)
                {
                    if (Network.isActive)
                        this._netQuack.Play(pit: leftTrigger);
                    else if (equipment != null)
                        equipment.Quack(1f, leftTrigger);
                    else
                        this._netQuack.Play(pit: leftTrigger);
                }
                if (this.isServerForObject)
                    ++Global.data.quacks.valueInt;
                ++this.profile.stats.quacks;
                this.quack = 20;
            }
            if (!this.inputProfile.Down("QUACK"))
                this.quack = Maths.CountDown(this.quack, 1, 0);
            if (!this.inputProfile.Released("QUACK"))
                return;
            this.quack = 0;
        }

        public bool HasEquipment(Equipment t) => this.HasEquipment(t.GetType());

        public bool HasEquipment(System.Type t)
        {
            foreach (Thing thing in this._equipment)
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
            this.ReturnItemToWorld(h);
        }

        public override float holdWeightMultiplier
        {
            get
            {
                float val2 = 1f;
                if (this.holdObject != null)
                    val2 = this.holdObject.weightMultiplier;
                if (this.holstered != null)
                    val2 = Math.Min(this.holstered.weightMultiplier, val2);
                return val2;
            }
        }

        public override float holdWeightMultiplierSmall => this.holdObject != null ? this.holdObject.weightMultiplierSmall : 1f;

        public void ThrowItem(bool throwWithForce = true)
        {
            if (this.holdObject == null)
                return;
            if (this._throwFondle)
                this.Fondle(holdObject);
            this.ObjectThrown(this.holdObject);
            this.holdObject.hSpeed = 0.0f;
            this.holdObject.vSpeed = 0.0f;
            this.holdObject.clip.Add(this);
            this.holdObstructed = false;
            if (this.holdObject is Mine && !(this.holdObject as Mine).pin && (!this.crouch || !this.grounded))
                (this.holdObject as Mine).Arm();
            if (!this.crouch)
            {
                float num1 = 1f;
                float num2 = 1f;
                if (this.inputProfile.Down("LEFT") || this.inputProfile.Down("RIGHT"))
                    num1 = 2.5f;
                if ((double)num1 == 1.0 && this.inputProfile.Down("UP"))
                {
                    this.holdObject.vSpeed -= 5f * this.holdWeightMultiplier;
                }
                else
                {
                    float num3 = num1 * this.holdWeightMultiplier;
                    if (this.inputProfile.Down("UP"))
                        num2 = 2f;
                    float num4 = num2 * this.holdWeightMultiplier;
                    if (this.offDir > 0)
                        this.holdObject.hSpeed += 3f * num3;
                    else
                        this.holdObject.hSpeed -= 3f * num3;
                    if (this.reverseThrow)
                        this.holdObject.hSpeed = -this.holdObject.hSpeed;
                    this.holdObject.vSpeed -= 2f * num4;
                }
            }
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogAction(2);
            this.holdObject.hSpeed += 0.3f * offDir;
            this.holdObject.hSpeed *= this.holdObject.throwSpeedMultiplier;
            if (!throwWithForce)
                this.holdObject.hSpeed = this.holdObject.vSpeed = 0.0f;
            else if (Network.isActive)
            {
                if (this.isServerForObject)
                    this._netTinyMotion.Play();
            }
            else
                SFX.Play("tinyMotion");
            this._lastHoldItem = this.holdObject;
            this._timeSinceThrow = 0;
            this._holdObject = null;
        }

        public void GiveHoldable(Holdable h)
        {
            if (this.holdObject == h)
                return;
            if (this.holdObject != null)
                this.ThrowItem(false);
            if (h == null)
                return;
            if (this.profile.localPlayer)
            {
                if (h is RagdollPart)
                {
                    RagdollPart ragdollPart = h as RagdollPart;
                    if (ragdollPart.doll != null)
                    {
                        ragdollPart.doll.connection = this.connection;
                        Ragdoll doll = ragdollPart.doll;
                        doll.authority += 8;
                    }
                }
                else
                {
                    h.connection = this.connection;
                    Holdable holdable = h;
                    holdable.authority += 8;
                }
            }
            this.holdObject = h;
            this.holdObject.owner = this;
            this.holdObject.solid = false;
            h.hSpeed = 0.0f;
            h.vSpeed = 0.0f;
            h.enablePhysics = false;
            h._sleeping = false;
        }

        private void TryGrab()
        {
            foreach (Holdable h in (IEnumerable<Holdable>)Level.CheckCircleAll<Holdable>(new Vec2(this.x, this.y + 4f), 18f).OrderBy<Holdable, Holdable>(h => h, new CompareHoldablePriorities(this)))
            {
                if (h.owner == null && h.canPickUp && (h != this._lastHoldItem || this._timeSinceThrow >= 30) && h.active && h.visible && Level.CheckLine<Block>(this.position, h.position) == null)
                {
                    this.GiveHoldable(h);
                    if ((double)this.holdObject.weight > 5.0)
                    {
                        if ((double)Rando.Float(1f) < 0.5)
                            this.PlaySFX("liftBarrel", pitch: Rando.Float(-0.1f, 0.2f));
                        else
                            this.PlaySFX("liftBarrel2", pitch: Rando.Float(-0.1f, 0.2f));
                        this.quack = 10;
                    }
                    else if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            this._netTinyMotion.Play();
                    }
                    else
                        SFX.Play("tinyMotion");
                    if (this.holdObject.disarmedFrom != this && (DateTime.Now - this.holdObject.disarmTime).TotalSeconds < 0.5)
                        this.AddCoolness(2);
                    this.tryGrabFrames = 0;
                    break;
                }
            }
        }

        private void UpdateThrow()
        {
            if (!this.isServerForObject)
                return;
            bool flag1 = false;
            if (this.CanMove())
            {
                if (this.HasEquipment(typeof(Holster)) && this.inputProfile.Down("UP") && this.inputProfile.Pressed("GRAB") && (this.holdObject == null || this.holdObject.holsterable) && this.GetEquipment(typeof(Holster)) is Holster equipment && (!equipment.chained.value || equipment.containedObject == null))
                {
                    Holdable h = null;
                    bool flag2 = false;
                    if (equipment.containedObject != null)
                    {
                        h = equipment.containedObject;
                        equipment.SetContainedObject(null);
                        this.ObjectThrown(h);
                        flag2 = true;
                    }
                    if (this.holdObject != null)
                    {
                        if (this.holdObject is RagdollPart)
                        {
                            RagdollPart holdObject = this.holdObject as RagdollPart;
                            if (holdObject.doll != null && holdObject.doll.part3 != null)
                            {
                                this.holdObject.owner = null;
                                this.holdObject = holdObject.doll.part3;
                            }
                        }
                        this.holdObject.owner = this;
                        equipment.SetContainedObject(this.holdObject);
                        if (equipment.chained.value)
                        {
                            SFX.PlaySynchronized("equip");
                            for (int index = 0; index > 3; ++index)
                                Level.Add(SmallSmoke.New(this.holdObject.x + Rando.Float(-3f, 3f), this.holdObject.y + Rando.Float(-3f, 3f)));
                        }
                        this.holdObject = null;
                        flag2 = true;
                    }
                    if (h != null)
                    {
                        this.GiveHoldable(h);
                        if (Network.isActive)
                        {
                            if (this.isServerForObject)
                                this._netTinyMotion.Play();
                        }
                        else
                            SFX.Play("tinyMotion");
                    }
                    if (flag2)
                        return;
                }
                if (this.holdObject != null && this.inputProfile.Pressed("GRAB"))
                    this.doThrow = true;
                if (!this._isGhost && this.inputProfile.Pressed("GRAB") && this.holdObject == null)
                {
                    this.tryGrabFrames = 2;
                    this.TryGrab();
                }
            }
            if (flag1 || !this.doThrow || this.holdObject == null)
                return;
            Holdable holdObject1 = this.holdObject;
            this.doThrow = false;
            this.ThrowItem();
        }

        private void UpdateAnimation()
        {
            this._updatedAnimation = true;
            if (this._hovering)
            {
                ++this._flapFrame;
                if (this._flapFrame > 8)
                    this._flapFrame = 0;
            }
            this.UpdateCurrentAnimation();
        }

        private void UpdateCurrentAnimation()
        {
            if (this.dead && this._eyesClosed)
                this._sprite.currentAnimation = "dead";
            else if (this.inNet)
                this._sprite.currentAnimation = "netted";
            else if (this.listening)
                this._sprite.currentAnimation = "listening";
            else if (this.crouch)
            {
                this._sprite.currentAnimation = "crouch";
                if (!this.sliding)
                    return;
                this._sprite.currentAnimation = "groundSlide";
            }
            else if (this.grounded)
            {
                if ((double)this.hSpeed > 0.0 && !this._gripped)
                {
                    this._sprite.currentAnimation = "run";
                    if (this.strafing || Math.Sign(this.offDir) == Math.Sign(this.hSpeed))
                        return;
                    this._sprite.currentAnimation = "slide";
                }
                else if ((double)this.hSpeed < 0.0 && !this._gripped)
                {
                    this._sprite.currentAnimation = "run";
                    if (this.strafing || Math.Sign(this.offDir) == Math.Sign(this.hSpeed))
                        return;
                    this._sprite.currentAnimation = "slide";
                }
                else
                    this._sprite.currentAnimation = "idle";
            }
            else
            {
                this._sprite.currentAnimation = "jump";
                this._sprite.speed = 0.0f;
                if ((double)this.vSpeed < 0.0 && !this._hovering)
                    this._sprite.frame = 0;
                else
                    this._sprite.frame = 2;
            }
        }

        private void UpdateBurning()
        {
            this.burnSpeed = 0.005f;
            if (this.onFire && !this.dead)
            {
                if (_flameWait > 1.0)
                    RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.Short));
                this.profile.stats.timeOnFire += Maths.IncFrameTimer();
                if (this.wallCollideLeft != null)
                {
                    this.offDir = 1;
                }
                else
                {
                    if (this.wallCollideRight == null)
                        return;
                    this.offDir = -1;
                }
            }
            else
            {
                if (this.onFire || this.dead)
                    return;
                this.burnt -= 0.005f;
                if (burnt >= 0.0)
                    return;
                this.burnt = 0.0f;
            }
        }

        public override void Extinquish()
        {
            if (this._trapped != null)
                this._trapped.Extinquish();
            if (this._ragdollInstance != null)
                this._ragdollInstance.Extinguish();
            base.Extinquish();
        }

        public void ResetNonServerDeathState()
        {
            this._isGhost = false;
            this._killed = false;
            this.forceDead = false;
            this.unfocus = 1f;
            this.unfocused = false;
            this.active = true;
            this.solid = true;
            this.beammode = false;
            this.immobilized = false;
            this.gravMultiplier = 1f;
            if (!(Level.current is TeamSelect2) || (Level.current as TeamSelect2)._beam == null || (Level.current as TeamSelect2)._beam2 == null)
                return;
            (Level.current as TeamSelect2)._beam.RemoveDuck(this);
            (Level.current as TeamSelect2)._beam2.RemoveDuck(this);
        }

        public void Ressurect()
        {
            this.dead = false;
            if (this.ragdoll != null)
                this.ragdoll.Unragdoll();
            this.ResetNonServerDeathState();
            this.Regenerate();
            this.crouch = false;
            this.sliding = false;
            this.burnt = 0.0f;
            this._onFire = false;
            this.hSpeed = 0.0f;
            this.vSpeed = 0.0f;
            if (Level.current.camera is FollowCam)
                (Level.current.camera as FollowCam).Add(this);
            this._cooked = null;
            Duck.ResurrectEffect(this.position);
            this.vSpeed = -3f;
            if (!Network.isActive || !this.isServerForObject)
                return;
            Thing.SuperFondle(_cookedInstance, DuckNetwork.localConnection);
            this._cookedInstance.visible = false;
            this._cookedInstance.active = false;
            ++this.lastAppliedLifeChange;
            Send.Message(new NMRessurect(this.position, this, this.lastAppliedLifeChange));
        }

        public static void ResurrectEffect(Vec2 pPosition)
        {
            for (int index = 0; index < 6; ++index)
                Level.Add(new CampingSmoke(pPosition.x - 5f + Rando.Float(10f), (float)(pPosition.y + 6.0 - 3.0 + (double)Rando.Float(6f) - index * 1.0))
                {
                    move = {
            x = (Rando.Float(0.6f) - 0.3f),
            y = (Rando.Float(0.8f) - 1.8f)
          }
                });
        }

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
                GhostPack ghostPack = new GhostPack(0.0f, 0.0f);
                this._equipment.Add(ghostPack);
                ghostPack.Equip(this);
                Level.Add(ghostPack);
            }
            if (!this._isGhost)
                return;
            this._ghostTimer -= 23f / 1000f;
            if (_ghostTimer >= 0.0)
                return;
            this._ghostTimer = 1f;
            this._isGhost = false;
            this.Ressurect();
        }

        public void Swear()
        {
            if (Network.isActive)
            {
                if (this.isServerForObject)
                    this._netSwear.Play();
            }
            else
            {
                float num = 0f;
                if (this.profile.team != null && this.profile.team.name == "Sailors")
                    num += 0.1f;
                if (Rando.Float(1f) < 0.03f + this.profile.funslider * 0.045f + num)
                {
                    SFX.Play("quackBleep", 0.8f, Rando.Float(-0.05f, 0.05f));
                    Event.Log(new SwearingEvent(this.profile, this.profile));
                }
                else if (Rando.Float(1f) < 0.5f)
                    SFX.Play("cutOffQuack", pitch: Rando.Float(-0.05f, 0.05f));
                else
                    SFX.Play("cutOffQuack2", pitch: Rando.Float(-0.05f, 0.05f));
            }
            this.quack = 10;
        }

        public void Scream()
        {
            if (Network.isActive)
            {
                if (this.isServerForObject)
                    this._netScream.Play();
            }
            else if (Rando.Float(1f) < 0.03f + this.profile.funslider * 0.045f)
            {
                SFX.Play("quackBleep", 0.9f);
                Event.Log(new SwearingEvent(this.profile, this.profile));
            }
            else if (Rando.Float(1f) < 0.5f)
                SFX.Play("quackYell03");
            else if (Rando.Float(1f) < 0.5f)
                SFX.Play("quackYell02");
            else
                SFX.Play("quackYell01");
            this.quack = 10;
        }

        public void Disarm(Thing disarmedBy)
        {
            if (!this.isServerForObject)
                return;
            if (this.holdObject != null && (!Network.isActive || disarmedBy != null && disarmedBy.isServerForObject))
                ++Global.data.disarms.valueInt;
            Profile responsibleProfile = disarmedBy?.responsibleProfile;
            if (responsibleProfile != null && this.holdObject != null)
            {
                this.disarmIndex = responsibleProfile.networkIndex;
                this.disarmIndexCooldown = 0.5f;
            }
            else
            {
                this.disarmIndex = 9;
                this.disarmIndexCooldown = 0.5f;
            }
            this._disarmedBy = responsibleProfile;
            this._disarmedAt = DateTime.Now;
            this._holdingAtDisarm = holdObject;
            if (this.holdObject != null)
            {
                this.Fondle(holdObject);
                this.holdObject.disarmedFrom = this;
                this.holdObject.disarmTime = DateTime.Now;
                if (Network.isActive)
                {
                    if (this.isServerForObject)
                        this._netDisarm.Play();
                }
                else
                    SFX.Play("disarm", 0.3f, Rando.Float(0.2f, 0.4f));
            }
            Event.Log(new DisarmEvent(responsibleProfile, this.profile));
            this.ThrowItem();
            this.Swear();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            Holdable hold = with as Holdable;
            if (this._isGhost || with == null || (hold != null && hold.owner == this) || with is FeatherVolume || ((with == this._lastHoldItem || (with.owner != null && with.owner == this._lastHoldItem)) && this._timeSinceThrow < 7) || with == this._trapped || with == this._trappedInstance || with is Dart || (with.owner != null && with.owner is SpikeHelm))
            {
                return;
            }
            if (with is IceWedge)
            {
                this._iceWedging = 5;
            }
            if (with is RagdollPart)
            {
                RagdollPart part = with as RagdollPart;
                if (part != null && part.doll != null && part.doll.captureDuck != null && part.doll.captureDuck.killedByProfile == this.profile && part.doll.captureDuck.framesSinceKilled < 50)
                {
                    return;
                }
                if (part != null && part.doll != null && (part.doll.PartHeld() || (this.holdObject is Chainsaw && this._timeSinceChainKill < 50)))
                {
                    return;
                }
                if (this.holdObject != null && this.holdObject is RagdollPart && part != null && part.doll != null && part.doll.holdingOwner == this)
                {
                    return;
                }
                if (this.ragdoll != null && (with == this.ragdoll.part1 || with == this.ragdoll.part2 || with == this.ragdoll.part3))
                {
                    return;
                }
                if (part.doll == null || part.doll.captureDuck == this)
                {
                    return;
                }
                if (this._timeSinceThrow < 15 && part.doll != null && (part.doll.part1 == this._lastHoldItem || part.doll.part2 == this._lastHoldItem || part.doll.part3 == this._lastHoldItem))
                {
                    return;
                }
            }
            if (!this.dead && !this.swinging && with is PhysicsObject && with.totalImpactPower > with.weightMultiplierInv * 2f)
            {
                if (with is Duck && with.weight >= 5f)
                {
                    Duck d = with as Duck;
                    bool bootsmash = d.HasEquipment(typeof(Boots)) && !d.sliding;
                    if (from == ImpactedFrom.Top && with.bottom - 5f < base.top && with.impactPowerV > 2f && bootsmash)
                    {
                        this.vSpeed = with.impactDirectionV * 0.5f;
                        with.vSpeed = -with.vSpeed * 0.7f;
                        d._groundValid = 7;
                        d.slamWait = 6;
                        if (with.isServerForObject)
                        {
                            RumbleManager.AddRumbleEvent(d.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                            Duck.MakeStars(this.position + new Vec2(0f, (this.crouch || this.ragdoll != null) ? -2 : -6), base.velocity);
                            if (Network.isActive)
                            {
                                Send.Message(new NMBonk(this.position + new Vec2(0f, (this.crouch || this.ragdoll != null) ? -2 : -6), base.velocity));
                            }
                        }
                        Helmet h = this.GetEquipment(typeof(Helmet)) as Helmet;
                        if (h != null)
                        {
                            SFX.Play("metalRebound", 1f, 0f, 0f, false);
                            RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                            h.Crush(d);
                            return;
                        }
                        if (with.isServerForObject)
                        {
                            StatBinding ducksCrushed = Global.data.ducksCrushed;
                            int valueInt = ducksCrushed.valueInt;
                            ducksCrushed.valueInt = valueInt + 1;
                            this.Kill(new DTCrush(with as PhysicsObject));
                            return;
                        }
                    }
                }
                else if (with.dontCrush)
                {
                    if (with.alpha > 0.99f && (from == ImpactedFrom.Left || from == ImpactedFrom.Right) && ((!Network.isActive && with.impactPowerH > 2.3f) || with.impactPowerH > 3f))
                    {
                        bool processDisarm = with.isServerForObject;
                        if (!processDisarm && Level.CheckLine<Block>(this.position, with.position) != null)
                        {
                            processDisarm = true;
                        }
                        if (processDisarm)
                        {
                            this.hSpeed = with.impactDirectionH * 0.5f;
                            if (!(with is EnergyScimitar))
                            {
                                with.hSpeed = -with.hSpeed * with.bouncy;
                            }
                            if (base.isServerForObject && (!Network.isActive || this._disarmWait == 0) && this._disarmDisable <= 0)
                            {
                                this.Disarm(with);
                                this._disarmWait = 5;
                                RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                            }
                            if (!base.isServerForObject)
                            {
                                Send.Message(new NMDisarm(this, with.impactDirectionH * 0.5f), this.connection);
                                return;
                            }
                        }
                    }
                }
                else if (from == ImpactedFrom.Top && with.y < base.y && with.vSpeed > 0f && with.impactPowerV > 2f && with.weight >= 5f)
                {
                    if (with is PhysicsObject)
                    {
                        PhysicsObject wp = with as PhysicsObject;
                        if (wp.lastPosition.y + with.collisionOffset.y + with.collisionSize.y < base.top)
                        {
                            Helmet h2 = this.GetEquipment(typeof(Helmet)) as Helmet;
                            if (h2 != null && h2 is SpikeHelm && wp == (h2 as SpikeHelm).oldPoke)
                            {
                                return;
                            }
                            this.vSpeed = with.impactDirectionV * 0.5f;
                            with.vSpeed = -with.vSpeed * 0.5f;
                            if (with.isServerForObject)
                            {
                                Duck.MakeStars(this.position + new Vec2(0f, (this.crouch || this.ragdoll != null) ? -2 : -6), base.velocity);
                                if (Network.isActive)
                                {
                                    Send.Message(new NMBonk(this.position + new Vec2(0f, (this.crouch || this.ragdoll != null) ? -2 : -6), base.velocity));
                                }
                            }
                            if (h2 != null && this.ragdoll == null)
                            {
                                SFX.Play("metalRebound", 1f, 0f, 0f, false);
                                RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                                h2.Crush(wp);
                                return;
                            }
                            if (with.isServerForObject)
                            {
                                this.Kill(new DTCrush(with as PhysicsObject));
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
                    if ((this.holdObject is SledgeHammer && with is RagdollPart) || (this.holdObject is Sword && (this.holdObject as Sword).crouchStance && ((this.offDir < 0 && from == ImpactedFrom.Left) || (this.offDir > 0 && from == ImpactedFrom.Right))))
                    {
                        return;
                    }
                    bool processDisarm2 = with.isServerForObject;
                    if (!processDisarm2 && Level.CheckLine<Block>(this.position, with.position) != null)
                    {
                        processDisarm2 = true;
                    }
                    if (processDisarm2)
                    {
                        with.hSpeed = -with.hSpeed * with.bouncy;
                        if (with is TeamHat)
                        {
                            this.Swear();
                            return;
                        }
                        this.hSpeed = with.impactDirectionH * 0.5f;
                        if (base.isServerForObject && (!Network.isActive || this._disarmWait == 0) && this._disarmDisable <= 0)
                        {
                            this.Disarm(with);
                            RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
                            this._disarmWait = 5;
                        }
                        if (!base.isServerForObject)
                        {
                            Send.Message(new NMDisarm(this, with.impactDirectionH * 0.5f), this.connection);
                            return;
                        }
                    }
                }
                else if (!(with is TeamHat) && from == ImpactedFrom.Bottom && with.y > base.bottom && with.impactPowerV > 2f)
                {
                    this.vSpeed = with.impactDirectionV * 0.5f;
                    with.vSpeed = -with.vSpeed * 0.5f;
                }
            }
        }

        public override void OnTeleport()
        {
            if (this.holdObject != null)
                this.holdObject.OnTeleport();
            foreach (Thing thing in this._equipment)
                thing.OnTeleport();
            if (this._vine == null)
                return;
            this._vine.Degrapple();
            this._vine = null;
        }

        public void AdvanceServerTime(int frames)
        {
            while (frames > 0)
            {
                --frames;
                ++this.clientFrame;
                this.Update();
            }
        }

        public override void Initialize()
        {
            this.jumpSpeed = Duck.JumpSpeed;
            if (Level.current != null)
            {
                if (this.isServerForObject)
                {
                    foreach (Equipper equipper in Level.current.things[typeof(Equipper)])
                    {
                        if (equipper.radius.value == 0 || (double)(this.position - equipper.position).length <= equipper.radius.value)
                        {
                            Thing containedInstance = equipper.GetContainedInstance(this.position);
                            if (containedInstance != null)
                            {
                                Level.Add(containedInstance);
                                if (containedInstance is Holdable)
                                    (containedInstance as Holdable).UpdateMaterial();
                                if ((bool)equipper.holstered || (bool)equipper.powerHolstered)
                                {
                                    Holster e = !(bool)equipper.powerHolstered ? new Holster(this.position.x, this.position.y) : new PowerHolster(this.position.x, this.position.y);
                                    Level.Add(e);
                                    e.SetContainedObject(containedInstance as Holdable);
                                    this.Equip(e);
                                    e.chained = equipper.holsterChained;
                                }
                                else if (containedInstance is Equipment && (containedInstance as Equipment).wearable)
                                {
                                    if (containedInstance is Hat && this.GetEquipment(typeof(Hat)) is Hat equipment)
                                    {
                                        this.Unequip(equipment);
                                        equipment.position = this.position;
                                        equipment.vSpeed = 0.0f;
                                        equipment.hSpeed = 0.0f;
                                    }
                                    this.Equip(containedInstance as Equipment);
                                }
                                else if (containedInstance is Holdable)
                                {
                                    if (this.holdObject != null)
                                    {
                                        Holdable holdObject = this.holdObject;
                                        this.ThrowItem(false);
                                        holdObject.position = this.position;
                                        holdObject.vSpeed = 0.0f;
                                        holdObject.hSpeed = 0.0f;
                                    }
                                    this.GiveHoldable(containedInstance as Holdable);
                                }
                            }
                        }
                    }
                }
                Level.Add(_featherVolume);
            }
            if (Network.isServer)
                this._netProfileIndex = (byte)DuckNetwork.IndexOf(this.profile);
            if (Network.isActive)
                this._netQuack.pitchBinding = new FieldBinding(this, "quackPitch");
            base.Initialize();
        }

        public override void NetworkUpdate()
        {
        }

        public Ragdoll ragdoll
        {
            get => this._currentRagdoll;
            set
            {
                this._currentRagdoll = value;
                if (this._currentRagdoll == null)
                    return;
                this._currentRagdoll._duck = this;
            }
        }

        public void GoRagdoll()
        {
            if (Network.isActive && (this._ragdollInstance == null || this._ragdollInstance != null && this._ragdollInstance.visible || this._cookedInstance != null && this._cookedInstance.visible) || this.ragdoll != null || this._cooked != null)
                return;
            this._hovering = false;
            float ypos = this.y + 4f;
            float degrees;
            if (this.sliding)
            {
                ypos += 6f;
                degrees = this.offDir >= 0 ? 0.0f : 180f;
            }
            else
                degrees = -90f;
            Vec2 v = new Vec2(this._hSpeed, this._vSpeed);
            this.hSpeed = 0.0f;
            this.vSpeed = 0.0f;
            if (Network.isActive)
            {
                this.ragdoll = this._ragdollInstance;
                this._ragdollInstance.active = true;
                this._ragdollInstance.visible = true;
                this._ragdollInstance.solid = true;
                this._ragdollInstance.enablePhysics = true;
                this._ragdollInstance.x = this.x;
                this._ragdollInstance.y = this.y;
                this._ragdollInstance.owner = null;
                this._ragdollInstance.npi = netProfileIndex;
                this._ragdollInstance.SortOutParts(this.x, ypos, this, this.sliding, degrees, offDir, v);
                this.Fondle(_ragdollInstance);
            }
            else
            {
                this.ragdoll = new Ragdoll(this.x, ypos, this, this.sliding, degrees, offDir, v);
                Level.Add(ragdoll);
                this.ragdoll.RunInit();
            }
            if (this.ragdoll == null)
                return;
            this.ragdoll.connection = this.connection;
            this.ragdoll.part1.connection = this.connection;
            this.ragdoll.part2.connection = this.connection;
            this.ragdoll.part3.connection = this.connection;
            if (!this.fancyShoes)
            {
                Equipment equipment = this.GetEquipment(typeof(Hat));
                if (equipment != null && !(equipment as Hat).strappedOn)
                {
                    this.Unequip(equipment);
                    equipment.hSpeed = this.hSpeed * 1.2f;
                    equipment.vSpeed = this.vSpeed - 2f;
                }
                this.ThrowItem(false);
            }
            this.OnTeleport();
            if ((double)this.y > -4000.0)
                this.y -= 5000f;
            this.sliding = false;
            this.crouch = false;
        }

        public virtual void UpdateSkeleton()
        {
            Vec2 position = this.position;
            if (this._trapped != null)
            {
                this.x = this._trapped.x;
                this.y = this._trapped.y;
            }
            if (this.ragdoll != null)
            {
                if (this.ragdoll.part1 != null && this.ragdoll.part3 != null)
                {
                    this._skeleton.upperTorso.position = this.ragdoll.part1.Offset(new Vec2(0.0f, 7f));
                    this._skeleton.upperTorso.orientation = this.ragdoll.part1.offDir > 0 ? -this.ragdoll.part1.angle : this.ragdoll.part1.angle;
                    this._skeleton.lowerTorso.position = this.ragdoll.part3.Offset(new Vec2(5f, 11f));
                    this._skeleton.lowerTorso.orientation = (this.ragdoll.part3.offDir > 0 ? -this.ragdoll.part3.angle : this.ragdoll.part3.angle) + Maths.DegToRad(180f);
                    this._skeleton.head.position = this.ragdoll.part1.Offset(new Vec2(-2f, -6f));
                    this._skeleton.head.orientation = this.ragdoll.part1.offDir > 0 ? -this.ragdoll.part1.angle : this.ragdoll.part1.angle;
                }
            }
            else if (this._sprite != null)
            {
                this._skeleton.head.position = this.Offset(DuckRig.GetHatPoint(this._sprite.imageIndex)) + new Vec2(0.0f, this.verticalOffset);
                this._skeleton.upperTorso.position = this.Offset(DuckRig.GetChestPoint(this._sprite.imageIndex)) + new Vec2(0.0f, this.verticalOffset);
                this._skeleton.lowerTorso.position = this.position + new Vec2(0.0f, this.verticalOffset);
                if (this.sliding)
                {
                    this._skeleton.head.orientation = Maths.DegToRad(90f);
                    this._skeleton.upperTorso.orientation = Maths.DegToRad(90f);
                    this._skeleton.lowerTorso.orientation = 0.0f;
                }
                else
                {
                    float num = this.offDir < 0 ? this.angle : -this.angle;
                    this._skeleton.head.orientation = num;
                    this._skeleton.upperTorso.orientation = num;
                    this._skeleton.lowerTorso.orientation = num;
                }
                if (this._trapped != null)
                {
                    this._skeleton.head.orientation = 0.0f;
                    this._skeleton.upperTorso.orientation = 0.0f;
                    this._skeleton.lowerTorso.orientation = 0.0f;
                    this._skeleton.head.position = this.Offset(new Vec2(-1f, -10f));
                    this._skeleton.upperTorso.position = this.Offset(new Vec2(1f, 2f));
                    this._skeleton.lowerTorso.position = this.Offset(new Vec2(0.0f, -8f));
                }
            }
            this.position = position;
        }

        public override float angle
        {
            get => this._angle + this.tilt * 0.2f;
            set => this._angle = value;
        }

        public PhysicsSnapshotDuckProperties GetProperties() => new PhysicsSnapshotDuckProperties()
        {
            jumping = this.jumping
        };

        public void SetProperties(PhysicsSnapshotDuckProperties dat) => this.jumping = dat.jumping;

        public Holdable holstered
        {
            get => this.GetEquipment(typeof(Holster)) is Holster equipment ? equipment.containedObject : null;
            set
            {
                if (!(this.GetEquipment(typeof(Holster)) is Holster equipment))
                    return;
                equipment.SetContainedObject(value);
            }
        }

        public Holdable skewered => this.GetEquipment(typeof(SpikeHelm)) is SpikeHelm equipment ? equipment.poked as Holdable : null;

        public TV tvHeld => this.holdObject is TV holdObject && !holdObject._ruined ? holdObject : null;

        public TV tvHolster
        {
            get
            {
                TV tv = null;
                if (this.GetEquipment(typeof(Holster)) is Holster equipment && equipment.containedObject is TV)
                    tv = equipment.containedObject as TV;
                return tv != null && !tv._ruined ? tv : null;
            }
        }

        public TV tvHat
        {
            get
            {
                TV tv = null;
                if (this.GetEquipment(typeof(SpikeHelm)) is SpikeHelm equipment && equipment.poked is TV)
                    tv = equipment.poked as TV;
                return tv != null && !tv._ruined ? tv : null;
            }
        }

        public bool HasTV() => this.tvHeld != null || this.tvHolster != null || this.tvHat != null;

        public int HasTVs(bool pChannel)
        {
            int num = 0;
            if (this.tvHeld != null && this.tvHeld.channel == pChannel)
                ++num;
            if (this.tvHolster != null && this.tvHolster.channel == pChannel)
                ++num;
            if (this.tvHat != null && this.tvHat.channel == pChannel)
                ++num;
            return num;
        }

        public bool CheckTVChannel(bool pChannel) => this.tvHeld != null && this.tvHeld.channel == pChannel || this.tvHolster != null && this.tvHolster.channel == pChannel || this.tvHat != null && this.tvHat.channel == pChannel;

        public bool CheckTVJump()
        {
            if (this.pipeOut > 0)
                return false;
            bool flag = false;
            if (this.tvHeld != null && this.tvHeld.channel && this.tvHeld.jumpReady)
            {
                this.tvHeld.jumpReady = false;
                flag = true;
            }
            else if (this.tvHolster != null && this.tvHolster.channel && this.tvHolster.jumpReady)
            {
                this.tvHolster.jumpReady = false;
                flag = true;
            }
            else if (this.tvHat != null && this.tvHat.channel && this.tvHat.jumpReady)
            {
                this.tvHat.jumpReady = false;
                flag = true;
            }
            if (flag)
            {
                Level.Add(new ColorStar(this.x + this.hSpeed * 2f, this.y + 4f, new Vec2(-1.5f, -2.5f) + new Vec2((float)(((double)this.hSpeed + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f)), new Color(237, 94, 238)));
                Level.Add(new ColorStar(this.x + this.hSpeed * 2f, this.y + 4f, new Vec2(-0.9f, -1.5f) + new Vec2((float)(((double)this.hSpeed + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f)), new Color(49, 162, 242)));
                Level.Add(new ColorStar(this.x + this.hSpeed * 2f, this.y + 4f, new Vec2(0.9f, -1.5f) + new Vec2((float)(((double)this.hSpeed + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f)), new Color(247, 224, 90)));
                Level.Add(new ColorStar(this.x + this.hSpeed * 2f, this.y + 4f, new Vec2(1.5f, -2.5f) + new Vec2((float)(((double)this.hSpeed + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f)), new Color(192, 32, 45)));
            }
            return flag;
        }

        public void UpdateMove()
        {
            if (this.inputProfile == null)
                return;
            if (this.pipeOut > 0)
            {
                --this.pipeOut;
                if (this.pipeOut == 2 && !this.inputProfile.Down("JUMP"))
                    this.pipeOut = 0;
                else
                    this.vSpeed -= 0.5f;
            }
            if (this.pipeBoost > 0)
                --this.pipeBoost;
            if (this.slamWait > 0)
                --this.slamWait;
            this.tvJumped = false;
            ++this._timeSinceChainKill;
            this.weight = 5.3f;
            if (this.holdObject != null)
            {
                this.weight += Math.Max(0.0f, this.holdObject.weight - 5f);
                if (this.holdObject.destroyed)
                    this.ThrowItem();
            }
            if (this.isServerForObject)
                this.UpdateQuack();
            if (this.hat != null)
                this.hat.SetQuack(this.quack > 0 || this._mindControl != null && this._derpMindControl ? 1 : 0);
            if (this._ragdollInstance != null && this._ragdollInstance.part2 != null)
                this._ragdollInstance.part2.UpdateLastReasonablePosition(this.position);
            if (this.inNet && this._trapped != null)
            {
                this.x = this._trapped.x;
                this.y = this._trapped.y;
                if (this._ragdollInstance != null && this._ragdollInstance.part2 != null)
                    this._ragdollInstance.part2.UpdateLastReasonablePosition(this.position);
                this.owner = _trapped;
                this.ThrowItem(false);
            }
            else
            {
                this.owner = null;
                this.skipPlatFrames = Maths.CountDown(this.skipPlatFrames, 1, 0);
                this.crippleTimer = Maths.CountDown(this.crippleTimer, 0.1f);
                if (this.inputProfile.Pressed("JUMP"))
                {
                    this._jumpValid = 4;
                    if (!this.grounded && this.crouch)
                        this.skipPlatFrames = 10;
                }
                else
                    this._jumpValid = Maths.CountDown(this._jumpValid, 1, 0);
                this._skipPlatforms = false;
                if (this.inputProfile.Down("DOWN") && this.skipPlatFrames > 0)
                    this._skipPlatforms = true;
                bool flag1 = this.grounded;
                if (!flag1 && this.HasEquipment(typeof(ChokeCollar)))
                {
                    ChokeCollar equipment = this.GetEquipment(typeof(ChokeCollar)) as ChokeCollar;
                    if (equipment.ball.grounded && (double)equipment.ball.bottom < (double)this.top && (double)this.vSpeed > -1.0)
                        flag1 = true;
                }
                if (flag1)
                {
                    this.framesSinceJump = 0;
                    this._groundValid = 7;
                    this._hovering = false;
                    this._double = false;
                }
                else
                {
                    this._groundValid = Maths.CountDown(this._groundValid, 1, 0);
                    ++this.framesSinceJump;
                }
                if (this.mindControl != null)
                    this.mindControl.UpdateExtraInput();
                //this._heldLeft = false;
                //this._heldRight = false;
                Block block1 = Level.CheckRect<Block>(new Vec2(this.x - 3f, this.y - 9f), new Vec2(this.x + 3f, this.y + 4f));
                this._crouchLock = (this.crouch || this.sliding) && block1 != null && block1.solid;
                float num1 = 0.55f * this.holdWeightMultiplier * this.grappleMultiplier * this.accelerationMultiplier;
                this.maxrun = this._runMax * this.holdWeightMultiplier;
                if (this._isGhost)
                {
                    num1 *= 1.4f;
                    this.maxrun *= 1.5f;
                }
                int num2 = this.HasTVs(false);
                for (int index = 0; index < num2; ++index)
                {
                    num1 *= 1.4f;
                    this.maxrun *= 1.5f;
                }
                if (this.holdObject is EnergyScimitar && (this.holdObject as EnergyScimitar).dragSpeedBonus)
                {
                    if ((this.holdObject as EnergyScimitar)._spikeDrag)
                    {
                        num1 *= 0.5f;
                        this.maxrun *= 0.5f;
                    }
                    else
                    {
                        num1 *= 1.3f;
                        this.maxrun *= 1.35f;
                    }
                }
                if (specialFrictionMod > 0.0)
                    num1 *= Math.Min(this.specialFrictionMod * 2f, 1f);
                if (this.isServerForObject && this.isOffBottomOfLevel && !this.dead)
                {
                    if (this.ragdoll != null && this.ragdoll.part1 != null && this.ragdoll.part2 != null && this.ragdoll.part3 != null)
                    {
                        this.ragdoll.part1.y += 500f;
                        this.ragdoll.part2.y += 500f;
                        this.ragdoll.part3.y += 500f;
                    }
                    this.y += 500f;
                    this.Kill(new DTFall());
                    ++this.profile.stats.fallDeaths;
                }
                if (Network.isActive && this.ragdoll != null && this.ragdoll.connection != DuckNetwork.localConnection && this.ragdoll.TryingToControl() && !this.ragdoll.PartHeld())
                    this.Fondle(ragdoll);
                if (this.CanMove())
                {
                    if (!this._grounded)
                        this.profile.stats.airTime += Maths.IncFrameTimer();
                    if (this.isServerForObject && !this.sliding && this.inputProfile.Pressed("UP"))
                    {
                        Desk t = Level.Nearest<Desk>(this.position);
                        if (t != null && (double)(t.position - this.position).length < 22.0 && Level.CheckLine<Block>(this.position, t.position) == null)
                        {
                            this.Fondle(t);
                            t.Flip(this.offDir < 0);
                        }
                    }
                    float num3;
                    if (this.inputProfile.Down("LEFT"))
                    {
                        num3 = 1f;
                        if (this._leftPressedFrame == 0)
                            this._leftPressedFrame = (int)DuckGame.Graphics.frame;
                    }
                    else
                    {
                        num3 = Maths.NormalizeSection(Math.Abs(Math.Min(this.inputProfile.leftStick.x, 0f)), 0.2f, 0.9f);
                        if (num3 > 0.01f)
                        {
                            if (this._leftPressedFrame == 0)
                                this._leftPressedFrame = (int)DuckGame.Graphics.frame;
                        }
                        else
                            this._leftPressedFrame = 0;
                    }
                    float num4;
                    if (this.inputProfile.Down("RIGHT"))
                    {
                        num4 = 1f;
                        if (this._rightPressedFrame == 0)
                            this._rightPressedFrame = (int)DuckGame.Graphics.frame;
                    }
                    else
                    {
                        num4 = Maths.NormalizeSection(Math.Max(this.inputProfile.leftStick.x, 0f), 0.2f, 0.9f);
                        if ((double)num4 > 0.01f)
                        {
                            if (this._rightPressedFrame == 0)
                                this._rightPressedFrame = (int)DuckGame.Graphics.frame;
                        }
                        else
                            this._rightPressedFrame = 0;
                    }
                    bool flag2 = Options.Data.oldAngleCode;
                    if (!this.isServerForObject && this.inputProfile != null)
                        flag2 = this.inputProfile.oldAngles;
                    if (num3 < 0.01f && this.onFire && this.offDir == 1)
                        num4 = 1f;
                    if (num4 < 0.01f && this.onFire && this.offDir == -1)
                        num3 = 1f;
                    if (this.grappleMul)
                    {
                        num3 *= 1.5f;
                        num4 *= 1.5f;
                    }
                    if (DevConsole.qwopMode && Level.current is GameLevel)
                    {
                        if (num3 > 0f)
                            this.offDir = -1;
                        else if (num4 > 0f)
                            this.offDir = 1;
                        if (this._walkTime == 0)
                        {
                            num4 = num3 = 0f;
                        }
                        else
                        {
                            if (this.offDir < 0)
                                num3 = 1f;
                            else
                                num4 = 1f;
                            --this._walkTime;
                        }
                        if (this._walkCount > 0)
                            --this._walkCount;
                        if (this.inputProfile.Pressed("LTRIGGER"))
                        {
                            if (this._walkCount > 0 && this._nextTrigger)
                            {
                                this.GoRagdoll();
                                this._walkCount = 0;
                            }
                            else
                            {
                                this._walkCount += 20;
                                if (DevConsole.rhythmMode && Level.current is GameLevel)
                                    this._walkTime += 20;
                                else
                                    this._walkTime += 8;
                                if (this._walkTime > 20)
                                    this._walkTime = 20;
                                if (this._walkCount > 40)
                                    this._walkCount = 40;
                                this._nextTrigger = true;
                            }
                        }
                        else if (this.inputProfile.Pressed("RTRIGGER"))
                        {
                            if (this._walkCount > 0 && !this._nextTrigger)
                            {
                                this.GoRagdoll();
                                this._walkCount = 0;
                            }
                            else
                            {
                                this._walkCount += 20;
                                if (DevConsole.rhythmMode && Level.current is GameLevel)
                                    this._walkTime += 20;
                                else
                                    this._walkTime += 8;
                                if (this._walkTime > 20)
                                    this._walkTime = 20;
                                if (this._walkCount > 40)
                                    this._walkCount = 40;
                                this._nextTrigger = false;
                            }
                        }
                    }
                    bool flag3 = this._crouchLock && this.grounded && this.inputProfile.Pressed("ANY");
                    if (flag3 && this.offDir == -1)
                    {
                        num3 = 1f;
                        num4 = 0.0f;
                    }
                    if (flag3 && this.offDir == 1)
                    {
                        num4 = 1f;
                        num3 = 0.0f;
                    }
                    if (this._leftJump)
                        num3 = 0.0f;
                    else if (this._rightJump)
                        num4 = 0.0f;
                    this.strafing = false;
                    if (!this._moveLock)
                    {
                        this.strafing = this.inputProfile.Down("STRAFE");
                        if (num3 > 0.01f && !this.crouch | flag3)
                        {
                            if (this.hSpeed > -this.maxrun * num3)
                            {
                                this.hSpeed -= num1;
                                if (this.hSpeed < -this.maxrun * num3)
                                    this.hSpeed = -this.maxrun * num3;
                            }
                            //this._heldLeft = true;
                            if (!this.strafing && !flag3 && (flag2 || this._leftPressedFrame > this._rightPressedFrame))
                                this.offDir = -1;
                        }
                        if (num4 > 0.01f && !this.crouch | flag3)
                        {
                            if (this.hSpeed < maxrun * num4)
                            {
                                this.hSpeed += num1;
                                if (this.hSpeed > maxrun * num4)
                                    this.hSpeed = this.maxrun * num4;
                            }
                            //this._heldRight = true;
                            if (!this.strafing && !flag3 && (flag2 || this._rightPressedFrame > this._leftPressedFrame))
                                this.offDir = 1;
                        }
                        if (this.isServerForObject && this.strafing)
                            Global.data.strafeDistance.valueFloat += Math.Abs(this.hSpeed) * 0.00015f;
                        if (this._atWallFrames > 0)
                        {
                            --this._atWallFrames;
                        }
                        else
                        {
                            this.atWall = false;
                            this.leftWall = false;
                            this.rightWall = false;
                        }
                        this._canWallJump = this.GetEquipment(typeof(WallBoots)) != null;
                        int num5 = 6;
                        if (!this.grounded && this._canWallJump)
                        {
                            Block block2 = Level.CheckLine<Block>(this.topLeft + new Vec2(0.0f, 4f), this.bottomLeft + new Vec2(-3f, -4f));
                            Block block3 = Level.CheckLine<Block>(this.topRight + new Vec2(3f, 4f), this.bottomRight + new Vec2(0.0f, -4f));
                            if (this.inputProfile.Down("LEFT") && block2 != null && !block2.clip.Contains(this))
                            {
                                this.atWall = true;
                                this.leftWall = true;
                                this._atWallFrames = num5;
                                if (!this.onWall)
                                {
                                    this.onWall = true;
                                    SFX.Play("wallTouch", pitch: Rando.Float(-0.1f, 0.1f));
                                    for (int index = 0; index < 2; ++index)
                                    {
                                        Feather feather1 = Feather.New(this.x + (!this.leftWall ? -4f : 4f) + Rando.Float(-1f, 1f), this.y + Rando.Float(-4f, 4f), this.persona);
                                        Feather feather2 = feather1;
                                        feather2.velocity *= 0.9f;
                                        if (this.leftWall)
                                            feather1.hSpeed = Rando.Float(-1f, 2f);
                                        else
                                            feather1.hSpeed = Rando.Float(-2f, 1f);
                                        feather1.vSpeed = Rando.Float(-2f, 1.5f);
                                        Level.Add(feather1);
                                    }
                                }
                            }
                            else if (this.inputProfile.Down("RIGHT") && block3 != null && !block3.clip.Contains(this))
                            {
                                this.atWall = true;
                                this.rightWall = true;
                                this._atWallFrames = num5;
                                if (!this.onWall)
                                {
                                    this.onWall = true;
                                    SFX.Play("wallTouch", pitch: Rando.Float(-0.1f, 0.1f));
                                    for (int index = 0; index < 2; ++index)
                                    {
                                        Feather feather3 = Feather.New(this.x + (!this.leftWall ? -4f : 4f) + Rando.Float(-1f, 1f), this.y + Rando.Float(-4f, 4f), this.persona);
                                        feather3.vSpeed = Rando.Float(-2f, 1.5f);
                                        Feather feather4 = feather3;
                                        feather4.velocity *= 0.9f;
                                        if (this.leftWall)
                                            feather3.hSpeed = Rando.Float(-1f, 2f);
                                        else
                                            feather3.hSpeed = Rando.Float(-2f, 1f);
                                        Level.Add(feather3);
                                    }
                                }
                            }
                        }
                        if (this.onWall && this._atWallFrames != num5)
                        {
                            SFX.Play("wallLeave", pitch: Rando.Float(-0.1f, 0.1f));
                            for (int index = 0; index < 2; ++index)
                            {
                                Feather feather5 = Feather.New(this.x + (!this.leftWall ? -4f : 4f) + Rando.Float(-1f, 1f), this.y + Rando.Float(-4f, 4f), this.persona);
                                feather5.vSpeed = Rando.Float(-2f, 1.5f);
                                Feather feather6 = feather5;
                                feather6.velocity *= 0.9f;
                                if (this.leftWall)
                                    feather5.hSpeed = Rando.Float(-1f, 2f);
                                else
                                    feather5.hSpeed = Rando.Float(-2f, 1f);
                                Level.Add(feather5);
                            }
                            this.onWall = false;
                        }
                        if ((this.leftWall || this.rightWall) && (double)this.vSpeed > 1.0 && this._atWallFrames == num5)
                            this.vSpeed = 0.5f;
                        if (this._wallJump > 0)
                            --this._wallJump;
                        else
                            this._rightJump = this._leftJump = false;
                        bool flag4 = this._jumpValid > 0 && (this._groundValid > 0 && !this._crouchLock || this.atWall && this._wallJump == 0 || this.doFloat);
                        if (this._double && !this.HasJumpModEquipment() && !this._hovering && this.inputProfile.Pressed("JUMP"))
                        {
                            PhysicsRopeSection section = null;
                            if (this._vine == null)
                                section = Level.Nearest<PhysicsRopeSection>(this.x, this.y);
                            if (section != null && (double)(this.position - section.position).length < 18.0)
                            {
                                this._vine = section.rope.LatchOn(section, this);
                                this._double = false;
                                flag4 = false;
                                this._groundValid = 0;
                            }
                        }
                        bool flag5 = false;
                        if (flag4 && Math.Abs(this.hSpeed) < 0.2f && this.inputProfile.Down("DOWN") && Math.Abs(this.hSpeed) < 0.2f && this.inputProfile.Down("DOWN"))
                        {
                            foreach (IPlatform platform1 in Level.CheckLineAll<IPlatform>(this.bottomLeft + new Vec2(0.1f, 1f), this.bottomRight + new Vec2(-0.1f, 1f)))
                            {
                                if (platform1 is Block)
                                {
                                    flag4 = true;
                                    break;
                                }
                                if (platform1 is MaterialThing materialThing)
                                {
                                    this.clip.Add(materialThing);
                                    foreach (IPlatform platform2 in Level.CheckPointAll<IPlatform>(materialThing.topLeft + new Vec2(-2f, 2f)))
                                    {
                                        if (platform2 != null && platform2 is MaterialThing && !(platform2 is Block))
                                            this.clip.Add(platform2 as MaterialThing);
                                    }
                                    foreach (IPlatform platform3 in Level.CheckPointAll<IPlatform>(materialThing.topRight + new Vec2(2f, 2f)))
                                    {
                                        if (platform3 != null && platform3 is MaterialThing && !(platform3 is Block))
                                            this.clip.Add(platform3 as MaterialThing);
                                    }
                                    flag4 = false;
                                }
                            }
                            if (!flag4)
                            {
                                ++this.y;
                                this.vSpeed = 1f;
                                this._groundValid = 0;
                                this._hovering = false;
                                this.jumping = true;
                                flag5 = true;
                            }
                        }
                        PhysicsRopeSection section1 = null;
                        if (this._vine == null)
                        {
                            section1 = Level.Nearest<PhysicsRopeSection>(this.x, this.y);
                            if (section1 != null && (double)(this.position - section1.position).length >= 18.0)
                                section1 = null;
                        }
                        bool flag6 = false;
                        if (!flag5)
                        {
                            if (this.inputProfile.Pressed("JUMP"))
                            {
                                if (this.HasEquipment(typeof(Jetpack)) && (this._groundValid <= 0 || this.crouch || this.sliding))
                                {
                                    this.GetEquipment(typeof(Jetpack)).PressAction();
                                    flag6 = true;
                                }
                                if (!flag4 && this.HasTV() && this.CheckTVChannel(true) && this.CheckTVJump() && section1 == null)
                                {
                                    this._groundValid = 9999;
                                    flag4 = true;
                                    this.tvJumped = true;
                                }
                            }
                            if (this.inputProfile.Down("JUMP") && this.HasEquipment(typeof(Jetpack)) && (this._groundValid <= 0 || this.crouch || this.sliding))
                                flag6 = true;
                            if (this.inputProfile.Released("JUMP") && this.HasEquipment(typeof(Jetpack)))
                                this.GetEquipment(typeof(Jetpack)).ReleaseAction();
                            if (this.inputProfile.Pressed("JUMP") && this.HasEquipment(typeof(Grapple)) && !this.grounded && this._jumpValid <= 0 && this._groundValid <= 0)
                                flag6 = true;
                        }
                        bool flag7 = flag4 && !flag6;
                        bool flag8 = false;
                        bool flag9 = false;
                        bool flag10 = false;
                        if (!flag7 && this._vine != null && this.inputProfile.Released("JUMP"))
                        {
                            this._vine.Degrapple();
                            this._vine = null;
                            if (!this.inputProfile.Down("DOWN"))
                            {
                                flag7 = true;
                                flag8 = true;
                            }
                            if (!this.inputProfile.Down("UP"))
                                flag9 = true;
                            flag10 = true;
                        }
                        if (flag7)
                        {
                            if (this.atWall)
                            {
                                this._wallJump = 8;
                                if (this.leftWall)
                                {
                                    this.hSpeed += 4f;
                                    this._leftJump = true;
                                }
                                else if (this.rightWall)
                                {
                                    this.hSpeed -= 4f;
                                    this._rightJump = true;
                                }
                                this.vSpeed = this.jumpSpeed;
                            }
                            else
                                this.vSpeed = this.jumpSpeed;
                            this.jumping = true;
                            this.sliding = false;
                            if (Network.isActive)
                            {
                                if (this.isServerForObject)
                                    this._netJump.Play();
                            }
                            else
                                SFX.Play("jump", 0.5f);
                            this._groundValid = 0;
                            this._hovering = false;
                            this._jumpValid = 0;
                            ++this.profile.stats.timesJumped;
                            if (Recorder.currentRecording != null)
                                Recorder.currentRecording.LogAction(6);
                        }
                        if (flag8)
                        {
                            this.jumping = false;
                            if (flag9 && (double)this.vSpeed < 0.0)
                                this.vSpeed *= 0.7f;
                        }
                        if (this.inputProfile.Released("JUMP"))
                        {
                            if (this.jumping)
                            {
                                this.jumping = false;
                                this.pipeOut = 0;
                                if ((double)this.vSpeed < 0.0)
                                    this.vSpeed *= 0.5f;
                            }
                            this._hovering = false;
                        }
                        if (!flag7 && !this.HasJumpModEquipment() && this._groundValid <= 0)
                        {
                            bool flag11 = !this.crouch && this.holdingWeight <= 5f && (this.pipeOut <= 0 || this.vSpeed > -0.1f);
                            if (!this._hovering && this.inputProfile.Pressed("JUMP"))
                            {
                                if (section1 != null)
                                {
                                    this._vine = section1.rope.LatchOn(section1, this);
                                    this._double = false;
                                }
                                else if (this._vine == null && flag11)
                                {
                                    this._hovering = true;
                                    this._flapFrame = 0;
                                }
                            }
                            if (flag11 && this._hovering && (double)this.vSpeed >= 0f)
                            {
                                if ((double)this.vSpeed > 1f)
                                    this.vSpeed = 1f;
                                this.vSpeed -= 0.15f;
                            }
                        }
                        if (this.doFloat)
                            this._hovering = false;
                        if (this.isServerForObject)
                        {
                            if (this.inputProfile.Down("DOWN"))
                            {
                                if (!this.grounded && this.HasTV())
                                {
                                    if (this.slamWait <= 0)
                                    {
                                        if ((double)this.vSpeed < vMax)
                                            this.vSpeed += 0.6f;
                                        this.crouch = true;
                                    }
                                    else
                                        this.crouch = false;
                                }
                                else
                                    this.crouch = true;
                                if (!this.disableCrouch && !this.crouchCancel)
                                {
                                    if (this.grounded && Math.Abs(this.hSpeed) > 1f)
                                    {
                                        if (!this.sliding && slideBuildup < -0.3f)
                                        {
                                            this.slideBuildup = 0.4f;
                                            this.didFireSlide = true;
                                        }
                                        this.sliding = true;
                                    }
                                }
                                else
                                    this.crouch = false;
                            }
                            else
                            {
                                if (!this._crouchLock)
                                {
                                    this.crouch = false;
                                    this.sliding = false;
                                }
                                this.crouchCancel = false;
                            }
                            if (!this.sliding)
                                this.didFireSlide = false;
                            if (slideBuildup > 0.0 || !this.sliding || !this.didFireSlide)
                            {
                                this.slideBuildup -= Maths.IncFrameTimer();
                                if (slideBuildup <= -0.6f)
                                    this.slideBuildup = -0.6f;
                            }
                        }
                        if (this.isServerForObject && !(this.holdObject is DrumSet) && !(this.holdObject is Trumpet) && this.inputProfile.Pressed("RAGDOLL") && !(Level.current is TitleScreen) && this.pipeOut <= 0)
                        {
                            this.framesSinceRagdoll = 0;
                            this.GoRagdoll();
                        }
                        if (this.isServerForObject && this.grounded && (double)Math.Abs(this.vSpeed) + (double)Math.Abs(this.hSpeed) < 0.5 && !this._closingEyes && this.holdObject == null && this.inputProfile.Pressed("SHOOT"))
                        {
                            Ragdoll t = Level.Nearest<Ragdoll>(this.x, this.y, this);
                            if (t != null && t.active && t.visible && (double)(t.position - this.position).length < 100.0 && t.captureDuck != null && t.captureDuck.dead && !t.captureDuck._eyesClosed && (double)(t.part1.position - (this.position + new Vec2(0.0f, 8f))).length < 4.0)
                            {
                                Level.Add(new EyeCloseWing((double)t.part1.angle < 0.0 ? this.x - 4f : this.x - 11f, this.y + 7f, (double)t.part1.angle < 0.0 ? 1 : -1, this._spriteArms, this, t.captureDuck));
                                if (Network.isActive)
                                    Send.Message(new NMEyeCloseWing(this.position, this, t.captureDuck));
                                this._closingEyes = true;
                                ++this.profile.stats.respectGivenToDead;
                                this.AddCoolness(1);
                                this._timeSinceDuckLayedToRest = DateTime.Now;
                                Flower flower = Level.Nearest<Flower>(this.x, this.y);
                                if (flower != null && (double)(flower.position - this.position).length < 22.0)
                                {
                                    this.Fondle(t);
                                    this.Fondle(t.captureDuck);
                                    if (Network.isActive)
                                        Send.Message(new NMFuneral(this.profile, t.captureDuck));
                                    t.captureDuck.LayToRest(this.profile);
                                    if (!Music.currentSong.Contains("MarchOfDuck"))
                                    {
                                        if (Network.isActive)
                                            Send.Message(new NMPlayMusic("MarchOfDuck"));
                                        Music.Play("MarchOfDuck", false);
                                    }
                                }
                            }
                        }
                        if (this.inputProfile.Released("JUMP") || this.vineRelease)
                            this.vineRelease = false;
                        if (flag10)
                            this.vineRelease = true;
                    }
                }
                this.disableCrouch = false;
            }
        }

        public override Vec2 cameraPosition
        {
            get
            {
                Vec2 zero = Vec2.Zero;
                Vec2 cameraPosition = this.ragdoll == null ? (this._cooked == null ? (this._trapped == null ? base.cameraPosition : this._trapped.cameraPosition) : this._cooked.cameraPosition) : this.ragdoll.cameraPosition;
                if ((double)(this.cameraPositionOverride - this.position).length < 1000.0)
                    this.cameraPositionOverride = Vec2.Zero;
                if (this.cameraPositionOverride != Vec2.Zero)
                    return this.cameraPositionOverride;
                if (cameraPosition.y < -1000.0 || cameraPosition == Vec2.Zero || cameraPosition.x < -5000.0)
                    cameraPosition = this.prevCamPosition;
                else
                    this.prevCamPosition = cameraPosition;
                return cameraPosition;
            }
        }

        public override Vec2 anchorPosition => this.cameraPosition;

        public Thing followPart => this._followPart == null ? this : this._followPart;

        public bool underwater => this.doFloat && this._curPuddle != null && (double)this.top + 2.0 > (double)this._curPuddle.top;

        public void EmitBubbles(int num, float hVel)
        {
            if (!this.underwater)
                return;
            for (int index = 0; index < num; ++index)
                Level.Add(new TinyBubble(this.x + (this.offDir > 0 ? 6 : -6) * (this.sliding ? -1 : 1) + Rando.Float(-1f, 1f), this.top + 7f + Rando.Float(-1f, 1f), Rando.Float(hVel) * offDir, this._curPuddle.top + 7f));
        }

        public void MakeStars()
        {
            Level.Add(new DizzyStar(this.x + offDir * -3, this.y - 9f, new Vec2(Rando.Float(-0.8f, -1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(this.x + offDir * -3, this.y - 9f, new Vec2(Rando.Float(-0.8f, -1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(this.x + offDir * -3, this.y - 9f, new Vec2(Rando.Float(0.8f, 1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(this.x + offDir * -3, this.y - 9f, new Vec2(Rando.Float(0.8f, 1.5f), Rando.Float(0.5f, -1f))));
            Level.Add(new DizzyStar(this.x + offDir * -3, this.y - 9f, new Vec2(Rando.Float(-1.5f, 1.5f), Rando.Float(-0.5f, -1.1f))));
        }

        public static void MakeStars(Vec2 pPosition, Vec2 pVelocity)
        {
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(-1.7f, -1f) + new Vec2((float)((pVelocity.x + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(-0.7f, -0.5f) + new Vec2((float)((pVelocity.x + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(0.7f, -0.5f) + new Vec2((float)((pVelocity.x + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(1.7f, -1f) + new Vec2((float)((pVelocity.x + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f) - 1f), new Color(247, 224, 89)));
            Level.Add(new NewDizzyStar(pPosition.x + pVelocity.x * 2f, pPosition.y, new Vec2(0.0f, -1.4f) + new Vec2((float)((pVelocity.x + (double)Rando.Float(-0.5f, 0.5f)) * (double)Rando.Float(0.6f, 0.9f) / 2.0), Rando.Float(-0.5f, 0.0f) - 1f), new Color(247, 224, 89)));
        }

        public override bool active
        {
            get => this._active;
            set => this._active = value;
        }

        public bool chatting => this.profile != null && this.profile.netData.Get<bool>(nameof(chatting));

        public virtual void DuckUpdate()
        {
        }

        public override void SpecialNetworkUpdate()
        {
        }

        public override void OnGhostObjectAdded()
        {
            if (!this.isServerForObject)
                return;
            if (this._trappedInstance == null)
            {
                this._trappedInstance = new TrappedDuck(this.x, this.y - 9999f, this)
                {
                    active = false,
                    visible = false,
                    authority = (NetIndex8)80
                };
                if (!GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_trappedInstance);
                Level.Add(_trappedInstance);
                this.Fondle(_trappedInstance);
            }
            if (this._cookedInstance == null)
            {
                this._cookedInstance = new CookedDuck(this.x, this.y - 9999f)
                {
                    active = false,
                    visible = false,
                    authority = (NetIndex8)80
                };
                if (!GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_cookedInstance);
                Level.Add(_cookedInstance);
                if (this._profile.localPlayer)
                    this.Fondle(_cookedInstance);
            }
            if (this._ragdollInstance != null)
                return;
            this._ragdollInstance = new Ragdoll(this.x, this.y - 9999f, this, false, 0.0f, 0, Vec2.Zero)
            {
                npi = netProfileIndex
            };
            this._ragdollInstance.RunInit();
            this._ragdollInstance.active = false;
            this._ragdollInstance.visible = false;
            this._ragdollInstance.authority = (NetIndex8)80;
            Level.Add(_ragdollInstance);
            this.Fondle(_ragdollInstance);
        }

        private void RecoverServerControl()
        {
            Thing.Fondle(this, DuckNetwork.localConnection);
            Thing.Fondle(holdObject, DuckNetwork.localConnection);
            Thing.Fondle(_trappedInstance, DuckNetwork.localConnection);
            Thing.Fondle(_ragdollInstance, DuckNetwork.localConnection);
            foreach (Thing t in this._equipment)
                Thing.Fondle(t, DuckNetwork.localConnection);
        }

        public override void Update()
        {
            if (Network.isActive && this._trappedInstance != null && this._trappedInstance.ghostObject != null && !this._trappedInstance.ghostObject.IsInitialized())
                return;
            this.tilt = Lerp.FloatSmooth(this.tilt, 0.0f, 0.25f);
            this.verticalOffset = Lerp.FloatSmooth(this.verticalOffset, 0.0f, 0.25f);
            if (this.swordInvincibility > 0)
                --this.swordInvincibility;
            if ((this.ragdoll == null || this.ragdoll.tongueStuck == Vec2.Zero) && this.tongueCheck != Vec2.Zero && this.level.cold)
            {
                Block block = Level.CheckPoint<Block>(this.tongueCheck);
                if (block != null && block.physicsMaterial == PhysicsMaterial.Metal)
                {
                    this.GoRagdoll();
                    if (this.ragdoll != null)
                    {
                        this.ragdoll.tongueStuck = this.tongueCheck;
                        this.ragdoll.tongueStuckThing = block;
                        this.ragdoll.tongueShakes = 0;
                    }
                }
            }
            if (Network.isActive)
            {
                this.UpdateConnectionIndicators();
                if (this._profile == Profiles.EnvironmentProfile && this._netProfileIndex >= 0 && this._netProfileIndex < DG.MaxPlayers)
                    this.AssignNetProfileIndex((byte)this._netProfileIndex);
            }
            ++this.framesSinceRagdoll;
            if (this.killedByProfile != null)
                ++this.framesSinceKilled;
            int num = this.crouch ? 1 : 0;
            if (this._sprite == null)
                return;
            this.fancyShoes = this.HasEquipment(typeof(FancyShoes));
            if (this.isServerForObject && this.inputProfile != null)
            {
                if (NetworkDebugger.enabled)
                {
                    if (this.inputProfile.CheckCode(Level.core.konamiCode) || this.inputProfile.CheckCode(Level.core.konamiCodeAlternate))
                    {
                        this.position = this.cameraPosition;
                        this.Presto();
                    }
                }
                else if (this.inputProfile.CheckCode(Input.konamiCode) || this.inputProfile.CheckCode(Input.konamiCodeAlternate))
                {
                    this.position = this.cameraPosition;
                    this.Presto();
                }
            }
            if (this._disarmWait > 0)
                --this._disarmWait;
            if (this._disarmDisable > 0)
                --this._disarmDisable;
            if (killMultiplier > 0.0)
                this.killMultiplier -= 0.016f;
            else
                this.killMultiplier = 0.0f;
            if (this.isServerForObject && this.holdObject != null && this.holdObject.removeFromLevel)
                this.holdObject = null;
            if (Network.isActive)
            {
                if (this.isServerForObject)
                {
                    if (this._assignedIndex)
                    {
                        this._assignedIndex = false;
                        Thing.Fondle(this, DuckNetwork.localConnection);
                        if (this.holdObject != null)
                            Thing.PowerfulRuleBreakingFondle(this.holdObject, DuckNetwork.localConnection);
                        foreach (Equipment t in this._equipment)
                        {
                            if (t != null)
                                Thing.PowerfulRuleBreakingFondle(t, DuckNetwork.localConnection);
                        }
                    }
                    if (this.inputProfile != null && !this.manualQuackPitch)
                    {
                        float leftTrigger = this.inputProfile.leftTrigger;
                        if (this.inputProfile.hasMotionAxis)
                            leftTrigger += this.inputProfile.motionAxis;
                        this.quackPitch = (byte)((double)leftTrigger * byte.MaxValue);
                    }
                    ++Duck._framesSinceInput;
                    if (this.inputProfile != null && (this.inputProfile.Pressed("", true) || Level.current is RockScoreboard))
                    {
                        Duck._framesSinceInput = 0;
                        this.afk = false;
                    }
                    if (Duck._framesSinceInput > 1200)
                        this.afk = true;
                }
                else if (this.profile != null)
                {
                    if (this.disarmIndex != 9 && disarmIndex != _prevDisarmIndex && (_prevDisarmIndex == profile.networkIndex || this._prevDisarmIndex == 9) && this.disarmIndex >= 0 && this.disarmIndex < 8 && DuckNetwork.profiles[disarmIndex].connection == DuckNetwork.localConnection)
                        ++Global.data.disarms.valueInt;
                    this._prevDisarmIndex = this.disarmIndex;
                }
                if (this.isServerForObject)
                {
                    this.disarmIndexCooldown -= Maths.IncFrameTimer();
                    if (disarmIndexCooldown <= 0.0 && this.profile != null)
                    {
                        this.disarmIndexCooldown = 0.0f;
                        this.disarmIndex = this.profile.networkIndex;
                    }
                }
                if ((double)this.y > -999.0)
                    this._lastGoodPosition = this.position;
                if (Network.isActive)
                {
                    if (this._ragdollInstance != null)
                        this._ragdollInstance.captureDuck = this;
                    if (this.ragdoll != null && this.ragdoll.isServerForObject)
                    {
                        if (this._trapped != null && (double)this._trapped.y > -5000.0)
                        {
                            if (Network.isActive)
                            {
                                this.ragdoll.active = false;
                                this.ragdoll.visible = false;
                                this.ragdoll.owner = null;
                                if ((double)this.y > -1000.0)
                                {
                                    this.ragdoll.y = -9999f;
                                    if (this.ragdoll.part1 != null)
                                        this.ragdoll.part1.y = -9999f;
                                    if (this.ragdoll.part2 != null)
                                        this.ragdoll.part2.y = -9999f;
                                    if (this.ragdoll.part3 != null)
                                        this.ragdoll.part3.y = -9999f;
                                }
                            }
                            else
                                Level.Remove(this);
                            this.ragdoll = null;
                        }
                        if (this.ragdoll != null)
                        {
                            if ((double)this.ragdoll.y < -5000.0)
                            {
                                this.ragdoll.position = this.cameraPosition;
                                if (this.ragdoll.part1 != null)
                                    this.ragdoll.part1.position = this.cameraPosition;
                                if (this.ragdoll.part2 != null)
                                    this.ragdoll.part2.position = this.cameraPosition;
                                if (this.ragdoll.part3 != null)
                                    this.ragdoll.part3.position = this.cameraPosition;
                            }
                            if (this.ragdoll.part1 != null && this.ragdoll.part1.owner != null && (double)this.ragdoll.part1.owner.y < -5000.0)
                                this.ragdoll.part1.owner = null;
                            if (this.ragdoll.part2 != null && this.ragdoll.part2.owner != null && (double)this.ragdoll.part2.owner.y < -5000.0)
                                this.ragdoll.part2.owner = null;
                            if (this.ragdoll.part3 != null && this.ragdoll.part3.owner != null && (double)this.ragdoll.part3.owner.y < -5000.0)
                                this.ragdoll.part3.owner = null;
                        }
                    }
                    if (this._trapped != null && (double)this._trapped.y < -5000.0 && this._trapped.isServerForObject)
                        this._trapped.position = this.cameraPosition;
                    if (this._cooked != null && (double)this._cooked.y < -5000.0 && this._cooked.isServerForObject)
                        this._cooked.position = this.cameraPosition;
                }
                if (this._profile.localPlayer && !(this is RockThrowDuck) && this.isServerForObject)
                {
                    if (this.ragdoll == null && this._trapped == null && this._cooked == null && (double)this.y < -5000.0)
                        this.position = this.cameraPosition;
                    if (this._ragdollInstance != null)
                    {
                        if (this.ragdoll == null && (this._ragdollInstance.part1 != null && this._ragdollInstance.part1.owner != null || this._ragdollInstance.part2 != null && this._ragdollInstance.part2.owner != null || this._ragdollInstance.part3 != null && this._ragdollInstance.part3.owner != null))
                        {
                            Thing owner1 = this._ragdollInstance.part1.owner;
                            Thing owner2 = this._ragdollInstance.part2.owner;
                            Thing owner3 = this._ragdollInstance.part3.owner;
                            this.GoRagdoll();
                            if (owner1 != null)
                            {
                                this._ragdollInstance.connection = owner1.connection;
                                this._ragdollInstance.part1.owner = owner1;
                            }
                            if (owner2 != null)
                            {
                                this._ragdollInstance.connection = owner2.connection;
                                this._ragdollInstance.part2.owner = owner2;
                            }
                            if (owner3 != null)
                            {
                                this._ragdollInstance.connection = owner3.connection;
                                this._ragdollInstance.part3.owner = owner3;
                            }
                        }
                        if (this._ragdollInstance.visible)
                        {
                            this.ragdoll = this._ragdollInstance;
                        }
                        else
                        {
                            this._ragdollInstance.visible = true;
                            this._ragdollInstance.visible = false;
                            if (this._ragdollInstance.part1 != null)
                                this._ragdollInstance.part1.y = -9999f;
                            if (this._ragdollInstance.part2 != null)
                                this._ragdollInstance.part2.y = -9999f;
                            if (this._ragdollInstance.part3 != null)
                                this._ragdollInstance.part3.y = -9999f;
                            this.ragdoll = null;
                        }
                        if (this._cookedInstance != null)
                        {
                            if (this._cookedInstance.visible)
                            {
                                this._cooked = this._cookedInstance;
                                if (this._ragdollInstance != null)
                                {
                                    this._ragdollInstance.visible = false;
                                    this._ragdollInstance.active = false;
                                    this.ragdoll = null;
                                }
                            }
                            else
                            {
                                this._cooked = null;
                                this._cookedInstance.y = -9999f;
                            }
                        }
                    }
                }
            }
            if (this._profile.localPlayer && !(this is RockThrowDuck) && this.connection != DuckNetwork.localConnection && !this.CanBeControlled())
                this.RecoverServerControl();
            if (this._trappedInstance != null)
            {
                if (this._trappedInstance.visible || this._trappedInstance.owner != null)
                {
                    this._trapped = this._trappedInstance;
                }
                else
                {
                    this._trappedInstance.owner = null;
                    this._trapped = null;
                    this._trappedInstance.y = -9999f;
                }
            }
            if (this.profile != null && this.mindControl != null && Level.current is GameLevel)
                this.profile.stats.timeUnderMindControl += Maths.IncFrameTimer();
            if (this.underwater)
            {
                ++this._framesUnderwater;
                if (this._framesUnderwater >= 60)
                {
                    this._framesUnderwater = 0;
                    ++Global.data.secondsUnderwater.valueInt;
                }
                this._bubbleWait += Rando.Float(0.015f, 0.017f);
                if (Rando.Float(1f) > 0.99f)
                    this._bubbleWait += 0.5f;
                if (_bubbleWait > 1f)
                {
                    this._bubbleWait = Rando.Float(0.2f);
                    this.EmitBubbles(1, 1f);
                }
            }
            if (!this.quackStart && this.quack > 0)
            {
                this.quackStart = true;
                this.EmitBubbles(Rando.Int(3, 6), 1.2f);
                if (Level.current.cold && !this.underwater)
                    this.Breath();
            }
            if (this.quack <= 0)
                this.quackStart = false;
            ++this.wait;
            if (TeamSelect2.doCalc && this.wait > 10 && this.profile != null)
            {
                this.wait = 0;
                float profileScore = this.profile.endOfRoundStats.CalculateProfileScore();
                if (this.firstCalc)
                {
                    this.firstCalc = false;
                    this.lastCalc = profileScore;
                }
                if (Math.Abs(this.lastCalc - profileScore) > 0.005f)
                {
                    int c = (int)Math.Round((profileScore - lastCalc) / 0.005f);
                    if (this.plus == null || this.plus.removeFromLevel)
                    {
                        this.plus = new CoolnessPlus(this.x, this.y, this, c);
                        Level.Add(plus);
                    }
                    else
                        this.plus.change = c;
                }
                this.lastCalc = profileScore;
            }
            this.grappleMultiplier = !this.grappleMul ? 1f : 1.5f;
            ++this._timeSinceThrow;
            if (this._timeSinceThrow > 30)
                this._timeSinceThrow = 30;
            if (this._resetAction && !this.inputProfile.Down("SHOOT"))
                this._resetAction = false;
            if (this._converted == null)
            {
                this._sprite.texture = this.profile.persona.sprite.texture;
                this._spriteArms.texture = this.profile.persona.armSprite.texture;
                this._spriteQuack.texture = this.profile.persona.quackSprite.texture;
                this._spriteControlled.texture = this.profile.persona.controlledSprite.texture;
            }
            else
            {
                this._sprite.texture = this._converted.profile.persona.sprite.texture;
                this._spriteArms.texture = this._converted.profile.persona.armSprite.texture;
                this._spriteQuack.texture = this._converted.profile.persona.quackSprite.texture;
                this._spriteControlled.texture = this._converted.profile.persona.controlledSprite.texture;
            }
            --this.listenTime;
            if (this.listenTime < 0)
                this.listenTime = 0;
            if (this.listening && this.listenTime <= 0)
                this.listening = false;
            if (this.isServerForObject && !this.listening)
            {
                ++this.conversionResistance;
                if (this.conversionResistance > 100)
                    this.conversionResistance = 100;
            }
            //this._coolnessThisFrame = 0;
            this.UpdateBurning();
            this.UpdateGhostStatus();
            if (this.dead)
            {
                this.immobilized = true;
                if (unfocus > 0.0)
                    this.unfocus -= 0.015f;
                else if (!this.unfocused)
                {
                    if (!this.grounded && this._lives > 0)
                    {
                        IEnumerable<Thing> thing = Level.current.things[typeof(SpawnPoint)];
                        this.position = thing.ElementAt<Thing>(Rando.Int(thing.Count<Thing>() - 1)).position;
                    }
                    if (this.profile != null && this.profile.localPlayer && Level.current is TeamSelect2)
                    {
                        foreach (ProfileBox2 profile in (Level.current as TeamSelect2)._profiles)
                        {
                            if (profile.duck == this)
                            {
                                Thing.UnstoppableFondle(this, DuckNetwork.localConnection);
                                Vec2 vec2 = profile.position + new Vec2(82f, 58f);
                                if (!profile.rightRoom)
                                    vec2 = profile.position + new Vec2(58f, 58f);
                                this.position = vec2;
                                if (this._ragdollInstance != null)
                                {
                                    Thing.UnstoppableFondle(_ragdollInstance, DuckNetwork.localConnection);
                                    this._ragdollInstance.position = new Vec2(vec2.x, vec2.y - 3f);
                                    this._ragdollInstance.Unragdoll();
                                }
                                this.RecoverServerControl();
                                if (this.ragdoll != null)
                                    this.ragdoll.Unragdoll();
                                this.position = vec2;
                                SFX.PlaySynchronized("convert", 0.75f);
                                this.Ressurect();
                                if (Network.isActive && this.ghostObject != null)
                                    this.ghostObject.SuperDirtyStateMask();
                            }
                        }
                    }
                    else
                    {
                        Respawner respawner = Level.Nearest<Respawner>(this.position);
                        if (respawner != null && this.profile != null && this.profile.localPlayer)
                        {
                            if (this.ragdoll != null)
                                this.ragdoll.Unragdoll();
                            this.position = respawner.position + new Vec2(0.0f, -16f);
                            SFX.PlaySynchronized("respawn", 0.65f);
                            this.Ressurect();
                        }
                        else if (this._lives > 0)
                        {
                            --this._lives;
                            this.unfocus = 1f;
                            this._isGhost = true;
                            this.Regenerate();
                            this.immobilized = false;
                            this.crouch = false;
                            this.sliding = false;
                        }
                        else
                        {
                            this.unfocus = -1f;
                            this.unfocused = true;
                            if (this.isServerForObject)
                                this.visible = false;
                            if (!Network.isActive)
                                this.active = false;
                            if (Level.current.camera is FollowCam && !(Level.current is ChallengeLevel))
                                (Level.current.camera as FollowCam).Remove(this);
                            this.y -= 100000f;
                        }
                    }
                }
                this.sliding = true;
                this.crouch = true;
            }
            else if (this.quack > 0)
                this.profile.stats.timeWithMouthOpen += Maths.IncFrameTimer();
            if (DevConsole.rhythmMode && Level.current is GameLevel && (this.inputProfile.Pressed("DOWN") || this.inputProfile.Pressed("JUMP") || this.inputProfile.Pressed("SHOOT") || this.inputProfile.Pressed("QUACK") || this.inputProfile.Pressed("GRAB")) && !RhythmMode.inTime)
                this.GoRagdoll();
            --this._iceWedging;
            if (this._iceWedging < 0)
                this._iceWedging = 0;
            this.UpdateMove();
            if (this.inputProfile == null)
                return;
            if (this.sliding && this._iceWedging <= 0 && this.grounded && Level.CheckLine<Block>(this.position + new Vec2(-10f, 0.0f), this.position + new Vec2(10f, 0.0f)) != null)
            {
                foreach (IPlatform platform in Level.CheckPointAll<IPlatform>(new Vec2(this.position.x, this.bottom - 4f)))
                {
                    if (platform is Holdable)
                        this.sliding = false;
                }
            }
            if (this.ragdoll != null)
                this.ragdoll.UpdateUnragdolling();
            this.centerOffset = 8f;
            if (this.crouch)
                this.centerOffset = 24f;
            if (this.ragdoll == null && this.isServerForObject)
                base.Update();
            if (this.ragdoll == null && this._prevRagdoll != null)
            {
                Level.Add(SmallSmoke.New(this.x - Rando.Float(2f, 5f), (float)((double)this.y + (double)Rando.Float(-3f, 3f) + 16.0)));
                Level.Add(SmallSmoke.New(this.x + Rando.Float(2f, 5f), (float)((double)this.y + (double)Rando.Float(-3f, 3f) + 16.0)));
                Level.Add(SmallSmoke.New(this.x, (float)((double)this.y + (double)Rando.Float(-3f, 3f) + 16.0)));
            }
            this._prevRagdoll = this.ragdoll;
            if (kick > 0f)
                this.kick -= 0.1f;
            else
                this.kick = 0f;
            this._sprite.speed = (0.1f + Math.Abs(this.hSpeed) / maxrun * 0.1f);
            this._sprite.flipH = this.offDir < 0;
            if (!this.swinging)
                this.UpdateAnimation();
            if (this._trapped != null)
                this.SetCollisionMode("netted");
            else if (this._sprite.currentAnimation == "run" || this._sprite.currentAnimation == "jump" || this._sprite.currentAnimation == "idle")
                this.SetCollisionMode("normal");
            else if (this._sprite.currentAnimation == "slide")
                this.SetCollisionMode("normal");
            else if (this._sprite.currentAnimation == "crouch" || this._sprite.currentAnimation == "listening")
                this.SetCollisionMode("crouch");
            else if (this._sprite.currentAnimation == "groundSlide" || this._sprite.currentAnimation == "dead")
                this.SetCollisionMode("slide");
            Holdable holdObject = this.holdObject;
            if (this.holdObject != null && this.isServerForObject && (this.ragdoll == null || !this.fancyShoes) && !this.inPipe)
            {
                this.holdObject.isLocal = this.isLocal;
                this.holdObject.UpdateAction();
            }
            if (Network.isActive && this.holdObject != null && (this.holdObject.duck != this || !this.holdObject.active || !this.holdObject.visible || !this.holdObject.isServerForObject && !(this.holdObject is RagdollPart)) && this.isServerForObject)
                this.holdObject = null;
            if (this.tryGrabFrames > 0 && !this.inputProfile.Pressed("GRAB"))
            {
                --this.tryGrabFrames;
                this.TryGrab();
                if (this.holdObject != null)
                    this.tryGrabFrames = 0;
            }
            else
                this.tryGrabFrames = 0;
            this.UpdateThrow();
            this.doThrow = false;
            this.reverseThrow = false;
            this.UpdateHoldPosition();
            if (!this.isServerForObject)
                base.Update();
            this.forceFire = false;
            foreach (Equipment equipment in this._equipment)
                equipment.PositionOnOwner();
            this._gripped = false;
        }

        public override Thing realObject => this._trapped != null ? this._trapped : this;

        public bool protectedFromFire
        {
            get
            {
                if (this.holdObject != null && holdObject.heat < -0.05f || this.holstered != null && holstered.heat < -0.05f)
                    return true;
                return this.skewered != null && skewered.heat < -0.05f;
            }
        }

        public override void HeatUp(Vec2 location)
        {
            if (this.holdObject != null && holdObject.heat < -0.05f)
                this.holdObject.DoHeatUp(0.03f, location);
            else if (this.holstered != null && holstered.heat < -0.05f)
                this.holstered.DoHeatUp(0.03f, location);
            else if (this.skewered != null && skewered.heat < -0.05f)
                this.skewered.DoHeatUp(0.03f, location);
            base.HeatUp(location);
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (this.protectedFromFire)
                return false;
            this._burnTime -= 0.02f;
            if (!this.onFire)
            {
                if (!this.dead)
                {
                    if (Network.isActive)
                        this.Scream();
                    else
                        SFX.Play("quackYell0" + Change.ToString(Rando.Int(2) + 1), pitch: (Rando.Float(0.3f) - 0.3f));
                    SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                    if ((double)Rando.Float(1f) < 0.1f)
                        this.AddCoolness(-1);
                    Event.Log(new LitOnFireEvent(litBy?.responsibleProfile, this.profile));
                    ++this.profile.stats.timesLitOnFire;
                    if (Recorder.currentRecording != null)
                        Recorder.currentRecording.LogAction(9);
                    if (this.ragdoll == null)
                    {
                        for (int index = 0; index < 5; ++index)
                            Level.Add(SmallFire.New(Rando.Float(12f) - 6f, Rando.Float(16f) - 8f, 0f, 0f, stick: this));
                    }
                }
                this.onFire = true;
            }
            return true;
        }

        public virtual void UpdateHoldPosition(bool updateLerp = true)
        {
            if (this._sprite == null || this.y < -8000f)
                return;
            this.armOffY = 6f;
            this.armOffX = -3f * offDir;
            if (this.holdObject != null)
            {
                this.armOffY = 6f;
                this.armOffX = -2f * offDir;
            }
            this.holdOffX = 6f;
            this.holdOffY = -3f;
            if (this.holdObject != null)
            {
                this.holdObject._sleeping = false;
                if (this.holdObject.owner != this)
                    return;
                if (!this.onFire && holdObject.heat > 0.5f && this.holdObject.physicsMaterial == PhysicsMaterial.Metal)
                {
                    if (this._sizzle == null)
                        this._sizzle = SFX.Play("sizzle", 0.6f, looped: true);
                    this._handHeat += 0.016f;
                    if (_handHeat > 0.4f)
                    {
                        if (this.handSmokeWait <= 0)
                        {
                            Vec2 vec2 = new Vec2(this.armPosition.x + this.holdObject.handOffset.x * offDir, this.armPosition.y + this.holdObject.handOffset.y);
                            Level.Add(SmallSmoke.New(vec2.x, vec2.y, 0.8f, 1f));
                            this.handSmokeWait = 5;
                        }
                        --this.handSmokeWait;
                    }
                    if (_handHeat > 1.1f)
                    {
                        this._sizzle.Stop();
                        this.Scream();
                        this.ThrowItem();
                        this._handHeat = 0f;
                    }
                }
                else
                {
                    if (this._sizzle != null)
                    {
                        this._sizzle.Stop();
                        this._sizzle = null;
                    }
                    this._handHeat = 0.0f;
                }
                if (this._sprite.currentAnimation == "run")
                {
                    if (this._sprite.frame == 1)
                        ++this.holdOffY;
                    else if (this._sprite.frame == 2)
                    {
                        ++this.holdOffY;
                        --this.holdOffX;
                    }
                    else if (this._sprite.frame == 3)
                    {
                        ++this.holdOffY;
                        this.holdOffX -= 2f;
                    }
                    else if (this._sprite.frame == 4)
                    {
                        ++this.holdOffY;
                        --this.holdOffX;
                    }
                    else if (this._sprite.frame == 5)
                        ++this.holdOffY;
                }
                else if (this._sprite.currentAnimation == "jump")
                {
                    if (this._sprite.frame == 0)
                        ++this.holdOffY;
                    else if (this._sprite.frame == 2)
                        --this.holdOffY;
                }
            }
            else
            {
                if (this._sizzle != null)
                {
                    this._sizzle.Stop();
                    this._sizzle = null;
                }
                this._handHeat = 0.0f;
            }
            this.holdOffX *= offDir;
            if (this.holdObject == null || this.ragdoll != null && this.fancyShoes)
                return;
            this._spriteArms.angle = this.holdAngle;
            this._bionicArm.angle = this.holdAngle;
            if (this.gun != null)
                this.kick = this.gun.kick * 5f;
            if (this.holdObject is DrumSet)
                this.position = this.holdObject.position + new Vec2(0.0f, -12f);
            else
                this.holdObject.position = this.armPositionNoKick + this.holdObject.holdOffset + new Vec2(this.holdOffX, this.holdOffY) + new Vec2(2 * offDir, 0.0f);
            this.holdObject.CheckIfHoldObstructed();
            if (this.HasEquipment(typeof(Holster)))
            {
                Holster equipment = this.GetEquipment(typeof(Holster)) as Holster;
                if (!equipment.chained.value || equipment.containedObject == null)
                {
                    if (!this.isServerForObject)
                        this.holdObstructed = equipment.netRaise;
                    else if (this.holdObject != null && this.inputProfile.Down("UP") && this.holdObject.holsterable)
                        this.holdObstructed = true;
                }
            }
            if (!(this.holdObject is RagdollPart))
                this.holdObject.offDir = this.offDir;
            if (this._sprite.currentAnimation == "slide")
            {
                --this.holdOffY;
                ++this.holdOffX;
            }
            else if (this._sprite.currentAnimation == "crouch")
            {
                if (this.holdObject != null)
                    this.armOffY += 4f;
            }
            else if ((this._sprite.currentAnimation == "groundSlide" || this._sprite.currentAnimation == "dead") && this.holdObject != null)
                this.armOffY += 6f;
            this.UpdateHoldLerp(updateLerp);
            if (!(this.holdObject is DrumSet))
            {
                this.holdObject.position = this.HoldOffset(this.holdObject.holdOffset);
                if (!(this.holdObject is RagdollPart))
                    this.holdObject.angle = this.holdObject.handAngle + this.holdAngleOff;
            }
            double y = (double)this.holdObject.y;
        }

        public void UpdateHoldLerp(bool updateLerp = false, bool instant = false)
        {
            if (this.holdObject.canRaise && (this._hovering && this.holdObject.hoverRaise || this.holdObstructed || this.holdObject.keepRaised))
            {
                if (updateLerp)
                    this.holdAngleOff = Maths.LerpTowards(this.holdAngleOff, (float)-(1.5707964f * offDir) * this.holdObject.angleMul, instant ? 1f : this.holdObject.raiseSpeed * 2f);
                this.holdObject.raised = true;
            }
            else
            {
                if (updateLerp)
                    this.holdAngleOff = Maths.LerpTowards(this.holdAngleOff, 0f, instant ? 1f : (float)(holdObject.raiseSpeed * 2f * 2f));
                if (!this.holdObject.raised)
                    return;
                this.holdObject.raised = false;
            }
        }

        public Duck converted => this._converted;

        public void ConvertDuck(Duck to)
        {
            if (this._converted != to && to != null && to.profile != null)
                ++to.profile.stats.conversions;
            RumbleManager.AddRumbleEvent(this.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Short, RumbleFalloff.Short));
            this._converted = to;
            this._spriteArms = to._spriteArms.CloneMap();
            this._spriteControlled = to._spriteControlled.CloneMap();
            this._spriteQuack = to._spriteQuack.CloneMap();
            this._sprite = to._sprite.CloneMap();
            this.graphic = _sprite;
            if (!this.isConversionMessage)
            {
                Equipment equipment = this.GetEquipment(typeof(TeamHat));
                if (equipment != null)
                    this.Unequip(equipment);
                if (to.profile.team.hasHat)
                {
                    Hat e = new TeamHat(0.0f, 0.0f, to.profile.team, to.profile);
                    Level.Add(e);
                    this.Equip(e, false);
                }
            }
            for (int index = 0; index < 3; ++index)
                Level.Add(new MusketSmoke(this.x - 5f + Rando.Float(10f), (float)((double)this.y + 6.0 - 3.0 + (double)Rando.Float(6f) - index * 1.0))
                {
                    move = {
            x = (Rando.Float(0.4f) - 0.2f),
            y = (Rando.Float(0.4f) - 0.2f)
          }
                });
            this.listenTime = 0;
            this.listening = false;
            this.vSpeed -= 5f;
            SFX.Play("convert");
        }

        public void DoFuneralStuff()
        {
            Vec2 position = this.position;
            if (this.ragdoll != null)
                position = this.ragdoll.position;
            for (int index = 0; index < 3; ++index)
                Level.Add(new MusketSmoke(position.x - 5f + Rando.Float(10f), (float)(position.y + 6.0 - 3.0 + (double)Rando.Float(6f) - index * 1.0))
                {
                    move = {
            x = (Rando.Float(0.4f) - 0.2f),
            y = (Rando.Float(0.4f) - 0.2f)
          }
                });
            this._timeSinceFuneralPerformed = DateTime.Now;
            SFX.Play("death");
            ++this.profile.stats.funeralsRecieved;
        }

        public void LayToRest(Profile whoDid)
        {
            Vec2 position = this.position;
            if (this.ragdoll != null)
                position = this.ragdoll.position;
            if (!this.isConversionMessage)
            {
                Tombstone tombstone = new Tombstone(position.x, position.y);
                Level.Add(tombstone);
                tombstone.vSpeed = -2.5f;
            }
            this.DoFuneralStuff();
            if (this.ragdoll != null)
            {
                this.ragdoll.y += 10000f;
                this.ragdoll.part1.y += 10000f;
                this.ragdoll.part2.y += 10000f;
                this.ragdoll.part3.y += 10000f;
            }
            this.y += 10000f;
            if (whoDid == null)
                return;
            ++whoDid.stats.funeralsPerformed;
            whoDid.duck.AddCoolness(2);
        }

        public bool gripped
        {
            get => this._gripped;
            set => this._gripped = value;
        }

        public void UpdateLerp()
        {
            if ((double)this.lerpSpeed == 0.0)
                return;
            this.lerpPosition += this.lerpVector * this.lerpSpeed;
        }

        public bool IsQuacking()
        {
            if (this.quack > 0 || this._mindControl != null && this._derpMindControl)
                return true;
            return this.ragdoll != null && this.ragdoll.tongueStuck != Vec2.Zero;
        }

        public void DrawHat()
        {
            if (this.hat == null)
                return;
            if (this._sprite != null)
                this.hat.alpha = this._sprite.alpha;
            this.hat.offDir = this.offDir;
            this.hat.depth = this.depth + this.hat.equippedDepth;
            this.hat.angle = this.angle;
            this.hat.Draw();
            if (!DevConsole.showCollision)
                return;
            this.hat.DrawCollision();
        }

        public Vec2 GetPos()
        {
            Vec2 position = this.position;
            if (this.ragdoll != null && this.ragdoll.part1 != null)
                position = this.ragdoll.part1.position;
            else if (this._trapped != null)
                position = this._trapped.position;
            return position;
        }

        public Vec2 GetEdgePos()
        {
            Vec2 cameraPosition = this.cameraPosition;
            float num = 14f;
            if (cameraPosition.x < (double)Level.current.camera.left + (double)num)
                cameraPosition.x = Level.current.camera.left + num;
            if (cameraPosition.x > (double)Level.current.camera.right - (double)num)
                cameraPosition.x = Level.current.camera.right - num;
            if (cameraPosition.y < (double)Level.current.camera.top + (double)num)
                cameraPosition.y = Level.current.camera.top + num;
            if (cameraPosition.y > (double)Level.current.camera.bottom - (double)num)
                cameraPosition.y = Level.current.camera.bottom - num;
            return cameraPosition;
        }

        public bool ShouldDrawIcon()
        {
            Vec2 position = this.position;
            if (this.ragdoll != null)
            {
                if (this.ragdoll.part1 == null)
                    return false;
                position = this.ragdoll.part1.position;
            }
            else if (this._trapped != null)
                position = this._trapped.position;
            if (Network.isActive && this._trapped != null && this._trappedInstance != null && !this._trappedInstance.visible)
                position = this.position;
            if (Network.isActive && this.ragdoll != null && this._ragdollInstance != null && !this._ragdollInstance.visible)
                position = this.position;
            if (this._cooked != null)
                position = this._cooked.position;
            if (Network.isActive && this._cooked != null && this._cookedInstance != null && !this._cookedInstance.visible)
                position = this.position;
            if (position.x < (double)this.level.camera.left - 1000.0 || position.y < -3000.0)
                return false;
            float num = -6f;
            if (this.level != null && this.level.camera != null && !this.dead && !VirtualTransition.doingVirtualTransition)
            {
                switch (Level.current)
                {
                    case GameLevel _:
                    case ChallengeLevel _:
                        if (Level.current.simulatePhysics)
                            return position.x < (double)this.level.camera.left + (double)num || position.x > (double)this.level.camera.right - (double)num || position.y < (double)this.level.camera.top + (double)num || position.y > (double)this.level.camera.bottom - (double)num;
                        break;
                }
            }
            return false;
        }

        public void PrepareIconForFrame()
        {
            if (this.dead)
                return;
            RenderTarget2D iconMap = this.persona.iconMap;
            Viewport viewport = DuckGame.Graphics.viewport;
            RenderTarget2D renderTarget = DuckGame.Graphics.GetRenderTarget();
            DuckGame.Graphics.SetRenderTarget(iconMap);
            DuckGame.Graphics.viewport = new Viewport(0, 0, 96, 96);
            if (this._iconCamera == null)
                this._iconCamera = new Camera(0.0f, 0.0f, 48f, 48f);
            this._iconCamera.center = this.position + new Vec2(0.0f, 2f);
            if (this.crouch)
                this._iconCamera.centerY += 3f;
            if (this.sliding)
            {
                this._iconCamera.centerY += 6f;
                this._iconCamera.centerX -= offDir * 7;
            }
            if (this.ragdoll != null && this.ragdoll.part2 != null)
                this._iconCamera.center = this.ragdoll.part2.position - this.ragdoll.part2.velocity;
            if (this._trapped != null)
                this._iconCamera.center = this._trapped.position + new Vec2(0.0f, -5f);
            if (this._cooked != null)
                this._iconCamera.center = this._cooked.position + new Vec2(0.0f, -5f);
            Duck.renderingIcon = true;
            this._renderingDuck = true;
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, this._iconCamera.getMatrix());
            DuckGame.Graphics.DrawRect(this._iconCamera.rectangle, Colors.Transparent, (Depth)0.99f);
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.ResetSpanAdjust();
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, this._iconCamera.getMatrix());
            if (this._cooked != null && (!Network.isActive || this._cookedInstance != null && this._cookedInstance.visible))
                this._cooked.Draw();
            else if (this.ragdoll != null && this.ragdoll.part1 != null && this.ragdoll.part3 != null)
            {
                this.ragdoll.part1.Draw();
                this.ragdoll.part3.Draw();
                foreach (Thing thing in this._equipment)
                    thing.Draw();
            }
            else if (this._trapped != null)
            {
                this._trapped.Draw();
                foreach (Thing thing in this._equipment)
                    thing.Draw();
            }
            else
                this.Draw();
            if (this.onFire)
            {
                foreach (SmallFire smallFire in Level.current.things[typeof(SmallFire)])
                {
                    if (smallFire.stick != null && (smallFire.stick == this || smallFire.stick == this._trapped || this.ragdoll != null && (smallFire.stick == this.ragdoll.part1 || smallFire.stick == this.ragdoll.part2)))
                        smallFire.Draw();
                }
            }
            DuckGame.Graphics.screen.End();
            this._renderingDuck = false;
            Duck.renderingIcon = false;
            DuckGame.Graphics.SetRenderTarget(renderTarget);
            DuckGame.Graphics.viewport = viewport;
        }

        public void DrawIcon()
        {
            if (this.dead || this._iconCamera == null || this._renderingDuck || !this.ShouldDrawIcon())
                return;
            Vec2 position = this.position;
            if (this.ragdoll != null)
                position = this.ragdoll.part1.position;
            else if (this._trapped != null)
                position = this._trapped.position;
            Vec2 p2 = position;
            float num1 = (float)((double)Level.current.camera.width / 320.0 * 0.5);
            float num2 = 0.75f;
            float num3 = 22f * num2;
            Vec2 vec2_1 = new Vec2(0.0f, 0.0f);
            if (position.x < (double)Level.current.camera.left + (double)num3)
            {
                vec2_1.x = Math.Abs(Level.current.camera.left - position.x);
                position.x = Level.current.camera.left + num3;
            }
            if (position.x > (double)Level.current.camera.right - (double)num3)
            {
                vec2_1.x = Math.Abs(Level.current.camera.right - position.x);
                position.x = Level.current.camera.right - num3;
            }
            if (position.y < (double)Level.current.camera.top + (double)num3)
            {
                vec2_1.y = Math.Abs(Level.current.camera.top - position.y);
                position.y = Level.current.camera.top + num3;
            }
            if (position.y > (double)Level.current.camera.bottom - (double)num3)
            {
                vec2_1.y = Math.Abs(Level.current.camera.bottom - position.y);
                position.y = Level.current.camera.bottom - num3;
            }
            Vec2 vec2_2 = vec2_1 * (3f / 1000f);
            float num4 = num2 - Math.Min(vec2_2.length, 1f) * 0.4f;
            DuckGame.Graphics.Draw(persona.iconMap, position, new Rectangle?(this._iconRect), Color.White, 0.0f, new Vec2(48f, 48f), new Vec2(0.5f, 0.5f) * num4, SpriteEffects.None, (Depth)(0.9f + this.depth.span));
            int imageIndex = this._sprite.imageIndex;
            this._sprite.imageIndex = 21;
            float rad = Maths.DegToRad(Maths.PointDirection(position, p2));
            this._sprite.depth = (Depth)0.8f;
            this._sprite.angle = -rad;
            this._sprite.flipH = false;
            this._sprite.UpdateSpriteBox();
            this._sprite.position = new Vec2(position.x + (float)Math.Cos((double)rad) * 12f, position.y - (float)Math.Sin((double)rad) * 12f);
            this._sprite.DrawWithoutUpdate();
            this._sprite.angle = 0.0f;
            this._sprite.imageIndex = imageIndex;
            this._sprite.UpdateSpriteBox();
        }

        public Color blendColor
        {
            set
            {
                this._spriteArms.color = value;
                this._spriteControlled.color = value;
                this._spriteQuack.color = value;
                this._sprite.color = value;
            }
        }

        public virtual void OnDrawLayer(Layer pLayer)
        {
            if (this._sprite == null || !this.localSpawnVisible || !this.ShouldDrawIcon())
                return;
            if (pLayer == Layer.PreDrawLayer)
            {
                this.PrepareIconForFrame();
            }
            else
            {
                if (pLayer != Layer.Foreground)
                    return;
                this.DrawIcon();
            }
        }

        public override void Draw()
        {
            if (this._sprite == null || !this.localSpawnVisible)
                return;
            if (this.inNet)
            {
                this.DrawHat();
            }
            else
            {
                if (DevConsole.showCollision)
                    DuckGame.Graphics.DrawRect(this._featherVolume.rectangle, Color.LightGreen, (Depth)0.6f, false, 0.5f);
                int num1 = this._renderingDuck ? 1 : 0;
                bool flag1 = false;
                if (Network.isActive)
                {
                    if (this._trappedInstance != null && this._trappedInstance.visible)
                        flag1 = true;
                    if (this._ragdollInstance != null && this._ragdollInstance.visible)
                        flag1 = true;
                    if (this._cookedInstance != null && this._cookedInstance.visible)
                        flag1 = true;
                }
                Depth depth = this.depth;
                if (!flag1)
                {
                    if (!this._renderingDuck)
                    {
                        if (!this._updatedAnimation)
                            this.UpdateAnimation();
                        this._updatedAnimation = false;
                        this._sprite.UpdateFrame();
                    }
                    this._sprite.flipH = this.offDir < 0;
                    if (this.enteringWalldoor)
                        this.depth = -0.55f;
                    this._spriteArms.depth = this.depth + 11;
                    this._bionicArm.depth = this.depth + 11;
                    //this.DrawAIPath();
                    SpriteMap spriteQuack = this._spriteQuack;
                    SpriteMap spriteControlled = this._spriteControlled;
                    SpriteMap sprite = this._sprite;
                    SpriteMap spriteArms = this._spriteArms;
                    double num2 = this._isGhost ? 0.5 : 1.0;
                    double alpha = (double)this.alpha;
                    double num3;
                    float num4 = (float)(num3 = num2 * alpha);
                    spriteArms.alpha = (float)num3;
                    double num5;
                    float num6 = (float)(num5 = (double)num4);
                    sprite.alpha = (float)num5;
                    double num7;
                    float num8 = (float)(num7 = (double)num6);
                    spriteControlled.alpha = (float)num7;
                    double num9 = (double)num8;
                    spriteQuack.alpha = (float)num9;
                    this._spriteQuack.flipH = this._spriteControlled.flipH = this._sprite.flipH;
                    this._spriteControlled.depth = this.depth;
                    this._sprite.depth = this.depth;
                    this._spriteQuack.depth = this.depth;
                    this._sprite.angle = this._spriteQuack.angle = this._spriteControlled.angle = this.angle;
                    if (this.ragdoll != null && this.ragdoll.tongueStuck != Vec2.Zero)
                        this.quack = 10;
                    if (this.IsQuacking())
                    {
                        Vec2 tounge = this.tounge;
                        if (this.sliding)
                        {
                            if (tounge.y < 0.0)
                                tounge.y = 0.0f;
                            if (this.offDir > 0)
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
                            if (this.offDir > 0 && tounge.x < 0.0)
                                tounge.x = 0.0f;
                            if (this.offDir < 0 && tounge.x > 0.0)
                                tounge.x = 0.0f;
                            if (tounge.y < -0.3f)
                                tounge.y = -0.3f;
                            if (tounge.y > 0.4f)
                                tounge.y = 0.4f;
                        }
                        this._stickLerp = Lerp.Vec2Smooth(this._stickLerp, tounge, 0.2f);
                        this._stickSlowLerp = Lerp.Vec2Smooth(this._stickSlowLerp, tounge, 0.1f);
                        Vec2 stickLerp = this._stickLerp;
                        stickLerp.y *= -1f;
                        Vec2 stickSlowLerp = this._stickSlowLerp;
                        stickSlowLerp.y *= -1f;
                        int num10 = 0;
                        double length = (double)stickLerp.length;
                        if (length > 0.5)
                            num10 = 72;
                        DuckGame.Graphics.Draw(this._mindControl == null || !this._derpMindControl ? this._spriteQuack : this._spriteControlled, this._sprite.imageIndex + num10, this.x, this.y + this.verticalOffset, this.xscale, this.yscale);
                        if (length > 0.05f)
                        {
                            Vec2 vec2_1 = this.position + new Vec2(0.0f, 1f);
                            if (this.sliding)
                            {
                                vec2_1.y += 9f;
                                vec2_1.x -= 4 * offDir;
                            }
                            else if (this.crouch)
                                vec2_1.y += 4f;
                            else if (!this.grounded)
                                vec2_1.y -= 2f;
                            List<Vec2> vec2List = Curve.Bezier(8, vec2_1, vec2_1 + stickSlowLerp * 6f, vec2_1 + stickLerp * 6f);
                            Vec2 vec2_2 = Vec2.Zero;
                            float num11 = 1f;
                            foreach (Vec2 p2 in vec2List)
                            {
                                if (vec2_2 != Vec2.Zero)
                                {
                                    Vec2 vec2_3 = vec2_2 - p2;
                                    DuckGame.Graphics.DrawTexturedLine(DuckGame.Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2, new Color(223, 30, 30), 0.15f * num11, this.depth + 1);
                                    DuckGame.Graphics.DrawTexturedLine(DuckGame.Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2 - vec2_3.normalized * 0.4f, Color.Black, 0.3f * num11, this.depth - 1);
                                }
                                num11 -= 0.1f;
                                vec2_2 = p2;
                                this.tongueCheck = p2;
                            }
                            if (this._graphic != null)
                            {
                                this._spriteQuack.position = this.position;
                                this._spriteQuack.alpha = this.alpha;
                                this._spriteQuack.angle = this.angle;
                                this._spriteQuack.depth = this.depth + 2;
                                this._spriteQuack.scale = this.scale;
                                this._spriteQuack.center = this.center;
                                this._spriteQuack.frame += 36;
                                this._spriteQuack.Draw();
                                this._spriteQuack.frame -= 36;
                            }
                        }
                        else
                            this.tongueCheck = Vec2.Zero;
                    }
                    else
                    {
                        DuckGame.Graphics.DrawWithoutUpdate(this._sprite, this.x, this.y + this.verticalOffset, this.xscale, this.yscale);
                        this._stickLerp = Vec2.Zero;
                        this._stickSlowLerp = Vec2.Zero;
                    }
                }
                if (this._renderingDuck)
                {
                    if (this.holdObject != null)
                        this.holdObject.Draw();
                    foreach (Thing thing in this._equipment)
                        thing.Draw();
                }
                if (this._mindControl != null && this._derpMindControl || this.listening)
                {
                    this._swirlSpin += 0.2f;
                    this._swirl.angle = this._swirlSpin;
                    DuckGame.Graphics.Draw(this._swirl, this.x, this.y - 12f);
                }
                this.DrawHat();
                if (!flag1)
                {
                    Grapple equipment = this.GetEquipment(typeof(Grapple)) as Grapple;
                    bool flag2 = equipment != null;
                    int num12 = 0;
                    if (equipment != null && equipment.hookInGun)
                        num12 = 36;
                    this._spriteArms.imageIndex = this._sprite.imageIndex;
                    if (!this.inNet && !this._gripped && !this.listening)
                    {
                        Vec2 vec2 = Vec2.Zero;
                        if (this.gun != null)
                            vec2 = -this.gun.barrelVector * this.kick;
                        float num13 = Math.Abs((float)((_flapFrame - 4.0) / 4.0)) - 0.1f;
                        if (!this._hovering)
                            num13 = 0.0f;
                        this._spriteArms._frameInc = 0.0f;
                        this._spriteArms.flipH = this._sprite.flipH;
                        if (this.holdObject != null && !this.holdObject.ignoreHands && !this.holdObject.hideRightWing)
                        {
                            this._spriteArms.angle = this.holdAngle;
                            this._bionicArm.angle = this.holdAngle;
                            if (!flag2)
                            {
                                bool flipH = this._spriteArms.flipH;
                                if (this.holdObject.handFlip)
                                    this._spriteArms.flipH = !this._spriteArms.flipH;
                                DuckGame.Graphics.Draw(this._spriteArms, this._sprite.imageIndex + 18 + Maths.Int(this.action) * 18 * (this.holdObject.hasTrigger ? 1 : 0), this.armPosition.x + this.holdObject.handOffset.x * offDir, this.armPosition.y + this.holdObject.handOffset.y, this._sprite.xscale, this._sprite.yscale);
                                this._spriteArms._frameInc = 0.0f;
                                this._spriteArms.flipH = flipH;
                                if (this._sprite.currentAnimation == "jump")
                                {
                                    this._spriteArms.angle = 0.0f;
                                    this._spriteArms.depth = this.depth + -10;
                                    DuckGame.Graphics.Draw(this._spriteArms, this._sprite.imageIndex + 5 + (int)Math.Round((double)num13 * 2.0), (float)((double)this.x + vec2.x + 2 * offDir * (double)this.xscale), (float)((double)this.y + vec2.y + armOffY * (double)this.yscale), -this._sprite.xscale, this._sprite.yscale, true);
                                    this._spriteArms.depth = this.depth + 11;
                                }
                            }
                            else
                            {
                                this._bionicArm.flipH = this._sprite.flipH;
                                if (this.holdObject.handFlip)
                                    this._bionicArm.flipH = !this._bionicArm.flipH;
                                DuckGame.Graphics.Draw(this._bionicArm, this._sprite.imageIndex + 18 + num12, this.armPosition.x + this.holdObject.handOffset.x * offDir, this.armPosition.y + this.holdObject.handOffset.y, this._sprite.xscale, this._sprite.yscale);
                            }
                        }
                        else if (!this._closingEyes)
                        {
                            if (!flag2)
                            {
                                this._spriteArms.angle = 0.0f;
                                if (this._sprite.currentAnimation == "jump" && this._spriteArms.imageIndex == 9)
                                {
                                    int num14 = 2;
                                    if (this.HasEquipment(typeof(ChestPlate)))
                                        num14 = 3;
                                    if (this.holdObject == null || !this.holdObject.hideRightWing)
                                    {
                                        this._spriteArms.depth = this.depth + 11;
                                        DuckGame.Graphics.Draw(this._spriteArms, this._spriteArms.imageIndex + 5 + (int)Math.Round((double)num13 * 2.0), (float)((double)this.x + vec2.x - offDir * num14 * (double)this.xscale), (float)((double)this.y + vec2.y + armOffY * (double)this.yscale), this._sprite.xscale, this._sprite.yscale, true);
                                        this._spriteArms.depth = this.depth + -10;
                                    }
                                    if (this.holdObject == null || !this.holdObject.hideLeftWing)
                                    {
                                        this._spriteArms.imageIndex = 9;
                                        DuckGame.Graphics.Draw(this._spriteArms, this._spriteArms.imageIndex + 5 + (int)Math.Round((double)num13 * 2.0), (float)((double)this.x + vec2.x + 2 * offDir * (double)this.xscale), (float)((double)this.y + vec2.y + armOffY * (double)this.yscale), -this._sprite.xscale, this._sprite.yscale, true);
                                        this._spriteArms.depth = this.depth + 11;
                                    }
                                }
                                else if (this.holdObject == null || !this.holdObject.hideRightWing)
                                    DuckGame.Graphics.Draw(this._spriteArms, this._sprite.imageIndex, this.armPosition.x, this.armPosition.y, this._sprite.xscale, this._sprite.yscale);
                            }
                            else
                            {
                                this._bionicArm.angle = 0.0f;
                                this._bionicArm.flipH = this._sprite.flipH;
                                DuckGame.Graphics.Draw(this._bionicArm, this._sprite.imageIndex + num12, this.armPosition.x, this.armPosition.y, this._sprite.xscale, this._sprite.yscale);
                            }
                        }
                    }
                }
                if (Network.isActive && !this._renderingDuck)
                    this.DrawConnectionIndicators();
                Sprite graphic = this.graphic;
                this.graphic = null;
                base.Draw();
                this.graphic = graphic;
                if (!this.enteringWalldoor)
                    return;
                this.depth = depth;
            }
        }

        public void UpdateConnectionIndicators()
        {
            if (this._indicators == null)
                this._indicators = new Duck.ConnectionIndicators()
                {
                    duck = this
                };
            this._indicators.Update();
        }

        public void DrawConnectionIndicators()
        {
            if (this._indicators == null)
                this._indicators = new Duck.ConnectionIndicators()
                {
                    duck = this
                };
            this._indicators.Draw();
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
            private List<Duck.ConnectionIndicators.Indicator> _indicators;
            private int numProblems;
            private static Sprite _rainbowGradient;

            public ConnectionIndicators()
            {
                List<Duck.ConnectionIndicators.Indicator> indicatorList = new List<Duck.ConnectionIndicators.Indicator>();
                SpriteMap pSprite1 = new SpriteMap("lagturtle", 16, 16, 3)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite1)
                {
                    problem = Duck.ConnectionTrouble.AFK
                });
                SpriteMap pSprite2 = new SpriteMap("lagturtle", 16, 16, 4)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite2)
                {
                    problem = Duck.ConnectionTrouble.Chatting
                });
                SpriteMap pSprite3 = new SpriteMap("lagturtle", 16, 16, 2)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite3)
                {
                    problem = Duck.ConnectionTrouble.Disconnection
                });
                SpriteMap pSprite4 = new SpriteMap("lagturtle", 16, 16, 0)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite4)
                {
                    problem = Duck.ConnectionTrouble.Lag
                });
                SpriteMap pSprite5 = new SpriteMap("lagturtle", 16, 16, 1)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite5)
                {
                    problem = Duck.ConnectionTrouble.Loss
                });
                SpriteMap pSprite6 = new SpriteMap("lagturtle", 16, 16, 8)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite6)
                {
                    problem = Duck.ConnectionTrouble.Minimized
                });
                SpriteMap pSprite7 = new SpriteMap("lagturtle", 16, 16, 7)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite7)
                {
                    problem = Duck.ConnectionTrouble.Spectator
                });
                SpriteMap pSprite8 = new SpriteMap("lagturtle", 16, 16, 9)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite8)
                {
                    problem = Duck.ConnectionTrouble.Paused
                });
                SpriteMap pSprite9 = new SpriteMap("lagturtle", 16, 16, 10)
                {
                    center = new Vec2(8f)
                };
                indicatorList.Add(new Duck.ConnectionIndicators.Indicator(pSprite9)
                {
                    problem = Duck.ConnectionTrouble.DevConsole
                });
                this._indicators = indicatorList;
                // ISSUE: explicit constructor call
                // base.\u002Ector(); wtf weird
                foreach (Duck.ConnectionIndicators.Indicator indicator in this._indicators)
                    indicator.owner = this;
            }

            public void Update()
            {
                this.numProblems = 0;
                foreach (Duck.ConnectionIndicators.Indicator indicator in this._indicators)
                {
                    indicator.Update();
                    if (indicator.visible)
                        ++this.numProblems;
                }
            }

            public void Draw()
            {
                if (duck.position.x < -4000.0)
                    return;
                if (Duck.ConnectionIndicators._rainbowGradient == null)
                    Duck.ConnectionIndicators._rainbowGradient = new Sprite("rainbowGradient");
                if (this.numProblems <= 0)
                    return;
                float num1 = 37f;
                float num2 = (this.numProblems - 1) * num1;
                Vec2 vec2_1 = new Vec2(-1000f, -1000f);
                Vec2 pPos = this.duck.cameraPosition + new Vec2(0.0f, 6f);
                float num3 = numProblems / 5f;
                int num4 = 0;
                float num5 = -20f;
                bool flag = false;
                Vec2 vec2_2 = Vec2.Zero;
                foreach (Duck.ConnectionIndicators.Indicator indicator in this._indicators)
                {
                    if (indicator.visible)
                    {
                        double deg = -(double)num2 / 2.0 + num4 * (double)num1;
                        float x = (float)-(Math.Sin((double)Maths.DegToRad((float)deg)) * (double)num5);
                        float y = (float)Math.Cos((double)Maths.DegToRad((float)deg)) * num5;
                        Vec2 pOffset = new Vec2(x, y);
                        indicator.Draw(pPos, pOffset);
                        if (flag)
                        {
                            pOffset = pPos + pOffset;
                            Vec2 normalized = (pOffset - vec2_2).normalized;
                            DuckGame.Graphics.DrawTexturedLine(Duck.ConnectionIndicators._rainbowGradient.texture, vec2_2 - normalized, pOffset + normalized, Color.White * num3, (0.3f + 0.6f * num3), (Depth)0.85f);
                        }
                        flag = true;
                        vec2_2 = new Vec2(pPos.x + x, pPos.y + y);
                        ++num4;
                    }
                }
            }

            private class Indicator
            {
                public Duck.ConnectionTrouble problem;
                public SpriteMap sprite;
                public float bloop;
                public Duck.ConnectionIndicators owner;
                public float wait;
                public float activeLerp;
                public bool _prevActive;
                private Vec2 drawPos = Vec2.Zero;

                public bool noWait => this.problem == Duck.ConnectionTrouble.Chatting || this.problem == Duck.ConnectionTrouble.AFK || this.problem == Duck.ConnectionTrouble.Minimized;

                public bool active
                {
                    get
                    {
                        if (this.owner.duck.connection == null || this.owner.duck.profile == null)
                            return false;
                        if (this.problem == Duck.ConnectionTrouble.Chatting)
                            return this.owner.duck.chatting;
                        if (this.problem == Duck.ConnectionTrouble.AFK)
                            return this.owner.duck.afk;
                        if (this.problem == Duck.ConnectionTrouble.Disconnection)
                            return this.owner.duck.connection != DuckNetwork.localConnection && this.owner.duck.connection.isExperiencingConnectionTrouble;
                        if (this.problem == Duck.ConnectionTrouble.Lag)
                            return this.owner.duck.connection != DuckNetwork.localConnection && (double)this.owner.duck.connection.manager.ping > 0.25;
                        if (this.problem == Duck.ConnectionTrouble.Loss)
                            return this.owner.duck.connection != DuckNetwork.localConnection && this.owner.duck.connection.manager.accumulatedLoss > 10;
                        if (this.problem == Duck.ConnectionTrouble.Minimized)
                            return !this.owner.duck.profile.netData.Get<bool>("gameInFocus", true);
                        if (this.problem == Duck.ConnectionTrouble.Paused)
                            return this.owner.duck.profile.netData.Get<bool>("gamePaused", false);
                        return this.problem == Duck.ConnectionTrouble.DevConsole && this.owner.duck.profile.netData.Get<bool>("consoleOpen", false);
                    }
                }

                public bool visible => activeLerp > 0.0;

                public void Update()
                {
                    bool active = this.active;
                    if (active != this._prevActive)
                    {
                        this._prevActive = active;
                        if (active)
                            this.bloop = 1f;
                        if (this.problem == Duck.ConnectionTrouble.Chatting || this.problem == Duck.ConnectionTrouble.Minimized || this.problem == Duck.ConnectionTrouble.Paused || this.problem == Duck.ConnectionTrouble.DevConsole || this.problem == Duck.ConnectionTrouble.AFK)
                            SFX.Play("rainpop", 0.65f, Rando.Float(-0.1f, 0.1f));
                    }
                    if (!active)
                    {
                        this.wait = Lerp.Float(this.wait, 0.0f, 0.03f);
                        if (this.noWait)
                            this.wait = 0.0f;
                        if (wait <= 0.0)
                        {
                            if (this.sprite.currentAnimation != "pop")
                                this.sprite.SetAnimation("pop");
                            else if (this.sprite.finished)
                            {
                                this.sprite.SetAnimation("idle");
                                this.activeLerp = 0.0f;
                            }
                        }
                    }
                    else
                    {
                        this.sprite.SetAnimation("idle");
                        this.wait = 1f;
                        this.activeLerp = 1f;
                    }
                    this.bloop = Lerp.FloatSmooth(this.bloop, 0f, 0.21f);
                    if (bloop >= 0.1f)
                        return;
                    this.bloop = 0.0f;
                }

                public void Draw(Vec2 pPos, Vec2 pOffset)
                {
                    if ((double)(this.drawPos - pOffset).length > 16f)
                        this.drawPos = pOffset;
                    this.drawPos = Lerp.Vec2Smooth(this.drawPos, pOffset, 0.4f);
                    this.sprite.scale = new Vec2((float)(1.0 + bloop * 0.6f), (float)(1.0 + bloop * 0.35f));
                    this.sprite.depth = (Depth)0.9f;
                    DuckGame.Graphics.Draw(sprite, pPos.x + this.drawPos.x, pPos.y + this.drawPos.y);
                }

                public Indicator(SpriteMap pSprite)
                {
                    this.sprite = pSprite;
                    this.sprite.AddAnimation("idle", 1f, true, this.sprite.frame);
                    this.sprite.AddAnimation("pop", 0.4f, false, 5, 6);
                }
            }
        }
    }
}
