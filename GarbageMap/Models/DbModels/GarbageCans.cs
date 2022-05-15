namespace GarbageMap.Models.DbModels
{
    public class GarbageCans
    {
        public int Id { get; set; }
        public CameraPlace CameraPlace { get; set; }
        public int CameraPlaceId { get; set; }
        public GarbageCanType GarbageCanType { get; set; }
        public int GarbageCanTypeId { get; set; }
        public int FulfiledPercentage { get; set; }
    }
}
