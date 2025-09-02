using DxfTool.Models;
using DxfTool.Services;
using DxfTool.Views;
using DxfToolLib.Helpers;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DxfTool.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDxfParser parser;
        private readonly IUpdateService updateService;
        private readonly ILoggingService logger;
        private string _dxfFilePath = string.Empty;
        private string _soundPlanFilePath = string.Empty;
        private string _destinationFilePath = string.Empty;
        private string _statusMessage = string.Empty;
        private string _resultsText = string.Empty;
        private DataExtractionType _selectedDataType = DataExtractionType.HighPoints;
        private bool _updateAvailable = false;
        private string _updateMessage = string.Empty;
        
        public MainViewModel(IDxfParser parser, IUpdateService updateService, ILoggingService logger)
        {
            this.parser = parser;
            this.updateService = updateService;
            this.logger = logger;
            InitializeCommands();
            
            logger.LogInformation("MainViewModel initialized");
            _ = Task.Run(CheckForUpdatesAsync); // Fire and forget on background thread
        }

        public string DxfFilePath
        {
            get => _dxfFilePath;
            set 
            { 
                SetProperty(ref _dxfFilePath, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SoundPlanFilePath
        {
            get => _soundPlanFilePath;
            set 
            { 
                SetProperty(ref _soundPlanFilePath, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string DestinationFilePath
        {
            get => _destinationFilePath;
            set 
            { 
                SetProperty(ref _destinationFilePath, value);
                CommandManager.InvalidateRequerySuggested();
            }
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

        public DataExtractionType SelectedDataType
        {
            get => _selectedDataType;
            set 
            { 
                SetProperty(ref _selectedDataType, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Array DataExtractionTypes => Enum.GetValues(typeof(DataExtractionType));

        public bool UpdateAvailable
        {
            get => _updateAvailable;
            set => SetProperty(ref _updateAvailable, value);
        }

        public string UpdateMessage
        {
            get => _updateMessage;
            set => SetProperty(ref _updateMessage, value);
        }

        public ICommand BrowseDxfFileCommand { get; private set; } = null!;
        public ICommand BrowseSoundPlanFileCommand { get; private set; } = null!;
        public ICommand BrowseDestinationFileCommand { get; private set; } = null!;
        public ICommand ProcessFileCommand { get; private set; } = null!;
        public ICommand CheckForUpdatesCommand { get; private set; } = null!;
        public ICommand InstallUpdateCommand { get; private set; } = null!;
        public ICommand OpenLogFileCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            BrowseDxfFileCommand = new RelayCommand(BrowseDxfFile);
            BrowseSoundPlanFileCommand = new RelayCommand(BrowseSoundPlanFile);
            BrowseDestinationFileCommand = new RelayCommand(BrowseDestinationFile);
            ProcessFileCommand = new RelayCommand(ProcessFile, CanProcessFile);
            CheckForUpdatesCommand = new RelayCommand(async () => await CheckForUpdatesAsync());
            InstallUpdateCommand = new RelayCommand(async () => await InstallUpdateAsync(), () => UpdateAvailable);
            OpenLogFileCommand = new RelayCommand(() => logger.OpenLogFile());
        }

        private void BrowseDxfFile()
        {
            logger.LogDebug("Opening DXF file browser dialog");
            
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Pliki DXF (*.dxf)|*.dxf|Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz plik DXF"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DxfFilePath = openFileDialog.FileName;
                logger.LogInformation("DXF file selected: {FilePath}", DxfFilePath);
            }
            else
            {
                logger.LogDebug("DXF file selection cancelled");
            }
        }

        private void BrowseSoundPlanFile()
        {
            logger.LogDebug("Opening SoundPlan file browser dialog");
            
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz plik SoundPlan"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SoundPlanFilePath = openFileDialog.FileName;
                logger.LogInformation("SoundPlan file selected: {FilePath}", SoundPlanFilePath);
            }
            else
            {
                logger.LogDebug("SoundPlan file selection cancelled");
            }
        }

        private void BrowseDestinationFile()
        {
            logger.LogDebug("Opening destination file browser dialog");
            
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt|Pliki CSV (*.csv)|*.csv|Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz plik docelowy"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DestinationFilePath = saveFileDialog.FileName;
                logger.LogInformation("Destination file selected: {FilePath}", DestinationFilePath);
            }
            else
            {
                logger.LogDebug("Destination file selection cancelled");
            }
        }

        private bool CanProcessFile()
        {
            bool basicValidation = !string.IsNullOrWhiteSpace(DxfFilePath) && 
                                 !string.IsNullOrWhiteSpace(DestinationFilePath) &&
                                 File.Exists(DxfFilePath);

            // If GeometryPoints is selected, also require SoundPlan file
            if (SelectedDataType == DataExtractionType.GeometryPoints)
            {
                return basicValidation && 
                       !string.IsNullOrWhiteSpace(SoundPlanFilePath) && 
                       File.Exists(SoundPlanFilePath);
            }

            return basicValidation;
        }

        private void ProcessFile()
        {
            try
            {
                logger.LogInformation("Starting file processing - DataType: {DataType}, Source: {SourceFile}, Destination: {DestinationFile}, SoundPlan: {SoundPlanFile}", 
                    SelectedDataType, DxfFilePath, DestinationFilePath, SoundPlanFilePath ?? "N/A");
                
                StatusMessage = "Przetwarzanie...";
                ResultsText = string.Empty;

                int result = SelectedDataType switch
                {
                    DataExtractionType.HighPoints => parser.FindHighPoints(DxfFilePath, DestinationFilePath),
                    DataExtractionType.GeometryPoints => parser.FindPointsWithMultiLeadersSave(DxfFilePath, SoundPlanFilePath ?? string.Empty, DestinationFilePath),
                    _ => throw new NotSupportedException($"Typ ekstrakcji danych '{SelectedDataType}' nie jest obsługiwany.")
                };
                
                logger.LogInformation("File processing completed successfully - Found {Count} elements", result);
                
                StatusMessage = "Przetwarzanie zakończone pomyślnie";
                var resultText = $"Typ ekstrakcji danych: {SelectedDataType.GetDisplayName()}\n" +
                               $"Liczba znalezionych elementów: {result}\n" +
                               $"Plik źródłowy DXF: {DxfFilePath}\n";
                
                if (SelectedDataType == DataExtractionType.GeometryPoints && !string.IsNullOrWhiteSpace(SoundPlanFilePath))
                {
                    resultText += $"Plik SoundPlan: {SoundPlanFilePath}\n";
                }
                
                resultText += $"Plik wyjściowy: {DestinationFilePath}";
                ResultsText = resultText;

                MessageBox.Show($"Przetwarzanie zakończone!\nZnaleziono {SelectedDataType.GetDisplayName()}: {result}", 
                               "Sukces", 
                               MessageBoxButton.OK, 
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "File processing failed - DataType: {DataType}, Source: {SourceFile}, Destination: {DestinationFile}, SoundPlan: {SoundPlanFile}", 
                    SelectedDataType, DxfFilePath, DestinationFilePath, SoundPlanFilePath ?? "N/A");
                
                StatusMessage = "Wystąpił błąd podczas przetwarzania";
                ResultsText = $"Błąd: {ex.Message}";
                
                // Print detailed error to console (legacy support)
                Console.WriteLine("=== SZCZEGÓŁY BŁĘDU ===");
                Console.WriteLine($"Czas: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Typ błędu: {ex.GetType().Name}");
                Console.WriteLine($"Wiadomość: {ex.Message}");
                Console.WriteLine($"Ślad stosu:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("======================");
                
                // Create detailed error message for dialog
                string detailedErrorMessage = $"Typ błędu: {ex.GetType().Name}\n\n" +
                                            $"Wiadomość: {ex.Message}\n\n" +
                                            $"Ślad stosu:\n{ex.StackTrace}\n\n" +
                                            $"Czas: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                            $"Plik DXF: {DxfFilePath}\n" +
                                            $"Plik SoundPlan: {SoundPlanFilePath ?? "N/A"}\n" +
                                            $"Plik wyjściowy: {DestinationFilePath}\n" +
                                            $"Typ danych: {SelectedDataType}\n\n" +
                                            $"Plik logów: {logger.GetLogFilePath()}";
                
                // Show custom error dialog that allows copying
                ErrorDialog.ShowError(detailedErrorMessage, "Błąd przetwarzania");
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                logger.LogInformation("Checking for updates");
                
                var currentVersion = await updateService.GetCurrentVersionAsync();
                var updateAvailable = await updateService.CheckForUpdatesAsync();
                UpdateAvailable = updateAvailable;
                
                if (updateAvailable)
                {
                    var latestVersion = await updateService.GetLatestVersionAsync();
                    UpdateMessage = $"Dostępna aktualizacja: v{latestVersion} (bieżąca: v{currentVersion})";
                    
                    logger.LogInformation("Update available - Current: {CurrentVersion}, Latest: {LatestVersion}", 
                        currentVersion, latestVersion);
                }
                else
                {
                    UpdateMessage = $"Masz najnowszą wersję: v{currentVersion}";
                    logger.LogInformation("No updates available");
                }
            }
            catch (Exception ex)
            {
                var currentVersion = await updateService.GetCurrentVersionAsync();
                UpdateMessage = $"Nie udało się sprawdzić aktualizacji: {ex.Message} (bieżąca: v{currentVersion})";
                logger.LogError(ex, "Failed to check for updates");
            }
        }

        private async Task InstallUpdateAsync()
        {
            try
            {
                logger.LogInformation("Starting update installation");
                UpdateMessage = "Pobieranie i instalowanie aktualizacji...";
                await updateService.DownloadAndInstallUpdateAsync();
                logger.LogInformation("Update installation completed");
            }
            catch (Exception ex)
            {
                UpdateMessage = $"Nie udało się zainstalować aktualizacji: {ex.Message}";
                logger.LogError(ex, "Update installation failed");
                MessageBox.Show($"Aktualizacja nie powiodła się: {ex.Message}", "Błąd aktualizacji", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
