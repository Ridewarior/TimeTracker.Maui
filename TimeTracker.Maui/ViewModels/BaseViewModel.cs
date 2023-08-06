using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Interfaces;
using Mopups.Services;
using System.Globalization;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public readonly string NewRecordId = Guid.Empty.ToString();

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsLoaded))]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty] 
    private string _timeElapsed;

    [ObservableProperty]
    private string _runCount;

    public static TimeRecord RunningRecord { get; set; } = new();

    public static bool UnsavedChanges { get; set; }

    public bool IsLoaded => !IsLoading;

    public static bool TimerRunning => App.TimerService.Running;

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
        RunningRecord.RECORD_ID = Guid.NewGuid().ToString();
        RunningRecord.STOP_TIMESTAMP = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        RunningRecord.TIME_ELAPSED = TimeElapsed;
        RunningRecord.REC_TIMER_RUNNING = false;
        var result = App.DataService.AddRecord(RunningRecord);

        return result != 0;
    }

    public void ResetRunningRecord()
    {
        RunningRecord = new TimeRecord();
        RunCount = string.Empty;
    }

    public void IncrementRunCount()
    {
        var resumedRunCount = App.DataService.GetResumedRunCount(RunningRecord.PARENT_ID) + 1;

        RunningRecord.RUN_COUNT = resumedRunCount;
        RunCount = $"({resumedRunCount})";
    }
}