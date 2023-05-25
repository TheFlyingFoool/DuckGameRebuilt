// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsSnapshotBuffer
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (_frames == null)
                _frames = new PhysicsSnapshotObject[_numFrames];
            _frames[_curFrame] = frame;
            _curFrame = (_curFrame + 1) % _frames.Length;
            ++_storedFrames;
            if (_storedFrames <= _numFrames)
                return;
            _storedFrames = _numFrames;
        }

        public PhysicsSnapshotObject GetLatestFrame()
        {
            int index = _curFrame - 1;
            if (index < 0)
                index += _storedFrames;
            return _frames[index];
        }

        public int GetIndex(PhysicsSnapshotObject frame)
        {
            for (int index = 0; index < _frames.Length; ++index)
            {
                if (frame == _frames[index])
                    return index;
            }
            return 0;
        }

        public PhysicsSnapshotObject GetFrame(double time)
        {
            if (_frames == null)
                _frames = new PhysicsSnapshotObject[_numFrames];
            int index1 = _curFrame;
            int index2 = 0;
            double num1 = 99999.8984375;
            do
            {
                if (index1 > _storedFrames)
                    index1 = 0;
                if (_frames[index1] != null)
                {
                    double num2 = time - _frames[index1].clientTime;
                    if (num2 < num1 && num2 > 0)
                    {
                        index2 = index1;
                        num1 = _frames[index1].clientTime;
                    }
                    else if (num2 > num1)
                        return _frames[index2];
                    index1 = (index1 + 1) % _storedFrames;
                }
                else
                    break;
            }
            while (index1 != _curFrame);
            return _frames[index2];
        }

        public void FillGap(PhysicsSnapshotObject first, PhysicsSnapshotObject last)
        {
            PhysicsSnapshotObject[] physicsSnapshotObjectArray = new PhysicsSnapshotObject[_numFrames];
            PhysicsSnapshotObject latestFrame = GetLatestFrame();
            int index = 0;
            while (true)
            {
                physicsSnapshotObjectArray[index] = last;
                if (last != first)
                    last = GetNextFrame(last);
                else
                    break;
            }
            _frames = physicsSnapshotObjectArray;
            _curFrame = GetIndex(latestFrame);
            _storedFrames = index;
        }

        public PhysicsSnapshotObject GetFrame(PhysicsSnapshotObject reference)
        {
            if (_frames == null)
                _frames = new PhysicsSnapshotObject[_numFrames];
            int index1 = _curFrame;
            int index2 = 0;
            double num1 = 99999.8984375;
            do
            {
                if (index1 > _storedFrames)
                    index1 = 0;
                if (_frames[index1] != null)
                {
                    double num2 = reference.serverTime - _frames[index1].clientTime;
                    if (num2 < num1 && num2 > 0)
                    {
                        index2 = index1;
                        num1 = _frames[index1].clientTime;
                    }
                    else if (num2 > num1 && _frames[index2].position == reference.position && _frames[index2].velocity == reference.velocity)
                        return _frames[index2];
                    index1 = (index1 + 1) % _storedFrames;
                }
                else
                    break;
            }
            while (index1 != _curFrame);
            for (int index3 = 0; index3 < 8; ++index3)
            {
                PhysicsSnapshotObject previousFrame = GetPreviousFrame(_frames[index2]);
                if (previousFrame.position == reference.position && previousFrame.velocity == reference.velocity)
                    return previousFrame;
            }
            for (int index4 = 0; index4 < 8; ++index4)
            {
                PhysicsSnapshotObject nextFrame = GetNextFrame(_frames[index2]);
                if (nextFrame.position == reference.position && nextFrame.velocity == reference.velocity)
                    return nextFrame;
            }
            return _frames[index2];
        }

        public PhysicsSnapshotObject GetNextFrame(PhysicsSnapshotObject frame)
        {
            if (_frames == null)
                _frames = new PhysicsSnapshotObject[_numFrames];
            int num = _curFrame;
            for (int index = 0; index < _numFrames; ++index)
            {
                if (_frames[index] == frame)
                {
                    num = index;
                    break;
                }
            }
            int index1 = (num + 1) % _storedFrames;
            return _frames[index1] != null ? _frames[index1] : frame;
        }

        public PhysicsSnapshotObject GetPreviousFrame(PhysicsSnapshotObject frame)
        {
            if (_frames == null)
                _frames = new PhysicsSnapshotObject[_numFrames];
            int num = _curFrame;
            for (int index = 0; index < _numFrames; ++index)
            {
                if (_frames[index] == frame)
                {
                    num = index;
                    break;
                }
            }
            int index1 = num - 1;
            if (index1 < 0)
                index1 += _storedFrames;
            return _frames[index1] != null ? _frames[index1] : frame;
        }
    }
}
