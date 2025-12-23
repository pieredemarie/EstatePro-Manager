using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateObject = RealEstateAgency.DAL.Object;

namespace RealEstateAgency.Services.Interfaces
{
    public interface INavigationService
    {
        void OpenMainWindow(User user);
        void CloseLoginWindow();
        void OpenRegisterWindow();
        void ShowMessage(string title, string message);
        bool? ShowAddEditUserWindow(IUserRepository repository, User user = null);
        bool? ShowAddEditObjectWindow(IObjectRepository repository, RealEstateObject obj = null);
        bool? ShowBookingWindow(RealEstateObject selectedObject);

        bool ShowConfirmation(string title, string message);

    }
}
