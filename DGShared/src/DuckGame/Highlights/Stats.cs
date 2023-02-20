// Decompiled with JetBrains decompiler
// Type: DuckGame.Stats
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Stats
    {
        private static float _averageRoundTime;
        private static float _totalRoundTime;
        private static int _numberOfRounds;

        public static float averageRoundTime => _totalRoundTime / numberOfRounds;

        public static float totalRoundTime => _averageRoundTime;

        public static int numberOfRounds => _numberOfRounds;

        public static int lastMatchLength
        {
            get
            {
                DateTime dateTime = DateTime.Now;
                for (int index = Event.events.Count - 1; index > 0; --index)
                {
                    Event @event = Event.events[index];
                    switch (@event)
                    {
                        case RoundEndEvent _:
                            dateTime = @event.timestamp;
                            break;
                        case RoundStartEvent _:
                            return (int)(dateTime - @event.timestamp).TotalSeconds;
                    }
                }
                return 99;
            }
        }

        public static void CalculateStats()
        {
            DateTime dateTime = DateTime.Now;
            _totalRoundTime = 0f;
            _numberOfRounds = 0;
            foreach (Event @event in Event.events)
            {
                if (@event is RoundStartEvent)
                    dateTime = @event.timestamp;
                else if (@event is RoundEndEvent)
                {
                    _totalRoundTime += (int)(@event.timestamp - dateTime).TotalSeconds;
                    ++_numberOfRounds;
                }
            }
        }
    }
}
