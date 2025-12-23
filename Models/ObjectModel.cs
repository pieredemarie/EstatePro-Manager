using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Models
{
    public class ObjectModel
    {
        public int Id { get; set; }
        public int? RoomCount { get; set; }
        public decimal? Area { get; set; }
        public string Address { get; set; }
        public decimal? Price { get; set; }
        public string StatusName { get; set; }
        public string TypeOfSubjectName { get; set; }
        public string TypeOfDealName { get; set; }
        public string OwnerName { get; set; }
        public string MainPhotoUrl { get; set; }
    }
}
