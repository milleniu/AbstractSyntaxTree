using System;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class GeneticExpressionGeneratorTests
    {
        private const int Population = 1000;
        private const int GenerationCount = 100;
        private const int GenerationDepth = 10;

        [Test]
        public void Temporary_Test()
        {
            var bound = new ComputingBound( -1.0, 1.0, 20 );
            for( var i = 0; i <= bound.Step; ++i ) Console.WriteLine( (int) (bound.LowerBound + i * bound.Step) );

            var generator = new GeneticExpressionGenerator
            (
                in bound,
                in bound,
                Population,
                GenerationCount,
                GenerationDepth
            );
            
            var result = generator.StartGeneration( "3 + ( 5 * x ) / 7 - y + 10" );
            //result.Should().NotBeEmpty();
        }
    }
}
