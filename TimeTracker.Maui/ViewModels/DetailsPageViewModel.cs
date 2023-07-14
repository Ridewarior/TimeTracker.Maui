using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

public partial class DetailsPageViewModel : BaseViewModel
{
    private const string ExistingRecordText = "Resume";
    private const string NewRecordText = "Start";
    private const string CreateRecordText = "Create";
    private const string CancelText = "Cancel";
    private const string StopText = "Stop";
    private const string EnableDateTimeText = "Show Stop Date";
    private const string DisableDateTimeText = "Hide Stop Date";

    [ObservableProperty]
    private TimeSpan _startTime;

    [ObservableProperty]
    private TimeSpan _stopTime;

    [ObservableProperty]
    private bool _stopDateTimeChecked;

    [ObservableProperty] 
    private bool _stopDateTimeEnabled;

    [ObservableProperty]
    private string _enableDisableDateTimeTxt;

    [ObservableProperty]
    private string _startBtnText;

    [ObservableProperty]
    private string _stopCancelBtnText;

    [ObservableProperty] 
    private static TimeRecord _timeRecord;

    [ObservableProperty]
    private bool _startTimeEnabled = true;

    private DateTime StartingTime => StartTimeStamp.Add(StartTime);

    private DateTime StoppingTime => StopTimeStamp.Add(StopTime);

    private DateTime _currentTime;

    private DateTime _originalStartTime;

    public bool IsNewRec => TimeRecord.TIMERECORD_ID == Guid.Empty.ToString();

    public bool EnableStartBtn => !TimeRecord.REC_TIMER_RUNNING;

    public DetailsPageViewModel(string recordId)
    {
        TimeRecord = new TimeRecord
        {
            TIMERECORD_ID = recordId
        };

        PageLoad();
    }

    #region Private Methods

    private void PageLoad()
    {
        if (TimerRunning && !RunningRecord.REC_TIMER_RUNNING)
        {
            App.TimerService.StopTimer();
        }

        switch (IsNewRec)
        {
            case false:
            {
                TimeRecord = App.DataService.GetTimeRecord(TimeRecord.TIMERECORD_ID);

                if (DateTime.TryParse(TimeRecord.START_TIMESTAMP, out var strTime) && DateTime.TryParse(TimeRecord.STOP_TIMESTAMP, out var stpTime))
                {
                    StartTime = new TimeSpan(strTime.Hour, strTime.Minute, strTime.Second);
                    StartTimeStamp = strTime.Date;

                    StopTime = new TimeSpan(stpTime.Hour, stpTime.Minute, stpTime.Second);
                    StopTimeStamp = stpTime.Date;
                }

                StopDateTimeChecked = true;
                StopDateTimeEnabled = false;
                StopCancelBtnText = CancelText;
                StartTimeEnabled = false;
                break;
            }
            case true when TimerRunning:
            {
                TimeRecord.RECORD_TITLE = RunningRecord.RECORD_TITLE;
                TimeRecord.WORKITEM_TITLE = RunningRecord.WORKITEM_TITLE;
                TimeRecord.CLIENT_NAME = RunningRecord.CLIENT_NAME;
                TimeRecord.LOG_ID = RunningRecord.LOG_ID;
                TimeRecord.REC_TIMER_RUNNING = RunningRecord.REC_TIMER_RUNNING;

                if (DateTime.TryParse(RunningRecord.START_TIMESTAMP, out var strTime))
                {
                    StartTime = new TimeSpan(strTime.Hour, strTime.Minute, strTime.Second);
                    StartTimeStamp = strTime.Date;
                }

                StopDateTimeEnabled = false;
                StopCancelBtnText = StopText;
                StartTimeEnabled = false;
                break;
            }
            default:
            {
                StopDateTimeEnabled = true;
                _currentTime = DateTime.Now;
                StartTime = new TimeSpan(_currentTime.Hour, _currentTime.Minute, _currentTime.Second);
                StartTimeStamp = _currentTime.Date;

                StopTime = new TimeSpan(_currentTime.Hour, _currentTime.Minute, _currentTime.Second);
                StopTimeStamp = _currentTime.Date;

                _originalStartTime = StartingTime;
                StopCancelBtnText = CancelText;
                App.TimerService.StartTimer(DateTime.Now - StartingTime);
                break;
            }
        }

        StartBtnText = (StopDateTimeChecked && !StopDateTimeEnabled) ? ExistingRecordText : NewRecordText;
        EnableDisableDateTimeTxt = StopDateTimeChecked ? DisableDateTimeText : EnableDateTimeText;
    }

    private bool CreatePreSetRecord()
    {
        App.TimerService.StopTimer();

        TimeElapsed = App.TimerService.GetPresetTime(StartingTime, StoppingTime).ToString();

        var result = App.DataService.AddRecord(new TimeRecord
        {
            TIMERECORD_ID = Guid.NewGuid().ToString(),
            RECORD_TITLE = TimeRecord.RECORD_TITLE,
            START_TIMESTAMP = StartingTime.ToString(CultureInfo.InvariantCulture),
            STOP_TIMESTAMP = StoppingTime.ToString(CultureInfo.InvariantCulture),
            TIME_ELAPSED = TimeElapsed.ToString(CultureInfo.InvariantCulture),
            WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
            CLIENT_NAME = TimeRecord.CLIENT_NAME,
            LOG_ID = TimeRecord.LOG_ID
        });

        return result != 0;
    }

