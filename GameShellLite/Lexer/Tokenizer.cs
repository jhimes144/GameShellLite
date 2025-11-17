using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    static class Tokenizer
    {
        public static IReadOnlyList<Token> GetTokens(string command)
        {
            var strReader = new StringInputReader(command);
            var tokenReader = new TokenReader(strReader);
            var tokens = new List<Token>();

            var tokenReaders = new ITokenReader[]
            {
                new TrueReader(),
                new FalseReader(),
                new NullReader(),
                new NumberReader(),
                new StringReader(),
                new IdentifierReader(),

            };

            if (!strReader.IsAtEnd())
            {
                strReader.AdvanceRemainingWhiteSpace();
            }

            while (!strReader.IsAtEnd())
            {
                var tokenFound = false;

                foreach (var reader in tokenReaders)
                {
                    tokenReader.Reset();
                    var match = reader.GetIsMatch(tokenReader);

                    if (match)
                    {
                        tokens.Add(new Token(reader.GetTokenType(),
                            tokenReader.ToString(), strReader.Line, strReader.Pos));

                        strReader.Advance(tokenReader.Index);
                        strReader.AdvanceRemainingWhiteSpace();
                        tokenFound = true;
                        break;
                    }
                }

                if (!tokenFound)
                {
                    throw new Exception("Unexpected character(s) in command.");
                }
            }

            tokens.Add(new Token(TokenType.EOF, string.Empty, strReader.Line, strReader.Column));
            return tokens;
        }
    }
}
