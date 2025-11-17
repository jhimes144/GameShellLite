using GameShellLite.Lexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite
{
    /// <summary>
    /// Provides static methods for parsing command strings into structured CommandParseResult objects.
    /// </summary>
    public static class CommandParser
    {
        /// <summary>
        /// The default parser options used when none are specified.
        /// </summary>
        public static CommandParserOptions _defaultOpt = new CommandParserOptions();

        /// <summary>
        /// Parses a command string into a CommandParseResult containing the command name and arguments.
        /// </summary>
        /// <param name="commandStr">The command string to parse.</param>
        /// <param name="options">Optional parser options. If null, default options are used.</param>
        /// <returns>A CommandParseResult containing the parsed command and arguments.</returns>
        /// <exception cref="CommandParserException">Thrown when the command string contains invalid syntax or unexpected characters.</exception>
        public static CommandParseResult Parse(string commandStr, CommandParserOptions? options = null)
        {
            commandStr = commandStr.Trim();

            options ??= _defaultOpt;
            var tokens = Tokenizer.GetTokens(commandStr);
            var current = 0;

            var command = consume(TokenType.Identifier, "Was expecting an identifier for command name.");
            var arguments = new List<object?>();

            while (!isAtEnd())
            {
                var arg = advance();

                if (arg.Type == TokenType.Identifier)
                {
                    arguments.Add(arg.Value);
                }
                else if (arg.Type == TokenType.String)
                {
                    arguments.Add(parseStringToken(arg.Value));
                }
                else if (arg.Type == TokenType.Number)
                {
                    switch (options.NumberPrecision)
                    {
                        case NumberPrecision.Float:
                            arguments.Add(float.Parse(arg.Value));
                            break;
                        case NumberPrecision.Double:
                            arguments.Add(double.Parse(arg.Value));
                            break;
                        case NumberPrecision.Decimal:
                            arguments.Add(decimal.Parse(arg.Value));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (arg.Type == TokenType.True)
                {
                    arguments.Add(true);
                }
                else if (arg.Type == TokenType.False)
                {
                    arguments.Add(false);
                }
                else if (arg.Type == TokenType.Null)
                {
                    if (!options.AllowNull)
                    {
                        throw new CommandParserException($"Unexpected character(s) '{arg.Value}'.");
                    }

                    arguments.Add(null);
                }
                else
                {
                    throw new CommandParserException($"Unexpected character(s) '{arg.Value}'.");
                }
            }

            return new CommandParseResult(command.Value, arguments);

            Token consume(TokenType type, string errorMessage)
            {
                if (check(type))
                {
                    return advance();
                }

                throw new Exception(errorMessage);
            }

            bool check(TokenType type)
            {
                if (isAtEnd())
                {
                    return false;
                }

                return peek().Type == type;
            }

            Token advance()
            {
                if (!isAtEnd())
                {
                    current++;
                }

                return previous();
            }

            bool isAtEnd()
            {
                return peek().Type == TokenType.EOF;
            }

            Token peek()
            {
                return tokens[current];
            }

            Token previous()
            {
                return tokens[current - 1];
            }
        }

        static string parseStringToken(string token)
        {
            var content = token.Substring(1, token.Length - 2);
            var result = new StringBuilder(content.Length);

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] == '\\' && i + 1 < content.Length)
                {
                    var nextChar = content[i + 1];
                    switch (nextChar)
                    {
                        case '\\':
                            result.Append('\\');
                            i++;
                            break;
                        case 'n':
                            result.Append('\n');
                            i++;
                            break;
                        default:
                            throw new CommandParserException($"{nextChar} is not a valid escape sequence.");
                    }
                }
                else if (content[i] == '\\')
                {
                    throw new CommandParserException("Invalid escape sequence at end of string.");
                }
                else
                {
                    result.Append(content[i]);
                }
            }

            return result.ToString();
        }
    }
}
