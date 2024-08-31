namespace DuckGame
{
    // 01: no fullscreen, animated and playtest switch
    public class FFUI01Transition : FFUITransition
    {
        public override void Start()
        {
            // disable everything affected when animation happening
            
            MainWindow._slider.Disable = true;
            foreach (FFButton ffButton in MainWindow._topAnimControlButtons)
            {
                ffButton.Disable = true;
            }
            foreach (FFButton ffButton in MainWindow._bottomPlaytestControlButtons)
            {
                ffButton.Disable = true;
            }
        }

        public override void Update()
        {
            TransitionProgress += 1 / 20f;

            // activating:   0 -> 1
            // deactivating: 1 -> 0
            ProgressValue easedValue = Direction > 0
                ? EasingFunction(TransitionProgress)
                : 1 - EasingFunction(TransitionProgress);

            FFButton ptButton = MainWindow._playtestButton;
            FFHatPreview hatPreview = MainWindow._hatPreview;
            FFSlider slider = MainWindow._slider;
            FFButton[] topButtons = MainWindow._topAnimControlButtons;
            FFButton[] bottomButtons = MainWindow._bottomPlaytestControlButtons;
            
            // pt button swoosh
            ptButton.Bounds = new Rectangle(
                (lerp(188, 212, easedValue), ptButton.Bounds.y),
                (lerp(212, 260, easedValue), ptButton.Bounds.br.y)
            );

            // top buttons swoop, pegged to pt button
            for (int i = 0; i < topButtons.Length; i++)
            {
                FFButton ffButton = topButtons[i];
                
                ffButton.Alpha = 1 - easedValue;
                ffButton.Bounds.x = lerp(228, 276, easedValue) + (i * 16);
            }
            
            // hat preview rescaling to fit new bottom buttons
            hatPreview.Bounds.height = lerp(88, 84, easedValue);
            
            // slider swoosh out the way, y pegged to hat preview
            slider.Alpha = 1 - easedValue;
            slider.Bounds.y = hatPreview.Bounds.Bottom + 8 + (16 * easedValue);
            slider.Bounds.x = hatPreview.Bounds.Center.x - slider.Bounds.width / 2;
            
            // bottom buttons swoosh, pegged to slider
            for (int i = 0; i < bottomButtons.Length; i++)
            {
                FFButton ffButton = bottomButtons[i];
                
                ffButton.Bounds.x = hatPreview.Bounds.Center.x - 28 + (i * 16);
                ffButton.Bounds.y = slider.Bounds.y - 16;
            }
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