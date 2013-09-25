#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Text;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Hand-written tokenizer for conditions.
    /// </summary>
    internal sealed class ConditionTokenizer
    {
        private static readonly ConditionTokenType[] charIndexToTokenType = BuildCharIndexToTokenType();
        private readonly SimpleStringReader stringReader;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionTokenizer" /> class.
        /// </summary>
        /// <param name="stringReader">The string reader.</param>
        public ConditionTokenizer(SimpleStringReader stringReader)
        {
            this.stringReader = stringReader;
            TokenType = ConditionTokenType.BeginningOfInput;
            GetNextToken();
        }

        /// <summary>
        ///     Gets the token position.
        /// </summary>
        /// <value>The token position.</value>
        public int TokenPosition { get; private set; }

        /// <summary>
        ///     Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public ConditionTokenType TokenType { get; private set; }

        /// <summary>
        ///     Gets the token value.
        /// </summary>
        /// <value>The token value.</value>
        public string TokenValue { get; private set; }

        /// <summary>
        ///     Gets the value of a string token.
        /// </summary>
        /// <value>The string token value.</value>
        public string StringTokenValue
        {
            get
            {
                var s = TokenValue;

                return s.Substring(1, s.Length - 2).Replace("''", "'");
            }
        }

        /// <summary>
        ///     Asserts current token type and advances to the next token.
        /// </summary>
        /// <param name="tokenType">Expected token type.</param>
        /// <remarks>If token type doesn't match, an exception is thrown.</remarks>
        public void Expect(ConditionTokenType tokenType)
        {
            if (TokenType != tokenType)
            {
                throw new ConditionParseException("Expected token of type: " + tokenType + ", got " + TokenType + " (" + TokenValue + ").");
            }

            GetNextToken();
        }

        /// <summary>
        ///     Asserts that current token is a keyword and returns its value and advances to the next token.
        /// </summary>
        /// <returns>Keyword value.</returns>
        public string EatKeyword()
        {
            if (TokenType != ConditionTokenType.Keyword)
            {
                throw new ConditionParseException("Identifier expected");
            }

            var s = TokenValue;
            GetNextToken();
            return s;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether current keyword is equal to the specified value.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        ///     A value of <c>true</c> if current keyword is equal to the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKeyword(string keyword)
        {
            if (TokenType != ConditionTokenType.Keyword)
            {
                return false;
            }

            if (!TokenValue.Equals(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the tokenizer has reached the end of the token stream.
        /// </summary>
        /// <returns>
        ///     A value of <c>true</c> if the tokenizer has reached the end of the token stream; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEOF()
        {
            if (TokenType != ConditionTokenType.EndOfInput)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether current token is a number.
        /// </summary>
        /// <returns>
        ///     A value of <c>true</c> if current token is a number; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNumber()
        {
            return TokenType == ConditionTokenType.Number;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the specified token is of specified type.
        /// </summary>
        /// <param name="tokenType">The token type.</param>
        /// <returns>
        ///     A value of <c>true</c> if current token is of specified type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsToken(ConditionTokenType tokenType)
        {
            return TokenType == tokenType;
        }

        /// <summary>
        ///     Gets the next token and sets <see cref="TokenType" /> and <see cref="TokenValue" /> properties.
        /// </summary>
        public void GetNextToken()
        {
            if (TokenType == ConditionTokenType.EndOfInput)
            {
                throw new ConditionParseException("Cannot read past end of stream.");
            }

            SkipWhitespace();

            TokenPosition = TokenPosition;

            var i = PeekChar();
            if (i == -1)
            {
                TokenType = ConditionTokenType.EndOfInput;
                return;
            }

            var ch = (char) i;

            if (char.IsDigit(ch))
            {
                ParseNumber(ch);
                return;
            }

            if (ch == '\'')
            {
                ParseSingleQuotedString(ch);
                return;
            }

            if (ch == '_' || char.IsLetter(ch))
            {
                ParseKeyword(ch);
                return;
            }

            if (ch == '}' || ch == ':')
            {
                // when condition is embedded
                TokenType = ConditionTokenType.EndOfInput;
                return;
            }

            TokenValue = ch.ToString();

            if (ch == '<')
            {
                ReadChar();
                var nextChar = PeekChar();

                if (nextChar == '>')
                {
                    TokenType = ConditionTokenType.NotEqual;
                    TokenValue = "<>";
                    ReadChar();
                    return;
                }

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.LessThanOrEqualTo;
                    TokenValue = "<=";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.LessThan;
                TokenValue = "<";
                return;
            }

            if (ch == '>')
            {
                ReadChar();
                var nextChar = PeekChar();

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.GreaterThanOrEqualTo;
                    TokenValue = ">=";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.GreaterThan;
                TokenValue = ">";
                return;
            }

            if (ch == '!')
            {
                ReadChar();
                var nextChar = PeekChar();

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.NotEqual;
                    TokenValue = "!=";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.Not;
                TokenValue = "!";
                return;
            }

            if (ch == '&')
            {
                ReadChar();
                var nextChar = PeekChar();
                if (nextChar == '&')
                {
                    TokenType = ConditionTokenType.And;
                    TokenValue = "&&";
                    ReadChar();
                    return;
                }

                throw new ConditionParseException("Expected '&&' but got '&'");
            }

            if (ch == '|')
            {
                ReadChar();
                var nextChar = PeekChar();
                if (nextChar == '|')
                {
                    TokenType = ConditionTokenType.Or;
                    TokenValue = "||";
                    ReadChar();
                    return;
                }

                throw new ConditionParseException("Expected '||' but got '|'");
            }

            if (ch == '=')
            {
                ReadChar();
                var nextChar = PeekChar();

                if (nextChar == '=')
                {
                    TokenType = ConditionTokenType.EqualTo;
                    TokenValue = "==";
                    ReadChar();
                    return;
                }

                TokenType = ConditionTokenType.EqualTo;
                TokenValue = "=";
                return;
            }

            if (ch >= 32 && ch < 128)
            {
                var tt = charIndexToTokenType[ch];

                if (tt != ConditionTokenType.Invalid)
                {
                    TokenType = tt;
                    TokenValue = new string(ch, 1);
                    ReadChar();
                    return;
                }

                throw new ConditionParseException("Invalid punctuation: " + ch);
            }

            throw new ConditionParseException("Invalid token: " + ch);
        }

        private static ConditionTokenType[] BuildCharIndexToTokenType()
        {
            CharToTokenType[] charToTokenType =
                {
                    new CharToTokenType('(', ConditionTokenType.LeftParen),
                    new CharToTokenType(')', ConditionTokenType.RightParen),
                    new CharToTokenType('.', ConditionTokenType.Dot),
                    new CharToTokenType(',', ConditionTokenType.Comma),
                    new CharToTokenType('!', ConditionTokenType.Not),
                    new CharToTokenType('-', ConditionTokenType.Minus),
                };

            var result = new ConditionTokenType[128];

            for (var i = 0; i < 128; ++i)
            {
                result[i] = ConditionTokenType.Invalid;
            }

            foreach (var cht in charToTokenType)
            {
                // Console.WriteLine("Setting up {0} to {1}", cht.ch, cht.tokenType);
                result[cht.Character] = cht.TokenType;
            }

            return result;
        }

        private void ParseSingleQuotedString(char ch)
        {
            int i;
            TokenType = ConditionTokenType.String;

            var sb = new StringBuilder();

            sb.Append(ch);
            ReadChar();

            while ((i = PeekChar()) != -1)
            {
                ch = (char) i;

                sb.Append((char) ReadChar());

                if (ch == '\'')
                {
                    if (PeekChar() == '\'')
                    {
                        sb.Append('\'');
                        ReadChar();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (i == -1)
            {
                throw new ConditionParseException("String literal is missing a closing quote character.");
            }

            TokenValue = sb.ToString();
        }

        private void ParseKeyword(char ch)
        {
            int i;
            TokenType = ConditionTokenType.Keyword;

            var sb = new StringBuilder();

            sb.Append(ch);

            ReadChar();

            while ((i = PeekChar()) != -1)
            {
                if ((char) i == '_' || (char) i == '-' || char.IsLetterOrDigit((char) i))
                {
                    sb.Append((char) ReadChar());
                }
                else
                {
                    break;
                }
            }

            TokenValue = sb.ToString();
        }

        private void ParseNumber(char ch)
        {
            int i;
            TokenType = ConditionTokenType.Number;
            var sb = new StringBuilder();

            sb.Append(ch);
            ReadChar();

            while ((i = PeekChar()) != -1)
            {
                ch = (char) i;

                if (char.IsDigit(ch) || (ch == '.'))
                {
                    sb.Append((char) ReadChar());
                }
                else
                {
                    break;
                }
            }

            TokenValue = sb.ToString();
        }

        private void SkipWhitespace()
        {
            int ch;

            while ((ch = PeekChar()) != -1)
            {
                if (!char.IsWhiteSpace((char) ch))
                {
                    break;
                }

                ReadChar();
            }
        }

        private int PeekChar()
        {
            return stringReader.Peek();
        }

        private int ReadChar()
        {
            return stringReader.Read();
        }

        /// <summary>
        ///     Mapping between characters and token types for punctuations.
        /// </summary>
        private struct CharToTokenType
        {
            public readonly char Character;
            public readonly ConditionTokenType TokenType;

            /// <summary>
            ///     Initializes a new instance of the CharToTokenType struct.
            /// </summary>
            /// <param name="character">The character.</param>
            /// <param name="tokenType">Type of the token.</param>
            public CharToTokenType(char character, ConditionTokenType tokenType)
            {
                Character = character;
                TokenType = tokenType;
            }
        }
    }
}