using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    public static Vec2 topleft = new Vec2(0f, 0f);
    public static Vec2 bottomright = new Vec2(100f, 100f);
    //private static float offset = 4000000.0f;
    //public static float cellsize = 100f;
    public static void drawthething()
    {
        //Buckets.Keys
        //if (Level.current != null)
        //{
        //    //Vec2 offset = new Vec2(0f, 0f);
        //    //for (int x = 0; x < 21; x++)
        //    //{
        //    //    for (int y = 0; y < 21; y++)
        //    //    {
        //    //        DuckGame.Graphics.DrawRect(new Vec2(bottomright.x * x, bottomright.y * y), new Vec2(bottomright.x * (x + 1), bottomright.y * (y + 1)), Color.Orange * 0.8f, (Depth)1f, false, 0.5f);
        //    //    }
        //    //}
        //    float offset = QuadTreeObjectList.offset / QuadTreeObjectList.cellsize;
        //    float suboffset = QuadTreeObjectList.cellsize / 4;
        //    foreach (Vec2 bucket in Level.current.things.Buckets.Keys)
        //    {
        //        foreach(Thing t in Level.current.things.Buckets[bucket][typeof(Thing)])
        //        {
        //            DuckGame.Graphics.DrawRect(t.topLeft,t.bottomRight, Color.Orange * 0.8f, (Depth)1f, false, 0.5f);

        //        }
        //        Graphics.DrawString(bucket.x.ToString() + " " + bucket.y.ToString(), new Vec2(((bucket.x - offset) * QuadTreeObjectList.cellsize) + suboffset, ((bucket.y - offset) * QuadTreeObjectList.cellsize) + suboffset), Color.Green, default(Depth), null, 0.8f);
        //        DuckGame.Graphics.DrawRect(new Vec2((bucket.x - offset) * QuadTreeObjectList.cellsize, (bucket.y - offset) * QuadTreeObjectList.cellsize), new Vec2((bucket.x - offset + 1) * QuadTreeObjectList.cellsize, (bucket.y - offset + 1) * QuadTreeObjectList.cellsize), Color.Orange * 0.8f, (Depth)1f, false, 0.5f);

        //    }
        //}


    }
    public static bool graphicculling = true;
    [DevConsoleCommand(Name = "graphiccull")]
    public static void GraphiCullToggle()
    {
        graphicculling = !graphicculling;
        DevConsole.Log("Graphi Cull " + graphicculling.ToString());
    }
    public static bool runv2;
    [DevConsoleCommand(Name ="dantest")]
    public static void DanTest()
    {
        //Level.CheckRectAllDan<MaterialThing>(new Vec2(-1100.6f, -414.2592f), new Vec2(800.3334f, 497.3408f));
        // runv2 = !runv2;
        //DevConsole.Log(runv2.ToString());
        Vec2 vec = Vec2.Zero;
        Vec2 vec2 = Vec2.Zero;
        foreach (Duck d in Level.current.things[typeof(Duck)])
        {
            vec = d.topLeft + new Vec2(0f, 0.5f);
            vec2 = d.bottomRight + new Vec2(0f, -0.5f);
            break;
        }
        int count = Level.current.CollisionRectAll<MaterialThing>(vec, vec2, null).Count;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        for (int i = 0; i < 10000; i++)
        {
            Level.current.CollisionRectAll<MaterialThing>(vec, vec2, null);
        }
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;

        string elapsedTime = ts.TotalMilliseconds.ToString();
        DevConsole.Log("RunTime  " + count.ToString() + " "+ elapsedTime);
        count = Level.CheckRectAll<MaterialThing>(vec, vec2, null).Count;
        stopWatch = new Stopwatch();
        stopWatch.Start();
        for (int i = 0; i < 10000; i++)
        {//CollisionRectAllDan
            
            Level.CheckRectAllDan<MaterialThing>(vec, vec2);
        }
        stopWatch.Stop();
        ts = stopWatch.Elapsed;
        elapsedTime = ts.TotalMilliseconds.ToString();
        DevConsole.Log("RunTime2 " + count.ToString() + " " + elapsedTime);
    }
    public static unsafe void Test2()
    {
        DevConsole.Log(":PPP");
        Tex2D t;
        Tex2D t3;
        t = Content.Load<Tex2D>("defaultMod");
        Tex2D t2 = t;
        t3 = Content.Load<Tex2D>("defaultMod");
        TypedReference tr = __makeref(t);
        IntPtr ptr = **(IntPtr**)(&tr);
        TypedReference tr2 = __makeref(t2);
        IntPtr ptr2 = **(IntPtr**)(&tr2);
        TypedReference tr3 = __makeref(t3);
        IntPtr ptr3 = **(IntPtr**)(&tr3);
        DevConsole.Log(t.GetHashCode().ToString() + " " + ptr.ToString());
        DevConsole.Log(t2.GetHashCode().ToString() + " " + ptr2.ToString());
        DevConsole.Log(t3.GetHashCode().ToString() + " " + ptr3.ToString());
    }
    [DevConsoleCommand]
    public static void Test()
    {

        /*RoomEditorExtra.favoriteHats.Add(1);
        RoomEditorExtra.favoriteHats.Add(9);
        RoomEditorExtra.favoriteHats.Add(8);
        RoomEditorExtra.favoriteHats.Add(4);

        BitBuffer b = RoomEditorExtra.room1;
        b.Write(255);
        b.Write(false);
        b.Write("E");
        RoomEditorExtra.room1 = b;

        AutoConfigHandler.SaveAll(false);
        DevConsole.Log("yay");*/
    }
   
}