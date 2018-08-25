using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DistributedPizza.Core.Data.Entities;
using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;

namespace DistributedPizza.Core.Queues
{
    public class KafkaStreamProcessing : IStreamProcessingQueue
    {
        public void QueueOrder(Order order)
        {
            var orderJson = new JavaScriptSerializer().Serialize(order);
            string payload = orderJson;
            string topic = "PizzaOrderTopic";
            Message msg = new Message(payload);
            Uri uri = new Uri(ConfigurationManager.AppSettings.GetValueOrDefault("KafkaQueueUrl", "http://localhost:9092"));
            var options = new KafkaOptions(uri);
            var router = new BrokerRouter(options);
            var client = new Producer(router);
            client.SendMessageAsync(topic, new List<Message> { msg }).Wait();
        }

        public List<Order> RetrieveOrders(int? messagesToRetreive = null)
        {
            var count = 0;
            var orders = new List<Order>();
            string topic = "PizzaOrderTopic";
            Uri uri = new Uri(ConfigurationManager.AppSettings.GetValueOrDefault("KafkaQueueUrl", "http://localhost:9092"));
            var options = new KafkaOptions(uri);

            var router = new BrokerRouter(options);
            var consumer = new Consumer(new ConsumerOptions(topic, router));
            foreach (var message in consumer.Consume())
            {
                var order = new JavaScriptSerializer().Deserialize<Order>(Encoding.UTF8.GetString(message.Value));
                orders.Add(order);
                count++;

                if (messagesToRetreive != null && count >= messagesToRetreive)
                    break;
            }

            return orders;
        }
    }
}
