// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [ChunkVersion(2)]
    [MagicNumber(5033950674723417)]
    public class LevelData : BinaryClassChunk
    {
        private LevelMetaData _rerouteMetadata;
        private string _path;
        private LevelLocation _location;
        private WorkshopItem _publishingWorkshopItem;

        public void RerouteMetadata(LevelMetaData data) => this._rerouteMetadata = data;

        public void SetPath(string path) => this._path = path;

        public string GetPath() => this._path;

        public void SetLocation(LevelLocation loc) => this._location = loc;

        public LevelLocation GetLocation() => this._location;

        public LevelMetaData metaData => this._rerouteMetadata == null ? this.GetChunk<LevelMetaData>(nameof(metaData)) : this._rerouteMetadata;

        public LevelCustomData customData => this.GetChunk<LevelCustomData>(nameof(customData));

        public WorkshopMetaData workshopData => this.GetChunk<WorkshopMetaData>(nameof(workshopData), false, true);

        public ModMetaData modData => this.GetChunk<ModMetaData>(nameof(modData), false, true);

        public WorkshopItem GetPublishingWorkshopItem() => this._publishingWorkshopItem;

        public void SetPublishingWorkshopItem(WorkshopItem pItem) => this._publishingWorkshopItem = pItem;

        public ProceduralChunkData proceduralData => this.GetChunk<ProceduralChunkData>(nameof(proceduralData));

        public PreviewData previewData => this.GetChunk<PreviewData>(nameof(previewData));

        public LevelObjects objects => this.GetChunk<LevelObjects>(nameof(objects));

        public LevelData Clone()
        {
            BinaryClassChunk.fullDeserializeMode = true;
            LevelData levelData = DuckFile.LoadLevel(this.Serialize().buffer);
            BinaryClassChunk.fullDeserializeMode = false;
            levelData._path = this._path;
            levelData._location = this._location;
            return levelData;
        }
    }
}
