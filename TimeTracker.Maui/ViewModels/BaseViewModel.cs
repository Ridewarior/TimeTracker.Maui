using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Interfaces;
using Mopups.Services;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public const int NewRecordId = 0;

    [ObservableProperty]
    private string _pageTitle;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsLoaded))]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private DateTime _startTimeStamp;

    [ObservableProperty]
    private DateTime _stopTimeStamp;
    
    [ObservableProperty]
    private string _timeElapsed;

    public static TimeRecord RunningRecord { get; set; }

    public bool IsLoaded => !IsLoading;

    public bool TimerRunning => App.TimerService.Running;

    protected readonly Shell CurShell = Shell.Current;

    public IPopupNavigation MopupInstance = MopupService.Instance;

    public BaseViewModel()
    {
        TimeElapsed = "00:00:00";
        App.TimerService.TimerElapsed += TimerUpdated;
    }

    private void TimerUpdated(object sender, EventArgs e)
    {
        TimeElapsed = App.TimerService.ElapsedTime.ToString(App.TimeFormat);
    }

    public bool StopAndSave()
    {
        App.TimerService.StopTimer();
        RunningRecord.STOP_TIMESTAMP = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        RunningRecord.TIME_ELAPSED = TimeElapsed;
        var result = App.DataService.AddRecord(RunningRecord);

        return result != 0;
    }

    public bool StopAndSave(TimeRecord record)
    {
        App.TimerService.StopTimer();
        RunningRecord.RECORD_TITLE = record.RECORD_TITLE;
        RunningRecord.CLIENT_NAME = record.CLIENT_NAME;
        RunningRecord.WORKITEM_TITLE = record.WORKITEM_TITLE;
        RunningRecord.LOG_ID = record.LOG_ID;
        RunningRecord.STOP_TIMESTAMP = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        RunningRecord.TIME_ELAPSED = TimeElapsed;
        var result = App.DataService.AddRecord(RunningRecord);

        return result != 0;

    }

    public static void ResetRunningRecord()
    {
        RunningRecord = null;
    }
}