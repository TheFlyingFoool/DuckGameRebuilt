using System;

namespace DuckGame
{
    public static class MessageBoxFna
    {
        public static void Show(string message, string title = "Info", Icon icon = Icon.Info)
        {
            if (Program.IS_SDL2) {
                SDL2.SDL.SDL_ShowSimpleMessageBox((SDL2.SDL.SDL_MessageBoxFlags)icon, title ?? "", message ?? "", IntPtr.Zero);
            }
            else {
                SDL3.SDL.SDL_ShowSimpleMessageBox((SDL3.SDL.SDL_MessageBoxFlags)icon, title ?? "", message ?? "", IntPtr.Zero);
            }
        }

        public enum Icon : uint
        {
            Error = 0x10,
            Warning = 0x20,
            Info = 0x40,
        }
    }
}
