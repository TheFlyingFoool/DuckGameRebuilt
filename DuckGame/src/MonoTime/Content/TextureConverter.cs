using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using XnaToFna;

namespace DuckGame
{
    internal static class TextureConverter
    {
        private const int _fromColor = -65281;
        private const int _toColor = 0;
        public static bool lastLoadResultedInResize = false;
        private static Vec2 _maxDimensions = Vec2.Zero;
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
        {
            return new Color(r * a / 255, g * a / 255, b * a / 255, a);
        }
        internal static Texture2D MemLoadPNGDataWithPinkAwesomeness(GraphicsDevice device,
          Bitmap bitmap,
          bool process)
        {
            lastLoadResultedInResize = false;
            //Console.WriteLine(bitmap.Width.ToString() + " " + bitmap.Height.ToString());
            if (_maxDimensions != Vec2.Zero)
            {
                float width1 = _maxDimensions.x;
                float height1 = _maxDimensions.y;
                float num = Math.Min(width1 / bitmap.Width, height1 / bitmap.Height);
                if (width1 < bitmap.Width || height1 < bitmap.Height)
                {
                    lastLoadResultedInResize = true;
                    if (bitmap.Width * num < width1)
                        width1 = bitmap.Width * num;
                    if (bitmap.Height * num < height1)
                        height1 = bitmap.Height * num;
                    int width3 = (int)width1;
                    int height3 = (int)height1;
                    Bitmap bitmap1 = new Bitmap(width3, height3);
                    System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap1);
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    ImageAttributes imageAttr = new ImageAttributes();
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    //int width2 = bitmap.Width;
                    //int height2 = bitmap.Height;
                    System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, width3, height3);
                    graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
                    bitmap.Dispose();
                    graphics.Dispose();
                    bitmap = bitmap1;
                }
            }
            if (process)
                bitmap.MakeTransparent(System.Drawing.Color.Magenta);
            Texture2D Tex;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                Tex = Texture2D.FromStream(device, ms);
                Color[] buffer = new Color[Tex.Width * Tex.Height];
                Tex.GetData(buffer);
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = FromNonPremultiplied(buffer[i].r, buffer[i].g, buffer[i].b, buffer[i].a); // Needs to handle transparent textures that use other types of draw calls
                Tex.SetData(buffer);
            }
            return Tex;

            //int offset = n.Length - ((bitmap.Width * bitmap.Height) * 4);
            //for (var i = n.Length - 1 - offset; i >= 0; i-=4)
            //{
            //    byte[] bytes = { n[i], n[i-1], n[i-2], n[i-3] };
            //    int num = BitConverter.ToInt32(bytes, 0);
            //    destination3[i/4] = num;
            //}
            //BitmapData bitmapdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb); // 
            //int[] destination2 = new int[bitmapdata.Width * bitmapdata.Height];
            //Marshal.Copy(bitmapdata.Scan0, destination2, 0, destination2.Length);
            //Console.WriteLine("3");
            //int num1 = bitmapdata.Width * bitmapdata.Height;
            //int num2 = 0;
            //int* scan0 = (int*)(void*)bitmapdata.Scan0;
            //while (num2 < num1)
            //{
            //    if (process && *scan0 == -65281)
            //    {
            //        *scan0 = 0;
            //    }
            //    else
            //    {
            //        byte* numPtr = (byte*)scan0;
            //        byte num3 = *numPtr;
            //        *numPtr = numPtr[2];
            //        numPtr[2] = num3;
            //        float num4 = numPtr[3] / (float)byte.MaxValue;
            //        for (int index = 0; index < 3; ++index)
            //            numPtr[index] = (byte)(numPtr[index] * (double)num4);
            //    }
            //    ++num2;
            //    ++scan0;
            //}
            //Console.WriteLine("4");
            //int[] destination = new int[bitmapdata.Width * bitmapdata.Height];
            //Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);
            //PNGData pngData = new PNGData
            //{
            //    data = destination,
            //    width = bitmapdata.Width,
            //    height = bitmapdata.Height
            //};
            //Console.WriteLine("5");
            //bitmap.UnlockBits(bitmapdata);
            //Console.WriteLine("6");
            //return pngData;
        }

        internal static PNGData LoadPNGDataWithPinkAwesomeness(
    Texture2D texture,
    bool process)
        {
            lastLoadResultedInResize = false;

            int width = texture.Width;
            int height = texture.Height;

            Color[] data = new Color[width * height];
            texture.GetData(data);

            // -----------------------------
            // Resize (replacement for Bitmap scaling)
            // -----------------------------
            if (_maxDimensions != Vec2.Zero)
            {
                int maxW = (int)_maxDimensions.x;
                int maxH = (int)_maxDimensions.y;

                float scale = Math.Min((float)maxW / width, (float)maxH / height);

                if (scale < 1f)
                {
                    lastLoadResultedInResize = true;

                    int newW = (int)(width * scale);
                    int newH = (int)(height * scale);

                    data = ResizeBilinear(data, width, height, newW, newH);
                    width = newW;
                    height = newH;
                }
            }

            // -----------------------------
            // Process pixels (replaces unsafe block)
            // -----------------------------
            int[] destination = new int[width * height];

            for (int i = 0; i < data.Length; i++)
            {
                Color c = data[i];

                // Magenta → transparent (same as *scan0 == -65281)
                if (process && c.r == 255 && c.g == 0 && c.b == 255)
                {
                    destination[i] = 0;
                    continue;
                }

                // Premultiply alpha
                float a = c.a / 255f;

                byte r = (byte)(c.r * a);
                byte g = (byte)(c.g * a);
                byte b = (byte)(c.b * a);

                // Pack into int (same layout as original ARGB)
                destination[i] =
                    (c.a << 24) |
                    (r << 16) |
                    (g << 8) |
                    b;
            }

            return new PNGData
            {
                data = destination,
                width = width,
                height = height
            };
        }

        internal static unsafe PNGData LoadPNGDataWithPinkAwesomeness(
          Bitmap bitmap,
          bool process)
        {
            lastLoadResultedInResize = false;
            if (_maxDimensions != Vec2.Zero)
            {
                float width1 = _maxDimensions.x;
                float height1 = _maxDimensions.y;
                float num = Math.Min(width1 / bitmap.Width, height1 / bitmap.Height);
                if (width1 < bitmap.Width || height1 < bitmap.Height)
                {
                    lastLoadResultedInResize = true;
                    if (bitmap.Width * num < width1)
                        width1 = bitmap.Width * num;
                    if (bitmap.Height * num < height1)
                        height1 = bitmap.Height * num;
                    Bitmap bitmap1 = new Bitmap((int)width1, (int)height1);
                    System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap1);
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    ImageAttributes imageAttr = new ImageAttributes();
                    imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    int width2 = bitmap.Width;
                    int height2 = bitmap.Height;
                    System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, (int)width1, (int)height1);
                    graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
                    bitmap.Dispose();
                    graphics.Dispose();
                    bitmap = bitmap1;
                }
            }
            BitmapData bitmapdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int num1 = bitmapdata.Width * bitmapdata.Height;
            int num2 = 0;
            int* scan0 = (int*)(void*)bitmapdata.Scan0;
            while (num2 < num1)
            {
                if (process && *scan0 == -65281)
                {
                    *scan0 = 0;
                }
                else
                {
                    byte* numPtr = (byte*)scan0;
                    byte num3 = *numPtr;
                    *numPtr = numPtr[2];
                    numPtr[2] = num3;
                    float num4 = numPtr[3] / (float)byte.MaxValue;
                    for (int index = 0; index < 3; ++index)
                        numPtr[index] = (byte)(numPtr[index] * num4);
                }
                ++num2;
                ++scan0;
            }
            int[] destination = new int[bitmapdata.Width * bitmapdata.Height];
            Marshal.Copy(bitmapdata.Scan0, destination, 0, destination.Length);
            PNGData pngData = new PNGData
            {
                data = destination,
                width = bitmapdata.Width,
                height = bitmapdata.Height
            };
            bitmap.UnlockBits(bitmapdata);
            return pngData;
        }



        internal static Texture2D LoadPNGWithPinkAwesomeness(
            GraphicsDevice device,
            byte[] pData,
            bool process)
        {
            using (MemoryStream ms = new MemoryStream(pData))
            {
                Texture2D tex = Texture2D.FromStream(device, ms);

                return MemLoadPNGDataWithPinkAwesomeness(device, tex, process);
            }
        }
        internal static Texture2D LoadPNGWithPinkAwesomenessNew(
         GraphicsDevice device,
         Stream stream,
         bool process)
        {
            Texture2D tex = Texture2D.FromStream(device, stream);

            return MemLoadPNGDataWithPinkAwesomeness(device, tex, process);
        }
        internal static Texture2D MemLoadPNGDataWithPinkAwesomeness(
            GraphicsDevice device,
            Texture2D tex,
            bool process)
        {
            lastLoadResultedInResize = false;

            int srcW = tex.Width;
            int srcH = tex.Height;

            Color[] src = new Color[srcW * srcH];
            tex.GetData(src);
            if (_maxDimensions != Vec2.Zero)
            {
                int maxW = (int)_maxDimensions.x;
                int maxH = (int)_maxDimensions.y;

                float scale = Math.Min((float)maxW / srcW, (float)maxH / srcH);

                if (scale < 1f)
                {
                    lastLoadResultedInResize = true;

                    int newW = (int)(srcW * scale);
                    int newH = (int)(srcH * scale);

                    src = ResizeBilinear(src, srcW, srcH, newW, newH);
                    srcW = newW;
                    srcH = newH;
                }
            }
            if (process)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    Color c = src[i];

                    // match System.Drawing.Color.Magenta (255,0,255)
                    if (c.r == 255 && c.g == 0 && c.b == 255)
                        src[i] = Color.Transparent;
                }
            }
            // Premultiplied alpha conversion (
            //FromNonPremultiplied)
            for (int i = 0; i < src.Length; i++)
            {
                Color c = src[i];

                float a = c.a / 255f;

                src[i] = new Color(
                    (int)(c.r * a),
                    (int)(c.g * a),
                    (int)(c.b * a),
                    c.a
                );
            }

            Texture2D result = new Texture2D(device, srcW, srcH);
            result.SetData(src);
            return result;
        }
        private static Color[] ResizeBilinear(Color[] src, int srcW, int srcH, int dstW, int dstH)
        {
            Color[] dst = new Color[dstW * dstH];

            float scaleX = (float)srcW / dstW;
            float scaleY = (float)srcH / dstH;

            for (int y = 0; y < dstH; y++)
            {
                for (int x = 0; x < dstW; x++)
                {
                    float gx = x * scaleX;
                    float gy = y * scaleY;

                    int x0 = (int)gx;
                    int y0 = (int)gy;
                    int x1 = Math.Min(x0 + 1, srcW - 1);
                    int y1 = Math.Min(y0 + 1, srcH - 1);

                    float tx = gx - x0;
                    float ty = gy - y0;

                    Color c00 = src[y0 * srcW + x0];
                    Color c10 = src[y0 * srcW + x1];
                    Color c01 = src[y1 * srcW + x0];
                    Color c11 = src[y1 * srcW + x1];

                    dst[y * dstW + x] = new Color(
                        (byte)MathHelper.Lerp(
                            MathHelper.Lerp(c00.r, c10.r, tx),
                            MathHelper.Lerp(c01.r, c11.r, tx),
                            ty),

                        (byte)MathHelper.Lerp(
                            MathHelper.Lerp(c00.g, c10.g, tx),
                            MathHelper.Lerp(c01.g, c11.g, tx),
                            ty),

                        (byte)MathHelper.Lerp(
                            MathHelper.Lerp(c00.b, c10.b, tx),
                            MathHelper.Lerp(c01.b, c11.b, tx),
                            ty),

                        (byte)MathHelper.Lerp(
                            MathHelper.Lerp(c00.a, c10.a, tx),
                            MathHelper.Lerp(c01.a, c11.a, tx),
                            ty)
                    );
                }
            }

            return dst;
        }
        internal static Texture2D LoadPNGWithPinkAwesomeness(
          GraphicsDevice device,
          Bitmap bitmap,
          bool process)
        {
            //PNGData pngData = TextureConverter.LoadPNGDataWithPinkAwesomeness(bitmap, process);
            //Texture2D texture2D = new Texture2D(device, pngData.width, pngData.height);
            //texture2D.SetData<int>(pngData.data);
            Texture2D texture2D = MemLoadPNGDataWithPinkAwesomeness(device, bitmap, process);
            return texture2D;
        }


        internal static Texture2D LoadPNGWithPinkAwesomenessAndMaxDimensions(
          GraphicsDevice device,
          Bitmap bitmap,
          bool process,
          Vec2 pMaxDimensions)
        {
            _maxDimensions = pMaxDimensions;
            Texture2D texture2D = MemLoadPNGDataWithPinkAwesomeness(device, bitmap, process);
            //PNGData pngData = TextureConverter.LoadPNGDataWithPinkAwesomeness(bitmap, process);
            _maxDimensions = Vec2.Zero;
            // Texture2D texture2D = new Texture2D(device, pngData.width, pngData.height);
            // texture2D.SetData<int>(pngData.data);
            return texture2D;
        }

        internal static Texture2D LoadPNGWithPinkAwesomeness(
          GraphicsDevice device,
          Stream stream,
          bool process)
        {

            return TextureConverter.LoadPNGWithPinkAwesomenessNew(device, stream, process);
        }

        internal static PNGData LoadPNGDataWithPinkAwesomeness(Stream stream, bool process)
        {
            return LoadPNGDataWithPinkAwesomeness(Texture2D.FromStream(Graphics.device, stream), process);
           // using (Bitmap bitmap = new Bitmap(stream))
             //   return LoadPNGDataWithPinkAwesomeness(Graphics.device, stream, process);
        }

        internal static Texture2D LoadPNGWithPinkAwesomeness(
          GraphicsDevice device,
          string fileName,
          bool process)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                fileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(fileName), true);
            }
            try
            {
                using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return LoadPNGWithPinkAwesomeness(device, fs, process);
                //using (Bitmap bitmap = new Bitmap(fileName))
                //     return LoadPNGWithPinkAwesomeness(device, bitmap, process);
            }
            catch {
                return null;
            }
        }

        internal static Texture2D LoadPNGWithPinkAwesomenessAndMaxDimensions(
          GraphicsDevice device,
          string fileName,
          bool process,
          Vec2 maxDimensions)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                fileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(fileName), true);
            }
            return LoadPNGWithPinkAwesomenessAndMaxDimensions(device, File.OpenRead(fileName), process, maxDimensions);
            //using (Bitmap bitmap = new Bitmap(fileName))
            //    return LoadPNGWithPinkAwesomenessAndMaxDimensions(device, bitmap, process, maxDimensions);
        }

        internal static Texture2D LoadPNGWithPinkAwesomenessAndMaxDimensions(
         GraphicsDevice device,
         Stream stream,
         bool process,
         Vec2 pMaxDimensions)
        {
            _maxDimensions = pMaxDimensions;
            Texture2D texture2D = Texture2D.FromStream(device, stream);
            texture2D = MemLoadPNGDataWithPinkAwesomeness(device, texture2D, process);
            //PNGData pngData = TextureConverter.LoadPNGDataWithPinkAwesomeness(bitmap, process);
            _maxDimensions = Vec2.Zero;
            // Texture2D texture2D = new Texture2D(device, pngData.width, pngData.height);
            // texture2D.SetData<int>(pngData.data);
            return texture2D;
        }
        internal static PNGData LoadPNGDataWithPinkAwesomeness(
          GraphicsDevice device,
          string fileName,
          bool process)
        {
            if (Program.IsLinuxD || Program.isLinux)
            {
                fileName = XnaToFnaHelper.GetActualCaseForFileName(XnaToFnaHelper.FixPath(fileName), true);
            }
            return LoadPNGDataWithPinkAwesomeness(Texture2D.FromStream(Graphics.device, File.OpenRead(fileName)), process);
            //using (Bitmap bitmap = new Bitmap(fileName))
            //    return LoadPNGDataWithPinkAwesomeness(bitmap, process);
        }
    }
}