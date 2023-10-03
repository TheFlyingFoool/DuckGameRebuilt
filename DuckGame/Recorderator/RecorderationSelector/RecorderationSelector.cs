using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace DuckGame
{
    public class RecorderationSelector : Level
    {
        public static new RecorderationSelector current => (RecorderationSelector) Level.current;
        public static string CordsPath => Corderator.CordsPath;

        public int SelectedItemIndex
        {
            get => MenuItems.Count == 0 ? -1 : _selectedItemIndex;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MenuItems.Count - 1)
                    value = MenuItems.Count - 1;

                if (value != _selectedItemIndex)
                {
                    MenuItems[_selectedItemIndex].OnUnhover();
                    MenuItems[value].OnHover();
                }
                
                _selectedItemIndex = value;
            }
        }

        public SpriteMap LoadingAnimation;
        public SpriteMap IconSheet;
        public Sprite MainSprite;
        public Sprite DoorSprite;
        
        public const float FONT_SIZE = 0.8f;
        
        public List<IRMenuItem> MenuItems = new();
        public bool IsLoadingReplayPreview;
        public ReplayInfo ReplayToPlay;
        public ReplayInfo ReplayToLoadPreview;

        public int _selectedItemIndex;
        public int ScrollIndex = 0;
        
        public override void Initialize()
        {
            IconSheet = new SpriteMap("iconSheet", 16, 16) {frame = 12};
            IconSheet.CenterOrigin();
            MainSprite = new Sprite("RecorderatorMain");
            DoorSprite = new Sprite("RecorderatorDoor");

            LoadingAnimation = new SpriteMap("quackLoader", 31, 31);
            LoadingAnimation.AddAnimation("load", 0.3f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            LoadingAnimation.SetAnimation("load");
            LoadingAnimation.scale = new Vec2(2);
            LoadingAnimation.CenterOrigin();

            backgroundColor = Color.Black;

            UpdateMenuItemList();
            
            base.Initialize();
        }

        public void UpdateMenuItemList()
        {
            MenuItems.Clear();
            
            string[] replayPaths = Directory.GetFiles(CordsPath, "*.crf", SearchOption.TopDirectoryOnly);
            string[] folderPaths = Directory.GetDirectories(CordsPath);

            MenuItems.AddRange(folderPaths.Select(x => new FolderInfo(x)));
            MenuItems.AddRange(replayPaths.OrderByDescending(x => new FileInfo(x).CreationTime).Select(x => new ReplayInfo(x)));
        }
        
        public override void Update()
        {
            if (ReplayToLoadPreview != null)
            {
                ReplayToLoadPreview.LoadPreview();
                ReplayToLoadPreview = null;
                Graphics.fade = 1;
            }

            if (SelectedItemIndex != -1)
            {
                MenuItems[SelectedItemIndex].UpdateHovered();
            }

            UpdateInputs();

            if (ReplayToPlay != null)
            {
                Recorderator.PlayReplay(ReplayToPlay.ReplayFilePath);
                ReplayToPlay = null;
            }

            base.Update();
        }

        private void UpdateInputs()
        {
            if (Input.Pressed(Triggers.MenuDown))
            {
                SelectedItemIndex++;
            }
            else if (Input.Pressed(Triggers.MenuUp))
            {
                SelectedItemIndex--;
            }
            else if ((Input.Pressed(Triggers.Select) || (DGRSettings.MenuMouse && Mouse.left == InputState.Pressed)) && SelectedItemIndex != -1)
            {
                MenuItems[SelectedItemIndex].OnSelect();
            }
        }

        public override void Draw()
        {
            Layer.lighting = false;

            Graphics.Draw(MainSprite, 0, 0, -0.8f);
            Graphics.Draw(DoorSprite, 194, 10.5f, -1f);

            if (IsLoadingReplayPreview)
                Graphics.Draw(LoadingAnimation, 258, 46, (Depth)(-0.6f));

            Vec2 mousePos = Mouse.positionScreen;
            Rectangle exitButtonBounds = new Rectangle(new Vec2(304, 160), new Vec2(318, 174));
            
            if (exitButtonBounds.Contains(mousePos))
            {
                IconSheet.frame = 12;
                IconSheet.scale = new Vec2(1.2f);
                Material prevMat = Graphics.material;
                Graphics.material = new MaterialSelection();
                Graphics.Draw(IconSheet, 311, 169);
                Graphics.material = prevMat;
            }
            else
            {
                IconSheet.frame = 12;
                IconSheet.scale = new Vec2(1.2f);
                Graphics.Draw(IconSheet, 311, 169);
            }

            if (DGRSettings.MenuMouse)
                FeatherFashion.DrawCursor(mousePos);

            IconSheet.scale = new Vec2(FONT_SIZE * 0.75f);
            float accumulatedYOffset = 0f;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                int itemIndex = i; // for scrolling later
                
                IconSheet.frame = 1;
                Vec2 drawPos = new(18, 1 + accumulatedYOffset);
                Rectangle textBounds = MenuItems[i].Draw(drawPos, itemIndex == SelectedItemIndex);

                accumulatedYOffset += textBounds.height + 1;

                if (DGRSettings.MenuMouse && textBounds.Shrink(-10, 0, 0, 0).Contains(Mouse.positionScreen))
                {
                    SelectedItemIndex = itemIndex;
                }
            }
        }
    }
}