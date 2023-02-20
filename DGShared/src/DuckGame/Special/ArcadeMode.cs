// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeMode
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ArcadeMode : Thing
    {
        public ArcadeMode()
          : base()
        {
            graphic = new Sprite("arcadeIcon");
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Arcade";
            _canFlip = false;
            _canHaveChance = false;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
        }
    }
}
