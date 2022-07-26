// Decompiled with JetBrains decompiler
// Type: DuckGame.MonoFileDialog
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;

namespace DuckGame
{
    public class MonoFileDialog : ContextMenu
    {
        private bool _save;
        private string _currentDirectory;
        private string _rootFolder;
        private bool _scrollBar;
        private float _scrollPosition;
        private float _scrollLerp;
        private int _maxItems = 15;
        private float _scrollWait;
        private bool _doDeleteDialog;
        private bool _doOverwriteDialog;
        private ContextFileType _type;
        private Sprite _badTileset;
        private Sprite _badParallax;
        private Sprite _badArcade;
        private FancyBitmapFont _fancyFont;
        private TextEntryDialog _dialog;
        private MessageDialogue _deleteDialog;
        private MessageDialogue _overwriteDialog;
        private string _overwriteName = "";
        private bool _selectLevels;
        private bool _loadLevel;
        public string result;
        private float hOffset = -86f;
        private float _percentStorageUsed;
        private float _fdHeight = 262f;
        private string modRootPath;
        private string prevDirectory;
        private ZipArchive _openedArchive;
        private int _framesSinceSelected = 999;
        private ContextMenu _lastItemSelected;
        private string _prevPreviewPath = "";
        private string _previewName = "";
        private LevelMetaData.PreviewPair _previewPair;
        private Tex2D _preview;
        private Sprite _previewSprite;
        private byte[] _currentPreviewZipData;
        public bool drag;

        public string rootFolder => this._rootFolder;

        public MonoFileDialog()
          : base((IContextListener)null)
        {
        }

        public override void Initialize()
        {
            this.layer = Layer.HUD;
            this.depth = (Depth)0.9f;
            this._showBackground = false;
            this._fancyFont = new FancyBitmapFont("smallFont");
            this.itemSize = new Vec2(390f, 16f);
            this._root = true;
            this._dialog = new TextEntryDialog();
            this._dialog.filename = true;
            Level.Add((Thing)this._dialog);
            this._deleteDialog = new MessageDialogue();
            Level.Add((Thing)this._deleteDialog);
            this._overwriteDialog = new MessageDialogue();
            Level.Add((Thing)this._overwriteDialog);
            this.drawControls = false;
        }

