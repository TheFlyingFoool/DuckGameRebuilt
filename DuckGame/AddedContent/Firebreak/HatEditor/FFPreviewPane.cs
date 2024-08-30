using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public partial class FeatherFashion
    {
        public static class FFPreviewPane
        {
            public static bool AllowUserControl = false;
            public static int CurrentPLayingAnimationIndex;
            public static bool IsLoopingAnimation;
            public static bool IsPlayingAnimation = true;
            public static Dictionary<string, AnimationData> DuckAnimations = new();
            public static Tex2D[] CurrentDuckAnimation;
            
            private static string[] s_animationsIndexed;
            private static Tex2D s_hatTexture;
            private static bool s_scheduleAnimationChange;
            private static int s_currentAnimationFrame = 0;
            private static Rectangle s_progressBarRect;
            private static bool s_hoveringProgressBar;
            private static bool s_clickedInProgressBar;

            public static void OnSwitch()
            {
                s_hatTexture = GetFullHatTexture();
                InitializeAnimations();
                
                if (AllowUserControl)
                    InitializePlayable();
            }
            
            public static void OnSwitchOutOf()
            {
                DisposeCurrentAnimation();
                ClearThings();
            }

            private static void InitializeAnimations()
            {
                // reflection in my own code? YIPPIE
                DuckAnimations.Clear();
                foreach (FieldInfo field in typeof(FFPreviewAnimations).GetFields())
                {
                    DuckAnimations.Add(field.Name, (AnimationData) field.GetValue(null));
                }

                s_animationsIndexed = DuckAnimations.Select(x => x.Key).ToArray();
                OnSwitchAnimation();
            }

            public static void AnimationInitialize(Duck duck)
            {
                Team team = GetHatTeam();
                duck.profile.team = team;
                
                // turns out this is done automatically, and
                // manually doing it actually makes you equip
                // two hats at once, lmao
                
                // TeamHat hat = new(9999, 9999, team, duck.profile);
                // duck._equipment.Add(hat);
                // hat.Equip(duck);
                // Add(hat);

                duck.AiInput = new DuckAI();
            }

            private static Team GetHatTeam()
            {
                Team team = new(HatName ?? string.Empty, s_hatTexture);

                team._capeTexture = new Tex2D(32, 32);
                team._capeTexture.SetData(CapeFrameBuffer);
                team._rockTexture = new Tex2D(24, 24);
                team._rockTexture.SetData(RockFrameBuffer);
                for (int i = 0; i < 4; i++)
                {
                    Tex2D particle = new(12, 12);
                    particle.SetData(ParticleAnimationBuffer[i]);
                    team._customParticles.Add(particle);
                }

                Team.ProcessMetadata(s_hatTexture, team);
                return team;
            }

            public static void Update()
            {
                
            }

            private static void TogglePlayerControl()
            {
                if (AllowUserControl ^= true)
                    InitializePlayable();
                else ClearThings();
            }

            private static void InitializePlayable()
            {
                Rectangle innerBounds = new(92, 32, 136, 64);
                Duck duck = new(innerBounds.x + (innerBounds.width / 2), innerBounds.y + innerBounds.height - 1, Profiles.DefaultPlayer1);
                AnimationInitialize(duck);
                duck.AiInput = null;
                Add(duck);

                HatPreviewLevel.SpawnInvisiblePrison(innerBounds);
            }

            private static void GoPreviousAnimation()
            {
                CurrentPLayingAnimationIndex--;
                if (CurrentPLayingAnimationIndex < 0)
                    CurrentPLayingAnimationIndex = DuckAnimations.Count - 1;
                OnSwitchAnimation();
            }

            private static void GoNextAnimation()
            {
                CurrentPLayingAnimationIndex++;
                CurrentPLayingAnimationIndex %= DuckAnimations.Count;
                OnSwitchAnimation();
            }

            private static void OnSwitchAnimation()
            {
                s_currentAnimationFrame = 0;

                DisposeCurrentAnimation();

                AnimationData nextAnimationData = DuckAnimations[s_animationsIndexed[CurrentPLayingAnimationIndex]];
                CurrentDuckAnimation = SimRenderer.RenderLevelAnimation(nextAnimationData, 136 * 2, 64 * 2, 2, new Vec2(0, 0));
            }

            public static void DisposeCurrentAnimation()
            {
                if (CurrentDuckAnimation is null)
                    return;

                foreach (Tex2D tex2D in CurrentDuckAnimation)
                {
                    tex2D?.Dispose();
                }

                CurrentDuckAnimation = null;
            }

            public static void Draw()
            {
                Vec2 center = current.camera.center;
                Sprite frames = FFIcons.PreviewPaneFrames;
                Vec2 halfFramesSize = new(frames.w / 2f, frames.h / 2f);
                Rectangle framesBounds = new(center - halfFramesSize, center + halfFramesSize);

                Graphics.Draw(frames, framesBounds.x, framesBounds.y, 1f);
                if (!AllowUserControl)
                    DrawAnimation(framesBounds.tl + (Vec2.One * 4), 0.9f);
                else Graphics.DrawRect(new Rectangle(framesBounds.x + 4, framesBounds.y + 4, 136, 64), Color.SlateGray, 0.9f);

                float textScale = 0.6f;
                string animationName = s_animationsIndexed[CurrentPLayingAnimationIndex];
                Graphics.DrawString(animationName, framesBounds.tl + new Vec2(5, Extensions.GetStringSize(animationName, textScale).Height + 1), FFColors.Focus, 1.1f, scale: textScale);

                (SpriteMap, string, bool?, Rectangle)[] iconBoundsPairs =
                {
                    (
                        FFIcons.Loop,
                        "Loop playing current animation",
                        IsLoopingAnimation,
                        new Rectangle(framesBounds.x + 4, framesBounds.y + 80, 8, 8)
                    ),
                    (
                        FFIcons.Play,
                        "Toggle animation playing",
                        IsPlayingAnimation,
                        new Rectangle(framesBounds.x + 20, framesBounds.y + 80, 8, 8)
                    ),
                    (
                        FFIcons.PreviousAnimation,
                        "Play previous animation",
                        null,
                        new Rectangle(framesBounds.x + 4, framesBounds.y + 96, 8, 8)
                    ),
                    (
                        FFIcons.NextAnimation,
                        "Play next animation",
                        null,
                        new Rectangle(framesBounds.x + 20, framesBounds.y + 96, 8, 8)
                    ),
                    (
                        FFIcons.PlayTest,
                        "Take control of the duck",
                        AllowUserControl,
                        new Rectangle(framesBounds.x + 4, framesBounds.y + 112, 24, 8)
                    )
                };

                Rectangle animationSelectorBounds = new(framesBounds.x + 36, framesBounds.y + 80, 92, 40);
                Graphics.DrawFancyString("ignore this void\n\nmight be a thing later", animationSelectorBounds.tl + Vec2.One, FFColors.Focus, 1f, 0.6f);

                foreach ((SpriteMap icon, string tooltip, bool? toggle, Rectangle bounds) in iconBoundsPairs)
                {
                    if (bounds.Contains(Mouse.positionScreen))
                    {
                        icon.frame = 1;
                    }
                    else if (toggle is not null && toggle.Value)
                        icon.frame = 1;
                    else icon.frame = 0;
                    
                    Graphics.Draw(icon, bounds.x, bounds.y, 1f);
                }

                s_progressBarRect = new Rectangle(framesBounds.x + 136, framesBounds.y + 80, 4, 40);
                Rectangle progressIndicatorRect = new(
                    s_progressBarRect.x,
                    s_progressBarRect.y + ((s_progressBarRect.height / CurrentDuckAnimation.Length) * s_currentAnimationFrame),
                    s_progressBarRect.width,
                    (s_progressBarRect.height * 1.5f) / CurrentDuckAnimation.Length
                );
                s_hoveringProgressBar = s_progressBarRect.Contains(Mouse.positionScreen);
                
                Graphics.DrawRect(s_progressBarRect, FFColors.PrimaryDim, 0.9f);
                Graphics.DrawRect(progressIndicatorRect, s_hoveringProgressBar ? FFColors.Focus : FFColors.PrimaryHighlight, 1.1f);
            }

            private static void DrawAnimation(Vec2 position, Depth depth)
            {
                Tex2D[] renderFrames = CurrentDuckAnimation;
                Tex2D currentFrame = renderFrames[s_currentAnimationFrame];

                if (IsPlayingAnimation && !s_clickedInProgressBar && MonoMain.UpdateLerpState)
                {
                    s_currentAnimationFrame++;

                    if (s_currentAnimationFrame >= renderFrames.Length)
                    {
                        s_currentAnimationFrame = 0;

                        if (!IsLoopingAnimation)
                            s_scheduleAnimationChange = true;
                    }
                }
                
                Graphics.Draw(currentFrame, position.x, position.y, 0.25f, 0.25f, depth);
            }
        }
    }
}