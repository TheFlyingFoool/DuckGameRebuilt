namespace DuckGame
{
    public interface IMPEditorMessage
    {
    }
    [ClientOnly]
    [FixedNetworkID(30240)]
    public class NMMPEditorPeerState : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public bool testing;
        public uint testKey;
        public bool reply;

        public NMMPEditorPeerState() => manager = BelongsToManager.EventManager;

        public NMMPEditorPeerState(uint pEditorSession, bool pTesting, uint pTestKey, bool pReply)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            testing = pTesting;
            testKey = pTestKey;
            reply = pReply;
        }

        public override void Activate() => MPEditorNet.OnPeerState(this);
    }
}
