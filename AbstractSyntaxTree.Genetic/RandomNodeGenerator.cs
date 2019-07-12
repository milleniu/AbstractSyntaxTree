using System;
using System.Runtime.CompilerServices;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;
using AbstractSyntaxTree.Visitor;

namespace AbstractSyntaxTree.Genetic
{
    public class RandomNodeGenerator
    {
        private const int DefaultMaxDepth = 5;

        private readonly Random _random;
        private readonly OptimizationMutator _optimizationMutator;

        public RandomNodeGenerator()
            : this( new Random() ) { }

        public RandomNodeGenerator( Random random )
        {
            _random = random;
            _optimizationMutator = new OptimizationMutator();
        }

        public Node GenerateRandomTree()
            => GenerateRandomTree( DefaultMaxDepth );

        public Node GenerateRandomTree( int maxDepth )
            => GenerateRandomTree( 0, maxDepth );

        private Node GenerateRandomTree( int currentDepth, int maxDepth )
        {
            /*
             * 0: Constant with random double
             * 1: Identifier with either x or y randomly
             * 2: Unary node with minus operator and random tree
             * 3: Binary node with random binary operator and two random tree
             * 4: Ternary node with three random tree
             */
            var rootBalancer = 1.0D / maxDepth * (maxDepth - currentDepth);
            var leafBalancer = 3.0D / maxDepth * currentDepth;

            var next = Math.Clamp( _random.Next( 5 ) + rootBalancer - leafBalancer, 0, 4 );
            next = Math.Clamp( next, 0, 4 );

            if( next <= 0 )
                return new ConstantNode( _random.NextConstantValue() );

            if( next <= 1 )
                return new IdentifierNode( _random.Next() % 2 == 0 ? "x" : "y" );

            if( next <= 2 )
                return new UnaryNode( TokenType.Minus, GenerateRandomTree( currentDepth + 1, maxDepth ) );

            if( next <= 3 )
                return new TernaryNode
                (
                    GenerateRandomTree( currentDepth + 1, maxDepth ),
                    GenerateRandomTree( currentDepth + 1, maxDepth ),
                    GenerateRandomTree( currentDepth + 1, maxDepth )
                );

            if( next <= 4 )
                return new BinaryNode
                (
                    GetRandomUnaryTokenType(),
                    GenerateRandomTree( currentDepth + 1, maxDepth ),
                    GenerateRandomTree( currentDepth + 1, maxDepth )
                );
            throw new ArgumentOutOfRangeException();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private TokenType GetRandomUnaryTokenType()
        {
            switch( _random.Next( 0, 4 ) )
            {
                case 0:  return TokenType.Minus;
                case 1:  return TokenType.Plus;
                case 2:  return TokenType.Divide;
                case 3:  return TokenType.Multiplicative;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
