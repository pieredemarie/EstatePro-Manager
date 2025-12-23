using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string ObjectAddress { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }

        // Форматированные даты для отображения
        public string BookingDateFormatted => BookingDate.ToString("dd.MM.yyyy");
        public string PeriodFormatted => CheckInDate.HasValue && CheckOutDate.HasValue
            ? $"{CheckInDate.Value:dd.MM.yyyy} - {CheckOutDate.Value:dd.MM.yyyy}"
            : "Не указано";
    }
}
