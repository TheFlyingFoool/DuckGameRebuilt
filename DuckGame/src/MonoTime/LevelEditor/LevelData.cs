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
            fullDeserializeMode = true;
            LevelData levelData = DuckFile.LoadLevel(Serialize().buffer);
            fullDeserializeMode = false;
            levelData._path = _path;
            levelData._location = _location;
            return levelData;
        }
    }
}
