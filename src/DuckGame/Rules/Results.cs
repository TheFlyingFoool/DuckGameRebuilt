// Decompiled with JetBrains decompiler
// Type: DuckGame.Results
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Results
    {
        public static List<ResultData> teams
        {
            get
            {
                List<ResultData> teams = new List<ResultData>();
                foreach (Team t in Teams.all)
                {
                    if (t.activeProfiles.Count > 0)
                        teams.Add(new ResultData(t));
                }
                return teams;
            }
        }

        public static ResultData winner
        {
            get
            {
                List<ResultData> teams = Results.teams;
                teams.Sort((a, b) =>
               {
                   if (a.score == b.score)
                       return 0;
                   return a.score >= b.score ? -1 : 1;
               });
                return teams[0];
            }
        }

        public static ResultData runnerUp
        {
            get
            {
                List<ResultData> teams = Results.teams;
                teams.Sort((a, b) =>
               {
                   if (a.score == b.score)
                       return 0;
                   return a.score >= b.score ? -1 : 1;
               });
                return teams[1];
            }
        }

        public static ResultData loser
        {
            get
            {
                List<ResultData> teams = Results.teams;
                teams.Sort((a, b) =>
               {
                   if (a.score == b.score)
                       return 0;
                   return a.score <= b.score ? -1 : 1;
               });
                return teams[0];
            }
        }
    }
}
