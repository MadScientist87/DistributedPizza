using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DistributedPizza.Core;
using DistributedPizza.Core.Data;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Grains;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Ninject;

namespace DistributedPizza.Tests.OrleansTests
{
    [TestClass]
    public class TestOrleansSilo
    {

        const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;

        [TestMethod]
        public void TestMethod1()
        {
            Task.Run(async () =>
            {
                using (var client = await StartClientWithRetries())
                {
                    await DoClientWork(client);
                }
                // Actual test code here.
            }).GetAwaiter().GetResult();

        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            attempt = 0;
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .Build();

            await client.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            attempt++;
            Console.WriteLine($"Cluster client attempt {attempt} of {initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }

        private static async Task DoClientWork(IClusterClient client)
        {


            var pizzas = new List<Pizza>();
            pizzas.Add(new Pizza { Id = 1, Status = PizzaStatus.Bake });
            pizzas.Add(new Pizza { Id = 2, Status = PizzaStatus.Bake });
            var order = new Order {CustomerName = "test", Pizza = pizzas };
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IOrderGrain>(0);
            var response = await friend.SetupOrder(order);
            Console.WriteLine("\n\n{0}\n\n", response);
        }
    }
}
