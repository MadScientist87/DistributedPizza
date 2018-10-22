using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using Newtonsoft.Json;
using Ninject;
using RestSharp;

namespace DistributedPizza.Core
{
    public class OrderManager
    {
        private readonly BetterRandom random;
        List<Toppings> _toppingsFromDB = new List<Toppings>();
        [Inject]
        public OrderManager(List<Toppings> toppingsFromDB, BetterRandom random)
        {
            _toppingsFromDB = toppingsFromDB;
            this.random = random;
        }

        public Order GenerateRandomOrder()
        {
            var customerManager = new CustomerManager(random);

            var pizzaManger = new PizzaManager(_toppingsFromDB, random);
            var customer = customerManager.GetRandomCustomer(random.Next(0, 6));
            var randomPizzas = pizzaManger.GetRandomPizzas();

            var order = new Order
            {
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhoneNumber,
                Pizza = randomPizzas
            };

            return order;
        }

        public void GenerateNextOrderNumber(Order order)
        {
            order.OrderReferenceId = GetNextSeq();
        }


        private string GetNextSeq()
        {
            //var client = new RestClient("http://localhost/distributedpizza.web.api/");
            //var request = new RestRequest("/api/orders/GetNextSeq", Method.POST);

            //request.RequestFormat = DataFormat.Json;

            //var response = client.ExecuteAsync(request).w;
            //var prefixdto = JsonConvert.DeserializeObject<PrefixDTO>(response.Result.Content);
            //return prefixdto.OrderId;
            return string.Empty;
        }


    }
}
