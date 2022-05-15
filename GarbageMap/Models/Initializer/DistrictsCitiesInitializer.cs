using GarbageMap.Models.DbModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageMap.Models.Initializer
{
    public class DistrictsCitiesInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (context?.Districts != null && !context.Districts.Any())
            {
                await InitializeDistricts(context);
            }

            if (context?.Cities != null && !context.Cities.Any())
            {
                await InitializeCities(context);
            }
        }

        private static async Task InitializeDistricts(ApplicationDbContext context)
        {
            var lvivRegion = context.Regions.Single(r => r.Index == 46);

            var districts = new List<District>()
            {
                new District() { Name = "Pustomyty District", Index = 236, Region = lvivRegion, RegionId = lvivRegion.Id }
            };

            await context.Districts.AddRangeAsync(districts);
            await context.SaveChangesAsync();
        }

        private static async Task InitializeCities(ApplicationDbContext context)
        {
            var lvivRegion = context.Regions.Single(r => r.Index == 46);
            var pustomytyDistrict = context.Districts.Single(r => r.Index == 236);

            var cities = new List<City>()
            {
                new City() {Name = "Lviv", Index = 101, Region = lvivRegion, RegionId = lvivRegion.Id },
                new City() {Name = "Pustomyty", Index = 101, District = pustomytyDistrict, DistrictId = pustomytyDistrict.Id }
            };

            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();
        }
    }
}
