// Decompiled with JetBrains decompiler
// Type: DuckGame.Highlights
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Highlights
    {
        private static List<Recording> _recordings = new List<Recording>();
        public static float highlightRatingMultiplier = 1f;

        public static void Initialize()
        {
            MonoMain.NloadMessage = "Loading Highlights";
            for (int index = 0; index < 6; ++index)
                _recordings.Add(new Recording());
        }

        public static List<Recording> GetHighlights()
        {
            List<Recording> highlights = new List<Recording>();
            foreach (Recording recording in _recordings)
            {
                if (Recorder.currentRecording != recording && highlights.Count < 6)
                {
                    recording.Rewind();
                    highlights.Add(recording);
                }
            }
            return highlights;
        }

        public static void ClearHighlights()
        {
            foreach (Recording recording in _recordings)
                recording.Reset();
        }

        public static void FinishRound()
        {
            if (Network.isActive) return;
            if (_recordings.Count == 0) Initialize();
            highlightRatingMultiplier = 1f;
            Recording currentRecording = Recorder.currentRecording;
            Recorder.currentRecording = null;
            if (currentRecording == null) return;
            float num = 0f;
            float lastMatchLength = Stats.lastMatchLength;
            currentRecording.Rewind();
            while (!currentRecording.StepForward()) num += currentRecording.GetFrameTotal();
            if (lastMatchLength < 5f) num *= 1.3f;
            if (lastMatchLength < 7f) num *= 1.2f;
            if (lastMatchLength < 10f) num *= 1.1f;
            currentRecording.highlightScore = num;
        }

        public static void StartRound()
        {
            if (Network.isActive) return;
            if (_recordings.Count == 0) Initialize();
            Recording recording1 = _recordings[0];
            foreach (Recording recording2 in _recordings)
            {
                if (recording2.startFrame == recording2.endFrame)
                {
                    recording1 = recording2;
                    break;
                }
                if (recording2.highlightScore < recording1.highlightScore) recording1 = recording2;
                if (recording2.highlightScore == recording1.highlightScore && Rando.Float(1f) >= 0f) recording1 = recording2;
            }
            recording1.Reset();
            Recorder.currentRecording = recording1;
        }
    }
}
