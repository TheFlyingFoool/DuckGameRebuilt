using System;
using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// Fake chat messages for testing the chat (the testchat command). They only show up on this screen, nothing is sent.
    /// </summary>
    public static partial class ChatBox
    {
        private static Profile[] s_testProfiles;
        private static readonly Queue<(Profile who, string text)> s_testQueue = new Queue<(Profile, string)>();
        private static int s_testTimer;
        private static int s_testInterval;

        /// <summary>A mix of everything the chat can show: formatting, mojis, links, code, long and multi line messages.</summary>
        private static readonly string[] s_testMessages =
        {
            "gg",
            "lol",
            "rematch?",
            "|B|bold|B|, |I|italic|I|, |U|underline|U| and |S|strikethrough|S|",
            ":happyface: nice one",
            ":yayface::yayface::yayface:",
            ":shrug: idk",
            "check this out https://github.com/TheFlyingFoool/DuckGameRebuilt",
            "use |C|/steal :name:|C| to take someone's moji",
            "|CB|for (int i = 0; i < 3; i++)\n    Quack();|CB|",
            "|RED|red|R| |BLUE|blue|R| |GREEN|green|R| and back to normal",
            "line one\nline two\nline three\nline four",
            "this is a really long message to see how wrapping and the collapsed view handle it, it just keeps going and going like a duck that refuses to stop quacking at the worst possible moment",
            "who keeps taking the sword :angryface:",
            "brb",
        };

        private static Profile[] testProfiles
        {
            get
            {
                if (s_testProfiles == null)
                {
                    s_testProfiles = new[]
                    {
                        new Profile("Player2", varDefaultPersona: Persona.Duck2, varID: "CHATTEST2"),
                        new Profile("Player3", varDefaultPersona: Persona.Duck3, varID: "CHATTEST3"),
                        new Profile("Player4", varDefaultPersona: Persona.Duck4, varID: "CHATTEST4"),
                    };
                }
                return s_testProfiles;
            }
        }

        /// <summary>The fake players have no connection, so they're kept around where real players who left would be dropped.</summary>
        private static bool IsTestProfile(Profile profile) => s_testProfiles != null && Array.IndexOf(s_testProfiles, profile) >= 0;

        /// <summary>
        /// Queues <paramref name="count"/> fake messages from Player2, Player3 and Player4, one every <paramref name="seconds"/>.
        /// </summary>
        /// <returns>False if the chat isn't showing (it's only drawn online).</returns>
        public static bool QueueTestMessages(int count, float seconds)
        {
            if (!Network.isActive || DuckNetwork.localProfile == null)
                return false;
            count = Maths.Clamp(count, 1, 200);
            s_testInterval = Math.Max(0, (int)Math.Round(seconds * 60f));
            Profile[] profiles = testProfiles;
            for (int i = 0; i < count; i++)
                s_testQueue.Enqueue((profiles[Rando.Int(profiles.Length - 1)], s_testMessages[Rando.Int(s_testMessages.Length - 1)]));
            s_testTimer = 0;
            return true;
        }

        /// <summary>Posts the queued fake messages, called every frame from <see cref="Update"/>.</summary>
        private static void UpdateTestMessages()
        {
            if (s_testQueue.Count == 0)
                return;
            if (!Network.isActive)
            {
                s_testQueue.Clear();
                return;
            }
            if (s_testTimer-- > 0)
                return;
            s_testTimer = s_testInterval;
            // With no delay everything goes out at once
            do
            {
                (Profile who, string message) = s_testQueue.Dequeue();
                core.AddChatMessage(new ChatMessage(who, message, core.chatIndex++));
            }
            while (s_testInterval == 0 && s_testQueue.Count > 0);
            SFX.DontSave = 1;
            SFX.Play("chatmessage", 0.8f, Rando.Float(-0.15f, 0.15f));
        }
    }
}
