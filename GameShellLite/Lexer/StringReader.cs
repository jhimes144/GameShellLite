using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    class StringReader : ITokenReader
    {
        static readonly char[] _validEscapes = new char[]
        {
            '\\',
            'n'
        };

        public bool GetIsMatch(TokenReader reader)
        {
            var inside = false;

            while (true)
            {
                var c = reader.Consume();

                if (c == '"')
                {
                    if (!inside)
                    {
                        inside = true;
                        continue;
                    }

                    // Found closing quote
                    return true;
                }

                if (!inside)
                {
                    return false;
                }

                if (c == '\0')
                {
                    return false; // Unterminated string
                }

                if (c == '\\')
                {
                    // Skip the next character (it's escaped)

                    var escaped = reader.Consume();

                    if (!_validEscapes.Contains(escaped))
                    {
                        throw new Exception($"{escaped} is not a valid escape sequence.");
                    }

                    if (escaped == '\0')
                    {
                        throw new Exception("String was never terminated.");
                    }

                    continue;
                }
            }
        }

        public TokenType GetTokenType() => TokenType.String;
    }
}
