using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using SDL2;
using System.Linq;
using System.Reflection;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Hidden = true)]
        public static void Test()
        {
            float width = Graphics.width;
            float height = Graphics.height;
            float scale = height / 540;
            
            Zoom(scale);
            console.Print($"dimensions: ({width}, {height})");
            console.Print($"scaling factor: {scale}");
        }
        
        [MMCommand(Hidden = true)]
        public static void Test2()
        {
            Level.current = new GameLevel(DuckFile.LoadLevel($"{Content.path}/levels/deathmatch/snow07.lev").metaData.guid);
        }
    }
}