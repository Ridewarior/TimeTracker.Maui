using TimeTracker.Maui.Models;
using TimeTracker.Maui.Services;

namespace TimeTracker.Maui;

public partial class App : Application
{
	// TODO eventually create an interface for the DataServices to create less coupling
	public static SQLiteDataService DataService { get; private set; }
	public App(SQLiteDataService dataService)
	{
		InitializeComponent();

		MainPage = new AppShell();
        DataService = dataService;
    }
}
