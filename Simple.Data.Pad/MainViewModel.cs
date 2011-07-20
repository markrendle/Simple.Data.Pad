using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Pad
{
    public class MainViewModel : ViewModelBase
    {
        private string _queryText;
        public string QueryText
        {
            get { return _queryText; }
            set
            {
                Set(ref _queryText, value, "QueryText");
            }
        }
    }
}
