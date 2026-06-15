using FieldOps.Mobile.Pages;

namespace FieldOps.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(WorkOrderDetailPage), typeof(WorkOrderDetailPage));
    }
}
