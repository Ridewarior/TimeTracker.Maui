﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTracker.Maui.Models;
using TimeTracker.Maui.Views;

namespace TimeTracker.Maui.ViewModels;

public partial class DashBoardViewModel : BaseViewModel
{
    private readonly Shell _currentShell = Shell.Current;
    public ObservableCollection<TimeRecord> TimeRecords { get; private set; } = new();

    [ObservableProperty]
    private int _recordId;
    
    [ObservableProperty]
    private string _recordTitle;

    [ObservableProperty]
    private string _timeElapsed;

    public DashBoardViewModel()
    {
        PageTitle = "DashBoard";
        GetTimeRecords().Wait();
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
    
    [RelayCommand]
    public async Task CreateRecord()
    {
        var currentTime = DateTime.Now;
        var record = new TimeRecord
        {
            StartTime = currentTime.ToString(CultureInfo.InvariantCulture),
            StopTime = currentTime.ToString(CultureInfo.InvariantCulture)
        };

        var newRecordId = App.DataService.AddRecord(record);

        await GoToRecordDetails(newRecordId);
    }

    [RelayCommand]
    public async Task DeleteRecord(int id)
    {
        if (id == 0)
        {
            await _currentShell.DisplayAlert("Invalid Record", "Please try again", "OK");
            return;
        }

        var result = App.DataService.DeleteRecord(id);
        if (result == 0)
        {
            await _currentShell.DisplayAlert("Invalid Data", "Please insert valid data", "OK0");
        }
        else
        {
            await _currentShell.DisplayAlert("Delete Successful", "Record removed successfully", "OK");
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

        await _currentShell.GoToAsync($"{nameof(RecordDetailsPage)}?RecordId={id}", true);
    }
    
    #endregion
}