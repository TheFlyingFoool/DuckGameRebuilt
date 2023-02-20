using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.InputMethodServices;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Org.Libsdl.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Environment = Android.OS.Environment;
using File = System.IO.File;
using JFile = Java.IO.File;
using Keycode = Android.Views.Keycode;
using Stream = System.IO.Stream;

namespace DuckGame
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        HardwareAccelerated = true,
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class MainActivity : SDLActivity
    {
        private Game _game;
        private static View _view;
        private static InputMethodManager inputmanager;
        public static MainActivity instance;
        AudioManager audioManager;

        public static (int, int) GetDimensions()
        {
            //int width = (int)(instance.Resources.DisplayMetrics.WidthPixels / instance.Resources.DisplayMetrics.Density);
            //int height = (int)(instance.Resources.DisplayMetrics.HeightPixels / instance.Resources.DisplayMetrics.Density);
            int width = (int)(instance.Resources.DisplayMetrics.WidthPixels) - 100;
            int height = (int)(instance.Resources.DisplayMetrics.HeightPixels);
            return (width, height);
        }

        public override void LoadLibraries()
        {
            base.LoadLibraries();
            Java.Lang.JavaSystem.LoadLibrary("fnadroid-ext");
            SetMain(SDL_Main);
        }

        public static void SDL_Main()
        {
            Program.Main(new string[] { "-nosteam", "-linux", "-nosa" });
            instance._game = Program.main;
            instance._game.Run();
        }

        [DllImport("main")]
        public static extern void SetMain(System.Action main);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.Hide();
            //this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            View decorView = Window.DecorView;
            var uiOptions = (int)decorView.SystemUiVisibility;
            var newUiOptions = (int)uiOptions;

            newUiOptions |= (int)SystemUiFlags.LowProfile;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

            this.Immersive = true;
            instance = this;

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 0);
            }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 0);
            }
            FetchStorage();
            //Program.Main(new string[]{"-nosteam", "-linux", "-nosa"});
            //_game = Program.main;
            //_view = _game.Services.GetService(typeof(View)) as View;
            audioManager = (AudioManager)GetSystemService(Context.AudioService);
            //_view.SetOnKeyListener(new KeyListener());
            //_view.KeyPress += KeyPress;

            inputmanager = Application.GetSystemService(InputMethodService) as InputMethodManager;

            System.Environment.SetEnvironmentVariable("FNADROID", "1");
            RuntimeHelpers.RunClassConstructor(typeof(Game).TypeHandle);
            //SetContentView(_view);
            //_game.Run();
        }

        public static Stream OpenStream(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        public static string DocPath = null;
        public static string SavePath = null;

        public void FetchStorage()
        {
            List<string> docPaths = new List<string>();
            var directories = ContextCompat.GetExternalFilesDirs(this.ApplicationContext, null);
            bool found = false;
            string deniedPath = "";
            foreach (JFile dir in directories)
            {
                try
                {
                    string path = dir.AbsolutePath;
                    int i = path.IndexOf("/Android/data/");
                    if (i == -1) continue;
                    path = path.Substring(0, i);
                    var dgpath = new JFile(path + "/Documents/DuckGame/");
                    docPaths.Add(dgpath.AbsolutePath);
                    if (dgpath.Exists())
                    {
                        found = true;
                        if (!dgpath.CanWrite() || !dgpath.CanRead())
                        {
                            deniedPath = dgpath.AbsolutePath;
                            break;
                        }
                        DocPath = dgpath.AbsolutePath + "/";
                        SavePath = path + "/Documents/";
                        return;
                    }
                }
                catch
                {
                }
            }
            AlertDialog dialog = null;
            
            instance.RunOnUiThread(() =>
            {
                using (AlertDialog.Builder build = new AlertDialog.Builder(this))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(found ? "Access denied to DuckGame folder: " : "DuckGame folder not found at: ");
                    if (found)
                    {
                        stringBuilder.AppendLine(deniedPath);
                    }
                    else
                    {
                        foreach (var p in docPaths)
                        {
                            stringBuilder.AppendLine(p);
                        }
                    }
                    build.SetMessage(stringBuilder.ToString());
                    build.SetCancelable(false);
                    dialog = build.Create();
                    dialog.Show();
                }
            });

            while (dialog == null || dialog.IsShowing)
            {
                System.Threading.Thread.Sleep(0);
            }
            _game.Exit();
            dialog.Dispose();
        }

        //public List<string> FetchStorage()
        //{
        //    List<string> extStorage = new List<string>();
        //    var directories = ContextCompat.GetExternalFilesDirs(this.ApplicationContext, null);

        //    foreach (JFile dir in directories)
        //    {
        //        try
        //        {
        //            new JFile()
        //            bool isRemovable = Environment.InvokeIsExternalStorageRemovable(dir);
        //            string path = dir.AbsolutePath;
        //            int i = path.IndexOf("/Android/data/");
        //            if(i == -1)
        //            {

        //            }
        //            path = path.Substring(0, i);
        //            if (isRemovable)
        //                extStorage.Add(path);
        //        }
        //        catch
        //        {
        //            AlertDialog.Builder
        //        }
        //    }
        //    return extStorage;
        //}

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutStable |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.ImmersiveSticky
                );
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            if (newConfig.HardKeyboardHidden == HardKeyboardHidden.No)
            {

            }
            else
            {

            }
            base.OnConfigurationChanged(newConfig);
        }

        public static List<Keycode> Pressed = new List<Keycode>();

        //public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        //{
        //    //Pressed.Remove(keyCode);
        //    return base.OnKeyUp(keyCode, e);
        //}

        public void KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Up)
            {
                Pressed.Remove(e.KeyCode);
            }
            if (e.Event.Action == KeyEventActions.Down)
            {
                int min = audioManager.GetStreamMinVolume(Android.Media.Stream.Music);
                int max = audioManager.GetStreamMaxVolume(Android.Media.Stream.Music);
                int current = audioManager.GetStreamVolume(Android.Media.Stream.Music);
                if (e.KeyCode == Keycode.VolumeUp && current + 1 <= max)
                {
                    audioManager.SetStreamVolume(Android.Media.Stream.Music, current + 1, VolumeNotificationFlags.ShowUi);
                    return;
                }
                else if (e.KeyCode == Keycode.VolumeDown && current - 1 >= min)
                {
                    audioManager.SetStreamVolume(Android.Media.Stream.Music, current - 1, VolumeNotificationFlags.ShowUi);
                    return;
                }
                if (!Pressed.Contains(e.KeyCode))
                    Pressed.Add(e.KeyCode);
                //if (e.KeyCode == Keycode.Enter || !inputmanager.IsActive)
                //{
                //    finishedinput = true;
                //    inputmanager.HideSoftInputFromWindow(_view.WindowToken, HideSoftInputFlags.None);
                //    return;
                //}
                //if (e.KeyCode == Keycode.Del && TextInput.Length > 0)
                //{
                //    TextInput = TextInput.Remove(TextInput.Length - 1);
                //    return;
                //}
                //if (e.KeyCode == Keycode.ShiftLeft) return;
                //var s = e.KeyCode.ToString();
                //if (s.Length == 4 && s.StartsWith("Num"))
                //{
                //    TextInput += int.Parse(s[3].ToString());
                //    return;
                //}
                //if (e.Event.IsShiftPressed)
                //{
                //    TextInput += s;
                //    return;
                //}
                //TextInput += e.KeyCode.ToString().ToLower();
            }
        }

        internal static void ShowKeyboard()
        {
            inputmanager.ShowSoftInput(_view, ShowFlags.Forced);
            inputmanager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        internal static string TextInput = "";
        static bool finishedinput;

        internal static bool InputIsReady()
        {
            if (finishedinput)
            {
                return true;
            }
            return false;
        }

        internal static void Clear()
        {
            finishedinput = false;
            TextInput = "";
        }
    }
}
