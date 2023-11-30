using SDL2;
using System;
namespace DuckGame.AddedContent.othello7
{
    public static class HelperMethods //or somethin idk
    {
        public static void OpenURL(String URL)
        {
            if (!DGRSettings.OpenURLsInBrowser)
            {
                Steam.OverlayOpenURL(URL);
                return;
            }

            SDL.SDL_OpenURL(URL);
            HUD.AddPlayerChangeDisplay("@CLIPCOPY@Browser Opened!");
        }
    }
}
