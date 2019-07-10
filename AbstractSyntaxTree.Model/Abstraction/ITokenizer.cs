namespace AbstractSyntaxTree.Model.Abstraction
{
    public interface ITokenizer
    {
        TokenType CurrentToken { get; }

        TokenType GetNextToken();

        TokenType GetCurrentAndForward();

        /// <summary>
        /// Checks that the <see cref="CurrentToken"/> is equal to
        /// <paramref name="type"/> and forwards the head on success.
        /// </summary>
        /// <param name="type">Type of the expected token.</param>
        /// <returns><c>true</c> if token is of the given type; otherwise <c>false</c>.</returns>
        bool Match( TokenType type );

        bool MatchInteger( int expected );
        bool MatchInteger( out int expected );
        bool MatchDouble( out double expected );

        bool MatchIdentifier( string id );
        bool MatchIdentifier( out string id );

        bool MatchString( out string value );
    }
}
