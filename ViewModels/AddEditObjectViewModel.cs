using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity; 
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class AddEditObjectViewModel : ViewModelBase
    {
        private readonly IObjectRepository _objectRepository; 
        private readonly RealEstateDBEntity _context;

        private DAL.Object _object;
        private bool _isEditMode;
        private string _mainPhotoUrl; 

        
        public AddEditObjectViewModel(IObjectRepository objectRepository)
        {
            _objectRepository = objectRepository;
            _context = new RealEstateDBEntity(); // Создаем свой контекст
            Object = new DAL.Object();
            _isEditMode = false;
            Initialize();
        }

       
        public AddEditObjectViewModel(IObjectRepository objectRepository, DAL.Object obj)
        {
            _objectRepository = objectRepository;
            _context = new RealEstateDBEntity();

            
            Object = _context.Object
                .Include(o => o.Status)
                .Include(o => o.TypeOfSubject)
                .Include(o => o.TypeOfDeal)
                .Include(o => o.Owner)
                .Include(o => o.ObjectPhoto) 
                .FirstOrDefault(o => o.Id == obj.Id) ?? obj;

            _isEditMode = true;

           
            var existingPhoto = Object.ObjectPhoto?.FirstOrDefault();
            if (existingPhoto != null)
            {
                MainPhotoUrl = existingPhoto.PhotoUrl;
            }

            Initialize();
        }

        public DAL.Object Object
        {
            get => _object;
            set { _object = value; OnPropertyChanged(); }
        }

        
        public string MainPhotoUrl
        {
            get => _mainPhotoUrl;
            set
            {
                _mainPhotoUrl = value;
                OnPropertyChanged();
            }
        }

       
        public string Address
        {
            get => Object?.Address;
            set { if (Object != null) { Object.Address = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public int? RoomCount
        {
            get => Object?.RoomCount;
            set { if (Object != null) { Object.RoomCount = value ?? 0; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public decimal? Area
        {
            get => Object?.Area;
            set { if (Object != null && value.HasValue) { Object.Area = Convert.ToInt32(value.Value); OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public decimal? Price
        {
            get => Object?.Price;
            set { if (Object != null && value.HasValue) { Object.Price = Convert.ToInt32(value.Value); OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public int? StatusId
        {
            get => Object?.StatusId;
            set { if (Object != null) { Object.StatusId = value ?? 0; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public int? TypeOfSubjectId
        {
            get => Object?.TypeOfSubjectId;
            set { if (Object != null) { Object.TypeOfSubjectId = value ?? 0; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public int? TypeOfDealId
        {
            get => Object?.TypeOfDealId;
            set { if (Object != null) { Object.TypeOfDealId = value ?? 0; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public int? OwnerId
        {
            get => Object?.OwnerId;
            set { if (Object != null) { Object.OwnerId = value ?? 0; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
        }

        
        public ObservableCollection<Status> Statuses { get; private set; }
        public ObservableCollection<TypeOfSubject> Types { get; private set; }
        public ObservableCollection<TypeOfDeal> Deals { get; private set; }
        public ObservableCollection<Owner> Owners { get; private set; }

        public string WindowTitle => _isEditMode ? "Редактирование объекта" : "Добавление объекта";
        public string SubmitButtonText => _isEditMode ? "Сохранить" : "Добавить";

        public ICommand SubmitCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void Initialize()
        {
            LoadReferenceData();
            SubmitCommand = new RelayCommand(Submit, CanSubmit);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadReferenceData()
        {
            
            Statuses = new ObservableCollection<Status>(_context.Status.ToList());
            Types = new ObservableCollection<TypeOfSubject>(_context.TypeOfSubject.ToList());
            Deals = new ObservableCollection<TypeOfDeal>(_context.TypeOfDeal.ToList());
            Owners = new ObservableCollection<Owner>(_context.Owner.ToList());

            OnPropertyChanged(nameof(Statuses));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(Deals));
            OnPropertyChanged(nameof(Owners));
        }

        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(Address) &&
                   (RoomCount ?? 0) > 0 &&
                   (Area ?? 0) > 0 &&
                   (Price ?? 0) > 0 &&
                   (StatusId ?? 0) > 0 &&
                   (TypeOfSubjectId ?? 0) > 0 &&
                   (TypeOfDealId ?? 0) > 0 &&
                   (OwnerId ?? 0) > 0;
        }

        private void Submit()
        {
            if (!CanSubmit())
            {
                
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                if (_isEditMode)
                {
                    if (_context.Entry(Object).State == EntityState.Detached)
                        _context.Object.Attach(Object);

                    _context.Entry(Object).State = EntityState.Modified;
                }
                else
                {
                    _context.Object.Add(Object);
                }

               
                if (!string.IsNullOrWhiteSpace(MainPhotoUrl))
                {
                   
                    var existingPhoto = _context.ObjectPhoto.FirstOrDefault(p => p.ObjectId == Object.Id);

                    if (existingPhoto != null)
                    {
                        
                        existingPhoto.PhotoUrl = MainPhotoUrl;
                        _context.Entry(existingPhoto).State = EntityState.Modified;
                    }
                    else
                    {
                       
                        var newPhoto = new ObjectPhoto
                        {
                            ObjectId = Object.Id,
                            PhotoUrl = MainPhotoUrl
                        };
                        _context.ObjectPhoto.Add(newPhoto);
                    }

                    
                    var extraPhotos = _context.ObjectPhoto
                        .Where(p => p.ObjectId == Object.Id && p.PhotoUrl != MainPhotoUrl)
                        .ToList();
                    if (extraPhotos.Any())
                    {
                        _context.ObjectPhoto.RemoveRange(extraPhotos);
                    }
                }

                
                _context.SaveChanges();

                MessageBox.Show(_isEditMode ? "Объект успешно обновлен" : "Объект успешно добавлен",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка");
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