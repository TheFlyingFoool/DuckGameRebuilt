namespace DuckGame
{
    public abstract class Goody : MaterialThing, ISequenceItem
    {
        public string collectSound = "goody";
        public bool hidden;

        public Goody(float xpos, float ypos, Sprite sprite)
          : base(xpos, ypos)
        {
            graphic = sprite;
            center = new Vec2(sprite.w / 2, sprite.h / 2);
            _collisionSize = new Vec2(10f, 10f);
            collisionOffset = new Vec2(-5f, -5f);
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            enablePhysics = false;
            _impactThreshold = 1E-06f;
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor) && sequence.waitTillOrder && sequence.order != 0)
            {
                visible = false;
                hidden = true;
            }
            base.Initialize();
        }

        public override void OnSequenceActivate()
        {
            if (sequence.waitTillOrder)
            {
                if (_visibleInGame)
                    visible = true;
                hidden = false;
            }
            base.OnSequenceActivate();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (hidden)
                return;
            switch (with)
            {
                case Duck _:
                case RagdollPart _:
                case TrappedDuck _:
                    if (with.destroyed)
                        break;
                    visible = false;
                    hidden = true;
                    if (collectSound != null && collectSound != "")
                        SFX.Play(collectSound, 0.8f);
                    if (_visibleInGame)
                    {
                        Profile profileToRumble = null;
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
                    _sequence.Finished();
                    if (!ChallengeLevel.running || !sequence.isValid)
                        break;
                    ++ChallengeLevel.goodiesGot;
                    break;
            }
        }
    }
}
