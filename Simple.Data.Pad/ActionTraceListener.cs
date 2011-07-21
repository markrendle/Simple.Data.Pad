using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Simple.Data.Pad
{
    class ActionTraceListener : TraceListener
    {
        private readonly Action<string> _action;

        public ActionTraceListener(Action<string> action)
        {
            _action = action;
        }

        public override void Write(string message)
        {
            _action(message);
        }

        public override void WriteLine(string message)
        {
            _action(message + Environment.NewLine);
        }
    }
}
