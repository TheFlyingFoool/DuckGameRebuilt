using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DuckGame
{
    /// <summary>
    /// Dropping an image file onto the game window while the chat is open attaches it.
    /// <para>
    /// SDL already turns drops into events, FNA just ignores them, so an SDL event watch picks them up here
    /// (works with both the SDL2 and SDL3 backends, no changes to FNA needed).
    /// </para>
    /// </summary>
    public static partial class ChatBox
    {
        private static bool s_dropHooked;
        private static readonly ConcurrentQueue<string> s_droppedFiles = new ConcurrentQueue<string>();
        // Kept in fields so the garbage collector doesn't take the callbacks away from SDL
        private static SDL2.SDL.SDL_EventFilter s_sdl2DropWatch;
        private static SDL3.SDL.SDL_EventFilter s_sdl3DropWatch;

        private static unsafe void HookFileDrops()
        {
            if (s_dropHooked)
                return;
            s_dropHooked = true;
            try
            {
                if (Program.IS_SDL2)
                {
                    SDL2.SDL.SDL_EventState(SDL2.SDL.SDL_EventType.SDL_DROPFILE, 1);
                    s_sdl2DropWatch = Sdl2DropWatch;
                    SDL2.SDL.SDL_AddEventWatch(s_sdl2DropWatch, IntPtr.Zero);
                }
                else
                {
                    SDL3.SDL.SDL_SetEventEnabled((uint)SDL3.SDL.SDL_EventType.SDL_EVENT_DROP_FILE, true);
                    s_sdl3DropWatch = Sdl3DropWatch;
                    SDL3.SDL.SDL_AddEventWatch(s_sdl3DropWatch, IntPtr.Zero);
                }
            }
            catch (Exception e)
            {
                DevConsole.Log(e);
            }
        }

        // SDL calls these as events come in, possibly not on the game thread, so they only queue the path
        private static int Sdl2DropWatch(IntPtr userdata, IntPtr sdlEvent)
        {
            try
            {
                if (sdlEvent != IntPtr.Zero && Marshal.ReadInt32(sdlEvent) == (int)SDL2.SDL.SDL_EventType.SDL_DROPFILE)
                {
                    SDL2.SDL.SDL_DropEvent drop = Marshal.PtrToStructure<SDL2.SDL.SDL_DropEvent>(sdlEvent);
                    string path = ReadUtf8(drop.file);
                    if (!string.IsNullOrEmpty(path))
                        s_droppedFiles.Enqueue(path);
                }
            }
            catch (Exception)
            {
            }
            return 1;
        }

        private static unsafe bool Sdl3DropWatch(IntPtr userdata, SDL3.SDL.SDL_Event* sdlEvent)
        {
            try
            {
                if (sdlEvent != null && sdlEvent->type == (uint)SDL3.SDL.SDL_EventType.SDL_EVENT_DROP_FILE)
                {
                    SDL3.SDL.SDL_DropEvent drop = Marshal.PtrToStructure<SDL3.SDL.SDL_DropEvent>((IntPtr)sdlEvent);
                    string path = ReadUtf8((IntPtr)drop.data);
                    if (!string.IsNullOrEmpty(path))
                        s_droppedFiles.Enqueue(path);
                }
            }
            catch (Exception)
            {
            }
            return true;
        }

        private static string ReadUtf8(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                return null;
            int length = 0;
            while (Marshal.ReadByte(pointer, length) != 0 && length < 32768)
                length++;
            byte[] bytes = new byte[length];
            Marshal.Copy(pointer, bytes, 0, length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>Attaches a dropped image, called every frame from <see cref="Update"/>. Drops while the chat is closed are ignored.</summary>
        private static void UpdateFileDrops(bool open)
        {
            HookFileDrops();
            if (s_droppedFiles.IsEmpty)
                return;
            List<string> files = new List<string>();
            while (s_droppedFiles.TryDequeue(out string path))
                files.Add(path);
            if (!open || DevConsole.open)
                return;

            if (!ChatImages.sendingEnabled)
            {
                ChatImages.ShowAttachmentError("Turn on Chat Images (Options > Rebuilt > Chat) to send images");
                return;
            }
            if (ChatImages.preparingAttachment)
                return;
            // Several files at once: the first image
            string file = files.FirstOrDefault(x => ChatImages.FileExtensions.Contains(Path.GetExtension(x).TrimStart('.').ToLowerInvariant())) ?? files[0];
            if (Directory.Exists(file))
            {
                ChatImages.ShowAttachmentError("That's a folder, drop an image file");
                return;
            }
            ChatImages.AttachFile(file);
            ResetBlink();
        }
    }
}
