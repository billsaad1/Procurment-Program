using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ProcurementManager.UI.ViewModels;
using ProcurementManager.UI.Views;

namespace ProcurementManager.UI.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _viewModelViewMap = new()
        {
            { typeof(AddEditSupplierViewModel), typeof(AddEditSupplierView) },
            { typeof(AddEditProductViewModel), typeof(AddEditProductView) },
            { typeof(AddEditPurchaseRequisitionViewModel), typeof(AddEditPurchaseRequisitionView) }
        };

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult?> ShowDialogAsync<TResult>(IDialogViewModel viewModel) where TResult : class
        {
            var viewType = _viewModelViewMap[viewModel.GetType()];
            var view = (Control)_serviceProvider.GetRequiredService(viewType);
            view.DataContext = viewModel;

            var dialogWindow = new Window
            {
                Content = view,
                Title = viewModel.Title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Width = 800,
                Height = 600,
                CanResize = true
            };

            var tcs = new TaskCompletionSource<TResult?>();

            var originalSaveCommand = viewModel.SaveCommand;

            if (viewModel is ValidationViewModelBase validationViewModel)
            {
                validationViewModel.SaveCommand = new RelayCommand(() =>
                {
                    originalSaveCommand.Execute(null);
                    tcs.SetResult(viewModel.GetResult() as TResult);
                    dialogWindow.Close();
                }, () => originalSaveCommand.CanExecute(null));
            }

            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            await dialogWindow.ShowDialog(lifetime.MainWindow!);

            if (!tcs.Task.IsCompleted)
            {
                tcs.SetResult(null);
            }

            return await tcs.Task;
        }

        public async Task<bool> ShowConfirmationDialogAsync(string title, string message)
        {
            var view = new ConfirmationDialogView
            {
                DataContext = new ConfirmationViewModel { Message = message }
            };

            var dialogWindow = new Window
            {
                Content = view,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Width = 350,
                Height = 180,
                CanResize = false
            };

            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            var result = await dialogWindow.ShowDialog<bool?>(lifetime.MainWindow!);
            return result ?? false;
        }
    }
}
