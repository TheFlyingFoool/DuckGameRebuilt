using System.Collections.Generic;

namespace DuckGame
{
    public class TextLine
    {
        public List<TextSegment> segments = new List<TextSegment>();
        public Color lineColor = Color.White;

        public string text
        {
            get
            {
                string text = "";
                foreach (TextSegment segment in segments)
                    text += segment.text;
                return text;
            }
        }

        public void Add(char letter)
        {
            if (segments.Count == 0)
                segments.Add(new TextSegment()
                {
                    color = lineColor
                });
            segments[0].text += letter.ToString();
        }

        public void Add(string val)
        {
            if (segments.Count == 0)
                segments.Add(new TextSegment()
                {
                    color = lineColor
                });
            segments[0].text += val;
        }

        public void SwitchColor(Color c)
        {
            lineColor = c;
            if (segments.Count > 0 && segments[segments.Count - 1].text.Length == 0)
                segments[segments.Count - 1].color = c;
            else
                segments.Insert(0, new TextSegment()
                {
                    color = c
                });
        }

        public int Length()
        {
            int num1 = 0;
            foreach (TextSegment segment in segments)
            {
                int num2 = 0;
                for (int index = 0; index < segment.text.Length; ++index)
                {
                    if (segment.text[index] == '@')
                    {
                        ++index;
                        while (index < segment.text.Length && segment.text[index] != '@') // added index < segment.text.Length to prevent crash
                        {
                            ++index;
                        }
                    }
                    else
                        ++num2;
                }
                num1 += num2;
            }
            return num1;
        }
    }
}
