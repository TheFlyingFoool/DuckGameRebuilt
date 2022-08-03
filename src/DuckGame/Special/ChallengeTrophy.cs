// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeTrophy
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ChallengeTrophy : Serializable
    {
        private TrophyType _type;
        private int _goodies = -1;
        private int _targets = -1;
        private int _timeRequirement;
        private int _timeRequirementMilliseconds;
        private ChallengeData _owner;

        public TrophyType type
        {
            get => _type;
            set => _type = value;
        }

        public int goodies
        {
            get => _goodies;
            set
            {
                _goodies = value;
                _owner.Update();
            }
        }

        public int targets
        {
            get => _targets;
            set
            {
                _targets = value;
                _owner.Update();
            }
        }

        public int timeRequirement
        {
            get => _timeRequirement;
            set
            {
                _timeRequirement = value;
                _owner.Update();
            }
        }

        public int timeRequirementMilliseconds
        {
            get => _timeRequirementMilliseconds;
            set
            {
                _timeRequirementMilliseconds = value;
                _owner.Update();
            }
        }

        public ChallengeTrophy(ChallengeData owner) => _owner = owner;

        public Color color
        {
            get
            {
                if (type == TrophyType.Bronze)
                    return Colors.Bronze;
                if (type == TrophyType.Silver)
                    return Colors.Silver;
                if (type == TrophyType.Gold)
                    return Colors.Gold;
                return type == TrophyType.Platinum ? Colors.Platinum : Colors.Developer;
            }
        }

        public string colorString
        {
            get
            {
                if (type == TrophyType.Bronze)
                    return "|CBRONZE|";
                if (type == TrophyType.Silver)
                    return "|CSILVER|";
                if (type == TrophyType.Gold)
                    return "|CGOLD|";
                return type == TrophyType.Platinum ? "|CPLATINUM|" : "|CDEV|";
            }
        }

        public string name
        {
            get
            {
                if (type == TrophyType.Bronze)
                    return "BRONZE";
                if (type == TrophyType.Silver)
                    return "SILVER";
                if (type == TrophyType.Gold)
                    return "GOLD";
                return type == TrophyType.Platinum ? "PLATINUM" : "UR THE BEST";
            }
        }

        public BinaryClassChunk Serialize()
        {
            BinaryClassChunk element = new BinaryClassChunk();
            SerializeField(element, "type");
            SerializeField(element, "goodies");
            SerializeField(element, "targets");
            SerializeField(element, "timeRequirement");
            SerializeField(element, "timeRequirementMilliseconds");
            return element;
        }

        public bool Deserialize(BinaryClassChunk node)
        {
            DeserializeField(node, "type");
            DeserializeField(node, "goodies");
            DeserializeField(node, "targets");
            DeserializeField(node, "timeRequirement");
            DeserializeField(node, "timeRequirementMilliseconds");
            return true;
        }

        public DXMLNode LegacySerialize()
        {
            DXMLNode element = new DXMLNode("challengeTrophy");
            LegacySerializeField(element, "type");
            LegacySerializeField(element, "goodies");
            LegacySerializeField(element, "targets");
            LegacySerializeField(element, "timeRequirement");
            LegacySerializeField(element, "timeRequirementMilliseconds");
            return element;
        }

        public bool LegacyDeserialize(DXMLNode node)
        {
            LegacyDeserializeField(node, "type");
            LegacyDeserializeField(node, "goodies");
            LegacyDeserializeField(node, "targets");
            LegacyDeserializeField(node, "timeRequirement");
            LegacyDeserializeField(node, "timeRequirementMilliseconds");
            return true;
        }
    }
}
