using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        TimeRecordId = Convert.ToInt32(HttpUtility.UrlDecode(query[nameof(TimeRecordId)].ToString()));
        TimeRecord = App.DataService.GetTimeRecord(TimeRecordId);
    }

    public DetailsPageViewModel()
    {
        PageTitle = $"Record Details: {RecordTitle}";
    }
}