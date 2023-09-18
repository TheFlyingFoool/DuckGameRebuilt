using System;
using System.Collections.Generic;
using System.Linq;
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
            
            public static AnimationData Walk = new(120, AnimationInitialize, new()
            {
                {0, duck => duck.AiInput.Press(Triggers.Left)},
                {1, duck => duck.AiInput.HoldDown(Triggers.Left)},
                {30, duck => duck.AiInput.Release(Triggers.Left)},
                {40, duck => duck.AiInput.Press(Triggers.Right)},
                {41, duck => duck.AiInput.HoldDown(Triggers.Right)},
                {100, duck => duck.AiInput.Release(Triggers.Right)},
            });
            
            public static AnimationData Jump = new(60, AnimationInitialize, new()
            {
                {0, duck => duck.AiInput.Press(Triggers.Jump)},
                {1, duck => duck.AiInput.HoldDown(Triggers.Jump)},
                {60, duck => duck.AiInput.Release(Triggers.Jump)},
            });
            
            public static AnimationData Flutter = new(60, AnimationInitialize, new()
            {
                {0, duck => duck.AiInput.Press(Triggers.Jump)},
                {1, duck => duck.AiInput.HoldDown(Triggers.Jump)},
                {10, duck => duck.AiInput.Release(Triggers.Jump)},
                {11, duck => duck.AiInput.Press(Triggers.Jump)},
                {12, duck => duck.AiInput.HoldDown(Triggers.Jump)},
            });
            
            public static AnimationData Ragdoll = new(60, AnimationInitialize, new()
            {
                {0, duck => duck.AiInput.Press(Triggers.Ragdoll)},
                {1, duck => duck.AiInput.Release(Triggers.Ragdoll)},
            });
            
            public static AnimationData Netted = new(120, duck =>
            {
                AnimationInitialize(duck);
                Add(new NetGun(duck.x - 48, duck.y){gravMultiplier = 0});
            }, new()
            {
                {0, duck =>
                {
                    NetGun netGun = (NetGun) current.things[typeof(NetGun)].First();
                    netGun.position = new Vec2(duck.x - 48, duck.y);
                }},
                {20, duck =>
                {
                    NetGun netGun = (NetGun) current.things[typeof(NetGun)].First();
                    netGun.OnPressAction();
                    netGun.position = new Vec2(duck.x - 48, duck.y);
                }},
                {21, duck =>
                {
                    NetGun netGun = (NetGun) current.things[typeof(NetGun)].First();
                    netGun.position = new Vec2(duck.x - 48, duck.y);
                }}
            });
            
            public static AnimationData Equip = new(120, duck =>
            {
                AnimationInitialize(duck);
                duck.Unequip(duck._equipment.First());
                current.things[typeof(TeamHat)].First().position = duck.position + new Vec2(8, -8);
            }, new()
            {
                {20, duck => duck.AiInput.Press(Triggers.Grab)},
                {21, duck => duck.AiInput.Release(Triggers.Grab)},
                {40, duck => duck.AiInput.Press(Triggers.Shoot)},
                {41, duck => duck.AiInput.Release(Triggers.Shoot)},
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