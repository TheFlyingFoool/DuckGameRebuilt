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
    
    
    [DrawingContext(DrawingLayer.HUD, DoDraw = true)]
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
        Graphics.polyBatcher.SetScreenView();

        //if(Graphics.frame % 240 == 0) testUI.SetCol(UiCols.Main, Color.Random());
        //if (Graphics.frame % 300 == 0) testUI.SetCol(UiCols.Alternate, Color.RandomGray(0, 100));

        ((UiList)testUI).Padding = new Vector2(2f, 2f);
        testUI.UpdateContent();
        testUI.DrawContent();
        
        if(InputData.MouseLeftPressed()) testUI.OnMouseAction(MouseAction.LeftClick);
        if(InputData.MouseLeftReleased()) testUI.OnMouseAction(MouseAction.LeftRelease);
        if(InputData.MouseScroll != 0f) testUI.OnMouseAction(MouseAction.Scrolled, InputData.MouseScroll * 0.1f);

        DevConsole.Log(InputData.MouseProjectedPosition);

        Graphics.SetRenderTarget(Graphics.defaultRenderTarget);
        float xscale = Graphics.currentLayer.width / Graphics.viewport.Width;
        float yscale = Graphics.currentLayer.height / Graphics.viewport.Height;
        Graphics.Draw(target, 0, 0, xscale, yscale);
    }
    
    
    [DrawingContext(DrawingLayer.HUD, DoDraw = true)]
    public static void TexTest()
    {
        Graphics.polyBatcher.BlendState = BlendState.NonPremultiplied;
        Graphics.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
        Graphics.polyBatcher.SetScreenView();
        PolyRenderer.Rect(Vector2.One * 50, Vector2.One * 250, Color.PaleTurquoise);
        StaticFont.DrawString("XYZ:", Vector2.One * 150, Color.Blue);
    }
}