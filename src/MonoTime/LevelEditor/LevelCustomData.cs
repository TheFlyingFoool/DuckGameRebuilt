// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelCustomData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class LevelCustomData : BinaryClassChunk
    {
        public List<string> scriptPackages = new List<string>();

        public CustomTileDataChunk customTileset01Data => this.GetChunk<CustomTileDataChunk>(nameof(customTileset01Data));

        public CustomTileDataChunk customTileset02Data => this.GetChunk<CustomTileDataChunk>(nameof(customTileset02Data));

        public CustomTileDataChunk customTileset03Data => this.GetChunk<CustomTileDataChunk>(nameof(customTileset03Data));

        public CustomTileDataChunk customBackground01Data => this.GetChunk<CustomTileDataChunk>(nameof(customBackground01Data));

        public CustomTileDataChunk customBackground02Data => this.GetChunk<CustomTileDataChunk>(nameof(customBackground02Data));

        public CustomTileDataChunk customBackground03Data => this.GetChunk<CustomTileDataChunk>(nameof(customBackground03Data));

        public CustomTileDataChunk customPlatform01Data => this.GetChunk<CustomTileDataChunk>(nameof(customPlatform01Data));

        public CustomTileDataChunk customPlatform02Data => this.GetChunk<CustomTileDataChunk>(nameof(customPlatform02Data));

        public CustomTileDataChunk customPlatform03Data => this.GetChunk<CustomTileDataChunk>(nameof(customPlatform03Data));

        public CustomTileDataChunk customParallaxData => this.GetChunk<CustomTileDataChunk>(nameof(customParallaxData));
    }
}
