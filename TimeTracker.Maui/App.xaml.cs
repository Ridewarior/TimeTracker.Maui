using MetroLog.Maui;
using Microsoft.Maui.Handlers;
using TimeTracker.Maui.Services;

namespace TimeTracker.Maui;

public partial class App
{
    private const int WindowWidth = 1250;
    private const int WindowHeight = 850;

    public static IServiceProvider Services;
    public static SQLiteDataService DataService; // will use interface soon
    public static IAlertService AlertService;
    public static TimerService TimerService;

    // Eventually this should be read in from preferences
    public static string TimeFormat { get; private set; }
	public App(IServiceProvider provider)
	{
		InitializeComponent();

        Services = provider;
        DataService = Services.GetService<SQLiteDataService>();
        TimerService = Services.GetService<TimerService>();
        AlertService = Services.GetService<IAlertService>();
		TimeFormat = @"hh\:mm\:ss"; // should be moved to the SettingsService

        MainPage = new AppShell();
        LogController.InitializeNavigation(
            page => MainPage!.Navigation.PushModalAsync(page),
            () => MainPage!.Navigation.PopModalAsync());
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Width = WindowWidth;
        window.MinimumWidth = WindowWidth;
        window.Height = WindowHeight;
        window.MinimumHeight = WindowHeight;

        return window;
    }

#endif
}
