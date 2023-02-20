using Microsoft.Xna.Framework;

namespace AddedContent.Hyeve.DebugUI.Basic
{
    public class UiSeamless : UiBasic
    {
        public UiSeamless(Vector2 position, Vector2 size, string name = "UiSeamless", float scale = 1) : base(position, size, name, scale)
        {
            Draggable = false;
            Resizeable = false;
            Closeable = false;
            MinSize = Vector2.One;
            SizeInternal = size;
        }
    }
}