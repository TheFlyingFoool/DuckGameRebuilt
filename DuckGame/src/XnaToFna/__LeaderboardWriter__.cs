using MonoMod;
using System;
using System.Reflection;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class __LeaderboardWriter__
    {
        private static Type t_LeaderboardEntry;
        private static ConstructorInfo ctor_LeaderboardEntry;

        [MonoModHook("Microsoft.Xna.Framework.GamerServices.LeaderboardEntry Microsoft.Xna.Framework.GamerServices.LeaderboardWriter::GetLeaderboard(Microsoft.Xna.Framework.GamerServices.LeaderboardIdentity)")]
        public static object GetLeaderboard(object writer, object identity)
        {
            if (t_LeaderboardEntry == null)
            {
                t_LeaderboardEntry = StubXDKHelper.GamerServicesAsm.GetType("Microsoft.Xna.Framework.GamerServices.LeaderboardEntry");
                ctor_LeaderboardEntry = t_LeaderboardEntry.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, new Type[0], null);
            }
            return ctor_LeaderboardEntry.Invoke(new object[0]);
        }
    }
}
