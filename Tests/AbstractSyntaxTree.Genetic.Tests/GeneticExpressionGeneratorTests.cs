using System;
using AbstractSyntaxTree.Analyzer;
using AbstractSyntaxTree.Model.Abstraction;
using AbstractSyntaxTree.Visitor;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class GeneticExpressionGeneratorTests
    {
        private const int Population = 1000;
        private const int GenerationCount = 100;
        private const int GenerationDepth = 10;

        private InRangeExpressionComputer _inRangeExpressionComputer;
        private GeneticExpressionGenerator _geneticExpressionGenerator;
        private ComputingBound _bound;

        [SetUp]
        public void Setup()
        {
            _inRangeExpressionComputer = new InRangeExpressionComputer();
            _geneticExpressionGenerator = new GeneticExpressionGenerator();
            _bound = new ComputingBound( -1.0, 1.0, 20 );
        }

        [TestCase( "3 + ( 5 * x ) / 7 - y + 10" )]
        public void simple_test( string expression )
        {
            var theoreticalNode = new NodeAnalyzer().Parse( expression );
            var theoreticalResultSet = new double[ _bound.StepCount + 1, _bound.StepCount + 1 ];
            _inRangeExpressionComputer.Compute( theoreticalNode, theoreticalResultSet, _bound );

            Node best = null;
            _geneticExpressionGenerator
               .Invoking
                (
                    sut => best = sut.Generate
                    (
                        theoreticalResultSet,
                        _bound,
                        _bound,
                        Population,
                        GenerationCount,
                        GenerationDepth
                    )
                )
               .Should()
               .NotThrow();
            best.Should().NotBeNull();

            var printer = new PrettyPrintVisitor();
            printer.VisitNode( best );
            Console.WriteLine( printer.Output );
        }
    }
}
