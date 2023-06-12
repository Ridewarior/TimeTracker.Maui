using CommunityToolkit.Mvvm.ComponentModel;

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
    private DateTime _startTime;
    
    [ObservableProperty]
    private string _timeElapsed;

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