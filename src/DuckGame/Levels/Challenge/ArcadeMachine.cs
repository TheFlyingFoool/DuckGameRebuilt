// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeMachine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    [EditorGroup("Special|Arcade", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ArcadeMachine : Thing
    {
        public EditorProperty<string> name = new EditorProperty<string>("NAMELESS");
        public EditorProperty<bool> lit = new EditorProperty<bool>(true);
        public EditorProperty<int> style = new EditorProperty<int>(0, max: 16f, increment: 1f);
        public EditorProperty<int> requirement = new EditorProperty<int>(0, max: 100f, increment: 1f);
        public EditorProperty<float> respect = new EditorProperty<float>(0f, increment: 0.05f);
        public EditorProperty<string> challenge01 = new EditorProperty<string>("", increment: 0f, isLevel: true);
        public EditorProperty<string> challenge02 = new EditorProperty<string>("", increment: 0f, isLevel: true);
        public EditorProperty<string> challenge03 = new EditorProperty<string>("", increment: 0f, isLevel: true);
        protected bool _underlayStyle = true;
        protected SpriteMap _sprite;
        private Sprite _customMachineOverlay;
        private Sprite _customMachineOverlayMask;
        private Sprite _customMachineUnderlay;
        private Sprite _outline;
        private BitmapFont _font;
        protected int _styleOffsetX;
        protected int _styleOffsetY;
        protected int _screenOffsetX;
        protected int _screenOffsetY;
        private float _hoverFade;
        private ChallengeGroup _data;
        private SpriteMap _light;
        private Sprite _fixture;
        private DustSparkleEffect _dust;
        private bool _unlocked = true;
        private int _lightColor = 1;
        public bool flip;
        public bool hover;
        private SpriteMap _flash;
        private SpriteMap _flashLarge;
        private SpriteMap _flashWagnus;
        private Sprite _covered;
        private Sprite _boom;
        private Sprite _wagnus;
        private Sprite _wagnusOverlay;
        private Thing _lighting;
        public string machineStyle = "";
        private string _previousMachineStyle = "";
        private Sprite _machineStyleSprite;
        private int _previousStyleOffsetX;
        private int _previousStyleOffsetY;
        public ulong challenge01WorkshopID;
        public ulong challenge02WorkshopID;
        public ulong challenge03WorkshopID;
        public LevelData challenge01Data;
        public LevelData challenge02Data;
        public LevelData challenge03Data;

        public ChallengeGroup data => _data;

        public override bool visible
        {
            get => base.visible;
            set
            {
                base.visible = value;
                _dust.visible = base.visible;
            }
        }

        public bool unlocked
        {
            get => _unlocked;
            set => _unlocked = value;
        }

        public int lightColor
        {
            get => _lightColor;
            set => _lightColor = value;
        }

        public ArcadeMachine(float xpos, float ypos, ChallengeGroup c, int index)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("arcade/arcadeMachines", 29, 36)
            {
                frame = index
            };
            graphic = _sprite;
            depth = -0.5f;
            _canHaveChance = false;
            _customMachineOverlay = new Sprite("arcade/customMachine");
            _outline = new Sprite("arcade/arcadeMachineOutline")
            {
                depth = depth + 1
            };
            _outline.CenterOrigin();
            _customMachineOverlayMask = new Sprite("arcade/customOverlay");
            _boom = new Sprite("arcade/boommachine");
            _wagnus = new Sprite("arcade/wagnustrainer");
            _wagnusOverlay = new Sprite("arcade/wagnusOverlay");
            _font = new BitmapFont("biosFont", 8);
            center = new Vec2(_sprite.width / 2, _sprite.h / 2);
            _data = c;
            _light = new SpriteMap("arcade/lights2", 56, 57);
            _fixture = new Sprite("arcade/fixture");
            _flash = new SpriteMap("arcade/monitorFlash", 11, 9);
            _flash.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            _flash.SetAnimation("idle");
            _flashLarge = new SpriteMap("arcade/monitorFlashLarge", 13, 10);
            _flashLarge.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            _flashLarge.SetAnimation("idle");
            _flashWagnus = new SpriteMap("arcade/monitorFlashWagnus", 15, 11);
            _flashWagnus.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            _flashWagnus.SetAnimation("idle");
            _covered = new Sprite("arcade/coveredMachine");
            _collisionSize = new Vec2(28f, 34f);
            _collisionOffset = new Vec2(-14f, -17f);
            hugWalls = WallHug.Floor;
            respect._tooltip = "How much Chancy needs to like you before this machine unlocks.";
            requirement._tooltip = "How many challenges must be completed before this machine unlocks.";
            name._tooltip = "What's this collection of challenges called?";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _data = new ChallengeGroup
            {
                name = name.value,
                trophiesRequired = requirement.value
            };
            _data.challenges.Add(challenge01.value);
            _data.challenges.Add(challenge02.value);
            _data.challenges.Add(challenge03.value);
            if (level == null || level.bareInitialize)
                return;
            _dust = new DustSparkleEffect(x - 28f, y - 40f, false, (bool)lit);
            _lighting = !(bool)lit ? new ArcadeScreen(this.x, this.y) : new ArcadeLight(this.x - 1f, this.y - 41f);
            if (Content.readyToRenderPreview)
                _dust.y -= 10f;
            else
                Level.Add(_lighting);
            Level.Add(_dust);
            _dust.depth = depth - 2;
        }

        public bool CheckUnlocked(bool ignoreAlreadyUnlocked = true)
        {
            if (_data == null || ignoreAlreadyUnlocked && _unlocked)
                return false;
            if (_data.required.Count > 0)
            {
                foreach (string name in _data.required)
                {
                    ChallengeData challenge = Challenges.GetChallenge(name);
                    if (challenge != null)
                    {
                        ChallengeSaveData saveData = Profiles.active[0].GetSaveData(challenge.levelID, true);
                        if (saveData == null || saveData.trophy == TrophyType.Baseline)
                            return false;
                    }
                }
            }
            if ((float)respect != 0f && Challenges.GetChallengeSkillIndex() < (float)respect)
                return false;
            return (int)requirement <= 0 || Challenges.GetNumTrophies(Profiles.active[0]) >= (int)requirement;
        }

        public void UpdateStyle()
        {
            if (!(_previousMachineStyle != machineStyle) && _previousStyleOffsetX == _styleOffsetX && _previousStyleOffsetY == _styleOffsetY)
                return;
            _previousStyleOffsetX = _styleOffsetX;
            _previousStyleOffsetY = _styleOffsetY;
            if (machineStyle == null || machineStyle == "")
            {
                _machineStyleSprite = null;
                _customMachineUnderlay = null;
            }
            else
            {
                _machineStyleSprite = new Sprite((Tex2D)Editor.StringToTexture(machineStyle));
                if (Thing._alphaTestEffect == null)
                    Thing._alphaTestEffect = (Effect)Content.Load<MTEffect>("Shaders/alphatest");
                RenderTarget2D t = new RenderTarget2D(48, 48, true);
                Camera camera = new Camera(0f, 0f, 48f, 48f);
                DuckGame.Graphics.SetRenderTarget(t);
                DepthStencilState depthStencilState = new DepthStencilState()
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = 1,
                    DepthBufferEnable = false
                };
                DuckGame.Graphics.Clear(new Color(0, 0, 0, 0));
                DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, (MTEffect)Thing._alphaTestEffect, camera.getMatrix());
                DuckGame.Graphics.Draw(_machineStyleSprite, _styleOffsetX, _styleOffsetY, -0.9f);
                DuckGame.Graphics.Draw(_customMachineOverlayMask, 0f, 0f, (Depth)0.9f);
                DuckGame.Graphics.screen.End();
                DuckGame.Graphics.SetRenderTarget(null);
                Texture2D tex = new Texture2D(DuckGame.Graphics.device, t.width, t.height);
                Color[] data = t.GetData();
                for (int index = 0; index < tex.Width * tex.Height; ++index)
                {
                    if (data[index].r == 250 && data[index].g == 0 && data[index].b == byte.MaxValue)
                        data[index] = new Color(0, 0, 0, 0);
                }
                tex.SetData<Color>(data);
                _customMachineUnderlay = new Sprite((Tex2D)tex);
            }
            _previousMachineStyle = machineStyle;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("machineStyle", machineStyle);
            binaryClassChunk.AddProperty("arcadeMachineMode", Editor.arcadeMachineMode);
            if (Editor.arcadeMachineMode)
            {
                binaryClassChunk.AddProperty("challenge01WorkshopID", challenge01WorkshopID);
                binaryClassChunk.AddProperty("challenge02WorkshopID", challenge02WorkshopID);
                binaryClassChunk.AddProperty("challenge03WorkshopID", challenge03WorkshopID);
            }
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            machineStyle = node.GetProperty<string>("machineStyle");
            if (machineStyle == null)
                machineStyle = "";
            if (node.GetProperty<bool>("arcadeMachineMode"))
            {
                challenge01WorkshopID = node.GetProperty<ulong>("challenge01WorkshopID");
                challenge02WorkshopID = node.GetProperty<ulong>("challenge02WorkshopID");
                challenge03WorkshopID = node.GetProperty<ulong>("challenge03WorkshopID");
                UpdateData();
            }
            return true;
        }

        public void UpdateData()
        {
            challenge01Data = Content.GetLevel(challenge01.value);
            challenge02Data = Content.GetLevel(challenge02.value);
            challenge03Data = Content.GetLevel(challenge03.value);
        }

        public override void EditorUpdate()
        {
            UpdateStyle();
            base.EditorUpdate();
        }

        public override void Update()
        {
            UpdateStyle();
            if (_unlocked)
            {
                Duck duck = Level.Nearest<Duck>(x, y);
                if (duck != null)
                {
                    if (duck.grounded && (duck.position - position).length < 20f)
                    {
                        _hoverFade = Lerp.Float(_hoverFade, 1f, 0.1f);
                        hover = true;
                    }
                    else
                    {
                        _hoverFade = Lerp.Float(_hoverFade, 0f, 0.1f);
                        hover = false;
                    }
                }
            }
            _dust.fade = 0.7f;
            _dust.visible = _lighting.visible = _unlocked && visible;
        }

        public override ContextMenu GetContextMenu()
        {
            ContextMenu contextMenu = base.GetContextMenu();
            contextMenu.AddItem(new ContextFile("style", null, new FieldBinding(this, "machineStyle"), ContextFileType.ArcadeStyle, "Custom Arcade Machine Art (48 x 48 PNG located in My Documents/Duck Game/Custom/Arcade)"));
            return contextMenu;
        }

        public override void Draw()
        {
            if (Content.readyToRenderPreview)
            {
                y -= 3f;
                for (int index = 0; index < 3; ++index)
                {
                    LevelData levelData = index != 0 ? (index != 1 ? challenge03Data : challenge02Data) : challenge01Data;
                    if (levelData != null && levelData.previewData.preview != null)
                    {
                        Tex2D texture = (Tex2D)Editor.StringToTexture(levelData.previewData.preview);
                        Vec2 vec2 = new Vec2(x - 28f, (y + 30f - texture.width / 8f - 6f));
                        switch (index)
                        {
                            case 1:
                                vec2 = new Vec2((float)(x + 28f - texture.width / 8f), (y + 30f - texture.width / 8f - 6f));
                                break;
                            case 2:
                                vec2 = new Vec2(x - (float)(texture.width / 8f / 2f), (y + 30f - texture.width / 8f));
                                break;
                        }
                        DuckGame.Graphics.DrawRect(new Vec2(vec2.x - 0.5f, vec2.y - 0.5f), new Vec2((vec2.x + texture.width / 8f + 0.5f), (vec2.y + texture.height / 8f + 0.5f)), Color.White, (Depth)(index == 2 ? 0.9f : 0.8f));
                        DuckGame.Graphics.Draw(texture, vec2.x, vec2.y, 0.125f, 0.125f, (Depth)(index == 2 ? 0.99f : 0.85f));
                    }
                }
                y -= 6f;
            }
            _sprite.frame = style.value;
            _light.depth = depth - 6;
            _flash.depth = depth + 1;
            if (_unlocked)
            {
                _light.frame = _lightColor;
                graphic.color = Color.White;
                if (style.value == 16)
                {
                    _flashWagnus.depth = depth + 4;
                    if (flipHorizontal)
                        DuckGame.Graphics.Draw(_flashWagnus, x - 3f, y - 8f);
                    else
                        DuckGame.Graphics.Draw(_flashWagnus, x - 8f, y - 9f);
                }
                else if (style.value == 15)
                {
                    if (flipHorizontal)
                        DuckGame.Graphics.Draw(_flashLarge, x - 3f, y - 8f);
                    else
                        DuckGame.Graphics.Draw(_flashLarge, x - 7f, y - 8f);
                }
                else if (flipHorizontal)
                    DuckGame.Graphics.Draw(_flash, x - 3f + _screenOffsetX, y - 7f + _screenOffsetY);
                else
                    DuckGame.Graphics.Draw(_flash, x - 7f + _screenOffsetX, y - 7f + _screenOffsetY);
            }
            else
            {
                _light.frame = 0;
                graphic.color = Color.Black;
            }
            if ((bool)lit)
            {
                DuckGame.Graphics.Draw(_light, x - 28f, y - 40f);
                _fixture.depth = depth - 1;
                DuckGame.Graphics.Draw(_fixture, x - 10f, y - 65f);
            }
            _sprite.flipH = false;
            if (style.value == 15)
            {
                _boom.flipH = false;
                _boom.depth = depth;
                DuckGame.Graphics.Draw(_boom, x - 17f, y - 36f);
            }
            else if (style.value == 16)
            {
                _wagnus.flipH = false;
                _wagnus.depth = depth;
                DuckGame.Graphics.Draw(_wagnus, x - 17f, y - 20f);
                _wagnusOverlay.flipH = false;
                _wagnusOverlay.depth = depth + 10;
                DuckGame.Graphics.Draw(_wagnusOverlay, x - 17f, y - 6f);
            }
            else if (_machineStyleSprite != null)
            {
                if (_underlayStyle)
                {
                    _customMachineUnderlay.center = new Vec2(23f, 30f);
                    _customMachineUnderlay.depth = depth;
                    DuckGame.Graphics.Draw(_customMachineUnderlay, x, y);
                }
                else
                {
                    _machineStyleSprite.center = new Vec2(23f, 30f);
                    _machineStyleSprite.depth = depth;
                    DuckGame.Graphics.Draw(_machineStyleSprite, x, y);
                }
            }
            else
                base.Draw();
            if (!_unlocked)
            {
                _covered.depth = depth + 2;
                if (flipHorizontal)
                {
                    _covered.flipH = true;
                    DuckGame.Graphics.Draw(_covered, x + 19f, y - 19f);
                }
                else
                    DuckGame.Graphics.Draw(_covered, x - 18f, y - 19f);
            }
            if (_hoverFade <= 0.0)
                return;
            _outline.alpha = _hoverFade;
            _outline.flipH = flipHorizontal;
            if (flipHorizontal)
                DuckGame.Graphics.Draw(_outline, x, y);
            else
                DuckGame.Graphics.Draw(_outline, x + 1f, y);
            string name = _data.name;
            _font.alpha = _hoverFade;
        }
    }
}
