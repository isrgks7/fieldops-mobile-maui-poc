using FieldOps.Application.Abstractions;
using FieldOps.Application.Common;
using FieldOps.Application.DTOs;
using FieldOps.Domain.Enums;

namespace FieldOps.Application.UseCases;

public sealed class GetWorkOrdersUseCase
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetWorkOrdersUseCase(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Result<IReadOnlyList<WorkOrderDto>>> ExecuteAsync(
        WorkOrderQuery query,
        CancellationToken cancellationToken = default)
    {
        var workOrders = await _workOrderRepository.GetAllAsync(cancellationToken);
        var filtered = workOrders
            .Where(w => MatchesFilter(w.Status, query.Filter))
            .Where(w => MatchesSearch(w, query.SearchText))
            .OrderBy(w => w.DueDate)
            .Select(WorkOrderMapper.ToDto)
            .ToList();

        return Result<IReadOnlyList<WorkOrderDto>>.Success(filtered);
    }

    private static bool MatchesFilter(WorkOrderStatus status, WorkOrderListFilter filter)
    {
        return filter switch
        {
            WorkOrderListFilter.All => true,
            WorkOrderListFilter.Open => status == WorkOrderStatus.Open,
            WorkOrderListFilter.InProgress => status == WorkOrderStatus.InProgress,
            WorkOrderListFilter.Completed => status == WorkOrderStatus.Completed,
            _ => true
        };
    }

    private static bool MatchesSearch(Domain.Entities.WorkOrder workOrder, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        var term = searchText.Trim();
        return workOrder.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || workOrder.Title.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}
