using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using TimeTracker.Maui.Services;
using TimeTracker.Maui.ViewModels;
using TimeTracker.Maui.Pages;

namespace TimeTracker.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
		// Regular DbPath
		//var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TimeTrackerApp.db3");

		// Temp DbPath
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TimeTrackerApp.db3");

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureMopups()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, dbPath));
		builder.Services.AddSingleton<TimerService>();

        builder.Services.AddSingleton<DashBoardViewModel>();
        builder.Services.AddTransient<DetailsPageViewModel>();

        builder.Services.AddSingleton<DashBoardPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
