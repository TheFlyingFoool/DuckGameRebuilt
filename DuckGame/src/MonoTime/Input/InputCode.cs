using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class InputCode
    {
        public string name = "";
        public string description = "";
        public string chancyComment = "";
        public Action action;
        private Dictionary<string, InputCodeProfileStatus> status = new Dictionary<string, InputCodeProfileStatus>();
        public List<string> triggers = new List<string>();
        public float breakSpeed = 0.04f;
        private bool hasDoubleInputs;
        private bool _initializedDoubleInputs;
        private static Dictionary<string, InputCode> _codes = new Dictionary<string, InputCode>();

        public InputCodeProfileStatus GetStatus(InputProfile p)
        {
            InputCodeProfileStatus status;
            if (!this.status.TryGetValue(p.name, out status))
            {
                status = new InputCodeProfileStatus();
                this.status[p.name] = status;
            }
            return status;
        }

        public bool Update(InputProfile p)
        {
            if (p == null)
                return false;
            if (!_initializedDoubleInputs)
            {
                _initializedDoubleInputs = true;
                foreach (string trigger in triggers)
                {
                    if (trigger.Contains("|"))
                    {
                        hasDoubleInputs = true;
                        break;
                    }
                }
            }
            InputCodeProfileStatus status = GetStatus(p);
            if (status.lastUpdateFrame == Graphics.frame)
                return status.lastResult;
            status.lastUpdateFrame = Graphics.frame;
            status.breakTimer -= breakSpeed;
            if (status.breakTimer <= 0f)
                status.Break();
            if (status.currentIndex > triggers.Count || status.currentIndex < 0)//dan check into later, caused a crash on weird linux machine ArgumentOutOfRangeException triggers
            {
                return false;
            }
            string trigger1 = triggers[status.currentIndex];
            int num = 0;
            if (hasDoubleInputs && trigger1.Contains("|"))
            {
                string str1 = trigger1;
                char[] chArray = new char[1] { '|' };
                foreach (string str2 in str1.Split(chArray))
                    num |= 1 << Network.synchronizedTriggers.Count - Network.synchronizedTriggers.IndexOf(str2);
            }
            else
                num = 1 << Network.synchronizedTriggers.Count - Network.synchronizedTriggers.IndexOf(trigger1);
            if (p.state == num)
            {
                if (!status.release)
                {
                    if (status.currentIndex == triggers.Count - 1)
                    {
                        status.Break();
                        status.lastResult = true;
                        return true;
                    }
                    status.release = true;
                    if (status.currentIndex == 0)
                        status.breakTimer = 1f;
                }
            }
            else if (p.state == 0)
            {
                if (status.release)
                    status.Progress();
            }
            else
                status.Break();
            status.lastResult = false;
            return false;
        }

        public static implicit operator InputCode(string s)
        {
            InputCode inputCode;
            if (!_codes.TryGetValue(s, out inputCode))
            {
                inputCode = new InputCode
                {
                    triggers = new List<string>(s.Split('|'))
                };
                _codes[s] = inputCode;
            }
            return inputCode;
        }

        public class InputCodeProfileStatus
        {
            public long lastUpdateFrame;
            public bool lastResult;
            public int currentIndex;
            public bool release;
            public float breakTimer = 1f;

            public void Break()
            {
                if (currentIndex <= 0)
                    return;
                currentIndex = 0;
                release = false;
                breakTimer = 1f;
            }

            public void Progress()
            {
                ++currentIndex;
                release = false;
                breakTimer = 1f;
            }
        }
    }
}
