using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite
{
    /// <summary>
    /// Represents the result of parsing a command string, containing the command name and its arguments.
    /// </summary>
    public class CommandParseResult
    {
        /// <summary>
        /// Gets the name of the parsed command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the list of arguments parsed from the command string.
        /// </summary>
        public IReadOnlyList<object?> Arguments { get; }

        /// <summary>
        /// Initializes a new instance of the CommandParseResult class.
        /// </summary>
        /// <param name="command">The command name.</param>
        /// <param name="arguments">The list of arguments.</param>
        public CommandParseResult(string command, IReadOnlyList<object?> arguments)
        {
            Command = command;
            Arguments = arguments;
        }
    }
}
