namespace DuckGame
{
    // 03: fullscreen active, animated-playlist transition
    public class FFUI03Transition : FFUITransition
    {
        public override void Start()
        {
            MainWindow._slider.Disable = true;
            foreach (FFButton ffButton in MainWindow._topAnimControlButtons)
            {
                ffButton.Disable = true;
            }
            foreach (FFButton ffButton in MainWindow._bottomPlaytestControlButtons)
            {
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
            FFHatPreview hatPreview = MainWindow._hatPreview;
            FFMetapixelList metapixelList = MainWindow._metapixelList;
            FFSlider slider = MainWindow._slider;
            FFButton[] topButtons = MainWindow._topAnimControlButtons;
            FFButton[] bottomButtons = MainWindow._bottomPlaytestControlButtons;
            FFButton fsButton = bottomButtons[3];
            
            ptButton.Bounds.width = lerp(24, 48, easedValue);
            ptButton.Bounds.x = lerp(188, 154, easedValue);
            ptButton.Bounds.y = lerp(36, 144, easedValue);
            
            for (int i = 0; i < topButtons.Length; i++)
            {
                FFButton ffButton = topButtons[i];
                
                ffButton.Alpha = 1 - easedValue;
                ffButton.Bounds.x = lerp(228, 276, easedValue) + (i * 16);
            }
            
            hatPreview.Bounds = new Rectangle(
                lerp((188, 52), (32, 36), easedValue),
                hatPreview.Bounds.br
            );
            hatPreview.Bounds.height = lerp(88, 100, easedValue);
            
            metapixelList.Bounds.x = hatPreview.Bounds.x - (metapixelList.Bounds.width + 12);
            metapixelList.Alpha = 1 - easedValue;
            
            slider.Alpha = 1 - easedValue;
            slider.Bounds.y = hatPreview.Bounds.Bottom + 8 + (16 * easedValue);
            slider.Bounds.x = hatPreview.Bounds.Center.x - slider.Bounds.width / 2;
            
            for (int i = 0; i < bottomButtons.Length; i++)
            {
                FFButton ffButton = bottomButtons[i];
                
                ffButton.Bounds.y = slider.Bounds.y - 16;
                if (ffButton != fsButton)
                    ffButton.Bounds.x = lerp(208, ptButton.Bounds.x - (48 + 8), easedValue) + (16 * i);
            }
            
            fsButton.Bounds.x = lerp(256, ptButton.Bounds.Right + 8, easedValue);
        }

        public override void End()
        {
            // if activating playtest mode, disable slider and top buttons
            // and enable bottom buttons
            
            bool anim01 = Direction > 0;
            
            MainWindow._slider.Disable = anim01;
            foreach (FFButton ffButton in MainWindow._topAnimControlButtons)
            {
                ffButton.Disable = anim01;
            }
            foreach (FFButton ffButton in MainWindow._bottomPlaytestControlButtons)
            {
                ffButton.Disable = !anim01;
            }
        }
    }
}