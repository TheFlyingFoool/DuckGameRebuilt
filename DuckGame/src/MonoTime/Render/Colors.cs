// Decompiled with JetBrains decompiler
// Type: DuckGame.Colors
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Colors
    {
        public static Color MenuOption = new Color(235, 137, 51);
        public static Color Bronze = new Color(216, 105, 65);
        public static Color Silver = new Color(191, 191, 191);
        public static Color Gold = new Color(247, 224, 90);
        public static Color Platinum = new Color(178, 220, 239);
        public static Color Developer = new Color(222, 32, 45);
        public static Color DGYellow = new Color(247, 224, 90);
        public static Color DGOrange = new Color(235, 136, 49);
        public static Color DGPink = new Color(235, 190, 242);
        public static Color DGPink2 = new Color(246, 88, 191);
        public static Color DGEgg = new Color(245, 235, 211);
        public static Color DGBlue = new Color(49, 162, 242);
        public static Color DGGreen = new Color(163, 206, 39);
        public static Color DGRed = new Color(192, 32, 45);
        public static Color BlueGray = new Color(47, 73, 79);
        public static Color DGPurple = new Color(179, 54, 242);
        public static Color Transparent = new Color(0, 0, 0, 0);
        public static Color TransparentPink = new Color((int)byte.MaxValue, 0, (int)byte.MaxValue, 0);
        public static Color DGVanilla = new Color((int)byte.MaxValue, 246, 214);
        public static Color Duck1 = new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
        public static Color Duck2 = new Color(125, 125, 125);
        public static Color Duck3 = new Color(247, 224, 90);
        public static Color Duck4 = new Color(205, 107, 29);
        public static Color SuperDarkBlueGray = new Color(8, 12, 16);
        public static Color SystemGray = new Color(173, 173, 173);
        public static Color[] Rainbow = new Color[] { 
            new Color(255, 0, 0),
            new Color(255, 127, 0),
            new Color(255, 255, 0),
            new Color(127, 255, 0),
            new Color(0, 255, 0),
            new Color(0, 255, 127),
            new Color(0, 255, 255),
            new Color(0, 127, 255),
            new Color(0, 0, 255),
            new Color(127, 0, 255),
            new Color(255, 0, 255),
            new Color(255, 0, 127) };
        private static Dictionary<string, Color> _colorMap = new Dictionary<string, Color>()
        {
            {
                "AQUA",
                new Color(0, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue)
            },
            {
                "RED",
                Color.Red
            },
            {
                "WHITE",
                Color.White
            },
            {
                "BLACK",
                Color.Black
            },
            {
                "DARKNESS",
                new Color(10, 10, 10)
            },
            {
                "BLUE",
                Color.Blue
            },
            {
                "DGBLUE",
                DGBlue
            },
            {
                "DGRED",
                DGRed
            },
            {
                "DGREDDD",
                DGRed
            },
            {
                "DGGREEN",
                DGGreen
            },
            {
                "DGGREENN",
                DGGreen
            },
            {
                "DGYELLOW",
                DGYellow
            },
            {
                "DGYELLO",
                DGYellow
            },
            {
                "DGORANGE",
                new Color(235, 136, 49)
            },
            {
                "ORANGE",
                new Color(235, 137, 51)
            },
            {
                "MENUORANGE",
                MenuOption
            },
            {
                "YELLOW",
                new Color(247, 224, 90)
            },
            {
                "GREEN",
                Color.LimeGreen
            },
            {
                "LIME",
                Color.LimeGreen
            },
            {
                "TIMELIME",
                Color.Lime
            },
            {
                "GRAY",
                new Color(70, 70, 70)
            },
            {
                "LIGHTGRAY",
                new Color(96, 119, 124)
            },
            {
                "CREDITSGRAY",
                new Color(137, 159, 164)
            },
            {
                "BLUEGRAY",
                BlueGray
            },
            {
                "PINK",
                DGPink
            },
            {
                "PURPLE",
                new Color(115, 48, 242)
            },
            {
                "DGPURPLE",
                new Color(115, 48, 242)
            },
            {
                "CBRONZE",
                Bronze
            },
            {
                "CSILVER",
                Silver
            },
            {
                "CGOLD",
                Gold
            },
            {
                "CPLATINUM",
                Platinum
            },
            {
                "CDEV",
                Developer
            },
            {
                "DUCKCOLOR1",
                Duck1
            },
            {
                "DUCKCOLOR2",
                Duck2
            },
            {
                "DUCKCOLOR3",
                Duck3
            },
            {
                "DUCKCOLOR4",
                Duck4
            },
            {
                "RBOW_1",
                new Color(192, 35, 45)
            },
            {
                "RBOW_2",
                new Color(237, 94, 238)
            },
            {
                "RBOW_3",
                new Color(138, 38, 190)
            },
            {
                "RBOW_4",
                new Color(49, 162, 242)
            },
            {
                "RBOW_5",
                new Color(149, 188, 37)
            },
            {
                "RBOW_6",
                new Color(247, 224, 90)
            },
            {
                "RBOW_7",
                new Color(235, 137, 49)
            }
        };
        private static float HueToRGB(float v1, float v2, float vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if ((6 * vH) < 1)
                return (v1 + (v2 - v1) * 6 * vH);

            if ((2 * vH) < 1)
                return v2;

            if ((3 * vH) < 2)
                return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);

            return v1;
        }
        public static long GetHash(byte alpha, byte red, byte green, byte blue)
        {
            return (red << 16 | green << 8 | blue | alpha << 24) & -1; //& (ulong)-1);
        }

        public static long GetHash(Color color)
        {
            return (color.r << 16 | color.g << 8 | color.b | color.a << 24) &
                           -1; //& (ulong)-1); 
        }

        public static Color ParseColor(string color)
        {
            if (!_colorMap.TryGetValue(color, out Color parseColor))
            {
                string[] strArray = color.TrimSplit(',');

                if (strArray.Length != 3
                    || !byte.TryParse(strArray[0], out byte r)
                    || !byte.TryParse(strArray[1], out byte g)
                    || !byte.TryParse(strArray[2], out byte b))
                    return parseColor;

                return new Color(r, g, b);
            }

            return parseColor;
        }

        public static Color ColorFromHSL(float H, float S, float L)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (S == 0)
            {
                r = g = b = (byte)(L * 255);
            }
            else
            {
                float v1, v2;
                float hue = (float)H / 360;

                v2 = (L < 0.5) ? (L * (1 + S)) : ((L + S) - (L * S));
                v1 = 2 * L - v2;

                r = (byte)(255 * HueToRGB(v1, v2, hue + (1.0f / 3)));
                g = (byte)(255 * HueToRGB(v1, v2, hue));
                b = (byte)(255 * HueToRGB(v1, v2, hue - (1.0f / 3)));
            }
            return new Color(r, g, b);
        }
        public static Vec3 HSLFromColor(Color color)
        {
            float R = (color.r / 255f);
            float G = (color.g / 255f);
            float B = (color.b / 255f);

            float Min = Math.Min(Math.Min(R, G), B);
            float Max = Math.Max(Math.Max(R, G), B);
            float Delta = Max - Min;

            float H = 0;
            float S = 0;
            float L = (float)((Max + Min) / 2.0f);

            if (Delta != 0)
            {
                if (L < 0.5f)
                {
                    S = (float)(Delta / (Max + Min));
                }
                else
                {
                    S = (float)(Delta / (2.0f - Max - Min));
                }

                float Delta_R = (float)(((Max - R) / 6.0f + (Delta / 2.0f)) / Delta);
                float Delta_G = (float)(((Max - G) / 6.0f + (Delta / 2.0f)) / Delta);
                float Delta_B = (float)(((Max - B) / 6.0f + (Delta / 2.0f)) / Delta);

                if (R == Max)
                {
                    H = Delta_B - Delta_G;
                }
                else if (G == Max)
                {
                    H = (1.0f / 3.0f) + Delta_R - Delta_B;
                }
                else if (B == Max)
                {
                    H = (2.0f / 3.0f) + Delta_G - Delta_R;
                }

                if (H < 0) H += 1.0f;
                if (H > 1) H -= 1.0f;
            }

            return new Vec3(H * 360f, S, L);
        }
    }
    public static class ColorExtensions
    {
        public static Vec3 GetHSL(this Color color)
        {
            float R = (color.r / 255f);
            float G = (color.g / 255f);
            float B = (color.b / 255f);

            float Min = Math.Min(Math.Min(R, G), B);
            float Max = Math.Max(Math.Max(R, G), B);
            float Delta = Max - Min;

            float H = 0;
            float S = 0;
            float L = (float)((Max + Min) / 2.0f);

            if (Delta != 0)
            {
                if (L < 0.5f)
                {
                    S = (float)(Delta / (Max + Min));
                }
                else
                {
                    S = (float)(Delta / (2.0f - Max - Min));
                }

                float Delta_R = (float)(((Max - R) / 6.0f + (Delta / 2.0f)) / Delta);
                float Delta_G = (float)(((Max - G) / 6.0f + (Delta / 2.0f)) / Delta);
                float Delta_B = (float)(((Max - B) / 6.0f + (Delta / 2.0f)) / Delta);

                if (R == Max)
                {
                    H = Delta_B - Delta_G;
                }
                else if (G == Max)
                {
                    H = (1.0f / 3.0f) + Delta_R - Delta_B;
                }
                else if (B == Max)
                {
                    H = (2.0f / 3.0f) + Delta_G - Delta_R;
                }

                if (H < 0) H += 1.0f;
                if (H > 1) H -= 1.0f;
            }

            return new Vec3(H * 360f, S, L);
        }
    }
}