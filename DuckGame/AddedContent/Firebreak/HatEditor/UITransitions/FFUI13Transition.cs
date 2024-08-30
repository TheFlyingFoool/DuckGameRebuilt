namespace DuckGame
{
    
    // 13: playtest active, fullscreen-nofullscreen animation
    public class FFUI13Transition : FFUITransition
    {
        public override void Start()
        {
            // disable everything affected when animation happening
            
            MainWindow._playtestButton.Disable = true;
            for (int i = 0; i < MainWindow._bottomPlaytestControlButtons.Length - 1; i++)
            {
                FFButton ffButton = MainWindow._bottomPlaytestControlButtons[i];
                
                ffButton.Disable = true;
            }
            
            // todo: disable metapixel list
        }

        public override void Update()
        {
            TransitionProgress += 1 / 20f;

            ProgressValue easedValue = Direction > 0
                ? EasingFunction(TransitionProgress)
                : 1 - EasingFunction(TransitionProgress);

            FFButton ptButton = MainWindow._playtestButton;
            FFButton fsButton = MainWindow._bottomPlaytestControlButtons[3]; // prob bad but meh
            FFHatPreview hatPreview = MainWindow._hatPreview;
            FFMetapixelList metapixelList = MainWindow._metapixelList;

            hatPreview.Bounds = new Rectangle(
                Vec2.Lerp((188, 52), (32, 36), easedValue),
                hatPreview.Bounds.br
            );

            ptButton.Bounds.x = lerp(hatPreview.Bounds.Center.x - ptButton.Bounds.width / 2, hatPreview.Bounds.Center.x - 4, easedValue);
            ptButton.Bounds.y = lerp(36, 144, easedValue);
            
            fsButton.Bounds.x = lerp(256, ptButton.Bounds.Right + 8, easedValue);

            for (int i = 0; i < MainWindow._bottomPlaytestControlButtons.Length - 1; i++)
            {
                FFButton button = MainWindow._bottomPlaytestControlButtons[i];
                button.Bounds.x = lerp(208, ptButton.Bounds.x - (48 + 8), easedValue) + (16 * i);
            }

            metapixelList.Bounds.x = hatPreview.Bounds.x - (metapixelList.Bounds.width + 12);
            metapixelList.Alpha = 1 - easedValue;
        }

        public override void End()
        {
            MainWindow._playtestButton.Disable = false;
            for (int i = 0; i < MainWindow._bottomPlaytestControlButtons.Length - 1; i++)
            {
                FFButton ffButton = MainWindow._bottomPlaytestControlButtons[i];
                
                ffButton.Disable = false;
            }
        }
    }
}