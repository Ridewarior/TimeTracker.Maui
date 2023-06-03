using System.Text;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

[QueryProperty(nameof(TimeRecordId), nameof(TimeRecordId))]
public partial class DetailsPageViewModel : BaseViewModel, IQueryAttributable
{
    [ObservableProperty]
    private TimeRecord _timeRecord;

    [ObservableProperty]
    private int _timeRecordId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageTitle))]
    private string _recordTitle;

    [ObservableProperty]
    private DateTime _startTime;

    [ObservableProperty]
    private DateTime _stopTime;

    [ObservableProperty]
    private TimeSpan _timeElapsed;

    [ObservableProperty]
    private string _workItemTitle;

    [ObservableProperty]
    private string _clientName;

    [ObservableProperty] 
    private string _logId;

    [ObservableProperty]
    private int _parentRecordId;

    [ObservableProperty] 
    private bool _isNewRecord;

    [ObservableProperty]
    private bool _presetStopTime;

    public DetailsPageViewModel()
    {
        // TODO this needs fixed. It's supposed to update the page title as we type it out in the form.
        PageTitle = $"Record Details: {RecordTitle}";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        TimeRecordId = Convert.ToInt32(HttpUtility.UrlDecode(query[nameof(TimeRecordId)].ToString()));
        if (TimeRecordId > 0)
        {
            TimeRecord = App.DataService.GetTimeRecord(TimeRecordId);
        }
        else
        {
            IsNewRecord = true;
            var currentDateTime = DateTime.Now;
            StartTime = currentDateTime;
            StopTime = currentDateTime;
        }
    }

    [RelayCommand]
    public async Task StartTimer()
    {
        // Start the timer here. We won't actually commit the record to the data source until the timer has stopped.
        await CurShell.Navigation.PopAsync(true);
    }
}