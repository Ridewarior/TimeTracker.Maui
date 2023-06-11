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
    private TimeSpan _timeElapsed;

    public bool IsLoaded => !IsLoading;

    protected readonly Shell CurShell = Shell.Current;
}