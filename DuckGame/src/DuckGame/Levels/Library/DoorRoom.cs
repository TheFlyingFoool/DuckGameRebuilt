// Decompiled with JetBrains decompiler
// Type: DuckGame.DoorRoom
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _centeredView = true;
            if (p != null)
            {
                _profiles = Profiles.allCustomProfiles;
                for (int index = 0; index < _profiles.Count; ++index)
                {
                    if (_profiles[index] == p)
                    {
                        _selectorPosition = index;
                        break;
                    }
                }
                _desiredSelectorPosition = _selectorPosition;
            }
            _profile = p;
        }

        public override void Initialize()
        {
            if (Music.currentSong != "RaceDay")
                Music.Play("RaceDay");
            _door = new Sprite("litDoor");
            _door.CenterOrigin();
            _unlitDoor = new Sprite("unlitDoor");
            _unlitDoor.CenterOrigin();
            _profiles = Profiles.allCustomProfiles;
            _profile = _profiles.Count != 0 ? _profiles[_selectorPosition] : Profiles.DefaultPlayer1;
            if (_profiles.Count > 0)
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@STATS");
            HUD.AddCornerMessage(HUDCorner.BottomMiddle, "@MENU2@ALBUM");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
            backgroundColor = Color.Black;
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.CheckCode((InputCode)"LEFT|LEFT|RIGHT|RIGHT|SHOOT|SHOOT|UP|UP|DOWN|DOWN|SHOOT|SHOOT"))
                Global.data.onlineMatches.value = 100;
            if (Input.CheckCode((InputCode)"UP|UP|UP|SHOOT|DOWN|DOWN|DOWN|SHOOT|LEFT|RIGHT|SHOOT"))
                Global.data.winsAsHair.value = 100;
            if (_desiredTransition != _transition)
            {
                _fade = Lerp.Float(_fade, 0f, 0.06f);
                if (_fade <= 0.0)
                {
                    _transition = _desiredTransition;
                    if (_transition == DoorTransition.Profile)
                    {
                        Graphics.fade = 0f;
                        current = new LockerRoom(_profile);
                    }
                    else if (_transition == DoorTransition.Exit)
                    {
                        Graphics.fade = 0f;
                        current = new TitleScreen();
                    }
                    else if (_transition == DoorTransition.Album)
                    {
                        Graphics.fade = 0f;
                        current = new Album();
                    }
                }
            }
            else
            {
                _fade = Lerp.Float(_fade, 1f, 0.06f);
                if (_selectorPosition == _desiredSelectorPosition)
                {
                    if (InputProfile.active.Down(Triggers.MenuLeft))
                        SelectUp();
                    if (InputProfile.active.Down(Triggers.MenuRight))
                        SelectDown();
                    if (InputProfile.active.Pressed(Triggers.Select) && _profile != null)
                    {
                        _desiredTransition = DoorTransition.Profile;
                        HUD.CloseAllCorners();
                    }
                }
                if (InputProfile.active.Pressed(Triggers.Cancel))
                {
                    _desiredTransition = DoorTransition.Exit;
                    HUD.CloseAllCorners();
                }
                if (InputProfile.active.Pressed(Triggers.Menu2))
                {
                    _desiredTransition = DoorTransition.Album;
                    HUD.CloseAllCorners();
                }
                if (_slideTo != 0.0 && _slide != _slideTo)
                    _slide = Lerp.Float(_slide, _slideTo, 0.05f);
                else if (_slideTo != 0.0 && _slide == _slideTo)
                {
                    _slide = 0f;
                    _slideTo = 0f;
                    SFX.Play("textLetter", 0.7f);
                    _selectorPosition = _desiredSelectorPosition;
                    if (_profiles.Count > 0)
                        _profile = _profiles[_selectorPosition];
                }
            }
            base.Update();
        }

        public void SelectDown()
        {
            if (_desiredSelectorPosition >= _profiles.Count - 1)
                _desiredSelectorPosition = 0;
            else
                ++_desiredSelectorPosition;
            _slideTo = 1f;
        }

        public void SelectUp()
        {
            if (_desiredSelectorPosition <= 0)
                _desiredSelectorPosition = _profiles.Count - 1;
            else
                --_desiredSelectorPosition;
            _slideTo = -1f;
        }

        private int ProfileIndexAdd(int index, int plus)
        {
            if (_profiles.Count == 0)
                return -1;
            int num = index + plus;
            while (num >= _profiles.Count)
                num -= _profiles.Count;
            while (num < 0)
                num += _profiles.Count;
            return num;
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Background)
            {
                Vec2 vec2_1 = new Vec2(0f, 0f);
                float num1 = -260f;
                float num2 = 140f;
                for (int index1 = 0; index1 < 7; ++index1)
                {
                    int index2 = ProfileIndexAdd(_selectorPosition, index1 - 3);
                    string str = "NO PROFILE";
                    if (index2 != -1)
                        str = _profiles[index2].name;
                    float num3 = (float)(vec2_1.x + num1 + 3.0 * num2);
                    float x = (float)(vec2_1.x + num1 + index1 * num2 + -_slide * num2);
                    double num4 = Maths.Clamp((float)((100.0 - Math.Abs(x - num3)) / 100.0), 0f, 1f);
                    float num5 = (float)num4 * Maths.NormalizeSection((float)num4, 0f, 0.9f);
                    _door.color = Color.White * num5 * _fade;
                    _door.depth = (Depth)(num5 * 0.8f);
                    if (num5 < 1.0)
                    {
                        _unlitDoor.alpha = (float)((1.0 - num5) * 0.5) * _fade;
                        Graphics.Draw(_unlitDoor, x, 90f);
                    }
                    if (num5 > 0.0)
                        Graphics.Draw(_door, x, 90f);
                    string text = str;
                    float num6 = (float)((num5 + 1.0) * 0.5);
                    float num7 = 0f;
                    float num8 = 0f;
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
                    Graphics.DrawString(text, new Vec2(x - Graphics.GetStringWidth(text, scale: vec2_2.x) / 2f + num8, 35f + num7), new Color((byte)Math.Round(165.0 * num6), (byte)Math.Round(100.0 * num6), (byte)Math.Round(34.0 * num6)) * _fade, (Depth)0.9f, scale: vec2_2.x);
                }
                _door.scale = new Vec2(1f, 1f);
                _door.depth = (Depth)0.4f;
            }
            base.PostDrawLayer(layer);
        }
    }
}
