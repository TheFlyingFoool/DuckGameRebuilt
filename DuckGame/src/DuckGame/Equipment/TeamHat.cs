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
        private List<CustomParticle> _addedParticles;
        private float _quackWait;
        private float _quackHold;

        public ushort netTeamIndex
        {
            get => _team == null ? (ushort)0 : (ushort)Teams.IndexOf(_team);
            set => team = Teams.ParseFromIndex(value);
        }

        public Team team
        {
            get => _team;
            set
            {
                _team = value;
                _shouldUpdateSprite = true;
            }
        }

        public override void SetQuack(int pValue)
        {
            PositionOnOwner();
            frame = pValue;
            if (_equippedDuck != null && !destroyed)
            {
                if (_prevFrame == 0 && _sprite.frame == 1)
                    OpenHat();
                else if (_prevFrame == 1 && _sprite.frame == 0)
                    CloseHat();
            }
            _prevFrame = _sprite.frame;
        }

        public void UpdateSprite()
        {
            if (_profile == null && equippedDuck != null && equippedDuck.profile == Profiles.EnvironmentProfile)
            {
                _shouldUpdateSprite = true;
            }
            else
            {
                if (_team != null && (_team != _lastLoadedTeam && (_team.facade == null || _team.facade != _lastLoadedTeam) || _prevHatID != _team.hatID || _team.filter != _filter))
                {
                    _filter = _team.filter;
                    if (_profile == null && equippedDuck != null)
                        _profile = equippedDuck.profile;
                    sprite = _team.hat.CloneMap();
                    pickupSprite = _team.hat.Clone();
                    DuckPersona pPersona = quickPersona;
                    if (_profile != null)
                        pPersona = _profile.persona;
                    if (pPersona != null && _team.metadata != null && _team.metadata.UseDuckColor.value)
                    {
                        sprite = _team.GetHat(pPersona).CloneMap();
                        pickupSprite = _team.GetHat(pPersona).Clone();
                    }
                    sprite.center = new Vec2(16f, 16f);
                    hatOffset = _team.hatOffset;
                    UpdateCape();
                    _lastLoadedTeam = _team.facade != null ? _team.facade : _team;
                    _prevHatID = _team.hatID;
                    graphic = sprite;
                }
                if (_specialInitialized || _team == null)
                    return;
                _specialInitialized = true;
                _isKatanaHat = _sprite.texture.textureName == "hats/katanaman";
                if (!_isKatanaHat)
                    return;
                _katanaMaterial = new MaterialKatanaman(this);
            }
        }

        public TeamHat(float xpos, float ypos, Team t)
          : base(xpos, ypos)
        {
            team = t;
            depth = -0.5f;
            shouldbegraphicculled = false;
        }

        public TeamHat(float xpos, float ypos, Team t, Profile p)
          : base(xpos, ypos)
        {
            _profile = p;
            team = t;
            depth = -0.5f;
            shouldbegraphicculled = false;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("teamIndex", Teams.all.IndexOf(_team));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            int property = node.GetProperty<int>("teamIndex");
            if (property >= 0 && property < Teams.all.Count - 1)
                team = Teams.all[property];
            return base.Deserialize(node);
        }

        public override void Initialize()
        {
            UpdateCape();
            base.Initialize();
        }

        public void UpdateCape()
        {
            if (_team == null)
                return;
            if (_sprite.texture.textureName == "hats/johnnys")
                quacks = false;
            if (_cape != null)
            {
                if (_cape.level != null)
                    Level.Remove(_cape);
                _cape = null;
            }
            if (_sprite.texture.textureName == "hats/suit")
            {
                Tex2D tex = null;
                int idx = Global.data.flag;
                if (Network.isActive)
                    idx = _networkCape;
                if (idx < 0)
                    return;
                Sprite flag = UIFlagSelection.GetFlag(idx);
                if (flag != null)
                    tex = flag.texture;
                if (tex == null)
                    return;
                _cape = new Cape(x, y, this);
                if (!tex.textureName.Contains("full_"))
                    _cape.halfFlag = true;
                _cape.SetCapeTexture((Texture2D)tex);
            }
            else
            {
                if (_team.capeTexture == null)
                    return;
                _cape = new Cape(x, y, this);
                if (_team.metadata != null) _cape.metadata = _team.metadata;
                _cape.SetCapeTexture(_team.capeTexture);
            }
        }

        public override void Terminate()
        {
            if (_cape != null)
                Level.Remove(_cape);
            base.Terminate();
        }

        public override void Update()
        {
            if (_cape != null && _cape.level == null)
                Level.Add(_cape);
            if (team != null && team.metadata != null)
            {
                bouncy = team.metadata.Bouncyness.value; //YUH 
                if (team.metadata.Roll.value && isServerForObject)
                {
                    if (grounded) angleDegrees += hSpeed * 3;
                    else angleDegrees += hSpeed * 2;
                }
                if (team.metadata.NoQuackSFX.value) quacks = false;
                if (team.metadata.PassiveParticleRate.value > 0 && (duck == null || duck.localSpawnVisible))
                {
                    if (PassiveParticleTimer < -100)
                    {
                        if (team.metadata.ParticleCount.value != 1) PassiveParticleTimer = Maths.Clamp(team.metadata.PassiveParticleRate.value, 0.1f, 2.5f);
                        else PassiveParticleTimer = team.metadata.PassiveParticleRate.value;
                    }
                    PassiveParticleTimer -= 0.017f;
                    if (PassiveParticleTimer <= 0 && PassiveParticlesActive)
                    {
                        if (team.metadata.ParticleCount.value != 1) PassiveParticleTimer = Maths.Clamp(team.metadata.PassiveParticleRate.value, 0.1f, 2.5f);
                        else PassiveParticleTimer = team.metadata.PassiveParticleRate.value;

                        bool canSpawn;
                        switch (team.metadata.PassiveParticleCondition.value)
                        {
                            case 5:
                                canSpawn = equippedDuck == null && owner != null;
                                break;
                            case 4:
                                canSpawn = frame == 0;
                                break;
                            case 3:
                                canSpawn = frame > 0;
                                break;
                            case 2:
                                canSpawn = equippedDuck == null;
                                break;
                            case 1:
                                canSpawn = equippedDuck != null;
                                break;
                            case 0:
                            default:
                                canSpawn = true; 
                                break;
                        }
                        if (canSpawn) SpawnParticles();
                    }
                }
            }
            if (Network.isActive)
            {
                if (_team != null && _team.filter != _filter)
                {
                    UpdateCape();
                    _shouldUpdateSprite = true;
                }
                if (_networkCape < 0 && duck != null && duck.profile != null)
                {
                    _networkCape = !duck.profile.localPlayer ? duck.profile.flagIndex : Global.data.flag;
                    UpdateCape();
                }
                if (Network.inLobby && _team != null && (sprite == null || sprite != null && sprite.globalIndex != _team.hat.globalIndex)) _shouldUpdateSprite = true;
            }
            else if (Level.current is TeamSelect2 && _equippedDuck != null && team != null && team.customHatPath != null && Keyboard.Pressed(Keys.F5) && !Network.isActive)
            {
                int index = Teams.core.extraTeams.IndexOf(team);
                if (index >= 0) // issue nike was having realted to hat reloading F6 and F5, unsure why the teams hat wouldnt be in the list but this should stop the crash
                {
                    Team.deserializeInto = team;
                    Teams.core.extraTeams[index] = Team.Deserialize(team.customHatPath);
                    Team.deserializeInto = null;
                    Duck equippedDuck = _equippedDuck;
                    _equippedDuck.Unequip(this);
                    Level.Remove(this);
                    TeamHat teamHat = new TeamHat(x, y, Teams.core.extraTeams[index]);
                    Level.Add(teamHat);
                    TeamHat e = teamHat;
                    equippedDuck.Equip(e, false);
                }
                else // well if it is that odd cause i guess ill just kill it
                {
                    _equippedDuck.Unequip(this);
                    Level.Remove(this);
                }
            }
            if (_shouldUpdateSprite)
            {
                _shouldUpdateSprite = false;
                UpdateSprite();
            }
            if (_equippedDuck != null && !destroyed)
            {
                if (_sprite.frame == 1) _timeOpen += 0.1f;
                else _timeOpen = 0f;
            }
            if (_sprite.frame == 1 && _prevFrame == 0) glow = 1.2f;
            _prevFrame = _sprite.frame;
            if (destroyed) alpha -= 0.05f;
            if (alpha < 0f) Level.Remove(this);
            if (_quackWait > 0) _quackWait -= Maths.IncFrameTimer();
            else if (_quackHold > 0) _quackHold -= Maths.IncFrameTimer();
            base.Update();
        }

        public override void Quack(float volume, float pitch)
        {
            if (duck != null && _sprite.texture.textureName == "hats/hearts")
            {
                SFX.Play("heartfart", volume, Math.Min(pitch + 0.4f - Rando.Float(0.1f), 1f));
                HeartPuff heartPuff = new HeartPuff(x, y)
                {
                    anchor = (Anchor)this
                };
                Level.Add(heartPuff);
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x, y);
                    smallSmoke.sprite.color = Color.Green * (0.4f + Rando.Float(0.3f));
                    Level.Add(smallSmoke);
                }
            }
            else SFX.Play("quack", volume, pitch);
        }

        public void SpawnParticles()
        {
            if (team.customParticles.Count <= 0)
                return;
            if (_addedParticles == null)
                _addedParticles = new List<CustomParticle>();
            int particleCount = team.metadata.ParticleCount.value;
            Vec2 vec2_1 = new Vec2((float)(-team.metadata.ParticleEmitShapeSize.value.x / 2f), (float)(-team.metadata.ParticleEmitShapeSize.value.y / 2f));
            Vec2 vec2_2 = new Vec2(team.metadata.ParticleEmitShapeSize.value.x / 2f, team.metadata.ParticleEmitShapeSize.value.y / 2f);
            Vec2 vec2_3 = team.metadata.ParticleEmitterOffset.value;
            for (int index1 = 0; index1 < particleCount; ++index1)
            {
                Vec2 pPosition = vec2_3;
                if (team.metadata.ParticleEmitShape.value.x == 1f)
                {
                    float rad = Maths.DegToRad(team.metadata.ParticleEmitShape.value.y == 2f ? index1 * (360f / particleCount) : Rando.Float(360f));
                    Vec2 vec2_4 = new Vec2((float)Math.Cos(rad) * (team.metadata.ParticleEmitShapeSize.value.x / 2f), (float)-Math.Sin(rad) * (team.metadata.ParticleEmitShapeSize.value.y / 2f));
                    if (team.metadata.ParticleEmitShape.value.y == 1f) pPosition += vec2_4 * Rando.Float(1f);
                    else pPosition += vec2_4;
                }
                else if (team.metadata.ParticleEmitShape.value.x == 2f)
                {
                    if (team.metadata.ParticleEmitShape.value.y == 0f)
                    {
                        float num2 = Rando.Float(1f) >= 0.5 ? 1f : -1f;
                        if (Rando.Float(1f) >= 0.5f) pPosition += new Vec2(team.metadata.ParticleEmitShapeSize.value.x * num2, Rando.Float((float)(-team.metadata.ParticleEmitShapeSize.value.y / 2f), team.metadata.ParticleEmitShapeSize.value.y / 2f));
                        else pPosition += new Vec2(Rando.Float((float)(-team.metadata.ParticleEmitShapeSize.value.x / 2f), team.metadata.ParticleEmitShapeSize.value.x / 2f), team.metadata.ParticleEmitShapeSize.value.y * num2);
                    }
                    else if (team.metadata.ParticleEmitShape.value.y == 1f) pPosition += new Vec2(Rando.Float((float)(-team.metadata.ParticleEmitShapeSize.value.x / 2f), team.metadata.ParticleEmitShapeSize.value.x / 2f), Rando.Float((float)(-team.metadata.ParticleEmitShapeSize.value.y / 2f), team.metadata.ParticleEmitShapeSize.value.y / 2f));
                    else if (team.metadata.ParticleEmitShape.value.y == 2f)
                    {
                        float rad = Maths.DegToRad(team.metadata.ParticleEmitShape.value.y == 2f ? index1 * (360f / particleCount) : Rando.Float(360f));
                        Vec2 vec2_5 = new Vec2((float)Math.Cos(rad) * 100f, (float)-Math.Sin(rad) * 100f);
                        //Vec2 zero = Vec2.Zero; what -Lucky
                        for (int index2 = 0; index2 < 4; ++index2)
                        {
                            Vec2 vec2_6 = Vec2.Zero;
                            if (index2 == 0)
                            {
                                if (Collision.LineIntersect(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_1.x, vec2_2.y))) vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_1.x, vec2_2.y));
                            }
                            else if (index2 == 1)
                            {
                                if (Collision.LineIntersect(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_2.x, vec2_1.y))) vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, vec2_1, new Vec2(vec2_2.x, vec2_1.y));
                            }
                            else if (index2 == 2)
                            {
                                if (Collision.LineIntersect(Vec2.Zero, vec2_5, new Vec2(vec2_1.x, vec2_2.y), vec2_2)) vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, new Vec2(vec2_1.x, vec2_2.y), vec2_2);
                            }
                            else if (index2 == 3 && Collision.LineIntersect(Vec2.Zero, vec2_5, new Vec2(vec2_2.x, vec2_1.y), vec2_2)) vec2_6 = Collision.LineIntersectPoint(Vec2.Zero, vec2_5, new Vec2(vec2_2.x, vec2_1.y), vec2_2);
                            if (vec2_6 != Vec2.Zero)
                            {
                                vec2_5 = vec2_6;
                                break;
                            }
                        }
                        pPosition += vec2_5;
                    }
                }
                CustomParticle customParticle = new CustomParticle(pPosition, this, team.metadata);
                if (team.metadata.ParticleAnimated.value)
                {
                    customParticle.animationFrames = team.customParticles;
                    customParticle.animationSpeed = team.metadata.ParticleAnimationSpeed.value * 0.5f;
                    customParticle.animationLoop = team.metadata.ParticleAnimationLoop.value;
                }
                Level.Add(customParticle);
                _addedParticles.Add(customParticle);
            }
        }

        public bool PassiveParticlesActive = true;
        public float PassiveParticleTimer = -1000;
        public override void OpenHat()
        {
            if (duck == null || duck.z != 0f)
                return;
            if (team != null && team.metadata != null && !team.metadata.PassiveParticleOverrideQuack.value)
            {
                if (team.metadata.QuackSuppressRequack.value && (_quackWait > 0 || _quackHold > 0))
                    return;
                _quackWait = team.metadata.QuackDelay.value;
                _quackHold = team.metadata.QuackHold.value;
                if (team.metadata.PassiveParticleToggle.value) PassiveParticlesActive = !PassiveParticlesActive;
                SpawnParticles();
            }
            else if (_sprite.texture.textureName == "hats/burgers")
            {
                FluidData ketchup = Fluid.Ketchup;
                ketchup.amount = Rando.Float(0.0005f, 0.001f);
                int num = Rando.Int(4) + 1;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(x + duck.offDir * (2f + Rando.Float(0f, 7f)), y + 3f + Rando.Float(0f, 3f), new Vec2(duck.offDir * Rando.Float(0.5f, 3f), Rando.Float(0f, -2f)), ketchup, thickMult: 2.5f)
                    {
                        depth = depth + 1
                    };
                    Level.Add(fluid);
                }
            }
            else if (_sprite.texture.textureName == "hats/divers" || _sprite.texture.textureName == "hats/fridge")
            {
                FluidData water = Fluid.Water;
                water.amount = Rando.Float(0.0001f, 0.0005f);
                int num = Rando.Int(3) + 1;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(x + duck.offDir * (2f + Rando.Float(0f, 4f)), y + 3f + Rando.Float(0f, 3f), new Vec2(duck.offDir * Rando.Float(0.5f, 3f), Rando.Float(0f, -2f)), water, thickMult: 5f)
                    {
                        depth = depth + 1
                    };
                    Level.Add(fluid);
                }
            }
            else if (_sprite.texture.textureName == "hats/gross")
            {
                FluidData water = Fluid.Water;
                water.amount = Rando.Float(0.0002f, 0.0007f);
                int num = Rando.Int(6) + 2;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(x + duck.offDir * (6f + Rando.Float(-2f, 4f)), y + Rando.Float(-2f, 4f), new Vec2(duck.offDir * Rando.Float(1.2f, 4f), Rando.Float(0f, -2.8f)), water, thickMult: 5f)
                    {
                        depth = depth + 1
                    };
                    Level.Add(fluid);
                }
            }
            else
            {
                if (!(_sprite.texture.textureName == "hats/tube"))
                    return;
                if (DGRSettings.S_ParticleMultiplier != 0)
                {
                    for (int index = 0; index < 4; ++index)
                    {
                        TinyBubble tinyBubble = new TinyBubble(x + Rando.Float(-4f, 4f), y + Rando.Float(0f, 4f), Rando.Float(-1.5f, 1.5f), y - 12f, true)
                        {
                            depth = depth + 1
                        };
                        Level.Add(tinyBubble);
                    }
                }
            }
        }

        public override void CloseHat()
        {
            if (duck == null)
                return;
            if (team != null && team.metadata != null)
            {
                if (team.metadata.WetLips.value && _timeOpen > 1f)
                    SFX.Play("smallSplat", 0.9f, Rando.Float(-0.4f, 0.4f));
                if (team.metadata.MechanicalLips.value && _timeOpen > 2f)
                    SFX.Play("smallDoorShut", pitch: Rando.Float(-0.1f, 0.1f));
            }
            if (_sprite.texture.textureName == "hats/burgers")
            {
                if (_timeOpen <= 1)
                    return;
                FluidData ketchup = Fluid.Ketchup;
                ketchup.amount = Rando.Float(0.0005f, 0.001f);
                int num = Rando.Int(3) + 1;
                for (int index = 0; index < num; ++index)
                {
                    Fluid fluid = new Fluid(x + duck.offDir * (3f + Rando.Float(0f, 6f)), y + 4f + Rando.Float(0f, 1f), new Vec2(duck.offDir * Rando.Float(-2f, 2f), Rando.Float(-1f, -2f)), ketchup, thickMult: 2.5f)
                    {
                        depth = depth + 1
                    };
                    Level.Add(fluid);
                }
                SFX.Play("smallSplat", 0.9f, Rando.Float(-0.4f, 0.4f));
            }
            else
            {
                if (!(_sprite.texture.textureName == "hats/divers") && !(_sprite.texture.textureName == "hats/fridge") || _timeOpen <= 2)
                    return;
                SFX.Play("smallDoorShut", pitch: Rando.Float(-0.1f, 0.1f));
            }
        }

        public override void Draw()
        {
            int frame = _sprite.frame;
            sbyte offDir = this.offDir;
            if (_team == null && duck != null) _team = duck.team;
            Vec2 hatOffset = _hatOffset;
            if (_team != null)
            {
                if (_team.noCrouchOffset && duck != null && duck.crouch) _hatOffset.y++;
                if (_team.metadata != null)
                {
                    if (_team.metadata.HatNoFlip.value && this.offDir < 0) _hatOffset.x -= 4f;
                    if (duck != null && duck.sliding) _hatOffset.y++;
                    if (_quackWait > 0f) _sprite.frame = 0;
                    else if (_quackHold > 0f) _sprite.frame = 1;
                }
            }
            if (MonoMain.UpdateLerpState) _wave.Update();
            if (_isKatanaHat && !(Level.current is RockScoreboard))
            {
                Graphics.material = _katanaMaterial;
                base.Draw();
                Graphics.material = null;
            }
            else if (_team != null && _team.metadata != null)
            {
                PositionOnOwner();
                if (graphic != null)
                {
                    if (!_team.metadata.HatNoFlip.value)
                        graphic.flipH = this.offDir <= 0;
                    _graphic.position = position;
                    _graphic.alpha = alpha;
                    _graphic.angle = angle;
                    _graphic.depth = depth;
                    _graphic.scale = scale;
                    _graphic.center = center;
                    _graphic.LerpState.CanLerp = true;
                    if (_graphic != null && duck != null)
                    {
                        _graphic.LerpState.CanAngleLerp = false;
                        _graphic.LerpState.ParentInterp = duck.DuckLerp;
                        _graphic.LerpState.SpecialAngleResetParentTrackingForHat = true;
                    }
                    _graphic.SkipIntraTick = SkipIntratick;
                    _graphic.Draw();
                }
            }
            else
            {
                if (_graphic != null && duck != null)
                {
                    _graphic.LerpState.CanAngleLerp = false;
                    _graphic.LerpState.ParentInterp = duck.DuckLerp;
                    _graphic.LerpState.SpecialAngleResetParentTrackingForHat = true;
                }
                base.Draw();
            }
            _hatOffset = hatOffset;
            if (duck != null)
            {
                if (_sprite.texture.textureName == "hats/sensei")
                {
                    if (_specialSprite == null)
                    {
                        _specialSprite = new Sprite("hats/senpaiStar");
                        _specialSprite.CenterOrigin();
                    }
                    if (MonoMain.UpdateLerpState) _fade = Lerp.Float(_fade, this.frame == 1 ? 1f : 0f, 0.1f);
                    if (_fade > 0.01f)
                    {
                        _specialSprite.alpha = alpha * 0.7f * (0.5f + _wave.normalized * 0.5f) * _fade;
                        _specialSprite.scale = scale;
                        _specialSprite.depth = depth - 10;
                        if (MonoMain.UpdateLerpState) _specialSprite.angle += 0.02f;
                        float num = 0.8f + _wave.normalized * 0.2f;
                        _specialSprite.scale = new Vec2(num, num);
                        Vec2 vec2 = Offset(new Vec2(2f, 4f));
                        Graphics.Draw(ref _specialSprite, vec2.x, vec2.y);
                    }
                }
                else if (_sprite.frame == 1 && _sprite.texture.textureName == "hats/master")
                {
                    if (_specialSprite == null)
                    {
                        _specialSprite = new Sprite("hats/master_glow");
                        _specialSprite.CenterOrigin();
                    }
                    _specialSprite.alpha = Math.Min(glow, 1f);
                    _specialSprite.scale = scale;
                    _specialSprite.depth = depth + 10;
                    _specialSprite.angle = angle;
                    if (this.offDir < 0)
                    {
                        Vec2 vec2_1 = Offset(new Vec2(1f, 2f));
                        Graphics.Draw(ref _specialSprite, vec2_1.x, vec2_1.y);
                        Vec2 vec2_2 = Offset(new Vec2(5f, 2f));
                        Graphics.Draw(ref _specialSprite, vec2_2.x, vec2_2.y);
                    }
                    else
                    {
                        Vec2 vec2_3 = Offset(new Vec2(0f, 2f));
                        Graphics.Draw(ref _specialSprite, vec2_3.x, vec2_3.y);
                        Vec2 vec2_4 = Offset(new Vec2(4f, 2f));
                        Graphics.Draw(ref _specialSprite, vec2_4.x, vec2_4.y);
                    }
                    if (glow > 0f && MonoMain.UpdateLerpState) glow -= 0.02f;
                }
            }
            if (_addedParticles != null)
            {
                foreach (Thing addedParticle in _addedParticles)
                    addedParticle.DoDraw();
                _addedParticles.Clear();
            }
            _sprite.frame = frame;
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
                _metadata = pMetadata;
                _owner = pOwner;
                _gravMult = 0f;
                graphic = new Sprite((Tex2D)_metadata.team.customParticles[Rando.Int(_metadata.team.customParticles.Count - 1)]);
                graphic.CenterOrigin();
                offDir = pOwner.offDir;
                if (!_metadata.HatNoFlip.value)
                    graphic.flipH = offDir < 0;
                center = new Vec2(graphic.width / 2, graphic.height / 2);
                _lifespan = _metadata.ParticleLifespan.value;
                //this._prevOwnerPosition = this._owner.position;
                _particleAlpha = _metadata.ParticleAlpha.value;
                _particleScale = _metadata.ParticleScale.value;
                if (_metadata.NoAirFriction.value) _airFriction = 0;
                uncappedSpeed = _metadata.UncappedSpeed.value;
                _particleGravity = new Vec2(_owner.OffsetLocal(_metadata.ParticleGravity.value).x, _metadata.ParticleGravity.value.y);
                _particleFriction = _metadata.ParticleFriction.value;
                _particleRotation = _metadata.ParticleRotation.value;
                depth = _metadata.ParticleBackground.value ? pOwner.depth - 8 : pOwner.depth + 8;
                velocity = _owner.OffsetLocal(_metadata.ParticleVelocity.value);
                _life = 1f;
                if (!_metadata.ParticleAnchor.value)
                {
                    sbyte offDir = _owner.offDir;
                    if (_metadata.HatNoFlip.value)
                        _owner.offDir = 1;
                    position = _owner.Offset(position);
                    _owner.offDir = offDir;
                }
                if (_metadata.ParticleAnimationRandomFrame.value)
                    _currentAnimationFrame = Rando.Int(_metadata.team.customParticles.Count - 1);
                UpdateAppearance();
            }

            private void UpdateAppearance()
            {
                xscale = yscale = Lerp.FloatSmooth(_particleScale.x, _particleScale.y, 1f - _life);
                xscale = Maths.Clamp(xscale, 0f, 1f);
                yscale = Maths.Clamp(yscale, 0f, 1f);
                alpha = Lerp.FloatSmooth(_particleAlpha.x, _particleAlpha.y, 1f - _life);
                angleDegrees = Lerp.FloatSmooth(_particleRotation.x, _particleRotation.y, 1f - _life) * 10f;
            }
            public bool uncappedSpeed;
            public override void Update()
            {
                
                velocity += _particleGravity;
                velocity *= _particleFriction;
                if (uncappedSpeed)
                {
                    hSpeed = Maths.Clamp(hSpeed, -8f, 8f);
                    vSpeed = Maths.Clamp(vSpeed, -8f, 8f);
                }
                else
                {
                    hSpeed = Maths.Clamp(hSpeed, -4f, 4f);
                    vSpeed = Maths.Clamp(vSpeed, -4f, 4f);
                }
                _life -= (float)(60 / (60 * _lifespan)) * Maths.IncFrameTimer();
                UpdateAppearance();
                if (_life <= 0)
                    Level.Remove(this);

                if (_metadata.ParticleAnchor.value)
                {
                    Vec2 position1 = position;
                    sbyte offDir = _owner.offDir;
                    if (_metadata.HatNoFlip.value)
                        _owner.offDir = 1;
                    position = _owner.Offset(position);
                    _owner.offDir = offDir;
                    Vec2 position2 = position;
                    base.Update();
                    position = position1 + (position - position2);
                }
                else
                    base.Update();
                if (animationFrames == null)
                    return;
                graphic.texture = (Tex2D)animationFrames[(int)_currentAnimationFrame % animationFrames.Count];
                _currentAnimationFrame += animationSpeed;
                if (animationLoop || _currentAnimationFrame < animationFrames.Count)
                    return;
                _currentAnimationFrame = animationFrames.Count - 1;
            }
            
            public override void Draw()
            {
                if (_metadata.ParticleAnchor.value)
                {
                    Vec2 position1 = position;
                    sbyte offDir = _owner.offDir;
                    if (_metadata.HatNoFlip.value) _owner.offDir = 1;
                    position = _owner.Offset(position);
                    _owner.offDir = offDir;
                    float angle = this.angle;
                    if (_metadata.ParticleAnchorOrientation.value) angleDegrees += _owner.angleDegrees;
                    Vec2 position2 = position;
                    base.Draw();
                    position = position1 + (position - position2);
                    this.angle = angle;
                }
                else base.Draw();
            }
        }
    }
}
