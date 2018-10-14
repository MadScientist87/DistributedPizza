using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using DistributedPizza.Core.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;

namespace DistributedPizza.Core
{
    public class SiloManager
    {
        public static async Task<bool> StartOrder(Order order)
        {
            using (var client = await StartClientWithRetries())
            {
                var friend = client.GetGrain<IOrderGrain>(order.Id);
                var response = await friend.SetupOrder(order);
                return response;
            }
        }

        const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;
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
    }
}
