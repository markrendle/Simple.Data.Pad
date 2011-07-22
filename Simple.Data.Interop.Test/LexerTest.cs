using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Interop.Test
{
    using Xunit;

    public class LexerTest
    {
        private static IEnumerator<Token> Lex(string source)
        {
            return new Lexer(source).GetTokens().GetEnumerator();
        }

        [Fact]
        public void OpenAndCloseParenAreTokenizedCorrectly()
        {
            using (var e = Lex("()"))
            {
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);
                Assert.False(e.MoveNext());
            }
        }

        [Fact]
        public void SimpleFindByWithIntegerConstantIsTokenizedCorrectly()
        {
            using (var e = Lex("db.Users.FindById(1)"))
            {
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("db", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("Users", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("FindById", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Number, e.Current.Type);
                Assert.Equal("1", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);

                Assert.False(e.MoveNext());
            }
        }

        [Fact]
        public void SimpleFindByWithDecimalConstantIsTokenizedCorrectly()
        {
            using (var e = Lex("db.Users.FindById(1.0)"))
            {
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("db", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("Users", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("FindById", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Number, e.Current.Type);
                Assert.Equal("1.0", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);

                Assert.False(e.MoveNext());
            }
        }

        [Fact]
        public void SimpleFindByWithDateTimeConstantIsTokenizedCorrectly()
        {
            using (var e = Lex("db.Users.FindByDate(new DateTime())"))
            {
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("db", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("Users", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("FindByDate", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("new", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("DateTime", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);

                Assert.False(e.MoveNext());
            }
        }

        [Fact]
        public void SimpleFindByWithStringConstantIsTokenizedCorrectly()
        {
            using (var e = Lex(@"db.Users.FindByName(""Bob"")"))
            {
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("db", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("Users", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("FindByName", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.String, e.Current.Type);
                Assert.Equal("Bob", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);

                Assert.False(e.MoveNext());
            }
        }

        [Fact]
        public void FindByWithNamedParameterStringConstantIsTokenizedCorrectly()
        {
            using (var e = Lex(@"db.Users.FindBy(Name: ""Bob"")"))
            {
                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("db", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("Users", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Dot, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("FindBy", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.OpenParen, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Identifier, e.Current.Type);
                Assert.Equal("Name", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.Colon, e.Current.Type);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.String, e.Current.Type);
                Assert.Equal("Bob", e.Current.Value);

                Assert.True(e.MoveNext());
                Assert.Equal(TokenType.CloseParen, e.Current.Type);

                Assert.False(e.MoveNext());
            }
        }

        [Fact]
        public void ShouldAccountForNestedParentheses()
        {
            var tokens = new[]
                             {
                                 new Token(TokenType.Identifier), new Token(TokenType.OpenParen),
                                 new Token(TokenType.Identifier), new Token(TokenType.OpenParen),
                                 new Token(TokenType.Identifier), new Token(TokenType.CloseParen),
                                 new Token(TokenType.CloseParen),
                             };

            Assert.Equal(1, Lexer.FindIndexOfOpeningToken(tokens, 6, TokenType.OpenParen));
        }

        [Fact]
        public void ShouldFindOpeningTokenInsideNested()
        {
            var tokens = new[]
                             {
                                 new Token(TokenType.Identifier), new Token(TokenType.OpenParen),
                                 new Token(TokenType.Identifier), new Token(TokenType.OpenParen),
                                 new Token(TokenType.Identifier), new Token(TokenType.CloseParen),
                                 new Token(TokenType.CloseParen),
                             };

            Assert.Equal(3, Lexer.FindIndexOfOpeningToken(tokens, 5, TokenType.OpenParen));
        }
    }
}
