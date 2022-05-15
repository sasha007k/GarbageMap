using System.Globalization;

namespace GarbageMap.Models
{
    public class PointModel
    {
        public int CameraPlaceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            return $"{Latitude.ToString(nfi)},{Longitude.ToString(nfi)}";
        }
    }
}
