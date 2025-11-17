using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite.Lexer
{
    class FalseReader : ITokenReader
    {
        public bool GetIsMatch(TokenReader reader)
        {
            return reader.ConsumeKeyword("false", true);
        }

        public TokenType GetTokenType() => TokenType.False;
    }
}
