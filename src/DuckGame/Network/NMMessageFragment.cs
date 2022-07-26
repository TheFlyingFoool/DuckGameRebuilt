// Decompiled with JetBrains decompiler
// Type: DuckGame.NMMessageFragment
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public static int FragmentsRequired(NetMessage pMessage) => (int)Math.Ceiling((double)pMessage.serializedData.lengthInBytes / 700.0);

        public static List<NMMessageFragment> BreakApart(NetMessage pMessage)
        {
            List<NMMessageFragment> nmMessageFragmentList = new List<NMMessageFragment>();
            int num1 = NMMessageFragment.FragmentsRequired(pMessage);
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
                byte[] numArray = new byte[(int)nmMessageFragment.length];
                Array.Copy((Array)pMessage.serializedData.buffer, index * 700, (Array)numArray, 0, (int)nmMessageFragment.length);
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
            if (this.mod != null)
            {
                if (this.mod is CoreMod)
                {
                    DevConsole.Log(DCSection.DuckNet, "|GRAY|Ignoring fragmented message from unknown client mod.");
                    return (NetMessage)null;
                }
                netMessage = this.mod.constructorToMessageID[this.type].Invoke((object[])null) as NetMessage;
            }
            else
                netMessage = Network.constructorToMessageID[this.type].Invoke((object[])null) as NetMessage;
            netMessage.order = this.order;
            netMessage.connection = this.connection;
            netMessage.priority = this.priority;
            netMessage.session = this.session;
            netMessage.typeIndex = data.ReadUShort();
            netMessage.order = this.order;
            netMessage.packet = this.packet;
            netMessage.SetSerializedData(data);
            return netMessage;
        }

        protected override void OnSerialize()
        {
            if (this.finalFragment)
            {
                this._serializedData.Write(true);
                if (this.mod != null)
                {
                    this._serializedData.Write(ushort.MaxValue);
                    this._serializedData.Write(this.type);
                    this._serializedData.Write(this.mod.identifierHash);
                }
                else
                    this._serializedData.Write(this.type);
            }
            else
                this._serializedData.Write(false);
            this._serializedData.Write(this.data, true);
        }

        public override void OnDeserialize(BitBuffer pData)
        {
            this.finalFragment = pData.ReadBool();
            if (this.finalFragment)
            {
                this.type = pData.ReadUShort();
                if (this.type == ushort.MaxValue)
                {
                    this.type = pData.ReadUShort();
                    this.mod = ModLoader.GetModFromHash(pData.ReadUInt());
                    if (this.mod == null)
                        this.mod = (Mod)CoreMod.coreMod;
                }
            }
            this.data = pData.ReadBitBuffer();
        }
    }
}
