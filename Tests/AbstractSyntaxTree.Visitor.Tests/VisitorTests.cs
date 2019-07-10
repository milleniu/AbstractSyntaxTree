using System;
using AbstractSyntaxTree.Analyzer;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Visitor.Tests
{
    [TestFixture]
    public class VisitorTests
    {
        [TestCase( "3*5", 3.0 * 5 )]
        [TestCase( "3+5", 3.0 + 5 )]
        [TestCase( " 3  *  (  2  +  2  )  ", 3.0 * (2 + 2) )]
        [TestCase( "3 + 5 * 125 / 7 - 6 + 10", 3.0 + 5 * 125 / 7.0 - 6 + 10 )]
        [TestCase( "7 - 6 + 10", 7.0 - 6 + 10 )]
        [TestCase( "7 - -2", 7.0 - -2.0 )]
        [TestCase( "7 + -2", 7.0 + -2.0 )]
        [TestCase( "7 * -(5+2*3)", 7.0 * -(5 + 2 * 3.0) )]
        [TestCase( "7 + 3 ? 12 : 14", 12 )]
        public void compute_visitor( string expression, double expected )
        {
            var analyzer = new NodeAnalyzer();
            var tree = analyzer.Parse( expression );
            var computeVisitor = new ComputeVisitor();
            computeVisitor.VisitNode( tree );
            computeVisitor.ComputedResult.Should().Be( expected );
        }

        [TestCase( "3*5" )]
        [TestCase( "3+5" )]
        [TestCase( " 3  *  (  2  +  2  )  " )]
        [TestCase( "3 + 5 * 125 / 7 - 6 + 10" )]
        [TestCase( "7 - 6 + 10" )]
        [TestCase( "7 - -2" )]
        [TestCase( "7 + -2" )]
        [TestCase( "7 * -(5+2*3)" )]
        [TestCase( "7 + 3 ? 12 : 14" )]
        public void pretty_print_visitor( string expression )
        {
            var analyzer = new NodeAnalyzer();
            var tree = analyzer.Parse( expression );
            var computeVisitor = new PrettyPrintVisitor();
            computeVisitor.VisitNode( tree );
            var output = computeVisitor.Output;
            computeVisitor.Output.Should().NotBeNullOrEmpty();
            Console.WriteLine( output );
        }
    }
}
