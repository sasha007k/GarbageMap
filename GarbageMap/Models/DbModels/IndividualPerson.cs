using Microsoft.AspNetCore.Identity;

namespace GarbageMap.Models.DbModels
{
    public class IndividualPerson : IdentityUser
    {
        public string FullName { get; set; }
        public Address Address { get; set; }
    }
}
