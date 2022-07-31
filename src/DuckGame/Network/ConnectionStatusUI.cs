// Decompiled with JetBrains decompiler
// Type: DuckGame.ConnectionStatusUI
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ConnectionStatusUI
    {
        private static ConnectionStatusUICore _core = new ConnectionStatusUICore();
        private static BitmapFont _smallBios;
        private static BitmapFont _smallBios_m;
        private static Sprite _bar;
        private static int spectatorNum = 0;

        public static ConnectionStatusUICore core
        {
            get => ConnectionStatusUI._core;
            set => ConnectionStatusUI._core = value;
        }

        public static void Initialize()
        {
            ConnectionStatusUI._smallBios = new BitmapFont("smallBiosFont", 7, 6);
            ConnectionStatusUI._smallBios_m = new BitmapFont("smallBiosFont", 7, 6);
            ConnectionStatusUI._bar = new Sprite("statusBar");
        }

        public static void Show()
        {
            ConnectionStatusUI.spectatorNum = 0;
            ConnectionStatusUI._core.bars.Clear();
            foreach (Profile profile in Profiles.active)
            {
                ConnectionStatusUI._core.bars.Add(new ConnectionStatusBar()
                {
                    profile = profile
                });
                if (profile.slotType == SlotType.Spectator || profile.pendingSpectatorMode == SlotType.Spectator)
                    ++ConnectionStatusUI.spectatorNum;
            }
            ConnectionStatusUI._core.bars = new List<ConnectionStatusBar>(ConnectionStatusUI._core.bars.OrderBy<ConnectionStatusBar, bool>(x => x.profile.slotType == SlotType.Spectator || x.profile.pendingSpectatorMode == SlotType.Spectator));
            ConnectionStatusUI._core.open = true;
        }

        public static void Hide() => ConnectionStatusUI._core.open = false;

        public static void Update()
        {
            if (ConnectionStatusUI._core.tempShow > 0)
            {
                if (!ConnectionStatusUI.core.open)
                    ConnectionStatusUI.Show();
                --ConnectionStatusUI._core.tempShow;
                if (ConnectionStatusUI._core.tempShow <= 0)
                {
                    ConnectionStatusUI._core.tempShow = 0;
                    ConnectionStatusUI.Hide();
                }
            }
            if (ConnectionStatusUI._core.open)
            {
                ConnectionStatusBar connectionStatusBar = null;
                foreach (ConnectionStatusBar bar in ConnectionStatusUI._core.bars)
                {
                    if (connectionStatusBar == null || connectionStatusBar.position > 0.3f)
                        bar.position = Lerp.FloatSmooth(bar.position, 1f, 0.16f, 1.1f);
                    connectionStatusBar = bar;
                }
            }
            else
            {
                ConnectionStatusBar connectionStatusBar = null;
                foreach (ConnectionStatusBar bar in ConnectionStatusUI._core.bars)
                {
                    if (connectionStatusBar == null || connectionStatusBar.position < 0.7f)
                        bar.position = Lerp.FloatSmooth(bar.position, 0f, 0.08f, 1.1f);
                    connectionStatusBar = bar;
                }
            }
        }

        public static void Draw()
        {
            int count = ConnectionStatusUI._core.bars.Count;
            if (ConnectionStatusUI.spectatorNum > 0)
                ++count;
            float num1 = 14f;
            Vec2 vec2_1 = new Vec2(30f, Layer.HUD.height / 2f - count * num1 / 2f);
            bool flag1 = false;
            int num2 = 0;
            foreach (ConnectionStatusBar bar in ConnectionStatusUI._core.bars)
            {
                if (bar.profile.slotType == SlotType.Spectator && !flag1)
                {
                    flag1 = true;
                    ++num2;
                }
                if (bar.profile.connection != null && bar.profile.connection.status != ConnectionStatus.Disconnected)
                {
                    if (bar.position > 0.01f)
                    {
                        Vec2 vec2_2 = new Vec2(vec2_1.x, vec2_1.y + num2 * 14);
                        vec2_2.x -= Layer.HUD.width * (1f - bar.position);
                        ConnectionStatusUI._bar.depth = (Depth)0.84f;
                        Graphics.Draw(ConnectionStatusUI._bar, vec2_2.x, vec2_2.y);
                        ConnectionStatusUI._smallBios.depth = (Depth)0.9f;
                        string str1 = "CUSTOM";
                        int num3 = 0;
                        bool flag2 = false;
                        int num4;
                        if (bar.profile.connection == DuckNetwork.localConnection)
                        {
                            num4 = DuckNetwork.core.levelTransferProgress;
                            num3 = DuckNetwork.core.levelTransferSize;
                            if (DuckNetwork.core.logTransferSize > 0)
                            {
                                num4 = DuckNetwork.core.logTransferProgress * 500;
                                num3 = DuckNetwork.core.logTransferSize * 500;
                                str1 = "LOG";
                            }
                            flag2 = true;
                        }
                        else
                        {
                            num4 = bar.profile.connection.dataTransferProgress;
                            num3 = bar.profile.connection.dataTransferSize;
                            if (bar.profile.connection.logTransferSize > 0)
                            {
                                num4 = bar.profile.connection.logTransferProgress * 500;
                                num3 = bar.profile.connection.logTransferSize * 500;
                                str1 = "LOG";
                            }
                        }
                        if (num4 != num3)
                        {
                            ConnectionStatusUI._smallBios.scale = new Vec2(0.5f, 0.5f);
                            if (str1 == "LOG")
                            {
                                if (flag2)
                                    ConnectionStatusUI._smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|SENDING LOG   " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                                else
                                    ConnectionStatusUI._smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|DOWNLOADING LOG " + str1 + " " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                            }
                            else if (flag2)
                                ConnectionStatusUI._smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|DOWNLOADING   " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                            else
                                ConnectionStatusUI._smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|SENDING " + str1 + " " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                            float num5 = num4 / (float)num3;
                            int num6 = 3;
                            int x = 11;
                            int y = 7;
                            int num7 = 90;
                            Graphics.DrawRect(vec2_2 + new Vec2(x, y), vec2_2 + new Vec2(x + num7, y + num6), Color.White, (Depth)0.9f, false, 0.5f);
                            Graphics.DrawRect(vec2_2 + new Vec2(x, y), vec2_2 + new Vec2(x + num7 * num5, y + num6), Colors.DGGreen, (Depth)0.87f);
                            Graphics.DrawRect(vec2_2 + new Vec2(x, y), vec2_2 + new Vec2(x + num7, y + num6), Colors.DGRed, (Depth)0.84f);
                        }
                        else if (bar.profile.connection.levelIndex != DuckNetwork.levelIndex)
                            ConnectionStatusUI._smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|SENDING...", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                        else
                            ConnectionStatusUI._smallBios.Draw("@ONLINEGOOD@|DGGREEN|READY!", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                        ConnectionStatusUI._smallBios.scale = new Vec2(1f, 1f);
                        string str2 = bar.profile.nameUI;
                        if (str2.Length > 14)
                            str2 = str2.Substring(0, 14) + "..";
                        string[] strArray = new string[7];
                        strArray[0] = "|";
                        Color colorUsable = bar.profile.persona.colorUsable;
                        strArray[1] = colorUsable.r.ToString();
                        strArray[2] = ",";
                        colorUsable = bar.profile.persona.colorUsable;
                        strArray[3] = colorUsable.g.ToString();
                        strArray[4] = ",";
                        colorUsable = bar.profile.persona.colorUsable;
                        strArray[5] = colorUsable.b.ToString();
                        strArray[6] = "|";
                        string str3 = string.Concat(strArray);
                        if (bar.profile.connection != null && bar.profile.connection.isHost)
                            str2 = "@HOSTCROWN@" + str2;
                        if (bar.profile.slotType == SlotType.Spectator || bar.profile.pendingSpectatorMode == SlotType.Spectator)
                        {
                            str2 = "@SPECTATOR@" + str2;
                            str3 = "|DGPURPLE|";
                        }
                        string text1 = str3 + str2;
                        ConnectionStatusUI._smallBios.Draw(text1, new Vec2((float)(vec2_2.x + (double)ConnectionStatusUI._bar.width - 3.0 - (double)ConnectionStatusUI._smallBios.GetWidth(text1) - 60.0), vec2_2.y + 3f), Color.White, (Depth)0.9f);
                        int num8 = (int)Math.Round((double)bar.profile.connection.manager.ping * 1000.0);
                        if (bar.profile.connection == DuckNetwork.localConnection)
                            num8 = 0;
                        string source = num8.ToString() + "|WHITE|MS";
                        source.Count<char>();
                        string text2 = num8 >= 150 ? (num8 >= 250 ? (bar.profile.connection.status != ConnectionStatus.Connected ? "|DGRED|" + source + "@SIGNALDEAD@" : "|DGRED|" + source + "@SIGNALBAD@") : "|DGYELLOW|" + source + "@SIGNALNORMAL@") : "|DGGREEN|" + source + "@SIGNALGOOD@";
                        ConnectionStatusUI._smallBios.Draw(text2, new Vec2((float)(vec2_2.x + (double)ConnectionStatusUI._bar.width - 3.0) - ConnectionStatusUI._smallBios.GetWidth(text2), vec2_2.y + 3f), Color.White, (Depth)0.9f);
                    }
                    ++num2;
                }
            }
        }
    }
}
