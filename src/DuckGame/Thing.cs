// Decompiled with JetBrains decompiler
// Type: DuckGame.Thing
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace DuckGame
{
    /// <summary>
    /// The base class for everything in Duck Game. Things can be added to the world
    /// with Level.Add and they will be drawn and updated automatically.
    /// </summary>
    public abstract class Thing : Transform
    {
        public int maxPlaceable = -1;
        public GhostPriority syncPriority;
        public ushort _ghostType = 1;
        public bool isLocal = true;
        public ushort specialSyncIndex;
        private int _networkSeed;
        private bool _initializedNetworkSeed;
        protected NetIndex8 _authority = (NetIndex8)2;
        public NetIndex16 _lastAuthorityChange;
        protected NetworkConnection _connection;
        protected int _framesSinceTransfer = 999;
        private int _networkDrawIndex;
        protected bool _isStateObject;
        protected bool _isStateObjectInitialized;
        public Dictionary<NetworkConnection, uint> currentTick = new Dictionary<NetworkConnection, uint>();
        public bool inPipe;
        private static ushort _staticGlobalIndex = 0;
        private static ushort _staticPhysicsIndex = 0;
        private ushort _globalIndex = Thing.GetGlobalIndex();
        protected ushort _physicsIndex;
        private Vec2 _lerpPosition = Vec2.Zero;
        private Vec2 _lerpVector = Vec2.Zero;
        private float _lerpSpeed;
        private Portal _portal;
        protected SequenceItem _sequence;
        protected string _type = "";
        protected Level _level;
        protected float _lastTeleportDirection;
        private bool _removeFromLevel;
        protected bool _placed;
        protected bool _canBeGrouped;
        public float overfollow;
        protected Thing _owner;
        public Thing _prevOwner;
        protected Thing _lastThrownBy;
        protected bool _opaque;
        protected bool _opacityFromGraphic;
        protected Sprite _graphic;
        private DamageMap _damageMap;
        public bool lowLighting;
        private bool _visible = true;
        private Material _material;
        protected bool _enablePhysics = true;
        protected Profile _responsibleProfile;
        private System.Type _killThingType;
        public float _hSpeed;
        public float _vSpeed;
        protected bool _active = true;
        public bool serverOnly;
        private bool _action;
        private Anchor _anchor;
        public sbyte _offDir = 1;
        protected Layer _layer;
        protected bool _initialized;
        protected int _placementCost = 4;
        protected string _editorName = "";
        public Layer placementLayerOverride;
        public bool _canFlip = true;
        protected bool _canFlipVert;
        protected bool _canHaveChance = true;
        protected float _likelyhoodToExist = 1f;
        protected bool _editorCanModify = true;
        protected bool _processedByEditor;
        protected bool _visibleInGame = true;
        private Vec2 _editorOffset;
        private WallHug _hugWalls;
        protected bool _editorImageCenter;
        public static LevelData loadingLevel = null;
        private bool _isAccessible = true;
        public System.Type editorCycleType;
        protected bool _flipHorizontal;
        protected bool _flipVertical;
        private int _chanceGroup = -1;
        public string editorTooltip;
        /// <summary>
        /// Adding context menu item names to this filters out 'EditorProperty' values with the same name.
        /// This is useful for removing undesired inherited EditorProperty members from the right click menu.
        /// </summary>
        protected HashSet<string> _contextMenuFilter = new HashSet<string>();
        public static Effect _alphaTestEffect;
        private bool _skipPositioning;
        private static Dictionary<System.Type, Sprite> _editorIcons = new Dictionary<System.Type, Sprite>();
        protected Sprite _editorIcon;
        protected bool _solid = true;
        protected Vec2 _collisionOffset;
        protected Vec2 _collisionSize;
        protected float _topQuick;
        protected float _bottomQuick;
        protected float _leftQuick;
        protected float _rightQuick;
        protected bool _isStatic;
        public static bool skipLayerAdding = false;
        private bool _networkInitialized;
        public int forceEditorGrid;
        public int wasSuperFondled;
        public ProfileNetData _netData;
        public bool isBitBufferCreatedGhostThing;
        private GhostObject _ghostObject;
        public NetIndex16 fixedGhostIndex;
        private bool _ignoreGhosting;
        public Vec2 prevEndVelocity = Vec2.Zero;
        private bool _redoLayer;

        /// <summary>
        /// Gets the path to an asset that the mod that this Thing is a part of.
        /// </summary>
        /// <param name="asset">The asset name, relative to the mods' Content folder.</param>
        /// <returns>The path.</returns>
        public string GetPath(string asset) => ModLoader._modAssemblies[this.GetType().Assembly].configuration.contentDirectory + asset.Replace('\\', '/');

        /// <summary>Gets the path to an asset from a mod.</summary>
        /// <typeparam name="T">The mod type to fetch from</typeparam>
        /// <param name="asset">The asset name, relative to the mods' Content folder.</param>
        /// <returns>The path.</returns>
        public static string GetPath<T>(string asset) where T : Mod => Mod.GetPath<T>(asset);

        public virtual Vec2 netPosition
        {
            get => this.position;
            set => this.position = value;
        }

        public ushort ghostType
        {
            get => !this._removeFromLevel ? this._ghostType : (ushort)0;
            set
            {
                if (_ghostType == value)
                    return;
                this._ghostType = value;
            }
        }

        public int networkSeed
        {
            get
            {
                if (Network.isServer && !this._initializedNetworkSeed && this.isStateObject)
                {
                    this._networkSeed = Rando.Int(2147483646);
                    this._initializedNetworkSeed = true;
                }
                return this._networkSeed;
            }
            set
            {
                this._networkSeed = value;
                this._initializedNetworkSeed = true;
            }
        }

        public virtual NetIndex8 authority
        {
            get => this._authority;
            set
            {
                if (this._authority != value)
                    this._lastAuthorityChange = Network.synchronizedTime;
                this._authority = value;
            }
        }

        public virtual NetworkConnection connection
        {
            get => this._connection;
            set
            {
                if (value != this._connection && this.ghostObject != null)
                {
                    this.ghostObject.KillNetworkData();
                    if (value == DuckNetwork.localConnection)
                        this.ghostObject.TakeOwnership();
                }
                this._connection = value;
            }
        }

        public void Fondle(Thing t)
        {
            if (t == null || !t.CanBeControlled() || t.connection == this.connection)
                return;
            t.OnFondle(this.connection, 1);
        }

        public static void Fondle(Thing t, NetworkConnection c)
        {
            if (t == null || t == null || t.connection == c)
                return;
            t.OnFondle(c, 1);
        }

        public static void ExtraFondle(Thing t, NetworkConnection c) => t?.OnFondle(c, 8);

        public static void SuperFondle(Thing t, NetworkConnection c) => t?.OnFondle(c, 25);

        public static void UltraFondle(Thing t, NetworkConnection c) => t?.OnFondle(c, 30);

        public static void UnstoppableFondle(Thing t, NetworkConnection c) => t?.OnFondle(c, 35);

        protected virtual void OnFondle(NetworkConnection c, int fondleSize, bool pBreakRules = false)
        {
            if (!Network.isActive || c != DuckNetwork.localConnection)
                return;
            this.connection = c;
            this.authority += fondleSize;
        }

        public static void PowerfulRuleBreakingFondle(Thing t, NetworkConnection c)
        {
            if (t == null || t == null || t.connection == c)
                return;
            t.OnFondle(c, 1, true);
        }

        public static void AuthorityFondle(Thing t, NetworkConnection c, int fondleSize) => t?.OnFondle(c, fondleSize * c.authorityPower);

        public virtual bool CanBeControlled() => true;

        public void IgnoreNetworkSync()
        {
            this._isStateObject = false;
            this._isStateObjectInitialized = true;
        }

        public virtual bool TransferControl(NetworkConnection to, NetIndex8 auth)
        {
            if (to == this.connection)
            {
                if (auth > this.authority)
                    this.authority = auth;
                return true;
            }
            if (this.connection.profile != null && this.connection.profile.slotType != SlotType.Spectator && (auth < this.authority || this.connection != null && this.CanBeControlled() && this.connection.profile != null && this.connection.profile.slotType != SlotType.Spectator && auth == this.authority && (connection.profile.networkIndex + DuckNetwork.levelIndex) % GameLevel.NumberOfDucks < (to.profile.networkIndex + DuckNetwork.levelIndex) % GameLevel.NumberOfDucks))
                return false;
            if (NetIndex8.Difference(auth, this.authority) > 19)
                this.wasSuperFondled = 120;
            this._framesSinceTransfer = 0;
            this.connection = to;
            this.authority = auth;
            return true;
        }

        public void PlaySFX(string sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            if (!this.isServerForObject)
                return;
            SFX.PlaySynchronized(sound, vol, pitch, pan, looped);
        }

        public virtual void SpecialNetworkUpdate()
        {
        }

        public int networkDrawIndex
        {
            get => this._networkDrawIndex;
            set => this._networkDrawIndex = value;
        }

        public bool isStateObject
        {
            get
            {
                if (!this._isStateObjectInitialized)
                {
                    this._isStateObject = Editor.AllStateFields[this.GetType()].Length != 0;
                    this._isStateObjectInitialized = true;
                }
                return this._isStateObject;
            }
        }

        public bool isServerForObject => !Network.isActive || this.connection == null || this.connection == DuckNetwork.localConnection || Thing.loadingLevel != null;

        public bool isClientWhoCreatedObject => this._ghostObject == null || Thing.loadingLevel != null || this._ghostObject.ghostObjectIndex._index / GhostManager.kGhostIndexMax == DuckNetwork.localProfile.fixedGhostIndex;

        public virtual void SetTranslation(Vec2 translation) => this.position += translation;

        public virtual Vec2 cameraPosition => this.position;

        public static ushort GetGlobalIndex()
        {
            Thing._staticGlobalIndex = (ushort)((_staticGlobalIndex + 1) % ushort.MaxValue);
            if (Thing._staticGlobalIndex == 0)
                ++Thing._staticGlobalIndex;
            return Thing._staticGlobalIndex;
        }

        public static ushort GetPhysicsIndex()
        {
            Thing._staticPhysicsIndex = (ushort)((_staticPhysicsIndex + 1) % ushort.MaxValue);
            if (Thing._staticPhysicsIndex == 0)
                ++Thing._staticPhysicsIndex;
            return Thing._staticPhysicsIndex;
        }

        public ushort globalIndex
        {
            get => this._globalIndex;
            set => this._globalIndex = value;
        }

        public ushort physicsIndex
        {
            get => this._globalIndex;
            set => this._globalIndex = value;
        }

        public Vec2 lerpPosition
        {
            get => this._lerpPosition;
            set => this._lerpPosition = value;
        }

        public Vec2 lerpVector
        {
            get => this._lerpVector;
            set => this._lerpVector = value;
        }

        public float lerpSpeed
        {
            get => this._lerpSpeed;
            set => this._lerpSpeed = value;
        }

        public Portal portal
        {
            get => this._portal;
            set => this._portal = value;
        }

        public SequenceItem sequence
        {
            get => this._sequence;
            set => this._sequence = value;
        }

        public string type => this._type;

        public Level level
        {
            get => this._level;
            set => this._level = value;
        }

        public float lastTeleportDirection
        {
            get => this._lastTeleportDirection;
            set => this._lastTeleportDirection = value;
        }

        public bool removeFromLevel => this._removeFromLevel;

        public virtual int frame
        {
            get => !(this.graphic is SpriteMap) ? 0 : (this.graphic as SpriteMap).frame;
            set
            {
                if (!(this.graphic is SpriteMap))
                    return;
                (this.graphic as SpriteMap).frame = value;
            }
        }

        public bool placed
        {
            get => this._placed;
            set => this._placed = value;
        }

        public bool canBeGrouped => this._canBeGrouped;

        public virtual Thing realObject => this;

        public virtual Thing owner
        {
            get => this._owner;
            set
            {
                if (this._owner != value)
                    this._prevOwner = this._owner;
                this._lastThrownBy = this._owner;
                this._owner = value;
            }
        }

        public Thing prevOwner => this._prevOwner;

        public Thing lastThrownBy => this._lastThrownBy;

        public bool opaque => false;

        public virtual Sprite graphic
        {
            get => this._graphic;
            set => this._graphic = value;
        }

        public DamageMap damageMap
        {
            get => this._damageMap;
            set => this._damageMap = value;
        }

        public virtual bool visible
        {
            get => this._visible;
            set => this._visible = value;
        }

        public Material material
        {
            get => this._material;
            set => this._material = value;
        }

        public virtual bool enablePhysics
        {
            get => this._enablePhysics;
            set => this._enablePhysics = value;
        }

        public float Distance(Thing pOther) => pOther != null ? (this.position - pOther.position).length : float.MaxValue;

        public void Presto()
        {
            if (!this.isServerForObject || this.removeFromLevel)
                return;
            if (Network.isActive)
                Send.Message(new NMPop(this.position));
            NMPop.AmazingDisappearingParticles(this.position);
            if (this is MaterialThing)
                (this as MaterialThing).Destroy(new DTPop());
            this.y += 9999f;
            if (this is Duck)
                return;
            Level.Remove(this);
        }

        public Profile responsibleProfile
        {
            set => this._responsibleProfile = value;
            get
            {
                if (this._responsibleProfile != null)
                {
                    return this._responsibleProfile;
                }
                Duck d = this as Duck;
                if (d == null)
                {
                    d = (this.owner as Duck);
                    if (d == null)
                    {
                        d = (this.prevOwner as Duck);
                        if (d == null && this.owner != null)
                        {
                            d = (this.owner.owner as Duck);
                            if (d == null)
                            {
                                d = (this.owner.prevOwner as Duck);
                                if (d == null && this.prevOwner != null)
                                {
                                    d = (this.prevOwner.owner as Duck);
                                    if (d == null)
                                    {
                                        d = (this.prevOwner.prevOwner as Duck);
                                    }
                                }
                            }
                        }
                    }
                }
                if (d == null && this is Bullet)
                {
                    Bullet b = this as Bullet;
                    if (b.firedFrom != null && !(b.firedFrom is Bullet))
                    {
                        return b.firedFrom.responsibleProfile;
                    }
                }
                if (d == null)
                {
                    return null;
                }
                return d.profile;
            }
        }

        public System.Type killThingType
        {
            get
            {
                if (this._killThingType != null)
                    return this._killThingType;
                if (this is Bullet)
                {
                    Bullet bullet = this as Bullet;
                    if (bullet.firedFrom != null)
                        return bullet.firedFrom.GetType();
                }
                if (this is SmallFire)
                {
                    SmallFire smallFire = this as SmallFire;
                    if (smallFire.firedFrom != null)
                        return smallFire.firedFrom.GetType();
                }
                return this.GetType();
            }
            set => this._killThingType = value;
        }

        public virtual float hSpeed
        {
            get => this._hSpeed;
            set => this._hSpeed = value;
        }

        public virtual float vSpeed
        {
            get => this._vSpeed;
            set => this._vSpeed = value;
        }

        public Vec2 velocity
        {
            get => new Vec2(this.hSpeed, this.vSpeed);
            set
            {
                this._hSpeed = value.x;
                this._vSpeed = value.y;
            }
        }

        public void ApplyForce(Vec2 force)
        {
            this._hSpeed += force.x;
            this._vSpeed += force.y;
        }

        public void ApplyForce(Vec2 force, Vec2 limits)
        {
            limits = new Vec2(Math.Abs(limits.x), Math.Abs(limits.y));
            if (force.x < 0.0 && _hSpeed > -limits.x || force.x > 0.0 && _hSpeed < limits.x)
                this._hSpeed += force.x;
            if ((force.y >= 0.0 || _vSpeed <= -limits.y) && (force.y <= 0.0 || _vSpeed >= limits.y))
                return;
            this._vSpeed += force.y;
        }

        public void ApplyForceLimited(Vec2 force)
        {
            this._hSpeed += force.x;
            if (force.x < 0.0 && _hSpeed < force.x || force.x > 0.0 && _hSpeed > force.x)
                this._hSpeed = force.x;
            this._vSpeed += force.y;
            if ((force.y >= 0.0 || _vSpeed >= force.y) && (force.y <= 0.0 || _vSpeed <= force.y))
                return;
            this._vSpeed = force.y;
        }

        public virtual bool active
        {
            get => this._active;
            set => this._active = value;
        }

        public virtual bool ShouldUpdate() => true;

        public virtual bool action
        {
            get => this._action;
            set => this._action = value;
        }

        public Anchor anchor
        {
            get => this._anchor;
            set => this._anchor = value;
        }

        public virtual Vec2 anchorPosition => this.position;

        public virtual sbyte offDir
        {
            get => this._offDir;
            set => this._offDir = value;
        }

        public Layer layer
        {
            get => this._layer;
            set
            {
                if (this._layer == value)
                    return;
                if (this._level != null)
                {
                    if (this._layer != null)
                        this._layer.Remove(this);
                    value.Add(this);
                }
                this._layer = value;
            }
        }

        public bool isInitialized => this._initialized;

        public List<System.Type> GetAllTypes() => Editor.AllBaseTypes[this.GetType()];

        public List<System.Type> GetAllTypesFiltered(System.Type stopAt) => Thing.GetAllTypes(this.GetType(), stopAt);

        public static List<System.Type> GetAllTypes(System.Type t, System.Type stopAt = null)
        {
            List<System.Type> allTypes = new List<System.Type>(t.GetInterfaces());
            allTypes.Add(t);
            for (System.Type baseType = t.BaseType; baseType != null && !(baseType == typeof(Thing)) && !(baseType == typeof(object)) && !(baseType == stopAt); baseType = baseType.BaseType)
                allTypes.Add(baseType);
            return allTypes;
        }

        public int placementCost => this._placementCost;

        public string editorName
        {
            get
            {
                if (this._editorName == "")
                {
                    this._editorName = this.GetType().Name;
                    if (this._editorName.Length > 1)
                    {
                        for (int index = 1; index < this._editorName.Length; ++index)
                        {
                            char ch1 = this._editorName[index];
                            if (ch1 >= 'A' && ch1 <= 'Z')
                            {
                                char ch2 = this._editorName[index - 1];
                                if (ch2 >= 'a' && ch2 <= 'z')
                                {
                                    this._editorName = this._editorName.Insert(index, " ");
                                    ++index;
                                }
                            }
                        }
                    }
                }
                return this._editorName;
            }
        }

        public void SetEditorName(string s) => this._editorName = s;

        public virtual void TabRotate()
        {
            if (!this._canFlip)
                return;
            this.flipHorizontal = !this.flipHorizontal;
        }

        public Layer placementLayer => this.placementLayerOverride != null ? this.placementLayerOverride : this.layer;

        public float likelyhoodToExist
        {
            get => this._likelyhoodToExist;
            set => this._likelyhoodToExist = value;
        }

        public bool editorCanModify => this._editorCanModify;

        public bool processedByEditor
        {
            get => this._processedByEditor;
            set => this._processedByEditor = value;
        }

        public bool visibleInGame => this._visibleInGame;

        public Vec2 editorOffset
        {
            get => this._editorOffset;
            set => this._editorOffset = value;
        }

        public WallHug hugWalls
        {
            get => this._hugWalls;
            set => this._hugWalls = value;
        }

        public static Thing Instantiate(System.Type t) => Editor.CreateThing(t);

        public static bool CheckForBozoData(Thing pThing)
        {
            if (pThing == null || Math.Abs(pThing.y) > 99999.0 || Math.Abs(pThing.x) > 99999.0)
                return true;
            if (!(pThing is ThingContainer))
                return false;
            (pThing as ThingContainer).bozocheck = true;
            return false;
        }

        public virtual void PrepareForHost()
        {
            GhostManager.context.MakeGhost(this, initLevel: true);
            this.ghostType = Editor.IDToType[this.GetType()];
            DuckNetwork.AssignToHost(this);
        }

        public static Thing LoadThing(BinaryClassChunk node, bool chance = true)
        {
            System.Type type = Editor.GetType(node.GetProperty<string>("type"));
            if (!(type != null))
                return null;
            Thing thing = Editor.CreateThing(type);
            if (!thing.Deserialize(node))
                return null;
            if (!(Level.current is Editor) && chance && thing.likelyhoodToExist != 1.0 && !Level.PassedChanceGroup(thing.chanceGroup, thing.likelyhoodToExist))
                return null;
            if (thing is IContainPossibleThings)
                (thing as IContainPossibleThings).PreparePossibilities();
            return thing;
        }

        public virtual BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = new BinaryClassChunk();
            System.Type type = this.GetType();
            binaryClassChunk.AddProperty("type", ModLoader.SmallTypeName(type));
            binaryClassChunk.AddProperty("x", x);
            binaryClassChunk.AddProperty("y", y);
            binaryClassChunk.AddProperty("chance", _likelyhoodToExist);
            binaryClassChunk.AddProperty("accessible", _isAccessible);
            binaryClassChunk.AddProperty("chanceGroup", _chanceGroup);
            binaryClassChunk.AddProperty("flipHorizontal", _flipHorizontal);
            if (this._canFlipVert)
                binaryClassChunk.AddProperty("flipVertical", flipVertical);
            if (this.sequence != null)
            {
                binaryClassChunk.AddProperty("loop", sequence.loop);
                binaryClassChunk.AddProperty("popUpOrder", sequence.order);
                binaryClassChunk.AddProperty("waitTillOrder", sequence.waitTillOrder);
            }
            foreach (FieldInfo fieldInfo in Editor.EditorFieldsForType[type])
            {
                object obj1 = fieldInfo.GetValue(this);
                obj1.GetType().GetProperty("info").GetValue(obj1, null);
                object obj2 = obj1.GetType().GetProperty("value").GetValue(obj1, null);
                binaryClassChunk.AddProperty(fieldInfo.Name, obj2);
            }
            return binaryClassChunk;
        }

        public virtual bool Deserialize(BinaryClassChunk node)
        {
            this.x = node.GetPrimitive<float>("x");
            this.y = node.GetPrimitive<float>("y");
            this._likelyhoodToExist = node.GetPrimitive<float>("chance");
            this._isAccessible = node.GetPrimitive<bool>("accessible");
            this.chanceGroup = node.GetPrimitive<int>("chanceGroup");
            this.flipHorizontal = node.GetPrimitive<bool>("flipHorizontal");
            if (this._canFlipVert)
                this.flipVertical = node.GetPrimitive<bool>("flipVertical");
            if (this.sequence != null)
            {
                this.sequence.loop = node.GetPrimitive<bool>("loop");
                this.sequence.order = node.GetPrimitive<int>("popUpOrder");
                this.sequence.waitTillOrder = node.GetPrimitive<bool>("waitTillOrder");
            }
            System.Type type = this.GetType();
            foreach (FieldInfo fieldInfo in Editor.EditorFieldsForType[type])
            {
                if (node.HasProperty(fieldInfo.Name))
                {
                    System.Type genericArgument = fieldInfo.FieldType.GetGenericArguments()[0];
                    object obj = fieldInfo.GetValue(this);
                    obj.GetType().GetProperty("value").SetValue(obj, node.GetProperty(fieldInfo.Name), null);
                }
            }
            return true;
        }

        public static Thing LegacyLoadThing(DXMLNode node, bool chance = true)
        {
            System.Type type = Editor.GetType(node.Element("type").Value);
            if (!(type != null))
                return null;
            Thing thing = Editor.CreateThing(type);
            if (!thing.LegacyDeserialize(node))
                thing = null;
            return Level.current is Editor || !chance || thing.likelyhoodToExist == 1.0 || Level.PassedChanceGroup(thing.chanceGroup, thing.likelyhoodToExist) ? thing : null;
        }

        public bool isAccessible
        {
            get => this._isAccessible;
            set => this._isAccessible = value;
        }

        public virtual DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = new DXMLNode("Object");
            System.Type type = this.GetType();
            dxmlNode.Add(new DXMLNode("type", type.AssemblyQualifiedName));
            dxmlNode.Add(new DXMLNode("x", x));
            dxmlNode.Add(new DXMLNode("y", y));
            dxmlNode.Add(new DXMLNode("chance", _likelyhoodToExist));
            dxmlNode.Add(new DXMLNode("accessible", _isAccessible));
            dxmlNode.Add(new DXMLNode("chanceGroup", _chanceGroup));
            dxmlNode.Add(new DXMLNode("flipHorizontal", _flipHorizontal));
            if (this._canFlipVert)
                dxmlNode.Add(new DXMLNode("flipVertical", _flipVertical));
            if (this.sequence != null)
            {
                dxmlNode.Add(new DXMLNode("loop", sequence.loop));
                dxmlNode.Add(new DXMLNode("popUpOrder", sequence.order));
                dxmlNode.Add(new DXMLNode("waitTillOrder", sequence.waitTillOrder));
            }
            foreach (System.Type key in Editor.AllBaseTypes[type])
            {
                if (!key.IsInterface)
                {
                    foreach (FieldInfo fieldInfo in Editor.AllEditorFields[key])
                    {
                        object obj = fieldInfo.GetValue(this);
                        object varValue = obj.GetType().GetProperty("value").GetValue(obj, null);
                        dxmlNode.Add(new DXMLNode(fieldInfo.Name, varValue));
                    }
                }
            }
            return dxmlNode;
        }

        public virtual bool LegacyDeserialize(DXMLNode node)
        {
            DXMLNode dxmlNode1 = node.Element("x");
            if (dxmlNode1 != null)
                this.x = Change.ToSingle(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("y");
            if (dxmlNode2 != null)
                this.y = Change.ToSingle(dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("chance");
            if (dxmlNode3 != null)
                this._likelyhoodToExist = Change.ToSingle(dxmlNode3.Value);
            DXMLNode dxmlNode4 = node.Element("accessible");
            if (dxmlNode4 != null)
                this._isAccessible = Change.ToBoolean(dxmlNode4.Value);
            DXMLNode dxmlNode5 = node.Element("chanceGroup");
            if (dxmlNode5 != null)
                this.chanceGroup = Convert.ToInt32(dxmlNode5.Value);
            DXMLNode dxmlNode6 = node.Element("flipHorizontal");
            if (dxmlNode6 != null)
                this.flipHorizontal = Convert.ToBoolean(dxmlNode6.Value);
            if (this._canFlipVert)
            {
                DXMLNode dxmlNode7 = node.Element("flipVertical");
                if (dxmlNode7 != null)
                    this.flipVertical = Convert.ToBoolean(dxmlNode7.Value);
            }
            if (this.sequence != null)
            {
                DXMLNode dxmlNode8 = node.Element("loop");
                if (dxmlNode8 != null)
                    this.sequence.loop = Convert.ToBoolean(dxmlNode8.Value);
                DXMLNode dxmlNode9 = node.Element("popUpOrder");
                if (dxmlNode9 != null)
                    this.sequence.order = Convert.ToInt32(dxmlNode9.Value);
                DXMLNode dxmlNode10 = node.Element("waitTillOrder");
                if (dxmlNode10 != null)
                    this.sequence.waitTillOrder = Convert.ToBoolean(dxmlNode10.Value);
            }
            foreach (System.Type key in Editor.AllBaseTypes[this.GetType()])
            {
                if (!key.IsInterface)
                {
                    foreach (FieldInfo fieldInfo in Editor.AllEditorFields[key])
                    {
                        DXMLNode dxmlNode11 = node.Element(fieldInfo.Name);
                        if (dxmlNode11 != null)
                        {
                            System.Type genericArgument = fieldInfo.FieldType.GetGenericArguments()[0];
                            object obj = fieldInfo.GetValue(this);
                            PropertyInfo property = obj.GetType().GetProperty("value");
                            if (genericArgument == typeof(int))
                                property.SetValue(obj, Convert.ToInt32(dxmlNode11.Value), null);
                            else if (genericArgument == typeof(float))
                                property.SetValue(obj, Convert.ToSingle(dxmlNode11.Value), null);
                            else if (genericArgument == typeof(string))
                            {
                                EditorPropertyInfo editorPropertyInfo = obj.GetType().GetProperty("info").GetValue(obj, null) as EditorPropertyInfo;
                                object guid = dxmlNode11.Value;
                                if (editorPropertyInfo.isLevel)
                                {
                                    LevelData levelData = DuckFile.LoadLevel(Content.path + "levels/" + guid?.ToString() + ".lev");
                                    if (levelData != null)
                                        guid = levelData.metaData.guid;
                                }
                                property.SetValue(obj, guid, null);
                            }
                            else if (genericArgument == typeof(bool))
                                property.SetValue(obj, Convert.ToBoolean(dxmlNode11.Value), null);
                            else if (genericArgument == typeof(byte))
                                property.SetValue(obj, Convert.ToByte(dxmlNode11.Value), null);
                            else if (genericArgument == typeof(short))
                                property.SetValue(obj, Convert.ToInt16(dxmlNode11.Value), null);
                            else if (genericArgument == typeof(long))
                                property.SetValue(obj, Convert.ToInt64(dxmlNode11.Value), null);
                        }
                    }
                }
            }
            return true;
        }

        public virtual void EditorPropertyChanged(object property)
        {
        }

        public virtual void EditorObjectsChanged()
        {
        }

        public virtual void EditorAdded()
        {
        }

        public virtual void EditorRemoved()
        {
        }

        public virtual void EditorFlip(bool pVertical)
        {
        }

        public virtual bool flipHorizontal
        {
            get => this._flipHorizontal;
            set
            {
                this._flipHorizontal = value;
                this.offDir = this._flipHorizontal ? (sbyte)-1 : (sbyte)1;
            }
        }

        public virtual bool flipVertical
        {
            get => this._flipVertical;
            set => this._flipVertical = value;
        }

        public int chanceGroup
        {
            get => this._chanceGroup;
            set => this._chanceGroup = value;
        }

        public virtual ContextMenu GetContextMenu()
        {
            EditorGroupMenu owner = new EditorGroupMenu(null, true);
            if (this._canFlip)
                owner.AddItem(new ContextCheckBox("Flip", null, new FieldBinding(this, "flipHorizontal")));
            if (this._canFlipVert)
                owner.AddItem(new ContextCheckBox("Flip V", null, new FieldBinding(this, "flipVertical")));
            if (this._canHaveChance)
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(owner)
                {
                    text = "@CHANCEICON@Chance",
                    tooltip = "Likelyhood for this object to exist in the level."
                };
                owner.AddItem(editorGroupMenu);
                editorGroupMenu.AddItem(new ContextSlider("Chance", null, new FieldBinding(this, "likelyhoodToExist"), 0.05f, null, false, null, "Chance for object to exist. 1.0 = 100% chance."));
                editorGroupMenu.AddItem(new ContextSlider("Chance Group", null, new FieldBinding(this, "chanceGroup", -1f, 10f), 1f, null, false, null, "All objects in a chance group will exist, if their group's chance roll is met. -1 means no grouping."));
                editorGroupMenu.AddItem(new ContextCheckBox("Accessible", null, new FieldBinding(this, "isAccessible"), null, "Flag for level generation, set this to false if the object is behind a locked door and not neccesarily accessible."));
            }
            if (this.sequence != null && !this._contextMenuFilter.Contains("Sequence"))
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(owner)
                {
                    text = "Sequence"
                };
                owner.AddItem(editorGroupMenu);
                if (!this._contextMenuFilter.Contains("Sequence|Loop"))
                    editorGroupMenu.AddItem(new ContextCheckBox("Loop", null, new FieldBinding(sequence, "loop")));
                if (!this._contextMenuFilter.Contains("Sequence|Order"))
                    editorGroupMenu.AddItem(new ContextSlider("Order", null, new FieldBinding(sequence, "order", max: 100f), 1f, "RAND"));
                if (!this._contextMenuFilter.Contains("Sequence|Wait"))
                    editorGroupMenu.AddItem(new ContextCheckBox("Wait", null, new FieldBinding(sequence, "waitTillOrder")));
            }
            List<string> stringList = new List<string>();
            foreach (System.Type key in Editor.AllBaseTypes[this.GetType()])
            {
                if (!key.IsInterface)
                {
                    foreach (FieldInfo fieldInfo in Editor.AllEditorFields[key])
                    {
                        if (!stringList.Contains(fieldInfo.Name) && !this._contextMenuFilter.Contains(fieldInfo.Name))
                        {
                            string text = fieldInfo.Name.Replace("_", " ");
                            object thing = fieldInfo.GetValue(this);
                            EditorPropertyInfo editorPropertyInfo = thing.GetType().GetProperty("info").GetValue(thing, null) as EditorPropertyInfo;
                            if (editorPropertyInfo.name != null)
                                text = editorPropertyInfo.name;
                            if (editorPropertyInfo.value.GetType() == typeof(int) || editorPropertyInfo.value.GetType() == typeof(float) || editorPropertyInfo.value.GetType().IsEnum)
                                owner.AddItem(new ContextSlider(text, null, new FieldBinding(thing, "value", editorPropertyInfo.min, editorPropertyInfo.max), editorPropertyInfo.increment, editorPropertyInfo.minSpecial, editorPropertyInfo.isTime, null, editorPropertyInfo.tooltip));
                            else if (editorPropertyInfo.value.GetType() == typeof(bool))
                                owner.AddItem(new ContextCheckBox(text, null, new FieldBinding(thing, "value"), null, editorPropertyInfo.tooltip));
                            else if (editorPropertyInfo.value.GetType() == typeof(string))
                            {
                                if (editorPropertyInfo.isLevel)
                                    owner.AddItem(new ContextFile(text, null, new FieldBinding(thing, "value"), ContextFileType.Level, editorPropertyInfo.tooltip));
                                else
                                    owner.AddItem(new ContextTextbox(text, null, new FieldBinding(thing, "value"), editorPropertyInfo.tooltip));
                            }
                            stringList.Add(fieldInfo.Name);
                        }
                    }
                }
            }
            return owner;
        }

        public virtual void DrawHoverInfo()
        {
        }

        public Sprite GetEditorImage(
          int wide = 16,
          int high = 16,
          bool transparentBack = false,
          Effect effect = null,
          RenderTarget2D target = null)
        {
            return this.GetEditorImage(wide, high, transparentBack, effect, target, false);
        }

        public Sprite GetEditorImage(
          int wide,
          int high,
          bool transparentBack,
          Effect effect,
          RenderTarget2D target,
          bool pUseCollisionSize)
        {
            Sprite editorImage1;
            if (Thing._editorIcons.TryGetValue(this.GetType(), out editorImage1))
                return editorImage1;
            if (Thread.CurrentThread != MonoMain.mainThread)
                return new Sprite("basketBall");
            if (Thing._alphaTestEffect == null)
                Thing._alphaTestEffect = (Effect)Content.Load<MTEffect>("Shaders/alphatest");
            if (pUseCollisionSize && collisionSize.x > 0.0)
            {
                if (wide <= 0)
                    wide = (int)this.collisionSize.x;
                if (high <= 0)
                    high = (int)this.collisionSize.y;
            }
            else if (this.graphic != null)
            {
                if (wide <= 0)
                    wide = this.graphic.w;
                if (high <= 0)
                    high = this.graphic.h;
            }
            int num1 = wide > high ? wide : high;
            if (target == null)
                target = new RenderTarget2D(wide, high, true);
            if (this.graphic == null)
                return new Sprite(target, 0f, 0f);
            float num2 = num1 / (collisionSize.x > 0.0 & pUseCollisionSize ? this.collisionSize.x : graphic.width);
            Camera camera = new Camera(0f, 0f, wide, high)
            {
                position = new Vec2(this.x - this.centerx * num2, this.y - this.centery * num2)
            };
            if (pUseCollisionSize && collisionSize.x > 0.0)
                camera.center = new Vec2((int)((this.left + this.right) / 2.0), (int)((this.top + this.bottom) / 2.0));
            RenderTarget2D currentRenderTarget = DuckGame.Graphics.currentRenderTarget;
            DuckGame.Graphics.SetRenderTarget(target);
            DepthStencilState depthStencilState = new DepthStencilState()
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };
            DuckGame.Graphics.Clear(transparentBack ? new Color(0, 0, 0, 0) : new Color(15, 4, 16));
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, (MTEffect)(effect == null ? Thing._alphaTestEffect : effect), camera.getMatrix());
            this.Draw();
            DuckGame.Graphics.screen.End();
            if (currentRenderTarget == null || currentRenderTarget.IsDisposed)
                DuckGame.Graphics.SetRenderTarget(null);
            else
                DuckGame.Graphics.SetRenderTarget(currentRenderTarget);
            Texture2D tex = new Texture2D(DuckGame.Graphics.device, target.width, target.height);
            tex.SetData<Color>(target.GetData());
            Sprite editorImage2 = new Sprite((Tex2D)tex);
            Thing._editorIcons[this.GetType()] = editorImage2;
            return editorImage2;
        }

        public virtual Sprite GeneratePreview(
          int wide = 16,
          int high = 16,
          bool transparentBack = false,
          Effect effect = null,
          RenderTarget2D target = null)
        {
            bool flag = ((wide != 16 ? 0 : (high == 16 ? 1 : 0)) & (transparentBack ? 1 : 0)) != 0 && effect == null && target == null;
            if (flag && this._editorIcon != null)
                return this._editorIcon;
            if (Thread.CurrentThread != MonoMain.mainThread)
                return new Sprite("basketBall");
            if (Thing._alphaTestEffect == null)
                Thing._alphaTestEffect = (Effect)Content.Load<MTEffect>("Shaders/alphatest");
            if (this.graphic != null)
            {
                if (wide <= 0)
                    wide = this.graphic.w;
                if (high <= 0)
                    high = this.graphic.h;
            }
            if (target == null)
                target = new RenderTarget2D(wide, high, true);
            if (this.graphic == null)
                return new Sprite(target, 0f, 0f);
            Camera camera = new Camera(0f, 0f, wide, high)
            {
                position = new Vec2(this.x - wide / 2, this.y - high / 2)
            };
            DuckGame.Graphics.SetRenderTarget(target);
            DepthStencilState depthStencilState = new DepthStencilState()
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };
            DuckGame.Graphics.Clear(transparentBack ? new Color(0, 0, 0, 0) : new Color(30, 30, 30));
            DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, (MTEffect)(effect == null ? Thing._alphaTestEffect : effect), camera.getMatrix());
            this.Draw();
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.SetRenderTarget(null);
            Texture2D tex = new Texture2D(DuckGame.Graphics.device, target.width, target.height);
            tex.SetData<Color>(target.GetData());
            Sprite preview = new Sprite((Tex2D)tex);
            if (flag)
                this._editorIcon = preview;
            return preview;
        }

        public virtual bool solid
        {
            get => this._solid;
            set
            {
                if (value && !this._solid)
                    this.FixClipping();
                this._solid = value;
            }
        }

        public void FixClipping()
        {
            foreach (Block block in Level.CheckRectAll<Block>(this.topLeft, this.bottomRight))
                ;
        }

        private string GetPropertyDetails()
        {
            string propertyDetails = "";
            foreach (FieldInfo fieldInfo in Editor.AllEditorFields[this.GetType()])
            {
                object obj = fieldInfo.GetValue(this);
                EditorPropertyInfo editorPropertyInfo = obj.GetType().GetProperty("info").GetValue(obj, null) as EditorPropertyInfo;
                if (editorPropertyInfo.value.GetType() == typeof(int) || editorPropertyInfo.value.GetType() == typeof(float))
                    propertyDetails = propertyDetails + fieldInfo.Name + ": " + Convert.ToString(editorPropertyInfo.value) + "\n";
            }
            return propertyDetails;
        }

        public virtual string GetDetailsString()
        {
            if (_likelyhoodToExist == 1.0 && this._chanceGroup == -1)
                return this.GetPropertyDetails();
            return "Chance: " + Math.Round(this.likelyhoodToExist / 1.0 * 100.0).ToString() + "%\nChance Group: " + (this._chanceGroup == -1 ? "None" : this._chanceGroup.ToString(CultureInfo.InvariantCulture)) + "\n" + this.GetPropertyDetails();
        }

        public virtual void ReturnItemToWorld(Thing t)
        {
            Block block1 = Level.CheckLine<Block>(this.position, this.position + new Vec2(16f, 0f));
            if (block1 != null && block1.solid && t.right > block1.left)
                t.right = block1.left;
            Block block2 = Level.CheckLine<Block>(this.position, this.position - new Vec2(16f, 0f));
            if (block2 != null && block2.solid && t.left < block2.right)
                t.left = block2.right;
            Block block3 = Level.CheckLine<Block>(this.position, this.position + new Vec2(0f, -16f));
            if (block3 != null && block3.solid && t.top < block3.bottom)
                t.top = block3.bottom;
            Block block4 = Level.CheckLine<Block>(this.position, this.position + new Vec2(0f, 16f));
            if (block4 == null || !block4.solid || t.bottom <= block4.top)
                return;
            t.bottom = block4.top;
        }

        public bool isOffBottomOfLevel => this.y > Level.activeLevel.lowestPoint + 100.0 && this.top > Level.current.camera.bottom + 8.0;

        public virtual Vec2 collisionOffset
        {
            get => this._collisionOffset;
            set => this._collisionOffset = value;
        }

        public virtual Vec2 collisionSize
        {
            get => this._collisionSize;
            set => this._collisionSize = value;
        }

        public float topQuick => this._topQuick;

        public float bottomQuick => this._bottomQuick;

        public float leftQuick => this._leftQuick;

        public float rightQuick => this._rightQuick;

        public float topLocal => this.collisionOffset.y;

        public float bottomLocal => this.collisionOffset.y + this.collisionSize.y;

        public float top
        {
            get => this.position.y + this._collisionOffset.y;
            set => this.position.y = value + (this.position.y - this.top);
        }

        public float bottom
        {
            get => this.position.y + this._collisionOffset.y + this._collisionSize.y;
            set => this.position.y = value + (this.position.y - this.bottom);
        }

        public float left
        {
            get => this.offDir <= 0 ? this.position.x - this._collisionSize.x - this._collisionOffset.x : this.position.x + this._collisionOffset.x;
            set => this.x = value + (this.x - this.left);
        }

        public float right
        {
            get => this.offDir <= 0 ? this.position.x - this._collisionOffset.x : this.position.x + this._collisionOffset.x + this._collisionSize.x;
            set => this.x = value + (this.x - this.right);
        }

        public Vec2 topLeft => new Vec2(this.left, this.top);

        public Vec2 topRight => new Vec2(this.right, this.top);

        public Vec2 bottomLeft => new Vec2(this.left, this.bottom);

        public Vec2 bottomRight => new Vec2(this.right, this.bottom);

        public Vec2 NearestCorner(Vec2 to)
        {
            Vec2 vec2 = this.topLeft;
            float num = (this.topLeft - to).length;
            float length1 = (this.topRight - to).length;
            if (length1 < num)
            {
                vec2 = this.topRight;
                num = length1;
            }
            float length2 = (this.bottomLeft - to).length;
            if (length2 < num)
            {
                vec2 = this.bottomLeft;
                num = length2;
            }
            if ((this.bottomRight - to).length < num)
                vec2 = this.bottomRight;
            return vec2;
        }

        public Vec2 NearestOpenCorner(Vec2 to)
        {
            Vec2 vec2_1 = Vec2.Zero;
            float num = 9999999f;
            float length1 = (this.topLeft - to).length;
            if (length1 < num && Level.CheckCircle<Block>(this.topLeft, 2f, this) == null)
            {
                vec2_1 = this.topLeft;
                num = length1;
            }
            Vec2 vec2_2 = this.topRight - to;
            float length2 = vec2_2.length;
            if (length2 < num && Level.CheckCircle<Block>(this.topRight, 2f, this) == null)
            {
                vec2_1 = this.topRight;
                num = length2;
            }
            vec2_2 = this.bottomLeft - to;
            float length3 = vec2_2.length;
            if (length3 < num && Level.CheckCircle<Block>(this.bottomLeft, 2f, this) == null)
            {
                vec2_1 = this.bottomLeft;
                num = length3;
            }
            vec2_2 = this.bottomRight - to;
            if (vec2_2.length < num && Level.CheckCircle<Block>(this.bottomRight, 2f, this) == null)
                vec2_1 = this.bottomRight;
            return vec2_1;
        }

        public bool isStatic
        {
            get => this._isStatic;
            set => this._isStatic = value;
        }

        public float halfWidth => this.width / 2f;

        public float halfHeight => this.height / 2f;

        public float width => this._collisionSize.x * this.scale.x;

        public float height => this._collisionSize.y * this.scale.y;

        public float w => this.width;

        public float h => this.height;

        public Rectangle rectangle => new Rectangle((int)this.left, (int)this.top, (int)(this.right - this.left), (int)(this.bottom - this.top));

        public Vec2 collisionCenter
        {
            get => new Vec2(this.left + this.collisionSize.x / 2f, this.top + this.collisionSize.y / 2f);
            set
            {
                this.left = value.x - this.collisionSize.x / 2f;
                this.top = value.y - this.collisionSize.y / 2f;
            }
        }

        public Thing(float xval = 0f, float yval = 0f, Sprite sprite = null)
        {
            this.x = xval;
            this.y = yval;
            this.graphic = sprite;
            if (sprite != null)
                this._collisionSize = new Vec2(sprite.w, sprite.h);
            if (!Network.isActive)
                return;
            this.connection = DuckNetwork.localConnection;
        }

        public virtual Vec2 OffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * this.scale;
            if (this.offDir < 0)
                vec2.x *= -1f;
            return vec2.Rotate(this.angle, new Vec2(0f, 0f));
        }

        public virtual Vec2 ReverseOffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * this.scale;
            vec2 = vec2.Rotate(-this.angle, new Vec2(0f, 0f));
            return vec2;
        }

        public virtual Vec2 Offset(Vec2 pos) => this.position + this.OffsetLocal(pos);

        public virtual Vec2 ReverseOffset(Vec2 pos)
        {
            pos -= this.position;
            return this.ReverseOffsetLocal(pos);
        }

        public virtual float OffsetX(float pos)
        {
            Vec2 vec2 = new Vec2(pos, 0f);
            if (this.offDir < 0)
                vec2.x *= -1f;
            return (this.position + vec2.Rotate(this.angle, new Vec2(0f, 0f))).x;
        }

        public virtual float OffsetY(float pos)
        {
            Vec2 vec2 = new Vec2(0f, pos);
            if (this.offDir < 0)
                vec2.x *= -1f;
            return (this.position + vec2.Rotate(this.angle, new Vec2(0f, 0f))).y;
        }

        public virtual void ResetProperties()
        {
            this._level = null;
            this._removeFromLevel = false;
            this._initialized = false;
        }

        public void AddToLayer()
        {
            if (this._layer == null)
                this._layer = Layer.Game;
            if (Thing.skipLayerAdding)
                return;
            this._layer.Add(this);
        }

        public void DoNetworkInitialize()
        {
            if (this._networkInitialized)
                return;
            if (this.isStateObject)
            {
                this._ghostType = Editor.IDToType[this.GetType()];
                if (Network.isServer)
                    this.connection = DuckNetwork.localConnection;
            }
            this._networkInitialized = true;
        }

        public virtual void DoInitialize()
        {
            if (this._redoLayer)
            {
                this.AddToLayer();
                this._redoLayer = false;
            }
            if (this._initialized)
                return;
            if (Network.isActive)
                this.DoNetworkInitialize();
            this._networkDrawIndex = NetworkDebugger.currentIndex;
            this.Initialize();
            this._initialized = true;
        }

        public virtual void Initialize()
        {
        }

        public virtual void DoUpdate()
        {
            if (this.wasSuperFondled > 0)
                --this.wasSuperFondled;
            if (this._anchor != null)
                this.position = this._anchor.position;
            this.Update();
            this._topQuick = this.top;
            this._bottomQuick = this.bottom;
            this._leftQuick = this.left;
            this._rightQuick = this.right;
        }

        public virtual void Update()
        {
        }

        public virtual void InactiveUpdate()
        {
        }

        public virtual void DoEditorUpdate()
        {
            this._topQuick = this.top;
            this._bottomQuick = this.bottom;
            this._leftQuick = this.left;
            this._rightQuick = this.right;
            this.EditorUpdate();
        }

        public virtual void EditorUpdate()
        {
        }

        public virtual void DoEditorRender() => this.EditorRender();

        public virtual void EditorRender()
        {
        }

        public virtual void OnEditorLoaded()
        {
        }

        public void Glitch()
        {
            if (!(this.material is MaterialGlitch))
                return;
            (this.material as MaterialGlitch).yoffset = Rando.Float(1f);
            (this.material as MaterialGlitch).amount = Rando.Float(0.9f, 1.2f);
        }

        public ProfileNetData GetOrCreateNetData()
        {
            if (this._netData == null)
                this._netData = new ProfileNetData();
            return this._netData;
        }

        public void NetworkSet<T>(string pVariable, T pValue)
        {
            if (!this.isServerForObject)
                return;
            if (this._netData == null)
                this._netData = new ProfileNetData();
            this._netData.Set<T>(pVariable, pValue);
        }
        public T NetworkGet<T>(string pVariable, T pDefault = default(T))
        {
            if (this._netData == null)
            {
                return default(T);
            }
            return this._netData.Get<T>(pVariable, pDefault);
        }
        public GhostObject ghostObject
        {
            get => this._ghostObject;
            set => this._ghostObject = value;
        }

        public virtual void OnGhostObjectAdded()
        {
        }

        /// <summary>
        /// If true, this object's Update function is run via Level.UpdateThings. Otherwise, it's run via GhostManager.UpdateGhostLerp
        /// </summary>
        public bool shouldRunUpdateLocally => (this.connection == null || this.connection.data == null) && this.level != null;

        public bool ignoreGhosting
        {
            get => this._ignoreGhosting;
            set => this._ignoreGhosting = value;
        }

        public virtual void DoDraw()
        {
            if (NetworkDebugger.currentIndex >= 0 && NetworkDebugger.currentIndex != this._networkDrawIndex)
                return;
            DuckGame.Graphics.material = this._material;
            if (this._material != null)
                this._material.Update();
            this.Draw();
            DuckGame.Graphics.material = null;
        }

        public virtual void Draw()
        {
            if (this._graphic == null)
                return;
            if (!this._skipPositioning)
            {
                this._graphic.position = this.position;
                this._graphic.alpha = this.alpha;
                this._graphic.angle = this.angle;
                this._graphic.depth = this.depth;
                this._graphic.scale = this.scale;
                this._graphic.center = this.center;
            }
            this._graphic.Draw();
        }

        public void DrawCollision()
        {
            DuckGame.Graphics.DrawRect(this.topLeft, this.bottomRight, Color.Orange * 0.8f, (Depth)1f, false, 0.5f);
            if (!(this is PhysicsObject))
                return;
            int num = (this as PhysicsObject).sleeping ? 1 : 0;
        }

        public void Draw(Sprite spr, float xpos, float ypos, int d = 1) => this.Draw(spr, new Vec2(xpos, ypos), d);

        public void Draw(Sprite spr, Vec2 pos, int d = 1)
        {
            Vec2 vec2 = this.Offset(pos);
            if (this.graphic != null)
                spr.flipH = this.graphic.flipH;
            spr.angle = this.angle;
            spr.alpha = this.alpha;
            spr.depth = this.depth + d;
            spr.scale = this.scale;
            spr.flipH = this.offDir < 0;
            DuckGame.Graphics.Draw(spr, vec2.x, vec2.y);
        }

        public void DrawIgnoreAngle(Sprite spr, Vec2 pos, int d = 1)
        {
            Vec2 vec2 = this.Offset(pos);
            spr.alpha = this.alpha;
            spr.depth = this.depth + d;
            spr.scale = this.scale;
            DuckGame.Graphics.Draw(spr, vec2.x, vec2.y);
        }

        public virtual void OnTeleport()
        {
        }

        public virtual void DoTerminate() => this.Terminate();

        public virtual void Terminate()
        {
        }

        public virtual void Added(Level parent)
        {
            this._removeFromLevel = false;
            this._redoLayer = true;
            this._level = parent;
            this.DoInitialize();
        }

        public virtual void Added(Level parent, bool redoLayer, bool reinit)
        {
            if (reinit)
                this._initialized = false;
            this._removeFromLevel = false;
            this._redoLayer = redoLayer;
            this._level = parent;
            this.DoInitialize();
        }

        public virtual void Removed()
        {
            this._removeFromLevel = true;
            if (this._layer == null)
                return;
            this._layer.RemoveSoon(this);
        }

        public virtual void NetworkUpdate()
        {
        }

        public virtual void OnSequenceActivate()
        {
        }
    }
}
