using RealEstateAgency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateAgency.DAL;

namespace RealEstateAgency.Services
{
    public interface IAuthenticationService
    {
        bool IsUserExists(string email);
        User Authenticate(string email, string password);
        bool Register(RegisterData registerData);
    }
}
