using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Tests;
using Microsoft.Extensions.Logging;
using Orleans;

namespace DistributedPizza.Core.Grains
{

    public interface IPizzaGrain : Orleans.IGrainWithIntegerKey, IGrainObserver
    {
        Task<bool> Test(IOrderGrain observer, Pizza pizza);
        Task Subscribe(IOrderGrain observer);
    }
    public class PizzaGrain : Orleans.Grain, IPizzaGrain
    {
        private readonly ILogger _logger;
        private Pizza _pizza;
        public override async Task OnActivateAsync()
        {
            // We created the utility at activation time.
            await base.OnActivateAsync();
        }

        public PizzaGrain(ILogger<PizzaGrain> logger)
        {
            this._logger = logger;
        }

        public Task Subscribe(IOrderGrain observer)
        {

            return Task.CompletedTask;
        }

        async Task<bool> IPizzaGrain.Test(IOrderGrain observer, Pizza pizza)
        {
          
            _logger.LogInformation($"I am a pizza grain");
            await observer.ReceiveMessage("hey");
            return await Task.FromResult(true);
        }
    }


}
