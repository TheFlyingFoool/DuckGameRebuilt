// Decompiled with JetBrains decompiler
// Type: DuckGame.Thing
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public bool shouldbegraphicculled = true;
        public bool shouldbeinupdateloop = true;
        public int hashcodeindex; // dont touch :)
        public Vec2 oldposition = Vec2.Zero;
        public Vec2 oldcollisionOffset = Vec2.Zero;
        public Vec2 oldcollisionSize = Vec2.Zero;
        public Vec2[] Buckets = new Vec2[0];
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
        private ushort _globalIndex = GetGlobalIndex();
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
        private Type _killThingType;
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
        public Type editorCycleType;
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
        private static Dictionary<Type, Sprite> _editorIcons = new Dictionary<Type, Sprite>();
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

        //public override int GetHashCode()
        //{
        //    return (int)(hashcodeindex + this.x + this.y);
        //}
        public string GetPath(string asset) => ModLoader._modAssemblies[GetType().Assembly].configuration.contentDirectory + asset.Replace('\\', '/');

        /// <summary>Gets the path to an asset from a mod.</summary>
        /// <typeparam name="T">The mod type to fetch from</typeparam>
        /// <param name="asset">The asset name, relative to the mods' Content folder.</param>
        /// <returns>The path.</returns>

        public virtual void PreLevelInitialize()
        {
        }
        public static string GetPath<T>(string asset) where T : Mod => Mod.GetPath<T>(asset);

        public virtual Vec2 netPosition
        {
            get => position;
            set => position = value;
        }

        public ushort ghostType
        {
            get => !_removeFromLevel ? _ghostType : (ushort)0;
            set
            {
                if (_ghostType == value)
                    return;
                _ghostType = value;
            }
        }

        public int networkSeed
        {
            get
            {
                if (Network.isServer && !_initializedNetworkSeed && isStateObject)
                {
                    _networkSeed = Rando.Int(2147483646);
                    _initializedNetworkSeed = true;
                }
                return _networkSeed;
            }
            set
            {
                _networkSeed = value;
                _initializedNetworkSeed = true;
            }
        }

        public virtual NetIndex8 authority
        {
            get => _authority;
            set
            {
                if (_authority != value)
                    _lastAuthorityChange = Network.synchronizedTime;
                _authority = value;
            }
        }

        public virtual NetworkConnection connection
        {
            get => _connection;
            set
            {
                if (value != _connection && ghostObject != null)
                {
                    ghostObject.KillNetworkData();
                    if (value == DuckNetwork.localConnection)
                        ghostObject.TakeOwnership();
                }
                _connection = value;
            }
        }

        public void Fondle(Thing t)
        {
            if (t == null || !t.CanBeControlled() || t.connection == connection)
                return;
            t.OnFondle(connection, 1);
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
            connection = c;
            authority += fondleSize;
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
            _isStateObject = false;
            _isStateObjectInitialized = true;
        }

        public virtual bool TransferControl(NetworkConnection to, NetIndex8 auth)
        {
            if (to == connection)
            {
                if (auth > authority)
                    authority = auth;
                return true;
            }
            if (connection.profile != null && connection.profile.slotType != SlotType.Spectator && (auth < authority || connection != null && CanBeControlled() && connection.profile != null && connection.profile.slotType != SlotType.Spectator && auth == authority && (connection.profile.networkIndex + DuckNetwork.levelIndex) % GameLevel.NumberOfDucks < (to.profile.networkIndex + DuckNetwork.levelIndex) % GameLevel.NumberOfDucks))
                return false;
            if (NetIndex8.Difference(auth, authority) > 19)
                wasSuperFondled = 120;
            _framesSinceTransfer = 0;
            connection = to;
            authority = auth;
            return true;
        }

        public void PlaySFX(string sound, float vol = 1f, float pitch = 0f, float pan = 0f, bool looped = false)
        {
            if (!isServerForObject)
                return;
            SFX.PlaySynchronized(sound, vol, pitch, pan, looped);
        }

        public virtual void SpecialNetworkUpdate()
        {
        }

        public int networkDrawIndex
        {
            get => _networkDrawIndex;
            set => _networkDrawIndex = value;
        }

        public bool isStateObject
        {
            get
            {
                if (!_isStateObjectInitialized)
                {
                    _isStateObject = Editor.AllStateFields[GetType()].Length != 0;
                    _isStateObjectInitialized = true;
                }
                return _isStateObject;
            }
        }

        public bool isServerForObject => !Network.isActive || connection == null || connection == DuckNetwork.localConnection || loadingLevel != null;

        public bool isClientWhoCreatedObject => _ghostObject == null || loadingLevel != null || _ghostObject.ghostObjectIndex._index / GhostManager.kGhostIndexMax == DuckNetwork.localProfile.fixedGhostIndex;

        public virtual void SetTranslation(Vec2 translation)
        {
            position += translation;
            //Level.current.things.UpdateObject(this);
        }

        public virtual Vec2 cameraPosition => position;

        public static ushort GetGlobalIndex()
        {
            _staticGlobalIndex = (ushort)((_staticGlobalIndex + 1) % ushort.MaxValue);
            if (_staticGlobalIndex == 0)
                ++_staticGlobalIndex;
            return _staticGlobalIndex;
        }

        public static ushort GetPhysicsIndex()
        {
            _staticPhysicsIndex = (ushort)((_staticPhysicsIndex + 1) % ushort.MaxValue);
            if (_staticPhysicsIndex == 0)
                ++_staticPhysicsIndex;
            return _staticPhysicsIndex;
        }

        public ushort globalIndex
        {
            get => _globalIndex;
            set => _globalIndex = value;
        }

        public ushort physicsIndex
        {
            get => _globalIndex;
            set => _globalIndex = value;
        }

        public Vec2 lerpPosition
        {
            get => _lerpPosition;
            set => _lerpPosition = value;
        }

        public Vec2 lerpVector
        {
            get => _lerpVector;
            set => _lerpVector = value;
        }

        public float lerpSpeed
        {
            get => _lerpSpeed;
            set => _lerpSpeed = value;
        }

        public Portal portal
        {
            get => _portal;
            set => _portal = value;
        }

        public SequenceItem sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        public string type => _type;

        public Level level
        {
            get => _level;
            set => _level = value;
        }

        public float lastTeleportDirection
        {
            get => _lastTeleportDirection;
            set => _lastTeleportDirection = value;
        }

        public bool removeFromLevel
        {
            get => _removeFromLevel;
            set => _removeFromLevel = value;
        }

        public virtual int frame
        {
            get => !(graphic is SpriteMap) ? 0 : (graphic as SpriteMap).frame;
            set
            {
                if (!(graphic is SpriteMap))
                    return;
                (graphic as SpriteMap).frame = value;
            }
        }

        public bool placed
        {
            get => _placed;
            set => _placed = value;
        }

        public bool canBeGrouped => _canBeGrouped;

        public virtual Thing realObject => this;

        public virtual Thing owner
        {
            get => _owner;
            set
            {
                if (_owner != value)
                    _prevOwner = _owner;
                _lastThrownBy = _owner;
                _owner = value;
            }
        }

        public Thing prevOwner => _prevOwner;

        public Thing lastThrownBy => _lastThrownBy;

        public bool opaque => false;

        public virtual Sprite graphic
        {
            get => _graphic;
            set => _graphic = value;
        }

        public DamageMap damageMap
        {
            get => _damageMap;
            set => _damageMap = value;
        }

        public virtual bool visible
        {
            get => _visible;
            set => _visible = value;
        }
        public Material prevmaterial;
        public Material material
        {
            get => _material;
            set
            {
                prevmaterial = _material;
                _material = value;
            }
        }

        public virtual bool enablePhysics
        {
            get => _enablePhysics;
            set => _enablePhysics = value;
        }

        public float Distance(Thing pOther) => pOther != null ? (position - pOther.position).length : float.MaxValue;

        public void Presto()
        {
            if (!isServerForObject || removeFromLevel)
                return;
            if (Network.isActive)
                Send.Message(new NMPop(position));
            NMPop.AmazingDisappearingParticles(position);
            if (this is MaterialThing)
                (this as MaterialThing).Destroy(new DTPop());
            y += 9999f;
            if (this is Duck)
                return;
            Level.Remove(this);
        }

        public Profile responsibleProfile
        {
            set => _responsibleProfile = value;
            get
            {
                if (_responsibleProfile != null)
                {
                    return _responsibleProfile;
                }
                Duck d = this as Duck;
                if (d == null)
                {
                    d = (owner as Duck);
                    if (d == null)
                    {
                        d = (prevOwner as Duck);
                        if (d == null && owner != null)
                        {
                            d = (owner.owner as Duck);
                            if (d == null)
                            {
                                d = (owner.prevOwner as Duck);
                                if (d == null && prevOwner != null)
                                {
                                    d = (prevOwner.owner as Duck);
                                    if (d == null)
                                    {
                                        d = (prevOwner.prevOwner as Duck);
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

        public Type killThingType
        {
            get
            {
                if (_killThingType != null)
                    return _killThingType;
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
                return GetType();
            }
            set => _killThingType = value;
        }

        public virtual float hSpeed
        {
            get => _hSpeed;
            set => _hSpeed = value;
        }

        public virtual float vSpeed
        {
            get => _vSpeed;
            set => _vSpeed = value;
        }

        public Vec2 velocity
        {
            get => new Vec2(hSpeed, vSpeed);
            set
            {
                _hSpeed = value.x;
                _vSpeed = value.y;
            }
        }

        public void ApplyForce(Vec2 force)
        {
            _hSpeed += force.x;
            _vSpeed += force.y;
        }

        public void ApplyForce(Vec2 force, Vec2 limits)
        {
            limits = new Vec2(Math.Abs(limits.x), Math.Abs(limits.y));
            if (force.x < 0.0 && _hSpeed > -limits.x || force.x > 0.0 && _hSpeed < limits.x)
                _hSpeed += force.x;
            if ((force.y >= 0.0 || _vSpeed <= -limits.y) && (force.y <= 0.0 || _vSpeed >= limits.y))
                return;
            _vSpeed += force.y;
        }

        public void ApplyForceLimited(Vec2 force)
        {
            _hSpeed += force.x;
            if (force.x < 0.0 && _hSpeed < force.x || force.x > 0.0 && _hSpeed > force.x)
                _hSpeed = force.x;
            _vSpeed += force.y;
            if ((force.y >= 0.0 || _vSpeed >= force.y) && (force.y <= 0.0 || _vSpeed <= force.y))
                return;
            _vSpeed = force.y;
        }

        public virtual bool active
        {
            get => _active;
            set => _active = value;
        }

        public virtual bool ShouldUpdate() => true;

        public virtual bool action
        {
            get => _action;
            set => _action = value;
        }

        public Anchor anchor
        {
            get => _anchor;
            set => _anchor = value;
        }

        public virtual Vec2 anchorPosition => position;

        public virtual sbyte offDir
        {
            get => _offDir;
            set => _offDir = value;
        }

        public Layer layer
        {
            get => _layer;
            set
            {
                if (_layer == value)
                    return;
                if (_level != null)
                {
                    if (_layer != null)
                        _layer.Remove(this);
                    value.Add(this);
                }
                _layer = value;
            }
        }

        public bool isInitialized => _initialized;

        public List<Type> GetAllTypes() => Editor.AllBaseTypes[GetType()];

        public List<Type> GetAllTypesFiltered(Type stopAt) => GetAllTypes(GetType(), stopAt);

        public static List<Type> GetAllTypes(Type t, Type stopAt = null)
        {
            List<Type> allTypes = new List<Type>(t.GetInterfaces());
            allTypes.Add(t);
            for (Type baseType = t.BaseType; baseType != null && !(baseType == typeof(Thing)) && !(baseType == typeof(object)) && !(baseType == stopAt); baseType = baseType.BaseType)
                allTypes.Add(baseType);
            return allTypes;
        }

        public int placementCost => _placementCost;

        public string editorName
        {
            get
            {
                if (_editorName == "")
                {
                    _editorName = GetType().Name;
                    if (_editorName.Length > 1)
                    {
                        for (int index = 1; index < _editorName.Length; ++index)
                        {
                            char ch1 = _editorName[index];
                            if (ch1 >= 'A' && ch1 <= 'Z')
                            {
                                char ch2 = _editorName[index - 1];
                                if (ch2 >= 'a' && ch2 <= 'z')
                                {
                                    _editorName = _editorName.Insert(index, " ");
                                    ++index;
                                }
                            }
                        }
                    }
                }
                return _editorName;
            }
        }

        public void SetEditorName(string s) => _editorName = s;

        public virtual void TabRotate()
        {
            if (!_canFlip)
                return;
            flipHorizontal = !flipHorizontal;
        }

        public Layer placementLayer => placementLayerOverride != null ? placementLayerOverride : layer;

        public float likelyhoodToExist
        {
            get => _likelyhoodToExist;
            set => _likelyhoodToExist = value;
        }

        public bool editorCanModify => _editorCanModify;

        public bool processedByEditor
        {
            get => _processedByEditor;
            set => _processedByEditor = value;
        }

        public bool visibleInGame => _visibleInGame;

        public Vec2 editorOffset
        {
            get => _editorOffset;
            set => _editorOffset = value;
        }

        public WallHug hugWalls
        {
            get => _hugWalls;
            set => _hugWalls = value;
        }

        public static Thing Instantiate(Type t) => Editor.CreateThing(t);

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
            ghostType = Editor.IDToType[GetType()];
            DuckNetwork.AssignToHost(this);
        }

        public static Thing LoadThing(BinaryClassChunk node, bool chance = true)
        {
            Type type = Editor.GetType(node.GetProperty<string>("type"));
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
            Type type = GetType();
            binaryClassChunk.AddProperty("type", ModLoader.SmallTypeName(type));
            binaryClassChunk.AddProperty("x", x);
            binaryClassChunk.AddProperty("y", y);
            binaryClassChunk.AddProperty("chance", _likelyhoodToExist);
            binaryClassChunk.AddProperty("accessible", _isAccessible);
            binaryClassChunk.AddProperty("chanceGroup", _chanceGroup);
            binaryClassChunk.AddProperty("flipHorizontal", _flipHorizontal);
            if (_canFlipVert)
                binaryClassChunk.AddProperty("flipVertical", flipVertical);
            if (sequence != null)
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
            x = node.GetPrimitive<float>("x");
            y = node.GetPrimitive<float>("y");
            _likelyhoodToExist = node.GetPrimitive<float>("chance");
            _isAccessible = node.GetPrimitive<bool>("accessible");
            chanceGroup = node.GetPrimitive<int>("chanceGroup");
            flipHorizontal = node.GetPrimitive<bool>("flipHorizontal");
            if (_canFlipVert)
                flipVertical = node.GetPrimitive<bool>("flipVertical");
            if (sequence != null)
            {
                sequence.loop = node.GetPrimitive<bool>("loop");
                sequence.order = node.GetPrimitive<int>("popUpOrder");
                sequence.waitTillOrder = node.GetPrimitive<bool>("waitTillOrder");
            }
            Type type = GetType();
            foreach (FieldInfo fieldInfo in Editor.EditorFieldsForType[type])
            {
                if (node.HasProperty(fieldInfo.Name))
                {
                    Type genericArgument = fieldInfo.FieldType.GetGenericArguments()[0];
                    object obj = fieldInfo.GetValue(this);
                    obj.GetType().GetProperty("value").SetValue(obj, node.GetProperty(fieldInfo.Name), null);
                }
            }
            return true;
        }

        public static Thing LegacyLoadThing(DXMLNode node, bool chance = true)
        {
            Type type = Editor.GetType(node.Element("type").Value);
            if (!(type != null))
                return null;
            Thing thing = Editor.CreateThing(type);
            if (!thing.LegacyDeserialize(node))
                thing = null;
            return Level.current is Editor || !chance || thing.likelyhoodToExist == 1.0 || Level.PassedChanceGroup(thing.chanceGroup, thing.likelyhoodToExist) ? thing : null;
        }

        public bool isAccessible
        {
            get => _isAccessible;
            set => _isAccessible = value;
        }

        public virtual DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = new DXMLNode("Object");
            Type type = GetType();
            dxmlNode.Add(new DXMLNode("type", type.AssemblyQualifiedName));
            dxmlNode.Add(new DXMLNode("x", x));
            dxmlNode.Add(new DXMLNode("y", y));
            dxmlNode.Add(new DXMLNode("chance", _likelyhoodToExist));
            dxmlNode.Add(new DXMLNode("accessible", _isAccessible));
            dxmlNode.Add(new DXMLNode("chanceGroup", _chanceGroup));
            dxmlNode.Add(new DXMLNode("flipHorizontal", _flipHorizontal));
            if (_canFlipVert)
                dxmlNode.Add(new DXMLNode("flipVertical", _flipVertical));
            if (sequence != null)
            {
                dxmlNode.Add(new DXMLNode("loop", sequence.loop));
                dxmlNode.Add(new DXMLNode("popUpOrder", sequence.order));
                dxmlNode.Add(new DXMLNode("waitTillOrder", sequence.waitTillOrder));
            }
            foreach (Type key in Editor.AllBaseTypes[type])
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
                x = Change.ToSingle(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("y");
            if (dxmlNode2 != null)
                y = Change.ToSingle(dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("chance");
            if (dxmlNode3 != null)
                _likelyhoodToExist = Change.ToSingle(dxmlNode3.Value);
            DXMLNode dxmlNode4 = node.Element("accessible");
            if (dxmlNode4 != null)
                _isAccessible = Change.ToBoolean(dxmlNode4.Value);
            DXMLNode dxmlNode5 = node.Element("chanceGroup");
            if (dxmlNode5 != null)
                chanceGroup = Convert.ToInt32(dxmlNode5.Value);
            DXMLNode dxmlNode6 = node.Element("flipHorizontal");
            if (dxmlNode6 != null)
                flipHorizontal = Convert.ToBoolean(dxmlNode6.Value);
            if (_canFlipVert)
            {
                DXMLNode dxmlNode7 = node.Element("flipVertical");
                if (dxmlNode7 != null)
                    flipVertical = Convert.ToBoolean(dxmlNode7.Value);
            }
            if (sequence != null)
            {
                DXMLNode dxmlNode8 = node.Element("loop");
                if (dxmlNode8 != null)
                    sequence.loop = Convert.ToBoolean(dxmlNode8.Value);
                DXMLNode dxmlNode9 = node.Element("popUpOrder");
                if (dxmlNode9 != null)
                    sequence.order = Convert.ToInt32(dxmlNode9.Value);
                DXMLNode dxmlNode10 = node.Element("waitTillOrder");
                if (dxmlNode10 != null)
                    sequence.waitTillOrder = Convert.ToBoolean(dxmlNode10.Value);
            }
            foreach (Type key in Editor.AllBaseTypes[GetType()])
            {
                if (!key.IsInterface)
                {
                    foreach (FieldInfo fieldInfo in Editor.AllEditorFields[key])
                    {
                        DXMLNode dxmlNode11 = node.Element(fieldInfo.Name);
                        if (dxmlNode11 != null)
                        {
                            Type genericArgument = fieldInfo.FieldType.GetGenericArguments()[0];
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
            get => _flipHorizontal;
            set
            {
                _flipHorizontal = value;
                offDir = _flipHorizontal ? (sbyte)-1 : (sbyte)1;
            }
        }

        public virtual bool flipVertical
        {
            get => _flipVertical;
            set => _flipVertical = value;
        }

        public int chanceGroup
        {
            get => _chanceGroup;
            set => _chanceGroup = value;
        }

        public virtual ContextMenu GetContextMenu()
        {
            EditorGroupMenu owner = new EditorGroupMenu(null, true);
            if (_canFlip)
                owner.AddItem(new ContextCheckBox("Flip", null, new FieldBinding(this, "flipHorizontal")));
            if (_canFlipVert)
                owner.AddItem(new ContextCheckBox("Flip V", null, new FieldBinding(this, "flipVertical")));
            if (_canHaveChance)
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
            if (sequence != null && !_contextMenuFilter.Contains("Sequence"))
            {
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(owner)
                {
                    text = "Sequence"
                };
                owner.AddItem(editorGroupMenu);
                if (!_contextMenuFilter.Contains("Sequence|Loop"))
                    editorGroupMenu.AddItem(new ContextCheckBox("Loop", null, new FieldBinding(sequence, "loop")));
                if (!_contextMenuFilter.Contains("Sequence|Order"))
                    editorGroupMenu.AddItem(new ContextSlider("Order", null, new FieldBinding(sequence, "order", max: 100f), 1f, "RAND"));
                if (!_contextMenuFilter.Contains("Sequence|Wait"))
                    editorGroupMenu.AddItem(new ContextCheckBox("Wait", null, new FieldBinding(sequence, "waitTillOrder")));
            }
            List<string> stringList = new List<string>();
            foreach (Type key in Editor.AllBaseTypes[GetType()])
            {
                if (!key.IsInterface)
                {
                    foreach (FieldInfo fieldInfo in Editor.AllEditorFields[key])
                    {
                        if (!stringList.Contains(fieldInfo.Name) && !_contextMenuFilter.Contains(fieldInfo.Name))
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
            return GetEditorImage(wide, high, transparentBack, effect, target, false);
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
            if (_editorIcons.TryGetValue(GetType(), out editorImage1))
                return editorImage1;
            if (Thread.CurrentThread != MonoMain.mainThread)
                return new Sprite("basketBall");
            if (_alphaTestEffect == null)
                _alphaTestEffect = (Effect)Content.Load<MTEffect>("Shaders/alphatest");
            if (pUseCollisionSize && collisionSize.x > 0.0)
            {
                if (wide <= 0)
                    wide = (int)collisionSize.x;
                if (high <= 0)
                    high = (int)collisionSize.y;
            }
            else if (graphic != null)
            {
                if (wide <= 0)
                    wide = graphic.w;
                if (high <= 0)
                    high = graphic.h;
            }
            int num1 = wide > high ? wide : high;
            if (target == null)
                target = new RenderTarget2D(wide, high, true);
            if (graphic == null)
                return new Sprite(target, 0f, 0f);
            float num2 = num1 / (collisionSize.x > 0.0 & pUseCollisionSize ? collisionSize.x : graphic.width);
            Camera camera = new Camera(0f, 0f, wide, high)
            {
                position = new Vec2(x - centerx * num2, y - centery * num2)
            };
            if (pUseCollisionSize && collisionSize.x > 0.0)
                camera.center = new Vec2((int)((left + right) / 2.0), (int)((top + bottom) / 2.0));
            RenderTarget2D currentRenderTarget = Graphics.currentRenderTarget;
            Graphics.SetRenderTarget(target);
            DepthStencilState depthStencilState = new DepthStencilState()
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };
            Graphics.Clear(transparentBack ? Color.Transparent : new Color(15, 4, 16));
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, (MTEffect)(effect == null ? _alphaTestEffect : effect), camera.getMatrix());
            Draw();
            Graphics.screen.End();
            if (currentRenderTarget == null || currentRenderTarget.IsDisposed)
                Graphics.SetRenderTarget(null);
            else
                Graphics.SetRenderTarget(currentRenderTarget);
            Texture2D tex = new Texture2D(Graphics.device, target.width, target.height);
            tex.SetData(target.GetData());
            Sprite editorImage2 = new Sprite((Tex2D)tex);
            _editorIcons[GetType()] = editorImage2;
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
            if (flag && _editorIcon != null)
                return _editorIcon;
            if (Thread.CurrentThread != MonoMain.mainThread)
                return new Sprite("basketBall");
            if (_alphaTestEffect == null)
                _alphaTestEffect = (Effect)Content.Load<MTEffect>("Shaders/alphatest");
            if (graphic != null)
            {
                if (wide <= 0)
                    wide = graphic.w;
                if (high <= 0)
                    high = graphic.h;
            }
            if (target == null)
                target = new RenderTarget2D(wide, high, true);
            if (graphic == null)
                return new Sprite(target, 0f, 0f);
            Camera camera = new Camera(0f, 0f, wide, high)
            {
                position = new Vec2(x - wide / 2, y - high / 2)
            };
            Graphics.SetRenderTarget(target);
            DepthStencilState depthStencilState = new DepthStencilState()
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };
            Graphics.Clear(transparentBack ? Color.Transparent : new Color(30, 30, 30));
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, (MTEffect)(effect == null ? _alphaTestEffect : effect), camera.getMatrix());
            Draw();
            Graphics.screen.End();
            Graphics.SetRenderTarget(null);
            Texture2D tex = new Texture2D(Graphics.device, target.width, target.height);
            tex.SetData(target.GetData());
            List<string> text = Content.RSplit(GetType().AssemblyQualifiedName, ',', -1);
            string texname = text[0] + text[1] + wide.ToString() + " " + high.ToString();
            tex.Name = texname;
            Content.textures[texname] = tex; //spritea las stuff
            Sprite preview = new Sprite((Tex2D)tex);
            if (flag)
                _editorIcon = preview;
            return preview;
        }

        public virtual bool solid
        {
            get => _solid;
            set
            {
                if (value && !_solid)
                    FixClipping();
                _solid = value;
            }
        }

        public void FixClipping()
        {
            foreach (Block block in Level.CheckRectAll<Block>(topLeft, bottomRight))
                ;
        }

        private string GetPropertyDetails()
        {
            string propertyDetails = "";
            foreach (FieldInfo fieldInfo in Editor.AllEditorFields[GetType()])
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
            if (_likelyhoodToExist == 1.0 && _chanceGroup == -1)
                return GetPropertyDetails();
            return "Chance: " + Math.Round(likelyhoodToExist / 1.0 * 100.0).ToString() + "%\nChance Group: " + (_chanceGroup == -1 ? "None" : _chanceGroup.ToString(CultureInfo.InvariantCulture)) + "\n" + GetPropertyDetails();
        }

        public virtual void ReturnItemToWorld(Thing t)
        {
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

        public bool isOffBottomOfLevel => y > Level.activeLevel.lowestPoint + 100.0 && top > Level.current.camera.bottom + 8.0;

        public virtual Vec2 collisionOffset
        {
            get => _collisionOffset;
            set => _collisionOffset = value;
        }

        public virtual Vec2 collisionSize
        {
            get => _collisionSize;
            set => _collisionSize = value;
        }

        public float topQuick => _topQuick;

        public float bottomQuick => _bottomQuick;

        public float leftQuick => _leftQuick;

        public float rightQuick => _rightQuick;

        public float topLocal => collisionOffset.y;

        public float bottomLocal => collisionOffset.y + collisionSize.y;

        public float top
        {
            get => position.y + _collisionOffset.y;
            set => position.y = value + (position.y - top);
        }

        public float bottom
        {
            get => position.y + _collisionOffset.y + _collisionSize.y;
            set => position.y = value + (position.y - bottom);
        }

        public float left
        {
            get => offDir <= 0 ? position.x - _collisionSize.x - _collisionOffset.x : position.x + _collisionOffset.x;
            set => x = value + (x - left);
        }

        public float right
        {
            get => offDir <= 0 ? position.x - _collisionOffset.x : position.x + _collisionOffset.x + _collisionSize.x;
            set => x = value + (x - right);
        }

        public Vec2 topLeft => new Vec2(left, top);

        public Vec2 topRight => new Vec2(right, top);

        public Vec2 bottomLeft => new Vec2(left, bottom);

        public Vec2 bottomRight => new Vec2(right, bottom);

        public Vec2 NearestCorner(Vec2 to)
        {
            Vec2 vec2 = topLeft;
            float num = (topLeft - to).length;
            float length1 = (topRight - to).length;
            if (length1 < num)
            {
                vec2 = topRight;
                num = length1;
            }
            float length2 = (bottomLeft - to).length;
            if (length2 < num)
            {
                vec2 = bottomLeft;
                num = length2;
            }
            if ((bottomRight - to).length < num)
                vec2 = bottomRight;
            return vec2;
        }

        public Vec2 NearestOpenCorner(Vec2 to)
        {
            Vec2 vec2_1 = Vec2.Zero;
            float num = 9999999f;
            float length1 = (topLeft - to).length;
            if (length1 < num && Level.CheckCircle<Block>(topLeft, 2f, this) == null)
            {
                vec2_1 = topLeft;
                num = length1;
            }
            Vec2 vec2_2 = topRight - to;
            float length2 = vec2_2.length;
            if (length2 < num && Level.CheckCircle<Block>(topRight, 2f, this) == null)
            {
                vec2_1 = topRight;
                num = length2;
            }
            vec2_2 = bottomLeft - to;
            float length3 = vec2_2.length;
            if (length3 < num && Level.CheckCircle<Block>(bottomLeft, 2f, this) == null)
            {
                vec2_1 = bottomLeft;
                num = length3;
            }
            vec2_2 = bottomRight - to;
            if (vec2_2.length < num && Level.CheckCircle<Block>(bottomRight, 2f, this) == null)
                vec2_1 = bottomRight;
            return vec2_1;
        }

        public bool isStatic
        {
            get => _isStatic;
            set => _isStatic = value;
        }

        public float halfWidth => width / 2f;

        public float halfHeight => height / 2f;

        public float width => _collisionSize.x * scale.x;

        public float height => _collisionSize.y * scale.y;

        public float w => width;

        public float h => height;

        public Rectangle rectangle => new Rectangle((int)left, (int)top, (int)(right - left), (int)(bottom - top));

        public Vec2 collisionCenter
        {
            get => new Vec2(left + collisionSize.x / 2f, top + collisionSize.y / 2f);
            set
            {
                left = value.x - collisionSize.x / 2f;
                top = value.y - collisionSize.y / 2f;
            }
        }

        public Thing(float xval = 0f, float yval = 0f, Sprite sprite = null)
        {
            x = xval;
            y = yval;
            graphic = sprite;
            if (sprite != null)
                _collisionSize = new Vec2(sprite.w, sprite.h);
            if (!Network.isActive)
                return;
            connection = DuckNetwork.localConnection;
        }

        public virtual Vec2 OffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * scale;
            if (offDir < 0)
                vec2.x *= -1f;
            return vec2.Rotate(angle, new Vec2(0f, 0f));
        }

        public virtual Vec2 ReverseOffsetLocal(Vec2 pos)
        {
            Vec2 vec2 = pos * scale;
            vec2 = vec2.Rotate(-angle, new Vec2(0f, 0f));
            return vec2;
        }

        public virtual Vec2 Offset(Vec2 pos) => position + OffsetLocal(pos);

        public virtual Vec2 ReverseOffset(Vec2 pos)
        {
            pos -= position;
            return ReverseOffsetLocal(pos);
        }

        public virtual float OffsetX(float pos)
        {
            Vec2 vec2 = new Vec2(pos, 0f);
            if (offDir < 0)
                vec2.x *= -1f;
            return (position + vec2.Rotate(angle, new Vec2(0f, 0f))).x;
        }

        public virtual float OffsetY(float pos)
        {
            Vec2 vec2 = new Vec2(0f, pos);
            if (offDir < 0)
                vec2.x *= -1f;
            return (position + vec2.Rotate(angle, new Vec2(0f, 0f))).y;
        }

        public virtual void ResetProperties()
        {
            _level = null;
            _removeFromLevel = false;
            _initialized = false;
        }

        public void AddToLayer()
        {
            if (_layer == null)
                _layer = Layer.Game;
            if (skipLayerAdding)
                return;
            _layer.Add(this);
        }

        public void DoNetworkInitialize()
        {
            if (_networkInitialized)
                return;
            if (isStateObject)
            {
                _ghostType = Editor.IDToType[GetType()];
                if (Network.isServer)
                    connection = DuckNetwork.localConnection;
            }
            _networkInitialized = true;
        }

        public virtual void DoInitialize()
        {
            if (_redoLayer)
            {
                AddToLayer();
                _redoLayer = false;
            }
            if (_initialized)
                return;
            if (Network.isActive)
                DoNetworkInitialize();
            _networkDrawIndex = NetworkDebugger.currentIndex;
            Initialize();
            _initialized = true;
        }

        public virtual void Initialize()
        {
        }

        public virtual void DoUpdate()
        {
            if (wasSuperFondled > 0)
                --wasSuperFondled;
            if (_anchor != null)
                position = _anchor.position;
            Update();
            if (Buckets.Length > 0 && ((oldcollisionOffset != collisionOffset || oldcollisionSize != collisionSize) || (oldposition - position).LengthSquared() > 100f) && Level.current != null) //((oldposition - position)).length > 10
            {
                oldcollisionOffset = collisionOffset;
                oldcollisionSize = collisionSize;
                oldposition = position;
                Level.current.things.UpdateObject(this);
            }
            //_topQuick = top;
            //_bottomQuick = bottom;
            //_leftQuick = left;
            //_rightQuick = right;
        }

        public virtual void Update()
        {
        }

        public virtual void InactiveUpdate()
        {
        }

        public virtual void DoEditorUpdate()
        {
            //_topQuick = top;
            //_bottomQuick = bottom;
            //_leftQuick = left;
            //_rightQuick = right;
            EditorUpdate();
        }

        public virtual void EditorUpdate()
        {
        }

        public virtual void DoEditorRender() => EditorRender();

        public virtual void EditorRender()
        {
        }

        public virtual void OnEditorLoaded()
        {
        }

        public void Glitch()
        {
            if (!(material is MaterialGlitch))
                return;
            (material as MaterialGlitch).yoffset = Rando.Float(1f);
            (material as MaterialGlitch).amount = Rando.Float(0.9f, 1.2f);
        }

        public ProfileNetData GetOrCreateNetData()
        {
            if (_netData == null)
                _netData = new ProfileNetData();
            return _netData;
        }

        public void NetworkSet<T>(string pVariable, T pValue)
        {
            if (!isServerForObject)
                return;
            if (_netData == null)
                _netData = new ProfileNetData();
            _netData.Set(pVariable, pValue);
        }
        public T NetworkGet<T>(string pVariable, T pDefault = default(T))
        {
            if (_netData == null)
            {
                return default(T);
            }
            return _netData.Get(pVariable, pDefault);
        }
        public GhostObject ghostObject
        {
            get => _ghostObject;
            set => _ghostObject = value;
        }

        public virtual void OnGhostObjectAdded()
        {
        }

        /// <summary>
        /// If true, this object's Update function is run via Level.UpdateThings. Otherwise, it's run via GhostManager.UpdateGhostLerp
        /// </summary>
        public bool shouldRunUpdateLocally => (connection == null || connection.data == null) && level != null;

        public bool ignoreGhosting
        {
            get => _ignoreGhosting;
            set => _ignoreGhosting = value;
        }

        public virtual void DoDraw()
        {
            //if (NetworkDebugger.currentIndex >= 0 && NetworkDebugger.currentIndex != _networkDrawIndex)
            //     return;
            Graphics.material = _material;
            if (_material != null)
                _material.Update();
            Draw();
            Graphics.material = null;
        }

        public virtual void Draw()
        {
            if (_graphic == null)
                return;
            if (!_skipPositioning)
            {
                _graphic.position = position;
                _graphic.alpha = alpha;
                _graphic.angle = angle;
                _graphic.depth = depth;
                _graphic.scale = scale;
                _graphic.center = center;
            }
            _graphic.Draw();
        }
        public void DrawOwner()
        {
            if (this.ghostObject == null)
            {
                return;
            }
            if (this.connection == null || this.connection.profile == null)
            {
                DuckGame.Graphics.DrawRect(topLeft, bottomRight, Colors.Duck1, (Depth)1f, false, 0.5f);
            }
            else
            {
                DuckGame.Graphics.DrawRect(topLeft, bottomRight, this.connection.profile.persona.colorUsable, (Depth)1f, false, 0.5f);
            }
        }
        public void DrawCollision()
        {
            Graphics.DrawRect(topLeft, bottomRight, Color.Orange * 0.8f, 1f, false, 0.5f);

            if (this is PhysicsObject)
            {
                if ((this as PhysicsObject).sleeping)
                {
                    Graphics.DrawRect(topLeft, bottomRight, Color.LightBlue * 0.8f, (Depth)1f, false, 0.5f);
                }
                else
                {
                    Graphics.DrawRect(topLeft, bottomRight, Color.Red * 0.8f, (Depth)1f, false, 0.5f);
                }

            }
            else if (this is PhysicsParticle)
            {
                if ((this as PhysicsParticle)._grounded)
                {
                    Graphics.DrawRect(topLeft, bottomRight, Color.LightBlue * 0.8f, (Depth)1f, false, 0.5f);
                }
                else
                {
                    Graphics.DrawRect(topLeft, bottomRight, Color.Red * 0.8f, (Depth)1f, false, 0.5f);
                }
            }
            else if (this is FluidPuddle)
            {
                Graphics.DrawRect(topLeft, bottomRight, Color.Orange * 0.8f, (Depth)1f, false, 0.5f);
            }
            //else
            //{
            //    DuckGame.Graphics.DrawRect(topLeft, bottomRight, Color.Purple * 0.8f, (Depth)1f, false, 0.5f);
            //}
            //     return;
            // int num = (this as PhysicsObject).sleeping ? 1 : 0;

            // if (!(this is PhysicsObject))
            //     return;
            // int num = (this as PhysicsObject).sleeping ? 1 : 0;
        }

        public void Draw(Sprite spr, float xpos, float ypos, int d = 1) => Draw(spr, new Vec2(xpos, ypos), d);

        public void Draw(Sprite spr, Vec2 pos, int d = 1)
        {
            Vec2 vec2 = Offset(pos);
            if (graphic != null)
                spr.flipH = graphic.flipH;
            spr.angle = angle;
            spr.alpha = alpha;
            spr.depth = depth + d;
            spr.scale = scale;
            spr.flipH = offDir < 0;
            Graphics.Draw(spr, vec2.x, vec2.y);
        }

        public void DrawIgnoreAngle(Sprite spr, Vec2 pos, int d = 1)
        {
            Vec2 vec2 = Offset(pos);
            spr.alpha = alpha;
            spr.depth = depth + d;
            spr.scale = scale;
            Graphics.Draw(spr, vec2.x, vec2.y);
        }

        public virtual void OnTeleport()
        {
        }

        public virtual void DoTerminate() => Terminate();

        public virtual void Terminate()
        {
        }

        public virtual void Added(Level parent)
        {
            _removeFromLevel = false;
            _redoLayer = true;
            _level = parent;
            DoInitialize();
        }

        public virtual void Added(Level parent, bool redoLayer, bool reinit)
        {
            if (reinit)
                _initialized = false;
            _removeFromLevel = false;
            _redoLayer = redoLayer;
            _level = parent;
            DoInitialize();
        }

        public virtual void Removed()
        {
            _removeFromLevel = true;
            if (_layer == null)
                return;
            _layer.RemoveSoon(this);
        }

        public virtual void NetworkUpdate()
        {
        }

        public virtual void OnSequenceActivate()
        {
        }
    }
}
