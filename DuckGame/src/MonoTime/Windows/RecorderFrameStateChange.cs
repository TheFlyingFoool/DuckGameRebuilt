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
