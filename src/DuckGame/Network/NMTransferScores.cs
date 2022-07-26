// Decompiled with JetBrains decompiler
// Type: DuckGame.NMTransferScores
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMTransferScores : NMEvent
    {
        public List<int> scores = new List<int>();

        public NMTransferScores(List<int> scrs) => this.scores = scrs;

        public NMTransferScores()
        {
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            this._serializedData.Write((byte)this.scores.Count);
            foreach (byte score in this.scores)
                this._serializedData.Write(score);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            byte num = d.ReadByte();
            for (int index = 0; index < (int)num; ++index)
                this.scores.Add((int)d.ReadByte());
        }

        public override void Activate()
        {
            int index = 0;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.team != null && index < this.scores.Count)
                    profile.team.score = this.scores[index];
                ++index;
            }
            GameMode.RunPostRound(false);
            Send.Message((NetMessage)new NMScoresReceived());
        }
    }
}
