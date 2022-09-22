using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public static partial class CursorHandler
    {
        public static GameCursor? CurrentCursor = null;
        public static Dictionary<string, GameCursor> CursorMappings = new();

        [PostInitialize]
        public static void Initialize()
        {

        }

        [DrawingContext(CustomID = "CursorDraw")]
        public static void Draw()
        {
            CurrentCursor?.DrawCursor();
        }

        // im too lazy to hook this to an update method, so change this later
        [DrawingContext(CustomID = "CursorUpdate")]
        public static void Update()
        {
            if (Mouse.left == InputState.Pressed)
                CurrentCursor?.LeftPressed(Mouse.position);
            if (Mouse.left == InputState.Released)
                CurrentCursor?.LeftReleased(Mouse.position);

            if (Mouse.right == InputState.Pressed)
                CurrentCursor?.RightPressed(Mouse.position);
            if (Mouse.right == InputState.Released)
                CurrentCursor?.RightReleased(Mouse.position);

            if (Mouse.middle == InputState.Pressed)
                CurrentCursor?.MiddlePressed(Mouse.position);
            if (Mouse.middle == InputState.Released)
                CurrentCursor?.MiddleReleased(Mouse.position);
        }

        public static bool TryChangeCursor(string newCursorId)
        {
            throw new NotImplementedException();
        }
    }
}