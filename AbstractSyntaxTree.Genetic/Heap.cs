using System;

namespace AbstractSyntaxTree.Genetic
{
    public class Heap<T>
        where T : IComparable<T>
    {
        private const int DefaultLength = 100;
        private T[] _items;

        public int Count { get; private set; }

        public Heap()
            : this( DefaultLength ) { }

        public Heap( int length )
        {
            _items = new T[ length ];
            Count = 0;
        }

        private static int Parent( int index )
            => (index - 1) / 2;

        private void GrowBackingArray()
            => Array.Resize( ref _items, _items.Length * 2 );

        private int IndexOfMaxChild( int left, int right )
            => right > Count ? left : _items[ left ].CompareTo( _items[ right ] ) > 0 ? left : right;

        public void Add( T value )
        {
            if( Count >= _items.Length )
                GrowBackingArray();

            _items[ Count ] = value;

            var index = Count;
            while( index > 0 && _items[ index ].CompareTo( _items[ Parent( index ) ] ) > 0 )
            {
                Swap( index, Parent( index ) );
                index = Parent( index );
            }

            ++Count;
        }

        public T Peek()
        {
            if( Count <= 0 ) throw new InvalidOperationException();
            return _items[ 0 ];
        }

        public T RemoveMax()
        {
            if( Count <= 0 ) throw new InvalidOperationException();

            var max = _items[ 0 ];
            _items[ 0 ] = _items[ Count - 1 ];
            --Count;

            var index = 0;
            while( index < Count )
            {
                var left = 2 * index + 1;
                var right = 2 * index + 2;

                if( left >= Count ) break;

                var maxChildIndex = IndexOfMaxChild( left, right );
                if( _items[ index ].CompareTo( _items[ maxChildIndex ] ) > 0 ) break;

                Swap( index, maxChildIndex );
                index = maxChildIndex;
            }

            return max;
        }

        public void Clear()
        {
            Count = 0;
            _items = new T[ DefaultLength ];
        }

        private void Swap( int left, int right )
            => (_items[ left ], _items[ right ]) = (_items[ right ], _items[ left ]);
    }
}
