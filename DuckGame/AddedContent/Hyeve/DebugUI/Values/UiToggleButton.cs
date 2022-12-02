using System.Drawing;
using AddedContent.Hyeve.DebugUI.Basic;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using Microsoft.Xna.Framework;
using Color = DuckGame.Color;

namespace AddedContent.Hyeve.DebugUI.Values
{
    public class UiToggleButton : UiSeamless
    {
        protected int CharSize;

        public virtual bool Toggled { get; set; }

        public UiToggleButton(Vector2 position, Vector2 size, string name = "UiToggle", float scale = 1) : base(position, size, name, scale)
        {
            CharSize = (int)(20 * scale);
        }

        public override void DrawContent()
        {
            if (!Visible()) return;
            base.DrawContent();
            Color main = Colors.Main;
            Color enabled = Colors.Enabled;
            Color disabled = Colors.Disabled;

            if (Hovered())
            {
                main = main.Brighter();
            }
            else
            {
                enabled = enabled.Darker();
                disabled = disabled.Darker();
            }
            
            Vector2 offset = new(_accentLineWidth);
            PolyRenderer.Rect(Position - offset, Position + Size + offset, Colors.Accent);
            PolyRenderer.Rect(Position, Position + Size, Toggled ? enabled : disabled);
            FontDatabase.DrawString(Name,Position + Size.ZeroX() / 2f + Vector2.UnitY * CharSize / 2f, Colors.Text, CharSize);
        }

        protected override void HandleClicked(MouseAction action)
        {
            if (!Hovered()) return;
            Toggled = !Toggled;
        }
    }
}