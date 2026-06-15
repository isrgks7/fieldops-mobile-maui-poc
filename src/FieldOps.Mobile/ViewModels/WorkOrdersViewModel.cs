using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldOps.Application.DTOs;
using FieldOps.Application.UseCases;
using FieldOps.Domain.Enums;
using FieldOps.Mobile.Services;

namespace FieldOps.Mobile.ViewModels;

public partial class WorkOrdersViewModel : BaseViewModel
{
    private readonly GetWorkOrdersUseCase _getWorkOrdersUseCase;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private WorkOrderListFilter _selectedFilter = WorkOrderListFilter.All;

    [ObservableProperty]
    private IReadOnlyList<WorkOrderDto> _workOrders = Array.Empty<WorkOrderDto>();

    public WorkOrdersViewModel(
        AppBootstrapper bootstrapper,
        GetWorkOrdersUseCase getWorkOrdersUseCase)
        : base(bootstrapper)
    {
        _getWorkOrdersUseCase = getWorkOrdersUseCase;
        Title = "Work Orders";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            await EnsureInitializedAsync();

            var query = new WorkOrderQuery
            {
                Filter = SelectedFilter,
                SearchText = SearchText
            };

            var result = await _getWorkOrdersUseCase.ExecuteAsync(query);
            if (!result.IsSuccess || result.Value is null)
            {
                SetError(result.Error ?? "Unable to load work orders.");
                return;
            }

            WorkOrders = result.Value;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ApplyFilterAsync(WorkOrderListFilter filter)
    {
        SelectedFilter = filter;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task OpenWorkOrderAsync(WorkOrderDto workOrder)
    {
        if (workOrder is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(Pages.WorkOrderDetailPage)}?id={workOrder.Id}");
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = LoadAsync();
    }
}
