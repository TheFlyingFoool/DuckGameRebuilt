namespace DuckGame
{
    public static class ConfigTest
    {
        // [AutoConfigField]
        public static string Test
        {
            get => "test value";
            set => DevConsole.Log(value);
        }
    }
}