using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.Models;
using RealEstateAgency.Services.Interfaces;
using RealEstateAgency.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class ObjectViewModel : ViewModelBase
    {
        private readonly IObjectRepository _objectRepository;
        private readonly INavigationService _navigationService;
        private ObjectModel _selectedObject;

        public ObjectViewModel(IObjectRepository objectRepository, INavigationService navigationService)
        {
            _objectRepository = objectRepository;
            _navigationService = navigationService;

            InitializeCommands();
            LoadObjects();
            LoadReferenceData();
        }

       
        public ObservableCollection<ObjectModel> Objects { get; private set; }
        public ObservableCollection<Status> Statuses { get; private set; }
        public ObservableCollection<TypeOfSubject> Types { get; private set; }
        public ObservableCollection<TypeOfDeal> Deals { get; private set; }
        public ObservableCollection<Owner> Owners { get; private set; }

        public ObjectModel SelectedObject
        {
            get => _selectedObject;
            set { _selectedObject = value; OnPropertyChanged(); }
        }

       
        public ICommand AddObjectCommand { get; private set; }
        public ICommand EditObjectCommand { get; private set; }
        public ICommand DeleteObjectCommand { get; private set; }
        public ICommand RefreshObjectsCommand { get; private set; }

        private void InitializeCommands()
        {
            AddObjectCommand = new RelayCommand(AddObject);
            EditObjectCommand = new RelayCommand(EditObject, () => SelectedObject != null);
            DeleteObjectCommand = new RelayCommand(DeleteObject, () => SelectedObject != null);
            RefreshObjectsCommand = new RelayCommand(RefreshObjects);
        }

        private void LoadObjects()
        {
            try
            {
                
                var allObjects = _objectRepository.GetAll().ToList();

                var objectModels = allObjects
                    .Select(o => new ObjectModel
                    {
                        Id = o.Id,
                        RoomCount = o.RoomCount,
                        Area = o.Area,
                        Address = o.Address,
                        Price = o.Price,
                       
                        StatusName = o.Status?.Name ?? "Не указан",
                        TypeOfSubjectName = o.TypeOfSubject?.Name ?? "Не указан",
                        TypeOfDealName = o.TypeOfDeal?.Name ?? "Не указан",
                        OwnerName = o.Owner?.FullName ?? "Не указан"
                    })
                    .OrderBy(o => o.Price)
                    .ToList();

                Objects = new ObservableCollection<ObjectModel>(objectModels);
                OnPropertyChanged(nameof(Objects));
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка загрузки объектов: {ex.Message}");
            }
        }

        private void LoadReferenceData()
        {
            try
            {
                Statuses = new ObservableCollection<Status>(_objectRepository.GetAllStatuses());
                Types = new ObservableCollection<TypeOfSubject>(_objectRepository.GetAllTypes());
                Deals = new ObservableCollection<TypeOfDeal>(_objectRepository.GetAllDeals());
                Owners = new ObservableCollection<Owner>(_objectRepository.GetAllOwners());

                OnPropertyChanged(nameof(Statuses));
                OnPropertyChanged(nameof(Types));
                OnPropertyChanged(nameof(Deals));
                OnPropertyChanged(nameof(Owners));
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка загрузки справочных данных: {ex.Message}");
            }
        }

        private void AddObject()
        {
            var addObjectWindow = new AddEditObjectWindow(_objectRepository);
            if (addObjectWindow.ShowDialog() == true)
            {
                LoadObjects();
                _navigationService.ShowMessage("Успех", "Объект добавлен и список обновлен");
              
            }
        }

        private void EditObject()
        {
            if (SelectedObject == null) return;

           
            var dbObject = _objectRepository.GetById(SelectedObject.Id);
            if (dbObject == null)
            {
                _navigationService.ShowMessage("Ошибка", "Объект не найден в базе данных");
                
                return;
            }

            var result = _navigationService.ShowAddEditObjectWindow(_objectRepository, dbObject);
            if (result == true)
            {
                RefreshObjects();
            }
        }

        private void DeleteObject()
        {
            if (SelectedObject == null) return;

            var result = MessageBox.Show(
                $"Удалить объект по адресу: {SelectedObject.Address}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _objectRepository.Delete(SelectedObject.Id);
                    Objects.Remove(SelectedObject);
                    _navigationService.ShowMessage("Успех", "Объект успешно удален");
                }
                catch (Exception ex)
                {
                    _navigationService.ShowMessage("Ошибка", $"Ошибка при удалении объекта: {ex.Message}");
                }
            }
        }

        private void RefreshObjects()
        {
            try
            {
                foreach (var obj in Objects)
                {
                    var dbObj = _objectRepository.GetById(obj.Id);
                    if (dbObj != null)
                    {
                        dbObj.Address = obj.Address;
                        dbObj.Price = obj.Price.HasValue ? (int)obj.Price.Value : dbObj.Price;
                        dbObj.RoomCount = obj.RoomCount.HasValue ? obj.RoomCount.Value : dbObj.RoomCount;
                        dbObj.Area = obj.Area.HasValue ? (int)obj.Area.Value : dbObj.Area;

                        dbObj.StatusId = Statuses.FirstOrDefault(s => s.Name == obj.StatusName)?.Id ?? dbObj.StatusId;
                        dbObj.TypeOfSubjectId = Types.FirstOrDefault(t => t.Name == obj.TypeOfSubjectName)?.Id ?? dbObj.TypeOfSubjectId;
                        dbObj.TypeOfDealId = Deals.FirstOrDefault(d => d.Name == obj.TypeOfDealName)?.Id ?? dbObj.TypeOfDealId;
                        dbObj.OwnerId = Owners.FirstOrDefault(o => o.FullName == obj.OwnerName)?.Id ?? dbObj.OwnerId;

                        _objectRepository.Update(dbObj);
                    }
                }

                
                LoadObjects();
                _navigationService.ShowMessage("Обновление", "Все изменения сохранены и список обновлен");
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка обновления объектов: {ex.Message}");
            }
        }

    }
}