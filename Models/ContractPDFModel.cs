using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Models
{
    public class ContractPdfDto
    {
        public int ContractId { get; set; }
        public DateTime SigningDate { get; set; }

        // Объект
        public string ObjectAddress { get; set; }
        public string ObjectType { get; set; }
        public double Area { get; set; }
        public int RoomCount { get; set; } 
        public string OwnerName { get; set; }
        public string OwnerPassport { get; set; }

        // Клиент
        public string ClientName { get; set; }
        public string ClientPassport { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; } 

        // Агент
        public string AgentName { get; set; }
        public string AgentPhone { get; set; }
        public string AgentEmail { get; set; } 

        // Деньги
        public decimal Amount { get; set; }
        public decimal Commission => Amount * 0.03m;
        public decimal Total => Amount + Commission;
    }
}
