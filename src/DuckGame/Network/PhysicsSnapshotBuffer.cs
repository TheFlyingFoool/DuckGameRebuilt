// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsSnapshotBuffer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class PhysicsSnapshotBuffer
    {
        private PhysicsSnapshotObject[] _frames;
        private int _curFrame;
        private int _numFrames = 120;
        private int _storedFrames;

        public void StoreFrame(PhysicsSnapshotObject frame)
        {
            if (this._frames == null)
                this._frames = new PhysicsSnapshotObject[this._numFrames];
            this._frames[this._curFrame] = frame;
            this._curFrame = (this._curFrame + 1) % _frames.Count<PhysicsSnapshotObject>();
            ++this._storedFrames;
            if (this._storedFrames <= this._numFrames)
                return;
            this._storedFrames = this._numFrames;
        }

        public PhysicsSnapshotObject GetLatestFrame()
        {
            int index = this._curFrame - 1;
            if (index < 0)
                index += this._storedFrames;
            return this._frames[index];
        }

        public int GetIndex(PhysicsSnapshotObject frame)
        {
            for (int index = 0; index < _frames.Count<PhysicsSnapshotObject>(); ++index)
            {
                if (frame == this._frames[index])
                    return index;
            }
            return 0;
        }

        public PhysicsSnapshotObject GetFrame(double time)
        {
            if (this._frames == null)
                this._frames = new PhysicsSnapshotObject[this._numFrames];
            int index1 = this._curFrame;
            int index2 = 0;
            double num1 = 99999.8984375;
            do
            {
                if (index1 > this._storedFrames)
                    index1 = 0;
                if (this._frames[index1] != null)
                {
                    double num2 = time - this._frames[index1].clientTime;
                    if (num2 < num1 && num2 > 0.0)
                    {
                        index2 = index1;
                        num1 = this._frames[index1].clientTime;
                    }
                    else if (num2 > num1)
                        return this._frames[index2];
                    index1 = (index1 + 1) % this._storedFrames;
                }
                else
                    break;
            }
            while (index1 != this._curFrame);
            return this._frames[index2];
        }

        public void FillGap(PhysicsSnapshotObject first, PhysicsSnapshotObject last)
        {
            PhysicsSnapshotObject[] physicsSnapshotObjectArray = new PhysicsSnapshotObject[this._numFrames];
            PhysicsSnapshotObject latestFrame = this.GetLatestFrame();
            int index = 0;
            while (true)
            {
                physicsSnapshotObjectArray[index] = last;
                if (last != first)
                    last = this.GetNextFrame(last);
                else
                    break;
            }
            this._frames = physicsSnapshotObjectArray;
            this._curFrame = this.GetIndex(latestFrame);
            this._storedFrames = index;
        }

        public PhysicsSnapshotObject GetFrame(PhysicsSnapshotObject reference)
        {
            if (this._frames == null)
                this._frames = new PhysicsSnapshotObject[this._numFrames];
            int index1 = this._curFrame;
            int index2 = 0;
            double num1 = 99999.8984375;
            do
            {
                if (index1 > this._storedFrames)
                    index1 = 0;
                if (this._frames[index1] != null)
                {
                    double num2 = reference.serverTime - this._frames[index1].clientTime;
                    if (num2 < num1 && num2 > 0.0)
                    {
                        index2 = index1;
                        num1 = this._frames[index1].clientTime;
                    }
                    else if (num2 > num1 && this._frames[index2].position == reference.position && this._frames[index2].velocity == reference.velocity)
                        return this._frames[index2];
                    index1 = (index1 + 1) % this._storedFrames;
                }
                else
                    break;
            }
            while (index1 != this._curFrame);
            for (int index3 = 0; index3 < 8; ++index3)
            {
                PhysicsSnapshotObject previousFrame = this.GetPreviousFrame(this._frames[index2]);
                if (previousFrame.position == reference.position && previousFrame.velocity == reference.velocity)
                    return previousFrame;
            }
            for (int index4 = 0; index4 < 8; ++index4)
            {
                PhysicsSnapshotObject nextFrame = this.GetNextFrame(this._frames[index2]);
                if (nextFrame.position == reference.position && nextFrame.velocity == reference.velocity)
                    return nextFrame;
            }
            return this._frames[index2];
        }

        public PhysicsSnapshotObject GetNextFrame(PhysicsSnapshotObject frame)
        {
            if (this._frames == null)
                this._frames = new PhysicsSnapshotObject[this._numFrames];
            int num = this._curFrame;
            for (int index = 0; index < this._numFrames; ++index)
            {
                if (this._frames[index] == frame)
                {
                    num = index;
                    break;
                }
            }
            int index1 = (num + 1) % this._storedFrames;
            return this._frames[index1] != null ? this._frames[index1] : frame;
        }

        public PhysicsSnapshotObject GetPreviousFrame(PhysicsSnapshotObject frame)
        {
            if (this._frames == null)
                this._frames = new PhysicsSnapshotObject[this._numFrames];
            int num = this._curFrame;
            for (int index = 0; index < this._numFrames; ++index)
            {
                if (this._frames[index] == frame)
                {
                    num = index;
                    break;
                }
            }
            int index1 = num - 1;
            if (index1 < 0)
                index1 += this._storedFrames;
            return this._frames[index1] != null ? this._frames[index1] : frame;
        }
    }
}
