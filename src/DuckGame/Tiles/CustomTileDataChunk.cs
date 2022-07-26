// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomTileDataChunk
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CustomTileDataChunk : BinaryClassChunk
    {
        public string path;
        public string textureData;
        public int verticalWidthThick;
        public int verticalWidth;
        public int horizontalHeight;
        public bool leftNubber;
        public bool rightNubber;
        public uint textureChecksum;

        public CustomTileData GetTileData()
        {
            CustomTileData tileData = new CustomTileData();
            if (this.textureData == null)
                return tileData;
            tileData.path = this.path;
            tileData.texture = Editor.StringToTexture(this.textureData);
            tileData.verticalWidthThick = this.verticalWidthThick;
            tileData.verticalWidth = this.verticalWidth;
            tileData.horizontalHeight = this.horizontalHeight;
            tileData.leftNubber = this.leftNubber;
            tileData.rightNubber = this.rightNubber;
            tileData.checksum = this.textureChecksum;
            return tileData;
        }
    }
}
