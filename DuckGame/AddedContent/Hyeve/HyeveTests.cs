using System.Collections.Generic;
using AddedContent.Hyeve.DebugUI;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = DuckGame.Color;
using RenderTarget2D = DuckGame.RenderTarget2D;

namespace AddedContent.Hyeve
{
    public static class HyeveTests
    {

        private static UiBasic testUI;

        private static RenderTarget2D target = new(Graphics.viewport.Width, Graphics.viewport.Height, false, false, 8, RenderTargetUsage.DiscardContents);

        static HyeveTests()
        {
            RegenUI();
        }

        private static bool _uiDead = false;

        private static void OnUiKilled(IAmUi ui)
        {
            _uiDead = true;
        }

        private static void RegenUI()
        {
            FontDatabase.GenerateFontSize(20);
            UiTabber ui = new(Vector2.One * 80, Vector2.One * 50, Color.Coral, new List<IAmUi>(), "name", 5f);
            for (int i = 0; i < 3; i++)
            {
                UiList subUi = new(Vector2.Zero, Vector2.One * 30, Color.Coral, new List<IAmUi>(), "name", 5f)
                {
                    Draggable = Rando.Int(10) > 3,
                    Closeable = Rando.Int(10) > 3,
                    Resizeable = true
                };
                
                ui.AddContent(subUi);
            }

            testUI = ui;
            testUI.Draggable = true;
            testUI.Closeable = true;
            testUI.Resizeable = true;
            testUI.OnKilled += OnUiKilled;
        }


        [DrawingContext(DrawingLayer.HUD, DoDraw = false)]
        public static void PolyDrawTest()
        {
            if (_uiDead) return;
            
            if(InputData.KeyPressed(Keys.F10)) RegenUI();

            if (target.width != Graphics.viewport.Width || target.height != Graphics.viewport.Height)
            {
                target.Dispose();
                target = new(Graphics.viewport.Width, Graphics.viewport.Height, false, false, 8,
                    RenderTargetUsage.DiscardContents);
            }

            Graphics.SetRenderTarget(target);
            Graphics.Clear(Color.Transparent);


            Graphics.mouseVisible = true;
            Graphics.polyBatcher.BlendState = BlendState.AlphaBlend;
            Graphics.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
            Graphics.polyBatcher.ScissorMode = ScissorStackMode.Intersect;
            Graphics.polyBatcher.SetScreenView();

            
            testUI.UpdateContent();
            testUI.DrawContent();

            if (InputData.MouseLeftPressed()) testUI.OnMouseAction(MouseAction.LeftClick);
            if (InputData.MouseLeftReleased()) testUI.OnMouseAction(MouseAction.LeftRelease);
            if (InputData.MouseScroll != 0f) testUI.OnMouseAction(MouseAction.Scrolled, InputData.MouseScroll * 0.1f);

            DevConsole.Log(InputData.MouseProjectedPosition);

            Graphics.SetRenderTarget(Graphics.defaultRenderTarget);
            
            Graphics.polyBatcher.Texture = target;
            PolyRenderer.TexRect(Vector2.Zero, InputData.ViewportSize);
            Graphics.polyBatcher.Texture = null;
        }


        [DrawingContext(DrawingLayer.HUD, DoDraw = false)]
        public static void TexTest()
        {
            PolyRenderer.Rect(Vector2.Zero, Vector2.One * 50, Color.Aqua);
            FontDatabase.DrawString("G", Vector2.One * 30, Color.White, 35);
        }
    }
}