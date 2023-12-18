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
