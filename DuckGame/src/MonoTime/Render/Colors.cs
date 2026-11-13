using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class ColorName
    {
        public const string PREV = "PREV";
        public const string AQUA = "AQUA";
        public const string RED = "RED";
        public const string WHITE = "WHITE";
        public const string BLACK = "BLACK";
        public const string DARKNESS = "DARKNESS";
        public const string BLUE = "BLUE";
        public const string DGBLUE = "DGBLUE";
        public const string DGRED = "DGRED";
        public const string DGREDDD = "DGREDDD";
        public const string DGGREEN = "DGGREEN";
        public const string DGGREENN = "DGGREENN";
        public const string DGYELLOW = "DGYELLOW";
        public const string DGYELLO = "DGYELLO";
        public const string DGORANGE = "DGORANGE";
        public const string ORANGE = "ORANGE";
        public const string MENUORANGE = "MENUORANGE";
        public const string YELLOW = "YELLOW";
        public const string GREEN = "GREEN";
        public const string LIME = "LIME";
        public const string TIMELIME = "TIMELIME";
        public const string GRAY = "GRAY";
        public const string LIGHTGRAY = "LIGHTGRAY";
        public const string CREDITSGRAY = "CREDITSGRAY";
        public const string BLUEGRAY = "BLUEGRAY";
        public const string PINK = "PINK";
        public const string PURPLE = "PURPLE";
        public const string DGPURPLE = "DGPURPLE";
        public const string CBRONZE = "CBRONZE";
        public const string CSILVER = "CSILVER";
        public const string CGOLD = "CGOLD";
        public const string CPLATINUM = "CPLATINUM";
        public const string CDEV = "CDEV";
        public const string DUCKCOLOR1 = "DUCKCOLOR1";
        public const string DUCKCOLOR2 = "DUCKCOLOR2";
        public const string DUCKCOLOR3 = "DUCKCOLOR3";
        public const string DUCKCOLOR4 = "DUCKCOLOR4";
        public const string RBOW_1 = "RBOW_1";
        public const string RBOW_2 = "RBOW_2";
        public const string RBOW_3 = "RBOW_3";
        public const string RBOW_4 = "RBOW_4";
        public const string RBOW_5 = "RBOW_5";
        public const string RBOW_6 = "RBOW_6";
        public const string RBOW_7 = "RBOW_7";
    }

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
        public static Color DGPink = new Color(235, 88, 191);
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
            new Color(255, 0, 127)
        };

        private static Dictionary<string, Color> _colorMap = new Dictionary<string, Color>()
        {
            { ColorName.AQUA, new Color(0, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue) },
            { ColorName.RED, Color.Red },
            { ColorName.WHITE, Color.White },
            { ColorName.BLACK, Color.Black },
            { ColorName.DARKNESS, new Color(10, 10, 10) },
            { ColorName.BLUE, Color.Blue },
            { ColorName.DGBLUE, DGBlue },
            { ColorName.DGRED, DGRed },
            { ColorName.DGREDDD, DGRed },
            { ColorName.DGGREEN, DGGreen },
            { ColorName.DGGREENN, DGGreen },
            { ColorName.DGYELLOW, DGYellow },
            { ColorName.DGYELLO, DGYellow },
            { ColorName.DGORANGE, new Color(235, 136, 49) },
            { ColorName.ORANGE, new Color(235, 137, 51) },
            { ColorName.MENUORANGE, MenuOption },
            { ColorName.YELLOW, new Color(247, 224, 90) },
            { ColorName.GREEN, Color.LimeGreen },
            { ColorName.LIME, Color.LimeGreen },
            { ColorName.TIMELIME, Color.Lime },
            { ColorName.GRAY, new Color(70, 70, 70) },
            { ColorName.LIGHTGRAY, new Color(96, 119, 124) },
            { ColorName.CREDITSGRAY, new Color(137, 159, 164) },
            { ColorName.BLUEGRAY, BlueGray },
            { ColorName.PINK, DGPink },
            { ColorName.PURPLE, new Color(115, 48, 242) },
            { ColorName.DGPURPLE, new Color(115, 48, 242) },
            { ColorName.CBRONZE, Bronze },
            { ColorName.CSILVER, Silver },
            { ColorName.CGOLD, Gold },
            { ColorName.CPLATINUM, Platinum },
            { ColorName.CDEV, Developer },
            { ColorName.DUCKCOLOR1, Duck1 },
            { ColorName.DUCKCOLOR2, Duck2 },
            { ColorName.DUCKCOLOR3, Duck3 },
            { ColorName.DUCKCOLOR4, Duck4 },
            { ColorName.RBOW_1, new Color(192, 35, 45) },
            { ColorName.RBOW_2, new Color(237, 94, 238) },
            { ColorName.RBOW_3, new Color(138, 38, 190) },
            { ColorName.RBOW_4, new Color(49, 162, 242) },
            { ColorName.RBOW_5, new Color(149, 188, 37) },
            { ColorName.RBOW_6, new Color(247, 224, 90) },
            { ColorName.RBOW_7, new Color(235, 137, 49) }
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