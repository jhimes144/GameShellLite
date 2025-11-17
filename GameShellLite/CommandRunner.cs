using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameShellLite
{
    /// <summary>
    /// Manages command registration and execution, providing a fluent API for building commands with typed arguments.
    /// </summary>
    public class CommandRunner
    {
        readonly List<CommandData> _commands = new List<CommandData>();
        readonly CommandParserOptions _parserOptions;
        readonly bool _caseInsensitive;

        /// <summary>
        /// Initializes a new instance of the CommandRunner class.
        /// </summary>
        /// <param name="caseInsensitiveCommandNames">Whether command names should be case-insensitive. Default is false.</param>
        /// <param name="parserOptions">Optional parser options. If null, default options are used.</param>
        public CommandRunner(bool caseInsensitiveCommandNames = false, CommandParserOptions? parserOptions = null)
        {
            _caseInsensitive = caseInsensitiveCommandNames;
            _parserOptions = parserOptions ?? new CommandParserOptions();
        }

        /// <summary>
        /// Registers a new command with the specified name.
        /// </summary>
        /// <param name="name">The name of the command to register.</param>
        /// <returns>A CommandBuilder instance for configuring the command.</returns>
        /// <exception cref="CommandRunnerException">Thrown when a command with the same name or alias already exists, or when the name is invalid.</exception>
        public CommandBuilder RegisterCommand(string name)
        {
            if (GetIsValidCommand(name))
            {
                throw new CommandRunnerException($"A command already exists with name or alias '{name}'");
            }

            ValidateCommandName(name);
            var builder = new CommandBuilder(this, _parserOptions.NumberPrecision, name);
            _commands.Add(builder.Data);
            return builder;
        }

        /// <summary>
        /// Removes a command with the specified name.
        /// </summary>
        /// <param name="name">The name of the command to remove.</param>
        /// <returns>True if the command was found and removed; otherwise, false.</returns>
        public bool RemoveCommand(string name)
        {
            var command = _commands.FirstOrDefault(x => x.Name == name);

            if (command != null)
            {
                _commands.Remove(command);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all registered commands.
        /// </summary>
        public void ClearCommands()
        {
            _commands.Clear();
        }

        /// <summary>
        /// Determines whether a command with the specified name or alias exists.
        /// </summary>
        /// <param name="commandName">The command name or alias to check.</param>
        /// <returns>True if the command exists; otherwise, false.</returns>
        public bool GetIsValidCommand(string commandName)
        {
            return _commands.Any(c => isMatchingCommand(commandName, c));
        }

        /// <summary>
        /// Gets formatted help text for the specified command.
        /// </summary>
        /// <param name="commandName">The command name or alias to get help for.</param>
        /// <returns>The formatted help text, or null if the command does not exist.</returns>
        public string? GetHelpPrintFor(string commandName)
        {
            var command = _commands.FirstOrDefault(c => isMatchingCommand(commandName, c));

            if (command == null)
            {
                return null;
            }

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("-- " + command.Name + " --");

            if (command.Aliases.Any())
            {
                strBuilder.AppendLine("(or):" + string.Join(",", command.Aliases));
            }

            strBuilder.AppendLine();
            strBuilder.AppendLine(command.HelpDescription);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Parses and executes a command string.
        /// </summary>
        /// <param name="commandStr">The command string to execute.</param>
        /// <exception cref="CommandRunnerNotFoundException">Thrown when the command does not exist.</exception>
        /// <exception cref="CommandRunnerException">Thrown when the command has not been implemented, or when argument validation fails.</exception>
        /// <exception cref="CommandParserException">Thrown when the command string contains invalid syntax.</exception>
        public void Execute(string commandStr)
        {
            var parsed = CommandParser.Parse(commandStr, _parserOptions);
            var command = _commands.FirstOrDefault(c => isMatchingCommand(parsed.Command, c));

            if (command == null)
            {
                throw new CommandRunnerNotFoundException(commandStr, $"Command '{parsed.Command}' does not exist.");
            }

            if (command.Execution == null && command.DirectExecution == null)
            {
                throw new CommandRunnerException($"Command '{parsed.Command}' has not been implemented.");
            }

            if (command.DirectExecution != null)
            {
                command.DirectExecution(parsed);
            }
            else if (command.Execution is Action action)
            {
                action();
            }
            else
            {
                var actionType = command.Execution!.GetType();
                var argTypes = actionType.GetGenericArguments();

                if (parsed.Arguments.Count > argTypes.Length)
                {
                    throw new CommandRunnerException($"Command '{parsed.Command}' takes a max of {argTypes.Length}" +
                        $" argument(s) but {parsed.Arguments.Count} were supplied.");
                }

                var requiredArgCount = command.OptionalArgFlags
                    .Where(o => !o)
                    .Count();

                if (parsed.Arguments.Count < requiredArgCount)
                {
                    throw new CommandRunnerException($"Command '{parsed.Command}' takes a min of {requiredArgCount}" +
                        $" argument(s) but {parsed.Arguments.Count} were supplied.");
                }

                var argsToPass = new object?[argTypes.Length];

                for (var i = 0; i < argTypes.Length; i++)
                {
                    if (parsed.Arguments.Count > i)
                    {
                        if (parsed.Arguments[i] == null)
                        {
                            argsToPass[i] = getDefaultValue(argTypes[i]);
                        }
                        else if (argTypes[i].IsAssignableFrom(parsed.Arguments[i]!.GetType()))
                        {
                            argsToPass[i] = parsed.Arguments[i];
                        }
                        else
                        {
                            throw new CommandRunnerException($"Arg #{i + 1}: Wrong type supplied. Got '{getTypeName(parsed.Arguments[i]!.GetType())}'" +
                                $" but was expecting '{getTypeName(argTypes[i])}'");
                        }
                    }
                    else
                    {
                        // optional arg
                        argsToPass[i] = getDefaultValue(argTypes[i]);
                    }
                }

                var executionInvokeMethod = actionType.GetMethod("Invoke");
                executionInvokeMethod.Invoke(command.Execution, argsToPass);
            }
        }

        bool isMatchingCommand(string commandNameInput, CommandData commandData)
        {
            if (commandEq(commandNameInput, commandData.Name))
            {
                return true;
            }

            if (commandData.Aliases.Any(a => commandEq(commandNameInput, a)))
            {
                return true;
            }

            return false;
        }

        bool commandEq(string input, string command)
        {
            if (_caseInsensitive)
            {
                return string.Equals(input, command, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return input == command;
            }
        }

        static object? getDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        static string getTypeName(Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(bool))
            {
                return "bool";
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return "number";
            }

            return "unknown";
        }

        internal static void ValidateCommandName(string commandName)
        {
            for (var i = 0; i < commandName.Length; i++)
            {
                var c = commandName[i];

                if (char.IsDigit(c) || char.IsLetter(c) || c == '_')
                {
                    continue;
                }

                throw new CommandRunnerException("Command name is invalid. Command names can only be digits, underscores, or letters.");
            }
        }
    }
}
