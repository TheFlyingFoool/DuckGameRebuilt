using System;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// Minimal Chat: while the chat is closed, instead of the messages themselves only a small bubble per player is shown
    /// with how many messages they sent since the chat was last opened. Opening the chat shows everything and marks it all as read.
    /// </summary>
    public static partial class ChatBox
    {
        private sealed class UnreadSender
        {
            public Profile who;
            public int count;
            /// <summary>Goes up with every message, the most recent sender is drawn first.</summary>
            public int order;
            /// <summary>1 right after a new message arrives, used for a little pop.</summary>
            public float pop;
        }

        private const int MaxUnreadShown = 8;
        private static readonly List<UnreadSender> s_unread = new List<UnreadSender>();
        private static int s_unreadOrder;

        private static bool minimalChat => DGRSettings.MinimalChat;

        /// <summary>Called by <see cref="DuckNetworkCore.AddChatMessage"/> for every message that shows up in the chat.</summary>
        public static void OnMessageAdded(Profile who)
        {
            // Our own messages and anything arriving while the chat is open are already read
            if (who == null || who.connection == DuckNetwork.localConnection || core.enteringText)
                return;
            UnreadSender sender = s_unread.Find(x => x.who == who);
            if (sender == null)
            {
                sender = new UnreadSender { who = who };
                s_unread.Add(sender);
            }
            sender.count++;
            sender.order = ++s_unreadOrder;
            sender.pop = 1f;
        }

        /// <summary>Marks everything as read, when the chat is opened or cleared.</summary>
        private static void MarkAllRead() => s_unread.Clear();

        /// <summary>The unread counters drawn in place of the messages while the chat is closed with Minimal Chat on.</summary>
        private static void DrawUnreadCounters(Vec2 screen, float alpha)
        {
            // Players who left don't need a counter anymore
            s_unread.RemoveAll(x => x.who == null || x.who.connection == null && !IsTestProfile(x.who));
            if (s_unread.Count == 0)
                return;
            s_unread.Sort((a, b) => b.order.CompareTo(a.order));

            float chatScale = DuckNetwork.chatScale;
            float head = headOffset;
            float x = 14f;
            float depth = 0.1f;
            int shown = 0;
            foreach (UnreadSender sender in s_unread)
            {
                if (shown++ >= MaxUnreadShown)
                    break;
                if (MonoMain.UpdateLerpState)
                    sender.pop = Math.Max(0f, sender.pop - 0.06f);

                // Same look as a chat bubble, with just the count in it (it pops a little when a new message comes in)
                FancyBitmapFont font = SetupFont(core._chatFont is RasterFont ? 1f : 1f + sender.pop * 0.35f);
                string count = sender.count > 99 ? "99+" : sender.count.ToString();
                float textWidth = MeasurePlain(font, count);
                float height = (font.characterHeight + 2) * font.scale.y;
                float width = head + textWidth + 6f * chatScale;
                Vec2 p1 = new Vec2(x, screen.y - (height + 10f));
                Vec2 p2 = p1 + new Vec2(width, height);

                Graphics.DrawRect(p1 + new Vec2(-1f, -1f), p2 + new Vec2(1f, 1f), Color.Black * 0.8f * alpha, D(depth - 0.0017f), false, 1f * chatScale);
                Color color = DrawChatHead(sender.who, p1, alpha, depth, out bool hasPersona);
                if (hasPersona)
                {
                    color *= 0.85f;
                    color.a = byte.MaxValue;
                }
                Graphics.DrawRect(p1, p2, color * 0.85f * alpha, D(depth - 1f / 500f));
                DrawPlain(font, count, p1.x + 2f + head, p1.y + 1f * font.scale.y, Color.Black * alpha, 1f);

                x = p2.x + 6f * chatScale;
                depth -= 0.01f;
            }
            SetupFont();
        }
    }
}
