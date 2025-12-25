using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcurementManager.UI.ViewModels
{
    public abstract partial class ValidationViewModelBase : ObservableValidator, IDialogViewModel
    {
        [ObservableProperty]
        private string _title = string.Empty;
    }
}
