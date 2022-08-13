// Decompiled with JetBrains decompiler
// Type: DuckGame.HUDCore
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class HUDCore
    {
        public List<CornerDisplay> _cornerDisplays = new List<CornerDisplay>();
        public List<CornerDisplay> _inputChangeDisplays = new List<CornerDisplay>();
        public List<CornerDisplay> _playerChangeDisplays = new List<CornerDisplay>();
        public bool _hide;
    }
}
