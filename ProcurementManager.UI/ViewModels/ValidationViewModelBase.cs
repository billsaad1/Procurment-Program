using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProcurementManager.UI.ViewModels
{
    public abstract partial class ValidationViewModelBase : ObservableValidator, IDialogViewModel
    {
        [ObservableProperty]
        private string _title = string.Empty;

        public IRelayCommand SaveCommand { get; set; } = null!;

        public abstract object GetResult();
    }
}
