using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;

namespace DistributedPizza.Core.Data
{
    public class DistributedPizzaDbContextSeed : System.Data.Entity.DropCreateDatabaseIfModelChanges<DistributedPizzaDbContext>
    {
        protected override void Seed(DistributedPizzaDbContext context)
        {
           if (!context.Toppings.Any())
            {
                var toppings = new List<Toppings>
                {
                    new Toppings {Name = "Pepperoni"},
                    new Toppings {Name = "Ham"},
                    new Toppings {Name = "Extra Cheese"},
                    new Toppings {Name = "Green Peppers"},
                    new Toppings {Name = "Pineapple"},
                    new Toppings {Name = "Spinach"}
                };

                toppings.ForEach(s => context.Toppings.Add(s));
                context.SaveChanges();
            }
        }
    }
}
