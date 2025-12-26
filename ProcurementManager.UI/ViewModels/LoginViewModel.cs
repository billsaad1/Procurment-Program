using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.DataAccess;
using ProcurementManager.UI.Services;

namespace ProcurementManager.UI.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private readonly IUserSessionService _userSessionService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsErrorMessageVisible))]
        private string? _errorMessage;

        public bool IsErrorMessageVisible => !string.IsNullOrEmpty(ErrorMessage);

        public event EventHandler? LoginSuccessful;

        public LoginViewModel(ProcurementManagerDbContext dbContext, IUserSessionService userSessionService)
        {
            _dbContext = dbContext;
            _userSessionService = userSessionService;
            Title = "Login";
        }

        [RelayCommand]
        private async Task Login()
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == Username);

            if (user != null && user.PasswordHash == Password)
            {
                ErrorMessage = null;
                _userSessionService.SetCurrentUser(user);
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
            }
        }
    }
}
