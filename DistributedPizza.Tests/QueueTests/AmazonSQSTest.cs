using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DistributedPizza.Tests.QueueTests
{
    [TestClass]
    public class AmazonSQSTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Setup Order
            var pizzas = new List<Pizza>();
            pizzas.Add(new Pizza { Id = 1, Status = PizzaStatus.Prep });
            pizzas.Add(new Pizza { Id = 2, Status = PizzaStatus.Prep });
            var order = new Order { CustomerName = "test", Pizza = pizzas };
            order.Id = 0;
            order.Status = Status.Started;

            IStreamProcessingQueue queue = new AmazonSQSProcessingQueue();
            queue.QueueOrder(order);
            var loggerFactory = new LoggerFactory();
           // queue.RetrieveOrders(loggerFactory.CreateLogger("queuetest"), null,1);
            // Assert.AreEqual(orders.First().CustomerName, "test");
        }
    }
}
