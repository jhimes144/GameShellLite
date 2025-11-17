using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    interface ITokenReader
    {
        bool GetIsMatch(TokenReader reader);
        TokenType GetTokenType();
    }
}
