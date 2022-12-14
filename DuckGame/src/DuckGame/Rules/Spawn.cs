// Decompiled with JetBrains decompiler
// Type: DuckGame.Spawn
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Spawn
    {
        private static bool runningSecondSpawn;

        private static SpawnPoint AttemptTeamSpawn(
          Team team,
          List<SpawnPoint> usedSpawns,
          List<Duck> spawned)
        {
            Level current = Level.current;
            List<TeamSpawn> teamSpawnList = new List<TeamSpawn>();
            foreach (TeamSpawn teamSpawn in Level.current.things[typeof(TeamSpawn)])
            {
                if (!usedSpawns.Contains(teamSpawn) && (!teamSpawn.eightPlayerOnly.value || GameLevel.NumberOfDucks > 4))
                    teamSpawnList.Add(teamSpawn);
            }
            if (teamSpawnList.Count <= 0)
                return null;
            TeamSpawn teamSpawn1 = teamSpawnList[Rando.Int(teamSpawnList.Count - 1)];
            usedSpawns.Add(teamSpawn1);
            for (int index = 0; index < team.numMembers; ++index)
            {
                Vec2 position = teamSpawn1.position;
                if (team.numMembers == 2)
                {
                    float num = 18.82353f;
                    position.x = (float)(teamSpawn1.position.x - 16.0 + num * index);
                }
                else if (team.numMembers == 3)
                {
                    float num = 9.411764f;
                    position.x = (float)(teamSpawn1.position.x - 16.0 + num * index);
                }
                Duck duck = new Duck(position.x, position.y - 7f, team.activeProfiles[index])
                {
                    offDir = teamSpawn1.offDir
                };
                spawned.Add(duck);
            }
            return teamSpawn1;
        }

        private static SpawnPoint AttemptFreeSpawn(
          Profile profile,
          List<SpawnPoint> usedSpawns,
          List<Duck> spawned)
        {
            List<SpawnPoint> spawnPointList = new List<SpawnPoint>();
            foreach (FreeSpawn freeSpawn in Level.current.things[typeof(FreeSpawn)])
            {
                if (freeSpawn.secondSpawn.value == runningSecondSpawn && !usedSpawns.Contains(freeSpawn) && (!freeSpawn.eightPlayerOnly.value || GameLevel.NumberOfDucks > 4))
                    spawnPointList.Add(freeSpawn);
            }
            if (spawnPointList.Count == 0)
            {
                foreach (FreeSpawn freeSpawn in Level.current.things[typeof(FreeSpawn)])
                {
                    if (!usedSpawns.Contains(freeSpawn) && (!freeSpawn.eightPlayerOnly.value || GameLevel.NumberOfDucks > 4))
                        spawnPointList.Add(freeSpawn);
                }
            }
            if (spawnPointList.Count == 0)
                return null;
            SpawnPoint spawnPoint = spawnPointList[Rando.Int(spawnPointList.Count - 1)];
            usedSpawns.Add(spawnPoint);
            Duck duck = new Duck(spawnPoint.x, spawnPoint.y - 7f, profile)
            {
                offDir = spawnPoint.offDir
            };
            spawned.Add(duck);
            runningSecondSpawn = !runningSecondSpawn;
            return spawnPoint;
        }

        private static SpawnPoint AttemptCTFSpawn(
          Profile profile,
          List<SpawnPoint> usedSpawns,
          List<Duck> spawned,
          bool red)
        {
            int num = red ? 1 : 2;
            List<SpawnPoint> spawnPointList = new List<SpawnPoint>();
            foreach (FreeSpawn freeSpawn in Level.current.things[typeof(FreeSpawn)])
            {
                if (!usedSpawns.Contains(freeSpawn) && (int)freeSpawn.spawnType == num)
                    spawnPointList.Add(freeSpawn);
            }
            if (spawnPointList.Count == 0)
                return null;
            SpawnPoint spawnPoint = spawnPointList[Rando.Int(spawnPointList.Count - 1)];
            usedSpawns.Add(spawnPoint);
            Duck duck = new Duck(spawnPoint.x, spawnPoint.y - 7f, profile)
            {
                offDir = spawnPoint.offDir
            };
            spawned.Add(duck);
            return spawnPoint;
        }

        private static SpawnPoint AttemptAnySpawn(
          Profile profile,
          List<SpawnPoint> usedSpawns,
          List<Duck> spawned)
        {
            List<SpawnPoint> spawnPointList = new List<SpawnPoint>();
            foreach (SpawnPoint spawnPoint in Level.current.things[typeof(SpawnPoint)])
            {
                if (!usedSpawns.Contains(spawnPoint))
                    spawnPointList.Add(spawnPoint);
            }
            if (spawnPointList.Count == 0)
            {
                if (usedSpawns.Count <= 0)
                    return null;
                spawnPointList.AddRange(usedSpawns);
            }
            SpawnPoint spawnPoint1 = spawnPointList[Rando.Int(spawnPointList.Count - 1)];
            usedSpawns.Add(spawnPoint1);
            Duck duck = new Duck(spawnPoint1.x, spawnPoint1.y - 7f, profile)
            {
                offDir = spawnPoint1.offDir
            };
            spawned.Add(duck);
            return spawnPoint1;
        }

        public static List<Duck> SpawnPlayers() => SpawnPlayers(true);

        public static List<Duck> SpawnPlayers(bool recordStats)
        {
            List<Duck> duckList1 = new List<Duck>();
            List<SpawnPoint> usedSpawns = new List<SpawnPoint>();
            List<Team> teamList1 = Teams.allRandomized;
            if (GameMode.showdown)
            {
                List<Team> teamList2 = new List<Team>();
                int num = 0;
                foreach (Team team in teamList1)
                {
                    if (team.score > num)
                        num = team.score;
                }
                foreach (Team team in teamList1)
                {
                    if (team.score == num)
                        teamList2.Add(team);
                }
                teamList1 = teamList2;
            }
            int num1 = 0;
            foreach (Team team in teamList1)
            {
                if (team.activeProfiles.Count() != 0)
                    ++num1;
            }
            GameLevel.NumberOfDucks = num1;
            foreach (Team team in teamList1)
            {
                if (team.activeProfiles.Count() != 0)
                {
                    if (recordStats)
                    {
                        foreach (Profile activeProfile in team.activeProfiles)
                            ++activeProfile.stats.timesSpawned;
                    }
                    if (team.activeProfiles.Count() == 1)
                    {
                        SpawnPoint spawnPoint = AttemptFreeSpawn(team.activeProfiles[0], usedSpawns, duckList1);
                        if (spawnPoint == null)
                        {
                            spawnPoint = AttemptTeamSpawn(team, usedSpawns, duckList1);
                            if (spawnPoint == null)
                            {
                                usedSpawns.Clear();
                                spawnPoint = AttemptFreeSpawn(team.activeProfiles[0], usedSpawns, duckList1);
                                if (spawnPoint == null)
                                {
                                    usedSpawns.Clear();
                                    spawnPoint = AttemptTeamSpawn(team, usedSpawns, duckList1);
                                }
                            }
                        }
                        if (spawnPoint == null)
                            return duckList1;
                    }
                    else if (AttemptTeamSpawn(team, usedSpawns, duckList1) == null)
                    {
                        foreach (Profile activeProfile in team.activeProfiles)
                        {
                            SpawnPoint spawnPoint = AttemptFreeSpawn(activeProfile, usedSpawns, duckList1);
                            if (spawnPoint == null)
                            {
                                usedSpawns.Clear();
                                spawnPoint = AttemptFreeSpawn(activeProfile, usedSpawns, duckList1);
                            }
                            if (spawnPoint == null)
                                return duckList1;
                        }
                    }
                }
            }
            List<Duck> duckList2 = new List<Duck>();
            foreach (Duck duck1 in duckList1)
            {
                Duck d = duck1;
                Duck duck2 = duckList1.FirstOrDefault(x => x != d && x.position == d.position);
                if (duck2 != null && !duckList2.Contains(duck2) && !duckList2.Contains(d))
                {
                    d.x += 4f;
                    duck2.x -= 4f;
                    duckList2.Add(d);
                    duckList2.Add(duck2);
                }
            }
            return duckList1;
        }

        public static List<Duck> SpawnCTF()
        {
            List<Duck> spawned = new List<Duck>();
            List<SpawnPoint> usedSpawns = new List<SpawnPoint>();
            List<Team> all = Teams.all;
            int num = 0;
            foreach (Team team in all)
            {
                if (team.activeProfiles.Count() != 0)
                {
                    foreach (Profile activeProfile in team.activeProfiles)
                        ++activeProfile.stats.timesSpawned;
                    foreach (Profile activeProfile in team.activeProfiles)
                        AttemptCTFSpawn(activeProfile, usedSpawns, spawned, num == 0);
                    ++num;
                }
            }
            return spawned;
        }
    }
}
