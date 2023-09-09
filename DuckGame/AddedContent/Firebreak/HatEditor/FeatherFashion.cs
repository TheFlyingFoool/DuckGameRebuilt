using System;

namespace DuckGame
{
    public partial class FeatherFashion : Level
    {
        public WorkMode CurrentWorkMode = WorkMode.Editor;
        
        [PostInitialize]
        public static void StaticInitialize()
        {
            FFIcons.Initialize();
        }

        public override void Initialize()
        {
            backgroundColor = FFColors.Background;

            for (int i = 0; i < FFEditorPane.Metapixels.Length; i++)
            {
                FFEditorPane.Metapixels[i] = Color.Random();
            }
            
            base.Initialize();
        }

        public override void Update()
        {
            if (Keyboard.Pressed(Keys.Tab))
            {
                CurrentWorkMode += 1;
                CurrentWorkMode = (WorkMode) ((int)CurrentWorkMode % Enum.GetNames(typeof(WorkMode)).Length);
                switch (CurrentWorkMode)
                {
                    case WorkMode.Editor:
                        FFEditorPane.OnSwitch();
                        break;

                    case WorkMode.Preview:
                        FFPreviewPane.OnSwitch();
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                return;
            }
            
            switch (CurrentWorkMode)
            {
                case WorkMode.Editor:
                    FFEditorPane.Update();
                    break;

                case WorkMode.Preview:
                    FFPreviewPane.Update();
                    break;

                default: throw new InvalidOperationException();
            }
            
            base.Update();
        }

        public override void Draw()
        {
            switch (CurrentWorkMode)
            {
                case WorkMode.Editor:
                    FFEditorPane.Draw();
                    break;

                case WorkMode.Preview:
                    FFPreviewPane.Draw();
                    break;

                default: throw new InvalidOperationException();
            }
            
            base.Draw();
        }
    }
}