using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.DataAccess;
using ProcurementManager.Core.Models;

namespace ProcurementManager.UI.ViewModels
{
    public class SuppliersViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private ObservableCollection<Supplier> _suppliers;
        private Supplier? _selectedSupplier;

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        public ICommand AddSupplierCommand { get; }
        public ICommand EditSupplierCommand { get; }
        public ICommand DeleteSupplierCommand { get; }
        public IAsyncRelayCommand LoadSuppliersCommand { get; }

        public SuppliersViewModel(ProcurementManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            Title = "Suppliers";
            _suppliers = new ObservableCollection<Supplier>();

            LoadSuppliersCommand = new AsyncRelayCommand(LoadSuppliers);
            AddSupplierCommand = new RelayCommand(() => { /* TODO */ });
            EditSupplierCommand = new RelayCommand(() => { /* TODO */ }, () => SelectedSupplier != null);
            DeleteSupplierCommand = new RelayCommand(() => { /* TODO */ }, () => SelectedSupplier != null);

            LoadSuppliersCommand.Execute(null);
        }

        private async Task LoadSuppliers()
        {
            var suppliers = await _dbContext.Suppliers.ToListAsync();
            Suppliers = new ObservableCollection<Supplier>(suppliers);
        }
    }
}
