using System;
using System.Collections.Generic;
using System.Text;

namespace GameShellLite.Lexer
{
    class TrueReader : ITokenReader
    {
        public bool GetIsMatch(TokenReader reader)
        {
            return reader.ConsumeKeyword("true", true);
        }

        public TokenType GetTokenType() => TokenType.True;
    }
}
