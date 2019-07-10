using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class PlusToMinusMutator : NodeMutator
    {
        public override Node Mutate( BinaryNode node )
            => node.Type == TokenType.Plus
                ? new BinaryNode( TokenType.Minus, node.Left.Accept( this ), node.Right.Accept( this ) )
                : base.Mutate( node );
    }
}
