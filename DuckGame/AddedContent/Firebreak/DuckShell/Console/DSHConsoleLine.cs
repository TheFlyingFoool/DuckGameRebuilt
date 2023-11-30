using System;

namespace DuckGame.ConsoleInterface
{
    public class DSHConsoleLine
    {
        public string Text;
        public Significance LineSignificance;
        public DateTime CreationTime;

        public DSHConsoleLine(string text, Significance significance)
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