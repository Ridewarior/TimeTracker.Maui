using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using TimeTracker.Maui.Pages;
using TimeTracker.Maui.Services;
using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TimeTrackerApp.db3");
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureMopups()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, dbPath));
		builder.Services.AddSingleton<TimerService>();

        builder.Services.AddSingleton<BaseViewModel>();
        builder.Services.AddSingleton<DashBoardViewModel>();
        builder.Services.AddTransient<DetailsPageViewModel>();

        builder.Services.AddSingleton<DashBoardPage>();
        builder.Services.AddTransient<DetailsPopupPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
