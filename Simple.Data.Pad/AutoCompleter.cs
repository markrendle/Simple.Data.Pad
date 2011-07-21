namespace Simple.Data.Pad
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Ado;
    using Ado.Schema;
    using Interop;

    public class AutoCompleter
    {
        private static readonly Regex NonAlphaNumeric = new Regex("[^0-9a-zA-Z]+");
        private static readonly string[] Empty = new string[0];
        private readonly ISchemaProvider _schemaProvider;
        private readonly ConcurrentDictionary<string,string[]> _cache = new ConcurrentDictionary<string, string[]>();

        public AutoCompleter(Database database)
        {
            if (database != null)
            {
                var adoAdapter = database.GetAdapter() as AdoAdapter;
                if (adoAdapter != null)
                {
                    _schemaProvider = adoAdapter.SchemaProvider;
                }
            }
        }

        public IEnumerable<string> GetOptions(string currentText)
        {
            if (_schemaProvider == null || string.IsNullOrWhiteSpace(currentText) || (!currentText.Contains("."))) return Empty;

            var array = _cache.GetOrAdd(currentText, GetOptionsImpl);
            if (array.Length == 1 && currentText.Substring(currentText.LastIndexOf('.') + 1).Equals(array[0], StringComparison.CurrentCultureIgnoreCase))
                return Empty;
            return array;
        }

        private string[] GetOptionsImpl(string currentText)
        {
            var tokens = new Lexer(currentText).GetTokens().ToArray();
            if (tokens.Length < 2) return Empty;
            var db = tokens.First().Value;
            var token = tokens.Reverse().GetEnumerator();

            token.MoveNext();

            if (token.Current.Type != TokenType.Dot && token.Current.Type != TokenType.Identifier) return Empty;

            string partial = string.Empty;
            if (token.Current.Type == TokenType.Identifier)
            {
                partial = token.Current.Value.ToString();
                token.MoveNext();
            }

            if (token.Current.Type == TokenType.Dot)
            {
                token.MoveNext();
            }

            if (token.Current.Type != TokenType.Identifier)
            {
                return Empty;
            }

            var array = GetOptionsImpl(partial, token, db).ToArray();
            if (array.Length == 1 && array[0].Equals(partial, StringComparison.CurrentCultureIgnoreCase)) return Empty;
            return array;
        }

        private IEnumerable<string> GetOptionsImpl(string partial, IEnumerator<Token> token, object db)
        {
            if (token.Current.Value == db)
            {
                return DatabaseOptions()
                    .Select(Prettify)
                    .Where(s => s.StartsWith(partial, StringComparison.CurrentCultureIgnoreCase))
                    .OrderBy(s => s);
            }

            return TableOptions(token.Current.Value.ToString())
                .Where(s => s.StartsWith(partial, StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(s => s);
        }

        private IEnumerable<string> TableOptions(string tableName)
        {
            Table table = _schemaProvider.GetTables()
                .Where(t => Prettify(t.ActualName) == Prettify(tableName))
                .SingleOrDefault();

            if (table == null) yield break;
            foreach (var column in _schemaProvider.GetColumns(table).Select(c => Prettify(c.ActualName)))
            {
                yield return column;
                yield return "FindBy" + column;
                yield return "FindAllBy" + column;
            }
        }

        private IEnumerable<string> DatabaseOptions()
        {
            foreach (var table in _schemaProvider.GetTables())
            {
                yield return table.ActualName;
            }

            foreach (var storedProcedure in _schemaProvider.GetStoredProcedures())
            {
                yield return storedProcedure.Name;
            }
        }

        private static string Prettify(string source)
        {
            if (!NonAlphaNumeric.IsMatch(source)) return source;

            var builder = new StringBuilder();
            foreach (var word in NonAlphaNumeric.Replace(source, " ").Split(' '))
            {
                builder.Append(char.ToUpper(word[0]));
                builder.Append(word.Substring(1).ToLower());
            }

            return builder.ToString();
        }
    }
}