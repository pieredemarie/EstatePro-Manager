using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.Services.Interfaces;
using RealEstateAgency.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly IUserRepository _userRepository;
        private User _selectedUser;
        private readonly INavigationService _navService;
        public UserViewModel(IUserRepository userRepository, INavigationService navService)
        {
            _userRepository = userRepository;
            _navService = navService;
            InitializeCommands();
            LoadUsers();
        }

        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<TypeOfUser> UserTypes { get; private set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public ICommand AddUserCommand { get; private set; }
        public ICommand RefreshUsersCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }

        private void InitializeCommands()
        {
            AddUserCommand = new RelayCommand(AddUser);
            RefreshUsersCommand = new RelayCommand(RefreshUsers);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(_userRepository.GetAll());
            OnPropertyChanged(nameof(Users));
        }

        private void AddUser()
        {
            var addUserWindow = new AddEditUserWindow(_userRepository);
            if (addUserWindow.ShowDialog() == true)
            {
                RefreshUsers();
            }
        }

        private void RefreshUsers()
        {
            try
            {
                
                foreach (var user in Users)
                {
                    _userRepository.Update(user);
                }

               
                LoadUsers();

                _navService.ShowMessage("Все изменения успешно сохранены", "Обновление");
            }
            catch (System.Exception ex)
            {
                _navService.ShowMessage("Ошибка", $"Ошибка при сохранении: {ex.Message}");
           
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Удалить пользователя {SelectedUser.FullName}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _userRepository.Delete(SelectedUser.Id);
                    Users.Remove(SelectedUser);
                    _navService.ShowMessage("Успех", "Пользователь успешно удален");
                }
                catch (System.Exception ex)
                {
                    _navService.ShowMessage("Ошибка", $"Ошибка при удалении: {ex.Message}");
                    
                }
            }
        }
    }
}