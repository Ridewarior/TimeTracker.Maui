using TimeTracker.Maui.Services;

namespace TimeTracker.Maui;

public partial class App : Application
{
    private const int WindowWidth = 1250;

    private const int WindowHeight = 850;

	// Eventually create an interface for the DataServices to create less coupling
	public static SQLiteDataService DataService { get; private set; }
	
    public static TimerService TimerService { get; private set; }
    
    // Eventually this should be read in from preferences
    public static string TimeFormat { get; private set; }
	public App(SQLiteDataService dataService, TimerService timerService)
	{
		InitializeComponent();

        MainPage = new AppShell();
        DataService = dataService;
		TimerService = timerService;
		TimeFormat = @"hh\:mm\:ss";
	}

#if WINDOWS
    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Width = WindowWidth;
        window.Height = WindowHeight;
        window.X = 600;
        window.Y = 350;

        return window;
    }

#endif
}
