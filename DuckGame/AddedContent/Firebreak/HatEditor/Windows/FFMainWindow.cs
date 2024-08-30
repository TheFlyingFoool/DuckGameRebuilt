using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{
    public class FFMainWindow
    {
        public int FocusedChildIndex = -1;
        public List<FFBoundary> Components = new();

        public FFButton _playtestButton;
        public FFButton[] _topAnimControlButtons;
        public FFSlider _slider;
        public FFButton[] _bottomPlaytestControlButtons;
        public FFHatPreview _hatPreview;
        public FFMetapixelList _metapixelList;
        
        private FFUITransition _activeTransition;
        
        public FFMainWindow()
        {
            Components.Add(_metapixelList = new FFMetapixelList(new Rectangle(32, 36, 144, 116)));
            
            Components.Add(_hatPreview = new FFHatPreview(new Rectangle(188, 52, 96, 88)));
            
            Components.Add(_playtestButton = new FFButton(
                new Rectangle(188, 36, 24, 8),
                FeatherFashion.FFIcons.PlayTest,
                typeof(FFHatPreview).GetField(nameof(FFHatPreview.Playtest)), _hatPreview)
                {
                    ButtonToggleAction = StartTransitionAnimation
                }
            );

            Components.AddRange(_topAnimControlButtons = new[]
            {
                new FFButton(
                    new Rectangle(228, 36, 8, 8),
                    FeatherFashion.FFIcons.Loop,
                    typeof(FFHatPreview).GetField(nameof(FFHatPreview.LoopAnimation)), _hatPreview),
                new FFButton(
                    new Rectangle(244, 36, 8, 8),
                    FeatherFashion.FFIcons.PreviousAnimation,
                    null),
                new FFButton(
                    new Rectangle(260, 36, 8, 8),
                    FeatherFashion.FFIcons.PlayPause,
                    typeof(FFHatPreview).GetField(nameof(FFHatPreview.AnimationPlaying)), _hatPreview),
                new FFButton(
                    new Rectangle(276, 36, 8, 8),
                    FeatherFashion.FFIcons.NextAnimation,
                    null),
            });
            
            Components.Add(_slider = new FFSlider(
                new Rectangle(196, 148, 80, 4),
                new ProgressValue(0, 1, 0, 15),
                null)
            );
            
            Components.AddRange(_bottomPlaytestControlButtons = new []
            {
                new FFButton(
                    new Rectangle(0, 0, 8, 8),
                    FeatherFashion.FFIcons.Kill,
                    null) {
                        Disable = true,
                        Depth = 0.5f
                    },
                new FFButton(
                    new Rectangle(0, 0, 8, 8),
                    FeatherFashion.FFIcons.SpawnProp,
                    null) {
                        Disable = true,
                        Depth = 0.5f
                    },
                new FFButton(
                    new Rectangle(0, 0, 8, 8),
                    FeatherFashion.FFIcons.SpawnRandom,
                    null) {
                        Disable = true,
                        Depth = 0.5f
                    },
                new FFButton(
                    new Rectangle(0, 0, 8, 8),
                    FeatherFashion.FFIcons.Fullscreen,
                    typeof(FFHatPreview).GetField(nameof(FFHatPreview.Fullscreen)), _hatPreview) {
                        Disable = true,
                        Depth = 0.5f,
                        ButtonToggleAction = StartTransitionAnimation
                    },
            });
        }

        public void Update()
        {
            UpdateTransitionAnimations();
            
            FocusedChildIndex = FeatherFashion.InputMode switch
            {
                EditorInput.Mouse => Components.FindIndex(x => x.Bounds.Contains(Mouse.positionScreen)),
                EditorInput.Gamepad => FocusedChildIndex,
                _ => -1
            };

            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Update(i == FocusedChildIndex);
            }
        }

        private void StartTransitionAnimation(FFButton sourceButton, bool newValue)
        {
            float dir = float.NaN;
            
            if (_hatPreview.Fullscreen)
            {
                if (sourceButton == _playtestButton)
                {
                    _activeTransition = new FFUI03Transition();
                    dir = newValue ? 1 : -1;
                }
                else if (sourceButton == _bottomPlaytestControlButtons.Last())
                {
                    _activeTransition = new FFUI13Transition();
                    dir = 1;
                }
            }
            else
            {
                if (sourceButton == _playtestButton)
                {
                    _activeTransition = new FFUI01Transition();
                    dir = newValue ? 1 : -1;
                }
                else if (sourceButton == _bottomPlaytestControlButtons.Last())
                {
                    _activeTransition = new FFUI13Transition();
                    dir = -1;
                }
            }
            
            _activeTransition.MainWindow = this;
            _activeTransition.Direction = dir;
            _activeTransition.EasingFunction = Ease.Out.Quart;
            
            _activeTransition.Start();
        }

        private void UpdateTransitionAnimations()
        {
            if (_activeTransition is null)
                return;
            
            _activeTransition.Update();
            
            if (_activeTransition.Finished)
            {
                _activeTransition.End();
                _activeTransition = null;
            }
        }

        public void Draw()
        {
            Graphics.Draw(FeatherFashion.FFIcons.FFLogo, 2, 2);

            if (FeatherFashion.InputMode == EditorInput.Mouse)
            {
                FeatherFashion.DrawCursor(Mouse.positionScreen);
            }
            
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Draw(i == FocusedChildIndex);
            }
        }
    }
}