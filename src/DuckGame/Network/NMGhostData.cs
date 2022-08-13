// Decompiled with JetBrains decompiler
// Type: DuckGame.NMGhostData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DuckGame
{
    public class NMGhostData : NetMessage, INetworkChunk
    {
        public new byte levelIndex;
        public List<NMGhostState> states = new List<NMGhostState>();
        private List<GhostObject> _ghostSelection;
        private int _startIndex;
        private const int kMaxDataSize = 512;
        public List<NMGhostData.GhostMaskPair> ghostMaskPairs = new List<NMGhostData.GhostMaskPair>();

        public static NMGhostData GetSerializedGhostData(
          List<GhostObject> pGhosts,
          int pStartIndex)
        {
            NMGhostData serializedGhostData = new NMGhostData
            {
                _ghostSelection = pGhosts,
                _startIndex = pStartIndex
            };
            serializedGhostData.Serialize();
            return serializedGhostData;
        }

        public NMGhostData() => manager = BelongsToManager.GhostManager;

        protected override void OnSerialize()
        {
            _serializedData.Write(DuckNetwork.levelIndex);
            int position = _serializedData.position;
            _serializedData.Write((byte)0);
            ushort val = ushort.MaxValue;
            for (int startIndex = _startIndex; startIndex < _ghostSelection.Count; ++startIndex)
            {
                GhostObject ghostObject = _ghostSelection[startIndex];
                if (_serializedData.lengthInBytes + ghostObject.previouslySerializedData.lengthInBytes <= 512)
                {
                    if (val == ushort.MaxValue || val != ghostObject.thing.ghostType)
                    {
                        val = ghostObject.thing.ghostType;
                        _serializedData.Write(true);
                        _serializedData.Write(val);
                    }
                    else
                        _serializedData.Write(false);
                    ghostMaskPairs.Add(new NMGhostData.GhostMaskPair()
                    {
                        ghost = ghostObject,
                        mask = ghostObject.lastWrittenMask
                    });
                    _serializedData.Write(ghostObject.previouslySerializedData, true);
                }
                else
                    break;
            }
            _serializedData.position = position;
            _serializedData.bitOffset = 0;
            _serializedData.Write((byte)ghostMaskPairs.Count);
        }

        public override void OnDeserialize(BitBuffer pData)
        {
            levelIndex = pData.ReadByte();
            ushort num1 = pData.ReadByte();
            ushort num2 = 0;
            for (int index = 0; index < num1; ++index)
            {
                if (pData.ReadBool())
                    num2 = pData.ReadUShort();
                BitBuffer msg = pData.ReadBitBuffer();
                NMGhostState nmGhostState1 = new NMGhostState
                {
                    minimalState = true,
                    packet = packet
                };
                NMGhostState nmGhostState2 = nmGhostState1;
                if (num2 == 0)
                    nmGhostState2.header.id = (NetIndex16)msg.ReadUShort();
                else
                    nmGhostState2.Deserialize(msg);
                nmGhostState2.header.levelIndex = levelIndex;
                nmGhostState2.connection = connection;
                nmGhostState2.header.classID = num2;
                if (!nmGhostState2.header.delta)
                    nmGhostState2.header.tick = (NetIndex16)1;
                states.Add(nmGhostState2);
            }
        }

        //private void Compress(BitBuffer pCompress)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    BinaryWriter binaryWriter = new BinaryWriter(new GZipStream(memoryStream, CompressionMode.Compress));
        //    binaryWriter.Write((ushort)pCompress.lengthInBytes);
        //    binaryWriter.Write(pCompress.buffer, 0, pCompress.lengthInBytes);
        //    binaryWriter.Close();
        //    byte[] array = memoryStream.ToArray();
        //    this._serializedData.Write((ushort)array.Length);
        //    this._serializedData.Write(array, 0, -1);
        //}

        //private BitBuffer Decompress(BitBuffer pData)
        //{
        //    ushort bytes = pData.ReadUShort();
        //    BinaryReader binaryReader = new BinaryReader(new GZipStream(new MemoryStream(pData.ReadPacked(bytes)), CompressionMode.Decompress));
        //    return new BitBuffer(binaryReader.ReadBytes(binaryReader.ReadUInt16()));
        //}

        public struct GhostMaskPair
        {
            public GhostObject ghost;
            public long mask;
        }
    }
}
