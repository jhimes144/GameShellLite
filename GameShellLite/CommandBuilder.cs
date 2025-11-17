using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GameShellLite
{
    class CommandData
    {
        readonly List<Type> _validArgTypes;

        public CommandRunner Runner { get; }

        public string Name { get; set; }

        public string? HelpDescription { get; set; }

        public List<string> Aliases { get; set; } = new List<string>();

        public List<bool> OptionalArgFlags { get; set; } = new List<bool>();

        public object? Execution { get; set; }

        public Action<CommandParseResult>? DirectExecution { get; set; }

        public CommandData(CommandRunner runner, string name, NumberPrecision numberPrecision)
        {
            Runner = runner;
            Name = name;

            var validArgTypes = new List<Type>
            {
                typeof(string),
                typeof(bool),
                typeof(bool?)
            };

            switch (numberPrecision)
            {
                case NumberPrecision.Float:
                    validArgTypes.Add(typeof(float));
                    validArgTypes.Add(typeof(float?));
                    break;
                case NumberPrecision.Double:
                    validArgTypes.Add(typeof(double));
                    validArgTypes.Add(typeof(double?));
                    break;
                case NumberPrecision.Decimal:
                    validArgTypes.Add(typeof(decimal));
                    validArgTypes.Add(typeof(decimal?));
                    break;
                default:
                    break;
            }

            _validArgTypes = validArgTypes;
        }

        public void ValidateArgType(Type type)
        {
            if (!_validArgTypes.Contains(type))
            {
                throw new CommandRunnerException($"Invalid argument type {type}." +
                    $" Valid types are: {string.Join(",", _validArgTypes)}. Use NumberPrecision option to choose different numeric types.");
            }
        }

        public void ValidateOptional(bool optional)
        {
            if (!optional && OptionalArgFlags.Count > 0 && OptionalArgFlags.Last())
            {
                throw new CommandRunnerException("Cannot have non-optional argument after optional argument.");
            }
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with no arguments.
    /// </summary>
    public class CommandBuilder
    {
        internal CommandData Data { get; }

        internal CommandBuilder(CommandRunner runner, NumberPrecision numberPrecision, string name)
        {
            Data = new CommandData(runner, name, numberPrecision);
        }

        /// <summary>
        /// Adds an alias for the command being built.
        /// </summary>
        /// <param name="alias">The alias name for the command.</param>
        /// <returns>The current CommandBuilder instance for method chaining.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the alias is invalid or already exists.</exception>
        public CommandBuilder WithAlias(string alias)
        {
            CommandRunner.ValidateCommandName(alias);

            if (Data.Runner.GetIsValidCommand(alias))
            {
                throw new CommandRunnerException($"A command already exists with name or alias '{alias}'");
            }

            Data.Aliases.Add(alias);
            return this;
        }

        /// <summary>
        /// Sets the help description for the command.
        /// </summary>
        /// <param name="helpDescription">The help text to display for this command.</param>
        /// <returns>The current CommandBuilder instance for method chaining.</returns>
        public CommandBuilder WithHelp(string helpDescription)
        {
            Data.HelpDescription = helpDescription;
            return this;
        }

        /// <summary>
        /// Adds a typed argument to the command.
        /// </summary>
        /// <typeparam name="T1">The type of the argument (string, bool, or numeric type based on NumberPrecision).</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg1 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg1<T1> WithArg<T1>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T1));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg1<T1>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with no arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked.</param>
        public void WithExecution(Action executeAction)
        {
            Data.Execution = executeAction;
        }

        /// <summary>
        /// Sets the execution action for the command with direct access to the parse result.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the CommandParseResult.</param>
        public void WithExecution(Action<CommandParseResult> executeAction)
        {
            Data.DirectExecution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with one argument.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    public class CommandBuilderWArg1<T1>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg1(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a second typed argument to the command.
        /// </summary>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg2 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg2<T1, T2> WithArg<T2>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T2));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg2<T1, T2>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with one argument.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed argument.</param>
        public void WithExecution(Action<T1> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with two arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    public class CommandBuilderWArg2<T1, T2>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg2(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a third typed argument to the command.
        /// </summary>
        /// <typeparam name="T3">The type of the third argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg3 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg3<T1, T2, T3> WithArg<T3>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T3));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg3<T1, T2, T3>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with two arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with three arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    public class CommandBuilderWArg3<T1, T2, T3>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg3(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a fourth typed argument to the command.
        /// </summary>
        /// <typeparam name="T4">The type of the fourth argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg4 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg4<T1, T2, T3, T4> WithArg<T4>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T4));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg4<T1, T2, T3, T4>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with three arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with four arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    public class CommandBuilderWArg4<T1, T2, T3, T4>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg4(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a fifth typed argument to the command.
        /// </summary>
        /// <typeparam name="T5">The type of the fifth argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg5 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg5<T1, T2, T3, T4, T5> WithArg<T5>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T5));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg5<T1, T2, T3, T4, T5>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with four arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with five arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    public class CommandBuilderWArg5<T1, T2, T3, T4, T5>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg5(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a sixth typed argument to the command.
        /// </summary>
        /// <typeparam name="T6">The type of the sixth argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg6 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg6<T1, T2, T3, T4, T5, T6> WithArg<T6>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T6));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg6<T1, T2, T3, T4, T5, T6>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with five arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4, T5> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with six arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    public class CommandBuilderWArg6<T1, T2, T3, T4, T5, T6>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg6(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a seventh typed argument to the command.
        /// </summary>
        /// <typeparam name="T7">The type of the seventh argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg7 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg7<T1, T2, T3, T4, T5, T6, T7> WithArg<T7>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T7));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg7<T1, T2, T3, T4, T5, T6, T7>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with six arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4, T5, T6> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with seven arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    public class CommandBuilderWArg7<T1, T2, T3, T4, T5, T6, T7>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg7(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds an eighth typed argument to the command.
        /// </summary>
        /// <typeparam name="T8">The type of the eighth argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg8 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg8<T1, T2, T3, T4, T5, T6, T7, T8> WithArg<T8>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T8));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg8<T1, T2, T3, T4, T5, T6, T7, T8>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with seven arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4, T5, T6, T7> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with eight arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    public class CommandBuilderWArg8<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg8(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a ninth typed argument to the command.
        /// </summary>
        /// <typeparam name="T9">The type of the ninth argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg9 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg9<T1, T2, T3, T4, T5, T6, T7, T8, T9> WithArg<T9>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T9));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg9<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with eight arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4, T5, T6, T7, T8> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with nine arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    /// <typeparam name="T9">The type of the ninth argument.</typeparam>
    public class CommandBuilderWArg9<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg9(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Adds a tenth typed argument to the command.
        /// </summary>
        /// <typeparam name="T10">The type of the tenth argument.</typeparam>
        /// <param name="optional">Whether the argument is optional. Default is false.</param>
        /// <returns>A CommandBuilderWArg10 instance for further configuration.</returns>
        /// <exception cref="CommandRunnerException">Thrown when the argument type is invalid or optional arguments are incorrectly ordered.</exception>
        public CommandBuilderWArg10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithArg<T10>(bool optional = false)
        {
            Data.ValidateArgType(typeof(T10));
            Data.ValidateOptional(optional);
            Data.OptionalArgFlags.Add(optional);
            return new CommandBuilderWArg10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Data);
        }

        /// <summary>
        /// Sets the execution action for the command with nine arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> executeAction)
        {
            Data.Execution = executeAction;
        }
    }

    /// <summary>
    /// Provides a fluent API for building and configuring commands with ten arguments.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    /// <typeparam name="T9">The type of the ninth argument.</typeparam>
    /// <typeparam name="T10">The type of the tenth argument.</typeparam>
    public class CommandBuilderWArg10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        internal CommandData Data { get; }

        internal CommandBuilderWArg10(CommandData data)
        {
            Data = data;
        }

        /// <summary>
        /// Sets the execution action for the command with ten arguments.
        /// </summary>
        /// <param name="executeAction">The action to execute when the command is invoked, receiving the typed arguments.</param>
        public void WithExecution(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> executeAction)
        {
            Data.Execution = executeAction;
        }
    }
}
