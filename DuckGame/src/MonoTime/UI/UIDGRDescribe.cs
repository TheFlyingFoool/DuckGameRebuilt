using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIDGRDescribe : LUIText
    {
        public UIDGRDescribe(Color c) : base("", c)
        {
        }

        public UIDGRDescribe(Func<string> textFunc, Color c) : base(textFunc, c) 
        {
        }

        public override void Draw()
        {
            int sel = ((UIBox)_parent).selection;

            List<UIComponent> uis = _parent.components.Where(t => t.dgrDescription != "").ToList();

            if (uis.Count > sel)
                text = uis[sel].dgrDescription;
            else
                text = "";

            base.Draw();
        }
    }
}