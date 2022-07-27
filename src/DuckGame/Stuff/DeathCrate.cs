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
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this._sprite = new SpriteMap("deathcrate", 16, 19);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 11f);
            this.collisionOffset = new Vec2(-8f, -11f);
            this.collisionSize = new Vec2(16f, 18f);
            this.depth = - 0.5f;
            this._editorName = "Death Crate";
            this.editorTooltip = "Explodes in a violent surprise when triggered.";
            this.thickness = 2f;
            this.weight = 5f;
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("activate", 0.35f, false, 1, 2, 3, 4, 4, 5, 4, 4, 5, 6, 6, 6, 6, 6, 6, 6, 6, 5, 7, 7, 7, 7, 7, 7, 7, 7, 5, 8, 8, 8, 8, 8, 8, 8, 8, 5, 9, 9, 5);
            this._sprite.SetAnimation("idle");
            this._holdOffset = new Vec2(2f, 0.0f);
            this.flammable = 0.0f;
            this.collideSounds.Add("crateHit");
            for (int index = 0; index < 100; ++index)
            {
                this.settingIndex = (byte)Rando.Int(DeathCrate._settings.Count - 1);
                if (_settings[settingIndex].likelyhood == 1.0 || (double)Rando.Float(1f) < _settings[settingIndex].likelyhood)
                    break;
            }
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with.isStateObject)
                with.Fondle(this);
            if (from == ImpactedFrom.Top || (double)Math.Abs(this.angleDegrees) > 90.0 && (double)Math.Abs(this.angleDegrees) < 270.0 && from == ImpactedFrom.Bottom && (double)with.totalImpactPower + (double)this.totalImpactPower > 0.100000001490116 && this._sprite.currentAnimation == "idle")
            {
                this.activated = true;
                this._sprite.SetAnimation("activate");
                SFX.Play("click");
                this.collisionOffset = new Vec2(-8f, -8f);
                this.collisionSize = new Vec2(16f, 15f);
            }
            base.OnSolidImpact(with, from);
        }

        public override void Terminate()
        {
            if (this.duck != null)
                this.duck.ThrowItem();
            base.Terminate();
        }

        public void ActivateSetting(bool isServer)
        {
            this._didActivation = true;
            this._storedSetting = this.setting;
            this._storedSetting.Activate(this, isServer);
            if (!isServer)
                return;
            Send.Message(new NMActivateDeathCrate(this.settingIndex, this));
        }

        public override void Update()
        {
            if (this.activated && this._sprite.currentAnimation != "activate")
            {
                this._sprite.SetAnimation("activate");
                this.collisionOffset = new Vec2(-8f, -8f);
                this.collisionSize = new Vec2(16f, 15f);
            }
            if (this._sprite.imageIndex == 6 && this._beeps == 0)
            {
                SFX.Play("singleBeep");
                ++this._beeps;
            }
            if (this._sprite.imageIndex == 7 && this._beeps == 1)
            {
                SFX.Play("singleBeep");
                ++this._beeps;
            }
            if (this._sprite.imageIndex == 8 && this._beeps == 2)
            {
                SFX.Play("singleBeep");
                ++this._beeps;
            }
            if (this._sprite.imageIndex == 5 && this._beeps == 3)
            {
                SFX.Play("doubleBeep", pitch: 0.2f);
                ++this._beeps;
            }
            if (this.isServerForObject)
            {
                if (this._didActivation && this._storedSetting != null)
                    this._storedSetting.Update(this);
                if (this._sprite.currentAnimation == "activate" && this._sprite.finished && !this._didActivation)
                    this.ActivateSetting(true);
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
