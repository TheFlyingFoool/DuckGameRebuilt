// Decompiled with JetBrains decompiler
// Type: DuckGame.LSItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class LSItem : Thing
    {
        private bool _selected;
        private bool _enabled;
        private bool _partiallyEnabled;
        private string _path = "";
        public string _name;
        private LSItemType _itemType;
        public Sprite _customIcon;
        private SpriteMap _icons;
        private BitmapFont _font;
        private Sprite _steamIcon;
        private List<string> _levelsInside = new List<string>();
        private LevelSelect _select;
        public bool isModPath;
        public bool isModRoot;
        public bool isCloudFolder;
        public MapPack mapPack;
        public LevelData data;

        public bool selected
        {
            get => this._selected;
            set => this._selected = value;
        }

        public bool enabled
        {
            get => this._enabled;
            set => this._enabled = value;
        }

        public bool partiallyEnabled
        {
            get => this._partiallyEnabled;
            set => this._partiallyEnabled = value;
        }

        public string path
        {
            get => this._path;
            set => this._path = value;
        }

        public LSItemType itemType
        {
            get => this._itemType;
            set => this._itemType = value;
        }

        public bool isFolder => this._itemType == LSItemType.Folder || this._itemType == LSItemType.MapPack;

        public bool isPlaylist => this._itemType == LSItemType.Playlist;

        public List<string> levelsInside => this._levelsInside;

        public LSItem(
          float xpos,
          float ypos,
          LevelSelect select,
          string PATH,
          bool isWorkshop = false,
          bool pIsModPath = false,
          bool pIsModRoot = false,
          bool pIsMapPack = false)
          : base(xpos, ypos)
        {
            this._select = select;
            this._icons = new SpriteMap("tinyIcons", 8, 8);
            this._font = new BitmapFont("biosFont", 8);
            this.isModPath = pIsModPath;
            this.isModRoot = pIsModRoot;
            this._path = PATH;
            if (this._path == "../")
            {
                this._name = "../";
                this._itemType = LSItemType.UpFolder;
            }
            else
            {
                string extension = Path.GetExtension(this._path);
                this._itemType = !(extension == ".lev") ? (!(extension == ".play") ? LSItemType.Folder : LSItemType.Playlist) : LSItemType.Level;
                if (isWorkshop)
                    this._itemType = LSItemType.Workshop;
                if (pIsMapPack)
                    this._itemType = LSItemType.MapPack;
                this._name = Path.GetFileNameWithoutExtension(this._path);
                string str1 = this._path.Replace('\\', '/');
                if (isWorkshop)
                {
                    this._path = "@WORKSHOP@";
                    this._levelsInside = LSItem.GetLevelsInside(this._select, "@WORKSHOP@");
                }
                else if (str1 == "@VANILLA@")
                {
                    this._path = "@VANILLA@";
                    this._levelsInside = LSItem.GetLevelsInside(this._select, "@VANILLA@");
                }
                else
                {
                    if (!this.isFolder && !this.isPlaylist)
                        str1 = str1.Substring(0, str1.Length - 4);
                    string str2 = str1.Substring(str1.LastIndexOf("/levels/", StringComparison.InvariantCultureIgnoreCase) + 8);
                    if (this.isFolder || this.isPlaylist)
                    {
                        this._levelsInside = LSItem.GetLevelsInside(this._select, this._path);
                        if (!this.isModPath)
                            this._path = "/" + str2;
                    }
                    else
                        this._path = str1 + ".lev";
                }
                bool flag1 = false;
                bool flag2 = true;
                foreach (string str3 in this._levelsInside)
                {
                    if (Editor.activatedLevels.Contains(str3))
                        flag1 = true;
                    else
                        flag2 = false;
                }
                this.enabled = flag1;
                this._partiallyEnabled = flag1 && !flag2;
                this.data = DuckFile.LoadLevelHeaderCached(this._path);
            }
        }

        public static List<string> GetLevelsInside(LevelSelect selector, string path)
        {
            List<string> levelsInside = new List<string>();
            if (path == "@WORKSHOP@")
            {
                foreach (WorkshopItem allWorkshopItem in Steam.GetAllWorkshopItems())
                {
                    if ((allWorkshopItem.stateFlags & WorkshopItemState.Installed) != WorkshopItemState.None && allWorkshopItem.path != null && Directory.Exists(allWorkshopItem.path))
                    {
                        foreach (string file in DuckFile.GetFiles(allWorkshopItem.path))
                        {
                            string lName = file;
                            if (lName.EndsWith(".lev") && selector.filters.TrueForAll(a => a.Filter(lName, LevelLocation.Workshop)))
                                levelsInside.Add(lName);
                        }
                    }
                }
            }
            else if (path == "@VANILLA@")
            {
                string path1 = DuckFile.contentDirectory + "Levels/deathmatch/";
                foreach (string directory in DuckFile.GetDirectories(path1))
                    levelsInside.AddRange(LSItem.GetLevelsInside(selector, directory).Where<string>(x => !x.Contains("online") && !x.Contains("holiday")));
                foreach (string file1 in Content.GetFiles(path1))
                {
                    string file = file1;
                    if (!file.Contains("online") && !file.Contains("holiday") && file.EndsWith(".lev") && selector.filters.TrueForAll(a => a.Filter(file)))
                    {
                        string str = file.Replace('\\', '/');
                        levelsInside.Add(str);
                    }
                }
            }
            else if (path.EndsWith(".play"))
            {
                DXMLNode node = DuckFile.LoadDuckXML(path).Element("playlist");
                if (node != null)
                {
                    LevelPlaylist levelPlaylist = new LevelPlaylist();
                    levelPlaylist.Deserialize(node);
                    foreach (string level in levelPlaylist.levels)
                    {
                        string lName = level;
                        if (selector.filters.TrueForAll(a => a.Filter(lName)))
                            levelsInside.Add(lName);
                    }
                }
            }
            else
            {
                foreach (string directory in DuckFile.GetDirectories(path))
                    levelsInside.AddRange(LSItem.GetLevelsInside(selector, directory));
                foreach (string file2 in Content.GetFiles(path))
                {
                    string file = file2;
                    if (file.EndsWith(".lev") && selector.filters.TrueForAll(a => a.Filter(file)))
                    {
                        string str = file.Replace('\\', '/');
                        levelsInside.Add(str);
                    }
                }
            }
            return levelsInside;
        }

        public override void Update()
        {
            if (this._itemType == LSItemType.UpFolder || this.isFolder || this._itemType == LSItemType.Playlist || this._itemType == LSItemType.Workshop || this._itemType == LSItemType.Vanilla)
                return;
            this.enabled = Editor.activatedLevels.Contains(this.path);
        }

        public override void Draw()
        {
            float x = this.x;
            if (this._selected)
            {
                this._icons.frame = 3;
                Graphics.Draw(_icons, x - 8f, this.y);
            }
            string text = this._name;
            if (text.Length > 15)
                text = text.Substring(0, 14) + ".";
            if (this._itemType != LSItemType.UpFolder)
            {
                this._icons.frame = this._partiallyEnabled ? 4 : (this._enabled ? 1 : 0);
                Graphics.Draw(_icons, x, this.y);
                x += 10f;
            }
            bool flag1 = false;
            bool flag2 = false;
            if (this._itemType == LSItemType.Folder || this._itemType == LSItemType.UpFolder)
            {
                this._icons.frame = 2;
                if (this.isModRoot)
                {
                    this._icons.frame = 6;
                    flag1 = true;
                }
                if (this.isCloudFolder)
                {
                    this._icons.frame = 7;
                    flag1 = true;
                }
                Graphics.Draw(_icons, x, this.y);
                x += 10f;
            }
            if (this._itemType == LSItemType.Playlist)
            {
                this._icons.frame = 5;
                Graphics.Draw(_icons, x, this.y);
                x += 10f;
                flag1 = true;
            }
            if (this._itemType == LSItemType.Workshop)
            {
                if (this._steamIcon == null)
                    this._steamIcon = new Sprite("steamIcon");
                this._steamIcon.scale = new Vec2(0.25f, 0.25f);
                Graphics.Draw(this._steamIcon, x, this.y);
                x += 10f;
                text = "Workshop";
            }
            if (this._itemType == LSItemType.Vanilla)
            {
                text = "@VANILLAICON@Vanilla";
                flag2 = true;
            }
            if (this._itemType == LSItemType.MapPack)
            {
                Graphics.Draw(this._customIcon, x, this.y);
                x += 10f;
                flag1 = true;
            }
            if (this.data != null && this.data.metaData.eightPlayer)
                text = "|DGPURPLE|(8)|PREV|" + text;
            if (text.EndsWith("_8"))
                text = text.Substring(0, text.Length - 2);
            if (flag2)
                this._font.Draw(text, x, this.y, this._selected ? Colors.DGVanilla : Colors.DGVanilla * 0.75f, (Depth)0.8f);
            else if (flag1)
                this._font.Draw(text, x, this.y, this._selected ? Colors.DGBlue : Colors.DGBlue * 0.75f, (Depth)0.8f);
            else
                this._font.Draw(text, x, this.y, this._selected ? Color.White : Color.Gray, (Depth)0.8f);
        }
    }
}
