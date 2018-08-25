using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;

namespace DistributedPizza.Core.Data.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string OrderReferenceId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public PizzaDTO[] Pizza { get; set; }
    }

    public class PizzaDTO
    {
        public int Id { get; set; }
        public SauceType SauceType { get; set; }
        public Size Size { get; set; }
        public ToppingDTO[] Toppings { get; set; }
    }

    public class ToppingDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
