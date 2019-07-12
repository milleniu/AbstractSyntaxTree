using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;
using AbstractSyntaxTree.Visitor;

namespace AbstractSyntaxTree.Genetic
{
    public class GeneticMutator : NodeMutator
    {
        private const double ConstantMutationProbability = 15.0D / 100;
        private const double IdentifierMutationProbability = 10.0D / 100;

        private const double SameFamilyOperatorMutationProbability = 10.0D / 100;
        private const double DifferentFamilyOperatorMutationProbability = 5.0D / 100;

        private const double ConditionInversionProbability = 5.0D / 100;

        private const double FullMutationProbability = 1.0D / 100;

        private double CrossoverProbability => 3.0D / _nodeSize;

        private readonly Random _random;
        private readonly IndexVisitor _indexVisitor;
        private readonly RandomNodeGenerator _generator;
        private readonly OptimizationMutator _optimizer;

        private int _nodeSize;

        private Node _otherNode;
        private int _otherNodeSize;
        private int _otherIndexLookup;

        public GeneticMutator()
            : this( new Random() ) { }

        public GeneticMutator( Random random )
        {
            _random = random;
            _indexVisitor = new IndexVisitor( () => _otherIndexLookup );
            _generator = new RandomNodeGenerator( _random );
            _optimizer = new OptimizationMutator();
            _otherIndexLookup = -1;
        }

        public Node MutateNodeWithCrossover
        (
            Node firstExpression,
            int firstSize,
            Node secondExpression,
            int secondSize
        )
        {
            var node = firstExpression ?? throw new ArgumentNullException( nameof( firstExpression ) );
            _nodeSize = firstSize > 0 ? firstSize : throw new ArgumentException( nameof( firstSize ) );
            _otherNode = secondExpression ?? throw new ArgumentNullException( nameof( secondExpression ) );
            _otherNodeSize = secondSize > 0 ? secondSize : throw new ArgumentException( nameof( secondSize ) );

            return MutateNode( node );
        }

        public override Node Mutate( ConstantNode node )
        {
            if( _random.NextDouble() <= CrossoverProbability )
                return Crossover();

            if( _random.NextDouble() <= ConstantMutationProbability )
                return new ConstantNode( _random.NextConstantValue() );

            if( _random.NextDouble() <= FullMutationProbability )
                return FullMutation();

            return base.Mutate( node );
        }

        public override Node Mutate( ErrorNode node )
        {
            if( _random.NextDouble() <= CrossoverProbability )
                return Crossover();

            if( _random.NextDouble() <= FullMutationProbability )
                return FullMutation();

            return base.Mutate( node );
        }

        public override Node Mutate( IdentifierNode node )
        {
            if( _random.NextDouble() <= CrossoverProbability )
                return Crossover();

            if( _random.NextDouble() <= IdentifierMutationProbability )
                return new IdentifierNode( node.Identifier.Equals( "x" ) ? "y" : "x" );

            if( _random.NextDouble() <= FullMutationProbability )
                return FullMutation();

            return base.Mutate( node );
        }

        public override Node Mutate( UnaryNode node )
        {
            Debug.Assert( node.Type == TokenType.Minus );

            if( _random.NextDouble() <= CrossoverProbability )
                return Crossover();

            if( _random.NextDouble() <= SameFamilyOperatorMutationProbability )
                return MutateNode( node.Operand );

            if( _random.NextDouble() <= FullMutationProbability )
                return FullMutation();

            return base.Mutate( node );
        }

        public override Node Mutate( BinaryNode node )
        {
            if( _random.NextDouble() <= CrossoverProbability )
                return Crossover();

            if( _random.NextDouble() <= SameFamilyOperatorMutationProbability )
            {
                var leftMutated = MutateNode( node.Left );
                var rightMutated = MutateNode( node.Right );

                switch( node.Type )
                {
                    case TokenType.Minus:
                        return new BinaryNode( TokenType.Plus, leftMutated, rightMutated );

                    case TokenType.Plus:
                        return new BinaryNode( TokenType.Minus, leftMutated, rightMutated );

                    case TokenType.Divide:
                        return new BinaryNode( TokenType.Multiplicative, leftMutated, rightMutated );

                    case TokenType.Multiplicative:
                        return new BinaryNode( TokenType.Divide, leftMutated, rightMutated );

                    default:
                        throw new NotSupportedException( nameof( node.Type ) );
                }
            }

            if( _random.NextDouble() <= DifferentFamilyOperatorMutationProbability )
            {
                var leftMutated = MutateNode( node.Left );
                var rightMutated = MutateNode( node.Right );

                switch( node.Type )
                {
                    case TokenType.Minus:
                    case TokenType.Plus:
                        var multiplicative = _random.Next( 2 ) == 0 ? TokenType.Minus : TokenType.Plus;
                        return new BinaryNode( multiplicative, leftMutated, rightMutated );

                    case TokenType.Divide:
                    case TokenType.Multiplicative:
                        var additive = _random.Next( 2 ) == 0 ? TokenType.Minus : TokenType.Plus;
                        return new BinaryNode( additive, leftMutated, rightMutated );

                    default:
                        throw new NotSupportedException( nameof( node.Type ) );
                }
            }

            if( _random.NextDouble() <= FullMutationProbability )
                return FullMutation();

            return base.Mutate( node );
        }

        public override Node Mutate( TernaryNode node )
        {
            if( _random.NextDouble() <= CrossoverProbability )
                return Crossover();

            if( node.WhenFalse != null && _random.NextDouble() <= ConditionInversionProbability )
                return new TernaryNode
                (
                    MutateNode( node.Condition ),
                    MutateNode( node.WhenFalse ),
                    MutateNode( node.WhenTrue )
                );

            if( _random.NextDouble() <= FullMutationProbability )
                return FullMutation();

            return base.Mutate( node );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private Node Crossover()
        {
            _otherIndexLookup = _random.Next( _otherNodeSize ) + 1;
            _indexVisitor.VisitNode( _otherNode );
            Debug.Assert( _indexVisitor.NodeAtIndex != null );
            return _indexVisitor.NodeAtIndex;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private Node FullMutation()
            => _optimizer.MutateNode( _generator.GenerateRandomTree( 5 ) );
    }
}
