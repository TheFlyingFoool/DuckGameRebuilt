using System.Reflection;

namespace DuckGame.ConsoleEngine
{
    public class TypeInterpreterParseContext
    {
        public TypeInterpreterParseContext(CommandRunner executionEngine, ShellCommand.Parameter parameterInfo)
        {
            ExecutionEngine = executionEngine;
            ParameterInfo = parameterInfo;
        }
        
        public CommandRunner ExecutionEngine;
        public ShellCommand.Parameter ParameterInfo;
    }
}