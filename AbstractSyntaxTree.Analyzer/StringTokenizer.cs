using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Analyzer
{
    public class StringTokenizer : ITokenizer
    {
        private readonly string _toParse;
        private readonly int _maxPosition;
        private readonly StringBuilder _stringBuilder;

        private int _position;
        private double _doubleValue;

        private bool IsEnd => _position >= _maxPosition;
        public TokenType CurrentToken { get; private set; }

        public StringTokenizer( string s )
            : this( s, 0, s.Length ) { }

        public StringTokenizer( string s, int startIndex )
            : this( s, startIndex, s.Length - startIndex ) { }

        public StringTokenizer( string s, int startIndex, int count )
        {
            CurrentToken = TokenType.None;
            _toParse = s;
            _position = startIndex;
            _maxPosition = startIndex + count;
            _stringBuilder = new StringBuilder();
        }

        public static IEnumerable<TokenType> Parse( string toParse )
        {
            var tokenizer = new StringTokenizer( toParse );
            while( tokenizer.GetNextToken() != TokenType.EndOfInput )
                yield return tokenizer.CurrentToken;
        }

        public TokenType GetNextToken()
        {
            if( IsEnd ) return CurrentToken = TokenType.EndOfInput;

            char c;
            var lineComment = false;
            var blockComment = false;

            while( lineComment || blockComment || char.IsWhiteSpace( c = Read() )
                || c == '/' && !IsEnd
                            && ((lineComment = Peek() == '/') || (blockComment = Peek() == '*') && Read() == '*') )
            {
                if( IsEnd ) return CurrentToken = blockComment ? TokenType.Error : TokenType.EndOfInput;
                if( !lineComment && !blockComment ) continue;

                c = Read();
                if( c == '\n' ) lineComment = false;
                if( c != '*' || Peek() != '/' ) continue;
                Forward();
                blockComment = false;
            }

            switch( c )
            {
                case '+': return CurrentToken = TokenType.Plus;
                case '-': return CurrentToken = TokenType.Minus;
                case '*': return CurrentToken = TokenType.Multiplicative;
                case '/': return CurrentToken = TokenType.Divide;
                case '(': return CurrentToken = TokenType.OpenParenthesis;
                case ')': return CurrentToken = TokenType.CloseParenthesis;
                case ';': return CurrentToken = TokenType.SemiColon;
                case ',': return CurrentToken = TokenType.Comma;
                case '.': return CurrentToken = TokenType.Dot;
                case '[': return CurrentToken = TokenType.OpenSquare;
                case ']': return CurrentToken = TokenType.CloseSquare;
                case '{': return CurrentToken = TokenType.OpenBracket;
                case '}': return CurrentToken = TokenType.CloseBracket;
                case '?': return CurrentToken = TokenType.QuestionMark;

                case ':':
                    if( IsEnd || Peek() != ':' ) return CurrentToken = TokenType.Colon;
                    Forward();
                    return CurrentToken = TokenType.DoubleColon;

                case '"':
                    _stringBuilder.Clear();
                    while( !IsEnd && (c = Read()) != '"' )
                        _stringBuilder.Append( c );
                    return CurrentToken = TokenType.String;

                default:

                    if( char.IsDigit( c ) )
                    {
                        // We directly calculate the value in case of int
                        // For double we will use the string builder
                        CurrentToken = TokenType.Integer;
                        _stringBuilder.Clear();

                        double value = c - '0';
                        _stringBuilder.Append( c );

                        while( !IsEnd && char.IsDigit( Peek() ) )
                        {
                            value = 10.0 * value + ( c = Read() ) - '0';
                            _stringBuilder.Append( c );
                        }

                        if( IsEnd )
                        {
                            _doubleValue = value;
                            return CurrentToken;
                        }

                        if( Peek() == '.' )
                        {
                            CurrentToken = TokenType.Double;
                            _stringBuilder.Append( Read() );

                            if( !char.IsDigit( c = Peek() ) && c != 'e' && c != 'E' ) return CurrentToken = TokenType.Error;
                            var exp = c == 'e' || c == 'E';
                            do
                            {
                                _stringBuilder.Append( Read() );
                            } while( !IsEnd && (char.IsDigit( c = Peek() ) || !exp && (c == 'e' || c == 'E')) );
                        }

                        _doubleValue = value;
                    }
                    else if( c == '_' || char.IsLetter( c ) )
                    {
                        _stringBuilder.Clear();
                        _stringBuilder.Append( c );
                        while( !IsEnd && ((c = Peek()) == '_' || char.IsLetterOrDigit( c )) )
                        {
                            _stringBuilder.Append( Read() );
                        }

                        return CurrentToken = TokenType.Identifier;
                    }
                    else
                        CurrentToken = TokenType.Error;

                    break;
            }

            return CurrentToken;
        }

        public bool Match( TokenType tokenType )
        {
            if( CurrentToken == tokenType )
            {
                GetNextToken();
                return true;
            }

            return false;
        }

        public bool MatchInteger( int expectedValue )
        {
            if( CurrentToken == TokenType.Integer
             && _doubleValue < int.MaxValue
             && (int) _doubleValue == expectedValue )
            {
                GetNextToken();
                return true;
            }

            return false;
        }

        public bool MatchInteger( out int value )
        {
            if( CurrentToken == TokenType.Integer
             && _doubleValue < int.MaxValue )
            {
                value = (int) _doubleValue;
                GetNextToken();
                return true;
            }

            value = 0;
            return false;
        }

        public bool MatchDouble( out double value )
        {
            if( (CurrentToken & TokenType.IsNumber) != TokenType.None )
            {
                if( !double.TryParse( _stringBuilder.ToString().AsSpan(), out value ) ) return false;
                GetNextToken();
                return true;
            }

            value = 0;
            return false;
        }

        public bool MatchIdentifier( string id )
        {
            if( CurrentToken == TokenType.Identifier
             && _stringBuilder.Equals( id.AsSpan() ) )
            {
                GetNextToken();
                return true;
            }

            return false;
        }

        public bool MatchIdentifier( out string id )
        {
            if( CurrentToken == TokenType.Identifier )
            {
                id = _stringBuilder.ToString();
                GetNextToken();
                return true;
            }

            id = string.Empty;
            return false;
        }

        public bool MatchString( out string value )
        {
            if( CurrentToken == TokenType.String )
            {
                value = _stringBuilder.ToString();
                GetNextToken();
                return true;
            }

            value = string.Empty;
            return false;
        }

        public TokenType GetCurrentAndForward()
        {
            var tokenType = CurrentToken;
            GetNextToken();
            return tokenType;
        }

        private char Peek()
        {
            Debug.Assert( !IsEnd );
            return _toParse[ _position ];
        }

        private char Read()
        {
            Debug.Assert( !IsEnd );
            return _toParse[ _position++ ];
        }

        private void Forward()
        {
            Debug.Assert( !IsEnd );
            ++_position;
        }
    }
}
