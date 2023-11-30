using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DM : GameMode
    {
        //private int waitFrames = 3;

        public DM(bool validityTest = false, bool editorTestMode = false)
          : base(validityTest, editorTestMode)
        {
        }

        protected override void Initialize()
        {
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
            List<Team> teamList = new List<Team>();
            foreach (Team team in Teams.all)
            {
                foreach (Profile activeProfile in team.activeProfiles)
                {
                    if (activeProfile.duck != null && !activeProfile.duck.dead)
                    {
                        if (activeProfile.duck.converted != null && activeProfile.duck.converted.profile.team != activeProfile.team)
                        {
                            if (!teamList.Contains(activeProfile.duck.converted.profile.team))
                                teamList.Add(activeProfile.duck.converted.profile.team);
                        }
                        else if (!teamList.Contains(team))
                            teamList.Add(team);
                    }
                }
            }
            if (teamList.Count <= 1)
            {
                EndMatch();
            }
            else
            {
                _matchOver = false;
                _roundEndWait = 1f;
            }
            base.Update();
        }

        protected override List<Duck> AssignSpawns() => Spawn.SpawnPlayers().OrderBy(sp => sp.x).ToList();

        protected override Level GetNextLevel() => _editorTestMode ? new GameLevel((Level.current as GameLevel).levelInputString, editorTestMode: true) : (Level)new GameLevel(Deathmatch.RandomLevelString(previousLevel));

        protected override List<Profile> AddPoints()
        {
            if (TeamSelect2.KillsForPoints)
                return new List<Profile>();
            List<Profile> profileList = new List<Profile>();
            List<Team> collection = new List<Team>();
            List<Team> source = new List<Team>();
            foreach (Team team in Teams.all)
            {
                foreach (Profile activeProfile in team.activeProfiles)
                {
                    if (activeProfile != null && activeProfile.duck != null && !activeProfile.duck.dead)
                    {
                        if (activeProfile.duck.converted != null && activeProfile.duck.converted.profile != null && activeProfile.duck.converted.profile.team != activeProfile.team)
                        {
                            if (!source.Contains(activeProfile.duck.converted.profile.team))
                                source.Add(activeProfile.duck.converted.profile.team);
                            if (!collection.Contains(activeProfile.duck.profile.team))
                            {
                                collection.Add(activeProfile.duck.profile.team);
                                break;
                            }
                            break;
                        }
                        if (!source.Contains(team))
                        {
                            source.Add(team);
                            break;
                        }
                        break;
                    }
                }
            }
            if (source.Count <= 1 && source.Count > 0)
            {
                source.AddRange(collection);
                lastWinners.Clear();
                Profile pTheRealWinnerHere = null;
                foreach (Team team in source)
                {
                    foreach (Profile activeProfile in team.activeProfiles)
                    {
                        if (activeProfile != null && activeProfile.duck != null && !activeProfile.duck.dead)
                        {
                            profileList.Add(activeProfile);
                            Profile p = activeProfile;
                            if (activeProfile.duck.converted != null)
                                p = pTheRealWinnerHere = activeProfile.duck.converted.profile;
                            lastWinners.Add(activeProfile);
                            if (p != null)
                            {
                                PlusOne plusOne = new PlusOne(0f, 0f, p, testMode: _editorTestMode)
                                {
                                    anchor = (Anchor)activeProfile.duck
                                };
                                plusOne.anchor.offset = new Vec2(0f, -16f);
                                Level.Add(plusOne);
                            }
                        }
                    }
                }
                if (Network.isActive && Network.isServer)
                    Send.Message(new NMAssignWin(lastWinners, pTheRealWinnerHere));
                ++source[0].score;
            }
            return profileList;
        }
    }
}
