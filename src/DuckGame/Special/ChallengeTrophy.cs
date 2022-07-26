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
            get => this._type;
            set => this._type = value;
        }

        public int goodies
        {
            get => this._goodies;
            set
            {
                this._goodies = value;
                this._owner.Update();
            }
        }

        public int targets
        {
            get => this._targets;
            set
            {
                this._targets = value;
                this._owner.Update();
            }
        }

        public int timeRequirement
        {
            get => this._timeRequirement;
            set
            {
                this._timeRequirement = value;
                this._owner.Update();
            }
        }

        public int timeRequirementMilliseconds
        {
            get => this._timeRequirementMilliseconds;
            set
            {
                this._timeRequirementMilliseconds = value;
                this._owner.Update();
            }
        }

        public ChallengeTrophy(ChallengeData owner) => this._owner = owner;

        public Color color
        {
            get
            {
                if (this.type == TrophyType.Bronze)
                    return Colors.Bronze;
                if (this.type == TrophyType.Silver)
                    return Colors.Silver;
                if (this.type == TrophyType.Gold)
                    return Colors.Gold;
                return this.type == TrophyType.Platinum ? Colors.Platinum : Colors.Developer;
            }
        }

        public string colorString
        {
            get
            {
                if (this.type == TrophyType.Bronze)
                    return "|CBRONZE|";
                if (this.type == TrophyType.Silver)
                    return "|CSILVER|";
                if (this.type == TrophyType.Gold)
                    return "|CGOLD|";
                return this.type == TrophyType.Platinum ? "|CPLATINUM|" : "|CDEV|";
            }
        }

        public string name
        {
            get
            {
                if (this.type == TrophyType.Bronze)
                    return "BRONZE";
                if (this.type == TrophyType.Silver)
                    return "SILVER";
                if (this.type == TrophyType.Gold)
                    return "GOLD";
                return this.type == TrophyType.Platinum ? "PLATINUM" : "UR THE BEST";
            }
        }

        public BinaryClassChunk Serialize()
        {
            BinaryClassChunk element = new BinaryClassChunk();
            this.SerializeField(element, "type");
            this.SerializeField(element, "goodies");
            this.SerializeField(element, "targets");
            this.SerializeField(element, "timeRequirement");
            this.SerializeField(element, "timeRequirementMilliseconds");
            return element;
        }

        public bool Deserialize(BinaryClassChunk node)
        {
            this.DeserializeField(node, "type");
            this.DeserializeField(node, "goodies");
            this.DeserializeField(node, "targets");
            this.DeserializeField(node, "timeRequirement");
            this.DeserializeField(node, "timeRequirementMilliseconds");
            return true;
        }

        public DXMLNode LegacySerialize()
        {
            DXMLNode element = new DXMLNode("challengeTrophy");
            this.LegacySerializeField(element, "type");
            this.LegacySerializeField(element, "goodies");
            this.LegacySerializeField(element, "targets");
            this.LegacySerializeField(element, "timeRequirement");
            this.LegacySerializeField(element, "timeRequirementMilliseconds");
            return element;
        }

        public bool LegacyDeserialize(DXMLNode node)
        {
            this.LegacyDeserializeField(node, "type");
            this.LegacyDeserializeField(node, "goodies");
            this.LegacyDeserializeField(node, "targets");
            this.LegacyDeserializeField(node, "timeRequirement");
            this.LegacyDeserializeField(node, "timeRequirementMilliseconds");
            return true;
        }
    }
}
