using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.Models;
using RealEstateAgency.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly IObjectRepository _objectRepository;
        private readonly INavigationService _navigationService;

       
        public ObservableCollection<ContractModel> Contracts { get; set; }

        private ContractModel _selectedContract;
        public ContractModel SelectedContract
        {
            get => _selectedContract;
            set
            {
                _selectedContract = value;
                OnPropertyChanged();
            }
        }

       
        public ICommand TerminateContractCommand { get; private set; }

        public ContractsViewModel(
            IContractRepository contractRepo,
            IObjectRepository objectRepo,
            INavigationService navService)
        {
            _contractRepository = contractRepo;
            _objectRepository = objectRepo;
            _navigationService = navService;

            Contracts = new ObservableCollection<ContractModel>();

            
            TerminateContractCommand = new RelayCommand(TerminateContract);

            LoadContracts();
        }

        public void LoadContracts()
        {
            try
            {
                Contracts.Clear();
                var data = _contractRepository.GetAll().ToList();

                foreach (var c in data)
                {
                   
                    bool isTerminated = c.Object != null && c.Object.StatusId == 1;

                    Contracts.Add(new ContractModel
                    {
                        Id = c.Id,
                        SigningDate = c.SigningDate,
                        Amount = c.Amount,
                        ClientName = c.User?.FullName ?? "Не указан",
                        ObjectAddress = c.Object?.Address ?? "Объект удален",
                        IsTerminated = isTerminated
                    });
                }
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", $"Ошибка загрузки: {ex.Message}");
            }
        }

        private void TerminateContract()
        {
          
            var item = SelectedContract;

            if (item == null)
            {
                _navigationService.ShowMessage("Ошибка", "Не выбран договор для расторжения.");
                return;
            }

           
            if (!_navigationService.ShowConfirmation("Расторжение", $"Расторгнуть договор №{item.Id}?"))
                return;

            try
            {
                
                var contract = _contractRepository.GetById(item.Id);

                if (contract != null && contract.ObjectId.HasValue)
                {
                    var realEstateObject = _objectRepository.GetById(contract.ObjectId.Value);

                    if (realEstateObject != null)
                    {
                        
                        realEstateObject.StatusId = 1;
                        _objectRepository.Update(realEstateObject);
                    }
                }

                
                LoadContracts();

                _navigationService.ShowMessage("Успех", "Договор расторгнут, объект возвращен в продажу.");
            }
            catch (Exception ex)
            {
                _navigationService.ShowMessage("Ошибка", ex.Message);
            }
        }
    }
}