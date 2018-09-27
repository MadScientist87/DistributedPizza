using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.StateMachines;
using DistributedPizza.Tests;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace DistributedPizza.Core.Grains
{

    public interface IOrderGrain : Orleans.IGrainWithIntegerKey
    {
        Task<bool> SetupOrder(Order order);
        Task<bool> ReceiveMessage(string message);
    }

    [StorageProvider(ProviderName = "OrleansStorage")]
    public class OrderGrain : Orleans.Grain, IOrderGrain
    {

        private readonly ILogger _logger;
        private List<IPizzaGrain> _pizzas = new List<IPizzaGrain>();
        private Order _order;
        public OrderGrain(ILogger<OrderGrain> logger)
        {
            this._logger = logger;
        }

        private void CreatePizzaGrains()
        {

            foreach (var pizza in _order.Pizza)
            {
                var grain = GrainFactory.GetGrain<IPizzaGrain>(0);
                _pizzas.Add(grain);
            }
        }

        async Task<bool> IOrderGrain.SetupOrder(Order order)
        {
            _order = order;
            var orderStateMachine = new OrderStateMachine(order);
            orderStateMachine.Fire(Trigger.UpdateOrder);
            _logger.LogInformation($"{order.CustomerName}");
            var grain = GrainFactory.GetGrain<IPizzaGrain>(0);
            CreatePizzaGrains();
            foreach (var pgrain in _pizzas)
            {
                await pgrain.Subscribe(this);
                await pgrain.Test(this, order.Pizza.FirstOrDefault());
            }

            return true;
        }

        public Task<bool> ReceiveMessage(string message)
        {
            _logger.LogInformation($"{message} I got your pizza");
            return Task.FromResult(true);
        }
    }
}
