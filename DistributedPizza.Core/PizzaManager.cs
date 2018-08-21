using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using Ninject;

namespace DistributedPizza.Core
{
    public class PizzaManager
    {
        readonly IDistributedPizzaDbContext distributedPizzaDbContext;
        private readonly Random random;
        [Inject]
        public PizzaManager(IDistributedPizzaDbContext distributedPizzaDbContext, Random random)
        {
            this.distributedPizzaDbContext = distributedPizzaDbContext;
            this.random = new Random();
        }

        public List<Pizza> GetRandomPizzas()
        {
            List<Pizza> pizzas = new List<Pizza>();
            int pizzasTogenerate = random.Next(1, 5);

            for (var x = 0; x < pizzasTogenerate; x++)
            {
                pizzas.Add(GetRandomPizza());
            }

            return pizzas;
        }

        private Pizza GetRandomPizza()
        {
            var pizza = new Pizza
            {
                SauceType = Extensions.Randomize<SauceType>(),
                Size = Extensions.Randomize<Size>(),
                Toppings = GetRandomToppings()
            };

            return pizza;
        }

        private List<Toppings> GetRandomToppings()
        {
            List<Toppings> toppings = new List<Toppings>();
            int toppingsTogenerate = random.Next(1, 3);

            for (var x = 0; x < toppingsTogenerate; x++)
            {
                toppings.Add(GetRandomTopping());
            }

            return toppings;
        }
        private Toppings GetRandomTopping()
        {
            var toppings = this.distributedPizzaDbContext.Toppings.ToList();
            int index = random.Next(toppings.Count);
            var topping = toppings[index];
            return topping;
        }
    }
}
