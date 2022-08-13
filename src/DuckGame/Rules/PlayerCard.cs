// Decompiled with JetBrains decompiler
// Type: DuckGame.PlayerCard
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class PlayerCard : Thing
    {
        private float _slideWait;
        private Vec2 _start;
        private Vec2 _end;
        private List<SpriteMap> _sprites = new List<SpriteMap>();
        private Team _team;

        public PlayerCard(float slideWait, Vec2 start, Vec2 end, Team team)
          : base()
        {
            layer = Layer.HUD;
            _start = start;
            _end = end;
            _slideWait = slideWait;
            position = _start;
            _team = team;
        }

        public override void Draw()
        {
        }
    }
}
