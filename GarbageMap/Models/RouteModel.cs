using System.Collections.Generic;

namespace GarbageMap.Models
{
    public class RouteModel
    {
        public PointModel StartPoint { get; set; }
        public PointModel EndPoint { get; set; }
        public List<PointModel> Points { get; set; }
    }
}
