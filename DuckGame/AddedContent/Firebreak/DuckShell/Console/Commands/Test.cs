using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(DebugOnly = true, To = ImplementTo.DuckShell)]
        public static string Test(Huh whuh, Bwuh muh)
        {
            return $"{(int) whuh}: {whuh}";
        }

        public enum Huh
        {
            Bell,
            Ride,
            Rational,
            Tube,
            Reputation,
        }

        public enum Bwuh
        {
            Rocket,
            Sock,
            Pupil,
            Gravity,
            Pigeon,
            Beginning,
        }
    }
}