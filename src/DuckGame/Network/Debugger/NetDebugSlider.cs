// Decompiled with JetBrains decompiler
// Type: DuckGame.NetDebugSlider
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class NetDebugSlider : NetDebugElement
    {
        private Func<float> _getValue;
        private Action<float> _setValue;
        private Func<float, string> _formatter;

        public NetDebugSlider(
          NetDebugInterface pInterface,
          string pName,
          Func<float> pGet,
          Action<float> pSet,
          Func<float, string> pDisplayFormatter)
          : base(pInterface)
        {
            _name = pName;
            _getValue = pGet;
            _setValue = pSet;
            _formatter = pDisplayFormatter;
        }

        public void Update()
        {
        }

        protected override bool Draw(Vec2 position, bool allowInput)
        {
            bool flag = !allowInput;
            position.x += indent;
            Graphics.DrawString(_name, position, Color.White, depth + 10);
            float num1 = _getValue();
            int num2 = 20;
            int num3 = (int)Math.Round(num1 * num2);
            Rectangle rectangle = new Rectangle(position.x + 100f, position.y, num2 * 5, 8f);
            float num4 = -1f;
            if (rectangle.Contains(Mouse.positionConsole) & allowInput)
            {
                num4 = (int)((Mouse.positionConsole.x - rectangle.Left) / rectangle.width * num2);
                if (Mouse.left == InputState.Down)
                {
                    _setValue(num4 / num2);
                    flag = true;
                }
            }
            Vec2 tl = rectangle.tl;
            for (int index = 0; index < num2; ++index)
            {
                Color col = Color.Gray;
                if (num4 >= index && num4 != -1.0)
                    col = Color.White;
                else if (num3 >= index)
                    col = new Color(200, 200, 200);
                Graphics.DrawRect(tl, tl + new Vec2(4f, 8f), col, depth + 5);
                tl.x += 5f;
            }
            tl.x += 2f;
            Graphics.DrawString("(" + _formatter(num1) + ")", tl, Color.White, depth + 5);
            return flag;
        }
    }
}
