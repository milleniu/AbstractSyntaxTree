using System.Collections.Generic;
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
        private NodeAnalyzer _nodeAnalyzer;

        [SetUp]
        public void Initialize()
        {
            _computer = new InRangeExpressionComputer();
            _nodeAnalyzer = new NodeAnalyzer();
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

            _computer.Compute( _nodeAnalyzer.Parse( "x + y" ), sut, bound );

            for( var i = 0; i <= bound.StepCount; ++i )
            for( var j = 0; j <= bound.StepCount; ++j )
                sut[ i, j ].Should().BeApproximately( expected[ i, j ], Precision );
        }

        private static IEnumerable<TestCaseData> ComputeGeneticDifference_test_case_data()
        {
            yield return new TestCaseData
            (
                "1",
                new double[ , ] { { 0, 1 }, { 2, 3 } },
                new ComputingBound( -1, 1, 1 ),
                6.0D
            );

            yield return new TestCaseData
            (
                "x + y",
                new double[ , ] { { 0, 1 }, { 1, 2 } },
                new ComputingBound( 0, 1, 1 ),
                0.0D
            );
        }

        [TestCaseSource( nameof( ComputeGeneticDifference_test_case_data ) )]
        public void ComputeGeneticDifference
        (
            string expression,
            double[ , ] theoretical,
            ComputingBound bound,
            double expectedDifference
        )
        {
            _computer
               .ComputeGeneticDifference( _nodeAnalyzer.Parse( expression ), theoretical, bound )
               .Should()
               .BeApproximately( expectedDifference, Precision );
        }
    }
}
