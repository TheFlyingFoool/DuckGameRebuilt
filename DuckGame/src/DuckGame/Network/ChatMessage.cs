using System.Collections.Generic;

namespace DuckGame
{
    public class ChatMessage
    {
        public Profile who;
        /// <summary>
        /// The message as plain Duck Game text (name prefix and color tags, DGR formatting tags stripped).
        /// The chat itself is drawn from <see cref="entries"/>.
        /// </summary>
        public string text;
        public ushort index;
        public float timeout = 10f;
        public float alpha = 1f;
        public float slide;
        public float scale = 1f;
        public int newlines = 1;

        /// <summary>Every message merged into this chat bubble, in order.</summary>
        public List<ChatEntry> entries = new List<ChatEntry>();
        /// <summary>Image sent along with this message, picked up when the message is added to the chat.</summary>
        public ChatImageAttachment image;

        public ChatTextLayout layout;
        public string layoutKey;
        public int layoutAge;

        public ChatMessage(Profile w, string t, ushort idx)
        {
            who = w;
            text = t;
            index = idx;
        }

        public void InvalidateLayout() => layoutKey = null;
    }

    public class ChatEntry
    {
        public Profile who;
        /// <summary>The message text with the language filter applied, including formatting tags.</summary>
        public string text;
        public ChatImageAttachment image;
    }
}
