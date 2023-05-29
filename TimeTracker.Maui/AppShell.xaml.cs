using TimeTracker.Maui.Views;

namespace TimeTracker.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(RecordDetailsPage), typeof(RecordDetailsPage));
	}
}
