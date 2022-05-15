using System.ComponentModel.DataAnnotations;

namespace GarbageMap.Models.ViewModels
{
    public class AddCameraViewModel
    {
        [Required]
        public int RegionIndex { get; set; }
        public int DistrictIndex { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public string Street { get; set; }

        [Display(Name = "House number")]
        public int HouseNumber { get; set; }
    }
}
