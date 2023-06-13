using CommunityToolkit.Mvvm.ComponentModel;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string _pageTitle;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsLoaded))]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _recordTitle;

    [ObservableProperty]
    private DateTime _startTimeStamp;

    [ObservableProperty]
    private DateTime _stopTimeStamp;
    
    [ObservableProperty]
    private string _timeElapsed;

    public TimeRecord DraftRecord { get; set; }

    public bool IsLoaded => !IsLoading;

    protected readonly Shell CurShell = Shell.Current;

    public BaseViewModel()
    {
        TimeElapsed = "00:00:00";
        App.TimerService.TimerElapsed += TimerUpdated;
    }

    private void TimerUpdated(object sender, EventArgs e)
    {
        TimeElapsed = App.TimerService.ElapsedTime.ToString(App.TimeFormat);
    }
}