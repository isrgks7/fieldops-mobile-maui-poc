using FieldOps.Application.Abstractions;
using FieldOps.Domain.Entities;
using FieldOps.Domain.Enums;

namespace FieldOps.Infrastructure.SeedData;

public sealed class WorkOrderSeedDataProvider : ISeedDataProvider
{
    public IReadOnlyList<WorkOrder> GetSeedWorkOrders()
    {
        var today = DateTimeOffset.UtcNow;

        return new List<WorkOrder>
        {
            WorkOrder.CreateNew(
                "WO-1001",
                "Apex Manufacturing",
                "1200 Industrial Blvd, Dallas, TX",
                "HVAC unit not cooling",
                "Warehouse zone B reports intermittent cooling. Inspect compressor and refrigerant levels.",
                WorkOrderPriority.High,
                WorkOrderStatus.Open,
                today.AddDays(1)),

            WorkOrder.CreateNew(
                "WO-1002",
                "Sunrise Retail Group",
                "455 Market Street, Austin, TX",
                "Replace damaged exit signage",
                "Front entrance exit sign is cracked and non-compliant. Replace with illuminated unit.",
                WorkOrderPriority.Medium,
                WorkOrderStatus.InProgress,
                today.AddDays(2),
                "Signage ordered. Will install on arrival."),

            WorkOrder.CreateNew(
                "WO-1003",
                "BlueRiver Logistics",
                "88 Harbor Way, Houston, TX",
                "Critical dock door malfunction",
                "Dock door #3 stuck halfway. Safety sensor may be misaligned. Immediate attention required.",
                WorkOrderPriority.Critical,
                WorkOrderStatus.Open,
                today.AddHours(6)),

            WorkOrder.CreateNew(
                "WO-1004",
                "Greenfield Apartments",
                "742 Elm Street, San Antonio, TX",
                "Water heater inspection",
                "Annual inspection for building C water heaters. Check pressure relief valves.",
                WorkOrderPriority.Low,
                WorkOrderStatus.Open,
                today.AddDays(5)),

            WorkOrder.CreateNew(
                "WO-1005",
                "Metro Health Clinic",
                "210 Wellness Drive, Fort Worth, TX",
                "Network closet temperature alert",
                "Temperature sensor triggered in network closet. Verify HVAC and fan operation.",
                WorkOrderPriority.High,
                WorkOrderStatus.InProgress,
                today.AddDays(1),
                "Temporary fan deployed. Monitoring temperature."),

            WorkOrder.CreateNew(
                "WO-1006",
                "Pioneer Office Suites",
                "15 Commerce Park, Plano, TX",
                "Lighting retrofit completion",
                "Finalize LED retrofit in conference rooms 2-4 and verify dimmer compatibility.",
                WorkOrderPriority.Medium,
                WorkOrderStatus.Completed,
                today.AddDays(-2),
                "All fixtures replaced and tested. Client signed off."),
        };
    }
}
