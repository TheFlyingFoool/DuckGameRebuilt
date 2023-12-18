using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Event
    {
        private static List<Event> _events = new List<Event>();
        protected DateTime _timestamp;
        protected Profile _victim;
        protected Profile _dealer;

        public static List<Event> events => _events;

        public static void Log(Event e) => _events.Add(e);

        public Event(Profile dealerVal, Profile victimVal)
        {
            _victim = victimVal;
            _dealer = dealerVal;
            _timestamp = DateTime.Now;
        }

        public DateTime timestamp => _timestamp;

        public Profile victim => _victim;

        public Profile dealer => _dealer;
    }
}
