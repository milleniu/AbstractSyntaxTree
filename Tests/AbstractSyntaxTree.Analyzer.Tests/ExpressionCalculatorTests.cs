using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Analyzer.Tests
{
    [TestFixture]
    public class ExpressionCalculatorTests
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
        public void test_calculator( string expression, double expected )
        {
            ExpressionCalculator.Compute( expression ).Should().Be( expected );
        }
    }
}
