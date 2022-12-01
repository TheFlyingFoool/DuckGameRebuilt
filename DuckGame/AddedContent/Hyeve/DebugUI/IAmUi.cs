using System;
using DuckGame;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;

namespace AddedContent.Hyeve.DebugUI
{
    public interface IAmUi
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector4 Expansion { get; }
        public string Name { get; set; }
        public event Action<IAmUi, Vector2> OnPositioned;

        public event Action<IAmUi, Vector2> OnResized;

        public event Action<IAmUi, UiCols, Color> OnColoured;

        public event Action<IAmUi> OnKilled;

        abstract void DrawContent();
        abstract void UpdateContent();
        abstract void OnMouseAction(MouseAction action, float scroll = 0f);
        abstract void OnKeyPressed(Keys keycode, char value);
        abstract bool IsOverlapping(Vector2 pos);

        abstract void SetCol(UiCols type, Color col);
        abstract Color GetCol(UiCols type);
        abstract void Kill();
    }

    [Flags]
    public enum MouseAction
    {
        RightClick = 0, LeftClick = 2, MiddleClick = 4,
        RightRelease = 8, LeftRelease = 16, MiddleRelease = 32,
        Scrolled = 64,

        AnyClick = 0 | 2 | 4, AnyRelease = 8 | 16 | 32,
    }

    public enum UiCols
    {
        Main, Text, Accent, Alternate, Data
    }
}
