using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Pages;

public partial class DashBoardPage
{
	public DashBoardPage(DashBoardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}