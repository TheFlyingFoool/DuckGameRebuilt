using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class LeaderboardEntryExtensions
    {
        [MonoModHook("System.Int32 XnaToFna.StubXDK.GamerServices.LeaderboardEntryExtensions::GetRank(Microsoft.Xna.Framework.GamerServices.LeaderboardEntry)")]
        public static int GetRank(object entry) => 0;
    }
}
