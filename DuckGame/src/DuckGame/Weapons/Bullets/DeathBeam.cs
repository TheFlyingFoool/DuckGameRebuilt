// Decompiled with JetBrains decompiler
// Type: DuckGame.DeathBeam
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DeathBeam : Thing
    {
        public float _blast = 1f;
        private Vec2 _target;
        private Thing _blastOwner;

        public DeathBeam(Vec2 pos, Vec2 target, Thing blastOwner)
          : base(pos.x, pos.y)
        {
            _target = target;
            _blastOwner = blastOwner;
            shouldbegraphicculled = false;
        }

        public DeathBeam(Vec2 pos, Vec2 target)
          : base(pos.x, pos.y)
        {
            _target = target;
            shouldbegraphicculled = false;
        }

        public override void Initialize()
        {
            Vec2 normalized1 = _target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
            Vec2 normalized2 = _target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
            Level.Add(new LaserLine(position, _target, normalized1, 4f, Color.White, 1f));
            Level.Add(new LaserLine(position, _target, normalized2, 4f, Color.White, 1f));
            Level.Add(new LaserLine(position, _target, normalized1, 2.5f, Color.White, 2f));
            Level.Add(new LaserLine(position, _target, normalized2, 2.5f, Color.White, 2f));
            if (isLocal)
            {
                int num = 0;
                Vec2 vec2 = position + normalized1 * 16f;
                for (int index = 0; index < 5; ++index)
                {
                    Vec2 p1 = vec2 + normalized2 * 8f * index;
                    foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(p1, p1 + _target))
                    {
                        if (_blastOwner != materialThing && Duck.GetAssociatedDuck(materialThing) != _blastOwner)
                        {
                            if (materialThing is IAmADuck && !materialThing.destroyed && !(materialThing is TargetDuck))
                            {
                                if (materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck != null && !(materialThing as RagdollPart).doll.captureDuck.dead)
                                    ++num;
                                if (materialThing is TrappedDuck && (materialThing as TrappedDuck).captureDuck != null && !(materialThing as TrappedDuck).captureDuck.dead)
                                    ++num;
                                if (materialThing is Duck && !(materialThing as Duck).dead)
                                    ++num;
                            }
                            SuperFondle(materialThing, DuckNetwork.localConnection);
                            materialThing.Destroy(new DTIncinerate(this));
                        }
                    }
                }
                if (num > 2)
                    Global.GiveAchievement("laser");
                Global.data.giantLaserKills += num;
            }
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogBonus();
        }

        public override void Update()
        {
            _blast = Maths.CountDown(_blast, 0.1f);
            if (_blast >= 0)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            double num1 = Maths.NormalizeSection(_blast, 0f, 0.2f);
            double num2 = Maths.NormalizeSection(_blast, 0.6f, 1f);
            Vec2 normalized1 = _target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
            Vec2 normalized2 = _target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
            Vec2 vec2 = position + normalized1 * 16f;
            for (int index = 0; index < 5; ++index)
            {
                Vec2 p1 = vec2 + normalized2 * 8f * index;
                Graphics.DrawLine(p1, p1 + _target, Color.Red * (_blast * 0.5f), 2f, (Depth)0.9f);
            }
        }
    }
}
