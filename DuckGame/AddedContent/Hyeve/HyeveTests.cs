using AddedContent.Firebreak;
using System.Collections.Generic;
using System.Reflection;
using AddedContent.Hyeve.DebugUI;
using AddedContent.Hyeve.DebugUI.Basic;
using AddedContent.Hyeve.DebugUI.Groups;
using AddedContent.Hyeve.DebugUI.ReflectionValues;
using AddedContent.Hyeve.DebugUI.Values;
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

        public static bool TestBool;
        
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
            UiTabber ui = new(Vector2.One * 300, Vector2.One * 300, new List<IAmUi>(), "name")
            {
                Padding = Vector2.Zero
            };
            for (int i = 0; i < 3; i++)
            {
                UiList subUi = new(Vector2.Zero, Vector2.One * 300,new List<IAmUi>(), $"Tab {i}!")
                {
                    Draggable = false,
                    Closeable = false,
                    Resizeable = false,
                    Padding = Vector2.UnitY * 5f,
                };
                
                UiReflectionToggleButton toggle = new(Vector2.Zero, new Vector2(300, 30), typeof(HyeveTests).GetField("TestBool"));
                subUi.AddContent(toggle);
                toggle = new(Vector2.Zero, new Vector2(300, 30), typeof(Duck).GetField("jumping"), Profiles.active[0].duck);
                subUi.AddContent(toggle);
                toggle = new(Vector2.Zero, new Vector2(300, 30), typeof(Duck).GetField("doThrow"), Profiles.active[0].duck);
                subUi.AddContent(toggle);
                UiNumberBar sub = new(Vector2.Zero, new Vector2(300, 30), $"Number {i}")
                {
                    Value = 324.89084342143
                };
                subUi.AddContent(sub);
                sub = new UiReflectionNumberBar(Vector2.Zero, new Vector2(300, 30),typeof(Duck).GetField("_runMax", BindingFlags.NonPublic | BindingFlags.Instance),  Profiles.active[0].duck);
                subUi.AddContent(sub);
                sub = new UiReflectionNumberBar(Vector2.Zero, new Vector2(300, 30),typeof(Duck).GetField("quack"),  Profiles.active[0].duck);
                subUi.AddContent(sub);
                UiVector2Bar bar = new UiReflectionVector2Bar(Vector2.Zero, new Vector2(300, 30), typeof(Duck).GetField("position"),  Profiles.active[0].duck);
                subUi.AddContent(bar);

                ui.AddContent(subUi);
            }

            testUI = ui;
            testUI.Draggable = true;
            testUI.Closeable = true;
            testUI.Resizeable = true;
            testUI.OnDestroyed += OnUiKilled;

            _uiDead = false;
        }

        [Marker.DrawingContext(DrawingLayer.HUD, DoDraw = false)]
        public static void PolyDrawTest()
        {
            if(InputData.KeyPressed(Keys.F10) && MonoMain.UpdateLerpState) 
                RegenUI();
            
            if (_uiDead) return;
            
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
            if (InputData.MouseRightPressed()) testUI.OnMouseAction(MouseAction.RightClick);
            if (InputData.MouseRightReleased()) testUI.OnMouseAction(MouseAction.RightRelease);
            if (InputData.MouseScroll != 0f) testUI.OnMouseAction(MouseAction.Scrolled, InputData.MouseScroll * 0.1f);

            Graphics.SetRenderTarget(Graphics.defaultRenderTarget);
            
            Graphics.polyBatcher.Texture = target;
            PolyRenderer.TexRect(Vector2.Zero, InputData.ViewportSize);
            Graphics.polyBatcher.Texture = null;
        }


        [Marker.DrawingContext(DrawingLayer.HUD, DoDraw = false)]
        public static void TexTest()
        {
            PolyRenderer.Rect(Vector2.Zero, Vector2.One * 50, Color.Aqua);
            FontDatabase.DrawString("G", Vector2.One * 30, Color.White, 35);
        }
    }
}