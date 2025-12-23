using iText.Bouncycastle;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.DAL.Repository;
using RealEstateAgency.Models;
using RealEstateAgency.Services;
using RealEstateAgency.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class AgentBookingViewModel : ViewModelBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IObjectRepository _objectRepository;
        private readonly INavigationService _navigationService;
        private BookingModel _selectedBooking;
        private User _currentAgent;
        private readonly IPdfExportService _pdfExportService;
        private readonly ContractService _contractService;
        public AgentBookingViewModel(IBookingRepository bookingRepository,
                                    IUserRepository userRepository,
                                    IObjectRepository objectRepository,
                                    INavigationService navigationService,
                                    int currentUserId)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _objectRepository = objectRepository;
            _navigationService = navigationService;

            _pdfExportService = new PdfExportService();
            _contractService = new ContractService(_pdfExportService);

            InitializeCommands();
            LoadCurrentAgent(currentUserId);
            LoadBookings();
        }

        public ObservableCollection<BookingModel> Bookings { get; private set; }

        public BookingModel SelectedBooking
        {
            get => _selectedBooking;
            set { _selectedBooking = value; OnPropertyChanged(); }
        }

        public ICommand CreateContractCommand { get; private set; }
        public ICommand DeleteBookingCommand { get; private set; }
        public ICommand RefreshBookingsCommand { get; private set; }

        private void InitializeCommands()
        {
            CreateContractCommand = new RelayCommand(CreateContract, () => SelectedBooking != null);
            DeleteBookingCommand = new RelayCommand(DeleteBooking, () => SelectedBooking != null);
            RefreshBookingsCommand = new RelayCommand(RefreshBookings);
        }

        private void LoadCurrentAgent(int userId)
        {
            try
            {
                _currentAgent = _userRepository.GetById(userId);

                if (_currentAgent == null || _currentAgent.TypeOfUser?.Name != "Агент")
                {
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных агента: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBookings()
        {
            try
            {
                using (var context = new RealEstateDBEntity())
                {
                    var bookings = context.Booking
                        .Include(b => b.Object)
                        .Include(b => b.User)
                        .Include(b => b.Object.Owner)
                        .Include(b => b.Object.TypeOfDeal)
                        .Include(b => b.Object.TypeOfSubject)
                        .Include(b => b.Object.Status)
                        .ToList();

                    var bookingModels = bookings.Select(b =>
                    {
                        return new BookingModel
                        {
                            Id = b.Id,
                            ObjectId = b.ObjectId,
                            ObjectAddress = b.Object?.Address ?? "Не указан",
                            ClientName = b.User?.FullName ?? "Не указан",
                            ClientPhone = b.User?.PhoneNumber ?? "Не указан",
                            ClientEmail = b.User?.Email ?? "Не указан",
                            BookingDate = b.CheckInDate,
                            CheckInDate = b.CheckInDate,
                            CheckOutDate = b.CheckOutDate,
                            Price = b.Object?.Price ?? 0,
                            Status = GetBookingStatus(b),
                            Notes = $"Забронировано с {b.CheckInDate:dd.MM.yyyy} по {b.CheckOutDate:dd.MM.yyyy}"
                        };
                    }).OrderByDescending(b => b.BookingDate).ToList();

                    Bookings = new ObservableCollection<BookingModel>(bookingModels);
                    OnPropertyChanged(nameof(Bookings));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки бронирований: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetBookingStatus(Booking booking)
        {
            if (booking == null)
                return "Не указаны даты";

            if (DateTime.Now < booking.CheckInDate)
                return "Ожидает";
            else if (DateTime.Now >= booking.CheckInDate && DateTime.Now <= booking.CheckOutDate)
                return "Активно";
            else
                return "Завершено";
        }

        private void CreateContract()
        {
            if (SelectedBooking == null) return;

        
            if (_currentAgent?.TypeOfUser?.Name != "Агент")
            {
                _navigationService.ShowMessage("Доступ запрещен", "Только агент может оформлять договоры.");
                return;
            }

       
            var result = _contractService.CreateContractFromBooking(SelectedBooking.Id, _currentAgent);

            if (result.Success)
            {
                _navigationService.ShowMessage("Успех", $"✅ Договор успешно оформлен!\nПуть: {result.PdfPath}");

                
                try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(result.PdfPath) { UseShellExecute = true }); } catch { }

              
                var bookingToRemove = Bookings.FirstOrDefault(b => b.Id == SelectedBooking.Id);
                if (bookingToRemove != null) Bookings.Remove(bookingToRemove);

                SelectedBooking = null;
                RefreshBookings(); 
            }
            else
            {
                _navigationService.ShowMessage("Ошибка", result.Message);
            }
        }

       

        private void DeleteBooking()
        {
            if (SelectedBooking == null) return;

            var result = MessageBox.Show(
                $"Удалить бронирование для объекта: {SelectedBooking.ObjectAddress}?\nКлиент: {SelectedBooking.ClientName}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new RealEstateDBEntity())
                    {
                        var booking = context.Booking.Find(SelectedBooking.Id);
                        if (booking != null)
                        {
                            context.Booking.Remove(booking);
                            context.SaveChanges();
                            Bookings.Remove(SelectedBooking);
                            SelectedBooking = null;

                            MessageBox.Show("Бронирование удалено", "Успех",
                                           MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshBookings()
        {
            LoadBookings();
        }
    }
}