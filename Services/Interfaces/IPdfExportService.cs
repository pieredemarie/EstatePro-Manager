using RealEstateAgency.Models;
using RealEstateAgency.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Services.Interfaces
{
    public interface IPdfExportService
    {
        void ExportProfitReport(string fileName, int year, List<ProfitDataItem> data, decimal totalProfit);
        string ExportContractPdf(ContractPdfDto model);
    }
}
