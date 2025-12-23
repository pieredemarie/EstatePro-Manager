using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class AddEditUserViewModel : ViewModelBase
    {
        private readonly IUserRepository _userRepository;
        private User _user;
        private bool _isEditMode;

        public AddEditUserViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            User = new User();
            _isEditMode = false;
            Initialize();
        }

        public AddEditUserViewModel(IUserRepository userRepository, User user)
        {
            _userRepository = userRepository;
            User = _userRepository.GetById(user.Id);
            _isEditMode = true;
            Initialize();
        }

        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

       
        public string FullName
        {
            get => User?.FullName;
            set
            {
                if (User != null)
                {
                    User.FullName = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Email
        {
            get => User?.Email;
            set
            {
                if (User != null)
                {
                    User.Email = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int UserTypeId
        {
            get => User?.TypeOfUserId ?? 0;
            set
            {
                if (User != null)
                {
                    User.TypeOfUserId = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string PhoneNumber
        {
            get => User?.PhoneNumber;
            set
            {
                if (User != null)
                {
                    User.PhoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Passport
        {
            get => User?.Passport;
            set
            {
                if (User != null)
                {
                    User.Passport = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<TypeOfUser> UserTypes { get; private set; }

        public string WindowTitle => _isEditMode ? "Редактирование пользователя" : "Добавление пользователя";
        public string SubmitButtonText => _isEditMode ? "Сохранить" : "Добавить";

        public ICommand SubmitCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void Initialize()
        {
            LoadUserTypes();

            SubmitCommand = new RelayCommand(Submit, CanSubmit);
            CancelCommand = new RelayCommand(Cancel);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void LoadUserTypes()
        {
            UserTypes = new ObservableCollection<TypeOfUser>(_userRepository.GetAllUserTypes());
            OnPropertyChanged(nameof(UserTypes));
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   UserTypeId > 0;
        }

        private void Submit()
        {
            if (!CanSubmit())
            {
                MessageBox.Show("Заполните все обязательные поля (ФИО, Email, Роль)",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    _userRepository.Update(User);
                }
                else
                {
                    string defaultPassword = "1234";
                    User.passwordHashed = HashPassword(defaultPassword);

                    _userRepository.Add(User);
                }

                MessageBox.Show(_isEditMode ? "Пользователь успешно обновлен" : "Пользователь успешно добавлен",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseWindow(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }
    }
}