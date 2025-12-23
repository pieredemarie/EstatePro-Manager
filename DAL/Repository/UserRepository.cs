using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RealEstateDBEntity _context;

        public UserRepository(RealEstateDBEntity context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User
                .Include(u => u.TypeOfUser)
                .OrderBy(u => u.FullName)
                .ToList();
        }

        public User GetById(int id)
        {
            return _context.User
                .Include(u => u.TypeOfUser)
                .FirstOrDefault(u => u.Id == id);
        }

        public void Add(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {

            if (_context.Entry(user).State == EntityState.Detached)
            {
                _context.User.Attach(user);
            }

            
            _context.Entry(user).State = EntityState.Modified;

           
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }

        public IEnumerable<User> Find(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.ToLower();
            return _context.User
                .Include(u => u.TypeOfUser)
                .Where(u => u.FullName.ToLower().Contains(term) ||
                           u.Email.ToLower().Contains(term) ||
                           u.PhoneNumber.Contains(searchTerm))
                .ToList();
        }

        public IEnumerable<TypeOfUser> GetAllUserTypes()
        {
            return _context.TypeOfUser.ToList();
        }
    }
}
