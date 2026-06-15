using FieldOps.Mobile.ViewModels;

namespace FieldOps.Mobile.Pages;

public partial class WorkOrdersPage : ContentPage
{
    private readonly WorkOrdersViewModel _viewModel;

    public WorkOrdersPage(WorkOrdersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
