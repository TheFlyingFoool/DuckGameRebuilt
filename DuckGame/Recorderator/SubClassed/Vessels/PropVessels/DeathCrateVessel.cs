using NAudio.MediaFoundation;
using System.Collections;

namespace DuckGame
{
    public class DeathCrateVessel : HoldableVessel
    {
        public DeathCrateVessel(Thing th) : base(th)
        {
            RemoveSynncl("infoed_h");
            AddSynncl("frame", new SomethingSync(typeof(byte)));
            tatchedTo.Add(typeof(DeathCrate));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            DeathCrateVessel v = new DeathCrateVessel(new DeathCrate(0, -2000));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            DeathCrate d = (DeathCrate)t;
            BitArray br = new BitArray(new byte[] { (byte)valOf("frame") });
            int b = 0;
            int divide = 8;
            for (int i = 0; i < 4; i++)
            {
                if (br[i]) b += divide;
                divide /= 2;
            }
            d.offDir = (sbyte)(br[4] ? 1 : -1);
            d.visible = br[5];
            d.grounded = br[6];
            d.sleeping = br[7];
            ((SpriteMap)d.graphic).ClearAnimations();
            ((SpriteMap)d.graphic).imageIndex = b;
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            DeathCrate d = (DeathCrate)t;
            int z = ((SpriteMap)d.graphic).imageIndex;

            BitArray br = new BitArray(8);
            br[0] = (z & 8) > 0;
            br[1] = (z & 4) > 0;
            br[2] = (z & 2) > 0;
            br[3] = (z & 1) > 0;
            br[4] = d.offDir > 0;
            br[5] = d.visible;
            br[6] = d.grounded;
            br[7] = d.sleeping;
            addVal("frame", BitCrusher.BitArrayToByte(br));
            /*h.offDir = (sbyte)(br[0] ? 1 : -1);
            h.visible = br[1];
            h.grounded = br[2];
            h.solid = br[3];
            h.sleeping = br[4];*/

            base.RecordUpdate();
        }
    }
}
