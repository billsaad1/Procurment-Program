using System.Threading.Tasks;
using ProcurementManager.UI.ViewModels;

namespace ProcurementManager.UI.Services
{
    public interface IDialogService
    {
        Task<TResult> ShowDialogAsync<TResult>(DialogViewModelBase<TResult> viewModel);
    }
}
