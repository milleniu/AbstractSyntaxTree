namespace AbstractSyntaxTree.Model.Abstraction
{
    public abstract class NodeVisitor
    {
        public void VisitNode( Node node )
            => node.Accept( this );

        public virtual void Visit( ConstantNode node ) { }

        public virtual void Visit( ErrorNode node ) { }

        public virtual void Visit( IdentifierNode node ) { }

        public virtual void Visit( UnaryNode node )
        {
            VisitNode( node.Operand );
        }

        public virtual void Visit( BinaryNode node )
        {
            VisitNode( node.Left );
            VisitNode( node.Right );
        }

        public virtual void Visit( TernaryNode node )
        {
            VisitNode( node.Condition );
            VisitNode( node.WhenTrue );
            if( node.WhenFalse != null )
                VisitNode( node.WhenFalse );
        }
    }
}
