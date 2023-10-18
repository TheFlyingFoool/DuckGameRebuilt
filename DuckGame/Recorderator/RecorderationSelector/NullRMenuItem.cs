using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using static DuckGame.RecorderationSelector;

namespace DuckGame
{
    public class NullRMenuItem : IRMenuItem
    {
        public Rectangle Draw(Vec2 position, bool selected)
        {
            Rectangle bounds = new Rectangle(position.x, position.y, Graphics._biosFont.height * FONT_SIZE, Graphics._biosFont.height * FONT_SIZE);
            Graphics.DrawRect(bounds, selected ? Color.Magenta : Color.Purple, 1f);
            return bounds;
        }

        public void UpdateHovered() { }

        public void OnSelect() { }

        public void OnHover() { }

        public void OnUnhover() { }

        public int FolderSub { get; set; }
        public IRMenuItem Parent { get; set; }
    }
}