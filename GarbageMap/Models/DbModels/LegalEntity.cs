namespace GarbageMap.Models.DbModels
{
    public class LegalEntity
    {
        public int Id { get; set; }
        public string LegalName { get; set; }
        public int LegalNumber { get; set; }
        public string Classification { get; set; }
        public Address Address { get; set; }

    }
}
