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
        private readonly IDxfService parser;
        private readonly IUpdateService updateService;
        private readonly ILoggingService logger;
        private readonly ISettingsService settingsService;
        private string _dxfFilePath = string.Empty;
        private string _soundPlanFilePath = string.Empty;
        private string _finalTableCsvFilePath = string.Empty;
        private string _destinationFilePath = string.Empty;
        private string _statusMessage = string.Empty;
        private string _resultsText = string.Empty;
        private DataExtractionType _selectedDataType = DataExtractionType.GeometryPoints;
        private bool _updateAvailable = false;
        private string _updateMessage = string.Empty;
        
        public MainViewModel(IDxfService parser, IUpdateService updateService, ILoggingService logger, ISettingsService settingsService)
        {
            this.parser = parser;
            this.updateService = updateService;
            this.logger = logger;
            this.settingsService = settingsService;
            InitializeCommands();
            
            logger.LogInformation("MainViewModel initialized");
            _ = Task.Run(CheckForUpdatesAsync); // Fire and forget on background thread
            _ = Task.Run(LoadUserPreferencesAsync); // Load saved preferences
        }

        public string DxfFilePath
        {
            get => _dxfFilePath;
            set 
            { 
                if (SetProperty(ref _dxfFilePath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    _ = SaveUserPreferencesAsync(); // Fire and forget
                }
            }
        }

        public string SoundPlanFilePath
        {
            get => _soundPlanFilePath;
            set 
            { 
                if (SetProperty(ref _soundPlanFilePath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    _ = SaveUserPreferencesAsync(); // Fire and forget
                }
            }
        }

        public string FinalTableCsvFilePath
        {
            get => _finalTableCsvFilePath;
            set 
            { 
                if (SetProperty(ref _finalTableCsvFilePath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    _ = SaveUserPreferencesAsync(); // Fire and forget
                }
            }
        }

        public string DestinationFilePath
        {
            get => _destinationFilePath;
            set 
            { 
                if (SetProperty(ref _destinationFilePath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    _ = SaveUserPreferencesAsync(); // Fire and forget
                }
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
                if (SetProperty(ref _selectedDataType, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                    _ = SaveUserPreferencesAsync(); // Fire and forget
                }
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
        public ICommand BrowseFinalTableCsvFileCommand { get; private set; } = null!;
        public ICommand BrowseDestinationFileCommand { get; private set; } = null!;
        public ICommand ProcessFileCommand { get; private set; } = null!;
        public ICommand CheckForUpdatesCommand { get; private set; } = null!;
        public ICommand InstallUpdateCommand { get; private set; } = null!;
        public ICommand OpenLogFileCommand { get; private set; } = null!;
        public ICommand ClearPreferencesCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            BrowseDxfFileCommand = new RelayCommand(BrowseDxfFile);
            BrowseSoundPlanFileCommand = new RelayCommand(BrowseSoundPlanFile);
            BrowseFinalTableCsvFileCommand = new RelayCommand(BrowseFinalTableCsvFile);
            BrowseDestinationFileCommand = new RelayCommand(BrowseDestinationFile);
            ProcessFileCommand = new RelayCommand(ProcessFile, CanProcessFile);
            CheckForUpdatesCommand = new RelayCommand(async () => await CheckForUpdatesAsync());
            InstallUpdateCommand = new RelayCommand(async () => await InstallUpdateAsync(), () => UpdateAvailable);
            OpenLogFileCommand = new RelayCommand(() => logger.OpenLogFile());
            ClearPreferencesCommand = new RelayCommand(async () => await ClearPreferencesAsync());
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

        private void BrowseFinalTableCsvFile()
        {
            logger.LogDebug("Opening Final Table CSV file browser dialog");
            
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Pliki CSV (*.csv)|*.csv|Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz plik CSV z tabelą końcową"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FinalTableCsvFilePath = openFileDialog.FileName;
                logger.LogInformation("Final Table CSV file selected: {FilePath}", FinalTableCsvFilePath);
            }
            else
            {
                logger.LogDebug("Final Table CSV file selection cancelled");
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

            // If GeometryPoints is selected, also require SoundPlan file and Final Table CSV file
            if (SelectedDataType == DataExtractionType.GeometryPoints)
            {
                return basicValidation && 
                       !string.IsNullOrWhiteSpace(SoundPlanFilePath) && 
                       File.Exists(SoundPlanFilePath) &&
                       !string.IsNullOrWhiteSpace(FinalTableCsvFilePath) && 
                       File.Exists(FinalTableCsvFilePath);
            }

            return basicValidation;
        }

        private void ProcessFile()
        {
            try
            {
            logger.LogInformation("Starting file processing - DataType: {DataType}, Source: {SourceFile}, Destination: {DestinationFile}, SoundPlan: {SoundPlanFile}, FinalTableCsv: {FinalTableCsvFile}", 
                SelectedDataType, DxfFilePath, DestinationFilePath, SoundPlanFilePath ?? "N/A", FinalTableCsvFilePath ?? "N/A");                StatusMessage = "Przetwarzanie...";
                ResultsText = string.Empty;

                int result = SelectedDataType switch
                {
                    DataExtractionType.HighPoints => parser.FindHighPoints(DxfFilePath, DestinationFilePath),
                    DataExtractionType.GeometryPoints => parser.FindPointsWithMultiLeadersSave(DxfFilePath, SoundPlanFilePath ?? string.Empty, FinalTableCsvFilePath ?? string.Empty, DestinationFilePath),
                    _ => throw new NotSupportedException($"Typ ekstrakcji danych '{SelectedDataType}' nie jest obsługiwany.")
                };
                
                logger.LogInformation("File processing completed successfully - Found {Count} elements", result);
                
                StatusMessage = "Przetwarzanie zakończone pomyślnie";
                var resultText = $"Typ ekstrakcji danych: {SelectedDataType.GetDisplayName()}\n" +
                               $"Liczba znalezionych elementów: {result}\n" +
                               $"Plik źródłowy DXF: {DxfFilePath}\n";
                
                if (SelectedDataType == DataExtractionType.GeometryPoints)
                {
                    if (!string.IsNullOrWhiteSpace(SoundPlanFilePath))
                    {
                        resultText += $"Plik SoundPlan: {SoundPlanFilePath}\n";
                    }
                    if (!string.IsNullOrWhiteSpace(FinalTableCsvFilePath))
                    {
                        resultText += $"Plik CSV z tabelą końcową: {FinalTableCsvFilePath}\n";
                    }
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
                logger.LogError(ex, "File processing failed - DataType: {DataType}, Source: {SourceFile}, Destination: {DestinationFile}, SoundPlan: {SoundPlanFile}, FinalTableCsv: {FinalTableCsvFile}", 
                    SelectedDataType, DxfFilePath, DestinationFilePath, SoundPlanFilePath ?? "N/A", FinalTableCsvFilePath ?? "N/A");
                
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
                                            $"Plik CSV z tabelą końcową: {FinalTableCsvFilePath ?? "N/A"}\n" +
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

        private async Task LoadUserPreferencesAsync()
        {
            try
            {
                logger.LogDebug("Loading user preferences");
                var preferences = await settingsService.LoadPreferencesAsync();
                
                // Update properties without triggering save (to avoid circular calls)
                _dxfFilePath = preferences.DxfFilePath;
                _soundPlanFilePath = preferences.SoundPlanFilePath;
                _finalTableCsvFilePath = preferences.FinalTableCsvFilePath;
                _destinationFilePath = preferences.DestinationFilePath;
                _selectedDataType = preferences.SelectedDataType;
                
                // Notify property changes
                OnPropertyChanged(nameof(DxfFilePath));
                OnPropertyChanged(nameof(SoundPlanFilePath));
                OnPropertyChanged(nameof(FinalTableCsvFilePath));
                OnPropertyChanged(nameof(DestinationFilePath));
                OnPropertyChanged(nameof(SelectedDataType));
                
                CommandManager.InvalidateRequerySuggested();
                
                logger.LogInformation("User preferences loaded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load user preferences");
            }
        }

        private async Task SaveUserPreferencesAsync()
        {
            try
            {
                var preferences = new UserPreferences
                {
                    DxfFilePath = DxfFilePath,
                    SoundPlanFilePath = SoundPlanFilePath,
                    FinalTableCsvFilePath = FinalTableCsvFilePath,
                    DestinationFilePath = DestinationFilePath,
                    SelectedDataType = SelectedDataType
                };
                
                await settingsService.SavePreferencesAsync(preferences);
                logger.LogDebug("User preferences saved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save user preferences");
            }
        }

        private async Task ClearPreferencesAsync()
        {
            try
            {
                logger.LogInformation("Clearing user preferences");
                
                var result = MessageBox.Show(
                    "Czy na pewno chcesz wyczyścić wszystkie zapisane preferencje?\nTo działanie nie może zostać cofnięte.", 
                    "Wyczyść preferencje", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    await settingsService.ClearPreferencesAsync();
                    
                    // Reset all fields to default values
                    DxfFilePath = string.Empty;
                    SoundPlanFilePath = string.Empty;
                    FinalTableCsvFilePath = string.Empty;
                    DestinationFilePath = string.Empty;
                    SelectedDataType = DataExtractionType.GeometryPoints;
                    StatusMessage = string.Empty;
                    ResultsText = string.Empty;
                    
                    logger.LogInformation("User preferences cleared successfully");
                    MessageBox.Show("Preferencje zostały wyczyszczone.", "Preferencje wyczyszczone", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    logger.LogDebug("User cancelled clearing preferences");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to clear user preferences");
                MessageBox.Show($"Nie udało się wyczyścić preferencji: {ex.Message}", "Błąd", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
