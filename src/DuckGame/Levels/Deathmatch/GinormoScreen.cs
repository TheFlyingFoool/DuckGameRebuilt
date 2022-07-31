// Decompiled with JetBrains decompiler
// Type: DuckGame.GinormoScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class GinormoScreen : Thing
    {
        private BitmapFont _font;

        public static Vec2 GetSize(bool pSmall) => new Vec2(185f, 103f);

        public GinormoScreen(float xpos, float ypos, BoardMode mode)
          : base(xpos, ypos)
        {
            this.layer = Layer.Foreground;
            this.depth = (Depth)0f;
            this._font = new BitmapFont("biosFont", 8);
            this._collisionSize = new Vec2(184f, 102f);
            List<Team> teamList = new List<Team>();
            int idx = 0;
            foreach (Team team in Teams.all)
            {
                if (team.activeProfiles.Count > 0)
                    teamList.Add(team);
            }
            teamList.Sort((a, b) =>
           {
               if (a.score == b.score)
                   return 0;
               return a.score >= b.score ? -1 : 1;
           });
            bool smallMode = teamList.Count > 4;
            foreach (Team team in teamList)
            {
                float y = this.y + 2f + (smallMode ? 12 : 25) * idx;
                if ((double)Graphics.aspect > 0.589999973773956)
                    y += 10f;
                Level.current.AddThing(new GinormoCard(idx * 1f, new Vec2(300f, y), new Vec2(this.x + (mode == BoardMode.Points ? 2f : 2f), y), team, mode, idx, smallMode));
                ++idx;
            }
        }

        public override void Draw()
        {
        }
    }
}
