using System;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Analyzer
{
    public static class ExpressionCalculator
    {
        public static double Compute( string expression )
        {
            var tokenizer = new StringTokenizer( expression );
            return tokenizer.GetNextToken() == TokenType.EndOfInput
                ? 0.0D
                : ComputeTernaryExpression( tokenizer );
        }

        /// <summary>
        /// ternaryExpression -> expression ? ternaryExpression : ternaryExpression
        /// </summary>
        private static double ComputeTernaryExpression( ITokenizer tokenizer )
        {
            var ternaryExpression = ComputeExpression( tokenizer );
            if( !tokenizer.Match( TokenType.QuestionMark ) ) return ternaryExpression;
            var truthlyDouble = ComputeTernaryExpression( tokenizer );
            if( !tokenizer.Match( TokenType.Colon ) ) throw new Exception( "Expected :." );
            var falsyDouble = ComputeTernaryExpression( tokenizer );
            return ternaryExpression > 0 ? truthlyDouble : falsyDouble;
        }

        /// <summary>
        /// expression → term  [operator-additive  term]* 
        /// </summary>
        private static double ComputeExpression( ITokenizer tokenizer )
        {
            var expression = ComputeTerm( tokenizer );
            while( (tokenizer.CurrentToken & TokenType.IsAdditiveOperator) != TokenType.None )
            {
                if( tokenizer.Match( TokenType.Plus ) ) expression += ComputeTerm( tokenizer );
                if( tokenizer.Match( TokenType.Minus ) ) expression -= ComputeTerm( tokenizer );
            }

            return expression;
        }

        /// <summary>
        /// term → factor [operator-multiplicative  factor]*
        /// </summary>
        private static double ComputeTerm( ITokenizer tokenizer )
        {
            var factor = ComputeFactor( tokenizer );
            while( (tokenizer.CurrentToken & TokenType.IsMultiplicativeOperator) != TokenType.None )
            {
                if( tokenizer.Match( TokenType.Multiplicative ) ) factor *= ComputeFactor( tokenizer );
                if( tokenizer.Match( TokenType.Divide ) ) factor /= ComputeFactor( tokenizer );
            }

            return factor;
        }

        /// <summary>
        /// factor ==> positive-factor | '-' positive-factor
        /// </summary>
        private static double ComputeFactor( ITokenizer tokenizer )
            => tokenizer.Match( TokenType.Minus )
                ? -ComputePositiveFactor( tokenizer )
                : ComputePositiveFactor( tokenizer );

        /// <summary>
        /// positive-factor → number  |  ‘(’  expression  ‘)’ 
        /// </summary>
        private static double ComputePositiveFactor( ITokenizer tokenizer )
        {
            if( tokenizer.MatchDouble( out var factor ) ) return factor;
            if( !tokenizer.Match( TokenType.OpenParenthesis ) ) throw new Exception( "Expected number or (." );
            factor = ComputeTernaryExpression( tokenizer );
            if( !tokenizer.Match( TokenType.CloseParenthesis ) ) throw new Exception( "Expected )." );
            return factor;
        }
    }
}
