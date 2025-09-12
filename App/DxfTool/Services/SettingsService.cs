using DxfTool.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DxfTool.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ILoggingService _logger;
        private readonly string _settingsFilePath;

        public SettingsService(ILoggingService logger)
        {
            _logger = logger;
            
            // Store settings in user's AppData\Roaming folder
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, "DxfTool");
            
            // Ensure the directory exists
            Directory.CreateDirectory(appFolder);
            
            _settingsFilePath = Path.Combine(appFolder, "user-preferences.json");
            
            _logger.LogDebug("Settings file path: {SettingsPath}", _settingsFilePath);
        }

        public async Task<UserPreferences> LoadPreferencesAsync()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    _logger.LogInformation("Settings file does not exist, returning default preferences");
                    return new UserPreferences();
                }

                var json = await File.ReadAllTextAsync(_settingsFilePath);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogInformation("Settings file is empty, returning default preferences");
                    return new UserPreferences();
                }

                var preferences = JsonSerializer.Deserialize<UserPreferences>(json, GetJsonOptions());
                
                if (preferences == null)
                {
                    _logger.LogWarning("Failed to deserialize preferences, returning default");
                    return new UserPreferences();
                }

                _logger.LogInformation("Successfully loaded user preferences from {Path}", _settingsFilePath);
                
                // Validate that referenced files still exist
                ValidateFilePaths(preferences);
                
                return preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user preferences from {Path}", _settingsFilePath);
                return new UserPreferences();
            }
        }

        public async Task SavePreferencesAsync(UserPreferences preferences)
        {
            try
            {
                preferences.LastSaved = DateTime.Now;
                
                var json = JsonSerializer.Serialize(preferences, GetJsonOptions());
                await File.WriteAllTextAsync(_settingsFilePath, json);
                
                _logger.LogInformation("Successfully saved user preferences to {Path}", _settingsFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save user preferences to {Path}", _settingsFilePath);
                throw;
            }
        }

        public Task ClearPreferencesAsync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                    _logger.LogInformation("Successfully cleared user preferences from {Path}", _settingsFilePath);
                }
                else
                {
                    _logger.LogDebug("Settings file does not exist, nothing to clear");
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear user preferences from {Path}", _settingsFilePath);
                throw;
            }
        }

        public Task<bool> PreferencesExistAsync()
        {
            return Task.FromResult(File.Exists(_settingsFilePath) && new FileInfo(_settingsFilePath).Length > 0);
        }

        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private void ValidateFilePaths(UserPreferences preferences)
        {
            // Clear file paths that no longer exist
            if (!string.IsNullOrEmpty(preferences.DxfFilePath) && !File.Exists(preferences.DxfFilePath))
            {
                _logger.LogWarning("DXF file no longer exists: {Path}", preferences.DxfFilePath);
                preferences.DxfFilePath = string.Empty;
            }

            if (!string.IsNullOrEmpty(preferences.SoundPlanFilePath) && !File.Exists(preferences.SoundPlanFilePath))
            {
                _logger.LogWarning("SoundPlan file no longer exists: {Path}", preferences.SoundPlanFilePath);
                preferences.SoundPlanFilePath = string.Empty;
            }

            if (!string.IsNullOrEmpty(preferences.FinalTableCsvFilePath) && !File.Exists(preferences.FinalTableCsvFilePath))
            {
                _logger.LogWarning("Final Table CSV file no longer exists: {Path}", preferences.FinalTableCsvFilePath);
                preferences.FinalTableCsvFilePath = string.Empty;
            }

            // For destination file, we only check if the directory exists (not the file itself)
            if (!string.IsNullOrEmpty(preferences.DestinationFilePath))
            {
                var directory = Path.GetDirectoryName(preferences.DestinationFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    _logger.LogWarning("Destination file directory no longer exists: {Path}", directory);
                    preferences.DestinationFilePath = string.Empty;
                }
            }
        }
    }
}
