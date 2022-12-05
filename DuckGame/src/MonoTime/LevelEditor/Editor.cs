// Decompiled with JetBrains decompiler
// Type: DuckGame.Editor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using static DuckGame.CMD;
//using System.Windows.Forms;
namespace DuckGame
{
    public class Editor : Level
    {
        public static bool editingContent = false;
        private static Stack<object> focusStack = new Stack<object>();
        private static int numPops;
        private EditorCam _editorCam;
        private SpriteMap _cursor;
        private SpriteMap _tileset;
        private BitmapFont _font;
        private ContextMenu _placementMenu;
        private ContextMenu _objectMenu;
        private CursorMode _cursorMode;
        public static bool active;
        public static bool selectingLevel = false;

        private InputType dragModeInputType;

        //private InputType dragStartInputType;
        private Vec2 _sizeRestriction = new Vec2(800f, 640f);
        public static int placementLimit;
        public int placementTotalCost;
        private Vec2 _topLeftMost = new Vec2(99999f, 99999f);
        private Vec2 _bottomRightMost = new Vec2(-99999f, -99999f);
        public static bool hasUnsavedChanges;
        public static Texture2D previewCapture;
        private BinaryClassChunk _eyeDropperSerialized;

        private static Dictionary<Type, List<MethodInfo>> _networkActionIndexes =
            new Dictionary<Type, List<MethodInfo>>();

        private static EditorGroup _placeables;
        protected List<Thing> _levelThingsNormal = new List<Thing>();
        protected List<Thing> _levelThingsAlternate = new List<Thing>();
        public static string placementItemDetails = "";
        private string _saveName = "";
        //private SaveFileDialog _saveForm = new SaveFileDialog(); wasnt used and created other issues
       // private OpenFileDialog _loadForm = new OpenFileDialog(); wasnt used and created other issues
        public static bool enteringText = false;
        private static ContextMenu _lockInput;
        private static ContextMenu _lockInputChange;
        private int _lastCommand = -1;
        private List<Command> _commands = new List<Command>();
        public static bool clickedMenu;
        public static bool clickedContextBackground;
        public bool clicked;
        private bool _updateEvenWhenInactive;
        private bool _editorLoadFinished;
        private NotifyDialogue _notify;
        private bool _placementMode = true;
        private bool _editMode;
        private bool _copyMode;
        public static bool hoverUI;
        public static EditorInput inputMode = EditorInput.Gamepad;
        private SpriteMap _editorButtons;
        private bool _loadingLevel;
        private List<Thing> _placeObjects = new List<Thing>();
        public bool minimalConversionLoad;
        private bool processingMirror;
        private bool _isPaste;
        private bool _looseClear;
        public static LevelData _currentLevelData = new LevelData();
        public static bool saving;
        private int _gridW = 40;
        private int _gridH = 24;
        private float _cellSize = 16f;
        private Vec2 _camSize;
        private Vec2 _panAnchor;
        private Vec2 _tilePosition;
        private bool _closeMenu;
        private bool _placingTiles;
        private bool _dragMode;
        private bool _deleteMode;
        private bool _didPan;
        private static bool _listLoaded;
        private Thing _placementType;
        private MonoFileDialog _fileDialog;
        private SteamUploadDialog _uploadDialog;
        private static string _initialDirectory;
        private bool _menuOpen;
        private Layer _gridLayer;
        private Layer _procLayer;
        private Layer _objectMenuLayer;
        public bool _pathNorth;
        public bool _pathSouth;
        public bool _pathEast;
        public bool _pathWest;
        private bool _quitting;
        private Sprite _cantPlace;
        private Sprite _sideArrow;
        private Sprite _sideArrowHover;
        private Sprite _die;
        private Sprite _dieHover;
        private Sprite _editorCurrency;
        private HashSet<Thing> _selection = new HashSet<Thing>();
        private Sprite _singleBlock;
        private Sprite _multiBlock;
        public bool _miniMode;
        public Vec2 _genSize = new Vec2(3f, 3f);
        public Vec2 _genTilePos = new Vec2(1f, 1f);
        public Vec2 _editTilePos = new Vec2(1f, 1f);
        public Vec2 _prevEditTilePos = new Vec2(1f, 1f);
        public MaterialSelection _selectionMaterial;
        public MaterialSelection _selectionMaterialPaste;
        public int generatorComplexity;
        private bool _editingOpenAirVariationPrev;
        public bool editingOpenAirVariation;
        private string _additionalSaveDirectory;
        private bool _doingResave;
        private List<string> existingGUID = new List<string>();
        private ContextMenu _lastHoverMenuOpen;
        private Thing _hover;
        private Thing _secondaryHover;
        private bool bMouseInput;
        private bool bGamepadInput;
        private bool bTouchInput;
        private Thing _move;
        private bool _showPlacementMenu;
        private static InputProfile _input;
        private MessageDialogue _noSpawnsDialogue;
        private bool _runLevelAnyway;
        public static bool copying;
        private Vec2 _tileDragContext = Vec2.MinValue;
        private Vec2 _tilePositionPrev = Vec2.Zero;
        private Vec2 _tileDragDif = Vec2.Zero;
        private Vec2 _lastTilePosDraw = Vec2.Zero;
        public static bool tookInput;
        public static bool didUIScroll;
        private ContextMenu _hoverMenu;
        private int _hoverMode;
        private GameContext _procContext;
        protected int _procSeed;
        private TileButton _hoverButton;
        public bool _doGen;
        private int _prevProcX;
        private int _prevProcY;
        private RandomLevelNode _currentMapNode;
        private int _loadPosX;
        private int _loadPosY;
        public static bool skipFrame;
        private bool firstClick;
        public static Thing openContextThing;
        public static Thing pretendPinned;
        public static Vec2 openPosition = Vec2.Zero;
        public static bool ignorePinning;
        public static bool reopenContextMenu;
        private Vec2 middleClickPos;
        private Vec2 lastMousePos = Vec2.Zero;
        public static string tooltip;
        private bool _twoFingerGesture;
        private bool _twoFingerGestureStarting;
        private bool _twoFingerZooming;
        private bool _threeFingerGesture;
        private bool _threeFingerGestureRelease;
        private float _twoFingerSpacing;
        private EditorTouchButton _activeTouchButton;

        private EditorTouchState _touchState;

        //private bool _prevTouch;
        private List<EditorTouchButton> _touchButtons = new List<EditorTouchButton>();
        private List<EditorTouchButton> _fileDialogButtons = new List<EditorTouchButton>();
        private EditorTouchButton _cancelButton;
        private EditorTouchButton _editTilesButton;
        public static bool fakeTouch = false;
        public static bool _clickedTouchButton;
        private Thing _oldHover;
        public static bool waitForNoTouchInput;
        public static bool bigInterfaceMode;
        private bool _openTileSelector;
        private List<ContextMenu.SearchPair> searchItems = new List<ContextMenu.SearchPair>();
        public static bool hoverMiniButton;
        private bool clearedKeyboardStringForSearch;
        private string _prevSearchString = "";
        private Vec2 _selectionDragStart = Vec2.Zero;
        private Vec2 _selectionDragEnd = Vec2.Zero;
        private Vec2 _moveDragStart = Vec2.Zero;
        public HashSet<Thing> _currentDragSelectionHover = new HashSet<Thing>();
        public HashSet<Thing> _currentDragSelectionHoverAdd = new HashSet<Thing>();
        private int focusWait = 5;
        private bool _openedEditMenu;
        private List<BinaryClassChunk> _selectionCopy = new List<BinaryClassChunk>();
        private List<Thing> _pasteBatch = new List<Thing>();
        private Vec2 _copyCenter;
        private Vec2 pasteOffset;
        private bool _performCopypaste;
        private bool _dragSelectShiftModifier;

        private Thing oldHover;

        //private Thing oldSecondaryHover;
        public static bool editorDraw;
        public static int _procXPos = 1;
        public static int _procYPos = 1;
        public static int _procTilesWide = 3;
        public static int _procTilesHigh = 3;
        public static bool hoverTextBox;

        private Rectangle _ultimateBounds;

        //private bool _leftSelectionDraw = true;
        private int _searchHoverIndex = -1;
        private bool rotateValid;
        private RenderTarget2D _procTarget;
        private Vec2 _procDrawOffset = Vec2.Zero;
        private bool _onlineSettingChanged;
        public bool hadGUID;
        public static long kMassiveBitmapStringHeader = 4967129034509872376;
        protected RandomLevelData _centerTile;
        public static bool isTesting;
        public bool searching;
        private static Dictionary<Type, Thing> _thingMap = new Dictionary<Type, Thing>();

        private static Dictionary<Type, List<ClassMember>> _classMembers =
            new Dictionary<Type, List<ClassMember>>();

        private static Dictionary<Type, List<ClassMember>> _staticClassMembers =
            new Dictionary<Type, List<ClassMember>>();

        private static Dictionary<Type, Dictionary<string, ClassMember>> _classMemberNames =
            new Dictionary<Type, Dictionary<string, ClassMember>>();

        public static Dictionary<Type, Dictionary<string, AccessorInfo>> _accessorCache =
            new Dictionary<Type, Dictionary<string, AccessorInfo>>();

        private static Dictionary<Type, object[]> _constructorParameters =
            new Dictionary<Type, object[]>();

        private static Dictionary<Type, ThingConstructor> _defaultConstructors =
            new Dictionary<Type, ThingConstructor>();

        private static Dictionary<Type, Func<object>> _constructorParameterExpressions =
            new Dictionary<Type, Func<object>>();

        public static List<Type> ThingTypes;
        public static List<Type> GroupThingTypes;
        public static Dictionary<Type, List<Type>> AllBaseTypes;
        public static Dictionary<Type, IEnumerable<FieldInfo>> AllEditorFields;
        public static Dictionary<Type, List<FieldInfo>> EditorFieldsForType;
        public static Dictionary<Type, FieldInfo[]> AllStateFields;
        public static Map<ushort, Type> IDToType = new Map<ushort, Type>();
        public static Dictionary<Type, Thing> _typeInstances = new Dictionary<Type, Thing>();
        public bool tabletMode;
        public static uint thingTypesHash;
        private static bool _clearOnce;

        public static void PopFocus() => ++numPops;

        public static void PopFocusNow()
        {
            if (focusStack.Count <= 0)
                return;
            focusStack.Pop();
        }

        public static object PeekFocus() => focusStack.Count > 0 ? focusStack.Peek() : null;

        public static void PushFocus(object o) => focusStack.Push(o);

        public static bool HasFocus() => focusStack.Count != 0;

        public bool placementLimitReached => placementLimit > 0 && placementTotalCost >= placementLimit;

        public bool placementOutOfSizeRange => false;

        public static List<string> activatedLevels => DuckNetwork.core._activatedLevels;

        public static int customLevelCount => activatedLevels.Count + clientLevelCount;

        public static int clientLevelCount
        {
            get
            {
                if (!(bool)TeamSelect2.GetMatchSetting("clientlevelsenabled").value)
                    return 0;
                int clientLevelCount = 0;
                if (DuckNetwork.profiles != null)
                {
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile != null && profile.connection != null &&
                            profile.connection.status != ConnectionStatus.Disconnected)
                            clientLevelCount += profile.numClientCustomLevels;
                    }
                }

