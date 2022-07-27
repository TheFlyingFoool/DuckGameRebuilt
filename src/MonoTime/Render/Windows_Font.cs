// Decompiled with JetBrains decompiler
// Type: DuckGame.FontInfo
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Drawing;

namespace DuckGame
{
    public class FontInfo
    {
        public float EmHeightPixels;
        public float AscentPixels;
        public float DescentPixels;
        public float CellHeightPixels;
        public float InternalLeadingPixels;
        public float LineSpacingPixels;
        public float ExternalLeadingPixels;
        public float RelTop;
        public float RelBaseline;
        public float RelBottom;

        public FontInfo(System.Drawing.Graphics gr, System.Drawing.Font the_font)
        {
            float emHeight = the_font.FontFamily.GetEmHeight(the_font.Style);
            this.EmHeightPixels = this.ConvertUnits(gr, the_font.Size, the_font.Unit, GraphicsUnit.Pixel);
            float num = this.EmHeightPixels / emHeight;
            this.AscentPixels = num * the_font.FontFamily.GetCellAscent(the_font.Style);
            this.DescentPixels = num * the_font.FontFamily.GetCellDescent(the_font.Style);
            this.CellHeightPixels = this.AscentPixels + this.DescentPixels;
            this.InternalLeadingPixels = this.CellHeightPixels - this.EmHeightPixels;
            this.LineSpacingPixels = num * the_font.FontFamily.GetLineSpacing(the_font.Style);
            this.ExternalLeadingPixels = this.LineSpacingPixels - this.CellHeightPixels;
            this.RelTop = this.InternalLeadingPixels;
            this.RelBaseline = this.AscentPixels;
            this.RelBottom = this.CellHeightPixels;
        }

        private float ConvertUnits(
          System.Drawing.Graphics gr,
          float value,
          GraphicsUnit from_unit,
          GraphicsUnit to_unit)
        {
            if (from_unit == to_unit)
                return value;
            switch (from_unit)
            {
                case GraphicsUnit.Pixel:
                    switch (to_unit)
                    {
                        case GraphicsUnit.Pixel:
                            return value;
                        case GraphicsUnit.Point:
                            value /= gr.DpiX / 72f;
                            goto case GraphicsUnit.Pixel;
                        case GraphicsUnit.Inch:
                            value /= gr.DpiX;
                            goto case GraphicsUnit.Pixel;
                        case GraphicsUnit.Document:
                            value /= gr.DpiX / 300f;
                            goto case GraphicsUnit.Pixel;
                        case GraphicsUnit.Millimeter:
                            value /= gr.DpiX / 25.4f;
                            goto case GraphicsUnit.Pixel;
                        default:
                            throw new Exception("Unknown output unit " + to_unit.ToString() + " in FontInfo.ConvertUnits");
                    }
                case GraphicsUnit.Point:
                    value *= gr.DpiX / 72f;
                    goto case GraphicsUnit.Pixel;
                case GraphicsUnit.Inch:
                    value *= gr.DpiX;
                    goto case GraphicsUnit.Pixel;
                case GraphicsUnit.Document:
                    value *= gr.DpiX / 300f;
                    goto case GraphicsUnit.Pixel;
                case GraphicsUnit.Millimeter:
                    value *= gr.DpiX / 25.4f;
                    goto case GraphicsUnit.Pixel;
                default:
                    throw new Exception("Unknown input unit " + from_unit.ToString() + " in FontInfo.ConvertUnits");
            }
        }
    }
}
