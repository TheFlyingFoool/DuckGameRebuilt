// Decompiled with JetBrains decompiler
// Type: DuckGame.FontGDIContext
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
    internal static class FontGDIContext
    {
        private static System.Drawing.Graphics _graphicsContext;
        private static Bitmap _drawingImage;
        //private static bool _dimensionsDirty = false;
        //private static float _setWidth = 0f;
        //private static float _setHeight = 0f;
        private static Font _systemFont;
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
        private static Font _lastFont;
        private static IntPtr _hfont;
        public static Dictionary<string, RasterFont.Data> _fontDatas = new Dictionary<string, RasterFont.Data>();
        private static List<FontRange> curFont;

        //public static void SetSize(float pSize)
        //{
        //    if (pSize == _size)
        //        return;
        //    FontGDIContext._size = pSize;
        //    //FontGDIContext._dirty = true;
        //}

        //public static void SetColor(Color pColor)
        //{
        //    if (!(FontGDIContext._color != pColor))
        //        return;
        //    FontGDIContext._color = pColor;
        //    //FontGDIContext._dirty = true;
        //}

        // public static int numCharactersToRender => FontGDIContext._numCharactersToRender;

        //public void SetNumCharactersToRender(int pNum)
        //{
        //    if (FontGDIContext._numCharactersToRender == pNum)
        //        return;
        //    FontGDIContext._numCharactersToRender = pNum;
        //    //FontGDIContext._dirty = true;
        //}

        //public static void SetAntiAliasing(bool pAnti)
        //{
        //    if (FontGDIContext._antiAliasing == pAnti)
        //        return;
        //    FontGDIContext._antiAliasing = pAnti;
        //    //FontGDIContext._dirty = true;
        //    //FontGDIContext._contextDirty = true;
        //}

        //public static void SetFontStyle(FontStyle pStyle)
        //{
        //    if (FontGDIContext._fontStyle == pStyle)
        //        return;
        //    FontGDIContext._fontStyle = pStyle;
        //    //FontGDIContext._dirty = true;
        //}

        private static StringFormat GetStringFormatting(bool pCenter = false)
        {
            if (_formatting == null)
            {
                _formatting = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    Trimming = StringTrimming.None,
                    FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoFontFallback | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip
                };
            }
            return _formatting;
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
          [MarshalAs(UnmanagedType.LPArray, SizeConst = 1, ArraySubType = UnmanagedType.LPStruct), Out] ABC[] lpabc);

        //private static FontGDIContext.ABC GetCharWidthABC(
        //  char ch,
        //  System.Drawing.Font font,
        //  System.Drawing.Graphics gr)
        //{
        //    FontGDIContext.ABC[] lpabc = new FontGDIContext.ABC[1];
        //    IntPtr hdc = gr.GetHdc();
        //    IntPtr hfont = ((System.Drawing.Font)font.Clone()).ToHfont();
        //    FontGDIContext.SelectObject(hdc, hfont);
        //    FontGDIContext.GetCharABCWidthsW(hdc, ch, ch, lpabc);
        //    FontGDIContext.DeleteObject(hfont);
        //    gr.ReleaseHdc();
        //    return lpabc[0];
        //}

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int SetMapMode(IntPtr hdc, int value);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetCharABCWidthsFloatW(
          IntPtr hdc,
          uint uFirstChar,
          uint uLastChar,
          [MarshalAs(UnmanagedType.LPArray, SizeConst = 1, ArraySubType = UnmanagedType.LPStruct), Out] ABCFloat[] lpabc);

        private static ABCFloat GetCharABCWidthsFloat(
          char ch,
          Font font,
          System.Drawing.Graphics gr)
        {
            ABCFloat[] lpabc = new ABCFloat[1];
            IntPtr hdc = gr.GetHdc();
            if (_lastFont != font)
                _hfont = ((Font)font.Clone()).ToHfont();
            SelectObject(hdc, _hfont);
            GetCharABCWidthsFloatW(hdc, ch, ch, lpabc);
            DeleteObject(_hfont);
            gr.ReleaseHdc();
            return lpabc[0];
        }

        private static ABCFloat[] GetCharABCWidthsRange(
          char ch,
          char chend,
          Font font,
          System.Drawing.Graphics gr)
        {
            ABCFloat[] lpabc = new ABCFloat[chend - ch + 2];
            IntPtr hdc = gr.GetHdc();
            if (_lastFont != font)
                _hfont = ((Font)font.Clone()).ToHfont();
            SelectObject(hdc, _hfont);
            GetCharABCWidthsFloatW(hdc, ch, chend, lpabc);
            DeleteObject(_hfont);
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
            if (_fontDatas.TryGetValue(key, out rasterFontData1))
                return rasterFontData1;
            if (_fontDatas.Count > 8)
                _fontDatas.Clear();
            if (pSize > 120f)
                pSize = 120f;
            _size = pSize;
            _fontStyle = pStyle;
            RasterFont.Data rasterFontData2 = new RasterFont.Data
            {
                fontSize = pSize
            };
            _fontDatas[key] = rasterFontData2;
            _fontPath = pFullFontPath;
            int num1 = Resolution.current.y / 72;
            _systemFont = new Font(_fontPath, _size * RasterFont.fontScaleFactor, _fontStyle, GraphicsUnit.Pixel);
            _fontStyle = pStyle;
            _size = pSize;
            rasterFontData2.name = _systemFont.Name;
            _graphicsContext = System.Drawing.Graphics.FromImage(new Bitmap(32, 32, PixelFormat.Format32bppArgb));
            _graphicsContext.PageUnit = GraphicsUnit.Pixel;
            ABCFloat[] charAbcWidthsRange = GetCharABCWidthsRange(char.MinValue, 'ё', _systemFont, _graphicsContext);
            ABCFloat abcFloat1 = charAbcWidthsRange[87];
            float num2 = abcFloat1.abcB + Math.Abs(abcFloat1.abcA) + Math.Abs(abcFloat1.abcC);
            float val1 = (int)(num2 * Math.Sqrt(FancyBitmapFont._characters.Length) / num2) * (_systemFont.GetHeight() + 8f);
            float num3 = MonoMain.hidef ? Math.Min(val1, 4096f) : Math.Min(val1, 2048f);
            _drawingImage = new Bitmap((int)num3, (int)num3, PixelFormat.Format32bppPArgb);
            _graphicsContext = System.Drawing.Graphics.FromImage(_drawingImage);
            _graphicsContext.PageUnit = GraphicsUnit.Pixel;
            if (pSmooth)
            {
                _graphicsContext.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            }
            else
            {
                _graphicsContext.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                _graphicsContext.SmoothingMode = SmoothingMode.None;
                _graphicsContext.InterpolationMode = InterpolationMode.NearestNeighbor;
                _graphicsContext.PixelOffsetMode = PixelOffsetMode.None;
            }
            _graphicsContext.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));
            _brush = new SolidBrush(System.Drawing.Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
            Pen pen = new Pen(System.Drawing.Color.FromArgb(100, 0, byte.MaxValue, 0));
            rasterFontData2.fontHeight = _systemFont.GetHeight();
            float x = 0f;
            float y = 0f;
            foreach (char character in FancyBitmapFont._characters)
            {
                ABCFloat abcFloat2 = charAbcWidthsRange[character];
                BitmapFont_CharacterInfo fontCharacterInfo = new BitmapFont_CharacterInfo
                {
                    leading = abcFloat2.abcA,
                    width = abcFloat2.abcB,
                    trailing = abcFloat2.abcC
                };
                float abcB = abcFloat2.abcB;
                int height = (int)rasterFontData2.fontHeight + 8;
                float num4 = (abcB + Math.Abs(fontCharacterInfo.leading) + Math.Abs(fontCharacterInfo.trailing) + 8f);
                if (x + num4 > _drawingImage.Width)
                {
                    y += height + 2;
                    x = 0f;
                }
                fontCharacterInfo.area = new Rectangle(x, y, num4 - 2f, height);
                rasterFontData2.characters.Add(fontCharacterInfo);
                _graphicsContext.DrawString(character.ToString() ?? "", _systemFont, _brush, (x + fontCharacterInfo.trailing / 2f - fontCharacterInfo.leading / 2f + abcB / 2f + 2f), y, GetStringFormatting());
                x += num4;
            }
            _graphicsContext.Flush();
            uint[] numArray = new uint[_drawingImage.Width * _drawingImage.Height];
            uint* scan0 = (uint*)(void*)_drawingImage.LockBits(new System.Drawing.Rectangle(0, 0, _drawingImage.Width, _drawingImage.Height), ImageLockMode.ReadOnly, _drawingImage.PixelFormat).Scan0;
            for (int index = 0; index < numArray.Length; ++index)
            {
                uint num5 = scan0[index] << 8 | scan0[index] >> 24;
                numArray[index] = num5;
            }
            rasterFontData2.colors = numArray;
            rasterFontData2.colorsWidth = _drawingImage.Width;
            rasterFontData2.colorsHeight = _drawingImage.Height;
            _graphicsContext.Dispose();
            _drawingImage.Dispose();
            _systemFont.Dispose();
            return rasterFontData2;
        }

        [DllImport("gdi32.dll")]
        public static extern uint GetFontUnicodeRanges(IntPtr hdc, IntPtr lpgs);

        public static List<FontRange> GetUnicodeRangesForFont(Font font)
        {
            if (curFont == null)
            {
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                IntPtr hdc = graphics.GetHdc();
                IntPtr hfont = font.ToHfont();
                IntPtr hObj = SelectObject(hdc, hfont);
                IntPtr num1 = Marshal.AllocHGlobal((int)GetFontUnicodeRanges(hdc, IntPtr.Zero));
                int fontUnicodeRanges = (int)GetFontUnicodeRanges(hdc, num1);
                List<FontRange> fontRangeList = new List<FontRange>();
                int num2 = Marshal.ReadInt32(num1, 12);
                for (int index = 0; index < num2; ++index)
                {
                    FontRange fontRange = new FontRange()
                    {
                        Low = (ushort)Marshal.ReadInt16(num1, 16 + index * 4)
                    };
                    fontRange.High = (ushort)(fontRange.Low + Marshal.ReadInt16(num1, 18 + index * 4) - 1);
                    fontRangeList.Add(fontRange);
                }
                SelectObject(hdc, hObj);
                Marshal.FreeHGlobal(num1);
                graphics.ReleaseHdc(hdc);
                graphics.Dispose();
                curFont = fontRangeList;
            }
            return curFont;
        }

        //public static bool CheckIfCharInFont(char character, System.Drawing.Font font)
        //{
        //    ushort uint16 = Convert.ToUInt16(character);
        //    List<FontGDIContext.FontRange> unicodeRangesForFont = FontGDIContext.GetUnicodeRangesForFont(font);
        //    bool flag = false;
        //    foreach (FontGDIContext.FontRange fontRange in unicodeRangesForFont)
        //    {
        //        if (uint16 >= fontRange.Low && uint16 <= fontRange.High)
        //        {
        //            flag = true;
        //            break;
        //        }
        //    }
        //    return flag;
        //}

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
