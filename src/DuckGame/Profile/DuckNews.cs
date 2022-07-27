// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckNews
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class DuckNews
    {
        private static List<DuckNews> _stories = new List<DuckNews>();
        private NewsSection _section;
        private string _name;
        private List<ScriptStatement> _requirements = new List<ScriptStatement>();
        private CycleMode _cycle;
        private ScriptStatement _valueCalculation;
        private ScriptStatement _valueCalculation2;
        private List<DuckNews> _subStories = new List<DuckNews>();
        private List<string> _dialogue = new List<string>();

        public static void Initialize()
        {
            foreach (string file in Content.GetFiles("Content/news"))
            {
                Stream s = DuckFile.OpenStream(file);
                DuckXML duckXml = DuckXML.Load(s);
                IEnumerable<DXMLNode> source1 = duckXml.Elements("NewsStory");
                if (duckXml.invalid || s == null || source1.Count<DXMLNode>() == 0)
                    throw new Exception("Content Exception: Failed to load news story (" + Content.path + "news/" + file + ".news)! Is the file missing, or has it been edited?");
                if (source1 != null)
                {
                    if (DG.isHalloween)
                    {
                        IEnumerable<DXMLNode> source2 = source1.Elements<DXMLNode>("NewsStoryHalloween");
                        if (source2 != null && source2.Count<DXMLNode>() > 0)
                            source1 = source2;
                    }
                    DuckNews duckNews = DuckNews.Parse(source1.ElementAt<DXMLNode>(0));
                    if (duckNews != null)
                        DuckNews._stories.Add(duckNews);
                }
            }
            DuckNews._stories = DuckNews._stories.OrderBy<DuckNews, int>(x => (int)x._section).ToList<DuckNews>();
        }

        public static List<DuckStory> CalculateStories()
        {
            foreach (Profile profile in Profiles.active)
                profile.endOfRoundStats = null;
            List<DuckStory> stories = new List<DuckStory>();
            foreach (DuckNews storey in DuckNews._stories)
            {
                List<DuckStory> story = storey.CalculateStory();
                stories.AddRange(story);
            }
            return stories;
        }

        public string FillString(string text, List<Profile> p)
        {
            if (p != null)
            {
                if (p.Count > 0)
                    text = text.Replace("%NAME%", p[0].name);
                if (p.Count > 1)
                    text = text.Replace("%NAME2%", p[1].name);
                if (p.Count > 2)
                    text = text.Replace("%NAME3%", p[2].name);
                if (p.Count > 3)
                    text = text.Replace("%NAME4%", p[3].name);
            }
            text = text.Replace("%PRICE%", Main.GetPriceString());
            if (this.valueCalculation != null)
            {
                object result = this.valueCalculation.result;
                switch (result)
                {
                    case float _:
                    case int _:
                        float single = Change.ToSingle(result);
                        text = text.Replace("%VALUE%", Change.ToString(single));
                        int int32 = Convert.ToInt32(result);
                        text = text.Replace("%INTVALUE%", Change.ToString(int32));
                        break;
                    case string _:
                        text = text.Replace("%VALUE%", result as string);
                        break;
                }
            }
            if (this.valueCalculation2 != null)
            {
                object result = this.valueCalculation2.result;
                switch (result)
                {
                    case float _:
                    case int _:
                        float single = Change.ToSingle(result);
                        text = text.Replace("%VALUE2%", Change.ToString(single));
                        int int32 = Convert.ToInt32(result);
                        text = text.Replace("%INTVALUE2%", Change.ToString(int32));
                        break;
                    case string _:
                        text = text.Replace("%VALUE2%", result as string);
                        break;
                }
            }
            return text;
        }

        public List<DuckStory> CalculateStory()
        {
            List<DuckStory> story = new List<DuckStory>();
            List<Profile> p = new List<Profile>();
            if (this._cycle == CycleMode.Once)
            {
                p.Add(Profiles.DefaultPlayer1);
                story.AddRange(this.CalculateStory(p));
            }
            else if (this._cycle == CycleMode.PerProfile)
            {
                foreach (Profile profile in Profiles.active)
                {
                    p.Add(profile);
                    story.AddRange(this.CalculateStory(p));
                    p.Clear();
                }
            }
            else if (this._cycle == CycleMode.PerPosition && this._valueCalculation != null)
            {
                List<List<Profile>> source = new List<List<Profile>>();
                List<Profile> active = Profiles.active;
                foreach (Profile profile in Profiles.active)
                {
                    float num = -999999f;
                    Script.activeProfile = profile;
                    object result = this.valueCalculation.result;
                    if (result != null && result is float || result is int || result is double)
                        num = Change.ToSingle(result);
                    profile.storeValue = num;
                    bool flag = false;
                    for (int index = 0; index < source.Count; ++index)
                    {
                        if (source[index][0].storeValue < (double)num)
                        {
                            source.Insert(index, new List<Profile>());
                            source[index].Add(profile);
                            flag = true;
                            break;
                        }
                        if (source[index][0].storeValue == (double)num)
                        {
                            source[index].Add(profile);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        source.Add(new List<Profile>());
                        source.Last<List<Profile>>().Add(profile);
                    }
                }
                source.Reverse();
                Script.positions = source;
                int num1 = source.Count - 1;
                foreach (List<Profile> collection in source)
                {
                    Script.currentPosition = num1;
                    p.AddRange(collection);
                    story.AddRange(this.CalculateStory(p));
                    p.Clear();
                    --num1;
                }
            }
            return story;
        }

        public List<DuckStory> CalculateStory(List<Profile> p)
        {
            List<DuckStory> story = new List<DuckStory>();
            Script.activeNewsStory = this;
            if (p == null || p.Count > 0)
                Script.activeProfile = p[0];
            foreach (ScriptStatement requirement in this._requirements)
            {
                if (requirement.result is bool result && !result)
                    return story;
            }
            if (this._dialogue.Count > 0)
            {
                DuckStory duckStory = new DuckStory()
                {
                    section = this._section,
                    text = this._dialogue[Rando.Int(this._dialogue.Count - 1)]
                };
                duckStory.text = this.FillString(duckStory.text, p);
                story.Add(duckStory);
            }
            foreach (DuckNews subStorey in this._subStories)
            {
                if (subStorey._valueCalculation == null)
                    subStorey._valueCalculation = this._valueCalculation;
                if (subStorey._valueCalculation2 == null)
                    subStorey._valueCalculation2 = this._valueCalculation2;
                if (subStorey._section == NewsSection.None)
                    subStorey._section = this._section;
                if (subStorey._cycle == CycleMode.None)
                    story.AddRange(subStorey.CalculateStory(p));
                else
                    story.AddRange(subStorey.CalculateStory());
            }
            return story;
        }

        public static DuckNews Parse(DXMLNode rootElement)
        {
            DuckNews duckNews1 = new DuckNews();
            DXMLAttribute dxmlAttribute1 = rootElement.Attributes("name").FirstOrDefault<DXMLAttribute>();
            if (dxmlAttribute1 != null)
                duckNews1._name = dxmlAttribute1.Value;
            foreach (DXMLNode element in rootElement.Elements())
            {
                if (element.Name == "Section")
                {
                    DXMLAttribute dxmlAttribute2 = element.Attributes("name").FirstOrDefault<DXMLAttribute>();
                    if (dxmlAttribute2 != null)
                    {
                        try
                        {
                            duckNews1._section = (NewsSection)Enum.Parse(typeof(NewsSection), dxmlAttribute2.Value);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
                else if (element.Name == "Requirement")
                {
                    Script.activeProfile = Profiles.DefaultPlayer1;
                    duckNews1._requirements.Add(ScriptStatement.Parse(element.Value + " "));
                }
                else if (element.Name == "Dialogue")
                {
                    DXMLAttribute dxmlAttribute3 = element.Attributes("value").FirstOrDefault<DXMLAttribute>();
                    if (dxmlAttribute3 != null)
                        duckNews1._dialogue.Add(dxmlAttribute3.Value);
                }
                else if (element.Name == "VALUE")
                {
                    Script.activeProfile = Profiles.DefaultPlayer1;
                    duckNews1._valueCalculation = ScriptStatement.Parse(element.Value + " ");
                }
                else if (element.Name == "VALUE2")
                {
                    Script.activeProfile = Profiles.DefaultPlayer1;
                    duckNews1._valueCalculation2 = ScriptStatement.Parse(element.Value + " ");
                }
                else if (element.Name == "Cycle")
                {
                    DXMLAttribute dxmlAttribute4 = element.Attributes("value").FirstOrDefault<DXMLAttribute>();
                    if (dxmlAttribute4 != null)
                        duckNews1._cycle = (CycleMode)Enum.Parse(typeof(CycleMode), dxmlAttribute4.Value);
                }
                else if (element.Name == "SubStory")
                {
                    DuckNews duckNews2 = DuckNews.Parse(element);
                    duckNews1._subStories.Add(duckNews2);
                }
            }
            return duckNews1;
        }

        public ScriptStatement valueCalculation => this._valueCalculation;

        public ScriptStatement valueCalculation2 => this._valueCalculation2;
    }
}
