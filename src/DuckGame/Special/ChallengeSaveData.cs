// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeSaveData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ChallengeSaveData : Serializable
    {
        public string challenge;
        public TrophyType trophy;
        public int bestTime;
        public int targets;
        public int goodies;
        public string profileID = "";
        public string frameID = "";
        public string frameImage = "";

        public ChallengeSaveData Clone() => new ChallengeSaveData()
        {
            trophy = this.trophy,
            bestTime = this.bestTime,
            profileID = this.profileID,
            targets = this.targets,
            goodies = this.goodies
        };

        public BitBuffer ToBuffer()
        {
            BitBuffer buffer = new BitBuffer(false);
            buffer.Write(this.challenge);
            buffer.Write((int)this.trophy);
            buffer.Write(this.bestTime);
            buffer.Write(this.targets);
            buffer.Write(this.goodies);
            buffer.Write(this.frameID);
            buffer.Write(this.frameImage);
            return buffer;
        }

        public static ChallengeSaveData FromBuffer(BitBuffer pBuffer) => new ChallengeSaveData()
        {
            challenge = pBuffer.ReadString(),
            trophy = (TrophyType)pBuffer.ReadInt(),
            bestTime = pBuffer.ReadInt(),
            targets = pBuffer.ReadInt(),
            goodies = pBuffer.ReadInt(),
            frameID = pBuffer.ReadString(),
            frameImage = pBuffer.ReadString()
        };

        public BinaryClassChunk Serialize()
        {
            BinaryClassChunk element = new BinaryClassChunk();
            this.SerializeField(element, "trophy");
            this.SerializeField(element, "bestTime");
            this.SerializeField(element, "profileID");
            this.SerializeField(element, "targets");
            this.SerializeField(element, "goodies");
            this.SerializeField(element, "frameID");
            this.SerializeField(element, "frameImage");
            return element;
        }

        public bool Deserialize(BinaryClassChunk node)
        {
            this.DeserializeField(node, "trophy");
            this.DeserializeField(node, "bestTime");
            this.DeserializeField(node, "profileID");
            this.DeserializeField(node, "targets");
            this.DeserializeField(node, "goodies");
            this.DeserializeField(node, "frameID");
            this.DeserializeField(node, "frameImage");
            return true;
        }

        public DXMLNode LegacySerialize()
        {
            DXMLNode element = new DXMLNode("challengeSaveData");
            this.LegacySerializeField(element, "trophy");
            this.LegacySerializeField(element, "bestTime");
            this.LegacySerializeField(element, "profileID");
            this.LegacySerializeField(element, "targets");
            this.LegacySerializeField(element, "goodies");
            this.LegacySerializeField(element, "frameID");
            this.LegacySerializeField(element, "frameImage");
            return element;
        }

        public bool LegacyDeserialize(DXMLNode node)
        {
            this.LegacyDeserializeField(node, "trophy");
            this.LegacyDeserializeField(node, "bestTime");
            this.LegacyDeserializeField(node, "profileID");
            this.LegacyDeserializeField(node, "targets");
            this.LegacyDeserializeField(node, "goodies");
            this.LegacyDeserializeField(node, "frameID");
            this.LegacyDeserializeField(node, "frameImage");
            return true;
        }
    }
}
