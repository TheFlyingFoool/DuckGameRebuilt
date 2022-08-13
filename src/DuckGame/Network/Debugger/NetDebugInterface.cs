// Decompiled with JetBrains decompiler
// Type: DuckGame.NetDebugInterface
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NetDebugInterface
    {
        private bool _visible;
        private NetworkInstance _instance;
        private List<NetDebugElement> _elements = new List<NetDebugElement>();
        private bool _tookInput;

        public bool visible => _visible;

        public NetDebugInterface(NetworkInstance pInstance)
        {
            NetDebugInterface netDebugInterface = this;
            _instance = pInstance;
            NetDebugDropdown netDebugDropdown = new NetDebugDropdown(this, "Connection: ", () =>
           {
               List<NetDebugDropdown.Element> elementList = new List<NetDebugDropdown.Element>();
               elementList.Add(new NetDebugDropdown.Element()
               {
                   name = "ALL",
                   value = null
               });
               foreach (NetworkConnection allConnection in netDebugInterface._instance.network.core.allConnections)
               {
                   string str = allConnection.name;
                   if (allConnection.profile != null)
                   {
                       string[] strArray = new string[8];
                       strArray[0] = "|";
                       Color colorUsable = allConnection.profile.persona.colorUsable;
                       strArray[1] = colorUsable.r.ToString();
                       strArray[2] = ",";
                       colorUsable = allConnection.profile.persona.colorUsable;
                       strArray[3] = colorUsable.g.ToString();
                       strArray[4] = ",";
                       colorUsable = allConnection.profile.persona.colorUsable;
                       strArray[5] = colorUsable.b.ToString();
                       strArray[6] = "|";
                       strArray[7] = str;
                       str = string.Concat(strArray) + " |WHITE|(" + allConnection.profile.networkIndex.ToString() + ")";
                   }
                   elementList.Add(new NetDebugDropdown.Element()
                   {
                       name = str,
                       value = allConnection
                   });
               }
               return elementList;
           })
            {
                leading = 4f
            };
            NetDebugDropdown connectionDropdown = netDebugDropdown;
            _elements.Add(connectionDropdown);
            connectionDropdown.right = new NetDebugButton(this, "Lag Out", null, () =>
         {
             if (!(connectionDropdown.selected.value is NetworkConnection networkConnection2))
                 return;
             networkConnection2.debuggerContext.lagSpike = 10;
         });
            connectionDropdown.right.right = new NetDebugButton(this, "Close", null, () =>
         {
             for (int index = 0; index < 5; ++index)
                 Send.ImmediateUnreliableBroadcast(new NMClientClosedGame());
             netDebugInterface._instance = NetworkDebugger.Reboot(netDebugInterface._instance);
         });
            connectionDropdown.right.right.right = new NetDebugButton(this, "Reboot", null, () => netDebugInterface._instance = NetworkDebugger.Reboot(netDebugInterface._instance));
            List<NetDebugElement> elements1 = _elements;
            NetDebugSlider netDebugSlider1 = new NetDebugSlider(this, "Latency: ", () =>
           {
               NetDebugDropdown.Element selected = connectionDropdown.selected;
               return (selected.value != null ? selected.value as NetworkConnection : pInstance.duckNetworkCore.localConnection).debuggerContext.latency;
           }, f =>
     {
         NetDebugDropdown.Element selected = connectionDropdown.selected;
         if (selected.value == null)
             pInstance.duckNetworkCore.localConnection.debuggerContext.latency = f;
         else
             (selected.value as NetworkConnection).debuggerContext.latency = f;
     }, f =>
{
    string str1 = (f * 1000f).ToString() + "ms";
    NetDebugDropdown.Element selected = connectionDropdown.selected;
    string str2;
    if (selected.value == null)
    {
        string str3 = str1;
        int num = (int)(pInstance.network.core.averagePing * 1000.0);
        string str4 = num.ToString();
        string str5 = str3 + " |GREEN|(" + str4 + "ms actual)";
        num = (int)(pInstance.network.core.averagePingPeak * 1000.0);
        string str6 = num.ToString();
        str2 = str5 + " |YELLOW|(" + str6 + "ms peak)";
    }
    else
    {
        NetworkConnection networkConnection3 = selected.value as NetworkConnection;
        str2 = str1 + " |GREEN|(" + ((int)(networkConnection3.manager.ping * 1000.0)).ToString() + "ms actual)" + " |YELLOW|(" + ((int)(networkConnection3.manager.pingPeak * 1000.0)).ToString() + "ms peak)";
    }
    return str2;
})
            {
                indent = 16f
            };
            elements1.Add(netDebugSlider1);
            List<NetDebugElement> elements2 = _elements;
            NetDebugSlider netDebugSlider2 = new NetDebugSlider(this, "Jitter: ", () =>
           {
               NetDebugDropdown.Element selected = connectionDropdown.selected;
               return (selected.value != null ? selected.value as NetworkConnection : pInstance.duckNetworkCore.localConnection).debuggerContext.jitter;
           }, f =>
     {
         NetDebugDropdown.Element selected = connectionDropdown.selected;
         if (selected.value == null)
             pInstance.duckNetworkCore.localConnection.debuggerContext.jitter = f;
         else
             (selected.value as NetworkConnection).debuggerContext.jitter = f;
     }, f => (f * 1000f).ToString() + "ms")
            {
                indent = 16f
            };
            elements2.Add(netDebugSlider2);
            List<NetDebugElement> elements3 = _elements;
            NetDebugSlider netDebugSlider3 = new NetDebugSlider(this, "Loss: ", () =>
           {
               NetDebugDropdown.Element selected = connectionDropdown.selected;
               return (selected.value != null ? selected.value as NetworkConnection : pInstance.duckNetworkCore.localConnection).debuggerContext.loss;
           }, f =>
     {
         NetDebugDropdown.Element selected = connectionDropdown.selected;
         if (selected.value == null)
             pInstance.duckNetworkCore.localConnection.debuggerContext.loss = f;
         else
             (selected.value as NetworkConnection).debuggerContext.loss = f;
     }, f =>
{
    string str7 = (f * 100f).ToString() + "%";
    NetDebugDropdown.Element selected = connectionDropdown.selected;
    string str8;
    if (selected.value == null)
    {
        str8 = str7 + " |RED|(" + pInstance.network.core.averagePacketLoss.ToString() + " lost)" + " |RED|(" + pInstance.network.core.averagePacketLossPercent.ToString() + "% avg)";
    }
    else
    {
        NetworkConnection networkConnection4 = selected.value as NetworkConnection;
        string str9 = str7 + " |RED|(" + networkConnection4.manager.losses.ToString() + " lost)";
        int num = 0;
        if (networkConnection4.manager.losses != 0)
            num = (int)(networkConnection4.manager.sent / networkConnection4.manager.losses * 100.0);
        str8 = str9 + " |RED|(" + num.ToString() + "% avg)";
    }
    return str8;
})
            {
                indent = 16f
            };
            elements3.Add(netDebugSlider3);
            List<NetDebugElement> elements4 = _elements;
            NetDebugSlider netDebugSlider4 = new NetDebugSlider(this, "Duplicate: ", () =>
           {
               NetDebugDropdown.Element selected = connectionDropdown.selected;
               return (selected.value != null ? selected.value as NetworkConnection : pInstance.duckNetworkCore.localConnection).debuggerContext.duplicate;
           }, f =>
     {
         NetDebugDropdown.Element selected = connectionDropdown.selected;
         if (selected.value == null)
             pInstance.duckNetworkCore.localConnection.debuggerContext.duplicate = f;
         else
             (selected.value as NetworkConnection).debuggerContext.duplicate = f;
     }, f => (f * 100f).ToString() + "%")
            {
                indent = 16f
            };
            elements4.Add(netDebugSlider4);
        }

        public void Update()
        {
        }

        public void Draw()
        {
            Rectangle consoleSize = _instance.consoleSize;
            if (consoleSize.Contains(Mouse.positionConsole) && Mouse.right == InputState.Pressed)
                _visible = !_visible;
            if (!_visible)
                return;
            consoleSize.x += 8f;
            consoleSize.width -= 18f;
            consoleSize.y += 8f;
            consoleSize.height = 120f;
            float num = 0.8f;
            Vec2 position = consoleSize.tl + new Vec2(8f, 8f);
            foreach (NetDebugElement element in _elements)
            {
                element.depth = (Depth)num;
                _tookInput |= element.DoDraw(position, !_tookInput);
                position.y += 10f + element.leading;
                num -= 0.01f;
            }
            if (Mouse.left == InputState.Released)
                _tookInput = false;
            Graphics.DrawRect(consoleSize, Color.Black * 0.5f, (Depth)0f);
        }
    }
}
