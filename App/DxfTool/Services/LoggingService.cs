using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace DxfTool.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly string _logFilePath;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
            
            // Create logs directory in the application's folder
            var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DxfTool");
            Directory.CreateDirectory(appDataFolder);
            
            _logFilePath = Path.Combine(appDataFolder, "DxfTool.log");
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public string GetLogFilePath()
        {
            return _logFilePath;
        }

        public void OpenLogFile()
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _logFilePath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open log file");
            }
        }
    }
}
