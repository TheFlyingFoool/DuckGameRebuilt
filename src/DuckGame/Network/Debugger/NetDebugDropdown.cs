// Decompiled with JetBrains decompiler
// Type: DuckGame.NetDebugDropdown
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NetDebugDropdown : NetDebugElement
    {
        private Func<List<NetDebugDropdown.Element>> _elements;
        public NetDebugDropdown.Element selected;
        private Sprite _downArrow;
        private bool _dropped;

        public NetDebugDropdown(
          NetDebugInterface pInterface,
          string pName,
          Func<List<NetDebugDropdown.Element>> pElements)
          : base(pInterface)
        {
            _name = pName;
            _elements = pElements;
            selected = _elements().FirstOrDefault<NetDebugDropdown.Element>();
            _downArrow = new Sprite("cloudDown");
            _downArrow.CenterOrigin();
        }

        protected override bool Draw(Vec2 position, bool allowInput)
        {
            bool flag1 = !allowInput;
            position.x += indent;
            Vec2 vec2 = new Vec2(160f, 12f);
            Graphics.DrawString(_name, position, Color.White, depth + 10);
            position.x += 100f;
            position.y -= 2f;
            width = 280f;
            List<NetDebugDropdown.Element> elementList = _elements();
            Rectangle r1 = new Rectangle(position.x + (vec2.x - vec2.y), position.y, vec2.y, vec2.y);
            Rectangle rectangle = new Rectangle(position.x, position.y, vec2.x, vec2.y);
            if (_dropped)
            {
                flag1 = true;
                Rectangle r2 = new Rectangle(position.x, (float)(position.y + vec2.y + 4.0), vec2.x, vec2.y * elementList.Count);
                Graphics.DrawRect(r2, Color.White, depth + 2, false);
                Graphics.DrawRect(r2, Color.Black * 0.8f, depth + 1);
                foreach (NetDebugDropdown.Element element in elementList)
                {
                    Rectangle r3 = new Rectangle(r2.x, r2.y, vec2.x, vec2.y);
                    if (r3.Contains(Mouse.positionConsole))
                    {
                        Graphics.DrawRect(r3, Color.White * 0.5f, depth + 3);
                        if (Mouse.left == InputState.Pressed)
                        {
                            _dropped = false;
                            selected = element;
                        }
                    }
                    Graphics.DrawString(element.name, r2.tl + new Vec2(2f, 2f), Color.White, depth + 5);
                    r2.y += vec2.y;
                }
                if (Mouse.right == InputState.Pressed || Mouse.left == InputState.Pressed && !r2.Contains(Mouse.positionConsole))
                    _dropped = false;
            }
            bool flag2 = rectangle.Contains(Mouse.positionConsole);
            if (((!flag2 ? 0 : (Mouse.left == InputState.Pressed ? 1 : 0)) & (allowInput ? 1 : 0)) != 0)
            {
                _dropped = true;
                flag1 = true;
            }
            Graphics.DrawRect(position, position + vec2, Color.White, depth + 2, false);
            Graphics.DrawRect(position, position + vec2, Color.Black * 0.8f, depth + 1);
            Graphics.DrawRect(r1, Color.White, depth + 6, false);
            Graphics.DrawRect(r1, flag2 ? Color.White * 0.6f : Color.Gray * 0.5f, depth + 5);
            Graphics.Draw(_downArrow, r1.Center.x, r1.Center.y, depth + 8);
            string text = "-";
            if (selected != null)
                text = selected.name;
            Graphics.DrawString(text, position + new Vec2(4f, 2f), Color.White, depth + 10);
            return flag1;
        }

        public class Element
        {
            public string name;
            public object value;
        }
    }
}
