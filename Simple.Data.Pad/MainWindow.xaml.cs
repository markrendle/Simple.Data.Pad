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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simple.Data.Pad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new MainViewModel();
        }

        void MainWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) _viewModel.Run.Execute(null);
        }

        private void TextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            var textBox = ((TextBox) sender);
            _viewModel.CursorPosition = textBox.SelectionStart + textBox.SelectionLength;
        }
    }
}
