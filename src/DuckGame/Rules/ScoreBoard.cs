// Decompiled with JetBrains decompiler
// Type: DuckGame.ScoreBoard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ScoreBoard : Thing
    {
        public ScoreBoard()
          : base()
        {
        }

        public override void Initialize()
        {
            int num = 0;
            foreach (Team team in Teams.all)
            {
                if (team.activeProfiles.Count > 0)
                {
                    Level.current.AddThing((Thing)new PlayerCard((float)num * 1f, new Vec2(-400f, (float)(140 * num + 120)), new Vec2((float)(Graphics.width / 2 - 200), (float)(140 * num + 120)), team));
                    ++num;
                }
            }
        }
    }
}
