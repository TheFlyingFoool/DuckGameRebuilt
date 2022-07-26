// Decompiled with JetBrains decompiler
// Type: DuckGame.Teams
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class Teams
    {
        private static TeamsCore _core;
        public static int kCustomOffset = 5000;
        public static int kCustomSpread = 2000;

        public static TeamsCore core
        {
            get => Teams._core;
            set => Teams._core = value;
        }

        public static SpriteMap hats => Teams._core.hats;

        public static Team Player1 => Teams._core.teams[0];

        public static Team Player2 => Teams._core.teams[1];

        public static Team Player3 => Teams._core.teams[2];

        public static Team Player4 => Teams._core.teams[3];

        public static Team Player5 => Teams._core.teams[4];

        public static Team Player6 => Teams._core.teams[5];

        public static Team Player7 => Teams._core.teams[6];

        public static Team Player8 => Teams._core.teams[7];

        public static Team NullTeam => Teams._core.nullTeam;

        public static int numTeams => Teams._core.all.Count;

        public static Team GetTeam(string name) => Teams._core.all.FirstOrDefault<Team>((Func<Team, bool>)(x => x.name == name)) ?? Teams._core.teams[8];

        public static int IndexOf(Team t)
        {
            if (Network.isActive && Teams._core.extraTeams.Contains(t))
                return (int)DuckNetwork.localProfile.customTeamIndexOffset + Teams._core.extraTeams.IndexOf(t);
            return t.owner != null ? t.owner.IndexOfCustomTeam(t) : Teams._core.all.IndexOf(t);
        }

        public static Team ParseFromIndex(ushort pIndex)
        {
            Team fromIndex = (Team)null;
            try
            {
                if (pIndex >= (ushort)0)
                {
                    if ((int)pIndex < Teams.kCustomOffset)
                    {
                        fromIndex = Teams.all[(int)pIndex];
                    }
                    else
                    {
                        int index = ((int)pIndex - Teams.kCustomOffset) / Teams.kCustomSpread;
                        if (index >= 0)
                        {
                            if (index < DuckNetwork.profilesFixedOrder.Count)
                            {
                                int pIndex1 = ((int)pIndex - Teams.kCustomOffset) % Teams.kCustomSpread;
                                fromIndex = DuckNetwork.profilesFixedOrder[index].GetCustomTeam((ushort)pIndex1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return fromIndex;
        }

        public static int CurrentGameTeamIndex(Team t)
        {
            List<Team> teamList = new List<Team>();
            foreach (Team team in Teams.active)
            {
                if (team.activeProfiles.Count > 1)
                    teamList.Add(team);
            }
            return teamList.IndexOf(t);
        }

        public static List<Team> all => Teams._core.all;

        public static List<Team> allStock => Teams._core.allStock;

        public static List<Team> allRandomized
        {
            get
            {
                List<Team> teamList = new List<Team>();
                teamList.AddRange((IEnumerable<Team>)Teams.all);
                List<Team> allRandomized = new List<Team>();
                while (teamList.Count > 0)
                {
                    int index = Rando.Int(teamList.Count - 1);
                    allRandomized.Add(teamList[index]);
                    teamList.RemoveAt(index);
                }
                return allRandomized;
            }
        }

        public static List<Team> active
        {
            get
            {
                List<Team> active = new List<Team>();
                foreach (Team team in Teams.all)
                {
                    if (team.activeProfiles.Where<Profile>((Func<Profile, bool>)(x => x.slotType != SlotType.Spectator)).Count<Profile>() > 0)
                        active.Add(team);
                }
                return active;
            }
        }

        public static List<Team> winning
        {
            get
            {
                List<Team> winning = new List<Team>();
                foreach (Team team in Teams.all)
                {
                    if (team.activeProfiles.Count > 0)
                    {
                        if (winning.Count == 0 || team.score > winning[0].score)
                        {
                            winning.Clear();
                            winning.Add(team);
                        }
                        else if (winning.Count != 0 && team.score == winning[0].score)
                            winning.Add(team);
                    }
                }
                return winning;
            }
        }

        public static void AddExtraTeam(Team t) => Teams._core.extraTeams.Add(t);

        public static void Initialize()
        {
            if (Teams._core != null)
                return;
            Teams._core = new TeamsCore();
            Teams._core.Initialize();
        }

        public static void PostInitialize()
        {
            foreach (Team deserializedTeam in Team.deserializedTeams)
                Teams.AddExtraTeam(deserializedTeam);
        }
    }
}
