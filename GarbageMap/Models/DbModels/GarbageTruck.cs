namespace GarbageMap.Models.DbModels
{
    public class GarbageTruck
    {
        public int Id { get; set; }
        public LegalEntity LegalEntity { get; set; }
        public IndividualPerson Driver { get; set; }
        public int Capacity { get; set; }
    }
}
