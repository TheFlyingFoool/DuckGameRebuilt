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
            trophy = trophy,
            bestTime = bestTime,
            profileID = profileID,
            targets = targets,
            goodies = goodies
        };

        public BitBuffer ToBuffer()
        {
            BitBuffer buffer = new BitBuffer(false);
            buffer.Write(challenge);
            buffer.Write((int)trophy);
            buffer.Write(bestTime);
            buffer.Write(targets);
            buffer.Write(goodies);
            buffer.Write(frameID);
            buffer.Write(frameImage);
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
            SerializeField(element, "trophy");
            SerializeField(element, "bestTime");
            SerializeField(element, "profileID");
            SerializeField(element, "targets");
            SerializeField(element, "goodies");
            SerializeField(element, "frameID");
            SerializeField(element, "frameImage");
            return element;
        }

        public bool Deserialize(BinaryClassChunk node)
        {
            DeserializeField(node, "trophy");
            DeserializeField(node, "bestTime");
            DeserializeField(node, "profileID");
            DeserializeField(node, "targets");
            DeserializeField(node, "goodies");
            DeserializeField(node, "frameID");
            DeserializeField(node, "frameImage");
            return true;
        }

        public DXMLNode LegacySerialize()
        {
            DXMLNode element = new DXMLNode("challengeSaveData");
            LegacySerializeField(element, "trophy");
            LegacySerializeField(element, "bestTime");
            LegacySerializeField(element, "profileID");
            LegacySerializeField(element, "targets");
            LegacySerializeField(element, "goodies");
            LegacySerializeField(element, "frameID");
            LegacySerializeField(element, "frameImage");
            return element;
        }

        public bool LegacyDeserialize(DXMLNode node)
        {
            LegacyDeserializeField(node, "trophy");
            LegacyDeserializeField(node, "bestTime");
            LegacyDeserializeField(node, "profileID");
            LegacyDeserializeField(node, "targets");
            LegacyDeserializeField(node, "goodies");
            LegacyDeserializeField(node, "frameID");
            LegacyDeserializeField(node, "frameImage");
            return true;
        }
    }
}
