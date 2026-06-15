using CommunityToolkit.Mvvm.ComponentModel;
using FieldOps.Mobile.Services;

namespace FieldOps.Mobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    private readonly AppBootstrapper _bootstrapper;

    protected BaseViewModel(AppBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }

    protected Task EnsureInitializedAsync() => _bootstrapper.InitializationTask;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string _title = string.Empty;

    protected void SetError(string message) => ErrorMessage = message;

    protected void ClearError() => ErrorMessage = null;
}
