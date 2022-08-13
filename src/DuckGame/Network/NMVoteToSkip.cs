// Decompiled with JetBrains decompiler
// Type: DuckGame.NMVoteToSkip
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMVoteToSkip : NMEvent
    {
        public Profile profile;

        public NMVoteToSkip()
        {
        }

        public NMVoteToSkip(Profile pProfile) => profile = pProfile;

        public override void Activate()
        {
            if (profile != null && Level.current is RockScoreboard)
                Vote.RegisterVote(profile, VoteType.Skip);
            base.Activate();
        }
    }
}
