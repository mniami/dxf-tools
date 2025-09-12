using DxfTool.Models;
using System.Threading.Tasks;

namespace DxfTool.Services
{
    public interface ISettingsService
    {
        /// <summary>
        /// Loads user preferences from storage
        /// </summary>
        /// <returns>The loaded user preferences, or default values if none exist</returns>
        Task<UserPreferences> LoadPreferencesAsync();

        /// <summary>
        /// Saves user preferences to storage
        /// </summary>
        /// <param name="preferences">The preferences to save</param>
        Task SavePreferencesAsync(UserPreferences preferences);

        /// <summary>
        /// Clears all saved preferences
        /// </summary>
        Task ClearPreferencesAsync();

        /// <summary>
        /// Checks if preferences exist
        /// </summary>
        /// <returns>True if preferences exist, false otherwise</returns>
        Task<bool> PreferencesExistAsync();
    }
}
