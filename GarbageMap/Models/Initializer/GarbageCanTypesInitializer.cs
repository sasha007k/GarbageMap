using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarbageMap.Models.DbModels;

namespace GarbageMap.Models.Initializer
{
    public class GarbageCanTypesInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (context?.GarbageCanTypes != null && !context.GarbageCanTypes.Any())
            {
                var containers = new List<GarbageCanType>()
                {
                    new GarbageCanType() { Model = "SHW-240", Capacity = 240},
                    new GarbageCanType() { Model = "SHW-250", Capacity = 250},
                    new GarbageCanType() { Model = "SHW-500", Capacity = 500},
                    new GarbageCanType() { Model = "SHW-750", Capacity = 750},
                    new GarbageCanType() { Model = "SHW-1100", Capacity = 1100},
                };

                await context.GarbageCanTypes.AddRangeAsync(containers);
                await context.SaveChangesAsync();
            }
        }
    }
}
