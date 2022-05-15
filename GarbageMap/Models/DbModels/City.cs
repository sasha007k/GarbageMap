namespace GarbageMap.Models.DbModels
{
    public class City
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public Region Region { get; set; }
        public int? RegionId { get; set; }
        public District District { get; set; }
        public int? DistrictId { get; set; }
    }
}
