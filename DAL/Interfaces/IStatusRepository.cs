using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Interfaces
{
    public interface IStatusRepository
    {
        void Add(Status status);
        void Update(Status status);
        void Delete(Status status);
        Status GetById(int id);
        IEnumerable<Status> GetAll();
        Status GetByName(string name);
    }
}
