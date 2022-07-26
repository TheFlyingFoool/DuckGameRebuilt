// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorWorkshopItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
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
                if (this._preview == null)
                {
                    if (this._mod != null)
                    {
                        string pathToScreenshot = this._mod.generateAndGetPathToScreenshot;
                        if (!System.IO.File.Exists(pathToScreenshot))
                            return (Tex2D)null;
                        using (FileStream fileStream = System.IO.File.Open(pathToScreenshot, FileMode.Open))
                            this._preview = (Tex2D)Texture2D.FromStream(DuckGame.Graphics.device, (Stream)fileStream);
                    }
                    else
                    {
                        RenderTarget2D pCustomPreviewTarget;
                        if (this._level.metaData.type == LevelType.Arcade_Machine)
                        {
                            pCustomPreviewTarget = new RenderTarget2D(512, 512);
                            Content.customPreviewWidth = 128;
                            Content.customPreviewHeight = 128;
                            Content.customPreviewCenter = (Level.current as Editor).levelThings[0].position;
                        }
                        else
                            pCustomPreviewTarget = new RenderTarget2D(1280, 720);
                        Content.GeneratePreview(this._level, true, pCustomPreviewTarget);
                        Content.customPreviewWidth = 0;
                        Content.customPreviewHeight = 0;
                        Content.customPreviewCenter = Vec2.Zero;
                        this._preview = (Tex2D)new Texture2D(DuckGame.Graphics.device, pCustomPreviewTarget.width, pCustomPreviewTarget.height);
                        Color[] colorArray = new Color[pCustomPreviewTarget.width * pCustomPreviewTarget.height];
                        pCustomPreviewTarget.GetData<Color>(colorArray);
                        this._preview.SetData<Color>(colorArray);
                    }
                }
                return this._preview;
            }
        }

        public IEnumerable<EditorWorkshopItem> subItems => (IEnumerable<EditorWorkshopItem>)this._subItems;

        public int subIndex => this._parent == null ? -1 : this._parent._subItems.IndexOf(this);

        public EditorWorkshopItem parent => this._parent;

        public LevelType levelType => this._level.metaData.type;

        public LevelSize levelSize => this._level.metaData.size;

        public IEnumerable<string> tags => (IEnumerable<string>)this.workshopData.tags;

        public void AddTag(string pTag)
        {
            if (this.workshopData.tags.Contains(pTag))
                return;
            this.workshopData.tags.Add(pTag);
        }

        public void RemoveTag(string pTag) => this.workshopData.tags.Remove(pTag);

        private WorkshopMetaData workshopData => this._mod == null ? this._level.workshopData : this._mod.workshopData;

        public string name
        {
            get => this.workshopData.name;
            set => this.workshopData.name = value;
        }

        public string description
        {
            get => this.workshopData.description;
            set => this.workshopData.description = value;
        }

        public SteamResult result => this._item.result;

        public bool finishedProcessing => this._item.finishedProcessing;

        public WorkshopItem item => this._item;

        public EditorWorkshopItem(LevelData pLevel, EditorWorkshopItem pParent = null)
        {
            this._parent = pParent;
            this._level = pLevel;
            if (this._level.metaData.workshopID != 0UL)
            {
                this._item = WorkshopItem.GetItem(this._level.metaData.workshopID);
                Steam.RequestWorkshopInfo(new List<WorkshopItem>()
        {
          this._item
        });
                this.Wait();
                this._level.workshopData.name = this._item.data.name;
                this._level.workshopData.description = this._item.data.description;
                this._level.workshopData.tags = new List<string>((IEnumerable<string>)this._item.data.tags);
            }
            if (this._level.workshopData.name == "")
                this._level.workshopData.name = Path.GetFileNameWithoutExtension(this._level.GetPath());
            if (this._level.metaData.type != LevelType.Arcade_Machine)
                return;
            if (((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge01Data != null)
                this._subItems.Add(new EditorWorkshopItem(((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge01Data, this));
            if (((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge02Data != null)
                this._subItems.Add(new EditorWorkshopItem(((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge02Data, this));
            if (((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge03Data == null)
                return;
            this._subItems.Add(new EditorWorkshopItem(((Level.current as Editor).levelThings[0] as ArcadeMachine).challenge03Data, this));
        }

        public EditorWorkshopItem(Mod pMod, EditorWorkshopItem pParent = null)
        {
            this._parent = pParent;
            this._mod = pMod;
            if (this._mod.configuration.workshopID != 0UL)
            {
                this._item = WorkshopItem.GetItem(this._mod.configuration.workshopID);
                Steam.RequestWorkshopInfo(new List<WorkshopItem>()
        {
          this._item
        });
                this.Wait();
                this._mod.workshopData.name = this._item.data.name;
                this._mod.workshopData.description = this._item.data.description;
                this._mod.workshopData.tags = new List<string>((IEnumerable<string>)this._item.data.tags);
            }
            this._mod.workshopData.name = this._mod.configuration.displayName;
            if (this.workshopData.tags.Contains("Mod"))
                return;
            this.AddTag("Mod");
        }

        public SteamResult PrepareItem()
        {
            if (this._item == null)
            {
                this._item = Steam.CreateItem();
                this.Wait();
                this._level.metaData.workshopID = this._item.id;
                this._item.SetDetails(this.workshopData.name, new WorkshopItemData());
                if (this._parent != null && this._parent._level.metaData.type == LevelType.Arcade_Machine)
                {
                    this._level.workshopData.name = this._parent._item.name + " Sub Challenge " + this.subIndex.ToString();
                    this._level.workshopData.description = "One of the challenges in the \"" + this._parent._item.name + "\" Arcade Machine.";
                }
            }
            if (this.result != SteamResult.OK)
                return this.result;
            this._item.data.name = this.workshopData.name;
            this._item.data.description = this.workshopData.description;
            this.workshopData.tags.RemoveAll((Predicate<string>)(x => !SteamUploadDialog.possibleTags.Contains(x)));
            if (this._level.metaData.type != LevelType.Arcade_Machine)
            {
                this.AddTag("Map");
                this.AddTag(this._level.metaData.size.ToString());
            }
            if (this._level.metaData.type != LevelType.Deathmatch)
                this.AddTag(this._level.metaData.type.ToString().Replace("_", " "));
            if (this.deathmatchTestSuccess)
                this.AddTag("Deathmatch");
            if (this._level.metaData.eightPlayer)
                this.AddTag("EightPlayer");
            if (this._level.metaData.eightPlayerRestricted)
                this.AddTag("EightPlayerOnly");
            else if (this._level.metaData.type == LevelType.Arcade_Machine)
            {
                if (this._subItems.Count == 3)
                {
                    bool flag = true;
                    foreach (EditorWorkshopItem subItem in this._subItems)
                    {
                        if (!subItem.challengeTestSuccess)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        this.AddTag("Tested Machine");
                }
            }
            else if (this._level.metaData.type == LevelType.Challenge && this.challengeTestSuccess)
                this.AddTag("Tested Challenge");
            else if (this._level.metaData.type == LevelType.Deathmatch)
                this.AddTag("Strange");
            if ((Level.current as Editor).levelThings.Exists((Predicate<Thing>)(x => x is CustomCamera)))
                this.AddTag("Fixed Camera");
            if (this._level.metaData.hasCustomArt)
                this.AddTag("Custom Art");
            this._item.data.tags = new List<string>((IEnumerable<string>)this.workshopData.tags);
            foreach (ulong dependency in this._level.workshopData.dependencies)
                Steam.WorkshopRemoveDependency(this._item, WorkshopItem.GetItem(dependency));
            this._level.workshopData.dependencies.Clear();
            foreach (EditorWorkshopItem subItem in this.subItems)
            {
                if (subItem.PrepareItem() != SteamResult.OK)
                    return subItem.result;
                this._level.workshopData.dependencies.Add(subItem.item.id);
                Steam.WorkshopAddDependency(this._item, subItem.item);
            }
            this.CopyFiles();
            return SteamResult.OK;
        }

        private void CopyFiles()
        {
            DuckFile.SaveChunk((BinaryClassChunk)this._level, this._level.GetPath());
            string pathString1 = DuckFile.workshopDirectory + this._level.metaData.workshopID.ToString() + "/";
            string pathString2 = DuckFile.workshopDirectory + this._level.metaData.workshopID.ToString() + "-preview/";
            DuckFile.CreatePath(pathString1);
            DuckFile.CreatePath(pathString2);
            string withoutExtension = Path.GetFileNameWithoutExtension(this._level.GetPath());
            string str = pathString1 + Path.GetFileName(this._level.GetPath());
            if (System.IO.File.Exists(str))
                System.IO.File.Delete(str);
            System.IO.File.Copy(this._level.GetPath(), str);
            System.IO.File.SetAttributes(this._level.GetPath(), FileAttributes.Normal);
            this._item.data.contentFolder = pathString1;
            string path = pathString2 + withoutExtension + ".png";
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            Stream stream = (Stream)DuckFile.Create(path);
            ((Texture2D)this.preview.nativeObject).SaveAsPng(stream, this.preview.width, this.preview.height);
            stream.Dispose();
            this._item.data.previewPath = path;
        }

        public void Upload()
        {
            this._item.ResetProcessing();
            this._item.ApplyWorkshopData(this._item.data);
        }

        public void FinishUpload()
        {
            if (!this._item.needsLegal)
                return;
            Steam.ShowWorkshopLegalAgreement(this._item.id.ToString());
        }

        private void Wait()
        {
            while (!this._item.finishedProcessing)
                Steam.Update();
        }
    }
}
