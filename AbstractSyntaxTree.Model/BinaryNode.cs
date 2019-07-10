using System;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Model
{
    public class BinaryNode : Node
    {
        public TokenType Type { get; }
        public Node Left { get; }
        public Node Right { get; }

        public BinaryNode( TokenType type, Node left, Node right )
        {
            Type = type;
            Left = left ?? throw new ArgumentException( nameof( left ) );
            Right = right ?? throw new ArgumentException( nameof( right ) );
        }

        public override void Accept( NodeVisitor nodeVisitor )
            => nodeVisitor.Visit( this );

        public override Node Accept( NodeMutator nodeMutator )
            => nodeMutator.Mutate( this );

        public override string ToString()
            => $"({Type} {Left} {Right})";
    }
}
