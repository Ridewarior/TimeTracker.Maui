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

    private bool IsNewRec => TimeRecord.TIMERECORD_ID == 0;

    private DateTime StartingTime => StartTimeStamp.Add(StartTime);

    private DateTime StoppingTime => StopTimeStamp.Add(StopTime);

    public bool EnableStartBtn => !TimerRunning;

    public bool EnableDeleteBtn => TimeRecord.TIMERECORD_ID > 0;

    public DetailsPageViewModel(int recordId)
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
        if (TimeRecord.TIMERECORD_ID > 0)
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
        }
        else if (IsNewRec && TimerRunning)
        {
            TimeRecord = new TimeRecord
            {
                RECORD_TITLE = RunningRecord.RECORD_TITLE,
                WORKITEM_TITLE = RunningRecord.WORKITEM_TITLE,
                CLIENT_NAME = RunningRecord.CLIENT_NAME,
                LOG_ID = RunningRecord.LOG_ID
            };

            if (DateTime.TryParse(RunningRecord.START_TIMESTAMP, out var strTime))
            {
                StartTime = new TimeSpan(strTime.Hour, strTime.Minute, strTime.Second);
                StartTimeStamp = strTime.Date;
            }

            StopDateTimeEnabled = false;
            StopCancelBtnText = StopText;
        }
        else
        {
            StopDateTimeEnabled = true;
            var currentTime = DateTime.Now;
            StartTime = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);
            StartTimeStamp = currentTime.Date;

            StopTime = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);
            StopTimeStamp = currentTime.Date;

            StopCancelBtnText = CancelText;
        }

        StartBtnText = (StopDateTimeChecked && !StopDateTimeEnabled) ? ExistingRecordText : NewRecordText;
        EnableDisableDateTimeTxt = StopDateTimeChecked ? DisableDateTimeText : EnableDateTimeText;
    }

    private bool CreatePreSetRecord()
    {
        TimeElapsed = App.TimerService.GetPresetTime(StartingTime, StoppingTime).ToString();

        var result = App.DataService.AddRecord(new TimeRecord
        {
            RECORD_TITLE = TimeRecord.RECORD_TITLE,
            START_TIMESTAMP = StartTimeStamp.Add(StartTime).ToString(CultureInfo.InvariantCulture),
            STOP_TIMESTAMP = StopTimeStamp.Add(StopTime).ToString(CultureInfo.InvariantCulture),
            TIME_ELAPSED = TimeElapsed.ToString(CultureInfo.InvariantCulture),
            WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
            CLIENT_NAME = TimeRecord.CLIENT_NAME,
            LOG_ID = TimeRecord.LOG_ID
        });

        return result != 0;
    }

    #endregion

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
                break;
        }
    }

    [RelayCommand]
    public async Task StartEvents()
    {
        // 1. If this is a brand new record and StopDateTimeEnabled flag is not set we can just start the timer and exit the page
        if (IsNewRec && !StopDateTimeChecked)
        {
            RunningRecord = new TimeRecord
            {
                RECORD_TITLE = TimeRecord.RECORD_TITLE,
                START_TIMESTAMP = StartTimeStamp.Add(StartTime).ToString(CultureInfo.InvariantCulture),
                WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
                CLIENT_NAME = TimeRecord.CLIENT_NAME,
                LOG_ID = TimeRecord.LOG_ID
            };

            App.TimerService.StartTimer();
            await MopupInstance.PopAsync();
        }
        // 2. If this is a brand new record and we have the StopDateTimeEnabled flag set then we need to run some basic validations on the dates then call GetPresetTime method
        else if (IsNewRec && StopDateTimeChecked)
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
        }
        // 3. If This is an existing record we need to display the option to start the timer again.
        else if (!IsNewRec)
        {
            RunningRecord = new TimeRecord
            {
                RECORD_TITLE = TimeRecord.RECORD_TITLE,
                START_TIMESTAMP = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                WORKITEM_TITLE = TimeRecord.WORKITEM_TITLE,
                CLIENT_NAME = TimeRecord.CLIENT_NAME,
                LOG_ID = TimeRecord.LOG_ID
            };

            App.TimerService.StartTimer();
            await MopupInstance.PopAsync();
        }
    }

    [RelayCommand]
    public async Task CancelOrStop()
    {
        if (TimerRunning && IsNewRec)
        {
            if (StopAndSave(TimeRecord))
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