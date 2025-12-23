using RealEstateAgency.DAL;
using RealEstateAgency.Models;
using RealEstateAgency.Services;
using RealEstateAgency.Services.Interfaces;
using RealEstateAgency.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RealEstateAgency.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navService;
        private string _email;
        private string _password;
        private string _statusMessage;

        public event Action<User> LoginSuccessful;
        public event Action OpenRegisterRequested;
        public event Action<string, string> ShowMessage;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool CanLogin => !string.IsNullOrWhiteSpace(Email) &&
                                !string.IsNullOrWhiteSpace(Password);

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(IAuthenticationService authService, INavigationService navService)
        {
            _authService = authService;
            _navService = navService;

            LoginCommand = new RelayCommand(
            execute: ExecuteLogin,       
            canExecute: () => CanLogin   
        );

            RegisterCommand = new RelayCommand(
                execute: ExecuteRegister      
            );
        }
        private void ExecuteLogin()
        {
            var user = _authService.Authenticate(Email, Password);

            if (user != null)
            {
                StatusMessage = $"Добро пожаловать, {user.FullName}!";
                Application.Current.Properties["CurrentUser"] = user;

                _navService.OpenMainWindow(user);
                _navService.CloseLoginWindow();
                
            }
            else
            {
                StatusMessage = "Ошибка входа";
                _navService.ShowMessage("Ошибка входа", "Неверный email или пароль!");
               
            }
        }

        private void ExecuteRegister()
        {
          
           _navService.OpenRegisterWindow();
        }

    }
}
