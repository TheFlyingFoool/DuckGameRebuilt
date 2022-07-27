// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelSelect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public List<IFilterLSItems> filters => this._filters;

        public LevelSelect(string root = "", Level returnLevel = null, UIMenu returnMenu = null, bool onlineMode = false)
        {
            this._centeredView = true;
            if (root == "")
                root = DuckFile.levelDirectory;
            root = root.TrimEnd('/');
            this._rootDirectory = root;
            this._font = new BitmapFont("biosFont", 8);
            this._returnLevel = returnLevel;
            this._dialog = new TextEntryDialog
            {
                filename = true
            };
            this._returnMenu = returnMenu;
            this._iconSheet = new SpriteMap("iconSheet", 16, 16);
            this._onlineMode = onlineMode;
            this._filters.Add(new LSFilterLevelType(LevelType.Deathmatch, true));
            this._filters.Add(new LSFilterMods(true));
        }

        public void SetCurrentFolder(string folder) => this.SetCurrentFolder(folder, false, false);

        public void SetCurrentFolder(string folder, bool isModPath, bool isModRoot, MapPack pMapPack = null)
        {
            if (isModRoot)
                this.modRoot = folder;
            this._currentDirectory = folder;
            if (this._currentDirectory == this._rootDirectory)
            {
                this._selectedItem = 0;
                this.mapPack = null;
            }
            else
                this._selectedItem = 1;
            if (pMapPack != null)
                this.mapPack = pMapPack;
            this.HUDTopRightSetup();
            this._topIndex = 0;
            this._items.Clear();
            if (this._currentDirectory != this._rootDirectory)
            {
                this.AddItem(new LSItem(0.0f, 0.0f, this, "../"));
            }
            else
            {
                this.AddItem(new LSItem(0.0f, 0.0f, this, "@VANILLA@")
                {
                    itemType = LSItemType.Vanilla
                });
                if (Steam.GetNumWorkshopItems() > 0)
                    this.AddItem(new LSItem(0.0f, 0.0f, this, "@WORKSHOP@", true));
            }
            if (folder.EndsWith(".play") || folder == "@WORKSHOP@" || folder == "@VANILLA@")
            {
                List<string> levelsInside = LSItem.GetLevelsInside(this, folder);
                levelsInside.Sort();
                foreach (string PATH in levelsInside)
                    this.AddItem(new LSItem(0.0f, 0.0f, this, PATH));
                this._items = this._items.OrderBy<LSItem, bool>(x => x.data != null && x.data.metaData.eightPlayer).ToList<LSItem>();
                this.PositionItems();
            }
            else
            {
                string[] directories = DuckFile.GetDirectories(folder);
                string[] files = DuckFile.GetFiles(folder);
                Array.Sort<string>(directories);
                Array.Sort<string>(files);
                if (this._currentDirectory == this._rootDirectory)
                {
                    foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                    {
                        if (accessibleMod.configuration != null && accessibleMod.configuration.content != null && accessibleMod.configuration.content.levels.Count > 0)
                            this.AddItem(new LSItem(0.0f, 0.0f, this, accessibleMod.configuration.contentDirectory + "/Levels", pIsModPath: true, pIsModRoot: true)
                            {
                                _name = accessibleMod.configuration.name
                            });
                    }
                    foreach (MapPack mapPack in MapPack.active)
                        this.AddItem(new LSItem(0.0f, 0.0f, this, mapPack.path, pIsModPath: true, pIsModRoot: true, pIsMapPack: true)
                        {
                            _name = mapPack.name,
                            _customIcon = mapPack.icon,
                            mapPack = mapPack
                        });
                }
                foreach (string str in directories)
                {
                    if (DuckFile.GetFiles(str, "*.lev", SearchOption.AllDirectories).Count<string>() > 0)
                        this.AddItem(new LSItem(0.0f, 0.0f, this, str, pIsModPath: isModPath));
                }
                List<string> stringList = new List<string>();
                foreach (string str in files)
                {
                    string file = str;
                    if (Path.GetExtension(file) == ".lev" && this._filters.TrueForAll(a => a.Filter(file)))
                        stringList.Add(file);
                    else if (Path.GetExtension(file) == ".play")
                        this.AddItem(new LSItem(0.0f, 0.0f, this, file, pIsModPath: isModPath));
                }
                foreach (string PATH in stringList)
                    this.AddItem(new LSItem(0.0f, 0.0f, this, PATH, pIsModPath: isModPath));
                this.PositionItems();
            }
        }

        public override void Initialize()
        {
            InputProfile.repeat = true;
            Keyboard.repeat = true;
            this.SetCurrentFolder(this._rootDirectory);
            this.isInitialized = true;
            this._dialog.DoInitialize();
            float num1 = 320f;
            float num2 = 180f;
            this._confirmMenu = new UIMenu("DELETE FILE!?", num1 / 2f, num2 / 2f, 160f, conString: "@CANCEL@CANCEL @SELECT@SELECT");
            if (this._returnMenu != null)
            {
                this._confirmMenu.Add(new UIMenuItem("WHAT? NO!", new UIMenuActionOpenMenu(_confirmMenu, _returnMenu), backButton: true), true);
                this._confirmMenu.Add(new UIMenuItem("YEAH!", new UIMenuActionOpenMenuSetBoolean(_confirmMenu, _returnMenu, this._deleteFile)), true);
                this._notOnlineMenu = new UIMenu("NO WAY", num1 / 2f, num2 / 2f, 160f, conString: "@SELECT@OH :(");
                BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
                UIText component1 = new UIText("THIS LEVEL CONTAINS", Color.White);
                component1.SetFont(f);
                this._notOnlineMenu.Add(component1, true);
                UIText component2 = new UIText("OFFLINE ONLY STUFF.", Color.White);
                component2.SetFont(f);
                this._notOnlineMenu.Add(component2, true);
                UIText component3 = new UIText(" ", Color.White);
                component3.SetFont(f);
                this._notOnlineMenu.Add(component3, true);
                this._notOnlineMenu.Add(new UIMenuItem("OH", new UIMenuActionOpenMenu(_confirmMenu, _returnMenu), backButton: true), true);
            }
            else
            {
                this._confirmMenu.Add(new UIMenuItem("WHAT? NO!", new UIMenuActionCloseMenu(_confirmMenu), backButton: true), true);
                this._confirmMenu.Add(new UIMenuItem("YEAH!", new UIMenuActionCloseMenuSetBoolean(_confirmMenu, this._deleteFile)), true);
            }
            this._confirmMenu.Close();
            Level.Add(_confirmMenu);
        }

        public void AddItem(LSItem item) => this._items.Add(item);

        public void PositionItems()
        {
            int num1 = 0;
            int num2 = 0;
            foreach (LSItem lsItem in this._items)
            {
                if (num1 >= this._topIndex + this._maxItems || num1 < this._topIndex)
                {
                    lsItem.visible = false;
                    ++num1;
                }
                else
                {
                    lsItem.visible = true;
                    lsItem.x = this._leftPos;
                    lsItem.y = this._topPos + num2 * 10;
                    if (num1 == this._selectedItem)
                    {
                        lsItem.selected = true;
                        this._selectedLevel = lsItem;
                    }
                    else
                        lsItem.selected = false;
                    ++num1;
                    ++num2;
                }
            }
        }

        public void FolderUp() => this.FolderUp(false);

        public void FolderUp(bool pIsModPath)
        {
            if (this._currentDirectory == "@WORKSHOP@" || this.modRoot != null && this.modRoot.Contains(this._currentDirectory) || this._currentDirectory == "@VANILLA@")
            {
                this.SetCurrentFolder(this._rootDirectory);
                this.modRoot = null;
            }
            else
                this.SetCurrentFolder(this._currentDirectory.Substring(0, this._currentDirectory.LastIndexOf('/')), pIsModPath, false);
        }

        public void HUDRefresh()
        {
            this.showPlaylistOption = false;
            this.HUDBottomRightSetup();
            this.HUDTopRightSetup();
        }

        private void HUDBottomRightSetup()
        {
            HUD.CloseCorner(HUDCorner.BottomRight);
            if (this._selectedLevel.itemType == LSItemType.UpFolder)
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@RETURN");
            else if (this._selectedLevel.itemType == LSItemType.Folder || this._selectedLevel.itemType == LSItemType.Playlist || this._selectedLevel.itemType == LSItemType.Workshop || this._selectedLevel.itemType == LSItemType.Vanilla)
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
            if (this._currentDirectory == this._rootDirectory)
            {
                HUD.AddCornerControl(HUDCorner.TopRight, "@CANCEL@DONE");
                HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@DELETE", allowStacking: true);
            }
            else
            {
                HUD.AddCornerControl(HUDCorner.TopRight, "@CANCEL@RETURN");
                if (this.modRoot != null || !(this._currentDirectory != "@VANILLA@"))
                    return;
                HUD.AddCornerControl(HUDCorner.TopRight, "@MENU2@DELETE", allowStacking: true);
            }
        }

        public override void Update()
        {
            HUD.CloseCorner(HUDCorner.TopLeft);
            this._dialog.DoUpdate();
            if (this._dialog.opened)
                return;
            Editor.lockInput = null;
            if (this._dialog.result != null && this._dialog.result != "")
            {
                string result = this._dialog.result;
                LevelPlaylist levelPlaylist = new LevelPlaylist();
                levelPlaylist.levels.AddRange(Editor.activatedLevels);
                DuckXML doc = new DuckXML();
                doc.Add(levelPlaylist.Serialize());
                DuckFile.SaveDuckXML(doc, DuckFile.levelDirectory + result + ".play");
                this.SetCurrentFolder(this._rootDirectory);
                this._dialog.result = null;
            }
            else
            {
                if (this._selectedLevel == null)
                    this._exiting = true;
                if (Editor.activatedLevels.Count > 0)
                {
                    if (!this.showPlaylistOption)
                    {
                        this.showPlaylistOption = true;
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@RAGDOLL@NEW PLAYLIST");
                    }
                }
                else if (this.showPlaylistOption)
                {
                    this.showPlaylistOption = false;
                    HUD.CloseCorner(HUDCorner.BottomLeft);
                }
                if (this._deleteFile.value)
                {
                    foreach (string str in this._selectedLevel.levelsInside)
                        Editor.activatedLevels.Remove(str);
                    Editor.activatedLevels.Remove(this._selectedLevel.path);
                    if (this._selectedLevel.itemType == LSItemType.Folder)
                        DuckFile.DeleteFolder(DuckFile.levelDirectory + this._selectedLevel.path);
                    else if (this._selectedLevel.itemType == LSItemType.Playlist)
                        DuckFile.Delete(DuckFile.levelDirectory + this._selectedLevel.path);
                    else
                        Editor.Delete(this._selectedLevel.path);
                    Thread.Sleep(100);
                    this.SetCurrentFolder(this._currentDirectory);
                    this._deleteFile.value = false;
                }
                if (this._exiting)
                {
                    HUD.CloseAllCorners();
                    DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 0.0f, 0.04f);
                    if ((double)DuckGame.Graphics.fade >= 0.00999999977648258)
                        return;
                    this.isClosed = true;
                }
                else
                {
                    DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 1f, 0.04f);
                    if (Input.Pressed("MENUUP"))
                    {
                        if (this._selectedItem > 0)
                            --this._selectedItem;
                        else
                            this._selectedItem = this._items.Count<LSItem>() - 1;
                        if (this._selectedItem < this._topIndex)
                            this._topIndex = this._selectedItem;
                        if (this._selectedItem >= this._topIndex + this._maxItems)
                            this._topIndex = this._selectedItem + 1 - this._maxItems;
                    }
                    else if (Input.Pressed("MENUDOWN"))
                    {
                        if (this._selectedItem < this._items.Count<LSItem>() - 1)
                            ++this._selectedItem;
                        else
                            this._selectedItem = 0;
                        if (this._selectedItem < this._topIndex)
                            this._topIndex = this._selectedItem;
                        if (this._selectedItem >= this._topIndex + this._maxItems)
                            this._topIndex = this._selectedItem + 1 - this._maxItems;
                    }
                    else if (Input.Pressed("MENULEFT"))
                    {
                        this._selectedItem -= this._maxItems - 1;
                        if (this._selectedItem < 0)
                            this._selectedItem = 0;
                        if (this._selectedItem < this._topIndex)
                            this._topIndex = this._selectedItem;
                    }
                    else if (Input.Pressed("MENURIGHT"))
                    {
                        this._selectedItem += this._maxItems - 1;
                        if (this._selectedItem > this._items.Count<LSItem>() - 1)
                            this._selectedItem = this._items.Count<LSItem>() - 1;
                        if (this._selectedItem >= this._topIndex + this._maxItems)
                            this._topIndex = this._selectedItem + 1 - this._maxItems;
                    }
                    else if (Input.Pressed("MENU1"))
                    {
                        if (this._selectedLevel.itemType != LSItemType.UpFolder)
                        {
                            if (this._selectedLevel.isFolder || this._selectedLevel.itemType == LSItemType.Playlist || this._selectedLevel.itemType == LSItemType.Workshop || this._selectedLevel.itemType == LSItemType.Vanilla)
                            {
                                if (!this._selectedLevel.enabled)
                                {
                                    this._selectedLevel.enabled = true;
                                    this._selectedLevel.partiallyEnabled = false;
                                    Editor.activatedLevels.AddRange(_selectedLevel.levelsInside);
                                }
                                else
                                {
                                    this._selectedLevel.enabled = false;
                                    this._selectedLevel.partiallyEnabled = false;
                                    foreach (string str in this._selectedLevel.levelsInside)
                                        Editor.activatedLevels.Remove(str);
                                }
                            }
                            else if (Editor.activatedLevels.Contains(this._selectedLevel.path))
                                Editor.activatedLevels.Remove(this._selectedLevel.path);
                            else
                                Editor.activatedLevels.Add(this._selectedLevel.path);
                        }
                    }
                    else if (Input.Pressed("SELECT"))
                    {
                        if (this._selectedLevel.itemType == LSItemType.Workshop || this._selectedLevel.itemType == LSItemType.Vanilla)
                            this.SetCurrentFolder(this._selectedLevel.path);
                        else if (this._selectedLevel.isFolder || this._selectedLevel.itemType == LSItemType.Playlist)
                        {
                            if (this._selectedLevel.isModRoot || this._selectedLevel.isModPath)
                                this.SetCurrentFolder(this._selectedLevel.path, this._selectedLevel.isModPath, this._selectedLevel.isModRoot, this._selectedLevel.mapPack);
                            else
                                this.SetCurrentFolder(this._rootDirectory + this._selectedLevel.path);
                        }
                        else if (this._selectedLevel.itemType == LSItemType.UpFolder)
                            this.FolderUp(this._selectedLevel.isModPath);
                    }
                    else if (Input.Pressed("CANCEL"))
                    {
                        if (this._currentDirectory != this._rootDirectory)
                            this.FolderUp(this._selectedLevel.isModPath);
                        else
                            this._exiting = true;
                    }
                    else if (Input.Pressed("RAGDOLL"))
                    {
                        this._dialog.Open("New Playlist...");
                        Editor.lockInput = _dialog;
                    }
                    else if (Input.Pressed("MENU2") && this.modRoot == null && this._currentDirectory != "@VANILLA@" && this._selectedLevel.path != "@VANILLA@" && this._currentDirectory != "@WORKSHOP@" && this.mapPack == null && MonoMain.pauseMenu != this._confirmMenu && this._selectedLevel.itemType != LSItemType.UpFolder && this._selectedLevel.itemType != LSItemType.Workshop && this._selectedLevel.itemType != LSItemType.MapPack)
                    {
                        LevelSelect._skipCompanionOpening = true;
                        MonoMain.pauseMenu = _confirmMenu;
                        HUD.CloseAllCorners();
                        this._confirmMenu.Open();
                        SFX.Play("pause", 0.6f);
                    }
                    this.PositionItems();
                    if (this._selectedLevel != this._lastSelection)
                    {
                        if (this._lastSelection == null || this._selectedLevel.itemType != this._lastSelection.itemType)
                            this.HUDBottomRightSetup();
                        this._lastSelection = this._selectedLevel;
                    }
                    if (this._selectedLevel != this._previewItem)
                    {
                        if (this._selectedLevel.itemType == LSItemType.Level)
                        {
                            LevelMetaData.PreviewPair preview = Content.GeneratePreview(this._selectedLevel.path);
                            if (preview != null)
                            {
                                this._preview = preview.preview;
                                this._previewSprite = this._preview == null ? null : new Sprite((Tex2D)this._preview);
                            }
                            else
                                this._previewSprite = null;
                        }
                        else
                            this._previewSprite = null;
                        this._previewItem = this._selectedLevel;
                    }
                    foreach (Thing thing in this._items)
                        thing.Update();
                }
            }
        }

        public void DrawThings(bool drawBack = false)
        {
            if (drawBack)
                DuckGame.Graphics.DrawRect(new Vec2(0.0f, 0.0f), new Vec2(Layer.HUD.camera.width, Layer.HUD.camera.height), Color.Black, -0.8f);
            foreach (LSItem lsItem in this._items)
            {
                if (lsItem.visible)
                    lsItem.Draw();
            }
            Depth depth = this._font.depth;
            if (this._previewSprite != null)
            {
                this._previewSprite.scale = new Vec2(0.5f, 0.5f);
                this._previewSprite.depth = (Depth)0.9f;
                DuckGame.Graphics.Draw(this._previewSprite, 150f, 45f);
            }
            else if (this._selectedLevel.mapPack != null && this._selectedLevel.mapPack.preview != null)
            {
                Tex2D preview = this._selectedLevel.mapPack.preview;
                Vec2 vec2 = new Vec2(320f / preview.width, 180f / preview.height) * 0.5f;
                DuckGame.Graphics.Draw(preview, 150f, 45f, vec2.x, vec2.y);
            }
            this._font.depth = depth;
            this._font.Draw(this.mapPack == null ? (!(this._currentDirectory == "@WORKSHOP@") ? (!(this._currentDirectory == "@VANILLA@") ? "Levels" + this._currentDirectory.Substring(this._rootDirectory.Length, this._currentDirectory.Length - this._rootDirectory.Length) : "Levels/Deathmatch") : "Levels/Workshop") : this.mapPack.name, this._leftPos, this._topPos - 10f, Color.LimeGreen);
            this._dialog.DoDraw();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
                this.DrawThings();
            base.PostDrawLayer(layer);
        }

        public override void Terminate()
        {
            this._items.Clear();
            InputProfile.repeat = false;
            Keyboard.repeat = false;
        }
    }
}
