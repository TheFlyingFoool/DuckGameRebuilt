using AddedContent.Firebreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RectpackSharp;
using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DuckGame
{
    public static class DansTestArea
    {
        public static Vec2 topleft = new Vec2(0f, 0f);
        public static Vec2 bottomright = new Vec2(100f, 100f);
        //private static float offset = 4000000.0f;
        //public static float cellsize = 100f;
        static void SaveAsImage(List<KeyValuePair<Texture2D, string>> texs, PackingRectangle[] rectangles, in PackingRectangle bounds, string file)
        {
            int size = Math.Max((int)bounds.Width, (int)bounds.Height);
            DirectBitmap bigimage = new DirectBitmap(size, size);
            for (int x = 0; x < bigimage.Width; x++)
            {
                for (int y = 0; y < bigimage.Height; y++)
                {
                    bigimage.SetPixelDG(x, y, Color.DarkRed);
                }
            }
            List<string> strings = new List<string>();
            for (int i = 0; i < rectangles.Length; i++)
            {
                string texturename = "";
                try
                {
                    PackingRectangle r = rectangles[i];
                    texturename = texs[r.Id].Value;
                    Texture2D tex = texs[r.Id].Key;
                    Color[] data = new Color[tex.Width * tex.Height];
                    texs[r.Id].Key.GetData(data);
                    for (int x = 0; x < r.Width; x++)
                    {
                        for (int y = 0; y < r.Height; y++)
                        {
                            bigimage.SetPixelDG(x + (int)r.X, y + (int)r.Y, data[x + y * tex.Width]);
                        }
                    }
                    if (file == "unsaved")
                    {
                        Stream stream = File.Create(@"..\unnamedtexs\" + texs[r.Id].Value.Replace("/", "__") + ".png");
                        tex.SaveAsPng(stream, (int)r.Width, (int)r.Height);
                        stream.Dispose();
                    }

                    strings.Add(texs[r.Id].Value + " " + r.X.ToString() + " " + r.Y.ToString() + " " + r.Height.ToString() + " " + r.Width.ToString());
                }
                catch (Exception ex)
                {

                    DevConsole.Log("Error handling Texture " + texturename + " " + file + " " + ex.Message, Color.Red);
                }
            }
            File.WriteAllLines(@"..\" + file + "_offsets.txt", strings);
            bigimage.Bitmap.Save(@"..\" + file + ".png", ImageFormat.Png);
            bigimage.Dispose();
        }
        public static bool dothing = true;
        public static void PackTextures(List<Tex2D> Textures, List<string> skip = null, string filename = "")
        {
            if (skip == null)
            {
                skip = new List<string>();
            }
            List<PackingRectangle> rectangles = new List<PackingRectangle>();
            List<KeyValuePair<Texture2D, string>> texs = new List<KeyValuePair<Texture2D, string>>();
            List<Texture2D> inlist = new List<Texture2D>();
            int n = -1;
            foreach (Texture2D thing in Textures)
            {
                Texture2D t = thing;
                if (t.Name != null)
                {
                    if (skip.Contains(t.Name))
                    {
                        continue;
                    }
                }
                n += 1;
                inlist.Add(t);
                string texname = "empty" + n.ToString();
                if (t.Name != null)
                {
                    texname = t.Name;
                }
                texs.Add(new KeyValuePair<Texture2D, string>(t, texname));
                rectangles.Add(new PackingRectangle(0, 0, (uint)t.Width, (uint)t.Height, n));

            }
            PackingRectangle[] arrayrecs = rectangles.OrderBy(p => p.Area).ToArray();
            if (rectangles.Count > 0)
            {

                RectanglePacker.Pack(arrayrecs, out PackingRectangle bounds);
                SaveAsImage(texs, arrayrecs, in bounds, filename);
            }
        }
        public static void TextureInit()
        {
            Tex2D extraButton = new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(HatSelector.ButtonSprite))), "button")
            {
                Namebase = "nikoextraButton"
            };
            Content.textures[extraButton.Namebase] = extraButton;
            foreach (Thing thing in Editor.thingMap.Values) // those texures get created on the fly so we just going to make them here to save
            {
                thing.GeneratePreview(48, 48, true); // IceBlock
                thing.GeneratePreview(32, 32, true); // ItemSpawner
                thing.GeneratePreview(16, 16, true); // menus
            }
        }
        public static void SaveTextures()
        {
            List<string> unneeedtexs = new List<string>() { "shot01", "message", "screen05", "albumpic", "gym", "furni", "ginormoScore", "logo_armature", "arcade/arcadeBackground", "looptex", "civ/desertTileset", "civ/grassTileset", "civ/snowTileset", "civ/grass" };
            List<string> issuestexs = new List<string>() { "arcade/gradient", "arcade/plasma2", "arcade/plasma" };
            unneeedtexs.AddRange(issuestexs);
            TextureInit();
            PackTextures(MTSpriteBatcher.Texidonthave, null, "unsaved"); ;
            PackTextures(Content.textures.Values.ToList(), unneeedtexs, "spriteatlas");
        }
        //[DrawingContext(DrawingLayer.Foreground, CustomID = "test")]
        //public static void DrawTest()
        //{
        //    if (Level.current != null || Level.current.camera != null)
        //    {
        //        Vec2 ingamepos = Level.current.camera.transformScreenVector(Mouse.mousePos);
        //        Graphics.DrawRect(ingamepos, ingamepos + new Vec2(10f), Color.Green);
        //    }
        //}
        [Marker.DrawingContext(Marker.DrawingLayer.Foreground, CustomID = "cells", DoDraw = false)]
        public static void DrawCells()
        {
            //Buckets.Keys
            if (Level.current != null)
            {


                //Vec2 offset = new Vec2(0f, 0f);
                //for (int x = 0; x < 21; x++)
                //{
                //    for (int y = 0; y < 21; y++)
                //    {
                //        DuckGame.Graphics.DrawRect(new Vec2(bottomright.x * x, bottomright.y * y), new Vec2(bottomright.x * (x + 1), bottomright.y * (y + 1)), Color.Orange * 0.8f, (Depth)1f, false, 0.5f);
                //    }
                //}
                float offset = QuadTreeObjectList.offset / QuadTreeObjectList.cellsize;
                float suboffset = QuadTreeObjectList.cellsize / 4;
                foreach (Vec2 bucket in Level.current.things.Buckets.Keys)
                {
                    //foreach (Thing t in Level.current.things.Buckets[bucket][typeof(Thing)])
                    //{
                    //    DuckGame.Graphics.DrawRect(t.topLeft, t.bottomRight, Color.Orange * 0.8f, (Depth)1f, false, 0.5f);
                    //}
                    Graphics.DrawString(bucket.x.ToString() + " " + bucket.y.ToString(), new Vec2(((bucket.x - offset) * QuadTreeObjectList.cellsize) + suboffset, ((bucket.y - offset) * QuadTreeObjectList.cellsize) + suboffset), Color.Green, (Depth)1f, null, 0.8f);
                    Graphics.DrawRect(new Vec2((bucket.x - offset) * QuadTreeObjectList.cellsize, (bucket.y - offset) * QuadTreeObjectList.cellsize), new Vec2((bucket.x - offset + 1) * QuadTreeObjectList.cellsize, (bucket.y - offset + 1) * QuadTreeObjectList.cellsize), Color.Orange * 0.8f, (Depth)1f, false, 0.5f);
                }
            }

        }
        [Marker.DevConsoleCommand]
        public static void graphiccull()
        {
            DGRSettings.GraphicsCulling = !DGRSettings.GraphicsCulling;
            DevConsole.Log("grahpic culling " + DGRSettings.GraphicsCulling.ToString());

        }
        public static bool looking;
        [Marker.DevConsoleCommand(Name = "search")]
        public static void Search()
        {
            if (!looking)
            {
                looking = true;
                Steam.SearchForLobbyWorldwide();
            }
            int num1 = Steam.lobbiesFound;

            DevConsole.Log("----------");

            for (int i = 0; i < num1; ++i)
            {
                Lobby lobby = Network.activeNetwork.core.GetSearchLobbyAtIndex(i);
                DevConsole.Log(lobby.id.ToString() + lobby.name);
            }
            DevConsole.Log("----------");
            if (Steam.lobbySearchComplete)
            {
                looking = false;
            }
        }
        //public static int usedfornonsense = 0;
        //[DevConsoleCommand(Name = "dividebyzero")]
        //public static void mathexpection()
        //{
        //    usedfornonsense = 1 / usedfornonsense;
        //}

        //SPItemPool Needs Additional testing
        //[Marker.DevConsoleCommand(Name = "gc")]
        //public static void gc(int size)
        //{
        //    for (int i = 0; i < MTSpriteBatcher.mTSpriteBatchers.Count; i++)
        //    {
        //        MTSpriteBatcher n = MTSpriteBatcher.mTSpriteBatchers[i];

        //        n.ResetInitializePool(size);
        //        DevConsole.Log(i, n.GetPool().Count);
        //    }
        //    DevConsole.Log("pool sizes reset");
        //}
        //[Marker.DevConsoleCommand(Name = "gcl")]
        //public static void gcl()
        //{
        //    for (int i = 0; i < MTSpriteBatcher.mTSpriteBatchers.Count; i++)
        //    {
        //        MTSpriteBatcher n = MTSpriteBatcher.mTSpriteBatchers[i];
        //        DevConsole.Log(i, n.GetPool().Count);
        //    }
        //}
        [Marker.DevConsoleCommand(Name = "steamjoin")]
        public static void Join(string id)
        {
            ulong id2 = 0;
            try
            {
                id2 = Convert.ToUInt64(id);

            }
            catch
            {
                DevConsole.Log("wrong data type try ulong, its a number");
                return;
            }
            DevConsole.Log("joining");
            Level.current = new JoinServer(id2);
        }
        [Marker.DevConsoleCommand(Name = "lanjoin")]
        public static void LanJoin(string id)
        {
            DevConsole.Log("Trying to join " + id);
            Level.current = new JoinServer(id);
        }
        [Marker.DevConsoleCommand]
        public static void Res(int width, int height, int screenmode)
        {
            if (screenmode < 1 || screenmode > 4)
            {
                DevConsole.Log("Invalid input 1 - 4 = Windowed, Fullscreen, Borderless, Max", Color.Red);
                return;
            }
            Resolution r = new Resolution()
            {
                dimensions = new Vec2(width, height)
            };
            ScreenMode mode = (ScreenMode)(screenmode - 1);
            r.mode = mode;
            Resolution.Set(r);
            Resolution.Apply();
        }
        [Marker.DevConsoleCommand(Name = "windowtoggle",
            To = ImplementTo.DuckHack)]
        public static void windowtoggle()
        {
            windowed = !windowed;
            SDL.SDL_SetWindowBordered(MonoMain.instance.Window.Handle, windowed ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
            DevConsole.Log("Windowed Mode is " + windowed.ToString());
        }
        public static bool windowed = true;// SDL.SDL_SetWindowPosition(Resolution._window, 0, 0);


        [Marker.DevConsoleCommand(Name = "windowpos",
            To = ImplementTo.DuckHack)]
        public static void windowtoggle(int x, int y)
        {
            SDL.SDL_SetWindowPosition(MonoMain.instance.Window.Handle, x, y);
            DevConsole.Log("Set Window Pos is " + x.ToString() + " " + y.ToString());
        }
        [Marker.DevConsoleCommand(Name = "tilescreen")]
        public static void tilescreen(int n)
        {
            int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            int boxWidth = 321;
            int boxHeight = 181;
            if (n != 0)
            {
                // Given dimensions of the plane
                double planeWidth = 1920;
                double planeHeight = 1080;

                // Total number of boxes
                int numBoxes = n;

                // Aspect ratio of each box
                double aspectRatio = 1.77777777778;

                // Calculate the total area of the plane
                double totalArea = planeWidth * planeHeight;

                // Area of each box
                double areaPerBox = totalArea / numBoxes;

                // Calculate the height and width of each box
                // Height (H) is the square root of (Area of Each Box / Aspect Ratio)
                double _boxHeight = Math.Sqrt(areaPerBox / aspectRatio);

                // Width (W) is Aspect Ratio * Height
                double _boxWidth = aspectRatio * _boxHeight;
                boxHeight = (int)_boxHeight + 1;
                boxWidth = (int)_boxWidth + 1;


            }
            for (int x = 0; x < width; x += boxWidth)
            {
                for (int y = 0; y < height; y += boxHeight)
                {
                    Process.Start(Application.ExecutablePath, Program.commandLine + "-nomusic -lanjoiner +screentile " + x.ToString() + " " + y.ToString() + " " + boxWidth.ToString() + " " + boxHeight.ToString());
                }
            }
            //Process.Start(Application.ExecutablePath, Program.commandLine + " +screentile 0 0");
            //Process.Start(Application.ExecutablePath, Program.commandLine + " +screentile 321 0"); //+screentile 0 0
            DevConsole.Log("Tiling with DGs" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width.ToString() + " " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height.ToString());
           // Application.Exit();
         //   Program.main.KillEverything();
          //  Program.main.Exit();
        }
        [Marker.DevConsoleCommand(Name = "crashtest", CanCrash = true,
            To = ImplementTo.DuckHack)]
        public static void crashtest()
        {
            DuckNetwork.CheckVersion(null);
        }
        // SDL.SDL_SetWindowBordered(Resolution._window, true ? SDL.SDL_bool.SDL_FALSE : SDL.SDL_bool.SDL_TRUE); 
        [Marker.DevConsoleCommand(Name = "rlevel",
            To = ImplementTo.DuckHack)]
        public static void randomnesstest2()
        {//Content.GetLevels("pyramid", LevelLocation.Content)
            List<string> levels = Content.GetLevels("pyramid", LevelLocation.Content);
            foreach (string GUID in levels)
            {
                LevelData LevelData = Content.GetLevel(GUID, LevelLocation.Content);
                DevConsole.Log(LevelData.GetPath().ToString() + " level");
            }
            DevConsole.Log(levels.Count.ToString() + " rlevel");
        }
        [Marker.DevConsoleCommand(Name = "random",
            To = ImplementTo.DuckHack)]
        public static void randomnesstest()
        {//Content.GetLevels("pyramid", LevelLocation.Content)
            Random rand = new Random(42069);
            double d = rand.NextDouble();
            DevConsole.Log(d.ToString() + " random");
        }
        [Marker.DevConsoleCommand(Name = "testdg")]
        public static void starttestdg()
        {
            Process.Start(Application.ExecutablePath, Program.commandLine + " -lanjoiner");
            DevConsole.Log("Starting Lan Test bud");
        }
        //RandomSkySay();

        public static void SetControllerLightBar(int index, Color color)
        {
            GamePadState state = FNAPlatform.GetGamePadState(index, GamePadDeadZone.IndependentAxes);
            if (state.IsConnected)
                FNAPlatform.SetGamePadLightBar(index, (Microsoft.Xna.Framework.Color)color);
        }

        [Marker.DevConsoleCommand(Name = "dr",
            To = ImplementTo.DuckHack)]
        public static void debugrandom()
        {
            SetControllerLightBar(Persona.alllist[0].index, Color.Magenta);
            for (int index = 0; index < MonoMain.MaximumGamepadCount; index++)
            {
                SetControllerLightBar(index, Color.Magenta);
            }
           
            //if (Level.current == null || !(Level.current.things[typeof(CityBackground)].FirstOrDefault<Thing>() is CityBackground cityBackground))
            //    return;
            //cityBackground.RandomSkySay();
            //DevConsole.Log("random test");
        }
        [Marker.DevConsoleCommand(Name = "savegraphic",
            To = ImplementTo.DuckHack)]
        public static void seetheunseen()
        {
            SaveTextures();
            DevConsole.Log("wasnt in spriteatlas " + MTSpriteBatcher.Texidonthave.Count.ToString());
        }
        public static bool runv2;
        [Marker.DevConsoleCommand(Name = "dantest",
            To = ImplementTo.DuckHack)]
        public static void DanTest()
        {
            //Level.CheckRectAllDan<MaterialThing>(new Vec2(-1100.6f, -414.2592f), new Vec2(800.3334f, 497.3408f));
            // runv2 = !runv2;
            //DevConsole.Log(runv2.ToString());
            //Vec2 vec = Vec2.Zero;
            //Vec2 vec2 = Vec2.Zero;
            //foreach (Duck d in Level.current.things[typeof(Duck)])
            //{
            //    vec = d.topLeft + new Vec2(0f, 0.5f);
            //    vec2 = d.bottomRight + new Vec2(0f, -0.5f);
            //    break;
            //}
            //int count = Level.current.CollisionRectAll<MaterialThing>(vec, vec2, null).Count;
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //for (int i = 0; i < 10000; i++)
            //{
            //    Level.current.CollisionRectAll<MaterialThing>(vec, vec2, null);
            //}
            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = ts.TotalMilliseconds.ToString();
            //DevConsole.Log("RunTime  " + count.ToString() + " " + elapsedTime);
            //count = Level.CheckRectAll<MaterialThing>(vec, vec2, null).Count;
            //stopWatch = new Stopwatch();
            //stopWatch.Start();
            //for (int i = 0; i < 10000; i++)
            //{//CollisionRectAllDan
            //    Level.CheckRectAllDan<MaterialThing>(vec, vec2);
            //}
            //stopWatch.Stop();
            //ts = stopWatch.Elapsed;
            //elapsedTime = ts.TotalMilliseconds.ToString();
            //DevConsole.Log("RunTime2 " + count.ToString() + " " + elapsedTime);
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
        //[DevConsoleCommand]
        //public static void Test(bool one, bool two = false, string three = "three")
        //{
        //    if (one)
        //        DevConsole.Log("one");
        //    if (two)
        //        DevConsole.Log("two");
        //    if (string.IsNullOrEmpty(three))
        //        DevConsole.Log(three);
        //}
    }
}