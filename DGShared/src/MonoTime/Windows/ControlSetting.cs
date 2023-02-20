// Decompiled with JetBrains decompiler
// Type: DuckGame.ControlSetting
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ControlSetting
    {
        public string name;
        public string trigger;
        public Vec2 position;
        public int column;
        public Action<ProfileSelector> action;
        public Func<InputDevice, bool> condition;
        public bool caption;
    }
}
