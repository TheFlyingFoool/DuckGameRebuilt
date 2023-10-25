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
            get => _core;
            set => _core = value;
        }

        public static SpriteMap hats => _core.hats;

        public static Team Player1 => _core.teams[0];

        public static Team Player2 => _core.teams[1];

        public static Team Player3 => _core.teams[2];

        public static Team Player4 => _core.teams[3];

        public static Team Player5 => _core.teams[4];

        public static Team Player6 => _core.teams[5];

        public static Team Player7 => _core.teams[6];

        public static Team Player8 => _core.teams[7];

        public static Team NullTeam => _core.nullTeam;

        public static int numTeams => _core.all.Count;

        public static Team GetTeam(string name) => _core.all.FirstOrDefault(x => x.name == name) ?? _core.teams[8];

        public static int IndexOf(Team t)
        {
            if (Network.isActive && _core.extraTeams.Contains(t))
                return DuckNetwork.localProfile.customTeamIndexOffset + _core.extraTeams.IndexOf(t);
            return t.owner != null ? t.owner.IndexOfCustomTeam(t) : _core.all.IndexOf(t);
        }

        public static Team ParseFromIndex(ushort pIndex)
        {
            Team fromIndex = null;
            try
            {
                if (pIndex >= 0)
                {
                    if (pIndex < kCustomOffset)
                    {
                        fromIndex = all[pIndex];
                    }
                    else
                    {
                        int index = (pIndex - kCustomOffset) / kCustomSpread;
                        if (index >= 0)
                        {
                            if (index < DuckNetwork.profilesFixedOrder.Count)
                            {
                                int pIndex1 = (pIndex - kCustomOffset) % kCustomSpread;
                                fromIndex = DuckNetwork.profilesFixedOrder[index].GetCustomTeam((ushort)pIndex1);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return fromIndex;
        }

        public static int CurrentGameTeamIndex(Team t)
        {
            List<Team> teamList = new List<Team>();
            foreach (Team team in active)
            {
                if (team.activeProfiles.Count > 1)
                    teamList.Add(team);
            }
            return teamList.IndexOf(t);
        }

        public static List<Team> all => _core.all;

        public static List<Team> allStock => _core.allStock;

        public static List<Team> allRandomized
        {
            get
            {
                List<Team> teamList = new List<Team>();
                teamList.AddRange(all);
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
                foreach (Team team in all)
                {
                    if (team.activeProfiles.Where(x => x.slotType != SlotType.Spectator).Count() > 0)
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
                foreach (Team team in all)
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

        public static void AddExtraTeam(Team t) => _core.extraTeams.Add(t);

        public static void Initialize()
        {
            if (_core != null)
                return;
            _core = new TeamsCore();
            _core.Initialize();
        }

        public static void PostInitialize()
        {
            foreach (Team deserializedTeam in Team.deserializedTeams)
                AddExtraTeam(deserializedTeam);
        }
    }
}
