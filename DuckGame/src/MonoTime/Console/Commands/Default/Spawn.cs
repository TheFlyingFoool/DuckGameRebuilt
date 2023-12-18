using AddedContent.Firebreak;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Spawns a thing at the specified location", IsCheat = true)]
        public static void Spawn(Thing thing, Vec2 position)
        {
            thing.position = position;
            Level.Add(thing);
            SFX.Play("hitBox");
        }
    }
}