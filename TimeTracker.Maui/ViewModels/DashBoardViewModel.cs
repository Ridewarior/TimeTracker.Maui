using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Events;
using TimeTracker.Maui.Models;
using TimeTracker.Maui.Pages;

namespace TimeTracker.Maui.ViewModels;

public partial class DashBoardViewModel : BaseViewModel
{
    private const int NewRecId = -1;

    private const string StartTimerText = "Start Timer";

    private const string StopTimerText = "Stop Timer";

    private const int TextMaxLength = 25;

    [ObservableProperty] 
    private int _recordId;

    [ObservableProperty] 
    private string _startStopButtonText;

    public ObservableCollection<TimeRecord> TimeRecords { get; private set; } = new();

    public DashBoardViewModel()
    {
        PageTitle = "DashBoard";
        StartStopButtonText = App.TimerService.Running ? StopTimerText : StartTimerText;

        MopupInstance.Popped += OnPopupPopped;
        GetTimeRecords().Wait();
    }

    #region Private Methods

    private void TruncateLongText(TimeRecord record)
    {
        if (record == null)
        {
            return;
        }

        var recordTitle = record.RECORD_TITLE;
        var workItemTitle = record.WORKITEM_TITLE;
        var clientName = record.CLIENT_NAME;

        if (recordTitle?.Length > TextMaxLength)
        {
            record.RECORD_TITLE = recordTitle[..TextMaxLength].TrimEnd() + "...";
        }

        if (workItemTitle?.Length > TextMaxLength)
        {
            record.WORKITEM_TITLE = workItemTitle[..TextMaxLength].TrimEnd() + "...";
        }

        if (clientName?.Length > TextMaxLength)
        {
            record.CLIENT_NAME = clientName[..TextMaxLength].TrimEnd() + "...";
        }

    }

    private void OnPopupPopped(object sender, PopupNavigationEventArgs e)
    {
        GetTimeRecords().Wait();
    }

    #endregion

    #region Page Commands

    [RelayCommand]
    public async Task GetTimeRecords()
    {
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            if (TimeRecords.Any())
            {
                TimeRecords.Clear();
            }

            // Eventually this should use a different DataService method better suited for the DashBoard so we don't pull the entire object.
            var timeRecords = App.DataService.GetTimeRecords();
            foreach (var record in timeRecords)
            {
                TruncateLongText(record);
                TimeRecords.Add(record);
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get list of Time Records: \n {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Failed to get list of Time Records", "OK");
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task StartNewTimeRecord()
    {
        await GoToRecordDetails(NewRecId);
    }

    [RelayCommand]
    public async Task DeleteRecord(int id)
    {
        if (id == 0)
        {
            await CurShell.DisplayAlert("Invalid Record", "Please try again", "OK");
            return;
        }

        var result = App.DataService.DeleteRecord(id);
        if (result == 0)
        {
            await CurShell.DisplayAlert("Invalid Data", "Please insert valid data", "OK0");
        }
        else
        {
            await CurShell.DisplayAlert("Delete Successful", "Record removed successfully", "OK");
            await GetTimeRecords();
        }
    }

    [RelayCommand]
    public async Task GoToRecordDetails(int id)
    {
        if (id == 0)
        {
            return;
        }

        //await CurShell.GoToAsync($"{nameof(RecordDetailsPage)}?TimeRecordId={id}", true);
        await MopupInstance.PushAsync(new DetailsPopupPage(new DetailsPageViewModel(id)));
    }

    #endregion
}