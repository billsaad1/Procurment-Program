using CommunityToolkit.Mvvm.Input;

namespace ProcurementManager.UI.ViewModels
{
    public interface IDialogViewModel
    {
        string Title { get; set; }
        IRelayCommand SaveCommand { get; }
        object GetResult();
    }
}
