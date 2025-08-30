using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace DxfTool.Services
{
    public class UpdateService : IUpdateService
    {
        private string? _latestVersion;
        private string? _downloadUrl;
        private readonly HttpClient _httpClient = new();
        
        public bool IsUpdateAvailable { get; private set; }

        // TODO: Replace with your actual website URL
        private const string UpdateApiUrl = "https://your-website.com/updates/latest.json";

        public async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(UpdateApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    
                    _latestVersion = doc.RootElement.GetProperty("version").GetString();
                    _downloadUrl = doc.RootElement.GetProperty("downloadUrl").GetString();
                    
                    if (_latestVersion != null)
                    {
                        var currentVersion = await GetCurrentVersionAsync();
                        
                        // Simple version comparison
                        IsUpdateAvailable = !string.Equals(currentVersion, _latestVersion, StringComparison.OrdinalIgnoreCase);
                        return IsUpdateAvailable;
                    }
                }
                
                IsUpdateAvailable = false;
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking for updates: {ex.Message}");
                IsUpdateAvailable = false;
                return false;
            }
        }

        public Task DownloadAndInstallUpdateAsync()
        {
            try
            {
                // Open your website download page
                var downloadUrl = _downloadUrl ?? "https://your-website.com";
                Process.Start(new ProcessStartInfo(downloadUrl) { UseShellExecute = true });
                
                MessageBox.Show($"A new version (v{_latestVersion}) is available!\n\nYour browser will open the download page.\nDownload and run the new installer to update.", 
                    "Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening update page: {ex.Message}");
                MessageBox.Show($"Failed to open update page: {ex.Message}", "Update Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            return Task.CompletedTask;
        }

        public Task<string> GetCurrentVersionAsync()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return Task.FromResult(version?.ToString(3) ?? "1.0.12"); // Return only major.minor.patch
        }

        public Task<string> GetLatestVersionAsync()
        {
            return Task.FromResult(_latestVersion ?? "Unknown");
        }
    }
}
