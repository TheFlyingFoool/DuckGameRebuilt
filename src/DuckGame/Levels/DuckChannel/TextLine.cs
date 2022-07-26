// Decompiled with JetBrains decompiler
// Type: DuckGame.TextLine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                foreach (TextSegment segment in this.segments)
                    text += segment.text;
                return text;
            }
        }

        public void Add(char letter)
        {
            if (this.segments.Count == 0)
                this.segments.Add(new TextSegment()
                {
                    color = this.lineColor
                });
            this.segments[0].text += letter.ToString();
        }

        public void Add(string val)
        {
            if (this.segments.Count == 0)
                this.segments.Add(new TextSegment()
                {
                    color = this.lineColor
                });
            this.segments[0].text += val;
        }

        public void SwitchColor(Color c)
        {
            this.lineColor = c;
            if (this.segments.Count > 0 && this.segments[this.segments.Count - 1].text.Length == 0)
                this.segments[this.segments.Count - 1].color = c;
            else
                this.segments.Insert(0, new TextSegment()
                {
                    color = c
                });
        }

        public int Length()
        {
            int num1 = 0;
            foreach (TextSegment segment in this.segments)
            {
                int num2 = 0;
                for (int index = 0; index < segment.text.Length; ++index)
                {
                    if (segment.text[index] == '@')
                    {
                        ++index;
                        while (segment.text[index] != '@')
                            ++index;
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
