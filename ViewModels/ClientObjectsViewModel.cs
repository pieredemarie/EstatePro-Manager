using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.DAL.Repository;
using RealEstateAgency.Models;
using RealEstateAgency.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RealEstateAgency.ViewModels
{
    public class ClientObjectsViewModel : ViewModelBase
    {
        private readonly IObjectRepository _objectRepository;
        private readonly INavigationService _navigationService;

        private string _selectedObjectType;
        private string _selectedDealType;
        private string _minPrice;
        private string _maxPrice;

        public ClientObjectsViewModel(IObjectRepository objectRepository, INavigationService navigationService)
        {
            _objectRepository = objectRepository;
            _navigationService = navigationService;

            InitializeProperties();
            InitializeCommands();
            LoadAvailableObjects();
        }

        public ObservableCollection<ObjectModel> AllObjects { get; private set; }
        public ObservableCollection<ObjectModel> FilteredObjects { get; private set; }
        public ObservableCollection<ObjectModel> Objects => FilteredObjects;

        public ObservableCollection<string> ObjectTypes { get; private set; }
        public ObservableCollection<string> DealTypes { get; private set; }

        public string SelectedObjectType
        {
            get => _selectedObjectType;
            set { _selectedObjectType = value; OnPropertyChanged(); ApplyFiltersInternal(); }
        }

        public string SelectedDealType
        {
            get => _selectedDealType;
            set { _selectedDealType = value; OnPropertyChanged(); ApplyFiltersInternal(); }
        }

        public string MinPrice
        {
            get => _minPrice;
            set { _minPrice = value; OnPropertyChanged(); ApplyFiltersInternal(); }
        }

        public string MaxPrice
        {
            get => _maxPrice;
            set { _maxPrice = value; OnPropertyChanged(); ApplyFiltersInternal(); }
        }

        public RelayCommand BookObjectCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand ApplyFiltersCommand { get; private set; }
        public RelayCommand ClearFiltersCommand { get; private set; }

        private void InitializeProperties()
        {
            ObjectTypes = new ObservableCollection<string> { "Все типы", "Дом", "Квартира" };
            DealTypes = new ObservableCollection<string> { "Все сделки", "Аренда", "Покупка" };

            SelectedObjectType = ObjectTypes.First();
            SelectedDealType = DealTypes.First();

            AllObjects = new ObservableCollection<ObjectModel>();
            FilteredObjects = new ObservableCollection<ObjectModel>();
        }

        private void InitializeCommands()
        {
            
            BookObjectCommand = new RelayCommand(() => { }); 
            RefreshCommand = new RelayCommand(LoadAvailableObjects);
            ApplyFiltersCommand = new RelayCommand(ApplyFiltersInternal);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
        }

        public void BookObjectFromButton(object parameter)
        {
            if (parameter is ObjectModel obj)
                BookSelectedObject(obj);
        }

        private void LoadAvailableObjects()
        {
            try
            {
                var objects = _objectRepository.GetAll()
                    .Where(o => o.Status != null &&
                                (o.Status.Name == "Свободен" ||
                                 o.Status.Name == "Доступен" ||
                                 o.Status.Name == "Активен") &&
                                o.ObjectPhoto != null &&
                                o.ObjectPhoto.Any())
                    .Select(o => new ObjectModel
                    {
                        Id = o.Id,
                        RoomCount = o.RoomCount,
                        Area = o.Area,
                        Address = o.Address,
                        Price = o.Price,
                        StatusName = o.Status?.Name,
                        TypeOfSubjectName = o.TypeOfSubject?.Name,
                        TypeOfDealName = o.TypeOfDeal?.Name,
                        OwnerName = o.Owner?.FullName,
                        MainPhotoUrl = o.ObjectPhoto.First().PhotoUrl
                    })
                    .OrderBy(o => o.Price)
                    .ToList();

                AllObjects = new ObservableCollection<ObjectModel>(objects);
                ApplyFiltersInternal();
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка загрузки объектов: {ex.Message}");
            }
        }

        private void ApplyFiltersInternal()
        {
            if (AllObjects == null) return;

            var filtered = AllObjects.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedObjectType) && SelectedObjectType != "Все типы")
                filtered = filtered.Where(o => o.TypeOfSubjectName == SelectedObjectType);

            if (!string.IsNullOrEmpty(SelectedDealType) && SelectedDealType != "Все сделки")
                filtered = filtered.Where(o => o.TypeOfDealName == SelectedDealType);

            if (decimal.TryParse(MinPrice, out var minPrice))
                filtered = filtered.Where(o => o.Price >= minPrice);

            if (decimal.TryParse(MaxPrice, out var maxPrice))
                filtered = filtered.Where(o => o.Price <= maxPrice);

            FilteredObjects = new ObservableCollection<ObjectModel>(filtered.ToList());
            OnPropertyChanged(nameof(Objects));
        }

        private void ClearFilters()
        {
            SelectedObjectType = ObjectTypes.First();
            SelectedDealType = DealTypes.First();
            MinPrice = string.Empty;
            MaxPrice = string.Empty;

            FilteredObjects = new ObservableCollection<ObjectModel>(AllObjects);
            OnPropertyChanged(nameof(Objects));
        }

        private void BookSelectedObject(ObjectModel obj)
        {
            if (obj == null) return;

            try
            {
                var fullObject = _objectRepository.GetByIdWithoutTracking(obj.Id);

                if (fullObject == null)
                {
                    _navigationService.ShowMessage("Ошибка", "Объект не найден в базе данных");
                    return;
                }

                var isAvailable = fullObject.Status?.Name == "Доступен";

                if (!isAvailable)
                {
                    _navigationService.ShowMessage("Ошибка", "Этот объект недоступен для бронирования");
                    return;
                }

                var result = _navigationService.ShowBookingWindow(fullObject);

                if (result == true)
                {
                    LoadAvailableObjects();
                    _navigationService.ShowMessage("Успех", "Объект успешно забронирован!");
                   
                }
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка при бронировании: {ex.Message}");

            }
        }
    }
}
