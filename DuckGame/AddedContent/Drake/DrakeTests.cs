using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using DuckGame.AddedContent.Drake.DebugUI;
using DuckGame.AddedContent.Drake.PolyRender;
using DuckGame.AddedContent.Drake.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = System.Drawing.Point;

namespace DuckGame.AddedContent.Drake;

public static class DrakeTests
{
    private static UiBasic testUI;

    private static RenderTarget2D target = new (Graphics.viewport.Width, Graphics.viewport.Height, false, false, 8, RenderTargetUsage.DiscardContents);
    static DrakeTests()
    {
        UiTabber ui = new UiTabber(Vector2.One * 80, Vector2.One * 50, Color.Coral, new List<IAmUi>());
        for (int i = 0; i < 3; i++)
        {
            var subUi = new UiList(Vector2.Zero, Vector2.One * 30, Color.Coral, new List<IAmUi>());
            subUi.Draggable = Rando.Int(10) > 3;
            subUi.Closeable = Rando.Int(10) > 3;
            subUi.Resizeable = true;
            ui.AddContent(subUi);
        }

        testUI = ui;
        testUI.Draggable = true;
        testUI.Closeable = true;
        testUI.Resizeable = true;
        testUI.OnKilled += OnUiKilled;
    }

    private static bool uiDead = false;
    
    private static void OnUiKilled(IAmUi ui)
    {
        uiDead = true;
    }
    
    
    [DrawingContext(DrawingLayer.HUD, DoDraw = false)]
    public static void PolyDrawTest()
    {
        if(uiDead) return;

        if (target.width != Graphics.viewport.Width || target.height != Graphics.viewport.Height)
        {
            target.Dispose();
            target = new(Graphics.viewport.Width, Graphics.viewport.Height, false, false, 8, RenderTargetUsage.DiscardContents);
        }
        
        Graphics.SetRenderTarget(target);
        Graphics.Clear(Color.Transparent);
        
        
        Graphics.mouseVisible = true;
        Graphics.polyBatcher.BlendState = BlendState.NonPremultiplied;
        Graphics.polyBatcher.ScissorMode = ScissorStackMode.Intersect;

        //if(Graphics.frame % 240 == 0) testUI.SetCol(UiCols.Main, Color.Random());
        //if (Graphics.frame % 300 == 0) testUI.SetCol(UiCols.Alternate, Color.RandomGray(0, 100));

        ((UiList)testUI).Padding = new Vector2(2f, 2f);
        testUI.UpdateContent();
        testUI.DrawContent();
        
        if(InputChecker.MouseLeftPressed()) testUI.OnMouseAction(MouseAction.LeftClick);
        if(InputChecker.MouseLeftReleased()) testUI.OnMouseAction(MouseAction.LeftRelease);
        if(InputChecker.MouseScroll != 0f) testUI.OnMouseAction(MouseAction.Scrolled, InputChecker.MouseScroll * 0.1f);


        Graphics.SetRenderTarget(Graphics.defaultRenderTarget);
        float xscale = Graphics.currentLayer.width / Graphics.viewport.Width;
        float yscale = Graphics.currentLayer.height / Graphics.viewport.Height;
        Graphics.Draw(target, 0, 0, xscale, yscale);
    }

    private static Texture2D tex;
    
    [DrawingContext(DrawingLayer.HUD, DoDraw = true)]
    public static void TexTest()
    {
        if (tex == null) return;

        Graphics.polyBatcher.BlendState = BlendState.NonPremultiplied;
        Graphics.polyBatcher.Texture = tex;
        PolyRenderer.TexRect(Vector2.Zero, Vector2.One * 300);
        
    }
    
    [DevConsoleCommand]
    public static void GDT()
    {
        Font font = new Font("Jetbrains Mono", 127, FontStyle.Regular, GraphicsUnit.Pixel);
        Bitmap img = new Bitmap(1024, 1024, PixelFormat.Format32bppArgb);
        using System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(img);
        {
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawString("TestText", font, new SolidBrush(System.Drawing.Color.White), 0, 0);
        }
        var data = BmpToColors(img);
        tex = new Tex2D(1024, 1024);
        tex.SetData(data);
    }

    private static Color[] BmpToColors(Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        bmp.Save(ms, ImageFormat.Bmp);
        byte[] bmpBytes = ms.GetBuffer();
        var length = bmpBytes.Length / 4;
        Color[] cols = new Color[length];
        //offset by 2 - buffer seems to have 2 empty bytes at the start?
        for (int i = 0; i < length; i++) cols[i] = new Color(bmpBytes[i * 4 + 4], bmpBytes[i * 4 + 3], bmpBytes[i * 4 + 2], bmpBytes[i * 4 + 5]);
        return cols;
    }
}