using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class RecorderationSelector : Level
    {
        public SpriteMap LoadingAnimation;
        public SpriteMap IconSheet;
        public Sprite MainSprite;
        public Sprite DoorSprite;
        
        public const float FONT_SIZE = 0.6f;
        public bool IsLoadingReplayPreview;
        public FolderInfo CurrentFolder;
        public FolderInfo SelectedFolder;
        public ReplayInfo ReplayToPlay;
        
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

            CurrentFolder = new FolderInfo(DuckFile.saveDirectory + "Recorderations/");
            CurrentFolder.Initialize();
            
            base.Initialize();
        }
        
        public override void Update()
        {
            if (SelectedFolder != null)
            {
                SelectedFolder.Initialize();
                CurrentFolder = SelectedFolder;
                SelectedFolder = null;
            }
            
            if (ReplayToPlay != null)
            {
                Recorderator.PlayReplay(ReplayToPlay.ReplayFilePath);
                ReplayToPlay = null;
            }
            
            if (Mouse.scrollingUp || Keyboard.Pressed(Keys.PageUp))
                CurrentFolder.ScrollIndex++;
            else if (Mouse.scrollingDown || Keyboard.Pressed(Keys.PageDown))
                CurrentFolder.ScrollIndex--;

            base.Update();
        }
        
        public override void Draw()
        {
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
                Graphics.Draw(IconSheet, 311, 169);
            }
            else
            {
                IconSheet.frame = 12;
                IconSheet.scale = new Vec2(1);
                Graphics.Draw(IconSheet, 311, 169);
            }

            FeatherFashion.DrawCursor(mousePos);

            if (CurrentFolder == null)
                return;

            IconSheet.scale = new Vec2(FONT_SIZE - 0.1f);
            for (int i = 0; i < CurrentFolder.SubFolders.Count; i++)
            {
                IconSheet.frame = 1;
                string displayName = CurrentFolder.SubFolders[i].DisplayName;
                Vec2 stringSize = Extensions.GetFancyStringSize(displayName, FONT_SIZE);
                Rectangle textBounds = new(28, 1 + (stringSize.y + 1) * i, stringSize.x, stringSize.y);
                Graphics.Draw(IconSheet, textBounds.Left - 5, textBounds.y + 3);

                bool hovered = textBounds.Shrink(-10, 0, 0, 0).Contains(Mouse.positionScreen);
                Color textColor = hovered ? Color.Yellow : Color.White;

                if (SelectedFolder == null && hovered && Mouse.left == InputState.Pressed)
                {
                    SelectedFolder = CurrentFolder.SubFolders[i];
                }

                Graphics.DrawFancyString(displayName, textBounds.tl, textColor, 1f, FONT_SIZE);
            }

            IsLoadingReplayPreview = false;

            for (int i = 0; i < CurrentFolder.Replays.Count; i++)
            {
                ReplayInfo replayInfo = CurrentFolder.Replays[CurrentFolder.Replays.Count - 1 - i];

                string displayName = replayInfo.DisplayName;
                Vec2 stringSize = Extensions.GetFancyStringSize(displayName, FONT_SIZE);
                Rectangle textBounds = new(18, 1 + (stringSize.y + 1) * (i + CurrentFolder.SubFolders.Count), stringSize.x, stringSize.y);

                bool hovered = textBounds.Contains(Mouse.positionScreen);
                Color textColor = hovered ? Color.Yellow : Color.White;

                Graphics.DrawFancyString(displayName, textBounds.tl, textColor, 1f, FONT_SIZE);

                if (hovered && Mouse.left == InputState.Pressed && ReplayToPlay == null && SelectedFolder == null)
                {
                    ReplayToPlay = replayInfo;
                }
                
                if (!hovered)
                {
                    replayInfo.FramesUntilPreviewLoad = 0;
                }
                else
                {
                    if (!replayInfo.DidLoadInfo)
                        replayInfo.LoadInfo();

                    TimeSpan matchDuration = TimeSpan.FromSeconds(replayInfo.MatchDurationFrames / 60f);
                    string infoString =
                        $"DURATION {(matchDuration.Minutes + (matchDuration.Hours * 60)):00}:{matchDuration.Seconds:00} " +
                        $"HOST {replayInfo.HostName}\n" +
                        $"PLAYERS {replayInfo.PlayerCount} " +
                        $"SPECTATORS {replayInfo.SpectatorCount}";

                    Graphics.DrawFancyString(infoString, new Vec2(195, 95f), Color.White, 1f, 0.5f);

                    if (!replayInfo.DidLoadPreview)
                    {
                        if (replayInfo.FramesUntilPreviewLoad > 10)
                        {
                            replayInfo.LoadPreview();
                        }
                        else
                        {
                            replayInfo.FramesUntilPreviewLoad++;
                        }

                        IsLoadingReplayPreview = true;
                    }
                    else
                    {
                        Graphics.Draw(replayInfo.Preview, 0, 0, 1, 0.5f, -1);
                        IsLoadingReplayPreview = false;
                    }

                    for (int j = 0; j < replayInfo.Profiles.Count; j++)
                    {
                        ProfileInfo profile = replayInfo.Profiles[j];

                        if (profile.Team != null)
                        {
                            profile.Team.hat.center = new Vec2(0);
                            profile.Team.hat.scale = new Vec2(0.4f);
                            profile.Team.hat.depth = -0.98f;
                            profile.Team.hat.alpha = profile.IsSpectator ? 0.5f : 1;

                            Graphics.Draw(profile.Team.hat, 191 + (j * 10), 141.6f);
                        }

                        profile.ChatBustDuck.depth = -0.99f;
                        profile.ChatBustDuck.scale = new Vec2(0.4f);
                        profile.ChatBustDuck.alpha = profile.IsSpectator ? 0.5f : 1;

                        Graphics.Draw(profile.ChatBustDuck, 198 + (j * 10), 150.12f);
                    }
                }
            }
        }
    }
}