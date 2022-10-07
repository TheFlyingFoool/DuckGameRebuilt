// Decompiled with JetBrains decompiler
// Type: DuckGame.Profiles
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Profiles
    {
        private static ProfilesCore _core = new ProfilesCore();

        public static ProfilesCore core
        {
            get => Profiles._core;
            set => Profiles._core = value;
        }

        public static List<Profile> all => Profiles._core.all;

        public static List<Profile> allCustomProfiles => Profiles._core.allCustomProfiles;

        public static IEnumerable<Profile> universalProfileList => Profiles._core.universalProfileList;

        public static List<Profile> defaultProfiles => new List<Profile>()
        {
          Profiles.DefaultPlayer1,
          Profiles.DefaultPlayer2,
          Profiles.DefaultPlayer3,
          Profiles.DefaultPlayer4,
          Profiles.DefaultPlayer5,
          Profiles.DefaultPlayer6,
          Profiles.DefaultPlayer7,
          Profiles.DefaultPlayer8
        };

        public static Profile DefaultPlayer1 => Profiles._core.DefaultPlayer1;

        public static Profile DefaultPlayer2 => Profiles._core.DefaultPlayer2;

        public static Profile DefaultPlayer3 => Profiles._core.DefaultPlayer3;

        public static Profile DefaultPlayer4 => Profiles._core.DefaultPlayer4;

        public static Profile DefaultPlayer5 => Profiles._core.DefaultPlayer5;

        public static Profile DefaultPlayer6 => Profiles._core.DefaultPlayer6;

        public static Profile DefaultPlayer7 => Profiles._core.DefaultPlayer7;

        public static Profile DefaultPlayer8 => Profiles._core.DefaultPlayer8;

        public static Team EnvironmentTeam => Profiles._core.EnvironmentTeam;

        public static Profile EnvironmentProfile => Profiles._core.EnvironmentProfile;

        public static int DefaultProfileNumber(Profile p) => Profiles._core.DefaultProfileNumber(p);

        public static List<Profile> active => Profiles._core.active;

        public static List<Profile> activeNonSpectators => Profiles._core.activeNonSpectators;

        public static void Initialize() => Profiles._core.Initialize();

        public static List<ProfileStatRank> GetEndOfRoundStatRankings(StatInfo stat) => Profiles._core.GetEndOfRoundStatRankings(stat);

        public static bool IsDefault(Profile p) => Profiles._core.IsDefault(p);

        public static bool IsExperience(Profile p) => Profiles._core.IsExperience(p);

        public static bool IsDefaultName(string p) => Profiles._core.IsDefaultName(p);

        public static void Add(Profile p) => Profiles._core.Add(p);

        public static void Remove(Profile p) => Profiles._core.Remove(p);

        public static void Save(Profile p) => Profiles._core.Save(p);

        public static void Save(Profile p, string pPrepend) => Profiles._core.Save(p, pPrepend);

        public static Profile GetLastProfileWithInput()
        {
            Profile profileWithInput = null;
            long num = -1;
            foreach (Profile profile in Profiles.active)
            {
                if (profile.inputProfile != null && profile.inputProfile.lastPressFrame > num)
                {
                    num = profile.inputProfile.lastPressFrame;
                    profileWithInput = profile;
                }
            }
            return profileWithInput;
        }

        public static void SaveActiveProfiles()
        {
            DuckFile.BeginDataCommit();
            foreach (Team team in Teams.all)
            {
                foreach (Profile activeProfile in team.activeProfiles)
                    Profiles.Save(activeProfile);
            }
            DuckFile.EndDataCommit();
        }

        public static Profile experienceProfile => Profiles.core.DefaultExperienceProfile;

        public static void Delete(Profile p) => Profiles._core.Delete(p);

        public static float MostTimeOnFire()
        {
            float num = 0f;
            foreach (Profile profile in Profiles.all)
            {
                if (profile.stats.timeOnFire > num)
                    num = profile.stats.timeOnFire;
            }
            return num;
        }

        public static Profile Get(string pID)
        {
            foreach (Profile profile in Profiles.all)
            {
                if (profile.id == pID)
                    return profile;
            }
            return null;
        }
    }
}
