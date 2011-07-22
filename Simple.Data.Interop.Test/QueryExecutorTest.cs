namespace Simple.Data.Interop.Test
{
    using System.Collections.Generic;
    using Xunit;

    public class QueryExecutorTest
    {
        [Fact]
        public void CanExecuteQuery()
        {
            var database = Database.OpenConnection("data source=.;initial catalog=SimpleTest;integrated security=true");
            var query = "db.Users.FindById(1)";
            var executor = new QueryExecutor(query);
            object result;
            Assert.True(executor.CompileAndRun(database, out result));
            var dict = result as IDictionary<string, object>;
            Assert.NotNull(dict);
        }
    }
}