using System.Globalization;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

[QueryProperty(nameof(TimeRecordId), nameof(TimeRecordId))]
public partial class DetailsPageViewModel : BaseViewModel, IQueryAttributable
{
    private const string ExistingRecordStartText = "Continue Timer";
    private const string NewRecordStartText = "Start Timer";

    [ObservableProperty]
    private int _timeRecordId;

    [ObservableProperty]
    private TimeSpan _startTime;

    [ObservableProperty]
    private TimeSpan _stopTime;

    [ObservableProperty]
    private string _workItemTitle;

    [ObservableProperty]
    private string _clientName;

    [ObservableProperty] 
    private string _logId;

    [ObservableProperty]
    private bool _stopDateTimeChecked;

    [ObservableProperty] 
    private bool _stopDateTimeEnabled;

    [ObservableProperty]
    private string _startBtnText;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        TimeRecordId = Convert.ToInt32(HttpUtility.UrlDecode(query[nameof(TimeRecordId)].ToString()));

        PageLoad();
    }

    private void PageLoad()
    {
        if (TimeRecordId > 0)
        {
            DraftRecord = App.DataService.GetTimeRecord(TimeRecordId);
            RecordTitle = DraftRecord.RECORD_TITLE;
            WorkItemTitle = DraftRecord.WORKITEM_TITLE;
            ClientName = DraftRecord.CLIENT_NAME;
            LogId = DraftRecord.LOG_ID;

            if (DateTime.TryParse(DraftRecord.START_TIMESTAMP, out var strTime) && DateTime.TryParse(DraftRecord.STOP_TIMESTAMP, out var stpTime))
            {
                StartTime = new TimeSpan(strTime.Ticks);
                StartTimeStamp = strTime;

                StopTime = new TimeSpan(stpTime.Ticks);
                StopTimeStamp = stpTime;
            }

            PageTitle = $"Record Details: {RecordTitle}";
            StopDateTimeChecked = true;
            StopDateTimeEnabled = false;
        }
        else
        {
            PageTitle = "Starting New Record";
            StopDateTimeEnabled = true;
            var currentTime = DateTime.Now;
            StartTime = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);
            StartTimeStamp = currentTime;

            StopTime = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);
            StopTimeStamp = currentTime;
        }

        StartBtnText = (StopDateTimeChecked && !StopDateTimeEnabled) ? ExistingRecordStartText : NewRecordStartText;
    }

    private bool CreatePreSetRecord()
    {
        TimeElapsed = App.TimerService.GetPresetTime(StartTimeStamp, StopTimeStamp).ToString();

        var result = App.DataService.AddRecord(new TimeRecord
        {
            RECORD_TITLE = RecordTitle,
            START_TIMESTAMP = StartTimeStamp.ToString(CultureInfo.InvariantCulture),
            STOP_TIMESTAMP = StopTimeStamp.ToString(CultureInfo.InvariantCulture),
            TIME_ELAPSED = TimeElapsed.ToString(CultureInfo.InvariantCulture),
            WORKITEM_TITLE = WorkItemTitle,
            CLIENT_NAME = ClientName,
            LOG_ID = LogId
        });

        if (result > 0)
        {
            return true;
        }

        return false;
    }

    [RelayCommand]
    public async Task StartEvents()
    {
        // Need a few checks here.
        // 1. If this is a brand new record and StopDateTimeEnabled flag is not set we can just start the timer and exit the page
        if (TimeRecordId < 0 && !StopDateTimeChecked)
        {
            App.TimerService.StartTimer();
            await CurShell.Navigation.PopAsync();
        }
        // 2. If this is a brand new record and we have the StopDateTimeEnabled flag set then we need to run some basic validations on the dates then call GetPresetTime method
        if (TimeRecordId < 0 && StopDateTimeChecked)
        {
            if (StopTimeStamp <= StartTimeStamp)
            {
                await CurShell.DisplayAlert("Error", "The Stop time cannot be at or before the Start time.", "Cancel");
            }
            else if (CreatePreSetRecord())
            {
                await CurShell.Navigation.PopAsync();
            }
            else
            {
                await CurShell.DisplayAlert("Error", "An error occurred while creating the Time Record, Please try again", "Cancel");
            }
        }
        // 3. If This is an existing record we need to display the option to start the timer again.
    }
}