using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.StateMachines;
using DistributedPizza.Tests;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;

namespace DistributedPizza.Core.Grains
{

    public class OrderGrainState
    {
        public Order Order { get; set; }
    }
    public interface IOrderGrain : Orleans.IGrainWithIntegerKey
    {
        Task<bool> SetupOrder(Order order);
        Task<bool> ReceiveMessage(string message);
        Task<bool> UpdateOrder(Pizza pizza);
    }

    [StorageProvider(ProviderName = "OrleansStorage")]
    [Reentrant]
    public class OrderGrain : Orleans.Grain<OrderGrainState>, IOrderGrain
    {
        private readonly ILogger _logger;
        private Order _order;

        public OrderGrain(ILogger<OrderGrain> logger)
        {
            this._logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            // We created the utility at activation time.
            await base.OnActivateAsync();
           // await base.ReadStateAsync();

            if (State.Order != null)
                await ((IOrderGrain)this).SetupOrder(State.Order);

            _logger.LogInformation($"Order Activated");
        }

        async Task<bool> IOrderGrain.SetupOrder(Order order)
        {
            _order = order;

            _logger.LogInformation($"Entering order");

            foreach (var pizza in _order.Pizza)
            {
                var grain = GrainFactory.GetGrain<IPizzaGrain>(pizza.Id);
                // await grain.Subscribe(this);
                await grain.SetupPizza(order.Id, pizza);
            }

            //Write grain to persistent storage
            base.State.Order = order;
            // base.State.PizzasGrains = _pizzaGrains;
           // await WriteStateAsync();

            return true;
        }

        public Task<bool> ReceiveMessage(string message)
        {
            _logger.LogInformation($"{message} I got your pizza");
            return Task.FromResult(true);
        }

        public Task<bool> UpdateOrder(Pizza pizza)
        {
            var old = _order.Pizza.SingleOrDefault(x => x.Id == pizza.Id);
            if (old != null) old.Status = pizza.Status;
            if (old != null) _logger.LogInformation($"Order has been updated new status {old.Status}");
            var orderStateMachine = new OrderStateMachine(_order);
            if (orderStateMachine.CanFire(Trigger.UpdateOrder))
                orderStateMachine.Fire(Trigger.UpdateOrder);
            if (_order.Status == Status.Delivering)
            {
                if (orderStateMachine.CanFire(Trigger.UpdateOrder))
                    orderStateMachine.Fire(Trigger.UpdateOrder);
                if (_order.Status == Status.Delivered)
                {

                    _logger.LogInformation($"Order Was delivered");
                    //try
                    //{
                    //    base.ClearStateAsync();
                    //}
                    //catch (Exception e)
                    //{
                    //    _logger.LogError($"Exception clearing the grain storage: {e}");

                    //}
                }
            }

            _logger.LogInformation($"Order status {_order.Status}");
            return Task.FromResult(true);
        }
    }
}
