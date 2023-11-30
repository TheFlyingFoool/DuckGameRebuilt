namespace DuckGame
{
    public class ChatMessage
    {
        public Profile who;
        public string text;
        public ushort index;
        public float timeout = 10f;
        public float alpha = 1f;
        public float slide;
        public float scale = 1f;
        public int newlines = 1;

        public ChatMessage(Profile w, string t, ushort idx)
        {
            who = w;
            text = t;
            index = idx;
        }
    }
}
