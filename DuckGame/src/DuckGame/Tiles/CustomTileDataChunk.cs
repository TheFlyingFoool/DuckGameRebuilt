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
            if (textureData == null)
                return tileData;
            tileData.path = path;
            tileData.texture = Editor.StringToTexture(textureData);
            tileData.verticalWidthThick = verticalWidthThick;
            tileData.verticalWidth = verticalWidth;
            tileData.horizontalHeight = horizontalHeight;
            tileData.leftNubber = leftNubber;
            tileData.rightNubber = rightNubber;
            tileData.checksum = textureChecksum;
            return tileData;
        }
    }
}
