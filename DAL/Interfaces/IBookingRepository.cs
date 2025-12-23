using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Interfaces
{
    public interface IBookingRepository
    {
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(Booking booking);
        Booking GetById(int id);
        IEnumerable<Booking> GetAll();
        IEnumerable<Booking> GetByObjectId(int objectId);
        IEnumerable<Booking> GetByUserId(int userId);


    }
}
