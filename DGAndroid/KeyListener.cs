using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame
{
    internal class KeyListener : Java.Lang.Object, View.IOnKeyListener
    {
        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            MainActivity.instance.KeyPress(v, new View.KeyEventArgs(false, keyCode, e));
            return true;
        }
    }
}