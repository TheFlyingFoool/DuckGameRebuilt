// Decompiled with JetBrains decompiler
// Type: DuckGame.SplitScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class SplitScreen
    {
        private static Dictionary<Profile, FollowCam> _cameras = new Dictionary<Profile, FollowCam>();

        public static void Draw()
        {
            Camera camera = Level.current.camera;
            foreach (Profile key in Profiles.active)
            {
                Viewport viewport1 = DuckGame.Graphics.viewport;
                if (key.duck != null)
                {
                    FollowCam followCam1 = (FollowCam)null;
                    if (!SplitScreen._cameras.TryGetValue(key, out followCam1))
                    {
                        followCam1 = SplitScreen._cameras[key] = new FollowCam();
                        Viewport viewport2;
                        if (SplitScreen._cameras.Count == 1)
                        {
                            FollowCam followCam2 = followCam1;
                            viewport2 = DuckGame.Graphics.viewport;
                            int width = viewport2.Width / 2 - 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int height = viewport2.Height / 2 - 2;
                            Viewport viewport3 = new Viewport(0, 0, width, height);
                            followCam2.storedViewport = viewport3;
                        }
                        else if (SplitScreen._cameras.Count == 2)
                        {
                            FollowCam followCam3 = followCam1;
                            viewport2 = DuckGame.Graphics.viewport;
                            int x = viewport2.Width / 2 + 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int width = viewport2.Width / 2 - 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int height = viewport2.Height / 2 - 2;
                            Viewport viewport4 = new Viewport(x, 0, width, height);
                            followCam3.storedViewport = viewport4;
                        }
                        else if (SplitScreen._cameras.Count == 3)
                        {
                            FollowCam followCam4 = followCam1;
                            viewport2 = DuckGame.Graphics.viewport;
                            int y = viewport2.Height / 2 + 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int width = viewport2.Width / 2 - 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int height = viewport2.Height / 2 - 2;
                            Viewport viewport5 = new Viewport(0, y, width, height);
                            followCam4.storedViewport = viewport5;
                        }
                        else
                        {
                            FollowCam followCam5 = followCam1;
                            viewport2 = DuckGame.Graphics.viewport;
                            int x = viewport2.Width / 2 + 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int y = viewport2.Height / 2 + 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int width = viewport2.Width / 2 - 2;
                            viewport2 = DuckGame.Graphics.viewport;
                            int height = viewport2.Height / 2 - 2;
                            Viewport viewport6 = new Viewport(x, y, width, height);
                            followCam5.storedViewport = viewport6;
                        }
                    }
                    DuckGame.Graphics.viewport = followCam1.storedViewport;
                    followCam1.minSize = 160f;
                    followCam1.Clear();
                    if (key.duck.ragdoll != null)
                        followCam1.Add((Thing)key.duck.ragdoll);
                    else if (key.duck._trapped != null)
                        followCam1.Add((Thing)key.duck._trapped);
                    else if (key.duck._cooked != null)
                        followCam1.Add((Thing)key.duck._cooked);
                    else
                        followCam1.Add((Thing)key.duck);
                    Level.current.camera = (Camera)followCam1;
                    followCam1.lerpSpeed = 0.11f;
                    followCam1.DoUpdate();
                    Layer.DrawLayers();
                    DuckGame.Graphics.viewport = viewport1;
                }
            }
            Level.current.camera = camera;
        }
    }
}
