using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Keybind
    {
        public Keybind(List<string[]> sequence)
        {
            
        }
        
        public static Keybind FromString(string s)
        {
            throw new NotImplementedException();
        }

        public class Input
        {
            public InputType Type;
            public Keys[] KeyboardInput;
            public string[] DuckInput;
            public string[] MenuInput;
        }

        public enum InputType
        {
            DuckInput,
            MenuInput,
            Keyboard,
        }
    }
}