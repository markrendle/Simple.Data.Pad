namespace Simple.Data.Interop.Test
{
    using System.Linq;
    using Xunit;

    public class EnumerableExTest
    {
        [Fact]
        public void MultiCountShouldReturnCorrectSizedArrayForOnePredicate()
        {
            var actual = Enumerable.Empty<string>().MultiCount(string.IsNullOrWhiteSpace);
            Assert.Equal(1, actual.Length);
        }

        [Fact]
        public void MultiCountShouldReturnCorrectSizedArrayForTwoPredicates()
        {
            var actual = Enumerable.Empty<string>().MultiCount(string.IsNullOrWhiteSpace, string.IsNullOrEmpty);
            Assert.Equal(2, actual.Length);
        }

        [Fact]
        public void MultiCountShouldReturnCorrectSizedArrayForThreePredicates()
        {
            var actual = Enumerable.Empty<string>().MultiCount(string.IsNullOrWhiteSpace, string.IsNullOrEmpty, s => s == "foo");
            Assert.Equal(3, actual.Length);
        }

        [Fact]
        public void MultiCountShouldGetCorrectCountForOnePredicate()
        {
            var actual = "Hello world!".MultiCount(c => c == 'H');
            Assert.Equal(1, actual[0]);
        }

        [Fact]
        public void MultiCountShouldGetCorrectCountsForTwoPredicates()
        {
            var actual = "Hello world!".MultiCount(c => c == 'H', c => c == 'o');
            Assert.Equal(1, actual[0]);
            Assert.Equal(2, actual[1]);
        }

        [Fact]
        public void MultiCountShouldGetCorrectCountsForThreePredicates()
        {
            var actual = "Hello world!".MultiCount(c => c == 'H', c => c == 'o', c => c == '!');
            Assert.Equal(1, actual[0]);
            Assert.Equal(2, actual[1]);
            Assert.Equal(1, actual[2]);
        }
    }
}