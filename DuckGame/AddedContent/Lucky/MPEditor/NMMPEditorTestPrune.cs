using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    [FixedNetworkID(30244)]
    public class NMMPEditorTestPrune : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public uint testKey;
        public List<ushort> removed = new List<ushort>();

        public NMMPEditorTestPrune() => manager = BelongsToManager.EventManager;

        public NMMPEditorTestPrune(uint pEditorSession, uint pTestKey, List<ushort> pRemoved)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            testKey = pTestKey;
            removed = pRemoved;
        }

        protected override void OnSerialize()
        {
            serializedData.Write(editorSession);
            serializedData.Write(testKey);
            serializedData.Write(removed.Count);
            foreach (ushort id in removed)
                serializedData.Write(id);
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            editorSession = msg.ReadUInt();
            testKey = msg.ReadUInt();
            int count = msg.ReadInt();
            removed = new List<ushort>(count);
            for (int i = 0; i < count; ++i)
                removed.Add(msg.ReadUShort());
        }

        public override void Activate() => MPEditorNet.OnTestPrune(this);
    }
}
