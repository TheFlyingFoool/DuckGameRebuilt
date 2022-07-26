// Decompiled with JetBrains decompiler
// Type: DuckGame.Goody
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class Goody : MaterialThing, ISequenceItem
    {
        public string collectSound = "goody";
        public bool hidden;

        public Goody(float xpos, float ypos, Sprite sprite)
          : base(xpos, ypos)
        {
            this.graphic = sprite;
            this.center = new Vec2((float)(sprite.w / 2), (float)(sprite.h / 2));
            this._collisionSize = new Vec2(10f, 10f);
            this.collisionOffset = new Vec2(-5f, -5f);
            this.sequence = new SequenceItem((Thing)this);
            this.sequence.type = SequenceItemType.Goody;
            this.enablePhysics = false;
            this._impactThreshold = 1E-06f;
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor) && this.sequence.waitTillOrder && this.sequence.order != 0)
            {
                this.visible = false;
                this.hidden = true;
            }
            base.Initialize();
        }

        public override void OnSequenceActivate()
        {
            if (this.sequence.waitTillOrder)
            {
                if (this._visibleInGame)
                    this.visible = true;
                this.hidden = false;
            }
            base.OnSequenceActivate();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.hidden)
                return;
            switch (with)
            {
                case Duck _:
                case RagdollPart _:
                case TrappedDuck _:
                    if (with.destroyed)
                        break;
                    this.visible = false;
                    this.hidden = true;
                    if (this.collectSound != null && this.collectSound != "")
                        SFX.Play(this.collectSound, 0.8f);
                    if (this._visibleInGame)
                    {
                        Profile profileToRumble = (Profile)null;
                        switch (with)
                        {
                            case Duck _:
                                profileToRumble = (with as Duck).profile;
                                break;
                            case RagdollPart _:
                                if ((with as RagdollPart).doll != null && (with as RagdollPart).doll._duck != null)
                                {
                                    profileToRumble = (with as RagdollPart).doll._duck.profile;
                                    break;
                                }
                                break;
                            case TrappedDuck _:
                                profileToRumble = (with as TrappedDuck)._duckOwner.profile;
                                break;
                        }
                        if (profileToRumble != null)
                            RumbleManager.AddRumbleEvent(profileToRumble, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.Short));
                    }
                    if (Level.current is Editor)
                        break;
                    this._sequence.Finished();
                    if (!ChallengeLevel.running || !this.sequence.isValid)
                        break;
                    ++ChallengeLevel.goodiesGot;
                    break;
            }
        }
    }
}
