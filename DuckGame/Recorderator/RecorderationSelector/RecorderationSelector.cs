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

            // for (int i = 0; i < CurrentFolder.Replays.Count; i++)
            // {
            //     ReplayInfo replayInfo = CurrentFolder.Replays[CurrentFolder.Replays.Count - 1 - i];
            //
            //     string displayName = replayInfo.DisplayName;
            //     Vec2 stringSize = Extensions.GetFancyStringSize(displayName, FONT_SIZE);
            //     Rectangle textBounds = new(18, 1 + (stringSize.y + 1) * (i + CurrentFolder.SubFolders.Count), stringSize.x, stringSize.y);
            //
            //     bool hovered = textBounds.Contains(Mouse.positionScreen);
            //     Color textColor = hovered ? Color.Yellow : Color.White;
            //
            //     Graphics.DrawFancyString(displayName, textBounds.tl, textColor, 1f, FONT_SIZE);
            //
            //     if (hovered && Mouse.left == InputState.Pressed && ReplayToPlay == null && SelectedFolder == null)
            //     {
            //         ReplayToPlay = replayInfo;
            //     }
            //     
            //     if (!hovered)
            //     {
            //         replayInfo.FramesUntilPreviewLoad = 0;
            //     }
            //     else
            //     {
            //         if (!replayInfo.DidLoadInfo)
            //             replayInfo.LoadInfo();
            //
            //         TimeSpan matchDuration = TimeSpan.FromSeconds(replayInfo.MatchDurationFrames / 60f);
            //         string infoString =
            //             $"DURATION {(matchDuration.Minutes + (matchDuration.Hours * 60)):00}:{matchDuration.Seconds:00} " +
            //             $"HOST {replayInfo.HostName}\n" +
            //             $"PLAYERS {replayInfo.PlayerCount} " +
            //             $"SPECTATORS {replayInfo.SpectatorCount}";
            //
            //         Graphics.DrawFancyString(infoString, new Vec2(195, 95f), Color.White, 1f, 0.5f);
            //
            //         if (!replayInfo.DidLoadPreview)
            //         {
            //             if (replayInfo.FramesUntilPreviewLoad > 15)
            //             {
            //                 ReplayToLoadPreview = replayInfo;
            //             }
            //             else
            //             {
            //                 replayInfo.FramesUntilPreviewLoad++;
            //             }
            //
            //             IsLoadingReplayPreview = true;
            //         }
            //         else
            //         {
            //             Graphics.Draw(replayInfo.Preview, 194, 9, 0.1f, 0.1f, -1);
            //             IsLoadingReplayPreview = false;
            //         }
            //
            //         for (int j = 0; j < replayInfo.Profiles.Count; j++)
            //         {
            //             ProfileInfo profile = replayInfo.Profiles[j];
            //
            //             if (profile.Team != null)
            //             {
            //                 profile.Team.hat.center = Vec2.Zero;
            //                 if (profile.Team.metadata != null) 
            //                     profile.Team.hat.center = profile.Team.metadata.HatOffset.value;
            //                 profile.Team.hat.scale = new Vec2(0.4f);
            //                 profile.Team.hat.depth = -0.98f;
            //                 profile.Team.hat.alpha = profile.IsSpectator ? 0.5f : 1;
            //
            //                 Graphics.Draw(profile.Team.hat, 191 + (j * 10), 141.6f);
            //             }
            //
            //             profile.ChatBustDuck.depth = -0.99f;
            //             profile.ChatBustDuck.scale = new Vec2(0.4f);
            //             profile.ChatBustDuck.alpha = profile.IsSpectator ? 0.5f : 1;
            //
            //             Graphics.Draw(profile.ChatBustDuck, 198 + (j * 10), 150.12f);
            //         }
            //     }
            // }
        }
    }
}