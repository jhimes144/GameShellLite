using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    class TokenReader
    {
        readonly StringInputReader _input;
        readonly StringBuilder _tokenBuffer = new StringBuilder();

        public int Index { get; private set; }

        public TokenReader(StringInputReader reader)
        {
            _input = reader;
        }

        public void Reset()
        {
            Index = 0;
            _tokenBuffer.Clear();
        }

        public bool IsAtEnd()
        {
            return _input.IsAtEnd();
        }

        public char Consume()
        {
            var c = _input.Peek(Index);
            Index++;
            _tokenBuffer.Append(c);
            return c;
        }

        public char Skip()
        {
            var c = _input.Peek(Index);
            Index++;
            return c;
        }

        public char Peek()
        {
            return _input.Peek(Index);
        }

        public bool PeakIsDelimiter()
        {
            var c = _input.Peek(Index);
            return !char.IsLetterOrDigit(c) && c != '_';
        }

        public bool ConsumeKeyword(string keyword, bool caseInsensitive)
        {
            var hasKeyword = ConsumeMatch(keyword, caseInsensitive);

            if (hasKeyword)
            {
                return PeakIsDelimiter();
            }

            return false;
        }

        public bool ConsumeMatch(string match, bool caseInsensitive)
        {
            for (int i = 0; i < match.Length; i++)
            {
                char c = Consume();

                if (caseInsensitive)
                {
                    if (c == '\0' || char.ToLowerInvariant(c) != char.ToLowerInvariant(match[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    if (c == '\0' || c != match[i])
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        public override string ToString()
        {
            return _tokenBuffer.ToString();
        }
    }
}
