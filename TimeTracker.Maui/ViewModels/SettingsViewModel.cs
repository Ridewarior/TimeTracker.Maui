using Microsoft.Extensions.Logging;
using TimeTracker.Maui.Enums;

namespace TimeTracker.Maui.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ILogger<SettingsViewModel> _logger;

    // app theme radio btn selection => int
    // color theme selected => int (enum)
    // truncated text switch => bool
    // major and minor text limits => int
    // date format selected => int (enum)
    // duration format selected => (enum)
    // record grouping radio btn selection => int
    // first day of the week => string
    // storage type => int

    public SettingsViewModel(ILogger<SettingsViewModel> logger)
    {
        _logger = logger;
    }
}