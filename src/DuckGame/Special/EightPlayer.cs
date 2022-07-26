// Decompiled with JetBrains decompiler
// Type: DuckGame.EightPlayer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Spawns")]
    public class EightPlayer : Thing
    {
        public EditorProperty<bool> eightPlayerOnly = new EditorProperty<bool>(false);

        public EightPlayer(float x, float y)
          : base(x, y)
        {
            this._editorName = "Eight Player";
            this.graphic = new Sprite("eight_player");
            this.center = new Vec2(8f, 8f);
            this.depth = (Depth)0.55f;
            this._visibleInGame = false;
            this.editorTooltip = "Place in a level to make it an 8 Player map!";
            this.eightPlayerOnly._tooltip = "If true, this map will not appear when less than 5 players are present in the game.";
            this.solid = false;
            this._collisionSize = new Vec2(0.0f, 0.0f);
        }
    }
}
