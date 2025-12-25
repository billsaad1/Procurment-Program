using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcurementManager.UI.ViewModels
{
    public class ViewModelBase : ObservableObject, IDialogViewModel
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
