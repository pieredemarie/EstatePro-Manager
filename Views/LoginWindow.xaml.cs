using RealEstateAgency.DAL;
using RealEstateAgency.Models;
using RealEstateAgency.Services;
using RealEstateAgency.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RealEstateAgency.DAL;
using RealEstateAgency.Services.Interfaces;
namespace RealEstateAgency.Views
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var dbContext = new RealEstateDBEntity();
            var authService = new AuthenticationService(dbContext);
            var navService = new NavigationService();
            var viewModel = new LoginViewModel(authService,navService);

            
            viewModel.OpenRegisterRequested += () => OnOpenRegisterRequested();
            viewModel.ShowMessage += (title, message) => OnShowMessage(title, message);

            DataContext = viewModel;
        }

       

        private void OnOpenRegisterRequested()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }

        private void OnShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = ((PasswordBox)sender).Password;

            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = password;
            }
        }
    }
}
