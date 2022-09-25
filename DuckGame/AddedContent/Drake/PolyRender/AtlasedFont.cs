using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.PolyRender
{
public class AtlasedFont
{
    public const char Invoker = '$';
    public const string Options = "$RBIUSC";
    
    private Vector2 _charBounds;
    
    private CharData[] _regularData;
    private CharData[] _boldData;
    private CharData[] _italicsData;
    private CharData[] _underlineData;
    private CharData[] _strikethroughData;

    private Tex2D _regularAtlas;
    private Tex2D _boldAtlas;
    private Tex2D _italicsAtlas;
    private Tex2D _underlineAtlas;
    private Tex2D _strikethroughAtlas;

    public AtlasedFont(FontFamily font, int fontSize, AtlasFontStyle styles = AtlasFontStyle.RegBolItal, int charCount = 256)
    {
        RunSetup("nofont", fontSize, styles, charCount, font);
    }
    
    public AtlasedFont(string fontName, int fontSize, AtlasFontStyle styles = AtlasFontStyle.RegBolItal, int charCount = 256)
    {
        RunSetup(fontName, fontSize, styles, charCount);
    }

    public void RunSetup(string fontName, int fontSize, AtlasFontStyle styles, int charCount = 256, FontFamily memFont = null)
    {
        if (styles.HasFlag(AtlasFontStyle.Regular))
            Setup(out _regularData, out _regularAtlas, fontName, fontSize, charCount, FontStyle.Regular, memFont);
        if (styles.HasFlag(AtlasFontStyle.Bold))
            Setup(out _boldData, out _boldAtlas, fontName, fontSize, charCount, FontStyle.Bold, memFont);
        if (styles.HasFlag(AtlasFontStyle.Italics))
            Setup(out _italicsData, out _italicsAtlas, fontName, fontSize, charCount, FontStyle.Italic, memFont);
        if (styles.HasFlag(AtlasFontStyle.Underline))
            Setup(out _underlineData, out _underlineAtlas, fontName, fontSize, charCount, FontStyle.Underline, memFont);
        if (styles.HasFlag(AtlasFontStyle.Strikethrough))
            Setup(out _strikethroughData, out _strikethroughAtlas, fontName, fontSize, charCount, FontStyle.Strikeout, memFont);
    }

    private void DrawStringInternal(string text, Vector2 pos, Color col, float scale, float margin, bool formatting)
    {
        Graphics.polyBatcher.Texture = _regularAtlas;
        var charData = _regularData;
        for (var index = 0; index < text.Length; index++)
        {
            if (index > _regularData.Length) continue;
            var character = text[index];

            if (formatting)
            {
                if(character == Invoker && index < text.Length - 1)
                {
                    
                    int option = Options.IndexOf(text[index + 1]);
                    switch (option - 1)
                    {
                        case -1: //Escape!!
                            index++;
                            break;
                        case 0: //Regular Text
                            if (_regularAtlas != null)
                            {
                                Graphics.polyBatcher.Texture = _regularAtlas;
                                charData = _regularData;
                                index++;
                                continue;
                            }

                            break;
                        case 1: //Bold Text
                            if (_boldAtlas != null)
                            {
                                Graphics.polyBatcher.Texture = _boldAtlas;
                                charData = _boldData;
                                index++;
                                continue;
                            }

                            break;
                        case 2: //Italic Text
                            if (_italicsAtlas != null)
                            {
                                Graphics.polyBatcher.Texture = _italicsAtlas;
                                charData = _italicsData;
                                index++;
                                continue;
                            }

                            break;
                        case 3: //Underlined Text
                            if (_underlineAtlas != null)
                            {
                                Graphics.polyBatcher.Texture = _underlineAtlas;
                                charData = _underlineData;
                                index++;
                                continue;
                            }

                            break;
                        case 4: //Strikethrough Text
                            if (_strikethroughAtlas != null)
                            {
                                Graphics.polyBatcher.Texture = _strikethroughAtlas;
                                charData = _strikethroughData;
                                index++;
                                continue;
                            }

                            break;
                        case 5: //Recolor Text
                            if (index < text.Length - 8)
                            {
                                col = Color.FromHexString(text.Substring(index + 2, 8));
                                index += 9;
                                continue;
                            }
                            break;
                    }
                }
            }
            
            CharData data = charData[character];
            PolyRenderer.TexRect(pos, pos + _charBounds * scale, data.AtlasPos, data.AtlasPos + data.AtlasSize, col);
            pos.X += _charBounds.X * margin * scale;
        }
        Graphics.polyBatcher.Texture = null;
    }

    public void DrawString(string text, Vector2 pos, Color col,  float scale = 1f, float margin = 0.6f) => DrawStringInternal(text, pos, col,  scale, margin, true);
    public void DrawRawString(string text, Vector2 pos, Color col, float scale = 1f, float margin = 0.6f) => DrawStringInternal(text, pos, col, scale, margin, false);

    public void DrawStringSized(string text, Vector2 pos, Color col, float cWidth,  float margin = 0.6f)
    {
        float scaler = cWidth / _charBounds.X;
        DrawString(text, pos, col, scaler, margin);
    }

    private void Setup(out CharData[] charData, out Tex2D atlas, string fontName, int fontSize, int charCount, FontStyle style, FontFamily memFont = null)
    {
        Font font = memFont == null ? new Font(fontName, fontSize, style, GraphicsUnit.Pixel) : new Font(memFont, fontSize, style, GraphicsUnit.Pixel);
        var verticalPad = fontSize / 4;
        var atlasWidth = fontSize * charCount;
        var atlasHeight = fontSize + verticalPad * 2;
        
        _charBounds = new Vector2(fontSize, fontSize + verticalPad * 2);
        
        charData = new CharData[charCount];

        using var image = new DirectBitmap(atlasWidth, atlasHeight);
        {
            using var graphics = System.Drawing.Graphics.FromImage(image.Bitmap);
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                var charAtlasWidth = 1f / charCount;
                var charAtlasSize = new Vector2(charAtlasWidth, 1f);

                var x = 0;
                var y = verticalPad;

                for (int i = 0; i < charCount; i++)
                {
                    char character = (char)i;
                    graphics.DrawString("" + character, font, new SolidBrush(System.Drawing.Color.White), x, y);
                    Vector2 charAtlasPos = new Vector2(charAtlasWidth * i, 0f);
                    CharData data = new CharData(character, charAtlasPos, charAtlasSize);
                    charData[i] = data;
                    x += fontSize;
                }
            }

            atlas = new Tex2D(image.Width, image.Height);
            atlas.SetData(image.GetData());
        }
    }
}

