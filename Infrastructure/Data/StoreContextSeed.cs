using Core.Entities;
using Core.Entities.OrderAggregate;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedData(StoreDatabaseContext context)
        {
            var path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);   
            

            if (!context.DeliveryMethods.Any())
            {
                var deliveryData = File.ReadAllText(path + @"/Data/SeedData/delivery.json");
                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);
                // ADD THIS LOOP:
                foreach (var item in methods)
                {
                    item.Id = 0; // Tells SQL Server to generate the ID automatically!
                }
                context.DeliveryMethods.AddRange(methods);
            }

            if (context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();
        }
    }
}
