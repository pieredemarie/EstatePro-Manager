using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using RealEstateAgency.Services.Interfaces;
using RealEstateAgency.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RealEstateObject = RealEstateAgency.DAL.Object; // ну кто ж знал что Object это зарезервированное слово то а

namespace RealEstateAgency.Services
{
    public class NavigationService : INavigationService
    {

        public NavigationService() { }
        public void OpenMainWindow(User user)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
            });
        }

        public void CloseLoginWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is LoginWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            });
        }

        public void OpenRegisterWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var registerWindow = new RegisterWindow();
                registerWindow.Show();
            });
        }

        public void ShowMessage(string title, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }


        public bool? ShowAddEditUserWindow(IUserRepository repository, User user = null)
        {
            bool? result = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                AddEditUserWindow window;
                if (user == null)
                    window = new AddEditUserWindow(repository);
                else
                    window = new AddEditUserWindow(repository, user);

                result = window.ShowDialog();
            });

            return result;
        }

        public bool? ShowAddEditObjectWindow(IObjectRepository repository, DAL.Object obj = null)
        {
            bool? result = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                AddEditObjectWindow window;
                if (obj == null)
                    window = new AddEditObjectWindow(repository);
                else
                    window = new AddEditObjectWindow(repository, obj);

                result = window.ShowDialog();
            });

            return result;
        }
        public bool? ShowBookingWindow(DAL.Object selectedObject)
        {
            bool? result = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var bookingWindow = new BookingWindow(selectedObject);
                result = bookingWindow.ShowDialog();
            });

            return result;
        }

        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        }
    }