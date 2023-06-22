using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine;

public class CommandRunner
{
    public readonly List<MMCommandAttribute> Commands = new();
    public readonly List<ITypeInterpreter> TypeInterpreterModules = new();
    public readonly long GenerationID = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    
    public Regex ValidNameRegex { get; } = new("^[^ ]+$", RegexOptions.Compiled);

    public event Action<MMCommandAttribute, object> PostCommandRunAction;

    public virtual ValueOrException<object?> Run(string input)
    {
        if (string.IsNullOrEmpty(input))
            return ValueOrException<object>.FromValue(null);

        string[] commandSegments;

        try
        {
            commandSegments = Tokenize(input);
        }
        catch (Exception e)
        {
            return e;
        }

        if (commandSegments.Length == 0)
            return ValueOrException<object>.FromValue(null);

        if (commandSegments.Contains(";"))
        {
            List<string>[] commandSequence = new List<string>[commandSegments.Count(x => x == ";") + 1];

            for (int i = 0; i < commandSequence.Length; i++)
            {
                commandSequence[i] = new List<string>();
            }
            
            for (int i=0, j=0; i < commandSegments.Length; i++)
            {
                string segment = commandSegments[i];
                
                if (segment == ";")
                {
                    j++;
                    continue;
                }

                commandSequence[j].Add(segment);
            }

            ValueOrException<string> accumulativeResult = "";
            foreach (List<string> cmd in commandSequence)
            {
                ValueOrException<string[]> cmdParseResult = ParseCodeBlocks(cmd.ToArray());
                
                ValueOrException<object?> result = null!;
                
                cmdParseResult.TryUse(
                    tokens => result = RunFromTokens(tokens),
                    exception => result = exception
                );

                ValueOrException<string> stringResult = result.Failed
                    ? result.Error
                    : result.Value?.ToString();

                accumulativeResult.AppendResult(stringResult);
            }

            return accumulativeResult.Failed
                ? accumulativeResult.Error
                : accumulativeResult.Value;
        }

        ValueOrException<string[]> parseResult = ParseCodeBlocks(commandSegments);
        
        return parseResult.Failed 
            ? parseResult.Error 
            : RunFromTokens(parseResult.Value);
    }

    protected virtual ValueOrException<object?> RunFromTokens(string[] tokens)
    {
        if (tokens.Length == 0)
            return null;
        
        string commandName = tokens[0];

        string[] commandArgs = new string[tokens.Length - 1];
        for (int i = 1; i < tokens.Length; i++)
        {
            commandArgs[i - 1] = tokens[i];
        }

        foreach (MMCommandAttribute attribute in Commands)
        {
            if (!string.Equals(attribute.Name, commandName, StringComparison.CurrentCultureIgnoreCase))
                continue;

            Command.Parameter[] parameterInfos = attribute.Command.Parameters;
            object?[] appliedParameters = new object?[parameterInfos.Length];

            for (int i = 0; i < appliedParameters.Length; i++)
            {
                Command.Parameter parameterInfo = parameterInfos[i];
                object? appliedParameterValue;
                if (i >= commandArgs.Length && !parameterInfos.Last().IsParams)
                {
                    if (parameterInfo.IsOptional)
                        appliedParameterValue = parameterInfo.DefaultValue;
                    else return new Exception($"Missing Argument: {parameterInfo.Name}");
                }
                else
                {
                    Type parseType = parameterInfo.ParameterType;
                    string argString;

                    if (i == appliedParameters.Length - 1
                        && (parameterInfo.IsParams || parseType == typeof(string)))
                    {
                        int length = commandArgs.Length - i;
                        string[] resultArray = new string[length];
                        Array.Copy(commandArgs, i, resultArray, 0, length);

                        if (parseType == typeof(string))
                            argString = string.Join(" ", resultArray);
                        else if (parameterInfo.IsParams)
                            argString = resultArray.Length == 0 ? null : string.Join("\xb1,", resultArray);
                        else throw new InvalidOperationException();
                    }
                    else argString = commandArgs[i];

                    if (!TypeInterpreterModules.TryGet(x => x.ParsingType.IsAssignableFrom(parseType), out ITypeInterpreter interpreter))
                        return new Exception($"No conversion module found: {parseType.Name}");

                    ValueOrException<object?> parseResult = interpreter.ParseString(argString, parseType, this);

                    if (parseResult.Failed)
                        return new Exception($"Parsing Error: {parseResult.Error.Message}");

                    appliedParameterValue = parseResult.Value;
                }

                appliedParameters[i] = appliedParameterValue;
            }

            ValueOrException<object?> result;
            try
            {
                object? invokationValue = attribute.Command.Invoke(appliedParameters);
                PostCommandRunAction?.Invoke(attribute, invokationValue);

                result = ValueOrException<object?>.FromValue(invokationValue);
            }
            catch (TargetInvocationException e)
            {
                return e.InnerException ?? e;
            }

            return result;
        }

        return new Exception($"Command not found: {commandName}");
    }

    private ValueOrException<string[]> ParseCodeBlocks(string[] commandSegments)
    {
        List<string> segments = new();
        foreach (string segment in commandSegments)
        {
            if (!segment.StartsWith("±{♥}"))
            {
                segments.Add(segment);
                continue;
            }

            ValueOrException<object?> runResult = Run(segment.Substring(4));
            runResult.TryUse(output =>
            {
                segments.Add(output?.ToString() ?? "");
            }, _ => { });

            if (runResult.Failed)
                return runResult.Error;
        }

        return segments.ToArray();
    }

