using GarbageMap.Models.DbModels;

namespace GarbageMap.Models.ViewModels
{
    public class CameraPointViewModel
    {
        public CameraPlace CameraPlace { get; set; }
        public int NumberOfCans { get; set; }
        public int TotalCapacity { get; set; }
        public double FulfiledPercentage { get; set; }
    }
}
