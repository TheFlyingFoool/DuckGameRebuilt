using System.Collections;

namespace DuckGame
{
    public class WindowVessel : SomethingSomethingVessel
    {
        public WindowVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Window));
            tatchedTo.Add(typeof(FloorWindow));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 v = b.ReadVec2();
            byte bot = b.ReadByte();
            byte heig = b.ReadByte();
            BitArray arr = new BitArray(new byte[] { bot });
            int z = 0;
            int divide = 32;
            for (int i = 0; i < 6; i++)
            {
                if (arr[i]) z += divide;
                divide /= 2;
            }
            Window w;
            if (arr[6]) w = new FloorWindow(v.x, v.y);
            else w = new Window(v.x, v.y);
            w.tint.value = z;
            w.bars.value = arr[7];
            w.windowHeight.value = heig;
            w.UpdateHeight();
            WindowVessel v2 = new WindowVessel(w);
            return v2;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(t.position);
            BitArray b = new BitArray(8);
            Window w = (Window)t;
            int tint = w.tint.value;
            b[0] = (tint & 32) > 0;
            b[1] = (tint & 16) > 0;
            b[2] = (tint & 8) > 0;
            b[3] = (tint & 4) > 0;
            b[4] = (tint & 2) > 0;
            b[5] = (tint & 1) > 0;
            b[6] = w.floor;
            b[7] = w.bars.value;
            prevBuffer.Write(Extensions.BitArrayToByte(b));
            prevBuffer.Write((byte)w.windowHeight.value);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Window w = (Window)t;
            //BitArray br = new BitArray((byte)valOf("shake/death"))
            if (bArray[0]) Extensions.SetPrivateFieldValue(w, "_shakeVal", 3.14f);
            Extensions.SetPrivateFieldValue(w, "_hasGlass", bArray[1]);
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Window w = (Window)t;


            //lol, so basically each something something vessel has a constant stream of at least 1 byte
            //that is used to optimize variables, its best use right now is on the DuckVessel because
            //it used to be at minimum 8 bytes per frame but now its 1 byte per frame (at its best)
            //and here i dind't add any variables but instead use the stream to store single booleans
            //so, win
            if (Extensions.GetPrivateFieldValue<float>(w, "_shakeVal") > 3.08f) bArray[0] = true;
            bArray[1] = Extensions.GetPrivateFieldValue<bool>(w, "_hasGlass");

            //addVal("shake/death", Extensions.GetPrivateFieldValue<float>(w, "_shakeVal"));
        }
    }
}