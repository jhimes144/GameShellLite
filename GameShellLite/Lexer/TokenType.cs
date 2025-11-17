using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    enum TokenType
    {
        Number,
        String,
        Identifier,
        False,
        True,
        Null,
        EOF,
    }
}
