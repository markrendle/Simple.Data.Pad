namespace Simple.Data.Interop.Test
{
    using System.Collections.Generic;
    using Xunit;

    public class ParserTest
    {
        private static IEnumerable<Token> Lex(string source)
        {
            return new Lexer(source).GetTokens();
        }

        [Fact]
        public void SimpleFindByIdIsParsedCorrectly()
        {
            var tokens = Lex("db.FindById(1)");
            AstNode actual = new Parser(tokens).Parse();
            Assert.NotNull(actual);
            Assert.Equal(AstNodeType.Identifier, actual.Type);
            Assert.Equal("db", actual.Value);
            Assert.Equal(1, actual.Nodes.Count);

            var callNode = actual.Nodes[0];
            Assert.Equal(AstNodeType.Call, callNode.Type);
            Assert.Equal(1, callNode.Nodes.Count);

            var methodNode = callNode.Nodes[0];
            Assert.Equal(AstNodeType.Method, methodNode.Type);
            Assert.Equal("FindById", methodNode.Value);
            Assert.Equal(1, methodNode.Nodes.Count);

            var argumentNode = methodNode.Nodes[0];
            Assert.Equal(AstNodeType.Literal, argumentNode.Type);
            Assert.Equal("1", argumentNode.Value);
            Assert.Equal(0, argumentNode.Nodes.Count);
        }
    }
}