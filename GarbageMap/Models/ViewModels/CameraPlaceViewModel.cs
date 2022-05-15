using GarbageMap.Models.DbModels;

namespace GarbageMap.Models.ViewModels
{
    public class CameraPlaceViewModel
    {
        public CameraPlace Place { get; set; }

        public double AverageFullfill { get; set; } = 0;
    }
}
