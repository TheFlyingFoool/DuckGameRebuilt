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
            var me = Extensions.GetMe().duck;
            
            me._ragdollInstance.tongueStuckThing = new WhiteRectangle(0,0,0,0);
            typeof(Thing).GetField("_removeFromLevel", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(me._ragdollInstance.tongueStuckThing, true);
            
            return;
            SDL.SDL_SetClipboardText(string.Join("\n", typeof(Thing).GetFields()
                .Select(x => $"{x.Name}: {x.GetValue(Extensions.GetMe().duck._ragdollInstance.tongueStuckThing)}")
                .Concat(typeof(Thing).GetProperties()
                    .Where(x => x.CanRead)
                    .Select(x => $"{x.Name}: {x.GetValue(Extensions.GetMe().duck._ragdollInstance.tongueStuckThing)}")) 
            ));
        }
        
        [MMCommand(Hidden = true)]
        public static void Test2()
        {
            Level.current = new GameLevel(DuckFile.LoadLevel($"{Content.path}/levels/deathmatch/snow07.lev").metaData.guid);
        }
    }
}