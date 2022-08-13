// Decompiled with JetBrains decompiler
// Type: DuckGame.RecorderFrameStateChange
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public struct RecorderFrameStateChange
    {
        public SpriteSortMode sortMode;
        public BlendState blendState;
        public SamplerState samplerState;
        public DepthStencilState depthStencilState;
        public RasterizerState rasterizerState;
        public short effectIndex;
        public Matrix camera;
        public int stateIndex;
        public Rectangle scissor;
    }
}
