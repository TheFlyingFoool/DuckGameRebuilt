// Decompiled with JetBrains decompiler
// Type: DuckGame.LockerRoom
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class LockerRoom : Level
    {
        private Sprite _background;
        private Sprite _boardHighlight;
        private Sprite _trophiesHighlight;
        private SinWave _pulse = (SinWave)0.1f;
        private LockerSelection _selection;
        private LockerScreen _screen = LockerScreen.Stats;
        private LockerScreen _desiredScreen = LockerScreen.Stats;
        private float _statScroll;
        private List<LockerStat> _stats = new List<LockerStat>();
        private float _fade = 1f;
        private Profile _profile;
        private UIComponent _confirmGroup;
        private UIMenu _confirmMenu;
        private MenuBoolean _clearStats = new MenuBoolean();

        public LockerRoom(Profile p)
        {
            _centeredView = true;
            _profile = p;
        }

        public override void Initialize()
        {
            //_background = new Sprite("gym");
            _boardHighlight = new Sprite("boardHighlight");
            _boardHighlight.CenterOrigin();
            _trophiesHighlight = new Sprite("trophiesHighlight");
            _trophiesHighlight.CenterOrigin();
            HUD.AddCornerMessage(HUDCorner.TopLeft, _profile.name);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
            HUD.AddCornerControl(HUDCorner.BottomRight, "@MENU2@RESET");
            backgroundColor = Color.Black;
            _confirmGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _confirmMenu = new UIMenu("RESET STATS?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenu(_confirmGroup)), true);
            _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_confirmGroup, _clearStats)), true);
            _confirmMenu.Close();
            _confirmGroup.Add(_confirmMenu, false);
            _confirmGroup.Close();
            Add(_confirmGroup);
            Profile profile = _profile;
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("QUACKS: " + Change.ToString(profile.stats.quacks), Color.Orange));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("FANS: " + Change.ToString(profile.stats.GetFans()), Color.Lime));
            int fans = profile.stats.GetFans();
            int num1 = 0;
            if (fans > 0)
                num1 = (int)Math.Round(profile.stats.loyalFans / profile.stats.GetFans() * 100f);
            _stats.Add(new LockerStat("FAN LOYALTY: " + Change.ToString(num1) + "%", Color.Lime));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("KILLS: " + Change.ToString(profile.stats.kills), Color.GreenYellow));
            _stats.Add(new LockerStat("DEATHS: " + Change.ToString(profile.stats.timesKilled), Color.Red));
            _stats.Add(new LockerStat("SUICIDES: " + Change.ToString(profile.stats.suicides), Color.Red));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("ROUNDS WON: " + Change.ToString(profile.stats.matchesWon), Color.GreenYellow));
            _stats.Add(new LockerStat("ROUNDS LOST: " + Change.ToString(profile.stats.timesSpawned - profile.stats.matchesWon), Color.Red));
            _stats.Add(new LockerStat("GAMES WON: " + Change.ToString(profile.stats.trophiesWon), Color.GreenYellow));
            _stats.Add(new LockerStat("GAMES LOST: " + Change.ToString(profile.stats.gamesPlayed - profile.stats.trophiesWon), Color.Red));
            float num2 = 0f;
            if (profile.stats.bulletsFired > 0)
                num2 = profile.stats.bulletsThatHit / (float)profile.stats.bulletsFired;
            _stats.Add(new LockerStat("ACCURACY: " + Change.ToString((int)Math.Round(num2 * 100f)) + "%", num2 > 0.6f ? Color.Green : Color.Red));
            _stats.Add(new LockerStat("TRICK SHOT KILLS: " + profile.stats.trickShots.ToString(), Color.Green));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("MINES STEPPED ON: " + Change.ToString(profile.stats.minesSteppedOn), Color.Orange));
            _stats.Add(new LockerStat("PRESENTS OPENED: " + Change.ToString(profile.stats.presentsOpened), Color.Orange));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("SPIRITUALITY", Color.White));
            _stats.Add(new LockerStat("FUNERALS: " + Change.ToString(profile.stats.funeralsPerformed), Color.Orange));
            _stats.Add(new LockerStat("CONVERSIONS: " + Change.ToString(profile.stats.conversions), Color.Orange));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("TIME SPENT", Color.White));
            _stats.Add(new LockerStat("IN NET: " + TimeSpan.FromSeconds(profile.stats.timeInNet).ToString("hh\\:mm\\:ss"), Color.Orange));
            _stats.Add(new LockerStat("ON FIRE: " + TimeSpan.FromSeconds(profile.stats.timeOnFire).ToString("hh\\:mm\\:ss"), Color.Orange));
            _stats.Add(new LockerStat("BRAINWASHED: " + TimeSpan.FromSeconds(profile.stats.timesMindControlled).ToString("hh\\:mm\\:ss"), Color.Orange));
            _stats.Add(new LockerStat("MOUTH OPEN: " + TimeSpan.FromSeconds(profile.stats.timeWithMouthOpen).ToString("hh\\:mm\\:ss"), Color.Orange));
            base.Initialize();
        }

        public override void Update()
        {
            int num = (int)_selection;
            if (_desiredScreen != _screen)
            {
                _fade = Lerp.Float(_fade, 0f, 0.06f);
                if (_fade <= 0f)
                {
                    _screen = _desiredScreen;
                    if (_screen == LockerScreen.Stats)
                    {
                        _statScroll = 0f;
                        HUD.AddCornerControl(HUDCorner.TopLeft, "@WASD@MOVE");
                        HUD.AddCornerMessage(HUDCorner.TopRight, _profile.name);
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                    }
                    else if (_screen == LockerScreen.Trophies)
                    {
                        _statScroll = 0f;
                        HUD.AddCornerControl(HUDCorner.TopLeft, "@WASD@MOVE");
                        HUD.AddCornerMessage(HUDCorner.TopRight, "TROPHIES");
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                    }
                    else if (_screen == LockerScreen.Locker)
                    {
                        HUD.AddCornerControl(HUDCorner.TopLeft, "@WASD@MOVE");
                        HUD.AddCornerMessage(HUDCorner.TopRight, "LOCKER ROOM");
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@EXIT");
                    }
                    else if (_screen == LockerScreen.Exit)
                    {
                        Graphics.fade = 0f;
                        current = new DoorRoom(_profile);
                    }
                }
            }
            else
            {
                _fade = Lerp.Float(_fade, 1f, 0.06f);
                if (_screen == LockerScreen.Locker)
                {
                    if (InputProfile.active.Pressed(Triggers.MenuLeft))
                    {
                        --num;
                        if (num < 0)
                            num = 1;
                    }
                    if (InputProfile.active.Pressed(Triggers.MenuRight))
                    {
                        ++num;
                        if (num >= 2)
                            num = 0;
                    }
                    _selection = (LockerSelection)num;
                    if (InputProfile.active.Pressed(Triggers.Select))
                    {
                        if (_selection == LockerSelection.Stats)
                        {
                            _desiredScreen = LockerScreen.Stats;
                            HUD.CloseAllCorners();
                        }
                        if (_selection == LockerSelection.Trophies)
                        {
                            _desiredScreen = LockerScreen.Trophies;
                            HUD.CloseAllCorners();
                        }
                    }
                    if (InputProfile.active.Pressed(Triggers.Cancel))
                    {
                        _desiredScreen = LockerScreen.Exit;
                        HUD.CloseAllCorners();
                    }
                }
                else if (_screen == LockerScreen.Stats)
                {
                    if (InputProfile.active.Down(Triggers.MenuUp))
                    {
                        _statScroll -= 0.02f;
                        if (_statScroll < 0f)
                            _statScroll = 0f;
                    }
                    if (InputProfile.active.Down(Triggers.MenuDown))
                    {
                        _statScroll += 0.02f;
                        if (_statScroll > 1f)
                            _statScroll = 1f;
                    }
                    if (InputProfile.active.Pressed(Triggers.Cancel))
                    {
                        _desiredScreen = LockerScreen.Exit;
                        HUD.CloseAllCorners();
                    }
                    if (_clearStats.value)
                    {
                        _clearStats.value = false;
                        _profile.stats = new ProfileStats();
                        Profiles.Save(_profile);
                        current = new LockerRoom(_profile);
                    }
                    if (InputProfile.active.Pressed(Triggers.Menu2))
                    {
                        MonoMain.pauseMenu = _confirmGroup;
                        _confirmGroup.Open();
                        _confirmMenu.Open();
                    }
                }
                else if (_screen == LockerScreen.Trophies && InputProfile.active.Pressed(Triggers.Cancel))
                {
                    _desiredScreen = LockerScreen.Locker;
                    HUD.CloseAllCorners();
                }
            }
            base.Update();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Background)
            {
                if (_screen == LockerScreen.Locker)
                {
                    //_background.scale = new Vec2(1f, 1f); just removing this for now this part of the code cant be run ill remove the rest later
                    //_background.depth = (Depth)0.4f;
                    //_background.alpha = _fade;
                    //Graphics.Draw(_background, 0f, 0f);
                    string text = _profile.name;
                    Vec2 vec2 = new Vec2(115f, 46f);
                    Graphics.DrawString(text, vec2 + new Vec2((-Graphics.GetStringWidth(text) / 2f), 0f), Color.Gray * _fade, (Depth)0.5f);
                    if (_selection == LockerSelection.Stats)
                    {
                        _boardHighlight.depth = (Depth)0.5f;
                        _boardHighlight.alpha = (0.5f + _pulse.normalized * 0.5f) * _fade;
                        _boardHighlight.xscale = _boardHighlight.yscale = (1f + _pulse.normalized * 0.1f);
                        Graphics.Draw(_boardHighlight, 75 + _boardHighlight.w / 2, 60 + _boardHighlight.h / 2);
                        text = "STATISTICS";
                    }
                    else if (_selection == LockerSelection.Trophies)
                    {
                        _trophiesHighlight.depth = (Depth)0.5f;
                        _trophiesHighlight.alpha = (0.5f + _pulse.normalized * 0.5f) * _fade;
                        _trophiesHighlight.xscale = _trophiesHighlight.yscale = (1f + _pulse.normalized * 0.1f);
                        Graphics.Draw(_trophiesHighlight, 161 + _trophiesHighlight.w / 2, 53 + _trophiesHighlight.h / 2);
                        text = "TROPHIES";
                    }
                    vec2 = new Vec2(160f, 140f);
                    Graphics.DrawString(text, vec2 + new Vec2((-Graphics.GetStringWidth(text) / 2f), 0f), new Color(14, 20, 27) * _fade, (Depth)0.5f);
                }
                else if (_screen == LockerScreen.Stats)
                {
                    int num = 0;
                    foreach (LockerStat stat in _stats)
                    {
                        Vec2 vec2 = new Vec2(160f, 18 + num * 10 - _statScroll * (_stats.Count * 10 - 150));
                        string name = stat.name;
                        Graphics.DrawString(name, vec2 + new Vec2((-Graphics.GetStringWidth(name) / 2f), 0f), stat.color * _fade, (Depth)0.5f);
                        ++num;
                    }
                }
                else if (_screen == LockerScreen.Trophies)
                {
                    Vec2 vec2 = new Vec2(160f, 84f);
                    string text = "NOPE";
                    Graphics.DrawString(text, vec2 + new Vec2((-Graphics.GetStringWidth(text) / 2f), 0f), Color.White * _fade, (Depth)0.5f);
                }
            }
            base.PostDrawLayer(layer);
        }
    }
}
