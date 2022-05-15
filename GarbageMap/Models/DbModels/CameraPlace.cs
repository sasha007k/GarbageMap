namespace GarbageMap.Models.DbModels
{
    public class CameraPlace
    {
        public int Id { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string PathToImage { get; set; }
        public IndividualPerson Organization { get; set; }
        public string OrganizationId { get; set; }

        public override string ToString()
        {
            var address = $"{Address.City.Region.Name}, {Address.City.Name}, {Address.Street}";
            if (Address.HouseNumber > 0)
            {
                address += $" {Address.HouseNumber}";
            }
            return address;
        }
    }
}
