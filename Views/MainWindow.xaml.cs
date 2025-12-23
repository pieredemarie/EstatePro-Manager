using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Repository;
using RealEstateAgency.Models;
using RealEstateAgency.Services;
using RealEstateAgency.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RealEstateAgency.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User loggedInUser)
        {
            InitializeComponent();
            _currentUser = loggedInUser;

           
            using (var context = new RealEstateDBEntity())
            {
                if (_currentUser.TypeOfUser == null)
                {
                    _currentUser = context.User
                        .Include("TypeOfUser")
                        .FirstOrDefault(u => u.Id == _currentUser.Id);
                }
            }

       
            ApplyRoleInterface();
        }

        private void ApplyRoleInterface()
        {
            string role = _currentUser.TypeOfUser?.Name;

            
            UsersToggle.Visibility = Visibility.Collapsed;
           
            Report1Toggle.Visibility = Visibility.Collapsed;
            ObjectsToggle.Visibility = Visibility.Collapsed;
            AgentObjectsToggle.Visibility = Visibility.Collapsed;

         
            var context = new RealEstateDBEntity();
            var reportViewModel = new ProfitReportViewModel();
            var userRepository = new UserRepository(context);
            var contractRepository = new ContractRepository(context);
            var objectRepository = new ObjectRepository(context);
            var navService = new NavigationService();
            var contractViewModel = new ContractsViewModel(contractRepository,objectRepository,navService);
            var objectViewModel = new ObjectViewModel(objectRepository, navService);
            var bookingRepository = new BookingRepository(context);
            var agentBookingViewModel = new AgentBookingViewModel(
                bookingRepository,
                userRepository,
                objectRepository,
                navService,
                _currentUser.Id  
            ); 

            if (role == "Админ")
            {
               
                var userViewModel = new UserViewModel(userRepository, navService);

               
                UsersPanel.DataContext = userViewModel;
                Report1Panel.DataContext = reportViewModel;
                
                UsersToggle.Visibility = Visibility.Visible;
             
                Report1Toggle.Visibility = Visibility.Visible;
                ObjectsToggle.Visibility = Visibility.Collapsed;
                ContractsToggle.Visibility = Visibility.Collapsed;
                BookingToggle.Visibility = Visibility.Collapsed;

              
                UsersToggle.IsChecked = true;
                UsersPanel.Visibility = Visibility.Visible;
            }
            else if (role == "Агент")
            {
               
                AgentObjectsPanel.DataContext = objectViewModel;
                BookingPanel.DataContext = agentBookingViewModel;
                ContractsPanel.DataContext = contractViewModel;
               
                AgentObjectsToggle.Visibility = Visibility.Visible; 
                BookingToggle.Visibility = Visibility.Visible;
                ContractsToggle.Visibility = Visibility.Visible;
                UsersToggle.Visibility = Visibility.Collapsed;
                
                Report1Toggle.Visibility = Visibility.Collapsed;
                ObjectsToggle.Visibility = Visibility.Collapsed; 

               
                AgentObjectsToggle.IsChecked = true;
                AgentObjectsPanel.Visibility = Visibility.Visible;
            }
            else if (role == "Клиент")
            {
                var clientObjectsViewModel = new ClientObjectsViewModel(objectRepository, navService);
                

                ObjectsPanel.DataContext = clientObjectsViewModel;

              

           
                ObjectsToggle.Visibility = Visibility.Visible;
                UsersToggle.Visibility = Visibility.Collapsed;
               
                Report1Toggle.Visibility = Visibility.Collapsed;
                ContractsToggle.Visibility = Visibility.Collapsed;
                BookingToggle.Visibility = Visibility.Collapsed;
                AgentObjectsToggle.Visibility = Visibility.Collapsed;

                // Показываем панель объектов по умолчанию
                ObjectsToggle.IsChecked = true;
                ObjectsPanel.Visibility = Visibility.Visible;
            }
        }

        private void HideAllPanels()
        {
            ObjectsPanel.Visibility = Visibility.Collapsed;
            AgentObjectsPanel.Visibility = Visibility.Collapsed;
            UsersPanel.Visibility = Visibility.Collapsed;
            ContractsPanel.Visibility = Visibility.Collapsed;
            BookingPanel.Visibility = Visibility.Collapsed;
            DirectoriesPanel.Visibility = Visibility.Collapsed;
            Report1Panel.Visibility = Visibility.Collapsed;
        }

        private void UncheckAllToggleButtons()
        {
            ObjectsToggle.IsChecked = false;
            AgentObjectsToggle.IsChecked = false;
            UsersToggle.IsChecked = false;

            Report1Toggle.IsChecked = false;
            ContractsToggle.IsChecked = false;
            BookingToggle.IsChecked = false;
        }

        private void ObjectsToggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            ObjectsPanel.Visibility = Visibility.Visible;
            ObjectsToggle.IsChecked = true;
        }

        private void AgentObjectsToggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            AgentObjectsPanel.Visibility = Visibility.Visible;
            AgentObjectsToggle.IsChecked = true;
        }

        private void BookingToggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            BookingPanel.Visibility = Visibility.Visible;
            BookingToggle.IsChecked = true;
        }

        private void ContractsToggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            ContractsPanel.Visibility = Visibility.Visible;
            ContractsToggle.IsChecked = true;
        }

        private void UsersToggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            UsersPanel.Visibility = Visibility.Visible;
            UsersToggle.IsChecked = true;
        }

        private void DirectoriesToggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            DirectoriesPanel.Visibility = Visibility.Visible;
          
        }

        private void Report1Toggle_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            UncheckAllToggleButtons();
            Report1Panel.Visibility = Visibility.Visible;
            Report1Toggle.IsChecked = true;
        }


        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ObjectModel obj)
            {
                if (ObjectsPanel.DataContext is ClientObjectsViewModel vm)
                {
                    vm.BookObjectFromButton(obj);
                }
                   
            }
        }
    }
}