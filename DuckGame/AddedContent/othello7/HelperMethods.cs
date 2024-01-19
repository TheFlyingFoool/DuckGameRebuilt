using SDL2;
using System;
using System.Collections.Generic;
namespace DuckGame
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
        public static T FirstOrNull<T>(this IEnumerable<T> sequence) where T : class
        {
            foreach (T item in sequence)
            {
                return item;
            }
            return null;
        }
    }
}
