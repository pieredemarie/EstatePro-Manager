using RealEstateAgency.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RealEstateAgency.DAL.Repository
{
    public class ContractRepository : IContractRepository
    {
        private readonly RealEstateDBEntity _context;

        public ContractRepository(RealEstateDBEntity context)
        {
            _context = context;
        }

        public IEnumerable<Contract> GetAll()
        {
            return _context.Contract
                .Include(c => c.Object)
                .Include(c => c.User)
                .Include(c => c.Object.TypeOfSubject)
                .ToList();
        }

        public Contract GetById(int id)
        {
            return _context.Contract
                .Include(c => c.Object)
                .Include(c => c.Object.TypeOfSubject)
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(Contract entity)
        {
            _context.Contract.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Contract entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Contract entity)
        {
            _context.Contract.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<Contract> GetContractsByYear(int year)
        {
            return _context.Contract
                .Include(c => c.Object)
                .Include(c => c.Object.TypeOfSubject)
                .Where(c => c.SigningDate.Year == year)
                .ToList();
        }

        public IEnumerable<Contract> GetCompletedContracts()
        {
            
            return _context.Contract
                .Include(c => c.Object)
                .Include(c => c.Object.TypeOfSubject)
                .ToList();
        }

        public decimal GetTotalProfitByYear(int year)
        {
            var result = _context.Contract
                .Where(c => c.SigningDate.Year == year)
                .Sum(c => (decimal?)c.Amount) ?? 0;

            return result;
        }
    }
}