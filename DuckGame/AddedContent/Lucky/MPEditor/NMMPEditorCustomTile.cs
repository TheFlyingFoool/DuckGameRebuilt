using System.Text;

namespace DuckGame
{
    [ClientOnly]
    [FixedNetworkID(30246)]
    public class NMMPEditorCustomTile : NMEvent, IMPEditorMessage
    {
        public uint editorSession;
        public byte slot;
        public uint version;
        public byte writer;
        public CustomTileDataChunk tile;

        public NMMPEditorCustomTile() => manager = BelongsToManager.EventManager;

        public NMMPEditorCustomTile(uint pEditorSession, byte pSlot, uint pVersion, byte pWriter, CustomTileDataChunk pTile)
        {
            manager = BelongsToManager.EventManager;
            editorSession = pEditorSession;
            slot = pSlot;
            version = pVersion;
            writer = pWriter;
            tile = pTile;
        }

        protected override void OnSerialize()
        {
            serializedData.Write(editorSession);
            serializedData.Write(slot);
            serializedData.Write(version);
            serializedData.Write(writer);
            serializedData.Write(tile != null);
            if (tile == null)
                return;
            serializedData.Write(tile.path);
            
            serializedData.Write(new BitBuffer(Encoding.UTF8.GetBytes(tile.textureData ?? "")), true);
            serializedData.Write(tile.verticalWidthThick);
            serializedData.Write(tile.verticalWidth);
            serializedData.Write(tile.horizontalHeight);
            serializedData.Write(tile.leftNubber);
            serializedData.Write(tile.rightNubber);
            serializedData.Write(tile.textureChecksum);
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            editorSession = msg.ReadUInt();
            slot = msg.ReadByte();
            version = msg.ReadUInt();
            writer = msg.ReadByte();
            if (!msg.ReadBool()) return;
            tile = new CustomTileDataChunk
            {
                path = msg.ReadString(),
                textureData = Encoding.UTF8.GetString(msg.ReadBitBuffer().GetBytes()),
                verticalWidthThick = msg.ReadInt(),
                verticalWidth = msg.ReadInt(),
                horizontalHeight = msg.ReadInt(),
                leftNubber = msg.ReadBool(),
                rightNubber = msg.ReadBool(),
                textureChecksum = msg.ReadUInt()
            };
        }

        public override void Activate() => MPEditorNet.OnCustomTile(this);
    }
}