        public void Open(
          string rootFolder,
          string currentFolder,
          bool save,
          bool selectLevels = false,
          bool loadLevel = true,
          ContextFileType type = ContextFileType.Level)
        {
            this._type = type;
            this._selectLevels = selectLevels;
            this._loadLevel = loadLevel;
            if (this._type == ContextFileType.Block || this._type == ContextFileType.Background || this._type == ContextFileType.Platform)
                this._badTileset = new Sprite("badTileset");
            if (this._type == ContextFileType.Parallax)
                this._badParallax = new Sprite("badParallax");
            if (this._type == ContextFileType.ArcadeStyle)
                this._badArcade = new Sprite("badArcade");
            this._preview = (Tex2D)null;
            this._previewSprite = (Sprite)null;
            float num1 = 350f;
            float num2 = 350f;
            Vec2 vec2_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0) + this.hOffset, (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0) + this.hOffset, (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0));
            this.position = vec2_1 + new Vec2(4f, 40f);
            this._save = save;
            rootFolder = rootFolder.Replace('\\', '/');
            currentFolder = currentFolder.Replace('\\', '/');
            this._currentDirectory = !(currentFolder == "") ? currentFolder : rootFolder;
            this._rootFolder = rootFolder;
            if (this.prevDirectory != null)
                this._currentDirectory = this.prevDirectory;
            this.SetDirectory(this._currentDirectory);
            Editor.lockInput = (ContextMenu)this;
            this.ComputeAvailableStorageSpace();
            SFX.Play("openClick", 0.4f);
            this.opened = true;
        }

        private void ComputeAvailableStorageSpace()
        {
            float percent = 0.0f;
            if (!DuckFile.GetLevelSpacePercentUsed(ref percent))
                return;
            this._percentStorageUsed = (double)percent > 100.0 ? 100f : percent;
        }

        public void Close()
        {
            Editor.lockInput = (ContextMenu)null;
            this.opened = false;
            this.ClearItems();
        }

        public string TypeExtension()
        {
            if (this._type == ContextFileType.Level)
                return ".lev";
            return this._type == ContextFileType.Block || this._type == ContextFileType.Background || this._type == ContextFileType.Platform || this._type == ContextFileType.Parallax || this._type == ContextFileType.ArcadeAnimation || this._type == ContextFileType.ArcadeStyle ? ".png" : "";
        }

        public void SetDirectory(string dir) => this.SetDirectory(dir, false);

        private string FixPath(string pPath)
        {
            string str = Path.GetFullPath(pPath).Replace('\\', '/');
            while (str.StartsWith("/"))
                str = str.Substring(1);
            if (str.EndsWith("/"))
                str = str.Substring(0, str.Length - 1);
            return str;
        }

        public void SetDirectory(string dir, bool pIsModPath)
        {
            if (this._openedArchive != null)
            {
                this._openedArchive.Dispose();
                this._openedArchive = (ZipArchive)null;
            }
            DevConsole.Log("MonoFileDialog.SetDirectory(" + dir + ")");
            if (!pIsModPath && dir.Contains("Mods") && (dir.Contains(DuckFile.globalModsDirectory) || dir.Contains(DuckFile.modsDirectory)))
                pIsModPath = true;
            this.isModPath = pIsModPath;
            dir = this.FixPath(dir);
            this._drawIndex = 0;
            DevConsole.Log("MonoFileDialog.SetDirectory(postfix)(" + dir + ")");
            if (this.modRootPath != null && this.modRootPath == dir)
            {
                dir = this._rootFolder;
                this.modRootPath = (string)null;
                this.isModPath = false;
            }
            if (dir.Length < this._rootFolder.Length && !pIsModPath)
                dir = this._rootFolder;
            int num1 = 0;
            this._currentDirectory = dir;
            if (this._currentDirectory != this._rootFolder)
                ++num1;
            if (this._save)
                ++num1;
            this.prevDirectory = dir;
            string[] directories = DuckFile.GetDirectories(this._currentDirectory);
            string[] files = DuckFile.GetFiles(this._currentDirectory);
            int num2 = num1 + (directories.Length + files.Length);
            Array.Sort<string>(directories);
            Array.Sort<string>(files);
            float x = 338f;
            this._scrollBar = false;
            this._scrollPosition = 0.0f;
            if (num2 > this._maxItems)
            {
                x = 326f;
                this._scrollBar = true;
            }
            if (this._save)
            {
                ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                contextMenu.layer = this.layer;
                contextMenu.text = "@NEWICONTINY@New File...";
                contextMenu.data = "New File...";
                contextMenu.itemSize = new Vec2(x, 16f);
                this.AddItem(contextMenu);
            }
            if (this._currentDirectory != this._rootFolder)
            {
                ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                contextMenu.layer = this.layer;
                bool flag = false;
                contextMenu.text = "@LOADICON@../";
                contextMenu.data = "../";
                contextMenu.itemSize = new Vec2(x, 16f);
                contextMenu.isModPath = flag;
                this.AddItem(contextMenu);
            }
            else
            {
                foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                {
                    if (accessibleMod.configuration != null && accessibleMod.configuration.content != null && accessibleMod.configuration.content.levels.Count > 0 && accessibleMod.configuration.contentDirectory.Contains(DuckFile.saveDirectory))
                    {
                        ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                        contextMenu.layer = this.layer;
                        contextMenu.fancy = true;
                        contextMenu.text = "@RAINBOWTINY@|DGBLUE|" + accessibleMod.configuration.name;
                        contextMenu.data = accessibleMod.configuration.contentDirectory + "/Levels";
                        contextMenu.itemSize = new Vec2(x, 16f);
                        contextMenu.mod = accessibleMod;
                        contextMenu.isModPath = true;
                        contextMenu.isModRoot = true;
                        this.AddItem(contextMenu);
                    }
                }
                foreach (MapPack mapPack in MapPack.active)
                {
                    if (mapPack.mod != null && mapPack.mod.configuration.directory.Contains(DuckFile.saveDirectory))
                    {
                        ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                        contextMenu.layer = this.layer;
                        contextMenu.fancy = true;
                        contextMenu.text = "|DGBLUE|" + mapPack.mod.configuration.name;
                        contextMenu.data = mapPack.mod.configuration.directory;
                        contextMenu.itemSize = new Vec2(x, 16f);
                        contextMenu.mod = mapPack.mod;
                        contextMenu.isModPath = true;
                        contextMenu.isModRoot = true;
                        contextMenu.customIcon = mapPack.mod.configuration.mapPack.icon;
                        this.AddItem(contextMenu);
                    }
                }
            }
            foreach (string path in directories)
            {
                string fileName = Path.GetFileName(path);
                ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                contextMenu.layer = this.layer;
                contextMenu.fancy = true;
                contextMenu.text = "@LOADICON@" + fileName;
                contextMenu.data = !pIsModPath ? fileName : path;
                contextMenu.isModPath = pIsModPath;
                contextMenu.itemSize = new Vec2(x, 16f);
                this.AddItem(contextMenu);
            }
            int num3 = 0;
            foreach (string path3 in files)
            {
                string path = path3;
                if (path.StartsWith("|"))
                    path = path.Substring(1, path.Length - 1);
                string fileName = Path.GetFileName(path);
                if (!this._selectLevels)
                {
                    if (fileName.EndsWith(this.TypeExtension()))
                    {
                        ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                        contextMenu.layer = this.layer;
                        contextMenu.fancy = true;
                        contextMenu.text = fileName;
                        contextMenu.text = fileName.Substring(0, fileName.Length - 4);
                        contextMenu.data = !pIsModPath ? fileName : path;
                        contextMenu.itemSize = new Vec2(x, 16f);
                        contextMenu.isModPath = pIsModPath;
                        this.AddItem(contextMenu);
                    }
                }
                else
                {
                    string str1 = path.Replace('\\', '/');
                    string str2 = str1.Substring(0, str1.Length - 4);
                    string str3 = str2.Substring(str2.IndexOf("/levels/", StringComparison.InvariantCultureIgnoreCase) + 8);
                    ContextCheckBox contextCheckBox = new ContextCheckBox(fileName, (IContextListener)this);
                    contextCheckBox.layer = this.layer;
                    contextCheckBox.fancy = true;
                    contextCheckBox.path = str3;
                    contextCheckBox.isChecked = Editor.activatedLevels.Contains(str3);
                    contextCheckBox.itemSize = new Vec2(x, 16f);
                    this.AddItem((ContextMenu)contextCheckBox);
                }
                ++num3;
            }
            int num4 = (int)Math.Round((double)(this._items.Count - 1 - this._maxItems) * (double)this._scrollPosition);
            int num5 = 0;
            for (int index = 0; index < this._items.Count; ++index)
            {
                if (index < num4 || index > num4 + this._maxItems)
                {
                    this._items[index].visible = false;
                }
                else
                {
                    this._items[index].visible = true;
                    this._items[index].position = new Vec2(this._items[index].position.x, (float)((double)this.y + 3.0 + (double)num5 * ((double)this._items[index].itemSize.y + 1.0)));
                    ++num5;
                }
            }
            this.menuSize.y = this._fdHeight;
        }

        private Tex2D GetArcadeSizeTex2D(string pTex, Tex2D pOriginalTex)
        {
            if (pOriginalTex.width == 48 && pOriginalTex.height == 48)
                return pOriginalTex;
            Image image = Image.FromFile(this._currentDirectory + "/" + Path.GetFileName(pTex));
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, 48, 48);
            Bitmap bitmap = new Bitmap(48, 48);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage((Image)bitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (ImageAttributes imageAttr = new ImageAttributes())
                {
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save((Stream)memoryStream, ImageFormat.Png);
            return (Tex2D)Texture2D.FromStream(DuckGame.Graphics.device, (Stream)memoryStream);
        }

        public override void Selected(ContextMenu item)
        {
            bool flag = false;
            if (this._framesSinceSelected < 20 && this._lastItemSelected == item)
                flag = true;
            this._lastItemSelected = item;
            this._framesSinceSelected = 0;
            if (!flag && Editor.inputMode == EditorInput.Touch)
                return;
            if ((double)this._percentStorageUsed >= 100.0 && (this._save || item.text == "@NEWICONTINY@New File..."))
            {
                SFX.Play("consoleError");
            }
            else
            {
                SFX.Play("highClick", 0.3f, 0.2f);
                if (item.text == "@NEWICONTINY@New File...")
                {
                    this._dialog.Open("Save File As...");
                    Editor.lockInput = (ContextMenu)this._dialog;
                }
                else if (item.data.EndsWith(this.TypeExtension()) && this._type != ContextFileType.All)
                {
                    if (!this._selectLevels)
                    {
                        if (!this._save)
                        {
                            this.Close();
                            string str = this._currentDirectory + "/" + item.data;
                            if (item.isModPath)
                                str = item.data;
                            if (this._loadLevel)
                                (Level.current as Editor).LoadLevel(str);
                            else
                                this.result = this._type != ContextFileType.ArcadeStyle ? str.Replace(this._rootFolder, "") : Editor.TextureToString((Texture2D)this.GetArcadeSizeTex2D(str, Content.Load<Tex2D>(str)));
                        }
                        else
                        {
                            this._overwriteDialog.Open("OVERWRITE " + item.data + "?");
                            Editor.lockInput = (ContextMenu)this._overwriteDialog;
                            this._doOverwriteDialog = true;
                            this._overwriteDialog.result = false;
                            this._overwriteName = item.data;
                            Editor.tookInput = true;
                        }
                    }
                    else if (item is ContextCheckBox contextCheckBox)
                    {
                        contextCheckBox.isChecked = !contextCheckBox.isChecked;
                        if (!contextCheckBox.isChecked)
                            Editor.activatedLevels.Remove(contextCheckBox.path);
                        else
                            Editor.activatedLevels.Add(contextCheckBox.path);
                    }
                }
                else
                {
                    this.ClearItems();
                    if (item.isModPath)
                    {
                        if (item.isModRoot)
                            this.modRootPath = this.FixPath(item.data + "/../");
                        this.SetDirectory(item.data, item.isModPath);
                    }
                    else
                        this.SetDirectory(this._currentDirectory + "/" + item.data);
                }
                this.ComputeAvailableStorageSpace();
            }
        }

        public override void Toggle(ContextMenu item)
        {
        }

        public override void Update()
        {
            ++this._framesSinceSelected;
            if (!this.opened || this._dialog.opened || this._deleteDialog.opened || this._overwriteDialog.opened || this._opening)
            {
                this._opening = false;
                foreach (ContextMenu contextMenu in this._items)
                    contextMenu.disabled = true;
            }
            else
            {
                bool flag1 = false;
                foreach (ContextMenu contextMenu in this._items)
                {
                    contextMenu.disabled = false;
                    if (!flag1 && contextMenu.hover)
                    {
                        flag1 = true;
                        string str1 = "";
                        int startIndex = this._currentDirectory.IndexOf(this._rootFolder);
                        if (startIndex != -1)
                            str1 = this._currentDirectory.Remove(startIndex, this._rootFolder.Length);
                        if (str1 != "" && !str1.EndsWith("/"))
                            str1 += "/";
                        if (str1.StartsWith("/"))
                            str1 = str1.Substring(1, str1.Length - 1);
                        string str2 = this._rootFolder + "/" + str1 + contextMenu.data;
                        if (contextMenu.isModPath)
                            str2 = contextMenu.data;
                        bool flag2 = true;
                        if (this._prevPreviewPath != str2)
                        {
                            if (str2.EndsWith(".lev"))
                            {
                                this._previewName = contextMenu.data;
                                try
                                {
                                    if (contextMenu.zipItem != null)
                                    {
                                        ZipArchiveEntry zipItem = contextMenu.zipItem as ZipArchiveEntry;
                                        this._currentPreviewZipData = new byte[zipItem.Length];
                                        zipItem.Open().Read(this._currentPreviewZipData, 0, (int)zipItem.Length);
                                        Content.generatePreviewBytes = this._currentPreviewZipData;
                                    }
                                    if (this._previewPair != null && this._previewPair.preview != null)
                                        this._previewPair.preview.Dispose();
                                    this._previewPair = Content.GeneratePreview(str2);
                                }
                                catch (Exception ex)
                                {
                                    this._previewPair = (LevelMetaData.PreviewPair)null;
                                    flag2 = false;
                                    this._previewSprite = (Sprite)null;
                                }
                                if (this._previewPair != null)
                                    this._preview = (Tex2D)this._previewPair.preview;
                            }
                            else if (str2.EndsWith(".png"))
                            {
                                string str3 = this._currentDirectory + "/" + Path.GetFileName(str2);
                                Texture2D pOriginalTex = ContentPack.LoadTexture2D(str3);
                                this._preview = pOriginalTex == null ? Content.invalidTexture : (this._type != ContextFileType.Block && this._type != ContextFileType.Background && this._type != ContextFileType.Platform || pOriginalTex.Width == 128 && pOriginalTex.Height == 128 ? (this._type != ContextFileType.Parallax || pOriginalTex.Width == 320 && pOriginalTex.Height == 240 ? (this._type != ContextFileType.ArcadeStyle ? (Tex2D)pOriginalTex : this.GetArcadeSizeTex2D(str3, (Tex2D)pOriginalTex)) : this._badParallax.texture) : this._badTileset.texture);
                            }
                            else
                            {
                                this._prevPreviewPath = (string)null;
                                flag2 = false;
                            }
                            if (flag2)
                            {
                                this._previewSprite = new Sprite(this._preview);
                                if (this._type == ContextFileType.Block || this._type == ContextFileType.Background || this._type == ContextFileType.Platform || this._type == ContextFileType.Parallax)
                                    this._previewSprite.scale = new Vec2(2f, 2f);
                            }
                            else
                                this._previewSprite = (Sprite)null;
                            this._prevPreviewPath = str2;
                        }
                    }
                }
                if (!flag1 && this._type == ContextFileType.ArcadeStyle)
                {
                    this._preview = this._badArcade.texture;
                    this._previewSprite = new Sprite(this._preview);
                    this._prevPreviewPath = (string)null;
                }
                Editor.lockInput = (ContextMenu)this;
                base.Update();
                this._scrollWait = Lerp.Float(this._scrollWait, 0.0f, 0.2f);
                if (this._dialog.result != null && this._dialog.result != "")
                {
                    string[] files = DuckFile.GetFiles(this._currentDirectory, this._dialog.result + ".lev");
                    if (files != null && files.Length != 0)
                    {
                        this._overwriteDialog.Open("OVERWRITE " + this._dialog.result + "?");
                        Editor.lockInput = (ContextMenu)this._overwriteDialog;
                        this._doOverwriteDialog = true;
                        this._overwriteDialog.result = false;
                        this._overwriteName = this._dialog.result;
                        this._dialog.result = "";
                    }
                    else
                    {
                        Editor current = Level.current as Editor;
                        Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                        Editor._currentLevelData.workshopData.Reset();
                        string saveName = this._currentDirectory + "/" + this._dialog.result;
                        current.DoSave(saveName);
                        this._dialog.result = "";
                        this.Close();
                    }
                }
                if (!this._overwriteDialog.opened && this._doOverwriteDialog)
                {
                    this._doOverwriteDialog = false;
                    if (this._overwriteDialog.result)
                    {
                        Editor current = Level.current as Editor;
                        try
                        {
                            Editor._currentLevelData.metaData.guid = (DuckFile.LoadLevel(this._currentDirectory + "/" + this._overwriteName) ?? throw new Exception()).metaData.guid;
                        }
                        catch (Exception ex)
                        {
                            if (string.IsNullOrEmpty(Editor._currentLevelData.metaData.guid))
                                Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                        }
                        current.DoSave(this._currentDirectory + "/" + this._overwriteName);
                        this._overwriteDialog.result = false;
                        this._overwriteName = "";
                        this.Close();
                    }
                }
                if (!this._deleteDialog.opened && this._doDeleteDialog)
                {
                    this._doDeleteDialog = false;
                    if (this._deleteDialog.result)
                    {
                        foreach (ContextMenu contextMenu in this._items)
                        {
                            if (contextMenu.hover)
                            {
                                Editor.Delete(this._currentDirectory + "/" + contextMenu.text + ".lev");
                                this.ComputeAvailableStorageSpace();
                                break;
                            }
                        }
                        this.ClearItems();
                        this.SetDirectory(this._currentDirectory);
                    }
                }
                if (Keyboard.Pressed(Keys.Escape) || Mouse.right == InputState.Pressed || Input.Pressed("CANCEL"))
                    this.Close();
                if (Input.Down("STRAFE"))
                {
                    if (Input.Pressed("RAGDOLL"))
                    {
                        try
                        {
                            Process.Start(Path.GetFullPath(this._currentDirectory));
                        }
                        catch (Exception ex1)
                        {
                            try
                            {
                                Process.Start(this._currentDirectory);
                            }
                            catch (Exception ex2)
                            {
                                DevConsole.Log("|DGRED|Could not open directory '" + Path.GetFullPath(this._currentDirectory) + "' (" + ex1.Message + ")");
                            }
                        }
                    }
                }
                if (!this._selectLevels && Input.Pressed("MENU2"))
                {
                    this._deleteDialog.Open("CONFIRM DELETE");
                    Editor.lockInput = (ContextMenu)this._deleteDialog;
                    this._doDeleteDialog = true;
                    this._deleteDialog.result = false;
                }
                else
                {
                    if (Input.Pressed("MENULEFT"))
                        this._selectedIndex -= this._maxItems;
                    else if (Input.Pressed("MENURIGHT"))
                        this._selectedIndex += this._maxItems;
                    this._selectedIndex = Maths.Clamp(this._selectedIndex, 0, this._items.Count - 1);
                    float num1 = 1f / (float)(this._items.Count - this._maxItems);
                    if ((double)Mouse.scroll != 0.0)
                    {
                        this._scrollPosition += (float)Math.Sign(Mouse.scroll) * num1;
                        if ((double)this._scrollPosition > 1.0)
                            this._scrollPosition = 1f;
                        if ((double)this._scrollPosition < 0.0)
                            this._scrollPosition = 0.0f;
                    }
                    bool flag3 = false;
                    int num2 = (int)Math.Round(((double)(this._items.Count - this._maxItems) - 1.0) * (double)this._scrollPosition);
                    int num3 = 0;
                    int num4 = 0;
                    for (int index = 0; index < this._items.Count; ++index)
                    {
                        if (flag3)
                            this._items[index].hover = false;
                        if (this._items[index].hover)
                        {
                            num4 = index;
                            break;
                        }
                    }
                    if (Editor.inputMode == EditorInput.Gamepad && !flag3)
                    {
                        if (num4 > num2 + this._maxItems)
                            this._scrollPosition += (float)(num4 - (num2 + this._maxItems)) * num1;
                        else if (num4 < num2)
                            this._scrollPosition -= (float)(num2 - num4) * num1;
                    }
                    for (int index = 0; index < this._items.Count; ++index)
                    {
                        this._items[index].disabled = false;
                        if (index < num2 || index > num2 + this._maxItems)
                        {
                            this._items[index].visible = false;
                            this._items[index].hover = false;
                        }
                        else
                        {
                            ContextMenu contextMenu = this._items[index];
                            this._items[index].visible = true;
                            this._items[index].position = new Vec2(this._items[index].position.x, (float)((double)this.y + 3.0 + (double)num3 * (double)this._items[index].itemSize.y));
                            ++num3;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            this.menuSize.y = this._fdHeight;
            if (!this.opened)
                return;
            base.Draw();
            float num1 = 350f;
            float num2 = this._fdHeight + 22f;
            Vec2 p1_1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0 + (double)this.hOffset - 1.0), (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0 - 15.0));
            Vec2 p2_1 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0 + (double)this.hOffset + 1.0), (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0 - 12.0));
            DuckGame.Graphics.DrawRect(p1_1, p2_1, new Color(70, 70, 70), this.depth, false);
            DuckGame.Graphics.DrawRect(p1_1, p2_1, new Color(30, 30, 30), this.depth - 8);
            DuckGame.Graphics.DrawRect(p1_1 + new Vec2(3f, 23f), p2_1 + new Vec2(-18f, -4f), new Color(10, 10, 10), this.depth - 4);
            Vec2 p1_2 = new Vec2(p2_1.x - 16f, p1_1.y + 23f);
            Vec2 p2_2 = p2_1 + new Vec2(-3f, -4f);
            DuckGame.Graphics.DrawRect(p1_2, p2_2, new Color(10, 10, 10), this.depth - 4);
            DuckGame.Graphics.DrawRect(p1_1 + new Vec2(3f, 3f), new Vec2(p2_1.x - 3f, p1_1.y + 19f), new Color(70, 70, 70), this.depth - 4);
            if (this._scrollBar)
            {
                this._scrollLerp = Lerp.Float(this._scrollLerp, this._scrollPosition, 0.05f);
                Vec2 p1_3 = new Vec2(p2_1.x - 14f, (float)((double)this.topRight.y + 7.0 + (240.0 * (double)this._scrollLerp - 4.0)));
                Vec2 p2_3 = new Vec2(p2_1.x - 5f, (float)((double)this.topRight.y + 11.0 + (240.0 * (double)this._scrollLerp + 8.0)));
                bool flag = false;
                if ((double)Mouse.x > (double)p1_3.x && (double)Mouse.x < (double)p2_3.x && (double)Mouse.y > (double)p1_3.y && (double)Mouse.y < (double)p2_3.y)
                {
                    flag = true;
                    if (Mouse.left == InputState.Pressed)
                        this.drag = true;
                }
                if (Mouse.left == InputState.None)
                    this.drag = false;
                if (this.drag)
                {
                    this._scrollPosition = (float)(((double)Mouse.y - (double)p1_2.y - 10.0) / ((double)p2_2.y - (double)p1_2.y - 20.0));
                    if ((double)this._scrollPosition < 0.0)
                        this._scrollPosition = 0.0f;
                    if ((double)this._scrollPosition > 1.0)
                        this._scrollPosition = 1f;
                    this._scrollLerp = this._scrollPosition;
                }
                DuckGame.Graphics.DrawRect(p1_3, p2_3, this.drag ? new Color(190, 190, 190) : (flag ? new Color(120, 120, 120) : new Color(70, 70, 70)), this.depth + 4);
            }
            string str1 = this._currentDirectory;
            int startIndex1 = this._currentDirectory.IndexOf(this._rootFolder);
            if (startIndex1 != -1)
                str1 = this._currentDirectory.Remove(startIndex1, this._rootFolder.Length);
            string str2 = Path.GetFileName(this._rootFolder) + str1;
            if (this.isModPath)
                str2 = this._currentDirectory.Replace(DuckFile.modsDirectory, "").Replace(DuckFile.globalModsDirectory, "");
            if (str2 == "")
                str2 = this._type != ContextFileType.Block ? (this._type != ContextFileType.Platform ? (this._type != ContextFileType.Background ? (this._type != ContextFileType.Parallax ? (this._type != ContextFileType.ArcadeStyle ? "LEVELS" : "Custom/Arcade") : "Custom/Parallax") : "Custom/Background") : "Custom/Platform") : "Custom/Blocks";
            string str3 = !this._save ? (!this._selectLevels ? (this._type != ContextFileType.Block ? (this._type != ContextFileType.Platform ? (this._type != ContextFileType.Background ? (this._type != ContextFileType.Parallax ? (this._type != ContextFileType.ArcadeStyle ? "@LOADICON@Load Level" : "@LOADICON@Custom") : "@LOADICON@Custom") : "@LOADICON@Custom") : "@LOADICON@Custom") : "@LOADICON@Custom") : "Select Active Levels") : "@SAVEICON@Save Level";
            string str4 = str2;
            DuckGame.Graphics.DrawString(str3 + (str4 == "" ? "" : " - " + str4), p1_1 + new Vec2(5f, 7f), Color.White, this.depth + 8);
            Vec2 p1_4 = new Vec2(p2_1.x + 2f, p1_1.y);
            Vec2 p2_4 = p1_4 + new Vec2(164f, 120f);
            if (this._previewSprite != null && this._previewSprite.texture != null && (this._type == ContextFileType.Block || this._type == ContextFileType.Background || this._type == ContextFileType.Platform || this._type == ContextFileType.Parallax || this._type == ContextFileType.ArcadeStyle || this._type == ContextFileType.ArcadeAnimation))
                p2_4 = this._type != ContextFileType.Parallax ? p1_4 + new Vec2((float)(this._previewSprite.width + 4), (float)(this._previewSprite.height + 4)) : p1_4 + new Vec2((float)(this._previewSprite.width / 2 + 4), (float)(this._previewSprite.height / 2 + 4));
            DuckGame.Graphics.DrawRect(p1_4, p2_4, new Color(70, 70, 70), this.depth, false);
            DuckGame.Graphics.DrawRect(p1_4, p2_4, new Color(30, 30, 30), this.depth - 8);
            if (this._previewSprite == null || this._previewSprite.texture == null)
                return;
            this._previewSprite.depth = (Depth)0.95f;
            this._previewSprite.scale = new Vec2(0.5f);
            if (this._type == ContextFileType.Block || this._type == ContextFileType.Background || this._type == ContextFileType.Platform)
                this._previewSprite.scale = new Vec2(1f);
            DuckGame.Graphics.Draw(this._previewSprite, p1_4.x + 2f, p1_4.y + 2f);
            if (this._previewPair == null)
                return;
            string str5 = this._previewName;
            int startIndex2 = str5.LastIndexOf("/");
            if (startIndex2 != -1)
                str5 = str5.Substring(startIndex2, str5.Length - startIndex2);
            if (str5.Length > 19)
            {
                string str6 = str5.Substring(0, 18) + ".";
            }
            this._fancyFont.maxWidth = 160;
            string str7 = "";
            if (this._previewPair.strange)
            {
                DuckGame.Graphics.DrawString(str7 + "STRANGE LEVEL", p1_4 + new Vec2(5f, 107f), Colors.DGPurple, this.depth + 8);
                Vec2 p1_5 = p1_4 + new Vec2(0.0f, 122f);
                Vec2 p2_5 = p1_5 + new Vec2(166f, 36f);
                DuckGame.Graphics.DrawRect(p1_5, p2_5, new Color(70, 70, 70), this.depth, false);
                DuckGame.Graphics.DrawRect(p1_5, p2_5, new Color(30, 30, 30), this.depth - 8);
                this._fancyFont.Draw("Must place at least one Duck Spawn Point to make a valid level.", p1_5.x + 4f, p1_5.y + 4f, Color.White, this.depth + 8);
            }
            else if (this._previewPair.arcade)
                DuckGame.Graphics.DrawString(str7 + "ARCADE LAYOUT", p1_4 + new Vec2(5f, 107f), Colors.DGYellow, this.depth + 8);
            else if (this._previewPair.challenge)
                DuckGame.Graphics.DrawString(str7 + "CHALLENGE LEVEL", p1_4 + new Vec2(5f, 107f), Colors.DGRed, this.depth + 8);
            else if (this._previewPair.invalid == null || this._previewPair.invalid.Count == 0)
            {
                DuckGame.Graphics.DrawString(str7 + "ONLINE LEVEL", p1_4 + new Vec2(5f, 107f), Colors.DGGreen, this.depth + 8);
            }
            else
            {
                DuckGame.Graphics.DrawString(str7 + "LOCAL LEVEL", p1_4 + new Vec2(5f, 107f), Colors.DGBlue, this.depth + 8);
                Vec2 p1_6 = p1_4 + new Vec2(0.0f, 122f);
                this._fancyFont.Draw("Contains the following Local-Only objects:", p1_6.x + 4f, p1_6.y + 4f, Color.White, this.depth + 8);
                int num3 = 22;
                if (this._previewPair.invalid != null)
                {
                    foreach (KeyValuePair<string, int> keyValuePair in this._previewPair.invalid)
                    {
                        string text = "- " + keyValuePair.Key + (keyValuePair.Value > 1 ? " (x" + keyValuePair.Value.ToString() + ")" : "");
                        this._fancyFont.Draw(text, p1_6.x + 4f, p1_6.y + 4f + (float)num3, Color.White, this.depth + 8);
                        if ((double)this._fancyFont.GetWidth(text) > 160.0)
                            num3 += 12;
                        num3 += 12;
                    }
                }
                Vec2 p2_6 = p1_6 + new Vec2(166f, (float)(6 + num3));
                DuckGame.Graphics.DrawRect(p1_6, p2_6, new Color(70, 70, 70), this.depth, false);
                DuckGame.Graphics.DrawRect(p1_6, p2_6, new Color(30, 30, 30), this.depth - 8);
            }
        }
    }
}
