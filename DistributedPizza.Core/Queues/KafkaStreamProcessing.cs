using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Runtime;

namespace DistributedPizza.Core.Queues
{
    public class KafkaStreamProcessing : IStreamProcessingQueue
    {
        public void QueueOrder(Order order)
        {
            var orderJson = JsonConvert.SerializeObject(order);
            string payload = orderJson;
            string topic = "PizzaOrderTopic";

            var options = new KafkaOptions(new Uri("http://localhost:9092"), new Uri("http://localhost:9092"));
            var router = new BrokerRouter(options);
            var client = new Producer(router); ;


            try
            {
                Task.Run(async () =>
                    {
                        await client.SendMessageAsync("PizzaOrderTopic", new[] { new Message(payload) });

                    });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Delivery failed: {e}");
            }


        }

        public void RetrieveOrders(ILogger _logger, int? messagesToRetreive = null, CancellationToken? token = null)
        {
            try
            {
                var count = 0;
                var orders = new List<Order>();
                var options = new KafkaOptions(new Uri("http://localhost:9092"), new Uri("http://localhost:9092"));
                var router = new BrokerRouter(options);
                var consumer = new Consumer(new ConsumerOptions("PizzaOrderTopic", router));
                foreach (var message in consumer.Consume())
                {

                    if (token != null && ((CancellationToken)token).IsCancellationRequested || (messagesToRetreive != null && count >= messagesToRetreive))
                    {
                        break;
                    }
                    try
                    {

                        var order = JsonConvert.DeserializeObject<Order>(System.Text.Encoding.Default.GetString(message.Value));
                        orders.Add(order);
                        Task.Run(async () => { await SiloManager.StartOrder(order); });
                        count++;
                        // Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (Exception e)
                    {
                        _logger.Info($"Error occured Kafka: {e}");
                    }
                    Console.WriteLine("Response: P{0},O{1} : {2}",
                        message.Meta.PartitionId, message.Meta.Offset, message.Value);
                }

            }
            catch (Exception e)
            {
                _logger.Info($"Error occured Kafka: {e}");
                RetrieveOrders(_logger, messagesToRetreive, token);
            }


            //return orders;
        }
    }
}
