// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelSelect
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DuckGame
{
    public class LevelSelect : Level
    {
        private string _currentDirectory;
        private string _rootDirectory;
        private List<LSItem> _items = new List<LSItem>();
        private int _topIndex;
        private int _maxItems = 15;
        private int _selectedItem;
        private LSItem _selectedLevel;
        private float _leftPos = 12f;
        private float _topPos = 21f;
        private Texture2D _preview;
        private Sprite _previewSprite;
        private LSItem _previewItem;
        private LSItem _lastSelection;
        private BitmapFont _font;
        private Level _returnLevel;
        private bool _exiting;
        private TextEntryDialog _dialog;
        private UIMenu _returnMenu;
        private SpriteMap _iconSheet;
        private bool _onlineMode;
        private List<IFilterLSItems> _filters = new List<IFilterLSItems>();
        public string modRoot;
        public MapPack mapPack;
        public bool isInitialized;
        public static bool _skipCompanionOpening;
        public bool showPlaylistOption;
        public bool isClosed;
        private UIMenu _confirmMenu;
        private UIMenu _notOnlineMenu;
        private MenuBoolean _deleteFile = new MenuBoolean();

        public List<IFilterLSItems> filters => _filters;

        public LevelSelect(string root = "", Level returnLevel = null, UIMenu returnMenu = null, bool onlineMode = false)
        {
            _centeredView = true;
            if (root == "")
                root = DuckFile.levelDirectory;
            root = root.TrimEnd('/');
            _rootDirectory = root;
            _font = new BitmapFont("biosFont", 8);
            _returnLevel = returnLevel;
            _dialog = new TextEntryDialog
            {
                filename = true
            };
            _returnMenu = returnMenu;
            _iconSheet = new SpriteMap("iconSheet", 16, 16);
            _onlineMode = onlineMode;
            _filters.Add(new LSFilterLevelType(LevelType.Deathmatch, true));
            _filters.Add(new LSFilterMods(true));
        }

        public void SetCurrentFolder(string folder) => SetCurrentFolder(folder, false, false);

        public void SetCurrentFolder(string folder, bool isModPath, bool isModRoot, MapPack pMapPack = null)
        {
            if (isModRoot)
                modRoot = folder;
            _currentDirectory = folder;
            if (_currentDirectory == _rootDirectory)
            {
                _selectedItem = 0;
                mapPack = null;
            }
            else
                _selectedItem = 1;
            if (pMapPack != null)
                mapPack = pMapPack;
            HUDTopRightSetup();
            _topIndex = 0;
            _items.Clear();
            if (_currentDirectory != _rootDirectory)
            {
                AddItem(new LSItem(0f, 0f, this, "../"));
            }
            else
            {
                AddItem(new LSItem(0f, 0f, this, "@VANILLA@")
                {
                    itemType = LSItemType.Vanilla
                });
                if (Steam.GetNumWorkshopItems() > 0)
                    AddItem(new LSItem(0f, 0f, this, "@WORKSHOP@", true));
            }
            if (folder.EndsWith(".play") || folder == "@WORKSHOP@" || folder == "@VANILLA@")
            {
                List<string> levelsInside = LSItem.GetLevelsInside(this, folder);
                levelsInside.Sort();
                foreach (string PATH in levelsInside)
                    AddItem(new LSItem(0f, 0f, this, PATH));
                _items = _items.OrderBy(x => x.data != null && x.data.metaData.eightPlayer).ToList();
                PositionItems();
            }
            else
            {
                string[] directories = DuckFile.GetDirectories(folder);
                string[] files = DuckFile.GetFiles(folder);
                Array.Sort(directories);
                Array.Sort(files);
                if (_currentDirectory == _rootDirectory)
                {
                    foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                    {
                        if (accessibleMod.configuration != null && accessibleMod.configuration.content != null && accessibleMod.configuration.content.levels.Count > 0)
                            AddItem(new LSItem(0f, 0f, this, accessibleMod.configuration.contentDirectory + "/Levels", pIsModPath: true, pIsModRoot: true)
                            {
                                _name = accessibleMod.configuration.name
                            });
                    }
                    foreach (MapPack mapPack in MapPack.active)
                        AddItem(new LSItem(0f, 0f, this, mapPack.path, pIsModPath: true, pIsModRoot: true, pIsMapPack: true)
                        {
                            _name = mapPack.name,
                            _customIcon = mapPack.icon,
                            mapPack = mapPack
                        });
                }
                foreach (string str in directories)
                {
                    if (DuckFile.GetFiles(str, "*.lev", SearchOption.AllDirectories).Count() > 0)
                        AddItem(new LSItem(0f, 0f, this, str, pIsModPath: isModPath));
                }
                List<string> stringList = new List<string>();
                foreach (string str in files)
                {
                    string file = str;
                    if (Path.GetExtension(file) == ".lev" && _filters.TrueForAll(a => a.Filter(file)))
                        stringList.Add(file);
                    else if (Path.GetExtension(file) == ".play")
                        AddItem(new LSItem(0f, 0f, this, file, pIsModPath: isModPath));
                }
                foreach (string PATH in stringList)
                    AddItem(new LSItem(0f, 0f, this, PATH, pIsModPath: isModPath));
                PositionItems();
            }
        }

        public override void Initialize()
        {
            InputProfile.repeat = true;
            Keyboard.repeat = true;
            HUD.CloseCorner(HUDCorner.BottomLeft); // close FORCE START tip
            SetCurrentFolder(_rootDirectory);
            isInitialized = true;
            _dialog.DoInitialize();
            float num1 = 320f;
            float num2 = 180f;
            _confirmMenu = new UIMenu("DELETE FILE!?", num1 / 2f, num2 / 2f, 160f, conString: "@CANCEL@CANCEL @SELECT@SELECT");
            if (_returnMenu != null)
            {
                _confirmMenu.Add(new UIMenuItem("WHAT? NO!", new UIMenuActionOpenMenu(_confirmMenu, _returnMenu), backButton: true), true);
                _confirmMenu.Add(new UIMenuItem("YEAH!", new UIMenuActionOpenMenuSetBoolean(_confirmMenu, _returnMenu, _deleteFile)), true);
                _notOnlineMenu = new UIMenu("NO WAY", num1 / 2f, num2 / 2f, 160f, conString: "@SELECT@OH :(");
                BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
                UIText component1 = new UIText("THIS LEVEL CONTAINS", Color.White);
                component1.SetFont(f);
                _notOnlineMenu.Add(component1, true);
                UIText component2 = new UIText("OFFLINE ONLY STUFF.", Color.White);
                component2.SetFont(f);
                _notOnlineMenu.Add(component2, true);
                UIText component3 = new UIText(" ", Color.White);
                component3.SetFont(f);
                _notOnlineMenu.Add(component3, true);
                _notOnlineMenu.Add(new UIMenuItem("OH", new UIMenuActionOpenMenu(_confirmMenu, _returnMenu), backButton: true), true);
            }
            else
            {
                _confirmMenu.Add(new UIMenuItem("WHAT? NO!", new UIMenuActionCloseMenu(_confirmMenu), backButton: true), true);
                _confirmMenu.Add(new UIMenuItem("YEAH!", new UIMenuActionCloseMenuSetBoolean(_confirmMenu, _deleteFile)), true);
            }
            _confirmMenu.Close();
            Add(_confirmMenu);
        }

        public void AddItem(LSItem item) => _items.Add(item);

        public void PositionItems()
        {
            int num1 = 0;
            int num2 = 0;
            foreach (LSItem lsItem in _items)
            {
                if (num1 >= _topIndex + _maxItems || num1 < _topIndex)
                {
                    lsItem.visible = false;
                    ++num1;
                }
                else
                {
                    lsItem.visible = true;
                    lsItem.x = _leftPos;
                    lsItem.y = _topPos + num2 * 10;
                    if (num1 == _selectedItem)
                    {
                        lsItem.selected = true;
                        _selectedLevel = lsItem;
                    }
                    else
                        lsItem.selected = false;
                    ++num1;
                    ++num2;
                }
            }
        }

        public void FolderUp() => FolderUp(false);

        public void FolderUp(bool pIsModPath)
        {
            if (_currentDirectory == "@WORKSHOP@" || modRoot != null && modRoot.Contains(_currentDirectory) || _currentDirectory == "@VANILLA@")
            {
                SetCurrentFolder(_rootDirectory);
                modRoot = null;
            }
            else
                SetCurrentFolder(_currentDirectory.Substring(0, _currentDirectory.LastIndexOf('/')), pIsModPath, false);
        }

        public void HUDRefresh()
        {
            showPlaylistOption = false;
            HUDBottomRightSetup();
            HUDTopRightSetup();
        }

        private void HUDBottomRightSetup()
        {
            HUD.CloseCorner(HUDCorner.BottomRight);
            if (_selectedLevel.itemType == LSItemType.UpFolder)
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@RETURN");
            else if (_selectedLevel.itemType == LSItemType.Folder || _selectedLevel.itemType == LSItemType.Playlist || _selectedLevel.itemType == LSItemType.Workshop || _selectedLevel.itemType == LSItemType.Vanilla)
            {
                HUD.AddCornerControl(HUDCorner.BottomRight, "@MENU1@TOGGLE");
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@OPEN", allowStacking: true);
            }
            else
                HUD.AddCornerControl(HUDCorner.BottomRight, "@MENU1@TOGGLE");
        }

        private void HUDTopRightSetup()
        {
            HUD.CloseCorner(HUDCorner.TopRight);
            if (_currentDirectory == _rootDirectory)
            {
                HUD.AddCornerControl(HUDCorner.TopRight, "@CANCEL@DONE");
                HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@DELETE", allowStacking: true);
            }
            else
            {
                HUD.AddCornerControl(HUDCorner.TopRight, "@CANCEL@RETURN");
                if (modRoot != null || !(_currentDirectory != "@VANILLA@"))
                    return;
                HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@DELETE", allowStacking: true);
            }
        }

        public override void Update()
        {
            HUD.CloseCorner(HUDCorner.TopLeft);
            _dialog.DoUpdate();
            if (_dialog.opened)
                return;
            Editor.lockInput = null;
            if (_dialog.result != null && _dialog.result != "")
            {
                string result = _dialog.result;
                LevelPlaylist levelPlaylist = new LevelPlaylist();
                levelPlaylist.levels.AddRange(Editor.activatedLevels);
                DuckXML doc = new DuckXML();
                doc.Add(levelPlaylist.Serialize());
                DuckFile.SaveDuckXML(doc, DuckFile.levelDirectory + result + ".play");
                SetCurrentFolder(_rootDirectory);
                _dialog.result = null;
            }
            else
            {
                if (_selectedLevel == null)
                    _exiting = true;
                if (Editor.activatedLevels.Count > 0)
                {
                    if (!showPlaylistOption)
                    {
                        showPlaylistOption = true;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@RAGDOLL@NEW PLAYLIST");
                    }
                }
                else if (showPlaylistOption)
                {
                    showPlaylistOption = false;
                    HUD.CloseCorner(HUDCorner.BottomLeft);
                }
                if (_deleteFile.value)
                {
                    foreach (string str in _selectedLevel.levelsInside)
                        Editor.activatedLevels.Remove(str);
                    Editor.activatedLevels.Remove(_selectedLevel.path);
                    if (_selectedLevel.itemType == LSItemType.Folder)
                        DuckFile.DeleteFolder(DuckFile.levelDirectory + _selectedLevel.path);
                    else if (_selectedLevel.itemType == LSItemType.Playlist)
                        DuckFile.Delete(DuckFile.levelDirectory + _selectedLevel.path);
                    else
                        Editor.Delete(_selectedLevel.path);
                    Thread.Sleep(100);
                    SetCurrentFolder(_currentDirectory);
                    _deleteFile.value = false;
                }
                if (_exiting)
                {
                    HUD.CloseAllCorners();
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.04f);
                    if (Graphics.fade >= 0.01f)
                        return;
                    isClosed = true;
                }
                else
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.04f);
                    if (Input.Pressed(Triggers.MenuUp))
                    {
                        if (_selectedItem > 0)
                            --_selectedItem;
                        else
                            _selectedItem = _items.Count() - 1;
                        if (_selectedItem < _topIndex)
                            _topIndex = _selectedItem;
                        if (_selectedItem >= _topIndex + _maxItems)
                            _topIndex = _selectedItem + 1 - _maxItems;
                    }
                    else if (Input.Pressed(Triggers.MenuDown))
                    {
                        if (_selectedItem < _items.Count() - 1)
                            ++_selectedItem;
                        else
                            _selectedItem = 0;
                        if (_selectedItem < _topIndex)
                            _topIndex = _selectedItem;
                        if (_selectedItem >= _topIndex + _maxItems)
                            _topIndex = _selectedItem + 1 - _maxItems;
                    }
                    else if (Input.Pressed(Triggers.MenuLeft))
                    {
                        _selectedItem -= _maxItems - 1;
                        if (_selectedItem < 0)
                            _selectedItem = 0;
                        if (_selectedItem < _topIndex)
                            _topIndex = _selectedItem;
                    }
                    else if (Input.Pressed(Triggers.MenuRight))
                    {
                        _selectedItem += _maxItems - 1;
                        if (_selectedItem > _items.Count() - 1)
                            _selectedItem = _items.Count() - 1;
                        if (_selectedItem >= _topIndex + _maxItems)
                            _topIndex = _selectedItem + 1 - _maxItems;
                    }
                    else if (Input.Pressed(Triggers.Menu1))
                    {
                        if (_selectedLevel.itemType != LSItemType.UpFolder)
                        {
                            if (_selectedLevel.isFolder || _selectedLevel.itemType == LSItemType.Playlist || _selectedLevel.itemType == LSItemType.Workshop || _selectedLevel.itemType == LSItemType.Vanilla)
                            {
                                if (!_selectedLevel.enabled)
                                {
                                    _selectedLevel.enabled = true;
                                    _selectedLevel.partiallyEnabled = false;
                                    Editor.activatedLevels.AddRange(_selectedLevel.levelsInside);
                                }
                                else
                                {
                                    _selectedLevel.enabled = false;
                                    _selectedLevel.partiallyEnabled = false;
                                    foreach (string str in _selectedLevel.levelsInside)
                                        Editor.activatedLevels.Remove(str);
                                }
                            }
                            else if (Editor.activatedLevels.Contains(_selectedLevel.path))
                                Editor.activatedLevels.Remove(_selectedLevel.path);
                            else
                                Editor.activatedLevels.Add(_selectedLevel.path);
                        }
                    }
                    else if (Input.Pressed(Triggers.Select))
                    {
                        if (_selectedLevel.itemType == LSItemType.Workshop || _selectedLevel.itemType == LSItemType.Vanilla)
                            SetCurrentFolder(_selectedLevel.path);
                        else if (_selectedLevel.isFolder || _selectedLevel.itemType == LSItemType.Playlist)
                        {
                            if (_selectedLevel.isModRoot || _selectedLevel.isModPath)
                                SetCurrentFolder(_selectedLevel.path, _selectedLevel.isModPath, _selectedLevel.isModRoot, _selectedLevel.mapPack);
                            else
                                SetCurrentFolder(_rootDirectory + _selectedLevel.path);
                        }
                        else if (_selectedLevel.itemType == LSItemType.UpFolder)
                            FolderUp(_selectedLevel.isModPath);
                    }
                    else if (Input.Pressed(Triggers.Cancel))
                    {
                        if (_currentDirectory != _rootDirectory)
                            FolderUp(_selectedLevel.isModPath);
                        else
                            _exiting = true;
                    }
                    else if (Input.Pressed(Triggers.Ragdoll))
                    {
                        _dialog.Open("New Playlist...");
                        Editor.lockInput = _dialog;
                    }
                    else if (Input.Pressed(Triggers.Menu2) && modRoot == null && _currentDirectory != "@VANILLA@" && _selectedLevel.path != "@VANILLA@" && _currentDirectory != "@WORKSHOP@" && mapPack == null && MonoMain.pauseMenu != _confirmMenu && _selectedLevel.itemType != LSItemType.UpFolder && _selectedLevel.itemType != LSItemType.Workshop && _selectedLevel.itemType != LSItemType.MapPack)
                    {
                        _skipCompanionOpening = true;
                        MonoMain.pauseMenu = _confirmMenu;
                        HUD.CloseAllCorners();
                        _confirmMenu.Open();
                        SFX.Play("pause", 0.6f);
                    }
                    PositionItems();
                    if (_selectedLevel != _lastSelection)
                    {
                        if (_lastSelection == null || _selectedLevel.itemType != _lastSelection.itemType)
                            HUDBottomRightSetup();
                        _lastSelection = _selectedLevel;
                    }
                    if (_selectedLevel != _previewItem)
                    {
                        if (_selectedLevel.itemType == LSItemType.Level)
                        {
                            LevelMetaData.PreviewPair preview = Content.GeneratePreview(_selectedLevel.path);
                            if (preview != null)
                            {
                                _preview = preview.preview;
                                _previewSprite = _preview == null ? null : new Sprite((Tex2D)_preview);
                            }
                            else
                                _previewSprite = null;
                        }
                        else
                            _previewSprite = null;
                        _previewItem = _selectedLevel;
                    }
                    foreach (Thing thing in _items)
                        thing.Update();
                }
            }
        }

        public void DrawThings(bool drawBack = false)
        {
            if (drawBack)
                Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(Layer.HUD.camera.width, Layer.HUD.camera.height), Color.Black, -0.8f);
            foreach (LSItem lsItem in _items)
            {
                if (lsItem.visible)
                    lsItem.Draw();
            }
            Depth depth = _font.depth;
            if (_previewSprite != null)
            {
                _previewSprite.scale = new Vec2(0.5f, 0.5f);
                _previewSprite.depth = (Depth)0.9f;
                Graphics.Draw(_previewSprite, 150f, 45f);
            }
            else if (_selectedLevel.mapPack != null && _selectedLevel.mapPack.preview != null)
            {
                Tex2D preview = _selectedLevel.mapPack.preview;
                Vec2 vec2 = new Vec2(320f / preview.width, 180f / preview.height) * 0.5f;
                Graphics.Draw(preview, 150f, 45f, vec2.x, vec2.y);
            }
            _font.depth = depth;
            _font.Draw(mapPack == null ? (!(_currentDirectory == "@WORKSHOP@") ? (!(_currentDirectory == "@VANILLA@") ? "Levels" + _currentDirectory.Substring(_rootDirectory.Length, _currentDirectory.Length - _rootDirectory.Length) : "Levels/Deathmatch") : "Levels/Workshop") : mapPack.name, _leftPos, _topPos - 10f, Color.LimeGreen);
            _dialog.DoDraw();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
                DrawThings();
            base.PostDrawLayer(layer);
        }

        public override void Terminate()
        {
            _items.Clear();
            InputProfile.repeat = false;
            Keyboard.repeat = false;
        }
    }
}
