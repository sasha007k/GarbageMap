namespace GarbageMap.Models.DbModels
{
    public class District
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public Region Region { get; set; }
        public int RegionId { get; set; }
    }
}
