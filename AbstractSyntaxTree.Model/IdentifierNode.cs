using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Model
{
    public class IdentifierNode : Node
    {
        public string Identifier { get; }

        public IdentifierNode( string identifier )
        {
            Identifier = identifier;
        }

        public override void Accept( NodeVisitor nodeVisitor )
            => nodeVisitor.Visit( this );

        public override Node Accept( NodeMutator nodeMutator )
            => nodeMutator.Mutate( this );

        public override string ToString()
            => $"{Identifier}";
    }
}
