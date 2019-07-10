using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AbstractSyntaxTree.Genetic
{
    public class BestKeeper<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        private const int DefaultLength = 100;
        private T[] _items;

        public int Count { get; private set; }
        public T Best => _items.Take( Count ).Max();

        public BestKeeper()
            : this( DefaultLength ) { }

        public BestKeeper( int length )
        {
            _items = new T[ length ];
            Count = 0;
        }

        private static int Parent( int index )
            => (index - 1) / 2;

        private int IndexOfMinChild( int left, int right )
            => right > Count ? left : _items[left].CompareTo( _items[right] ) < 0 ? left : right;

        public void Submit( T value )
        {
            if( Count >= _items.Length )
            {
                if( _items[ 0 ].CompareTo( value ) >= 0 )
                    return;

                RemoveMin();
            }

            _items[ Count ] = value;

            var index = Count;
            while( index > 0 && _items[ index ].CompareTo( _items[ Parent( index ) ] ) < 0 )
            {
                Swap( index, Parent( index ) );
                index = Parent( index );
            }

            ++Count;
        }

        private T RemoveMin()
        {
            if( Count <= 0 ) throw new InvalidOperationException();

            var max = _items[0];
            _items[0] = _items[Count - 1];
            --Count;

            var index = 0;
            while( index < Count )
            {
                var left = 2 * index + 1;
                var right = 2 * index + 2;

                if( left >= Count ) break;

                var maxChildIndex = IndexOfMinChild( left, right );
                if( _items[index].CompareTo( _items[maxChildIndex] ) < 0 ) break;

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

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private void Swap( int left, int right )
        {
            var temp = _items[ left ];
            _items[ left ] = _items[ right ];
            _items[ right ] = temp;
        }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly T[] _holder;
            private int _currentIdx;

            internal Enumerator( T[] holder )
            {
                _holder = holder;
                _currentIdx = -1;
            }

            public T Current
                => _currentIdx < _holder.Length
                    ? _holder[ _currentIdx ]
                    : throw new InvalidOperationException();

            object IEnumerator.Current => Current;

            public bool MoveNext()
                => _currentIdx < _holder.Length
                    ? ++_currentIdx != _holder.Length
                    : throw new InvalidOperationException();

            public void Reset()
                => throw new NotSupportedException();

            public void Dispose() { }
        }

        public IEnumerator<T> GetEnumerator()
            => new Enumerator( _items );

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
