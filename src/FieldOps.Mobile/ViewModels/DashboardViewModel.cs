using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldOps.Application.DTOs;
using FieldOps.Application.UseCases;
using FieldOps.Mobile.Services;

namespace FieldOps.Mobile.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly GetDashboardSummaryUseCase _getDashboardSummaryUseCase;
    private readonly SyncWorkOrdersUseCase _syncWorkOrdersUseCase;

    [ObservableProperty]
    private int _totalWorkOrders;

    [ObservableProperty]
    private int _openCount;

    [ObservableProperty]
    private int _inProgressCount;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private int _pendingSyncCount;

    [ObservableProperty]
    private string _lastSyncDisplay = "Never synced";

    public DashboardViewModel(
        AppBootstrapper bootstrapper,
        GetDashboardSummaryUseCase getDashboardSummaryUseCase,
        SyncWorkOrdersUseCase syncWorkOrdersUseCase)
        : base(bootstrapper)
    {
        _getDashboardSummaryUseCase = getDashboardSummaryUseCase;
        _syncWorkOrdersUseCase = syncWorkOrdersUseCase;
        Title = "Dashboard";
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

            var result = await _getDashboardSummaryUseCase.ExecuteAsync();
            if (!result.IsSuccess || result.Value is null)
            {
                SetError(result.Error ?? "Unable to load dashboard.");
                return;
            }

            ApplySummary(result.Value);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SyncNowAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            await EnsureInitializedAsync();

            var result = await _syncWorkOrdersUseCase.ExecuteAsync();
            if (!result.IsSuccess)
            {
                SetError(result.Error ?? "Sync failed.");
                return;
            }

            await LoadAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplySummary(DashboardSummaryDto summary)
    {
        TotalWorkOrders = summary.Total;
        OpenCount = summary.Open;
        InProgressCount = summary.InProgress;
        CompletedCount = summary.Completed;
        PendingSyncCount = summary.PendingSync;
        LastSyncDisplay = summary.LastSyncAt?.ToLocalTime().ToString("g") ?? "Never synced";
    }
}
