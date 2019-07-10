using System;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Model
{
    public class ErrorNode : Node
    {
        public string Message { get; }

        public ErrorNode( string message )
        {
            if( string.IsNullOrWhiteSpace( message ) )
                throw new ArgumentNullException( nameof( message ) );

            Message = message;
        }

        public override void Accept( NodeVisitor nodeVisitor )
            => nodeVisitor.Visit( this );
        
        public override Node Accept( NodeMutator nodeMutator )
            => nodeMutator.Mutate( this );

        public override string ToString()
            => $"Error: {Message}";
    }
}
