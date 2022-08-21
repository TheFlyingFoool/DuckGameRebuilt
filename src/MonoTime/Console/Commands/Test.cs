<<<<<<< Updated upstream
﻿using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
=======
﻿using Microsoft.Xna.Framework.Graphics;
using RectpackSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DuckGame;
public class DirectBitmap : IDisposable
{
    public Bitmap Bitmap { get; private set; }
    public Int32[] Bits { get; private set; }
    public bool Disposed { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }

    protected GCHandle BitsHandle { get; private set; }

    public DirectBitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Bits = new Int32[width * height];
        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    }
    public DirectBitmap(int Width, int Height, Color[] data)
    {
        Bits = new Int32[Width * Height];
        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
               // this.SetPixel(x, y, data[x + y * Width]);
            }
        }
    }
    private static int MakeArgb(byte alpha, byte red, byte green, byte blue)
    {
        return ((int)((ulong)((int)red << 16 | (int)green << 8 | (int)blue | (int)alpha << 24)) & -1);
    }
    private static Color FromArgb(long Value)
    {
        return new Color() { r = (byte)(Value >> 16 & 255L), g = (byte)(Value >> 8 & 255L), b = (byte)(Value & 255L), a = (byte)(Value >> 24 & 255L) };
    }
    public void SetPixel(int x, int y, Color colour)
    {
        int index = x + (y * Width);
        int col = MakeArgb(colour.a,colour.r, colour.g, colour.b);

        Bits[index] = col;
    }

    public Color GetPixel(int x, int y)
    {
        int index = x + (y * Width);
        int col = Bits[index];
        //System.Drawing.Color result = System.Drawing.Color.FromArgb(col);
        return FromArgb(col);
    }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }
}
public static partial class DevConsoleCommands
{
    public static Vec2 topleft = new Vec2(0f, 0f);
    public static Vec2 bottomright = new Vec2(100f, 100f);
    //private static float offset = 4000000.0f;
    //public static float cellsize = 100f;

