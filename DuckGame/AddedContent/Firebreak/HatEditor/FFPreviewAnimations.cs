using System;
using System.Collections.Generic;
using static DuckGame.FeatherFashion.FFPreviewPane;

namespace DuckGame
{
    public partial class FeatherFashion
    {
        public static class FFPreviewAnimations
        {
            public static AnimationData Idle = new(120, AnimationInitialize, new());
            
            public static AnimationData Quack = new(120, AnimationInitialize, new()
            {
                {30, duck => duck.AiInput.Press(Triggers.Quack)},
                {31, duck => duck.AiInput.HoldDown(Triggers.Quack)},
                {60, duck => duck.AiInput.Release(Triggers.Quack)},
            });
        }

        public class AnimationData
        {
            public Action<Duck> Initialize;
            public Dictionary<uint, Action<Duck>> Timeline;
            public int Length;

            public AnimationData(int length, Action<Duck> initialize, Dictionary<uint, Action<Duck>> timeline)
            {
                Initialize = initialize;
                Length = length;
                Timeline = timeline;
            }
        }
    }
}