namespace AbstractSyntaxTree.Model.Abstraction
{
    public abstract class NodeMutator
    {
        public Node MutateNode( Node node )
            => node.Accept( this );

        public virtual Node Mutate( ConstantNode node )
            => node;

        public virtual Node Mutate( ErrorNode node )
            => node;

        public virtual Node Mutate( IdentifierNode node )
            => node;

        public virtual Node Mutate( UnaryNode node )
        {
            var mutated = MutateNode( node.Operand );
            return mutated == node.Operand
                ? node
                : new UnaryNode( node.Type, mutated );
        }

        public virtual Node Mutate( BinaryNode node )
        {
            var leftMutated = MutateNode( node.Left );
            var rightMutated = MutateNode( node.Right );

            return leftMutated == node.Left && rightMutated == node.Right
                   ? node
                   : new BinaryNode( node.Type, leftMutated, rightMutated );
        }

        public virtual Node Mutate( TernaryNode node )
        {
            var condition = MutateNode( node.Condition );
            var whenTrue = MutateNode( node.WhenTrue );
            var whenFalse = node.WhenFalse == null ? null : MutateNode( node.WhenFalse );

            return condition == node.Condition && whenTrue == node.WhenTrue && whenFalse == node.WhenFalse
                ? node
                : new TernaryNode( condition, whenTrue, whenFalse );
        }
    }
}
