// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeMode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new Sprite("arcadeIcon");
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._editorName = "Arcade";
            this._canFlip = false;
            this._canHaveChance = false;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
        }
    }
}
