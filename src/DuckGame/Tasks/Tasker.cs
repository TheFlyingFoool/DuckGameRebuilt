// Decompiled with JetBrains decompiler
// Type: DuckGame.Tasker
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            lock (Tasker._taskLock)
            {
                while (Tasker._promises.Count != 0 && max > 0U)
                {
                    --max;
                    Tasker._promises.Dequeue().Execute();
                }
            }
        }

        private static bool IsMainThread => Thread.CurrentThread == MonoMain.mainThread;

        public static Promise<T> Task<T>(Func<T> function)
        {
            lock (Tasker._taskLock)
            {
                Promise<T> promise = new Promise<T>(function);
                if (Tasker.IsMainThread)
                    promise.Execute();
                else
                    Tasker._promises.Enqueue((Promise)promise);
                return promise;
            }
        }

        public static Promise Task(Action function)
        {
            lock (Tasker._taskLock)
            {
                Promise promise = new Promise(function);
                if (Tasker.IsMainThread)
                    promise.Execute();
                else
                    Tasker._promises.Enqueue(promise);
                return promise;
            }
        }
    }
}
