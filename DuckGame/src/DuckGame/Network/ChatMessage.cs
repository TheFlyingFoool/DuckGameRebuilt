// Decompiled with JetBrains decompiler
// Type: DuckGame.ChatMessage
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        public bool newLinesAdded;
        public bool addedToReplay;

        public ChatMessage(Profile w, string t, ushort idx)
        {
            who = w;
            text = t;
            index = idx;
        }
    }
}
