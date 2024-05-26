using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Reflection;

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
        private float prevScrollPosition = -1;
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

        public string rootFolder => _rootFolder;

        public MonoFileDialog()
          : base(null)
        {
        }

        public override void Initialize()
        {
            layer = Layer.HUD;
            depth = (Depth)0.9f;
            _showBackground = false;
            _fancyFont = new FancyBitmapFont("smallFont");
            itemSize = new Vec2(390f, 16f);
            _root = true;
            _dialog = new TextEntryDialog
            {
                filename = true
            };
            Level.Add(_dialog);
            _deleteDialog = new MessageDialogue();
            Level.Add(_deleteDialog);
            _overwriteDialog = new MessageDialogue();
            Level.Add(_overwriteDialog);
            drawControls = false;
        }

        public void Open(
          string rootFolder,
          string currentFolder,
          bool save,
          bool selectLevels = false,
          bool loadLevel = true,
          ContextFileType type = ContextFileType.Level)
        {
            _type = type;
            _selectLevels = selectLevels;
            _loadLevel = loadLevel;
            if (_type == ContextFileType.Block || _type == ContextFileType.Background || _type == ContextFileType.Platform)
                _badTileset = new Sprite("badTileset");
            if (_type == ContextFileType.Parallax)
                _badParallax = new Sprite("badParallax");
            if (_type == ContextFileType.ArcadeStyle)
                _badArcade = new Sprite("badArcade");
            _preview = null;
            _previewSprite = null;
            float num1 = 350f;
            float num2 = 350f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2f - num1 / 2f) + hOffset, (float)(layer.height / 2f - num2 / 2f));
            Vec2 vec2_2 = new Vec2((float)(layer.width / 2f + num1 / 2f) + hOffset, (float)(layer.height / 2f + num2 / 2f));
            position = vec2_1 + new Vec2(4f, 40f);
            _save = save;
            rootFolder = rootFolder.Replace('\\', '/');
            currentFolder = currentFolder.Replace('\\', '/');
            _currentDirectory = !(currentFolder == "") ? currentFolder : rootFolder;
            _rootFolder = rootFolder;
            if (prevDirectory != null)
                _currentDirectory = prevDirectory;
            SetDirectory(_currentDirectory);
            if (prevScrollPosition != -1)
            {
                _scrollPosition = prevScrollPosition;
            }
            Editor.lockInput = this;
            ComputeAvailableStorageSpace();
            SFX.Play("openClick", 0.4f);
            opened = true;
        }

        private void ComputeAvailableStorageSpace()
        {
            float percent = 0f;
            if (!DuckFile.GetLevelSpacePercentUsed(ref percent))
                return;
            _percentStorageUsed = percent > 100f ? 100f : percent;
        }

        public void Close()
        {
            Editor.lockInput = null;
            opened = false;
            ClearItems();
        }

        public string TypeExtension()
        {
            if (_type == ContextFileType.Level)
                return ".lev";
            return _type == ContextFileType.Block || _type == ContextFileType.Background || _type == ContextFileType.Platform || _type == ContextFileType.Parallax || _type == ContextFileType.ArcadeAnimation || _type == ContextFileType.ArcadeStyle ? ".png" : "";
        }

        public void SetDirectory(string dir) => SetDirectory(dir, false);

        private string FixPath(string pPath)
        {
            string str = Path.GetFullPath(pPath).Replace('\\', '/');
            while (str.StartsWith("/") && (!Program.IsLinuxD || !Path.IsPathRooted(str)))
                str = str.Substring(1);
            if (str.EndsWith("/"))
                str = str.Substring(0, str.Length - 1);
            return str;
        }

        public void SetDirectory(string dir, bool pIsModPath)
        {
            if (_openedArchive != null)
            {
                _openedArchive.Dispose();
                _openedArchive = null;
            }
            DevConsole.Log("MonoFileDialog.SetDirectory(" + dir + ")");
            if (!pIsModPath && dir.Contains("Mods") && (dir.Contains(DuckFile.globalModsDirectory) || dir.Contains(DuckFile.modsDirectory)))
                pIsModPath = true;
            isModPath = pIsModPath;
            dir = FixPath(dir);
            _drawIndex = 0;
            DevConsole.Log("MonoFileDialog.SetDirectory(postfix)(" + dir + ")");
            if (modRootPath != null && modRootPath == dir)
            {
                dir = _rootFolder;
                modRootPath = null;
                isModPath = false;
            }
            if (dir.Length < _rootFolder.Length && !pIsModPath) dir = _rootFolder;
            int num1 = 0;
            _currentDirectory = dir;
            if (_currentDirectory != _rootFolder) num1++;
            if (_save) num1++;
            prevDirectory = dir;
            string[] directories = DuckFile.GetDirectories(_currentDirectory);
            string[] files = DuckFile.GetFiles(_currentDirectory);
            int num2 = num1 + (directories.Length + files.Length);
            Array.Sort(directories);
            Array.Sort(files);
            float x = 338f;
            _scrollBar = false;
            _scrollPosition = 0f;
            if (num2 > _maxItems)
            {
                x = 326f;
                _scrollBar = true;
            }
            if (_save)
            {
                ContextMenu contextMenu = new ContextMenu(this)
                {
                    layer = layer,
                    text = "@NEWICONTINY@New File...",
                    data = "New File...",
                    itemSize = new Vec2(x, 16f)
                };
                AddItem(contextMenu);
            }
            if (_currentDirectory != _rootFolder)
            {
                ContextMenu contextMenu = new ContextMenu(this)
                {
                    layer = layer
                };
                bool flag = false;
                contextMenu.text = "@LOADICON@../";
                contextMenu.data = "../";
                contextMenu.itemSize = new Vec2(x, 16f);
                contextMenu.isModPath = flag;
                AddItem(contextMenu);
            }
            else
            {
                foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                {
                    if (accessibleMod.configuration != null && accessibleMod.configuration.content != null && accessibleMod.configuration.content.levels.Count > 0 && accessibleMod.configuration.contentDirectory.Contains(DuckFile.saveDirectory))
                    {
                        ContextMenu contextMenu = new ContextMenu(this)
                        {
                            layer = layer,
                            fancy = true,
                            text = "@RAINBOWTINY@|DGBLUE|" + accessibleMod.configuration.name,
                            data = accessibleMod.configuration.contentDirectory + "/Levels",
                            itemSize = new Vec2(x, 16f),
                            mod = accessibleMod,
                            isModPath = true,
                            isModRoot = true
                        };
                        AddItem(contextMenu);
                    }
                }
                foreach (MapPack mapPack in MapPack.active)
                {
                    if (mapPack.mod != null && mapPack.mod.configuration.directory.Contains(DuckFile.saveDirectory))
                    {
                        ContextMenu contextMenu = new ContextMenu(this)
                        {
                            layer = layer,
                            fancy = true,
                            text = "|DGBLUE|" + mapPack.mod.configuration.name,
                            data = mapPack.mod.configuration.directory,
                            itemSize = new Vec2(x, 16f),
                            mod = mapPack.mod,
                            isModPath = true,
                            isModRoot = true,
                            customIcon = mapPack.mod.configuration.mapPack.icon
                        };
                        AddItem(contextMenu);
                    }
                }
            }
            foreach (string path in directories)
            {
                string fileName = Path.GetFileName(path);
                ContextMenu contextMenu = new ContextMenu(this)
                {
                    layer = layer,
                    fancy = true,
                    text = "@LOADICON@" + fileName,
                    data = !pIsModPath ? fileName : path,
                    isModPath = pIsModPath,
                    itemSize = new Vec2(x, 16f)
                };
                AddItem(contextMenu);
            }
            int num3 = 0;
            List<string> _files = new List<string>(files);
            string devfilepath = Program.GameDirectory + "Content\\levels\\devtestlev.lev";
            /*
            if (File.Exists(devfilepath))
            {
                _files.Insert(0, devfilepath);
            }
            */
            foreach (string path3 in _files)
            {
                bool _pIsModPath = pIsModPath;
                if (path3 == devfilepath)
                {
                    _pIsModPath = true;
                }
                string path = path3;
                if (path.StartsWith("|"))
                    path = path.Substring(1, path.Length - 1);
                string fileName = Path.GetFileName(path);
                bool heart33 = false;
                if (DGRSettings.PreferredLevel != "" && path == DGRSettings.PreferredLevel)
                {
                    heart33 = true;
                }
                if (!_selectLevels)
                {
                    if (fileName.EndsWith(TypeExtension()))
                    {
                        string text = fileName;
                        if (heart33) text = text.Insert(0, "|PINK|♥|WHITE|");
                        ContextMenu contextMenu = new ContextMenu(this)
                        {
                            layer = layer,
                            fancy = true,
                            text = text
                        };
                        contextMenu.text = text.Substring(0, text.Length - 4);
                        contextMenu.data = !_pIsModPath ? fileName : path;
                        contextMenu.itemSize = new Vec2(x, 16f);
                        contextMenu.isModPath = _pIsModPath;
                        AddItem(contextMenu);
                    }
                }
                else
                {
                    string str1 = path.Replace('\\', '/');
                    string str2 = str1.Substring(0, str1.Length - 4);
                    string str3 = str2.Substring(str2.IndexOf("/levels/", StringComparison.InvariantCultureIgnoreCase) + 8);
                    ContextCheckBox contextCheckBox = new ContextCheckBox(fileName, this)
                    {
                        layer = layer,
                        fancy = true,
                        path = str3,
                        isChecked = Editor.activatedLevels.Contains(str3),
                        itemSize = new Vec2(x, 16f)
                    };
                    AddItem(contextCheckBox);
                }
                num3++;
            }
            int num4 = (int)Math.Round((_items.Count - 1 - _maxItems) * _scrollPosition);
            int n = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                ContextMenu cm = _items[i];
                if (i < num4 || i > num4 + _maxItems)
                {
                    cm.visible = false;
                }
                else
                {
                    cm.visible = true;
                    cm.position = new Vec2(_items[i].position.x, (float)(y + 3f + n * (_items[i].itemSize.y + 1f)));
                    n++;
                }
            }
            menuSize.y = _fdHeight;
        }

        private Tex2D GetArcadeSizeTex2D(string pTex, Tex2D pOriginalTex)
        {
            if (pOriginalTex.width == 48 && pOriginalTex.height == 48)
                return pOriginalTex;
            Image image = Image.FromFile(_currentDirectory + "/" + Path.GetFileName(pTex));
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, 48, 48);
            Bitmap bitmap = new Bitmap(48, 48);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
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
            bitmap.Save(memoryStream, ImageFormat.Png);
            return (Tex2D)Texture2D.FromStream(Graphics.device, memoryStream);
        }

        public override void Selected(ContextMenu item)
        {
            bool flag = false;
            if (_framesSinceSelected < 20 && _lastItemSelected == item)
                flag = true;
            _lastItemSelected = item;
            _framesSinceSelected = 0;
            if (!flag && Editor.inputMode == EditorInput.Touch)
                return;
            if (_percentStorageUsed >= 100f && (_save || item.text == "@NEWICONTINY@New File..."))
            {
                SFX.Play("consoleError");
            }
            else
            {
                SFX.Play("highClick", 0.3f, 0.2f);
                if (item.text == "@NEWICONTINY@New File...")
                {
                    _dialog.Open("Save File As...");
                    Editor.lockInput = _dialog;
                }
                else if (item.data.EndsWith(TypeExtension()) && _type != ContextFileType.All)
                {
                    prevScrollPosition = _scrollPosition;
                    if (!_selectLevels)
                    {
                        if (!_save)
                        {
                            Close();
                            string str = _currentDirectory + "/" + item.data;
                            if (item.isModPath)
                                str = item.data;
                            if (_loadLevel)
                                (Level.current as Editor).LoadLevel(str);
                            else
                                result = _type != ContextFileType.ArcadeStyle ? str.Replace(_rootFolder, "") : Editor.TextureToString((Texture2D)GetArcadeSizeTex2D(str, Content.Load<Tex2D>(str)));
                        }
                        else
                        {
                            _overwriteDialog.Open("OVERWRITE " + item.data + "?");
                            Editor.lockInput = _overwriteDialog;
                            _doOverwriteDialog = true;
                            _overwriteDialog.result = false;
                            _overwriteName = item.data;
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
                    ClearItems();
                    if (item.isModPath)
                    {
                        if (item.isModRoot)
                            modRootPath = FixPath(item.data + "/../");
                        SetDirectory(item.data, item.isModPath);
                    }
                    else
                        SetDirectory(_currentDirectory + "/" + item.data);
                }
                ComputeAvailableStorageSpace();
            }
        }

        public override void Toggle(ContextMenu item)
        {
        }

        public override void Update()
        {
            ++_framesSinceSelected;
            if (!opened || _dialog.opened || _deleteDialog.opened || _overwriteDialog.opened || _opening)
            {
                _opening = false;
                foreach (ContextMenu contextMenu in _items)
                    contextMenu.disabled = true;
            }
            else
            {
                bool flag1 = false;
                foreach (ContextMenu contextMenu in _items)
                {
                    contextMenu.disabled = false;
                    if (!flag1 && contextMenu.hover)
                    {
                        flag1 = true;
                        string str1 = "";
                        int startIndex = _currentDirectory.IndexOf(_rootFolder);
                        if (startIndex != -1)
                            str1 = _currentDirectory.Remove(startIndex, _rootFolder.Length);
                        if (str1 != "" && !str1.EndsWith("/"))
                            str1 += "/";
                        if (str1.StartsWith("/") && (!Program.IsLinuxD || !Path.IsPathRooted(str1)))
                            str1 = str1.Substring(1, str1.Length - 1);
                        string str2 = _rootFolder + "/" + str1 + contextMenu.data;
                        if (contextMenu.isModPath)
                            str2 = contextMenu.data;
                        bool flag2 = true;
                        if (_prevPreviewPath != str2)
                        {
                            if (str2.EndsWith(".lev"))
                            {
                                _previewName = contextMenu.data;
                                try
                                {
                                    if (contextMenu.zipItem != null)
                                    {
                                        ZipArchiveEntry zipItem = contextMenu.zipItem as ZipArchiveEntry;
                                        _currentPreviewZipData = new byte[zipItem.Length];
                                        zipItem.Open().Read(_currentPreviewZipData, 0, (int)zipItem.Length);
                                        Content.generatePreviewBytes = _currentPreviewZipData;
                                    }
                                    if (_previewPair != null && _previewPair.preview != null)
                                        _previewPair.preview.Dispose();
                                    _previewPair = Content.GeneratePreview(str2);
                                }
                                catch (Exception)
                                {
                                    _previewPair = null;
                                    flag2 = false;
                                    _previewSprite = null;
                                }
                                if (_previewPair != null)
                                    _preview = (Tex2D)_previewPair.preview;
                            }
                            else if (str2.EndsWith(".png"))
                            {
                                string str3 = _currentDirectory + "/" + Path.GetFileName(str2);
                                Texture2D pOriginalTex = ContentPack.LoadTexture2D(str3);
                                _preview = pOriginalTex == null ? Content.invalidTexture : (_type != ContextFileType.Block && _type != ContextFileType.Background && _type != ContextFileType.Platform || pOriginalTex.Width == 128 && pOriginalTex.Height == 128 ? (_type != ContextFileType.Parallax || pOriginalTex.Width == 320 && pOriginalTex.Height == 240 ? (_type != ContextFileType.ArcadeStyle ? (Tex2D)pOriginalTex : GetArcadeSizeTex2D(str3, (Tex2D)pOriginalTex)) : _badParallax.texture) : _badTileset.texture);
                            }
                            else
                            {
                                _prevPreviewPath = null;
                                flag2 = false;
                            }
                            if (flag2)
                            {
                                _previewSprite = new Sprite(_preview);
                                if (_type == ContextFileType.Block || _type == ContextFileType.Background || _type == ContextFileType.Platform || _type == ContextFileType.Parallax)
                                    _previewSprite.scale = new Vec2(2f, 2f);
                            }
                            else
                                _previewSprite = null;
                            _prevPreviewPath = str2;
                        }
                    }
                }
                if (!flag1 && _type == ContextFileType.ArcadeStyle)
                {
                    _preview = _badArcade.texture;
                    _previewSprite = new Sprite(_preview);
                    _prevPreviewPath = null;
                }
                Editor.lockInput = this;
                base.Update();
                _scrollWait = Lerp.Float(_scrollWait, 0f, 0.2f);
                if (_dialog.result != null && _dialog.result != "")
                {
                    string[] files = DuckFile.GetFiles(_currentDirectory, _dialog.result + ".lev");
                    if (files != null && files.Length != 0)
                    {
                        _overwriteDialog.Open("OVERWRITE " + _dialog.result + "?");
                        Editor.lockInput = _overwriteDialog;
                        _doOverwriteDialog = true;
                        _overwriteDialog.result = false;
                        _overwriteName = _dialog.result;
                        _dialog.result = "";
                    }
                    else
                    {
                        Editor current = Level.current as Editor;
                        Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                        Editor._currentLevelData.workshopData.Reset();
                        string saveName = _currentDirectory + "/" + _dialog.result;
                        current.DoSave(saveName);
                        _dialog.result = "";
                        Close();
                    }
                }
                if (!_overwriteDialog.opened && _doOverwriteDialog)
                {
                    _doOverwriteDialog = false;
                    if (_overwriteDialog.result)
                    {
                        Editor current = Level.current as Editor;
                        try
                        {
                            Editor._currentLevelData.metaData.guid = (DuckFile.LoadLevel(_currentDirectory + "/" + _overwriteName) ?? throw new Exception()).metaData.guid;
                        }
                        catch (Exception)
                        {
                            if (string.IsNullOrEmpty(Editor._currentLevelData.metaData.guid))
                                Editor._currentLevelData.metaData.guid = Guid.NewGuid().ToString();
                        }
                        current.DoSave(_currentDirectory + "/" + _overwriteName);
                        _overwriteDialog.result = false;
                        _overwriteName = "";
                        Close();
                    }
                }
                if (!_deleteDialog.opened && _doDeleteDialog)
                {
                    _doDeleteDialog = false;
                    if (_deleteDialog.result)
                    {
                        foreach (ContextMenu contextMenu in _items)
                        {
                            if (contextMenu.hover)
                            {
                                Editor.Delete(_currentDirectory + "/" + contextMenu.text + ".lev");
                                ComputeAvailableStorageSpace();
                                break;
                            }
                        }
                        ClearItems();
                        SetDirectory(_currentDirectory);
                    }
                }
                if (Keyboard.Pressed(Keys.Escape) || Mouse.right == InputState.Pressed || Input.Pressed(Triggers.Cancel))
                    Close();
                if (Input.Down(Triggers.Strafe))
                {
                    if (Input.Pressed(Triggers.Ragdoll))
                    {
                        try
                        {
                            Process.Start(Path.GetFullPath(_currentDirectory));
                        }
                        catch (Exception ex1)
                        {
                            try
                            {
                                Process.Start(_currentDirectory);
                            }
                            catch (Exception)
                            {
                                DevConsole.Log("|DGRED|Could not open directory '" + Path.GetFullPath(_currentDirectory) + "' (" + ex1.Message + ")");
                            }
                        }
                    }
                }
                if (Input.Pressed(Triggers.Shoot) && _items.Count > 0)
                {
                    if (_selectedIndex < 0 || _selectedIndex >= _items.Count) // crashed found related to _selectedIndex being out of bounds
                    {
                        _selectedIndex = 0;
                    }
                    ContextMenu cm = _items[_selectedIndex];
                    if (cm.text.Contains("@"))
                    {
                        SFX.Play("consoleError");

                    }
                    else
                    {
                        if (DGRSettings.PreferredLevel == _currentDirectory + cm.data)
                        {
                            SFX.Play("cutOffQuack2", 0.5f);
                            cm.text = cm.text.Remove(0, 14);
                            DGRSettings.PreferredLevel = "";

                        }
                        else
                        {
                            if (DGRSettings.PreferredLevel != "")
                            {
                                ContextMenu cl = _items.Find(cl => _currentDirectory + cl.data == DGRSettings.PreferredLevel);
                                if (cl != null) cl.text = cl.text.Remove(0, 14);
                            }
                            SFX.Play("preach3", 0.5f, Rando.Float(0.2f));
                            cm.text = cm.text.Insert(0, "|PINK|♥|WHITE|");
                            DGRSettings.PreferredLevel = _currentDirectory + cm.data;
                        }
                    }
                }
                if (!_selectLevels && Input.Pressed(Triggers.Menu2))
                {
                    _deleteDialog.Open("CONFIRM DELETE");
                    Editor.lockInput = _deleteDialog;
                    _doDeleteDialog = true;
                    _deleteDialog.result = false;
                }
                else
                {
                    if (Input.Pressed(Triggers.MenuLeft))
                        _selectedIndex -= _maxItems;
                    else if (Input.Pressed(Triggers.MenuRight))
                        _selectedIndex += _maxItems;
                    _selectedIndex = Maths.Clamp(_selectedIndex, 0, _items.Count - 1);
                    float num1 = 1f / (_items.Count - _maxItems);
                    if (Mouse.scroll != 0)
                    {
                        _scrollPosition += Math.Sign(Mouse.scroll) * num1;
                        if (_scrollPosition > 1)
                            _scrollPosition = 1f;
                        if (_scrollPosition < 0)
                            _scrollPosition = 0f;
                    }
                    bool flag3 = false;
                    int num2 = (int)Math.Round((_items.Count - _maxItems - 1f) * _scrollPosition);
                    int num3 = 0;
                    int num4 = 0;
                    for (int index = 0; index < _items.Count; ++index)
                    {
                        if (flag3)
                            _items[index].hover = false;
                        if (_items[index].hover)
                        {
                            num4 = index;
                            break;
                        }
                    }
                    if (Editor.inputMode == EditorInput.Gamepad && !flag3)
                    {
                        if (num4 > num2 + _maxItems)
                            _scrollPosition += (num4 - (num2 + _maxItems)) * num1;
                        else if (num4 < num2)
                            _scrollPosition -= (num2 - num4) * num1;
                    }
                    for (int index = 0; index < _items.Count; ++index)
                    {
                        _items[index].disabled = false;
                        if (index < num2 || index > num2 + _maxItems)
                        {
                            _items[index].visible = false;
                            _items[index].hover = false;
                        }
                        else
                        {
                            ContextMenu contextMenu = _items[index];
                            _items[index].visible = true;
                            _items[index].position = new Vec2(_items[index].position.x, (float)(y + 3f + num3 * _items[index].itemSize.y));
                            ++num3;
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            menuSize.y = _fdHeight;
            if (!opened)
                return;
            base.Draw();
            float num1 = 350f;
            float num2 = _fdHeight + 22f;
            Vec2 topLeft = new Vec2((float)(layer.width / 2f - num1 / 2f + hOffset - 1f), (float)(layer.height / 2f - num2 / 2f - 15f));
            Vec2 bottomRight = new Vec2((float)(layer.width / 2f + num1 / 2f + hOffset + 1f), (float)(layer.height / 2f + num2 / 2f - 12f));
            Graphics.DrawRect(topLeft, bottomRight, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(topLeft, bottomRight, new Color(30, 30, 30), depth - 8);
            Graphics.DrawRect(topLeft + new Vec2(3f, 23f), bottomRight + new Vec2(-18f, -4f), new Color(10, 10, 10), depth - 4);
            Vec2 p1_2 = new Vec2(bottomRight.x - 16f, topLeft.y + 23f);
            Vec2 p2_2 = bottomRight + new Vec2(-3f, -4f);
            Graphics.DrawRect(p1_2, p2_2, new Color(10, 10, 10), depth - 4);
            Graphics.DrawRect(topLeft + new Vec2(3f, 3f), new Vec2(bottomRight.x - 3f, topLeft.y + 19f), new Color(70, 70, 70), depth - 4);
            if (_scrollBar)
            {
                _scrollLerp = Lerp.Float(_scrollLerp, _scrollPosition, 0.05f);
                Vec2 p1_3 = new Vec2(bottomRight.x - 14f, (float)(topRight.y + 7f + (240f * _scrollLerp - 4f)));
                Vec2 p2_3 = new Vec2(bottomRight.x - 5f, (float)(topRight.y + 11f + (240f * _scrollLerp + 8f)));
                bool flag = false;
                if (Mouse.x > p1_3.x && Mouse.x < p2_3.x && Mouse.y > p1_3.y && Mouse.y < p2_3.y)
                {
                    flag = true;
                    if (Mouse.left == InputState.Pressed)
                        drag = true;
                }
                if (Mouse.left == InputState.None)
                    drag = false;
                if (drag)
                {
                    _scrollPosition = (float)((Mouse.y - p1_2.y - 10f) / (p2_2.y - p1_2.y - 20f));
                    if (_scrollPosition < 0f)
                        _scrollPosition = 0f;
                    if (_scrollPosition > 1f)
                        _scrollPosition = 1f;
                    _scrollLerp = _scrollPosition;
                }
                Graphics.DrawRect(p1_3, p2_3, drag ? new Color(190, 190, 190) : (flag ? new Color(120, 120, 120) : new Color(70, 70, 70)), depth + 4);
            }
            string str1 = _currentDirectory;
            int startIndex1 = _currentDirectory.IndexOf(_rootFolder);
            if (startIndex1 != -1)
                str1 = _currentDirectory.Remove(startIndex1, _rootFolder.Length);
            string path = Path.GetFileName(_rootFolder) + str1;
            if (isModPath)
                path = _currentDirectory.Replace(DuckFile.modsDirectory, "").Replace(DuckFile.globalModsDirectory, "");
            if (path == "")
                path = _type != ContextFileType.Block ? (_type != ContextFileType.Platform ? (_type != ContextFileType.Background ? (_type != ContextFileType.Parallax ? (_type != ContextFileType.ArcadeStyle ? "LEVELS" : "Custom/Arcade") : "Custom/Parallax") : "Custom/Background") : "Custom/Platform") : "Custom/Blocks";
            string headingString = !_save ? (!_selectLevels ? (_type != ContextFileType.Block ? (_type != ContextFileType.Platform ? (_type != ContextFileType.Background ? (_type != ContextFileType.Parallax ? (_type != ContextFileType.ArcadeStyle ? "@LOADICON@Load Level" : "@LOADICON@Custom") : "@LOADICON@Custom") : "@LOADICON@Custom") : "@LOADICON@Custom") : "@LOADICON@Custom") : "Select Active Levels") : "@SAVEICON@Save Level";
            string drawPath = path;

            Graphics.DrawString(headingString + (drawPath == "" ? "" : " - " + drawPath), topLeft + new Vec2(5f, 7f), Color.White, depth + 8);
            Vec2 part2TL = new Vec2(bottomRight.x + 2f, topLeft.y);
            Vec2 part2BR = part2TL + new Vec2(164f, 120f);
            if (_previewSprite != null && _previewSprite.texture != null && (_type == ContextFileType.Block || _type == ContextFileType.Background || _type == ContextFileType.Platform || _type == ContextFileType.Parallax || _type == ContextFileType.ArcadeStyle || _type == ContextFileType.ArcadeAnimation))
                part2BR = _type != ContextFileType.Parallax ? part2TL + new Vec2(_previewSprite.width + 4, _previewSprite.height + 4) : part2TL + new Vec2(_previewSprite.width / 2 + 4, _previewSprite.height / 2 + 4);
            Graphics.DrawRect(part2TL, part2BR, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(part2TL, part2BR, new Color(30, 30, 30), depth - 8);
            if (_previewSprite == null || _previewSprite.texture == null)
                return;
            _previewSprite.depth = (Depth)0.95f;
            _previewSprite.scale = new Vec2(0.5f);
            if (_type == ContextFileType.Block || _type == ContextFileType.Background || _type == ContextFileType.Platform)
                _previewSprite.scale = new Vec2(1f);
            Graphics.Draw(_previewSprite, part2TL.x + 2f, part2TL.y + 2f);
            if (_previewPair == null)
                return;
            string str5 = _previewName;
            int startIndex2 = str5.LastIndexOf("/");
            if (startIndex2 != -1)
                str5 = str5.Substring(startIndex2, str5.Length - startIndex2);
            if (str5.Length > 19)
            {
                string str6 = str5.Substring(0, 18) + ".";
            }
            _fancyFont.maxWidth = 160;
            string str7 = "";
            if (_previewPair.strange)
            {
                Graphics.DrawString(str7 + "STRANGE LEVEL", part2TL + new Vec2(5f, 107f), Colors.DGPurple, depth + 8);
                Vec2 p1_5 = part2TL + new Vec2(0f, 122f);
                Vec2 p2_5 = p1_5 + new Vec2(166f, 36f);
                Graphics.DrawRect(p1_5, p2_5, new Color(70, 70, 70), depth, false);
                Graphics.DrawRect(p1_5, p2_5, new Color(30, 30, 30), depth - 8);
                _fancyFont.Draw("Must place at least one Duck Spawn Point to make a valid level.", p1_5.x + 4f, p1_5.y + 4f, Color.White, depth + 8);
            }
            else if (_previewPair.arcade)
                Graphics.DrawString(str7 + "ARCADE LAYOUT", part2TL + new Vec2(5f, 107f), Colors.DGYellow, depth + 8);
            else if (_previewPair.challenge)
                Graphics.DrawString(str7 + "CHALLENGE LEVEL", part2TL + new Vec2(5f, 107f), Colors.DGRed, depth + 8);
            else if (_previewPair.invalid == null || _previewPair.invalid.Count == 0)
            {
                Graphics.DrawString(str7 + "ONLINE LEVEL", part2TL + new Vec2(5f, 107f), Colors.DGGreen, depth + 8);
            }
            else
            {
                Graphics.DrawString(str7 + "LOCAL LEVEL", part2TL + new Vec2(5f, 107f), Colors.DGBlue, depth + 8);
                Vec2 p1_6 = part2TL + new Vec2(0f, 122f);
                _fancyFont.Draw("Contains the following Local-Only objects:", p1_6.x + 4f, p1_6.y + 4f, Color.White, depth + 8);
                int num3 = 22;
                if (_previewPair.invalid != null)
                {
                    foreach (KeyValuePair<string, int> keyValuePair in _previewPair.invalid)
                    {
                        string text = "- " + keyValuePair.Key + (keyValuePair.Value > 1 ? " (x" + keyValuePair.Value.ToString() + ")" : "");
                        _fancyFont.Draw(text, p1_6.x + 4f, p1_6.y + 4f + num3, Color.White, depth + 8);
                        if (_fancyFont.GetWidth(text) > 160f)
                            num3 += 12;
                        num3 += 12;
                    }
                }
                Vec2 p2_6 = p1_6 + new Vec2(166f, 6 + num3);
                Graphics.DrawRect(p1_6, p2_6, new Color(70, 70, 70), depth, false);
                Graphics.DrawRect(p1_6, p2_6, new Color(30, 30, 30), depth - 8);
            }
        }
    }
}
