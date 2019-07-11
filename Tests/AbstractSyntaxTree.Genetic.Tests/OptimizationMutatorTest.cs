using System.Collections.Generic;
using AbstractSyntaxTree.Analyzer;
using AbstractSyntaxTree.Model;
using AbstractSyntaxTree.Model.Abstraction;
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

        [TestCase( "1", 1 )]
        [TestCase( "x", 1 )]
        [TestCase( "¤", 1 )]
        [TestCase( "-x", 2 )]
        [TestCase( "-1", 1 )]
        [TestCase( "x + y", 3 )]
        [TestCase( "1 + 1 ", 1 )]
        [TestCase( "x / x ", 1 )]
        [TestCase( "x ? 1 : 0", 4 )]
        [TestCase( "1 ? x : y", 1 )]
        [TestCase( "0 ? x : y", 1 )]
        public void MutateNodeWithCount( string expression, int expectedCount )
        {
            var node = new NodeAnalyzer().Parse( expression );
            var optimizedNode = new OptimizationMutator().MutateNodeWithCount( node );
            optimizedNode.Count.Should().Be( expectedCount );
        }
    }
}
