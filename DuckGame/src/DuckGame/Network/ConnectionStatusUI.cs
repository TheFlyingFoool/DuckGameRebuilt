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
        private static Interp ConLerp = new Interp(true);
        public static ConnectionStatusUICore core
        {
            get => _core;
            set => _core = value;
        }

        public static void Initialize()
        {
            _smallBios = new BitmapFont("smallBiosFont", 7, 6);
            _smallBios_m = new BitmapFont("smallBiosFont", 7, 6);
            _bar = new Sprite("statusBar");
        }

        public static void Show()
        {
            spectatorNum = 0;
            _core.bars.Clear();
            foreach (Profile profile in Profiles.active)
            {
                _core.bars.Add(new ConnectionStatusBar()
                {
                    profile = profile
                });
                if (profile.slotType == SlotType.Spectator || profile.pendingSpectatorMode == SlotType.Spectator)
                    ++spectatorNum;
            }
            _core.bars = new List<ConnectionStatusBar>(_core.bars.OrderBy(x => x.profile.slotType == SlotType.Spectator || x.profile.pendingSpectatorMode == SlotType.Spectator));
            _core.open = true;
        }

        public static void Hide() => _core.open = false;

        public static void Update()
        {
            if (_core.tempShow > 0)
            {
                if (!core.open)
                    Show();
                --_core.tempShow;
                if (_core.tempShow <= 0)
                {
                    _core.tempShow = 0;
                    Hide();
                }
            }
            if (_core.open)
            {
                ConnectionStatusBar connectionStatusBar = null;
                foreach (ConnectionStatusBar bar in _core.bars)
                {
                    if (connectionStatusBar == null || connectionStatusBar.position > 0.3f)
                        bar.position = Lerp.FloatSmooth(bar.position, 1f, 0.16f, 1.1f);
                    connectionStatusBar = bar;
                }
            }
            else
            {
                ConnectionStatusBar connectionStatusBar = null;
                foreach (ConnectionStatusBar bar in _core.bars)
                {
                    if (connectionStatusBar == null || connectionStatusBar.position < 0.7f)
                        bar.position = Lerp.FloatSmooth(bar.position, 0f, 0.08f, 1.1f);
                    connectionStatusBar = bar;
                }
            }
        }

        public static void Draw()
        {
            int count = _core.bars.Count;
            if (spectatorNum > 0)
                ++count;
            float num1 = 14f;
            Vec2 vec2_1 = new Vec2(30f, Layer.HUD.height / 2f - count * num1 / 2f);
            bool flag1 = false;
            int num2 = 0;
            foreach (ConnectionStatusBar bar in _core.bars)
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
                        Vec2 barPos = new Vec2(vec2_1.x, vec2_1.y + num2 * 14);
                        barPos.x -= Layer.HUD.width * (1f - bar.position);
                        Vec2 vec2_2 = barPos;

                        _bar.depth = (Depth)0.84f;
                        Graphics.Draw(_bar, vec2_2.x, vec2_2.y);
                        _smallBios.depth = (Depth)0.9f;
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
                            _smallBios.scale = new Vec2(0.5f, 0.5f);
                            if (str1 == "LOG")
                            {
                                if (flag2)
                                    _smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|SENDING LOG   " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                                else
                                    _smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|DOWNLOADING LOG " + str1 + " " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                            }
                            else if (flag2)
                                _smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|DOWNLOADING   " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                            else
                                _smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|SENDING " + str1 + " " + num4.ToString() + "\\" + num3.ToString() + "B", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
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
                            _smallBios.Draw("@ONLINENEUTRAL@|DGYELLOW|SENDING...", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                        else
                            _smallBios.Draw("@ONLINEGOOD@|DGGREEN|READY!", new Vec2(vec2_2.x + 3f, vec2_2.y + 3f), Color.White, (Depth)0.9f);
                        _smallBios.scale = new Vec2(1f, 1f);
                        
                        string name = bar.profile.nameUI;
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
                        string colorString = string.Concat(strArray);
                        const int lim = 14;
                        int coloredTagsLength = name.Length - Program.RemoveColorTags(name).Length;
                        if (name.Length - coloredTagsLength > lim)
                            name = name.Substring(0, lim + coloredTagsLength) + $"{colorString}..";
                        if (bar.profile.connection != null && bar.profile.connection.isHost)
                            name = "@HOSTCROWN@" + name;
                        if (bar.profile.slotType == SlotType.Spectator || bar.profile.pendingSpectatorMode == SlotType.Spectator)
                        {
                            name = "@SPECTATOR@" + name;
                            colorString = "|DGPURPLE|";
                        }
                        name = colorString + name;
                        _smallBios.Draw(name, new Vec2((float)(vec2_2.x + _bar.width - 3f - _smallBios.GetWidth(name) - 60f), vec2_2.y + 3f), Color.White, (Depth)0.9f);
                        int pingval = (int)Math.Round(bar.profile.connection.manager.ping * 1000f);
                        if (bar.profile.connection == DuckNetwork.localConnection)
                            pingval = 0;
                        string source = pingval.ToString() + "|WHITE|MS";
                        string ping = pingval >= 150 ? (pingval >= 250 ? (bar.profile.connection.status != ConnectionStatus.Connected ? "|DGRED|" + source + "@SIGNALDEAD@" : "|DGRED|" + source + "@SIGNALBAD@") : "|DGYELLOW|" + source + "@SIGNALNORMAL@") : "|DGGREEN|" + source + "@SIGNALGOOD@";
                        _smallBios.Draw(ping, new Vec2((float)(vec2_2.x + _bar.width - 3f) - _smallBios.GetWidth(ping), vec2_2.y + 3f), Color.White, (Depth)0.9f);
                    }
                    ++num2;
                }
            }
        }
    }
}
