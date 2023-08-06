using Microsoft.Extensions.Logging;
using TimeTracker.Maui.Enums;

namespace TimeTracker.Maui.Services;

public class SettingsService
{
    private readonly ILogger<SettingsService> _logger;

    private readonly Dictionary<string, object> _defaultSettings;

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;
        _defaultSettings = new Dictionary<string, object>
        {
            {AppSettings.AppTheme.ToString(), 1}, // 1 = Auto, 2 = Light, 3 = Dark
            {AppSettings.ColorTheme.ToString(), 1}, // See ColorTheme Enum
            {AppSettings.TruncateText.ToString(), true}, // Bool
            {AppSettings.MajorTextLimit.ToString(), 30},
            {AppSettings.MinorTextLimit.ToString(), 22},
            {AppSettings.DateFormat.ToString(), 1},
            {AppSettings.ElapsedTimeFormat.ToString(), 2} // 1 = Standard, 2 = Decimal
            //{AppSettings.RecordGrouping.ToString(), 2}, // 1 = Day, 2 = Week
            //{AppSettings.FirstDayOfWeek.ToString(), "Sunday"}, // 1 = Sunday
            //{AppSettings.StorageFormat.ToString(), 1} // 1 = SQLite, 2 = JSON
        };

        StartupCheckLoadSettings(); // should be run at app start to load up our settings.
    }

    private void StartupCheckLoadSettings()
    {
        // Ensure all default settings are set
        foreach (var setting in _defaultSettings.Where(x => !Preferences.Default.ContainsKey(x.Key)))
        {
            // log which setting needed loaded in
            Preferences.Default.Set(setting.Key, setting.Value);
        }

        // logic for starting up based on the assigned settings
    }

    public Task GetSetting(AppSettings setting)
    {
        var settingKey = setting.ToString();

        if (!_defaultSettings.TryGetValue(settingKey, out var defaultValue))
        {
            // log error
            return Task.FromCanceled(CancellationToken.None);
        }
        
        var result = Preferences.Default.Get(settingKey, defaultValue);
        return Task.FromResult(result);
    }

    public Task SaveSetting<T>(AppSettings setting, T value)
    {
        var settingKey = setting.ToString();
        var keyFound = _defaultSettings.TryGetValue(settingKey, out var defaultValue);

        if (!keyFound || defaultValue?.GetType() != value.GetType())
        {
            // log error
            return Task.FromCanceled(CancellationToken.None);
        }

        Preferences.Default.Set(settingKey, value);
        return Task.CompletedTask;
    }
}