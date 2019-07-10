using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Analyzer.Tests
{
    [TestFixture]
    public class AnalyzerTests
    {
        [TestCase( "3 + 5", "(Plus 3 5)" )]
        [TestCase( "3 + -5 - 4", "(Minus (Plus 3 (Minus 5)) 4)" )]
        public void parsing_simple_expression( string toParse, string toString )
        {
            var a = new NodeAnalyzer();
            a.Parse( toParse ).ToString().Should().Be( toString );
        }
    }
}
