// Decompiled with JetBrains decompiler
// Type: DuckGame.EnergyScimitarBlast
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EnergyScimitarBlast : Thing
    {
        public float _blast = 1f;
        public Vec2 _target;

        public EnergyScimitarBlast(Vec2 pos, Vec2 target)
          : base(pos.x, pos.y)
        {
            this._target = target;
        }

        public override void Initialize()
        {
            SFX.Play("laserBlast");
            Vec2 normalized1 = this._target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
            Vec2 normalized2 = this._target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
            Level.Add(new LaserLine(this.position, this._target, normalized1, 4f, Color.White, 1f));
            Level.Add(new LaserLine(this.position, this._target, normalized2, 4f, Color.White, 1f));
            Level.Add(new LaserLine(this.position, this._target, normalized1, 2.5f, Color.White, 2f));
            Level.Add(new LaserLine(this.position, this._target, normalized2, 2.5f, Color.White, 2f));
            if (this.isLocal)
            {
                int num = 0;
                Vec2 vec2 = this.position + normalized1 * 16f;
                for (int index = 0; index < 5; ++index)
                {
                    Vec2 p1 = vec2 + normalized2 * 8f * index;
                    foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(p1, p1 + this._target))
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
                        materialThing.Destroy(new DTIncinerate(this));
                    }
                }
                Global.data.energyScimitarBlastKills += num;
            }
            if (Recorder.currentRecording == null)
                return;
            Recorder.currentRecording.LogBonus();
        }

        public override void Update()
        {
            this._blast = Maths.CountDown(this._blast, 0.1f);
            if (_blast >= 0.0)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            double num1 = (double)Maths.NormalizeSection(this._blast, 0f, 0.2f);
            double num2 = (double)Maths.NormalizeSection(this._blast, 0.6f, 1f);
            Vec2 normalized1 = this._target.Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
            Vec2 normalized2 = this._target.Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
            Vec2 vec2 = this.position + normalized1 * 16f;
            for (int index = 0; index < 5; ++index)
            {
                Vec2 p1 = vec2 + normalized2 * 8f * index;
                Graphics.DrawLine(p1, p1 + this._target, Color.LightBlue * (this._blast * 0.5f), 2f, (Depth)0.9f);
            }
        }
    }
}
