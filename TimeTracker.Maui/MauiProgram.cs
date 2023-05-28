using Microsoft.Extensions.Logging;
using TimeTracker.Maui.Services;
using TimeTracker.Maui.ViewModels;
using TimeTracker.Maui.Views;

namespace TimeTracker.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TimeTrackerApp.db3");
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, dbPath));

        builder.Services.AddSingleton<DashBoardViewModel>();

        builder.Services.AddSingleton<DashBoardPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
