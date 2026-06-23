using System;
using System.Collections.Generic;
using System.Linq;
using static SDL3.SDL;

namespace DuckGame
{
    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_ShowOpenFileDialog
    /// https://wiki.libsdl.org/SDL3/SDL_ShowSaveFileDialog
    /// </summary>
    public class FileDialogFna
    {
        // Use static properties to prevent GC errors by keeping a reference to the callback.
        // Only supports one type of dialog at a time.
        public static OpenDialogFileCallback OnOpenFile;
        public static SaveDialogFileCallback OnSaveFile;
        public static SDL_DialogFileCallback OnOpenFileCallback;
        public static SDL_DialogFileCallback OnSaveFileCallback;
        public delegate void OpenDialogFileCallback(List<string> filepathsSelected);
        public delegate void SaveDialogFileCallback(string filepathSelected);

        public void ShowOpenFileDialog(
             OpenDialogFileCallback onOpenFile
            ,IntPtr? userdata = null
            ,IntPtr? window = null
            ,SDL_DialogFileFilter[] filters = null
            ,string default_location = null
            ,bool allowMany = false
        )
        {
            OnOpenFile = onOpenFile;
            OnOpenFileCallback = OpenFileCallback;
            SDL_ShowOpenFileDialog(OnOpenFileCallback
                ,userdata ?? IntPtr.Zero
                ,window ?? IntPtr.Zero
                ,filters
                ,filters?.Length ?? 0
                ,default_location
                ,allowMany
            );
        }

        public void ShowSaveFileDialog(
             SaveDialogFileCallback onSaveFile
            ,IntPtr? userdata = null
            ,IntPtr? window = null
            ,SDL_DialogFileFilter[] filters = null
            ,string default_location = null
        )
        {
            OnSaveFile = onSaveFile;
            OnSaveFileCallback = SaveFileCallback;
            SDL_ShowSaveFileDialog(OnSaveFileCallback
               ,userdata ?? IntPtr.Zero
               ,window ?? IntPtr.Zero
               ,filters
               ,filters?.Length ?? 0
               ,default_location
           );
        }

        public static SDL_DialogFileFilter[] GetDialogFileFilters(string name, string pattern)
        {
            var filters = new SDL_DialogFileFilter[] {
                GetDialogFileFilter(name, pattern)
            };
            return filters;
        }

        public unsafe static SDL_DialogFileFilter GetDialogFileFilter(string name, string pattern)
        {
            var filter = new SDL_DialogFileFilter {
                name = MarshalHelper.ToUTF8(name),
                pattern = MarshalHelper.ToUTF8(pattern)
            };
            return filter;
        }

        private void OpenFileCallback(IntPtr userdata, IntPtr filelistPtr, int filter)
        {
            try
            {
                var fileNameList = MarshalHelper.StringUTF8PointersToStringList(filelistPtr);
                if (!fileNameList.Any()) {
                    return; // Canceled
                }
                OnOpenFile?.Invoke(fileNameList);
            }
            finally
            {
                OnOpenFile = null;
            }
        }

        private void SaveFileCallback(IntPtr userdata, IntPtr filelistPtr, int filter)
        {
            try
            {
                var fileNameList = MarshalHelper.StringUTF8PointersToStringList(filelistPtr);
                if (!fileNameList.Any()) {
                    return; // Canceled
                }
                OnSaveFile?.Invoke(fileNameList?.FirstOrDefault());
            }
            finally {
                OnSaveFile = null;
            }
        }

        public static string EnsureFileExtension(string fileName, string extension)
        {
            if (fileName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)) {
                return fileName;
            }
            else {
                return $"{fileName}{extension}";
            }
        }
    }
}
