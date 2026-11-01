using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    [FixedNetworkID(30242)]
    public class NMMPEditorOps : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public List<MPEditorOp> ops = new List<MPEditorOp>();

        public NMMPEditorOps() => manager = BelongsToManager.EventManager;

        public NMMPEditorOps(uint pEditorSession, List<MPEditorOp> pOps)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            ops = pOps;
        }

        protected override void OnSerialize()
        {
            serializedData.Write(editorSession);
            serializedData.Write(ops.Count);
            foreach (MPEditorOp op in ops)
                op.Write(serializedData);
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            editorSession = msg.ReadUInt();
            int count = msg.ReadInt();
            ops = new List<MPEditorOp>(count);
            for (int i = 0; i < count; ++i)
                ops.Add(MPEditorOp.Read(msg));
        }

        public override void Activate() => MPEditorNet.OnOps(this);
    }
}
