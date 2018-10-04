using System;
using System.Collections.Generic;
using System.Linq;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.StateMachines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DistributedPizza.Tests.StateMachines
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pizzas = new List<Pizza>();
            pizzas.Add(new Pizza { Id = 1, Status = PizzaStatus.Bake });
            pizzas.Add(new Pizza { Id = 2, Status = PizzaStatus.Bake });
            var order = new Order { CustomerName = "test", Pizza = pizzas };

            var pizzaStatemachine = new PizzaStateMachine(pizzas.First());
            //pizzaStatemachine.Fire(Up);
        }
    }
}
