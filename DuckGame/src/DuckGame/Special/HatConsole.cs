// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeHatConsole
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ArcadeHatConsole : Thing
    {
        private ProfileBox2 _profileBox;
        private Sprite _consoleHighlight;
        private Sprite _consoleFlash;
        private SpriteMap _selectConsole;
        private Sprite _base;
        private PointLight _light;
        private float _consoleFade;
        private Profile _profile;
        private Duck _duck;
        public bool hover;

        public ArcadeHatConsole(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _consoleHighlight = new Sprite("consoleHighlight");
            _base = new Sprite("hatConsoleBase");
            _consoleFlash = new Sprite("consoleFlash");
            _consoleFlash.CenterOrigin();
            _selectConsole = new SpriteMap("selectConsole", 20, 19);
            _selectConsole.AddAnimation("idle", 1f, true, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            _selectConsole.SetAnimation("idle");
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(0f, 0f);
            depth = -0.5f;
            graphic = _selectConsole;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _light = new PointLight(x + 9f, y + 7f, new Color(160, (int)byte.MaxValue, 160), 70f, new List<LightOccluder>());
            Level.Add(_light);
        }

        public void MakeHatSelector(Duck d)
        {
            if (_profileBox != null || d == null)
                return;
            _profileBox = new ProfileBox2(9999f, 9999f, d.inputProfile, d.profile, null, 0)
            {
                duck = d
            };
            _profileBox._hatSelector.layer = Layer.HUD;
            _profileBox._hatSelector.isArcadeHatSelector = true;
            if (RoomEditorExtra.arcadeHat != "")
            {
                Team t = Teams.all.Find(t => t.name == RoomEditorExtra.arcadeHat);
                if (t != null)
                {
                    Hat equipment = d.GetEquipment(typeof(Hat)) as Hat;
                    Hat hat = new TeamHat(0f, 0f, t, d.profile);
                    Level.Add(hat);
                    d.Equip(hat, false);
                    d.profile.team = t;
                    if (equipment != null) Level.Remove(equipment);
                }
            }
            _profile = d.profile;
            _duck = d;
            Level.Add(_profileBox);
        }

        public bool IsOpen() => _profileBox._hatSelector.open;

        public void Open()
        {
            if (_duck == null)
                return;
            _profileBox._hatSelector.position = new Vec2(85f, 45f);
            _profileBox._hatSelector.Open(_duck.profile);
            _profileBox.OpenCorners();
            SFX.Play("consoleOpen", 0.5f);
        }

        public override void Update()
        {
            bool hover = this.hover;
            Duck duck = Level.Nearest<Duck>((position + new Vec2(8f, 0f)), 16f);
            this.hover = duck != null;// && (duck.position - (position + new Vec2(8f, 0f))).length < 16f;
            if (!hover && this.hover)
                HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@PROFILE");
            else if (hover && !this.hover)
                HUD.CloseAllCorners();
            _consoleFade = Lerp.Float(_consoleFade, this.hover ? 1f : 0f, 0.1f);
            base.Update();
        }

        public override void Draw()
        {
            if (_light != null)
            {
                _consoleFlash.scale = new Vec2(0.75f, 0.75f);
                if (_selectConsole.imageIndex == 0)
                {
                    _light.visible = true;
                    _consoleFlash.alpha = 0.3f;
                }
                else if (_selectConsole.imageIndex == 1)
                {
                    _light.visible = true;
                    _consoleFlash.alpha = 0.1f;
                }
                else if (_selectConsole.imageIndex == 2)
                {
                    _light.visible = false;
                    _consoleFlash.alpha = 0f;
                }
            }
            _consoleFlash.depth = depth + 10;
            Graphics.Draw(_consoleFlash, x + 9f, y + 7f);
            _base.depth = depth - 10;
            Graphics.Draw(_base, x + 3f, y + 13f);
            if (_consoleFade > 0.01f)
            {
                _consoleHighlight.depth = depth + 5;
                _consoleHighlight.alpha = _consoleFade;
                Graphics.Draw(_consoleHighlight, x, y);
            }
            base.Draw();
        }
    }
}
