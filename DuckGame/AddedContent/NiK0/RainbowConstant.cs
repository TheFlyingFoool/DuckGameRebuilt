using DuckGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class RainbowConstant
    {
        public int stage = 1;
        public Color value;
        public RainbowConstant(int startstage = 1)
        {
            value = Color.Red;
            stage = startstage;
        }
        public static implicit operator Color(RainbowConstant val)
        {
            return val.value;
        }
        public static Color operator *(RainbowConstant value1, float value2)
        {
            return value1.value * value2;
        }
        public void Update(float speed = 0.02f)
        {
            switch (stage)
            {
                case 1:
                    value = Lerp.Color(value, new Color(255, 255, 0), speed);
                    if (value == new Color(255, 255, 0))
                    {
                        stage++;
                    }
                    break;
                case 2:
                    value = Lerp.Color(value, new Color(0, 255, 0), speed);
                    if (value == new Color(0, 255, 0))
                    {
                        stage++;
                    }
                    break;
                case 3:
                    value = Lerp.Color(value, new Color(0, 255, 255), speed);
                    if (value == new Color(0, 255, 255))
                    {
                        stage++;
                    }
                    break;
                case 4:
                    value = Lerp.Color(value, new Color(0, 0, 255), speed);
                    if (value == new Color(0, 0, 255))
                    {
                        stage++;
                    }
                    break;
                case 5:
                    value = Lerp.Color(value, new Color(255, 0, 255), speed);
                    if (value == new Color(255, 0, 255))
                    {
                        stage++;
                    }
                    break;
                case 6:
                    value = Lerp.Color(value, new Color(255, 0, 0), speed);
                    if (value == new Color(255, 0, 0))
                    {
                        stage = 1;
                    }
                    break;
                default:
                    value = Lerp.Color(value, new Color(255, 255, 0), speed);
                    if (value == new Color(255, 255, 0))
                    {
                        stage++;
                    }
                    break;
            }
        }
    }
}
