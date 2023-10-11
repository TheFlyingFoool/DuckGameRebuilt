using System.Linq;

namespace DuckGame
{
    //ugh
    /*public class FluidPuddleVessel : SomethingSomethingVessel
    {
        public FluidPuddleVessel(Thing th) : base(th)
        {
            AddSynncl("amount", new SomethingSync(typeof(float)));
            AddSynncl("heat", new SomethingSync(typeof(float)));
            AddSynncl("color", new SomethingSync(typeof(Vec4)));
            AddSynncl("wide", new SomethingSync(typeof(float)));
            tatchedTo.Add(typeof(FluidPuddle));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Main.SpecialCode = "rd1";
            Vec2 position = b.ReadVec2();
            Main.SpecialCode = "rd2";
            int block = b.ReadUShort() - 1;
            Block bl = null;
            if (block != -1)
            {
                Main.SpecialCode = "rd152";
                //DevConsole.Log("Blocks  " + (((ReplayLevel)Level.current).reAdd.Cast<AutoBlock>().ToList()).Count());
                    //Level.current.things[typeof(T)].Cast<T>().ToList()
                foreach (Thing t in ((ReplayLevel)Level.current).reAdd)
                {
                    if (t is AutoBlock autoBlock && autoBlock.blockIndex == block)
                    {
                        bl = autoBlock;
                        break;
                    }
                }
            }

            Main.SpecialCode = "rd1444";
            return new FluidPuddleVessel(new FluidPuddle(position.x, position.y, bl));
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(t.position);
            FluidPuddle fl = (FluidPuddle)t;

            if (fl._block != null && fl._block is AutoBlock ab) prevBuffer.Write((ushort)(ab.blockIndex + 1));
            else prevBuffer.Write((ushort)0);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            FluidPuddle fp = (FluidPuddle)t;
            fp.data.amount = (float)valOf("amount");
            fp.data.color = (Vec4)valOf("color");
            fp.data.heat = (float)valOf("heat");
            fp._wide = (float)valOf("wide");
            fp.onFire = bArray[7];
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            FluidPuddle fp = (FluidPuddle)t;
            addVal("amount", fp.data.amount);
            addVal("color", fp.data.color);
            addVal("heat", fp.data.heat);
            addVal("wide", fp._wide);
            bArray[7] = fp.onFire;
        }
    }*/
}
