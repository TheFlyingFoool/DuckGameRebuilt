// Decompiled with JetBrains decompiler
// Type: DuckGame.UIModManagement
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DuckGame
{
    public class UIModManagement : UIMenu
    {
        private const int FO_DELETE = 3;
        private const int FOF_ALLOWUNDO = 64;
        private const int FOF_NOCONFIRMATION = 16;
        private UIMenu _openOnClose;
        private Sprite _moreArrow;
        private Sprite _noImage;
        private Sprite _steamIcon;
        private SpriteMap _cursor;
        private SpriteMap _localIcon;
        private SpriteMap _newIcon;
        private SpriteMap _settingsIcon;
        private IList<Mod> _mods;
        private int _hoverIndex;
        private UIBox _box;
        private FancyBitmapFont _fancyFont;
        private int _maxModsToShow;
        private UIMenuItem _uploadItem;
        private UIMenuItem _disableOrEnableItem;
        private UIMenuItem _deleteOrUnsubItem;
        private UIMenuItem _visitItem;
        public UIMenu _editModMenu;
        public UIMenu _yesNoMenu;
        private UIMenuItem _yesNoYes;
        private UIMenuItem _yesNoNo;
        private SteamUploadDialog _uploadDialog;
        private WorkshopItem _transferItem;
        private bool _transferring;
        private bool _awaitingChanges;
        private Textbox _updateTextBox;
        private Rectangle _updateButton;
        private string _updateButtonText = "UPDATE MOD!";
        private int _pressWait;
        private Sprite _modErrorIcon;
        public UIMenu _modSettingsMenu;
        private bool _showingMenu;
        private bool _draggingScrollbar;
        private Vec2 _oldPos;
        private Mod _selectedMod;
        private bool modsChanged;
        public string showingError;
        private bool fixView = true;
        private const int boxHeight = 36;
        private const int scrollWidth = 12;
        private const int boxSideMargin = 14;
        private const int scrollBarHeight = 32;
        private int scrollBarTop;
        private int scrollBarBottom;
        private int scrollBarScrollableHeight;
        private int scrollBarOffset;
        private int _scrollItemOffset;
        private bool _gamepadMode = true;
        private bool _needsUpdateNotes;

        public override void Close()
        {
            if (!fixView)
            {
                _showingMenu = false;
                _editModMenu.Close();
                Layer.HUD.camera.width /= 2f;
                Layer.HUD.camera.height /= 2f;
                fixView = true;
                DevConsole.RestoreDevConsole();
            }
            base.Close();
        }

        private void EnableDisableMod()
        {
            _awaitingChanges = true;
            if (_selectedMod.configuration.disabled)
                _selectedMod.configuration.Enable();
            else
                _selectedMod.configuration.Disable();
            modsChanged = true;
            _editModMenu.Close();
            Open();
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        private static void DeleteFileOrFolder(string path)
        {
            SHFILEOPSTRUCT fileop = default(SHFILEOPSTRUCT);
            fileop.wFunc = 3;
            fileop.pFrom = path + "\0\0";
            fileop.fFlags = 80;
            SHFileOperation(ref fileop);
        }

        private void DeleteMod() => ShowYesNo(_editModMenu, () =>
       {
           _awaitingChanges = true;
           if (_selectedMod.configuration.workshopID == 0UL)
               DeleteFileOrFolder(_selectedMod.configuration.directory);
           else
               Steam.WorkshopUnsubscribe(_selectedMod.configuration.workshopID);
           _mods.Remove(_selectedMod);
           _hoverIndex = -1;
           _yesNoMenu.Close();
           _editModMenu.Close();
           Open();
       });

        private void ShowYesNo(UIMenu goBackTo, UIMenuActionCallFunction.Function onYes)
        {
            _yesNoNo.menuAction = new UIMenuActionCallFunction(() =>
          {
              _yesNoMenu.Close();
              goBackTo.Open();
          });
            _yesNoYes.menuAction = new UIMenuActionCallFunction(onYes);
            new UIMenuActionOpenMenu(_editModMenu, _yesNoMenu).Activate();
        }

        private void UploadMod()
        {
            _editModMenu.Close();
            Open();
            if (_selectedMod.configuration.workshopID == 0UL)
            {
                _transferItem = Steam.CreateItem();
            }
            else
            {
                _transferItem = new WorkshopItem(_selectedMod.configuration.workshopID);
                _needsUpdateNotes = true;
                _updateTextBox.GainFocus();
                _gamepadMode = false;
            }
            _transferring = false;
        }

        private void VisitModPage()
        {
            _editModMenu.Close();
            Open();
            Steam.OverlayOpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + _selectedMod.configuration.workshopID.ToString());
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] directories = directoryInfo1.GetDirectories();
            if (!directoryInfo1.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);
            foreach (FileInfo file in directoryInfo1.GetFiles())
            {
                string str = Path.Combine(destDirName, file.Name);
                file.CopyTo(str, false);
                File.SetAttributes(str, FileAttributes.Normal);
            }
            if (!copySubDirs)
                return;
            foreach (DirectoryInfo directoryInfo2 in directories)
            {
                string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
                DirectoryCopy(directoryInfo2.FullName, destDirName1, copySubDirs);
            }
        }

        public UIModManagement(
          UIMenu openOnClose,
          string title,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          string conString = "",
          InputProfile conProfile = null)
          : base(title, xpos, ypos, wide, high, conString, conProfile)
        {
            domouse = false;
            _splitter.topSection.components[0].align = UIAlign.Left;
            _openOnClose = openOnClose;
            _moreArrow = new Sprite("moreArrow");
            _moreArrow.CenterOrigin();
            _steamIcon = new Sprite("steamIconSmall")
            {
                scale = new Vec2(1f) / 2f
            };
            _localIcon = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(1f) / 2f
            };
            _localIcon.SetFrameWithoutReset(1);
            _modErrorIcon = new Sprite("modloadError");
            _newIcon = new SpriteMap("presents", 16, 16)
            {
                scale = new Vec2(2f)
            };
            _newIcon.SetFrameWithoutReset(0);
            _settingsIcon = new SpriteMap("settingsWrench", 16, 16)
            {
                scale = new Vec2(2f)
            };
            _noImage = new Sprite("notexture")
            {
                scale = new Vec2(2f)
            };
            _cursor = new SpriteMap("cursors", 16, 16);
            _mods = ModLoader.allMods.Where(a => !(a is CoreMod)).ToList();
            _mods.Insert(0, new UI_ModSettings());
            _mods.Add(null);
            _maxModsToShow = 8;
            _box = new UIBox(0f, 0f, high: _maxModsToShow * 36, isVisible: false);
            Add(_box, true);
            _fancyFont = new FancyBitmapFont("smallFont")
            {
                maxWidth = (int)width - 60,
                maxRows = 2
            };
            scrollBarOffset = 0;
            _editModMenu = new UIMenu("<mod name>", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@SELECT@SELECT");
            _editModMenu.Add(_disableOrEnableItem = new UIMenuItem("DISABLE", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(EnableDisableMod))), true);
            _deleteOrUnsubItem = new UIMenuItem("DELETE", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(DeleteMod)));
            _uploadItem = new UIMenuItem("UPLOAD", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(UploadMod)));
            _visitItem = new UIMenuItem("VISIT PAGE", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(VisitModPage)));
            _editModMenu.Add(new UIText(" ", Color.White), true);
            _editModMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_editModMenu, this)), true);
            _editModMenu.Close();
            _yesNoMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            _yesNoMenu.Add(_yesNoYes = new UIMenuItem("YES"), true);
            _yesNoMenu.Add(_yesNoNo = new UIMenuItem("NO"), true);
            _yesNoMenu.Close();
            _updateTextBox = new Textbox(0f, 0f, 0f, 0f)
            {
                depth = (Depth)0.9f,
                maxLength = 5000
            };
            _modSettingsMenu = new UIMenu("@WRENCH@MOD SETTINGS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@WASD@ADJUST @CANCEL@EXIT");
            _modSettingsMenu.Add(new UIText("If CRASH DISABLE is ON,", Colors.DGBlue), true);
            _modSettingsMenu.Add(new UIText("a mod will automatically be", Colors.DGBlue), true);
            _modSettingsMenu.Add(new UIText(" disabled if it causes", Colors.DGBlue), true);
            _modSettingsMenu.Add(new UIText("the game to crash.", Colors.DGBlue), true);
            _modSettingsMenu.Add(new UIText(" ", Colors.DGBlue), true);
            _modSettingsMenu.Add(new UIMenuItemToggle("CRASH DISABLE", field: new FieldBinding(Options.Data, "disableModOnCrash")), true);
            _modSettingsMenu.Add(new UIMenuItemToggle("LOAD FAILURE DISABLE", field: new FieldBinding(Options.Data, "disableModOnLoadFailure")), true);
            _modSettingsMenu.Add(new UIMenuItemToggle("SHOW NETWORK WARNING", field: new FieldBinding(Options.Data, "showNetworkModWarning")), true);
            _modSettingsMenu.Add(new UIText(" ", Colors.DGBlue), true);
            _modSettingsMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_modSettingsMenu, this), backButton: true), true);
            _modSettingsMenu.Close();
        }

        public override void Open()
        {
            if (_uploadDialog == null)
            {
                _uploadDialog = new SteamUploadDialog();
                Level.Add(_uploadDialog);
                Level.current.things.RefreshState();
            }
            _pressWait = 30;
            base.Open();
            DevConsole.SuppressDevConsole();
            _oldPos = Mouse.positionScreen;
        }

        public override void Update()
        {
            if (_uploadDialog != null && _uploadDialog.opened)
            {
                Editor.clickedMenu = false;
                Editor.inputMode = EditorInput.Mouse;
                Level.current.things.RefreshState();
                foreach (Thing thing in Level.current.things[typeof(ContextMenu)])
                    thing.Update();
            }
            else
            {
                if (_pressWait > 0)
                    --_pressWait;
                if (showingError != null)
                {
                    _controlString = "@CANCEL@BACK";
                    if (Input.Pressed(Triggers.Quack))
                        showingError = null;
                    base.Update();
                }
                else
                {
                    if (_editModMenu.open)
                    {
                        if (!globalUILock && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
                        {
                            _editModMenu.Close();
                            Open();
                            return;
                        }
                    }
                    else if (_modSettingsMenu.open)
                    {
                        if (!globalUILock && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
                        {
                            _modSettingsMenu.Close();
                            Open();
                            return;
                        }
                    }
                    else if (open)
                    {
                        if (_transferItem != null && !_needsUpdateNotes)
                        {
                            if (!_transferring)
                            {
                                if (_transferItem.result == SteamResult.OK)
                                {
                                    WorkshopItemData dat = new WorkshopItemData();
                                    if (_selectedMod.configuration.workshopID == 0UL)
                                    {
                                        _selectedMod.configuration.SetWorkshopID(_transferItem.id);
                                        dat.name = _selectedMod.configuration.displayName;
                                        dat.description = _selectedMod.configuration.description;
                                        dat.visibility = RemoteStoragePublishedFileVisibility.Private;
                                        dat.tags = new List<string>();
                                        dat.tags.Add("Mod");
                                        if (_selectedMod.configuration.modType == ModConfiguration.Type.MapPack)
                                            dat.tags.Add("Map Pack");
                                        else if (_selectedMod.configuration.modType == ModConfiguration.Type.HatPack)
                                            dat.tags.Add("Hat Pack");
                                        else if (_selectedMod.configuration.modType == ModConfiguration.Type.Reskin)
                                            dat.tags.Add("Texture Pack");
                                    }
                                    else
                                        dat.changeNotes = _updateTextBox.text;
                                    string pathToScreenshot = _selectedMod.generateAndGetPathToScreenshot;
                                    dat.previewPath = pathToScreenshot;
                                    string str = DuckFile.workshopDirectory + _transferItem.id.ToString() + "/content";
                                    if (Directory.Exists(str))
                                        Directory.Delete(str, true);
                                    DuckFile.CreatePath(str);
                                    DirectoryCopy(_selectedMod.configuration.directory, str + "/" + _selectedMod.configuration.name, true);
                                    if (Directory.Exists(str + _selectedMod.configuration.name + "/build"))
                                        Directory.Delete(str + _selectedMod.configuration.name + "/build", true);
                                    if (Directory.Exists(str + _selectedMod.configuration.name + "/.vs"))
                                        Directory.Delete(str + _selectedMod.configuration.name + "/.vs", true);
                                    if (File.Exists(str + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.dll"))
                                    {
                                        string path = str + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.dll";
                                        File.SetAttributes(path, FileAttributes.Normal);
                                        File.Delete(path);
                                    }
                                    if (File.Exists(str + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.hash"))
                                    {
                                        string path = str + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.hash";
                                        File.SetAttributes(path, FileAttributes.Normal);
                                        File.Delete(path);
                                    }
                                    dat.contentFolder = str;
                                    _transferItem.ApplyWorkshopData(dat);
                                    if (_transferItem.needsLegal)
                                        Steam.ShowWorkshopLegalAgreement(_transferItem.id.ToString());
                                    _transferring = true;
                                    _transferItem.ResetProcessing();
                                }
                            }
                            else if (_transferItem.finishedProcessing)
                            {
                                Steam.OverlayOpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + _transferItem.id.ToString());
                                Directory.Delete(DuckFile.workshopDirectory + _transferItem.id.ToString() + "/", true);
                                _transferItem.ResetProcessing();
                                _transferItem = null;
                                _transferring = false;
                            }
                            base.Update();
                            return;
                        }
                        if (_gamepadMode)
                        {
                            if (_hoverIndex < 0)
                                _hoverIndex = 0;
                        }
                        else
                        {
                            _hoverIndex = -1;
                            for (int index = 0; index < _maxModsToShow && _scrollItemOffset + index < _mods.Count; ++index)
                            {
                                if (new Rectangle((int)(_box.x - _box.halfWidth), (int)(_box.y - _box.halfHeight + 36 * index), (int)_box.width - 14, 36f).Contains(Mouse.position))
                                {
                                    _hoverIndex = _scrollItemOffset + index;
                                    break;
                                }
                            }
                        }
                        if (_transferItem != null)
                        {
                            if (_updateTextBox != null)
                            {
                                Editor.hoverTextBox = false;
                                _updateTextBox.position = new Vec2((float)(_box.x - _box.halfWidth + 16.0), (float)(_box.y - _box.halfHeight + 48.0));
                                _updateTextBox.size = new Vec2(_box.width - 32f, _box.height - 80f);
                                _updateTextBox._maxLines = (int)(_updateTextBox.size.y / _fancyFont.characterHeight);
                                _updateTextBox.Update();
                                float stringWidth = Graphics.GetStringWidth(_updateButtonText, scale: 2f);
                                float height = Graphics.GetStringHeight(_updateButtonText) * 2f;
                                _updateButton = new Rectangle(_box.x - stringWidth / 2f, (float)(_box.y + _box.halfHeight - 24.0), stringWidth, height);
                                if (_updateButton.Contains(Mouse.position) && Mouse.left == InputState.Pressed)
                                {
                                    _needsUpdateNotes = false;
                                    _updateTextBox.LoseFocus();
                                }
                                else if (Keyboard.Pressed(Keys.Escape))
                                {
                                    _needsUpdateNotes = false;
                                    _transferItem = null;
                                    _updateTextBox.LoseFocus();
                                    new UIMenuActionOpenMenu(this, _editModMenu).Activate();
                                    return;
                                }
                            }
                        }
                        else if (_hoverIndex != -1)
                        {
                            _selectedMod = _mods[_hoverIndex];
                            if (_selectedMod is UI_ModSettings)
                                _controlString = "@WASD@@SELECT@SETTINGS @CANCEL@BACK";
                            else if (_selectedMod != null && _selectedMod.configuration.error != null)
                            {
                                if (_selectedMod.configuration.forceHarmonyLegacyLoad)
                                    _controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @MENU2@DISABLE FORCED LOAD @START@SHOW ERROR";
                                else
                                    _controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @MENU2@FORCE LEGACY LOAD @START@SHOW ERROR";
                            }
                            else
                                _controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @CANCEL@BACK";
                            if (Input.Pressed(Triggers.Menu1))
                            {
                                if (_selectedMod != null && _selectedMod.configuration != null)
                                {
                                    if (_selectedMod.configuration.disabled)
                                        _selectedMod.configuration.Enable();
                                    else
                                        _selectedMod.configuration.Disable();
                                    _selectedMod.configuration.error = null;
                                    modsChanged = true;
                                    SFX.Play("rockHitGround", 0.8f);
                                }
                            }
                            else if (_selectedMod != null && _selectedMod.configuration != null && _selectedMod.configuration.error != null && Input.Pressed(Triggers.Menu2))
                            {
                                if (_selectedMod.configuration != null)
                                {
                                    _selectedMod.configuration.forceHarmonyLegacyLoad = !_selectedMod.configuration.forceHarmonyLegacyLoad;
                                    ModLoader.DisabledModsChanged();
                                    modsChanged = true;
                                    SFX.Play("rockHitGround", 0.8f);
                                }
                            }
                            else
                            {
                                if (Input.Pressed(Triggers.Start) && _selectedMod != null && _selectedMod.configuration != null && _selectedMod.configuration.error != null)
                                {
                                    string str = DuckFile.saveDirectory + "error_info.txt";
                                    File.WriteAllText(str, _selectedMod.configuration.error);
                                    Process.Start(str);
                                    SFX.Play("rockHitGround", 0.8f);
                                    return;
                                }
                                if (Input.Pressed(Triggers.Select) && _pressWait == 0 && _gamepadMode || !_gamepadMode && Mouse.left == InputState.Pressed)
                                {
                                    if (_selectedMod != null)
                                    {
                                        if (_selectedMod is UI_ModSettings)
                                        {
                                            SFX.Play("rockHitGround", 0.8f);
                                            _modSettingsMenu.dirty = true;
                                            new UIMenuActionOpenMenu(this, _modSettingsMenu).Activate();
                                            return;
                                        }
                                        _editModMenu.title = _selectedMod.configuration.loaded ? "|YELLOW|" + _selectedMod.configuration.displayName : "|YELLOW|" + _selectedMod.configuration.name;
                                        _editModMenu.Remove(_deleteOrUnsubItem);
                                        _editModMenu.Remove(_uploadItem);
                                        _editModMenu.Remove(_visitItem);
                                        if (!_selectedMod.configuration.isWorkshop && _selectedMod.configuration.loaded)
                                        {
                                            _uploadItem.text = _selectedMod.configuration.workshopID == 0UL ? "UPLOAD" : "UPDATE";
                                            _editModMenu.Insert(_uploadItem, 1, true);
                                        }
                                        if (!_selectedMod.configuration.isWorkshop && !_selectedMod.configuration.loaded)
                                        {
                                            _deleteOrUnsubItem.text = "DELETE";
                                            _editModMenu.Insert(_deleteOrUnsubItem, 1, true);
                                        }
                                        else if (_selectedMod.configuration.isWorkshop)
                                        {
                                            _deleteOrUnsubItem.text = "UNSUBSCRIBE";
                                            _editModMenu.Insert(_deleteOrUnsubItem, 1, true);
                                        }
                                        if (_selectedMod.configuration.isWorkshop)
                                            _editModMenu.Insert(_visitItem, 1, true);
                                        _disableOrEnableItem.text = _selectedMod.configuration.disabled ? "ENABLE" : "DISABLE";
                                        _editModMenu.dirty = true;
                                        SFX.Play("rockHitGround", 0.8f);
                                        new UIMenuActionOpenMenu(this, _editModMenu).Activate();
                                        return;
                                    }
                                    Steam.OverlayOpenURL("http://steamcommunity.com/workshop/browse/?appid=312530&searchtext=&childpublishedfileid=0&browsesort=trend&section=readytouseitems&requiredtags%5B%5D=Mod");
                                }
                            }
                        }
                        else
                            _selectedMod = null;
                        if (_gamepadMode)
                        {
                            _draggingScrollbar = false;
                            if (Input.Pressed(Triggers.MenuDown))
                                ++_hoverIndex;
                            else if (Input.Pressed(Triggers.MenuUp))
                                --_hoverIndex;
                            if (Input.Pressed(Triggers.Strafe))
                                _hoverIndex -= 10;
                            else if (Input.Pressed(Triggers.Ragdoll))
                                _hoverIndex += 10;
                            if (_hoverIndex < 0)
                                _hoverIndex = 0;
                            if ((_oldPos - Mouse.positionScreen).lengthSq > 200.0)
                                _gamepadMode = false;
                        }
                        else
                        {
                            if (!_draggingScrollbar)
                            {
                                if (Mouse.left == InputState.Pressed && ScrollBarBox().Contains(Mouse.position))
                                {
                                    _draggingScrollbar = true;
                                    _oldPos = Mouse.position;
                                }
                                if (Mouse.scroll > 0.0)
                                {
                                    _scrollItemOffset += 5;
                                    _hoverIndex += 5;
                                }
                                else if (Mouse.scroll < 0.0)
                                {
                                    _scrollItemOffset -= 5;
                                    _hoverIndex -= 5;
                                    if (_hoverIndex < 0)
                                        _hoverIndex = 0;
                                }
                            }
                            else if (Mouse.left != InputState.Down)
                            {
                                _draggingScrollbar = false;
                            }
                            else
                            {
                                Vec2 vec2 = Mouse.position - _oldPos;
                                _oldPos = Mouse.position;
                                scrollBarOffset += (int)vec2.y;
                                if (scrollBarOffset > scrollBarScrollableHeight)
                                    scrollBarOffset = scrollBarScrollableHeight;
                                else if (scrollBarOffset < 0)
                                    scrollBarOffset = 0;
                                _scrollItemOffset = (int)((_mods.Count - _maxModsToShow) * (scrollBarOffset / (float)scrollBarScrollableHeight));
                            }
                            if (Input.Pressed(Triggers.Any))
                            {
                                _gamepadMode = true;
                                _oldPos = Mouse.positionScreen;
                            }
                        }
                        if (_scrollItemOffset < 0)
                            _scrollItemOffset = 0;
                        else if (_scrollItemOffset > Math.Max(0, _mods.Count - _maxModsToShow))
                            _scrollItemOffset = Math.Max(0, _mods.Count - _maxModsToShow);
                        if (_hoverIndex >= _mods.Count)
                            _hoverIndex = _mods.Count - 1;
                        else if (_hoverIndex >= _scrollItemOffset + _maxModsToShow)
                            _scrollItemOffset += _hoverIndex - (_scrollItemOffset + _maxModsToShow) + 1;
                        else if (_hoverIndex >= 0 && _hoverIndex < _scrollItemOffset)
                            _scrollItemOffset -= _scrollItemOffset - _hoverIndex;
                        scrollBarOffset = _scrollItemOffset == 0 ? 0 : (int)Lerp.FloatSmooth(0f, scrollBarScrollableHeight, _scrollItemOffset / (float)(_mods.Count - _maxModsToShow));
                        if (!Editor.hoverTextBox && !globalUILock && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
                        {
                            if (modsChanged)
                            {
                                Close();
                                MonoMain.pauseMenu = DuckNetwork.OpenModsRestartWindow(_openOnClose);
                            }
                            else
                                new UIMenuActionOpenMenu(this, _openOnClose).Activate();
                            modsChanged = false;
                            return;
                        }
                    }
                    if (_showingMenu)
                    {
                        HUD.CloseAllCorners();
                        _showingMenu = false;
                    }
                    base.Update();
                }
            }
        }

        private Rectangle ScrollBarBox() => new Rectangle((float)(_box.x + _box.halfWidth - 12.0 + 1.0), (float)(_box.y - _box.halfHeight + 1.0) + scrollBarOffset, 10f, 32f);

        public override void Draw()
        {
            if (open)
            {
                if (Mouse.available && !_gamepadMode)
                {
                    _cursor.depth = (Depth)1f;
                    _cursor.scale = new Vec2(1f, 1f);
                    _cursor.position = Mouse.position;
                    _cursor.frame = 0;
                    if (Editor.hoverTextBox)
                    {
                        _cursor.frame = 7;
                        _cursor.position.y -= 4f;
                        _cursor.scale = new Vec2(0.5f, 1f);
                    }
                    _cursor.Draw();
                }
                if (_uploadDialog != null && _uploadDialog.opened)
                {
                    Editor.hoverTextBox = false;
                    _gamepadMode = false;
                    using (IEnumerator<Thing> enumerator = Level.current.things[typeof(ContextMenu)].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            enumerator.Current.Draw();
                        return;
                    }
                }
                else
                {
                    if (showingError != null)
                    {
                        float num1 = _box.x - _box.halfWidth;
                        float num2 = _box.y - _box.halfHeight;
                        _fancyFont.scale = new Vec2(1f);
                        int maxWidth = _fancyFont.maxWidth;
                        _fancyFont.maxRows = 40;
                        _fancyFont.maxWidth = (int)width - 10;
                        _fancyFont.Draw(showingError, new Vec2(num1 + 4f, num2 + 4f), Color.White, (Depth)0.5f);
                        _fancyFont.maxRows = 2;
                        _fancyFont.maxWidth = maxWidth;
                        base.Draw();
                        return;
                    }
                    scrollBarTop = (int)(_box.y - _box.halfHeight + 1.0 + 16.0);
                    scrollBarBottom = (int)(_box.y + _box.halfHeight - 1.0 - 16.0);
                    scrollBarScrollableHeight = scrollBarBottom - scrollBarTop;
                    if (fixView)
                    {
                        Layer.HUD.camera.width *= 2f;
                        Layer.HUD.camera.height *= 2f;
                        fixView = false;
                    }
                    Graphics.DrawRect(new Vec2(_box.x - _box.halfWidth, _box.y - _box.halfHeight), new Vec2((float)(_box.x + _box.halfWidth - 12.0 - 2.0), _box.y + _box.halfHeight), Color.Black, (Depth)0.4f);
                    Graphics.DrawRect(new Vec2((float)(_box.x + _box.halfWidth - 12.0), _box.y - _box.halfHeight), new Vec2(_box.x + _box.halfWidth, _box.y + _box.halfHeight), Color.Black, (Depth)0.4f);
                    Rectangle r = ScrollBarBox();
                    Graphics.DrawRect(r, _draggingScrollbar || r.Contains(Mouse.position) ? Color.LightGray : Color.Gray, (Depth)0.5f);
                    for (int index1 = 0; index1 < _maxModsToShow; ++index1)
                    {
                        int index2 = _scrollItemOffset + index1;
                        if (index2 < _mods.Count)
                        {
                            float x = _box.x - _box.halfWidth;
                            float y = _box.y - _box.halfHeight + 36 * index1;
                            if (_transferItem == null && _hoverIndex == index2)
                                Graphics.DrawRect(new Vec2(x, y), new Vec2((float)(x + _box.width - 14.0), y + 36f), Color.White * 0.6f, (Depth)0.45f);
                            else if ((index2 & 1) != 0)
                                Graphics.DrawRect(new Vec2(x, y), new Vec2((float)(x + _box.width - 14.0), y + 36f), Color.White * 0.1f, (Depth)0.45f);
                            Mod mod = _mods[index2];
                            if (mod != null)
                            {
                                if (mod is UI_ModSettings)
                                {
                                    Graphics.Draw(_settingsIcon, x + 2f, y + 1f, (Depth)0.5f);
                                    _fancyFont.scale = new Vec2(1.5f);
                                    _fancyFont.Draw("Mod Settings", new Vec2(x + 36f, y + 11f), Color.White, (Depth)0.5f);
                                    _fancyFont.scale = new Vec2(1f);
                                }
                                else
                                {
                                    Tex2D previewTexture = mod.previewTexture;
                                    if (previewTexture != null && _noImage.texture != previewTexture)
                                    {
                                        _noImage.texture = previewTexture;
                                        _noImage.scale = new Vec2(32f / previewTexture.width);
                                    }
                                    Graphics.DrawRect(new Vec2(x + 2f, y + 2f), new Vec2((float)(x + 36.0 - 2.0), (float)(y + 36.0 - 2.0)), Color.Gray, (Depth)0.44f, false, 2f);
                                    Graphics.Draw(_noImage, x + 2f, y + 2f, (Depth)0.5f);
                                    string str1 = "#" + index2.ToString() + ": ";
                                    if (mod.configuration.error != null)
                                    {
                                        _modErrorIcon.scale = new Vec2(2f);
                                        Graphics.Draw(_modErrorIcon, x + 2f, y + 2f, (Depth)0.55f);
                                        str1 += "|DGRED|";
                                    }
                                    if (mod.configuration.error != null || mod.configuration.disabled)
                                        Graphics.DrawRect(new Vec2(x, y), new Vec2((float)(x + _box.width - 14.0), y + 36f), Color.Black * 0.4f, (Depth)0.85f);
                                    bool flag = mod.configuration.modType == ModConfiguration.Type.Reskin || mod.configuration.isExistingReskinMod;
                                    string str2;
                                    if (!mod.configuration.loaded)
                                        str2 = str1 + mod.configuration.name;
                                    else if (flag)
                                        str2 = str1 + mod.configuration.displayName + "|WHITE| by |PURPLE|" + mod.configuration.author;
                                    else
                                        str2 = str1 + mod.configuration.displayName + "|WHITE| v" + mod.configuration.version.ToString() + " by |PURPLE|" + mod.configuration.author;
                                    if (flag)
                                        str2 += "|DGPURPLE| (Reskin Pack)";
                                    else if (mod.configuration.modType == ModConfiguration.Type.MapPack)
                                        str2 += "|DGPURPLE| (Map Pack)";
                                    else if (mod.configuration.modType == ModConfiguration.Type.HatPack)
                                        str2 += "|DGPURPLE| (Hat Pack)";
                                    _fancyFont.Draw(str2 + (mod.configuration.disabled ? "|DGRED| (Disabled)" : "|DGGREEN| (Enabled)"), new Vec2((float)(x + 36.0 + 10.0), y + 2f), Color.Yellow, (Depth)0.5f);
                                    Graphics.Draw(!mod.configuration.isWorkshop ? _localIcon : _steamIcon, x + 36f, y + 2.5f, (Depth)0.5f);
                                    if (mod.configuration.error != null && (mod.configuration.disabled || mod is ErrorMod))
                                    {
                                        string str3 = mod.configuration.error;
                                        if (str3.Length > 150)
                                            str3 = str3.Substring(0, 150);
                                        _fancyFont.Draw(mod.configuration.error.StartsWith("!") ? "|DGYELLOW|" + str3.Substring(1, str3.Length - 1) : "|DGRED|Failed with error: " + str3, new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.White, (Depth)0.5f);
                                    }
                                    else if (!mod.configuration.loaded)
                                    {
                                        if (mod.configuration.disabled)
                                            _fancyFont.Draw("Mod is disabled.", new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.LightGray, (Depth)0.5f);
                                        else
                                            _fancyFont.Draw("|DGGREEN|Mod will be enabled on next restart.", new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.Orange, (Depth)0.5f);
                                    }
                                    else if (mod.configuration.disabled)
                                        _fancyFont.Draw("|DGRED|Mod will be disabled on next restart.", new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.Orange, (Depth)0.5f);
                                    else
                                        _fancyFont.Draw(mod.configuration.description, new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.White, (Depth)0.5f);
                                }
                            }
                            else
                            {
                                Graphics.Draw(_newIcon, x + 2f, y + 1f, (Depth)0.5f);
                                _fancyFont.scale = new Vec2(1.5f);
                                _fancyFont.Draw("Get " + (_mods.Count == 1 ? "some" : "more") + " mods!", new Vec2(x + 36f, y + 11f), Color.White, (Depth)0.5f);
                                _fancyFont.scale = new Vec2(1f);
                            }
                        }
                        else
                            break;
                    }
                    if (_awaitingChanges)
                        Graphics.DrawString("Restart required for some changes to take effect!", new Vec2((float)(x - halfWidth + 128.0), (float)(y - halfHeight + 8.0)), Color.Red, (Depth)0.6f);
                    if (_transferItem != null)
                    {
                        Graphics.DrawRect(new Rectangle(_box.x - _box.halfWidth, _box.y - _box.halfHeight, _box.width, _box.height), Color.Black * 0.9f, (Depth)0.7f);
                        string text = "Creating item...";
                        if (_transferring)
                        {
                            TransferProgress uploadProgress = _transferItem.GetUploadProgress();
                            string str;
                            switch (uploadProgress.status)
                            {
                                case ItemUpdateStatus.PreparingConfig:
                                    str = "Preparing config";
                                    break;
                                case ItemUpdateStatus.PreparingContent:
                                    str = "Preparing content";
                                    break;
                                case ItemUpdateStatus.UploadingContent:
                                    str = "Uploading content";
                                    break;
                                case ItemUpdateStatus.UploadingPreviewFile:
                                    str = "Uploading preview";
                                    break;
                                case ItemUpdateStatus.CommittingChanges:
                                    str = "Committing changes";
                                    break;
                                default:
                                    str = "Waiting";
                                    break;
                            }
                            if (uploadProgress.bytesTotal != 0UL)
                            {
                                float amount = uploadProgress.bytesDownloaded / (float)uploadProgress.bytesTotal;
                                str = str + " (" + ((int)(amount * 100.0)).ToString() + "%)";
                                Graphics.DrawRect(new Rectangle((float)(_box.x - _box.halfWidth + 8.0), _box.y - 8f, _box.width - 16f, 16f), Color.LightGray, (Depth)0.8f);
                                Graphics.DrawRect(new Rectangle((float)(_box.x - _box.halfWidth + 8.0), _box.y - 8f, Lerp.FloatSmooth(0f, _box.width - 16f, amount), 16f), Color.Green, (Depth)0.8f);
                            }
                            text = str + "...";
                        }
                        else if (_needsUpdateNotes)
                        {
                            Graphics.DrawRect(new Rectangle(_updateTextBox.position.x - 1f, _updateTextBox.position.y - 1f, _updateTextBox.size.x + 2f, _updateTextBox.size.y + 2f), Color.Gray, (Depth)0.85f, false);
                            Graphics.DrawRect(new Rectangle(_updateTextBox.position.x, _updateTextBox.position.y, _updateTextBox.size.x, _updateTextBox.size.y), Color.Black, (Depth)0.85f);
                            _updateTextBox.Draw();
                            text = "Enter change notes:";
                            Graphics.DrawString(_updateButtonText, new Vec2(_updateButton.x, _updateButton.y), _updateButton.Contains(Mouse.position) ? Color.Yellow : Color.White, (Depth)0.9f, scale: 2f);
                        }
                        float stringWidth = Graphics.GetStringWidth(text, scale: 2f);
                        Graphics.DrawString(text, new Vec2(_box.x - stringWidth / 2f, (float)(_box.y - _box.halfHeight + 24.0)), Color.White, (Depth)0.8f, scale: 2f);
                    }
                }
            }
            base.Draw();
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        private sealed class UI_ModSettings : Mod
        {
        }
    }
}
