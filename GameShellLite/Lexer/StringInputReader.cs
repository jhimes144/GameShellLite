using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShellLite.Lexer
{
    class StringInputReader
    {
        readonly string _input;
        int _pos;
        int _line;
        int _column;
        bool _atNewLine;

        public int Pos => _pos;
        public int Line => _line;
        public int Column => _column;

        public StringInputReader(string input)
        {
            _input = input;
        }

        public char Peek(int offset)
        {
            if (_pos + offset >= _input.Length)
            {
                return '\0';
            }

            return _input[_pos + offset];
        }

        public bool Advance(int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (IsAtEnd())
                {
                    return false;
                }

                if (_atNewLine)
                {
                    _line++;
                    _atNewLine = false;
                }

                var c = _input[_pos];
                _pos++;
                _column++;

                if (c == '\n')
                {
                    _atNewLine = true;
                }
            }

            return true;
        }

        public bool AdvanceRemainingWhiteSpace()
        {
            while (true)
            {
                if (IsAtEnd())
                {
                    return false;
                }

                if (_atNewLine)
                {
                    _line++;
                    _atNewLine = false;
                }

                var c = _input[_pos];

                if (!char.IsWhiteSpace(c))
                {
                    return true;
                }

                _pos++;
                _column++;

                if (c == '\n')
                {
                    _atNewLine = true;
                }
            }
        }

        public bool IsAtEnd() => _pos >= _input.Length;
    }
}
