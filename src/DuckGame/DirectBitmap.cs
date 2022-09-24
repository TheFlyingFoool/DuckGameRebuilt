using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DuckGame
{
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; set; }
        public int[] Bits { get; set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, System.Drawing.Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public System.Drawing.Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            System.Drawing.Color result = System.Drawing.Color.FromArgb(col);

            return result;
        }
        private static int MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return ((int)((ulong)((int)red << 16 | (int)green << 8 | (int)blue | (int)alpha << 24)) & -1);
        }
        private static Color FromArgbDG(long Value)
        {
            return new Color() { r = (byte)(Value >> 16 & 255L), g = (byte)(Value >> 8 & 255L), b = (byte)(Value & 255L), a = (byte)(Value >> 24 & 255L) };
        }
        public void SetPixelDG(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = MakeArgb(colour.a, colour.r, colour.g, colour.b);
            Bits[index] = col;
        }
        public Color GetPixelDG(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            //System.Drawing.Color result = System.Drawing.Color.FromArgb(col);
            return FromArgbDG(col);
        }
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
        public void UnPink()
        {
            for (int y = 0; y < Bitmap.Height; y++)
                for (int x = 0; x < Bitmap.Width; x++)
                {
                    System.Drawing.Color PixelColor = Bitmap.GetPixel(x, y);
                    if (PixelColor.R == 255 && PixelColor.B == 255 && PixelColor.G == 0 && (int)PixelColor.A == 255)
                    {
                        Bitmap.SetPixel(x, y, System.Drawing.Color.Transparent);
                    }
                }
        }
    }
}
