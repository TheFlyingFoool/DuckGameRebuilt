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
