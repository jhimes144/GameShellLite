using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    class IdentifierReader : ITokenReader
    {
        public bool GetIsMatch(TokenReader reader)
        {
            var foundDigitOrLetter = false;

            while (true)
            {
                var c = reader.Peek();

                if (char.IsDigit(c) || char.IsLetter(c) || c == '_')
                {
                    foundDigitOrLetter = true;
                    reader.Consume();
                    continue;
                }

                if (foundDigitOrLetter)
                {
                    return true;
                }

                return false;
            }
        }

        public TokenType GetTokenType() => TokenType.Identifier;
    }
}


