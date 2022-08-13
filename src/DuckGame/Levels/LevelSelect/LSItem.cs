// Decompiled with JetBrains decompiler
// Type: DuckGame.LSItem
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => _selected;
            set => _selected = value;
        }

        public bool enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public bool partiallyEnabled
        {
            get => _partiallyEnabled;
            set => _partiallyEnabled = value;
        }

        public string path
        {
            get => _path;
            set => _path = value;
        }

        public LSItemType itemType
        {
            get => _itemType;
            set => _itemType = value;
        }

        public bool isFolder => _itemType == LSItemType.Folder || _itemType == LSItemType.MapPack;

        public bool isPlaylist => _itemType == LSItemType.Playlist;

        public List<string> levelsInside => _levelsInside;

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
            _select = select;
            _icons = new SpriteMap("tinyIcons", 8, 8);
            _font = new BitmapFont("biosFont", 8);
            isModPath = pIsModPath;
            isModRoot = pIsModRoot;
            _path = PATH;
            if (_path == "../")
            {
                _name = "../";
                _itemType = LSItemType.UpFolder;
            }
            else
            {
                string extension = Path.GetExtension(_path);
                _itemType = !(extension == ".lev") ? (!(extension == ".play") ? LSItemType.Folder : LSItemType.Playlist) : LSItemType.Level;
                if (isWorkshop)
                    _itemType = LSItemType.Workshop;
                if (pIsMapPack)
                    _itemType = LSItemType.MapPack;
                _name = Path.GetFileNameWithoutExtension(_path);
                string str1 = _path.Replace('\\', '/');
                if (isWorkshop)
                {
                    _path = "@WORKSHOP@";
                    _levelsInside = LSItem.GetLevelsInside(_select, "@WORKSHOP@");
                }
                else if (str1 == "@VANILLA@")
                {
                    _path = "@VANILLA@";
                    _levelsInside = LSItem.GetLevelsInside(_select, "@VANILLA@");
                }
                else
                {
                    if (!isFolder && !isPlaylist)
                        str1 = str1.Substring(0, str1.Length - 4);
                    string str2 = str1.Substring(str1.LastIndexOf("/levels/", StringComparison.InvariantCultureIgnoreCase) + 8);
                    if (isFolder || isPlaylist)
                    {
                        _levelsInside = LSItem.GetLevelsInside(_select, _path);
                        if (!isModPath)
                            _path = "/" + str2;
                    }
                    else
                        _path = str1 + ".lev";
                }
                bool flag1 = false;
                bool flag2 = true;
                foreach (string str3 in _levelsInside)
                {
                    if (Editor.activatedLevels.Contains(str3))
                        flag1 = true;
                    else
                        flag2 = false;
                }
                enabled = flag1;
                _partiallyEnabled = flag1 && !flag2;
                data = DuckFile.LoadLevelHeaderCached(_path);
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
            if (_itemType == LSItemType.UpFolder || isFolder || _itemType == LSItemType.Playlist || _itemType == LSItemType.Workshop || _itemType == LSItemType.Vanilla)
                return;
            enabled = Editor.activatedLevels.Contains(path);
        }

        public override void Draw()
        {
            float x = this.x;
            if (_selected)
            {
                _icons.frame = 3;
                Graphics.Draw(_icons, x - 8f, y);
            }
            string text = _name;
            if (text.Length > 15)
                text = text.Substring(0, 14) + ".";
            if (_itemType != LSItemType.UpFolder)
            {
                _icons.frame = _partiallyEnabled ? 4 : (_enabled ? 1 : 0);
                Graphics.Draw(_icons, x, y);
                x += 10f;
            }
            bool flag1 = false;
            bool flag2 = false;
            if (_itemType == LSItemType.Folder || _itemType == LSItemType.UpFolder)
            {
                _icons.frame = 2;
                if (isModRoot)
                {
                    _icons.frame = 6;
                    flag1 = true;
                }
                if (isCloudFolder)
                {
                    _icons.frame = 7;
                    flag1 = true;
                }
                Graphics.Draw(_icons, x, y);
                x += 10f;
            }
            if (_itemType == LSItemType.Playlist)
            {
                _icons.frame = 5;
                Graphics.Draw(_icons, x, y);
                x += 10f;
                flag1 = true;
            }
            if (_itemType == LSItemType.Workshop)
            {
                if (_steamIcon == null)
                    _steamIcon = new Sprite("steamIcon");
                _steamIcon.scale = new Vec2(0.25f, 0.25f);
                Graphics.Draw(_steamIcon, x, y);
                x += 10f;
                text = "Workshop";
            }
            if (_itemType == LSItemType.Vanilla)
            {
                text = "@VANILLAICON@Vanilla";
                flag2 = true;
            }
            if (_itemType == LSItemType.MapPack)
            {
                Graphics.Draw(_customIcon, x, y);
                x += 10f;
                flag1 = true;
            }
            if (data != null && data.metaData.eightPlayer)
                text = "|DGPURPLE|(8)|PREV|" + text;
            if (text.EndsWith("_8"))
                text = text.Substring(0, text.Length - 2);
            if (flag2)
                _font.Draw(text, x, y, _selected ? Colors.DGVanilla : Colors.DGVanilla * 0.75f, (Depth)0.8f);
            else if (flag1)
                _font.Draw(text, x, y, _selected ? Colors.DGBlue : Colors.DGBlue * 0.75f, (Depth)0.8f);
            else
                _font.Draw(text, x, y, _selected ? Color.White : Color.Gray, (Depth)0.8f);
        }
    }
}
