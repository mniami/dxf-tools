using DxfToolLib.Helpers;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DxfTool.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDxfParser parser;
        private string _dxfFilePath = string.Empty;
        private string _destinationFilePath = string.Empty;
        private string _statusMessage = "Ready";
        private string _resultsText = string.Empty;

        public MainViewModel(IDxfParser parser)
        {
            this.parser = parser;
            InitializeCommands();
        }

        public string DxfFilePath
        {
            get => _dxfFilePath;
            set => SetProperty(ref _dxfFilePath, value);
        }

        public string DestinationFilePath
        {
            get => _destinationFilePath;
            set => SetProperty(ref _destinationFilePath, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string ResultsText
        {
            get => _resultsText;
            set => SetProperty(ref _resultsText, value);
        }

        public ICommand BrowseDxfFileCommand { get; private set; } = null!;
        public ICommand BrowseDestinationFileCommand { get; private set; } = null!;
        public ICommand ProcessFileCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            BrowseDxfFileCommand = new RelayCommand(BrowseDxfFile);
            BrowseDestinationFileCommand = new RelayCommand(BrowseDestinationFile);
            ProcessFileCommand = new RelayCommand(ProcessFile, CanProcessFile);
        }

        private void BrowseDxfFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "DXF Files (*.dxf)|*.dxf|All Files (*.*)|*.*",
                Title = "Select DXF File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DxfFilePath = openFileDialog.FileName;
            }
        }

        private void BrowseDestinationFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Select Destination File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DestinationFilePath = saveFileDialog.FileName;
            }
        }

        private bool CanProcessFile()
        {
            return !string.IsNullOrWhiteSpace(DxfFilePath) && 
                   !string.IsNullOrWhiteSpace(DestinationFilePath) &&
                   File.Exists(DxfFilePath);
        }

        private void ProcessFile()
        {
            try
            {
                StatusMessage = "Processing...";
                ResultsText = string.Empty;

                var result = parser.FindHighPoints("", DxfFilePath, DestinationFilePath);
                
                StatusMessage = "Processing completed successfully";
                ResultsText = $"Number of high points found: {result}\n" +
                             $"Source file: {DxfFilePath}\n" +
                             $"Output file: {DestinationFilePath}";

                MessageBox.Show($"Processing completed!\nHigh points found: {result}", 
                               "Success", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = "Error occurred during processing";
                ResultsText = $"Error: {ex.Message}";
                
                MessageBox.Show($"An error occurred: {ex.Message}", 
                               "Error", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Error);
            }
        }
    }
}