                return clientLevelCount;
            }
        }

        public static byte NetworkActionIndex(Type pType, MethodInfo pMethod)
        {
            int num = GetNetworkActionMethods(pType).IndexOf(pMethod);
            return num >= 0 ? (byte)num : byte.MaxValue;
        }

        public static MethodInfo MethodFromNetworkActionIndex(Type pType, byte pIndex)
        {
            List<MethodInfo> networkActionMethods = GetNetworkActionMethods(pType);
            return pIndex < networkActionMethods.Count ? networkActionMethods[pIndex] : null;
        }

        private static List<MethodInfo> GetNetworkActionMethods(Type pType)
        {
            if (!_networkActionIndexes.ContainsKey(pType))
            {
                List<MethodInfo> methodInfoList = new List<MethodInfo>();
                foreach (MethodInfo method in pType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                               BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttributes(typeof(NetworkAction), false).Any())
                        methodInfoList.Add(method);
                }

                if (pType.BaseType != null)
                {
                    List<MethodInfo> networkActionMethods = GetNetworkActionMethods(pType.BaseType);
                    methodInfoList.AddRange(networkActionMethods);
                }

                _networkActionIndexes[pType] = methodInfoList;
            }

            return _networkActionIndexes[pType];
        }

        public static EditorGroup Placeables
        {
            get
            {
                while (!_listLoaded)
                    Thread.Sleep(16);
                return _placeables;
            }
        }

        protected List<Thing> _levelThings => !editingOpenAirVariation ? _levelThingsNormal : _levelThingsAlternate;

        public List<Thing> levelThings => _levelThings;

        public string saveName
        {
            get => _saveName;
            set => _saveName = value;
        }

        public static ContextMenu lockInput
        {
            get => _lockInput;
            set => _lockInputChange = value;
        }

        //private void RunCommand(Command command)
        //{
        //    Editor.hasUnsavedChanges = true;
        //    if (this._lastCommand < this._commands.Count - 1)
        //        this._commands.RemoveRange(this._lastCommand + 1, this._commands.Count - (this._lastCommand + 1));
        //    this._commands.Add(command);
        //    ++this._lastCommand;
        //    command.Do();
        //}

        //private void UndoCommand()
        //{
        //    Editor.hasUnsavedChanges = true;
        //    if (this._lastCommand < 0)
        //        return;
        //    this._commands[this._lastCommand--].Undo();
        //}

        //private void RedoCommand()
        //{
        //    Editor.hasUnsavedChanges = true;
        //    if (this._lastCommand >= this._commands.Count - 1)
        //        return;
        //    this._commands[++this._lastCommand].Do();
        //}

        public void AddObject(Thing obj)
        {
            hasUnsavedChanges = true;
            if (obj == null)
                return;
            switch (obj)
            {
                case ThingContainer _:
                    ThingContainer thingContainer = obj as ThingContainer;
                    if (thingContainer.bozocheck)
                    {
                        using (List<Thing>.Enumerator enumerator = thingContainer.things.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Thing current = enumerator.Current;
                                if (!Thing.CheckForBozoData(current))
                                    AddObject(current);
                            }
                            return;
                        }
                    }
                    else
                    {
                        using (List<Thing>.Enumerator enumerator = thingContainer.things.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                                AddObject(enumerator.Current);
                            return;
                        }
                    }
                case BackgroundUpdater _:
                    for (int index = 0; index < _levelThings.Count; ++index)
                    {
                        Thing t = _levelThings[index];
                        if (t is BackgroundUpdater)
                        {
                            History.Add(() => RemoveObject(t), () => AddObject(t));
                            --index;
                        }
                    }
                    break;
            }
            obj.active = false;
            AddThing(obj);
            _levelThings.Add(obj);
            if (!_loadingLevel && obj is IDontMove)
                _placeObjects.Add(obj);
            placementTotalCost += CalculatePlacementCost(obj);
            if (_sizeRestriction.x > 0.0)
                AdjustSizeLimits(obj);
            if (_loadingLevel)
                return;
            if (!_isPaste)
                obj.EditorAdded();
            if (obj is MirrorMode || processingMirror || obj is BackgroundUpdater)
                return;
            processingMirror = true;
            foreach (MirrorMode mirrorMode in things[typeof(MirrorMode)])
            {
                if (((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both ||
                     (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Vertical) &&
                    Math.Abs(mirrorMode.position.x - obj.position.x) > 2.0)
                {
                    Vec2 vec2 = obj.position -
                                new Vec2((float)((obj.position.x - mirrorMode.position.x) * 2.0), 0f);
                    Thing thing = Thing.LoadThing(obj.Serialize());
                    thing.position = vec2;
                    thing.flipHorizontal = !obj.flipHorizontal;
                    AddObject(thing);
                    thing.EditorFlip(false);
                }

                if (((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both ||
                     (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Horizontal) &&
                    Math.Abs(mirrorMode.position.y - obj.position.y) > 2.0)
                {
                    Vec2 vec2 = obj.position -
                                new Vec2(0f, (float)((obj.position.y - mirrorMode.position.y) * 2.0));
                    Thing thing = Thing.LoadThing(obj.Serialize());
                    thing.position = vec2;
                    AddObject(thing);
                    thing.EditorFlip(true);
                }

                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both &&
                    Math.Abs(mirrorMode.position.x - obj.position.x) > 2.0 &&
                    Math.Abs(mirrorMode.position.y - obj.position.y) > 2.0)
                {
                    Vec2 vec2 = obj.position - new Vec2((float)((obj.position.x - mirrorMode.position.x) * 2.0),
                        (float)((obj.position.y - mirrorMode.position.y) * 2.0));
                    Thing thing = Thing.LoadThing(obj.Serialize());
                    thing.position = vec2;
                    thing.flipHorizontal = !obj.flipHorizontal;
                    AddObject(thing);
                    thing.EditorFlip(false);
                    thing.EditorFlip(true);
                }
            }

            processingMirror = false;
        }

        public void RemoveObject(Thing obj)
        {
            hasUnsavedChanges = true;
            current.RemoveThing(obj);
            _levelThings.Remove(obj);
            if (obj is IDontMove)
                _placeObjects.Add(obj);
            placementTotalCost -= CalculatePlacementCost(obj);
            if (_sizeRestriction.x > 0.0 && (obj.x <= _topLeftMost.x || obj.x >= _bottomRightMost.x ||
                                             obj.y <= _topLeftMost.y || obj.y >= _bottomRightMost.y))
                RecalculateSizeLimits();
            obj.EditorRemoved();
            if (_loadingLevel || obj is MirrorMode || processingMirror || obj is BackgroundUpdater)
                return;
            processingMirror = true;
            foreach (MirrorMode mirrorMode in things[typeof(MirrorMode)])
            {
                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both ||
                    (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Vertical)
                {
                    Thing thing = current.CollisionPoint(
                        obj.position + new Vec2((float)(-(obj.position.x - mirrorMode.position.x) * 2.0), 0f),
                        obj.GetType());
                    if (thing != null)
                        RemoveObject(thing);
                }

                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both ||
                    (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Horizontal)
                {
                    Thing thing = current.CollisionPoint(
                        obj.position + new Vec2(0f, (float)(-(obj.position.y - mirrorMode.position.y) * 2.0)),
                        obj.GetType());
                    if (thing != null)
                        RemoveObject(thing);
                }

                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both)
                {
                    Thing thing = current.CollisionPoint(
                        obj.position + new Vec2((float)(-(obj.position.x - mirrorMode.position.x) * 2.0),
                            (float)(-(obj.position.y - mirrorMode.position.y) * 2.0)), obj.GetType());
                    if (thing != null)
                        RemoveObject(thing);
                }
            }

            processingMirror = false;
        }

        public void AdjustSizeLimits(Thing pObject)
        {
            if (pObject.x < _topLeftMost.x)
                _topLeftMost.x = pObject.x;
            if (pObject.x > _bottomRightMost.x)
                _bottomRightMost.x = pObject.x;
            if (pObject.y < _topLeftMost.y)
                _topLeftMost.y = pObject.y;
            if (pObject.y <= _bottomRightMost.y)
                return;
            _bottomRightMost.y = pObject.y;
        }

        public void RecalculateSizeLimits()
        {
            _topLeftMost = new Vec2(99999f, 99999f);
            _bottomRightMost = new Vec2(-99999f, -99999f);
            foreach (Thing levelThing in _levelThings)
                AdjustSizeLimits(levelThing);
        }

        public static int CalculatePlacementCost(Thing pObject) => pObject.placementCost;

        public void ClearEverything()
        {
            foreach (Thing t in _levelThingsNormal)
                current.RemoveThing(t);
            _levelThingsNormal.Clear();
            foreach (Thing t in _levelThingsAlternate)
                current.RemoveThing(t);
            _levelThingsAlternate.Clear();
            editingOpenAirVariation = _editingOpenAirVariationPrev = false;
            _lastCommand = -1;
            _commands.Clear();
            if (!_looseClear)
            {
                _procContext = null;
                _procTarget = null;
            }

            _pathNorth = false;
            _pathSouth = false;
            _pathWest = false;
            _pathEast = false;
            _miniMode = false;
            things.quadTree.Clear();
            generatorComplexity = 0;
            Custom.ClearCustomData();
            _currentLevelData = new LevelData();
            _currentLevelData.metaData.guid = Guid.NewGuid().ToString();
            previewCapture = null;
            hasUnsavedChanges = false;
            placementTotalCost = 0;
            RecalculateSizeLimits();
            History.Clear();
            foreach (Nubber nubber in Level.current.things[typeof(Nubber)])
            {
                current.RemoveThing(nubber);
            }
        }

        public float cellSize
        {
            get => _cellSize;
            set => _cellSize = value;
        }

        //private float width => (float)this._gridW * this._cellSize;

        //private float height => (float)this._gridH * this._cellSize;

        public Thing placementType
        {
            set
            {
                _placementType = value;
                _eyeDropperSerialized = null;
            }
            get => _placementType;
        }

        private LevelType GetLevelType()
        {
            if (arcadeMachineMode)
                return LevelType.Arcade_Machine;
            LevelType levelType = LevelType.Deathmatch;
            if (_levelThings.FirstOrDefault(x => x is ChallengeMode) != null)
                levelType = LevelType.Challenge;
            else if (_levelThings.FirstOrDefault(x => x is ArcadeMode) != null)
                levelType = LevelType.Arcade;
            return levelType;
        }

        private LevelSize GetLevelSize()
        {
            _topLeft = new Vec2(99999f, 99999f);
            _bottomRight = new Vec2(-99999f, -99999f);
            CalculateBounds();
            double length = (topLeft - bottomRight).length;
            LevelSize levelSize = LevelSize.Ginormous;
            if (length < 900.0)
                levelSize = LevelSize.Large;
            if (length < 650.0)
                levelSize = LevelSize.Medium;
            if (length < 400.0)
                levelSize = LevelSize.Small;
            if (length < 200.0)
                levelSize = LevelSize.Tiny;
            return levelSize;
        }

        private bool LevelIsOnlineCapable()
        {
            foreach (object levelThing in _levelThings)
            {
                if (!ContentProperties.GetBag(levelThing.GetType()).GetOrDefault("isOnlineCapable", true))
                    return false;
            }

            return true;
        }

        public void SteamUpload()
        {
            if (arcadeMachineMode)
            {
                (_levelThings[0] as ArcadeMachine).UpdateData();
                if ((_levelThings[0] as ArcadeMachine).challenge01Data == null ||
                    (_levelThings[0] as ArcadeMachine).challenge02Data == null ||
                    (_levelThings[0] as ArcadeMachine).challenge02Data == null)
                {
                    DoMenuClose();
                    _closeMenu = false;
                    _notify.Open("You must select 3 valid Challenges!");
                    return;
                }
            }

            if (_saveName == "")
            {
                DoMenuClose();
                _closeMenu = false;
                _notify.Open("Please save the level first...");
            }
            else
            {
                Save();
                _uploadDialog.Open(_currentLevelData);
                DoMenuClose();
                _closeMenu = false;
                Content.customPreviewWidth = 0;
                Content.customPreviewHeight = 0;
                Content.customPreviewCenter = Vec2.Zero;
            }
        }

        public MonoFileDialog fileDialog => _fileDialog;

        public static string initialDirectory => _initialDirectory;

        public static Layer objectMenuLayer => Main.editor._objectMenuLayer;

        public void EnterEditor()
        {
            focusWait = 10;
            Layer.ClearLayers();
            _gridLayer = new Layer("GRID", Layer.Background.depth + 5, Layer.Background.camera)
            {
                allowTallAspect = true
            };
            Layer.Add(_gridLayer);
            _procLayer = new Layer("PROC", Layer.Background.depth + 25,
                new Camera(0f, 0f, Graphics.width, Graphics.height))
            {
                allowTallAspect = true
            };
            Layer.Add(_procLayer);
            Music.Stop();
            if (!isTesting)
            {
                _placementType = null;
                CenterView();
                _tilePosition = new Vec2(0f, 0f);
            }

            _ultimateBounds = current.things.quadTree.rectangle;
            Layer.HUD.camera.InitializeToScreenAspect();
            Layer.HUD.camera.width *= 2f;
            Layer.HUD.camera.height *= 2f;
            Layer.HUD.allowTallAspect = true;
            if (Resolution.current.aspect > 2.0)
            {
                Layer.HUD.camera.width *= 2f;
                Layer.HUD.camera.height *= 2f;
            }

            if (_objectMenuLayer == null)
            {
                _objectMenuLayer = new Layer("OBJECTMENU", Layer.HUD.depth - 25,
                    new Camera(0f, 0f, Layer.HUD.camera.width, Layer.HUD.camera.height))
                {
                    allowTallAspect = true
                };
            }

            Layer.Add(_objectMenuLayer);
            backgroundColor = new Color(20, 20, 20);
            focusStack.Clear();
            active = true;
            isTesting = false;
            inputMode = EditorInput.Gamepad;
            shoulddo = true;
        }
        public bool shoulddo;
        public void Quit() => _quitting = true;

        public override void DoInitialize()
        {
            SFX.StopAllSounds();
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }
            else
            {
                EnterEditor();
                base.DoInitialize();
            }
        }

        public override void Terminate()
        {
        }

        public static bool miniMode
        {
            get => current is Editor && (current as Editor)._miniMode;
            set
            {
                if (!(current is Editor))
                    return;
                (current as Editor)._miniMode = value;
            }
        }

        public float _chance
        {
            get => _currentLevelData.proceduralData.chance;
            set => _currentLevelData.proceduralData.chance = value;
        }

        public int _maxPerLevel
        {
            get => _currentLevelData.proceduralData.maxPerLevel;
            set => _currentLevelData.proceduralData.maxPerLevel = value;
        }

        public bool _enableSingle
        {
            get => _currentLevelData.proceduralData.enableSingle;
            set => _currentLevelData.proceduralData.enableSingle = value;
        }

        public bool _enableMulti
        {
            get => _currentLevelData.proceduralData.enableMulti;
            set => _currentLevelData.proceduralData.enableMulti = value;
        }

        public bool _canMirror
        {
            get => _currentLevelData.proceduralData.canMirror;
            set => _currentLevelData.proceduralData.canMirror = value;
        }

        public bool _isMirrored
        {
            get => _currentLevelData.proceduralData.isMirrored;
            set => _currentLevelData.proceduralData.isMirrored = value;
        }

        public void UpdateObjectMenu()
        {
            if (_objectMenu != null)
                Remove(_objectMenu);
            _objectMenu = new PlacementMenu(0f, 0f);
            Add(_objectMenu);
            _objectMenu.visible = _objectMenu.active = false;
        }

        public string additionalSaveDirectory => _additionalSaveDirectory;

        public override void Initialize()
        {
            while (!_listLoaded)
                Thread.Sleep(16);
            _editorCam = new EditorCam();
            camera = _editorCam;
            camera.InitializeToScreenAspect();
            _selectionMaterial = new MaterialSelection();
            _selectionMaterialPaste = new MaterialSelection
            {
                fade = 0.5f
            };
            _cursor = new SpriteMap("cursors", 16, 16);
            _tileset = new SpriteMap("industrialTileset", 16, 16);
            _sideArrow = new Sprite("Editor/sideArrow");
            _sideArrow.CenterOrigin();
            _sideArrowHover = new Sprite("Editor/sideArrowHover");
            _sideArrowHover.CenterOrigin();
            _cantPlace = new Sprite("cantPlace");
            _cantPlace.CenterOrigin();
            _editorCurrency = new Sprite("editorCurrency");
            _die = new Sprite("die");
            _dieHover = new Sprite("dieHover");
            _singleBlock = new Sprite("Editor/singleplayerBlock");
            _multiBlock = new Sprite("Editor/multiplayerBlock");
            Layer.Background.camera.InitializeToScreenAspect();
            Layer.Game.camera.InitializeToScreenAspect();
            Layer.Game.camera.width *= 2f;
            Layer.Game.camera.height *= 2f;
            CalculateGridRestriction();
            EnterEditor();
            _camSize = new Vec2(camera.width, camera.height);
            _font = new BitmapFont("biosFont", 8);
            _input = InputProfile.Get(InputProfile.MPPlayer1);
            _tilePosition = new Vec2(0f, 0f);
            _tilePositionPrev = _tilePosition;
            _objectMenu = new PlacementMenu(0f, 0f);
            Add(_objectMenu);
            _objectMenu.visible = _objectMenu.active = false;
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_chance", increment: 0.05f),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/dieBlock", 16, 16),
                "CHANCE - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridBottomLeft));
            Add(new TileButton(0f, 16f, new FieldBinding(this, "_maxPerLevel", -1f, 8f, 1f),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/numBlock", 16, 16),
                "MAX IN LEVEL - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridBottomLeft));
            Add(new TileButton(-16f, 0f, new FieldBinding(this, "_enableSingle"),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/singleplayerBlock", 16, 16),
                "AVAILABLE IN SINGLE PLAYER - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_enableMulti"),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/multiplayerBlock", 16, 16),
                "AVAILABLE IN MULTI PLAYER - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Add(new TileButton(-16f, 16f, new FieldBinding(this, "_canMirror"),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/canMirror", 16, 16),
                "TILE CAN BE MIRRORED - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Add(new TileButton(0f, 16f, new FieldBinding(this, "_isMirrored"),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/isMirrored", 16, 16),
                "PRE MIRRORED TILE - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Add(new TileButton(0f, 32f, new FieldBinding(this, "editingOpenAirVariation"),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/openAir", 16, 16),
                "OPEN AIR VARIATION - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathEast"), new FieldBinding(this, "_miniMode"),
                new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS EAST - @SELECT@TOGGLE",
                TileButtonAlign.TileGridRight, 90f));
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathWest"), new FieldBinding(this, "_miniMode"),
                new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS WEST - @SELECT@TOGGLE",
                TileButtonAlign.TileGridLeft, -90f));
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathNorth"), new FieldBinding(this, "_miniMode"),
                new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS NORTH - @SELECT@TOGGLE",
                TileButtonAlign.TileGridTop));
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathSouth"), new FieldBinding(this, "_miniMode"),
                new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS SOUTH - @SELECT@TOGGLE",
                TileButtonAlign.TileGridBottom, 180f));
            Add(new TileButton(16f, 0f, new FieldBinding(this, "_genTilePos", max: 6f, increment: 1f),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/moveBlock", 16, 16),
                "MOVE GEN - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopLeft));
            Add(new TileButton(32f, 0f, new FieldBinding(this, "_editTilePos", max: 6f, increment: 1f),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/editBlock", 16, 16),
                "MOVE GEN - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopLeft));
            Add(new TileButton(48f, 0f, new FieldBinding(this, "generatorComplexity", max: 9f, increment: 1f),
                new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/dieBlockRed", 16, 16),
                "NUM TILES - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopLeft));
            Add(new TileButton(0f, 0f, new FieldBinding(this, "_doGen"), new FieldBinding(this, "_miniMode"),
                new SpriteMap("Editor/regenBlock", 16, 16), "REGENERATE - HOLD @SELECT@ AND MOVE @DPAD@",
                TileButtonAlign.TileGridTopRight));
            _notify = new NotifyDialogue();
            Add(_notify);
            Vec2 vec2_1 = new Vec2(12f, 12f);
            _touchButtons.Add(new EditorTouchButton
            {
                caption = "MENU",
                explanation = "Pick an object for placement...",
                state = EditorTouchState.OpenMenu,
                threeFingerGesture = true
            });
            _touchButtons.Add(new EditorTouchButton
            {
                caption = "COPY",
                explanation = "Pick an object to copy...",
                state = EditorTouchState.Eyedropper
            });
            _touchButtons.Add(new EditorTouchButton
            {
                caption = "EDIT",
                explanation = "Press objects to edit them!",
                state = EditorTouchState.EditObject
            });
            _cancelButton = new EditorTouchButton
            {
                caption = "CANCEL",
                explanation = "",
                state = EditorTouchState.Normal
            };
            _editTilesButton = new EditorTouchButton
            {
                caption = "PICK TILE",
                explanation = "",
                state = EditorTouchState.PickTile
            };
            _editTilesButton.size =
                new Vec2(Graphics.GetStringWidth(_editTilesButton.caption) + 6f, 15f) + vec2_1;
            _editTilesButton.position = Layer.HUD.camera.OffsetTL(10f, 10f);
            Vec2 vec2_2 = Layer.HUD.camera.OffsetBR(-14f, -14f);
            for (int index = _touchButtons.Count - 1; index >= 0; --index)
            {
                EditorTouchButton touchButton = _touchButtons[index];
                if (index == _touchButtons.Count - 1)
                {
                    _cancelButton.size = new Vec2(Graphics.GetStringWidth(_cancelButton.caption) + 6f, 15f) +
                                         vec2_1;
                    _cancelButton.position = vec2_2 - _cancelButton.size;
                }

                touchButton.size = new Vec2(Graphics.GetStringWidth(touchButton.caption) + 6f, 15f) + vec2_1;
                touchButton.position = vec2_2 - touchButton.size;
                vec2_2.x -= touchButton.size.x + 4f;
            }

            _initialDirectory = DuckFile.levelDirectory;
            _initialDirectory = Path.GetFullPath(_initialDirectory);
            _fileDialog = new MonoFileDialog();
            Add(_fileDialog);
            _uploadDialog = new SteamUploadDialog();
            Add(_uploadDialog);
            _editorButtons = new SpriteMap("editorButtons", 32, 32);
            _doingResave = true;
            _doingResave = false;
            ClearEverything();
            string filepath = DuckFile.levelDirectory + Program.StartinEditorLevelName;
            if (Path.GetExtension(filepath) != ".lev")
            {
                filepath += ".lev";
            }
            if (MonoMain.startInEditor && Program.StartinEditorLevelName != "")
            {
                if (File.Exists(filepath))
                {
                    DevConsole.Log("Loading Level " + filepath, Color.Green);
                    Main.editor.LoadLevel(filepath);
                    Main.editor.DoUpdate();
                    Main.editor.DoUpdate();
                    Main.editor.DoUpdate();
                    Main.editor.Play();
                }
                else
                {
                    DevConsole.Log("Level Not Found " + filepath, Color.Red);
                }
            }
        }

        public Vec2 GetAlignOffset(TileButtonAlign align)
        {
            switch (align)
            {
                case TileButtonAlign.ProcGridTopLeft:
                    int num1 = 192;
                    int num2 = 144;
                    return new Vec2
                    {
                        x = -(_procTilesWide - (_procTilesWide - _procXPos)) * num1,
                        y = -(_procTilesHigh - (_procTilesHigh - _procYPos)) * num2 - 16
                    };
                case TileButtonAlign.TileGridTopLeft:
                    return new Vec2 { x = 0f, y = -16f };
                case TileButtonAlign.TileGridTopRight:
                    int num3 = 192;
                    return new Vec2
                    {
                        x = num3 - 16,
                        y = -16f
                    };
                case TileButtonAlign.TileGridBottomLeft:
                    int num4 = 144;
                    return new Vec2 { x = 0f, y = num4 };
                case TileButtonAlign.TileGridBottomRight:
                    int num5 = 144;
                    int num6 = 192;
                    return new Vec2
                    {
                        x = num6 - 16,
                        y = num5
                    };
                case TileButtonAlign.TileGridRight:
                    return new Vec2(192f, 144 / 2 - 8);
                case TileButtonAlign.TileGridTop:
                    return new Vec2(192 / 2 - 8, -16f);
                case TileButtonAlign.TileGridLeft:
                    return new Vec2(-16f, 144 / 2 - 8);
                case TileButtonAlign.TileGridBottom:
                    return new Vec2(192 / 2 - 8, 144f);
                default:
                    return Vec2.Zero;
            }
        }

        private void Resave(string root)
        {
            foreach (string load in DuckFile.GetFilesNoCloud(root, "*.lev"))
            {
                try
                {
                    LoadLevel(load);
                    _things.RefreshState();
                    _updateEvenWhenInactive = true;
                    Update();
                    _updateEvenWhenInactive = false;
                    if (existingGUID.Contains(_currentLevelData.metaData.guid))
                        _currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                    existingGUID.Add(_currentLevelData.metaData.guid);
                    Save();
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                }
            }

            foreach (string root1 in DuckFile.GetDirectoriesNoCloud(root))
                Resave(root1);
        }

        public static InputProfile input => _input;

        public void ShowNoSpawnsDialogue()
        {
            if (_noSpawnsDialogue == null)
            {
                _noSpawnsDialogue = new MessageDialogue(null);
                Add(_noSpawnsDialogue);
            }

            _noSpawnsDialogue.Open("NO SPAWNS",
                pDescription:
                "Your level has no spawns.\n\n\n@_!DUCKSPAWN@\n\n\nPlease place a |DGBLUE|Spawns/Spawn Point|PREV|\n in your level.");
            lockInput = _noSpawnsDialogue;
            _noSpawnsDialogue.okayOnly = true;
            _noSpawnsDialogue.windowYOffsetAdd = -30f;
        }

        public void CompleteDialogue(ContextMenu pItem)
        {
        }

        public void OpenMenu(ContextMenu menu)
        {
            menu.active = menu.visible = true;
            if (inputMode == EditorInput.Mouse)
            {
                menu.x = Mouse.x;
                menu.y = Mouse.y;
            }

            if (openPosition != Vec2.Zero)
            {
                menu.position = openPosition + new Vec2(-2f, -3f);
                openPosition = Vec2.Zero;
            }

            if (_showPlacementMenu)
            {
                menu.x = 96f;
                menu.y = 32f;
                _showPlacementMenu = false;
            }

            if (inputMode == EditorInput.Gamepad || inputMode == EditorInput.Touch)
            {
                menu.x = 16f;
                menu.y = 16f;
            }

            menu.opened = true;
            _placementMenu = menu;
            disableDragMode();
        }

        private Layer GetLayerOrOverride(Thing thingToCheck)
        {
            Layer layerOrOverride = thingToCheck != null ? thingToCheck.placementLayer : Layer.Game;
            if (thingToCheck != null && thingToCheck.placementLayerOverride != null)
                layerOrOverride = thingToCheck.placementLayerOverride;
            else if (thingToCheck is AutoBlock)
                layerOrOverride = Layer.Blocks;
            if (layerOrOverride == null)
                layerOrOverride = Layer.Game;
            return layerOrOverride;
        }

        private void EndCurrentTouchMode()
        {
            if (!_showPlacementMenu)
                _closeMenu = true;
            _touchState = EditorTouchState.Normal;
            _activeTouchButton = null;
            clickedMenu = true;
            _editMode = false;
            _copyMode = false;
            _hover = null;
        }

        private void disableDragMode()
        {
            _dragMode = false;
            _deleteMode = false;
            if (_move != null)
                _move = null;
            dragModeInputType = InputType.eNone;
            History.EndUndoSection();
        }

        private void HugObjectPlacement()
        {
            if (_placementType is ItemSpawner)
                (_placementType as ItemSpawner)._seated = false;
            if ((_placementType.hugWalls & WallHug.Right) != WallHug.None &&
                CollisionLine<IPlatform>(_tilePosition, _tilePosition + new Vec2(16f, 0f), _placementType) is Thing
                    thing1 && thing1.GetType() != _placementType.GetType())
                _tilePosition.x = thing1.left - _placementType.collisionSize.x - _placementType.collisionOffset.x;
            if ((_placementType.hugWalls & WallHug.Left) != WallHug.None &&
                CollisionLine<IPlatform>(_tilePosition, _tilePosition + new Vec2(-16f, 0f), _placementType) is Thing
                    thing2 && thing2.GetType() != _placementType.GetType())
                _tilePosition.x = thing2.right - _placementType.collisionOffset.x;
            if ((_placementType.hugWalls & WallHug.Ceiling) != WallHug.None &&
                CollisionLine<IPlatform>(_tilePosition, _tilePosition + new Vec2(0f, -16f), _placementType) is Thing
                    thing3 && thing3.GetType() != _placementType.GetType())
                _tilePosition.y = thing3.bottom - _placementType.collisionOffset.y;
            if ((_placementType.hugWalls & WallHug.Floor) == WallHug.None ||
                !(CollisionLine<IPlatform>(_tilePosition, _tilePosition + new Vec2(0f, 16f), _placementType) is Thing
                    thing4) || !(thing4.GetType() != _placementType.GetType()))
                return;
            _tilePosition.y = thing4.top - _placementType.collisionSize.y - _placementType.collisionOffset.y;
            if (!(_placementType is ItemSpawner))
                return;
            (_placementType as ItemSpawner)._seated = true;
        }

        public static float interfaceSizeMultiplier => inputMode != EditorInput.Touch ? 1f : 2f;

        public override void Update()
        {
            if (shoulddo)
            {
                if (DGRSettings.PreferredLevel != "" && _currentLevelData.objects.objects.Count == 0)
                {
                    string filepath = DGRSettings.PreferredLevel;
                    if (File.Exists(filepath))
                    {
                        DevConsole.Log("Loading Level " + filepath, Color.Green);
                        Main.editor.LoadLevel(filepath);
                    }
                    else
                    {
                        DevConsole.Log("Level Not Found " + filepath, Color.Red);
                    }
                }
                shoulddo = false;
            }
            if (!Graphics.inFocus)
                focusWait = 5;
            else if (focusWait > 0)
            {
                --focusWait;
            }
            else
            {
                ++MonoMain.timeInEditor;
                tooltip = null;
                foreach (Thing thing in things)
                    thing.DoEditorUpdate();
                if (lastMousePos == Vec2.Zero)
                    lastMousePos = Mouse.position;
                if (clickedContextBackground)
                {
                    clickedContextBackground = false;
                    clickedMenu = true;
                }

                int inputMode1 = (int)inputMode;
                if (Mouse.left == InputState.Pressed || Mouse.right == InputState.Pressed ||
                    Mouse.middle == InputState.Pressed ||
                    !fakeTouch && (lastMousePos - Mouse.position).length > 3.0)
                    inputMode = fakeTouch ? EditorInput.Touch : EditorInput.Mouse;
                else if (inputMode != EditorInput.Gamepad && InputProfile.active.Pressed("ANY", true))
                {
                    if ((_selection.Count == 0 || !Keyboard.Pressed(Keys.F, true)) &&
                        !InputProfile.active.Pressed("RSTICK") && !InputProfile.active.Pressed("CANCEL") &&
                        !InputProfile.active.Pressed("MENU1") && !Keyboard.Down(Keys.LeftShift) &&
                        !Keyboard.Down(Keys.RightShift) && !Keyboard.Down(Keys.LeftControl) &&
                        !Keyboard.Down(Keys.RightControl))
                    {
                        if (inputMode == EditorInput.Mouse)
                        {
                            _tilePosition = Maths.Snap(Mouse.positionScreen + new Vec2(8f, 8f), 16f, 16f);
                            _tilePositionPrev = _tilePosition;
                        }

                        inputMode = EditorInput.Gamepad;
                    }
                }
                else if (TouchScreen.IsScreenTouched())
                    inputMode = EditorInput.Touch;

                if (inputMode == EditorInput.Mouse)
                    _input.lastActiveDevice = Input.GetDevice<Keyboard>();
                int inputMode2 = (int)inputMode;
                if (inputMode1 != inputMode2 && inputMode == EditorInput.Touch)
                {
                    clickedMenu = true;
                    waitForNoTouchInput = true;
                }
                else if (waitForNoTouchInput && TouchScreen.GetTouches().Count > 0)
                {
                    clickedMenu = true;
                }
                else
                {
                    waitForNoTouchInput = false;
                    lastMousePos = Mouse.position;
                    if (_editingOpenAirVariationPrev != editingOpenAirVariation)
                    {
                        if (editingOpenAirVariation)
                        {
                            foreach (Thing t in _levelThingsNormal)
                                current.RemoveThing(t);
                            foreach (Thing t in _levelThingsAlternate)
                                current.AddThing(t);
                        }
                        else
                        {
                            foreach (Thing t in _levelThingsAlternate)
                                current.RemoveThing(t);
                            foreach (Thing t in _levelThingsNormal)
                                current.AddThing(t);
                        }

                        _editingOpenAirVariationPrev = editingOpenAirVariation;
                    }

                    if (inputMode == EditorInput.Touch)
                    {
                        if (_placementMenu == null)
                        {
                            _objectMenuLayer.camera.width = Layer.HUD.width * 0.75f;
                            _objectMenuLayer.camera.height = Layer.HUD.height * 0.75f;
                            bigInterfaceMode = true;
                        }

                        if (_fileDialog.opened && _touchState != EditorTouchState.OpenLevel)
                        {
                            EndCurrentTouchMode();
                            _touchState = EditorTouchState.OpenLevel;
                        }
                        else if (!_fileDialog.opened && _touchState == EditorTouchState.OpenLevel)
                            EndCurrentTouchMode();

                        Touch tap = TouchScreen.GetTap();
                        if (_touchState == EditorTouchState.Normal)
                        {
                            _activeTouchButton = null;
                            foreach (EditorTouchButton touchButton in _touchButtons)
                            {
                                if (tap != Touch.None && tap.positionHUD.x > touchButton.position.x &&
                                    tap.positionHUD.x < touchButton.position.x + touchButton.size.x &&
                                    tap.positionHUD.y > touchButton.position.y &&
                                    tap.positionHUD.y < touchButton.position.y + touchButton.size.y ||
                                    touchButton.threeFingerGesture && _threeFingerGesture)
                                {
                                    _touchState = touchButton.state;
                                    _activeTouchButton = touchButton;
                                    clickedMenu = true;
                                    _threeFingerGesture = false;
                                    _clickedTouchButton = true;
                                    SFX.Play("highClick", 0.3f, 0.2f);
                                }
                            }
                        }
                        else if (tap.positionHUD.x > _cancelButton.position.x &&
                                 tap.positionHUD.x < _cancelButton.position.x + _cancelButton.size.x &&
                                 tap.positionHUD.y > _cancelButton.position.y &&
                                 tap.positionHUD.y < _cancelButton.position.y + _cancelButton.size.y ||
                                 _activeTouchButton != null && _activeTouchButton.threeFingerGesture &&
                                 _threeFingerGesture ||
                                 _activeTouchButton != null && !_activeTouchButton.threeFingerGesture &&
                                 _threeFingerGesture || _activeTouchButton != null &&
                                 _activeTouchButton.threeFingerGesture && _twoFingerGesture)
                        {
                            EndCurrentTouchMode();
                            if (_fileDialog.opened)
                                _fileDialog.Close();
                            SFX.Play("highClick", 0.3f, 0.2f);
                            return;
                        }

                        if (_placingTiles && _placementMenu == null &&
                            tap.positionHUD.x > _editTilesButton.position.x &&
                            tap.positionHUD.x < _editTilesButton.position.x + _editTilesButton.size.x &&
                            tap.positionHUD.y > _editTilesButton.position.y && tap.positionHUD.y <
                            _editTilesButton.position.y + _editTilesButton.size.y)
                        {
                            _openTileSelector = true;
                            clickedMenu = true;
                        }

                        if (_touchState == EditorTouchState.OpenMenu)
                        {
                            if (_placementMenu == null)
                                _showPlacementMenu = true;
                            EndCurrentTouchMode();
                        }
                        else if (_touchState == EditorTouchState.EditObject)
                            _editMode = true;
                        else if (_touchState == EditorTouchState.Eyedropper)
                            _copyMode = true;
                    }
                    else
                    {
                        _editMode = false;
                        _copyMode = false;
                        _activeTouchButton = null;
                        _touchState = EditorTouchState.Normal;
                        if (_placementMenu == null)
                        {
                            _objectMenuLayer.camera.width = Layer.HUD.width;
                            _objectMenuLayer.camera.height = Layer.HUD.height;
                            bigInterfaceMode = false;
                            pretendPinned = null;
                        }
                    }

                    if (!Graphics.inFocus && !_updateEvenWhenInactive)
                        _tileDragDif = Vec2.MaxValue;
                    else if (clickedMenu)
                    {
                        clickedMenu = false;
                    }
                    else
                    {
                        if (_notify.opened)
                            return;
                        if (reopenContextMenu)
                        {
                            int num = ignorePinning ? 1 : 0;
                            reopenContextMenu = false;
                            if (_placementMenu != null)
                                _placementMenu.opened = false;
                            ignorePinning = num != 0;
                            if (_placementMenu == null)
                                _placementMenu = _objectMenu;
                            OpenMenu(_placementMenu);
                            if (openContextThing != null)
                                _placementMenu.OpenInto(openContextThing);
                            openContextThing = null;
                            SFX.Play("openClick", 0.4f);
                        }

                        hoverTextBox = false;
                        if (numPops > 0)
                        {
                            for (int index = 0; index < numPops && focusStack.Count != 0; ++index)
                                focusStack.Pop();
                            numPops = 0;
                        }

                        if (tookInput)
                            tookInput = false;
                        else if (focusStack.Count > 0 || skipFrame)
                        {
                            skipFrame = false;
                        }
                        else
                        {
                            if (_placementMenu != null)
                                _placementMenu.visible = _lockInput == null;
                            if (lockInput != null)
                            {
                                if (_lockInputChange == lockInput)
                                    return;
                                _lockInput = _lockInputChange;
                            }
                            else
                            {
                                if (_lockInputChange != lockInput)
                                    _lockInput = _lockInputChange;
                                if (Keyboard.Pressed(Keys.OemComma))
                                    searching = true;
                                if (searching)
                                {
                                    Input._imeAllowed = true;
                                    if (!clearedKeyboardStringForSearch)
                                    {
                                        clearedKeyboardStringForSearch = true;
                                        Keyboard.keyString = "";
                                    }

                                    if (searchItems != null && searchItems.Count > 0)
                                    {
                                        if (Keyboard.Pressed(Keys.Down))
                                        {
                                            if (_searchHoverIndex == 0)
                                                _searchHoverIndex = Math.Min(searchItems.Count - 1, 9);
                                            else
                                                --_searchHoverIndex;
                                            if (_searchHoverIndex < 0)
                                                _searchHoverIndex = 0;
                                        }
                                        else if (Keyboard.Pressed(Keys.Up))
                                        {
                                            if (_searchHoverIndex < 0)
                                                _searchHoverIndex = 0;
                                            else
                                                ++_searchHoverIndex;
                                            if (_searchHoverIndex > Math.Min(searchItems.Count - 1, 9))
                                                _searchHoverIndex = 0;
                                        }

                                        _searchHoverIndex = Math.Min(searchItems.Count - 1, _searchHoverIndex);
                                    }
                                    else
                                        _searchHoverIndex = -1;

                                    bool flag = Mouse.left == InputState.Pressed || Keyboard.Pressed(Keys.Enter);
                                    if (((Mouse.right == InputState.Released || Mouse.middle == InputState.Pressed
                                            ? 1
                                            : (Keyboard.Pressed(Keys.Escape) ? 1 : 0)) | (flag ? 1 : 0)) != 0)
                                    {
                                        if (flag && _searchHoverIndex != -1 && _searchHoverIndex < searchItems.Count)
                                        {
                                            _placementType = searchItems[_searchHoverIndex].thing.thing;
                                            _eyeDropperSerialized = null;
                                        }

                                        searching = false;
                                        clearedKeyboardStringForSearch = false;
                                        searchItems = null;
                                        _searchHoverIndex = -1;
                                    }

                                    if (_prevSearchString != Keyboard.keyString)
                                    {
                                        searchItems = _objectMenu.Search(Keyboard.keyString);
                                        _prevSearchString = Keyboard.keyString;
                                    }

                                    if (_placementMenu == null)
                                        return;
                                    CloseMenu();
                                }

                                if (Keyboard.control && Keyboard.Pressed(Keys.S))
                                {
                                    if (Keyboard.shift)
                                        SaveAs();
                                    else
                                        Save();
                                }

                                if (_onlineSettingChanged && _placementMenu != null &&
                                    _placementMenu is EditorGroupMenu)
                                {
                                    (_placementMenu as EditorGroupMenu).UpdateGrayout();
                                    _onlineSettingChanged = false;
                                }

                                Graphics.fade = Lerp.Float(Graphics.fade, _quitting ? 0f : 1f, 0.02f);
                                if (_quitting && Graphics.fade < 0.01f)
                                {
                                    _quitting = false;
                                    active = false;
                                    current = new TitleScreen();
                                }

                                if (Graphics.fade < 0.95f)
                                    return;
                                Layer placementLayer = GetLayerOrOverride(_placementType);
                                switch (inputMode)
                                {
                                    case EditorInput.Mouse:
                                        clicked = Mouse.left == InputState.Pressed;
                                        if (Mouse.middle == InputState.Pressed)
                                        {
                                            middleClickPos = Mouse.position;
                                        }

                                        break;
                                    case EditorInput.Touch:
                                        clicked = TouchScreen.GetTap() != Touch.None;
                                        break;
                                }

                                if (_cursorMode == CursorMode.Normal &&
                                    (Keyboard.Down(Keys.RightShift) || Keyboard.Down(Keys.LeftShift)))
                                {
                                    Vec2 vec2 = new Vec2(0f, 0f);
                                    if (Keyboard.Pressed(Keys.Up))
                                        vec2.y -= 16f;
                                    if (Keyboard.Pressed(Keys.Down))
                                        vec2.y += 16f;
                                    if (Keyboard.Pressed(Keys.Left))
                                        vec2.x -= 16f;
                                    if (Keyboard.Pressed(Keys.Right))
                                        vec2.x += 16f;
                                    if (vec2 != Vec2.Zero)
                                    {
                                        foreach (Thing thing1 in current.things)
                                        {
                                            Thing thing2 = thing1;
                                            thing2.position += vec2;
                                            if (thing1 is IDontMove)
                                            {
                                                current.things.quadTree.Remove(thing1);
                                                current.things.quadTree.Add(thing1);
                                            }
                                        }
                                    }
                                }

                                _menuOpen = false;
                                if (!_editMode)
                                {
                                    foreach (ContextMenu contextMenu in things[typeof(ContextMenu)])
                                    {
                                        if (contextMenu.visible && contextMenu.opened)
                                        {
                                            clicked = false;
                                            _menuOpen = true;
                                        }
                                    }
                                }

                                if (inputMode == EditorInput.Gamepad)
                                    _input = InputProfile.active;
                                if (_prevEditTilePos != _editTilePos)
                                {
                                    if (_editTilePos.x < 0f)
                                        _editTilePos.x = 0f;
                                    if (_editTilePos.x >= _procTilesWide)
                                        _editTilePos.x = _procTilesWide - 1;
                                    if (_editTilePos.y < 0f)
                                        _editTilePos.y = 0f;
                                    if (_editTilePos.y >= _procTilesHigh)
                                        _editTilePos.y = _procTilesHigh - 1;
                                    if (_currentMapNode != null)
                                    {
                                        RandomLevelData data = _currentMapNode
                                            .map[(int)_editTilePos.x, (int)_editTilePos.y].data;
                                        if (_levelThings.Count > 0)
                                            Save();
                                        _looseClear = true;
                                        if (data == null)
                                        {
                                            ClearEverything();
                                            _saveName = "";
                                        }
                                        else
                                            LoadLevel(Directory.GetCurrentDirectory() + "/../../../assets/levels/" + data.file + ".lev");

                                        _procXPos = (int)_editTilePos.x;
                                        _procYPos = (int)_editTilePos.y;
                                        _genTilePos = new Vec2(_procXPos, _procYPos);
                                        _prevEditTilePos = _editTilePos;
                                        int num1 = 144;
                                        int num2 = 192;
                                        _procDrawOffset += new Vec2((_procXPos - _prevProcX) * num2,
                                            (_procYPos - _prevProcY) * num1);
                                        _prevProcX = _procXPos;
                                        _prevProcY = _procYPos;
                                    }
                                }

                                if (_procXPos != _prevProcX)
                                    _doGen = true;
                                else if (_procYPos != _prevProcY)
                                    _doGen = true;
                                _prevEditTilePos = _editTilePos;
                                _prevProcX = _procXPos;
                                _prevProcY = _procYPos;
                                if (_miniMode && (Keyboard.Pressed(Keys.F1) || _doGen) && !_doingResave)
                                {
                                    if (_saveName == "")
                                        _saveName = _initialDirectory + "/pyramid/" + Guid.NewGuid() +
                                                    ".lev";
                                    LevelGenerator.ReInitialize();
                                    LevelGenerator.complexity = generatorComplexity;
                                    if (!Keyboard.Down(Keys.RightShift) && !Keyboard.Down(Keys.LeftShift))
                                        _procSeed = Rando.Int(2147483646);
                                    string str = _saveName.Substring(_saveName.LastIndexOf("assets/levels/") +
                                                                     "assets/levels/".Length);
                                    string realName = str.Substring(0, str.Length - 4);
                                    RandomLevelData tile = LevelGenerator.LoadInTile(SaveTempVersion(), realName);
                                    _loadPosX = _procXPos;
                                    _loadPosY = _procYPos;
                                    LevGenType type = LevGenType.Any;
                                    if (_currentLevelData.proceduralData.enableSingle &&
                                        !_currentLevelData.proceduralData.enableMulti)
                                        type = LevGenType.SinglePlayer;
                                    else if (!_currentLevelData.proceduralData.enableSingle &&
                                             _currentLevelData.proceduralData.enableMulti)
                                        type = LevGenType.Deathmatch;
                                    _editTilePos = _prevEditTilePos = _genTilePos;
                                    int num3 = 0;
                                    Level level;
                                    while (true)
                                    {
                                        _currentMapNode = LevelGenerator.MakeLevel(tile, _pathEast && _pathWest,
                                            _procSeed, type, _procTilesWide, _procTilesHigh, _loadPosX,
                                            _loadPosY);
                                        _procDrawOffset = new Vec2(0f, 0f);
                                        _procContext = new GameContext();
                                        _procContext.ApplyStates();
                                        level = new Level
                                        {
                                            backgroundColor = new Color(0, 0, 0, 0)
                                        };
                                        core.currentLevel = level;
                                        RandomLevelNode.editorLoad = true;
                                        int num4 = _currentMapNode.LoadParts(0f, 0f, level, _procSeed) ? 1 : 0;
                                        RandomLevelNode.editorLoad = false;
                                        if (num4 == 0 && num3 <= 100)
                                            ++num3;
                                        else
                                            break;
                                    }

                                    level.CalculateBounds();
                                    _procContext.RevertStates();
                                    _doGen = false;
                                }

                                _looseClear = false;
                                Vec2 vec2_1;
                                if (inputMode == EditorInput.Touch)
                                {
                                    if (!_twoFingerGestureStarting && TouchScreen.GetTouches().Count == 2)
                                    {
                                        _twoFingerGestureStarting = true;
                                        _panAnchor = TouchScreen.GetAverageOfTouches().positionHUD;
                                        _twoFingerSpacing = (TouchScreen.GetTouches()[0].positionHUD -
                                                             TouchScreen.GetTouches()[1].positionHUD).length;
                                    }
                                    else if (TouchScreen.GetTouches().Count != 2)
                                    {
                                        _twoFingerGesture = false;
                                        _twoFingerGestureStarting = false;
                                    }

                                    if (_twoFingerGestureStarting && TouchScreen.GetTouches().Count == 2 &&
                                        !_twoFingerGesture)
                                    {
                                        vec2_1 = _panAnchor - TouchScreen.GetAverageOfTouches().positionHUD;
                                        if (vec2_1.length > 6.0)
                                        {
                                            _twoFingerZooming = false;
                                            _twoFingerGesture = true;
                                        }
                                        else
                                        {
                                            double twoFingerSpacing = _twoFingerSpacing;
                                            vec2_1 = TouchScreen.GetTouches()[0].positionHUD -
                                                     TouchScreen.GetTouches()[1].positionHUD;
                                            double length = vec2_1.length;
                                            if (Math.Abs((float)(twoFingerSpacing - length)) > 4.0)
                                            {
                                                _twoFingerZooming = true;
                                                _twoFingerGesture = true;
                                            }
                                        }
                                    }

                                    if (!_threeFingerGestureRelease && TouchScreen.GetTouches().Count == 3)
                                    {
                                        _threeFingerGesture = true;
                                        _threeFingerGestureRelease = true;
                                    }
                                    else if (TouchScreen.GetTouches().Count != 3)
                                    {
                                        _threeFingerGesture = false;
                                        _threeFingerGestureRelease = false;
                                    }
                                }

                                if (inputMode == EditorInput.Mouse && Mouse.middle == InputState.Pressed)
                                    _panAnchor = Mouse.position;
                                if (_procContext != null)
                                    _procContext.Update();
                                if (tabletMode && clicked)
                                {
                                    if (Mouse.x < 32.0 && Mouse.y < 32.0)
                                    {
                                        _placementMode = true;
                                        _editMode = false;
                                        clicked = false;
                                        return;
                                    }

                                    if (Mouse.x < 64.0 && Mouse.y < 32.0)
                                    {
                                        _placementMode = false;
                                        _editMode = true;
                                        clicked = false;
                                        return;
                                    }

                                    if (Mouse.x < 96.0 && Mouse.y < 32.0)
                                    {
                                        if (_placementMenu == null)
                                            _showPlacementMenu = true;
                                        else
                                            CloseMenu();
                                        clicked = false;
                                        return;
                                    }
                                }

                                if (_editorLoadFinished)
                                {
                                    foreach (Thing levelThing in _levelThings)
                                        levelThing.OnEditorLoaded();
                                    foreach (PathNode pathNode in things[typeof(PathNode)])
                                    {
                                        pathNode.UninitializeLinks();
                                        pathNode.Update();
                                    }

                                    _editorLoadFinished = false;
                                }

                                things.RefreshState();
                                if (_placeObjects.Count > 0)
                                {
                                    foreach (Thing placeObject in _placeObjects)
                                    {
                                        foreach (Thing thing in CheckRectAll<IDontMove>(
                                                     placeObject.topLeft + new Vec2(-16f, -16f),
                                                     placeObject.bottomRight + new Vec2(16f, 16f)))
                                            thing.EditorObjectsChanged();
                                    }

                                    things.CleanAddList();
                                    _placeObjects.Clear();
                                }

                                if (_placementMenu != null && inputMode == EditorInput.Mouse &&
                                    Mouse.right == InputState.Released)
                                {
                                    _placementMenu.Disappear();
                                    CloseMenu();
                                }

                                if (Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl))
                                {
                                    bool flag = false;
                                    if (Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift))
                                        flag = true;
                                    if (Keyboard.Pressed(Keys.Z))
                                    {
                                        if (flag)
                                            History.Redo();
                                        else
                                            History.Undo();
                                        _selection.Clear();
                                        _currentDragSelectionHover.Clear();
                                        foreach (Thing levelThing in _levelThings)
                                            levelThing.EditorObjectsChanged();
                                    }
                                }

                                if (inputMode == EditorInput.Gamepad && _placementMenu == null)
                                {
                                    if (_input.Pressed("STRAFE"))
                                    {
                                        History.Undo();
                                        _selection.Clear();
                                        _currentDragSelectionHover.Clear();
                                        foreach (Thing levelThing in _levelThings)
                                            levelThing.EditorObjectsChanged();
                                    }

                                    if (_input.Pressed("RAGDOLL"))
                                    {
                                        History.Redo();
                                        _selection.Clear();
                                        _currentDragSelectionHover.Clear();
                                        foreach (Thing levelThing in _levelThings)
                                            levelThing.EditorObjectsChanged();
                                    }
                                }

                                if ((_input.Pressed("MENU2") || _showPlacementMenu) &&
                                    _cursorMode == CursorMode.Normal)
                                {
                                    if (_placementMenu == null)
                                    {
                                        _placementMenu = _objectMenu;
                                        OpenMenu(_placementMenu);
                                        SFX.Play("openClick", 0.4f);
                                    }
                                    else
                                        CloseMenu();
                                }

                                if (_clickedTouchButton)
                                {
                                    _clickedTouchButton = false;
                                }
                                else
                                {
                                    if (_placementType is AutoBlock || _placementType is PipeTileset)
                                        cellSize = 16f;
                                    if (_cursorMode != CursorMode.Selection && _placementMenu == null)
                                    {
                                        switch (inputMode)
                                        {
                                            case EditorInput.Gamepad:
                                                if (_input.Pressed("CANCEL"))
                                                    _selectionDragStart = _tilePosition;
                                                if (_selectionDragStart != Vec2.Zero)
                                                {
                                                    vec2_1 = _selectionDragStart - _tilePosition;
                                                    if (vec2_1.length > 4.0)
                                                    {
                                                        _dragSelectShiftModifier = _selection.Count != 0;
                                                        _cursorMode = CursorMode.Selection;
                                                        _selectionDragEnd = _tilePosition;
                                                        return;
                                                    }
                                                }

                                                if (_input.Released("CANCEL"))
                                                {
                                                    _selectionDragStart = Vec2.Zero;
                                                }

                                                break;
                                            case EditorInput.Mouse:
                                                bool flag1 = Mouse.left == InputState.Pressed &&
                                                             _dragSelectShiftModifier;
                                                if (_placementMenu == null && Mouse.right == InputState.Pressed | flag1)
                                                    _selectionDragStart = Mouse.positionScreen;
                                                if (_dragSelectShiftModifier && (Mouse.right == InputState.Released ||
                                                        Mouse.left == InputState.Released))
                                                {
                                                    if (_hover != null)
                                                    {
                                                        _selection.Add(_hover);
                                                        _currentDragSelectionHover.Add(_hover);
                                                    }

                                                    if (_secondaryHover != null)
                                                    {
                                                        _selection.Add(_secondaryHover);
                                                        _currentDragSelectionHover.Add(_secondaryHover);
                                                    }

                                                    UpdateSelection(false);
                                                    _selectionDragStart = Vec2.Zero;
                                                    if (_selection.Count > 0)
                                                    {
                                                        _cursorMode = CursorMode.HasSelection;
                                                        return;
                                                    }
                                                }

                                                if (_selectionDragStart != Vec2.Zero)
                                                {
                                                    vec2_1 = _selectionDragStart - Mouse.positionScreen;
                                                    if (vec2_1.length > 8.0)
                                                    {
                                                        if (!_dragSelectShiftModifier)
                                                        {
                                                            _selection.Clear();
                                                            _currentDragSelectionHover.Clear();
                                                        }

                                                        _cursorMode = CursorMode.Selection;
                                                        _selectionDragEnd = Mouse.positionScreen;
                                                        return;
                                                    }
                                                }

                                                if (Mouse.right == InputState.Released ||
                                                    Mouse.left == InputState.Released)
                                                {
                                                    _selectionDragStart = Vec2.Zero;
                                                }

                                                break;
                                        }
                                    }

                                    if ((_placementMenu == null || _editMode) && _hoverMode == 0)
                                    {
                                        UpdateHover(placementLayer, _tilePosition);
                                        bool flag2 = false;
                                        if (inputMode == EditorInput.Mouse &&
                                            Mouse.middle == InputState.Released)
                                        {
                                            vec2_1 = middleClickPos - Mouse.position;
                                            if (vec2_1.length < 2.0)
                                                flag2 = true;
                                        }

                                        Thing thing = null;
                                        if (_secondaryHover != null)
                                        {
                                            if (Input.Released("CANCEL") | flag2)
                                            {
                                                copying = true;
                                                _eyeDropperSerialized = _secondaryHover.Serialize();
                                                copying = false;
                                                _placementType = Thing.LoadThing(_eyeDropperSerialized);
                                            }
                                            else if (Input.Pressed("START"))
                                                thing = _secondaryHover;
                                        }
                                        else if (_hover != null)
                                        {
                                            if (_copyMode || Input.Released("CANCEL") | flag2)
                                            {
                                                copying = true;
                                                _eyeDropperSerialized = _hover.Serialize();
                                                copying = false;
                                                _placementType = Thing.LoadThing(_eyeDropperSerialized);
                                                if (inputMode == EditorInput.Touch)
                                                {
                                                    EndCurrentTouchMode();
                                                    return;
                                                }
                                            }
                                            else if (Input.Pressed("START"))
                                                thing = _hover;
                                        }
                                        else if (_placementType != null && Input.Pressed("START"))
                                            thing = _placementType;

                                        if (thing != null)
                                        {
                                            ignorePinning = true;
                                            reopenContextMenu = true;
                                            openContextThing = thing;
                                        }

                                        TileButton tileButton = CollisionPoint<TileButton>(_tilePosition);
                                        if (tileButton != null)
                                        {
                                            if (!tileButton.visible)
                                            {
                                                tileButton = null;
                                            }
                                            else
                                            {
                                                tileButton.hover = true;
                                                if (inputMode == EditorInput.Mouse)
                                                    hoverMiniButton = true;
                                                tileButton.focus =
                                                    inputMode == EditorInput.Gamepad &&
                                                    _input.Down("SELECT") ||
                                                    inputMode == EditorInput.Mouse &&
                                                    (Mouse.left == InputState.Down ||
                                                     Mouse.left == InputState.Pressed) ||
                                                    inputMode == EditorInput.Touch &&
                                                    TouchScreen.IsScreenTouched()
                                                        ? _input
                                                        : null;
                                            }
                                        }

                                        if (tileButton != _hoverButton && _hoverButton != null)
                                            _hoverButton.focus = null;
                                        _hoverButton = tileButton;
                                    }

                                    if (inputMode == EditorInput.Mouse)
                                    {
                                        int right = (int)Mouse.right;
                                    }

                                    if (_cursorMode == CursorMode.Normal)
                                    {
                                        if (_hoverMenu != null && !_placingTiles &&
                                            (inputMode == EditorInput.Mouse &&
                                                Mouse.right == InputState.Released || _input.Pressed("MENU1") &&
                                                !_input.Down("SELECT")))
                                        {
                                            if (_placementMenu == null)
                                            {
                                                if (_hover != null)
                                                {
                                                    _placementMenu = _hover.GetContextMenu();
                                                    if (_placementMenu != null)
                                                        AddThing(_placementMenu);
                                                }
                                                else if (_secondaryHover != null)
                                                {
                                                    _placementMenu = _secondaryHover.GetContextMenu();
                                                    if (_placementMenu != null)
                                                        AddThing(_placementMenu);
                                                }

                                                if (_placementMenu != null)
                                                {
                                                    OpenMenu(_placementMenu);
                                                    SFX.Play("openClick", 0.4f);
                                                }
                                            }
                                            else if (inputMode == EditorInput.Mouse &&
                                                     Mouse.right == InputState.Pressed)
                                                CloseMenu();
                                        }

                                        if (hoverMiniButton)
                                        {
                                            _tilePosition.x = (float)Math.Round(Mouse.positionScreen.x / _cellSize) *
                                                              _cellSize;
                                            _tilePosition.y = (float)Math.Round(Mouse.positionScreen.y / _cellSize) *
                                                              _cellSize;
                                            hoverMiniButton = false;
                                            return;
                                        }

                                        if (_hoverMenu == null && inputMode == EditorInput.Mouse &&
                                            Mouse.right == InputState.Released)
                                        {
                                            if (_hover is BackgroundTile)
                                            {
                                                if (_placingTiles && _placementMenu == null)
                                                {
                                                    int frame = _placementType.frame;
                                                    _placementMenu =
                                                        new ContextBackgroundTile(_placementType, null, false);
                                                    _placementMenu.opened = true;
                                                    SFX.Play("openClick", 0.4f);
                                                    _placementMenu.x = 16f;
                                                    _placementMenu.y = 16f;
                                                    _placementMenu.selectedIndex = frame;
                                                    Add(_placementMenu);
                                                }
                                            }
                                            else if (_placementMenu == null)
                                            {
                                                _placementMenu = _objectMenu;
                                                OpenMenu(_placementMenu);
                                                SFX.Play("openClick", 0.4f);
                                            }
                                            else
                                                CloseMenu();
                                        }
                                    }

                                    if (_cursorMode == CursorMode.Normal)
                                    {
                                        if (inputMode == EditorInput.Gamepad &&
                                            _input.Pressed("CANCEL") && _placementMenu != null)
                                            CloseMenu();
                                        if (_placementType != null && _objectMenu != null)
                                        {
                                            rotateValid = _placementType._canFlip ||
                                                          _placementType.editorCycleType != null;
                                            if (_input.Pressed("RSTICK") || Keyboard.Pressed(Keys.Tab))
                                            {
                                                if (_placementType.editorCycleType != null)
                                                {
                                                    _placementType =
                                                        _objectMenu.GetPlacementType(_placementType.editorCycleType);
                                                    _eyeDropperSerialized = null;
                                                }
                                                else
                                                {
                                                    Thing thing = _eyeDropperSerialized != null
                                                        ? Thing.LoadThing(_eyeDropperSerialized)
                                                        : CreateThing(_placementType.GetType());
                                                    thing.TabRotate();
                                                    _placementType = thing;
                                                    _eyeDropperSerialized = thing.Serialize();
                                                }
                                            }
                                        }
                                    }

                                    float num5 = 0f;
                                    if (inputMode == EditorInput.Mouse)
                                        num5 = Mouse.scroll;
                                    else if (inputMode == EditorInput.Touch && _twoFingerGesture &&
                                             _twoFingerZooming)
                                    {
                                        vec2_1 = TouchScreen.GetTouches()[0].positionHUD -
                                                 TouchScreen.GetTouches()[1].positionHUD;
                                        float length = vec2_1.length;
                                        if (Math.Abs(length - _twoFingerSpacing) > 2.0)
                                            num5 = (float)(-(length - _twoFingerSpacing) * 1.0);
                                        _twoFingerSpacing = length;
                                    }

                                    if (inputMode == EditorInput.Gamepad)
                                    {
                                        num5 = _input.leftTrigger - _input.rightTrigger;
                                        float num6 = (float)(camera.width / MonoMain.screenWidth * 5.0);
                                        if (_input.Down("LSTICK"))
                                            num6 *= 2f;
                                        if (_input.Pressed("LOPTION"))
                                            cellSize = cellSize >= 10.0 ? 8f : 16f;
                                        if (num6 < 5.0)
                                            num6 = 5f;
                                        camera.x += _input.rightStick.x * num6;
                                        camera.y -= _input.rightStick.y * num6;
                                    }

                                    if (num5 != 0.0 && !didUIScroll && !hoverUI)
                                    {
                                        int num7 = Math.Sign(num5);
                                        double num8 = camera.height / camera.width;
                                        float num9 = num7 * 64f;
                                        switch (inputMode)
                                        {
                                            case EditorInput.Gamepad:
                                                num9 = num5 * 32f;
                                                break;
                                            case EditorInput.Touch:
                                                num9 = num5;
                                                break;
                                        }

                                        Vec2 vec2_2 = new Vec2(camera.width, camera.height);
                                        Vec2 vec2_3 = camera.transformScreenVector(Mouse.mousePos);
                                        if (inputMode == EditorInput.Touch && _twoFingerGesture)
                                            vec2_3 = TouchScreen.GetAverageOfTouches().positionCamera;
                                        if (inputMode == EditorInput.Gamepad)
                                            vec2_3 = _tilePosition;
                                        camera.width += num9;
                                        if (camera.width < 64.0)
                                            camera.width = 64f;
                                        camera.height = camera.width / Resolution.current.aspect;
                                        Vec2 position = camera.position;
                                        Vec3 translation;
                                        (Matrix.CreateTranslation(new Vec3(position.x, position.y, 0f)) *
                                         Matrix.CreateTranslation(new Vec3(-vec2_3.x, -vec2_3.y, 0f)) *
                                         Matrix.CreateScale(camera.width / vec2_2.x, camera.height / vec2_2.y, 1f) *
                                         Matrix.CreateTranslation(new Vec3(vec2_3.x, vec2_3.y, 0f)))
                                            .Decompose(out Vec3 _, out Quaternion _, out translation);
                                        camera.position = new Vec2(translation.x, translation.y);
                                    }

                                    didUIScroll = false;
                                    switch (inputMode)
                                    {
                                        case EditorInput.Mouse:
                                            if (Mouse.middle == InputState.Pressed)
                                                _panAnchor = Mouse.position;
                                            if (Mouse.middle == InputState.Down)
                                            {
                                                Vec2 vec2_4 = Mouse.position - _panAnchor;
                                                _panAnchor = Mouse.position;
                                                float num10 = camera.width / Layer.HUD.width;
                                                if (vec2_4.length > 0.01)
                                                    _didPan = true;
                                                camera.x -= vec2_4.x * num10;
                                                camera.y -= vec2_4.y * num10;
                                            }

                                            if (Mouse.middle == InputState.Released)
                                            {
                                                int num11 = _didPan ? 1 : 0;
                                                _didPan = false;
                                            }

                                            break;
                                        case EditorInput.Touch:
                                            if (_twoFingerGesture && !_twoFingerZooming)
                                            {
                                                Vec2 vec2_5 = TouchScreen.GetAverageOfTouches().positionHUD -
                                                              _panAnchor;
                                                _panAnchor = TouchScreen.GetAverageOfTouches().positionHUD;
                                                float num12 = camera.width / Layer.HUD.width;
                                                if (vec2_5.length > 0.1)
                                                {
                                                    _didPan = true;
                                                    camera.x -= vec2_5.x * num12;
                                                    camera.y -= vec2_5.y * num12;
                                                }
                                            }

                                            break;
                                    }

                                    bool flag3 = Keyboard.Down(Keys.LeftAlt) || Keyboard.Down(Keys.RightAlt);
                                    bool flag4 = Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl);
                                    bool flag5 = false;
                                    if (flag3 & flag4)
                                    {
                                        _hover = null;
                                        _secondaryHover = null;
                                        flag5 = true;
                                    }

                                    if ((inputMode == EditorInput.Gamepad ||
                                         inputMode == EditorInput.Touch) && _placementMenu == null)
                                    {
                                        int num13 = 1;
                                        if (_input.Down("LSTICK"))
                                            num13 = 4;
                                        _tilePosition = _tilePositionPrev;
                                        if (_tilePosition.x < camera.left)
                                            _tilePosition.x = camera.left + 32f;
                                        if (_tilePosition.x > camera.right)
                                            _tilePosition.x = camera.right - 32f;
                                        if (_tilePosition.y < camera.top)
                                            _tilePosition.y = camera.top + 32f;
                                        if (_tilePosition.y > camera.bottom)
                                            _tilePosition.y = camera.bottom - 32f;
                                        int num14 = 0;
                                        int num15 = 0;
                                        if (_hoverMode == 0 && (_hoverButton == null || _hoverButton.focus == null))
                                        {
                                            if (_input.Pressed("MENULEFT"))
                                                num15 = -1;
                                            if (_input.Pressed("MENURIGHT"))
                                                num15 = 1;
                                            if (_input.Pressed("MENUUP"))
                                                num14 = -1;
                                            if (_input.Pressed("MENUDOWN"))
                                                num14 = 1;
                                        }

                                        float num16 = _cellSize * num13 * num15;
                                        float num17 = _cellSize * num13 * num14;
                                        _tilePosition.x += num16;
                                        _tilePosition.y += num17;
                                        if (_tilePosition.x < camera.left || _tilePosition.x > camera.right)
                                            camera.x += num16;
                                        if (_tilePosition.y < camera.top || _tilePosition.y > camera.bottom)
                                            camera.y += num17;
                                        if (TouchScreen.GetTouch() != Touch.None)
                                        {
                                            _tilePosition.x =
                                                (float)Math.Round(TouchScreen.GetTouch().positionCamera.x /
                                                                   _cellSize) * _cellSize;
                                            _tilePosition.y =
                                                (float)Math.Round(TouchScreen.GetTouch().positionCamera.y /
                                                                   _cellSize) * _cellSize;
                                            _tilePositionPrev = _tilePosition;
                                        }
                                        else
                                        {
                                            _tilePosition.x = (float)Math.Round(_tilePosition.x / _cellSize) *
                                                              _cellSize;
                                            _tilePosition.y = (float)Math.Round(_tilePosition.y / _cellSize) *
                                                              _cellSize;
                                            _tilePositionPrev = _tilePosition;
                                        }
                                    }
                                    else if (inputMode == EditorInput.Mouse)
                                    {
                                        if (flag3)
                                        {
                                            _tilePosition.x = (float)Math.Round(Mouse.positionScreen.x / 1.0) * 1f;
                                            _tilePosition.y = (float)Math.Round(Mouse.positionScreen.y / 1.0) * 1f;
                                        }
                                        else
                                        {
                                            _tilePosition.x = (float)Math.Round(Mouse.positionScreen.x / _cellSize) *
                                                              _cellSize;
                                            _tilePosition.y = (float)Math.Round(Mouse.positionScreen.y / _cellSize) *
                                                              _cellSize;
                                        }
                                    }

                                    if (_placementType != null && _placementMenu == null)
                                    {
                                        _tilePosition += _placementType.editorOffset;
                                        if (!flag3)
                                            HugObjectPlacement();
                                    }

                                    if (_move != null)
                                        _move.position = new Vec2(_tilePosition);
                                    UpdateDragSelection();
                                    if (!_editMode && !_copyMode && _cursorMode == CursorMode.Normal &&
                                        !_dragSelectShiftModifier && _placementMenu == null)
                                    {
                                        bMouseInput = false;
                                        bGamepadInput = false;
                                        bTouchInput = false;
                                        if (inputMode == EditorInput.Mouse && Mouse.left == InputState.Pressed)
                                        {
                                            bMouseInput = true;
                                            dragModeInputType = InputType.eMouse;
                                        }
                                        else if (inputMode == EditorInput.Gamepad &&
                                                 _input.Pressed("SELECT"))
                                        {
                                            bGamepadInput = true;
                                            dragModeInputType = InputType.eGamepad;
                                        }
                                        else if (inputMode == EditorInput.Touch &&
                                                 TouchScreen.GetDrag() != Touch.None ||
                                                 TouchScreen.GetTap() != Touch.None)
                                        {
                                            bTouchInput = true;
                                            dragModeInputType = InputType.eTouch;
                                        }

                                        if (!_dragMode && (bMouseInput || bGamepadInput || bTouchInput) &&
                                            _placementMode && _hoverMode == 0 &&
                                            (_hoverButton == null || _hoverButton.focus == null))
                                        {
                                            firstClick = true;
                                            _dragMode = true;
                                            History.BeginUndoSection();
                                            Thing hover = _hover;
                                            if (hover != null && (!(_hover is BackgroundTile) ||
                                                                  _placementType != null && _hover.GetType() ==
                                                                  _placementType.GetType()))
                                            {
                                                if ((Keyboard.Down(Keys.LeftControl) ||
                                                     Keyboard.Down(Keys.RightControl)) &&
                                                    !(_placementType is BackgroundTile))
                                                    _move = hover;
                                                else if (!Keyboard.control)
                                                    _deleteMode = true;
                                            }
                                        }

                                        if (_dragMode)
                                        {
                                            if (_tileDragDif == Vec2.MaxValue ||
                                                inputMode == EditorInput.Gamepad)
                                                _tileDragDif = _tilePosition;
                                            Vec2 vec2_6 = Maths.Snap(_tilePosition, _cellSize, _cellSize);
                                            Vec2 vec2_7 = _tilePosition;
                                            Vec2 vec2_8 = Vec2.MaxValue;
                                            do
                                            {
                                                Vec2 vec2_9 = Maths.Snap(vec2_7, _cellSize, _cellSize);
                                                if ((Keyboard.control || Input.Down("SELECT") && Input.Down("MENU1")) &&
                                                    _tileDragContext == Vec2.MinValue)
                                                    _tileDragContext = vec2_9;
                                                if (!(vec2_9 == Maths.Snap(_tileDragDif, _cellSize, _cellSize)) ||
                                                    !(vec2_9 != Maths.Snap(_tilePosition, _cellSize, _cellSize)))
                                                {
                                                    if (vec2_6 != _tilePosition)
                                                    {
                                                        vec2_9 = _tilePosition;
                                                        _tileDragDif = _tilePosition;
                                                    }

                                                    vec2_7 = Lerp.Vec2(vec2_7, _tileDragDif, _cellSize);
                                                    if (_tileDragDif != _tilePosition)
                                                        UpdateHover(placementLayer, vec2_9, true);
                                                    if (!_deleteMode && _placementType != null)
                                                    {
                                                        Thing thing = _hover;
                                                        if (thing == null && !(_placementType is BackgroundTile))
                                                            thing = CollisionPointFilter<Thing>(vec2_9, x =>
                                                            {
                                                                if (x.placementLayer != placementLayer)
                                                                    return false;
                                                                return !(_placementType is PipeTileset) ||
                                                                       x.GetType() == _placementType.GetType();
                                                            });
                                                        if (thing is TileButton)
                                                            thing = null;
                                                        else if (thing != null && !_levelThings.Contains(thing))
                                                            thing = null;
                                                        else if (flag4 & flag3)
                                                            thing = null;
                                                        else if (_placementType is BackgroundTile &&
                                                                 !(thing is BackgroundTile))
                                                            thing = null;
                                                        else if (firstClick && _hover == null)
                                                            thing = null;
                                                        firstClick = false;
                                                        if ((thing == null ||
                                                             _placementType is WireTileset &&
                                                             thing is IWirePeripheral ||
                                                             _placementType is IWirePeripheral &&
                                                             thing is WireTileset) && !placementLimitReached &&
                                                            !placementOutOfSizeRange && vec2_8 != vec2_9)
                                                        {
                                                            vec2_8 = vec2_9;
                                                            Type type = _placementType.GetType();
                                                            Thing newThing = null;
                                                            newThing = _eyeDropperSerialized != null
                                                                ? Thing.LoadThing(_eyeDropperSerialized)
                                                                : CreateThing(type);
                                                            newThing.x = vec2_9.x;
                                                            newThing.y = vec2_9.y;
                                                            if (_placementType is SubBackgroundTile)
                                                                (newThing.graphic as SpriteMap).frame =
                                                                    ((_placementType as SubBackgroundTile)
                                                                        .graphic as SpriteMap).frame;
                                                            if (_placementType is BackgroundTile)
                                                            {
                                                                int num18 = (int)((vec2_9.x - _tileDragContext.x) /
                                                                    16.0);
                                                                int num19 = (int)((vec2_9.y - _tileDragContext.y) /
                                                                    16.0);
                                                                (newThing as BackgroundTile).frame =
                                                                    (_placementType as BackgroundTile).frame + num18 +
                                                                    (int)(num19 * (newThing.graphic.texture.width /
                                                                        16.0));
                                                            }
                                                            else if (_placementType is ForegroundTile)
                                                                (newThing.graphic as SpriteMap).frame =
                                                                    ((_placementType as ForegroundTile)
                                                                        .graphic as SpriteMap).frame;

                                                            if (_hover is BackgroundTile)
                                                                newThing.depth = _hover.depth + 1;
                                                            History.Add(() => AddObject(newThing),
                                                                () => RemoveObject(newThing));
                                                            if (newThing is PathNode)
                                                                _editorLoadFinished = true;
                                                            if (flag5)
                                                                disableDragMode();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Thing col = _hover;
                                                        if (col != null)
                                                        {
                                                            History.Add(() => RemoveObject(col), () => AddObject(col));
                                                            if (col is PathNode)
                                                                _editorLoadFinished = true;
                                                            _hover = null;
                                                        }
                                                    }

                                                    things.RefreshState();
                                                    vec2_1 = vec2_7 - _tileDragDif;
                                                }
                                                else
                                                    break;
                                            } while (vec2_1.length > 2.0);
                                        }

                                        if (Mouse.left == InputState.Released &&
                                            dragModeInputType == InputType.eMouse ||
                                            _input.Released("SELECT") &&
                                            dragModeInputType == InputType.eGamepad ||
                                            TouchScreen.GetRelease() != Touch.None &&
                                            dragModeInputType == InputType.eTouch)
                                            disableDragMode();
                                    }

                                    if (!Keyboard.control && !Input.Down("MENU1"))
                                        _tileDragContext = Vec2.MinValue;
                                    _tileDragDif = _tilePosition;
                                    _placingTiles = false;
                                    if (_placementType is BackgroundTile)
                                        _placingTiles = true;
                                    if (_placingTiles && _placementMenu == null &&
                                        (_input.Pressed("MENU1") && !_input.Down("SELECT") ||
                                         _openTileSelector) && _cursorMode == CursorMode.Normal)
                                    {
                                        DoMenuClose();
                                        int frame = _placementType.frame;
                                        _placementMenu = new ContextBackgroundTile(_placementType, null, false)
                                        {
                                            positionCursor = true
                                        };
                                        _placementMenu.opened = true;
                                        SFX.Play("openClick", 0.4f);
                                        _placementMenu.x = 16f;
                                        _placementMenu.y = 16f;
                                        _placementMenu.selectedIndex = frame;
                                        Add(_placementMenu);
                                        _openTileSelector = false;
                                    }

                                    if (_editMode && _cursorMode == CursorMode.Normal)
                                    {
                                        if (_twoFingerGesture || _threeFingerGesture)
                                            DoMenuClose();
                                        if (clicked && _hover != null)
                                        {
                                            DoMenuClose();
                                            _placementMenu = _hover.GetContextMenu();
                                            if (_placementMenu != null)
                                            {
                                                _placementMenu.x = 96f;
                                                _placementMenu.y = 32f;
                                                if (inputMode == EditorInput.Gamepad ||
                                                    inputMode == EditorInput.Touch)
                                                {
                                                    _placementMenu.x = 16f;
                                                    _placementMenu.y = 16f;
                                                }

                                                _openedEditMenu = true;
                                                AddThing(_placementMenu);
                                                _placementMenu.opened = true;
                                                SFX.Play("openClick", 0.4f);
                                                clicked = false;
                                                _oldHover = _hover;
                                                _lastHoverMenuOpen = _placementMenu;
                                            }
                                        }
                                    }

                                    hoverUI = false;
                                    if (_closeMenu)
                                        DoMenuClose();
                                    base.Update();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void PostUpdate()
        {
            if (_placementMenu != null)
            {
                if (_editMode && clickedMenu)
                    _hover = _oldHover;
                if (inputMode == EditorInput.Touch && TouchScreen.GetTap() != Touch.None &&
                    !clickedMenu && !clickedContextBackground && !_openedEditMenu)
                {
                    if (_touchState == EditorTouchState.OpenMenu)
                        EndCurrentTouchMode();
                    _showPlacementMenu = false;
                    CloseMenu();
                }
            }

            if (_touchState == EditorTouchState.OpenMenu && _placementMenu == null)
            {
                _touchState = EditorTouchState.Normal;
                _activeTouchButton = null;
            }

            _openedEditMenu = false;
        }

        public void DoMenuClose()
        {
            if (_placementMenu != null)
            {
                if (_placementMenu != _objectMenu)
                {
                    RemoveThing(_placementMenu);
                }
                else
                {
                    _placementMenu.visible = false;
                    _placementMenu.active = false;
                    _placementMenu.opened = false;
                }
            }

            _placementMenu = null;
            _closeMenu = false;
        }

        private void UpdateSelection(bool pObjectsChanged = true)
        {
            foreach (Thing levelThing in _levelThings)
            {
                if (pObjectsChanged)
                    levelThing.EditorObjectsChanged();
                levelThing.material = null;
                if (levelThing is Holdable)
                {
                    (levelThing as Holdable).UpdateMaterial();
                }
            }

            foreach (Thing thing in current.things)
            {
                thing.material = null;
                if (thing is Holdable)
                {
                    (thing as Holdable).UpdateMaterial();
                }
            }
            foreach (Thing thing in _selection)
            {
                switch (thing)
                {
                    case AutoBlock _:
                        AutoBlock autoBlock = thing as AutoBlock;
                        if (autoBlock._bLeftNub != null)
                            _currentDragSelectionHover.Add(autoBlock._bLeftNub);
                        if (autoBlock._bRightNub != null)
                        {
                            _currentDragSelectionHover.Add(autoBlock._bRightNub);
                        }

                        continue;
                    case AutoPlatform _:
                        AutoPlatform autoPlatform = thing as AutoPlatform;
                        if (autoPlatform._leftNub != null)
                            _currentDragSelectionHover.Add(autoPlatform._leftNub);
                        if (autoPlatform._rightNub != null)
                        {
                            _currentDragSelectionHover.Add(autoPlatform._rightNub);
                        }

                        continue;
                    case Door _:
                        Door door = thing as Door;
                        if (door._frame != null)
                        {
                            _currentDragSelectionHover.Add(door._frame);
                        }

                        continue;
                    case ItemSpawner _:
                        ItemSpawner itemSpawner = thing as ItemSpawner;
                        if (itemSpawner._ball1 != null)
                            _currentDragSelectionHover.Add(itemSpawner._ball1);
                        if (itemSpawner._ball2 != null)
                        {
                            _currentDragSelectionHover.Add(itemSpawner._ball2);
                        }

                        continue;
                    default:
                        continue;
                }
            }

            foreach (Thing thing in _currentDragSelectionHover)
                thing.material = _selectionMaterial;
            foreach (Thing thing in _currentDragSelectionHoverAdd)
                if (!_currentDragSelectionHover.Contains(thing))
                {
                    thing.material = _selectionMaterial;
                }
                else
                {
                    thing.material = (Material)null;
                    if (thing is Holdable)
                    {
                        (thing as Holdable).UpdateMaterial();
                    }
                }
        }

        private void RebuildPasteBatch()
        {
            _pasteBatch.Clear();
            foreach (BinaryClassChunk node in _selectionCopy)
                _pasteBatch.Add(Thing.LoadThing(node));
        }

        private void UpdateDragSelection()
        {
            _dragSelectShiftModifier = Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift) ||
                                       inputMode == EditorInput.Gamepad && _selection.Count > 0;
            if (_cursorMode == CursorMode.Selection)
            {
                _selectionDragEnd = inputMode == EditorInput.Mouse ? Mouse.positionScreen : _tilePosition;
                Vec2 selectionDragStart = _selectionDragStart;
                Vec2 selectionDragEnd = _selectionDragEnd;
                if (selectionDragEnd.x < selectionDragStart.x)
                {
                    (selectionDragEnd.x, selectionDragStart.x) = (selectionDragStart.x, selectionDragEnd.x);
                }

                if (selectionDragEnd.y < selectionDragStart.y)
                {
                    (selectionDragEnd.y, selectionDragStart.y) = (selectionDragStart.y, selectionDragEnd.y);
                }
                if (_dragSelectShiftModifier)
                {
                    _currentDragSelectionHoverAdd.Clear();
                    foreach (Thing thing in CheckRectAll<Thing>(selectionDragStart, selectionDragEnd))
                        _currentDragSelectionHoverAdd.Add(thing);
                }
                else
                {
                    _currentDragSelectionHover.Clear();
                    foreach (Thing thing in CheckRectAll<Thing>(selectionDragStart, selectionDragEnd))
                        _currentDragSelectionHover.Add(thing);
                }

                if (Mouse.right == InputState.Released || Mouse.left == InputState.Released ||
                    inputMode == EditorInput.Gamepad && _input.Released("CANCEL"))
                {
                    if (_dragSelectShiftModifier)
                    {
                        foreach (Thing thing in _currentDragSelectionHoverAdd)
                        {
                            if (_currentDragSelectionHover.Contains(thing))
                            {
                                _currentDragSelectionHover.Remove(thing);
                                _selection.Remove(thing);
                            }
                            else
                                _currentDragSelectionHover.Add(thing);
                        }
                    }

                    foreach (Thing thing in _currentDragSelectionHover)
                    {
                        if (!(thing is ContextMenu) && _levelThings.Contains(thing) && !_selection.Contains(thing))
                            _selection.Add(thing);
                    }

                    _currentDragSelectionHoverAdd.Clear();
                    //this.dragStartInputType = InputType.eNone;
                    _cursorMode = _selection.Count > 0 ? CursorMode.HasSelection : CursorMode.Normal;
                    clickedMenu = true;
                    _selectionDragStart = Vec2.Zero;
                }

                UpdateSelection(false);
            }
            else if (_cursorMode == CursorMode.Drag)
            {
                Vec2 vec2 = Maths.Snap(Mouse.positionScreen + new Vec2(_cellSize / 2f), _cellSize, _cellSize);
                if (inputMode == EditorInput.Gamepad)
                    vec2 = Maths.Snap(_tilePosition + new Vec2(_cellSize / 2f), _cellSize, _cellSize);
                if (vec2 != _moveDragStart)
                {
                    Vec2 dif = vec2 - _moveDragStart;
                    _moveDragStart = vec2;
                    foreach (Thing thing1 in _currentDragSelectionHover)
                    {
                        Thing t = thing1;
                        History.Add(() =>
                        {
                            Thing thing2 = t;
                            thing2.position += dif;
                            if (!(t is IDontMove))
                                return;
                            current.things.quadTree.Remove(t);
                            current.things.quadTree.Add(t);
                        }, () =>
                        {
                            Thing thing3 = t;
                            thing3.position -= dif;
                            if (!(t is IDontMove))
                                return;
                            current.things.quadTree.Remove(t);
                            current.things.quadTree.Add(t);
                        });
                    }
                }

                if (Mouse.left != InputState.Released && !_input.Released("SELECT"))
                    return;
                _cursorMode = CursorMode.HasSelection;
                UpdateSelection();
                History.EndUndoSection();
                hasUnsavedChanges = true;
            }
            else
            {
                if (_performCopypaste ||
                    (Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl) ||
                     _cursorMode == CursorMode.Pasting) && (_cursorMode == CursorMode.Normal ||
                                                            _cursorMode == CursorMode.HasSelection ||
                                                            _cursorMode == CursorMode.Pasting ||
                                                            _cursorMode == CursorMode.DragHover))
                {
                    bool flag = Keyboard.Pressed(Keys.X);
                    if (_selection.Count > 0 && (Keyboard.Pressed(Keys.C) | flag || _performCopypaste))
                    {
                        _selectionCopy.Clear();
                        _copyCenter = Vec2.Zero;
                        History.BeginUndoSection();
                        foreach (Thing thing in _selection)
                        {
                            Thing t = thing;
                            copying = true;
                            _selectionCopy.Add(t.Serialize());
                            _copyCenter += t.position;
                            copying = false;
                            if (flag)
                                History.Add(() => RemoveObject(t), () => AddObject(t));
                        }

                        _copyCenter /= _selection.Count;
                        if (flag)
                        {
                            _selection.Clear();
                            _currentDragSelectionHover.Clear();
                            UpdateSelection();
                        }

                        History.EndUndoSection();
                        RebuildPasteBatch();
                        HUD.AddPlayerChangeDisplay("@CLIPCOPY@Selection copied!", 1f);
                    }

                    if (Keyboard.Pressed(Keys.V) && _pasteBatch.Count > 0 || _performCopypaste)
                    {
                        _selection.Clear();
                        _currentDragSelectionHover.Clear();
                        _cursorMode = CursorMode.Pasting;
                        UpdateSelection(false);
                    }

                    pasteOffset = Maths.Snap(_copyCenter - Mouse.positionScreen, 16f, 16f);
                    if (inputMode == EditorInput.Gamepad)
                        pasteOffset = Maths.Snap(_copyCenter - _tilePosition, 16f, 16f);
                    _performCopypaste = false;
                    if (_cursorMode == CursorMode.Pasting)
                    {
                        if (Mouse.right == InputState.Released || _input.Released("CANCEL") &&
                            inputMode == EditorInput.Gamepad)
                            _cursorMode = CursorMode.Normal;
                        if (Mouse.left == InputState.Pressed ||
                            _input.Pressed("SELECT") && inputMode == EditorInput.Gamepad)
                        {
                            History.BeginUndoSection();
                            _selection.Clear();
                            _currentDragSelectionHover.Clear();
                            _isPaste = true;
                            foreach (Thing thing4 in _pasteBatch)
                            {
                                _selection.Add(thing4);
                                Thing thing5 = thing4;
                                thing5.position -= pasteOffset;
                                foreach (Thing thing6 in CollisionRectAllDan<Thing>(thing4.position + new Vec2(-6f, -6f),
                                             thing4.position + new Vec2(6f, 6f), null))
                                {
                                    Thing col = thing6;
                                    if (col.placementLayer == thing4.placementLayer && _levelThings.Contains(col))
                                        History.Add(() => RemoveObject(col), () => AddObject(col));
                                }
                            }

                            foreach (Thing thing in _selection)
                            {
                                Thing t = thing;
                                History.Add(() => AddObject(t), () => RemoveObject(t));
                            }

                            _selection.Clear();
                            _currentDragSelectionHover.Clear();
                            _isPaste = false;
                            RebuildPasteBatch();
                            _placeObjects.Clear();
                            things.RefreshState();
                            UpdateSelection();
                            disableDragMode();
                        }
                    }
                }
                else if (_cursorMode == CursorMode.Pasting)
                    _cursorMode = CursorMode.Normal;

                if (_selection.Count > 0 && _cursorMode != CursorMode.Pasting && (Keyboard.Pressed(Keys.F) ||
                        _input.Pressed("MENU1") && inputMode == EditorInput.Gamepad))
                {
                    Vec2 zero = Vec2.Zero;
                    Vec2 pPosition;
                    if (_cursorMode == CursorMode.Pasting)
                    {
                        foreach (Thing thing in _pasteBatch)
                            zero += thing.position;
                        pPosition = zero / _pasteBatch.Count;
                    }
                    else
                    {
                        foreach (Thing thing in _selection)
                            zero += thing.position;
                        pPosition = zero / _selection.Count;
                    }

                    Vec2 vec2 = Maths.SnapRound(pPosition, _cellSize / 2f, _cellSize / 2f);
                    if (_cursorMode == CursorMode.Pasting)
                    {
                        foreach (Thing thing in _pasteBatch)
                        {
                            thing.SetTranslation(new Vec2((float)(-(thing.position.x - vec2.x) * 2.0), 0f));
                            thing.EditorFlip(false);
                            thing.flipHorizontal = !thing.flipHorizontal;
                            Level.current.things.UpdateObject(thing);
                        }
                    }
                    else
                    {
                        History.BeginUndoSection();
                        foreach (Thing thing in _selection)
                        {
                            Thing t = thing;
                            float dif = t.position.x - vec2.x;
                            History.Add(() =>
                            {
                                t.SetTranslation(new Vec2((float)(-dif * 2.0), 0f));
                                t.EditorFlip(false);
                                t.flipHorizontal = !t.flipHorizontal;
                                if (!(t is IDontMove))
                                    return;
                                current.things.quadTree.Remove(t);
                                current.things.quadTree.Add(t);
                                Level.current.things.UpdateObject(t);
                            }, () =>
                            {
                                t.SetTranslation(new Vec2(dif * 2f, 0f));
                                t.EditorFlip(false);
                                t.flipHorizontal = !t.flipHorizontal;
                                if (!(t is IDontMove))
                                    return;
                                current.things.quadTree.Remove(t);
                                current.things.quadTree.Add(t);
                                Level.current.things.UpdateObject(t);
                            });
                        }

                        UpdateSelection();
                        History.EndUndoSection();
                    }

                    UpdateSelection();
                }

                if (_selection.Count > 0)
                {
                    _cursorMode = CursorMode.HasSelection;
                    switch (inputMode)
                    {
                        case EditorInput.Gamepad:
                            using (HashSet<Thing>.Enumerator enumerator = _selection.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    if (Collision.Point(_tilePosition, enumerator.Current))
                                    {
                                        _cursorMode = CursorMode.DragHover;
                                        break;
                                    }
                                }

                                break;
                            }
                        case EditorInput.Mouse:
                            using (HashSet<Thing>.Enumerator enumerator = _selection.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    if (Collision.Point(Mouse.positionScreen, enumerator.Current))
                                    {
                                        _cursorMode = CursorMode.DragHover;
                                        break;
                                    }
                                }

                                break;
                            }
                    }

                    bool flag = false;
                    if (Keyboard.Pressed(Keys.Delete) ||
                        _input.Pressed("MENU2") && inputMode == EditorInput.Gamepad)
                    {
                        History.BeginUndoSection();
                        foreach (Thing thing in _selection)
                        {
                            Thing t = thing;
                            History.Add(() => RemoveObject(t), () => AddObject(t));
                        }

                        UpdateSelection();
                        History.EndUndoSection();
                        flag = true;
                    }

                    if (Mouse.left == InputState.Pressed ||
                        _input.Pressed("SELECT") && inputMode == EditorInput.Gamepad)
                    {
                        if (_cursorMode == CursorMode.DragHover)
                        {
                            History.BeginUndoSection();
                            _cursorMode = CursorMode.Drag;
                            _moveDragStart = inputMode != EditorInput.Gamepad
                                ? Maths.Snap(Mouse.positionScreen + new Vec2(_cellSize / 2f), _cellSize, _cellSize)
                                : Maths.Snap(_tilePosition + new Vec2(_cellSize / 2f), _cellSize, _cellSize);
                        }
                        else
                            flag = true;
                    }

                    if (_input.Released("CANCEL"))
                    {
                        if (_cursorMode == CursorMode.DragHover)
                            _performCopypaste = true;
                        else
                            flag = true;
                    }

                    if (_cursorMode != CursorMode.Pasting && Mouse.right == InputState.Released | flag &&
                        (!_dragSelectShiftModifier || inputMode == EditorInput.Gamepad))
                    {
                        _cursorMode = CursorMode.Normal;
                        _selection.Clear();
                        _currentDragSelectionHover.Clear();
                        UpdateSelection(false);
                    }

                    Vec2 offset = new Vec2(0f, 0f);
                    if (Keyboard.Pressed(Keys.Up))
                        offset.y -= cellSize;
                    if (Keyboard.Pressed(Keys.Down))
                        offset.y += cellSize;
                    if (Keyboard.Pressed(Keys.Left))
                        offset.x -= cellSize;
                    if (Keyboard.Pressed(Keys.Right))
                        offset.x += cellSize;
                    if (!(offset != Vec2.Zero))
                        return;
                    hasUnsavedChanges = true;
                    History.BeginUndoSection();
                    foreach (Thing thing in _selection)
                    {
                        Thing t = thing;
                        History.Add(() =>
                        {
                            t.SetTranslation(offset);
                            if (!(t is IDontMove))
                                return;
                            current.things.quadTree.Remove(t);
                            current.things.quadTree.Add(t);
                            Level.current.things.UpdateObject(t);
                        }, () =>
                        {
                            t.SetTranslation(-offset);
                            if (!(t is IDontMove))
                                return;
                            current.things.quadTree.Remove(t);
                            current.things.quadTree.Add(t);
                            Level.current.things.UpdateObject(t);

                        });
                    }

                    UpdateSelection();
                    History.EndUndoSection();
                }
                else
                {
                    if (_cursorMode != CursorMode.HasSelection)
                        return;
                    _cursorMode = CursorMode.Normal;
                }
            }
        }

        private void UpdateHover(Layer placementLayer, Vec2 tilePosition, bool isDrag = false)
        {
            IEnumerable<Thing> source1 = new List<Thing>();
            if (inputMode == EditorInput.Gamepad | isDrag)
                source1 = CollisionPointAll<Thing>(tilePosition);
            else if (inputMode == EditorInput.Touch && TouchScreen.IsScreenTouched())
            {
                if (_editMode || _copyMode)
                {
                    if (TouchScreen.GetTap() != Touch.None)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            source1 = CollisionCircleAll<Thing>(TouchScreen.GetTap().positionCamera, index * 2f);
                            if (source1.Count() > 0)
                                break;
                        }

                        _hover = null;
                    }
                }
                else if (TouchScreen.GetTouch() != Touch.None)
                    source1 = CollisionPointAll<Thing>(tilePosition);
            }
            else if (inputMode == EditorInput.Mouse && !isDrag)
                source1 = CollisionPointAll<Thing>(Mouse.positionScreen);

            oldHover = _hover;
            if (!_editMode)
                _hover = null;
            _secondaryHover = null;
            List<Thing> source2 = new List<Thing>();
            foreach (Thing thing in source1)
            {
                if (!(thing is TileButton) && _placeables.Contains(thing.GetType()) && thing.editorCanModify &&
                    _things.Contains(thing) && (!(_placementType is WireTileset) || !(thing is IWirePeripheral)) &&
                    (!(_placementType is IWirePeripheral) || !(thing is WireTileset)))
                {
                    if (_placementType is PipeTileset && thing is PipeTileset &&
                        _placementType.GetType() != thing.GetType())
                        source2.Add(thing);
                    else if (thing.placementLayer != placementLayer && !_copyMode && !_editMode)
                        source2.Add(thing);
                    else if (_hover == null)
                    {
                        if (_placementType != null && _placementType is BackgroundTile)
                        {
                            if (_things.Contains(thing))
                            {
                                if (thing.GetType() == _placementType.GetType())
                                    _hover = thing;
                                else
                                    source2.Add(thing);
                            }
                        }
                        else if (thing.editorCanModify)
                            _hover = thing;
                    }
                    else if (thing != _hover)
                        source2.Add(thing);
                }
            }

            if (inputMode == EditorInput.Mouse && !isDrag && _hover == null &&
                !(_placementType is BackgroundTile) && !(_placementType is PipeTileset))
            {
                List<KeyValuePair<float, Thing>> keyValuePairList = current.nearest(tilePosition,
                    _levelThings.AsEnumerable(), null, placementLayer, true);
                if (keyValuePairList.Count > 0 &&
                    (!(_placementType is WireTileset) || !(keyValuePairList[0].Value is IWirePeripheral)) &&
                    (!(_placementType is IWirePeripheral) || !(keyValuePairList[0].Value is WireTileset)) &&
                    (keyValuePairList[0].Value.position - tilePosition).length < 8.0)
                    _hover = keyValuePairList[0].Value;
            }

            if (_hover == null || oldHover == null || _hover.GetType() != oldHover.GetType())
                _hoverMenu = _hover != null ? _hover.GetContextMenu() : null;
            if (source2.Count > 0)
            {
                IOrderedEnumerable<Thing> source3 =
                    source2.OrderBy(x => x.placementLayer == null ? -99999 : x.placementLayer.depth);
                if (Keyboard.control)
                    _hover = _hover != null
                        ? source3.First()
                        : source2.OrderBy(x => x.placementLayer == null ? 99999 : -x.placementLayer.depth)
                            .First();
                else if (_hover == null || Keyboard.control || _placementType != null &&
                         source3.First().placementLayer == _placementType.placementLayer)
                {
                    _secondaryHover = source3.First();
                    if (_hoverMenu == null)
                        _hoverMenu = _secondaryHover.GetContextMenu();
                }
            }

            if (_secondaryHover != null || !(_hover is Block) || source2.Count <= 0)
                return;
            _secondaryHover = source2.FirstOrDefault(x => x is PipeTileset);
            if (_secondaryHover == null || (_secondaryHover as PipeTileset)._foregroundDraw)
                return;
            _secondaryHover = null;
        }

        public override void Draw() => base.Draw();

        public static bool arcadeMachineMode => Level.current is Editor current && current._levelThings.Count == 1 &&
                                                current._levelThings[0] is ImportMachine;

        private void CalculateGridRestriction()
        {
            Vec2 vec2 = _sizeRestriction * 2f - (_bottomRightMost - _topLeftMost) - new Vec2(16f, 16f);
            if (vec2.x > _sizeRestriction.x * 2.0)
                vec2.x = _sizeRestriction.x * 2f;
            if (vec2.y > _sizeRestriction.y * 2.0)
                vec2.y = _sizeRestriction.y * 2f;
            _gridW = (int)(vec2.x / _cellSize);
            _gridH = (int)(vec2.y / _cellSize);
        }

        public override void PostDrawLayer(Layer layer)
        {
            base.PostDrawLayer(layer);
            if (layer == Layer.Foreground)
            {
                foreach (Thing thing in things)
                    thing.DoEditorRender();
            }

            if (layer == _procLayer && _procTarget != null && _procContext != null)
                Graphics.Draw(_procTarget, new Vec2(0f, 0f), new Rectangle?(), Color.White * 0.5f, 0f,
                    Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None);
            if (layer == _gridLayer)
            {
                backgroundColor = new Color(20, 20, 20);
                Color col = new Color(38, 38, 38);
                if (arcadeMachineMode)
                {
                    Graphics.DrawRect(_levelThings[0].position + new Vec2(-17f, -21f),
                        _levelThings[0].position + new Vec2(18f, 21f), col, -0.9f, false);
                }
                else
                {
                    float x = (float)(-_cellSize / 2.0);
                    float y = (float)(-_cellSize / 2.0);
                    if (_sizeRestriction.x > 0.0)
                    {
                        Vec2 vec2 = -new Vec2((float)(_gridW * _cellSize / 2.0),
                            (float)((_gridH - 1) * _cellSize / 2.0)) + new Vec2(8f, 0f);
                        x += (int)(vec2.x / _cellSize) * _cellSize;
                        y += (int)(vec2.y / _cellSize) * _cellSize;
                    }

                    int num1 = _gridW;
                    int num2 = _gridH;
                    if (_miniMode)
                    {
                        num1 = 12;
                        num2 = 9;
                    }

                    if (x < _ultimateBounds.x)
                    {
                        int num3 = (int)((_ultimateBounds.x - x) / _cellSize) + 1;
                        x = (int)(_ultimateBounds.x / _cellSize * _cellSize) + _cellSize / 2f;
                        num1 -= num3;
                    }

                    if (y < _ultimateBounds.y)
                    {
                        int num4 = (int)((_ultimateBounds.y - y) / _cellSize) + 1;
                        y = (int)(_ultimateBounds.y / _cellSize * _cellSize) + _cellSize / 2f;
                        num2 -= num4;
                    }

                    float num5 = x + num1 * _cellSize;
                    if (num5 > _ultimateBounds.Right)
                    {
                        int num6 = (int)((num5 - _ultimateBounds.Right) / _cellSize) + 1;
                        num1 -= num6;
                        x = (int)((_ultimateBounds.Right - num1 * _cellSize) / _cellSize * _cellSize) - _cellSize / 2f;
                    }

                    float num7 = y + num2 * _cellSize;
                    if (y + num2 * _cellSize > _ultimateBounds.Bottom)
                    {
                        int num8 = (int)((num7 - _ultimateBounds.Bottom) / _cellSize) + 1;
                        num2 -= num8;
                        y = (int)((_ultimateBounds.Bottom - num2 * _cellSize) / _cellSize * _cellSize) -
                            _cellSize / 2f;
                    }

                    int num9 = num1 * (int)_cellSize;
                    int num10 = num2 * (int)_cellSize;
                    int num11 = (int)(num9 / _cellSize);
                    int num12 = (int)(num10 / _cellSize);
                    for (int index = 0; index < num11 + 1; ++index)
                        Graphics.DrawLine(new Vec2(x + index * _cellSize, y),
                            new Vec2(x + index * _cellSize, y + num12 * _cellSize), col, 2f, -0.9f);
                    for (int index = 0; index < num12 + 1; ++index)
                        Graphics.DrawLine(new Vec2(x, y + index * _cellSize),
                            new Vec2(x + num11 * _cellSize, y + index * _cellSize), col, 2f, -0.9f);
                    Graphics.DrawLine(new Vec2(_ultimateBounds.Left, _ultimateBounds.Top),
                        new Vec2(_ultimateBounds.Right, _ultimateBounds.Top), col, 2f, -0.9f);
                    Graphics.DrawLine(new Vec2(_ultimateBounds.Right, _ultimateBounds.Top),
                        new Vec2(_ultimateBounds.Right, _ultimateBounds.Bottom), col, 2f, -0.9f);
                    Graphics.DrawLine(new Vec2(_ultimateBounds.Right, _ultimateBounds.Bottom),
                        new Vec2(_ultimateBounds.Left, _ultimateBounds.Bottom), col, 2f, -0.9f);
                    Graphics.DrawLine(new Vec2(_ultimateBounds.Left, _ultimateBounds.Bottom),
                        new Vec2(_ultimateBounds.Left, _ultimateBounds.Top), col, 2f, -0.9f);
                    if (_miniMode)
                    {
                        int num13 = 0;
                        if (!_pathNorth)
                        {
                            _sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            _sideArrow.color = new Color(100, 200, 100);
                            Graphics.DrawLine(new Vec2(x + num9 / 2, y - 10f),
                                new Vec2(x + num9 / 2, (float)(y + num10 / 2 - 8.0)), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }

                        if (!_pathWest)
                        {
                            _sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            _sideArrow.color = new Color(100, 200, 100);
                            Graphics.DrawLine(new Vec2(x - 10f, y + num10 / 2),
                                new Vec2((float)(x + num9 / 2 - 8.0), y + num10 / 2), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }

                        if (!_pathEast)
                        {
                            _sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            _sideArrow.color = new Color(100, 200, 100);
                            Graphics.DrawLine(new Vec2((float)(x + num9 / 2 + 8.0), y + num10 / 2),
                                new Vec2((float)(x + num9 + 10.0), y + num10 / 2), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }

                        if (!_pathSouth)
                        {
                            _sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            _sideArrow.color = new Color(100, 200, 100);
                            Graphics.DrawLine(new Vec2(x + num9 / 2, (float)(y + num10 / 2 + 8.0)),
                                new Vec2(x + num9 / 2, (float)(y + num10 + 10.0)), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }

                        if (num13 > 0)
                            Graphics.DrawLine(new Vec2((float)(x + num9 / 2 - 8.0), y + num10 / 2),
                                new Vec2((float)(x + num9 / 2 + 8.0), y + num10 / 2), Color.Lime * 0.06f, 16f);
                    }
                }
            }

            if (layer == Layer.Foreground)
            {
                float num14 = (float)(-_cellSize / 2.0);
                float num15 = (float)(-_cellSize / 2.0);
                int num16 = _gridW;
                int num17 = _gridH;
                if (_miniMode)
                {
                    num16 = 12;
                    num17 = 9;
                }

                int num18 = num16 * 16;
                int num19 = num17 * 16;
                if (_miniMode)
                {
                    _procTilesWide = (int)_genSize.x;
                    _procTilesHigh = (int)_genSize.y;
                    _procXPos = (int)_genTilePos.x;
                    _procYPos = (int)_genTilePos.y;
                    if (_procXPos > _procTilesWide)
                        _procXPos = _procTilesWide;
                    if (_procYPos > _procTilesHigh)
                        _procYPos = _procTilesHigh;
                    for (int index1 = 0; index1 < _procTilesWide; ++index1)
                    {
                        for (int index2 = 0; index2 < _procTilesHigh; ++index2)
                        {
                            int num20 = index1 - _procXPos;
                            int num21 = index2 - _procYPos;
                            if (index1 != _procXPos || index2 != _procYPos)
                                Graphics.DrawRect(new Vec2(num14 + num18 * num20, num15 + num19 * num21),
                                    new Vec2(num14 + num18 * (num20 + 1), num15 + num19 * (num21 + 1)),
                                    Color.White * 0.2f, 1f, false);
                        }
                    }
                }

                if (_hoverButton == null)
                {
                    if (_cursorMode != CursorMode.Pasting)
                    {
                        if (_secondaryHover != null && _placementMode)
                            Graphics.DrawRect(_secondaryHover.topLeft, _secondaryHover.bottomRight,
                                Color.White * 0.5f, 1f, false);
                        else if (_hover != null && _placementMode &&
                                 (inputMode != EditorInput.Touch || _editMode))
                        {
                            Graphics.DrawRect(_hover.topLeft, _hover.bottomRight, Color.White * 0.5f,
                                1f, false);
                            _hover.DrawHoverInfo();
                        }
                    }

                    if (DevConsole.wagnusDebug)
                    {
                        Graphics.DrawLine(_tilePosition, _tilePosition + new Vec2(128f, 0f),
                            Color.White * 0.5f);
                        Graphics.DrawLine(_tilePosition, _tilePosition + new Vec2(sbyte.MinValue, 0f),
                            Color.White * 0.5f);
                        Graphics.DrawLine(_tilePosition, _tilePosition + new Vec2(0f, 128f),
                            Color.White * 0.5f);
                        Graphics.DrawLine(_tilePosition, _tilePosition + new Vec2(0f, sbyte.MinValue),
                            Color.White * 0.5f);
                    }

                    if ((_hover == null || _cursorMode == CursorMode.DragHover || _cursorMode == CursorMode.Drag) &&
                        inputMode == EditorInput.Gamepad)
                    {
                        if (_cursorMode == CursorMode.DragHover || _cursorMode == CursorMode.Drag)
                        {
                            _cursor.depth = 1f;
                            _cursor.scale = new Vec2(1f, 1f);
                            _cursor.position = _tilePosition;
                            if (_cursorMode == CursorMode.DragHover)
                                _cursor.frame = 1;
                            else if (_cursorMode == CursorMode.Drag)
                                _cursor.frame = 5;
                            _cursor.Draw();
                        }
                        else if (_placementMenu == null)
                            Graphics.DrawRect(_tilePosition - new Vec2(_cellSize / 2f, _cellSize / 2f),
                                _tilePosition + new Vec2(_cellSize / 2f, _cellSize / 2f), Color.White * 0.5f,
                                1f, false);
                    }

                    if (_cursorMode == CursorMode.Normal && _hover == null && _placementMode &&
                        inputMode != EditorInput.Touch && _placementMenu == null && _placementType != null)
                    {
                        _placementType.depth = 0.9f;
                        _placementType.x = _tilePosition.x;
                        _placementType.y = _tilePosition.y;
                        _placementType.Draw();
                        if (placementLimitReached || placementOutOfSizeRange)
                            Graphics.Draw(_cantPlace, _placementType.x, _placementType.y, 0.95f);
                    }
                }

                if (_cursorMode == CursorMode.Selection || _cursorMode == CursorMode.HasSelection ||
                    _cursorMode == CursorMode.Drag || _cursorMode == CursorMode.DragHover)
                {
                    //this._leftSelectionDraw = false;
                    if (_cursorMode == CursorMode.Selection)
                        Graphics.DrawDottedRect(_selectionDragStart, _selectionDragEnd, Color.White * 0.5f,
                            1f, 2f, 4f);
                }

                if (_cursorMode == CursorMode.Pasting)
                {
                    Graphics.material = _selectionMaterialPaste;
                    foreach (Thing thing in _pasteBatch)
                    {
                        Vec2 position = thing.position;
                        thing.position -= pasteOffset;
                        thing.Draw();
                        thing.position = position;
                    }

                    Graphics.material = null;
                }
            }

            if (layer == Layer.HUD)
            {
                if (inputMode == EditorInput.Touch)
                {
                    float l = -24f;
                    if (_activeTouchButton != null || _fileDialog.opened)
                    {
                        if (_activeTouchButton != null)
                            Graphics.DrawString(_activeTouchButton.explanation,
                                Layer.HUD.camera.OffsetBR(-20f, l) - new Vec2(
                                    Graphics.GetStringWidth(_activeTouchButton.explanation) +
                                    (_cancelButton.size.x + 4f), 0f), Color.Gray, 0.99f);
                        else if (_fileDialog.opened)
                        {
                            string text = "Double tap level to open!";
                            Graphics.DrawString(text,
                                Layer.HUD.camera.OffsetBR(-20f, l) -
                                new Vec2(Graphics.GetStringWidth(text) + (_cancelButton.size.x + 4f), 0f),
                                Color.Gray, 0.99f);
                        }

                        Graphics.DrawRect(_cancelButton.position, _cancelButton.position + _cancelButton.size,
                            new Color(70, 70, 70), 0.99f, false);
                        Graphics.DrawRect(_cancelButton.position, _cancelButton.position + _cancelButton.size,
                            new Color(30, 30, 30), 0.98f);
                        Graphics.DrawString(_cancelButton.caption,
                            _cancelButton.position + _cancelButton.size / 2f + new Vec2(
                                (float)(-Graphics.GetStringWidth(_cancelButton.caption) / 2.0), -4f),
                            Color.White, 0.99f);
                    }
                    else if (!_fileDialog.opened)
                    {
                        float num = 0f;
                        foreach (EditorTouchButton touchButton in _touchButtons)
                        {
                            Graphics.DrawRect(touchButton.position, touchButton.position + touchButton.size,
                                new Color(70, 70, 70), 0.99f, false);
                            Graphics.DrawRect(touchButton.position, touchButton.position + touchButton.size,
                                new Color(30, 30, 30), 0.98f);
                            Graphics.DrawString(touchButton.caption,
                                touchButton.position + touchButton.size / 2f + new Vec2(
                                    (float)(-Graphics.GetStringWidth(touchButton.caption) / 2.0), -4f),
                                Color.White, 0.99f);
                            num += touchButton.size.x;
                        }

                        if (_placementMenu != null && _placementMenu is EditorGroupMenu)
                        {
                            string text = "Double tap to select!";
                            Graphics.DrawString(text,
                                Layer.HUD.camera.OffsetBR(-20f, l) -
                                new Vec2(Graphics.GetStringWidth(text) + (num + 8f), 0f), Color.Gray,
                                0.99f);
                        }
                    }

                    if (_placingTiles && _placementMenu == null)
                    {
                        Graphics.DrawRect(_editTilesButton.position,
                            _editTilesButton.position + _editTilesButton.size, new Color(70, 70, 70), 0.99f,
                            false);
                        Graphics.DrawRect(_editTilesButton.position,
                            _editTilesButton.position + _editTilesButton.size, new Color(30, 30, 30), 0.98f);
                        Graphics.DrawString(_editTilesButton.caption,
                            _editTilesButton.position + _editTilesButton.size / 2f + new Vec2(
                                (float)(-Graphics.GetStringWidth(_editTilesButton.caption) / 2.0), -4f),
                            Color.White, 0.99f);
                    }
                }

                if (hasUnsavedChanges)
                    Graphics.DrawFancyString("*", new Vec2(4f, 4f), Color.White * 0.6f, 0.99f);
                if (tooltip != null)
                {
                    Graphics.DrawRect(new Vec2(16f, Layer.HUD.height - 14f),
                        new Vec2((float)(16.0 + Graphics.GetFancyStringWidth(tooltip) + 2.0),
                            Layer.HUD.height - 2f), new Color(0, 0, 0) * 0.75f, 0.99f);
                    Graphics.DrawFancyString(tooltip, new Vec2(18f, Layer.HUD.height - 12f),
                        Color.White, 0.99f);
                }

                bool flag1 = _input.lastActiveDevice is Keyboard;
                if (_hoverMode == 0 && _hoverButton == null)
                {
                    string text1 = "";
                    string str1 = "@CANCEL@";
                    string str2 = "@SELECT@";
                    string str3 = "@CANCEL@";
                    string str4 = "@MENU2@";
                    string str5 = "@START@";
                    string str6 = "@STRAFE@";
                    string str7 = "@RAGDOLL@";
                    if (flag1)
                    {
                        str1 = "@RIGHTMOUSE@" + str1;
                        str2 = "@LEFTMOUSE@" + str2;
                        str3 = "@MIDDLEMOUSE@" + str3;
                        str4 = "@RIGHTMOUSE@" + str4;
                    }

                    if (_cursorMode == CursorMode.HasSelection || _cursorMode == CursorMode.Drag ||
                        _cursorMode == CursorMode.DragHover)
                    {
                        if (inputMode == EditorInput.Gamepad)
                        {
                            if (_cursorMode == CursorMode.DragHover || _cursorMode == CursorMode.Drag)
                                text1 += "@SELECT@DRAG  ";
                            if (_cursorMode == CursorMode.HasSelection || _cursorMode == CursorMode.DragHover)
                                text1 = text1 + "@CANCEL@DRAG ADD  " + "@MENU1@FLIP  " + "@MENU2@DELETE  ";
                            text1 += _cursorMode == CursorMode.DragHover ? "@CANCEL@COPY  " : "@CANCEL@DESELECT  ";
                        }
                        else
                        {
                            string str8 = "@KBDARROWS@NUDGE  ";
                            if (_cursorMode == CursorMode.DragHover)
                                str8 += "@LEFTMOUSE@DRAG  ";
                            text1 = str8 + "@RIGHTMOUSE@DESELECT  ";
                            if (_cursorMode == CursorMode.HasSelection || _cursorMode == CursorMode.DragHover)
                                text1 = text1 + "@KBDSHIFT@ADD SELECTION  " + "@KBDF@FLIP  ";
                        }
                    }
                    else if (_cursorMode == CursorMode.Pasting)
                        text1 = inputMode != EditorInput.Gamepad
                            ? text1 + "@LEFTMOUSE@PASTE  " + "@RIGHTMOUSE@CANCEL  "
                            : text1 + "@SELECT@PASTE  " + "@CANCEL@CANCEL  ";
                    else if (_fileDialog.opened)
                        text1 = "@SHOOT@SET DEFAULT  @WASD@MOVE  " + str2 + "SELECT  @MENU2@DELETE  " + str1 +
                                "CANCEL  @STRAFE@+@RAGDOLL@BROWSE..";
                    else if (_menuOpen && inputMode == EditorInput.Gamepad)
                        text1 = "@SHOOT@SET DEFAULT  @WASD@MOVE  " + str2 + "SELECT  @RIGHT@EXPAND  " + str1 + "CLOSE";
                    else if (inputMode == EditorInput.Gamepad || inputMode == EditorInput.Mouse)
                    {
                        int num = _secondaryHover != null ? 1 : (_hover != null ? 1 : 0);
                        bool flag2 = num != 0 || _placingTiles || _placementType != null;
                        if (_placementType != null && _hover != null &&
                            GetLayerOrOverride(_placementType) == GetLayerOrOverride(_hover))
                            text1 = text1 + str2 + "ERASE  ";
                        else if (_placementType != null)
                        {
                            text1 = text1 + str2 + "PLACE  ";
                            if (rotateValid)
                                text1 += "@RSTICK@ROTATE  ";
                        }

                        if (num != 0)
                            text1 = text1 + str3 + "COPY  ";
                        if (_hover != null && !_placingTiles && _hoverMenu != null)
                            text1 += "@MENU1@EDIT  ";
                        if (inputMode == EditorInput.Gamepad)
                        {
                            if (History.hasUndo)
                                text1 = text1 + str6 + "UNDO  ";
                            if (History.hasRedo)
                                text1 = text1 + str7 + "REDO  ";
                            text1 += "@CANCEL@DRAG SELECT  ";
                        }

                        if (_placingTiles)
                            text1 += "@MENU1@TILES  ";
                        if (flag2)
                            text1 = text1 + str5 + "BROWSE  ";
                        text1 = text1 + str4 + "MENU";
                        if (_font.GetWidth(text1) < 397.0)
                            text1 = "@WASD@MOVE  " + text1;
                        if (inputMode == EditorInput.Mouse)
                            text1 += "  @RIGHTMOUSE@DRAG SELECT";
                    }

                    if (inputMode == EditorInput.Touch)
                        text1 = "";
                    if (text1 != "")
                    {
                        float width = _font.GetWidth(text1);
                        Vec2 vec2 = new Vec2(layer.width - 22f - width, layer.height - 28f);
                        _font.depth = 0.8f;
                        _font.Draw(text1, vec2.x, vec2.y, Color.White, 0.7f, _input);
                    }

                    _font.scale = new Vec2(0.5f, 0.5f);
                    float num22 = 0f;
                    if (placementLimit > 0)
                    {
                        num22 -= 16f;
                        Vec2 vec2 = new Vec2(128f, 12f);
                        Vec2 p1_1 = new Vec2(31f, layer.height - 19f - vec2.y);
                        Graphics.DrawRect(p1_1, p1_1 + vec2, Color.Black * 0.5f, 0.6f);
                        Graphics.Draw(_editorCurrency, p1_1.x - 10f, p1_1.y + 2f, 0.95f);
                        float x = (vec2.x - 4f) * Math.Min(placementTotalCost / (float)placementLimit, 1f);
                        string text2 = placementTotalCost + "/" + placementLimit;
                        if (placementLimitReached)
                            text2 += " FULL!";
                        float width = _font.GetWidth(text2);
                        _font.Draw(text2, (float)(p1_1.x + vec2.x / 2.0 - width / 2.0), p1_1.y + 4f, Color.White,
                            0.7f);
                        Vec2 p1_2 = p1_1 + new Vec2(2f, 2f);
                        Graphics.DrawRect(p1_2, p1_2 + new Vec2(x, vec2.y - 4f),
                            (placementLimitReached ? Colors.DGRed : Colors.DGGreen) * 0.5f, 0.6f);
                    }

                    if (searching)
                    {
                        Graphics.DrawRect(Vec2.Zero, new Vec2(layer.width, layer.height), Color.Black * 0.5f,
                            0.9f);
                        Vec2 position = new Vec2(8f, layer.height - 26f);
                        Graphics.DrawString("@searchiconwhitebig@", position, Color.White, 0.95f);
                        if (Keyboard.keyString == "")
                            Graphics.DrawString("|GRAY|Type to search...", position + new Vec2(26f, 7f),
                                Color.White, 0.95f);
                        else
                            Graphics.DrawString(Keyboard.keyString + "_", position + new Vec2(26f, 7f),
                                Color.White, 0.95f);
                        if (inputMode == EditorInput.Mouse)
                            _searchHoverIndex = -1;
                        float num23 = 200f;
                        if (searchItems != null && searchItems.Count > 0)
                        {
                            position.y -= 22f;
                            for (int index = 0; index < 10 && index < searchItems.Count; ++index)
                            {
                                Graphics.DrawString(searchItems[index].thing.thing.editorName,
                                    new Vec2(position.x + 24f, position.y + 6f), Color.White, 0.95f);
                                searchItems[index].thing.image.depth = 0.95f;
                                searchItems[index].thing.image.x = position.x + 4f;
                                searchItems[index].thing.image.y = position.y;
                                searchItems[index].thing.image.color = Color.White;
                                searchItems[index].thing.image.scale = new Vec2(1f);
                                searchItems[index].thing.image.Draw();
                                if (inputMode == EditorInput.Mouse && Mouse.x > position.x &&
                                    Mouse.x < position.x + 200.0 && Mouse.y > position.y - 2.0 &&
                                    Mouse.y < position.y + 19.0 || index == _searchHoverIndex)
                                {
                                    _searchHoverIndex = index;
                                    Graphics.DrawRect(position + new Vec2(2f, -2f),
                                        position + new Vec2(num23 - 2f, 18f), new Color(70, 70, 70), 0.93f);
                                }

                                position.y -= 20f;
                            }

                            Graphics.DrawRect(position + new Vec2(0f, 16f),
                                new Vec2(position.x + num23, layer.height - 28f), new Color(30, 30, 30), 0.91f);
                        }

                        Graphics.DrawRect(new Vec2(8f, layer.height - 26f), new Vec2(300f, layer.height - 6f),
                            new Color(30, 30, 30), 0.91f);
                    }

                    float num24 = 0f;
                    if (_placementType != null && _cursorMode == CursorMode.Normal && _placementMenu == null)
                    {
                        Vec2 vec2 = new Vec2(_placementType.width, _placementType.height);
                        vec2.x += 4f;
                        vec2.y += 4f;
                        if (vec2.x < 32.0)
                            vec2.x = 32f;
                        if (vec2.y < 32.0)
                            vec2.y = 32f;
                        Vec2 p1 = new Vec2(19f, layer.height - 19f - vec2.y + num22);
                        string str9 = _placementType.GetDetailsString();
                        while (str9.Count(x => x == '\n') > 5)
                            str9 = str9.Substring(0, str9.LastIndexOf('\n'));
                        float x1 = _font.GetWidth(str9) + 8f;
                        if (str9 != "")
                            _font.Draw(str9, (float)(p1.x + vec2.x + 4.0), p1.y + 4f, Color.White, 0.7f);
                        else
                            x1 = 0f;
                        Graphics.DrawRect(p1, p1 + vec2 + new Vec2(x1, 0f), Color.Black * 0.5f, 0.6f);
                        editorDraw = true;
                        _placementType.left = p1.x + (float)(vec2.x / 2.0 - _placementType.w / 2.0);
                        _placementType.top = p1.y + (float)(vec2.y / 2.0 - _placementType.h / 2.0);
                        _placementType.depth = 0.7f;
                        _placementType.Draw();
                        editorDraw = false;
                        _font.Draw("Placing (" + _placementType.editorName + ")", p1.x, p1.y - 6f, Color.White,
                            0.7f);
                        num24 = vec2.y;
                    }

                    Thing thing = _hover;
                    if (_secondaryHover != null)
                        thing = _secondaryHover;
                    if (thing != null && _cursorMode == CursorMode.Normal && _hoverMode == 0)
                    {
                        Vec2 vec2 = new Vec2(thing.width, thing.height);
                        vec2.x += 4f;
                        vec2.y += 4f;
                        if (vec2.x < 32.0)
                            vec2.x = 32f;
                        if (vec2.y < 32.0)
                            vec2.y = 32f;
                        Vec2 p1 = new Vec2(19f, (float)(layer.height - 19.0 - vec2.y - (num24 + 10.0)) + num22);
                        string str10 = thing.GetDetailsString();
                        while (str10.Count(x => x == '\n') > 5)
                            str10 = str10.Substring(0, str10.LastIndexOf('\n'));
                        float x2 = _font.GetWidth(str10) + 8f;
                        if (str10 != "")
                            _font.Draw(str10, (float)(p1.x + vec2.x + 4.0), p1.y + 4f, Color.White, 0.7f);
                        else
                            x2 = 0f;
                        Graphics.DrawRect(p1, p1 + vec2 + new Vec2(x2, 0f), Color.Black * 0.5f, 0.6f);
                        Vec2 position = thing.position;
                        Depth depth = thing.depth;
                        editorDraw = true;
                        thing.left = p1.x + (float)(vec2.x / 2.0 - thing.w / 2.0);
                        thing.top = p1.y + (float)(vec2.y / 2.0 - thing.h / 2.0);
                        thing.depth = 0.7f;
                        thing.Draw();
                        editorDraw = false;
                        thing.position = position;
                        thing.depth = depth;
                        _font.Draw("Hovering (" + thing.editorName + ")", p1.x, p1.y - 6f, Color.White);
                    }
                }
                else if (_hoverButton != null)
                {
                    string hoverText = _hoverButton.hoverText;
                    if (hoverText != null)
                    {
                        float width = _font.GetWidth(hoverText);
                        Vec2 vec2 = new Vec2(layer.width - 28f - width, layer.height - 28f);
                        _font.depth = 0.8f;
                        _font.Draw(hoverText, vec2.x, vec2.y, Color.White, 0.8f);
                        Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(width + 2f, 9f),
                            Color.Black * 0.5f, 0.6f);
                    }
                }

                _font.scale = new Vec2(1f, 1f);
            }
            else
            {
                if (layer != _objectMenuLayer)
                    return;
                if (inputMode == EditorInput.Mouse)
                {
                    _cursor.depth = 1f;
                    _cursor.scale = new Vec2(1f, 1f);
                    _cursor.position = Mouse.position;
                    if (_cursorMode == CursorMode.Normal)
                        _cursor.frame = 0;
                    else if (_cursorMode == CursorMode.DragHover)
                        _cursor.frame = 1;
                    else if (_cursorMode == CursorMode.Drag)
                        _cursor.frame = 5;
                    else if (_cursorMode == CursorMode.Selection)
                        _cursor.frame = _dragSelectShiftModifier ? 6 : 2;
                    else if (_cursorMode == CursorMode.HasSelection)
                        _cursor.frame = _dragSelectShiftModifier ? 6 : 0;
                    if (hoverTextBox)
                    {
                        _cursor.frame = 7;
                        _cursor.position.y -= 4f;
                        _cursor.scale = new Vec2(0.5f, 1f);
                    }

                    _cursor.Draw();
                }

                if (inputMode != EditorInput.Touch)
                    return;
                if (TouchScreen.GetTouches().Count == 0)
                {
                    Vec2 pos1 = _objectMenuLayer.camera.transformScreenVector(Mouse.positionConsole +
                                                                              new Vec2(TouchScreen._spoofFingerDistance,
                                                                                  0f));
                    Vec2 pos2 = _objectMenuLayer.camera.transformScreenVector(Mouse.positionConsole -
                                                                              new Vec2(TouchScreen._spoofFingerDistance,
                                                                                  0f));
                    Graphics.DrawCircle(pos1, 4f, Color.White * 0.2f, 2f, 1f);
                    Graphics.DrawCircle(pos2, 4f, Color.White * 0.2f, 2f, 1f);
                    Graphics.DrawRect(pos1 + new Vec2(-0.5f, -0.5f), pos1 + new Vec2(0.5f, 0.5f), Color.White,
                        1f);
                    Graphics.DrawRect(pos2 + new Vec2(-0.5f, -0.5f), pos2 + new Vec2(0.5f, 0.5f), Color.White,
                        1f);
                }
                else
                {
                    foreach (Touch touch in TouchScreen.GetTouches())
                        Graphics.DrawCircle(touch.Transform(_objectMenuLayer.camera), 4f, Color.White, 2f,
                            1f);
                }
            }
        }

        public override void StartDrawing()
        {
            if (_procTarget == null)
                _procTarget = new RenderTarget2D(Graphics.width, Graphics.height);
            if (_procContext == null)
                return;
            _procContext.Draw(_procTarget, current.camera, _procDrawOffset);
        }

        public void CloseMenu() => _closeMenu = true;

        public void DoSave(string saveName)
        {
            _saveName = saveName;
            if (!_saveName.EndsWith(".lev"))
                _saveName += ".lev";
            Save();
        }

        //private void onLoad(object sender, CancelEventArgs e)
        //{
        //    if (e.Cancel)
        //        return;
        //    string fileName = this._loadForm.FileName;
        //    this._saveName = fileName;
        //    IEnumerable<DXMLNode> source = DuckXML.Load(fileName).Element("Level").Elements("Objects");
        //    if (source == null)
        //        return;
        //    this.ClearEverything();
        //    foreach (DXMLNode element in source.Elements<DXMLNode>("Object"))
        //        this.AddObject(Thing.LegacyLoadThing(element));
        //}

        public void LoadLevel(string load)
        {
            load = load.Replace('\\', '/');
            while (load.StartsWith("/") && (!Program.IsLinuxD || !Path.IsPathRooted(load)))
                load = load.Substring(1);
            ClearEverything();
            _saveName = load;
            _currentLevelData = DuckFile.LoadLevel(load);
            Thing.loadingLevel = _currentLevelData;
            if (_currentLevelData == null)
            {
                _currentLevelData = new LevelData();
                Thing.loadingLevel = null;
            }
            else
            {
                _currentLevelData.SetPath(_saveName);
                if (_currentLevelData.metaData.guid == null || !editingContent &&
                    Content.GetLevel(_currentLevelData.metaData.guid, LevelLocation.Content) != null)
                    _currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                _onlineSettingChanged = true;
                if (_currentLevelData.customData != null)
                {
                    if (_currentLevelData.customData.customTileset01Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customTileset01Data.GetTileData(), 0,
                            CustomType.Block);
                    if (_currentLevelData.customData.customTileset02Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customTileset02Data.GetTileData(), 1,
                            CustomType.Block);
                    if (_currentLevelData.customData.customTileset03Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customTileset03Data.GetTileData(), 2,
                            CustomType.Block);
                    if (_currentLevelData.customData.customBackground01Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customBackground01Data.GetTileData(),
                            0, CustomType.Background);
                    if (_currentLevelData.customData.customBackground02Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customBackground02Data.GetTileData(),
                            1, CustomType.Background);
                    if (_currentLevelData.customData.customBackground03Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customBackground03Data.GetTileData(),
                            2, CustomType.Background);
                    if (_currentLevelData.customData.customPlatform01Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customPlatform01Data.GetTileData(),
                            0, CustomType.Platform);
                    if (_currentLevelData.customData.customPlatform02Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customPlatform02Data.GetTileData(),
                            1, CustomType.Platform);
                    if (_currentLevelData.customData.customPlatform03Data != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customPlatform03Data.GetTileData(),
                            2, CustomType.Platform);
                    if (_currentLevelData.customData.customParallaxData != null)
                        Custom.ApplyCustomData(_currentLevelData.customData.customParallaxData.GetTileData(), 0,
                            CustomType.Parallax);
                }

                previewCapture = LoadPreview(_currentLevelData.previewData.preview);
                _pathNorth = false;
                _pathSouth = false;
                _pathEast = false;
                _pathWest = false;
                _miniMode = false;
                int sideMask = _currentLevelData.proceduralData.sideMask;
                if ((sideMask & 1) != 0)
                    _pathNorth = true;
                if ((sideMask & 2) != 0)
                    _pathEast = true;
                if ((sideMask & 4) != 0)
                    _pathSouth = true;
                if ((sideMask & 8) != 0)
                    _pathWest = true;
                if (sideMask != 0)
                    _miniMode = true;
                _loadingLevel = true;
                LoadObjects(false);
                LoadObjects(true);
                _loadingLevel = false;
                _editorLoadFinished = true;
                if (!_looseClear)
                    CenterView();
                hasUnsavedChanges = false;
                Thing.loadingLevel = null;
            }
        }

        public void LoadObjects(bool pAlternate)
        {
            foreach (BinaryClassChunk node in pAlternate
                         ? _currentLevelData.proceduralData.openAirAlternateObjects.objects
                         : _currentLevelData.objects.objects)
            {
                Thing pThing = Thing.LoadThing(node);
                if (!Thing.CheckForBozoData(pThing) && pThing != null && pThing.editorCanModify)
                {
                    if (pAlternate)
                    {
                        pThing.active = false;
                        if (pThing is ThingContainer)
                        {
                            ThingContainer thingContainer = pThing as ThingContainer;
                            if (thingContainer.bozocheck)
                            {
                                foreach (Thing thing in thingContainer.things)
                                {
                                    if (!Thing.CheckForBozoData(thing))
                                        _levelThingsAlternate.Add(thing);
                                }
                            }
                            else
                            {
                                foreach (Thing thing in thingContainer.things)
                                    _levelThingsAlternate.Add(thing);
                            }
                        }
                        else
                            _levelThingsAlternate.Add(pThing);
                    }
                    else
                        AddObject(pThing);
                }
            }
        }

        public void LegacyLoadLevel(string load)
        {
            load = load.Replace('\\', '/');
            while (load.StartsWith("/") && (!Program.IsLinuxD || !Path.IsPathRooted(load)))
                load = load.Substring(1);
            DuckXML doc = _additionalSaveDirectory != null ? DuckXML.Load(load) : DuckFile.LoadDuckXML(load);
            _saveName = load;
            LegacyLoadLevelParts(doc);
            hasUnsavedChanges = false;
        }

        public void LegacyLoadLevelParts(DuckXML doc)
        {
            hadGUID = false;
            ClearEverything();
            DXMLNode e = doc.Element("Level");
            DXMLNode dxmlNode1 = e.Element("ID");
            if (dxmlNode1 != null)
            {
                _currentLevelData.metaData.guid = dxmlNode1.Value;
                hadGUID = true;
            }

            DXMLNode dxmlNode2 = e.Element("ONLINE");
            _currentLevelData.metaData.onlineMode = dxmlNode2 != null && Convert.ToBoolean(dxmlNode2.Value);
            previewCapture = LegacyLoadPreview(e);
            _pathNorth = false;
            _pathSouth = false;
            _pathEast = false;
            _pathWest = false;
            _miniMode = false;
            DXMLNode dxmlNode3 = e.Element("PathMask");
            if (dxmlNode3 != null)
            {
                int int32 = Convert.ToInt32(dxmlNode3.Value);
                if ((int32 & 1) != 0)
                    _pathNorth = true;
                if ((int32 & 2) != 0)
                    _pathEast = true;
                if ((int32 & 4) != 0)
                    _pathSouth = true;
                if ((int32 & 8) != 0)
                    _pathWest = true;
                if (int32 != 0)
                    _miniMode = true;
            }

            DXMLNode dxmlNode4 = e.Element("workshopID");
            if (dxmlNode4 != null)
                _currentLevelData.metaData.workshopID = Convert.ToUInt64(dxmlNode4.Value);
            DXMLNode dxmlNode5 = e.Element("workshopName");
            if (dxmlNode5 != null)
                _currentLevelData.workshopData.name = dxmlNode5.Value;
            DXMLNode dxmlNode6 = e.Element("workshopDescription");
            if (dxmlNode6 != null)
                _currentLevelData.workshopData.description = dxmlNode6.Value;
            DXMLNode dxmlNode7 = e.Element("workshopVisibility");
            if (dxmlNode7 != null)
                _currentLevelData.workshopData.visibility =
                    (RemoteStoragePublishedFileVisibility)Convert.ToInt32(dxmlNode7.Value);
            DXMLNode dxmlNode8 = e.Element("workshopTags");
            if (dxmlNode8 != null)
            {
                string[] source = dxmlNode8.Value.Split('|');
                _currentLevelData.workshopData.tags = new List<string>();
                if (source.Count() != 0 && source[0] != "")
                    _currentLevelData.workshopData.tags = source.ToList();
            }

            DXMLNode dxmlNode9 = e.Element("Chance");
            if (dxmlNode9 != null)
                _currentLevelData.proceduralData.chance = Convert.ToSingle(dxmlNode9.Value);
            DXMLNode dxmlNode10 = e.Element("MaxPerLev");
            if (dxmlNode10 != null)
                _currentLevelData.proceduralData.maxPerLevel = Convert.ToInt32(dxmlNode10.Value);
            DXMLNode dxmlNode11 = e.Element("Single");
            if (dxmlNode11 != null)
                _currentLevelData.proceduralData.enableSingle = Convert.ToBoolean(dxmlNode11.Value);
            DXMLNode dxmlNode12 = e.Element("Multi");
            if (dxmlNode12 != null)
                _currentLevelData.proceduralData.enableMulti = Convert.ToBoolean(dxmlNode12.Value);
            DXMLNode dxmlNode13 = e.Element("CanMirror");
            if (dxmlNode13 != null)
                _currentLevelData.proceduralData.canMirror = Convert.ToBoolean(dxmlNode13.Value);
            DXMLNode dxmlNode14 = e.Element("IsMirrored");
            if (dxmlNode14 != null)
                _currentLevelData.proceduralData.isMirrored = Convert.ToBoolean(dxmlNode14.Value);
            _loadingLevel = true;
            IEnumerable<DXMLNode> source1 = e.Elements("Objects");
            if (source1 != null)
            {
                foreach (DXMLNode element in source1.Elements("Object"))
                    AddObject(Thing.LegacyLoadThing(element));
            }

            _loadingLevel = false;
            _editorLoadFinished = true;
            if (_looseClear)
                return;
            CenterView();
        }

        private void CenterView()
        {
            camera.width = _gridW * 16;
            camera.height = camera.width / Resolution.current.aspect;
            camera.centerX = (float)(camera.width / 2.0 - 8.0);
            camera.centerY = (float)(camera.height / 2.0 - 8.0);
            float width = camera.width;
            float height = camera.height;
            camera.width *= 0.3f;
            camera.height *= 0.3f;
            camera.centerX -= (float)((camera.width - width) / 2.0);
            camera.centerY -= (float)((camera.height - height) / 2.0);
            if (_sizeRestriction.x <= 0.0)
                return;
            camera.center = (_topLeftMost + _bottomRightMost) / 2f;
        }

        public static Texture2D LoadPreview(string s)
        {
            try
            {
                return s != null
                    ? Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(s)))
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public static Texture2D LegacyLoadPreview(DXMLNode e)
        {
            try
            {
                DXMLNode dxmlNode = e.Element("Preview");
                return dxmlNode != null
                    ? Texture2D.FromStream(Graphics.device,
                        new MemoryStream(Convert.FromBase64String(dxmlNode.Value)))
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public static string LegacyLoadPreviewString(DXMLNode e)
        {
            try
            {
                return e.Element("Preview")?.Value;
            }
            catch
            {
                return null;
            }
        }

        public static string ScriptToString(byte[] scriptData)
        {
            try
            {
                return Convert.ToBase64String(scriptData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] StringToScript(string script)
        {
            try
            {
                return Convert.FromBase64String(script);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string BytesToString(byte[] pData)
        {
            try
            {
                return Convert.ToBase64String(pData);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static byte[] StringToBytes(string pString)
        {
            try
            {
                return Convert.FromBase64String(pString);
            }
            catch
            {
                return null;
            }
        }

        public static string TextureToString(Texture2D tex)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                tex.SaveAsPng(memoryStream, tex.Width, tex.Height);
                memoryStream.Flush();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception)
            {
                return
                    "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAPUExURQAAAGwXbeBu4P///8AgLYwkid8AAAC9SURBVDhPY2RgYPgPxGQDsAE54rkQHhCUhBdDWRDQs7IXyoIAZHmFSQoMTFA2BpCfKA/Gk19MAmNcAKsBII0HFfVQMC5DwF54kPcAwgMCmGZswP7+JYZciTwoj4FhysvJuL0AAiANIIwPYBgAsgGmEdk2XACrC0AaidEMAnijETk8YC4iKRrRNWMDeAORGIDTgIf5D4kKTIx0AEu6oISD7AWQgSCAnLQJpgNiAE4DQM6GeQFmOzZAYXZmYAAAEzJYPzQv17kAAAAASUVORK5CYII=";
            }
        }

        public static Texture2D StringToTexture(string tex)
        {
            if (string.IsNullOrWhiteSpace(tex))
                return null;
            try
            {
                return Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(tex)));
            }
            catch
            {
                return null;
            }
        }

        public static string TextureToMassiveBitmapString(Texture2D tex)
        {
            Color[] colorArray = new Color[tex.Width * tex.Height];
            tex.GetData(colorArray);
            return TextureToMassiveBitmapString(colorArray, tex.Width, tex.Height);
        }

        public static string TextureToMassiveBitmapString(Color[] colors, int width, int height)
        {
            try
            {
                BitBuffer bitBuffer = new BitBuffer(false);
                bitBuffer.Write(kMassiveBitmapStringHeader);
                bitBuffer.Write(width);
                bitBuffer.Write(height);
                bool flag = false;
                Color color1 = new Color();
                int val = 0;
                foreach (Color color2 in colors)
                {
                    if (!flag || color1 != color2)
                    {
                        color1 = color2;
                        if (flag)
                        {
                            bitBuffer.Write(val);
                            val = 0;
                        }

                        bitBuffer.WriteRGBColor(color2);
                        flag = true;
                    }

                    ++val;
                }

                bitBuffer.Write(val);
                return Convert.ToBase64String(bitBuffer.GetBytes());
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static Texture2D MassiveBitmapStringToTexture(string pTexture)
        {
            try
            {
                BitBuffer bitBuffer = new BitBuffer(Convert.FromBase64String(pTexture));
                if (bitBuffer.lengthInBytes < 8)
                    throw new Exception("(Editor.MassiveBitmapStringToTexture) Preview texture is empty...");
                Texture2D texture = bitBuffer.ReadLong() == kMassiveBitmapStringHeader
                    ? new Texture2D(Graphics.device, bitBuffer.ReadInt(), bitBuffer.ReadInt())
                    : throw new Exception("(Editor.MassiveBitmapStringToTexture) Header was invalid.");
                Color[] data = new Color[texture.Width * texture.Height];
                int index1 = 0;
                do
                {
                    Color color = bitBuffer.ReadRGBColor();
                    int num = bitBuffer.ReadInt();
                    for (int index2 = 0; index2 < num; ++index2)
                    {
                        data[index1] = color;
                        ++index1;
                    }
                } while (bitBuffer.position != bitBuffer.lengthInBytes);

                texture.SetData(data);
                return texture;
            }
            catch
            {
                return null;
            }
        }

        public LevelData CreateSaveData(bool isTempSaveForPlayTestMode = false)
        {
            Level currentLevel = core.currentLevel;
            core.currentLevel = this;
            _currentLevelData.SetExtraHeaderInfo(new LevelMetaData());
            _currentLevelData.Header<LevelMetaData>().type = GetLevelType();
            _currentLevelData.Header<LevelMetaData>().size = GetLevelSize();
            _currentLevelData.Header<LevelMetaData>().online = LevelIsOnlineCapable();
            _currentLevelData.Header<LevelMetaData>().guid = _currentLevelData.metaData.guid;
            _currentLevelData.Header<LevelMetaData>().workshopID = _currentLevelData.metaData.workshopID;
            _currentLevelData.Header<LevelMetaData>().deathmatchReady =
                _currentLevelData.metaData.deathmatchReady;
            _currentLevelData.Header<LevelMetaData>().onlineMode = _currentLevelData.metaData.onlineMode;
            _currentLevelData.RerouteMetadata(_currentLevelData.Header<LevelMetaData>());
            _currentLevelData.metaData.hasCustomArt = false;
            CustomTileData data1 = Custom.GetData(0, CustomType.Block);
            if (data1 != null && data1.path != null && data1.texture != null)
            {
                data1.ApplyToChunk(_currentLevelData.customData.customTileset01Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customTileset01Data.ignore = false;
            }
            else
                _currentLevelData.customData.customTileset01Data.ignore = true;

            CustomTileData data2 = Custom.GetData(1, CustomType.Block);
            if (data2 != null && data2.path != null && data2.texture != null)
            {
                data2.ApplyToChunk(_currentLevelData.customData.customTileset02Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customTileset02Data.ignore = false;
            }
            else
                _currentLevelData.customData.customTileset02Data.ignore = true;

            CustomTileData data3 = Custom.GetData(2, CustomType.Block);
            if (data3 != null && data3.path != null && data3.texture != null)
            {
                data3.ApplyToChunk(_currentLevelData.customData.customTileset03Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customTileset03Data.ignore = false;
            }
            else
                _currentLevelData.customData.customTileset03Data.ignore = true;

            CustomTileData data4 = Custom.GetData(0, CustomType.Background);
            if (data4 != null && data4.path != null && data4.texture != null)
            {
                data4.ApplyToChunk(_currentLevelData.customData.customBackground01Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customBackground01Data.ignore = false;
            }
            else
                _currentLevelData.customData.customBackground01Data.ignore = true;

            CustomTileData data5 = Custom.GetData(1, CustomType.Background);
            if (data5 != null && data5.path != null && data5.texture != null)
            {
                data5.ApplyToChunk(_currentLevelData.customData.customBackground02Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customBackground02Data.ignore = false;
            }
            else
                _currentLevelData.customData.customBackground02Data.ignore = true;

            CustomTileData data6 = Custom.GetData(2, CustomType.Background);
            if (data6 != null && data6.path != null && data6.texture != null)
            {
                data6.ApplyToChunk(_currentLevelData.customData.customBackground03Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customBackground03Data.ignore = false;
            }
            else
                _currentLevelData.customData.customBackground03Data.ignore = true;

            CustomTileData data7 = Custom.GetData(0, CustomType.Platform);
            if (data7 != null && data7.path != null && data7.texture != null)
            {
                data7.ApplyToChunk(_currentLevelData.customData.customPlatform01Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customPlatform01Data.ignore = false;
            }
            else
                _currentLevelData.customData.customPlatform01Data.ignore = true;

            CustomTileData data8 = Custom.GetData(1, CustomType.Platform);
            if (data8 != null && data8.path != null && data8.texture != null)
            {
                data8.ApplyToChunk(_currentLevelData.customData.customPlatform02Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customPlatform02Data.ignore = false;
            }
            else
                _currentLevelData.customData.customPlatform02Data.ignore = true;

            CustomTileData data9 = Custom.GetData(2, CustomType.Platform);
            if (data9 != null && data9.path != null && data9.texture != null)
            {
                data9.ApplyToChunk(_currentLevelData.customData.customPlatform03Data);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customPlatform03Data.ignore = false;
            }
            else
                _currentLevelData.customData.customPlatform03Data.ignore = true;

            CustomTileData data10 = Custom.GetData(0, CustomType.Parallax);
            if (data10 != null && data10.path != null && data10.texture != null)
            {
                data10.ApplyToChunk(_currentLevelData.customData.customParallaxData);
                _currentLevelData.metaData.hasCustomArt = true;
                _currentLevelData.customData.customParallaxData.ignore = false;
            }
            else
                _currentLevelData.customData.customParallaxData.ignore = true;

            _currentLevelData.modData.workshopIDs.Clear();
            if (_things.Count > 0)
            {
                HashSet<Mod> modSet = new HashSet<Mod>();
                foreach (Thing levelThing in levelThings)
                {
                    modSet.Add(ModLoader.GetModFromType(levelThing.GetType()));
                    if (levelThing is IContainThings)
                    {
                        IContainThings containThings = (IContainThings)levelThing;
                        if (containThings.contains != null)
                        {
                            foreach (Type contain in containThings.contains)
                                modSet.Add(ModLoader.GetModFromType(contain));
                        }
                    }
                    else if (levelThing is IContainAThing)
                    {
                        IContainAThing containAthing = (IContainAThing)levelThing;
                        if (containAthing.contains != null)
                            modSet.Add(ModLoader.GetModFromType(containAthing.contains));
                    }
                }

                modSet.RemoveWhere(a =>
                {
                    switch (a)
                    {
                        case null:
                        case CoreMod _:
                            return true;
                        default:
                            return a is DisabledMod;
                    }
                });
                if (modSet.Count != 0)
                {
                    foreach (Mod mod in modSet)
                    {
                        if (mod.configuration.workshopID != 0UL || mod.workshopIDFacade != 0UL)
                            _currentLevelData.modData.workshopIDs.Add(mod.workshopIDFacade != 0UL
                                ? mod.workshopIDFacade
                                : mod.configuration.workshopID);
                        else
                            _currentLevelData.modData.hasLocalMods = true;
                    }
                }
            }

            string str1 = "";
            string str2 = "";
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            _currentLevelData.metaData.eightPlayer = false;
            _currentLevelData.metaData.eightPlayerRestricted = false;
            _currentLevelData.objects.objects.Clear();
            if (_levelThings.Count > 0)
            {
                MultiMap<Type, Thing> multiMap = new MultiMap<Type, Thing>();
                foreach (Thing levelThing in _levelThings)
                {
                    if (levelThing is EightPlayer)
                    {
                        _currentLevelData.metaData.eightPlayer = true;
                        _currentLevelData.metaData.eightPlayerRestricted =
                            (levelThing as EightPlayer).eightPlayerOnly.value;
                    }

                    if (levelThing.editorCanModify && !levelThing.processedByEditor)
                    {
                        if (_miniMode)
                        {
                            switch (levelThing)
                            {
                                case Key _:
                                    ++num6;
                                    break;
                                case Door _ when (levelThing as Door).locked:
                                    ++num5;
                                    break;
                                case Equipment _:
                                    if (levelThing is ChestPlate || levelThing is Helmet || levelThing is KnightHelmet)
                                    {
                                        ++num1;
                                        break;
                                    }

                                    ++num2;
                                    break;
                                case Gun _:
                                    if (str1 != "")
                                        str1 += "|";
                                    str1 += ModLoader.SmallTypeName(levelThing.GetType());
                                    break;
                                case ItemSpawner _:
                                    ItemSpawner itemSpawner = levelThing as ItemSpawner;
                                    if (typeof(Gun).IsAssignableFrom(itemSpawner.contains) &&
                                        itemSpawner.likelyhoodToExist == 1.0 && !itemSpawner.randomSpawn)
                                    {
                                        if (itemSpawner.spawnNum < 1 && itemSpawner.spawnTime < 8.0 &&
                                            itemSpawner.isAccessible)
                                        {
                                            if (str2 != "")
                                                str2 += "|";
                                            str2 += ModLoader.SmallTypeName(itemSpawner.contains);
                                        }

                                        if (str1 != "")
                                            str1 += "|";
                                        str1 += ModLoader.SmallTypeName(itemSpawner.contains);
                                    }

                                    break;
                                default:
                                    if (levelThing.GetType() == typeof(ItemBox))
                                    {
                                        ItemBox itemBox = levelThing as ItemBox;
                                        if (typeof(Gun).IsAssignableFrom(itemBox.contains) &&
                                            itemBox.likelyhoodToExist == 1.0 && itemBox.isAccessible)
                                        {
                                            if (str2 != "")
                                                str2 += "|";
                                            str2 += ModLoader.SmallTypeName(itemBox.contains);
                                            if (str1 != "")
                                                str1 += "|";
                                            str1 += ModLoader.SmallTypeName(itemBox.contains);
                                        }
                                    }
                                    else if (levelThing is SpawnPoint)
                                    {
                                        ++num3;
                                    }
                                    else if (levelThing is TeamSpawn)
                                    {
                                        ++num4;
                                    }

                                    break;
                            }
                        }

                        levelThing.processedByEditor = true;
                    }
                }
            }

            SerializeObjects(false);
            SerializeObjects(true);
            _currentLevelData.proceduralData.sideMask = 0;
            if (_miniMode)
            {
                int num7 = 0;
                if (_pathNorth)
                    num7 |= 1;
                if (_pathEast)
                    num7 |= 2;
                if (_pathSouth)
                    num7 |= 4;
                if (_pathWest)
                    num7 |= 8;
                _currentLevelData.proceduralData.sideMask = num7;
                _currentLevelData.proceduralData.weaponConfig = str1;
                _currentLevelData.proceduralData.spawnerConfig = str2;
                _currentLevelData.proceduralData.numArmor = num1;
                _currentLevelData.proceduralData.numEquipment = num2;
                _currentLevelData.proceduralData.numSpawns = num3;
                _currentLevelData.proceduralData.numTeamSpawns = num4;
                _currentLevelData.proceduralData.numLockedDoors = num5;
                _currentLevelData.proceduralData.numKeys = num6;
            }

            if (previewCapture != null)
                _currentLevelData.previewData.preview = TextureToString(previewCapture);
            try
            {
                Content.doingTempSave = isTempSaveForPlayTestMode;
                Content.GeneratePreview(_currentLevelData, !isTempSaveForPlayTestMode);
                Content.doingTempSave = false;
            }
            catch (Exception)
            {
                DevConsole.Log(DCSection.General,
                    "Error creating preview for level " + _currentLevelData.metaData.guid);
            }

            LevelData saveData = _currentLevelData.Clone();
            saveData.RerouteMetadata(saveData.Header<LevelMetaData>());
            if (isTempSaveForPlayTestMode)
                saveData.metaData.guid = "tempPlayLevel";
            core.currentLevel = currentLevel;
            return saveData;
        }

        public void SerializeObjects(bool pAlternate)
        {
            List<BinaryClassChunk> binaryClassChunkList = pAlternate
                ? _currentLevelData.proceduralData.openAirAlternateObjects.objects
                : _currentLevelData.objects.objects;
            List<Thing> thingList = pAlternate ? _levelThingsAlternate : _levelThingsNormal;
            binaryClassChunkList.Clear();
            if (thingList.Count <= 0)
                return;
            foreach (Thing thing in thingList)
                thing.processedByEditor = false;
            MultiMap<Type, Thing> multiMap = new MultiMap<Type, Thing>();
            foreach (Thing element in thingList)
            {
                if (element.editorCanModify && !element.processedByEditor)
                {
                    element.processedByEditor = true;
                    if (element.canBeGrouped)
                        multiMap.Add(element.GetType(), element);
                    else
                        binaryClassChunkList.Add(element.Serialize());
                }
            }

            foreach (KeyValuePair<Type, List<Thing>> keyValuePair in
                     multiMap)
                binaryClassChunkList.Add(new ThingContainer(keyValuePair.Value, keyValuePair.Key)
                {
                    quickSerialize = minimalConversionLoad
                }.Serialize());
        }

        public bool Save(bool isTempSaveForPlayTestMode = false)
        {
            if (_saveName == "")
            {
                SaveAs();
            }
            else
            {
                saving = true;
                LevelData saveData = CreateSaveData(isTempSaveForPlayTestMode);
                saveData.customData.customTileset01Data.ignore =
                    _currentLevelData.customData.customTileset01Data.ignore;
                saveData.customData.customTileset02Data.ignore =
                    _currentLevelData.customData.customTileset02Data.ignore;
                saveData.customData.customTileset03Data.ignore =
                    _currentLevelData.customData.customTileset03Data.ignore;
                saveData.customData.customBackground01Data.ignore =
                    _currentLevelData.customData.customBackground01Data.ignore;
                saveData.customData.customBackground02Data.ignore =
                    _currentLevelData.customData.customBackground02Data.ignore;
                saveData.customData.customBackground03Data.ignore =
                    _currentLevelData.customData.customBackground03Data.ignore;
                saveData.customData.customPlatform01Data.ignore =
                    _currentLevelData.customData.customPlatform01Data.ignore;
                saveData.customData.customPlatform02Data.ignore =
                    _currentLevelData.customData.customPlatform02Data.ignore;
                saveData.customData.customPlatform03Data.ignore =
                    _currentLevelData.customData.customPlatform03Data.ignore;
                saveData.customData.customParallaxData.ignore =
                    _currentLevelData.customData.customParallaxData.ignore;
                saveData.SetPath(_saveName);
                if (!DuckFile.SaveChunk(saveData, _saveName))
                {
                    _notify.Open("Could not save data.");
                    return false;
                }

                if (!isTempSaveForPlayTestMode)
                    _currentLevelData.SetPath(_saveName);
                Content.MapLevel(saveData.metaData.guid, saveData, LevelLocation.Custom);
                if (_additionalSaveDirectory != null && _saveName.LastIndexOf("assets/levels/") != -1)
                {
                    string str1 =
                        _saveName.Substring(_saveName.LastIndexOf("assets/levels/") + "assets/levels/".Length);
                    string str2 = Directory.GetCurrentDirectory() + "/Content/levels/" + str1;
                    DuckFile.CreatePath(str2);
                    File.Copy(_saveName, str2, true);
                    File.SetAttributes(_saveName, FileAttributes.Normal);
                }

                if (_miniMode && !_doingResave)
                    LevelGenerator.ReInitialize();
                foreach (Thing levelThing in _levelThings)
                    levelThing.processedByEditor = false;
                saving = false;
                if (!isTempSaveForPlayTestMode)
                    hasUnsavedChanges = false;
            }

            return true;
        }

        public static LevelMetaData ReadLevelMetadata(byte[] pData, bool pNewMetadataOnly = false)
        {
            try
            {
                LevelData levelData1 = DuckFile.LoadLevel(pData, true);
                if (levelData1.GetExtraHeaderInfo() != null && levelData1.GetExtraHeaderInfo() is LevelMetaData)
                    return levelData1.GetExtraHeaderInfo() as LevelMetaData;
                if (pNewMetadataOnly)
                    return null;
                LevelData levelData2 = DuckFile.LoadLevel(pData, false);
                if (levelData2 != null)
                    return levelData2.metaData;
            }
            catch (Exception)
            {
            }

            DevConsole.Log(DCSection.General, "Editor failed loading metadata from byte[].");
            return null;
        }

        public static LevelMetaData ReadLevelMetadata(string pFile, bool pNewMetadataOnly = false)
        {
            try
            {
                LevelData levelData1 = DuckFile.LoadLevel(pFile, true);
                if (levelData1.GetExtraHeaderInfo() != null && levelData1.GetExtraHeaderInfo() is LevelMetaData)
                    return levelData1.GetExtraHeaderInfo() as LevelMetaData;
                if (pNewMetadataOnly)
                    return null;
                LevelData levelData2 = DuckFile.LoadLevel(pFile, false);
                if (levelData2 != null)
                    return levelData2.metaData;
            }
            catch (Exception)
            {
            }

            DevConsole.Log(DCSection.General, "Editor failed loading metadata from level (" + pFile + ")");
            return null;
        }

        public static LevelMetaData ReadLevelMetadata(LevelData pData)
        {
            if (pData == null)
                return null;
            try
            {
                return pData.GetExtraHeaderInfo() != null && pData.GetExtraHeaderInfo() is LevelMetaData
                    ? pData.GetExtraHeaderInfo() as LevelMetaData
                    : pData.metaData;
            }
            catch (Exception)
            {
            }

            DevConsole.Log(DCSection.General, "Editor failed loading metadata from level data.");
            return null;
        }

        public static void Delete(string file)
        {
            file = file.Replace('\\', '/');
            while (file.StartsWith("/") && (!Program.IsLinuxD || !Path.IsPathRooted(file)))
                file = file.Substring(1);
            string path = "";
            LevelMetaData data = ReadLevelMetadata(file);
            if (data != null)
            {
                activatedLevels.RemoveAll(x => x == data.guid);
                path = DuckFile.editorPreviewDirectory + data.guid;
            }

            File.SetAttributes(file, FileAttributes.Normal);
            DuckFile.Delete(file);
            if (!File.Exists(path))
                return;
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        public void SaveAs()
        {
            _fileDialog.Open(_initialDirectory, _initialDirectory, true);
            DoMenuClose();
            _closeMenu = false;
        }

        public void Load()
        {
            _fileDialog.Open(_initialDirectory, _initialDirectory, false);
            DoMenuClose();
            _closeMenu = false;
        }

        public string SaveTempVersion()
        {
            string saveName = _saveName;
            string str = Directory.GetCurrentDirectory() + "/Content/_tempPlayLevel.lev";
            _saveName = str;
            Save(true);
            _saveName = saveName;
            return str;
        }

        public void Play()
        {
            if (!_runLevelAnyway && !arcadeMachineMode && _levelThings.FirstOrDefault(x =>
                {
                    switch (x)
                    {
                        case FreeSpawn _:
                        case TeamSpawn _:
                            return true;
                        default:
                            return x is CustomCamera;
                    }
                }) == null)
            {
                CloseMenu();
                ShowNoSpawnsDialogue();
            }
            else
            {
                isTesting = true;
                string name;
                if (_miniMode && _procContext != null)
                {
                    LevelGenerator.ReInitialize();
                    _centerTile = LevelGenerator.LoadInTile(SaveTempVersion());
                    name = "RANDOM";
                }
                else
                    name = SaveTempVersion();

                CloseMenu();
                RunTestLevel(name);
            }
        }

        public virtual void RunTestLevel(string name)
        {
            isTesting = true;
            current = new TestArea(this, name, _procSeed, _centerTile);
            current.AddThing(new EditorTestLevel(this));
        }

        public static MemoryStream GetCompressedActiveLevelData()
        {
            MemoryStream compressedActiveLevelData = new MemoryStream();
            BinaryWriter binaryWriter =
                new BinaryWriter(new GZipStream(compressedActiveLevelData, CompressionMode.Compress));
            foreach (string activatedLevel in activatedLevels)
            {
                binaryWriter.Write(true);
                binaryWriter.Write(activatedLevel);
                byte[] buffer = File.ReadAllBytes(DuckFile.levelDirectory + activatedLevel + ".lev");
                binaryWriter.Write(buffer.Length);
                binaryWriter.Write(buffer);
            }

            binaryWriter.Write(false);
            return compressedActiveLevelData;
        }

        public static MemoryStream GetCompressedLevelData(string level)
        {
            MemoryStream compressedLevelData = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(new GZipStream(compressedLevelData, CompressionMode.Compress));
            binaryWriter.Write(level);
            byte[] buffer = File.ReadAllBytes(DuckFile.levelDirectory + level + ".lev");
            binaryWriter.Write(buffer.Length);
            binaryWriter.Write(buffer);
            return compressedLevelData;
        }

        public static ReceivedLevelInfo ReadCompressedLevelData(MemoryStream stream)
        {
            stream.Position = 0L;
            BinaryReader binaryReader = new BinaryReader(new GZipStream(stream, CompressionMode.Decompress));
            string str = binaryReader.ReadString();
            LevelData levelData = DuckFile.LoadLevel(binaryReader.ReadBytes(binaryReader.ReadInt32()));
            return new ReceivedLevelInfo
            {
                data = levelData,
                name = str
            };
        }

        public static uint Checksum(byte[] data) => CRC32.Generate(data);

        public static uint Checksum(byte[] data, int start, int length) => CRC32.Generate(data, start, length);

        public static Dictionary<Type, Thing> thingMap => _thingMap;

        public static void MapThing(Thing t) => _thingMap[t.GetType()] = t;

        public static Thing GetThing(Type t)
        {
            _thingMap.TryGetValue(t, out Thing thing);
            return thing;
        }

        public static List<ClassMember> GetMembers<T>() => GetMembers(typeof(T));

        public static List<ClassMember> GetMembers(Type t)
        {
            List<ClassMember> members1;
            if (_classMembers.TryGetValue(t, out members1))
                return members1;
            _classMemberNames[t] = new Dictionary<string, ClassMember>();
            List<ClassMember> members2 = new List<ClassMember>();
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties =
                t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                ClassMember classMember = new ClassMember(field.Name, t, field);
                _classMemberNames[t][field.Name] = classMember;
                members2.Add(classMember);
            }

            foreach (PropertyInfo property in properties)
            {
                ClassMember classMember = new ClassMember(property.Name, t, property);
                _classMemberNames[t][property.Name] = classMember;
                members2.Add(classMember);
            }

            _classMembers[t] = members2;
            return members2;
        }

        public static List<ClassMember> GetStaticMembers(Type t)
        {
            List<ClassMember> staticMembers1;
            if (_staticClassMembers.TryGetValue(t, out staticMembers1))
                return staticMembers1;
            _classMemberNames[t] = new Dictionary<string, ClassMember>();
            List<ClassMember> staticMembers2 = new List<ClassMember>();
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties =
                t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                ClassMember classMember = new ClassMember(field.Name, t, field);
                _classMemberNames[t][field.Name] = classMember;
                staticMembers2.Add(classMember);
            }

            foreach (PropertyInfo property in properties)
            {
                ClassMember classMember = new ClassMember(property.Name, t, property);
                _classMemberNames[t][property.Name] = classMember;
                staticMembers2.Add(classMember);
            }

            _staticClassMembers[t] = staticMembers2;
            return staticMembers2;
        }

        public static ClassMember GetMember<T>(string name) => GetMember(typeof(T), name);

        public static ClassMember GetMember(Type t, string name)
        {
            Dictionary<string, ClassMember> dictionary;
            if (!_classMemberNames.TryGetValue(t, out dictionary))
            {
                GetMembers(t);
                if (!_classMemberNames.TryGetValue(t, out dictionary))
                    return null;
            }

            ClassMember member;
            dictionary.TryGetValue(name, out member);
            return member;
        }

        internal static Type GetType(string name) => ModLoader.GetType(name);

        internal static Type DeSerializeTypeName(string serializedTypeName) =>
            serializedTypeName == "" ? null : GetType(serializedTypeName);

        internal static string SerializeTypeName(Type t) => t == null ? "" : ModLoader.SmallTypeName(t);

        public static void CopyClass(object source, object destination)
        {
            foreach (FieldInfo field in source.GetType()
                         .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                field.SetValue(destination, field.GetValue(source));
        }

        public static IEnumerable<Type> GetSubclasses(Type parentType)
        {
            return DG.assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    (type.IsSubclassOf(parentType) &&
                     (clientonlycontent || !type.IsDefined(typeof(ClientOnlyAttribute), false))))
                .OrderBy(t => t.FullName)
                .OrderBy(type => type.IsDefined(typeof(ClientOnlyAttribute), false) ? 1 : 0);
        }

        public static IEnumerable<Type> GetAllSubclasses(Type parentType) // Dan
        {
            return DG.assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(parentType)).OrderBy(t => t.FullName)
                .OrderBy(type => type.IsDefined(typeof(ClientOnlyAttribute), false) ? 1 : 0).ToArray();
        }

        public static IEnumerable<Type> GetSubclassesAndInterfaces(Type parentType) => DG.assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                (parentType.IsAssignableFrom(type) &&
                 (clientonlycontent || !type.IsDefined(typeof(ClientOnlyAttribute), false))))
            .OrderBy(t => t.FullName)
            .OrderBy(type => type.IsDefined(typeof(ClientOnlyAttribute), false) ? 1 : 0).ToArray();

        public static AccessorInfo GetAccessorInfo(
            Type t,
            string name,
            FieldInfo field = null,
            PropertyInfo property = null)
        {
            Dictionary<string, AccessorInfo> dictionary;
            if (_accessorCache.TryGetValue(t, out dictionary))
            {
                AccessorInfo accessorInfo;
                if (dictionary.TryGetValue(name, out accessorInfo))
                    return accessorInfo;
            }
            else
                _accessorCache[t] = new Dictionary<string, AccessorInfo>();

            AccessorInfo accessor = CreateAccessor(field, property, t, name);
            _accessorCache[t][name] = accessor;
            return accessor;
        }

        public static AccessorInfo CreateAccessor(
            FieldInfo field,
            PropertyInfo property,
            Type t,
            string name)
        {
            if (field == null && property == null)
            {
                BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                field = t.GetField(name, bindingAttr);
                if (field == null)
                    property = t.GetProperty(name, bindingAttr);
            }

            AccessorInfo accessor = null;
            if (field != null)
            {
                accessor = new AccessorInfo
                {
                    type = field.FieldType,
                    setAccessor = BuildSetAccessorField(t, field),
                    getAccessor = BuildGetAccessorField(t, field)
                };
            }
            else if (property != null)
            {
                accessor = new AccessorInfo
                {
                    type = property.PropertyType
                };
                MethodInfo setMethod = property.GetSetMethod(true);
                if (setMethod != null)
                    accessor.setAccessor = BuildSetAccessorProperty(t, setMethod);
                accessor.getAccessor = BuildGetAccessorProperty(t, property);
            }

            return accessor;
        }

        public static Action<object, object> BuildSetAccessorProperty(Type t, MethodInfo method)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "o");
            ParameterExpression value = Expression.Parameter(typeof(object));
            return Expression.Lambda<Action<object, object>>(Expression.Call(
                method.IsStatic ? null : Expression.Convert(obj, method.DeclaringType), method, Expression.Convert(value, method.GetParameters()[0].ParameterType)), obj, value).Compile();
        }

        public static Action<object, object> BuildSetAccessorField(Type t, FieldInfo field)
        {
            ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
            ParameterExpression valueExp = Expression.Parameter(typeof(object), "value");
            return Expression.Lambda<Action<object, object>>(
                Expression.Assign(Expression.Field(field.IsStatic ? null : Expression.Convert(targetExp, t), field),
                    Expression.Convert(valueExp, field.FieldType)), targetExp, valueExp).Compile();
        }

        public static Func<object, object> BuildGetAccessorProperty(Type t, PropertyInfo property)
        {
            if (property.GetGetMethod(true) == null)
            {
                return null;
            }

            ParameterExpression obj = Expression.Parameter(typeof(object), "o");
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Property(property.GetGetMethod(true).IsStatic ? null : Expression.Convert(obj, t),
                        property), typeof(object)), obj).Compile();
        }

        public static Func<object, object> BuildGetAccessorField(Type t, FieldInfo field)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "o");
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(Expression.Field(field.IsStatic ? null : Expression.Convert(obj, t), field),
                    typeof(object)), obj).Compile();
        }

        private static object GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

        public static Thing CreateThing(Type t)
        {
            ThingConstructor thingConstructor;
            return _defaultConstructors.TryGetValue(t, out thingConstructor)
                ? thingConstructor()
                : Activator.CreateInstance(t, GetConstructorParameters(t)) as Thing;
        }

        public static Thing CreateThing(Type t, object[] p) => Activator.CreateInstance(t, p) as Thing;

        public static Thing GetOrCreateTypeInstance(Type t)
        {
            Thing typeInstance;
            if (!_thingMap.TryGetValue(t, out typeInstance) && CreateObject(t) is Thing thing)
            {
                _thingMap[t] = thing;
                typeInstance = thing;
            }

            return typeInstance;
        }

        public static object CreateObject(Type t)
        {
            Func<object> func;
            return _constructorParameterExpressions.TryGetValue(t, out func) ? func() : null;
        }

        private static void RegisterEditorFields(Type pType)
        {
            List<FieldInfo> fieldInfoList;
            if (!EditorFieldsForType.TryGetValue(pType, out fieldInfoList))
                fieldInfoList = EditorFieldsForType[pType] = new List<FieldInfo>();
            foreach (Type key in AllBaseTypes[pType])
            {
                if (AllEditorFields.ContainsKey(key))
                    fieldInfoList.AddRange(AllEditorFields[key]);
            }
        }

        public static void InitializeConstructorLists()
        {
            MonoMain.NloadMessage = "Loading Constructor Lists";
            if (MonoMain.moddingEnabled)
            {
                MonoMain.NloadMessage = "Loading Constructor Lists";
                ThingTypes = ManagedContent.Things.AllSortedTypes.ToList();
            }
            else
                ThingTypes = GetAllSubclasses(typeof(Thing)).ToList();

            GroupThingTypes = new List<Type>();
            GroupThingTypes.AddRange(ThingTypes);
            AllBaseTypes = new Dictionary<Type, List<Type>>();
            AllEditorFields = new Dictionary<Type, IEnumerable<FieldInfo>>();
            AllStateFields = new Dictionary<Type, FieldInfo[]>();
            EditorFieldsForType = new Dictionary<Type, List<FieldInfo>>();
            Type editorFieldType = typeof(EditorProperty<>);
            Type stateFieldType = typeof(StateBinding);
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ushort key = 2;
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string str = "";
            foreach (Type thingType in ThingTypes)
            {
                AllBaseTypes[thingType] = Thing.GetAllTypes(thingType);
                FieldInfo[] fields = thingType.GetFields(bindingAttr);
                AllEditorFields[thingType] = fields.Where(val =>
                        val.FieldType.IsGenericType && val.FieldType.GetGenericTypeDefinition() == editorFieldType)
                    .ToArray();
                AllStateFields[thingType] = fields.Where(val => val.FieldType == stateFieldType)
                    .ToArray();
                if (AllStateFields[thingType].Count() > 0)
                {
                    IDToType[key] = thingType;
                    if (thingType.Assembly == executingAssembly &&
                        !thingType.IsDefined(typeof(ClientOnlyAttribute), false))
                        str += thingType.Name;
                    ++key;
                }
            }

            thingTypesHash = CRC32.Generate(str);
            foreach (Type thingType in ThingTypes)
            {
                if (!thingType.IsAbstract)
                {
                    RegisterEditorFields(thingType);
                    foreach (ConstructorInfo constructor in thingType.GetConstructors())
                    {
                        ParameterInfo[] parameters = constructor.GetParameters();
                        if (parameters.Length == 0)
                        {
                            LambdaExpression lambdaExpression = Expression.Lambda(typeof(ThingConstructor),
                                Expression.New(constructor, null), null);
                            _defaultConstructors[thingType] =
                                (ThingConstructor)lambdaExpression.Compile();
                            _constructorParameters[thingType] = new object[0];
                        }
                        else
                        {
                            Expression[] expressionArray = new Expression[parameters.Length];
                            object[] objArray = new object[parameters.Length];
                            int index = 0;
                            foreach (ParameterInfo parameterInfo in parameters)
                            {
                                Type parameterType = parameterInfo.ParameterType;
                                objArray[index] =
                                    parameterInfo.DefaultValue == null ||
                                    !(parameterInfo.DefaultValue.GetType() != typeof(DBNull))
                                        ? GetDefaultValue(parameterType)
                                        : parameterInfo.DefaultValue;
                                expressionArray[index] = Expression.Constant(objArray[index], parameterType);
                                ++index;
                            }

                            LambdaExpression lambdaExpression = Expression.Lambda(typeof(ThingConstructor),
                                Expression.New(constructor, expressionArray), null);
                            _defaultConstructors[thingType] =
                                (ThingConstructor)lambdaExpression.Compile();
                            _constructorParameters[thingType] = objArray;
                        }
                    }

                    ++MonoMain.loadyBits;
                }
            }

            Program.constructorsLoaded = _constructorParameters.Count;
            Program.thingTypes = ThingTypes.Count;
            foreach (Type thingType in ThingTypes)
            {
                foreach (ConstructorInfo constructor in thingType.GetConstructors())
                {
                    ConstructorInfo info = constructor;
                    ParameterInfo[] parameters = info.GetParameters();
                    if (parameters.Length == 0)
                    {
                        _constructorParameterExpressions[thingType] = () => info.Invoke(null);
                    }
                    else
                    {
                        Expression[] expressionArray = new Expression[parameters.Length];
                        int index = 0;
                        object[] vals = new object[parameters.Length];
                        foreach (ParameterInfo parameterInfo in parameters)
                        {
                            Type parameterType = parameterInfo.ParameterType;
                            vals[index] = GetDefaultValue(parameterType);
                            ++index;
                        }

                        _constructorParameterExpressions[thingType] = () => info.Invoke(vals);
                    }
                }

                ++MonoMain.loadyBits;
            }
        }

        public static bool clientonlycontent;

        public static void EnableClientOnlyContent()
        {
            clientonlycontent = true;
        }

        public static void DisableClientOnlyContent()
        {
            clientonlycontent = false;
        }

        public static void InitializePlaceableList()
        {
            if (_placeables != null)
                return;
            InitializeConstructorLists();
            InitializePlaceableGroup();
        }

        public static void InitializePlaceableGroup()
        {
            AutoUpdatables.ignoreAdditions = true;
            MonoMain.NloadMessage = "Loading Editor Groups";
            _placeables = new EditorGroup(null, null);
            AutoUpdatables.ignoreAdditions = false;
            if (!_clearOnce)
            {
                AutoUpdatables.Clear();
                _clearOnce = true;
            }

            _listLoaded = true;
        }

        public static bool HasConstructorParameter(Type t) => _constructorParameters.ContainsKey(t);

        public static object[] GetConstructorParameters(Type t)
        {
            object[] constructorParameters;
            _constructorParameters.TryGetValue(t, out constructorParameters);
            if (constructorParameters == null)
            {
                int num = 0;
                try
                {
                    ThingTypes = !MonoMain.moddingEnabled
                        ? GetAllSubclasses(typeof(Thing)).ToList()
                        : ManagedContent.Things.AllSortedTypes.ToList();
                    num = ThingTypes.Count;
                }
                catch (Exception)
                {
                }

                throw new Exception("Error loading constructor parameters for type " + t + "(" +
                                    _constructorParameters.Count + " parms vs " +
                                    Program.thingTypes + ", " + Program.constructorsLoaded +
                                    ", " + num + " things vs " + Program.thingTypes + ")");
            }

            return constructorParameters;
        }

        private enum EditorTouchState
        {
            Normal,
            OpenMenu,
            Eyedropper,
            EditObject,
            OpenLevel,
            PickTile,
        }

        private class EditorTouchButton
        {
            public string caption;
            public string explanation;
            public Vec2 position;
            public Vec2 size;
            public bool threeFingerGesture;
            public EditorTouchState state;
        }

        private delegate Thing ThingConstructor();
    }
}