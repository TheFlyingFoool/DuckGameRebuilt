using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class DevConsoleCore
    {
        public bool writeExecutedCommand = true;
        public HashSet<NetworkConnection> requestingLogs = new HashSet<NetworkConnection>();
        public HashSet<NetworkConnection> transferRequestsPending = new HashSet<NetworkConnection>();
        public Dictionary<NetworkConnection, string> receivingLogs = new Dictionary<NetworkConnection, string>();
        public Queue<NetMessage> pendingSends = new Queue<NetMessage>();
        public bool constantSync;
        public int viewOffset;
        public int logScores = -1;
        public Queue<DCLine> lines = new Queue<DCLine>();
        public List<DCLine> pendingLines = new List<DCLine>();
        public List<DCChartValue> pendingChartValues = new List<DCChartValue>();
        public BitmapFont font;
        public BitmapFont fpsfont;
        public FancyBitmapFont fancyFont;
        public float alpha;
        public bool open;

        public string Typing
        {
            get => typing;
            set
            {
                if (value == typing)
                    return;

                string prevTyping = typing;
                typing = value;
                
                OnTextChange?.Invoke(prevTyping, typing);
            }
        }

        public event Action<string, string> OnTextChange;
        public string typing = "";
        public List<string> previousLines = new List<string>();
        public bool splitScreen;
        public bool rhythmMode;
        public bool qwopMode;
        public bool showIslands;
        public bool showCollision;
        public bool shieldMode;
        public int cursorPosition;
        public int lastCommandIndex;
        public string lastLine = "";

        public void ReceiveLogData(string pData, NetworkConnection pConnection)
        {
            if (requestingLogs.Contains(pConnection))
            {
                string dat;
                if (!receivingLogs.TryGetValue(pConnection, out dat))
                {
                    receivingLogs[pConnection] = "";//dat = (this.receivingLogs[pConnection] = "");
                }
                Dictionary<NetworkConnection, string> dictionary = receivingLogs;
                dictionary[pConnection] += pData;
            }
        }
        public string GetReceivedLogData(NetworkConnection pConnection) => receivingLogs.ContainsKey(pConnection) ? receivingLogs[pConnection] : null;
    }
}
