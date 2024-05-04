namespace XnaToFna
{
    public static class FakeMonitor
    {
        public static void Enter(object o, ref bool b) => b = true;

        public static void Exit(object o)
        {
        }
    }
}
