// Decompiled with JetBrains decompiler
// Type: DuckGame.NMVoteToSkip
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public NMVoteToSkip(Profile pProfile) => this.profile = pProfile;

        public override void Activate()
        {
            if (this.profile != null && Level.current is RockScoreboard)
                Vote.RegisterVote(this.profile, VoteType.Skip);
            base.Activate();
        }
    }
}
