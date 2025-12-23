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
    /// Логика взаимодействия для AddEditObjectWindow.xaml
    /// </summary>
    public partial class AddEditObjectWindow : Window
    {
        public AddEditObjectWindow(IObjectRepository objectRepository)
        {
            InitializeComponent();
            DataContext = new ViewModels.AddEditObjectViewModel(objectRepository);
        }

        public AddEditObjectWindow(IObjectRepository objectRepository, DAL.Object obj)
        {
            InitializeComponent();
            DataContext = new ViewModels.AddEditObjectViewModel(objectRepository, obj);
        }
    }
}
