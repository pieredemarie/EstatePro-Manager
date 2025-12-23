using RealEstateAgency.DAL;
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

namespace RealEstateAgency.Views
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            var dbContext = new RealEstateDBEntity();
            var authService = new AuthenticationService(dbContext);
            var viewModel = new RegisterViewModel(authService);

            // Подписываемся на события
            viewModel.RegistrationSuccessful += OnRegistrationSuccessful;
            viewModel.CancelRequested += OnCancelRequested;
            viewModel.ShowMessage += OnShowMessage;

            DataContext = viewModel;
        }

        private void OnRegistrationSuccessful()
        {
            this.Close();
        }

        private void OnCancelRequested()
        {
            this.Close();
        }

        private void OnShowMessage(string title, string message)
        {
            MessageBoxImage icon = title.Contains("Ошибка") ? MessageBoxImage.Error : MessageBoxImage.Information;
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
