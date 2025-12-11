using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using ProcurementManager.UI.ViewModels;
using ProcurementManager.UI.Views;

namespace ProcurementManager.UI.Services
{
    public class DialogService : IDialogService
    {
        private readonly Dictionary<Type, Type> _viewModelViewMappings = new();

        public DialogService()
        {
            RegisterViewModelViewMappings();
        }

        private void RegisterViewModelViewMappings()
        {
            _viewModelViewMappings.Add(typeof(AddSupplierViewModel), typeof(AddSupplierView));
            _viewModelViewMappings.Add(typeof(EditSupplierViewModel), typeof(EditSupplierView));
            _viewModelViewMappings.Add(typeof(AddProductViewModel), typeof(AddProductView));
            _viewModelViewMappings.Add(typeof(EditProductViewModel), typeof(EditProductView));
        }

        public async Task<TResult> ShowDialogAsync<TResult>(DialogViewModelBase<TResult> viewModel)
        {
            var viewType = _viewModelViewMappings[viewModel.GetType()];
            var view = (Window)Activator.CreateInstance(viewType);
            view.DataContext = viewModel;

            var tcs = new TaskCompletionSource<TResult>();

            viewModel.Saved += (result) =>
            {
                tcs.SetResult(result);
                view.Close();
            };

            viewModel.Canceled += () =>
            {
                tcs.SetResult(default);
                view.Close();
            };

            view.Closed += (s, e) =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    tcs.SetResult(default);
                }
            };

            await view.ShowDialog(null);
            return await tcs.Task;
        }
    }
}
