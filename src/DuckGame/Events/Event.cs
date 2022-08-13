// Decompiled with JetBrains decompiler
// Type: DuckGame.Event
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public static List<Event> events => Event._events;

        public static void Log(Event e) => Event._events.Add(e);

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
