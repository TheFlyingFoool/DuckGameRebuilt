namespace DuckGame
{
    [ClientOnly]
    [FixedNetworkID(30241)]
    public class NMMPEditorCursor : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public Vec2 position;

        public NMMPEditorCursor() => manager = BelongsToManager.EventManager;

        public NMMPEditorCursor(uint pEditorSession, Vec2 pPosition)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            position = pPosition;
        }

        public override void Activate() => MPEditorNet.OnCursor(this);
    }
}
