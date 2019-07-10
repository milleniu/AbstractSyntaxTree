using System;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class HeapTests
    {
        [Test]
        public void heap_test()
        {
            var heap = new Heap<int>(3 );
            heap.Add( 2 );
            heap.Add( 3 );
            heap.Add( 1 );
            heap.Count.Should().Be( 3 );
            heap.Peek().Should().Be( 3 );
            heap.Count.Should().Be( 3 );
            heap.RemoveMax().Should().Be( 3 );
            heap.Count.Should().Be( 2 );
            heap.Add( 4 );
            heap.Add( 5 );
            heap.RemoveMax().Should().Be( 5 );
            heap.RemoveMax().Should().Be( 4 );
            heap.RemoveMax().Should().Be( 2 );
            heap.RemoveMax().Should().Be( 1 );
            heap.Invoking( sut => sut.RemoveMax() ).Should().Throw<InvalidOperationException>();
        }
    }
}
