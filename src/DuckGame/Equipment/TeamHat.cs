// Decompiled with JetBrains decompiler
// Type: DuckGame.TeamHat
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class TeamHat : Hat
    {
        public bool hasBeenStolen;
        private float _timeOpen;
        private int _prevFrame;
        private Sprite _specialSprite;
        private SinWaveManualUpdate _wave = (SinWaveManualUpdate)0.1f;
        private float _fade;
        public StateBinding _netTeamIndexBinding = new StateBinding(nameof(netTeamIndex));
        private bool _shouldUpdateSprite;
        private Team _team;
        public DuckPersona quickPersona;
        private Team _lastLoadedTeam;
        private string _prevHatID;
        private Profile _profile;
        private Cape _cape;
        private MaterialKatanaman _katanaMaterial;
        private bool _isKatanaHat;
        private bool _specialInitialized;
        private int _networkCape = -1;
        private float glow;
        private bool _filter;
        //private int _lastParticleFrame;
        private List<TeamHat.CustomParticle> _addedParticles;
        private float _quackWait;
        private float _quackHold;

        public ushort netTeamIndex
        {
            get => this._team == null ? (ushort)0 : (ushort)Teams.IndexOf(this._team);
            set => this.team = Teams.ParseFromIndex(value);
        }

        public Team team
        {
            get => this._team;
            set
            {
                this._team = value;
                this._shouldUpdateSprite = true;
            }
        }

        public override void SetQuack(int pValue)
        {
            this.PositionOnOwner();
            this.frame = pValue;
            if (this._equippedDuck != null && !this.destroyed)
            {
                if (this._prevFrame == 0 && this._sprite.frame == 1)
                    this.OpenHat();
                else if (this._prevFrame == 1 && this._sprite.frame == 0)
                    this.CloseHat();
            }
            this._prevFrame = this._sprite.frame;
        }

        public void UpdateSprite()
        {
            if (this._profile == null && this.equippedDuck != null && this.equippedDuck.profile == Profiles.EnvironmentProfile)
            {
                this._shouldUpdateSprite = true;
            }
            else
            {
                if (this._team != null && (this._team != this._lastLoadedTeam && (this._team.facade == null || this._team.facade != this._lastLoadedTeam) || this._prevHatID != this._team.hatID || this._team.filter != this._filter))
                {
                    this._filter = this._team.filter;
                    if (this._profile == null && this.equippedDuck != null)
                        this._profile = this.equippedDuck.profile;
                    this.sprite = this._team.hat.CloneMap();
                    this.pickupSprite = this._team.hat.Clone();
                    DuckPersona pPersona = this.quickPersona;
                    if (this._profile != null)
                        pPersona = this._profile.persona;
                    if (pPersona != null && this._team.metadata != null && this._team.metadata.UseDuckColor.value)
                    {
                        this.sprite = this._team.GetHat(pPersona).CloneMap();
                        this.pickupSprite = this._team.GetHat(pPersona).Clone();
                    }
                    this.sprite.center = new Vec2(16f, 16f);
                    this.hatOffset = this._team.hatOffset;
                    this.UpdateCape();
                    this._lastLoadedTeam = this._team.facade != null ? this._team.facade : this._team;
                    this._prevHatID = this._team.hatID;
                    this.graphic = sprite;
                }
                if (this._specialInitialized || this._team == null)
                    return;
                this._specialInitialized = true;
                this._isKatanaHat = this._sprite.texture.textureName == "hats/katanaman";
                if (!this._isKatanaHat)
                    return;
                this._katanaMaterial = new MaterialKatanaman(this);
            }
        }

        public TeamHat(float xpos, float ypos, Team t)
          : base(xpos, ypos)
        {
            this.team = t;
            this.depth = - 0.5f;
        }

        public TeamHat(float xpos, float ypos, Team t, Profile p)
          : base(xpos, ypos)
        {
            this._profile = p;
            this.team = t;
            this.depth = - 0.5f;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("teamIndex", Teams.all.IndexOf(this._team));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            int property = node.GetProperty<int>("teamIndex");
            if (property >= 0 && property < Teams.all.Count - 1)
                this.team = Teams.all[property];
            return base.Deserialize(node);
        }

        public override void Initialize()
        {
            this.UpdateCape();
            base.Initialize();
        }

        public void UpdateCape()
        {
            if (this._team == null)
                return;
            if (this._sprite.texture.textureName == "hats/johnnys")
                this.quacks = false;
            if (this._cape != null)
            {
                if (this._cape.level != null)
                    Level.Remove(_cape);
                this._cape = null;
            }
            if (this._sprite.texture.textureName == "hats/suit")
            {
                Tex2D tex = null;
                int idx = Global.data.flag;
                if (Network.isActive)
                    idx = this._networkCape;
                if (idx < 0)
                    return;
                Sprite flag = UIFlagSelection.GetFlag(idx);
                if (flag != null)
                    tex = flag.texture;
                if (tex == null)
                    return;
                this._cape = new Cape(this.x, this.y, this);
                if (!tex.textureName.Contains("full_"))
                    this._cape.halfFlag = true;
                this._cape.SetCapeTexture((Texture2D)tex);
            }
            else
            {
                if (this._team.capeTexture == null)
                    return;
                this._cape = new Cape(this.x, this.y, this);
                this._cape.SetCapeTexture(this._team.capeTexture);
                if (this._team.metadata == null)
                    return;
                this._cape.metadata = this._team.metadata;
            }
        }

        public override void Terminate()
        {
            if (this._cape != null)
                Level.Remove(_cape);
            base.Terminate();
        }

        public override void Update()
        {
            if (this._cape != null && this._cape.level == null)
                Level.Add(_cape);
            if (Network.isActive)
            {
                if (this._team != null && this._team.filter != this._filter)
                {
                    this.UpdateCape();
                    this._shouldUpdateSprite = true;
                }
                if (this._networkCape < 0 && this.duck != null && this.duck.profile != null)
                {
                    this._networkCape = !this.duck.profile.localPlayer ? this.duck.profile.flagIndex : Global.data.flag;
                    this.UpdateCape();
                }
                if (Network.InLobby() && this._team != null && (this.sprite == null || this.sprite != null && this.sprite.globalIndex != this._team.hat.globalIndex))
                    this._shouldUpdateSprite = true;
            }
            else if (Level.current is TeamSelect2 && this._equippedDuck != null && this.team != null && this.team.customHatPath != null && Keyboard.Pressed(Keys.F5) && !Network.isActive)
            {
                int index = Teams.core.extraTeams.IndexOf(this.team);
                Team.deserializeInto = this.team;
                Teams.core.extraTeams[index] = Team.Deserialize(this.team.customHatPath);
                Team.deserializeInto = null;
                Duck equippedDuck = this._equippedDuck;
                this._equippedDuck.Unequip(this);
                Level.Remove(this);
                TeamHat teamHat = new TeamHat(this.x, this.y, Teams.core.extraTeams[index]);
                Level.Add(teamHat);
                TeamHat e = teamHat;
                equippedDuck.Equip(e, false);
            }
            if (this._shouldUpdateSprite)
            {
                this._shouldUpdateSprite = false;
                this.UpdateSprite();
            }
            if (this._equippedDuck != null && !this.destroyed)
            {
                if (this._sprite.frame == 1)
                    this._timeOpen += 0.1f;
                else
                    this._timeOpen = 0.0f;
            }
            if (this._sprite.frame == 1 && this._prevFrame == 0)
                this.glow = 1.2f;
            this._prevFrame = this._sprite.frame;
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove(this);
            if (_quackWait > 0.0)
                this._quackWait -= Maths.IncFrameTimer();
            else if (_quackHold > 0.0)
                this._quackHold -= Maths.IncFrameTimer();
            base.Update();
        }

        public override void Quack(float volume, float pitch)
        {
            if (this.duck != null && this._sprite.texture.textureName == "hats/hearts")
            {
                SFX.Play("heartfart", volume, Math.Min(pitch + 0.4f - Rando.Float(0.1f), 1f));
                HeartPuff heartPuff = new HeartPuff(this.x, this.y)
                {
                    anchor = (Anchor)this
                };
                Level.Add(heartPuff);
                for (int index = 0; index < 2; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x, this.y);
                    smallSmoke.sprite.color = Color.Green * (0.4f + Rando.Float(0.3f));
                    Level.Add(smallSmoke);
                }
            }
            else
                SFX.Play("quack", volume, pitch);
        }

        public override void OpenHat()
        {
            if (this.duck == null || (double)this.duck.z != 0.0)
                return;
            if (this.team != null && this.team.metadata != null)
            {
                if (this.team.metadata.QuackSuppressRequack.value && (_quackWait > 0.0 || _quackHold > 0.0))
                    return;
                this._quackWait = this.team.metadata.QuackDelay.value;
                this._quackHold = this.team.metadata.QuackHold.value;
                if (this.team.customParticles.Count <= 0)
                    return;
                if (this._addedParticles == null)
                    this._addedParticles = new List<TeamHat.CustomParticle>();
                int num1 = this.team.metadata.ParticleCount.value;
                Vec2 vec2_1 = new Vec2((float)(-(double)this.team.metadata.ParticleEmitShapeSize.value.x / 2.0), (float)(-(double)this.team.metadata.ParticleEmitShapeSize.value.y / 2.0));
                Vec2 vec2_2 = new Vec2(this.team.metadata.ParticleEmitShapeSize.value.x / 2f, this.team.metadata.ParticleEmitShapeSize.value.y / 2f);
                Vec2 vec2_3 = this.team.metadata.ParticleEmitterOffset.value;
                for (int index1 = 0; index1 < num1; ++index1)
                {
                    Vec2 pPosition = vec2_3;
                    if (team.metadata.ParticleEmitShape.value.x == 1.0)
                    {
                        float rad = Maths.DegToRad(team.metadata.ParticleEmitShape.value.y == 2.0 ? index1 * (360f / num1) : Rando.Float(360f));
                        Vec2 vec2_4 = new Vec2((float)Math.Cos((double)rad) * (this.team.metadata.ParticleEmitShapeSize.value.x / 2f), (float)-Math.Sin((double)rad) * (this.team.metadata.ParticleEmitShapeSize.value.y / 2f));
                        if (team.metadata.ParticleEmitShape.value.y == 1.0)
                            pPosition += vec2_4 * Rando.Float(1f);
                        else
                            pPosition += vec2_4;
                    }
                    else if (team.metadata.ParticleEmitShape.value.x == 2.0)
                    {
                        if (team.metadata.ParticleEmitShape.value.y == 0.0)
                        {
                            float num2 = (double)Rando.Float(1f) >= 0.5 ? 1f : -1f;
                            if ((double)Rando.Float(1f) >= 0.5)
                                pPosition += new Vec2(this.team.metadata.ParticleEmitShapeSize.value.x * num2, Rando.Float((float)(-(double)this.team.metadata.ParticleEmitShapeSize.value.y / 2.0), this.team.metadata.ParticleEmitShapeSize.value.y / 2f));
                            else
                                pPosition += new Vec2(Rando.Float((float)(-(double)this.team.metadata.ParticleEmitShapeSize.value.x / 2.0), this.team.metadata.ParticleEmitShapeSize.value.x / 2f), this.team.metadata.ParticleEmitShapeSize.value.y * num2);
                        }
                        else if (team.metadata.ParticleEmitShape.value.y == 1.0)
                            pPosition += new Vec2(Rando.Float((float)(-(double)this.team.metadata.ParticleEmitShapeSize.value.x / 2.0), this.team.metadata.ParticleEmitShapeSize.value.x / 2f), Rando.Float((float)(-(double)this.team.metadata.ParticleEmitShapeSize.value.y / 2.0), this.team.metadata.ParticleEmitShapeSize.value.y / 2f));
                        else if (team.metadata.ParticleEmitShape.value.y == 2.0)
                        {
                            float rad = Maths.DegToRad(team.metadata.ParticleEmitShape.value.y == 2.0 ? index1 * (360f / num1) : Rando.Float(360f));
                            Vec2 vec2_5 = new Vec2((float)Math.Cos((double)rad) * 100f, (float)-Math.Sin((double)rad) * 100f);
                            Vec2 zero = Vec2.Zero;
                            for (int index2 = 0; index2 < 4; ++index2)
                            {
                                Vec2 vec2_6 = Vec2.Zero;
                                if (index2 == 0)
                                {
                                    if (Collision.LineIntersect(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_1.x, vec2_2.y)))
                                        vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_1.x, vec2_2.y));
                                }
                                else if (index2 == 1)
                                {
                                    if (Collision.LineIntersect(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_2.x, vec2_1.y)))
                                        vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_2.x, vec2_1.y));
                                }
                                else if (index2 == 2)
                                {
                                    if (Collision.LineIntersect(Vec2.Zero, vec2_5, new Vec2(vec2_1.x, vec2_2.y), vec2_2))
                                        vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, new Vec2(vec2_1.x, vec2_2.y), vec2_2);
                                }
                                else if (index2 == 3 && Collision.LineIntersect(Vec2.Zero, vec2_5, new Vec2(vec2_2.x, vec2_1.y), vec2_2))
                                    vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, new Vec2(vec2_2.x, vec2_1.y), vec2_2);
                                if (vec2_6 != Vec2.Zero)
                                {
                                    vec2_5 = vec2_6;
                                    break;
                                }
                            }
                            pPosition += vec2_5;
                        }
                    }
                    TeamHat.CustomParticle customParticle = new TeamHat.CustomParticle(pPosition, this, this.team.metadata);
                    if (this.team.metadata.ParticleAnimated.value)
                    {
                        customParticle.animationFrames = this.team.customParticles;
                        customParticle.animationSpeed = this.team.metadata.ParticleAnimationSpeed.value * 0.5f;
                        customParticle.animationLoop = this.team.metadata.ParticleAnimationLoop.value;
                    }
                    Level.Add(customParticle);
                    this._addedParticles.Add(customParticle);
                }
            }
            else if (this._sprite.texture.textureName == "hats/burgers")
            {
                FluidData ketchup = Fluid.Ketchup;
                ketchup.amount = Rando.Float(0.0005f, 1f / 1000f);
                int num = Rando.Int(4) + 1;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(this.x + duck.offDir * (2f + Rando.Float(0.0f, 7f)), this.y + 3f + Rando.Float(0.0f, 3f), new Vec2(duck.offDir * Rando.Float(0.5f, 3f), Rando.Float(0.0f, -2f)), ketchup, thickMult: 2.5f)
                    {
                        depth = this.depth + 1
                    };
                    Level.Add(fluid);
                }
            }
            else if (this._sprite.texture.textureName == "hats/divers" || this._sprite.texture.textureName == "hats/fridge")
            {
                FluidData water = Fluid.Water;
                water.amount = Rando.Float(0.0001f, 0.0005f);
                int num = Rando.Int(3) + 1;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(this.x + duck.offDir * (2f + Rando.Float(0.0f, 4f)), this.y + 3f + Rando.Float(0.0f, 3f), new Vec2(duck.offDir * Rando.Float(0.5f, 3f), Rando.Float(0.0f, -2f)), water, thickMult: 5f)
                    {
                        depth = this.depth + 1
                    };
                    Level.Add(fluid);
                }
            }
            else if (this._sprite.texture.textureName == "hats/gross")
            {
                FluidData water = Fluid.Water;
                water.amount = Rando.Float(0.0002f, 0.0007f);
                int num = Rando.Int(6) + 2;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(this.x + duck.offDir * (6f + Rando.Float(-2f, 4f)), this.y + Rando.Float(-2f, 4f), new Vec2(duck.offDir * Rando.Float(1.2f, 4f), Rando.Float(0.0f, -2.8f)), water, thickMult: 5f)
                    {
                        depth = this.depth + 1
                    };
                    Level.Add(fluid);
                }
            }
            else
            {
                if (!(this._sprite.texture.textureName == "hats/tube"))
                    return;
                for (int index = 0; index < 4; ++index)
                {
                    TinyBubble tinyBubble = new TinyBubble(this.x + Rando.Float(-4f, 4f), this.y + Rando.Float(0.0f, 4f), Rando.Float(-1.5f, 1.5f), this.y - 12f, true)
                    {
                        depth = this.depth + 1
                    };
                    Level.Add(tinyBubble);
                }
            }
        }

        public override void CloseHat()
        {
            if (this.duck == null)
                return;
            if (this.team != null && this.team.metadata != null)
            {
                if (this.team.metadata.WetLips.value && _timeOpen > 1.0)
                    SFX.Play("smallSplat", 0.9f, Rando.Float(-0.4f, 0.4f));
                if (this.team.metadata.MechanicalLips.value && _timeOpen > 2.0)
                    SFX.Play("smallDoorShut", pitch: Rando.Float(-0.1f, 0.1f));
            }
            if (this._sprite.texture.textureName == "hats/burgers")
            {
                if (_timeOpen <= 1.0)
                    return;
                FluidData ketchup = Fluid.Ketchup;
                ketchup.amount = Rando.Float(0.0005f, 1f / 1000f);
                int num = Rando.Int(3) + 1;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(this.x + duck.offDir * (3f + Rando.Float(0.0f, 6f)), this.y + 4f + Rando.Float(0.0f, 1f), new Vec2(duck.offDir * Rando.Float(-2f, 2f), Rando.Float(-1f, -2f)), ketchup, thickMult: 2.5f)
                    {
                        depth = this.depth + 1
                    };
                    Level.Add(fluid);
                }
                SFX.Play("smallSplat", 0.9f, Rando.Float(-0.4f, 0.4f));
            }
            else
            {
                if (!(this._sprite.texture.textureName == "hats/divers") && !(this._sprite.texture.textureName == "hats/fridge") || _timeOpen <= 2.0)
                    return;
                SFX.Play("smallDoorShut", pitch: Rando.Float(-0.1f, 0.1f));
            }
        }

        public override void Draw()
        {
            int frame = this._sprite.frame;
            sbyte offDir = this.offDir;
            if (this._team == null && this.duck != null)
                this._team = this.duck.team;
            Vec2 hatOffset = this._hatOffset;
            if (this._team != null)
            {
                if (this._team.noCrouchOffset && this.duck != null && this.duck.crouch)
                    ++this._hatOffset.y;
                if (this._team.metadata != null)
                {
                    if (this._team.metadata.HatNoFlip.value && this.offDir < 0)
                        this._hatOffset.x -= 4f;
                    if (this.duck != null && this.duck.sliding)
                        ++this._hatOffset.y;
                    if (_quackWait > 0.0)
                        this._sprite.frame = 0;
                    else if (_quackHold > 0.0)
                        this._sprite.frame = 1;
                }
            }
            this._wave.Update();
            if (this._isKatanaHat && !(Level.current is RockScoreboard))
            {
                DuckGame.Graphics.material = _katanaMaterial;
                base.Draw();
                DuckGame.Graphics.material = null;
            }
            else if (this._team != null && this._team.metadata != null)
            {
                this.PositionOnOwner();
                if (this.graphic != null)
                {
                    if (!this._team.metadata.HatNoFlip.value)
                        this.graphic.flipH = this.offDir <= 0;
                    this._graphic.position = this.position;
                    this._graphic.alpha = this.alpha;
                    this._graphic.angle = this.angle;
                    this._graphic.depth = this.depth;
                    this._graphic.scale = this.scale;
                    this._graphic.center = this.center;
                    this._graphic.Draw();
                }
            }
            else
                base.Draw();
            this._hatOffset = hatOffset;
            if (this.duck != null)
            {
                if (this._sprite.texture.textureName == "hats/sensei")
                {
                    if (this._specialSprite == null)
                    {
                        this._specialSprite = new Sprite("hats/senpaiStar");
                        this._specialSprite.CenterOrigin();
                    }
                    this._fade = Lerp.Float(this._fade, this.frame == 1 ? 1f : 0.0f, 0.1f);
                    if (_fade > 0.00999999977648258)
                    {
                        this._specialSprite.alpha = (float)((double)this.alpha * 0.699999988079071 * (0.5 + (double)this._wave.normalized * 0.5)) * this._fade;
                        this._specialSprite.scale = this.scale;
                        this._specialSprite.depth = this.depth - 10;
                        this._specialSprite.angle += 0.02f;
                        float num = (float)(0.800000011920929 + (double)this._wave.normalized * 0.200000002980232);
                        this._specialSprite.scale = new Vec2(num, num);
                        Vec2 vec2 = this.Offset(new Vec2(2f, 4f));
                        DuckGame.Graphics.Draw(this._specialSprite, vec2.x, vec2.y);
                    }
                }
                else if (this._sprite.frame == 1 && this._sprite.texture.textureName == "hats/master")
                {
                    if (this._specialSprite == null)
                    {
                        this._specialSprite = new Sprite("hats/master_glow");
                        this._specialSprite.CenterOrigin();
                    }
                    this._specialSprite.alpha = Math.Min(this.glow, 1f);
                    this._specialSprite.scale = this.scale;
                    this._specialSprite.depth = this.depth + 10;
                    this._specialSprite.angle = this.angle;
                    if (this.offDir < 0)
                    {
                        Vec2 vec2_1 = this.Offset(new Vec2(1f, 2f));
                        DuckGame.Graphics.Draw(this._specialSprite, vec2_1.x, vec2_1.y);
                        Vec2 vec2_2 = this.Offset(new Vec2(5f, 2f));
                        DuckGame.Graphics.Draw(this._specialSprite, vec2_2.x, vec2_2.y);
                    }
                    else
                    {
                        Vec2 vec2_3 = this.Offset(new Vec2(0.0f, 2f));
                        DuckGame.Graphics.Draw(this._specialSprite, vec2_3.x, vec2_3.y);
                        Vec2 vec2_4 = this.Offset(new Vec2(4f, 2f));
                        DuckGame.Graphics.Draw(this._specialSprite, vec2_4.x, vec2_4.y);
                    }
                    if (glow > 0.0)
                        this.glow -= 0.02f;
                }
            }
            if (this._addedParticles != null)
            {
                foreach (Thing addedParticle in this._addedParticles)
                    addedParticle.DoDraw();
                this._addedParticles.Clear();
            }
            this._sprite.frame = frame;
            this.offDir = offDir;
        }

        public class CustomParticle : PhysicsParticle, IFactory
        {
            private Team.CustomHatMetadata _metadata;
            private Vec2 _particleAlpha;
            private Vec2 _particleScale;
            private Vec2 _particleGravity;
            private Vec2 _particleFriction;
            private Vec2 _particleRotation;
            public List<Texture2D> animationFrames;
            public float animationSpeed;
            public bool animationLoop;
            private float _currentAnimationFrame;
            private float _lifespan;
            //private Vec2 _movementDif;
            //private Vec2 _prevOwnerPosition;
            //private Vec2 _gravityVelocity;

            public CustomParticle(Vec2 pPosition, Thing pOwner, Team.CustomHatMetadata pMetadata)
              : base(pPosition.x, pPosition.y)
            {
                this._metadata = pMetadata;
                this._owner = pOwner;
                this._gravMult = 0.0f;
                this.graphic = new Sprite((Tex2D)this._metadata.team.customParticles[Rando.Int(this._metadata.team.customParticles.Count - 1)]);
                this.graphic.CenterOrigin();
                this.offDir = pOwner.offDir;
                if (!this._metadata.HatNoFlip.value)
                    this.graphic.flipH = this.offDir < 0;
                this.center = new Vec2(this.graphic.width / 2, this.graphic.height / 2);
                this._lifespan = this._metadata.ParticleLifespan.value;
                //this._prevOwnerPosition = this._owner.position;
                this._particleAlpha = this._metadata.ParticleAlpha.value;
                this._particleScale = this._metadata.ParticleScale.value;
                this._particleGravity = new Vec2(this._owner.OffsetLocal(this._metadata.ParticleGravity.value).x, this._metadata.ParticleGravity.value.y);
                this._particleFriction = this._metadata.ParticleFriction.value;
                this._particleRotation = this._metadata.ParticleRotation.value;
                this.depth = this._metadata.ParticleBackground.value ? pOwner.depth - 8 : pOwner.depth + 8;
                this.velocity = this._owner.OffsetLocal(this._metadata.ParticleVelocity.value);
                this._life = 1f;
                if (!this._metadata.ParticleAnchor.value)
                {
                    sbyte offDir = this._owner.offDir;
                    if (this._metadata.HatNoFlip.value)
                        this._owner.offDir = 1;
                    this.position = this._owner.Offset(this.position);
                    this._owner.offDir = offDir;
                }
                if (this._metadata.ParticleAnimationRandomFrame.value)
                    this._currentAnimationFrame = Rando.Int(this._metadata.team.customParticles.Count - 1);
                this.UpdateAppearance();
            }

            private void UpdateAppearance()
            {
                this.xscale = this.yscale = Lerp.FloatSmooth(this._particleScale.x, this._particleScale.y, 1f - this._life);
                this.xscale = Maths.Clamp(this.xscale, 0.0f, 1f);
                this.yscale = Maths.Clamp(this.yscale, 0.0f, 1f);
                this.alpha = Lerp.FloatSmooth(this._particleAlpha.x, this._particleAlpha.y, 1f - this._life);
                this.angleDegrees = Lerp.FloatSmooth(this._particleRotation.x, this._particleRotation.y, 1f - this._life) * 10f;
            }

            public override void Update()
            {
                this.velocity += this._particleGravity;
                this.velocity *= this._particleFriction;
                this.hSpeed = Maths.Clamp(this.hSpeed, -4f, 4f);
                this.vSpeed = Maths.Clamp(this.vSpeed, -4f, 4f);
                this._life -= (float)(60.0 / (60.0 * _lifespan)) * Maths.IncFrameTimer();
                this.UpdateAppearance();
                if (_life <= 0.0)
                    Level.Remove(this);
                if (this._metadata.ParticleAnchor.value)
                {
                    Vec2 position1 = this.position;
                    sbyte offDir = this._owner.offDir;
                    if (this._metadata.HatNoFlip.value)
                        this._owner.offDir = 1;
                    this.position = this._owner.Offset(this.position);
                    this._owner.offDir = offDir;
                    Vec2 position2 = this.position;
                    base.Update();
                    this.position = position1 + (this.position - position2);
                }
                else
                    base.Update();
                if (this.animationFrames == null)
                    return;
                this.graphic.texture = (Tex2D)this.animationFrames[(int)this._currentAnimationFrame % this.animationFrames.Count];
                this._currentAnimationFrame += this.animationSpeed;
                if (this.animationLoop || _currentAnimationFrame < (double)this.animationFrames.Count)
                    return;
                this._currentAnimationFrame = this.animationFrames.Count - 1;
            }

            public override void Draw()
            {
                if (this._metadata.ParticleAnchor.value)
                {
                    Vec2 position1 = this.position;
                    sbyte offDir = this._owner.offDir;
                    if (this._metadata.HatNoFlip.value)
                        this._owner.offDir = 1;
                    this.position = this._owner.Offset(this.position);
                    this._owner.offDir = offDir;
                    float angle = this.angle;
                    if (this._metadata.ParticleAnchorOrientation.value)
                        this.angleDegrees += this._owner.angleDegrees;
                    Vec2 position2 = this.position;
                    base.Draw();
                    this.position = position1 + (this.position - position2);
                    this.angle = angle;
                }
                else
                    base.Draw();
            }
        }
    }
}
