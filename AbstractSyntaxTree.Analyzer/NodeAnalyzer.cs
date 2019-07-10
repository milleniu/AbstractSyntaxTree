using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Analyzer
{
    public class NodeAnalyzer
    {
        public Node Parse( string expression )
        {
            var tokenizer = new StringTokenizer( expression );
            return tokenizer.GetNextToken() == TokenType.EndOfInput
                ? null
                : Parse( tokenizer );
        }

        public Node Parse( StringTokenizer tokenizer )
            => ParseTernaryExpression( tokenizer );

        private Node ParseTernaryExpression( ITokenizer tokenizer )
        {
            var condition = ParseExpression( tokenizer );
            if( !tokenizer.Match( TokenType.QuestionMark ) ) return condition;

            var truthly = ParseTernaryExpression( tokenizer );
            if( !tokenizer.Match( TokenType.Colon ) )
                return new TernaryNode( condition, truthly, new ErrorNode( "Expected :." ) );
            var falsy = ParseTernaryExpression( tokenizer );

            return new TernaryNode( condition, truthly, falsy );
        }

        private Node ParseExpression( ITokenizer tokenizer )
        {
            var expression = ParseTerm( tokenizer );

            while( (tokenizer.CurrentToken & TokenType.IsAdditiveOperator) != TokenType.None )
            {
                expression = new BinaryNode( tokenizer.GetCurrentAndForward(), expression, ParseTerm( tokenizer ) );
            }

            return expression;
        }

        private Node ParseTerm( ITokenizer tokenizer )
        {
            var factor = ParseFactor( tokenizer );

            while( (tokenizer.CurrentToken & TokenType.IsMultiplicativeOperator) != TokenType.None )
            {
                factor = new BinaryNode( tokenizer.GetCurrentAndForward(), factor, ParseFactor( tokenizer ) );
            }

            return factor;
        }

        private Node ParseFactor( ITokenizer tokenizer )
            => tokenizer.Match( TokenType.Minus )
                ? new UnaryNode( TokenType.Minus, ParsePositiveFactor( tokenizer ) )
                : ParsePositiveFactor( tokenizer );

        private Node ParsePositiveFactor( ITokenizer tokenizer )
        {
            if( tokenizer.MatchDouble( out var value ) ) return new ConstantNode( value );
            if( tokenizer.MatchIdentifier( out var id ) ) return new IdentifierNode( id );

            if( !tokenizer.Match( TokenType.OpenParenthesis ) ) return new ErrorNode( "Expected number or (." );
            var factor = ParseExpression( tokenizer );
            if( !tokenizer.Match( TokenType.CloseParenthesis ) ) return new ErrorNode( "Expected )." );
            return factor;
        }
    }
}
