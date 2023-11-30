using System;
using System.Collections.Generic;
using System.Threading;

namespace DuckGame
{
    public static class Tasker
    {
        internal static object _taskLock = new object();
        private static Queue<Promise> _promises = new Queue<Promise>();

        public static void RunTasks(uint max = 4294967295)
        {
            lock (_taskLock)
            {
                while (_promises.Count != 0 && max > 0U)
                {
                    --max;
                    _promises.Dequeue().Execute();
                }
            }
        }

        private static bool IsMainThread => Thread.CurrentThread == MonoMain.mainThread;

        public static Promise<T> Task<T>(Func<T> function)
        {
            lock (_taskLock)
            {
                Promise<T> promise = new Promise<T>(function);
                if (IsMainThread)
                    promise.Execute();
                else
                    _promises.Enqueue(promise);
                return promise;
            }
        }

        public static Promise Task(Action function)
        {
            lock (_taskLock)
            {
                Promise promise = new Promise(function);
                if (IsMainThread)
                    promise.Execute();
                else
                    _promises.Enqueue(promise);
                return promise;
            }
        }
    }
}