    static void SaveAsImage(List<KeyValuePair<Texture2D, string>> texs, PackingRectangle[] rectangles, in PackingRectangle bounds, string file)
    {
        DirectBitmap bigimage = new DirectBitmap((int)bounds.Width, (int)bounds.Height);
        for (int x = 0; x < bigimage.Width; x++)
        {
            for (int y = 0; y < bigimage.Height; y++)
            {
                bigimage.SetPixel(x,y, Color.DarkRed);
            }
        }
        List<string> strings = new List<string>();
        for (int i = 0; i < rectangles.Length; i++)
        {
            try
            {
                PackingRectangle r = rectangles[i];
                Texture2D tex = texs[r.Id].Key;
                Color[] data = new Color[tex.Width * tex.Height];
                texs[r.Id].Key.GetData<Color>(data);
                for (int x = 0; x < r.Width; x++)
                {
                    for (int y = 0; y < r.Height; y++)
                    {
                        bigimage.SetPixel(x + (int)r.X, y + (int)r.Y, data[x + y * tex.Width]);
                    }
                }
                //DirectBitmap img = new DirectBitmap((int)r.Width, (int)r.Height, data);
                // img.Bitmap.Save(@"C:\Users\daniel\Documents\GitHub\DuckGamesOG\bin\king\named\" + tex + ".png", ImageFormat.Png);
                //img.Dispose();
                Stream stream = File.Create(@"C:\Users\daniel\Documents\GitHub\DuckGamesOG\bin\king\named\" + texs[r.Id].Value.Replace("/", "__") + ".png");
                tex.SaveAsPng(stream, (int)r.Width, (int)r.Height);
                stream.Dispose();
                // stream.Dispose();
                strings.Add(texs[r.Id].Value + " " + r.X.ToString() + " " + r.Y.ToString() + " " + r.Height.ToString() + " " + r.Width.ToString());
            }
            catch
            { }
        }
        System.IO.File.WriteAllLines(@"C:\Users\daniel\Documents\GitHub\DuckGamesOG\bin\king\" + file + "_offsets.txt",strings);
        bigimage.Bitmap.Save(@"C:\Users\daniel\Documents\GitHub\DuckGamesOG\bin\king\" + file + ".png", ImageFormat.Png);
        bigimage.Dispose();
    }
    public static bool dothing = true;
    public static void savenonexistent()
    {
        List<PackingRectangle> unnamedrectangles = new List<PackingRectangle>();
        List<KeyValuePair<Texture2D, string>> unnamedtexs = new List<KeyValuePair<Texture2D, string>>();
        List<PackingRectangle> rectangles = new List<PackingRectangle>();
        List<KeyValuePair<Texture2D, string>> texs = new List<KeyValuePair<Texture2D, string>>();
        List<Texture2D> inlist = new List<Texture2D>();
        int n = -1;
        int n2 = -1;
        List<string> unneeedtexs = new List<string>() { "shot01", "message", "screen05", "albumpic", "gym", "furni", "ginormoScore", "logo_armature", "arcade/arcadeBackground", "looptex", "civ/desertTileset", "civ/grassTileset", "civ/snowTileset", "civ/grass" };
        foreach (Texture2D thing in MTSpriteBatcher.Texidonthave)
        {
            Texture2D t =thing;
            inlist.Add(t);
            

            if (t.Name != null)
            {
                n += 1;
                string texname = t.Name;
                texs.Add(new KeyValuePair<Texture2D, string>(t, texname));
                rectangles.Add(new PackingRectangle(0, 0, (uint)t.Width, (uint)t.Height, n));
            }
            else
            {

                n2 += 1;
                string texname = n2.ToString() + " unnamed" ;
                unnamedtexs.Add(new KeyValuePair<Texture2D, string>(t, texname));
                unnamedrectangles.Add(new PackingRectangle(0, 0, (uint)t.Width, (uint)t.Height, n2));
            }
           
        }
        PackingRectangle[] arrayrecs = rectangles.OrderBy(p => p.Area).ToArray();
        if (rectangles.Count > 0)
        {

            RectanglePacker.Pack(arrayrecs, out PackingRectangle bounds);
            SaveAsImage(texs, arrayrecs, in bounds, "unseen");
        }
      
        PackingRectangle[] unnamedarrayrecs = unnamedrectangles.OrderBy(p => p.Area).ToArray();
        RectanglePacker.Pack(unnamedarrayrecs, out PackingRectangle unanmedbounds);
        SaveAsImage(unnamedtexs, unnamedarrayrecs, in unanmedbounds, "unnamedunseen");
    }
    public static void drawthething()
    {
        //if (dothing)
        //{
        //    dothing = false;
        //    List<PackingRectangle> rectangles = new List<PackingRectangle>();
        //    List<KeyValuePair<Texture2D, string>> texs = new List<KeyValuePair<Texture2D, string>>();
        //    List<Texture2D> inlist = new List<Texture2D>();
        //    int n = -1;
        //    List<string> unneeedtexs = new List<string>() { "shot01", "message", "screen05", "albumpic", "gym", "furni", "ginormoScore", "logo_armature", "arcade/arcadeBackground", "looptex", "civ/desertTileset", "civ/grassTileset", "civ/snowTileset", "civ/grass" };
        //    foreach (KeyValuePair<string, DuckGame.Tex2D> thing in Content.textures)
        //    {
        //        Texture2D t = (Texture2D)thing.Value;
        //        if (inlist.Contains(t) || unneeedtexs.Contains(t.Name))
        //        {
        //            continue;
        //        }
        //        inlist.Add(t);
        //        n += 1;
        //        texs.Add(new KeyValuePair<Texture2D, string>(t, t.Name));
        //        rectangles.Add(new PackingRectangle(0, 0, (uint)t.Width, (uint)t.Height, n));
        //    }
        //    PackingRectangle[] arrayrecs = rectangles.OrderBy(p => p.Area).ToArray();
        //    RectanglePacker.Pack(arrayrecs, out PackingRectangle bounds);
        //    SaveAsImage(texs, arrayrecs, in bounds, "profit.png");
        //    Console.WriteLine("e");
        //}
       
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

    [DevConsoleCommand(Name = "see")]
    public static void seetheunseen()
    {
        savenonexistent();
        DevConsole.Log("did see " + MTSpriteBatcher.Texidonthave.Count.ToString());
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
>>>>>>> Stashed changes
    [DevConsoleCommand]
    public static void Test(bool one, bool two = false, string three = "three")
    {
        if (one)
            DevConsole.Log("one");
        if (two)
            DevConsole.Log("two");
        if (string.IsNullOrEmpty(three))
            DevConsole.Log(three);
    }
}