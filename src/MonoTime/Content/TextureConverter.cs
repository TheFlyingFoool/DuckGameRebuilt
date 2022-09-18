// Decompiled with JetBrains decompiler
// Type: DuckGame.TextureConverter
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        internal static Texture2D MemLoadPNGDataWithPinkAwesomeness(GraphicsDevice device,
          Bitmap bitmap,
          bool process)
        {
            TextureConverter.lastLoadResultedInResize = false;
            //Console.WriteLine(bitmap.Width.ToString() + " " + bitmap.Height.ToString());
            if (TextureConverter._maxDimensions != Vec2.Zero)
            {
                float width1 = TextureConverter._maxDimensions.x;
                float height1 = TextureConverter._maxDimensions.y;
                float num = Math.Min(width1 / bitmap.Width, height1 / bitmap.Height);
                if (width1 < bitmap.Width || height1 < bitmap.Height)
                {
                    TextureConverter.lastLoadResultedInResize = true;
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
                    int width2 = bitmap.Width;
                    int height2 = bitmap.Height;
                    System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, width3, height3);
                    graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttr);
                    bitmap.Dispose();
                    graphics.Dispose();
                    bitmap = bitmap1;
                }
            }
            DirectBitmap DB = new DirectBitmap(bitmap.Width, bitmap.Height);
            DB.Bitmap = bitmap;
            DB.UnPink();
            Texture2D Tex;
            using (MemoryStream ms = new MemoryStream())
            {
                DB.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                Tex = Texture2D.FromStream(device, ms);
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
        internal static unsafe PNGData LoadPNGDataWithPinkAwesomeness(
          Bitmap bitmap,
          bool process)
        {
            TextureConverter.lastLoadResultedInResize = false;
            if (TextureConverter._maxDimensions != Vec2.Zero)
            {
                float width1 = TextureConverter._maxDimensions.x;
                float height1 = TextureConverter._maxDimensions.y;
                float num = Math.Min(width1 / bitmap.Width, height1 / bitmap.Height);
                if (width1 < bitmap.Width || height1 < bitmap.Height)
                {
                    TextureConverter.lastLoadResultedInResize = true;
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
            TextureConverter._maxDimensions = pMaxDimensions;
            Texture2D texture2D = MemLoadPNGDataWithPinkAwesomeness(device, bitmap, process);
            //PNGData pngData = TextureConverter.LoadPNGDataWithPinkAwesomeness(bitmap, process);
            TextureConverter._maxDimensions = Vec2.Zero;
            // Texture2D texture2D = new Texture2D(device, pngData.width, pngData.height);
            // texture2D.SetData<int>(pngData.data);
            return texture2D;
        }

        internal static Texture2D LoadPNGWithPinkAwesomeness(
          GraphicsDevice device,
          Stream stream,
          bool process)
        {
            using (Bitmap bitmap = new Bitmap(stream))
                return TextureConverter.LoadPNGWithPinkAwesomeness(device, bitmap, process);
        }

        internal static PNGData LoadPNGDataWithPinkAwesomeness(Stream stream, bool process)
        {
            using (Bitmap bitmap = new Bitmap(stream))
                return TextureConverter.LoadPNGDataWithPinkAwesomeness(bitmap, process);
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
            using (Bitmap bitmap = new Bitmap(fileName))
                return TextureConverter.LoadPNGWithPinkAwesomeness(device, bitmap, process);
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
            using (Bitmap bitmap = new Bitmap(fileName))
                return TextureConverter.LoadPNGWithPinkAwesomenessAndMaxDimensions(device, bitmap, process, maxDimensions);
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
            using (Bitmap bitmap = new Bitmap(fileName))
                return TextureConverter.LoadPNGDataWithPinkAwesomeness(bitmap, process);
        }
    }
}
