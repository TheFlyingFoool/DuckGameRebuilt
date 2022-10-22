// Decompiled with JetBrains decompiler
// Type: DuckGame.News
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class News
    {
        private static List<NewsStory> _availableStories = new List<NewsStory>();

        public static void Initialize()
        {
            System.Type newsType = typeof(NewsStory);
            foreach (System.Type type in Assembly.GetAssembly(typeof(NewsStory)).GetTypes().Where<System.Type>(t => newsType.IsAssignableFrom(t)))
                News._availableStories.Add(Activator.CreateInstance(type) as NewsStory);
        }

        public static List<NewsStory> GetStories()
        {
            Stats.CalculateStats();
            List<Team> active = Teams.active;
            List<NewsStory> stories = new List<NewsStory>();
            foreach (NewsStory availableStorey in News._availableStories)
            {
                availableStorey.DoCalculate(active);
                stories.Add(availableStorey);
            }
            News.FilterBest(stories, NewsSection.MatchComments, 1);
            News.FilterBest(stories, NewsSection.PlayerComments, 2);
            stories.Sort((a, b) =>
           {
               if (a.section == b.section)
                   return 0;
               return a.section >= b.section ? 1 : -1;
           });
            return stories;
        }

        public static void FilterBest(List<NewsStory> stories, NewsSection section, int numToPick)
        {
            List<NewsStory> list = stories.Where<NewsStory>(x => x.section == section).ToList<NewsStory>();
            list.OrderBy<NewsStory, float>(x => x.weight * x.importance);
            int num = 0;
            foreach (NewsStory newsStory in list)
            {
                if (num >= numToPick)
                    stories.Remove(newsStory);
                ++num;
            }
        }
    }
}
