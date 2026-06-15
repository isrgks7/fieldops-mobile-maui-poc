using FieldOps.Mobile.ViewModels;

namespace FieldOps.Mobile.Pages;

public partial class WorkOrderDetailPage : ContentPage
{
    private readonly WorkOrderDetailViewModel _viewModel;

    public WorkOrderDetailPage(WorkOrderDetailViewModel viewModel)
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
