using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class LeaderboardWriterExtensions
    {
        [MonoModHook("System.Void XnaToFna.StubXDK.GamerServices.LeaderboardWriterExtensions::SetScore(Microsoft.Xna.Framework.GamerServices.LeaderboardWriter,System.Int32)")]
        public static void SetScore(object writer, int score)
        {
        }
    }
}
