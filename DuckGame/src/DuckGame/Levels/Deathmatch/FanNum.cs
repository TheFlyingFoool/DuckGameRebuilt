namespace DuckGame
{
    public class FanNum
    {
        public Profile profile;
        public int loyalFans;
        public int unloyalFans;

        public int totalFans => loyalFans + unloyalFans;
    }
}
