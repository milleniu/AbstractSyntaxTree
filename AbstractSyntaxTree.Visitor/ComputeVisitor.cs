using System;
using System.Collections.Generic;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class ComputeVisitor : NodeVisitor
    {
        private readonly Func<string, double?> _variables;

        public ComputeVisitor()
            : this( name => null ) { }

        public ComputeVisitor( IReadOnlyDictionary<string, double> variables )
            : this( name => variables.TryGetValue( name, out var v ) ? (double?) v : null ) { }

        public ComputeVisitor( Func<string, double?> variables )
        {
            _variables = variables;
        }

        public double ComputedResult { get; private set; }

        public override void Visit( ConstantNode node )
            => ComputedResult = node.Value;

        public override void Visit( IdentifierNode node )
        {
            var value = _variables( node.Identifier );
            ComputedResult = value ?? double.NaN;
        }

        public override void Visit( ErrorNode node )
            => ComputedResult = double.NaN;

        public override void Visit( UnaryNode node )
        {
            switch( node.Type )
            {
                case TokenType.Minus:
                    VisitNode( node.Operand );
                    ComputedResult = -ComputedResult;
                    break;

                default:
                    throw new NotSupportedException( nameof( node.Type ) );
            }
        }

        public override void Visit( BinaryNode node )
        {
            VisitNode( node.Left );
            var left = ComputedResult;

            switch( node.Type )
            {
                case TokenType.Plus:
                    VisitNode( node.Right );
                    ComputedResult = left + ComputedResult;
                    break;

                case TokenType.Minus:
                    VisitNode( node.Right );
                    ComputedResult = left - ComputedResult;
                    break;

                case TokenType.Divide:
                    VisitNode( node.Right );
                    ComputedResult = 1.0D * left / ComputedResult;
                    break;

                case TokenType.Multiplicative:
                    VisitNode( node.Right );
                    ComputedResult = 1.0D * left * ComputedResult;
                    break;

                default:
                    throw new NotSupportedException( nameof( node.Type ) );
            }
        }

        public override void Visit( TernaryNode node )
        {
            VisitNode( node.Condition );
            VisitNode( ComputedResult > 0 ? node.WhenTrue : node.WhenFalse );
        }
    }
}
