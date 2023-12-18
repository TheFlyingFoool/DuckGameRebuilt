using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NMMessageFragment : NMEvent, INetworkChunk
    {
        public Mod mod;
        public const ushort kMaxFragmentSize = 700;
        public bool finalFragment;
        public ushort length = 700;
        public BitBuffer data;
        public ushort type;

        public static int FragmentsRequired(NetMessage pMessage) => (int)Math.Ceiling(pMessage.serializedData.lengthInBytes / 700f);

        public static List<NMMessageFragment> BreakApart(NetMessage pMessage)
        {
            List<NMMessageFragment> nmMessageFragmentList = new List<NMMessageFragment>();
            int num1 = FragmentsRequired(pMessage);
            for (int index = 0; index < num1; ++index)
            {
                NMMessageFragment nmMessageFragment = new NMMessageFragment();
                if (index == num1 - 1)
                {
                    nmMessageFragment.finalFragment = true;
                    int num2 = index * 700;
                    nmMessageFragment.length = (ushort)Math.Min(700, pMessage.serializedData.lengthInBytes - num2);
                    nmMessageFragment.mod = ModLoader.GetModFromTypeIgnoreCore(pMessage.GetType());
                    nmMessageFragment.type = Network.allMessageTypesToID[pMessage.GetType()];
                }
                byte[] numArray = new byte[nmMessageFragment.length];
                Array.Copy(pMessage.serializedData.buffer, index * 700, numArray, 0, nmMessageFragment.length);
                nmMessageFragment.data = new BitBuffer(numArray);
                nmMessageFragmentList.Add(nmMessageFragment);
            }
            return nmMessageFragmentList;
        }

        public NetMessage Finish(List<NMMessageFragment> pFragments)
        {
            BitBuffer data = new BitBuffer();
            foreach (NMMessageFragment pFragment in pFragments)
                data.WriteBufferData(pFragment.data);
            data.WriteBufferData(this.data);
            data.SeekToStart();
            NetMessage netMessage;
            if (mod != null)
            {
                if (mod is CoreMod)
                {
                    DevConsole.Log(DCSection.DuckNet, "|GRAY|Ignoring fragmented message from unknown client mod.");
                    return null;
                }
                netMessage = mod.constructorToMessageID[type].Invoke(null) as NetMessage;
            }
            else
                netMessage = Network.constructorToMessageID[type].Invoke(null) as NetMessage;
            netMessage.order = order;
            netMessage.connection = connection;
            netMessage.priority = priority;
            netMessage.session = session;
            netMessage.typeIndex = data.ReadUShort();
            netMessage.order = order;
            netMessage.packet = packet;
            netMessage.SetSerializedData(data);
            return netMessage;
        }

        protected override void OnSerialize()
        {
            if (finalFragment)
            {
                _serializedData.Write(true);
                if (mod != null)
                {
                    _serializedData.Write(ushort.MaxValue);
                    _serializedData.Write(type);
                    _serializedData.Write(mod.identifierHash);
                }
                else
                    _serializedData.Write(type);
            }
            else
                _serializedData.Write(false);
            _serializedData.Write(data, true);
        }

        public override void OnDeserialize(BitBuffer pData)
        {
            finalFragment = pData.ReadBool();
            if (finalFragment)
            {
                type = pData.ReadUShort();
                if (type == ushort.MaxValue)
                {
                    type = pData.ReadUShort();
                    mod = ModLoader.GetModFromHash(pData.ReadUInt());
                    if (mod == null)
                        mod = CoreMod.coreMod;
                }
            }
            data = pData.ReadBitBuffer();
        }
    }
}
