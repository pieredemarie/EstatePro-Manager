using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Repository;
using RealEstateAgency.Services;
using RealEstateAgency.Services.Interfaces;
using System.Windows; 
using System.Windows.Input;

namespace RealEstateAgency.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
       
        private readonly RealEstateDBEntity _context;
        private readonly INavigationService _navService;

        
        public UserViewModel UserVM { get; private set; }
        public ContractsViewModel ContractsVM { get; private set; }
        public ObjectViewModel AgentObjectsVM { get; private set; }
        public ClientObjectsViewModel ClientObjectsVM { get; private set; }
        public ProfitReportViewModel ReportVM { get; private set; }
        public AgentBookingViewModel BookingVM { get; private set; }

        public User CurrentUser { get; }

        
        private Visibility _usersVis = Visibility.Collapsed;
        private Visibility _contractsVis = Visibility.Collapsed;
        private Visibility _agentObjVis = Visibility.Collapsed;
        private Visibility _clientObjVis = Visibility.Collapsed;
        private Visibility _reportVis = Visibility.Collapsed;
        private Visibility _bookingVis = Visibility.Collapsed;

        public Visibility UsersVisibility { get => _usersVis; set { _usersVis = value; OnPropertyChanged(); } }
        public Visibility ContractsVisibility { get => _contractsVis; set { _contractsVis = value; OnPropertyChanged(); } }
        public Visibility AgentObjectsVisibility { get => _agentObjVis; set { _agentObjVis = value; OnPropertyChanged(); } }
        public Visibility ClientObjectsVisibility { get => _clientObjVis; set { _clientObjVis = value; OnPropertyChanged(); } }
        public Visibility ReportVisibility { get => _reportVis; set { _reportVis = value; OnPropertyChanged(); } }
        public Visibility BookingVisibility { get => _bookingVis; set { _bookingVis = value; OnPropertyChanged(); } }

       
        public Visibility AdminMenuVisibility => CurrentUser?.TypeOfUser?.Name == "Админ" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AgentMenuVisibility => CurrentUser?.TypeOfUser?.Name == "Агент" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ClientMenuVisibility => CurrentUser?.TypeOfUser?.Name == "Клиент" ? Visibility.Visible : Visibility.Collapsed;

        
        public ICommand ShowUsersCommand { get; }
        public ICommand ShowContractsCommand { get; }
        public ICommand ShowAgentObjectsCommand { get; }
        public ICommand ShowClientObjectsCommand { get; }
        public ICommand ShowReportCommand { get; }
        public ICommand ShowBookingCommand { get; }

        public MainViewModel(User user)
        {
            CurrentUser = user;
            _context = new RealEstateDBEntity();
            _navService = new NavigationService();

      
            var userRepo = new UserRepository(_context);
            var objRepo = new ObjectRepository(_context);
            var contractRepo = new ContractRepository(_context);
            var bookingRepo = new BookingRepository(_context);

            
            if (CurrentUser.TypeOfUser?.Name == "Админ")
            {
                UserVM = new UserViewModel(userRepo, _navService);
                ReportVM = new ProfitReportViewModel();
            }
            else if (CurrentUser.TypeOfUser?.Name == "Агент")
            {
                AgentObjectsVM = new ObjectViewModel(objRepo, _navService);
                ContractsVM = new ContractsViewModel(contractRepo, objRepo, _navService);
                BookingVM = new AgentBookingViewModel(bookingRepo, userRepo, objRepo, _navService, user.Id);
            }
            else if (CurrentUser.TypeOfUser?.Name == "Клиент")
            {
                ClientObjectsVM = new ClientObjectsViewModel(objRepo, _navService);
            }

            
            ShowUsersCommand = new RelayCommand(() => SetActivePanel(nameof(UsersVisibility)));
            ShowContractsCommand = new RelayCommand(() => SetActivePanel(nameof(ContractsVisibility)));
            ShowAgentObjectsCommand = new RelayCommand(() => SetActivePanel(nameof(AgentObjectsVisibility)));
            ShowClientObjectsCommand = new RelayCommand(() => SetActivePanel(nameof(ClientObjectsVisibility)));
            ShowReportCommand = new RelayCommand(() => SetActivePanel(nameof(ReportVisibility)));
            ShowBookingCommand = new RelayCommand(() => SetActivePanel(nameof(BookingVisibility)));

          
            InitializeStartPage();
        }

        
        private void SetActivePanel(string activePanelName)
        {
            UsersVisibility = Visibility.Collapsed;
            ContractsVisibility = Visibility.Collapsed;
            AgentObjectsVisibility = Visibility.Collapsed;
            ClientObjectsVisibility = Visibility.Collapsed;
            ReportVisibility = Visibility.Collapsed;
            BookingVisibility = Visibility.Collapsed;

        
            switch (activePanelName)
            {
                case nameof(UsersVisibility): UsersVisibility = Visibility.Visible; break;
                case nameof(ContractsVisibility): ContractsVisibility = Visibility.Visible; break;
                case nameof(AgentObjectsVisibility): AgentObjectsVisibility = Visibility.Visible; break;
                case nameof(ClientObjectsVisibility): ClientObjectsVisibility = Visibility.Visible; break;
                case nameof(ReportVisibility): ReportVisibility = Visibility.Visible; break;
                case nameof(BookingVisibility): BookingVisibility = Visibility.Visible; break;
            }
        }

        private void InitializeStartPage()
        {
            var role = CurrentUser?.TypeOfUser?.Name;
            if (role == "Админ") SetActivePanel(nameof(UsersVisibility));
            else if (role == "Агент") SetActivePanel(nameof(AgentObjectsVisibility));
            else if (role == "Клиент") SetActivePanel(nameof(ClientObjectsVisibility));
        }
    }
}