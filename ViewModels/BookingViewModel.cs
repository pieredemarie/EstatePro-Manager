using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.DAL.Repository;
using RealEstateAgency.Services.Interfaces;
using System;
using RealEstateAgency.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class BookingViewModel : ViewModelBase
    {
        private readonly DAL.Object _selectedObject;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly INavigationService _navigationService;
        private readonly IObjectRepository _objectRepository;
        private DateTime? _checkInDate;
        private DateTime? _checkOutDate;
        private string _passportNumber;
        private string _phoneNumber;
        private int? _selectedUserId;
        private string _validationError;

        public event EventHandler BookingCompleted;

        public BookingViewModel(DAL.Object selectedObject)
        {
            _selectedObject = selectedObject;

            var context = new RealEstateDBEntity();
            _bookingRepository = new BookingRepository(context);
            _userRepository = new UserRepository(context);
            _navigationService = new NavigationService();

           
            _checkInDate = DateTime.Today;
            _checkOutDate = DateTime.Today.AddDays(1);

            InitializeCommands();
            LoadUsers();
            _objectRepository = new ObjectRepository(context);
        }

        public DAL.Object SelectedObject => _selectedObject;

        public ObservableCollection<User> Users { get; private set; }

        public int? SelectedUserId
        {
            get => _selectedUserId;
            set
            {
                _selectedUserId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
                CommandManager.InvalidateRequerySuggested(); 
            }
        }

        public string PassportNumber
        {
            get => _passportNumber;
            set
            {
                _passportNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                _checkInDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                _checkOutDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string ValidationError
        {
            get => _validationError;
            set
            {
                _validationError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBook));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        
        public bool CanBook => CheckInDate.HasValue &&
                              CheckOutDate.HasValue &&
                              SelectedUserId.HasValue &&
                              string.IsNullOrEmpty(ValidationError);

       
        public ICommand BookCommand { get; private set; }

        private void InitializeCommands()
        {
            BookCommand = new RelayCommand(Book, () => CanBook);
        }

        private void LoadUsers()
        {
            try
            {
                Users = new ObservableCollection<User>(
                    _userRepository.GetAll().Where(u => u.TypeOfUser?.Name == "Клиент"));
                OnPropertyChanged(nameof(Users));
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private void Validate()
        {
            ValidationError = string.Empty;

            if (CheckInDate.HasValue && CheckInDate.Value < DateTime.Today)
            {
                ValidationError = "Дата заезда не может быть в прошлом";
                return;
            }

            if (CheckInDate.HasValue && CheckOutDate.HasValue &&
                CheckOutDate.Value <= CheckInDate.Value)
            {
                ValidationError = "Дата выезда должна быть позже даты заезда";
                return;
            }

            if (!string.IsNullOrWhiteSpace(PassportNumber) && PassportNumber.Length < 10)
            {
                ValidationError = "Некорректный формат паспорта";
                return;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber.Length < 10)
            {
                ValidationError = "Некорректный формат телефона";
                return;
            }

            if (!SelectedUserId.HasValue)
            {
                ValidationError = "Выберите клиента";
                return;
            }

            if (CheckInDate.HasValue && CheckOutDate.HasValue)
            {
                if (!IsObjectAvailable(_selectedObject.Id, CheckInDate.Value, CheckOutDate.Value))
                {
                    ValidationError = "Объект уже забронирован на эти даты";
                    return;
                }
            }
        }

        private bool IsObjectAvailable(int objectId, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                var existingBookings = _bookingRepository.GetAll()
                    .Where(b => b.ObjectId == objectId && b.CheckOutDate >= DateTime.Today)
                    .ToList();

                foreach (var booking in existingBookings)
                {
                    bool isOverlap = checkIn < booking.CheckOutDate && checkOut > booking.CheckInDate;

                    if (isOverlap)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Book()
        {
            try
            {
                
                Validate();
                if (!string.IsNullOrEmpty(ValidationError))
                {
                    _navigationService.ShowMessage("Ошибка", ValidationError);
                    return;
                }

                var booking = new Booking
                {
                    ObjectId = _selectedObject.Id,
                    UserId = SelectedUserId.Value,
                    CheckInDate = CheckInDate.Value,
                    CheckOutDate = CheckOutDate.Value
                };

                _bookingRepository.Add(booking);

                var user = _userRepository.GetById(SelectedUserId.Value);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(PassportNumber))
                        user.Passport = PassportNumber;

                    if (!string.IsNullOrWhiteSpace(PhoneNumber))
                        user.PhoneNumber = PhoneNumber;

                    _userRepository.Update(user);
                }

                var bookedObj = _objectRepository.GetById(_selectedObject.Id);

                if (bookedObj != null)
                {
                    bookedObj.StatusId = 2;

                    _objectRepository.Update(bookedObj);
                }

                _navigationService.ShowMessage("Успех",
                    $"Объект успешно забронирован!\n" +
                    $"Период: {CheckInDate.Value:dd.MM.yyyy} - {CheckOutDate.Value:dd.MM.yyyy}\n" +
                    $"Клиент: {user?.FullName ?? "Не указан"}");

                BookingCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка при бронировании: {ex.Message}");
            }
        }
    }
}