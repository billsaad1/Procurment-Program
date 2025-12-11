using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcurementManager.UI.ViewModels
{
    public abstract class DialogViewModelBase<TResult> : ObservableObject
    {
        public event Action<TResult> Saved;
        public event Action Canceled;

        protected void OnSaved(TResult result)
        {
            Saved?.Invoke(result);
        }

        protected void OnCanceled()
        {
            Canceled?.Invoke();
        }
    }
}
