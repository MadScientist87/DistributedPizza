using System;
using System.Linq;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Queues;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DistributedPizza.Tests.QueueTests
{
    [TestClass]
    public class KafkaQueueTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            IStreamProcessingQueue queue =  new KafkaStreamProcessing();
            queue.QueueOrder(new Order {CustomerName = "Lisa", OrderReferenceId = "DP"});
            var orders=queue.RetrieveOrders(1);
            Assert.AreEqual(orders.First().CustomerName, "Lisa");
        }
    }
}
