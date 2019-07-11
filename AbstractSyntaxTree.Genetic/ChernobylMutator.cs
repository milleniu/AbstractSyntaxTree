using System;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;

namespace AbstractSyntaxTree.Genetic
{
    public class ChernobylMutator : NodeMutator
    {
        private readonly Random _random;

        private Node _other;
        private int _size;

        public double CrossoverProbability => 3.0D / _size;

        public ChernobylMutator()
            : this( new Random() ) { }

        public ChernobylMutator( Random random )
        {
            _random = random;
        }

        public Node MutateNodeWithCrossover( Node node, Node other, int size )
        {
            if( node == null ) throw new ArgumentNullException( nameof( node ) );
            if( _size < 0 ) throw new ArgumentException();

            _other = other ?? throw new ArgumentNullException( nameof( other ) );
            _size = size;

            return MutateNode( node );
        }
    }
}
