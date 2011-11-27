using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Simple.Data.Pad
{
    /// <summary>
    /// Interaction logic for UnhandledExceptionDialog.xaml
    /// </summary>
    public partial class UnhandledExceptionDialog : Window
    {
        private readonly Exception _exception;

        public UnhandledExceptionDialog(Exception exception)
        {
            _exception = exception;
            InitializeComponent();
            ExceptionTextBox.Text = _exception.ToString();
        }

        private void MyFaultButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void YourFaultButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
