using RealEstateAgency.Models;
using RealEstateAgency.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private string _email;
        private string _password;
        private string _fullName;
        private string _statusMessage;

        public event Action RegistrationSuccessful;
        public event Action CancelRequested;
        public event Action<string, string> ShowMessage;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
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
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool CanRegister => !string.IsNullOrWhiteSpace(Email) &&
                                  !string.IsNullOrWhiteSpace(Password) &&
                                  !string.IsNullOrWhiteSpace(FullName);

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel(IAuthenticationService authService)
        {
            _authService = authService;

            RegisterCommand = new RelayCommand(
                execute: ExecuteRegister,
                canExecute: () => CanRegister
            );

            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteRegister()
        {
            if (_authService.IsUserExists(Email))
            {
                StatusMessage = "Email уже занят";
                ShowMessage?.Invoke("Ошибка регистрации", "Пользователь с таким email уже существует!");
                return;
            }

            var registerData = new RegisterData
            {
                Email = Email,
                Password = Password,
                FullName = FullName
            };

            if (_authService.Register(registerData))
            {
                StatusMessage = "Регистрация успешна!";
                ShowMessage?.Invoke("Успешная регистрация", "Регистрация успешна! Теперь вы можете войти в систему.");
                RegistrationSuccessful?.Invoke();
            }
            else
            {
                StatusMessage = "Ошибка регистрации";
                ShowMessage?.Invoke("Ошибка", "Ошибка при регистрации.");
            }
        }

        private void ExecuteCancel()
        {
            CancelRequested?.Invoke();
        }
    }
}