    #endregion

    public async Task AdjustStartTime()
    {
        if (!StopDateTimeChecked)
        {
            if (StartingTime > DateTime.Now)
            {
                await CurShell.DisplayAlert("Error", "Start time cannot be in the future", "OK");
                StartTimeStamp = _originalStartTime.Date;
                StartTime = new TimeSpan(_originalStartTime.Hour, _originalStartTime.Minute, _originalStartTime.Second);
            }

            if (StartingTime != _originalStartTime)
            {
                var pullBack = StartingTime < _originalStartTime;
                _originalStartTime = StartingTime;

                App.TimerService.AdjustTimer(DateTime.Now - StartingTime, pullBack);
            }
        }
    }

    [RelayCommand]
    public async Task EnableDisableStopDate()
    {
        StopDateTimeChecked = !StopDateTimeChecked;

        switch (StopDateTimeChecked)
        {
            case true when StopDateTimeEnabled:
                StartBtnText = CreateRecordText;
                EnableDisableDateTimeTxt = DisableDateTimeText;
                await CurShell.DisplayAlert("Setting a Stop Time", "Setting the stop time creates a new record using the time between those dates", "OK");
                break;
            case false:
                StartBtnText = NewRecordText;
                EnableDisableDateTimeTxt = EnableDateTimeText;
                await AdjustStartTime();
                break;
        }
    }

    [RelayCommand]
    public async Task StartEvents()
    {
        switch (IsNewRec)
        {
            case true when !StopDateTimeChecked:
            {
                if (StartingTime > _currentTime)
                {
                    await CurShell.DisplayAlert("Error", "Cannot start timer ahead of the current time", "OK");
                    return;
                }

                RunningRecord = new TimeRecord
                {
                    RECORD_TITLE = TimeRecord.RECORD_TITLE,
                    START_TIMESTAMP = StartingTime.ToString(CultureInfo.InvariantCulture),
                    WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
                    CLIENT_NAME = TimeRecord.CLIENT_NAME,
                    LOG_ID = TimeRecord.LOG_ID,
                    REC_TIMER_RUNNING = true
                };

                await MopupInstance.PopAsync();
                break;
            }
            case true when StopDateTimeChecked:
            {
                if (StoppingTime <= StartingTime)
                {
                    await CurShell.DisplayAlert("Error", "The Stop time cannot be the same or before the Start time.", "OK");
                }
                else if (CreatePreSetRecord())
                {
                    await MopupInstance.PopAsync();
                }
                else
                {
                    await CurShell.DisplayAlert("Error", "An error occurred while creating the Time Record, Please try again", "OK");
                }

                break;
            }
            case false:
            {
                _currentTime = DateTime.Now;

                if (!string.IsNullOrEmpty(TimeRecord.PARENT_RECORD_ID))
                {
                    RunningRecord = new TimeRecord
                    {
                        START_TIMESTAMP = _currentTime.ToString(CultureInfo.InvariantCulture),
                        WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
                        CLIENT_NAME = TimeRecord.CLIENT_NAME,
                        LOG_ID = TimeRecord.LOG_ID,
                        PARENT_RECORD_ID = TimeRecord.PARENT_RECORD_ID,
                        REC_TIMER_RUNNING = true
                    };
                }
                else
                {
                    RunningRecord = new TimeRecord
                    {
                        START_TIMESTAMP = _currentTime.ToString(CultureInfo.InvariantCulture),
                        WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
                        CLIENT_NAME = TimeRecord.CLIENT_NAME,
                        LOG_ID = TimeRecord.LOG_ID,
                        PARENT_RECORD_ID = TimeRecord.TIMERECORD_ID,
                        REC_TIMER_RUNNING = true
                    };
                }

                RunningRecord.RECORD_TITLE = BuildResumedRecordTitle();

                App.TimerService.StartTimer(DateTime.Now - _currentTime);
                await MopupInstance.PopAsync();
                break;
            }
        }
    }

    [RelayCommand]
    public async Task CancelOrStop()
    {
        if (IsNewRec && TimeRecord.REC_TIMER_RUNNING)
        {
            if (StopAndSave())
            {
                await CurShell.DisplayAlert("Success", "Record was saved successfully", "OK");
                ResetRunningRecord();
                await MopupInstance.PopAsync();
            }
            else
            {
                await CurShell.DisplayAlert("Error", "An error occurred while trying to stop and save this record, please try again", "OK");
            }
        }
        else
        {
            await MopupInstance.PopAsync();
        }
    }

    [RelayCommand]
    public async Task DeleteRecord()
    {
        var confirm = await CurShell.DisplayAlert("Warning", "Are you sure you want to delete this record?", "Delete", "Cancel");
        
        if (confirm)
        {
            var result = App.DataService.DeleteRecord(TimeRecord.TIMERECORD_ID);
            if (result != 0)
            {
                await CurShell.DisplayAlert("Success", "Record was deleted successfully", "OK");
                await MopupInstance.PopAsync();
            }
            else
            {
                await CurShell.DisplayAlert("Error", "An error occurred while trying to delete this record, please try again.", "OK");
            }
        }

    }
}