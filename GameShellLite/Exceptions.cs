using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite
{
	/// <summary>
	/// Exception thrown when an error occurs during command registration or execution.
	/// </summary>
	public class CommandRunnerException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the CommandRunnerException class.
		/// </summary>
		public CommandRunnerException() { }
		/// <summary>
		/// Initializes a new instance of the CommandRunnerException class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public CommandRunnerException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the CommandRunnerException class with a specified error message and inner exception.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.</param>
		public CommandRunnerException(string message, Exception inner) : base(message, inner) { }
	}

    /// <summary>
    /// Exception thrown when a command is not found during execution.
    /// </summary>
    public class CommandRunnerNotFoundException : Exception
    {
        /// <summary>
        /// Gets the name of the command that was not found.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Initializes a new instance of the CommandRunnerNotFoundException class with the specified command name.
        /// </summary>
        /// <param name="commandName">The name of the command that was not found.</param>
        public CommandRunnerNotFoundException(string commandName)
        { 
            CommandName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the CommandRunnerNotFoundException class with the specified command name and error message.
        /// </summary>
        /// <param name="commandName">The name of the command that was not found.</param>
        /// <param name="message">The message that describes the error.</param>
        public CommandRunnerNotFoundException(string commandName, string message) : base(message) 
        { 
            CommandName += commandName;
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs during command string parsing.
    /// </summary>
    public class CommandParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CommandParserException class.
        /// </summary>
        public CommandParserException() { }
        /// <summary>
        /// Initializes a new instance of the CommandParserException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandParserException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the CommandParserException class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public CommandParserException(string message, Exception inner) : base(message, inner) { }
    }
}
