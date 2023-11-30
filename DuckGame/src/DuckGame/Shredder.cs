using System.Collections.Generic;

namespace DuckGame
{
    public class Shredder
    {
        private static List<Note> _basicScale = new List<Note>()
    {
      Note.c,
      Note.d,
      Note.f,
      Note.a
    };
        private static int _currentNote = 0;
        private static float _noteWait = 0f;

        public static int GetNextNote(int note, List<Note> scale)
        {
            for (int index = 0; index < scale.Count; ++index)
            {
                if (scale[index] == (Note)(_currentNote % 12))
                {
                    if (index < scale.Count - 1)
                    {
                        int num1 = 0;
                        int num2 = (int)scale[index + 1];
                        int num3 = (int)scale[index];
                        while (num3 != num2)
                        {
                            num3 = (num3 + 1) % 12;
                            ++num1;
                        }
                        note += num1;
                    }
                    else
                    {
                        int num4 = 0;
                        int num5 = (int)scale[0];
                        int num6 = (int)scale[index];
                        while (num6 != num5)
                        {
                            num6 = (num6 + 1) % 12;
                            ++num4;
                        }
                        note += num4;
                    }
                }
            }
            return note % 48;
        }

        public static int GetPrevNote(int note, List<Note> scale)
        {
            for (int index = 0; index < scale.Count; ++index)
            {
                if (scale[index] == (Note)(_currentNote % 12))
                {
                    if (index > 0)
                    {
                        int num1 = 0;
                        int num2 = (int)scale[index - 1];
                        int num3 = (int)scale[index];
                        while (num3 != num2)
                        {
                            num3 = (num3 + 1) % 12;
                            ++num1;
                        }
                        note -= 12 - num1;
                    }
                    else
                    {
                        int num4 = 0;
                        int num5 = (int)scale[scale.Count - 1];
                        int num6 = (int)scale[index];
                        while (num6 != num5)
                        {
                            num6 = (num6 + 1) % 12;
                            ++num4;
                        }
                        note -= 12 - num4;
                    }
                }
            }
            if (note < 0)
                note = 48 + note;
            return note % 48;
        }

        public static void Update()
        {
            XInputPad device = Input.GetDevice<XInputPad>();
            if (_noteWait <= 0)
            {
                if (device.MapDown(16384, false))
                {
                    _currentNote = GetNextNote(_currentNote, _basicScale);
                    SFX.Play("guitar/guitar-" + Change.ToString(_currentNote));
                    _noteWait = 1f;
                }
                if (device.MapDown(32768, false))
                {
                    _currentNote = _currentNote = (int)_basicScale[0];
                    SFX.Play("guitar/guitar-" + Change.ToString(_currentNote));
                    _noteWait = 1f;
                }
                if (device.MapDown(8192, false))
                {
                    _currentNote = GetPrevNote(_currentNote, _basicScale);
                    SFX.Play("guitar/guitar-" + Change.ToString(_currentNote));
                    _noteWait = 1f;
                }
            }
            _noteWait -= 0.15f;
        }
    }
}
