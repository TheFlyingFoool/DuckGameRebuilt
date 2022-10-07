namespace DuckGame
{
    [ClientOnly]
    public class WumpBeam : Thing
    {
        public WumpBeam(Vec2 pos, Vec2 target, WumpHugeLaser blastOwner) : base(pos.x, pos.y, null)
        {
            _target = target;
            _blastOwner = blastOwner;
        }

        public WumpBeam(Vec2 pos, Vec2 target) : base(pos.x, pos.y, null)
        {
            _target = target;
        }
        public override void Initialize()
        {
            destroy = SFX.Get("scimiHum", 0, -0.2f);
            base.Initialize();
        }
        public Sound destroy;
        public override void Update()
        {
            if (isLocal)
            {
                if (destroy.State != Microsoft.Xna.Framework.Audio.SoundState.Playing)
                {
                    destroy.Play();
                }
                _blastOwner.uncharge = true;
                _blastOwner.weight = 7;
                position = _blastOwner.Offset(_blastOwner.barrelOffset);
                _target = _blastOwner.Offset(_blastOwner.barrelOffset + new Vec2(1200f, 0f)) - position;

                if (_blast > 0.6f && _blastOwner.duck != null)
                {
                    _blastOwner.ApplyKick();
                    int num = 0;
                    Vec2 normalized = _target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
                    Vec2 normalized2 = _target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
                    Vec2 value = position + normalized * 16f;
                    for (int i = 0; i < 5; i++)
                    {
                        Vec2 vec = value + normalized2 * 8f * i;
                        foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(vec, vec + _target))
                        {
                            if (materialThing is IAmADuck && !materialThing.destroyed && !(materialThing is TargetDuck))
                            {
                                if (materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck != null && !(materialThing as RagdollPart).doll.captureDuck.dead)
                                {
                                    num++;
                                }
                                if (materialThing is TrappedDuck && (materialThing as TrappedDuck).captureDuck != null && !(materialThing as TrappedDuck).captureDuck.dead)
                                {
                                    num++;
                                }
                                if (materialThing is Duck && !(materialThing as Duck).dead)
                                {
                                    num++;
                                }
                            }
                            materialThing.heat = Maths.CountDown(materialThing.heat, -1, 0.03f);
                            SuperFondle(materialThing, DuckNetwork.localConnection);
                            materialThing.Destroy(new DTIncinerate(this));
                        }
                    }
                    Global.data.giantLaserKills += num;
                }
            }
            if (Recorder.currentRecording != null)
            {
                Recorder.currentRecording.LogBonus();
            }
            life++;
            if (life > 200 || _blastOwner.duck == null)
            {
                _blastOwner.uncharge = false;
                _blastOwner.weight = 5;
                _blast -= 0.02f;
                destroy.Pitch -= 0.05f;
                destroy.Volume -= 0.02f;
                if (_blast < 0)
                {
                    destroy.Stop();
                    Level.Remove(this);
                }
            }
            else
            {
                destroy.Volume = 1;
                destroy.Pitch = sin * 0.1f - 0.2f;
            }
        }

        public override void Draw()
        {
            _blast -= sin * 0.005f;
            float num2 = Maths.NormalizeSection(_blast, 0.6f, 1f);
            float num3 = Maths.NormalizeSection(_blast, 0.75f, 1f);
            float num4 = Maths.NormalizeSection(_blast, 0.9f, 1f);
            float num5 = Maths.NormalizeSection(_blast, 0.8f, 1f) * 0.5f;

            Vec2 p = position;
            Vec2 p2 = position + _target;
            Graphics.DrawLine(p, p2, new Color(_blast * 0.7f + 0.3f, _blast, _blast) * (0.3f + num5), 1f + num2 * 12f, default(Depth));
            Graphics.DrawLine(p, p2, Color.LightBlue * (0.2f + num5), 1f + num3 * 28f, default(Depth));
            Graphics.DrawLine(p, p2, Color.LightBlue * (0.1f + num5), 0.2f + num4 * 40f, default(Depth));
        }

        public SinWave sin = new SinWave(0.1f);
        public int life;
        public float _blast = 1f;

        private Vec2 _target;

        private WumpHugeLaser _blastOwner;
    }
}
