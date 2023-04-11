// Decompiled with JetBrains decompiler
// Type: DuckGame.UIText
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIDGRDescribe : UIText
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
