// Decompiled with JetBrains decompiler
// Type: DuckGame.LockerRoom
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._centeredView = true;
            this._profile = p;
        }

        public override void Initialize()
        {
            this._background = new Sprite("gym");
            this._boardHighlight = new Sprite("boardHighlight");
            this._boardHighlight.CenterOrigin();
            this._trophiesHighlight = new Sprite("trophiesHighlight");
            this._trophiesHighlight.CenterOrigin();
            HUD.AddCornerMessage(HUDCorner.TopLeft, this._profile.name);
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
            HUD.AddCornerControl(HUDCorner.BottomRight, "@MENU2@RESET");
            this.backgroundColor = Color.Black;
            this._confirmGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            this._confirmMenu = new UIMenu("RESET STATS?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@SELECT@SELECT");
            this._confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenu(this._confirmGroup)), true);
            this._confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._confirmGroup, this._clearStats)), true);
            this._confirmMenu.Close();
            this._confirmGroup.Add(_confirmMenu, false);
            this._confirmGroup.Close();
            Level.Add(_confirmGroup);
            Profile profile = this._profile;
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("QUACKS: " + Change.ToString(profile.stats.quacks), Color.Orange));
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("FANS: " + Change.ToString(profile.stats.GetFans()), Color.Lime));
            int fans = profile.stats.GetFans();
            int num1 = 0;
            if (fans > 0)
                num1 = (int)Math.Round(profile.stats.loyalFans / profile.stats.GetFans() * 100f);
            this._stats.Add(new LockerStat("FAN LOYALTY: " + Change.ToString(num1) + "%", Color.Lime));
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("KILLS: " + Change.ToString(profile.stats.kills), Color.GreenYellow));
            this._stats.Add(new LockerStat("DEATHS: " + Change.ToString(profile.stats.timesKilled), Color.Red));
            this._stats.Add(new LockerStat("SUICIDES: " + Change.ToString(profile.stats.suicides), Color.Red));
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("ROUNDS WON: " + Change.ToString(profile.stats.matchesWon), Color.GreenYellow));
            this._stats.Add(new LockerStat("ROUNDS LOST: " + Change.ToString(profile.stats.timesSpawned - profile.stats.matchesWon), Color.Red));
            this._stats.Add(new LockerStat("GAMES WON: " + Change.ToString(profile.stats.trophiesWon), Color.GreenYellow));
            this._stats.Add(new LockerStat("GAMES LOST: " + Change.ToString(profile.stats.gamesPlayed - profile.stats.trophiesWon), Color.Red));
            float num2 = 0f;
            if (profile.stats.bulletsFired > 0)
                num2 = profile.stats.bulletsThatHit / (float)profile.stats.bulletsFired;
            this._stats.Add(new LockerStat("ACCURACY: " + Change.ToString((int)Math.Round(num2 * 100f)) + "%", num2 > 0.6f ? Color.Green : Color.Red));
            this._stats.Add(new LockerStat("TRICK SHOT KILLS: " + profile.stats.trickShots.ToString(), Color.Green));
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("MINES STEPPED ON: " + Change.ToString(profile.stats.minesSteppedOn), Color.Orange));
            this._stats.Add(new LockerStat("PRESENTS OPENED: " + Change.ToString(profile.stats.presentsOpened), Color.Orange));
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("SPIRITUALITY", Color.White));
            this._stats.Add(new LockerStat("FUNERALS: " + Change.ToString(profile.stats.funeralsPerformed), Color.Orange));
            this._stats.Add(new LockerStat("CONVERSIONS: " + Change.ToString(profile.stats.conversions), Color.Orange));
            this._stats.Add(new LockerStat("", Color.Red));
            this._stats.Add(new LockerStat("TIME SPENT", Color.White));
            this._stats.Add(new LockerStat("IN NET: " + TimeSpan.FromSeconds((double)profile.stats.timeInNet).ToString("hh\\:mm\\:ss"), Color.Orange));
            this._stats.Add(new LockerStat("ON FIRE: " + TimeSpan.FromSeconds((double)profile.stats.timeOnFire).ToString("hh\\:mm\\:ss"), Color.Orange));
            this._stats.Add(new LockerStat("BRAINWASHED: " + TimeSpan.FromSeconds(profile.stats.timesMindControlled).ToString("hh\\:mm\\:ss"), Color.Orange));
            this._stats.Add(new LockerStat("MOUTH OPEN: " + TimeSpan.FromSeconds((double)profile.stats.timeWithMouthOpen).ToString("hh\\:mm\\:ss"), Color.Orange));
            base.Initialize();
        }

        public override void Update()
        {
            int num = (int)this._selection;
            if (this._desiredScreen != this._screen)
            {
                this._fade = Lerp.Float(this._fade, 0f, 0.06f);
                if (_fade <= 0f)
                {
                    this._screen = this._desiredScreen;
                    if (this._screen == LockerScreen.Stats)
                    {
                        this._statScroll = 0f;
                        HUD.AddCornerControl(HUDCorner.TopLeft, "@WASD@MOVE");
                        HUD.AddCornerMessage(HUDCorner.TopRight, this._profile.name);
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                    }
                    else if (this._screen == LockerScreen.Trophies)
                    {
                        this._statScroll = 0f;
                        HUD.AddCornerControl(HUDCorner.TopLeft, "@WASD@MOVE");
                        HUD.AddCornerMessage(HUDCorner.TopRight, "TROPHIES");
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                    }
                    else if (this._screen == LockerScreen.Locker)
                    {
                        HUD.AddCornerControl(HUDCorner.TopLeft, "@WASD@MOVE");
                        HUD.AddCornerMessage(HUDCorner.TopRight, "LOCKER ROOM");
                        HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@EXIT");
                    }
                    else if (this._screen == LockerScreen.Exit)
                    {
                        Graphics.fade = 0f;
                        Level.current = new DoorRoom(this._profile);
                    }
                }
            }
            else
            {
                this._fade = Lerp.Float(this._fade, 1f, 0.06f);
                if (this._screen == LockerScreen.Locker)
                {
                    if (InputProfile.active.Pressed("MENULEFT"))
                    {
                        --num;
                        if (num < 0)
                            num = 1;
                    }
                    if (InputProfile.active.Pressed("MENURIGHT"))
                    {
                        ++num;
                        if (num >= 2)
                            num = 0;
                    }
                    this._selection = (LockerSelection)num;
                    if (InputProfile.active.Pressed("SELECT"))
                    {
                        if (this._selection == LockerSelection.Stats)
                        {
                            this._desiredScreen = LockerScreen.Stats;
                            HUD.CloseAllCorners();
                        }
                        if (this._selection == LockerSelection.Trophies)
                        {
                            this._desiredScreen = LockerScreen.Trophies;
                            HUD.CloseAllCorners();
                        }
                    }
                    if (InputProfile.active.Pressed("CANCEL"))
                    {
                        this._desiredScreen = LockerScreen.Exit;
                        HUD.CloseAllCorners();
                    }
                }
                else if (this._screen == LockerScreen.Stats)
                {
                    if (InputProfile.active.Down("MENUUP"))
                    {
                        this._statScroll -= 0.02f;
                        if (_statScroll < 0f)
                            this._statScroll = 0f;
                    }
                    if (InputProfile.active.Down("MENUDOWN"))
                    {
                        this._statScroll += 0.02f;
                        if (_statScroll > 1f)
                            this._statScroll = 1f;
                    }
                    if (InputProfile.active.Pressed("CANCEL"))
                    {
                        this._desiredScreen = LockerScreen.Exit;
                        HUD.CloseAllCorners();
                    }
                    if (this._clearStats.value)
                    {
                        this._clearStats.value = false;
                        this._profile.stats = new ProfileStats();
                        Profiles.Save(this._profile);
                        Level.current = new LockerRoom(this._profile);
                    }
                    if (InputProfile.active.Pressed("MENU2"))
                    {
                        MonoMain.pauseMenu = this._confirmGroup;
                        this._confirmGroup.Open();
                        this._confirmMenu.Open();
                    }
                }
                else if (this._screen == LockerScreen.Trophies && InputProfile.active.Pressed("CANCEL"))
                {
                    this._desiredScreen = LockerScreen.Locker;
                    HUD.CloseAllCorners();
                }
            }
            base.Update();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Background)
            {
                if (this._screen == LockerScreen.Locker)
                {
                    this._background.scale = new Vec2(1f, 1f);
                    this._background.depth = (Depth)0.4f;
                    this._background.alpha = this._fade;
                    Graphics.Draw(this._background, 0f, 0f);
                    string text = this._profile.name;
                    Vec2 vec2 = new Vec2(115f, 46f);
                    Graphics.DrawString(text, vec2 + new Vec2((-Graphics.GetStringWidth(text) / 2f), 0f), Color.Gray * this._fade, (Depth)0.5f);
                    if (this._selection == LockerSelection.Stats)
                    {
                        this._boardHighlight.depth = (Depth)0.5f;
                        this._boardHighlight.alpha = (0.5f + this._pulse.normalized * 0.5f) * this._fade;
                        this._boardHighlight.xscale = this._boardHighlight.yscale = (1f + this._pulse.normalized * 0.1f);
                        Graphics.Draw(this._boardHighlight, 75 + this._boardHighlight.w / 2, 60 + this._boardHighlight.h / 2);
                        text = "STATISTICS";
                    }
                    else if (this._selection == LockerSelection.Trophies)
                    {
                        this._trophiesHighlight.depth = (Depth)0.5f;
                        this._trophiesHighlight.alpha = (0.5f + this._pulse.normalized * 0.5f) * this._fade;
                        this._trophiesHighlight.xscale = this._trophiesHighlight.yscale = (1f + this._pulse.normalized * 0.1f);
                        Graphics.Draw(this._trophiesHighlight, 161 + this._trophiesHighlight.w / 2, 53 + this._trophiesHighlight.h / 2);
                        text = "TROPHIES";
                    }
                    vec2 = new Vec2(160f, 140f);
                    Graphics.DrawString(text, vec2 + new Vec2((-Graphics.GetStringWidth(text) / 2f), 0f), new Color(14, 20, 27) * this._fade, (Depth)0.5f);
                }
                else if (this._screen == LockerScreen.Stats)
                {
                    int num = 0;
                    foreach (LockerStat stat in this._stats)
                    {
                        Vec2 vec2 = new Vec2(160f, 18 + num * 10 - this._statScroll * (this._stats.Count * 10 - 150));
                        string name = stat.name;
                        Graphics.DrawString(name, vec2 + new Vec2((-Graphics.GetStringWidth(name) / 2f), 0f), stat.color * this._fade, (Depth)0.5f);
                        ++num;
                    }
                }
                else if (this._screen == LockerScreen.Trophies)
                {
                    Vec2 vec2 = new Vec2(160f, 84f);
                    string text = "NOPE";
                    Graphics.DrawString(text, vec2 + new Vec2((-Graphics.GetStringWidth(text) / 2f), 0f), Color.White * this._fade, (Depth)0.5f);
                }
            }
            base.PostDrawLayer(layer);
        }
    }
}
