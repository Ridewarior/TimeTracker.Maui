using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Mopups.Events;
using System.Collections.ObjectModel;
using TimeTracker.Maui.Models;
using TimeTracker.Maui.Pages;

namespace TimeTracker.Maui.ViewModels;

public partial class DashBoardViewModel : BaseViewModel
{
    private readonly ILogger<DashBoardViewModel> _logger;

    private const int MinorTextMaxLength = 22;

    private const int MajorTextMaxLength = 30;

    private const string StartTimerText = "Start Timer";

    private const string StopTimerText = "Stop Timer";

    public ObservableCollection<GroupedRecords> TimeRecords { get; } = new();

    [ObservableProperty]
    private TimeRecord _selectedRecord;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _recordTitle;

    [ObservableProperty]
    private string _workItemTitle;

    [ObservableProperty]
    private string _clientName;

    [ObservableProperty]
    private string _logId;

    [ObservableProperty]
    private string _btnMainText;

    [ObservableProperty]
    private Color _backgroundColor;

    [ObservableProperty]
    private bool _showRunCount;

    public DashBoardViewModel(ILogger<DashBoardViewModel> logger)
    {
        PageTitle = "TimeTracker: DashBoard";

        _logger = logger;
        MopupInstance.Popped += OnPopupPopped;
        GetTimeRecords().Wait();
        UpdateControls();
    }

    #region Private Methods

    private void UpdateControls()
    {
        if (TimerRunning && RunningRecord.REC_TIMER_RUNNING)
        {
            BtnMainText = StopTimerText;
            BackgroundColor = Color.FromArgb("#ff8c00");
            IsRunning = true;
            RecordTitle = TruncateLongText(RunningRecord.RECORD_TITLE);
            WorkItemTitle = TruncateLongText(RunningRecord.WORKITEM_TITLE);
            ClientName = TruncateLongText(RunningRecord.CLIENT_NAME);
            LogId = RunningRecord.LOG_ID;

            if (RunningRecord.RUN_COUNT > 1)
            {
                ShowRunCount = true;
            }
        }
        else
        {
            BtnMainText = StartTimerText;
            BackgroundColor = Color.FromArgb("#512BD4");
            IsRunning = false;
        }
    }

    private static void TruncateLongText(TimeRecord record)
    {
        if (record == null)
        {
            return;
        }

        var recordTitle = record.RECORD_TITLE;
        var workItemTitle = record.WORKITEM_TITLE;
        var clientName = record.CLIENT_NAME;

        if (recordTitle?.Length > MajorTextMaxLength)
        {
            record.RECORD_TITLE = recordTitle[..MajorTextMaxLength].TrimEnd() + "...";
        }

        if (workItemTitle?.Length > MinorTextMaxLength)
        {
            record.WORKITEM_TITLE = workItemTitle[..MinorTextMaxLength].TrimEnd() + "...";
        }

        if (clientName?.Length > MinorTextMaxLength)
        {
            record.CLIENT_NAME = clientName[..MinorTextMaxLength].TrimEnd() + "...";
        }
    }

    private static string TruncateLongText(string selectedText)
    {
        if (selectedText?.Length > MinorTextMaxLength)
        {
            return selectedText[..MinorTextMaxLength].TrimEnd() + "...";
        }

        return selectedText;
    }

    private void OnPopupPopped(object sender, PopupNavigationEventArgs e)
    {
        GetTimeRecords().Wait();
        UpdateControls();

        if (TimerRunning && !RunningRecord.REC_TIMER_RUNNING)
        {
            App.TimerService.StopTimer();
        }

        if (RecordModified)
        {
            App.TimerService.ResyncTimers();
        }
    }

    #endregion

    #region Page Commands

    [RelayCommand]
    public Task GetTimeRecords()
    {
        if (IsLoading)
        {
            return Task.CompletedTask;
        }

        try
        {
            IsLoading = true;

            if (TimeRecords.Any())
            {
                TimeRecords.Clear();
            }

            var sourceList = App.DataService.GetTimeRecords();

            if (sourceList == null)
            {
                _logger.LogError("Failed to retrieve the list of Time Records.");
                return Task.CompletedTask;
            }

            foreach (var record in sourceList)
            {
                TruncateLongText(record);
            }

            var orderedDict = sourceList.OrderByDescending(x => DateTime.Parse(x.START_TIMESTAMP))
                .GroupBy(o => DateTime.Parse(o.START_TIMESTAMP).ToString("ddd dd MMM"))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in orderedDict)
            {
                var accumulatedTime = TimeSpan.Zero;
                accumulatedTime = item.Value.Aggregate(accumulatedTime, (current, record) => current.Add(TimeSpan.Parse(record.TIME_ELAPSED)));
                TimeRecords.Add(new GroupedRecords(item.Key, accumulatedTime.ToString(), new List<TimeRecord>(item.Value)));
            }
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task StartNewRecord()
    {
        var newRecord = new TimeRecord
        {
            RECORD_ID = NewRecordId
        };

        await MopupInstance.PushAsync(new DetailsPopupPage(new DetailsPageViewModel(newRecord)));
    }

    [RelayCommand]
    public async Task MainButtonClicked()
    {
        if (!TimerRunning)
        {
            await StartNewRecord();
        }
        else
        {
            await StopTimer();
        }
    }

    // Not ready, currently waiting on context menu options
    //[RelayCommand]
    //public async Task DeleteRecord(string recordId)
    //{
    //    if (string.IsNullOrWhiteSpace(recordId))
    //    {
    //        await CurShell.DisplayAlert("Invalid Record", "Please try again", "OK");
    //        return;
    //    }

    //    var result = App.DataService.DeleteRecord(recordId);
    //    if (result == 0)
    //    {
    //        await CurShell.DisplayAlert("Invalid Data", "Please insert valid data", "OK0");
    //    }
    //    else
    //    {
    //        await CurShell.DisplayAlert("Delete Successful", "Record removed successfully", "OK");
    //        await GetTimeRecords();
    //    }
    //}

    [RelayCommand]
    public async Task GoToRecordDetails(object args)
    {
        if (args is not TimeRecord record)
        {
            return;
        }

        SelectedRecord = null!;

        await MopupInstance.PushAsync(new DetailsPopupPage(new DetailsPageViewModel(record)));
    }

    [RelayCommand]
    public async Task StopTimer()
    {
        if (StopAndSave())
        {
            await CurShell.DisplayAlert("Success", "Record was saved successfully", "OK");
            ResetRunningRecord();
            GetTimeRecords().Wait();
        }
        else
        {
            await CurShell.DisplayAlert("Error", "An error occurred while trying to stop and save this record, please try again", "OK");
        }

        UpdateControls();
    }

    #endregion
}