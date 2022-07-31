// Decompiled with JetBrains decompiler
// Type: DuckGame.UIModManagement
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            if (!this.fixView)
            {
                this._showingMenu = false;
                this._editModMenu.Close();
                Layer.HUD.camera.width /= 2f;
                Layer.HUD.camera.height /= 2f;
                this.fixView = true;
                DevConsole.RestoreDevConsole();
            }
            base.Close();
        }

        private void EnableDisableMod()
        {
            this._awaitingChanges = true;
            if (this._selectedMod.configuration.disabled)
                this._selectedMod.configuration.Enable();
            else
                this._selectedMod.configuration.Disable();
            this.modsChanged = true;
            this._editModMenu.Close();
            this.Open();
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref UIModManagement.SHFILEOPSTRUCT FileOp);

        private static void DeleteFileOrFolder(string path)
        {
            UIModManagement.SHFILEOPSTRUCT fileop = default(UIModManagement.SHFILEOPSTRUCT);
            fileop.wFunc = 3;
            fileop.pFrom = path + "\0\0";
            fileop.fFlags = 80;
            UIModManagement.SHFileOperation(ref fileop);
        }

        private void DeleteMod() => this.ShowYesNo(this._editModMenu, () =>
       {
           this._awaitingChanges = true;
           if (this._selectedMod.configuration.workshopID == 0UL)
               UIModManagement.DeleteFileOrFolder(this._selectedMod.configuration.directory);
           else
               Steam.WorkshopUnsubscribe(this._selectedMod.configuration.workshopID);
           this._mods.Remove(this._selectedMod);
           this._hoverIndex = -1;
           this._yesNoMenu.Close();
           this._editModMenu.Close();
           this.Open();
       });

        private void ShowYesNo(UIMenu goBackTo, UIMenuActionCallFunction.Function onYes)
        {
            this._yesNoNo.menuAction = new UIMenuActionCallFunction(() =>
          {
              this._yesNoMenu.Close();
              goBackTo.Open();
          });
            this._yesNoYes.menuAction = new UIMenuActionCallFunction(onYes);
            new UIMenuActionOpenMenu(_editModMenu, _yesNoMenu).Activate();
        }

        private void UploadMod()
        {
            this._editModMenu.Close();
            this.Open();
            if (this._selectedMod.configuration.workshopID == 0UL)
            {
                this._transferItem = Steam.CreateItem();
            }
            else
            {
                this._transferItem = new WorkshopItem(this._selectedMod.configuration.workshopID);
                this._needsUpdateNotes = true;
                this._updateTextBox.GainFocus();
                this._gamepadMode = false;
            }
            this._transferring = false;
        }

        private void VisitModPage()
        {
            this._editModMenu.Close();
            this.Open();
            Steam.OverlayOpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + this._selectedMod.configuration.workshopID.ToString());
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
                System.IO.File.SetAttributes(str, FileAttributes.Normal);
            }
            if (!copySubDirs)
                return;
            foreach (DirectoryInfo directoryInfo2 in directories)
            {
                string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
                UIModManagement.DirectoryCopy(directoryInfo2.FullName, destDirName1, copySubDirs);
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
            this._splitter.topSection.components[0].align = UIAlign.Left;
            this._openOnClose = openOnClose;
            this._moreArrow = new Sprite("moreArrow");
            this._moreArrow.CenterOrigin();
            this._steamIcon = new Sprite("steamIconSmall")
            {
                scale = new Vec2(1f) / 2f
            };
            this._localIcon = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(1f) / 2f
            };
            this._localIcon.SetFrameWithoutReset(1);
            this._modErrorIcon = new Sprite("modloadError");
            this._newIcon = new SpriteMap("presents", 16, 16)
            {
                scale = new Vec2(2f)
            };
            this._newIcon.SetFrameWithoutReset(0);
            this._settingsIcon = new SpriteMap("settingsWrench", 16, 16)
            {
                scale = new Vec2(2f)
            };
            this._noImage = new Sprite("notexture")
            {
                scale = new Vec2(2f)
            };
            this._cursor = new SpriteMap("cursors", 16, 16);
            this._mods = ModLoader.allMods.Where<Mod>(a => !(a is CoreMod)).ToList<Mod>();
            this._mods.Insert(0, new UIModManagement.UI_ModSettings());
            this._mods.Add(null);
            this._maxModsToShow = 8;
            this._box = new UIBox(0f, 0f, high: this._maxModsToShow * 36, isVisible: false);
            this.Add(_box, true);
            this._fancyFont = new FancyBitmapFont("smallFont")
            {
                maxWidth = (int)this.width - 60,
                maxRows = 2
            };
            this.scrollBarOffset = 0;
            this._editModMenu = new UIMenu("<mod name>", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 240f, conString: "@SELECT@SELECT");
            this._editModMenu.Add(this._disableOrEnableItem = new UIMenuItem("DISABLE", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.EnableDisableMod))), true);
            this._deleteOrUnsubItem = new UIMenuItem("DELETE", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.DeleteMod)));
            this._uploadItem = new UIMenuItem("UPLOAD", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.UploadMod)));
            this._visitItem = new UIMenuItem("VISIT PAGE", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(this.VisitModPage)));
            this._editModMenu.Add(new UIText(" ", Color.White), true);
            this._editModMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_editModMenu, this)), true);
            this._editModMenu.Close();
            this._yesNoMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            this._yesNoMenu.Add(this._yesNoYes = new UIMenuItem("YES"), true);
            this._yesNoMenu.Add(this._yesNoNo = new UIMenuItem("NO"), true);
            this._yesNoMenu.Close();
            this._updateTextBox = new Textbox(0f, 0f, 0f, 0f)
            {
                depth = (Depth)0.9f,
                maxLength = 5000
            };
            this._modSettingsMenu = new UIMenu("@WRENCH@MOD SETTINGS@SCREWDRIVER@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@WASD@ADJUST @CANCEL@EXIT");
            this._modSettingsMenu.Add(new UIText("If CRASH DISABLE is ON,", Colors.DGBlue), true);
            this._modSettingsMenu.Add(new UIText("a mod will automatically be", Colors.DGBlue), true);
            this._modSettingsMenu.Add(new UIText(" disabled if it causes", Colors.DGBlue), true);
            this._modSettingsMenu.Add(new UIText("the game to crash.", Colors.DGBlue), true);
            this._modSettingsMenu.Add(new UIText(" ", Colors.DGBlue), true);
            this._modSettingsMenu.Add(new UIMenuItemToggle("CRASH DISABLE", field: new FieldBinding(Options.Data, "disableModOnCrash")), true);
            this._modSettingsMenu.Add(new UIMenuItemToggle("LOAD FAILURE DISABLE", field: new FieldBinding(Options.Data, "disableModOnLoadFailure")), true);
            this._modSettingsMenu.Add(new UIMenuItemToggle("SHOW NETWORK WARNING", field: new FieldBinding(Options.Data, "showNetworkModWarning")), true);
            this._modSettingsMenu.Add(new UIText(" ", Colors.DGBlue), true);
            this._modSettingsMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_modSettingsMenu, this), backButton: true), true);
            this._modSettingsMenu.Close();
        }

        public override void Open()
        {
            if (this._uploadDialog == null)
            {
                this._uploadDialog = new SteamUploadDialog();
                Level.Add(_uploadDialog);
                Level.current.things.RefreshState();
            }
            this._pressWait = 30;
            base.Open();
            DevConsole.SuppressDevConsole();
            this._oldPos = Mouse.positionScreen;
        }

        public override void Update()
        {
            if (this._uploadDialog != null && this._uploadDialog.opened)
            {
                Editor.clickedMenu = false;
                Editor.inputMode = EditorInput.Mouse;
                Level.current.things.RefreshState();
                foreach (Thing thing in Level.current.things[typeof(ContextMenu)])
                    thing.Update();
            }
            else
            {
                if (this._pressWait > 0)
                    --this._pressWait;
                if (this.showingError != null)
                {
                    this._controlString = "@CANCEL@BACK";
                    if (Input.Pressed("QUACK"))
                        this.showingError = null;
                    base.Update();
                }
                else
                {
                    if (this._editModMenu.open)
                    {
                        if (!UIMenu.globalUILock && (Input.Pressed("CANCEL") || Keyboard.Pressed(Keys.Escape)))
                        {
                            this._editModMenu.Close();
                            this.Open();
                            return;
                        }
                    }
                    else if (this._modSettingsMenu.open)
                    {
                        if (!UIMenu.globalUILock && (Input.Pressed("CANCEL") || Keyboard.Pressed(Keys.Escape)))
                        {
                            this._modSettingsMenu.Close();
                            this.Open();
                            return;
                        }
                    }
                    else if (this.open)
                    {
                        if (this._transferItem != null && !this._needsUpdateNotes)
                        {
                            if (!this._transferring)
                            {
                                if (this._transferItem.result == SteamResult.OK)
                                {
                                    WorkshopItemData dat = new WorkshopItemData();
                                    if (this._selectedMod.configuration.workshopID == 0UL)
                                    {
                                        this._selectedMod.configuration.SetWorkshopID(this._transferItem.id);
                                        dat.name = this._selectedMod.configuration.displayName;
                                        dat.description = this._selectedMod.configuration.description;
                                        dat.visibility = RemoteStoragePublishedFileVisibility.Private;
                                        dat.tags = new List<string>();
                                        dat.tags.Add("Mod");
                                        if (this._selectedMod.configuration.modType == ModConfiguration.Type.MapPack)
                                            dat.tags.Add("Map Pack");
                                        else if (this._selectedMod.configuration.modType == ModConfiguration.Type.HatPack)
                                            dat.tags.Add("Hat Pack");
                                        else if (this._selectedMod.configuration.modType == ModConfiguration.Type.Reskin)
                                            dat.tags.Add("Texture Pack");
                                    }
                                    else
                                        dat.changeNotes = this._updateTextBox.text;
                                    string pathToScreenshot = this._selectedMod.generateAndGetPathToScreenshot;
                                    dat.previewPath = pathToScreenshot;
                                    string str = DuckFile.workshopDirectory + this._transferItem.id.ToString() + "/content";
                                    if (Directory.Exists(str))
                                        Directory.Delete(str, true);
                                    DuckFile.CreatePath(str);
                                    UIModManagement.DirectoryCopy(this._selectedMod.configuration.directory, str + "/" + this._selectedMod.configuration.name, true);
                                    if (Directory.Exists(str + this._selectedMod.configuration.name + "/build"))
                                        Directory.Delete(str + this._selectedMod.configuration.name + "/build", true);
                                    if (Directory.Exists(str + this._selectedMod.configuration.name + "/.vs"))
                                        Directory.Delete(str + this._selectedMod.configuration.name + "/.vs", true);
                                    if (System.IO.File.Exists(str + this._selectedMod.configuration.name + "/" + this._selectedMod.configuration.name + "_compiled.dll"))
                                    {
                                        string path = str + this._selectedMod.configuration.name + "/" + this._selectedMod.configuration.name + "_compiled.dll";
                                        System.IO.File.SetAttributes(path, FileAttributes.Normal);
                                        System.IO.File.Delete(path);
                                    }
                                    if (System.IO.File.Exists(str + this._selectedMod.configuration.name + "/" + this._selectedMod.configuration.name + "_compiled.hash"))
                                    {
                                        string path = str + this._selectedMod.configuration.name + "/" + this._selectedMod.configuration.name + "_compiled.hash";
                                        System.IO.File.SetAttributes(path, FileAttributes.Normal);
                                        System.IO.File.Delete(path);
                                    }
                                    dat.contentFolder = str;
                                    this._transferItem.ApplyWorkshopData(dat);
                                    if (this._transferItem.needsLegal)
                                        Steam.ShowWorkshopLegalAgreement(this._transferItem.id.ToString());
                                    this._transferring = true;
                                    this._transferItem.ResetProcessing();
                                }
                            }
                            else if (this._transferItem.finishedProcessing)
                            {
                                Steam.OverlayOpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + this._transferItem.id.ToString());
                                Directory.Delete(DuckFile.workshopDirectory + this._transferItem.id.ToString() + "/", true);
                                this._transferItem.ResetProcessing();
                                this._transferItem = null;
                                this._transferring = false;
                            }
                            base.Update();
                            return;
                        }
                        if (this._gamepadMode)
                        {
                            if (this._hoverIndex < 0)
                                this._hoverIndex = 0;
                        }
                        else
                        {
                            this._hoverIndex = -1;
                            for (int index = 0; index < this._maxModsToShow && this._scrollItemOffset + index < this._mods.Count; ++index)
                            {
                                if (new Rectangle((int)(this._box.x - this._box.halfWidth), (int)(this._box.y - this._box.halfHeight + 36 * index), (int)this._box.width - 14, 36f).Contains(Mouse.position))
                                {
                                    this._hoverIndex = this._scrollItemOffset + index;
                                    break;
                                }
                            }
                        }
                        if (this._transferItem != null)
                        {
                            if (this._updateTextBox != null)
                            {
                                Editor.hoverTextBox = false;
                                this._updateTextBox.position = new Vec2((float)(this._box.x - this._box.halfWidth + 16.0), (float)(this._box.y - this._box.halfHeight + 48.0));
                                this._updateTextBox.size = new Vec2(this._box.width - 32f, this._box.height - 80f);
                                this._updateTextBox._maxLines = (int)(_updateTextBox.size.y / this._fancyFont.characterHeight);
                                this._updateTextBox.Update();
                                float stringWidth = Graphics.GetStringWidth(this._updateButtonText, scale: 2f);
                                float height = Graphics.GetStringHeight(this._updateButtonText) * 2f;
                                this._updateButton = new Rectangle(this._box.x - stringWidth / 2f, (float)(this._box.y + this._box.halfHeight - 24.0), stringWidth, height);
                                if (this._updateButton.Contains(Mouse.position) && Mouse.left == InputState.Pressed)
                                {
                                    this._needsUpdateNotes = false;
                                    this._updateTextBox.LoseFocus();
                                }
                                else if (Keyboard.Pressed(Keys.Escape))
                                {
                                    this._needsUpdateNotes = false;
                                    this._transferItem = null;
                                    this._updateTextBox.LoseFocus();
                                    new UIMenuActionOpenMenu(this, _editModMenu).Activate();
                                    return;
                                }
                            }
                        }
                        else if (this._hoverIndex != -1)
                        {
                            this._selectedMod = this._mods[this._hoverIndex];
                            if (this._selectedMod is UIModManagement.UI_ModSettings)
                                this._controlString = "@WASD@@SELECT@SETTINGS @CANCEL@BACK";
                            else if (this._selectedMod != null && this._selectedMod.configuration.error != null)
                            {
                                if (this._selectedMod.configuration.forceHarmonyLegacyLoad)
                                    this._controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @MENU2@DISABLE FORCED LOAD @START@SHOW ERROR";
                                else
                                    this._controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @MENU2@FORCE LEGACY LOAD @START@SHOW ERROR";
                            }
                            else
                                this._controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @CANCEL@BACK";
                            if (Input.Pressed("MENU1"))
                            {
                                if (this._selectedMod != null && this._selectedMod.configuration != null)
                                {
                                    if (this._selectedMod.configuration.disabled)
                                        this._selectedMod.configuration.Enable();
                                    else
                                        this._selectedMod.configuration.Disable();
                                    this._selectedMod.configuration.error = null;
                                    this.modsChanged = true;
                                    SFX.Play("rockHitGround", 0.8f);
                                }
                            }
                            else if (this._selectedMod != null && this._selectedMod.configuration != null && this._selectedMod.configuration.error != null && Input.Pressed("MENU2"))
                            {
                                if (this._selectedMod.configuration != null)
                                {
                                    this._selectedMod.configuration.forceHarmonyLegacyLoad = !this._selectedMod.configuration.forceHarmonyLegacyLoad;
                                    ModLoader.DisabledModsChanged();
                                    this.modsChanged = true;
                                    SFX.Play("rockHitGround", 0.8f);
                                }
                            }
                            else
                            {
                                if (Input.Pressed("START") && this._selectedMod != null && this._selectedMod.configuration != null && this._selectedMod.configuration.error != null)
                                {
                                    string str = DuckFile.saveDirectory + "error_info.txt";
                                    System.IO.File.WriteAllText(str, this._selectedMod.configuration.error);
                                    Process.Start(str);
                                    SFX.Play("rockHitGround", 0.8f);
                                    return;
                                }
                                if (Input.Pressed("SELECT") && this._pressWait == 0 && this._gamepadMode || !this._gamepadMode && Mouse.left == InputState.Pressed)
                                {
                                    if (this._selectedMod != null)
                                    {
                                        if (this._selectedMod is UIModManagement.UI_ModSettings)
                                        {
                                            SFX.Play("rockHitGround", 0.8f);
                                            this._modSettingsMenu.dirty = true;
                                            new UIMenuActionOpenMenu(this, _modSettingsMenu).Activate();
                                            return;
                                        }
                                        this._editModMenu.title = this._selectedMod.configuration.loaded ? "|YELLOW|" + this._selectedMod.configuration.displayName : "|YELLOW|" + this._selectedMod.configuration.name;
                                        this._editModMenu.Remove(_deleteOrUnsubItem);
                                        this._editModMenu.Remove(_uploadItem);
                                        this._editModMenu.Remove(_visitItem);
                                        if (!this._selectedMod.configuration.isWorkshop && this._selectedMod.configuration.loaded)
                                        {
                                            this._uploadItem.text = this._selectedMod.configuration.workshopID == 0UL ? "UPLOAD" : "UPDATE";
                                            this._editModMenu.Insert(_uploadItem, 1, true);
                                        }
                                        if (!this._selectedMod.configuration.isWorkshop && !this._selectedMod.configuration.loaded)
                                        {
                                            this._deleteOrUnsubItem.text = "DELETE";
                                            this._editModMenu.Insert(_deleteOrUnsubItem, 1, true);
                                        }
                                        else if (this._selectedMod.configuration.isWorkshop)
                                        {
                                            this._deleteOrUnsubItem.text = "UNSUBSCRIBE";
                                            this._editModMenu.Insert(_deleteOrUnsubItem, 1, true);
                                        }
                                        if (this._selectedMod.configuration.isWorkshop)
                                            this._editModMenu.Insert(_visitItem, 1, true);
                                        this._disableOrEnableItem.text = this._selectedMod.configuration.disabled ? "ENABLE" : "DISABLE";
                                        this._editModMenu.dirty = true;
                                        SFX.Play("rockHitGround", 0.8f);
                                        new UIMenuActionOpenMenu(this, _editModMenu).Activate();
                                        return;
                                    }
                                    Steam.OverlayOpenURL("http://steamcommunity.com/workshop/browse/?appid=312530&searchtext=&childpublishedfileid=0&browsesort=trend&section=readytouseitems&requiredtags%5B%5D=Mod");
                                }
                            }
                        }
                        else
                            this._selectedMod = null;
                        if (this._gamepadMode)
                        {
                            this._draggingScrollbar = false;
                            if (Input.Pressed("MENUDOWN"))
                                ++this._hoverIndex;
                            else if (Input.Pressed("MENUUP"))
                                --this._hoverIndex;
                            if (Input.Pressed("STRAFE"))
                                this._hoverIndex -= 10;
                            else if (Input.Pressed("RAGDOLL"))
                                this._hoverIndex += 10;
                            if (this._hoverIndex < 0)
                                this._hoverIndex = 0;
                            if ((this._oldPos - Mouse.positionScreen).lengthSq > 200.0)
                                this._gamepadMode = false;
                        }
                        else
                        {
                            if (!this._draggingScrollbar)
                            {
                                if (Mouse.left == InputState.Pressed && this.ScrollBarBox().Contains(Mouse.position))
                                {
                                    this._draggingScrollbar = true;
                                    this._oldPos = Mouse.position;
                                }
                                if (Mouse.scroll > 0.0)
                                {
                                    this._scrollItemOffset += 5;
                                    this._hoverIndex += 5;
                                }
                                else if (Mouse.scroll < 0.0)
                                {
                                    this._scrollItemOffset -= 5;
                                    this._hoverIndex -= 5;
                                    if (this._hoverIndex < 0)
                                        this._hoverIndex = 0;
                                }
                            }
                            else if (Mouse.left != InputState.Down)
                            {
                                this._draggingScrollbar = false;
                            }
                            else
                            {
                                Vec2 vec2 = Mouse.position - this._oldPos;
                                this._oldPos = Mouse.position;
                                this.scrollBarOffset += (int)vec2.y;
                                if (this.scrollBarOffset > this.scrollBarScrollableHeight)
                                    this.scrollBarOffset = this.scrollBarScrollableHeight;
                                else if (this.scrollBarOffset < 0)
                                    this.scrollBarOffset = 0;
                                this._scrollItemOffset = (int)((this._mods.Count - this._maxModsToShow) * (scrollBarOffset / (float)this.scrollBarScrollableHeight));
                            }
                            if (Input.Pressed("ANY"))
                            {
                                this._gamepadMode = true;
                                this._oldPos = Mouse.positionScreen;
                            }
                        }
                        if (this._scrollItemOffset < 0)
                            this._scrollItemOffset = 0;
                        else if (this._scrollItemOffset > Math.Max(0, this._mods.Count - this._maxModsToShow))
                            this._scrollItemOffset = Math.Max(0, this._mods.Count - this._maxModsToShow);
                        if (this._hoverIndex >= this._mods.Count)
                            this._hoverIndex = this._mods.Count - 1;
                        else if (this._hoverIndex >= this._scrollItemOffset + this._maxModsToShow)
                            this._scrollItemOffset += this._hoverIndex - (this._scrollItemOffset + this._maxModsToShow) + 1;
                        else if (this._hoverIndex >= 0 && this._hoverIndex < this._scrollItemOffset)
                            this._scrollItemOffset -= this._scrollItemOffset - this._hoverIndex;
                        this.scrollBarOffset = this._scrollItemOffset == 0 ? 0 : (int)Lerp.FloatSmooth(0f, scrollBarScrollableHeight, _scrollItemOffset / (float)(this._mods.Count - this._maxModsToShow));
                        if (!Editor.hoverTextBox && !UIMenu.globalUILock && (Input.Pressed("CANCEL") || Keyboard.Pressed(Keys.Escape)))
                        {
                            if (this.modsChanged)
                            {
                                this.Close();
                                MonoMain.pauseMenu = DuckNetwork.OpenModsRestartWindow(this._openOnClose);
                            }
                            else
                                new UIMenuActionOpenMenu(this, _openOnClose).Activate();
                            this.modsChanged = false;
                            return;
                        }
                    }
                    if (this._showingMenu)
                    {
                        HUD.CloseAllCorners();
                        this._showingMenu = false;
                    }
                    base.Update();
                }
            }
        }

        private Rectangle ScrollBarBox() => new Rectangle((float)(this._box.x + this._box.halfWidth - 12.0 + 1.0), (float)(this._box.y - this._box.halfHeight + 1.0) + scrollBarOffset, 10f, 32f);

        public override void Draw()
        {
            if (this.open)
            {
                if (Mouse.available && !this._gamepadMode)
                {
                    this._cursor.depth = (Depth)1f;
                    this._cursor.scale = new Vec2(1f, 1f);
                    this._cursor.position = Mouse.position;
                    this._cursor.frame = 0;
                    if (Editor.hoverTextBox)
                    {
                        this._cursor.frame = 7;
                        this._cursor.position.y -= 4f;
                        this._cursor.scale = new Vec2(0.5f, 1f);
                    }
                    this._cursor.Draw();
                }
                if (this._uploadDialog != null && this._uploadDialog.opened)
                {
                    Editor.hoverTextBox = false;
                    this._gamepadMode = false;
                    using (IEnumerator<Thing> enumerator = Level.current.things[typeof(ContextMenu)].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            enumerator.Current.Draw();
                        return;
                    }
                }
                else
                {
                    if (this.showingError != null)
                    {
                        float num1 = this._box.x - this._box.halfWidth;
                        float num2 = this._box.y - this._box.halfHeight;
                        this._fancyFont.scale = new Vec2(1f);
                        int maxWidth = this._fancyFont.maxWidth;
                        this._fancyFont.maxRows = 40;
                        this._fancyFont.maxWidth = (int)this.width - 10;
                        this._fancyFont.Draw(this.showingError, new Vec2(num1 + 4f, num2 + 4f), Color.White, (Depth)0.5f);
                        this._fancyFont.maxRows = 2;
                        this._fancyFont.maxWidth = maxWidth;
                        base.Draw();
                        return;
                    }
                    this.scrollBarTop = (int)(this._box.y - this._box.halfHeight + 1.0 + 16.0);
                    this.scrollBarBottom = (int)(this._box.y + this._box.halfHeight - 1.0 - 16.0);
                    this.scrollBarScrollableHeight = this.scrollBarBottom - this.scrollBarTop;
                    if (this.fixView)
                    {
                        Layer.HUD.camera.width *= 2f;
                        Layer.HUD.camera.height *= 2f;
                        this.fixView = false;
                    }
                    Graphics.DrawRect(new Vec2(this._box.x - this._box.halfWidth, this._box.y - this._box.halfHeight), new Vec2((float)(this._box.x + this._box.halfWidth - 12.0 - 2.0), this._box.y + this._box.halfHeight), Color.Black, (Depth)0.4f);
                    Graphics.DrawRect(new Vec2((float)(this._box.x + this._box.halfWidth - 12.0), this._box.y - this._box.halfHeight), new Vec2(this._box.x + this._box.halfWidth, this._box.y + this._box.halfHeight), Color.Black, (Depth)0.4f);
                    Rectangle r = this.ScrollBarBox();
                    Graphics.DrawRect(r, this._draggingScrollbar || r.Contains(Mouse.position) ? Color.LightGray : Color.Gray, (Depth)0.5f);
                    for (int index1 = 0; index1 < this._maxModsToShow; ++index1)
                    {
                        int index2 = this._scrollItemOffset + index1;
                        if (index2 < this._mods.Count)
                        {
                            float x = this._box.x - this._box.halfWidth;
                            float y = this._box.y - this._box.halfHeight + 36 * index1;
                            if (this._transferItem == null && this._hoverIndex == index2)
                                Graphics.DrawRect(new Vec2(x, y), new Vec2((float)(x + this._box.width - 14.0), y + 36f), Color.White * 0.6f, (Depth)0.45f);
                            else if ((index2 & 1) != 0)
                                Graphics.DrawRect(new Vec2(x, y), new Vec2((float)(x + this._box.width - 14.0), y + 36f), Color.White * 0.1f, (Depth)0.45f);
                            Mod mod = this._mods[index2];
                            if (mod != null)
                            {
                                if (mod is UIModManagement.UI_ModSettings)
                                {
                                    Graphics.Draw(_settingsIcon, x + 2f, y + 1f, (Depth)0.5f);
                                    this._fancyFont.scale = new Vec2(1.5f);
                                    this._fancyFont.Draw("Mod Settings", new Vec2(x + 36f, y + 11f), Color.White, (Depth)0.5f);
                                    this._fancyFont.scale = new Vec2(1f);
                                }
                                else
                                {
                                    Tex2D previewTexture = mod.previewTexture;
                                    if (previewTexture != null && this._noImage.texture != previewTexture)
                                    {
                                        this._noImage.texture = previewTexture;
                                        this._noImage.scale = new Vec2(32f / previewTexture.width);
                                    }
                                    Graphics.DrawRect(new Vec2(x + 2f, y + 2f), new Vec2((float)(x + 36.0 - 2.0), (float)(y + 36.0 - 2.0)), Color.Gray, (Depth)0.44f, false, 2f);
                                    Graphics.Draw(this._noImage, x + 2f, y + 2f, (Depth)0.5f);
                                    string str1 = "#" + index2.ToString() + ": ";
                                    if (mod.configuration.error != null)
                                    {
                                        this._modErrorIcon.scale = new Vec2(2f);
                                        Graphics.Draw(this._modErrorIcon, x + 2f, y + 2f, (Depth)0.55f);
                                        str1 += "|DGRED|";
                                    }
                                    if (mod.configuration.error != null || mod.configuration.disabled)
                                        Graphics.DrawRect(new Vec2(x, y), new Vec2((float)(x + this._box.width - 14.0), y + 36f), Color.Black * 0.4f, (Depth)0.85f);
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
                                    this._fancyFont.Draw(str2 + (mod.configuration.disabled ? "|DGRED| (Disabled)" : "|DGGREEN| (Enabled)"), new Vec2((float)(x + 36.0 + 10.0), y + 2f), Color.Yellow, (Depth)0.5f);
                                    Graphics.Draw(!mod.configuration.isWorkshop ? _localIcon : this._steamIcon, x + 36f, y + 2.5f, (Depth)0.5f);
                                    if (mod.configuration.error != null && (mod.configuration.disabled || mod is ErrorMod))
                                    {
                                        string str3 = mod.configuration.error;
                                        if (str3.Length > 150)
                                            str3 = str3.Substring(0, 150);
                                        this._fancyFont.Draw(mod.configuration.error.StartsWith("!") ? "|DGYELLOW|" + str3.Substring(1, str3.Length - 1) : "|DGRED|Failed with error: " + str3, new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.White, (Depth)0.5f);
                                    }
                                    else if (!mod.configuration.loaded)
                                    {
                                        if (mod.configuration.disabled)
                                            this._fancyFont.Draw("Mod is disabled.", new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.LightGray, (Depth)0.5f);
                                        else
                                            this._fancyFont.Draw("|DGGREEN|Mod will be enabled on next restart.", new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.Orange, (Depth)0.5f);
                                    }
                                    else if (mod.configuration.disabled)
                                        this._fancyFont.Draw("|DGRED|Mod will be disabled on next restart.", new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.Orange, (Depth)0.5f);
                                    else
                                        this._fancyFont.Draw(mod.configuration.description, new Vec2(x + 36f, y + 6f + _fancyFont.characterHeight), Color.White, (Depth)0.5f);
                                }
                            }
                            else
                            {
                                Graphics.Draw(_newIcon, x + 2f, y + 1f, (Depth)0.5f);
                                this._fancyFont.scale = new Vec2(1.5f);
                                this._fancyFont.Draw("Get " + (this._mods.Count == 1 ? "some" : "more") + " mods!", new Vec2(x + 36f, y + 11f), Color.White, (Depth)0.5f);
                                this._fancyFont.scale = new Vec2(1f);
                            }
                        }
                        else
                            break;
                    }
                    if (this._awaitingChanges)
                        Graphics.DrawString("Restart required for some changes to take effect!", new Vec2((float)(this.x - this.halfWidth + 128.0), (float)(this.y - this.halfHeight + 8.0)), Color.Red, (Depth)0.6f);
                    if (this._transferItem != null)
                    {
                        Graphics.DrawRect(new Rectangle(this._box.x - this._box.halfWidth, this._box.y - this._box.halfHeight, this._box.width, this._box.height), Color.Black * 0.9f, (Depth)0.7f);
                        string text = "Creating item...";
                        if (this._transferring)
                        {
                            TransferProgress uploadProgress = this._transferItem.GetUploadProgress();
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
                                Graphics.DrawRect(new Rectangle((float)(this._box.x - this._box.halfWidth + 8.0), this._box.y - 8f, this._box.width - 16f, 16f), Color.LightGray, (Depth)0.8f);
                                Graphics.DrawRect(new Rectangle((float)(this._box.x - this._box.halfWidth + 8.0), this._box.y - 8f, Lerp.FloatSmooth(0f, this._box.width - 16f, amount), 16f), Color.Green, (Depth)0.8f);
                            }
                            text = str + "...";
                        }
                        else if (this._needsUpdateNotes)
                        {
                            Graphics.DrawRect(new Rectangle(this._updateTextBox.position.x - 1f, this._updateTextBox.position.y - 1f, this._updateTextBox.size.x + 2f, this._updateTextBox.size.y + 2f), Color.Gray, (Depth)0.85f, false);
                            Graphics.DrawRect(new Rectangle(this._updateTextBox.position.x, this._updateTextBox.position.y, this._updateTextBox.size.x, this._updateTextBox.size.y), Color.Black, (Depth)0.85f);
                            this._updateTextBox.Draw();
                            text = "Enter change notes:";
                            Graphics.DrawString(this._updateButtonText, new Vec2(this._updateButton.x, this._updateButton.y), this._updateButton.Contains(Mouse.position) ? Color.Yellow : Color.White, (Depth)0.9f, scale: 2f);
                        }
                        float stringWidth = Graphics.GetStringWidth(text, scale: 2f);
                        Graphics.DrawString(text, new Vec2(this._box.x - stringWidth / 2f, (float)(this._box.y - this._box.halfHeight + 24.0)), Color.White, (Depth)0.8f, scale: 2f);
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
