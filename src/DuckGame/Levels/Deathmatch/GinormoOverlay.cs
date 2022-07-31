// Decompiled with JetBrains decompiler
// Type: DuckGame.GinormoOverlay
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class GinormoOverlay : Thing
    {
        private Sprite _targetSprite;
        private Material _screenMaterial;
        private Tex2D _overlaySprite;
        private bool _smallMode;

        public GinormoOverlay(float xpos, float ypos, bool smallMode)
          : base(xpos, ypos)
        {
            this.depth = (Depth)0.9f;
            this.graphic = new Sprite("rockThrow/boardOverlay");
            this._smallMode = smallMode;
        }

        public override void Initialize()
        {
            this._overlaySprite = Content.Load<Tex2D>("rockThrow/boardOverlayLarge");
            this._targetSprite = new Sprite(GinormoBoard.boardLayer.target, 0f, 0f);
            this._screenMaterial = new Material("Shaders/lcdNoBlur");
            this._screenMaterial.SetValue("screenWidth", GinormoScreen.GetSize(this._smallMode).x);
            this._screenMaterial.SetValue("screenHeight", GinormoScreen.GetSize(this._smallMode).y);
            base.Initialize();
        }

        public override void Draw()
        {
            if (!RockScoreboard.drawingNormalTarget && !NetworkDebugger.enabled)
                return;
            Material material = DuckGame.Graphics.material;
            DuckGame.Graphics.material = this._screenMaterial;
            DuckGame.Graphics.device.Textures[1] = (Texture2D)this._overlaySprite;
            DuckGame.Graphics.device.SamplerStates[1] = SamplerState.LinearClamp;
            this._targetSprite.depth = (Depth)0.9f;
            DuckGame.Graphics.Draw(this._targetSprite, this.x - 92f, this.y - 33f);
            DuckGame.Graphics.material = material;
        }
    }
}
