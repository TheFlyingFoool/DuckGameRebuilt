using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace DuckGame
{
    public class Profiles
    {
        private static ProfilesCore _core = new ProfilesCore();

        public static ProfilesCore core
        {
            get => _core;
            set => _core = value;
        }

        public static IEnumerable<Profile> all => _core.all;
        public static List<Profile> alllist => _core.alllist;
        public static List<Profile> allCustomProfiles => _core.allCustomProfiles;

        public static IEnumerable<Profile> universalProfileList => _core.universalProfileList;

        public static List<Profile> defaultProfiles
        {
            get
            {
                List<Profile> profiles = new List<Profile>
                {
                    DefaultPlayer1,
                    DefaultPlayer2,
                    DefaultPlayer3,
                    DefaultPlayer4,
                    DefaultPlayer5,
                    DefaultPlayer6,
                    DefaultPlayer7,
                    DefaultPlayer8
                };
                if (DuckNetwork.FiftyPlayerMode)
                {
                    try
                    {
                        for (int i = 8; i < 50; i++)
                        {
                            Profile p = GetProfile(i);
                            if (Level.current is TitleScreen || !didsetinputs)
                            {
                                didsetinputs = true;
                                p.inputProfile = InputProfile.Get("MPPlayer" + (i + 1).ToString());
                            }

                            profiles.Add(p);
                        }
                    }
                    catch
                    {
                        DevConsole.Log("F get_defaultProfilesPrefix");
                    }
                }
                return profiles;
            }
        }
        public static bool didsetinputs;
        public static Profile GetProfile(int index)
        {

            if (!Network.isActive)
            {
                return Profile.defaultProfileMappings[index];
            }
            return Profiles.core.all.ElementAt(index);
        }

        public static Profile DefaultPlayer1 => _core.DefaultPlayer1;

        public static Profile DefaultPlayer2 => _core.DefaultPlayer2;

        public static Profile DefaultPlayer3 => _core.DefaultPlayer3;

        public static Profile DefaultPlayer4 => _core.DefaultPlayer4;

        public static Profile DefaultPlayer5 => _core.DefaultPlayer5;

        public static Profile DefaultPlayer6 => _core.DefaultPlayer6;

        public static Profile DefaultPlayer7 => _core.DefaultPlayer7;

        public static Profile DefaultPlayer8 => _core.DefaultPlayer8;

        public static Team EnvironmentTeam => _core.EnvironmentTeam;

        public static Profile EnvironmentProfile => _core.EnvironmentProfile;

        public static int DefaultProfileNumber(Profile p) => _core.DefaultProfileNumber(p);

        public static List<Profile> active => _core.active;

        public static List<Profile> activeNonSpectators => _core.activeNonSpectators;

        public static void Initialize() => _core.Initialize();

        public static List<ProfileStatRank> GetEndOfRoundStatRankings(StatInfo stat) => _core.GetEndOfRoundStatRankings(stat);

        public static bool IsDefault(Profile p) => _core.IsDefault(p);

        public static bool IsExperience(Profile p) => _core.IsExperience(p);

        public static bool IsDefaultName(string p) => _core.IsDefaultName(p);

        public static void Add(Profile p) => _core.Add(p);

        public static void Remove(Profile p) => _core.Remove(p);

        public static void Save(Profile p) => _core.Save(p);

        public static void Save(Profile p, string pPrepend) => _core.Save(p, pPrepend);

        public static Profile GetLastProfileWithInput()
        {
            Profile profileWithInput = null;
            long num = -1;
            foreach (Profile profile in active)
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
                    Save(activeProfile);
            }
            DuckFile.EndDataCommit();
        }

        public static Profile experienceProfile => core.DefaultExperienceProfile;

        public static void Delete(Profile p) => _core.Delete(p);

        public static float MostTimeOnFire()
        {
            float num = 0f;
            foreach (Profile profile in all)
            {
                if (profile.stats.timeOnFire > num)
                    num = profile.stats.timeOnFire;
            }
            return num;
        }

        public static Profile Get(string pID)
        {
            foreach (Profile profile in all)
            {
                if (profile.id == pID)
                    return profile;
            }
            return null;
        }
    }
}
