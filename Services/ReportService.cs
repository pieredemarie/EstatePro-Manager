using System.Linq;
using System.Collections.Generic;
using RealEstateAgency.DAL;
using RealEstateAgency.DAL.Repository;
using RealEstateAgency.Services.Interfaces;
using RealEstateAgency.ViewModels;

namespace RealEstateAgency.Services
{
    public class ReportService : IReportService
    {
        private readonly RealEstateDBEntity _context;

        public ReportService()
        {
            _context = new RealEstateDBEntity();
        }

        public List<int> GetAvailableYears()
        {
            var repo = new ContractRepository(_context);
            // Ваша логика получения годов
            var years = repo.GetAll()
                .Where(c => c.SigningDate != null)
                .Select(c => c.SigningDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            if (!years.Contains(System.DateTime.Now.Year))
                years.Insert(0, System.DateTime.Now.Year);

            return years;
        }

        public List<ProfitDataItem> GetProfitReport(int year)
        {
            var repo = new ContractRepository(_context);
            var contracts = repo.GetAll()
                .Where(c => c.SigningDate.Year == year && c.Object != null && c.Amount > 0)
                .ToList();

            // Вся математика ТУТ (включая 3%)
            var groupedData = contracts
                .GroupBy(c => c.Object?.TypeOfSubject?.Name ?? "Не указан")
                .Select(g => new ProfitDataItem
                {
                    ObjectTypeName = g.Key,
                    DealCount = g.Count(),
                    Profit = g.Sum(c => c.Amount * 0.03m), // Логика комиссии
                    Percentage = 0
                })
                .OrderByDescending(x => x.Profit)
                .ToList();

            // Расчет процентов
            decimal total = groupedData.Sum(x => x.Profit);
            if (total > 0)
            {
                groupedData.ForEach(x => x.Percentage = (double)(x.Profit / total * 100));
            }

            return groupedData;
        }
    }
}