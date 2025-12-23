using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Repository
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly RealEstateDBEntity _context;

        public ObjectRepository(RealEstateDBEntity context)
        {
            _context = context;
        }

        public IEnumerable<Object> GetAll()
        {
            return _context.Object
                .Include(o => o.Status)
                .Include(o => o.TypeOfSubject)
                .Include(o => o.TypeOfDeal)
                .Include(o => o.Owner)
                .Include(o => o.ObjectPhoto)
                .AsNoTracking()
                .ToList();
        }

        public Object GetById(int id)
        {
            return _context.Object.Find(id);
        }

        public void Add(Object entity)
        {
            _context.Object.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Object entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Object.Find(id);
            if (entity != null)
            {
                _context.Object.Remove(entity);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Object> Find(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            var term = searchTerm.ToLower();
            return _context.Object
                .Where(o => o.Address.ToLower().Contains(term) ||
                           o.Price.ToString().Contains(searchTerm))
                .ToList();
        }

        public IEnumerable<Object> GetWithDetails()
        {
            return _context.Object
                .Include(o => o.Status)
                .Include(o => o.TypeOfSubject)
                .Include(o => o.TypeOfDeal)
                .Include(o => o.Owner)
                .Include(o => o.ObjectPhoto)
                .ToList();
        }

        public IEnumerable<Status> GetAllStatuses()
        {
            return _context.Status.ToList();
        }

        public IEnumerable<TypeOfSubject> GetAllTypes()
        {
            return _context.TypeOfSubject.ToList();
        }

        public IEnumerable<TypeOfDeal> GetAllDeals()
        {
            return _context.TypeOfDeal.ToList();
        }

        public IEnumerable<Owner> GetAllOwners()
        {
            return _context.Owner.ToList();
        }

        public DAL.Object GetByIdWithoutTracking(int id)
        {
            return _context.Object
                .AsNoTracking() 
                .Include(o => o.Status)
                .Include(o => o.TypeOfSubject)
                .Include(o => o.TypeOfDeal)
                .Include(o => o.Owner)
                .Include(o => o.ObjectPhoto)
                .FirstOrDefault(o => o.Id == id);
        }
    }
}
