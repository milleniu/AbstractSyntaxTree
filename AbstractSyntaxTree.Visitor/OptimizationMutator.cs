using System;
using System.Diagnostics;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class OptimizationMutator : NodeMutator
    {
        private int _count;

        public (Node Node, int Count) MutateNodeWithCount( Node node )
        {
            var mutatedNode = MutateNode( node );
            return (mutatedNode, _count);
        }

        public override Node Mutate( ConstantNode node )
        {
            _count = 1;
            return node;
        }

        public override Node Mutate( ErrorNode node )
        {
            _count = 1;
            return node;
        }

        public override Node Mutate( IdentifierNode node )
        {
            _count = 1;
            return node;
        }

        public override Node Mutate( UnaryNode node )
        {
            Debug.Assert( node.Type == TokenType.Minus );

            var mutated = MutateNode( node.Operand );
            if( mutated is ConstantNode constant )
                return new ConstantNode( -constant.Value );

            _count += 1;
            return mutated != node ? mutated : node;
        }

        public override Node Mutate( BinaryNode node )
        {
            var leftMutated = MutateNode( node.Left );
            var leftCount = _count;

            var rightMutated = MutateNode( node.Right );
            _count = leftCount + _count + 1;

            var leftConstant = leftMutated as ConstantNode;
            var rightConstant = rightMutated as ConstantNode;
            if( leftConstant != null && rightConstant != null )
            {
                _count = 1;
                switch( node.Type )
                {
                    case TokenType.Plus:           return new ConstantNode( leftConstant.Value + rightConstant.Value );
                    case TokenType.Minus:          return new ConstantNode( leftConstant.Value - rightConstant.Value );
                    case TokenType.Multiplicative: return new ConstantNode( leftConstant.Value * rightConstant.Value );
                    case TokenType.Divide:         return new ConstantNode( leftConstant.Value / rightConstant.Value );
                    default:                       throw new NotSupportedException( nameof( node.Type ) );
                }
            }

            switch( node.Type ) {
                case TokenType.Minus when rightMutated is UnaryNode rightUnary:
                    _count -= 1;
                    return new BinaryNode( TokenType.Plus, leftMutated, rightUnary.Operand );

                case TokenType.Minus when rightConstant != null:
                    return new BinaryNode( TokenType.Plus, leftMutated, new ConstantNode( -rightConstant.Value ) );

                case TokenType.Divide when leftMutated.ToString() == rightMutated.ToString():
                    _count = 1;
                    return new ConstantNode( 1.0 );

                default:
                    return leftMutated == node.Left && rightMutated == node.Right
                        ? node
                        : new BinaryNode( node.Type, leftMutated, rightMutated );
            }
        }

        public override Node Mutate( TernaryNode node )
        {
            var conditionMutated = MutateNode( node.Condition );
            var conditionCount = _count;

            var whenTrueMutated = MutateNode( node.WhenTrue );
            var whenTrueCount = _count;

            Node whenFalseMutated = null;
            if( node.WhenFalse != null )
                whenFalseMutated = MutateNode( node.WhenFalse );
            else
                _count = 0;

            if( conditionMutated is ConstantNode conditionConstant )
            {
                if( !(conditionConstant.Value > 0) )
                    return whenFalseMutated;

                _count = whenTrueCount;
                return whenTrueMutated;

            }

            _count = conditionCount + whenTrueCount + _count + 1;
            return conditionMutated == node.Condition
                && whenTrueMutated == node.WhenTrue
                && whenFalseMutated == node.WhenFalse
                ? node
                : new TernaryNode( conditionMutated, whenTrueMutated, whenFalseMutated );
        }
    }
}
