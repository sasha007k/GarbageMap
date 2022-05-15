using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarbageMap.Models.DbModels;

namespace GarbageMap.Models.Initializer
{
    public class RegionsInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (context?.Regions != null && !context.Regions.Any())
            {
                var regions = new List<Region>
                {
                    new Region() { Index = 71, Name = "Cherkasy Oblast"},
                    new Region() { Index = 74, Name = "Chernihiv Oblast"},
                    new Region() { Index = 73, Name = "Chernivtsi Oblast"},
                    new Region() { Index = 12, Name = "Dnipropetrovsk Oblast"},
                    new Region() { Index = 14, Name = "Donetsk Oblast"},
                    new Region() { Index = 26, Name = "Ivano-Frankivsk Oblast"},
                    new Region() { Index = 63, Name = "Kharkiv Oblast"},
                    new Region() { Index = 65, Name = "Kherson Oblast"},
                    new Region() { Index = 68, Name = "Khmelnytskyi Oblast"},
                    new Region() { Index = 32, Name = "Kyiv Oblast"},
                    new Region() { Index = 35, Name = "Kirovohrad Oblast"},
                    new Region() { Index = 44, Name = "Luhansk Oblast"},
                    new Region() { Index = 46, Name = "Lviv Oblast"},
                    new Region() { Index = 48, Name = "Mykolaiv Oblast"},
                    new Region() { Index = 51, Name = "Odessa Oblast"},
                    new Region() { Index = 53, Name = "Poltava Oblast"},
                    new Region() { Index = 56, Name = "Rivne Oblast"},
                    new Region() { Index = 59, Name = "Sumy Oblast"},
                    new Region() { Index = 61, Name = "Ternopil Oblast"},
                    new Region() { Index = 05, Name = "Vinnytsia Oblast"},
                    new Region() { Index = 07, Name = "Volyn Oblast"},
                    new Region() { Index = 21, Name = "Zakarpattia Oblast"},
                    new Region() { Index = 23, Name = "Zaporizhzhia Oblast"},
                    new Region() { Index = 18, Name = "Zhytomyr Oblast"},
                };

                await context.Regions.AddRangeAsync(regions);
                await context.SaveChangesAsync();
            }
        }
    }
}
