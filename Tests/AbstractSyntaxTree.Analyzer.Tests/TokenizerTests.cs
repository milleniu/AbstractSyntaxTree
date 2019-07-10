using System;
using System.Linq;
using AbstractSyntaxTree.Model;
using FluentAssertions;
using NUnit.Framework;

namespace AbstractSyntaxTree.Analyzer.Tests
{
    [TestFixture]
    public class TokenizerTests
    {
        [Test]
        public void basic_test()
        {
            var tokenizer = new StringTokenizer( "42 + 3712" );
            tokenizer.GetNextToken().Should().Be( TokenType.Integer );
            tokenizer.MatchInteger( out var i ).Should().BeTrue();
            i.Should().Be( 42 );
            tokenizer.CurrentToken.Should().Be( TokenType.Plus );
            tokenizer.GetNextToken().Should().Be( TokenType.Integer );
            tokenizer.MatchInteger( out i ).Should().BeTrue();
            i.Should().Be( 3712 );
        }

        [Test]
        public void dump_test()
        {
            Action dump = () => TokenizerHelper.DumpTokens( "42 / ( 3712 - 789123 )" );
            dump.Should().NotThrow();
        }


        [TestCase( ",::;", "Comma,DoubleColon,SemiColon" )]
        [TestCase( ",,[", "Comma,Comma,OpenSquare" )]
        [TestCase( ":::", "DoubleColon,Colon" )]
        [TestCase( "4,8,32", "Integer,Comma,Integer,Comma,Integer" )]
        public void terminals( string toParse, string expected )
        {
            StringTokenizer.Parse( toParse )
                           .SequenceEqual( expected.Split( ',' )
                                                   .Select( Enum.Parse<TokenType> ) )
                           .Should().BeTrue();
        }

        [TestCase( ",::;//,::;", "Comma,DoubleColon,SemiColon" )]
        [TestCase( ":/*//::*/::", "Colon,DoubleColon" )]
        [TestCase( "4/*;*/,8,/**/32//3712\"\u2145\"", "Integer,Comma,Integer,Comma,Integer" )]
        public void comments( string toParse, string expected )
        {
            StringTokenizer.Parse( toParse )
                           .SequenceEqual( expected.Split( ',' )
                                                   .Select( Enum.Parse<TokenType> ) )
                           .Should().BeTrue();
        }

        [Test]
        public void identifiers()
        {
            var tokenizer = new StringTokenizer( "Count * ( _frequency - 1 )" );
            tokenizer.GetNextToken().Should().Be( TokenType.Identifier );
            tokenizer.MatchIdentifier( out var id ).Should().BeTrue();
            id.Should().Be( "Count" );
            tokenizer.CurrentToken.Should().Be( TokenType.Multiplicative );
            tokenizer.GetNextToken().Should().Be( TokenType.OpenParenthesis );
            tokenizer.GetNextToken().Should().Be( TokenType.Identifier );
            tokenizer.MatchIdentifier( out id ).Should().BeTrue( );
            id.Should().Be( "_frequency" );
            tokenizer.CurrentToken.Should().Be( TokenType.Minus );
            tokenizer.GetNextToken().Should().Be( TokenType.Integer );
            tokenizer.MatchInteger( out var i ).Should().BeTrue();
            i.Should().Be( 1 );
            tokenizer.CurrentToken.Should().Be( TokenType.CloseParenthesis );
        }

        [Test]
        public void strings()
        {
            var tokenizer = new StringTokenizer( "\"C\" + 3 + \"PO\"" );
            tokenizer.GetNextToken().Should().Be( TokenType.String );
            tokenizer.MatchString( out var s ).Should().BeTrue();
            s.Should().Be( "C" );
            tokenizer.CurrentToken.Should().Be( TokenType.Plus );
            tokenizer.GetNextToken().Should().Be( TokenType.Integer );
            tokenizer.MatchInteger( out var i ).Should().BeTrue();
            i.Should().Be( 3 );
            tokenizer.CurrentToken.Should().Be( TokenType.Plus );
            tokenizer.GetNextToken().Should().Be( TokenType.String );
            tokenizer.MatchString( out s ).Should().BeTrue();
            s.Should().Be( "PO" );
        }

        [Test]
        public void floating()
        {
            var tokenizer = new StringTokenizer( "37.12e3 + 4.E2" );
            tokenizer.GetNextToken().Should().Be( TokenType.Double );
            tokenizer.MatchInteger( out _ ).Should().BeFalse();
            tokenizer.MatchDouble( out var d ).Should().BeTrue();
            d.Should().BeApproximately( 37.12e3, double.Epsilon );
            tokenizer.CurrentToken.Should().Be( TokenType.Plus );
            tokenizer.GetNextToken().Should().Be( TokenType.Double );
            tokenizer.MatchDouble( out d ).Should().BeTrue();
            d.Should().BeApproximately( 4E2, double.Epsilon );
        }
    }
}
