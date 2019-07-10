using AbstractSyntaxTree.Analyzer;
using AbstractSyntaxTree.Visitor;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Genetic.Tests
{
    public class OptimizationMutatorTest
    {
        [TestCase( "10 + 4 - 7 * (3 + 5)", "-42" )]
        [TestCase( "x - -4", "( x + 4 )" )]
        [TestCase( "50 + 6 + x / x", "57" )]
        [TestCase( "(10 + 2 + x + y) / ( 12 + x + y )", "1" )]
        public void optimizer_in_action( string input, string optimized )
        {
            var node = new NodeAnalyzer().Parse( input );

            var optimizedNode = new OptimizationMutator().MutateNode( node );

            var printer = new PrettyPrintVisitor();
            printer.VisitNode( optimizedNode );
            printer.ToString().Should().Be( optimized );
        }
    }
}
