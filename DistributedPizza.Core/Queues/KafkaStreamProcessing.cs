using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace DistributedPizza.Core.Queues
{
    public class KafkaStreamProcessing : IStreamProcessingQueue
    {
        public void QueueOrder(Order order)
        {
            try
            {
                var orderJson = JsonConvert.SerializeObject(order);
                string payload = orderJson;
                string topic = "PizzaOrderTopic";

                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
                using (var p = new Producer<Null, string>(config))
                {

                    try
                    {
                        Task.Run(async () =>
                            {
                                var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value = payload });
                                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");

                            });
                    }
                    catch (KafkaException e)
                    {
                         Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void RetrieveOrders(int? messagesToRetreive = null, CancellationToken? token = null)
        {
            try
            {
                var count = 0;
                var orders = new List<Order>();

                var conf = new ConsumerConfig
                {
                    GroupId = "PizzaOrderTopicGroup",
                    BootstrapServers = "localhost:9092",
                    // Note: The AutoOffsetReset property determines the start offset in the event
                    // there are not yet any committed offsets for the consumer group for the
                    // topic/partitions of interest. By default, offsets are committed
                    // automatically, so in this example, consumption will only start from the
                    // eariest message in the topic 'my-topic' the first time you run the program.
                    AutoOffsetReset = AutoOffsetResetType.Earliest
                };
                using (var c = new Consumer<Ignore, string>(conf))
                {
                    c.Subscribe("PizzaOrderTopic");

                    bool consuming = true;
                    // The client will automatically recover from non-fatal errors. You typically
                    // don't need to take any action unless an error is marked as fatal.
                    c.OnError += (_, e) => consuming = !e.IsFatal;

                    while (consuming)
                    {
                        if (token != null && ((CancellationToken)token).IsCancellationRequested || (messagesToRetreive != null && count >= messagesToRetreive))
                        {
                            break;
                        }
                        try
                        {
                            var cr = c.Consume();
                            var order = JsonConvert.DeserializeObject<Order>(cr.Value);
                            orders.Add(order);
                            Task.Run(async () => { await SiloManager.StartOrder(order); });
                            count++;
                            // Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            // Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }

                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine(e);
            }


            //return orders;
        }
    }
}
