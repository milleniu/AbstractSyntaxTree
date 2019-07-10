using System;
using System.Diagnostics;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class OptimizationMutator : NodeMutator
    {
        public override Node Mutate( UnaryNode node )
        {
            Debug.Assert( node.Type == TokenType.Minus );

            var mutated = MutateNode( node.Operand );
            if( mutated is ConstantNode constant )
            {
                return new ConstantNode( -constant.Value );
            }

            return mutated != node ? mutated : node;
        }

        public override Node Mutate( BinaryNode node )
        {
            var leftMutated = MutateNode( node.Left );
            var rightMutated = MutateNode( node.Right );

            var leftConstant = leftMutated as ConstantNode;
            var rightConstant = rightMutated as ConstantNode;
            if( leftConstant != null && rightConstant != null )
            {
                switch( node.Type )
                {
                    case TokenType.Plus:           return new ConstantNode( leftConstant.Value + rightConstant.Value );
                    case TokenType.Minus:          return new ConstantNode( leftConstant.Value - rightConstant.Value );
                    case TokenType.Multiplicative: return new ConstantNode( leftConstant.Value * rightConstant.Value );
                    case TokenType.Divide:         return new ConstantNode( leftConstant.Value / rightConstant.Value );
                    default:                       throw new NotSupportedException( nameof( node.Type ) );
                }
            }

            if( node.Type == TokenType.Minus )
                if( rightMutated is UnaryNode rightUnary )
                    return new BinaryNode( TokenType.Plus, leftMutated, rightUnary.Operand );
                else if( rightConstant != null )
                    return new BinaryNode( TokenType.Plus, leftMutated, new ConstantNode( -rightConstant.Value ) );

            if( node.Type == TokenType.Divide && leftMutated.ToString() == rightMutated.ToString() )
                return new ConstantNode( 1.0 );

            return leftMutated == node.Left && rightMutated == node.Right
                ? node
                : new BinaryNode( node.Type, leftMutated, rightMutated );
        }

        public override Node Mutate( TernaryNode node )
        {
            var conditionMutated = MutateNode( node.Condition );
            var whenTrueMutated = MutateNode( node.WhenTrue );
            var whenFalseMutated = node.WhenFalse != null ? MutateNode( node.WhenFalse ) : null;

            if( conditionMutated is ConstantNode conditionConstant )
                return conditionConstant.Value > 0 ? whenTrueMutated : whenFalseMutated;

            return conditionMutated == node.Condition
                && whenTrueMutated == node.WhenTrue
                && whenFalseMutated == node.WhenFalse
                ? node
                : new TernaryNode( conditionMutated, whenTrueMutated, whenFalseMutated );
        }
    }
}
