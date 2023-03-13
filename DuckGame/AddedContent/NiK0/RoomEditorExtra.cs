using System.Collections.Generic;

namespace DuckGame
{
    public class RoomEditorExtra
    {
        [AutoConfigField]//TODO: this eventually
        public static List<byte> room1 = new List<byte>();
        [AutoConfigField]
        public static List<byte> room2 = new List<byte>();
        [AutoConfigField]
        public static List<byte> room3 = new List<byte>();

        [AutoConfigField]
        public static List<string> favoriteHats = new List<string>();
        [AutoConfigField]
        public static string arcadeHat = "";

        public static void Initialize()
        {
            List<string> rel = new List<string>();
            for (int i = 0; i < Teams.all.Count; i++)
            {
                Team t = Teams.all[i];
                if (t.defaultTeam)
                {
                    if (favoriteHats.Contains("D" + t.name))
                    {
                        t.favorited = true;
                        rel.Add("D" + t.name);
                    }
                }
                else
                {
                    if (favoriteHats.Contains("C" + t.name))
                    {
                        t.favorited = true;
                        rel.Add("C" + t.name);
                    }
                }
            }
            //If any hats have been renamed or deleted they get deleted from the list
            favoriteHats = rel;
        }
        public static void ReloadFavHats()
        {
            if (!Network.isActive)
            {
                List<Team> tts = new List<Team>();

                List<Team> laterer = new List<Team>();
                for (int i = 0; i < Teams.all.Count; i++)
                {
                    Team t = Teams.all[i];
                    if (t.favorited)
                    {
                        laterer.Add(t);
                    }
                    else
                    {
                        tts.Add(t);
                    }
                }
                tts.AddRange(laterer);

                HatSelector.remember = tts;
            }
            else
            {

            }

            favoriteHats.Clear();
            for (int i = 0; i < Teams.all.Count; i++)
            {
                Team t = Teams.all[i];
                if (t.favorited)
                {
                    if (t.defaultTeam)
                    {
                        favoriteHats.Add("D" + t.name);
                    }
                    else
                    {
                        favoriteHats.Add("C" + t.name);
                    }
                }
            }
        }
    }
}
