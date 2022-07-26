// Decompiled with JetBrains decompiler
// Type: DuckGame.DoorRoom
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class DoorRoom : Level
    {
        private Sprite _door;
        private Sprite _unlitDoor;
        private int _selectorPosition;
        private int _desiredSelectorPosition;
        private float _slide;
        private float _slideTo;
        private DoorTransition _transition;
        private DoorTransition _desiredTransition;
        private List<Profile> _profiles;
        private Profile _profile;
        private float _fade;

        public DoorRoom(Profile p = null)
        {
            this._centeredView = true;
            if (p != null)
            {
                this._profiles = Profiles.allCustomProfiles;
                for (int index = 0; index < this._profiles.Count; ++index)
                {
                    if (this._profiles[index] == p)
                    {
                        this._selectorPosition = index;
                        break;
                    }
                }
                this._desiredSelectorPosition = this._selectorPosition;
            }
            this._profile = p;
        }

        public override void Initialize()
        {
            if (Music.currentSong != "RaceDay")
                Music.Play("RaceDay");
            this._door = new Sprite("litDoor");
            this._door.CenterOrigin();
            this._unlitDoor = new Sprite("unlitDoor");
            this._unlitDoor.CenterOrigin();
            this._profiles = Profiles.allCustomProfiles;
            this._profile = this._profiles.Count != 0 ? this._profiles[this._selectorPosition] : Profiles.DefaultPlayer1;
            if (this._profiles.Count > 0)
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@STATS");
            HUD.AddCornerMessage(HUDCorner.BottomMiddle, "@MENU2@ALBUM");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
            this.backgroundColor = Color.Black;
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.CheckCode((InputCode)"LEFT|LEFT|RIGHT|RIGHT|SHOOT|SHOOT|UP|UP|DOWN|DOWN|SHOOT|SHOOT"))
                Global.data.onlineMatches.value = (object)100;
            if (Input.CheckCode((InputCode)"UP|UP|UP|SHOOT|DOWN|DOWN|DOWN|SHOOT|LEFT|RIGHT|SHOOT"))
                Global.data.winsAsHair.value = (object)100;
            if (this._desiredTransition != this._transition)
            {
                this._fade = Lerp.Float(this._fade, 0.0f, 0.06f);
                if ((double)this._fade <= 0.0)
                {
                    this._transition = this._desiredTransition;
                    if (this._transition == DoorTransition.Profile)
                    {
                        Graphics.fade = 0.0f;
                        Level.current = (Level)new LockerRoom(this._profile);
                    }
                    else if (this._transition == DoorTransition.Exit)
                    {
                        Graphics.fade = 0.0f;
                        Level.current = (Level)new TitleScreen();
                    }
                    else if (this._transition == DoorTransition.Album)
                    {
                        Graphics.fade = 0.0f;
                        Level.current = (Level)new Album();
                    }
                }
            }
            else
            {
                this._fade = Lerp.Float(this._fade, 1f, 0.06f);
                if (this._selectorPosition == this._desiredSelectorPosition)
                {
                    if (InputProfile.active.Down("MENULEFT"))
                        this.SelectUp();
                    if (InputProfile.active.Down("MENURIGHT"))
                        this.SelectDown();
                    if (InputProfile.active.Pressed("SELECT") && this._profile != null)
                    {
                        this._desiredTransition = DoorTransition.Profile;
                        HUD.CloseAllCorners();
                    }
                }
                if (InputProfile.active.Pressed("CANCEL"))
                {
                    this._desiredTransition = DoorTransition.Exit;
                    HUD.CloseAllCorners();
                }
                if (InputProfile.active.Pressed("MENU2"))
                {
                    this._desiredTransition = DoorTransition.Album;
                    HUD.CloseAllCorners();
                }
                if ((double)this._slideTo != 0.0 && (double)this._slide != (double)this._slideTo)
                    this._slide = Lerp.Float(this._slide, this._slideTo, 0.05f);
                else if ((double)this._slideTo != 0.0 && (double)this._slide == (double)this._slideTo)
                {
                    this._slide = 0.0f;
                    this._slideTo = 0.0f;
                    SFX.Play("textLetter", 0.7f);
                    this._selectorPosition = this._desiredSelectorPosition;
                    if (this._profiles.Count > 0)
                        this._profile = this._profiles[this._selectorPosition];
                }
            }
            base.Update();
        }

        public void SelectDown()
        {
            if (this._desiredSelectorPosition >= this._profiles.Count - 1)
                this._desiredSelectorPosition = 0;
            else
                ++this._desiredSelectorPosition;
            this._slideTo = 1f;
        }

        public void SelectUp()
        {
            if (this._desiredSelectorPosition <= 0)
                this._desiredSelectorPosition = this._profiles.Count - 1;
            else
                --this._desiredSelectorPosition;
            this._slideTo = -1f;
        }

        private int ProfileIndexAdd(int index, int plus)
        {
            if (this._profiles.Count == 0)
                return -1;
            int num = index + plus;
            while (num >= this._profiles.Count)
                num -= this._profiles.Count;
            while (num < 0)
                num += this._profiles.Count;
            return num;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Background)
            {
                Vec2 vec2_1 = new Vec2(0.0f, 0.0f);
                float num1 = -260f;
                float num2 = 140f;
                for (int index1 = 0; index1 < 7; ++index1)
                {
                    int index2 = this.ProfileIndexAdd(this._selectorPosition, index1 - 3);
                    string str = "NO PROFILE";
                    if (index2 != -1)
                        str = this._profiles[index2].name;
                    float num3 = (float)((double)vec2_1.x + (double)num1 + 3.0 * (double)num2);
                    float x = (float)((double)vec2_1.x + (double)num1 + (double)index1 * (double)num2 + -(double)this._slide * (double)num2);
                    double num4 = (double)Maths.Clamp((float)((100.0 - (double)Math.Abs(x - num3)) / 100.0), 0.0f, 1f);
                    float num5 = (float)num4 * Maths.NormalizeSection((float)num4, 0.0f, 0.9f);
                    this._door.color = Color.White * num5 * this._fade;
                    this._door.depth = (Depth)(num5 * 0.8f);
                    if ((double)num5 < 1.0)
                    {
                        this._unlitDoor.alpha = (float)((1.0 - (double)num5) * 0.5) * this._fade;
                        Graphics.Draw(this._unlitDoor, x, 90f);
                    }
                    if ((double)num5 > 0.0)
                        Graphics.Draw(this._door, x, 90f);
                    string text = str;
                    float num6 = (float)(((double)num5 + 1.0) * 0.5);
                    float num7 = 0.0f;
                    float num8 = 0.0f;
                    Vec2 vec2_2 = new Vec2(1f, 1f);
                    if (text.Length > 9)
                    {
                        vec2_2 = new Vec2(0.75f, 0.75f);
                        num7 = 1f;
                        num8 = 1f;
                    }
                    if (text.Length > 12)
                    {
                        vec2_2 = new Vec2(0.5f, 0.5f);
                        num7 = 2f;
                        num8 = 1f;
                    }
                    Graphics.DrawString(text, new Vec2(x - Graphics.GetStringWidth(text, scale: vec2_2.x) / 2f + num8, 35f + num7), new Color((byte)Math.Round(165.0 * (double)num6), (byte)Math.Round(100.0 * (double)num6), (byte)Math.Round(34.0 * (double)num6)) * this._fade, (Depth)0.9f, scale: vec2_2.x);
                }
                this._door.scale = new Vec2(1f, 1f);
                this._door.depth = (Depth)0.4f;
            }
            base.PostDrawLayer(layer);
        }
    }
}
