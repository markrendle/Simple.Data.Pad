namespace Simple.Data.Pad
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DatabaseSelectorViewModel : ViewModelBase
    {
        private static readonly MethodInfo[] DatabaseOpenerMethods = GetDatabaseOpenerMethods();

        private static MethodInfo[] GetDatabaseOpenerMethods()
        {
            return typeof (IDatabaseOpener).GetMethods()
                .Where(m => m.Name.StartsWith("Open") && m.GetParameters().All(p => p.ParameterType == typeof(string)))
                .ToArray();
        }

        public IEnumerable<MethodInfo> Methods
        {
            get { return DatabaseOpenerMethods; }
        }

        private MethodInfo _selectedMethod;
        public MethodInfo SelectedMethod
        {
            get { return _selectedMethod; }
            set
            {
                if (Set(ref _selectedMethod, value, "SelectedMethod"))
                {
                    SetParameterVisibility();
                }
            }
        }

        private void SetParameterVisibility()
        {
            if (SelectedMethod != null)
            {
                var parameters = SelectedMethod.GetParameters();
                HasParameter1 = parameters.Length > 0;
                if (HasParameter1)
                {
                    Parameter1 = parameters[0].Name;
                }
                HasParameter2 = parameters.Length > 1;
                if (HasParameter2)
                {
                    Parameter2 = parameters[1].Name;
                }
            }
        }

        private string _parameter1;
        public string Parameter1
        {
            get { return _parameter1; }
            set
            {
                Set(ref _parameter1, value, "Parameter1");
            }
        }

        private string _parameter2;
        public string Parameter2
        {
            get { return _parameter2; }
            set
            {
                Set(ref _parameter2, value, "Parameter2");
            }
        }

        private bool _hasParameter1;
        public bool HasParameter1
        {
            get { return _hasParameter1; }
            set
            {
                Set(ref _hasParameter1, value, "HasParameter1");
            }
        }

        private bool _hasParameter2;

        public DatabaseSelectorViewModel()
        {
            SelectedMethod = DatabaseOpenerMethods.Where(m => m.Name.Equals("OpenConnection")).OrderBy(m => m.GetParameters().Length).FirstOrDefault();
        }

        public bool HasParameter2
        {
            get { return _hasParameter2; }
            set
            {
                Set(ref _hasParameter2, value, "HasParameter2");
            }
        }
    }
}