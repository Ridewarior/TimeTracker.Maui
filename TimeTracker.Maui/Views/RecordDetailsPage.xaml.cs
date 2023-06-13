using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Views;

public partial class RecordDetailsPage : ContentPage
{
    public RecordDetailsPage(DetailsPageViewModel detailsPageViewModel)
    {
        InitializeComponent();
        BindingContext = detailsPageViewModel;
    }

    private void CbStopTime_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        switch (e.Value)
        {
            case true when CbStopTime.IsEnabled:
                BtnStart.Text = "Create Record";
                Shell.Current.DisplayAlert("Setting a Stop Time", "Setting the stop time creates a new record using the time between those dates", "OK");
                break;
            case false:
                BtnStart.Text = "Start Timer";
                break;
        }
    }
}