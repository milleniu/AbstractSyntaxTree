using System;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Model
{
    public class UnaryNode : Node
    {
        public TokenType Type { get; }
        public Node Operand { get; }

        public UnaryNode( TokenType type, Node operand )
        {
            Type = type;
            Operand = operand ?? throw new ArgumentException( nameof( operand ) );
        }

        public override void Accept( NodeVisitor nodeVisitor )
            => nodeVisitor.Visit( this );

        public override Node Accept( NodeMutator nodeMutator )
            => nodeMutator.Mutate( this );

        public override string ToString()
            => $"({Type} {Operand})";
    }
}
