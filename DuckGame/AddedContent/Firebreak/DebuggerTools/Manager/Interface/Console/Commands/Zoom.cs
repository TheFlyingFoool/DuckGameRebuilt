using DuckGame.ConsoleInterface;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Changes the Zoom value of MallardManager")]
        public static float Zoom(float newValue) => MallardManager.Config.Zoom = newValue;
    }
}