// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class ChallengeData : Serializable
    {
        private List<ChallengeTrophy> _trophies;
        private string _fileName = "";
        private string _levelID = "";
        public string preview;
        public ChallengeSaveData saveData = new ChallengeSaveData();
        public bool countTargets;
        public bool countGoodies;
        public string name = "BURGER CHALLENGE";
        public string description = "";
        public string goal = "";
        public string prefix = "";
        public string reward = "";
        public string prevchal = "";
        public string requirement = "";
        public int icon;
        private bool _updating;

        public List<ChallengeTrophy> trophies => _trophies;

        public string fileName
        {
            get => _fileName;
            set => _fileName = value;
        }

        public string levelID
        {
            get => _levelID;
            set => _levelID = value;
        }

        public bool hasTimeRequirements
        {
            get
            {
                if (_trophies[0].goodies > 0 || _trophies[0].targets > 0)
                    return true;
                for (int index = 1; index < _trophies.Count; ++index)
                {
                    if (_trophies[index].timeRequirement > 0)
                        return true;
                }
                return false;
            }
        }

        public void LoadSaveData()
        {
        }

        public string GetNameForDisplay() => name.ToUpperInvariant();

        public bool CheckRequirement(Profile p) => ChallengeData.CheckRequirement(p, requirement);

        public static bool CheckRequirement(Profile p, string req)
        {
            TrophyType trophyType = TrophyType.Baseline;
            if (req.Length <= 1)
                return true;
            if (req[0] == 'B')
                trophyType = TrophyType.Bronze;
            if (req[0] == 'S')
                trophyType = TrophyType.Silver;
            if (req[0] == 'G')
                trophyType = TrophyType.Gold;
            if (req[0] == 'P')
                trophyType = TrophyType.Platinum;
            if (trophyType == TrophyType.Baseline)
                return true;
            string str = req.Substring(1, req.Length - 1);
            int int32;
            try
            {
                int32 = Convert.ToInt32(str);
            }
            catch
            {
                return true;
            }
            if (int32 == 0)
                return true;
            int num = 0;
            foreach (ChallengeSaveData challengeSaveData in Challenges.GetAllSaveData(p))
            {
                if (challengeSaveData.trophy >= trophyType)
                    ++num;
            }
            return num >= int32;
        }

        public int GetRequirementValue()
        {
            TrophyType trophyType = TrophyType.Baseline;
            if (requirement.Length <= 1)
                return 0;
            if (requirement[0] == 'B')
                trophyType = TrophyType.Bronze;
            if (requirement[0] == 'S')
                trophyType = TrophyType.Silver;
            if (requirement[0] == 'G')
                trophyType = TrophyType.Gold;
            if (requirement[0] == 'P')
                trophyType = TrophyType.Platinum;
            if (requirement[0] == 'D')
                trophyType = TrophyType.Developer;
            if (trophyType == TrophyType.Baseline)
                return 0;
            string str = requirement.Substring(1, requirement.Length - 1);
            int int32;
            try
            {
                int32 = Convert.ToInt32(str);
            }
            catch
            {
                return 0;
            }
            if (int32 == 0)
                return 0;
            return int32 * (int)trophyType;
        }

        public ChallengeData() => _trophies = new List<ChallengeTrophy>()
    {
      new ChallengeTrophy(this) { type = TrophyType.Baseline },
      new ChallengeTrophy(this) { type = TrophyType.Bronze },
      new ChallengeTrophy(this) { type = TrophyType.Silver },
      new ChallengeTrophy(this) { type = TrophyType.Gold },
      new ChallengeTrophy(this) { type = TrophyType.Platinum },
      new ChallengeTrophy(this) { type = TrophyType.Developer }
    };

        public void Update()
        {
            if (_updating)
                return;
            _updating = true;
            _updating = false;
        }

        public BinaryClassChunk Serialize()
        {
            BinaryClassChunk element = new BinaryClassChunk();
            SerializeField(element, "name");
            SerializeField(element, "description");
            SerializeField(element, "goal");
            SerializeField(element, "reward");
            SerializeField(element, "requirement");
            SerializeField(element, "icon");
            SerializeField(element, "countGoodies");
            SerializeField(element, "countTargets");
            SerializeField(element, "prefix");
            SerializeField(element, "prevchal");
            foreach (ChallengeTrophy trophy in _trophies)
                element.AddProperty("trophy", trophy.Serialize());
            return element;
        }

        public bool Deserialize(BinaryClassChunk node)
        {
            DeserializeField(node, "name");
            DeserializeField(node, "description");
            DeserializeField(node, "goal");
            DeserializeField(node, "reward");
            DeserializeField(node, "requirement");
            DeserializeField(node, "icon");
            DeserializeField(node, "countGoodies");
            DeserializeField(node, "countTargets");
            DeserializeField(node, "prefix");
            DeserializeField(node, "prevchal");
            List<BinaryClassChunk> properties = node.GetProperties<BinaryClassChunk>("trophy");
            int index = 0;
            foreach (BinaryClassChunk node1 in properties)
            {
                ChallengeTrophy challengeTrophy = new ChallengeTrophy(this);
                challengeTrophy.Deserialize(node1);
                _trophies[index] = challengeTrophy;
                ++index;
            }
            return true;
        }

        public DXMLNode LegacySerialize()
        {
            DXMLNode element = new DXMLNode("challengeData");
            LegacySerializeField(element, "name");
            LegacySerializeField(element, "description");
            LegacySerializeField(element, "goal");
            LegacySerializeField(element, "reward");
            LegacySerializeField(element, "requirement");
            LegacySerializeField(element, "icon");
            LegacySerializeField(element, "countGoodies");
            LegacySerializeField(element, "countTargets");
            LegacySerializeField(element, "prefix");
            LegacySerializeField(element, "prevchal");
            foreach (ChallengeTrophy trophy in _trophies)
                element.Add(trophy.LegacySerialize());
            return element;
        }

        public bool LegacyDeserialize(DXMLNode node)
        {
            LegacyDeserializeField(node, "name");
            LegacyDeserializeField(node, "description");
            LegacyDeserializeField(node, "goal");
            LegacyDeserializeField(node, "reward");
            LegacyDeserializeField(node, "requirement");
            LegacyDeserializeField(node, "icon");
            LegacyDeserializeField(node, "countGoodies");
            LegacyDeserializeField(node, "countTargets");
            LegacyDeserializeField(node, "prefix");
            LegacyDeserializeField(node, "prevchal");
            LevelData levelData = DuckFile.LoadLevel(Content.path + "levels/" + prevchal + ".lev");
            if (levelData != null)
                prevchal = levelData.metaData.guid;
            int index = 0;
            foreach (DXMLNode element in node.Elements("challengeTrophy"))
            {
                ChallengeTrophy challengeTrophy = new ChallengeTrophy(this);
                challengeTrophy.LegacyDeserialize(element);
                _trophies[index] = challengeTrophy;
                ++index;
            }
            return true;
        }
    }
}
