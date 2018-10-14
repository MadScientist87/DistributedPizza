﻿using System;
using System.Net;
using System.Threading.Tasks;
using DistributedPizza.Core.Data;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using DistributedPizza.Core.Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Configuration;
using System.Threading;
using AutoMapper;
using DistributedPizza.Core;
using DistributedPizza.Core.Queues;

namespace DistributedPizza.Worker
{
    class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var token = new CancellationTokenSource();
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                
                IStreamProcessingQueue kafkaQueue = new KafkaStreamProcessing();
                //IStreamProcessingQueue amazonQueue = new AmazonSQSProcessingQueue();
                  Task.Run( () =>
                 {
                     kafkaQueue.RetrieveOrders(null, token.Token);
                     // amazonQueue.RetrieveOrders(null, token.Token);

                 }, token.Token);
                Console.ReadLine();
                Console.WriteLine("Shutting Down...");

                token.Dispose();
                await host.StopAsync();
                Console.WriteLine("Shut Down");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {


            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .AddAdoNetGrainStorage("OrleansStorage", options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = "Data Source=.;Database=DistributedPizzaOrleans;Trusted_Connection=true;MultipleActiveResultSets=True";
                    options.UseJsonFormat = true;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "DistributedPizzaWorker";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(OrderGrain).Assembly).WithReferences())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PizzaGrain).Assembly).WithReferences())
                .ConfigureServices(ConfigureServices);


            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            //Logger
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                MapperConfigurationHelper.Build(cfg);
            });
            var mapper= new Mapper(mapperConfig);
            NLog.LogManager.LoadConfiguration("nlog.config");
            services.TryAddSingleton<ILoggerFactory>(loggerFactory);
            services.TryAddSingleton<IMapper>(mapper);
        }
    }
}

