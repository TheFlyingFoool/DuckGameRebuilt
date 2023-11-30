namespace DuckGame
{
    public class DuckStory
    {
        public string text = "";
        public NewsSection section;

        public event OnStoryBeginDelegate OnStoryBegin;

        public void DoCallback()
        {
            if (OnStoryBegin == null)
                return;
            OnStoryBegin(this);
        }

        public delegate void OnStoryBeginDelegate(DuckStory story);
    }
}
