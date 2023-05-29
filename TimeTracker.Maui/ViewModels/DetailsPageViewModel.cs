using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;


public partial class DetailsPageViewModel : BaseViewModel, IQueryAttributable
{
    [ObservableProperty]
    private TimeRecord _timeRecord;

    [ObservableProperty]
    private int _recordId;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        RecordId = Convert.ToInt32(HttpUtility.UrlDecode(query[nameof(RecordId)].ToString()));
        TimeRecord = App.DataService.GetTimeRecord(RecordId);
    }
}