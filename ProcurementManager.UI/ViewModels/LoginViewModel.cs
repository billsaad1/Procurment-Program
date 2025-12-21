using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcurementManager.DataAccess;

namespace ProcurementManager.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ProcurementManagerDbContext _dbContext;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public LoginViewModel(ProcurementManagerDbContext dbContext)
        {
            _dbContext = dbContext;
            Title = "Login";
            LoginCommand = new AsyncRelayCommand(Login);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    OnPropertyChanged(nameof(IsErrorMessageVisible));
                }
            }
        }

        public bool IsErrorMessageVisible => !string.IsNullOrEmpty(ErrorMessage);

        public IAsyncRelayCommand LoginCommand { get; }

        public event EventHandler? LoginSuccessful;

        private async Task Login()
        {
            // TODO: THIS IS A TEMPORARY AND INSECURE LOGIN IMPLEMENTATION.
            // Replace this with a proper password hashing and verification library.
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == Username);

            if (user != null && user.PasswordHash == Password)
            {
                ErrorMessage = string.Empty;
                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
            }
        }
    }
}
