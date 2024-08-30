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
            ptButton.Bounds.x = lerp(hatPreview.Bounds.Center.x - ptButton.Bounds.width / 2, hatPreview.Bounds.Center.x - 4, easedValue);
            ptButton.Bounds.y = lerp(36, 144, easedValue);
            
            hatPreview.Bounds.height = lerp(88, 92, easedValue);
            hatPreview.Bounds = new Rectangle(
                Vec2.Lerp((188, 52), (32, 36), easedValue),
                hatPreview.Bounds.br
            );
            
            metapixelList.Bounds.x = hatPreview.Bounds.x - (metapixelList.Bounds.width + 12);
            metapixelList.Alpha = 1 - easedValue;
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