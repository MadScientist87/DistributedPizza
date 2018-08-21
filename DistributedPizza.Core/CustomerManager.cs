using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedPizza.Core
{
    public class CustomerManager
    {
        private readonly BetterRandom random;
        public CustomerManager(BetterRandom random)
        {
            this.random = random;
        }

        public Customer GetRandomCustomer(int index)
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerName = "Chantelle Patricia",
                    CustomerPhoneNumber = "555-3342"
                },
                    new Customer
                {
                    CustomerName = "Grover Butterfield",
                    CustomerPhoneNumber = "555-2244"
                },
                 new Customer
                {
                    CustomerName = "Holley Detwiler",
                    CustomerPhoneNumber = "555-1112"
                },
                  new Customer
                {
                    CustomerName = "Shelly Harnden",
                    CustomerPhoneNumber = "555-3433"
                },
                   new Customer
                {
                    CustomerName = "Lisa Lydon",
                    CustomerPhoneNumber = "555-5678"
                },
                 new Customer
                {
                    CustomerName = "Lu Ziemann",
                    CustomerPhoneNumber = "555-1397"
                }
            };
            
            var customer = customers[index];

            return customer;
        }
    }

    public class Customer
    {
        public string CustomerName { get; set; }

        public string CustomerPhoneNumber { get; set; }
    }
}
