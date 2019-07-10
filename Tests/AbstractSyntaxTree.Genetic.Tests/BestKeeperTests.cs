using System;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class BestKeeperTests
    {
        [Test]
        public void simple_scenario()
        {
            var bestKeeper = new BestKeeper<int>( 3 );
            bestKeeper.Submit( 2 );
            bestKeeper.Submit( 3 );
            bestKeeper.Submit( 1 );
            bestKeeper.Count.Should().Be( 3 );
            bestKeeper.Should().BeEquivalentTo( new[] { 1, 2, 3 } );

            bestKeeper.Submit( 1 );
            bestKeeper.Submit( 4 );
            bestKeeper.Submit( 5 );
            bestKeeper.Count.Should().Be( 3 );
            bestKeeper.Should().BeEquivalentTo( new[] { 3, 4, 5 } );

            bestKeeper.Submit( 5 );
            bestKeeper.Submit( 5 );
            bestKeeper.Submit( 6 );
            bestKeeper.Count.Should().Be( 3 );
            bestKeeper.Should().BeEquivalentTo( new[] { 5, 5, 6 } );
        }
    }
}
