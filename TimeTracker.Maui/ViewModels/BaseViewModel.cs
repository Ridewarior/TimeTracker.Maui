using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Interfaces;
using Mopups.Services;
using System.Globalization;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public readonly string NewRecordId = Guid.Empty.ToString();

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

    public static TimeRecord RunningRecord { get; set; } = new();

    public bool IsLoaded => !IsLoading;

    public static bool TimerRunning => App.TimerService.Running;

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
        if (!TimerRunning)
        {
            return false;
        }

        App.TimerService.StopTimer();
        RunningRecord.TIMERECORD_ID = Guid.NewGuid().ToString();
        RunningRecord.STOP_TIMESTAMP = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        RunningRecord.TIME_ELAPSED = TimeElapsed;
        RunningRecord.REC_TIMER_RUNNING = false;
        var result = App.DataService.AddRecord(RunningRecord);

        return result != 0;
    }

    public static void ResetRunningRecord()
    {
        RunningRecord = new TimeRecord();
    }

    public static string BuildResumedRecordTitle()
    {
        var originalRecordTitle = App.DataService.GetParentRecordTitle(RunningRecord.PARENT_RECORD_ID);
        var resumedRecordCount = App.DataService.GetResumedRecordCount(RunningRecord.PARENT_RECORD_ID);
        // always +2 the value returned since the original record will be uncounted
        var newRecordTitle = $"({resumedRecordCount + 2}) " + originalRecordTitle;
        return newRecordTitle;
    }
}