    private static string[] Tokenize(string input)
    {
        List<string> split = new();

        StringBuilder currentSegment = new();
        bool treatAsDefault = false;

        bool awaitCloseCodeBlock = false;
        int ignoreCloseCodeBlocks = 0;

        bool awaitCloseChevron = false;
        int ignoreCloseChevrons = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (treatAsDefault)
            {
                currentSegment.Append(c);
                treatAsDefault = false;
                continue;
            }
            
            switch (c)
            {
                case '\\':
                    treatAsDefault = true;
                    continue;
                
                case '<':
                    if (awaitCloseCodeBlock)
                        goto default;
                    
                    if (awaitCloseChevron)
                    {
                        ignoreCloseChevrons++;
                        goto default;
                    }

                    awaitCloseChevron = true;
                    continue;
                case '>':
                    if (awaitCloseCodeBlock)
                        goto default;
                    
                    if (awaitCloseChevron)
                    {
                        if (ignoreCloseChevrons > 0)
                        {
                            ignoreCloseChevrons--;
                            goto default;
                        }
                        else
                        {
                            awaitCloseChevron = false;
                        }
                    }
                    continue;
                
                case '{':
                    if (awaitCloseChevron)
                        goto default;

                    if (awaitCloseCodeBlock)
                    {
                        ignoreCloseCodeBlocks++;
                        goto default;
                    }

                    awaitCloseCodeBlock = true;
                    currentSegment.Append("±{♥}");
                    break;
                case '}':
                    if (awaitCloseChevron)
                        goto default;

                    if (awaitCloseCodeBlock)
                    {
                        if (ignoreCloseCodeBlocks > 0)
                        {
                            ignoreCloseCodeBlocks--;
                            goto default;
                        }
                        else
                        {
                            awaitCloseCodeBlock = false;
                        }
                    }
                    break;
                
                case '\n':
                case '\r':
                case ' ':
                    if (awaitCloseChevron || awaitCloseCodeBlock)
                        goto default;
                    
                    if (currentSegment.Length > 0)
                        split.Add(currentSegment.ToString());
                    
                    currentSegment.Clear();
                    break;
                
                default:
                    currentSegment.Append(c);
                    break;
            }
        }
        
        if (currentSegment.Length > 0)
            split.Add(currentSegment.ToString());

        return split.ToArray();
    }
    
    /// <param name="allClasses">The pool of classes to search from</param>
    public void AddTypeInterpretters(IEnumerable<TypeInfo>? allClasses = default)
    {
        allClasses ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
            .DefinedTypes;

        foreach (TypeInfo typeInfo in allClasses)
        {
            if (!typeof(ITypeInterpreter).IsAssignableFrom(typeInfo) // isn't a type interpreter
                || typeInfo.AsType() == typeof(ITypeInterpreter))    // or is ITypeInterpreter itself
                continue;

            ITypeInterpreter interpreterInstance = (ITypeInterpreter) Activator.CreateInstance(typeInfo)!;
            
            TypeInterpreterModules.Add(interpreterInstance);
        }
    }
    
    /// <param name="allMethods">The pool of methods to search from</param>
    public void AddCommandsUsingAttribute(IEnumerable<MethodInfo>? allMethods = default)
    {
        allMethods ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())
            .DefinedTypes
            .SelectMany(x => x.DeclaredMethods);

        foreach (MethodInfo methodInfo in allMethods)
        {
            if (methodInfo.GetCustomAttribute<MMCommandAttribute>() is not { } attr)
                continue; // not using attribute
            
            if (methodInfo.GetParameters().Any(x => x.ParameterType == typeof(object)))
                throw new Exception("Imprecise type [Object] is invalid");
            
            attr.Name ??= methodInfo.Name;
            attr.Command = Command.FromMethodInfo(methodInfo, false/*!Debugger.IsAttached // This causes crash and i dont feel like dealing with it*/);

            AddCommand(attr);
        }
    }

    /// <param name="command" />
    /// <param name="description">Describes the usage of this command</param>
    /// <param name="hidden">
    /// Whether or not this command is marked as "hidden".
    /// Doesn't matter by default, but can be used by your implementation
    /// </param>
    public void AddCommand(Command command, string? description = null, bool hidden = false)
    {
        AddCommand(new MMCommandAttribute()
        {
            Name = command.Name,
            Hidden = hidden,
            Description = description,
            Command = command
        });
    }

    public void RemoveCommand(string commandName)
    {
        Commands.RemoveAll(x => string.Equals(x.Name, commandName, StringComparison.InvariantCultureIgnoreCase));
    }

    private void AddCommand(MMCommandAttribute commandAttribute)
    {
        if (!ValidNameRegex.IsMatch(commandAttribute.Name))
            throw new Exception($"Invalid command name: {commandAttribute.Name}");

        if (Commands.Any(x =>
                x.Name == commandAttribute.Name &&
                x.Command.Parameters.SequenceEqual(commandAttribute.Command.Parameters)))
            throw new Exception($"Duplicate command signature: {commandAttribute.Name}");

        Commands.Add(commandAttribute);
    }
}