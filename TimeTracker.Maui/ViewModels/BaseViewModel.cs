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

    public bool IsLoaded => !IsLoading;

    public readonly Shell CurShell = Shell.Current;
}