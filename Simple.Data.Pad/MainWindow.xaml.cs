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
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string[] NewlineSplitArg = new[] { Environment.NewLine };
        private readonly Typeface _typeface;
        private readonly MainViewModel _viewModel;

        private bool _deferKeyUp;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new MainViewModel();
            _viewModel.PropertyChanged += ViewModelPropertyChanged;
            _typeface = new Typeface(QueryTextBox.FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            AutoCompletePopup.Opened += AutoCompletePopupOpened;
            AutoCompletePopup.Closed += AutoCompletePopupClosed;
        }

        void AutoCompletePopupClosed(object sender, EventArgs e)
        {
            QueryTextBox.PreviewKeyUp -= MainWindowPreviewKeyUp;
            QueryTextBox.PreviewTextInput -= QueryTextBoxPreviewTextInput;
        }

        void AutoCompletePopupOpened(object sender, EventArgs e)
        {
            _deferKeyUp = true;
            QueryTextBox.PreviewKeyUp += MainWindowPreviewKeyUp;
            QueryTextBox.PreviewTextInput += QueryTextBoxPreviewTextInput;
        }

        void QueryTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "." || e.Text == "(")
            {
                SelectAutoCompleteText(e.Text);
                e.Handled = true;
            }
        }

        void MainWindowPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (_deferKeyUp)
            {
                _deferKeyUp = false;
                return;
            }
            if (e.Key == Key.Down)
            {
                if (AutoCompleteListBox.SelectedIndex + 1 < AutoCompleteListBox.Items.Count)
                {
                    AutoCompleteListBox.SelectedIndex += 1;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                if (AutoCompleteListBox.SelectedIndex > 0)
                {
                    AutoCompleteListBox.SelectedIndex -= 1;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                SelectAutoCompleteText();
                e.Handled = true;
            }
        }


        void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoCompleteOptions")
            {
                if (!_viewModel.AutoCompleteOptions.Any())
                {
                    AutoCompletePopup.IsOpen = false;
                }
                else
                {
                    OpenPopup();
                }
            }
        }

        private void OpenPopup()
        {
            var wasOpen = AutoCompletePopup.IsOpen;

            var options = _viewModel.AutoCompleteOptions.ToArray();
            AutoCompletePopup.IsOpen = true;
            AutoCompletePopup.Width = options.Select(s => s.Length*8).Max() + 8;
            AutoCompletePopup.Height = Math.Min((options.Length*16) + 16, 200) + 8;

            var text = _viewModel.QueryTextToCursor;
            var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                                  _typeface, QueryTextBox.FontSize, Brushes.Black);
            AutoCompletePopup.VerticalOffset = formattedText.Height + 4;

            text = text.Split(NewlineSplitArg, StringSplitOptions.None).Last();
            formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                                              _typeface, QueryTextBox.FontSize, Brushes.Black);
            AutoCompletePopup.HorizontalOffset = formattedText.Width;

            if (!wasOpen) AutoCompleteListBox.SelectedIndex = 0;
        }

        void MainWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5) _viewModel.Run.Execute(null);
        }

        private void QueryTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.CursorPosition = QueryTextBox.CaretIndex;
        }

        private void QueryTextBoxTextChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.CursorPosition = QueryTextBox.CaretIndex;
        }

        private void ListBoxMouseUp(object sender, RoutedEventArgs e)
        {
            SelectAutoCompleteText();
        }

        private void SelectAutoCompleteText(string andAppend = "")
        {
            var selected = AutoCompleteListBox.SelectedItem;
            while (QueryTextBox.SelectedText.FirstOrDefault() != '.')
            {
                QueryTextBox.SelectionStart -= 1;
                QueryTextBox.SelectionLength += 1;
            }
            QueryTextBox.SelectedText = "." + selected + andAppend;
            QueryTextBox.SelectionStart += QueryTextBox.SelectionLength;
            QueryTextBox.SelectionLength = 0;
            QueryTextBox.CaretIndex = QueryTextBox.SelectionStart;
            _viewModel.CursorPosition = QueryTextBox.CaretIndex;
            if (andAppend != ".")
            {
                AutoCompletePopup.IsOpen = false;
            }
            else
            {
                _viewModel.ForceAutoCompleteOptionsUpdate();
            }
        }

        private void AutoCompleteListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutoCompleteListBox.SelectedItem == null)
            {
                AutoCompleteListBox.SelectedIndex = 0;
            }
            else
            {
                AutoCompleteListBox.ScrollIntoView(AutoCompleteListBox.SelectedItem);
            }
        }
    }
}
