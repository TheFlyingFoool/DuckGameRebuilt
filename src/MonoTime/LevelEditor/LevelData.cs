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

        public void RerouteMetadata(LevelMetaData data) => _rerouteMetadata = data;

        public void SetPath(string path) => _path = path;

        public string GetPath() => _path;

        public void SetLocation(LevelLocation loc) => _location = loc;

        public LevelLocation GetLocation() => _location;

        public LevelMetaData metaData => _rerouteMetadata == null ? GetChunk<LevelMetaData>(nameof(metaData)) : _rerouteMetadata;

        public LevelCustomData customData => GetChunk<LevelCustomData>(nameof(customData));

        public WorkshopMetaData workshopData => GetChunk<WorkshopMetaData>(nameof(workshopData), false, true);

        public ModMetaData modData => GetChunk<ModMetaData>(nameof(modData), false, true);

        public WorkshopItem GetPublishingWorkshopItem() => _publishingWorkshopItem;

        public void SetPublishingWorkshopItem(WorkshopItem pItem) => _publishingWorkshopItem = pItem;

        public ProceduralChunkData proceduralData => GetChunk<ProceduralChunkData>(nameof(proceduralData));

        public PreviewData previewData => GetChunk<PreviewData>(nameof(previewData));

        public LevelObjects objects => GetChunk<LevelObjects>(nameof(objects));

        public LevelData Clone()
        {
            BinaryClassChunk.fullDeserializeMode = true;
            LevelData levelData = DuckFile.LoadLevel(Serialize().buffer);
            BinaryClassChunk.fullDeserializeMode = false;
            levelData._path = _path;
            levelData._location = _location;
            return levelData;
        }
    }
}
