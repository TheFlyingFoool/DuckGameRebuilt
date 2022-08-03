// Decompiled with JetBrains decompiler
// Type: DuckGame.RecorderFrameItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public struct RecorderFrameItem
    {
        public short texture;
        public Vec2 topLeft;
        public Vec2 bottomRight;
        public float rotation;
        public Color color;
        public short texX;
        public short texY;
        public short texW;
        public short texH;
        public float depth;

        public void SetData(
          short textureVal,
          Vec2 topLeftVal,
          Vec2 bottomRightVal,
          float rotationVal,
          Color colorVal,
          short texXVal,
          short texYVal,
          short texWVal,
          short texHVal,
          float depthVal)
        {
            texture = textureVal;
            topLeft = topLeftVal;
            bottomRight = bottomRightVal;
            rotation = rotationVal;
            color = colorVal;
            texX = texXVal;
            texY = texYVal;
            texW = texWVal;
            texH = texHVal;
            depth = depthVal;
        }
    }
}
