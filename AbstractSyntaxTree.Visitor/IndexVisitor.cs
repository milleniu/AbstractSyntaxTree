using System;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Visitor
{
    public class IndexVisitor : NodeVisitor
    {
        private readonly Func<int> _getLookupIndex;

        private int _lookupIndex;
        private int _currentIndex;

        private bool Found => _lookupIndex == -1;
        public Node NodeAtIndex { get; private set; }

        public IndexVisitor( Func<int> getLookupIndex )
        {
            _getLookupIndex = getLookupIndex;
        }

        public new void VisitNode( Node node )
        {
            _lookupIndex = _getLookupIndex();
            _currentIndex = 0;

            if( _lookupIndex < 1 ) throw new ArgumentException( nameof( _getLookupIndex ) );

            base.VisitNode( node );

            if( !Found ) throw new ArgumentException( nameof( _getLookupIndex ) );
        }

        private void ChallengeNode( Node node )
        {
            if( Found ) return;

            _currentIndex += 1;
            if( _currentIndex != _lookupIndex ) return;

            _lookupIndex = -1;
            NodeAtIndex = node;
        }

        public override void Visit( ConstantNode node )
            => ChallengeNode( node );

        public override void Visit( ErrorNode node )
            => ChallengeNode( node );

        public override void Visit( IdentifierNode node )
            => ChallengeNode( node );

        public override void Visit( UnaryNode node )
        {
            ChallengeNode( node );
            if( Found ) return;
            base.Visit( node );
        }

        public override void Visit( BinaryNode node )
        {
            ChallengeNode( node );
            if( Found ) return;
            base.Visit( node );
        }

        public override void Visit( TernaryNode node )
        {
            ChallengeNode( node );
            if( Found ) return;
            base.Visit( node );
        }
    }
}
