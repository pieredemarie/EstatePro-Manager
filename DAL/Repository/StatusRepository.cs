using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Repository
{
    public class StatusRepository : IStatusRepository
    {
        private readonly RealEstateDBEntity _context;

        public StatusRepository(RealEstateDBEntity context)
        {
            _context = context;
        }

        public void Add(Status status)
        {
            _context.Status.Add(status);
            _context.SaveChanges();
        }

        public void Update(Status status)
        {
            var existing = _context.Status.Find(status.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(status);
                _context.SaveChanges();
            }
        }

        public void Delete(Status status)
        {
            _context.Status.Remove(status);
            _context.SaveChanges();
        }

        public Status GetById(int id)
        {
            return _context.Status.Find(id);
        }

        public IEnumerable<Status> GetAll()
        {
            return _context.Status.ToList();
        }

        public Status GetByName(string name)
        {
            return _context.Status.FirstOrDefault(s => s.Name == name);
        }
    }
}
