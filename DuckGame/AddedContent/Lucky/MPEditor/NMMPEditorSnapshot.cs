namespace DuckGame
{
    [ClientOnly]
    [FixedNetworkID(30243)]
    public class NMMPEditorSnapshot : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public bool reset;
        private byte[] _payload;

        public byte[] payload => _payload;

        public NMMPEditorSnapshot() => manager = BelongsToManager.EventManager;

        public NMMPEditorSnapshot(uint pEditorSession, bool pReset, byte[] pPayload)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            reset = pReset;
            _payload = pPayload;
        }

        protected override void OnSerialize()
        {
            serializedData.Write(editorSession);
            serializedData.Write(reset);
            serializedData.Write(new BitBuffer(_payload), true);
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            editorSession = msg.ReadUInt();
            reset = msg.ReadBool();
            _payload = msg.ReadBitBuffer().GetBytes();
        }

        public override void Activate() => MPEditorNet.OnSnapshot(this);
    }
}
