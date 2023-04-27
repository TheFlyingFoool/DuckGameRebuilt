using AddedContent.Hyeve.PolyRender;

namespace DuckGame
{
    public static class FireDebug
    {
        public static bool Debugging => MonoMain.firebreak;

        [DevConsoleCommand]
        public static bool KFP()
        {
            return TeamSelect2.KillsForPoints ^= true;
        }

        [DrawingContext(DrawingLayer.Console, DoDraw = false)]
        public static void FireDebugDraw()
        {
            float x = 0;
            float y = 0;
            float w = Layer.Console.width;
            float h = Layer.Console.height;
            Rectangle bounds = new(x, y, w / 2, h / 2);
            
            Graphics.DrawDottedRect(bounds.tl, bounds.br, Color.Red, 1.6f, 2f);
            
            // Graphics.PushLayerScissor(bounds);
            Graphics.device.ScissorRectangle = bounds;
            
            Graphics.DrawRect(new Rectangle(Mouse.positionConsole - new Vec2(4f), 8f, 8f), Color.Red, 1.6f);
        }

        [PostInitialize]
        public static void OnPostInitialize()
        {
            if (!Debugging)
                return;
            
            foreach (Furniture furni in RoomEditor.AllFurnis())
            {
                Profiles.experienceProfile.SetNumFurnitures(furni.index, 999);
            }

            RoomEditor.maxFurnitures = int.MaxValue;
            
            if (Profiles.experienceProfile.xp < 100000)
            {
                Profiles.experienceProfile.xp = 100000;
            }
            
            //Profiles.experienceProfile.unlocks = new List<string>()
            //{
            //    "MOOGRAV",
            //    "HELMY",
            //    "EXPLODEYCRATES",
            //    "INFAMMO",
            //    "GUNEXPL",
            //    "HATTY2",
            //    "HATTY1",
            //    "WINPRES",
            //    "SHOESTAR",
            //    "QWOPPY",
            //    "JETTY",
            //    "CORPSEBLOW",
            //    "BASEMENTKEY",
            //    "ULTIMATE"
            //};
        }
    }
}