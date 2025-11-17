using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite.Lexer
{
    class NullReader : ITokenReader
    {
        public bool GetIsMatch(TokenReader reader)
        {
            return reader.ConsumeKeyword("null", true);
        }

        public TokenType GetTokenType() => TokenType.Null;
    }
}
