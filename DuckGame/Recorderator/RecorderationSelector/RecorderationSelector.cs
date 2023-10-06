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
                else if (value > MAX_ELEMENTS_ON_SCREEN - 1)
                    value = MAX_ELEMENTS_ON_SCREEN - 1;

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
        public const int MAX_ELEMENTS_ON_SCREEN = 24;
        
        public List<IRMenuItem> MenuItems = new();
        public bool IsLoadingReplayPreview;
        public ReplayInfo ReplayToPlay;
        public ReplayInfo ReplayToLoadPreview;

        public int ScrollIndex = 0;
        private int _selectedItemIndex;
        private string[] _replayPaths;
        private string[] _folderPaths;
        
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

            MenuItems.Clear();
            
            _folderPaths = Directory.GetDirectories(CordsPath);
            _replayPaths = Directory.GetFiles(CordsPath, "*.crf", SearchOption.TopDirectoryOnly);

            MenuItems.AddRange(_folderPaths.Select(x => new FolderInfo(x)));
            MenuItems.AddRange(_replayPaths.OrderByDescending(x => new FileInfo(x).CreationTime).Select(x => new ReplayInfo(x)));
            
            base.Initialize();
        }

        public void UpdateMenuItemList()
        {
            List<(int, List<IRMenuItem>)> toAdd = new();
            
            for (int i = 0; i < MenuItems.Count; i++)
            {
                IRMenuItem item = MenuItems[i];

                if (item.Parent is FolderInfo parentFolder && !parentFolder.Open)
                {
                    parentFolder.Items.Add(item);
                    MenuItems[i] = null;
                    continue;
                }
                
                if (item is not FolderInfo folder)
                    continue;
                
                if (!folder.Open)
                    continue;
                
                folder.Items.ForEach(x =>
                {
                    x.FolderSub = folder.FolderSub + 1;
                    x.Parent = folder;
                });
                
                toAdd.Add((i + 1, folder.Items.ToList()));
                folder.Items.Clear();
            }

            foreach ((int insertionIndex, List<IRMenuItem> items) in toAdd)
            {
                MenuItems.InsertRange(insertionIndex, items);
            }

            MenuItems.RemoveAll(x => x is null);
        }
        
        public override void Update()
        {
            if (ReplayToLoadPreview != null)
            {
                ReplayToLoadPreview.LoadPreview();
                ReplayToLoadPreview = null;
                Graphics.fade = 1;
            }

            if (SelectedItemIndex + ScrollIndex != -1)
            {
                MenuItems[SelectedItemIndex + ScrollIndex].UpdateHovered();
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
                int prev = SelectedItemIndex++;
                if (prev == _selectedItemIndex)
                {
                    ScrollDown();
                    SelectedItemIndex++;
                }
            }
            else if (Input.Pressed(Triggers.MenuUp))
            {
                int prev = SelectedItemIndex--;
                if (prev == _selectedItemIndex)
                {
                    ScrollUp();
                    SelectedItemIndex--;
                }
            }
            else if ((Input.Pressed(Triggers.Select) || (DGRSettings.MenuMouse && Mouse.left == InputState.Pressed)) && SelectedItemIndex != -1)
            {
                MenuItems[SelectedItemIndex + ScrollIndex].OnSelect();
            }

            if (DGRSettings.MenuMouse)
            {
                if (Mouse.scrollingDown)
                {
                    for (int i = 0; i < (int)(Mouse.scroll / 120); i++)
                    {
                        ScrollDown();
                    }
                }
                else if (Mouse.scrollingUp)
                {
                    for (int i = 0; i < (int)(-Mouse.scroll / 120); i++)
                    {
                        ScrollUp();
                    }
                }
            }
        }

        private void ScrollUp()
        {
            if (ScrollIndex > 0)
                ScrollIndex--;
        }

        private void ScrollDown()
        {
            if (ScrollIndex < MenuItems.Count - MAX_ELEMENTS_ON_SCREEN)
                ScrollIndex++;
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

            float accumulatedYOffset = 0f;
            for (int i = ScrollIndex; i < MAX_ELEMENTS_ON_SCREEN + ScrollIndex; i++)
            {
                int itemIndex = i - ScrollIndex;
                IRMenuItem menuItem = MenuItems[i];
                
                Vec2 drawPos = new(18 + (menuItem.FolderSub * 6), 1 + accumulatedYOffset);
                Rectangle textBounds = menuItem.Draw(drawPos, itemIndex == SelectedItemIndex);

                accumulatedYOffset += textBounds.height + 1;

                if (DGRSettings.MenuMouse && textBounds.Shrink(-10, 0, 0, 0).Contains(Mouse.positionScreen))
                {
                    SelectedItemIndex = itemIndex;
                }
            }
        }
    }
}