using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleInterface.Panes
{
    public abstract class MMParentPane : MallardManagerPane
    {
        public override bool Borderless { get; } = true;

        public List<MallardManagerPane> Children = new();
        protected int s_focusPaneIndex;

        public override void Update()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                MallardManagerPane pane = Children[i];
                
                if (pane.SwitchToPane is not null)
                    Children[i] = pane.SwitchToPane;

                if (!pane.Active)
                    RemovePane(pane);
            }
        }

        public virtual MallardManagerPane? FocusedPane
        {
            get // fast
            {
                if (s_focusPaneIndex >= Children.Count || s_focusPaneIndex < 0)
                    return Children.FirstOrDefault();
                
                return Children[s_focusPaneIndex];
            }
            set // slow
            {
                int index = Children.IndexOf(value);
                
                if (index == -1)
                    return;
    
                s_focusPaneIndex = index;
                
                OnFocusedPaneChange();
            }
        }

        protected virtual void OnFocusedPaneChange()
        {
            FocusedPane?.OnFocus();
        }
        
        public void AddPane(MallardManagerPane pane)
        {
            Children.Add(pane);
        }

        /// <returns>Whether or not the removal succeeded</returns>
        public bool RemovePane(MallardManagerPane pane)
        {
            if (Children.Count == 1)
                Active = false;
            
            Children.Remove(pane);
            return true;
        }

        public override void OnFocus()
        {
            FocusedPane?.OnFocus();
        }
    }
}