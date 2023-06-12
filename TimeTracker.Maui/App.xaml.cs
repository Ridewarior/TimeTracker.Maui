using TimeTracker.Maui.Services;

namespace TimeTracker.Maui;

public partial class App : Application
{
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
}
