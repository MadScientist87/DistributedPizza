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
using Newtonsoft.Json;

namespace DistributedPizza.Core.Queues
{
    public class KafkaStreamProcessing : IStreamProcessingQueue
    {
        public async Task QueueOrder(Order order)
        {
            var orderJson = JsonConvert.SerializeObject(order);
            string payload = orderJson;
            string topic = "PizzaOrderTopic";
            Message msg = new Message(payload);
            Uri uri = new Uri(ConfigurationManager.AppSettings.GetValueOrDefault("KafkaQueueUrl", "http://localhost:9092"));
            var options = new KafkaOptions(uri);
            var router = new BrokerRouter(options);
            var client = new Producer(router);
            await client.SendMessageAsync(topic, new List<Message> { msg });
        }

        public void RetrieveOrders(int? messagesToRetreive = null, CancellationToken? token = null)
        {
            var count = 0;
            var orders = new List<Order>();
            string topic = "PizzaOrderTopic";
            Uri uri = new Uri(ConfigurationManager.AppSettings.GetValueOrDefault("KafkaQueueUrl", "http://localhost:9092"));
            var options = new KafkaOptions(uri);

            var router = new BrokerRouter(options);
            var consumer = new Consumer(new ConsumerOptions(topic, router));
            foreach (var message in consumer.Consume(token))
            {
                var order = JsonConvert.DeserializeObject<Order>(Encoding.UTF8.GetString(message.Value));
                orders.Add(order);
               Task.Run(async () => { await SiloManager.StartOrder(order); });


                count++;

                //if (messagesToRetreive != null && count >= messagesToRetreive)
                //    break;
            }

            //return orders;
        }
    }
}
