using System.Linq;
using AbstractSyntaxTree.Model.Abstraction;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class RandomNodeGeneratorTests
    {
        [TestCase( 3712 )]
        public void GenerateRandomTree( int sampleSize )
        {
            var generator = new RandomNodeGenerator();
            var sut = new Node[ sampleSize ];
            for( var i = 0; i < sampleSize; ++i ) sut[ i ] = generator.GenerateRandomTree();
            sut.Distinct().Count().Should().BeGreaterOrEqualTo( sampleSize / 2 );
        }
    }
}
