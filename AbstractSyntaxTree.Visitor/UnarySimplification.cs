using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class UnarySimplification : NodeMutator
    {
        public override Node Mutate( UnaryNode node )
        {
            return node;
        }
    }
}
