// Decompiled with JetBrains decompiler
// Type: DuckGame.GinormoOverlay
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            shouldbegraphicculled = false;
            depth = (Depth)0.9f;
            graphic = new Sprite("rockThrow/boardOverlay");
            _smallMode = smallMode;
        }

        public override void Initialize()
        {
            _overlaySprite = Content.Load<Tex2D>("rockThrow/boardOverlayLarge");
            _targetSprite = new Sprite(GinormoBoard.boardLayer.target, 0f, 0f);
            _screenMaterial = new Material("Shaders/lcdNoBlur");
            _screenMaterial.SetValue("screenWidth", GinormoScreen.GetSize(_smallMode).x);
            _screenMaterial.SetValue("screenHeight", GinormoScreen.GetSize(_smallMode).y);
            base.Initialize();
        }

        public override void Draw()
        {
            if (!RockScoreboard.drawingNormalTarget && !NetworkDebugger.enabled)
                return;
            Material material = Graphics.material;
            Graphics.material = _screenMaterial;
            Graphics.device.Textures[1] = (Texture2D)_overlaySprite;
            Graphics.device.SamplerStates[1] = SamplerState.LinearClamp;
            _targetSprite.depth = (Depth)0.9f;
            Graphics.Draw(ref _targetSprite, x - 92f, y - 33f);
            Graphics.material = material;
        }
    }
}
