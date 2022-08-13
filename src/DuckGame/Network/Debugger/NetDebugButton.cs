// Decompiled with JetBrains decompiler
// Type: DuckGame.NetDebugButton
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class NetDebugButton : NetDebugElement
    {
        private Action _pressAction;
        private Action _holdAction;
        private bool pressing;

        public NetDebugButton(NetDebugInterface pInterface, string pName, Action pPress, Action pHold)
          : base(pInterface)
        {
            _name = pName;
            _pressAction = pPress;
            _holdAction = pHold;
        }

        protected override bool Draw(Vec2 position, bool allowInput)
        {
            bool flag = !allowInput;
            position.y -= 2f;
            Vec2 vec2 = new Vec2(100f, 12f);
            width = 100f;
            Rectangle rectangle = new Rectangle(position.x, position.y, vec2.x, vec2.y);
            if (!flag && rectangle.Contains(Mouse.positionConsole) || pressing)
            {
                Graphics.DrawRect(position, position + vec2, Color.White, depth + 2, false);
                Graphics.DrawRect(position, position + vec2, Color.White * 0.3f, depth + 1);
                Graphics.DrawString(_name, position + new Vec2((float)(vec2.x / 2.0 - Graphics.GetStringWidth(_name) / 2.0), 2f), Color.White * 1f, depth + 10);
                if (Mouse.left == InputState.Pressed)
                {
                    if (_pressAction != null)
                        _pressAction();
                    pressing = true;
                    flag = true;
                }
                else if (pressing)
                {
                    if (_holdAction != null)
                        _holdAction();
                    flag = true;
                }
            }
            else
            {
                Graphics.DrawRect(position, position + vec2, Color.White, depth + 2, false);
                Graphics.DrawRect(position, position + vec2, Color.Black * 0.8f, depth + 1);
                Graphics.DrawString(_name, position + new Vec2((float)(vec2.x / 2.0 - Graphics.GetStringWidth(_name) / 2.0), 2f), Color.White * 0.8f, depth + 10);
            }
            if (Mouse.left == InputState.Released)
                pressing = false;
            return flag;
        }
    }
}
