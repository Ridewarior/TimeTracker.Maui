using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTracker.Maui.Models;

namespace TimeTracker.Maui.ViewModels;

public partial class DashBoardViewModel : BaseViewModel
{
    public ObservableCollection<TimeRecord> TimeRecords { get; private set; } = new();

    [ObservableProperty]
    private string _recordTitle;

    [ObservableProperty]
    private string _timeElapsed;

    public DashBoardViewModel()
    {
        PageTitle = "DashBoard";
    }

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

    #endregion
}