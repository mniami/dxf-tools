using System.Windows;

namespace DxfTool.Views
{
    public partial class ErrorDialog : Window
    {
        public string ErrorMessage { get; set; } = string.Empty;

        public ErrorDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ErrorDialog(string errorMessage) : this()
        {
            ErrorMessage = errorMessage;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(ErrorMessage);
                MessageBox.Show("Error message copied to clipboard!", "Copy Successful", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Copy Failed", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static void ShowError(string errorMessage, string title = "Error")
        {
            var dialog = new ErrorDialog(errorMessage)
            {
                Title = title,
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }
    }
}
