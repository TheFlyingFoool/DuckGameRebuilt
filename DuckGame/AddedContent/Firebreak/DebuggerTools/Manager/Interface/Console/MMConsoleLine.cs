using System;
using System.Text;

namespace DuckGame.ConsoleInterface
{
    public class MMConsoleLine
    {
        public string Text;
        public Significance LineSignificance;
        public DateTime CreationTime;

        public MMConsoleLine(string text, Significance significance)
        {
            Text = text;
            LineSignificance = significance;
            CreationTime = DateTime.Now;
        }

        public enum Significance
        {
            Neutral,
            User,
            Response,
            Highlight,
            Error,
            VeryFuckingImportant
        }
    }
}