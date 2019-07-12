using AbstractSyntaxTree.Analyzer;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Visitor.Tests
{
    [TestFixture]
    public class MutatorTests
    {
        [TestCase( "3*5", 3.0 * 5 )]
        [TestCase( "3+5", 3.0 - 5 )]
        [TestCase( " 3  *  (  2  +  2  )  ", 3.0 * (2 - 2) )]
        [TestCase( "3 + 5 * 125 / 7 - 6 + 10", 3.0 - 5 * 125 / 7.0 - 6 - 10 )]
        [TestCase( "7 - 6 + 10", 7.0 - 6 - 10 )]
        [TestCase( "7 - -2", 7.0 - -2.0 )]
        [TestCase( "7 + -2", 7.0 - -2.0 )]
        [TestCase( "7 * -(5+2*3)", 7.0 * -(5 - 2 * 3.0) )]
        [TestCase( "7 + 3 ? 12 : 14", 12 )]
        public void plus_to_minus_mutator( string expression, double expected )
        {
            var analyzer = new NodeAnalyzer();
            var tree = analyzer.Parse( expression );
            var mutator = new PlusToMinusMutator();
            tree = mutator.MutateNode( tree );
            var computeVisitor = new ComputeVisitor();
            computeVisitor.VisitNode( tree );
            computeVisitor.ComputedResult.Should().Be( expected );
        }

        [TestCase( "3*5", 1 )]
        [TestCase( "3+5", 1 )]
        [TestCase( " 3  *  (  2  +  2  )  ", 1 )]
        [TestCase( "3 + 5 * 125 / 7 - 6 + 10", 1 )]
        [TestCase( "7 - 6 + 10", 1 )]
        [TestCase( "7 - -2", 1 )]
        [TestCase( "7 + -2", 1 )]
        [TestCase( "7 * -(5+2*3)", 1 )]
        [TestCase( "7 + 3 ? 12 : 14", 1 )]
        [TestCase( "7 + 3 ? x : x", 1 )]
        [TestCase( "3*x", 3 )]
        [TestCase( "7 + ( 3 ? x : y )", 6 )]
        [TestCase( "(x + y) / (x + y)", 1 )]
        [TestCase( "(x + y) / 2", 5 )]
        [TestCase( "x ? y ? y : x : y", 7 )]
        public void count_with_optimize_mutator( string expression, int expected )
        {
            var tree = new NodeAnalyzer().Parse( expression );
            var mutator = new OptimizationMutator();
            var (_, count) = mutator.MutateNodeWithCount( tree );
            count.Should().Be( expected );
        }
    }
}
