using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class EditorWorkshopItem
    {
        public bool deathmatchTestSuccess;
        public bool challengeTestSuccess;
        private Tex2D _preview;
        private WorkshopItem _item;
        private LevelData _level;
        private Mod _mod;
        private List<EditorWorkshopItem> _subItems = new List<EditorWorkshopItem>();
        private EditorWorkshopItem _parent;

        public Tex2D preview
        {
            get
            {
                if (_preview == null)
                {
                    if (_mod != null)
                    {
                        string pathToScreenshot = _mod.generateAndGetPathToScreenshot;
                        if (!File.Exists(pathToScreenshot))
                            return null;
                        using (FileStream fileStream = File.Open(pathToScreenshot, FileMode.Open))
                            _preview = (Tex2D)Texture2D.FromStream(Graphics.device, fileStream);
                    }
                    else
                    {
                        RenderTarget2D pCustomPreviewTarget;
                        if (_level.metaData.type == LevelType.Arcade_Machine)
                        {
                            pCustomPreviewTarget = new RenderTarget2D(512, 512);
                            Content.customPreviewWidth = 128;
                            Content.customPreviewHeight = 128;
                            Content.customPreviewCenter = (Level.current as Editor).levelThings[0].position;
                        }
                        else
                            pCustomPreviewTarget = new RenderTarget2D(1280, 720);
                        Content.GeneratePreview(_level, true, pCustomPreviewTarget);
                        Content.customPreviewWidth = 0;
                        Content.customPreviewHeight = 0;
                        Content.customPreviewCenter = Vec2.Zero;
                        _preview = (Tex2D)new Texture2D(Graphics.device, pCustomPreviewTarget.width, pCustomPreviewTarget.height);
                        Color[] colorArray = new Color[pCustomPreviewTarget.width * pCustomPreviewTarget.height];
                        pCustomPreviewTarget.GetData(colorArray);
                        _preview.SetData<Color>(colorArray);
                    }
                }
                return _preview;
            }
        }

        public IEnumerable<EditorWorkshopItem> subItems => _subItems;

        public int subIndex => _parent == null ? -1 : _parent._subItems.IndexOf(this);

        public EditorWorkshopItem parent => _parent;

        public LevelType levelType => _level.metaData.type;

        public LevelSize levelSize => _level.metaData.size;

        public IEnumerable<string> tags => workshopData.tags;

        public void AddTag(string pTag)
        {
            if (workshopData.tags.Contains(pTag))
                return;
            workshopData.tags.Add(pTag);
        }

        public void RemoveTag(string pTag) => workshopData.tags.Remove(pTag);

        private WorkshopMetaData workshopData => _mod == null ? _level.workshopData : _mod.workshopData;

        public string name
        {
            get => workshopData.name;
            set => workshopData.name = value;
        }

        public string description
        {
            get => workshopData.description;
            set => workshopData.description = value;
        }

        public SteamResult result => _item.result;

        public bool finishedProcessing => _item.finishedProcessing;

        public WorkshopItem item => _item;

        public EditorWorkshopItem(LevelData pLevel, EditorWorkshopItem pParent = null)
        {
            _parent = pParent;
            _level = pLevel;
            if (_level.metaData.workshopID != 0UL)
            {
                _item = WorkshopItem.GetItem(_level.metaData.workshopID);
                Steam.RequestWorkshopInfo(new List<WorkshopItem>()
        {
          _item
        });
                Wait();
                _level.workshopData.name = _item.data.name;
                _level.workshopData.description = _item.data.description;
                _level.workshopData.tags = new List<string>(_item.data.tags);
            }
            if (_level.workshopData.name == "")
                _level.workshopData.name = Path.GetFileNameWithoutExtension(_level.GetPath());
            if (_level.metaData.type != LevelType.Arcade_Machine)
                return;
            if (((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge01Data != null)
                _subItems.Add(new EditorWorkshopItem(((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge01Data, this));
            if (((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge02Data != null)
                _subItems.Add(new EditorWorkshopItem(((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge02Data, this));
            if (((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge03Data == null)
                return;
            _subItems.Add(new EditorWorkshopItem(((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge03Data, this));
        }

        public EditorWorkshopItem(Mod pMod, EditorWorkshopItem pParent = null)
        {
            _parent = pParent;
            _mod = pMod;
            if (_mod.configuration.workshopID != 0UL)
            {
                _item = WorkshopItem.GetItem(_mod.configuration.workshopID);
                Steam.RequestWorkshopInfo(new List<WorkshopItem>()
        {
          _item
        });
                Wait();
                _mod.workshopData.name = _item.data.name;
                _mod.workshopData.description = _item.data.description;
                _mod.workshopData.tags = new List<string>(_item.data.tags);
            }
            _mod.workshopData.name = _mod.configuration.displayName;
            if (workshopData.tags.Contains("Mod"))
                return;
            AddTag("Mod");
        }

        public SteamResult PrepareItem()
        {
            if (_item == null)
            {
                _item = Steam.CreateItem();
                Wait();
                _level.metaData.workshopID = _item.id;
                _item.SetDetails(workshopData.name, new WorkshopItemData());
                if (_parent != null && _parent._level.metaData.type == LevelType.Arcade_Machine)
                {
                    _level.workshopData.name = _parent._item.name + " Sub Challenge " + subIndex.ToString();
                    _level.workshopData.description = "One of the challenges in the \"" + _parent._item.name + "\" Arcade Machine.";
                }
            }
            if (result != SteamResult.OK)
                return result;
            _item.data.name = workshopData.name;
            _item.data.description = workshopData.description;
            workshopData.tags.RemoveAll(x => !SteamUploadDialog.possibleTags.Contains(x));
            if (_level.metaData.type != LevelType.Arcade_Machine)
            {
                AddTag("Map");
                AddTag(_level.metaData.size.ToString());
            }
            if (_level.metaData.type != LevelType.Deathmatch)
                AddTag(_level.metaData.type.ToString().Replace("_", " "));
            if (deathmatchTestSuccess)
                AddTag("Deathmatch");
            if (_level.metaData.eightPlayer)
                AddTag("EightPlayer");
            if (_level.metaData.eightPlayerRestricted)
                AddTag("EightPlayerOnly");
            else if (_level.metaData.type == LevelType.Arcade_Machine)
            {
                if (_subItems.Count == 3)
                {
                    bool flag = true;
                    foreach (EditorWorkshopItem subItem in _subItems)
                    {
                        if (!subItem.challengeTestSuccess)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        AddTag("Tested Machine");
                }
            }
            else if (_level.metaData.type == LevelType.Challenge && challengeTestSuccess)
                AddTag("Tested Challenge");
            else if (_level.metaData.type == LevelType.Deathmatch)
                AddTag("Strange");
            if ((Level.current as Editor).levelThings.Exists(x => x is CustomCamera))
                AddTag("Fixed Camera");
            if (_level.metaData.hasCustomArt)
                AddTag("Custom Art");
            _item.data.tags = new List<string>(workshopData.tags);
            foreach (ulong dependency in _level.workshopData.dependencies)
                Steam.WorkshopRemoveDependency(_item, WorkshopItem.GetItem(dependency));
            _level.workshopData.dependencies.Clear();
            foreach (EditorWorkshopItem subItem in subItems)
            {
                if (subItem.PrepareItem() != SteamResult.OK)
                    return subItem.result;
                _level.workshopData.dependencies.Add(subItem.item.id);
                Steam.WorkshopAddDependency(_item, subItem.item);
            }
            CopyFiles();
            return SteamResult.OK;
        }

        private void CopyFiles()
        {
            DuckFile.SaveChunk(_level, _level.GetPath());
            string pathString1 = DuckFile.workshopDirectory + _level.metaData.workshopID.ToString() + "/";
            string pathString2 = DuckFile.workshopDirectory + _level.metaData.workshopID.ToString() + "-preview/";
            DuckFile.CreatePath(pathString1);
            DuckFile.CreatePath(pathString2);
            string withoutExtension = Path.GetFileNameWithoutExtension(_level.GetPath());
            string str = pathString1 + Path.GetFileName(_level.GetPath());
            if (File.Exists(str))
                File.Delete(str);
            File.Copy(_level.GetPath(), str);
            File.SetAttributes(_level.GetPath(), FileAttributes.Normal);
            _item.data.contentFolder = pathString1;
            string path = pathString2 + withoutExtension + ".png";
            if (File.Exists(path))
                File.Delete(path);
            Stream stream = DuckFile.Create(path);
            ((Texture2D)preview.nativeObject).SaveAsPng(stream, preview.width, preview.height);
            stream.Dispose();
            _item.data.previewPath = path;
        }

        public void Upload()
        {
            _item.ResetProcessing();
            _item.ApplyWorkshopData(_item.data);
        }

        public void FinishUpload()
        {
            if (!_item.needsLegal)
                return;
            Steam.ShowWorkshopLegalAgreement(_item.id.ToString());
        }

        private void Wait()
        {
            while (!_item.finishedProcessing)
                Steam.Update();
        }
    }
}
