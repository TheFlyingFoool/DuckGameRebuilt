using System.Collections.Generic;

namespace DuckGame
{
    public class LevelCustomData : BinaryClassChunk
    {
        public List<string> scriptPackages = new List<string>();

        public CustomTileDataChunk customTileset01Data => GetChunk<CustomTileDataChunk>(nameof(customTileset01Data));

        public CustomTileDataChunk customTileset02Data => GetChunk<CustomTileDataChunk>(nameof(customTileset02Data));

        public CustomTileDataChunk customTileset03Data => GetChunk<CustomTileDataChunk>(nameof(customTileset03Data));

        public CustomTileDataChunk customBackground01Data => GetChunk<CustomTileDataChunk>(nameof(customBackground01Data));

        public CustomTileDataChunk customBackground02Data => GetChunk<CustomTileDataChunk>(nameof(customBackground02Data));

        public CustomTileDataChunk customBackground03Data => GetChunk<CustomTileDataChunk>(nameof(customBackground03Data));

        public CustomTileDataChunk customPlatform01Data => GetChunk<CustomTileDataChunk>(nameof(customPlatform01Data));

        public CustomTileDataChunk customPlatform02Data => GetChunk<CustomTileDataChunk>(nameof(customPlatform02Data));

        public CustomTileDataChunk customPlatform03Data => GetChunk<CustomTileDataChunk>(nameof(customPlatform03Data));

        public CustomTileDataChunk customParallaxData => GetChunk<CustomTileDataChunk>(nameof(customParallaxData));
    }
}
