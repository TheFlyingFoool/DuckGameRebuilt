namespace DuckGame
{
    [ClientOnly]
    [FixedNetworkID(30245)]
    public class NMMPEditorTesterGhost : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public Vec2 position;
        public byte frame;
        public bool flip;

        public NMMPEditorTesterGhost() => manager = BelongsToManager.EventManager;

        public NMMPEditorTesterGhost(uint pEditorSession, Vec2 pPosition, byte pFrame, bool pFlip)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            position = pPosition;
            frame = pFrame;
            flip = pFlip;
        }

        public override void Activate() => MPEditorNet.OnTesterGhost(this);
    }
}
