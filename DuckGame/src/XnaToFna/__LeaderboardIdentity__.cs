using MonoMod;

namespace XnaToFna.StubXDK.GamerServices
{
    public static class __LeaderboardIdentity__
    {
        [MonoModHook("System.Void Microsoft.Xna.Framework.GamerServices.LeaderboardIdentity::set_Key(System.String)")]
        public static void set_Key(ref object identity, string value)
        {
        }

        [MonoModHook("System.String Microsoft.Xna.Framework.GamerServices.LeaderboardIdentity::get_Key()")]
        public static string get_Key(ref object identity) => "";
    }
}
