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
        private readonly int _maxDepth;
        private readonly OptimizationMutator _optimizationMutator;

        public RandomNodeGenerator()
            : this( DefaultMaxDepth ) { }

        public RandomNodeGenerator( int maxDepth )
            : this( maxDepth, new Random() ) { }

        public RandomNodeGenerator( int maxDepth, Random random )
        {
            _maxDepth = maxDepth;
            _random = random;
            _optimizationMutator = new OptimizationMutator();
        }

        public Node GenerateRandomTree()
            => _optimizationMutator.MutateNode( GenerateRandomTree( 0 ) );

        private Node GenerateRandomTree( int depth )
        {
            /*
             * 0: Constant with random double
             * 1: Identifier with either x or y randomly
             * 2: Unary node with minus operator and random tree
             * 3: Binary node with random binary operator and two random tree
             * 4: Ternary node with three random tree
             */
            var rootBalancer = 1.0D / _maxDepth * (_maxDepth - depth);
            var leafBalancer = 3.0D / _maxDepth * depth;

            var next = Math.Clamp( _random.Next( 5 ) + rootBalancer - leafBalancer, 0, 4 );
            next = Math.Clamp( next, 0, 4 );

            if( next <= 0 )
                return new ConstantNode( _random.NextDouble() );

            if( next <= 1 )
                return new IdentifierNode( _random.Next() %2 == 0 ? "x" : "y" );

            if( next <= 2 )
                return new UnaryNode( TokenType.Minus, GenerateRandomTree( depth + 1 ) );

            if( next <= 3 )
                return new TernaryNode
                (
                    GenerateRandomTree( depth + 1 ),
                    GenerateRandomTree( depth + 1 ),
                    GenerateRandomTree( depth + 1 )
                );

            if( next <= 4 )
                return new BinaryNode
                (
                    GetRandomUnaryTokenType(),
                    GenerateRandomTree( depth + 1 ),
                    GenerateRandomTree( depth + 1 )
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
