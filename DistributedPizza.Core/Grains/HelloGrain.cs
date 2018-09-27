using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.StateMachines;
using Microsoft.Extensions.Logging;

namespace DistributedPizza.Core.Grains
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
    public class HelloGrain : Orleans.Grain, IHello
    {
        private readonly ILogger _logger;
        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this._logger = logger;
        }

        Task<string> IHello.SayHello(string greeting)
        {
            var order = new Order {Status = Status.Started};
            var orderStateMachine = new OrderStateMachine(order);
            orderStateMachine.Fire(Trigger.UpdateOrder);

            _logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"You said: '{greeting}', I say: Hello! {order.Status}");
        }
    }
}
