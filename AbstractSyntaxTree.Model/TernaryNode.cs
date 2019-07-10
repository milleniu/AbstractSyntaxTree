using System;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Model
{
    public class TernaryNode : Node
    {
        public Node Condition { get; }
        public Node WhenTrue { get; }
        public Node WhenFalse { get; }

        public TernaryNode( Node condition, Node whenTrue, Node whenFalse )
        {
            Condition = condition ?? throw new ArgumentException( nameof( condition ) );
            WhenTrue = whenTrue ?? throw new ArgumentException( nameof( whenTrue ) );
            WhenFalse = whenFalse ?? throw new ArgumentException( nameof( whenFalse ) );
        }

        public override void Accept( NodeVisitor nodeVisitor )
            => nodeVisitor.Visit( this );

        public override Node Accept( NodeMutator nodeMutator )
            => nodeMutator.Mutate( this );


        public override string ToString()
            => $"{Condition} ? {WhenTrue} : {WhenFalse}";
    }
}
