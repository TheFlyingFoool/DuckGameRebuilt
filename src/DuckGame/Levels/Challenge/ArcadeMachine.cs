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
        public EditorProperty<float> respect = new EditorProperty<float>(0.0f, increment: 0.05f);
        public EditorProperty<string> challenge01 = new EditorProperty<string>("", increment: 0.0f, isLevel: true);
        public EditorProperty<string> challenge02 = new EditorProperty<string>("", increment: 0.0f, isLevel: true);
        public EditorProperty<string> challenge03 = new EditorProperty<string>("", increment: 0.0f, isLevel: true);
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

        public ChallengeGroup data => this._data;

        public override bool visible
        {
            get => base.visible;
            set
            {
                base.visible = value;
                this._dust.visible = base.visible;
            }
        }

        public bool unlocked
        {
            get => this._unlocked;
            set => this._unlocked = value;
        }

        public int lightColor
        {
            get => this._lightColor;
            set => this._lightColor = value;
        }

        public ArcadeMachine(float xpos, float ypos, ChallengeGroup c, int index)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("arcade/arcadeMachines", 29, 36)
            {
                frame = index
            };
            this.graphic = _sprite;
            this.depth = -0.5f;
            this._canHaveChance = false;
            this._customMachineOverlay = new Sprite("arcade/customMachine");
            this._outline = new Sprite("arcade/arcadeMachineOutline")
            {
                depth = this.depth + 1
            };
            this._outline.CenterOrigin();
            this._customMachineOverlayMask = new Sprite("arcade/customOverlay");
            this._boom = new Sprite("arcade/boommachine");
            this._wagnus = new Sprite("arcade/wagnustrainer");
            this._wagnusOverlay = new Sprite("arcade/wagnusOverlay");
            this._font = new BitmapFont("biosFont", 8);
            this.center = new Vec2(this._sprite.width / 2, this._sprite.h / 2);
            this._data = c;
            this._light = new SpriteMap("arcade/lights2", 56, 57);
            this._fixture = new Sprite("arcade/fixture");
            this._flash = new SpriteMap("arcade/monitorFlash", 11, 9);
            this._flash.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            this._flash.SetAnimation("idle");
            this._flashLarge = new SpriteMap("arcade/monitorFlashLarge", 13, 10);
            this._flashLarge.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            this._flashLarge.SetAnimation("idle");
            this._flashWagnus = new SpriteMap("arcade/monitorFlashWagnus", 15, 11);
            this._flashWagnus.AddAnimation("idle", 0.1f, true, 0, 1, 2);
            this._flashWagnus.SetAnimation("idle");
            this._covered = new Sprite("arcade/coveredMachine");
            this._collisionSize = new Vec2(28f, 34f);
            this._collisionOffset = new Vec2(-14f, -17f);
            this.hugWalls = WallHug.Floor;
            this.respect._tooltip = "How much Chancy needs to like you before this machine unlocks.";
            this.requirement._tooltip = "How many challenges must be completed before this machine unlocks.";
            this.name._tooltip = "What's this collection of challenges called?";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this._data = new ChallengeGroup
            {
                name = this.name.value,
                trophiesRequired = this.requirement.value
            };
            this._data.challenges.Add(this.challenge01.value);
            this._data.challenges.Add(this.challenge02.value);
            this._data.challenges.Add(this.challenge03.value);
            if (this.level == null || this.level.bareInitialize)
                return;
            this._dust = new DustSparkleEffect(this.x - 28f, this.y - 40f, false, (bool)this.lit);
            this._lighting = !(bool)this.lit ? new ArcadeScreen(this.x, this.y) : new ArcadeLight(this.x - 1f, this.y - 41f);
            if (Content.readyToRenderPreview)
                this._dust.y -= 10f;
            else
                Level.Add(this._lighting);
            Level.Add(_dust);
            this._dust.depth = this.depth - 2;
        }

        public bool CheckUnlocked(bool ignoreAlreadyUnlocked = true)
        {
            if (this._data == null || ignoreAlreadyUnlocked && this._unlocked)
                return false;
            if (this._data.required.Count > 0)
            {
                foreach (string name in this._data.required)
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
            if ((double)(float)this.respect != 0.0 && (double)Challenges.GetChallengeSkillIndex() < (double)(float)this.respect)
                return false;
            return (int)this.requirement <= 0 || Challenges.GetNumTrophies(Profiles.active[0]) >= (int)this.requirement;
        }

        public void UpdateStyle()
        {
            if (!(this._previousMachineStyle != this.machineStyle) && this._previousStyleOffsetX == this._styleOffsetX && this._previousStyleOffsetY == this._styleOffsetY)
                return;
            this._previousStyleOffsetX = this._styleOffsetX;
            this._previousStyleOffsetY = this._styleOffsetY;
            if (this.machineStyle == null || this.machineStyle == "")
            {
                this._machineStyleSprite = null;
                this._customMachineUnderlay = null;
            }
            else
            {
                this._machineStyleSprite = new Sprite((Tex2D)Editor.StringToTexture(this.machineStyle));
                if (Thing._alphaTestEffect == null)
                    Thing._alphaTestEffect = (Effect)Content.Load<MTEffect>("Shaders/alphatest");
                RenderTarget2D t = new RenderTarget2D(48, 48, true);
                Camera camera = new Camera(0.0f, 0.0f, 48f, 48f);
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
                DuckGame.Graphics.Draw(this._machineStyleSprite, _styleOffsetX, _styleOffsetY, -0.9f);
                DuckGame.Graphics.Draw(this._customMachineOverlayMask, 0.0f, 0.0f, (Depth)0.9f);
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
                this._customMachineUnderlay = new Sprite((Tex2D)tex);
            }
            this._previousMachineStyle = this.machineStyle;
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
            this.machineStyle = node.GetProperty<string>("machineStyle");
            if (this.machineStyle == null)
                this.machineStyle = "";
            if (node.GetProperty<bool>("arcadeMachineMode"))
            {
                this.challenge01WorkshopID = node.GetProperty<ulong>("challenge01WorkshopID");
                this.challenge02WorkshopID = node.GetProperty<ulong>("challenge02WorkshopID");
                this.challenge03WorkshopID = node.GetProperty<ulong>("challenge03WorkshopID");
                this.UpdateData();
            }
            return true;
        }

        public void UpdateData()
        {
            this.challenge01Data = Content.GetLevel(this.challenge01.value);
            this.challenge02Data = Content.GetLevel(this.challenge02.value);
            this.challenge03Data = Content.GetLevel(this.challenge03.value);
        }

        public override void EditorUpdate()
        {
            this.UpdateStyle();
            base.EditorUpdate();
        }

        public override void Update()
        {
            this.UpdateStyle();
            if (this._unlocked)
            {
                Duck duck = Level.Nearest<Duck>(this.x, this.y);
                if (duck != null)
                {
                    if (duck.grounded && (double)(duck.position - this.position).length < 20.0)
                    {
                        this._hoverFade = Lerp.Float(this._hoverFade, 1f, 0.1f);
                        this.hover = true;
                    }
                    else
                    {
                        this._hoverFade = Lerp.Float(this._hoverFade, 0.0f, 0.1f);
                        this.hover = false;
                    }
                }
            }
            this._dust.fade = 0.7f;
            this._dust.visible = this._lighting.visible = this._unlocked && this.visible;
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
                this.y -= 3f;
                for (int index = 0; index < 3; ++index)
                {
                    LevelData levelData = index != 0 ? (index != 1 ? this.challenge03Data : this.challenge02Data) : this.challenge01Data;
                    if (levelData != null && levelData.previewData.preview != null)
                    {
                        Tex2D texture = (Tex2D)Editor.StringToTexture(levelData.previewData.preview);
                        Vec2 vec2 = new Vec2(this.x - 28f, (float)((double)this.y + 30.0 - texture.width / 8.0 - 6.0));
                        switch (index)
                        {
                            case 1:
                                vec2 = new Vec2((float)((double)this.x + 28.0 - texture.width / 8.0), (float)((double)this.y + 30.0 - texture.width / 8.0 - 6.0));
                                break;
                            case 2:
                                vec2 = new Vec2(this.x - (float)(texture.width / 8.0 / 2.0), (float)((double)this.y + 30.0 - texture.width / 8.0));
                                break;
                        }
                        DuckGame.Graphics.DrawRect(new Vec2(vec2.x - 0.5f, vec2.y - 0.5f), new Vec2((float)(vec2.x + texture.width / 8.0 + 0.5), (float)(vec2.y + texture.height / 8.0 + 0.5)), Color.White, (Depth)(index == 2 ? 0.9f : 0.8f));
                        DuckGame.Graphics.Draw(texture, vec2.x, vec2.y, 0.125f, 0.125f, (Depth)(index == 2 ? 0.99f : 0.85f));
                    }
                }
                this.y -= 6f;
            }
            this._sprite.frame = this.style.value;
            this._light.depth = this.depth - 6;
            this._flash.depth = this.depth + 1;
            if (this._unlocked)
            {
                this._light.frame = this._lightColor;
                this.graphic.color = Color.White;
                if (this.style.value == 16)
                {
                    this._flashWagnus.depth = this.depth + 4;
                    if (this.flipHorizontal)
                        DuckGame.Graphics.Draw(_flashWagnus, this.x - 3f, this.y - 8f);
                    else
                        DuckGame.Graphics.Draw(_flashWagnus, this.x - 8f, this.y - 9f);
                }
                else if (this.style.value == 15)
                {
                    if (this.flipHorizontal)
                        DuckGame.Graphics.Draw(_flashLarge, this.x - 3f, this.y - 8f);
                    else
                        DuckGame.Graphics.Draw(_flashLarge, this.x - 7f, this.y - 8f);
                }
                else if (this.flipHorizontal)
                    DuckGame.Graphics.Draw(_flash, this.x - 3f + _screenOffsetX, this.y - 7f + _screenOffsetY);
                else
                    DuckGame.Graphics.Draw(_flash, this.x - 7f + _screenOffsetX, this.y - 7f + _screenOffsetY);
            }
            else
            {
                this._light.frame = 0;
                this.graphic.color = Color.Black;
            }
            if ((bool)this.lit)
            {
                DuckGame.Graphics.Draw(_light, this.x - 28f, this.y - 40f);
                this._fixture.depth = this.depth - 1;
                DuckGame.Graphics.Draw(this._fixture, this.x - 10f, this.y - 65f);
            }
            this._sprite.flipH = false;
            if (this.style.value == 15)
            {
                this._boom.flipH = false;
                this._boom.depth = this.depth;
                DuckGame.Graphics.Draw(this._boom, this.x - 17f, this.y - 36f);
            }
            else if (this.style.value == 16)
            {
                this._wagnus.flipH = false;
                this._wagnus.depth = this.depth;
                DuckGame.Graphics.Draw(this._wagnus, this.x - 17f, this.y - 20f);
                this._wagnusOverlay.flipH = false;
                this._wagnusOverlay.depth = this.depth + 10;
                DuckGame.Graphics.Draw(this._wagnusOverlay, this.x - 17f, this.y - 6f);
            }
            else if (this._machineStyleSprite != null)
            {
                if (this._underlayStyle)
                {
                    this._customMachineUnderlay.center = new Vec2(23f, 30f);
                    this._customMachineUnderlay.depth = this.depth;
                    DuckGame.Graphics.Draw(this._customMachineUnderlay, this.x, this.y);
                }
                else
                {
                    this._machineStyleSprite.center = new Vec2(23f, 30f);
                    this._machineStyleSprite.depth = this.depth;
                    DuckGame.Graphics.Draw(this._machineStyleSprite, this.x, this.y);
                }
            }
            else
                base.Draw();
            if (!this._unlocked)
            {
                this._covered.depth = this.depth + 2;
                if (this.flipHorizontal)
                {
                    this._covered.flipH = true;
                    DuckGame.Graphics.Draw(this._covered, this.x + 19f, this.y - 19f);
                }
                else
                    DuckGame.Graphics.Draw(this._covered, this.x - 18f, this.y - 19f);
            }
            if (_hoverFade <= 0.0)
                return;
            this._outline.alpha = this._hoverFade;
            this._outline.flipH = this.flipHorizontal;
            if (this.flipHorizontal)
                DuckGame.Graphics.Draw(this._outline, this.x, this.y);
            else
                DuckGame.Graphics.Draw(this._outline, this.x + 1f, this.y);
            string name = this._data.name;
            this._font.alpha = this._hoverFade;
        }
    }
}
