using System.Collections.Generic;

namespace DuckGame
{
    public class NMTransferScores : NMEvent
    {
        public List<int> scores = new List<int>();

        public NMTransferScores(List<int> scrs) => scores = scrs;

        public NMTransferScores()
        {
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write((byte)scores.Count);
            foreach (byte score in scores)
                _serializedData.Write(score);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                scores.Add(d.ReadByte());
        }

        public override void Activate()
        {
            int index = 0;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.team != null && index < scores.Count)
                    profile.team.score = scores[index];
                ++index;
            }
            GameMode.RunPostRound(false);
            Send.Message(new NMScoresReceived());
        }
    }
}
