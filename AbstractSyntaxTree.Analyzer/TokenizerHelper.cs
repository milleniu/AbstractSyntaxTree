using System;
using System.Text;
using AbstractSyntaxTree.Model;

namespace AbstractSyntaxTree.Analyzer
{
    public static class TokenizerHelper
    {
        public static void DumpTokens( string input )
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append( $"Tokens in {input}: " );
            var tokenizer = new StringTokenizer( input );
            var token = tokenizer.GetNextToken();
            while( token != TokenType.EndOfInput && token != TokenType.Error )
            {
                stringBuilder.Append( token );

                if( tokenizer.MatchDouble( out var d ) ) stringBuilder.Append( $" ({d})" );
                else if( tokenizer.MatchString( out var s ) ) stringBuilder.Append( $" ({s})" );
                else if( tokenizer.MatchIdentifier( out var id ) ) stringBuilder.Append( $" ({id})" );
                // [...]
                else tokenizer.GetNextToken();

                token = tokenizer.CurrentToken;
                stringBuilder.Append( ", " );
            }
            stringBuilder.Append( token == TokenType.Error ? "<error>" : "<eos>" );
            Console.WriteLine( stringBuilder.ToString() );
        }
    }
}
