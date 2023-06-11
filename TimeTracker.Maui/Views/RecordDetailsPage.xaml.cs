using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Views;

public partial class RecordDetailsPage : ContentPage
{
    public RecordDetailsPage(DetailsPageViewModel detailsPageViewModel)
    {
        InitializeComponent();
        BindingContext = detailsPageViewModel;
    }
}