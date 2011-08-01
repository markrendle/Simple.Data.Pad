using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Pad.Test
{
    using Ado.Schema;
    using Xunit;

    public class AutoCompleterTest
    {
        private static AutoCompleter CreateTarget()
        {
            return new AutoCompleter(new StubSchemaProvider());
        }

        [Fact]
        public void ShouldReturnEmptyForEmptyString()
        {
            var target = CreateTarget();
            var actual = target.GetOptions(string.Empty);
            Assert.Equal(0, actual.Count());
        }

        [Fact]
        public void ShouldReturnTableListForDb()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.").ToArray();
            Assert.Equal(1, actual.Length);
            Assert.Equal("Test", actual[0]);
        }

        [Fact]
        public void ShouldReturnFindByListForTable()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.").ToArray();
            Assert.Contains("FindById", actual);
            Assert.Contains("FindByName", actual);
        }

        [Fact]
        public void ShouldReturnFindByListForPluralizedTable()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Tests.").ToArray();
            Assert.Contains("FindById", actual);
            Assert.Contains("FindByName", actual);
        }

        [Fact]
        public void ShouldReturnFindAllByListForTable()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.").ToArray();
            Assert.Contains("FindAllById", actual);
            Assert.Contains("FindAllByName", actual);
        }

        [Fact]
        public void ShouldReturnColumnListForTable()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.").ToArray();
            Assert.Contains("Id", actual);
            Assert.Contains("Name", actual);
        }

        [Fact]
        public void ShouldNotReturnAnythingAfterFindBy()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.FindById(1).").ToArray();
            Assert.Empty(actual);
        }

        [Fact]
        public void ShouldReturnQueryMethodsAfterQuery()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.Query().").ToArray();
            foreach (var queryMethod in ExpectedQueryMethods(false, "Id", "Name"))
            {
                Assert.Contains(queryMethod, actual);
            }
        }

        [Fact]
        public void ShouldReturnQueryMethodsAfterQueryAndChainedMethod()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.Query().Where(stuff).").ToArray();
            foreach (var queryMethod in ExpectedQueryMethods(false, "Id", "Name"))
            {
                Assert.Contains(queryMethod, actual);
            }
        }

        [Fact]
        public void ShouldReturnThenByMethodsAfterQueryAndChainedOrderBy()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.Query().OrderById(stuff).").ToArray();
            foreach (var queryMethod in ExpectedQueryMethods(true, "Id", "Name"))
            {
                Assert.Contains(queryMethod, actual);
            }
        }

        [Fact]
        public void ShouldReturnFilteredListOfOrderByWithColumns()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.Query().OrderB").ToArray();
            Assert.Equal(6, actual.Length);
        }

        [Fact]
        public void ShouldReturnColumnsWithColonsForNakedFindBy()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.FindBy(").ToArray();
            Assert.Contains("Id:", actual);
            Assert.Contains("Name:", actual);
        }

        [Fact]
        public void ShouldReturnColumnsWithColonsForNakedFindByExcludingExistingNamedParameter()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.FindBy(Id: 1, ").ToArray();
            Assert.DoesNotContain("Id:", actual);
            Assert.Contains("Name:", actual);
        }

        [Fact]
        public void ShouldReturnEmptyForNakedFindByWhenInValuePart()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.FindBy(Id: 1").ToArray();
            Assert.Empty(actual);
        }

        [Fact]
        public void ShouldReturnEmptyForNakedFindByWhenJustAfterNamedParameter()
        {
            var target = CreateTarget();
            var actual = target.GetOptions("db.Test.FindBy(Id:").ToArray();
            Assert.Empty(actual);
        }

        private static IEnumerable<string> ExpectedQueryMethods(bool includeThenBy, params string[] columns)
        {
            yield return "Select";
            yield return "Where";
            yield return "ReplaceWhere";
            if (includeThenBy)
            {
                yield return "ThenBy";
                yield return "ThenByDescending";
            }
            else
            {
                yield return "OrderBy";
                yield return "OrderByDescending";
            }
            yield return "Skip";
            yield return "Take";
            yield return "Join";

            if (columns.Length > 0)
            {
                foreach (var column in columns)
                {
                    if (includeThenBy)
                    {
                        yield return "ThenBy" + column;
                        yield return "ThenBy" + column + "Descending";
                    }
                    else
                    {
                        yield return "OrderBy" + column;
                        yield return "OrderBy" + column + "Descending";
                    }
                }
            }
        }
    }

    class StubSchemaProvider : ISchemaProvider
    {
        public IEnumerable<Table> GetTables()
        {
            yield return new Table("Test", "dbo", TableType.Table);
        }

        public IEnumerable<Column> GetColumns(Table table)
        {
            yield return new Column("Id", table);
            yield return new Column("Name", table);
        }

        public IEnumerable<Procedure> GetStoredProcedures()
        {
            yield break;
        }

        public IEnumerable<Parameter> GetParameters(Procedure storedProcedure)
        {
            yield break;
        }

        public Key GetPrimaryKey(Table table)
        {
            return new Key(new[] {"Id"});
        }

        public IEnumerable<ForeignKey> GetForeignKeys(Table table)
        {
            yield break;
        }

        public string QuoteObjectName(string unquotedName)
        {
            return "[" + unquotedName + "]";
        }

        public string NameParameter(string baseName)
        {
            return "@" + baseName;
        }
    }
}
