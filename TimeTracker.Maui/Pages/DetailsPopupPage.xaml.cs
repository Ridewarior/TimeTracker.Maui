using System.ComponentModel;
using TimeTracker.Maui.Enums;
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

        if (_viewModel.IsNewRec)
        {
            LblElapsedTime.SetBinding(Label.TextProperty, nameof(_viewModel.TimeElapsed));
        }
    }

    private async void Switch_OnToggled(object sender, ToggledEventArgs e)
    {
        await _viewModel.EnableDisableStopDate();
    }

    private async void DateTime_OnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is not nameof(TimePicker.Time) or nameof(DatePicker.Date))
        {
            return;
        }

        var controlName = ((Element)sender).StyleId;

        switch (controlName)
        {
            case nameof(TpStartTime) or nameof(DpStartDate):
            {
                await _viewModel.FieldsModified(UpdateableControls.StartingTime);
                break;
            }
            case nameof(TpStopTime) or nameof(DpStopDate):
            {
                await _viewModel.FieldsModified(UpdateableControls.StoppingTime);
                break;
            }
        }
    }

    private async void CheckEntries_OnUnfocused(object sender, FocusEventArgs args)
    {
        var controlName = ((InputView)sender).StyleId;

        switch (controlName)
        {
            case nameof(EntRecTitle):
            {
                await _viewModel.FieldsModified(UpdateableControls.RecTitle);
                break;
            }
            case nameof(EntWiTitle):
            {
                await _viewModel.FieldsModified(UpdateableControls.WorkItemTitle);
                break;
            }
            case nameof(EntClientName):
            {
                await _viewModel.FieldsModified(UpdateableControls.ClientName);
                break;
            }
            case nameof(EntLogId):
            {
                await _viewModel.FieldsModified(UpdateableControls.LogId);
                break;
            }
        }
    }
}