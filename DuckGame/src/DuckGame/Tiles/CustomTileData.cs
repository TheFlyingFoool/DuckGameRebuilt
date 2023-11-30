using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class CustomTileData
    {
        public string path;
        public Texture2D texture;
        public int verticalWidthThick;
        public int verticalWidth;
        public int horizontalHeight;
        public bool leftNubber;
        public bool rightNubber;
        public uint checksum;

        public string IdentifierString()
        {
            if (checksum == 0U)
                CalculateChecksum();
            return path + "@" + checksum.ToString();
        }

        public void ApplyToChunk(CustomTileDataChunk chunk)
        {
            chunk.path = path;
            chunk.textureData = Editor.TextureToString(texture);
            chunk.verticalWidthThick = verticalWidthThick;
            chunk.verticalWidth = verticalWidth;
            chunk.horizontalHeight = horizontalHeight;
            chunk.leftNubber = leftNubber;
            chunk.rightNubber = rightNubber;
            if (checksum == 0U)
                CalculateChecksum();
            chunk.textureChecksum = checksum;
        }

        public void CalculateChecksum(string texString = "")
        {
            if (texture != null && texString == "")
                texString = Editor.TextureToString(texture);
            checksum = CRC32.Generate(texString);
        }
    }
}
