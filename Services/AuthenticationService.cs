using RealEstateAgency.DAL;
using RealEstateAgency.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Services
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly RealEstateDBEntity _dbContext;

        public AuthenticationService(RealEstateDBEntity dbContext)
        {
            _dbContext = dbContext;
        }

        
        public bool IsUserExists(string email)
        {
            return _dbContext.User.Any(u => u.Email == email);
        }

        
        public bool Register(RegisterData registerData)
        {
            if (IsUserExists(registerData.Email))
                return false;

            
            var role = _dbContext.TypeOfUser.FirstOrDefault(t => t.Name == "Клиент");
            if (role == null)
                return false; 

            var newUser = new User
            {
                Email = registerData.Email,
                passwordHashed = HashPassword(registerData.Password),
                FullName = registerData.FullName,
                TypeOfUserId = role.Id,
                Passport = "не указано"
            };

            _dbContext.User.Add(newUser);
            try
            {
                _dbContext.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                throw;
            }

            return true;
        }

       
        public User Authenticate(string email, string password)
        {
            var user = _dbContext.User
                        .Include(u => u.TypeOfUser)
                        .FirstOrDefault(u => u.Email == email);

            if (user == null)
                return null;

            bool isPasswordValid = VerifyPassword(password, user.passwordHashed);
            return isPasswordValid ? user : null;
        }

       
        public string GetUserRole(string email)
        {
            var user = _dbContext.User
                        .Include(u => u.TypeOfUser)
                        .FirstOrDefault(u => u.Email == email);

            return user?.TypeOfUser?.Name;
        }



        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hashedPassword;
        }
    }
}
