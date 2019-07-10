using AbstractSyntaxTree.Analyzer;
using AbstractSyntaxTree.Model.Abstraction;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class InRangeExpressionComputerTests
    {
        private const double Precision = 10e-5;

        private InRangeExpressionComputer _computer;
        private Node _addExpression;

        [SetUp]
        public void Initialize()
        {
            _computer = new InRangeExpressionComputer();
            _addExpression = new NodeAnalyzer().Parse( "x + y" );
        }

        [TestCase( -1.0, 1.0, 20 )]
        [TestCase( -10_000, 10_000, 3712 )]
        public void ComputeExpressionForBounds( double lowerBound, double upperBound, int stepCount )
        {
            var bound = new ComputingBound( lowerBound, upperBound, stepCount );
            var expected = new double[ bound.StepCount + 1, bound.StepCount + 1 ];
            var sut = new double[ bound.StepCount + 1, bound.StepCount + 1 ];

            for( var i = 0; i <= bound.StepCount; ++i )
            for( var j = 0; j <= bound.StepCount; ++j )
                expected[ i, j ] = bound.LowerBound + 1.0 * i * bound.Step + bound.LowerBound + 1.0 * j * bound.Step;

            _computer.Compute( _addExpression, sut, bound );

            for( var i = 0; i <= bound.StepCount; ++i )
            for( var j = 0; j <= bound.StepCount; ++j )
                sut[ i, j ].Should().BeApproximately( expected[ i, j ], Precision );
        }

        [Test]
        public void GetGeneticDifference()
        {
            var bound = new ComputingBound( -1, 1, 1 );
            var theoretical = new double[ , ] { { 0, 1 }, { 2, 3 } };
            var computed = new double[ , ] { { 1, 1 }, { 1, 1 } };
            const double difference = 6.0D;

            _computer
               .GetGeneticDifference( theoretical, computed, bound )
               .Should()
               .BeApproximately( difference, Precision );
        }
    }
}
