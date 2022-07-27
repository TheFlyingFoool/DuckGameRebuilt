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

        public List<ChallengeTrophy> trophies => this._trophies;

        public string fileName
        {
            get => this._fileName;
            set => this._fileName = value;
        }

        public string levelID
        {
            get => this._levelID;
            set => this._levelID = value;
        }

        public bool hasTimeRequirements
        {
            get
            {
                if (this._trophies[0].goodies > 0 || this._trophies[0].targets > 0)
                    return true;
                for (int index = 1; index < this._trophies.Count; ++index)
                {
                    if (this._trophies[index].timeRequirement > 0)
                        return true;
                }
                return false;
            }
        }

        public void LoadSaveData()
        {
        }

        public string GetNameForDisplay() => this.name.ToUpperInvariant();

        public bool CheckRequirement(Profile p) => ChallengeData.CheckRequirement(p, this.requirement);

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
            if (this.requirement.Length <= 1)
                return 0;
            if (this.requirement[0] == 'B')
                trophyType = TrophyType.Bronze;
            if (this.requirement[0] == 'S')
                trophyType = TrophyType.Silver;
            if (this.requirement[0] == 'G')
                trophyType = TrophyType.Gold;
            if (this.requirement[0] == 'P')
                trophyType = TrophyType.Platinum;
            if (this.requirement[0] == 'D')
                trophyType = TrophyType.Developer;
            if (trophyType == TrophyType.Baseline)
                return 0;
            string str = this.requirement.Substring(1, this.requirement.Length - 1);
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

        public ChallengeData() => this._trophies = new List<ChallengeTrophy>()
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
            if (this._updating)
                return;
            this._updating = true;
            this._updating = false;
        }

        public BinaryClassChunk Serialize()
        {
            BinaryClassChunk element = new BinaryClassChunk();
            this.SerializeField(element, "name");
            this.SerializeField(element, "description");
            this.SerializeField(element, "goal");
            this.SerializeField(element, "reward");
            this.SerializeField(element, "requirement");
            this.SerializeField(element, "icon");
            this.SerializeField(element, "countGoodies");
            this.SerializeField(element, "countTargets");
            this.SerializeField(element, "prefix");
            this.SerializeField(element, "prevchal");
            foreach (ChallengeTrophy trophy in this._trophies)
                element.AddProperty("trophy", trophy.Serialize());
            return element;
        }

        public bool Deserialize(BinaryClassChunk node)
        {
            this.DeserializeField(node, "name");
            this.DeserializeField(node, "description");
            this.DeserializeField(node, "goal");
            this.DeserializeField(node, "reward");
            this.DeserializeField(node, "requirement");
            this.DeserializeField(node, "icon");
            this.DeserializeField(node, "countGoodies");
            this.DeserializeField(node, "countTargets");
            this.DeserializeField(node, "prefix");
            this.DeserializeField(node, "prevchal");
            List<BinaryClassChunk> properties = node.GetProperties<BinaryClassChunk>("trophy");
            int index = 0;
            foreach (BinaryClassChunk node1 in properties)
            {
                ChallengeTrophy challengeTrophy = new ChallengeTrophy(this);
                challengeTrophy.Deserialize(node1);
                this._trophies[index] = challengeTrophy;
                ++index;
            }
            return true;
        }

        public DXMLNode LegacySerialize()
        {
            DXMLNode element = new DXMLNode("challengeData");
            this.LegacySerializeField(element, "name");
            this.LegacySerializeField(element, "description");
            this.LegacySerializeField(element, "goal");
            this.LegacySerializeField(element, "reward");
            this.LegacySerializeField(element, "requirement");
            this.LegacySerializeField(element, "icon");
            this.LegacySerializeField(element, "countGoodies");
            this.LegacySerializeField(element, "countTargets");
            this.LegacySerializeField(element, "prefix");
            this.LegacySerializeField(element, "prevchal");
            foreach (ChallengeTrophy trophy in this._trophies)
                element.Add(trophy.LegacySerialize());
            return element;
        }

        public bool LegacyDeserialize(DXMLNode node)
        {
            this.LegacyDeserializeField(node, "name");
            this.LegacyDeserializeField(node, "description");
            this.LegacyDeserializeField(node, "goal");
            this.LegacyDeserializeField(node, "reward");
            this.LegacyDeserializeField(node, "requirement");
            this.LegacyDeserializeField(node, "icon");
            this.LegacyDeserializeField(node, "countGoodies");
            this.LegacyDeserializeField(node, "countTargets");
            this.LegacyDeserializeField(node, "prefix");
            this.LegacyDeserializeField(node, "prevchal");
            LevelData levelData = DuckFile.LoadLevel(Content.path + "levels/" + this.prevchal + ".lev");
            if (levelData != null)
                this.prevchal = levelData.metaData.guid;
            int index = 0;
            foreach (DXMLNode element in node.Elements("challengeTrophy"))
            {
                ChallengeTrophy challengeTrophy = new ChallengeTrophy(this);
                challengeTrophy.LegacyDeserialize(element);
                this._trophies[index] = challengeTrophy;
                ++index;
            }
            return true;
        }
    }
}
