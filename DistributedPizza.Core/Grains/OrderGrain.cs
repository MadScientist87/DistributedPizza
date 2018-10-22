using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Data.Models;
using DistributedPizza.Core.StateMachines;
using DistributedPizza.Tests;
using Microsoft.AspNet.SignalR.Client;
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
        private readonly IMapper _mapper;
        private Order _order;

        public OrderGrain(ILogger<OrderGrain> logger, IMapper mapper)
        {
            this._logger = logger;
            _mapper = mapper;


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
            if (order.ReportBackToClient)
                OrderAPI.BroadCastMessage($"The status of your order for order number {order.OrderReferenceId} is {order.Status}.");
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
            if (orderStateMachine.CanFire(Trigger.UpdateOrder) && _order.Status <= Status.ReadyForDelivery)
            {
                orderStateMachine.Fire(Trigger.UpdateOrder);
                UpdateOrderForAPI(_order);
            }

            if (_order.Status == Status.Delivering)
            {
                if (orderStateMachine.CanFire(Trigger.UpdateOrder))
                {
                    orderStateMachine.Fire(Trigger.UpdateOrder);
                    if (_order.Status != Status.Delivered)
                        UpdateOrderForAPI(_order);
                    else
                    {
                        UpdateOrderForAPI(_order, false);
                    }
                }

                if (_order.Status == Status.Delivered)
                {
                    UpdateOrderForAPI(_order);
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

        private void UpdateOrderForAPI(Order order, bool sendsignalr = true)
        {


            OrderDTO orderDTO = _mapper.Map<Order, OrderDTO>(order);

            OrderAPI.UpdateOrder(orderDTO);
            try
            {
                if (sendsignalr && order.ReportBackToClient)
                    OrderAPI.BroadCastMessage($"The status of your order for order number {order.OrderReferenceId} is {order.Status}.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<OrderDTO, Order>().ReverseMap();
        }
    }
}
