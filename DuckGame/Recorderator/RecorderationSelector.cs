using System.IO;

namespace DuckGame
{
    // the gui selector for now, just cus using `playreplay <index>` is too inconvenient
    public class RecorderationSelector : Level
    {
        public string[] Recorderations;
        private int scrollIndex = 0;
        
        public override void Initialize()
        {
            backgroundColor = Color.Black;
            Recorderations = DuckFile.GetFiles(Corderator.CordsPath, "*.rdt", SearchOption.AllDirectories);
            base.Initialize();
        }

        public override void Update()
        {
            if (Mouse.scrollingUp || Keyboard.Pressed(Keys.PageUp))
                scrollIndex++;
            else if (Mouse.scrollingDown || Keyboard.Pressed(Keys.PageDown))
                scrollIndex--;

            base.Update();
        }

        public override void Draw()
        {
            const float fontSize = 0.6f;
            FeatherFashion.DrawCursor(Mouse.positionScreen);

            for (int i = Recorderations.Length - 1 + scrollIndex; i >= 0 + scrollIndex; i--)
            {
                int replayIndex = i - scrollIndex;
                FileInfo fileInfo = new(Recorderations[replayIndex]);

                // TODO hi niko i dont know how your things work could you do these
                float matchLengthSeconds = 0;
                string mapName = "LEVEL";
                
                string cordDisplayName = $"{fileInfo.CreationTime:ddd yyyy-MM-dd hh:mm:ss} [{mapName}] [{matchLengthSeconds}s]";
                Vec2 stringSize = Extensions.GetFancyStringSize(cordDisplayName, fontSize);
                Rectangle textBounds = new(1, 1 + (stringSize.y + 1) * i, stringSize.x, stringSize.y);

                bool hovered = textBounds.Contains(Mouse.positionScreen);
                Color textColor = hovered ? Color.Yellow : Color.White;

                if (hovered && Mouse.left == InputState.Pressed)
                {
                    Recorderator.PlayReplay(replayIndex);
                    return;
                }
                
                Graphics.DrawFancyString(cordDisplayName, textBounds.tl, textColor, 1f, fontSize);
            }
        }
    }
}