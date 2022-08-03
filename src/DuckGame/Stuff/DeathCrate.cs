// Decompiled with JetBrains decompiler
// Type: DuckGame.DeathCrate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("isOnlineCapable", true)]
    [BaggedProperty("isInDemo", true)]
    public class DeathCrate : Holdable, IPlatform
    {
        public StateBinding _settingIndexBinding = new StateBinding(nameof(settingIndex));
        public StateBinding _activatedBinding = new StateBinding(nameof(activated));
        private SpriteMap _sprite;
        public byte _beeps;
        private static List<DeathCrateSetting> _settings = new List<DeathCrateSetting>();
        private bool _didActivation;
        public bool activated;
        public byte settingIndex;
        public DeathCrateSetting _storedSetting;

        public static void InitializeDeathCrateSettings()
        {
            if (MonoMain.moddingEnabled)
            {
                foreach (System.Type sortedType in ManagedContent.DeathCrateSettings.SortedTypes)
                    DeathCrate._settings.Add(Activator.CreateInstance(sortedType) as DeathCrateSetting);
            }
            else
            {
                foreach (System.Type type in (IEnumerable<System.Type>)Editor.GetSubclasses(typeof(DeathCrateSetting)).ToList<System.Type>())
                    DeathCrate._settings.Add(Activator.CreateInstance(type) as DeathCrateSetting);
            }
        }

        public DeathCrateSetting setting => settingIndex < DeathCrate._settings.Count ? DeathCrate._settings[settingIndex] : new DCSwordAdventure();

        public DeathCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _maxHealth = 15f;
            _hitPoints = 15f;
            _sprite = new SpriteMap("deathcrate", 16, 19);
            graphic = _sprite;
            center = new Vec2(8f, 11f);
            collisionOffset = new Vec2(-8f, -11f);
            collisionSize = new Vec2(16f, 18f);
            depth = -0.5f;
            _editorName = "Death Crate";
            editorTooltip = "Explodes in a violent surprise when triggered.";
            thickness = 2f;
            weight = 5f;
            _sprite.AddAnimation("idle", 1f, true, new int[1]);
            _sprite.AddAnimation("activate", 0.35f, false, 1, 2, 3, 4, 4, 5, 4, 4, 5, 6, 6, 6, 6, 6, 6, 6, 6, 5, 7, 7, 7, 7, 7, 7, 7, 7, 5, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 5);
            _sprite.SetAnimation("idle");
            _holdOffset = new Vec2(2f, 0f);
            flammable = 0f;
            collideSounds.Add("crateHit");
            for (int index = 0; index < 100; ++index)
            {
                settingIndex = (byte)Rando.Int(DeathCrate._settings.Count - 1);
                if (_settings[settingIndex].likelyhood == 1.0 || Rando.Float(1f) < _settings[settingIndex].likelyhood)
                    break;
            }
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.isStateObject)
                with.Fondle(this);
            if (from == ImpactedFrom.Top || Math.Abs(angleDegrees) > 90f && Math.Abs(angleDegrees) < 270.0 && from == ImpactedFrom.Bottom && with.totalImpactPower + totalImpactPower > 0.1f && _sprite.currentAnimation == "idle")
            {
                activated = true;
                _sprite.SetAnimation("activate");
                SFX.Play("click");
                collisionOffset = new Vec2(-8f, -8f);
                collisionSize = new Vec2(16f, 15f);
            }
            base.OnSolidImpact(with, from);
        }

        public override void Terminate()
        {
            if (duck != null)
                duck.ThrowItem();
            base.Terminate();
        }

        public void ActivateSetting(bool isServer)
        {
            _didActivation = true;
            _storedSetting = setting;
            _storedSetting.Activate(this, isServer);
            if (!isServer)
                return;
            Send.Message(new NMActivateDeathCrate(settingIndex, this));
        }

        public override void Update()
        {
            if (activated && _sprite.currentAnimation != "activate")
            {
                _sprite.SetAnimation("activate");
                collisionOffset = new Vec2(-8f, -8f);
                collisionSize = new Vec2(16f, 15f);
            }
            if (_sprite.imageIndex == 6 && _beeps == 0)
            {
                SFX.Play("singleBeep");
                ++_beeps;
            }
            if (_sprite.imageIndex == 7 && _beeps == 1)
            {
                SFX.Play("singleBeep");
                ++_beeps;
            }
            if (_sprite.imageIndex == 8 && _beeps == 2)
            {
                SFX.Play("singleBeep");
                ++_beeps;
            }
            if (_sprite.imageIndex == 5 && _beeps == 3)
            {
                SFX.Play("doubleBeep", pitch: 0.2f);
                ++_beeps;
            }
            if (isServerForObject)
            {
                if (_didActivation && _storedSetting != null)
                    _storedSetting.Update(this);
                if (_sprite.currentAnimation == "activate" && _sprite.finished && !_didActivation)
                    ActivateSetting(true);
            }
            base.Update();
        }

        public override void Draw()
        {
            sbyte offDir = this.offDir;
            this.offDir = 1;
            base.Draw();
            this.offDir = offDir;
        }

        protected override bool OnDestroy(DestroyType type = null) => false;
    }
}
