// Decompiled with JetBrains decompiler
// Type: DuckGame.CTF
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class CTF : GameMode
    {
        public static bool hasWinner;
        public static bool winner;

        protected override void Initialize() => CTF.hasWinner = false;

        protected override void Start()
        {
        }

        public static void CaptureFlag(bool team)
        {
            CTF.hasWinner = true;
            CTF.winner = team;
        }

        protected override void Update()
        {
            if (!this._matchOver)
            {
                List<Team> teamList = new List<Team>();
                int num = 0;
                foreach (Team team in Teams.all)
                {
                    if (team.activeProfiles.Count<Profile>() != 0)
                    {
                        foreach (Profile activeProfile in team.activeProfiles)
                        {
                            if (activeProfile.duck != null)
                            {
                                activeProfile.duck.ctfTeamIndex = num;
                                if (!activeProfile.duck.dead)
                                {
                                    if (activeProfile.duck.converted != null && activeProfile.duck.converted.profile.team != activeProfile.team)
                                    {
                                        if (!teamList.Contains(activeProfile.duck.converted.profile.team))
                                            teamList.Add(activeProfile.duck.converted.profile.team);
                                    }
                                    else if (!teamList.Contains(team))
                                        teamList.Add(team);
                                }
                                else
                                {
                                    activeProfile.duck.position = activeProfile.duck.respawnPos;
                                    if (Level.current.camera is FollowCam)
                                        (Level.current.camera as FollowCam).Add(activeProfile.duck);
                                    activeProfile.duck.respawnTime += 0.016f;
                                    if (activeProfile.duck.respawnTime > 1.5)
                                    {
                                        activeProfile.duck.respawnTime = 0f;
                                        activeProfile.duck.dead = false;
                                        if (activeProfile.duck.ragdoll != null)
                                            activeProfile.duck.ragdoll.Unragdoll();
                                        activeProfile.duck.position = activeProfile.duck.respawnPos;
                                        activeProfile.duck.isGhost = true;
                                        activeProfile.duck.immobilized = false;
                                        activeProfile.duck.crouch = false;
                                        activeProfile.duck.sliding = false;
                                        activeProfile.duck._cooked = null;
                                        activeProfile.duck.onFire = false;
                                        activeProfile.duck.unfocus = 1f;
                                        if (activeProfile.duck._trapped != null)
                                            Level.Remove(activeProfile.duck._trapped);
                                        activeProfile.duck._trapped = null;
                                        if (Level.current.camera is FollowCam)
                                            (Level.current.camera as FollowCam).Add(activeProfile.duck);
                                        Level.Add(activeProfile.duck);
                                    }
                                }
                            }
                        }
                        ++num;
                    }
                }
                if (CTF.hasWinner)
                    this.EndMatch();
            }
            base.Update();
        }

        protected override List<Duck> AssignSpawns() => Spawn.SpawnCTF().OrderBy<Duck, float>(sp => sp.x).ToList<Duck>();

        protected override Level GetNextLevel() => new CTFLevel(Deathmatch.RandomLevelString(GameMode.previousLevel, "ctf"));

        protected override List<Profile> AddPoints()
        {
            List<Profile> pProfiles = new List<Profile>();
            List<Team> collection = new List<Team>();
            List<Team> source = new List<Team>();
            int num1 = CTF.winner ? 0 : 1;
            int num2 = 0;
            foreach (Team team in Teams.all)
            {
                if (team.activeProfiles.Count<Profile>() != 0)
                {
                    foreach (Profile activeProfile in team.activeProfiles)
                    {
                        if (activeProfile.duck != null && num2 == num1)
                        {
                            if (activeProfile.duck.converted != null && activeProfile.duck.converted.profile.team != activeProfile.team)
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
                    ++num2;
                }
            }
            if (source.Count <= 1 && source.Count > 0)
            {
                source.AddRange(collection);
                foreach (Team team in source)
                {
                    foreach (Profile activeProfile in team.activeProfiles)
                    {
                        if (activeProfile.duck != null && !activeProfile.duck.dead)
                        {
                            pProfiles.Add(activeProfile);
                            activeProfile.stats.lastWon = DateTime.Now;
                            ++activeProfile.stats.matchesWon;
                            Profile p = activeProfile;
                            if (activeProfile.duck.converted != null)
                                p = activeProfile.duck.converted.profile;
                            PlusOne plusOne = new PlusOne(0f, 0f, p)
                            {
                                anchor = (Anchor)activeProfile.duck
                            };
                            plusOne.anchor.offset = new Vec2(0f, -16f);
                            Level.Add(plusOne);
                        }
                    }
                }
                if (Network.isActive && Network.isServer)
                    Send.Message(new NMAssignWin(pProfiles, null));
                ++source.First<Team>().score;
            }
            return pProfiles;
        }

        public CTF()
          : base()
        {
        }
    }
}
