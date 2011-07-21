using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Pad
{
    using System.Collections;
    using System.Data;
    using System.Diagnostics;
    using System.Reflection;
    using System.Timers;
    using System.Windows.Input;
    using System.Windows.Media;
    using Interop;

    public class MainViewModel : ViewModelBase
    {
        private readonly ICommand _runCommand;
        private readonly Timer _timer;
        private AutoCompleter _autoCompleter = new AutoCompleter(null);

        public MainViewModel()
        {
            _databaseSelectorViewModel = new DatabaseSelectorViewModel();
            LoadSettings();
            _databaseSelectorViewModel.PropertyChanged += DatabaseSelectorViewModelPropertyChanged;
            _runCommand = new ActionCommand(RunImpl);
            _timer = new Timer(500) { AutoReset = false };
            _timer.Elapsed += TimerElapsed;
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _autoCompleter = new AutoCompleter(CreateDatabase());
            }
            catch (Exception)
            {
                Trace.WriteLine("Failed to open database.");
            }
        }

        void DatabaseSelectorViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _timer.Stop();
            _timer.Start();
        }

        private void LoadSettings()
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();
            }
            QueryText = Properties.Settings.Default.LastQuery;
            _databaseSelectorViewModel.SelectedMethod = _databaseSelectorViewModel.Methods
                .FirstOrDefault(
                    m =>
                    m.Name.Equals(Properties.Settings.Default.OpenMethod) &&
                    m.GetParameters().Length == Properties.Settings.Default.OpenMethodParameterCount);
            _databaseSelectorViewModel.Parameter1 = Properties.Settings.Default.OpenMethodParameter1;
            _databaseSelectorViewModel.Parameter2 = Properties.Settings.Default.OpenMethodParameter2;

            try
            {
                _autoCompleter = new AutoCompleter(CreateDatabase());
            }
            catch (Exception)
            {
                Trace.WriteLine("Failed to open database.");
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.LastQuery = QueryText;
            Properties.Settings.Default.OpenMethod = _databaseSelectorViewModel.SelectedMethod.Name;
            Properties.Settings.Default.OpenMethodParameterCount =
                _databaseSelectorViewModel.SelectedMethod.GetParameters().Length;
            Properties.Settings.Default.OpenMethodParameter1 = _databaseSelectorViewModel.Parameter1;
            Properties.Settings.Default.OpenMethodParameter2 = _databaseSelectorViewModel.Parameter2;
            Properties.Settings.Default.Save();
        }

        public string WindowTitle
        {
            get
            {
                return string.Format("Simple.Data.Pad {0}",
                                     Assembly.GetAssembly(typeof(Database)).GetName().Version.ToString(3));
            }
        }

        private string _queryText;

        public ICommand Run
        {
            get { return _runCommand; }
        }

        private readonly DatabaseSelectorViewModel _databaseSelectorViewModel;

        public DatabaseSelectorViewModel DatabaseSelectorViewModel
        {
            get { return _databaseSelectorViewModel; }
        }

        public int CursorPosition { get; set; }

        public string QueryText
        {
            get { return _queryText; }
            set
            {
                if (Set(ref _queryText, value, "QueryText"))
                {
                    RaisePropertyChanged("AutoCompleteOptions");
                }
            }
        }

        private string _resultText;
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                Set(ref _resultText, value, "ResultText");
            }
        }

        private Color _resultColor;
        public Color ResultColor
        {
            get { return _resultColor; }
            set
            {
                Set(ref _resultColor, value, "ResultColor");
            }
        }

        private bool _dataAvailable;
        public bool DataAvailable
        {
            get { return _dataAvailable; }
            set
            {
                Set(ref _dataAvailable, value, "DataAvailable");
            }
        }

        private object _data;
        public object Data
        {
            get { return _data; }
            set
            {
                Set(ref _data, value, "Data");
            }
        }

        public IEnumerable<string> AutoCompleteOptions
        {
            get { return _autoCompleter.GetOptions(QueryText.Substring(0, CursorPosition + 1)); }
        }

        void RunImpl()
        {
            SaveSettings();
            var database = CreateDatabase();
            var executor = new QueryExecutor(_queryText);
            object result;
            DataAvailable = executor.CompileAndRun(database, out result);
            ResultColor = DataAvailable ? Colors.Black : Colors.Red;
            Data = FormatResult(result);
        }

        private Database CreateDatabase()
        {
            var method = DatabaseSelectorViewModel.SelectedMethod;
            var parameters = BuildParameters(method);
            var database = method.Invoke(Database.Opener, parameters) as Database;
            return database;
        }

        private string[] BuildParameters(MethodInfo method)
        {
            string[] parameters;
            if (method.GetParameters().Length == 1)
            {
                parameters = new[] { DatabaseSelectorViewModel.Parameter1 };
            }
            else if (method.GetParameters().Length == 2)
            {
                parameters = new[] { DatabaseSelectorViewModel.Parameter1, DatabaseSelectorViewModel.Parameter2 };
            }
            else
            {
                parameters = new string[0];
            }
            return parameters;
        }

        private static object FormatResult(object result)
        {
            if (result is SimpleRecord)
            {
                return FormatDictionary(result as IDictionary<string, object>);
            }

            if (result is SimpleQuery)
            {
                return FormatQuery(result as SimpleQuery);
            }

            return result.ToString();
        }

        private static object FormatQuery(SimpleQuery simpleQuery)
        {
            var list = simpleQuery.ToList();
            if (list.Count == 0) return "No matching records.";

            var firstRow = list.FirstOrDefault() as IDictionary<string, object>;
            if (firstRow == null) throw new InvalidOperationException();

            var table = new DataTable();
            foreach (var kvp in firstRow)
            {
                table.Columns.Add(kvp.Key);
            }

            foreach (var row in list.Cast<IDictionary<string,object>>())
            {
                table.Rows.Add(row.Values.ToArray());
            }

            return table.DefaultView;
        }

        private static object FormatDictionary(IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            var table = new DataTable();
            table.Columns.Add("Property");
            table.Columns.Add("Value");

            foreach (var kvp in dictionary)
            {
                table.Rows.Add(kvp.Key, kvp.Value);
            }

            return table.DefaultView;
        }
    }
}
