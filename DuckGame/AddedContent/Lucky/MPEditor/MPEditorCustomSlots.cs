namespace DuckGame
{
    public class MPEditorCustomSlots
    {
        public const int kSlots = 10;
        public const int kPollInterval = 20;

        public static CustomType[] _types =
        {
            CustomType.Block, CustomType.Block, CustomType.Block,
            CustomType.Platform, CustomType.Platform, CustomType.Platform,
            CustomType.Background, CustomType.Background, CustomType.Background,
            CustomType.Parallax
        };
        public static int[] _indexes = { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0 };

        public MultiplayerEditor editor;
        public string[] lastKey = new string[kSlots];
        public string[] lastIdentity = new string[kSlots];
        public uint[] version = new uint[kSlots];
        public byte[] writer = new byte[kSlots];
        public int frame;

        public MPEditorCustomSlots(MultiplayerEditor pEditor) => editor = pEditor;

        public static string Key(int pSlot) => Custom.data[_types[pSlot]][_indexes[pSlot]] ?? "";

        public static string Identity(int pSlot)
        {
            if (Key(pSlot) == "")
                return "";
            CustomTileData data = Custom.GetData(_indexes[pSlot], _types[pSlot]);
            return data == null || data.texture == null ? "" : data.IdentifierString();
        }

        private static CustomTileDataChunk LevelChunk(LevelData pData, int pSlot)
        {
            LevelCustomData custom = pData.customData;
            switch (pSlot)
            {
                case 0: return custom.customTileset01Data;
                case 1: return custom.customTileset02Data;
                case 2: return custom.customTileset03Data;
                case 3: return custom.customPlatform01Data;
                case 4: return custom.customPlatform02Data;
                case 5: return custom.customPlatform03Data;
                case 6: return custom.customBackground01Data;
                case 7: return custom.customBackground02Data;
                case 8: return custom.customBackground03Data;
                default: return custom.customParallaxData;
            }
        }

        private static void StoreInLevelData(int pSlot, CustomTileDataChunk pTile)
        {
            if (Editor._currentLevelData == null)
                return;
            CustomTileDataChunk chunk = LevelChunk(Editor._currentLevelData, pSlot);
            if (pTile == null)
            {
                chunk.path = null;
                chunk.textureData = null;
                chunk.ignore = true;
                return;
            }
            chunk.path = pTile.path;
            chunk.textureData = pTile.textureData;
            chunk.verticalWidthThick = pTile.verticalWidthThick;
            chunk.verticalWidth = pTile.verticalWidth;
            chunk.horizontalHeight = pTile.horizontalHeight;
            chunk.leftNubber = pTile.leftNubber;
            chunk.rightNubber = pTile.rightNubber;
            chunk.textureChecksum = pTile.textureChecksum;
            chunk.ignore = false;
        }

        public void Rebaseline()
        {
            for (int slot = 0; slot < kSlots; ++slot)
            {
                lastKey[slot] = Key(slot);
                lastIdentity[slot] = Identity(slot);
            }
        }

        public void Update()
        {
            frame++;
            if (frame % kPollInterval != 0)
                return;
            SyncNow();
        }

        public void SyncNow()
        {
            for (int slot = 0; slot < kSlots; ++slot)
            {
                string key = Key(slot);
                if (key == lastKey[slot])
                    continue;
                lastKey[slot] = key;
                string identity = Identity(slot);
                if (identity == lastIdentity[slot])
                    continue;
                lastIdentity[slot] = identity;
                CustomTileDataChunk tile = null;
                if (identity != "")
                {
                    tile = new CustomTileDataChunk();
                    Custom.GetData(_indexes[slot], _types[slot]).ApplyToChunk(tile);
                }
                StoreInLevelData(slot, tile);
                ++version[slot];
                writer[slot] = MPEditorSync.localWriter;
                MPEditorNet.SendCustomTile((byte)slot, version[slot], writer[slot], tile);
            }
        }

        public void Apply(NMMPEditorCustomTile pMessage)
        {
            int slot = pMessage.slot;
            if (slot >= kSlots)
                return;
            if (!(pMessage.version > version[slot] || pMessage.version == version[slot] && pMessage.writer > writer[slot]))
                return;
            version[slot] = pMessage.version;
            writer[slot] = pMessage.writer;
            if (pMessage.tile != null && pMessage.tile.textureData != null)
                Custom.ApplyCustomData(pMessage.tile.GetTileData(), _indexes[slot], _types[slot]);
            else
                Custom.data[_types[slot]][_indexes[slot]] = "";
            StoreInLevelData(slot, pMessage.tile);
            lastKey[slot] = Key(slot);
            lastIdentity[slot] = Identity(slot);
            
            foreach (Thing thing in editor.syncedThings)
                thing.EditorUpdate();
        }
    }
}