internal class DirectBitmap : IDisposable
{
    public Bitmap Bitmap { get; private set; }
    public Int32[] Bits { get; private set; }
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
        Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
    }

    private static int MakeArgb(byte alpha, byte red, byte green, byte blue)
    {
        return ((int)((ulong)((int)red << 16 | (int)green << 8 | (int)blue | (int)alpha << 24)) & -1);
    }

    private static Color FromArgb(long Value)
    {
        return new Color()
        {
            r = (byte)(Value >> 16 & 255L), g = (byte)(Value >> 8 & 255L), b = (byte)(Value & 255L),
            a = (byte)(Value >> 24 & 255L)
        };
    }

    public void SetPixel(int x, int y, Color colour)
    {
        int index = x + (y * Width);
        int col = MakeArgb(colour.a, colour.r, colour.g, colour.b);
        Bits[index] = col;
    }

    public Color GetPixel(int x, int y)
    {
        int index = x + y * Width;
        int col = Bits[index];
        return FromArgb(col);
    }

    public Color[] GetData() => Bits.Select(bit => new Color((byte)((bit >> 16) & 0xFF), (byte)((bit >> 8) & 0xFF), (byte)(bit & 0xFF), (byte)((bit >> 24) & 0xFF))).ToArray();

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }
}

internal struct CharData
{
    public readonly char Character;

    public readonly Vector2 AtlasPos;
    public readonly Vector2 AtlasSize;
    public CharData(char character, Vector2 atlasPos, Vector2 atlasSize)
    {
        Character = character;
        AtlasPos = atlasPos;
        AtlasSize = atlasSize;
    }
}

[Flags]
public enum AtlasFontStyle
{
    Regular = 1,
    Bold = 2,
    Italics = 4,
    Underline = 8,
    Strikethrough = 16,
    
    RegBolItal = 1 | 2 | 4,
    All = 1 | 2 | 4 | 8 | 16,
}
}
