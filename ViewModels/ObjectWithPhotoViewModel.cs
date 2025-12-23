using RealEstateAgency.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace RealEstateAgency.ViewModels
{
    public class ObjectWithPhotoViewModel : ViewModelBase
    {
        private readonly ObjectModel _objectModel;

        public ObjectWithPhotoViewModel(ObjectModel objectModel)
        {
            _objectModel = objectModel;
        }

        public int Id => _objectModel.Id;
        public int? RoomCount => _objectModel.RoomCount;
        public decimal? Area => _objectModel.Area;
        public string Address => _objectModel.Address;
        public decimal? Price => _objectModel.Price;
        public string StatusName => _objectModel.StatusName;
        public string TypeOfSubjectName => _objectModel.TypeOfSubjectName;
        public string TypeOfDealName => _objectModel.TypeOfDealName;
        public string OwnerName => _objectModel.OwnerName;
        
        public string MainPhotoUrl => _objectModel.MainPhotoUrl;

        public ObservableCollection<string> PhotoUrls
        {
            get => new ObservableCollection<string> { _objectModel.MainPhotoUrl };
        }

        public ObjectModel GetObjectModel() => _objectModel;
    }
}