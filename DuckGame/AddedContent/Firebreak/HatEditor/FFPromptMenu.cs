using System;

namespace DuckGame
{
    // this is probably too big to finish before 1.2.0.. i'll just hardcode something for now
    // might go through with this later in some big refactor of the disasterous codebase that is FF
    public class FFPromptMenu
    {
        public string Title;
        public Button[][] ButtonColumns;

        public void Update()
        {
            
        }
        
        public void Draw()
        {
            
        }

        public class Button
        {
            public string Content;
            public Action OnPress;

            public Button(string content, Action onPress)
            {
                Content = content;
                OnPress = onPress;
            }
        }
    }
}