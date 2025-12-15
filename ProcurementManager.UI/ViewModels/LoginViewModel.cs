using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ProcurementManager.UI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public LoginViewModel()
        {
            Title = "Login";
            LoginCommand = new RelayCommand(Login);
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
            set => SetProperty(ref _errorMessage, value);
        }

        public IRelayCommand LoginCommand { get; }

        public event EventHandler? LoginSuccessful;

        private void Login()
        {
            // IMPORTANT: This is a placeholder for actual authentication logic.
            if (Username == "admin" && Password == "admin")
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
