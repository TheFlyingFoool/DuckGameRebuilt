using System.Collections;

namespace DuckGame
{
    public class HoldableVessel : SomethingSomethingVessel
    {
        public HoldableVessel(Thing th) : base(th)
        {
            AddSynncl("position", new SomethingSync(typeof(Vec2)));
            AddSynncl("infoed_h", new SomethingSync(typeof(byte)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            HoldableVessel v = new HoldableVessel(Editor.CreateThing(Editor.IDToType[b.ReadUShort()]));
            v.t.y = -2000;
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(Editor.IDToType[t.GetType()]);
            return prevBuffer;
        }
        public ItemSpawner lastHover;
        public override void PlaybackUpdate()
        {
            Holdable h = (Holdable)t;

            if (lastHover == null && h.duck == null) h.position = (Vec2)valOf("position");
            lastHover = h.hoverSpawner;
            if (syncled.ContainsKey("infoed_h"))
            {
                BitArray br = new BitArray(new byte[] { (byte)valOf("infoed_h") });
                h.offDir = (sbyte)(br[0] ? 1 : -1);
                h.visible = br[1];
                h.grounded = br[2];
                h.solid = br[3];
                h.sleeping = br[4];
                //h.offDir = (sbyte)(bArray[7] ? 1 : -1);
            }
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Holdable h = (Holdable)t;
            if ((lastHover == null && h.duck == null) || exFrames == 0) addVal("position", h.position);
            lastHover = h.hoverSpawner;
            BitArray br = new BitArray(8);
            br[0] = h.offDir > 0;
            br[1] = h.visible;
            br[2] = h.grounded;
            br[3] = h.solid;
            br[4] = h.sleeping;
            addVal("infoed_h", BitCrusher.BitArrayToByte(br));
            //bArray[7] = h.offDir > 0;
        }
    }
}
