// Decompiled with JetBrains decompiler
// Type: DuckGame.DevConsoleCore
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class DevConsoleCore
    {
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
        public FancyBitmapFont fancyFont;
        public float alpha;
        public bool open;
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
