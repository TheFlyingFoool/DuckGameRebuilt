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
        public static void Test()
        {
            // OnKeybindAttribute attr = typeof(Commands).GetMethod(nameof(ExampleMethod))!.GetCustomAttribute<OnKeybindAttribute>();
            //
            // return attr.KeybindString;
        }
    }
}