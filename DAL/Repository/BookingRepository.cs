using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly RealEstateDBEntity _context;

        public BookingRepository(RealEstateDBEntity context)
        {
            _context = context;
        }

        public void Add(Booking booking)
        {
            _context.Booking.Add(booking);
            _context.SaveChanges();
        }

        public void Update(Booking booking)
        {
            var existing = _context.Booking.Find(booking.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(booking);
                _context.SaveChanges();
            }
        }

        public void Delete(Booking booking)
        {
            _context.Booking.Remove(booking);
            _context.SaveChanges();
        }

        public Booking GetById(int id)
        {
            return _context.Booking.Find(id);
        }

        public IEnumerable<Booking> GetAll()
        {
            return _context.Booking.ToList();
        }

        public IEnumerable<Booking> GetByObjectId(int objectId)
        {
            return _context.Booking.Where(b => b.ObjectId == objectId).ToList();
        }

        public IEnumerable<Booking> GetByUserId(int userId)
        {
            return _context.Booking.Where(b => b.UserId == userId).ToList();
        }

    }
}