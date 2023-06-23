using Microsoft.Extensions.Logging;
using TimeTracker.Maui.Services;
using TimeTracker.Maui.ViewModels;
using TimeTracker.Maui.Views;

namespace TimeTracker.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
		// Regular DbPath
		//var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TimeTrackerApp.db3");

		// Temp DbPaths
        const string winDbPath = "C:\\Users\\Big Boss\\source\\repos\\TimeTracker.Maui\\TimeTrackerApp.db3";
        const string iosMacDbPath = "/Users/antoniomagallon/Desktop/repos/TimeTracker.Maui/TimeTrackerApp.db3";

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if IOS
        builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, iosMacDbPath));
#elif MACCATALYST
        builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, iosMacDbPath));
#else
        builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, winDbPath));
#endif
        //builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, dbPath));
        builder.Services.AddSingleton<TimerService>();

        builder.Services.AddSingleton<DashBoardViewModel>();
        builder.Services.AddTransient<DetailsPageViewModel>();

        builder.Services.AddSingleton<DashBoardPage>();
        builder.Services.AddTransient<RecordDetailsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
