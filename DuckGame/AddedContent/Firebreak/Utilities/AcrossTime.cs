using RectpackSharp;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public static class AcrossTime
    {
        private static Dictionary<int, TimedEvent> s_timedEvents = new();
        
        // public static void Do(object? caller, TimeSpan duration, Action<ProgressValue> action,
        //     [CallerArgumentExpression(nameof(action))] string expressionDoNotGiveValue = null)
        // {
        //     int uniqueID = HashCode.Combine(expressionDoNotGiveValue, caller ?? 0);
        //
        //     if (s_timedEvents.TryGetValue(uniqueID, out TimedEvent timedEvent))
        //     {
        //         bool withinDuration = timedEvent.StartTime + timedEvent.Duration > DateTime.Now;
        //
        //         if (!withinDuration)
        //             s_timedEvents.Remove(uniqueID);
        //     }
        //     else
        //     {
        //         timedEvent = new TimedEvent
        //         {
        //             Duration = duration,
        //             Action = action,
        //             StartTime = DateTime.Now
        //         };
        //         
        //         s_timedEvents.Add(uniqueID, timedEvent);
        //     }
        //     
        //     ProgressValue progressValue = ProgressValue.FromTime(
        //         timedEvent.StartTime,                                     // start
        //         timedEvent.StartTime + timedEvent.Duration,               // end
        //         timedEvent.StartTime + timedEvent.Duration - DateTime.Now // now
        //     );
        //     
        //     timedEvent.Action.Invoke(progressValue);
        // }

        private class TimedEvent
        {
            public TimeSpan Duration;
            public Action<ProgressValue> Action;
            public DateTime StartTime;
        }
    }
}