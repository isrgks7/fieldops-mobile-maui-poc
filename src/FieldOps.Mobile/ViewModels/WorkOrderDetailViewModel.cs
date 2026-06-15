using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FieldOps.Application.DTOs;
using FieldOps.Application.UseCases;
using FieldOps.Domain.Enums;
using FieldOps.Mobile.Services;

namespace FieldOps.Mobile.ViewModels;

[QueryProperty(nameof(WorkOrderIdString), "id")]
public partial class WorkOrderDetailViewModel : BaseViewModel
{
    private readonly GetWorkOrderByIdUseCase _getWorkOrderByIdUseCase;
    private readonly UpdateWorkOrderStatusUseCase _updateWorkOrderStatusUseCase;
    private readonly UpdateTechnicianNotesUseCase _updateTechnicianNotesUseCase;

    [ObservableProperty]
    private string? _workOrderIdString;

    [ObservableProperty]
    private Guid _workOrderId;

    [ObservableProperty]
    private WorkOrderDto? _workOrder;

    [ObservableProperty]
    private string _technicianNotes = string.Empty;

    [ObservableProperty]
    private bool _isCritical;

    public WorkOrderDetailViewModel(
        AppBootstrapper bootstrapper,
        GetWorkOrderByIdUseCase getWorkOrderByIdUseCase,
        UpdateWorkOrderStatusUseCase updateWorkOrderStatusUseCase,
        UpdateTechnicianNotesUseCase updateTechnicianNotesUseCase)
        : base(bootstrapper)
    {
        _getWorkOrderByIdUseCase = getWorkOrderByIdUseCase;
        _updateWorkOrderStatusUseCase = updateWorkOrderStatusUseCase;
        _updateTechnicianNotesUseCase = updateTechnicianNotesUseCase;
        Title = "Work Order";
    }

    partial void OnWorkOrderIdStringChanged(string? value)
    {
        if (Guid.TryParse(value, out var id))
            WorkOrderId = id;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (WorkOrderId == Guid.Empty)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            await EnsureInitializedAsync();

            var result = await _getWorkOrderByIdUseCase.ExecuteAsync(WorkOrderId);
            if (!result.IsSuccess || result.Value is null)
            {
                SetError(result.Error ?? "Work order not found.");
                return;
            }

            ApplyWorkOrder(result.Value);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task StartWorkAsync()
    {
        await UpdateStatusAsync(WorkOrderStatus.InProgress);
    }

    [RelayCommand]
    private async Task MarkCompletedAsync()
    {
        if (string.IsNullOrWhiteSpace(TechnicianNotes))
        {
            SetError("Technician notes are required before marking as completed.");
            return;
        }

        await EnsureInitializedAsync();

        var notesResult = await _updateTechnicianNotesUseCase.ExecuteAsync(WorkOrderId, TechnicianNotes);
        if (!notesResult.IsSuccess)
        {
            SetError(notesResult.Error ?? "Unable to save notes.");
            return;
        }

        await UpdateStatusAsync(WorkOrderStatus.Completed);
    }

    [RelayCommand]
    private async Task SaveNotesAsync()
    {
        if (WorkOrderId == Guid.Empty)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            await EnsureInitializedAsync();

            var result = await _updateTechnicianNotesUseCase.ExecuteAsync(WorkOrderId, TechnicianNotes);
            if (!result.IsSuccess || result.Value is null)
            {
                SetError(result.Error ?? "Unable to save notes.");
                return;
            }

            ApplyWorkOrder(result.Value);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateStatusAsync(WorkOrderStatus status)
    {
        if (WorkOrderId == Guid.Empty)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            await EnsureInitializedAsync();

            if (status == WorkOrderStatus.Completed && string.IsNullOrWhiteSpace(TechnicianNotes))
            {
                var notesResult = await _updateTechnicianNotesUseCase.ExecuteAsync(WorkOrderId, TechnicianNotes);
                if (!notesResult.IsSuccess)
                {
                    SetError(notesResult.Error ?? "Notes are required before completing.");
                    return;
                }
            }

            var result = await _updateWorkOrderStatusUseCase.ExecuteAsync(WorkOrderId, status);
            if (!result.IsSuccess || result.Value is null)
            {
                SetError(result.Error ?? "Unable to update status.");
                return;
            }

            ApplyWorkOrder(result.Value);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyWorkOrder(WorkOrderDto dto)
    {
        WorkOrder = dto;
        TechnicianNotes = dto.TechnicianNotes;
        IsCritical = dto.Priority == WorkOrderPriority.Critical;
        Title = dto.WorkOrderNumber;
        NotifyActionStates();
    }

    private void NotifyActionStates()
    {
        OnPropertyChanged(nameof(CanStartWork));
        OnPropertyChanged(nameof(CanMarkCompleted));
        OnPropertyChanged(nameof(IsCompleted));
    }

    public bool CanStartWork => WorkOrder?.Status == WorkOrderStatus.Open;
    public bool CanMarkCompleted => WorkOrder?.Status is WorkOrderStatus.Open or WorkOrderStatus.InProgress;
    public bool IsCompleted => WorkOrder?.Status == WorkOrderStatus.Completed;

    partial void OnWorkOrderChanged(WorkOrderDto? value) => NotifyActionStates();
}
