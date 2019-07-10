using System;
using System.Text;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class PrettyPrintVisitor : NodeVisitor
    {
        private readonly StringBuilder _stringBuilder;

        public string Output => _stringBuilder.ToString();

        public PrettyPrintVisitor()
        {
            _stringBuilder = new StringBuilder();
        }

        public override void Visit( ConstantNode node )
            => _stringBuilder.Append( node.Value );

        public override void Visit( ErrorNode node )
            => _stringBuilder.Append( node.Message );

        public override void Visit( IdentifierNode node )
            => _stringBuilder.Append( node.Identifier );

        public override void Visit( UnaryNode node )
        {
            switch( node.Type )
            {
                case TokenType.Minus:
                    _stringBuilder.Append( '-' );
                    node.Operand.Accept( this );
                    break;

                default:
                    throw new NotSupportedException( nameof( node.Type ) );
            }   
        }

        public override void Visit( BinaryNode node )
        {
            _stringBuilder.Append( "( " );
            node.Left.Accept( this );

            switch( node.Type )
            {
                case TokenType.Plus:
                    _stringBuilder.Append( " + " );
                    break;

                case TokenType.Minus:
                    _stringBuilder.Append( " - " );
                    break;

                case TokenType.Divide:
                    _stringBuilder.Append( " / " );
                    break;

                case TokenType.Multiplicative:
                    _stringBuilder.Append( " * " );
                    break;

                default:
                    throw new NotSupportedException( nameof( node.Type ) );
            }

            node.Right.Accept( this );
            _stringBuilder.Append( " )" );
        }

        public override void Visit( TernaryNode node )
        {
            _stringBuilder.Append( "( " );
            node.Condition.Accept( this );
            _stringBuilder.Append( " ? " );
            node.WhenTrue.Accept( this );
            _stringBuilder.Append( " : " );
            node.WhenFalse.Accept( this );
            _stringBuilder.Append( " )" );
        }

        public override string ToString()
            => _stringBuilder.ToString();
    }
}
