// Decompiled with JetBrains decompiler
// Type: DuckGame.LightingTwoPointOH
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Debug)]
    public class LightingTwoPointOH : Thing
    {
        public LightingTwoPointOH()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
            _editorName = "Lighting 2.0";
        }

        public override void Initialize()
        {
            Layer.lightingTwoPointOh = true;
            Layer.Lighting.depth = -20;
            base.Initialize();
        }
    }
}
