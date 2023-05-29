using TimeTracker.Maui.ViewModels;

namespace TimeTracker.Maui.Views;

public partial class DashBoardPage : ContentPage
{
	public DashBoardPage(DashBoardViewModel dashBoardViewModel)
	{
		InitializeComponent();
		BindingContext = dashBoardViewModel;
	}
}