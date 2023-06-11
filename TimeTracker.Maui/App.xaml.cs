using TimeTracker.Maui.Models;
using TimeTracker.Maui.Services;

namespace TimeTracker.Maui;

public partial class App : Application
{
	// TODO eventually create an interface for the DataServices to create less coupling
	public static SQLiteDataService DataService { get; private set; }
    public static TimerService TimerService { get; set; }
	public App(SQLiteDataService dataService, TimerService timerService)
	{
		InitializeComponent();

		MainPage = new AppShell();
        DataService = dataService;
		TimerService = timerService;
    }
}
