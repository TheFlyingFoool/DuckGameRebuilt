// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomTileData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (this.checksum == 0U)
                this.CalculateChecksum();
            return this.path + "@" + this.checksum.ToString();
        }

        public void ApplyToChunk(CustomTileDataChunk chunk)
        {
            chunk.path = this.path;
            chunk.textureData = Editor.TextureToString(this.texture);
            chunk.verticalWidthThick = this.verticalWidthThick;
            chunk.verticalWidth = this.verticalWidth;
            chunk.horizontalHeight = this.horizontalHeight;
            chunk.leftNubber = this.leftNubber;
            chunk.rightNubber = this.rightNubber;
            if (this.checksum == 0U)
                this.CalculateChecksum();
            chunk.textureChecksum = this.checksum;
        }

        public void CalculateChecksum(string texString = "")
        {
            if (this.texture != null && texString == "")
                texString = Editor.TextureToString(this.texture);
            this.checksum = CRC32.Generate(texString);
        }
    }
}
