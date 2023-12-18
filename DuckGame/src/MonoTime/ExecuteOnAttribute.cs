using System;

namespace DuckGame
{

    [AttributeUsage(AttributeTargets.Method)]
    public class ExecuteOnAttribute : Attribute
    {
        public ExecuteOnAttribute(ExecutionTime executionTime)
        {
            ExecutionTime = executionTime;
        }

        public ExecutionTime ExecutionTime;
    }

    public enum ExecutionTime
    {
        PreInitialize,
        Initialize,
        PostInitialize,
        PreUpdate,
        Update,
        PostUpdate,
        Draw,
        PostDraw,
    }
}