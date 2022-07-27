// Decompiled with JetBrains decompiler
// Type: DuckGame.FontGDIContext
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace DuckGame
{
    internal class FontGDIContext
    {
        private static System.Drawing.Graphics _graphicsContext;
        private static Bitmap _drawingImage;
        //private static bool _dimensionsDirty = false;
        //private static float _setWidth = 0.0f;
        //private static float _setHeight = 0.0f;
        private static System.Drawing.Font _systemFont;
        private static List<string> _loadedFonts = new List<string>();
        private static string _fontPath;
        private static float _size;
        private static Color _color;
        //private static bool _dirty = false;
        //private static bool _contextDirty = false;
        private static Brush _brush;
        private static int _numCharactersToRender = -1;
        private static bool _antiAliasing;
        private static FontStyle _fontStyle = FontStyle.Regular;
        private static StringFormat _formatting;
        private static System.Drawing.Font _lastFont;
        private static IntPtr _hfont;
        public static Dictionary<string, RasterFont.Data> _fontDatas = new Dictionary<string, RasterFont.Data>();
        private static List<FontGDIContext.FontRange> curFont;

        public static void SetSize(float pSize)
        {
            if ((double)pSize == _size)
                return;
            FontGDIContext._size = pSize;
            //FontGDIContext._dirty = true;
        }

        public static void SetColor(Color pColor)
        {
            if (!(FontGDIContext._color != pColor))
                return;
            FontGDIContext._color = pColor;
            //FontGDIContext._dirty = true;
        }

        public static int numCharactersToRender => FontGDIContext._numCharactersToRender;

        public void SetNumCharactersToRender(int pNum)
        {
            if (FontGDIContext._numCharactersToRender == pNum)
                return;
            FontGDIContext._numCharactersToRender = pNum;
            //FontGDIContext._dirty = true;
        }

        public static void SetAntiAliasing(bool pAnti)
        {
            if (FontGDIContext._antiAliasing == pAnti)
                return;
            FontGDIContext._antiAliasing = pAnti;
            //FontGDIContext._dirty = true;
            //FontGDIContext._contextDirty = true;
        }

        public static void SetFontStyle(FontStyle pStyle)
        {
            if (FontGDIContext._fontStyle == pStyle)
                return;
            FontGDIContext._fontStyle = pStyle;
            //FontGDIContext._dirty = true;
        }

        private static StringFormat GetStringFormatting(bool pCenter = false)
        {
            if (FontGDIContext._formatting == null)
            {
                FontGDIContext._formatting = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    Trimming = StringTrimming.None,
                    FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoFontFallback | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip
                };
            }
            return FontGDIContext._formatting;
        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern int DeleteObject(IntPtr hObj);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetCharABCWidthsW(
          IntPtr hdc,
          uint uFirstChar,
          uint uLastChar,
          [MarshalAs(UnmanagedType.LPArray, SizeConst = 1, ArraySubType = UnmanagedType.LPStruct), Out] FontGDIContext.ABC[] lpabc);

        private static FontGDIContext.ABC GetCharWidthABC(
          char ch,
          System.Drawing.Font font,
          System.Drawing.Graphics gr)
        {
            FontGDIContext.ABC[] lpabc = new FontGDIContext.ABC[1];
            IntPtr hdc = gr.GetHdc();
            IntPtr hfont = ((System.Drawing.Font)font.Clone()).ToHfont();
            FontGDIContext.SelectObject(hdc, hfont);
            FontGDIContext.GetCharABCWidthsW(hdc, ch, ch, lpabc);
            FontGDIContext.DeleteObject(hfont);
            gr.ReleaseHdc();
            return lpabc[0];
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int SetMapMode(IntPtr hdc, int value);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetCharABCWidthsFloatW(
          IntPtr hdc,
          uint uFirstChar,
          uint uLastChar,
          [MarshalAs(UnmanagedType.LPArray, SizeConst = 1, ArraySubType = UnmanagedType.LPStruct), Out] FontGDIContext.ABCFloat[] lpabc);

        private static FontGDIContext.ABCFloat GetCharABCWidthsFloat(
          char ch,
          System.Drawing.Font font,
          System.Drawing.Graphics gr)
        {
            FontGDIContext.ABCFloat[] lpabc = new FontGDIContext.ABCFloat[1];
            IntPtr hdc = gr.GetHdc();
            if (FontGDIContext._lastFont != font)
                FontGDIContext._hfont = ((System.Drawing.Font)font.Clone()).ToHfont();
            FontGDIContext.SelectObject(hdc, FontGDIContext._hfont);
            FontGDIContext.GetCharABCWidthsFloatW(hdc, ch, ch, lpabc);
            FontGDIContext.DeleteObject(FontGDIContext._hfont);
            gr.ReleaseHdc();
            return lpabc[0];
        }

        private static FontGDIContext.ABCFloat[] GetCharABCWidthsRange(
          char ch,
          char chend,
          System.Drawing.Font font,
          System.Drawing.Graphics gr)
        {
            FontGDIContext.ABCFloat[] lpabc = new FontGDIContext.ABCFloat[chend - ch + 2];
            IntPtr hdc = gr.GetHdc();
            if (FontGDIContext._lastFont != font)
                FontGDIContext._hfont = ((System.Drawing.Font)font.Clone()).ToHfont();
            FontGDIContext.SelectObject(hdc, FontGDIContext._hfont);
            FontGDIContext.GetCharABCWidthsFloatW(hdc, ch, chend, lpabc);
            FontGDIContext.DeleteObject(FontGDIContext._hfont);
            gr.ReleaseHdc();
            return lpabc;
        }

        public static string GetName(string fontFamilyName)
        {
            fontFamilyName = fontFamilyName.Replace("@BOLD@", "");
            fontFamilyName = fontFamilyName.Replace("@ITALIC@", "");
            try
            {
                using (FontFamily fontFamily = new FontFamily(fontFamilyName))
                    return fontFamily.IsStyleAvailable(FontStyle.Regular) || fontFamily.IsStyleAvailable(FontStyle.Bold) || fontFamily.IsStyleAvailable(FontStyle.Italic) || fontFamily.IsStyleAvailable(FontStyle.Bold | FontStyle.Italic) ? fontFamily.Name : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static unsafe RasterFont.Data CreateRasterFontData(
          string pFullFontPath,
          float pSize = 12f,
          FontStyle pStyle = FontStyle.Regular,
          bool pSmooth = true)
        {
            if (pFullFontPath.Contains("@BOLD@"))
            {
                pStyle = FontStyle.Bold;
                pFullFontPath = pFullFontPath.Replace("@BOLD@", "");
            }
            if (pFullFontPath.Contains("@ITALIC@"))
            {
                pStyle = FontStyle.Italic;
                pFullFontPath = pFullFontPath.Replace("@ITALIC@", "");
            }
            string key = pFullFontPath + pSize.ToString() + pStyle.ToString();
            RasterFont.Data rasterFontData1 = null;
            if (FontGDIContext._fontDatas.TryGetValue(key, out rasterFontData1))
                return rasterFontData1;
            if (FontGDIContext._fontDatas.Count > 8)
                FontGDIContext._fontDatas.Clear();
            if ((double)pSize > 120.0)
                pSize = 120f;
            FontGDIContext._size = pSize;
            FontGDIContext._fontStyle = pStyle;
            RasterFont.Data rasterFontData2 = new RasterFont.Data
            {
                fontSize = pSize
            };
            FontGDIContext._fontDatas[key] = rasterFontData2;
            FontGDIContext._fontPath = pFullFontPath;
            int num1 = Resolution.current.y / 72;
            FontGDIContext._systemFont = new System.Drawing.Font(FontGDIContext._fontPath, FontGDIContext._size * RasterFont.fontScaleFactor, FontGDIContext._fontStyle, GraphicsUnit.Pixel);
            FontGDIContext._fontStyle = pStyle;
            FontGDIContext._size = pSize;
            rasterFontData2.name = FontGDIContext._systemFont.Name;
            FontGDIContext._graphicsContext = System.Drawing.Graphics.FromImage(new Bitmap(32, 32, PixelFormat.Format32bppArgb));
            FontGDIContext._graphicsContext.PageUnit = GraphicsUnit.Pixel;
            FontGDIContext.ABCFloat[] charAbcWidthsRange = FontGDIContext.GetCharABCWidthsRange(char.MinValue, 'ё', FontGDIContext._systemFont, FontGDIContext._graphicsContext);
            FontGDIContext.ABCFloat abcFloat1 = charAbcWidthsRange[87];
            float num2 = abcFloat1.abcB + Math.Abs(abcFloat1.abcA) + Math.Abs(abcFloat1.abcC);
            float val1 = (int)((double)num2 * Math.Sqrt(FancyBitmapFont._characters.Length) / (double)num2) * (FontGDIContext._systemFont.GetHeight() + 8f);
            float num3 = MonoMain.hidef ? Math.Min(val1, 4096f) : Math.Min(val1, 2048f);
            FontGDIContext._drawingImage = new Bitmap((int)num3, (int)num3, PixelFormat.Format32bppPArgb);
            FontGDIContext._graphicsContext = System.Drawing.Graphics.FromImage(_drawingImage);
            FontGDIContext._graphicsContext.PageUnit = GraphicsUnit.Pixel;
            if (pSmooth)
            {
                FontGDIContext._graphicsContext.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            }
            else
            {
                FontGDIContext._graphicsContext.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                FontGDIContext._graphicsContext.SmoothingMode = SmoothingMode.None;
                FontGDIContext._graphicsContext.InterpolationMode = InterpolationMode.NearestNeighbor;
                FontGDIContext._graphicsContext.PixelOffsetMode = PixelOffsetMode.None;
            }
            FontGDIContext._graphicsContext.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));
            FontGDIContext._brush = new SolidBrush(System.Drawing.Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Pen pen = new Pen(System.Drawing.Color.FromArgb(100, 0, byte.MaxValue, 0));
            rasterFontData2.fontHeight = FontGDIContext._systemFont.GetHeight();
            float x = 0.0f;
            float y = 0.0f;
            foreach (char character in FancyBitmapFont._characters)
            {
                FontGDIContext.ABCFloat abcFloat2 = charAbcWidthsRange[character];
                BitmapFont_CharacterInfo fontCharacterInfo = new BitmapFont_CharacterInfo
                {
                    leading = abcFloat2.abcA,
                    width = abcFloat2.abcB,
                    trailing = abcFloat2.abcC
                };
                float abcB = abcFloat2.abcB;
                int height = (int)rasterFontData2.fontHeight + 8;
                float num4 = (float)((double)abcB + (double)Math.Abs(fontCharacterInfo.leading) + (double)Math.Abs(fontCharacterInfo.trailing) + 8.0);
                if ((double)x + (double)num4 > _drawingImage.Width)
                {
                    y += height + 2;
                    x = 0.0f;
                }
                fontCharacterInfo.area = new Rectangle(x, y, num4 - 2f, height);
                rasterFontData2.characters.Add(fontCharacterInfo);
                FontGDIContext._graphicsContext.DrawString(character.ToString() ?? "", FontGDIContext._systemFont, FontGDIContext._brush, (float)((double)x + fontCharacterInfo.trailing / 2.0 - fontCharacterInfo.leading / 2.0 + (double)abcB / 2.0 + 2.0), y, FontGDIContext.GetStringFormatting());
                x += num4;
            }
            FontGDIContext._graphicsContext.Flush();
            uint[] numArray = new uint[FontGDIContext._drawingImage.Width * FontGDIContext._drawingImage.Height];
            uint* scan0 = (uint*)(void*)FontGDIContext._drawingImage.LockBits(new System.Drawing.Rectangle(0, 0, FontGDIContext._drawingImage.Width, FontGDIContext._drawingImage.Height), ImageLockMode.ReadOnly, FontGDIContext._drawingImage.PixelFormat).Scan0;
            for (int index = 0; index < numArray.Length; ++index)
            {
                uint num5 = scan0[index] << 8 | scan0[index] >> 24;
                numArray[index] = num5;
            }
            rasterFontData2.colors = numArray;
            rasterFontData2.colorsWidth = FontGDIContext._drawingImage.Width;
            rasterFontData2.colorsHeight = FontGDIContext._drawingImage.Height;
            FontGDIContext._graphicsContext.Dispose();
            FontGDIContext._drawingImage.Dispose();
            FontGDIContext._systemFont.Dispose();
            return rasterFontData2;
        }

        [DllImport("gdi32.dll")]
        public static extern uint GetFontUnicodeRanges(IntPtr hdc, IntPtr lpgs);

        public static List<FontGDIContext.FontRange> GetUnicodeRangesForFont(System.Drawing.Font font)
        {
            if (FontGDIContext.curFont == null)
            {
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                IntPtr hdc = graphics.GetHdc();
                IntPtr hfont = font.ToHfont();
                IntPtr hObj = FontGDIContext.SelectObject(hdc, hfont);
                IntPtr num1 = Marshal.AllocHGlobal((int)FontGDIContext.GetFontUnicodeRanges(hdc, IntPtr.Zero));
                int fontUnicodeRanges = (int)FontGDIContext.GetFontUnicodeRanges(hdc, num1);
                List<FontGDIContext.FontRange> fontRangeList = new List<FontGDIContext.FontRange>();
                int num2 = Marshal.ReadInt32(num1, 12);
                for (int index = 0; index < num2; ++index)
                {
                    FontGDIContext.FontRange fontRange = new FontGDIContext.FontRange()
                    {
                        Low = (ushort)Marshal.ReadInt16(num1, 16 + index * 4)
                    };
                    fontRange.High = (ushort)(fontRange.Low + Marshal.ReadInt16(num1, 18 + index * 4) - 1);
                    fontRangeList.Add(fontRange);
                }
                FontGDIContext.SelectObject(hdc, hObj);
                Marshal.FreeHGlobal(num1);
                graphics.ReleaseHdc(hdc);
                graphics.Dispose();
                FontGDIContext.curFont = fontRangeList;
            }
            return FontGDIContext.curFont;
        }

        public static bool CheckIfCharInFont(char character, System.Drawing.Font font)
        {
            ushort uint16 = Convert.ToUInt16(character);
            List<FontGDIContext.FontRange> unicodeRangesForFont = FontGDIContext.GetUnicodeRangesForFont(font);
            bool flag = false;
            foreach (FontGDIContext.FontRange fontRange in unicodeRangesForFont)
            {
                if (uint16 >= fontRange.Low && uint16 <= fontRange.High)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public struct ABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;
        }

        public struct ABCFloat
        {
            public float abcA;
            public float abcB;
            public float abcC;
        }

        public struct FontRange
        {
            public ushort Low;
            public ushort High;
        }
    }
}
