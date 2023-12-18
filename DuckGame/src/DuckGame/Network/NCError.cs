namespace DuckGame
{
    public class NCError
    {
        public string text;
        public NCErrorType type;

        public NCError(string s, NCErrorType tp)
        {
            text = s;
            type = tp;
        }

        public Color color
        {
            get
            {
                switch (type)
                {
                    case NCErrorType.Success:
                        return Color.Lime;
                    case NCErrorType.Message:
                        return Color.White;
                    case NCErrorType.Warning:
                        return Color.Yellow;
                    case NCErrorType.Debug:
                        return Color.LightPink;
                    default:
                        return Color.Red;
                }
            }
        }
    }
}
