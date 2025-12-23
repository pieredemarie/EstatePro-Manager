using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Interfaces
{
    public interface IObjectRepository
    {
        IEnumerable<Object> GetAll();
        Object GetById(int id);
        void Add(Object entity);
        void Update(Object entity);
        void Delete(int id);
        IEnumerable<Object> Find(string searchTerm);

        IEnumerable<Status> GetAllStatuses();
        IEnumerable<TypeOfSubject> GetAllTypes();
        IEnumerable<TypeOfDeal> GetAllDeals();
        IEnumerable<Owner> GetAllOwners();
        DAL.Object GetByIdWithoutTracking(int id);
    }
}
