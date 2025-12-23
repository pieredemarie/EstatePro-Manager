using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
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
    /// Логика взаимодействия для AddEditUserWindow.xaml
    /// </summary>
    public partial class AddEditUserWindow : Window
    {
        public AddEditUserWindow(IUserRepository userRepository)
        {
            InitializeComponent();
            DataContext = new ViewModels.AddEditUserViewModel(userRepository);
        }

        public AddEditUserWindow(IUserRepository userRepository, User user)
        {
            InitializeComponent();
            DataContext = new ViewModels.AddEditUserViewModel(userRepository, user);
        }
    }
}
