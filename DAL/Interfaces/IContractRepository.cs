using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.DAL.Interfaces
{
    public interface IContractRepository
    {
        IEnumerable<Contract> GetAll();
        Contract GetById(int id);
        void Add(Contract entity);
        void Update(Contract entity);
        void Delete(Contract entity);

        // Специальные методы для отчетов
        IEnumerable<Contract> GetContractsByYear(int year);
        IEnumerable<Contract> GetCompletedContracts();
        decimal GetTotalProfitByYear(int year);
    }
}
