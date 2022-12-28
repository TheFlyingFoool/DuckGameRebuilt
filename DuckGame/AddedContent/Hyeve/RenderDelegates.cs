using System;
using DuckGame;

namespace AddedContent.Hyeve
{
    public class RenderDelegates
    {
        public class Layers
        {
            public static void InvokeFor(Layer layer)
            {
                switch(layer.name)
                {
                    case "PREDRAW":
                        AfterPredraw?.Invoke();
                        break;
                    case "PARALLAX":
                        AfterParallax?.Invoke();
                        break;
                    case "VIRTUAL":
                        AfterVirtual?.Invoke();
                        break;
                    case "BACKGROUND":
                        AfterBackground?.Invoke();
                        break;
                    case "GAME":
                        AfterGame?.Invoke();
                        break;
                    case "BLOCKS":
                        AfterBlocks?.Invoke();
                        break;
                    case "GLOW":
                        AfterGlow?.Invoke();
                        break;
                    case "LIGHTING":
                        AfterLighting?.Invoke();
                        break;
                    case "FOREGROUND":
                        AfterForeground?.Invoke();
                        break;
                    case "HUD":
                        AfterHUD?.Invoke();
                        break;
                    case "CONSOLE":
                        AfterConsole?.Invoke();
                        break;
                }
            }
            
            public static Action AfterPredraw;
            public static Action AfterParallax;
            public static Action AfterVirtual;
            public static Action AfterBackground;
            public static Action AfterGame;
            public static Action AfterBlocks;
            public static Action AfterGlow;
            public static Action AfterLighting;
            public static Action AfterForeground;
            public static Action AfterHUD;
            public static Action AfterConsole;
        }

        public static Action BeforeScreen;
        public static Action AfterScreen;
    }
}