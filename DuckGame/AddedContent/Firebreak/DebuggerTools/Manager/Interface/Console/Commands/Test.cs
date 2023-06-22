using DuckGame.MMConfig;
using Humanizer;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using SDL2;
using System;
using System.Linq;
using System.Reflection;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Hidden = true)]
        public static string Test()
        {
            OnKeybindAttribute attr = typeof(Commands).GetMethod(nameof(ExampleMethod))!.GetCustomAttribute<OnKeybindAttribute>();

            return attr.KeybindString;
        }

        [OnKeybind("UP+RIGHT,UP,DOWN,DOWN,UP,UP")]
        public static void ExampleMethod()
        {
            Graphics.FlashScreen();
        }
    }
}