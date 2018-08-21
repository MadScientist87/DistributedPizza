using System;
using System.Collections.Generic;
using System.Reflection;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace DistributedPizza.Tests.Orders
{
    [TestClass]
    public class OrderBusinessLogic
    {
        [TestMethod]
        public void GenerateRandomOrder()
        {
            IKernel _Kernal = new StandardKernel();
            _Kernal.Load(Assembly.GetExecutingAssembly());
            IDistributedPizzaDbContext dbContext = _Kernal.Get<DistributedPizzaDbContext>();
            BetterRandom random = new BetterRandom();
            var orderManager = new OrderManager(dbContext, random);
            
            var order = orderManager.GenerateRandomOrder();
            var order2 = orderManager.GenerateRandomOrder();
        }

        [TestMethod]
        public void GenerateRandomCustomer()
        {
            var random= new BetterRandom();
            var customerManager = new CustomerManager(random);
            List<Customer> customers = new List<Customer>();
            for (var x = 0; x < 9; x++)
            {
                var customer = customerManager.GetRandomCustomer(random.Next(0, 6));
                customers.Add(customer);
            }
            var count = customers.Count;
        }
    }
}
