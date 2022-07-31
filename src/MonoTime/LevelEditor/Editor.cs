// Decompiled with JetBrains decompiler
// Type: DuckGame.Editor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public class Editor : Level
    {
        public static bool editingContent = false;
        private static Stack<object> focusStack = new Stack<object>();
        private static int numPops = 0;
        private EditorCam _editorCam;
        private SpriteMap _cursor;
        private SpriteMap _tileset;
        private BitmapFont _font;
        private ContextMenu _placementMenu;
        private ContextMenu _objectMenu;
        private CursorMode _cursorMode;
        public static bool active = false;
        public static bool selectingLevel = false;
        private InputType dragModeInputType;
        //private InputType dragStartInputType;
        private Vec2 _sizeRestriction = new Vec2(800f, 640f);
        public static int placementLimit = 0;
        public int placementTotalCost;
        private Vec2 _topLeftMost = new Vec2(99999f, 99999f);
        private Vec2 _bottomRightMost = new Vec2(-99999f, -99999f);
        public static bool hasUnsavedChanges;
        public static Texture2D previewCapture;
        private BinaryClassChunk _eyeDropperSerialized;
        private static Dictionary<System.Type, List<MethodInfo>> _networkActionIndexes = new Dictionary<System.Type, List<MethodInfo>>();
        private static EditorGroup _placeables;
        protected List<Thing> _levelThingsNormal = new List<Thing>();
        protected List<Thing> _levelThingsAlternate = new List<Thing>();
        public static string placementItemDetails = "";
        private string _saveName = "";
        private SaveFileDialog _saveForm = new SaveFileDialog();
        private OpenFileDialog _loadForm = new OpenFileDialog();
        public static bool enteringText = false;
        private static ContextMenu _lockInput;
        private static ContextMenu _lockInputChange;
        private int _lastCommand = -1;
        private List<Command> _commands = new List<Command>();
        public static bool clickedMenu = false;
        public static bool clickedContextBackground = false;
        public bool clicked;
        private bool _updateEvenWhenInactive;
        private bool _editorLoadFinished;
        private NotifyDialogue _notify;
        private bool _placementMode = true;
        private bool _editMode;
        private bool _copyMode;
        public static bool hoverUI = false;
        public static EditorInput inputMode = EditorInput.Gamepad;
        private SpriteMap _editorButtons;
        private bool _loadingLevel;
        private List<Thing> _placeObjects = new List<Thing>();
        public bool minimalConversionLoad;
        private bool processingMirror;
        private bool _isPaste;
        private bool _looseClear;
        public static LevelData _currentLevelData = new LevelData();
        public static bool saving = false;
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
        private static bool _listLoaded = false;
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
        private static InputProfile _input = null;
        private MessageDialogue _noSpawnsDialogue;
        private bool _runLevelAnyway;
        public static bool copying = false;
        private Vec2 _tileDragContext = Vec2.MinValue;
        private Vec2 _tilePositionPrev = Vec2.Zero;
        private Vec2 _tileDragDif = Vec2.Zero;
        private Vec2 _lastTilePosDraw = Vec2.Zero;
        public static bool tookInput = false;
        public static bool didUIScroll = false;
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
        public static bool skipFrame = false;
        private bool firstClick;
        public static Thing openContextThing;
        public static Thing pretendPinned = null;
        public static Vec2 openPosition = Vec2.Zero;
        public static bool ignorePinning = false;
        public static bool reopenContextMenu = false;
        private Vec2 middleClickPos;
        private Vec2 lastMousePos = Vec2.Zero;
        public static string tooltip = null;
        private bool _twoFingerGesture;
        private bool _twoFingerGestureStarting;
        private bool _twoFingerZooming;
        private bool _threeFingerGesture;
        private bool _threeFingerGestureRelease;
        private float _twoFingerSpacing;
        private Editor.EditorTouchButton _activeTouchButton;
        private Editor.EditorTouchState _touchState;
        //private bool _prevTouch;
        private List<Editor.EditorTouchButton> _touchButtons = new List<Editor.EditorTouchButton>();
        private List<Editor.EditorTouchButton> _fileDialogButtons = new List<Editor.EditorTouchButton>();
        private Editor.EditorTouchButton _cancelButton;
        private Editor.EditorTouchButton _editTilesButton;
        public static bool fakeTouch = false;
        public static bool _clickedTouchButton = false;
        private Thing _oldHover;
        public static bool waitForNoTouchInput = false;
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
        public static bool editorDraw = false;
        public static int _procXPos = 1;
        public static int _procYPos = 1;
        public static int _procTilesWide = 3;
        public static int _procTilesHigh = 3;
        public static bool hoverTextBox = false;
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
        public static bool isTesting = false;
        public bool searching;
        private static Dictionary<System.Type, Thing> _thingMap = new Dictionary<System.Type, Thing>();
        private static Dictionary<System.Type, List<ClassMember>> _classMembers = new Dictionary<System.Type, List<ClassMember>>();
        private static Dictionary<System.Type, List<ClassMember>> _staticClassMembers = new Dictionary<System.Type, List<ClassMember>>();
        private static Dictionary<System.Type, Dictionary<string, ClassMember>> _classMemberNames = new Dictionary<System.Type, Dictionary<string, ClassMember>>();
        public static Dictionary<System.Type, Dictionary<string, AccessorInfo>> _accessorCache = new Dictionary<System.Type, Dictionary<string, AccessorInfo>>();
        private static Dictionary<System.Type, object[]> _constructorParameters = new Dictionary<System.Type, object[]>();
        private static Dictionary<System.Type, Editor.ThingConstructor> _defaultConstructors = new Dictionary<System.Type, Editor.ThingConstructor>();
        private static Dictionary<System.Type, Func<object>> _constructorParameterExpressions = new Dictionary<System.Type, Func<object>>();
        public static List<System.Type> ThingTypes;
        public static List<System.Type> GroupThingTypes;
        public static Dictionary<System.Type, List<System.Type>> AllBaseTypes;
        public static Dictionary<System.Type, IEnumerable<FieldInfo>> AllEditorFields;
        public static Dictionary<System.Type, List<FieldInfo>> EditorFieldsForType;
        public static Dictionary<System.Type, FieldInfo[]> AllStateFields;
        public static Map<ushort, System.Type> IDToType = new Map<ushort, System.Type>();
        public static Dictionary<System.Type, Thing> _typeInstances = new Dictionary<System.Type, Thing>();
        public bool tabletMode;
        public static uint thingTypesHash;
        private static bool _clearOnce = false;

        public static void PopFocus() => ++Editor.numPops;

        public static void PopFocusNow()
        {
            if (Editor.focusStack.Count <= 0)
                return;
            Editor.focusStack.Pop();
        }

        public static object PeekFocus() => Editor.focusStack.Count > 0 ? Editor.focusStack.Peek() : null;

        public static void PushFocus(object o) => Editor.focusStack.Push(o);

        public static bool HasFocus() => Editor.focusStack.Count != 0;

        public bool placementLimitReached => Editor.placementLimit > 0 && this.placementTotalCost >= Editor.placementLimit;

        public bool placementOutOfSizeRange => false;

        public static List<string> activatedLevels => DuckNetwork.core._activatedLevels;

        public static int customLevelCount => Editor.activatedLevels.Count + Editor.clientLevelCount;

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
                        if (profile != null && profile.connection != null && profile.connection.status != ConnectionStatus.Disconnected)
                            clientLevelCount += profile.numClientCustomLevels;
                    }
                }
                return clientLevelCount;
            }
        }

        public static byte NetworkActionIndex(System.Type pType, MethodInfo pMethod)
        {
            int num = Editor.GetNetworkActionMethods(pType).IndexOf(pMethod);
            return num >= 0 ? (byte)num : byte.MaxValue;
        }

        public static MethodInfo MethodFromNetworkActionIndex(System.Type pType, byte pIndex)
        {
            List<MethodInfo> networkActionMethods = Editor.GetNetworkActionMethods(pType);
            return pIndex < networkActionMethods.Count ? networkActionMethods[pIndex] : null;
        }

        private static List<MethodInfo> GetNetworkActionMethods(System.Type pType)
        {
            if (!Editor._networkActionIndexes.ContainsKey(pType))
            {
                List<MethodInfo> methodInfoList = new List<MethodInfo>();
                foreach (MethodInfo method in pType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttributes(typeof(NetworkAction), false).Any<object>())
                        methodInfoList.Add(method);
                }
                if (pType.BaseType != null)
                {
                    List<MethodInfo> networkActionMethods = Editor.GetNetworkActionMethods(pType.BaseType);
                    methodInfoList.AddRange(networkActionMethods);
                }
                Editor._networkActionIndexes[pType] = methodInfoList;
            }
            return Editor._networkActionIndexes[pType];
        }

        public static EditorGroup Placeables
        {
            get
            {
                while (!Editor._listLoaded)
                    Thread.Sleep(16);
                return Editor._placeables;
            }
        }

        protected List<Thing> _levelThings => !this.editingOpenAirVariation ? this._levelThingsNormal : this._levelThingsAlternate;

        public List<Thing> levelThings => this._levelThings;

        public string saveName
        {
            get => this._saveName;
            set => this._saveName = value;
        }

        public static ContextMenu lockInput
        {
            get => Editor._lockInput;
            set => Editor._lockInputChange = value;
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
            Editor.hasUnsavedChanges = true;
            if (obj == null)
                return;
            if (obj.maxPlaceable >= 0 && this.things[obj.GetType()].Count<Thing>() >= obj.maxPlaceable)
            {
                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| Too many placed!", 2f);
            }
            else
            {
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
                                        this.AddObject(current);
                                }
                                return;
                            }
                        }
                        else
                        {
                            using (List<Thing>.Enumerator enumerator = thingContainer.things.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                    this.AddObject(enumerator.Current);
                                return;
                            }
                        }
                    case BackgroundUpdater _:
                        for (int index = 0; index < this._levelThings.Count; ++index)
                        {
                            Thing t = this._levelThings[index];
                            if (t is BackgroundUpdater)
                            {
                                History.Add(() => this.RemoveObject(t), () => this.AddObject(t));
                                --index;
                            }
                        }
                        break;
                }
                obj.active = false;
                this.AddThing(obj);
                this._levelThings.Add(obj);
                if (!this._loadingLevel && obj is IDontMove)
                    this._placeObjects.Add(obj);
                this.placementTotalCost += Editor.CalculatePlacementCost(obj);
                if (_sizeRestriction.x > 0.0)
                    this.AdjustSizeLimits(obj);
                if (this._loadingLevel)
                    return;
                if (!this._isPaste)
                    obj.EditorAdded();
                if (obj is MirrorMode || this.processingMirror || obj is BackgroundUpdater)
                    return;
                this.processingMirror = true;
                foreach (MirrorMode mirrorMode in this.things[typeof(MirrorMode)])
                {
                    if (((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both || (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Vertical) && Math.Abs(mirrorMode.position.x - obj.position.x) > 2.0)
                    {
                        Vec2 vec2 = obj.position - new Vec2((float)((obj.position.x - mirrorMode.position.x) * 2.0), 0f);
                        Thing thing = Thing.LoadThing(obj.Serialize());
                        thing.position = vec2;
                        thing.flipHorizontal = !obj.flipHorizontal;
                        this.AddObject(thing);
                        thing.EditorFlip(false);
                    }
                    if (((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both || (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Horizontal) && Math.Abs(mirrorMode.position.y - obj.position.y) > 2.0)
                    {
                        Vec2 vec2 = obj.position - new Vec2(0f, (float)((obj.position.y - mirrorMode.position.y) * 2.0));
                        Thing thing = Thing.LoadThing(obj.Serialize());
                        thing.position = vec2;
                        this.AddObject(thing);
                        thing.EditorFlip(true);
                    }
                    if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both && Math.Abs(mirrorMode.position.x - obj.position.x) > 2.0 && Math.Abs(mirrorMode.position.y - obj.position.y) > 2.0)
                    {
                        Vec2 vec2 = obj.position - new Vec2((float)((obj.position.x - mirrorMode.position.x) * 2.0), (float)((obj.position.y - mirrorMode.position.y) * 2.0));
                        Thing thing = Thing.LoadThing(obj.Serialize());
                        thing.position = vec2;
                        thing.flipHorizontal = !obj.flipHorizontal;
                        this.AddObject(thing);
                        thing.EditorFlip(false);
                        thing.EditorFlip(true);
                    }
                }
                this.processingMirror = false;
            }
        }

        public void RemoveObject(Thing obj)
        {
            Editor.hasUnsavedChanges = true;
            Level.current.RemoveThing(obj);
            this._levelThings.Remove(obj);
            if (obj is IDontMove)
                this._placeObjects.Add(obj);
            this.placementTotalCost -= Editor.CalculatePlacementCost(obj);
            if (_sizeRestriction.x > 0.0 && (obj.x <= _topLeftMost.x || obj.x >= _bottomRightMost.x || obj.y <= _topLeftMost.y || obj.y >= _bottomRightMost.y))
                this.RecalculateSizeLimits();
            obj.EditorRemoved();
            if (this._loadingLevel || obj is MirrorMode || this.processingMirror || obj is BackgroundUpdater)
                return;
            this.processingMirror = true;
            foreach (MirrorMode mirrorMode in this.things[typeof(MirrorMode)])
            {
                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both || (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Vertical)
                {
                    Thing thing = Level.current.CollisionPoint(obj.position + new Vec2((float)(-(obj.position.x - mirrorMode.position.x) * 2.0), 0f), obj.GetType());
                    if (thing != null)
                        this.RemoveObject(thing);
                }
                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both || (MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Horizontal)
                {
                    Thing thing = Level.current.CollisionPoint(obj.position + new Vec2(0f, (float)(-(obj.position.y - mirrorMode.position.y) * 2.0)), obj.GetType());
                    if (thing != null)
                        this.RemoveObject(thing);
                }
                if ((MirrorMode.Setting)mirrorMode.mode == MirrorMode.Setting.Both)
                {
                    Thing thing = Level.current.CollisionPoint(obj.position + new Vec2((float)(-(obj.position.x - mirrorMode.position.x) * 2.0), (float)(-(obj.position.y - mirrorMode.position.y) * 2.0)), obj.GetType());
                    if (thing != null)
                        this.RemoveObject(thing);
                }
            }
            this.processingMirror = false;
        }

        public void AdjustSizeLimits(Thing pObject)
        {
            if (pObject.x < _topLeftMost.x)
                this._topLeftMost.x = pObject.x;
            if (pObject.x > _bottomRightMost.x)
                this._bottomRightMost.x = pObject.x;
            if (pObject.y < _topLeftMost.y)
                this._topLeftMost.y = pObject.y;
            if (pObject.y <= _bottomRightMost.y)
                return;
            this._bottomRightMost.y = pObject.y;
        }

        public void RecalculateSizeLimits()
        {
            this._topLeftMost = new Vec2(99999f, 99999f);
            this._bottomRightMost = new Vec2(-99999f, -99999f);
            foreach (Thing levelThing in this._levelThings)
                this.AdjustSizeLimits(levelThing);
        }

        public static int CalculatePlacementCost(Thing pObject) => pObject.placementCost;

        public void ClearEverything()
        {
            foreach (Thing t in this._levelThingsNormal)
                Level.current.RemoveThing(t);
            this._levelThingsNormal.Clear();
            foreach (Thing t in this._levelThingsAlternate)
                Level.current.RemoveThing(t);
            this._levelThingsAlternate.Clear();
            this.editingOpenAirVariation = this._editingOpenAirVariationPrev = false;
            this._lastCommand = -1;
            this._commands.Clear();
            if (!this._looseClear)
            {
                this._procContext = null;
                this._procTarget = null;
            }
            this._pathNorth = false;
            this._pathSouth = false;
            this._pathWest = false;
            this._pathEast = false;
            this._miniMode = false;
            this.things.quadTree.Clear();
            this.generatorComplexity = 0;
            Custom.ClearCustomData();
            Editor._currentLevelData = new LevelData();
            Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
            Editor.previewCapture = null;
            Editor.hasUnsavedChanges = false;
            this.placementTotalCost = 0;
            this.RecalculateSizeLimits();
            History.Clear();
        }

        public float cellSize
        {
            get => this._cellSize;
            set => this._cellSize = value;
        }

        //private float width => (float)this._gridW * this._cellSize;

        //private float height => (float)this._gridH * this._cellSize;

        public Thing placementType
        {
            set
            {
                this._placementType = value;
                this._eyeDropperSerialized = null;
            }
            get => this._placementType;
        }

        private LevelType GetLevelType()
        {
            if (Editor.arcadeMachineMode)
                return LevelType.Arcade_Machine;
            LevelType levelType = LevelType.Deathmatch;
            if (this._levelThings.FirstOrDefault<Thing>(x => x is ChallengeMode) != null)
                levelType = LevelType.Challenge;
            else if (this._levelThings.FirstOrDefault<Thing>(x => x is ArcadeMode) != null)
                levelType = LevelType.Arcade;
            return levelType;
        }

        private LevelSize GetLevelSize()
        {
            this._topLeft = new Vec2(99999f, 99999f);
            this._bottomRight = new Vec2(-99999f, -99999f);
            this.CalculateBounds();
            double length = (this.topLeft - this.bottomRight).length;
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
            foreach (object levelThing in this._levelThings)
            {
                if (!ContentProperties.GetBag(levelThing.GetType()).GetOrDefault("isOnlineCapable", true))
                    return false;
            }
            return true;
        }

        public void SteamUpload()
        {
            if (Editor.arcadeMachineMode)
            {
                (this._levelThings[0] as ArcadeMachine).UpdateData();
                if ((this._levelThings[0] as ArcadeMachine).challenge01Data == null || (this._levelThings[0] as ArcadeMachine).challenge02Data == null || (this._levelThings[0] as ArcadeMachine).challenge02Data == null)
                {
                    this.DoMenuClose();
                    this._closeMenu = false;
                    this._notify.Open("You must select 3 valid Challenges!");
                    return;
                }
            }
            if (this._saveName == "")
            {
                this.DoMenuClose();
                this._closeMenu = false;
                this._notify.Open("Please save the level first...");
            }
            else
            {
                this.Save();
                this._uploadDialog.Open(Editor._currentLevelData);
                this.DoMenuClose();
                this._closeMenu = false;
                Content.customPreviewWidth = 0;
                Content.customPreviewHeight = 0;
                Content.customPreviewCenter = Vec2.Zero;
            }
        }

        public MonoFileDialog fileDialog => this._fileDialog;

        public static string initialDirectory => Editor._initialDirectory;

        public static Layer objectMenuLayer => Main.editor._objectMenuLayer;

        public void EnterEditor()
        {
            this.focusWait = 10;
            Layer.ClearLayers();
            this._gridLayer = new Layer("GRID", Layer.Background.depth + 5, Layer.Background.camera)
            {
                allowTallAspect = true
            };
            Layer.Add(this._gridLayer);
            this._procLayer = new Layer("PROC", Layer.Background.depth + 25, new Camera(0f, 0f, Graphics.width, Graphics.height))
            {
                allowTallAspect = true
            };
            Layer.Add(this._procLayer);
            Music.Stop();
            if (!Editor.isTesting)
            {
                this._placementType = null;
                this.CenterView();
                this._tilePosition = new Vec2(0f, 0f);
            }
            this._ultimateBounds = Level.current.things.quadTree.rectangle;
            Layer.HUD.camera.InitializeToScreenAspect();
            Layer.HUD.camera.width *= 2f;
            Layer.HUD.camera.height *= 2f;
            Layer.HUD.allowTallAspect = true;
            if (Resolution.current.aspect > 2.0)
            {
                Layer.HUD.camera.width *= 2f;
                Layer.HUD.camera.height *= 2f;
            }
            if (this._objectMenuLayer == null)
            {
                this._objectMenuLayer = new Layer("OBJECTMENU", Layer.HUD.depth - 25, new Camera(0f, 0f, Layer.HUD.camera.width, Layer.HUD.camera.height))
                {
                    allowTallAspect = true
                };
            }
            Layer.Add(this._objectMenuLayer);
            this.backgroundColor = new Color(20, 20, 20);
            Editor.focusStack.Clear();
            Editor.active = true;
            Editor.isTesting = false;
            Editor.inputMode = EditorInput.Gamepad;
        }

        public void Quit() => this._quitting = true;

        public override void DoInitialize()
        {
            SFX.StopAllSounds();
            if (!this._initialized)
            {
                this.Initialize();
                this._initialized = true;
            }
            else
            {
                this.EnterEditor();
                base.DoInitialize();
            }
        }

        public override void Terminate()
        {
        }

        public static bool miniMode
        {
            get => Level.current is Editor && (Level.current as Editor)._miniMode;
            set
            {
                if (!(Level.current is Editor))
                    return;
                (Level.current as Editor)._miniMode = value;
            }
        }

        public float _chance
        {
            get => Editor._currentLevelData.proceduralData.chance;
            set => Editor._currentLevelData.proceduralData.chance = value;
        }

        public int _maxPerLevel
        {
            get => Editor._currentLevelData.proceduralData.maxPerLevel;
            set => Editor._currentLevelData.proceduralData.maxPerLevel = value;
        }

        public bool _enableSingle
        {
            get => Editor._currentLevelData.proceduralData.enableSingle;
            set => Editor._currentLevelData.proceduralData.enableSingle = value;
        }

        public bool _enableMulti
        {
            get => Editor._currentLevelData.proceduralData.enableMulti;
            set => Editor._currentLevelData.proceduralData.enableMulti = value;
        }

        public bool _canMirror
        {
            get => Editor._currentLevelData.proceduralData.canMirror;
            set => Editor._currentLevelData.proceduralData.canMirror = value;
        }

        public bool _isMirrored
        {
            get => Editor._currentLevelData.proceduralData.isMirrored;
            set => Editor._currentLevelData.proceduralData.isMirrored = value;
        }

        public void UpdateObjectMenu()
        {
            if (this._objectMenu != null)
                Level.Remove(_objectMenu);
            this._objectMenu = new PlacementMenu(0f, 0f);
            Level.Add(_objectMenu);
            this._objectMenu.visible = this._objectMenu.active = false;
        }

        public string additionalSaveDirectory => this._additionalSaveDirectory;

        public override void Initialize()
        {
            while (!Editor._listLoaded)
                Thread.Sleep(16);
            this._editorCam = new EditorCam();
            this.camera = _editorCam;
            this.camera.InitializeToScreenAspect();
            this._selectionMaterial = new MaterialSelection();
            this._selectionMaterialPaste = new MaterialSelection
            {
                fade = 0.5f
            };
            this._cursor = new SpriteMap("cursors", 16, 16);
            this._tileset = new SpriteMap("industrialTileset", 16, 16);
            this._sideArrow = new Sprite("Editor/sideArrow");
            this._sideArrow.CenterOrigin();
            this._sideArrowHover = new Sprite("Editor/sideArrowHover");
            this._sideArrowHover.CenterOrigin();
            this._cantPlace = new Sprite("cantPlace");
            this._cantPlace.CenterOrigin();
            this._editorCurrency = new Sprite("editorCurrency");
            this._die = new Sprite("die");
            this._dieHover = new Sprite("dieHover");
            this._singleBlock = new Sprite("Editor/singleplayerBlock");
            this._multiBlock = new Sprite("Editor/multiplayerBlock");
            Layer.Background.camera.InitializeToScreenAspect();
            Layer.Game.camera.InitializeToScreenAspect();
            Layer.Game.camera.width *= 2f;
            Layer.Game.camera.height *= 2f;
            this.CalculateGridRestriction();
            this.EnterEditor();
            this._camSize = new Vec2(this.camera.width, this.camera.height);
            this._font = new BitmapFont("biosFont", 8);
            Editor._input = InputProfile.Get(InputProfile.MPPlayer1);
            this._tilePosition = new Vec2(0f, 0f);
            this._tilePositionPrev = this._tilePosition;
            this._objectMenu = new PlacementMenu(0f, 0f);
            Level.Add(_objectMenu);
            this._objectMenu.visible = this._objectMenu.active = false;
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_chance", increment: 0.05f), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/dieBlock", 16, 16), "CHANCE - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridBottomLeft));
            Level.Add(new TileButton(0f, 16f, new FieldBinding(this, "_maxPerLevel", -1f, 8f, 1f), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/numBlock", 16, 16), "MAX IN LEVEL - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridBottomLeft));
            Level.Add(new TileButton(-16f, 0f, new FieldBinding(this, "_enableSingle"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/singleplayerBlock", 16, 16), "AVAILABLE IN SINGLE PLAYER - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_enableMulti"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/multiplayerBlock", 16, 16), "AVAILABLE IN MULTI PLAYER - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Level.Add(new TileButton(-16f, 16f, new FieldBinding(this, "_canMirror"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/canMirror", 16, 16), "TILE CAN BE MIRRORED - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Level.Add(new TileButton(0f, 16f, new FieldBinding(this, "_isMirrored"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/isMirrored", 16, 16), "PRE MIRRORED TILE - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Level.Add(new TileButton(0f, 32f, new FieldBinding(this, "editingOpenAirVariation"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/openAir", 16, 16), "OPEN AIR VARIATION - @SELECT@TOGGLE", TileButtonAlign.TileGridBottomRight));
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathEast"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS EAST - @SELECT@TOGGLE", TileButtonAlign.TileGridRight, 90f));
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathWest"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS WEST - @SELECT@TOGGLE", TileButtonAlign.TileGridLeft, -90f));
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathNorth"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS NORTH - @SELECT@TOGGLE", TileButtonAlign.TileGridTop));
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_pathSouth"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/sideArrow", 32, 16), "CONNECTS SOUTH - @SELECT@TOGGLE", TileButtonAlign.TileGridBottom, 180f));
            Level.Add(new TileButton(16f, 0f, new FieldBinding(this, "_genTilePos", max: 6f, increment: 1f), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/moveBlock", 16, 16), "MOVE GEN - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopLeft));
            Level.Add(new TileButton(32f, 0f, new FieldBinding(this, "_editTilePos", max: 6f, increment: 1f), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/editBlock", 16, 16), "MOVE GEN - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopLeft));
            Level.Add(new TileButton(48f, 0f, new FieldBinding(this, "generatorComplexity", max: 9f, increment: 1f), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/dieBlockRed", 16, 16), "NUM TILES - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopLeft));
            Level.Add(new TileButton(0f, 0f, new FieldBinding(this, "_doGen"), new FieldBinding(this, "_miniMode"), new SpriteMap("Editor/regenBlock", 16, 16), "REGENERATE - HOLD @SELECT@ AND MOVE @DPAD@", TileButtonAlign.TileGridTopRight));
            this._notify = new NotifyDialogue();
            Level.Add(_notify);
            Vec2 vec2_1 = new Vec2(12f, 12f);
            this._touchButtons.Add(new Editor.EditorTouchButton()
            {
                caption = "MENU",
                explanation = "Pick an object for placement...",
                state = Editor.EditorTouchState.OpenMenu,
                threeFingerGesture = true
            });
            this._touchButtons.Add(new Editor.EditorTouchButton()
            {
                caption = "COPY",
                explanation = "Pick an object to copy...",
                state = Editor.EditorTouchState.Eyedropper
            });
            this._touchButtons.Add(new Editor.EditorTouchButton()
            {
                caption = "EDIT",
                explanation = "Press objects to edit them!",
                state = Editor.EditorTouchState.EditObject
            });
            this._cancelButton = new Editor.EditorTouchButton()
            {
                caption = "CANCEL",
                explanation = "",
                state = Editor.EditorTouchState.Normal
            };
            this._editTilesButton = new Editor.EditorTouchButton()
            {
                caption = "PICK TILE",
                explanation = "",
                state = Editor.EditorTouchState.PickTile
            };
            this._editTilesButton.size = new Vec2(DuckGame.Graphics.GetStringWidth(this._editTilesButton.caption) + 6f, 15f) + vec2_1;
            this._editTilesButton.position = Layer.HUD.camera.OffsetTL(10f, 10f);
            Vec2 vec2_2 = Layer.HUD.camera.OffsetBR(-14f, -14f);
            for (int index = this._touchButtons.Count - 1; index >= 0; --index)
            {
                Editor.EditorTouchButton touchButton = this._touchButtons[index];
                if (index == this._touchButtons.Count - 1)
                {
                    this._cancelButton.size = new Vec2(DuckGame.Graphics.GetStringWidth(this._cancelButton.caption) + 6f, 15f) + vec2_1;
                    this._cancelButton.position = vec2_2 - this._cancelButton.size;
                }
                touchButton.size = new Vec2(DuckGame.Graphics.GetStringWidth(touchButton.caption) + 6f, 15f) + vec2_1;
                touchButton.position = vec2_2 - touchButton.size;
                vec2_2.x -= touchButton.size.x + 4f;
            }
            Editor._initialDirectory = DuckFile.levelDirectory;
            Editor._initialDirectory = Path.GetFullPath(Editor._initialDirectory);
            this._fileDialog = new MonoFileDialog();
            Level.Add(_fileDialog);
            this._uploadDialog = new SteamUploadDialog();
            Level.Add(_uploadDialog);
            this._editorButtons = new SpriteMap("editorButtons", 32, 32);
            this._doingResave = true;
            this._doingResave = false;
            this.ClearEverything();
        }

        public Vec2 GetAlignOffset(TileButtonAlign align)
        {
            switch (align)
            {
                case TileButtonAlign.ProcGridTopLeft:
                    int num1 = 192;
                    int num2 = 144;
                    return new Vec2()
                    {
                        x = -(Editor._procTilesWide - (Editor._procTilesWide - Editor._procXPos)) * num1,
                        y = -(Editor._procTilesHigh - (Editor._procTilesHigh - Editor._procYPos)) * num2 - 16
                    };
                case TileButtonAlign.TileGridTopLeft:
                    return new Vec2() { x = 0f, y = -16f };
                case TileButtonAlign.TileGridTopRight:
                    int num3 = 192;
                    return new Vec2()
                    {
                        x = num3 - 16,
                        y = -16f
                    };
                case TileButtonAlign.TileGridBottomLeft:
                    int num4 = 144;
                    return new Vec2() { x = 0f, y = num4 };
                case TileButtonAlign.TileGridBottomRight:
                    int num5 = 144;
                    int num6 = 192;
                    return new Vec2()
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
                    this.LoadLevel(load);
                    this._things.RefreshState();
                    this._updateEvenWhenInactive = true;
                    this.Update();
                    this._updateEvenWhenInactive = false;
                    if (this.existingGUID.Contains(Editor._currentLevelData.metaData.guid))
                        Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                    this.existingGUID.Add(Editor._currentLevelData.metaData.guid);
                    this.Save();
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                }
            }
            foreach (string root1 in DuckFile.GetDirectoriesNoCloud(root))
                this.Resave(root1);
        }

        public static InputProfile input => Editor._input;

        public void ShowNoSpawnsDialogue()
        {
            if (this._noSpawnsDialogue == null)
            {
                this._noSpawnsDialogue = new MessageDialogue(null);
                Level.Add(_noSpawnsDialogue);
            }
            this._noSpawnsDialogue.Open("NO SPAWNS", pDescription: "Your level has no spawns.\n\n\n@_!DUCKSPAWN@\n\n\nPlease place a |DGBLUE|Spawns/Spawn Point|PREV|\n in your level.");
            Editor.lockInput = _noSpawnsDialogue;
            this._noSpawnsDialogue.okayOnly = true;
            this._noSpawnsDialogue.windowYOffsetAdd = -30f;
        }

        public void CompleteDialogue(ContextMenu pItem)
        {
        }

        public void OpenMenu(ContextMenu menu)
        {
            menu.active = menu.visible = true;
            if (Editor.inputMode == EditorInput.Mouse)
            {
                menu.x = Mouse.x;
                menu.y = Mouse.y;
            }
            if (Editor.openPosition != Vec2.Zero)
            {
                menu.position = Editor.openPosition + new Vec2(-2f, -3f);
                Editor.openPosition = Vec2.Zero;
            }
            if (this._showPlacementMenu)
            {
                menu.x = 96f;
                menu.y = 32f;
                this._showPlacementMenu = false;
            }
            if (Editor.inputMode == EditorInput.Gamepad || Editor.inputMode == EditorInput.Touch)
            {
                menu.x = 16f;
                menu.y = 16f;
            }
            menu.opened = true;
            this._placementMenu = menu;
            this.disableDragMode();
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
            if (!this._showPlacementMenu)
                this._closeMenu = true;
            this._touchState = Editor.EditorTouchState.Normal;
            this._activeTouchButton = null;
            Editor.clickedMenu = true;
            this._editMode = false;
            this._copyMode = false;
            this._hover = null;
        }

        private void disableDragMode()
        {
            this._dragMode = false;
            this._deleteMode = false;
            if (this._move != null)
                this._move = null;
            this.dragModeInputType = InputType.eNone;
            History.EndUndoSection();
        }

        private void HugObjectPlacement()
        {
            if (this._placementType is ItemSpawner)
                (this._placementType as ItemSpawner)._seated = false;
            if ((this._placementType.hugWalls & WallHug.Right) != WallHug.None && this.CollisionLine<IPlatform>(this._tilePosition, this._tilePosition + new Vec2(16f, 0f), this._placementType) is Thing thing1 && thing1.GetType() != this._placementType.GetType())
                this._tilePosition.x = thing1.left - this._placementType.collisionSize.x - this._placementType.collisionOffset.x;
            if ((this._placementType.hugWalls & WallHug.Left) != WallHug.None && this.CollisionLine<IPlatform>(this._tilePosition, this._tilePosition + new Vec2(-16f, 0f), this._placementType) is Thing thing2 && thing2.GetType() != this._placementType.GetType())
                this._tilePosition.x = thing2.right - this._placementType.collisionOffset.x;
            if ((this._placementType.hugWalls & WallHug.Ceiling) != WallHug.None && this.CollisionLine<IPlatform>(this._tilePosition, this._tilePosition + new Vec2(0f, -16f), this._placementType) is Thing thing3 && thing3.GetType() != this._placementType.GetType())
                this._tilePosition.y = thing3.bottom - this._placementType.collisionOffset.y;
            if ((this._placementType.hugWalls & WallHug.Floor) == WallHug.None || !(this.CollisionLine<IPlatform>(this._tilePosition, this._tilePosition + new Vec2(0f, 16f), this._placementType) is Thing thing4) || !(thing4.GetType() != this._placementType.GetType()))
                return;
            this._tilePosition.y = thing4.top - this._placementType.collisionSize.y - this._placementType.collisionOffset.y;
            if (!(this._placementType is ItemSpawner))
                return;
            (this._placementType as ItemSpawner)._seated = true;
        }

        public static float interfaceSizeMultiplier => Editor.inputMode != EditorInput.Touch ? 1f : 2f;

        public override void Update()
        {
            if (!DuckGame.Graphics.inFocus)
                this.focusWait = 5;
            else if (this.focusWait > 0)
            {
                --this.focusWait;
            }
            else
            {
                ++MonoMain.timeInEditor;
                Editor.tooltip = null;
                foreach (Thing thing in this.things)
                    thing.DoEditorUpdate();
                if (this.lastMousePos == Vec2.Zero)
                    this.lastMousePos = Mouse.position;
                if (Editor.clickedContextBackground)
                {
                    Editor.clickedContextBackground = false;
                    Editor.clickedMenu = true;
                }
                int inputMode1 = (int)Editor.inputMode;
                if (Mouse.left == InputState.Pressed || Mouse.right == InputState.Pressed || Mouse.middle == InputState.Pressed || !Editor.fakeTouch && (this.lastMousePos - Mouse.position).length > 3.0)
                    Editor.inputMode = Editor.fakeTouch ? EditorInput.Touch : EditorInput.Mouse;
                else if (Editor.inputMode != EditorInput.Gamepad && InputProfile.active.Pressed("ANY", true))
                {
                    if ((this._selection.Count == 0 || !Keyboard.Pressed(Keys.F, true)) && !InputProfile.active.Pressed("RSTICK") && !InputProfile.active.Pressed("CANCEL") && !InputProfile.active.Pressed("MENU1") && !Keyboard.Down(Keys.LeftShift) && !Keyboard.Down(Keys.RightShift) && !Keyboard.Down(Keys.LeftControl) && !Keyboard.Down(Keys.RightControl))
                    {
                        if (Editor.inputMode == EditorInput.Mouse)
                        {
                            this._tilePosition = Maths.Snap(Mouse.positionScreen + new Vec2(8f, 8f), 16f, 16f);
                            this._tilePositionPrev = this._tilePosition;
                        }
                        Editor.inputMode = EditorInput.Gamepad;
                    }
                }
                else if (TouchScreen.IsScreenTouched())
                    Editor.inputMode = EditorInput.Touch;
                if (Editor.inputMode == EditorInput.Mouse)
                    Editor._input.lastActiveDevice = Input.GetDevice<Keyboard>();
                int inputMode2 = (int)Editor.inputMode;
                if (inputMode1 != inputMode2 && Editor.inputMode == EditorInput.Touch)
                {
                    Editor.clickedMenu = true;
                    Editor.waitForNoTouchInput = true;
                }
                else if (Editor.waitForNoTouchInput && TouchScreen.GetTouches().Count > 0)
                {
                    Editor.clickedMenu = true;
                }
                else
                {
                    Editor.waitForNoTouchInput = false;
                    this.lastMousePos = Mouse.position;
                    if (this._editingOpenAirVariationPrev != this.editingOpenAirVariation)
                    {
                        if (this.editingOpenAirVariation)
                        {
                            foreach (Thing t in this._levelThingsNormal)
                                Level.current.RemoveThing(t);
                            foreach (Thing t in this._levelThingsAlternate)
                                Level.current.AddThing(t);
                        }
                        else
                        {
                            foreach (Thing t in this._levelThingsAlternate)
                                Level.current.RemoveThing(t);
                            foreach (Thing t in this._levelThingsNormal)
                                Level.current.AddThing(t);
                        }
                        this._editingOpenAirVariationPrev = this.editingOpenAirVariation;
                    }
                    if (Editor.inputMode == EditorInput.Touch)
                    {
                        if (this._placementMenu == null)
                        {
                            this._objectMenuLayer.camera.width = Layer.HUD.width * 0.75f;
                            this._objectMenuLayer.camera.height = Layer.HUD.height * 0.75f;
                            Editor.bigInterfaceMode = true;
                        }
                        if (this._fileDialog.opened && this._touchState != Editor.EditorTouchState.OpenLevel)
                        {
                            this.EndCurrentTouchMode();
                            this._touchState = Editor.EditorTouchState.OpenLevel;
                        }
                        else if (!this._fileDialog.opened && this._touchState == Editor.EditorTouchState.OpenLevel)
                            this.EndCurrentTouchMode();
                        Touch tap = TouchScreen.GetTap();
                        if (this._touchState == Editor.EditorTouchState.Normal)
                        {
                            this._activeTouchButton = null;
                            foreach (Editor.EditorTouchButton touchButton in this._touchButtons)
                            {
                                if (tap != Touch.None && tap.positionHUD.x > touchButton.position.x && tap.positionHUD.x < touchButton.position.x + touchButton.size.x && tap.positionHUD.y > touchButton.position.y && tap.positionHUD.y < touchButton.position.y + touchButton.size.y || touchButton.threeFingerGesture && this._threeFingerGesture)
                                {
                                    this._touchState = touchButton.state;
                                    this._activeTouchButton = touchButton;
                                    Editor.clickedMenu = true;
                                    this._threeFingerGesture = false;
                                    Editor._clickedTouchButton = true;
                                    SFX.Play("highClick", 0.3f, 0.2f);
                                }
                            }
                        }
                        else if (tap.positionHUD.x > this._cancelButton.position.x && tap.positionHUD.x < _cancelButton.position.x + this._cancelButton.size.x && tap.positionHUD.y > this._cancelButton.position.y && tap.positionHUD.y < _cancelButton.position.y + this._cancelButton.size.y || this._activeTouchButton != null && this._activeTouchButton.threeFingerGesture && this._threeFingerGesture || this._activeTouchButton != null && !this._activeTouchButton.threeFingerGesture && this._threeFingerGesture || this._activeTouchButton != null && this._activeTouchButton.threeFingerGesture && this._twoFingerGesture)
                        {
                            this.EndCurrentTouchMode();
                            if (this._fileDialog.opened)
                                this._fileDialog.Close();
                            SFX.Play("highClick", 0.3f, 0.2f);
                            return;
                        }
                        if (this._placingTiles && this._placementMenu == null && tap.positionHUD.x > this._editTilesButton.position.x && tap.positionHUD.x < _editTilesButton.position.x + this._editTilesButton.size.x && tap.positionHUD.y > this._editTilesButton.position.y && tap.positionHUD.y < _editTilesButton.position.y + this._editTilesButton.size.y)
                        {
                            this._openTileSelector = true;
                            Editor.clickedMenu = true;
                        }
                        if (this._touchState == Editor.EditorTouchState.OpenMenu)
                        {
                            if (this._placementMenu == null)
                                this._showPlacementMenu = true;
                            this.EndCurrentTouchMode();
                        }
                        else if (this._touchState == Editor.EditorTouchState.EditObject)
                            this._editMode = true;
                        else if (this._touchState == Editor.EditorTouchState.Eyedropper)
                            this._copyMode = true;
                    }
                    else
                    {
                        this._editMode = false;
                        this._copyMode = false;
                        this._activeTouchButton = null;
                        this._touchState = Editor.EditorTouchState.Normal;
                        if (this._placementMenu == null)
                        {
                            this._objectMenuLayer.camera.width = Layer.HUD.width;
                            this._objectMenuLayer.camera.height = Layer.HUD.height;
                            Editor.bigInterfaceMode = false;
                            Editor.pretendPinned = null;
                        }
                    }
                    if (!DuckGame.Graphics.inFocus && !this._updateEvenWhenInactive)
                        this._tileDragDif = Vec2.MaxValue;
                    else if (Editor.clickedMenu)
                    {
                        Editor.clickedMenu = false;
                    }
                    else
                    {
                        if (this._notify.opened)
                            return;
                        if (Editor.reopenContextMenu)
                        {
                            int num = Editor.ignorePinning ? 1 : 0;
                            Editor.reopenContextMenu = false;
                            if (this._placementMenu != null)
                                this._placementMenu.opened = false;
                            Editor.ignorePinning = num != 0;
                            if (this._placementMenu == null)
                                this._placementMenu = this._objectMenu;
                            this.OpenMenu(this._placementMenu);
                            if (Editor.openContextThing != null)
                                this._placementMenu.OpenInto(openContextThing);
                            Editor.openContextThing = null;
                            SFX.Play("openClick", 0.4f);
                        }
                        Editor.hoverTextBox = false;
                        if (Editor.numPops > 0)
                        {
                            for (int index = 0; index < Editor.numPops && Editor.focusStack.Count != 0; ++index)
                                Editor.focusStack.Pop();
                            Editor.numPops = 0;
                        }
                        if (Editor.tookInput)
                            Editor.tookInput = false;
                        else if (Editor.focusStack.Count > 0 || Editor.skipFrame)
                        {
                            Editor.skipFrame = false;
                        }
                        else
                        {
                            if (this._placementMenu != null)
                                this._placementMenu.visible = Editor._lockInput == null;
                            if (Editor.lockInput != null)
                            {
                                if (Editor._lockInputChange == Editor.lockInput)
                                    return;
                                Editor._lockInput = Editor._lockInputChange;
                            }
                            else
                            {
                                if (Editor._lockInputChange != Editor.lockInput)
                                    Editor._lockInput = Editor._lockInputChange;
                                if (Keyboard.Pressed(Keys.OemComma))
                                    this.searching = true;
                                if (this.searching)
                                {
                                    Input._imeAllowed = true;
                                    if (!this.clearedKeyboardStringForSearch)
                                    {
                                        this.clearedKeyboardStringForSearch = true;
                                        Keyboard.keyString = "";
                                    }
                                    if (this.searchItems != null && this.searchItems.Count > 0)
                                    {
                                        if (Keyboard.Pressed(Keys.Down))
                                        {
                                            if (this._searchHoverIndex == 0)
                                                this._searchHoverIndex = Math.Min(this.searchItems.Count - 1, 9);
                                            else
                                                --this._searchHoverIndex;
                                            if (this._searchHoverIndex < 0)
                                                this._searchHoverIndex = 0;
                                        }
                                        else if (Keyboard.Pressed(Keys.Up))
                                        {
                                            if (this._searchHoverIndex < 0)
                                                this._searchHoverIndex = 0;
                                            else
                                                ++this._searchHoverIndex;
                                            if (this._searchHoverIndex > Math.Min(this.searchItems.Count - 1, 9))
                                                this._searchHoverIndex = 0;
                                        }
                                        this._searchHoverIndex = Math.Min(this.searchItems.Count - 1, this._searchHoverIndex);
                                    }
                                    else
                                        this._searchHoverIndex = -1;
                                    bool flag = Mouse.left == InputState.Pressed || Keyboard.Pressed(Keys.Enter);
                                    if (((Mouse.right == InputState.Released || Mouse.middle == InputState.Pressed ? 1 : (Keyboard.Pressed(Keys.Escape) ? 1 : 0)) | (flag ? 1 : 0)) != 0)
                                    {
                                        if (flag && this._searchHoverIndex != -1 && this._searchHoverIndex < this.searchItems.Count)
                                        {
                                            this._placementType = this.searchItems[this._searchHoverIndex].thing.thing;
                                            this._eyeDropperSerialized = null;
                                        }
                                        this.searching = false;
                                        this.clearedKeyboardStringForSearch = false;
                                        this.searchItems = null;
                                        this._searchHoverIndex = -1;
                                    }
                                    if (this._prevSearchString != Keyboard.keyString)
                                    {
                                        this.searchItems = this._objectMenu.Search(Keyboard.keyString);
                                        this._prevSearchString = Keyboard.keyString;
                                    }
                                    if (this._placementMenu == null)
                                        return;
                                    this.CloseMenu();
                                }
                                if (Keyboard.control && Keyboard.Pressed(Keys.S))
                                {
                                    if (Keyboard.shift)
                                        this.SaveAs();
                                    else
                                        this.Save();
                                }
                                if (this._onlineSettingChanged && this._placementMenu != null && this._placementMenu is EditorGroupMenu)
                                {
                                    (this._placementMenu as EditorGroupMenu).UpdateGrayout();
                                    this._onlineSettingChanged = false;
                                }
                                DuckGame.Graphics.fade = Lerp.Float(Graphics.fade, this._quitting ? 0f : 1f, 0.02f);
                                if (this._quitting && DuckGame.Graphics.fade < 0.01f)
                                {
                                    this._quitting = false;
                                    Editor.active = false;
                                    Level.current = new TitleScreen();
                                }
                                if (DuckGame.Graphics.fade < 0.95f)
                                    return;
                                Layer placementLayer = this.GetLayerOrOverride(this._placementType);
                                switch (Editor.inputMode)
                                {
                                    case EditorInput.Mouse:
                                        this.clicked = Mouse.left == InputState.Pressed;
                                        if (Mouse.middle == InputState.Pressed)
                                        {
                                            this.middleClickPos = Mouse.position;
                                            break;
                                        }
                                        break;
                                    case EditorInput.Touch:
                                        this.clicked = TouchScreen.GetTap() != Touch.None;
                                        break;
                                }
                                if (this._cursorMode == CursorMode.Normal && (Keyboard.Down(Keys.RightShift) || Keyboard.Down(Keys.LeftShift)))
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
                                        foreach (Thing thing1 in Level.current.things)
                                        {
                                            Thing thing2 = thing1;
                                            thing2.position += vec2;
                                            if (thing1 is IDontMove)
                                            {
                                                Level.current.things.quadTree.Remove(thing1);
                                                Level.current.things.quadTree.Add(thing1);
                                            }
                                        }
                                    }
                                }
                                this._menuOpen = false;
                                if (!this._editMode)
                                {
                                    foreach (ContextMenu contextMenu in this.things[typeof(ContextMenu)])
                                    {
                                        if (contextMenu.visible && contextMenu.opened)
                                        {
                                            this.clicked = false;
                                            this._menuOpen = true;
                                        }
                                    }
                                }
                                if (Editor.inputMode == EditorInput.Gamepad)
                                    Editor._input = InputProfile.active;
                                if (this._prevEditTilePos != this._editTilePos)
                                {
                                    if (_editTilePos.x < 0f)
                                        this._editTilePos.x = 0f;
                                    if (_editTilePos.x >= Editor._procTilesWide)
                                        this._editTilePos.x = Editor._procTilesWide - 1;
                                    if (_editTilePos.y < 0f)
                                        this._editTilePos.y = 0f;
                                    if (_editTilePos.y >= Editor._procTilesHigh)
                                        this._editTilePos.y = Editor._procTilesHigh - 1;
                                    if (this._currentMapNode != null)
                                    {
                                        RandomLevelData data = this._currentMapNode.map[(int)this._editTilePos.x, (int)this._editTilePos.y].data;
                                        if (this._levelThings.Count > 0)
                                            this.Save();
                                        this._looseClear = true;
                                        if (data == null)
                                        {
                                            this.ClearEverything();
                                            this._saveName = "";
                                        }
                                        else
                                            this.LoadLevel(Directory.GetCurrentDirectory() + "\\..\\..\\..\\assets\\levels\\" + data.file + ".lev");
                                        Editor._procXPos = (int)this._editTilePos.x;
                                        Editor._procYPos = (int)this._editTilePos.y;
                                        this._genTilePos = new Vec2(_procXPos, _procYPos);
                                        this._prevEditTilePos = this._editTilePos;
                                        int num1 = 144;
                                        int num2 = 192;
                                        this._procDrawOffset += new Vec2((Editor._procXPos - this._prevProcX) * num2, (Editor._procYPos - this._prevProcY) * num1);
                                        this._prevProcX = Editor._procXPos;
                                        this._prevProcY = Editor._procYPos;
                                    }
                                }
                                if (Editor._procXPos != this._prevProcX)
                                    this._doGen = true;
                                else if (Editor._procYPos != this._prevProcY)
                                    this._doGen = true;
                                this._prevEditTilePos = this._editTilePos;
                                this._prevProcX = Editor._procXPos;
                                this._prevProcY = Editor._procYPos;
                                if (this._miniMode && (Keyboard.Pressed(Keys.F1) || this._doGen) && !this._doingResave)
                                {
                                    if (this._saveName == "")
                                        this._saveName = Editor._initialDirectory + "/pyramid/" + Guid.NewGuid().ToString() + ".lev";
                                    LevelGenerator.ReInitialize();
                                    LevelGenerator.complexity = this.generatorComplexity;
                                    if (!Keyboard.Down(Keys.RightShift) && !Keyboard.Down(Keys.LeftShift))
                                        this._procSeed = Rando.Int(2147483646);
                                    string str = this._saveName.Substring(this._saveName.LastIndexOf("assets/levels/") + "assets/levels/".Length);
                                    string realName = str.Substring(0, str.Length - 4);
                                    RandomLevelData tile = LevelGenerator.LoadInTile(this.SaveTempVersion(), realName);
                                    this._loadPosX = Editor._procXPos;
                                    this._loadPosY = Editor._procYPos;
                                    LevGenType type = LevGenType.Any;
                                    if (Editor._currentLevelData.proceduralData.enableSingle && !Editor._currentLevelData.proceduralData.enableMulti)
                                        type = LevGenType.SinglePlayer;
                                    else if (!Editor._currentLevelData.proceduralData.enableSingle && Editor._currentLevelData.proceduralData.enableMulti)
                                        type = LevGenType.Deathmatch;
                                    this._editTilePos = this._prevEditTilePos = this._genTilePos;
                                    int num3 = 0;
                                    Level level;
                                    while (true)
                                    {
                                        this._currentMapNode = LevelGenerator.MakeLevel(tile, this._pathEast && this._pathWest, this._procSeed, type, Editor._procTilesWide, Editor._procTilesHigh, this._loadPosX, this._loadPosY);
                                        this._procDrawOffset = new Vec2(0f, 0f);
                                        this._procContext = new GameContext();
                                        this._procContext.ApplyStates();
                                        level = new Level
                                        {
                                            backgroundColor = new Color(0, 0, 0, 0)
                                        };
                                        Level.core.currentLevel = level;
                                        RandomLevelNode.editorLoad = true;
                                        int num4 = this._currentMapNode.LoadParts(0f, 0f, level, this._procSeed) ? 1 : 0;
                                        RandomLevelNode.editorLoad = false;
                                        if (num4 == 0 && num3 <= 100)
                                            ++num3;
                                        else
                                            break;
                                    }
                                    level.CalculateBounds();
                                    this._procContext.RevertStates();
                                    this._doGen = false;
                                }
                                this._looseClear = false;
                                Vec2 vec2_1;
                                if (Editor.inputMode == EditorInput.Touch)
                                {
                                    if (!this._twoFingerGestureStarting && TouchScreen.GetTouches().Count == 2)
                                    {
                                        this._twoFingerGestureStarting = true;
                                        this._panAnchor = TouchScreen.GetAverageOfTouches().positionHUD;
                                        this._twoFingerSpacing = (TouchScreen.GetTouches()[0].positionHUD - TouchScreen.GetTouches()[1].positionHUD).length;
                                    }
                                    else if (TouchScreen.GetTouches().Count != 2)
                                    {
                                        this._twoFingerGesture = false;
                                        this._twoFingerGestureStarting = false;
                                    }
                                    if (this._twoFingerGestureStarting && TouchScreen.GetTouches().Count == 2 && !this._twoFingerGesture)
                                    {
                                        vec2_1 = this._panAnchor - TouchScreen.GetAverageOfTouches().positionHUD;
                                        if (vec2_1.length > 6.0)
                                        {
                                            this._twoFingerZooming = false;
                                            this._twoFingerGesture = true;
                                        }
                                        else
                                        {
                                            double twoFingerSpacing = _twoFingerSpacing;
                                            vec2_1 = TouchScreen.GetTouches()[0].positionHUD - TouchScreen.GetTouches()[1].positionHUD;
                                            double length = vec2_1.length;
                                            if (Math.Abs((float)(twoFingerSpacing - length)) > 4.0)
                                            {
                                                this._twoFingerZooming = true;
                                                this._twoFingerGesture = true;
                                            }
                                        }
                                    }
                                    if (!this._threeFingerGestureRelease && TouchScreen.GetTouches().Count == 3)
                                    {
                                        this._threeFingerGesture = true;
                                        this._threeFingerGestureRelease = true;
                                    }
                                    else if (TouchScreen.GetTouches().Count != 3)
                                    {
                                        this._threeFingerGesture = false;
                                        this._threeFingerGestureRelease = false;
                                    }
                                }
                                if (Editor.inputMode == EditorInput.Mouse && Mouse.middle == InputState.Pressed)
                                    this._panAnchor = Mouse.position;
                                if (this._procContext != null)
                                    this._procContext.Update();
                                if (this.tabletMode && this.clicked)
                                {
                                    if (Mouse.x < 32.0 && Mouse.y < 32.0)
                                    {
                                        this._placementMode = true;
                                        this._editMode = false;
                                        this.clicked = false;
                                        return;
                                    }
                                    if (Mouse.x < 64.0 && Mouse.y < 32.0)
                                    {
                                        this._placementMode = false;
                                        this._editMode = true;
                                        this.clicked = false;
                                        return;
                                    }
                                    if (Mouse.x < 96.0 && Mouse.y < 32.0)
                                    {
                                        if (this._placementMenu == null)
                                            this._showPlacementMenu = true;
                                        else
                                            this.CloseMenu();
                                        this.clicked = false;
                                        return;
                                    }
                                }
                                if (this._editorLoadFinished)
                                {
                                    foreach (Thing levelThing in this._levelThings)
                                        levelThing.OnEditorLoaded();
                                    foreach (PathNode pathNode in this.things[typeof(PathNode)])
                                    {
                                        pathNode.UninitializeLinks();
                                        pathNode.Update();
                                    }
                                    this._editorLoadFinished = false;
                                }
                                this.things.RefreshState();
                                if (this._placeObjects.Count > 0)
                                {
                                    foreach (Thing placeObject in this._placeObjects)
                                    {
                                        foreach (Thing thing in Level.CheckRectAll<IDontMove>(placeObject.topLeft + new Vec2(-16f, -16f), placeObject.bottomRight + new Vec2(16f, 16f)))
                                            thing.EditorObjectsChanged();
                                    }
                                    this.things.CleanAddList();
                                    this._placeObjects.Clear();
                                }
                                if (this._placementMenu != null && Editor.inputMode == EditorInput.Mouse && Mouse.right == InputState.Released)
                                {
                                    this._placementMenu.Disappear();
                                    this.CloseMenu();
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
                                        this._selection.Clear();
                                        this._currentDragSelectionHover.Clear();
                                        foreach (Thing levelThing in this._levelThings)
                                            levelThing.EditorObjectsChanged();
                                    }
                                }
                                if (Editor.inputMode == EditorInput.Gamepad && this._placementMenu == null)
                                {
                                    if (Editor._input.Pressed("STRAFE"))
                                    {
                                        History.Undo();
                                        this._selection.Clear();
                                        this._currentDragSelectionHover.Clear();
                                        foreach (Thing levelThing in this._levelThings)
                                            levelThing.EditorObjectsChanged();
                                    }
                                    if (Editor._input.Pressed("RAGDOLL"))
                                    {
                                        History.Redo();
                                        this._selection.Clear();
                                        this._currentDragSelectionHover.Clear();
                                        foreach (Thing levelThing in this._levelThings)
                                            levelThing.EditorObjectsChanged();
                                    }
                                }
                                if ((Editor._input.Pressed("MENU2") || this._showPlacementMenu) && this._cursorMode == CursorMode.Normal)
                                {
                                    if (this._placementMenu == null)
                                    {
                                        this._placementMenu = this._objectMenu;
                                        this.OpenMenu(this._placementMenu);
                                        SFX.Play("openClick", 0.4f);
                                    }
                                    else
                                        this.CloseMenu();
                                }
                                if (Editor._clickedTouchButton)
                                {
                                    Editor._clickedTouchButton = false;
                                }
                                else
                                {
                                    if (this._placementType is AutoBlock || this._placementType is PipeTileset)
                                        this.cellSize = 16f;
                                    if (this._cursorMode != CursorMode.Selection && this._placementMenu == null)
                                    {
                                        switch (Editor.inputMode)
                                        {
                                            case EditorInput.Gamepad:
                                                if (Editor._input.Pressed("CANCEL"))
                                                    this._selectionDragStart = this._tilePosition;
                                                if (this._selectionDragStart != Vec2.Zero)
                                                {
                                                    vec2_1 = this._selectionDragStart - this._tilePosition;
                                                    if (vec2_1.length > 4.0)
                                                    {
                                                        this._dragSelectShiftModifier = this._selection.Count != 0;
                                                        this._cursorMode = CursorMode.Selection;
                                                        this._selectionDragEnd = this._tilePosition;
                                                        return;
                                                    }
                                                }
                                                if (Editor._input.Released("CANCEL"))
                                                {
                                                    this._selectionDragStart = Vec2.Zero;
                                                    break;
                                                }
                                                break;
                                            case EditorInput.Mouse:
                                                bool flag1 = Mouse.left == InputState.Pressed && this._dragSelectShiftModifier;
                                                if (this._placementMenu == null && Mouse.right == InputState.Pressed | flag1)
                                                    this._selectionDragStart = Mouse.positionScreen;
                                                if (this._dragSelectShiftModifier && (Mouse.right == InputState.Released || Mouse.left == InputState.Released))
                                                {
                                                    if (this._hover != null)
                                                    {
                                                        this._selection.Add(this._hover);
                                                        this._currentDragSelectionHover.Add(this._hover);
                                                    }
                                                    if (this._secondaryHover != null)
                                                    {
                                                        this._selection.Add(this._secondaryHover);
                                                        this._currentDragSelectionHover.Add(this._secondaryHover);
                                                    }
                                                    this.UpdateSelection(false);
                                                    this._selectionDragStart = Vec2.Zero;
                                                    if (this._selection.Count > 0)
                                                    {
                                                        this._cursorMode = CursorMode.HasSelection;
                                                        return;
                                                    }
                                                }
                                                if (this._selectionDragStart != Vec2.Zero)
                                                {
                                                    vec2_1 = this._selectionDragStart - Mouse.positionScreen;
                                                    if (vec2_1.length > 8.0)
                                                    {
                                                        if (!this._dragSelectShiftModifier)
                                                        {
                                                            this._selection.Clear();
                                                            this._currentDragSelectionHover.Clear();
                                                        }
                                                        this._cursorMode = CursorMode.Selection;
                                                        this._selectionDragEnd = Mouse.positionScreen;
                                                        return;
                                                    }
                                                }
                                                if (Mouse.right == InputState.Released || Mouse.left == InputState.Released)
                                                {
                                                    this._selectionDragStart = Vec2.Zero;
                                                    break;
                                                }
                                                break;
                                        }
                                    }
                                    if ((this._placementMenu == null || this._editMode) && this._hoverMode == 0)
                                    {
                                        this.UpdateHover(placementLayer, this._tilePosition);
                                        bool flag2 = false;
                                        if (Editor.inputMode == EditorInput.Mouse && Mouse.middle == InputState.Released)
                                        {
                                            vec2_1 = this.middleClickPos - Mouse.position;
                                            if (vec2_1.length < 2.0)
                                                flag2 = true;
                                        }
                                        Thing thing = null;
                                        if (this._secondaryHover != null)
                                        {
                                            if (Input.Released("CANCEL") | flag2)
                                            {
                                                Editor.copying = true;
                                                this._eyeDropperSerialized = this._secondaryHover.Serialize();
                                                Editor.copying = false;
                                                this._placementType = Thing.LoadThing(this._eyeDropperSerialized);
                                            }
                                            else if (Input.Pressed("START"))
                                                thing = this._secondaryHover;
                                        }
                                        else if (this._hover != null)
                                        {
                                            if (this._copyMode || Input.Released("CANCEL") | flag2)
                                            {
                                                Editor.copying = true;
                                                this._eyeDropperSerialized = this._hover.Serialize();
                                                Editor.copying = false;
                                                this._placementType = Thing.LoadThing(this._eyeDropperSerialized);
                                                if (Editor.inputMode == EditorInput.Touch)
                                                {
                                                    this.EndCurrentTouchMode();
                                                    return;
                                                }
                                            }
                                            else if (Input.Pressed("START"))
                                                thing = this._hover;
                                        }
                                        else if (this._placementType != null && Input.Pressed("START"))
                                            thing = this._placementType;
                                        if (thing != null)
                                        {
                                            Editor.ignorePinning = true;
                                            Editor.reopenContextMenu = true;
                                            Editor.openContextThing = thing;
                                        }
                                        TileButton tileButton = this.CollisionPoint<TileButton>(this._tilePosition);
                                        if (tileButton != null)
                                        {
                                            if (!tileButton.visible)
                                            {
                                                tileButton = null;
                                            }
                                            else
                                            {
                                                tileButton.hover = true;
                                                if (Editor.inputMode == EditorInput.Mouse)
                                                    Editor.hoverMiniButton = true;
                                                tileButton.focus = Editor.inputMode == EditorInput.Gamepad && Editor._input.Down("SELECT") || Editor.inputMode == EditorInput.Mouse && (Mouse.left == InputState.Down || Mouse.left == InputState.Pressed) || Editor.inputMode == EditorInput.Touch && TouchScreen.IsScreenTouched() ? Editor._input : null;
                                            }
                                        }
                                        if (tileButton != this._hoverButton && this._hoverButton != null)
                                            this._hoverButton.focus = null;
                                        this._hoverButton = tileButton;
                                    }
                                    if (Editor.inputMode == EditorInput.Mouse)
                                    {
                                        int right = (int)Mouse.right;
                                    }
                                    if (this._cursorMode == CursorMode.Normal)
                                    {
                                        if (this._hoverMenu != null && !this._placingTiles && (Editor.inputMode == EditorInput.Mouse && Mouse.right == InputState.Released || Editor._input.Pressed("MENU1") && !Editor._input.Down("SELECT")))
                                        {
                                            if (this._placementMenu == null)
                                            {
                                                if (this._hover != null)
                                                {
                                                    this._placementMenu = this._hover.GetContextMenu();
                                                    if (this._placementMenu != null)
                                                        this.AddThing(_placementMenu);
                                                }
                                                else if (this._secondaryHover != null)
                                                {
                                                    this._placementMenu = this._secondaryHover.GetContextMenu();
                                                    if (this._placementMenu != null)
                                                        this.AddThing(_placementMenu);
                                                }
                                                if (this._placementMenu != null)
                                                {
                                                    this.OpenMenu(this._placementMenu);
                                                    SFX.Play("openClick", 0.4f);
                                                }
                                            }
                                            else if (Editor.inputMode == EditorInput.Mouse && Mouse.right == InputState.Pressed)
                                                this.CloseMenu();
                                        }
                                        if (Editor.hoverMiniButton)
                                        {
                                            this._tilePosition.x = (float)Math.Round(Mouse.positionScreen.x / this._cellSize) * this._cellSize;
                                            this._tilePosition.y = (float)Math.Round(Mouse.positionScreen.y / this._cellSize) * this._cellSize;
                                            Editor.hoverMiniButton = false;
                                            return;
                                        }
                                        if (this._hoverMenu == null && Editor.inputMode == EditorInput.Mouse && Mouse.right == InputState.Released)
                                        {
                                            if (this._hover is BackgroundTile)
                                            {
                                                if (this._placingTiles && this._placementMenu == null)
                                                {
                                                    int frame = this._placementType.frame;
                                                    this._placementMenu = new ContextBackgroundTile(this._placementType, null, false);
                                                    this._placementMenu.opened = true;
                                                    SFX.Play("openClick", 0.4f);
                                                    this._placementMenu.x = 16f;
                                                    this._placementMenu.y = 16f;
                                                    this._placementMenu.selectedIndex = frame;
                                                    Level.Add(_placementMenu);
                                                }
                                            }
                                            else if (this._placementMenu == null)
                                            {
                                                this._placementMenu = this._objectMenu;
                                                this.OpenMenu(this._placementMenu);
                                                SFX.Play("openClick", 0.4f);
                                            }
                                            else
                                                this.CloseMenu();
                                        }
                                    }
                                    if (this._cursorMode == CursorMode.Normal)
                                    {
                                        if (Editor.inputMode == EditorInput.Gamepad && Editor._input.Pressed("CANCEL") && this._placementMenu != null)
                                            this.CloseMenu();
                                        if (this._placementType != null && this._objectMenu != null)
                                        {
                                            this.rotateValid = this._placementType._canFlip || this._placementType.editorCycleType != null;
                                            if (Editor._input.Pressed("RSTICK") || Keyboard.Pressed(Keys.Tab))
                                            {
                                                if (this._placementType.editorCycleType != null)
                                                {
                                                    this._placementType = this._objectMenu.GetPlacementType(this._placementType.editorCycleType);
                                                    this._eyeDropperSerialized = null;
                                                }
                                                else
                                                {
                                                    Thing thing = this._eyeDropperSerialized != null ? Thing.LoadThing(this._eyeDropperSerialized) : Editor.CreateThing(this._placementType.GetType());
                                                    thing.TabRotate();
                                                    this._placementType = thing;
                                                    this._eyeDropperSerialized = thing.Serialize();
                                                }
                                            }
                                        }
                                    }
                                    float num5 = 0f;
                                    if (Editor.inputMode == EditorInput.Mouse)
                                        num5 = Mouse.scroll;
                                    else if (Editor.inputMode == EditorInput.Touch && this._twoFingerGesture && this._twoFingerZooming)
                                    {
                                        vec2_1 = TouchScreen.GetTouches()[0].positionHUD - TouchScreen.GetTouches()[1].positionHUD;
                                        float length = vec2_1.length;
                                        if (Math.Abs(length - this._twoFingerSpacing) > 2.0)
                                            num5 = (float)(-(length - _twoFingerSpacing) * 1.0);
                                        this._twoFingerSpacing = length;
                                    }
                                    if (Editor.inputMode == EditorInput.Gamepad)
                                    {
                                        num5 = Editor._input.leftTrigger - Editor._input.rightTrigger;
                                        float num6 = (float)(this.camera.width / MonoMain.screenWidth * 5.0);
                                        if (Editor._input.Down("LSTICK"))
                                            num6 *= 2f;
                                        if (Editor._input.Pressed("LOPTION"))
                                            this.cellSize = this.cellSize >= 10.0 ? 8f : 16f;
                                        if (num6 < 5.0)
                                            num6 = 5f;
                                        this.camera.x += Editor._input.rightStick.x * num6;
                                        this.camera.y -= Editor._input.rightStick.y * num6;
                                    }
                                    if (num5 != 0.0 && !Editor.didUIScroll && !Editor.hoverUI)
                                    {
                                        int num7 = Math.Sign(num5);
                                        double num8 = this.camera.height / this.camera.width;
                                        float num9 = num7 * 64f;
                                        switch (Editor.inputMode)
                                        {
                                            case EditorInput.Gamepad:
                                                num9 = num5 * 32f;
                                                break;
                                            case EditorInput.Touch:
                                                num9 = num5;
                                                break;
                                        }
                                        Vec2 vec2_2 = new Vec2(this.camera.width, this.camera.height);
                                        Vec2 vec2_3 = this.camera.transformScreenVector(Mouse.mousePos);
                                        if (Editor.inputMode == EditorInput.Touch && this._twoFingerGesture)
                                            vec2_3 = TouchScreen.GetAverageOfTouches().positionCamera;
                                        if (Editor.inputMode == EditorInput.Gamepad)
                                            vec2_3 = this._tilePosition;
                                        this.camera.width += num9;
                                        if (this.camera.width < 64.0)
                                            this.camera.width = 64f;
                                        this.camera.height = this.camera.width / Resolution.current.aspect;
                                        Vec2 position = this.camera.position;
                                        Vec3 translation;
                                        (Matrix.CreateTranslation(new Vec3(position.x, position.y, 0f)) * Matrix.CreateTranslation(new Vec3(-vec2_3.x, -vec2_3.y, 0f)) * Matrix.CreateScale(this.camera.width / vec2_2.x, this.camera.height / vec2_2.y, 1f) * Matrix.CreateTranslation(new Vec3(vec2_3.x, vec2_3.y, 0f))).Decompose(out Vec3 _, out Quaternion _, out translation);
                                        this.camera.position = new Vec2(translation.x, translation.y);
                                    }
                                    Editor.didUIScroll = false;
                                    switch (Editor.inputMode)
                                    {
                                        case EditorInput.Mouse:
                                            if (Mouse.middle == InputState.Pressed)
                                                this._panAnchor = Mouse.position;
                                            if (Mouse.middle == InputState.Down)
                                            {
                                                Vec2 vec2_4 = Mouse.position - this._panAnchor;
                                                this._panAnchor = Mouse.position;
                                                float num10 = this.camera.width / Layer.HUD.width;
                                                if (vec2_4.length > 0.01)
                                                    this._didPan = true;
                                                this.camera.x -= vec2_4.x * num10;
                                                this.camera.y -= vec2_4.y * num10;
                                            }
                                            if (Mouse.middle == InputState.Released)
                                            {
                                                int num11 = this._didPan ? 1 : 0;
                                                this._didPan = false;
                                                break;
                                            }
                                            break;
                                        case EditorInput.Touch:
                                            if (this._twoFingerGesture && !this._twoFingerZooming)
                                            {
                                                Vec2 vec2_5 = TouchScreen.GetAverageOfTouches().positionHUD - this._panAnchor;
                                                this._panAnchor = TouchScreen.GetAverageOfTouches().positionHUD;
                                                float num12 = this.camera.width / Layer.HUD.width;
                                                if (vec2_5.length > 0.1)
                                                {
                                                    this._didPan = true;
                                                    this.camera.x -= vec2_5.x * num12;
                                                    this.camera.y -= vec2_5.y * num12;
                                                    break;
                                                }
                                                break;
                                            }
                                            break;
                                    }
                                    bool flag3 = Keyboard.Down(Keys.LeftAlt) || Keyboard.Down(Keys.RightAlt);
                                    bool flag4 = Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl);
                                    bool flag5 = false;
                                    if (flag3 & flag4)
                                    {
                                        this._hover = null;
                                        this._secondaryHover = null;
                                        flag5 = true;
                                    }
                                    if ((Editor.inputMode == EditorInput.Gamepad || Editor.inputMode == EditorInput.Touch) && this._placementMenu == null)
                                    {
                                        int num13 = 1;
                                        if (Editor._input.Down("LSTICK"))
                                            num13 = 4;
                                        this._tilePosition = this._tilePositionPrev;
                                        if (_tilePosition.x < this.camera.left)
                                            this._tilePosition.x = this.camera.left + 32f;
                                        if (_tilePosition.x > this.camera.right)
                                            this._tilePosition.x = this.camera.right - 32f;
                                        if (_tilePosition.y < this.camera.top)
                                            this._tilePosition.y = this.camera.top + 32f;
                                        if (_tilePosition.y > this.camera.bottom)
                                            this._tilePosition.y = this.camera.bottom - 32f;
                                        int num14 = 0;
                                        int num15 = 0;
                                        if (this._hoverMode == 0 && (this._hoverButton == null || this._hoverButton.focus == null))
                                        {
                                            if (Editor._input.Pressed("MENULEFT"))
                                                num15 = -1;
                                            if (Editor._input.Pressed("MENURIGHT"))
                                                num15 = 1;
                                            if (Editor._input.Pressed("MENUUP"))
                                                num14 = -1;
                                            if (Editor._input.Pressed("MENUDOWN"))
                                                num14 = 1;
                                        }
                                        float num16 = this._cellSize * num13 * num15;
                                        float num17 = this._cellSize * num13 * num14;
                                        this._tilePosition.x += num16;
                                        this._tilePosition.y += num17;
                                        if (_tilePosition.x < this.camera.left || _tilePosition.x > this.camera.right)
                                            this.camera.x += num16;
                                        if (_tilePosition.y < this.camera.top || _tilePosition.y > this.camera.bottom)
                                            this.camera.y += num17;
                                        if (TouchScreen.GetTouch() != Touch.None)
                                        {
                                            this._tilePosition.x = (float)Math.Round(TouchScreen.GetTouch().positionCamera.x / this._cellSize) * this._cellSize;
                                            this._tilePosition.y = (float)Math.Round(TouchScreen.GetTouch().positionCamera.y / this._cellSize) * this._cellSize;
                                            this._tilePositionPrev = this._tilePosition;
                                        }
                                        else
                                        {
                                            this._tilePosition.x = (float)Math.Round(_tilePosition.x / this._cellSize) * this._cellSize;
                                            this._tilePosition.y = (float)Math.Round(_tilePosition.y / this._cellSize) * this._cellSize;
                                            this._tilePositionPrev = this._tilePosition;
                                        }
                                    }
                                    else if (Editor.inputMode == EditorInput.Mouse)
                                    {
                                        if (flag3)
                                        {
                                            this._tilePosition.x = (float)Math.Round(Mouse.positionScreen.x / 1.0) * 1f;
                                            this._tilePosition.y = (float)Math.Round(Mouse.positionScreen.y / 1.0) * 1f;
                                        }
                                        else
                                        {
                                            this._tilePosition.x = (float)Math.Round(Mouse.positionScreen.x / this._cellSize) * this._cellSize;
                                            this._tilePosition.y = (float)Math.Round(Mouse.positionScreen.y / this._cellSize) * this._cellSize;
                                        }
                                    }
                                    if (this._placementType != null && this._placementMenu == null)
                                    {
                                        this._tilePosition += this._placementType.editorOffset;
                                        if (!flag3)
                                            this.HugObjectPlacement();
                                    }
                                    if (this._move != null)
                                        this._move.position = new Vec2(this._tilePosition);
                                    this.UpdateDragSelection();
                                    if (!this._editMode && !this._copyMode && this._cursorMode == CursorMode.Normal && !this._dragSelectShiftModifier && this._placementMenu == null)
                                    {
                                        this.bMouseInput = false;
                                        this.bGamepadInput = false;
                                        this.bTouchInput = false;
                                        if (Editor.inputMode == EditorInput.Mouse && Mouse.left == InputState.Pressed)
                                        {
                                            this.bMouseInput = true;
                                            this.dragModeInputType = InputType.eMouse;
                                        }
                                        else if (Editor.inputMode == EditorInput.Gamepad && Editor._input.Pressed("SELECT"))
                                        {
                                            this.bGamepadInput = true;
                                            this.dragModeInputType = InputType.eGamepad;
                                        }
                                        else if (Editor.inputMode == EditorInput.Touch && TouchScreen.GetDrag() != Touch.None || TouchScreen.GetTap() != Touch.None)
                                        {
                                            this.bTouchInput = true;
                                            this.dragModeInputType = InputType.eTouch;
                                        }
                                        if (!this._dragMode && (this.bMouseInput || this.bGamepadInput || this.bTouchInput) && this._placementMode && this._hoverMode == 0 && (this._hoverButton == null || this._hoverButton.focus == null))
                                        {
                                            this.firstClick = true;
                                            this._dragMode = true;
                                            History.BeginUndoSection();
                                            Thing hover = this._hover;
                                            if (hover != null && (!(this._hover is BackgroundTile) || this._placementType != null && this._hover.GetType() == this._placementType.GetType()))
                                            {
                                                if ((Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl)) && !(this._placementType is BackgroundTile))
                                                    this._move = hover;
                                                else if (!Keyboard.control)
                                                    this._deleteMode = true;
                                            }
                                        }
                                        if (this._dragMode)
                                        {
                                            if (this._tileDragDif == Vec2.MaxValue || Editor.inputMode == EditorInput.Gamepad)
                                                this._tileDragDif = this._tilePosition;
                                            Vec2 vec2_6 = Maths.Snap(this._tilePosition, this._cellSize, this._cellSize);
                                            Vec2 vec2_7 = this._tilePosition;
                                            Vec2 vec2_8 = Vec2.MaxValue;
                                            do
                                            {
                                                Vec2 vec2_9 = Maths.Snap(vec2_7, this._cellSize, this._cellSize);
                                                if ((Keyboard.control || Input.Down("SELECT") && Input.Down("MENU1")) && this._tileDragContext == Vec2.MinValue)
                                                    this._tileDragContext = vec2_9;
                                                if (!(vec2_9 == Maths.Snap(this._tileDragDif, this._cellSize, this._cellSize)) || !(vec2_9 != Maths.Snap(this._tilePosition, this._cellSize, this._cellSize)))
                                                {
                                                    if (vec2_6 != this._tilePosition)
                                                    {
                                                        vec2_9 = this._tilePosition;
                                                        this._tileDragDif = this._tilePosition;
                                                    }
                                                    vec2_7 = Lerp.Vec2(vec2_7, this._tileDragDif, this._cellSize);
                                                    if (this._tileDragDif != this._tilePosition)
                                                        this.UpdateHover(placementLayer, vec2_9, true);
                                                    if (!this._deleteMode && this._placementType != null)
                                                    {
                                                        Thing thing = this._hover;
                                                        if (thing == null && !(this._placementType is BackgroundTile))
                                                            thing = this.CollisionPointFilter<Thing>(vec2_9, x =>
                                                           {
                                                               if (x.placementLayer != placementLayer)
                                                                   return false;
                                                               return !(this._placementType is PipeTileset) || x.GetType() == this._placementType.GetType();
                                                           });
                                                        if (thing is TileButton)
                                                            thing = null;
                                                        else if (thing != null && !this._levelThings.Contains(thing))
                                                            thing = null;
                                                        else if (flag4 & flag3)
                                                            thing = null;
                                                        else if (this._placementType is BackgroundTile && !(thing is BackgroundTile))
                                                            thing = null;
                                                        else if (this.firstClick && this._hover == null)
                                                            thing = null;
                                                        this.firstClick = false;
                                                        if ((thing == null || this._placementType is WireTileset && thing is IWirePeripheral || this._placementType is IWirePeripheral && thing is WireTileset) && !this.placementLimitReached && !this.placementOutOfSizeRange && vec2_8 != vec2_9)
                                                        {
                                                            vec2_8 = vec2_9;
                                                            System.Type type = this._placementType.GetType();
                                                            Thing newThing = null;
                                                            newThing = this._eyeDropperSerialized != null ? Thing.LoadThing(this._eyeDropperSerialized) : Editor.CreateThing(type);
                                                            newThing.x = vec2_9.x;
                                                            newThing.y = vec2_9.y;
                                                            if (this._placementType is SubBackgroundTile)
                                                                (newThing.graphic as SpriteMap).frame = ((this._placementType as SubBackgroundTile).graphic as SpriteMap).frame;
                                                            if (this._placementType is BackgroundTile)
                                                            {
                                                                int num18 = (int)((vec2_9.x - this._tileDragContext.x) / 16.0);
                                                                int num19 = (int)((vec2_9.y - this._tileDragContext.y) / 16.0);
                                                                (newThing as BackgroundTile).frame = (this._placementType as BackgroundTile).frame + num18 + (int)(num19 * (newThing.graphic.texture.width / 16.0));
                                                            }
                                                            else if (this._placementType is ForegroundTile)
                                                                (newThing.graphic as SpriteMap).frame = ((this._placementType as ForegroundTile).graphic as SpriteMap).frame;
                                                            if (this._hover is BackgroundTile)
                                                                newThing.depth = this._hover.depth + 1;
                                                            History.Add(() => this.AddObject(newThing), () => this.RemoveObject(newThing));
                                                            if (newThing is PathNode)
                                                                this._editorLoadFinished = true;
                                                            if (flag5)
                                                                this.disableDragMode();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Thing col = this._hover;
                                                        if (col != null)
                                                        {
                                                            History.Add(() => this.RemoveObject(col), () => this.AddObject(col));
                                                            if (col is PathNode)
                                                                this._editorLoadFinished = true;
                                                            this._hover = null;
                                                        }
                                                    }
                                                    this.things.RefreshState();
                                                    vec2_1 = vec2_7 - this._tileDragDif;
                                                }
                                                else
                                                    break;
                                            }
                                            while (vec2_1.length > 2.0);
                                        }
                                        if (Mouse.left == InputState.Released && this.dragModeInputType == InputType.eMouse || Editor._input.Released("SELECT") && this.dragModeInputType == InputType.eGamepad || TouchScreen.GetRelease() != Touch.None && this.dragModeInputType == InputType.eTouch)
                                            this.disableDragMode();
                                    }
                                    if (!Keyboard.control && !Input.Down("MENU1"))
                                        this._tileDragContext = Vec2.MinValue;
                                    this._tileDragDif = this._tilePosition;
                                    this._placingTiles = false;
                                    if (this._placementType is BackgroundTile)
                                        this._placingTiles = true;
                                    if (this._placingTiles && this._placementMenu == null && (Editor._input.Pressed("MENU1") && !Editor._input.Down("SELECT") || this._openTileSelector) && this._cursorMode == CursorMode.Normal)
                                    {
                                        this.DoMenuClose();
                                        int frame = this._placementType.frame;
                                        this._placementMenu = new ContextBackgroundTile(this._placementType, null, false)
                                        {
                                            positionCursor = true
                                        };
                                        this._placementMenu.opened = true;
                                        SFX.Play("openClick", 0.4f);
                                        this._placementMenu.x = 16f;
                                        this._placementMenu.y = 16f;
                                        this._placementMenu.selectedIndex = frame;
                                        Level.Add(_placementMenu);
                                        this._openTileSelector = false;
                                    }
                                    if (this._editMode && this._cursorMode == CursorMode.Normal)
                                    {
                                        if (this._twoFingerGesture || this._threeFingerGesture)
                                            this.DoMenuClose();
                                        if (this.clicked && this._hover != null)
                                        {
                                            this.DoMenuClose();
                                            this._placementMenu = this._hover.GetContextMenu();
                                            if (this._placementMenu != null)
                                            {
                                                this._placementMenu.x = 96f;
                                                this._placementMenu.y = 32f;
                                                if (Editor.inputMode == EditorInput.Gamepad || Editor.inputMode == EditorInput.Touch)
                                                {
                                                    this._placementMenu.x = 16f;
                                                    this._placementMenu.y = 16f;
                                                }
                                                this._openedEditMenu = true;
                                                this.AddThing(_placementMenu);
                                                this._placementMenu.opened = true;
                                                SFX.Play("openClick", 0.4f);
                                                this.clicked = false;
                                                this._oldHover = this._hover;
                                                this._lastHoverMenuOpen = this._placementMenu;
                                            }
                                        }
                                    }
                                    Editor.hoverUI = false;
                                    if (this._closeMenu)
                                        this.DoMenuClose();
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
            if (this._placementMenu != null)
            {
                if (this._editMode && Editor.clickedMenu)
                    this._hover = this._oldHover;
                if (Editor.inputMode == EditorInput.Touch && TouchScreen.GetTap() != Touch.None && !Editor.clickedMenu && !Editor.clickedContextBackground && !this._openedEditMenu)
                {
                    if (this._touchState == Editor.EditorTouchState.OpenMenu)
                        this.EndCurrentTouchMode();
                    this._showPlacementMenu = false;
                    this.CloseMenu();
                }
            }
            if (this._touchState == Editor.EditorTouchState.OpenMenu && this._placementMenu == null)
            {
                this._touchState = Editor.EditorTouchState.Normal;
                this._activeTouchButton = null;
            }
            this._openedEditMenu = false;
        }

        public void DoMenuClose()
        {
            if (this._placementMenu != null)
            {
                if (this._placementMenu != this._objectMenu)
                {
                    this.RemoveThing(_placementMenu);
                }
                else
                {
                    this._placementMenu.visible = false;
                    this._placementMenu.active = false;
                    this._placementMenu.opened = false;
                }
            }
            this._placementMenu = null;
            this._closeMenu = false;
        }

        private void UpdateSelection(bool pObjectsChanged = true)
        {
            foreach (Thing levelThing in this._levelThings)
            {
                if (pObjectsChanged)
                    levelThing.EditorObjectsChanged();
                levelThing.material = null;
            }
            foreach (Thing thing in Level.current.things)
                thing.material = null;
            foreach (Thing thing in this._selection)
            {
                switch (thing)
                {
                    case AutoBlock _:
                        AutoBlock autoBlock = thing as AutoBlock;
                        if (autoBlock._bLeftNub != null)
                            this._currentDragSelectionHover.Add(autoBlock._bLeftNub);
                        if (autoBlock._bRightNub != null)
                        {
                            this._currentDragSelectionHover.Add(autoBlock._bRightNub);
                            continue;
                        }
                        continue;
                    case AutoPlatform _:
                        AutoPlatform autoPlatform = thing as AutoPlatform;
                        if (autoPlatform._leftNub != null)
                            this._currentDragSelectionHover.Add(autoPlatform._leftNub);
                        if (autoPlatform._rightNub != null)
                        {
                            this._currentDragSelectionHover.Add(autoPlatform._rightNub);
                            continue;
                        }
                        continue;
                    case Door _:
                        Door door = thing as Door;
                        if (door._frame != null)
                        {
                            this._currentDragSelectionHover.Add(door._frame);
                            continue;
                        }
                        continue;
                    case ItemSpawner _:
                        ItemSpawner itemSpawner = thing as ItemSpawner;
                        if (itemSpawner._ball1 != null)
                            this._currentDragSelectionHover.Add(itemSpawner._ball1);
                        if (itemSpawner._ball2 != null)
                        {
                            this._currentDragSelectionHover.Add(itemSpawner._ball2);
                            continue;
                        }
                        continue;
                    default:
                        continue;
                }
            }
            foreach (Thing thing in this._currentDragSelectionHover)
                thing.material = _selectionMaterial;
            foreach (Thing thing in this._currentDragSelectionHoverAdd)
                thing.material = !this._currentDragSelectionHover.Contains(thing) ? _selectionMaterial : (Material)null;
        }

        private void RebuildPasteBatch()
        {
            this._pasteBatch.Clear();
            foreach (BinaryClassChunk node in this._selectionCopy)
                this._pasteBatch.Add(Thing.LoadThing(node));
        }

        private void UpdateDragSelection()
        {
            this._dragSelectShiftModifier = Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift) || Editor.inputMode == EditorInput.Gamepad && this._selection.Count > 0;
            if (this._cursorMode == CursorMode.Selection)
            {
                this._selectionDragEnd = Editor.inputMode == EditorInput.Mouse ? Mouse.positionScreen : this._tilePosition;
                Vec2 selectionDragStart = this._selectionDragStart;
                Vec2 selectionDragEnd = this._selectionDragEnd;
                if (selectionDragEnd.x < selectionDragStart.x)
                {
                    (selectionDragEnd.x, selectionDragStart.x) = (selectionDragStart.x, selectionDragEnd.x);
                }
                if (selectionDragEnd.y < selectionDragStart.y)
                {
                    (selectionDragEnd.y, selectionDragStart.y) = (selectionDragStart.y, selectionDragEnd.y);
                }
                if (this._dragSelectShiftModifier)
                {
                    this._currentDragSelectionHoverAdd.Clear();
                    foreach (Thing thing in Level.CheckRectAll<Thing>(selectionDragStart, selectionDragEnd))
                        this._currentDragSelectionHoverAdd.Add(thing);
                }
                else
                {
                    this._currentDragSelectionHover.Clear();
                    foreach (Thing thing in Level.CheckRectAll<Thing>(selectionDragStart, selectionDragEnd))
                        this._currentDragSelectionHover.Add(thing);
                }
                if (Mouse.right == InputState.Released || Mouse.left == InputState.Released || Editor.inputMode == EditorInput.Gamepad && Editor._input.Released("CANCEL"))
                {
                    if (this._dragSelectShiftModifier)
                    {
                        foreach (Thing thing in this._currentDragSelectionHoverAdd)
                        {
                            if (this._currentDragSelectionHover.Contains(thing))
                            {
                                this._currentDragSelectionHover.Remove(thing);
                                this._selection.Remove(thing);
                            }
                            else
                                this._currentDragSelectionHover.Add(thing);
                        }
                    }
                    foreach (Thing thing in this._currentDragSelectionHover)
                    {
                        if (!(thing is ContextMenu) && this._levelThings.Contains(thing) && !this._selection.Contains(thing))
                            this._selection.Add(thing);
                    }
                    this._currentDragSelectionHoverAdd.Clear();
                    //this.dragStartInputType = InputType.eNone;
                    this._cursorMode = this._selection.Count > 0 ? CursorMode.HasSelection : CursorMode.Normal;
                    Editor.clickedMenu = true;
                    this._selectionDragStart = Vec2.Zero;
                }
                this.UpdateSelection(false);
            }
            else if (this._cursorMode == CursorMode.Drag)
            {
                Vec2 vec2 = Maths.Snap(Mouse.positionScreen + new Vec2(this._cellSize / 2f), this._cellSize, this._cellSize);
                if (Editor.inputMode == EditorInput.Gamepad)
                    vec2 = Maths.Snap(this._tilePosition + new Vec2(this._cellSize / 2f), this._cellSize, this._cellSize);
                if (vec2 != this._moveDragStart)
                {
                    Vec2 dif = vec2 - this._moveDragStart;
                    this._moveDragStart = vec2;
                    foreach (Thing thing1 in this._currentDragSelectionHover)
                    {
                        Thing t = thing1;
                        History.Add(() =>
                       {
                           Thing thing2 = t;
                           thing2.position += dif;
                           if (!(t is IDontMove))
                               return;
                           Level.current.things.quadTree.Remove(t);
                           Level.current.things.quadTree.Add(t);
                       }, () =>
           {
               Thing thing3 = t;
               thing3.position -= dif;
               if (!(t is IDontMove))
                   return;
               Level.current.things.quadTree.Remove(t);
               Level.current.things.quadTree.Add(t);
           });
                    }
                }
                if (Mouse.left != InputState.Released && !Editor._input.Released("SELECT"))
                    return;
                this._cursorMode = CursorMode.HasSelection;
                this.UpdateSelection();
                History.EndUndoSection();
                Editor.hasUnsavedChanges = true;
            }
            else
            {
                if (this._performCopypaste || (Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl) || this._cursorMode == CursorMode.Pasting) && (this._cursorMode == CursorMode.Normal || this._cursorMode == CursorMode.HasSelection || this._cursorMode == CursorMode.Pasting || this._cursorMode == CursorMode.DragHover))
                {
                    bool flag = Keyboard.Pressed(Keys.X);
                    if (this._selection.Count > 0 && (Keyboard.Pressed(Keys.C) | flag || this._performCopypaste))
                    {
                        this._selectionCopy.Clear();
                        this._copyCenter = Vec2.Zero;
                        History.BeginUndoSection();
                        foreach (Thing thing in this._selection)
                        {
                            Thing t = thing;
                            Editor.copying = true;
                            this._selectionCopy.Add(t.Serialize());
                            this._copyCenter += t.position;
                            Editor.copying = false;
                            if (flag)
                                History.Add(() => this.RemoveObject(t), () => this.AddObject(t));
                        }
                        this._copyCenter /= _selection.Count;
                        if (flag)
                        {
                            this._selection.Clear();
                            this._currentDragSelectionHover.Clear();
                            this.UpdateSelection();
                        }
                        History.EndUndoSection();
                        this.RebuildPasteBatch();
                        HUD.AddPlayerChangeDisplay("@CLIPCOPY@Selection copied!", 1f);
                    }
                    if (Keyboard.Pressed(Keys.V) && this._pasteBatch.Count > 0 || this._performCopypaste)
                    {
                        this._selection.Clear();
                        this._currentDragSelectionHover.Clear();
                        this._cursorMode = CursorMode.Pasting;
                        this.UpdateSelection(false);
                    }
                    this.pasteOffset = Maths.Snap(this._copyCenter - Mouse.positionScreen, 16f, 16f);
                    if (Editor.inputMode == EditorInput.Gamepad)
                        this.pasteOffset = Maths.Snap(this._copyCenter - this._tilePosition, 16f, 16f);
                    this._performCopypaste = false;
                    if (this._cursorMode == CursorMode.Pasting)
                    {
                        if (Mouse.right == InputState.Released || Editor._input.Released("CANCEL") && Editor.inputMode == EditorInput.Gamepad)
                            this._cursorMode = CursorMode.Normal;
                        if (Mouse.left == InputState.Pressed || Editor._input.Pressed("SELECT") && Editor.inputMode == EditorInput.Gamepad)
                        {
                            History.BeginUndoSection();
                            this._selection.Clear();
                            this._currentDragSelectionHover.Clear();
                            this._isPaste = true;
                            foreach (Thing thing4 in this._pasteBatch)
                            {
                                this._selection.Add(thing4);
                                Thing thing5 = thing4;
                                thing5.position -= this.pasteOffset;
                                foreach (Thing thing6 in this.CollisionRectAll<Thing>(thing4.position + new Vec2(-6f, -6f), thing4.position + new Vec2(6f, 6f), null))
                                {
                                    Thing col = thing6;
                                    if (col.placementLayer == thing4.placementLayer && this._levelThings.Contains(col))
                                        History.Add(() => this.RemoveObject(col), () => this.AddObject(col));
                                }
                            }
                            foreach (Thing thing in this._selection)
                            {
                                Thing t = thing;
                                History.Add(() => this.AddObject(t), () => this.RemoveObject(t));
                            }
                            this._selection.Clear();
                            this._currentDragSelectionHover.Clear();
                            this._isPaste = false;
                            this.RebuildPasteBatch();
                            this._placeObjects.Clear();
                            this.things.RefreshState();
                            this.UpdateSelection();
                            this.disableDragMode();
                        }
                    }
                }
                else if (this._cursorMode == CursorMode.Pasting)
                    this._cursorMode = CursorMode.Normal;
                if (this._selection.Count > 0 && this._cursorMode != CursorMode.Pasting && (Keyboard.Pressed(Keys.F) || Editor._input.Pressed("MENU1") && Editor.inputMode == EditorInput.Gamepad))
                {
                    Vec2 zero = Vec2.Zero;
                    Vec2 pPosition;
                    if (this._cursorMode == CursorMode.Pasting)
                    {
                        foreach (Thing thing in this._pasteBatch)
                            zero += thing.position;
                        pPosition = zero / _pasteBatch.Count;
                    }
                    else
                    {
                        foreach (Thing thing in this._selection)
                            zero += thing.position;
                        pPosition = zero / _selection.Count;
                    }
                    Vec2 vec2 = Maths.SnapRound(pPosition, this._cellSize / 2f, this._cellSize / 2f);
                    if (this._cursorMode == CursorMode.Pasting)
                    {
                        foreach (Thing thing in this._pasteBatch)
                        {
                            thing.SetTranslation(new Vec2((float)(-(thing.position.x - vec2.x) * 2.0), 0f));
                            thing.EditorFlip(false);
                            thing.flipHorizontal = !thing.flipHorizontal;
                        }
                    }
                    else
                    {
                        History.BeginUndoSection();
                        foreach (Thing thing in this._selection)
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
                               Level.current.things.quadTree.Remove(t);
                               Level.current.things.quadTree.Add(t);
                           }, () =>
             {
                 t.SetTranslation(new Vec2(dif * 2f, 0f));
                 t.EditorFlip(false);
                 t.flipHorizontal = !t.flipHorizontal;
                 if (!(t is IDontMove))
                     return;
                 Level.current.things.quadTree.Remove(t);
                 Level.current.things.quadTree.Add(t);
             });
                        }
                        this.UpdateSelection();
                        History.EndUndoSection();
                    }
                    this.UpdateSelection();
                }
                if (this._selection.Count > 0)
                {
                    this._cursorMode = CursorMode.HasSelection;
                    switch (Editor.inputMode)
                    {
                        case EditorInput.Gamepad:
                            using (HashSet<Thing>.Enumerator enumerator = this._selection.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    if (Collision.Point(this._tilePosition, enumerator.Current))
                                    {
                                        this._cursorMode = CursorMode.DragHover;
                                        break;
                                    }
                                }
                                break;
                            }
                        case EditorInput.Mouse:
                            using (HashSet<Thing>.Enumerator enumerator = this._selection.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    if (Collision.Point(Mouse.positionScreen, enumerator.Current))
                                    {
                                        this._cursorMode = CursorMode.DragHover;
                                        break;
                                    }
                                }
                                break;
                            }
                    }
                    bool flag = false;
                    if (Keyboard.Pressed(Keys.Delete) || Editor._input.Pressed("MENU2") && Editor.inputMode == EditorInput.Gamepad)
                    {
                        History.BeginUndoSection();
                        foreach (Thing thing in this._selection)
                        {
                            Thing t = thing;
                            History.Add(() => this.RemoveObject(t), () => this.AddObject(t));
                        }
                        this.UpdateSelection();
                        History.EndUndoSection();
                        flag = true;
                    }
                    if (Mouse.left == InputState.Pressed || Editor._input.Pressed("SELECT") && Editor.inputMode == EditorInput.Gamepad)
                    {
                        if (this._cursorMode == CursorMode.DragHover)
                        {
                            History.BeginUndoSection();
                            this._cursorMode = CursorMode.Drag;
                            this._moveDragStart = Editor.inputMode != EditorInput.Gamepad ? Maths.Snap(Mouse.positionScreen + new Vec2(this._cellSize / 2f), this._cellSize, this._cellSize) : Maths.Snap(this._tilePosition + new Vec2(this._cellSize / 2f), this._cellSize, this._cellSize);
                        }
                        else
                            flag = true;
                    }
                    if (Editor._input.Released("CANCEL"))
                    {
                        if (this._cursorMode == CursorMode.DragHover)
                            this._performCopypaste = true;
                        else
                            flag = true;
                    }
                    if (this._cursorMode != CursorMode.Pasting && Mouse.right == InputState.Released | flag && (!this._dragSelectShiftModifier || Editor.inputMode == EditorInput.Gamepad))
                    {
                        this._cursorMode = CursorMode.Normal;
                        this._selection.Clear();
                        this._currentDragSelectionHover.Clear();
                        this.UpdateSelection(false);
                    }
                    Vec2 offset = new Vec2(0f, 0f);
                    if (Keyboard.Pressed(Keys.Up))
                        offset.y -= this.cellSize;
                    if (Keyboard.Pressed(Keys.Down))
                        offset.y += this.cellSize;
                    if (Keyboard.Pressed(Keys.Left))
                        offset.x -= this.cellSize;
                    if (Keyboard.Pressed(Keys.Right))
                        offset.x += this.cellSize;
                    if (!(offset != Vec2.Zero))
                        return;
                    Editor.hasUnsavedChanges = true;
                    History.BeginUndoSection();
                    foreach (Thing thing in this._selection)
                    {
                        Thing t = thing;
                        History.Add(() =>
                       {
                           t.SetTranslation(offset);
                           if (!(t is IDontMove))
                               return;
                           Level.current.things.quadTree.Remove(t);
                           Level.current.things.quadTree.Add(t);
                       }, () =>
           {
               t.SetTranslation(-offset);
               if (!(t is IDontMove))
                   return;
               Level.current.things.quadTree.Remove(t);
               Level.current.things.quadTree.Add(t);
           });
                    }
                    this.UpdateSelection();
                    History.EndUndoSection();
                }
                else
                {
                    if (this._cursorMode != CursorMode.HasSelection)
                        return;
                    this._cursorMode = CursorMode.Normal;
                }
            }
        }

        private void UpdateHover(Layer placementLayer, Vec2 tilePosition, bool isDrag = false)
        {
            IEnumerable<Thing> source1 = new List<Thing>();
            if (Editor.inputMode == EditorInput.Gamepad | isDrag)
                source1 = this.CollisionPointAll<Thing>(tilePosition);
            else if (Editor.inputMode == EditorInput.Touch && TouchScreen.IsScreenTouched())
            {
                if (this._editMode || this._copyMode)
                {
                    if (TouchScreen.GetTap() != Touch.None)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            source1 = this.CollisionCircleAll<Thing>(TouchScreen.GetTap().positionCamera, index * 2f);
                            if (source1.Count<Thing>() > 0)
                                break;
                        }
                        this._hover = null;
                    }
                }
                else if (TouchScreen.GetTouch() != Touch.None)
                    source1 = this.CollisionPointAll<Thing>(tilePosition);
            }
            else if (Editor.inputMode == EditorInput.Mouse && !isDrag)
                source1 = this.CollisionPointAll<Thing>(Mouse.positionScreen);
            this.oldHover = this._hover;
            if (!this._editMode)
                this._hover = null;
            this._secondaryHover = null;
            List<Thing> source2 = new List<Thing>();
            foreach (Thing thing in source1)
            {
                if (!(thing is TileButton) && Editor._placeables.Contains(thing.GetType()) && thing.editorCanModify && this._things.Contains(thing) && (!(this._placementType is WireTileset) || !(thing is IWirePeripheral)) && (!(this._placementType is IWirePeripheral) || !(thing is WireTileset)))
                {
                    if (this._placementType is PipeTileset && thing is PipeTileset && this._placementType.GetType() != thing.GetType())
                        source2.Add(thing);
                    else if (thing.placementLayer != placementLayer && !this._copyMode && !this._editMode)
                        source2.Add(thing);
                    else if (this._hover == null)
                    {
                        if (this._placementType != null && this._placementType is BackgroundTile)
                        {
                            if (this._things.Contains(thing))
                            {
                                if (thing.GetType() == this._placementType.GetType())
                                    this._hover = thing;
                                else
                                    source2.Add(thing);
                            }
                        }
                        else if (thing.editorCanModify)
                            this._hover = thing;
                    }
                    else if (thing != this._hover)
                        source2.Add(thing);
                }
            }
            if (Editor.inputMode == EditorInput.Mouse && !isDrag && this._hover == null && !(this._placementType is BackgroundTile) && !(this._placementType is PipeTileset))
            {
                List<KeyValuePair<float, Thing>> keyValuePairList = Level.current.nearest(tilePosition, this._levelThings.AsEnumerable<Thing>(), null, placementLayer, true);
                if (keyValuePairList.Count > 0 && (!(this._placementType is WireTileset) || !(keyValuePairList[0].Value is IWirePeripheral)) && (!(this._placementType is IWirePeripheral) || !(keyValuePairList[0].Value is WireTileset)) && (keyValuePairList[0].Value.position - tilePosition).length < 8.0)
                    this._hover = keyValuePairList[0].Value;
            }
            if (this._hover == null || this.oldHover == null || this._hover.GetType() != this.oldHover.GetType())
                this._hoverMenu = this._hover != null ? this._hover.GetContextMenu() : null;
            if (source2.Count > 0)
            {
                IOrderedEnumerable<Thing> source3 = source2.OrderBy<Thing, int>(x => x.placementLayer == null ? -99999 : x.placementLayer.depth);
                if (Keyboard.control)
                    this._hover = this._hover != null ? source3.First<Thing>() : source2.OrderBy<Thing, int>(x => x.placementLayer == null ? 99999 : -x.placementLayer.depth).First<Thing>();
                else if (this._hover == null || Keyboard.control || this._placementType != null && source3.First<Thing>().placementLayer == this._placementType.placementLayer)
                {
                    this._secondaryHover = source3.First<Thing>();
                    if (this._hoverMenu == null)
                        this._hoverMenu = this._secondaryHover.GetContextMenu();
                }
            }
            if (this._secondaryHover != null || !(this._hover is Block) || source2.Count <= 0)
                return;
            this._secondaryHover = source2.FirstOrDefault<Thing>(x => x is PipeTileset);
            if (this._secondaryHover == null || (this._secondaryHover as PipeTileset)._foregroundDraw)
                return;
            this._secondaryHover = null;
        }

        public override void Draw() => base.Draw();

        public static bool arcadeMachineMode => Level.current is Editor current && current._levelThings.Count == 1 && current._levelThings[0] is ImportMachine;

        private void CalculateGridRestriction()
        {
            Vec2 vec2 = this._sizeRestriction * 2f - (this._bottomRightMost - this._topLeftMost) - new Vec2(16f, 16f);
            if (vec2.x > _sizeRestriction.x * 2.0)
                vec2.x = this._sizeRestriction.x * 2f;
            if (vec2.y > _sizeRestriction.y * 2.0)
                vec2.y = this._sizeRestriction.y * 2f;
            this._gridW = (int)(vec2.x / this._cellSize);
            this._gridH = (int)(vec2.y / this._cellSize);
        }

        public override void PostDrawLayer(Layer layer)
        {
            base.PostDrawLayer(layer);
            if (layer == Layer.Foreground)
            {
                foreach (Thing thing in this.things)
                    thing.DoEditorRender();
            }
            if (layer == this._procLayer && this._procTarget != null && this._procContext != null)
                DuckGame.Graphics.Draw(_procTarget, new Vec2(0f, 0f), new Rectangle?(), Color.White * 0.5f, 0f, Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None);
            if (layer == this._gridLayer)
            {
                this.backgroundColor = new Color(20, 20, 20);
                Color col = new Color(38, 38, 38);
                if (Editor.arcadeMachineMode)
                {
                    DuckGame.Graphics.DrawRect(this._levelThings[0].position + new Vec2(-17f, -21f), this._levelThings[0].position + new Vec2(18f, 21f), col, -0.9f, false);
                }
                else
                {
                    float x = (float)(-this._cellSize / 2.0);
                    float y = (float)(-this._cellSize / 2.0);
                    if (_sizeRestriction.x > 0.0)
                    {
                        Vec2 vec2 = -new Vec2((float)(_gridW * this._cellSize / 2.0), (float)((this._gridH - 1) * this._cellSize / 2.0)) + new Vec2(8f, 0f);
                        x += (int)(vec2.x / this._cellSize) * this._cellSize;
                        y += (int)(vec2.y / this._cellSize) * this._cellSize;
                    }
                    int num1 = this._gridW;
                    int num2 = this._gridH;
                    if (this._miniMode)
                    {
                        num1 = 12;
                        num2 = 9;
                    }
                    if (x < _ultimateBounds.x)
                    {
                        int num3 = (int)((_ultimateBounds.x - x) / _cellSize) + 1;
                        x = (int)(_ultimateBounds.x / this._cellSize * _cellSize) + this._cellSize / 2f;
                        num1 -= num3;
                    }
                    if (y < _ultimateBounds.y)
                    {
                        int num4 = (int)((_ultimateBounds.y - y) / _cellSize) + 1;
                        y = (int)(_ultimateBounds.y / this._cellSize * _cellSize) + this._cellSize / 2f;
                        num2 -= num4;
                    }
                    float num5 = x + num1 * this._cellSize;
                    if (num5 > this._ultimateBounds.Right)
                    {
                        int num6 = (int)((num5 - this._ultimateBounds.Right) / _cellSize) + 1;
                        num1 -= num6;
                        x = (int)((this._ultimateBounds.Right - num1 * this._cellSize) / _cellSize * _cellSize) - this._cellSize / 2f;
                    }
                    float num7 = y + num2 * this._cellSize;
                    if (y + num2 * this._cellSize > this._ultimateBounds.Bottom)
                    {
                        int num8 = (int)((num7 - this._ultimateBounds.Bottom) / _cellSize) + 1;
                        num2 -= num8;
                        y = (int)((this._ultimateBounds.Bottom - num2 * this._cellSize) / _cellSize * _cellSize) - this._cellSize / 2f;
                    }
                    int num9 = num1 * (int)this._cellSize;
                    int num10 = num2 * (int)this._cellSize;
                    int num11 = (int)(num9 / this._cellSize);
                    int num12 = (int)(num10 / this._cellSize);
                    for (int index = 0; index < num11 + 1; ++index)
                        DuckGame.Graphics.DrawLine(new Vec2(x + index * this._cellSize, y), new Vec2(x + index * this._cellSize, y + num12 * this._cellSize), col, 2f, -0.9f);
                    for (int index = 0; index < num12 + 1; ++index)
                        DuckGame.Graphics.DrawLine(new Vec2(x, y + index * this._cellSize), new Vec2(x + num11 * this._cellSize, y + index * this._cellSize), col, 2f, -0.9f);
                    DuckGame.Graphics.DrawLine(new Vec2(this._ultimateBounds.Left, this._ultimateBounds.Top), new Vec2(this._ultimateBounds.Right, this._ultimateBounds.Top), col, 2f, -0.9f);
                    DuckGame.Graphics.DrawLine(new Vec2(this._ultimateBounds.Right, this._ultimateBounds.Top), new Vec2(this._ultimateBounds.Right, this._ultimateBounds.Bottom), col, 2f, -0.9f);
                    DuckGame.Graphics.DrawLine(new Vec2(this._ultimateBounds.Right, this._ultimateBounds.Bottom), new Vec2(this._ultimateBounds.Left, this._ultimateBounds.Bottom), col, 2f, -0.9f);
                    DuckGame.Graphics.DrawLine(new Vec2(this._ultimateBounds.Left, this._ultimateBounds.Bottom), new Vec2(this._ultimateBounds.Left, this._ultimateBounds.Top), col, 2f, -0.9f);
                    if (this._miniMode)
                    {
                        int num13 = 0;
                        if (!this._pathNorth)
                        {
                            this._sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            this._sideArrow.color = new Color(100, 200, 100);
                            DuckGame.Graphics.DrawLine(new Vec2(x + num9 / 2, y - 10f), new Vec2(x + num9 / 2, (float)(y + num10 / 2 - 8.0)), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }
                        if (!this._pathWest)
                        {
                            this._sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            this._sideArrow.color = new Color(100, 200, 100);
                            DuckGame.Graphics.DrawLine(new Vec2(x - 10f, y + num10 / 2), new Vec2((float)(x + num9 / 2 - 8.0), y + num10 / 2), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }
                        if (!this._pathEast)
                        {
                            this._sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            this._sideArrow.color = new Color(100, 200, 100);
                            DuckGame.Graphics.DrawLine(new Vec2((float)(x + num9 / 2 + 8.0), y + num10 / 2), new Vec2((float)(x + num9 + 10.0), y + num10 / 2), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }
                        if (!this._pathSouth)
                        {
                            this._sideArrow.color = new Color(80, 80, 80);
                        }
                        else
                        {
                            this._sideArrow.color = new Color(100, 200, 100);
                            DuckGame.Graphics.DrawLine(new Vec2(x + num9 / 2, (float)(y + num10 / 2 + 8.0)), new Vec2(x + num9 / 2, (float)(y + num10 + 10.0)), Color.Lime * 0.06f, 16f);
                            ++num13;
                        }
                        if (num13 > 0)
                            DuckGame.Graphics.DrawLine(new Vec2((float)(x + num9 / 2 - 8.0), y + num10 / 2), new Vec2((float)(x + num9 / 2 + 8.0), y + num10 / 2), Color.Lime * 0.06f, 16f);
                    }
                }
            }
            if (layer == Layer.Foreground)
            {
                float num14 = (float)(-this._cellSize / 2.0);
                float num15 = (float)(-this._cellSize / 2.0);
                int num16 = this._gridW;
                int num17 = this._gridH;
                if (this._miniMode)
                {
                    num16 = 12;
                    num17 = 9;
                }
                int num18 = num16 * 16;
                int num19 = num17 * 16;
                if (this._miniMode)
                {
                    Editor._procTilesWide = (int)this._genSize.x;
                    Editor._procTilesHigh = (int)this._genSize.y;
                    Editor._procXPos = (int)this._genTilePos.x;
                    Editor._procYPos = (int)this._genTilePos.y;
                    if (Editor._procXPos > Editor._procTilesWide)
                        Editor._procXPos = Editor._procTilesWide;
                    if (Editor._procYPos > Editor._procTilesHigh)
                        Editor._procYPos = Editor._procTilesHigh;
                    for (int index1 = 0; index1 < Editor._procTilesWide; ++index1)
                    {
                        for (int index2 = 0; index2 < Editor._procTilesHigh; ++index2)
                        {
                            int num20 = index1 - Editor._procXPos;
                            int num21 = index2 - Editor._procYPos;
                            if (index1 != Editor._procXPos || index2 != Editor._procYPos)
                                DuckGame.Graphics.DrawRect(new Vec2(num14 + num18 * num20, num15 + num19 * num21), new Vec2(num14 + num18 * (num20 + 1), num15 + num19 * (num21 + 1)), Color.White * 0.2f, (Depth)1f, false);
                        }
                    }
                }
                if (this._hoverButton == null)
                {
                    if (this._cursorMode != CursorMode.Pasting)
                    {
                        if (this._secondaryHover != null && this._placementMode)
                            DuckGame.Graphics.DrawRect(this._secondaryHover.topLeft, this._secondaryHover.bottomRight, Color.White * 0.5f, (Depth)1f, false);
                        else if (this._hover != null && this._placementMode && (Editor.inputMode != EditorInput.Touch || this._editMode))
                        {
                            DuckGame.Graphics.DrawRect(this._hover.topLeft, this._hover.bottomRight, Color.White * 0.5f, (Depth)1f, false);
                            this._hover.DrawHoverInfo();
                        }
                    }
                    if (DevConsole.wagnusDebug)
                    {
                        DuckGame.Graphics.DrawLine(this._tilePosition, this._tilePosition + new Vec2(128f, 0f), Color.White * 0.5f);
                        DuckGame.Graphics.DrawLine(this._tilePosition, this._tilePosition + new Vec2(sbyte.MinValue, 0f), Color.White * 0.5f);
                        DuckGame.Graphics.DrawLine(this._tilePosition, this._tilePosition + new Vec2(0f, 128f), Color.White * 0.5f);
                        DuckGame.Graphics.DrawLine(this._tilePosition, this._tilePosition + new Vec2(0f, sbyte.MinValue), Color.White * 0.5f);
                    }
                    if ((this._hover == null || this._cursorMode == CursorMode.DragHover || this._cursorMode == CursorMode.Drag) && Editor.inputMode == EditorInput.Gamepad)
                    {
                        if (this._cursorMode == CursorMode.DragHover || this._cursorMode == CursorMode.Drag)
                        {
                            this._cursor.depth = (Depth)1f;
                            this._cursor.scale = new Vec2(1f, 1f);
                            this._cursor.position = this._tilePosition;
                            if (this._cursorMode == CursorMode.DragHover)
                                this._cursor.frame = 1;
                            else if (this._cursorMode == CursorMode.Drag)
                                this._cursor.frame = 5;
                            this._cursor.Draw();
                        }
                        else if (this._placementMenu == null)
                            DuckGame.Graphics.DrawRect(this._tilePosition - new Vec2(this._cellSize / 2f, this._cellSize / 2f), this._tilePosition + new Vec2(this._cellSize / 2f, this._cellSize / 2f), Color.White * 0.5f, (Depth)1f, false);
                    }
                    if (this._cursorMode == CursorMode.Normal && this._hover == null && this._placementMode && Editor.inputMode != EditorInput.Touch && this._placementMenu == null && this._placementType != null)
                    {
                        this._placementType.depth = (Depth)0.9f;
                        this._placementType.x = this._tilePosition.x;
                        this._placementType.y = this._tilePosition.y;
                        this._placementType.Draw();
                        if (this.placementLimitReached || this.placementOutOfSizeRange)
                            DuckGame.Graphics.Draw(this._cantPlace, this._placementType.x, this._placementType.y, (Depth)0.95f);
                    }
                }
                if (this._cursorMode == CursorMode.Selection || this._cursorMode == CursorMode.HasSelection || this._cursorMode == CursorMode.Drag || this._cursorMode == CursorMode.DragHover)
                {
                    //this._leftSelectionDraw = false;
                    if (this._cursorMode == CursorMode.Selection)
                        DuckGame.Graphics.DrawDottedRect(this._selectionDragStart, this._selectionDragEnd, Color.White * 0.5f, (Depth)1f, 2f, 4f);
                }
                if (this._cursorMode == CursorMode.Pasting)
                {
                    DuckGame.Graphics.material = _selectionMaterialPaste;
                    foreach (Thing thing in this._pasteBatch)
                    {
                        Vec2 position = thing.position;
                        thing.position -= this.pasteOffset;
                        thing.Draw();
                        thing.position = position;
                    }
                    DuckGame.Graphics.material = null;
                }
            }
            if (layer == Layer.HUD)
            {
                if (Editor.inputMode == EditorInput.Touch)
                {
                    float l = -24f;
                    if (this._activeTouchButton != null || this._fileDialog.opened)
                    {
                        if (this._activeTouchButton != null)
                            DuckGame.Graphics.DrawString(this._activeTouchButton.explanation, Layer.HUD.camera.OffsetBR(-20f, l) - new Vec2(DuckGame.Graphics.GetStringWidth(this._activeTouchButton.explanation) + (this._cancelButton.size.x + 4f), 0f), Color.Gray, (Depth)0.99f);
                        else if (this._fileDialog.opened)
                        {
                            string text = "Double tap level to open!";
                            DuckGame.Graphics.DrawString(text, Layer.HUD.camera.OffsetBR(-20f, l) - new Vec2(DuckGame.Graphics.GetStringWidth(text) + (this._cancelButton.size.x + 4f), 0f), Color.Gray, (Depth)0.99f);
                        }
                        DuckGame.Graphics.DrawRect(this._cancelButton.position, this._cancelButton.position + this._cancelButton.size, new Color(70, 70, 70), (Depth)0.99f, false);
                        DuckGame.Graphics.DrawRect(this._cancelButton.position, this._cancelButton.position + this._cancelButton.size, new Color(30, 30, 30), (Depth)0.98f);
                        DuckGame.Graphics.DrawString(this._cancelButton.caption, this._cancelButton.position + this._cancelButton.size / 2f + new Vec2((float)(-DuckGame.Graphics.GetStringWidth(this._cancelButton.caption) / 2.0), -4f), Color.White, (Depth)0.99f);
                    }
                    else if (!this._fileDialog.opened)
                    {
                        float num = 0f;
                        foreach (Editor.EditorTouchButton touchButton in this._touchButtons)
                        {
                            DuckGame.Graphics.DrawRect(touchButton.position, touchButton.position + touchButton.size, new Color(70, 70, 70), (Depth)0.99f, false);
                            DuckGame.Graphics.DrawRect(touchButton.position, touchButton.position + touchButton.size, new Color(30, 30, 30), (Depth)0.98f);
                            DuckGame.Graphics.DrawString(touchButton.caption, touchButton.position + touchButton.size / 2f + new Vec2((float)(-DuckGame.Graphics.GetStringWidth(touchButton.caption) / 2.0), -4f), Color.White, (Depth)0.99f);
                            num += touchButton.size.x;
                        }
                        if (this._placementMenu != null && this._placementMenu is EditorGroupMenu)
                        {
                            string text = "Double tap to select!";
                            DuckGame.Graphics.DrawString(text, Layer.HUD.camera.OffsetBR(-20f, l) - new Vec2(DuckGame.Graphics.GetStringWidth(text) + (num + 8f), 0f), Color.Gray, (Depth)0.99f);
                        }
                    }
                    if (this._placingTiles && this._placementMenu == null)
                    {
                        DuckGame.Graphics.DrawRect(this._editTilesButton.position, this._editTilesButton.position + this._editTilesButton.size, new Color(70, 70, 70), (Depth)0.99f, false);
                        DuckGame.Graphics.DrawRect(this._editTilesButton.position, this._editTilesButton.position + this._editTilesButton.size, new Color(30, 30, 30), (Depth)0.98f);
                        DuckGame.Graphics.DrawString(this._editTilesButton.caption, this._editTilesButton.position + this._editTilesButton.size / 2f + new Vec2((float)(-DuckGame.Graphics.GetStringWidth(this._editTilesButton.caption) / 2.0), -4f), Color.White, (Depth)0.99f);
                    }
                }
                if (Editor.hasUnsavedChanges)
                    DuckGame.Graphics.DrawFancyString("*", new Vec2(4f, 4f), Color.White * 0.6f, (Depth)0.99f);
                if (Editor.tooltip != null)
                {
                    DuckGame.Graphics.DrawRect(new Vec2(16f, Layer.HUD.height - 14f), new Vec2((float)(16.0 + DuckGame.Graphics.GetFancyStringWidth(Editor.tooltip) + 2.0), Layer.HUD.height - 2f), new Color(0, 0, 0) * 0.75f, (Depth)0.99f);
                    DuckGame.Graphics.DrawFancyString(Editor.tooltip, new Vec2(18f, Layer.HUD.height - 12f), Color.White, (Depth)0.99f);
                }
                bool flag1 = Editor._input.lastActiveDevice is Keyboard;
                if (this._hoverMode == 0 && this._hoverButton == null)
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
                    if (this._cursorMode == CursorMode.HasSelection || this._cursorMode == CursorMode.Drag || this._cursorMode == CursorMode.DragHover)
                    {
                        if (Editor.inputMode == EditorInput.Gamepad)
                        {
                            if (this._cursorMode == CursorMode.DragHover || this._cursorMode == CursorMode.Drag)
                                text1 += "@SELECT@DRAG  ";
                            if (this._cursorMode == CursorMode.HasSelection || this._cursorMode == CursorMode.DragHover)
                                text1 = text1 + "@CANCEL@DRAG ADD  " + "@MENU1@FLIP  " + "@MENU2@DELETE  ";
                            text1 += this._cursorMode == CursorMode.DragHover ? "@CANCEL@COPY  " : "@CANCEL@DESELECT  ";
                        }
                        else
                        {
                            string str8 = "@KBDARROWS@NUDGE  ";
                            if (this._cursorMode == CursorMode.DragHover)
                                str8 += "@LEFTMOUSE@DRAG  ";
                            text1 = str8 + "@RIGHTMOUSE@DESELECT  ";
                            if (this._cursorMode == CursorMode.HasSelection || this._cursorMode == CursorMode.DragHover)
                                text1 = text1 + "@KBDSHIFT@ADD SELECTION  " + "@KBDF@FLIP  ";
                        }
                    }
                    else if (this._cursorMode == CursorMode.Pasting)
                        text1 = Editor.inputMode != EditorInput.Gamepad ? text1 + "@LEFTMOUSE@PASTE  " + "@RIGHTMOUSE@CANCEL  " : text1 + "@SELECT@PASTE  " + "@CANCEL@CANCEL  ";
                    else if (this._fileDialog.opened)
                        text1 = "@WASD@MOVE  " + str2 + "SELECT  @MENU2@DELETE  " + str1 + "CANCEL  @STRAFE@+@RAGDOLL@BROWSE..";
                    else if (this._menuOpen && Editor.inputMode == EditorInput.Gamepad)
                        text1 = "@WASD@MOVE  " + str2 + "SELECT  @RIGHT@EXPAND  " + str1 + "CLOSE";
                    else if (Editor.inputMode == EditorInput.Gamepad || Editor.inputMode == EditorInput.Mouse)
                    {
                        int num = this._secondaryHover != null ? 1 : (this._hover != null ? 1 : 0);
                        bool flag2 = num != 0 || this._placingTiles || this._placementType != null;
                        if (this._placementType != null && this._hover != null && this.GetLayerOrOverride(this._placementType) == this.GetLayerOrOverride(this._hover))
                            text1 = text1 + str2 + "ERASE  ";
                        else if (this._placementType != null)
                        {
                            text1 = text1 + str2 + "PLACE  ";
                            if (this.rotateValid)
                                text1 += "@RSTICK@ROTATE  ";
                        }
                        if (num != 0)
                            text1 = text1 + str3 + "COPY  ";
                        if (this._hover != null && !this._placingTiles && this._hoverMenu != null)
                            text1 += "@MENU1@EDIT  ";
                        if (Editor.inputMode == EditorInput.Gamepad)
                        {
                            if (History.hasUndo)
                                text1 = text1 + str6 + "UNDO  ";
                            if (History.hasRedo)
                                text1 = text1 + str7 + "REDO  ";
                            text1 += "@CANCEL@DRAG SELECT  ";
                        }
                        if (this._placingTiles)
                            text1 += "@MENU1@TILES  ";
                        if (flag2)
                            text1 = text1 + str5 + "BROWSE  ";
                        text1 = text1 + str4 + "MENU";
                        if (this._font.GetWidth(text1) < 397.0)
                            text1 = "@WASD@MOVE  " + text1;
                        if (Editor.inputMode == EditorInput.Mouse)
                            text1 += "  @RIGHTMOUSE@DRAG SELECT";
                    }
                    if (Editor.inputMode == EditorInput.Touch)
                        text1 = "";
                    if (text1 != "")
                    {
                        float width = this._font.GetWidth(text1);
                        Vec2 vec2 = new Vec2(layer.width - 22f - width, layer.height - 28f);
                        this._font.depth = (Depth)0.8f;
                        this._font.Draw(text1, vec2.x, vec2.y, Color.White, (Depth)0.7f, Editor._input);
                    }
                    this._font.scale = new Vec2(0.5f, 0.5f);
                    float num22 = 0f;
                    if (Editor.placementLimit > 0)
                    {
                        num22 -= 16f;
                        Vec2 vec2 = new Vec2(128f, 12f);
                        Vec2 p1_1 = new Vec2(31f, layer.height - 19f - vec2.y);
                        DuckGame.Graphics.DrawRect(p1_1, p1_1 + vec2, Color.Black * 0.5f, (Depth)0.6f);
                        DuckGame.Graphics.Draw(this._editorCurrency, p1_1.x - 10f, p1_1.y + 2f, (Depth)0.95f);
                        float x = (vec2.x - 4f) * Math.Min(placementTotalCost / (float)Editor.placementLimit, 1f);
                        string text2 = this.placementTotalCost.ToString() + "/" + Editor.placementLimit.ToString();
                        if (this.placementLimitReached)
                            text2 += " FULL!";
                        float width = this._font.GetWidth(text2);
                        this._font.Draw(text2, (float)(p1_1.x + vec2.x / 2.0 - width / 2.0), p1_1.y + 4f, Color.White, (Depth)0.7f);
                        Vec2 p1_2 = p1_1 + new Vec2(2f, 2f);
                        DuckGame.Graphics.DrawRect(p1_2, p1_2 + new Vec2(x, vec2.y - 4f), (this.placementLimitReached ? Colors.DGRed : Colors.DGGreen) * 0.5f, (Depth)0.6f);
                    }
                    if (this.searching)
                    {
                        DuckGame.Graphics.DrawRect(Vec2.Zero, new Vec2(layer.width, layer.height), Color.Black * 0.5f, (Depth)0.9f);
                        Vec2 position = new Vec2(8f, layer.height - 26f);
                        DuckGame.Graphics.DrawString("@searchiconwhitebig@", position, Color.White, (Depth)0.95f);
                        if (Keyboard.keyString == "")
                            DuckGame.Graphics.DrawString("|GRAY|Type to search...", position + new Vec2(26f, 7f), Color.White, (Depth)0.95f);
                        else
                            DuckGame.Graphics.DrawString(Keyboard.keyString + "_", position + new Vec2(26f, 7f), Color.White, (Depth)0.95f);
                        if (Editor.inputMode == EditorInput.Mouse)
                            this._searchHoverIndex = -1;
                        float num23 = 200f;
                        if (this.searchItems != null && this.searchItems.Count > 0)
                        {
                            position.y -= 22f;
                            for (int index = 0; index < 10 && index < this.searchItems.Count; ++index)
                            {
                                DuckGame.Graphics.DrawString(this.searchItems[index].thing.thing.editorName, new Vec2(position.x + 24f, position.y + 6f), Color.White, (Depth)0.95f);
                                this.searchItems[index].thing.image.depth = (Depth)0.95f;
                                this.searchItems[index].thing.image.x = position.x + 4f;
                                this.searchItems[index].thing.image.y = position.y;
                                this.searchItems[index].thing.image.color = Color.White;
                                this.searchItems[index].thing.image.scale = new Vec2(1f);
                                this.searchItems[index].thing.image.Draw();
                                if (Editor.inputMode == EditorInput.Mouse && Mouse.x > position.x && Mouse.x < position.x + 200.0 && Mouse.y > position.y - 2.0 && Mouse.y < position.y + 19.0 || index == this._searchHoverIndex)
                                {
                                    this._searchHoverIndex = index;
                                    DuckGame.Graphics.DrawRect(position + new Vec2(2f, -2f), position + new Vec2(num23 - 2f, 18f), new Color(70, 70, 70), (Depth)0.93f);
                                }
                                position.y -= 20f;
                            }
                            DuckGame.Graphics.DrawRect(position + new Vec2(0f, 16f), new Vec2(position.x + num23, layer.height - 28f), new Color(30, 30, 30), (Depth)0.91f);
                        }
                        DuckGame.Graphics.DrawRect(new Vec2(8f, layer.height - 26f), new Vec2(300f, layer.height - 6f), new Color(30, 30, 30), (Depth)0.91f);
                    }
                    float num24 = 0f;
                    if (this._placementType != null && this._cursorMode == CursorMode.Normal && this._placementMenu == null)
                    {
                        Vec2 vec2 = new Vec2(this._placementType.width, this._placementType.height);
                        vec2.x += 4f;
                        vec2.y += 4f;
                        if (vec2.x < 32.0)
                            vec2.x = 32f;
                        if (vec2.y < 32.0)
                            vec2.y = 32f;
                        Vec2 p1 = new Vec2(19f, layer.height - 19f - vec2.y + num22);
                        string str9 = this._placementType.GetDetailsString();
                        while (str9.Count<char>(x => x == '\n') > 5)
                            str9 = str9.Substring(0, str9.LastIndexOf('\n'));
                        float x1 = this._font.GetWidth(str9) + 8f;
                        if (str9 != "")
                            this._font.Draw(str9, (float)(p1.x + vec2.x + 4.0), p1.y + 4f, Color.White, (Depth)0.7f);
                        else
                            x1 = 0f;
                        DuckGame.Graphics.DrawRect(p1, p1 + vec2 + new Vec2(x1, 0f), Color.Black * 0.5f, (Depth)0.6f);
                        Editor.editorDraw = true;
                        this._placementType.left = p1.x + (float)(vec2.x / 2.0 - this._placementType.w / 2.0);
                        this._placementType.top = p1.y + (float)(vec2.y / 2.0 - this._placementType.h / 2.0);
                        this._placementType.depth = (Depth)0.7f;
                        this._placementType.Draw();
                        Editor.editorDraw = false;
                        this._font.Draw("Placing (" + this._placementType.editorName + ")", p1.x, p1.y - 6f, Color.White, (Depth)0.7f);
                        num24 = vec2.y;
                    }
                    Thing thing = this._hover;
                    if (this._secondaryHover != null)
                        thing = this._secondaryHover;
                    if (thing != null && this._cursorMode == CursorMode.Normal && this._hoverMode == 0)
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
                        while (str10.Count<char>(x => x == '\n') > 5)
                            str10 = str10.Substring(0, str10.LastIndexOf('\n'));
                        float x2 = this._font.GetWidth(str10) + 8f;
                        if (str10 != "")
                            this._font.Draw(str10, (float)(p1.x + vec2.x + 4.0), p1.y + 4f, Color.White, (Depth)0.7f);
                        else
                            x2 = 0f;
                        DuckGame.Graphics.DrawRect(p1, p1 + vec2 + new Vec2(x2, 0f), Color.Black * 0.5f, (Depth)0.6f);
                        Vec2 position = thing.position;
                        Depth depth = thing.depth;
                        Editor.editorDraw = true;
                        thing.left = p1.x + (float)(vec2.x / 2.0 - thing.w / 2.0);
                        thing.top = p1.y + (float)(vec2.y / 2.0 - thing.h / 2.0);
                        thing.depth = (Depth)0.7f;
                        thing.Draw();
                        Editor.editorDraw = false;
                        thing.position = position;
                        thing.depth = depth;
                        this._font.Draw("Hovering (" + thing.editorName + ")", p1.x, p1.y - 6f, Color.White);
                    }
                }
                else if (this._hoverButton != null)
                {
                    string hoverText = this._hoverButton.hoverText;
                    if (hoverText != null)
                    {
                        float width = this._font.GetWidth(hoverText);
                        Vec2 vec2 = new Vec2(layer.width - 28f - width, layer.height - 28f);
                        this._font.depth = (Depth)0.8f;
                        this._font.Draw(hoverText, vec2.x, vec2.y, Color.White, (Depth)0.8f);
                        DuckGame.Graphics.DrawRect(vec2 + new Vec2(-2f, -2f), vec2 + new Vec2(width + 2f, 9f), Color.Black * 0.5f, (Depth)0.6f);
                    }
                }
                this._font.scale = new Vec2(1f, 1f);
            }
            else
            {
                if (layer != this._objectMenuLayer)
                    return;
                if (Editor.inputMode == EditorInput.Mouse)
                {
                    this._cursor.depth = (Depth)1f;
                    this._cursor.scale = new Vec2(1f, 1f);
                    this._cursor.position = Mouse.position;
                    if (this._cursorMode == CursorMode.Normal)
                        this._cursor.frame = 0;
                    else if (this._cursorMode == CursorMode.DragHover)
                        this._cursor.frame = 1;
                    else if (this._cursorMode == CursorMode.Drag)
                        this._cursor.frame = 5;
                    else if (this._cursorMode == CursorMode.Selection)
                        this._cursor.frame = this._dragSelectShiftModifier ? 6 : 2;
                    else if (this._cursorMode == CursorMode.HasSelection)
                        this._cursor.frame = this._dragSelectShiftModifier ? 6 : 0;
                    if (Editor.hoverTextBox)
                    {
                        this._cursor.frame = 7;
                        this._cursor.position.y -= 4f;
                        this._cursor.scale = new Vec2(0.5f, 1f);
                    }
                    this._cursor.Draw();
                }
                if (Editor.inputMode != EditorInput.Touch)
                    return;
                if (TouchScreen.GetTouches().Count == 0)
                {
                    Vec2 pos1 = this._objectMenuLayer.camera.transformScreenVector(Mouse.positionConsole + new Vec2(TouchScreen._spoofFingerDistance, 0f));
                    Vec2 pos2 = this._objectMenuLayer.camera.transformScreenVector(Mouse.positionConsole - new Vec2(TouchScreen._spoofFingerDistance, 0f));
                    DuckGame.Graphics.DrawCircle(pos1, 4f, Color.White * 0.2f, 2f, (Depth)1f);
                    DuckGame.Graphics.DrawCircle(pos2, 4f, Color.White * 0.2f, 2f, (Depth)1f);
                    DuckGame.Graphics.DrawRect(pos1 + new Vec2(-0.5f, -0.5f), pos1 + new Vec2(0.5f, 0.5f), Color.White, (Depth)1f);
                    DuckGame.Graphics.DrawRect(pos2 + new Vec2(-0.5f, -0.5f), pos2 + new Vec2(0.5f, 0.5f), Color.White, (Depth)1f);
                }
                else
                {
                    foreach (Touch touch in TouchScreen.GetTouches())
                        DuckGame.Graphics.DrawCircle(touch.Transform(this._objectMenuLayer.camera), 4f, Color.White, 2f, (Depth)1f);
                }
            }
        }

        public override void StartDrawing()
        {
            if (this._procTarget == null)
                this._procTarget = new RenderTarget2D(DuckGame.Graphics.width, DuckGame.Graphics.height);
            if (this._procContext == null)
                return;
            this._procContext.Draw(this._procTarget, Level.current.camera, this._procDrawOffset);
        }

        public void CloseMenu() => this._closeMenu = true;

        public void DoSave(string saveName)
        {
            this._saveName = saveName;
            if (!this._saveName.EndsWith(".lev"))
                this._saveName += ".lev";
            this.Save();
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
            while (load.StartsWith("/"))
                load = load.Substring(1);
            this.ClearEverything();
            this._saveName = load;
            Editor._currentLevelData = DuckFile.LoadLevel(load);
            Thing.loadingLevel = Editor._currentLevelData;
            if (Editor._currentLevelData == null)
            {
                Editor._currentLevelData = new LevelData();
                Thing.loadingLevel = null;
            }
            else
            {
                Editor._currentLevelData.SetPath(this._saveName);
                if (Editor._currentLevelData.metaData.guid == null || !Editor.editingContent && Content.GetLevel(Editor._currentLevelData.metaData.guid, LevelLocation.Content) != null)
                    Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                this._onlineSettingChanged = true;
                if (Editor._currentLevelData.customData != null)
                {
                    if (Editor._currentLevelData.customData.customTileset01Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customTileset01Data.GetTileData(), 0, CustomType.Block);
                    if (Editor._currentLevelData.customData.customTileset02Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customTileset02Data.GetTileData(), 1, CustomType.Block);
                    if (Editor._currentLevelData.customData.customTileset03Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customTileset03Data.GetTileData(), 2, CustomType.Block);
                    if (Editor._currentLevelData.customData.customBackground01Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customBackground01Data.GetTileData(), 0, CustomType.Background);
                    if (Editor._currentLevelData.customData.customBackground02Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customBackground02Data.GetTileData(), 1, CustomType.Background);
                    if (Editor._currentLevelData.customData.customBackground03Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customBackground03Data.GetTileData(), 2, CustomType.Background);
                    if (Editor._currentLevelData.customData.customPlatform01Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customPlatform01Data.GetTileData(), 0, CustomType.Platform);
                    if (Editor._currentLevelData.customData.customPlatform02Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customPlatform02Data.GetTileData(), 1, CustomType.Platform);
                    if (Editor._currentLevelData.customData.customPlatform03Data != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customPlatform03Data.GetTileData(), 2, CustomType.Platform);
                    if (Editor._currentLevelData.customData.customParallaxData != null)
                        Custom.ApplyCustomData(Editor._currentLevelData.customData.customParallaxData.GetTileData(), 0, CustomType.Parallax);
                }
                Editor.previewCapture = Editor.LoadPreview(Editor._currentLevelData.previewData.preview);
                this._pathNorth = false;
                this._pathSouth = false;
                this._pathEast = false;
                this._pathWest = false;
                this._miniMode = false;
                int sideMask = Editor._currentLevelData.proceduralData.sideMask;
                if ((sideMask & 1) != 0)
                    this._pathNorth = true;
                if ((sideMask & 2) != 0)
                    this._pathEast = true;
                if ((sideMask & 4) != 0)
                    this._pathSouth = true;
                if ((sideMask & 8) != 0)
                    this._pathWest = true;
                if (sideMask != 0)
                    this._miniMode = true;
                this._loadingLevel = true;
                this.LoadObjects(false);
                this.LoadObjects(true);
                this._loadingLevel = false;
                this._editorLoadFinished = true;
                if (!this._looseClear)
                    this.CenterView();
                Editor.hasUnsavedChanges = false;
                Thing.loadingLevel = null;
            }
        }

        public void LoadObjects(bool pAlternate)
        {
            foreach (BinaryClassChunk node in pAlternate ? Editor._currentLevelData.proceduralData.openAirAlternateObjects.objects : Editor._currentLevelData.objects.objects)
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
                                        this._levelThingsAlternate.Add(thing);
                                }
                            }
                            else
                            {
                                foreach (Thing thing in thingContainer.things)
                                    this._levelThingsAlternate.Add(thing);
                            }
                        }
                        else
                            this._levelThingsAlternate.Add(pThing);
                    }
                    else
                        this.AddObject(pThing);
                }
            }
        }

        public void LegacyLoadLevel(string load)
        {
            load = load.Replace('\\', '/');
            while (load.StartsWith("/"))
                load = load.Substring(1);
            DuckXML doc = this._additionalSaveDirectory != null ? DuckXML.Load(load) : DuckFile.LoadDuckXML(load);
            this._saveName = load;
            this.LegacyLoadLevelParts(doc);
            Editor.hasUnsavedChanges = false;
        }

        public void LegacyLoadLevelParts(DuckXML doc)
        {
            this.hadGUID = false;
            this.ClearEverything();
            DXMLNode e = doc.Element("Level");
            DXMLNode dxmlNode1 = e.Element("ID");
            if (dxmlNode1 != null)
            {
                Editor._currentLevelData.metaData.guid = dxmlNode1.Value;
                this.hadGUID = true;
            }
            DXMLNode dxmlNode2 = e.Element("ONLINE");
            Editor._currentLevelData.metaData.onlineMode = dxmlNode2 != null && Convert.ToBoolean(dxmlNode2.Value);
            Editor.previewCapture = Editor.LegacyLoadPreview(e);
            this._pathNorth = false;
            this._pathSouth = false;
            this._pathEast = false;
            this._pathWest = false;
            this._miniMode = false;
            DXMLNode dxmlNode3 = e.Element("PathMask");
            if (dxmlNode3 != null)
            {
                int int32 = Convert.ToInt32(dxmlNode3.Value);
                if ((int32 & 1) != 0)
                    this._pathNorth = true;
                if ((int32 & 2) != 0)
                    this._pathEast = true;
                if ((int32 & 4) != 0)
                    this._pathSouth = true;
                if ((int32 & 8) != 0)
                    this._pathWest = true;
                if (int32 != 0)
                    this._miniMode = true;
            }
            DXMLNode dxmlNode4 = e.Element("workshopID");
            if (dxmlNode4 != null)
                Editor._currentLevelData.metaData.workshopID = Convert.ToUInt64(dxmlNode4.Value);
            DXMLNode dxmlNode5 = e.Element("workshopName");
            if (dxmlNode5 != null)
                Editor._currentLevelData.workshopData.name = dxmlNode5.Value;
            DXMLNode dxmlNode6 = e.Element("workshopDescription");
            if (dxmlNode6 != null)
                Editor._currentLevelData.workshopData.description = dxmlNode6.Value;
            DXMLNode dxmlNode7 = e.Element("workshopVisibility");
            if (dxmlNode7 != null)
                Editor._currentLevelData.workshopData.visibility = (RemoteStoragePublishedFileVisibility)Convert.ToInt32(dxmlNode7.Value);
            DXMLNode dxmlNode8 = e.Element("workshopTags");
            if (dxmlNode8 != null)
            {
                string[] source = dxmlNode8.Value.Split('|');
                Editor._currentLevelData.workshopData.tags = new List<string>();
                if (source.Count<string>() != 0 && source[0] != "")
                    Editor._currentLevelData.workshopData.tags = source.ToList<string>();
            }
            DXMLNode dxmlNode9 = e.Element("Chance");
            if (dxmlNode9 != null)
                Editor._currentLevelData.proceduralData.chance = Convert.ToSingle(dxmlNode9.Value);
            DXMLNode dxmlNode10 = e.Element("MaxPerLev");
            if (dxmlNode10 != null)
                Editor._currentLevelData.proceduralData.maxPerLevel = Convert.ToInt32(dxmlNode10.Value);
            DXMLNode dxmlNode11 = e.Element("Single");
            if (dxmlNode11 != null)
                Editor._currentLevelData.proceduralData.enableSingle = Convert.ToBoolean(dxmlNode11.Value);
            DXMLNode dxmlNode12 = e.Element("Multi");
            if (dxmlNode12 != null)
                Editor._currentLevelData.proceduralData.enableMulti = Convert.ToBoolean(dxmlNode12.Value);
            DXMLNode dxmlNode13 = e.Element("CanMirror");
            if (dxmlNode13 != null)
                Editor._currentLevelData.proceduralData.canMirror = Convert.ToBoolean(dxmlNode13.Value);
            DXMLNode dxmlNode14 = e.Element("IsMirrored");
            if (dxmlNode14 != null)
                Editor._currentLevelData.proceduralData.isMirrored = Convert.ToBoolean(dxmlNode14.Value);
            this._loadingLevel = true;
            IEnumerable<DXMLNode> source1 = e.Elements("Objects");
            if (source1 != null)
            {
                foreach (DXMLNode element in source1.Elements<DXMLNode>("Object"))
                    this.AddObject(Thing.LegacyLoadThing(element));
            }
            this._loadingLevel = false;
            this._editorLoadFinished = true;
            if (this._looseClear)
                return;
            this.CenterView();
        }

        private void CenterView()
        {
            this.camera.width = this._gridW * 16;
            this.camera.height = this.camera.width / Resolution.current.aspect;
            this.camera.centerX = (float)(this.camera.width / 2.0 - 8.0);
            this.camera.centerY = (float)(this.camera.height / 2.0 - 8.0);
            float width = this.camera.width;
            float height = this.camera.height;
            this.camera.width *= 0.3f;
            this.camera.height *= 0.3f;
            this.camera.centerX -= (float)((this.camera.width - width) / 2.0);
            this.camera.centerY -= (float)((this.camera.height - height) / 2.0);
            if (_sizeRestriction.x <= 0.0)
                return;
            this.camera.center = (this._topLeftMost + this._bottomRightMost) / 2f;
        }

        public static Texture2D LoadPreview(string s)
        {
            try
            {
                return s != null ? Texture2D.FromStream(DuckGame.Graphics.device, new MemoryStream(Convert.FromBase64String(s))) : null;
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
                return dxmlNode != null ? Texture2D.FromStream(DuckGame.Graphics.device, new MemoryStream(Convert.FromBase64String(dxmlNode.Value))) : null;
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
                return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAPUExURQAAAGwXbeBu4P///8AgLYwkid8AAAC9SURBVDhPY2RgYPgPxGQDsAE54rkQHhCUhBdDWRDQs7IXyoIAZHmFSQoMTFA2BpCfKA/Gk19MAmNcAKsBII0HFfVQMC5DwF54kPcAwgMCmGZswP7+JYZciTwoj4FhysvJuL0AAiANIIwPYBgAsgGmEdk2XACrC0AaidEMAnijETk8YC4iKRrRNWMDeAORGIDTgIf5D4kKTIx0AEu6oISD7AWQgSCAnLQJpgNiAE4DQM6GeQFmOzZAYXZmYAAAEzJYPzQv17kAAAAASUVORK5CYII=";
            }
        }

        public static Texture2D StringToTexture(string tex)
        {
            if (string.IsNullOrWhiteSpace(tex))
                return null;
            try
            {
                return Texture2D.FromStream(DuckGame.Graphics.device, new MemoryStream(Convert.FromBase64String(tex)));
            }
            catch
            {
                return null;
            }
        }

        public static string TextureToMassiveBitmapString(Texture2D tex)
        {
            Color[] colorArray = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(colorArray);
            return Editor.TextureToMassiveBitmapString(colorArray, tex.Width, tex.Height);
        }

        public static string TextureToMassiveBitmapString(Color[] colors, int width, int height)
        {
            try
            {
                BitBuffer bitBuffer = new BitBuffer(false);
                bitBuffer.Write(Editor.kMassiveBitmapStringHeader);
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
                Texture2D texture = bitBuffer.ReadLong() == Editor.kMassiveBitmapStringHeader ? new Texture2D(DuckGame.Graphics.device, bitBuffer.ReadInt(), bitBuffer.ReadInt()) : throw new Exception("(Editor.MassiveBitmapStringToTexture) Header was invalid.");
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
                }
                while (bitBuffer.position != bitBuffer.lengthInBytes);
                texture.SetData<Color>(data);
                return texture;
            }
            catch
            {
                return null;
            }
        }

        public LevelData CreateSaveData(bool isTempSaveForPlayTestMode = false)
        {
            Level currentLevel = Level.core.currentLevel;
            Level.core.currentLevel = this;
            Editor._currentLevelData.SetExtraHeaderInfo(new LevelMetaData());
            Editor._currentLevelData.Header<LevelMetaData>().type = this.GetLevelType();
            Editor._currentLevelData.Header<LevelMetaData>().size = this.GetLevelSize();
            Editor._currentLevelData.Header<LevelMetaData>().online = this.LevelIsOnlineCapable();
            Editor._currentLevelData.Header<LevelMetaData>().guid = Editor._currentLevelData.metaData.guid;
            Editor._currentLevelData.Header<LevelMetaData>().workshopID = Editor._currentLevelData.metaData.workshopID;
            Editor._currentLevelData.Header<LevelMetaData>().deathmatchReady = Editor._currentLevelData.metaData.deathmatchReady;
            Editor._currentLevelData.Header<LevelMetaData>().onlineMode = Editor._currentLevelData.metaData.onlineMode;
            Editor._currentLevelData.RerouteMetadata(Editor._currentLevelData.Header<LevelMetaData>());
            Editor._currentLevelData.metaData.hasCustomArt = false;
            CustomTileData data1 = Custom.GetData(0, CustomType.Block);
            if (data1 != null && data1.path != null && data1.texture != null)
            {
                data1.ApplyToChunk(Editor._currentLevelData.customData.customTileset01Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customTileset01Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customTileset01Data.ignore = true;
            CustomTileData data2 = Custom.GetData(1, CustomType.Block);
            if (data2 != null && data2.path != null && data2.texture != null)
            {
                data2.ApplyToChunk(Editor._currentLevelData.customData.customTileset02Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customTileset02Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customTileset02Data.ignore = true;
            CustomTileData data3 = Custom.GetData(2, CustomType.Block);
            if (data3 != null && data3.path != null && data3.texture != null)
            {
                data3.ApplyToChunk(Editor._currentLevelData.customData.customTileset03Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customTileset03Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customTileset03Data.ignore = true;
            CustomTileData data4 = Custom.GetData(0, CustomType.Background);
            if (data4 != null && data4.path != null && data4.texture != null)
            {
                data4.ApplyToChunk(Editor._currentLevelData.customData.customBackground01Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customBackground01Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customBackground01Data.ignore = true;
            CustomTileData data5 = Custom.GetData(1, CustomType.Background);
            if (data5 != null && data5.path != null && data5.texture != null)
            {
                data5.ApplyToChunk(Editor._currentLevelData.customData.customBackground02Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customBackground02Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customBackground02Data.ignore = true;
            CustomTileData data6 = Custom.GetData(2, CustomType.Background);
            if (data6 != null && data6.path != null && data6.texture != null)
            {
                data6.ApplyToChunk(Editor._currentLevelData.customData.customBackground03Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customBackground03Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customBackground03Data.ignore = true;
            CustomTileData data7 = Custom.GetData(0, CustomType.Platform);
            if (data7 != null && data7.path != null && data7.texture != null)
            {
                data7.ApplyToChunk(Editor._currentLevelData.customData.customPlatform01Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customPlatform01Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customPlatform01Data.ignore = true;
            CustomTileData data8 = Custom.GetData(1, CustomType.Platform);
            if (data8 != null && data8.path != null && data8.texture != null)
            {
                data8.ApplyToChunk(Editor._currentLevelData.customData.customPlatform02Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customPlatform02Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customPlatform02Data.ignore = true;
            CustomTileData data9 = Custom.GetData(2, CustomType.Platform);
            if (data9 != null && data9.path != null && data9.texture != null)
            {
                data9.ApplyToChunk(Editor._currentLevelData.customData.customPlatform03Data);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customPlatform03Data.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customPlatform03Data.ignore = true;
            CustomTileData data10 = Custom.GetData(0, CustomType.Parallax);
            if (data10 != null && data10.path != null && data10.texture != null)
            {
                data10.ApplyToChunk(Editor._currentLevelData.customData.customParallaxData);
                Editor._currentLevelData.metaData.hasCustomArt = true;
                Editor._currentLevelData.customData.customParallaxData.ignore = false;
            }
            else
                Editor._currentLevelData.customData.customParallaxData.ignore = true;
            Editor._currentLevelData.modData.workshopIDs.Clear();
            if (this._things.Count > 0)
            {
                HashSet<Mod> modSet = new HashSet<Mod>();
                foreach (Thing levelThing in this.levelThings)
                {
                    modSet.Add(ModLoader.GetModFromType(levelThing.GetType()));
                    if (levelThing is IContainThings)
                    {
                        IContainThings containThings = (IContainThings)levelThing;
                        if (containThings.contains != null)
                        {
                            foreach (System.Type contain in containThings.contains)
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
                            Editor._currentLevelData.modData.workshopIDs.Add(mod.workshopIDFacade != 0UL ? mod.workshopIDFacade : mod.configuration.workshopID);
                        else
                            Editor._currentLevelData.modData.hasLocalMods = true;
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
            Editor._currentLevelData.metaData.eightPlayer = false;
            Editor._currentLevelData.metaData.eightPlayerRestricted = false;
            Editor._currentLevelData.objects.objects.Clear();
            if (this._levelThings.Count > 0)
            {
                MultiMap<System.Type, Thing> multiMap = new MultiMap<System.Type, Thing>();
                foreach (Thing levelThing in this._levelThings)
                {
                    if (levelThing is EightPlayer)
                    {
                        Editor._currentLevelData.metaData.eightPlayer = true;
                        Editor._currentLevelData.metaData.eightPlayerRestricted = (levelThing as EightPlayer).eightPlayerOnly.value;
                    }
                    if (levelThing.editorCanModify && !levelThing.processedByEditor)
                    {
                        if (this._miniMode)
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
                                    if (typeof(Gun).IsAssignableFrom(itemSpawner.contains) && itemSpawner.likelyhoodToExist == 1.0 && !itemSpawner.randomSpawn)
                                    {
                                        if (itemSpawner.spawnNum < 1 && itemSpawner.spawnTime < 8.0 && itemSpawner.isAccessible)
                                        {
                                            if (str2 != "")
                                                str2 += "|";
                                            str2 += ModLoader.SmallTypeName(itemSpawner.contains);
                                        }
                                        if (str1 != "")
                                            str1 += "|";
                                        str1 += ModLoader.SmallTypeName(itemSpawner.contains);
                                        break;
                                    }
                                    break;
                                default:
                                    if (levelThing.GetType() == typeof(ItemBox))
                                    {
                                        ItemBox itemBox = levelThing as ItemBox;
                                        if (typeof(Gun).IsAssignableFrom(itemBox.contains) && itemBox.likelyhoodToExist == 1.0 && itemBox.isAccessible)
                                        {
                                            if (str2 != "")
                                                str2 += "|";
                                            str2 += ModLoader.SmallTypeName(itemBox.contains);
                                            if (str1 != "")
                                                str1 += "|";
                                            str1 += ModLoader.SmallTypeName(itemBox.contains);
                                            break;
                                        }
                                        break;
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
            this.SerializeObjects(false);
            this.SerializeObjects(true);
            Editor._currentLevelData.proceduralData.sideMask = 0;
            if (this._miniMode)
            {
                int num7 = 0;
                if (this._pathNorth)
                    num7 |= 1;
                if (this._pathEast)
                    num7 |= 2;
                if (this._pathSouth)
                    num7 |= 4;
                if (this._pathWest)
                    num7 |= 8;
                Editor._currentLevelData.proceduralData.sideMask = num7;
                Editor._currentLevelData.proceduralData.weaponConfig = str1;
                Editor._currentLevelData.proceduralData.spawnerConfig = str2;
                Editor._currentLevelData.proceduralData.numArmor = num1;
                Editor._currentLevelData.proceduralData.numEquipment = num2;
                Editor._currentLevelData.proceduralData.numSpawns = num3;
                Editor._currentLevelData.proceduralData.numTeamSpawns = num4;
                Editor._currentLevelData.proceduralData.numLockedDoors = num5;
                Editor._currentLevelData.proceduralData.numKeys = num6;
            }
            if (Editor.previewCapture != null)
                Editor._currentLevelData.previewData.preview = Editor.TextureToString(Editor.previewCapture);
            try
            {
                Content.doingTempSave = isTempSaveForPlayTestMode;
                Content.GeneratePreview(Editor._currentLevelData, !isTempSaveForPlayTestMode);
                Content.doingTempSave = false;
            }
            catch (Exception)
            {
                DevConsole.Log(DCSection.General, "Error creating preview for level " + Editor._currentLevelData.metaData.guid.ToString());
            }
            LevelData saveData = Editor._currentLevelData.Clone();
            saveData.RerouteMetadata(saveData.Header<LevelMetaData>());
            if (isTempSaveForPlayTestMode)
                saveData.metaData.guid = "tempPlayLevel";
            Level.core.currentLevel = currentLevel;
            return saveData;
        }

        public void SerializeObjects(bool pAlternate)
        {
            List<BinaryClassChunk> binaryClassChunkList = pAlternate ? Editor._currentLevelData.proceduralData.openAirAlternateObjects.objects : Editor._currentLevelData.objects.objects;
            List<Thing> thingList = pAlternate ? this._levelThingsAlternate : this._levelThingsNormal;
            binaryClassChunkList.Clear();
            if (thingList.Count <= 0)
                return;
            foreach (Thing thing in thingList)
                thing.processedByEditor = false;
            MultiMap<System.Type, Thing> multiMap = new MultiMap<System.Type, Thing>();
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
            foreach (KeyValuePair<System.Type, List<Thing>> keyValuePair in (MultiMap<System.Type, Thing, List<Thing>>)multiMap)
                binaryClassChunkList.Add(new ThingContainer(keyValuePair.Value, keyValuePair.Key)
                {
                    quickSerialize = this.minimalConversionLoad
                }.Serialize());
        }

        public bool Save(bool isTempSaveForPlayTestMode = false)
        {
            if (this._saveName == "")
            {
                this.SaveAs();
            }
            else
            {
                Editor.saving = true;
                LevelData saveData = this.CreateSaveData(isTempSaveForPlayTestMode);
                saveData.customData.customTileset01Data.ignore = Editor._currentLevelData.customData.customTileset01Data.ignore;
                saveData.customData.customTileset02Data.ignore = Editor._currentLevelData.customData.customTileset02Data.ignore;
                saveData.customData.customTileset03Data.ignore = Editor._currentLevelData.customData.customTileset03Data.ignore;
                saveData.customData.customBackground01Data.ignore = Editor._currentLevelData.customData.customBackground01Data.ignore;
                saveData.customData.customBackground02Data.ignore = Editor._currentLevelData.customData.customBackground02Data.ignore;
                saveData.customData.customBackground03Data.ignore = Editor._currentLevelData.customData.customBackground03Data.ignore;
                saveData.customData.customPlatform01Data.ignore = Editor._currentLevelData.customData.customPlatform01Data.ignore;
                saveData.customData.customPlatform02Data.ignore = Editor._currentLevelData.customData.customPlatform02Data.ignore;
                saveData.customData.customPlatform03Data.ignore = Editor._currentLevelData.customData.customPlatform03Data.ignore;
                saveData.customData.customParallaxData.ignore = Editor._currentLevelData.customData.customParallaxData.ignore;
                saveData.SetPath(this._saveName);
                if (!DuckFile.SaveChunk(saveData, this._saveName))
                {
                    this._notify.Open("Could not save data.");
                    return false;
                }
                if (!isTempSaveForPlayTestMode)
                    Editor._currentLevelData.SetPath(this._saveName);
                Content.MapLevel(saveData.metaData.guid, saveData, LevelLocation.Custom);
                if (this._additionalSaveDirectory != null && this._saveName.LastIndexOf("assets/levels/") != -1)
                {
                    string str1 = this._saveName.Substring(this._saveName.LastIndexOf("assets/levels/") + "assets/levels/".Length);
                    string str2 = Directory.GetCurrentDirectory() + "/Content/levels/" + str1;
                    DuckFile.CreatePath(str2);
                    System.IO.File.Copy(this._saveName, str2, true);
                    System.IO.File.SetAttributes(this._saveName, FileAttributes.Normal);
                }
                if (this._miniMode && !this._doingResave)
                    LevelGenerator.ReInitialize();
                foreach (Thing levelThing in this._levelThings)
                    levelThing.processedByEditor = false;
                Editor.saving = false;
                if (!isTempSaveForPlayTestMode)
                    Editor.hasUnsavedChanges = false;
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
                return pData.GetExtraHeaderInfo() != null && pData.GetExtraHeaderInfo() is LevelMetaData ? pData.GetExtraHeaderInfo() as LevelMetaData : pData.metaData;
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
            while (file.StartsWith("/"))
                file = file.Substring(1);
            string path = "";
            LevelMetaData data = Editor.ReadLevelMetadata(file);
            if (data != null)
            {
                Editor.activatedLevels.RemoveAll(x => x == data.guid);
                path = DuckFile.editorPreviewDirectory + data.guid;
            }
            System.IO.File.SetAttributes(file, FileAttributes.Normal);
            DuckFile.Delete(file);
            if (!System.IO.File.Exists(path))
                return;
            System.IO.File.SetAttributes(path, FileAttributes.Normal);
            System.IO.File.Delete(path);
        }

        public void SaveAs()
        {
            this._fileDialog.Open(Editor._initialDirectory, Editor._initialDirectory, true);
            this.DoMenuClose();
            this._closeMenu = false;
        }

        public void Load()
        {
            this._fileDialog.Open(Editor._initialDirectory, Editor._initialDirectory, false);
            this.DoMenuClose();
            this._closeMenu = false;
        }

        public string SaveTempVersion()
        {
            string saveName = this._saveName;
            string str = Directory.GetCurrentDirectory() + "\\Content\\_tempPlayLevel.lev";
            this._saveName = str;
            this.Save(true);
            this._saveName = saveName;
            return str;
        }

        public void Play()
        {
            if (!this._runLevelAnyway && !Editor.arcadeMachineMode && this._levelThings.FirstOrDefault<Thing>(x =>
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
                this.CloseMenu();
                this.ShowNoSpawnsDialogue();
            }
            else
            {
                Editor.isTesting = true;
                string name;
                if (this._miniMode && this._procContext != null)
                {
                    LevelGenerator.ReInitialize();
                    this._centerTile = LevelGenerator.LoadInTile(this.SaveTempVersion());
                    name = "RANDOM";
                }
                else
                    name = this.SaveTempVersion();
                this.CloseMenu();
                this.RunTestLevel(name);
            }
        }

        public virtual void RunTestLevel(string name)
        {
            Editor.isTesting = true;
            Level.current = new TestArea(this, name, this._procSeed, this._centerTile);
            Level.current.AddThing(new EditorTestLevel(this));
        }

        public static MemoryStream GetCompressedActiveLevelData()
        {
            MemoryStream compressedActiveLevelData = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(new GZipStream(compressedActiveLevelData, CompressionMode.Compress));
            foreach (string activatedLevel in Editor.activatedLevels)
            {
                binaryWriter.Write(true);
                binaryWriter.Write(activatedLevel);
                byte[] buffer = System.IO.File.ReadAllBytes(DuckFile.levelDirectory + activatedLevel + ".lev");
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
            byte[] buffer = System.IO.File.ReadAllBytes(DuckFile.levelDirectory + level + ".lev");
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
            return new ReceivedLevelInfo()
            {
                data = levelData,
                name = str
            };
        }

        public static uint Checksum(byte[] data) => CRC32.Generate(data);

        public static uint Checksum(byte[] data, int start, int length) => CRC32.Generate(data, start, length);

        public static Dictionary<System.Type, Thing> thingMap => Editor._thingMap;

        public static void MapThing(Thing t) => Editor._thingMap[t.GetType()] = t;

        public static Thing GetThing(System.Type t)
        {
            Editor._thingMap.TryGetValue(t, out Thing thing);
            return thing;
        }

        public static List<ClassMember> GetMembers<T>() => Editor.GetMembers(typeof(T));

        public static List<ClassMember> GetMembers(System.Type t)
        {
            List<ClassMember> members1;
            if (Editor._classMembers.TryGetValue(t, out members1))
                return members1;
            Editor._classMemberNames[t] = new Dictionary<string, ClassMember>();
            List<ClassMember> members2 = new List<ClassMember>();
            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                ClassMember classMember = new ClassMember(field.Name, t, field);
                Editor._classMemberNames[t][field.Name] = classMember;
                members2.Add(classMember);
            }
            foreach (PropertyInfo property in properties)
            {
                ClassMember classMember = new ClassMember(property.Name, t, property);
                Editor._classMemberNames[t][property.Name] = classMember;
                members2.Add(classMember);
            }
            Editor._classMembers[t] = members2;
            return members2;
        }

        public static List<ClassMember> GetStaticMembers(System.Type t)
        {
            List<ClassMember> staticMembers1;
            if (Editor._staticClassMembers.TryGetValue(t, out staticMembers1))
                return staticMembers1;
            Editor._classMemberNames[t] = new Dictionary<string, ClassMember>();
            List<ClassMember> staticMembers2 = new List<ClassMember>();
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] properties = t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                ClassMember classMember = new ClassMember(field.Name, t, field);
                Editor._classMemberNames[t][field.Name] = classMember;
                staticMembers2.Add(classMember);
            }
            foreach (PropertyInfo property in properties)
            {
                ClassMember classMember = new ClassMember(property.Name, t, property);
                Editor._classMemberNames[t][property.Name] = classMember;
                staticMembers2.Add(classMember);
            }
            Editor._staticClassMembers[t] = staticMembers2;
            return staticMembers2;
        }

        public static ClassMember GetMember<T>(string name) => Editor.GetMember(typeof(T), name);

        public static ClassMember GetMember(System.Type t, string name)
        {
            Dictionary<string, ClassMember> dictionary;
            if (!Editor._classMemberNames.TryGetValue(t, out dictionary))
            {
                Editor.GetMembers(t);
                if (!Editor._classMemberNames.TryGetValue(t, out dictionary))
                    return null;
            }
            ClassMember member;
            dictionary.TryGetValue(name, out member);
            return member;
        }

        internal static System.Type GetType(string name) => ModLoader.GetType(name);

        internal static System.Type DeSerializeTypeName(string serializedTypeName) => serializedTypeName == "" ? null : Editor.GetType(serializedTypeName);

        internal static string SerializeTypeName(System.Type t) => t == null ? "" : ModLoader.SmallTypeName(t);

        public static void CopyClass(object source, object destination)
        {
            foreach (FieldInfo field in source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                field.SetValue(destination, field.GetValue(source));
        }

        public static IEnumerable<System.Type> GetSubclasses(System.Type parentType) => DG.assemblies.SelectMany<Assembly, System.Type>(assembly => assembly.GetTypes()).Where<System.Type>(type => type.IsSubclassOf(parentType)).OrderBy<System.Type, string>(t => t.FullName).ToArray<System.Type>();

        public static IEnumerable<System.Type> GetSubclassesAndInterfaces(System.Type parentType) => DG.assemblies.SelectMany<Assembly, System.Type>(assembly => assembly.GetTypes()).Where<System.Type>(type => parentType.IsAssignableFrom(type)).OrderBy<System.Type, string>(t => t.FullName).ToArray<System.Type>();

        public static AccessorInfo GetAccessorInfo(
          System.Type t,
          string name,
          FieldInfo field = null,
          PropertyInfo property = null)
        {
            Dictionary<string, AccessorInfo> dictionary;
            if (Editor._accessorCache.TryGetValue(t, out dictionary))
            {
                AccessorInfo accessorInfo;
                if (dictionary.TryGetValue(name, out accessorInfo))
                    return accessorInfo;
            }
            else
                Editor._accessorCache[t] = new Dictionary<string, AccessorInfo>();
            AccessorInfo accessor = Editor.CreateAccessor(field, property, t, name);
            Editor._accessorCache[t][name] = accessor;
            return accessor;
        }

        public static AccessorInfo CreateAccessor(
          FieldInfo field,
          PropertyInfo property,
          System.Type t,
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
                    setAccessor = Editor.BuildSetAccessorField(t, field),
                    getAccessor = Editor.BuildGetAccessorField(t, field)
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
                    accessor.setAccessor = Editor.BuildSetAccessorProperty(t, setMethod);
                accessor.getAccessor = Editor.BuildGetAccessorProperty(t, property);
            }
            return accessor;
        }

        public static Action<object, object> BuildSetAccessorProperty(Type t, MethodInfo method)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "o");
            ParameterExpression value = Expression.Parameter(typeof(object));
            return Expression.Lambda<Action<object, object>>(Expression.Call(method.IsStatic ? null : Expression.Convert(obj, method.DeclaringType), method, new Expression[]
            {
                Expression.Convert(value, method.GetParameters()[0].ParameterType)
            }), new ParameterExpression[]
            {
                obj,
                value
            }).Compile();
        }

        public static Action<object, object> BuildSetAccessorField(Type t, FieldInfo field)
        {
            ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
            ParameterExpression valueExp = Expression.Parameter(typeof(object), "value");
            return Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.Field(field.IsStatic ? null : Expression.Convert(targetExp, t), field), Expression.Convert(valueExp, field.FieldType)), new ParameterExpression[]
            {
                targetExp,
                valueExp
            }).Compile();
        }

        public static Func<object, object> BuildGetAccessorProperty(Type t, PropertyInfo property)
        {
            if (property.GetGetMethod(true) == null)
            {
                return null;
            }
            ParameterExpression obj = Expression.Parameter(typeof(object), "o");
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(property.GetGetMethod(true).IsStatic ? null : Expression.Convert(obj, t), property), typeof(object)), new ParameterExpression[]
            {
                obj
            }).Compile();
        }

        public static Func<object, object> BuildGetAccessorField(Type t, FieldInfo field)
        {
            ParameterExpression obj = Expression.Parameter(typeof(object), "o");
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Field(field.IsStatic ? null : Expression.Convert(obj, t), field), typeof(object)), new ParameterExpression[]
            {
                obj
            }).Compile();
        }

        private static object GetDefaultValue(System.Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

        public static Thing CreateThing(System.Type t)
        {
            ThingConstructor thingConstructor;
            return Editor._defaultConstructors.TryGetValue(t, out thingConstructor) ? thingConstructor() : Activator.CreateInstance(t, Editor.GetConstructorParameters(t)) as Thing;
        }

        public static Thing CreateThing(System.Type t, object[] p) => Activator.CreateInstance(t, p) as Thing;

        public static Thing GetOrCreateTypeInstance(System.Type t)
        {
            Thing typeInstance;
            if (!Editor._thingMap.TryGetValue(t, out typeInstance) && Editor.CreateObject(t) is Thing thing)
            {
                Editor._thingMap[t] = thing;
                typeInstance = thing;
            }
            return typeInstance;
        }

        public static object CreateObject(System.Type t)
        {
            Func<object> func;
            return Editor._constructorParameterExpressions.TryGetValue(t, out func) ? func() : null;
        }

        private static void RegisterEditorFields(System.Type pType)
        {
            List<FieldInfo> fieldInfoList;
            if (!Editor.EditorFieldsForType.TryGetValue(pType, out fieldInfoList))
                fieldInfoList = Editor.EditorFieldsForType[pType] = new List<FieldInfo>();
            foreach (System.Type key in Editor.AllBaseTypes[pType])
            {
                if (Editor.AllEditorFields.ContainsKey(key))
                    fieldInfoList.AddRange(Editor.AllEditorFields[key]);
            }
        }

        public static void InitializeConstructorLists()
        {
            MonoMain.loadMessage = "Loading Constructor Lists";
            if (MonoMain.moddingEnabled)
            {
                MonoMain.loadMessage = "Loading Constructor Lists";
                Editor.ThingTypes = ManagedContent.Things.SortedTypes.ToList<System.Type>();
            }
            else
                Editor.ThingTypes = Editor.GetSubclasses(typeof(Thing)).ToList<System.Type>();
            Editor.GroupThingTypes = new List<System.Type>();
            Editor.GroupThingTypes.AddRange(ThingTypes);
            Editor.AllBaseTypes = new Dictionary<System.Type, List<System.Type>>();
            Editor.AllEditorFields = new Dictionary<System.Type, IEnumerable<FieldInfo>>();
            Editor.AllStateFields = new Dictionary<System.Type, FieldInfo[]>();
            Editor.EditorFieldsForType = new Dictionary<System.Type, List<FieldInfo>>();
            System.Type editorFieldType = typeof(EditorProperty<>);
            System.Type stateFieldType = typeof(StateBinding);
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ushort key = 2;
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string str = "";
            foreach (System.Type thingType in Editor.ThingTypes)
            {
                Editor.AllBaseTypes[thingType] = Thing.GetAllTypes(thingType);
                FieldInfo[] fields = thingType.GetFields(bindingAttr);
                Editor.AllEditorFields[thingType] = fields.Where<FieldInfo>(val => val.FieldType.IsGenericType && val.FieldType.GetGenericTypeDefinition() == editorFieldType).ToArray<FieldInfo>();
                Editor.AllStateFields[thingType] = fields.Where<FieldInfo>(val => val.FieldType == stateFieldType).ToArray<FieldInfo>();
                if (Editor.AllStateFields[thingType].Count<FieldInfo>() > 0)
                {
                    Editor.IDToType[key] = thingType;
                    if (thingType.Assembly == executingAssembly)
                        str += thingType.Name;
                    ++key;
                }
            }
            Editor.thingTypesHash = CRC32.Generate(str);
            foreach (System.Type thingType in Editor.ThingTypes)
            {
                if (!thingType.IsAbstract)
                {
                    Editor.RegisterEditorFields(thingType);
                    foreach (ConstructorInfo constructor in thingType.GetConstructors())
                    {
                        ParameterInfo[] parameters = constructor.GetParameters();
                        if (parameters.Length == 0)
                        {
                            LambdaExpression lambdaExpression = Expression.Lambda(typeof(Editor.ThingConstructor), Expression.New(constructor, null), null);
                            Editor._defaultConstructors[thingType] = (Editor.ThingConstructor)lambdaExpression.Compile();
                            Editor._constructorParameters[thingType] = new object[0];
                        }
                        else
                        {
                            Expression[] expressionArray = new Expression[parameters.Length];
                            object[] objArray = new object[parameters.Length];
                            int index = 0;
                            foreach (ParameterInfo parameterInfo in parameters)
                            {
                                System.Type parameterType = parameterInfo.ParameterType;
                                objArray[index] = parameterInfo.DefaultValue == null || !(parameterInfo.DefaultValue.GetType() != typeof(DBNull)) ? Editor.GetDefaultValue(parameterType) : parameterInfo.DefaultValue;
                                expressionArray[index] = Expression.Constant(objArray[index], parameterType);
                                ++index;
                            }
                            LambdaExpression lambdaExpression = Expression.Lambda(typeof(Editor.ThingConstructor), Expression.New(constructor, expressionArray), null);
                            Editor._defaultConstructors[thingType] = (Editor.ThingConstructor)lambdaExpression.Compile();
                            Editor._constructorParameters[thingType] = objArray;
                        }
                    }
                    ++MonoMain.loadyBits;
                }
            }
            Program.constructorsLoaded = Editor._constructorParameters.Count;
            Program.thingTypes = Editor.ThingTypes.Count;
            foreach (System.Type thingType in Editor.ThingTypes)
            {
                foreach (ConstructorInfo constructor in thingType.GetConstructors())
                {
                    ConstructorInfo info = constructor;
                    ParameterInfo[] parameters = info.GetParameters();
                    if (parameters.Length == 0)
                    {
                        Editor._constructorParameterExpressions[thingType] = () => info.Invoke(null);
                    }
                    else
                    {
                        Expression[] expressionArray = new Expression[parameters.Length];
                        int index = 0;
                        object[] vals = new object[parameters.Length];
                        foreach (ParameterInfo parameterInfo in parameters)
                        {
                            System.Type parameterType = parameterInfo.ParameterType;
                            vals[index] = Editor.GetDefaultValue(parameterType);
                            ++index;
                        }
                        Editor._constructorParameterExpressions[thingType] = () => info.Invoke(vals);
                    }
                }
                ++MonoMain.loadyBits;
            }
        }

        public static void InitializePlaceableList()
        {
            if (Editor._placeables != null)
                return;
            Editor.InitializeConstructorLists();
            Editor.InitializePlaceableGroup();
        }

        public static void InitializePlaceableGroup()
        {
            AutoUpdatables.ignoreAdditions = true;
            MonoMain.loadMessage = "Loading Editor Groups";
            Editor._placeables = new EditorGroup(null, null);
            AutoUpdatables.ignoreAdditions = false;
            if (!Editor._clearOnce)
            {
                AutoUpdatables.Clear();
                Editor._clearOnce = true;
            }
            Editor._listLoaded = true;
        }

        public static bool HasConstructorParameter(System.Type t) => Editor._constructorParameters.ContainsKey(t);

        public static object[] GetConstructorParameters(System.Type t)
        {
            object[] constructorParameters;
            Editor._constructorParameters.TryGetValue(t, out constructorParameters);
            if (constructorParameters == null)
            {
                int num = 0;
                try
                {
                    Editor.ThingTypes = !MonoMain.moddingEnabled ? Editor.GetSubclasses(typeof(Thing)).ToList<System.Type>() : ManagedContent.Things.SortedTypes.ToList<System.Type>();
                    num = Editor.ThingTypes.Count;
                }
                catch (Exception)
                {
                }
                throw new Exception("Error loading constructor parameters for type " + t.ToString() + "(" + Editor._constructorParameters.Count.ToString() + " parms vs " + Program.thingTypes.ToString() + ", " + Program.constructorsLoaded.ToString() + ", " + num.ToString() + " things vs " + Program.thingTypes.ToString() + ")");
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
            public Editor.EditorTouchState state;
        }

        private delegate Thing ThingConstructor();
    }
}
