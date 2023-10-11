namespace DuckGame
{
    public class DartVessel : SomethingSomethingVessel
    {
        public DartVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Dart));
            AddSynncl("position", new SomethingSync(typeof(int)));
            AddSynncl("angle", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            DartVessel v = new DartVessel(new Dart(0, -2000, null, 0));
            v.faded = b.ReadBool();
            if (v.faded)
            {
                v.fadedPos = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 10000);
                v.fadedVel = CompressedVec2Binding.GetUncompressedVec2(b.ReadInt(), 20);
            }
            return v;
        }
        public bool faded;
        public Vec2 fadedPos;
        public Vec2 fadedVel;
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            Dart d = (Dart)t;
            prevBuffer.Write(d.faded);
            if (d.faded)
            {
                prevBuffer.Write(CompressedVec2Binding.GetCompressedVec2(d.fadedPos, 10000));
                prevBuffer.Write(CompressedVec2Binding.GetCompressedVec2(d.fadedVel, 20));
            }
            return base.RecSerialize(prevBuffer);
        }
        public override void OnRemove()
        {
            Dart d = (Dart)t;
            if (faded)
            {
                DartShell dartShell = new DartShell(fadedPos.x, fadedPos.y, -d.sprite.flipMultH * Rando.Float(0.6f), d.sprite.flipH)
                {
                    angle = d.angle
                };
                dartShell.velocity = fadedVel;
                Level.Add(dartShell);
            }
            else
            {
                DartShell dartShell = new DartShell(d.x, d.y, Rando.Float(0.1f) * -d.sprite.flipMultH, d.sprite.flipH)
                {
                    angle = angle
                };
                Level.Add(dartShell);
                dartShell.hSpeed = (float)((0.5 + Rando.Float(0.3f)) * -d.sprite.flipMultH);
            }
            base.OnRemove();
        }
        public override void PlaybackUpdate()
        {
            Dart d = (Dart)t;
            d.position = CompressedVec2Binding.GetUncompressedVec2((int)valOf("position"), 10000);
            d.angleDegrees = BitCrusher.UShortToFloat((ushort)valOf("angle"), 720) - 360;
            d._stuck = bArray[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Dart d = (Dart)t;
            addVal("position", CompressedVec2Binding.GetCompressedVec2(d.position, 10000));
            addVal("angle", BitCrusher.FloatToUShort(d.angleDegrees % 360 + 360, 720));
            bArray[7] = d._stuck;
        }
    }
}
