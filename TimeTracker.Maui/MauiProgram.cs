using CommunityToolkit.Maui;
using MetroLog.MicrosoftExtensions;
using MetroLog.Operators;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Mopups.Hosting;
using TimeTracker.Maui.Pages;
using TimeTracker.Maui.Services;
using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMopups()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-brands-400.otf", "FAB");
                fonts.AddFont("fa-regular-400.otf", "FAR");
                fonts.AddFont("fa-solid-900.otf", "FAS");
            });
#if WINDOWS
        builder.ConfigureLifecycleEvents(lifecycle =>
        {
            lifecycle.AddWindows((builder) =>
            {
                builder.OnWindowCreated(del =>
                {
                    del.Title = "TimeTracker";
                });
            });
        });
#endif

        //builder.Services.AddSingleton<ISettingsService, SettingsService>();

        // Once other data storage types are implemented the settings service can be called here to check for which IDataService class should be used. DB path will be different based on that.
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "TimeTrackerApp.db3");
        //builder.Services.AddSingleton<IDataService, SQLiteDataService>(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, dbPath));
        builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<SQLiteDataService>(s, dbPath));

        builder.Services.AddSingleton<IAlertService, AlertService>();
        builder.Services.AddSingleton<TimerService>();

        builder.Services.AddSingleton<BaseViewModel>();
        builder.Services.AddSingleton<DashBoardViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddTransient<DetailsPageViewModel>();

        builder.Services.AddSingleton<DashBoardPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddTransient<DetailsPopupPage>();


        builder.Logging
            .SetMinimumLevel(LogLevel.Trace)
            .AddInMemoryLogger(
                options =>
                {
                    options.MaxLines = 1024;
                    options.MinLevel = LogLevel.Debug;
                    options.MaxLevel = LogLevel.Critical;
                })
            .AddConsoleLogger(
                options =>
                {
                    options.MinLevel = LogLevel.Information;
                    options.MaxLevel = LogLevel.Critical;
                }) // Writes to the console log (logcat for android)
#if DEBUG
            .AddTraceLogger(
                options =>
                {
                    options.MinLevel = LogLevel.Trace;
                    options.MaxLevel = LogLevel.Critical;
                }); // Writes to the debug console
#else
            .AddStreamingFileLogger(
                options =>
                {
                    options.RetainDays = 2;
                    options.FolderPath = Path.Combine(FileSystem.CacheDirectory, "TimeTrackerLogs");
                });
#endif

        builder.Services.AddSingleton(LogOperatorRetriever.Instance);

		return builder.Build();
	}
}
