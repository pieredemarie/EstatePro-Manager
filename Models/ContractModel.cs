using RealEstateAgency.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Models
{
    public class ContractModel
    {
        public int Id { get; set; }
        public DateTime SigningDate { get; set; }
        public decimal Amount { get; set; }
        public string ClientName { get; set; }
        public string ObjectAddress { get; set; }

        // Статус
        public bool IsTerminated { get; set; }

        
        public string StatusText => IsTerminated ? "Расторгнут" : "Действует";
        public bool CanTerminate => !IsTerminated;
    }
}
