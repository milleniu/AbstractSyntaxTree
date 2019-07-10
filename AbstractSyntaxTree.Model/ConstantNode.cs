using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Model
{
    public class ConstantNode : Node
    {
        public double Value { get; }

        public ConstantNode( double value )
        {
            Value = value;
        }

        public override void Accept( NodeVisitor nodeVisitor )
            => nodeVisitor.Visit( this );
        
        public override Node Accept( NodeMutator nodeMutator )
            => nodeMutator.Mutate( this );

        public override string ToString()
            => $"{Value}";
    }
}
