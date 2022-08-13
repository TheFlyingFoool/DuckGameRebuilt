// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomTileDataChunk
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
