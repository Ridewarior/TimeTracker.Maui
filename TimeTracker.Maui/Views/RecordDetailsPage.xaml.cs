using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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