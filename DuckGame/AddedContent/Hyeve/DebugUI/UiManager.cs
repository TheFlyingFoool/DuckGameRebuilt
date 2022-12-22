using System;
using System.Collections.Generic;
using System.Reflection;
using AddedContent.Hyeve.DebugUI.Basic;
using AddedContent.Hyeve.DebugUI.Groups;
using AddedContent.Hyeve.DebugUI.ReflectionValues;
using AddedContent.Hyeve.PolyRender;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = DuckGame.Color;
using RenderTarget2D = DuckGame.RenderTarget2D;

namespace AddedContent.Hyeve.DebugUI
{
    public static class UiManager
    {
        private static RenderTarget2D _uiTarget = new(Graphics.viewport.Width, Graphics.viewport.Height, false, false, 8, RenderTargetUsage.DiscardContents);

        private static UiGroup _uiGroup = new UiGroup(Vector2.Zero, InputData.ViewportSize, new List<IAmUi>(), "DebugUIGroup");

        private static UiTabber _objectViewerPanel = new UiTabber(Vector2.One * 100, Vector2.One * 300, new List<IAmUi>(), "ObjectViewerUI");
        
        public static void AddPanel(IAmUi content) => _uiGroup.AddContent(content);

        public static void RemovePanel(IAmUi content) => _uiGroup.RemoveContent(content);


        public static void OpenObjectViewerPanel(object obj)
        {
            UiList tab = CreateObjectViewerList(obj, obj.GetType().Name);
            _objectViewerPanel.AddContent(tab);
            if (!_uiGroup.HasContent(_objectViewerPanel)) _uiGroup.AddContent(_objectViewerPanel);
        }

        public static UiTabber CreateTabber(List<IAmUi> content, string name = "UiTabber")
        {
            return new UiTabber(Vector2.Zero, Vector2.One * 300, content, name);
        }

        public static UiList CreateObjectViewerList(object obj, string name)
        {
            UiList list = new(Vector2.Zero, Vector2.One * 300,new List<IAmUi>(), name)
            {
                Draggable = false,
                Closeable = false,
                Resizeable = false,
                Padding = Vector2.UnitY * 3f,
            };

            foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if(field.IsInitOnly || field.IsLiteral) continue;
                Type type = field.FieldType;
                if (type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double)) list.AddContent(CreateReflectionNumberBar(field, field.IsStatic ? null : obj));
                else if(type == typeof(bool)) list.AddContent(CreateReflectionToggleButton(field, field.IsStatic ? null : obj));
                else if(type == typeof(Vector2) || type == typeof(Vec2)) list.AddContent(CreateReflectionVec2Bar(field, field.IsStatic ? null : obj));
            }

            return list;
        }

        public static UiReflectionNumberBar CreateReflectionNumberBar(FieldInfo info, object obj) => new(Vector2.Zero, new Vector2(300, 30), info, obj);
        public static UiReflectionToggleButton CreateReflectionToggleButton(FieldInfo info, object obj) => new(Vector2.Zero, new Vector2(300, 30), info, obj);
        public static UiReflectionVector2Bar CreateReflectionVec2Bar(FieldInfo info, object obj) => new(Vector2.Zero, new Vector2(300, 30), info, obj);
        

        [DrawingContext(DoDraw = false, CustomID = "DebugUI")]
        public static void DrawDebugUi()
        {
            if(InputData.KeyPressed(Keys.F10)) OpenObjectViewerPanel(Profiles.active[0].duck);
            
            Graphics.SetRenderTarget(_uiTarget);
            Graphics.Clear(Color.Transparent);
            
            SetupUiRender();
            
            _uiGroup.UpdateContent();
            _uiGroup.DrawContent();
            
            PassUiInput();

            RenderUiToScreen();
        }

        private static void CheckUiTarget()
        {
            if (_uiTarget.width == Graphics.viewport.Width && _uiTarget.height == Graphics.viewport.Height) return;
            _uiTarget.Dispose();
            _uiTarget = new(Graphics.viewport.Width, Graphics.viewport.Height, false, false, 8, RenderTargetUsage.DiscardContents);
        }

        private static void PassUiInput()
        {
            if (InputData.MouseLeftPressed()) _uiGroup.OnMouseAction(MouseAction.LeftClick);
            if (InputData.MouseLeftReleased()) _uiGroup.OnMouseAction(MouseAction.LeftRelease);
            if (InputData.MouseRightPressed()) _uiGroup.OnMouseAction(MouseAction.RightClick);
            if (InputData.MouseScroll != 0f) _uiGroup.OnMouseAction(MouseAction.Scrolled, InputData.MouseScroll * 0.1f);
        }

        private static void SetupUiRender()
        {
            Graphics.mouseVisible = true;
            Graphics.polyBatcher.BlendState = BlendState.AlphaBlend;
            Graphics.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
            Graphics.polyBatcher.ScissorMode = ScissorStackMode.Intersect;
            Graphics.polyBatcher.SetScreenView();
        }

        private static void RenderUiToScreen()
        {
            Graphics.SetRenderTarget(Graphics.defaultRenderTarget);
            Graphics.polyBatcher.Texture = _uiTarget;
            PolyRenderer.TexRect(Vector2.Zero, InputData.ViewportSize);
            Graphics.polyBatcher.Texture = null;
        }
    }
}