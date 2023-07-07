using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Pages;

public partial class DetailsPopupPage
{
	public DetailsPopupPage(DetailsPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}