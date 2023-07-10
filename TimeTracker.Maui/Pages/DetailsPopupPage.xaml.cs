using System.ComponentModel;
using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Pages;

public partial class DetailsPopupPage
{
    private readonly DetailsPageViewModel _viewModel;

	public DetailsPopupPage(DetailsPageViewModel viewModel)
	{
        _viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;

        if (viewModel.IsNewRec)
        {
            LblElapsedTime.SetBinding(Label.TextProperty, nameof(viewModel.TimeElapsed));
        }
    }

    private async void StartingTime_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(TpStartTime.Time) or nameof(DpStartDate.Date)))
        {
            return;
        }

        await _viewModel.AdjustStartTime();
    }
}