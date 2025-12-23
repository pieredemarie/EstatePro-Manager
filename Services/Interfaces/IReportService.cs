using RealEstateAgency.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Services.Interfaces
{
    public interface IReportService
    {
        
        List<int> GetAvailableYears();
        List<ProfitDataItem> GetProfitReport(int year);
    }
}
