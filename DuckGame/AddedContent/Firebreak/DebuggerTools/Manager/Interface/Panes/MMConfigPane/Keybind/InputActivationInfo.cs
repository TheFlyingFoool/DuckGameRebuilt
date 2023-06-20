using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DuckGame
{
    [DebuggerDisplay("{InputName}")]
    public struct InputActivationInfo
    {
        public DateTime ActivationTime;
        public string InputName;
        public bool IsDuckInput;
        /// pressed (true) or released (false)
        public bool Pressed;

        private static Regex s_uppercaseCheckRegex = new(@"^[A-Z]+$", RegexOptions.Compiled);

        public InputActivationInfo(string inputName, bool pressed)
        {
            if (string.IsNullOrEmpty(inputName))
                throw new ArgumentNullException(nameof(inputName));
            
            ActivationTime = DateTime.Now;
            InputName = inputName.ToLower();
            Pressed = pressed;
            
            IsDuckInput = s_uppercaseCheckRegex.Match(inputName).Success;
        }
    }
}