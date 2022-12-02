using System.Collections.Generic;
using AddedContent.Hyeve.DebugUI.Components;
using AddedContent.Hyeve.Utils;
using DuckGame;
using Microsoft.Xna.Framework;
using Rectangle = DuckGame.Rectangle;

namespace AddedContent.Hyeve.DebugUI.Basic
{
    public class UiGroup : UiBasic
    {
        public readonly List<IAmUi> SubContent = new();
        protected readonly List<IAmUi> ContentToRemove = new();
        protected readonly List<IAmUi> ContentToAdd = new();

        private UiColourHolder _colours;
        
        protected bool ContentChanged = false;

        protected virtual bool DrawSelf { get => false; }

        public bool UpdateSubColours = false;

        public override UiColourHolder Colors
        {
            get => _colours;
            set
            {
                _colours = value;
                if (!UpdateSubColours) return;
                foreach (IAmUi ui in SubContent) ui.Colors = value;
            }
        }

        public UiGroup(Vector2 position, Vector2 size, List<IAmUi> content, string name = "UiGroup", float scale = 1f) : base(position, size, name, scale)
        {
            foreach (IAmUi ui in content) AddContent(ui);
        }

        public override void DrawContent()
        {
            if (!Visible()) return;
            if (DrawSelf) base.DrawContent();
            DrawSubContent();
        }

        public override void UpdateContent()
        {
            base.UpdateContent();
            if (ContentToAdd.Count > 0)
            {
                SubContent.AddRange(ContentToAdd);
                ContentToAdd.Clear();
                ContentChanged = true;
            }

            if (ContentToRemove.Count > 0)
            {
                ContentToRemove.ForEach(content => SubContent.Remove(content));
                ContentToRemove.Clear();
                ContentChanged = true;
            }
            UpdateSubContent();
        }


        protected virtual void DrawSubContent()
        {
            Graphics.polyBatcher.PushScissor(CalcScissor());
            for (int i = SubContent.Count - 1; i >= 0; i--)
                SubContent[i].DrawContent(); //Draw in reverse order so that click events always hit the top UI
            Graphics.polyBatcher.PopScissor();
        }

        protected virtual void UpdateSubContent()
        {
            foreach (IAmUi ui in SubContent) ui.UpdateContent();
        }

        public virtual void AddContent(IAmUi content)
        {
            ContentToAdd.Add(content);
            content.OnDestroyed += OnSubContentDestroyed;
            content.OnResized += OnSubContentResized;
            content.OnPositioned += OnSubContentPositioned;
        }

        public virtual void RemoveContent(IAmUi content) => ContentToRemove.Add(content);

        public virtual bool HasContent(IAmUi content) => SubContent.Contains(content);
        
        public override void OnMouseAction(MouseAction action, float scroll = 0)
        {
            base.OnMouseAction(action, scroll);
            SendSubContentMouseAction(action, scroll);
        }

        protected virtual void SendSubContentMouseAction(MouseAction action, float scroll = 0)
        {
            if ((action & MouseAction.AnyClick) != 0 || action == MouseAction.Scrolled)
                SubContent.Find(content => content.Hovered())?.OnMouseAction(action, scroll);
            else
                foreach (IAmUi ui in SubContent)
                    ui.OnMouseAction(action, scroll);
        }

        public override void OnKeyPressed(Keys keycode, char value)
        {
            base.OnKeyPressed(keycode, value);
            SendSubContentKeyPressed(keycode, value);
        }

        protected virtual void SendSubContentKeyPressed(Keys keycode, char value)
        {
            foreach (IAmUi ui in SubContent) ui.OnKeyPressed(keycode, value);
        }

        protected virtual Rectangle CalcScissor()
        {
            return new Rectangle(Position, Position + Size);
        }

        protected virtual void OnSubContentResized(IAmUi subContent, Vector2 old) { }

        protected virtual void OnSubContentPositioned(IAmUi subContent, Vector2 old)
        { }

        protected virtual void OnSubContentDestroyed(IAmUi subContent)
        {
            RemoveContent(subContent);
        }
    }
}
