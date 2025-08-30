using System.Threading.Tasks;

namespace DxfTool.Services
{
    public interface IUpdateService
    {
        Task<bool> CheckForUpdatesAsync();
        Task DownloadAndInstallUpdateAsync();
        Task<string> GetCurrentVersionAsync();
        Task<string> GetLatestVersionAsync();
        bool IsUpdateAvailable { get; }
    }
}
