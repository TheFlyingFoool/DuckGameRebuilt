using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class RCControllerVessel : GunVessel
    {
        public RCControllerVessel(Thing t) : base(t)
        {
            tatchedTo.Add(typeof(RCController));
            AddSynncl("car", new SomethingSync(typeof(ushort)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            return new RCControllerVessel(new RCController(0, 0, null));
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            RCController rcd = (RCController)t;
            int car = (ushort)valOf("car") - 1; 
            
            if (car != -1 && Corderator.instance.somethingMap.Contains(car))
            {
                rcd._car = (RCCar)Corderator.instance.somethingMap[car];
            }

            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            RCController rcd = (RCController)t;
            if (rcd._car != null && Corderator.instance != null && Corderator.instance.somethingMap.Contains(rcd._car)) addVal("car", (ushort)(Corderator.instance.somethingMap[rcd._car] + 1));
            else addVal("car", (ushort)0);
            base.RecordUpdate();
        }
    }
}
