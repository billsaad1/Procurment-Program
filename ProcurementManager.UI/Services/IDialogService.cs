using System.Threading.Tasks;
using ProcurementManager.UI.ViewModels;

namespace ProcurementManager.UI.Services
{
    public interface IDialogService
    {
        Task<TResult?> ShowDialogAsync<TResult>(IDialogViewModel viewModel) where TResult : class;
        Task<bool> ShowConfirmationDialogAsync(string title, string message);
    }
}
