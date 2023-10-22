using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;
using System.Reflection;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        private const BindingFlags ALL_INSTANCE = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
        
        [Marker.DevConsoleCommand(Description = "Calls a parameterless method in the duck class of the given player's duck", IsCheat = true)]
        public static void Call(
            Duck duck,
            [ReflectionAutoCompl(typeof(Duck), MemberTypes.Method , ALL_INSTANCE)] string method,
            params string[] commandArgs)
        {
            MethodInfo methodInfo = typeof(Duck).GetMethod(method, ALL_INSTANCE);

            if (methodInfo is null)
                throw new Exception($"Method not found in Duck class: {method}");

            ShellCommand command = ShellCommand.FromMethodInfo(methodInfo);
            ShellCommand.Parameter[] parameterInfos = command.Parameters;
            object?[] appliedParameters = new object?[parameterInfos.Length];
            bool lastIsParams = parameterInfos.Length != 0 && parameterInfos[parameterInfos.Length - 1].IsParams;

            for (int i = 0; i < appliedParameters.Length; i++)
            {
                ShellCommand.Parameter parameterInfo = parameterInfos[i];
                object? appliedParameterValue;

                if (i >= commandArgs.Length && !lastIsParams)
                {
                    if (parameterInfo.IsOptional)
                        appliedParameterValue = parameterInfo.DefaultValue;
                    else throw new Exception($"Missing Argument: {parameterInfo.Name}");
                }
                else
                {
                    Type parseType = parameterInfo.ParameterType;
                    string argString;
                    ValueOrException<object> parseResult = null;

                    bool isLast = i == appliedParameters.Length - 1;

                    if (isLast && (parameterInfo.IsParams || parseType == typeof(string)))
                    {
                        int length = commandArgs.Length - i;
                        string[] resultArray = new string[length];
                        Array.Copy(commandArgs, i, resultArray, 0, length);

                        if (parseType == typeof(string))
                            argString = string.Join(" ", resultArray);
                        else if (parameterInfo.IsParams)
                        {
                            if (length != 0)
                            {
                                parseResult = ValueOrException<object>.FromValue(resultArray);
                            }

                            argString = null;
                        }
                        else throw new InvalidOperationException();
                    }
                    else if (commandArgs.Length == 0)
                    {
                        throw new Exception($"Missing Argument: {parameterInfo.Name}");
                    }
                    else argString = commandArgs[i];

                    if (parseResult is null)
                    {
                        if (!Commands.console.Shell.TypeInterpreterModules.TryFirst(x => x.ParsingType.IsAssignableFrom(parseType),
                                out ITypeInterpreter interpreter))
                            throw new Exception($"No conversion module found: {parseType.Name}");

                        parseResult = interpreter.ParseString(argString, parseType, Commands.console.Shell);
                    }

                    if (parseResult.Failed)
                        throw new Exception($"Parsing Error: {parseResult.Error.Message}");

                    appliedParameterValue = parseResult.Value;
                }

                appliedParameters[i] = appliedParameterValue;
            }

            methodInfo.Invoke(duck, appliedParameters);
        }
    }
}