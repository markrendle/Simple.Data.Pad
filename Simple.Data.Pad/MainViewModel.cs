using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Pad
{
    using System.Windows.Input;
    using System.Windows.Media;
    using Interop;

    public class MainViewModel : ViewModelBase
    {
        private readonly ICommand _runCommand;

        public MainViewModel()
        {
            _runCommand = new ActionCommand(RunImpl);
        }

        private string _queryText;

        public ICommand Run
        {
            get { return _runCommand; }
        }

        public string QueryText
        {
            get { return _queryText; }
            set
            {
                Set(ref _queryText, value, "QueryText");
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

        void RunImpl()
        {
            var database = Database.OpenConnection("data source=.;initial catalog=SimpleTest;integrated security=true");
            var executor = new QueryExecutor(_queryText);
            object result;
            ResultColor = executor.CompileAndRun(database, out result) ? Colors.Black : Colors.Red;
            ResultText = FormatResult(result);
        }

        private static string FormatResult(object result)
        {
            if (result is SimpleRecord)
            {
                return FormatDictionary(result as IDictionary<string, object>);
            }

            return result.ToString();
        }

        private static string FormatDictionary(IDictionary<string, object> dictionary)
        {
            return string.Join(Environment.NewLine,
                               dictionary.Select(kvp => string.Format("{0}: {1}", kvp.Key, kvp.Value)));
        }
    }

    class ActionCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public ActionCommand(Action execute) : this(execute, () => true)
        {
        }

        public ActionCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            if (canExecute == null) throw new ArgumentNullException("canExecute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged;
    }
}
