using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Amazon.SQS;
using Amazon.SQS.Model;
using DistributedPizza.Core.Data.Entities;

namespace DistributedPizza.Core.Queues
{
    public class AmazonSQSProcessingQueue : IStreamProcessingQueue
    {
        private const string MyQueueUrl = "https://sqs.us-east-1.amazonaws.com/047637105360/PizzaOrderQueue";

        public void QueueOrder(Order order)
        {
            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();

            var amazonSQSClient = new AmazonSQSClient(amazonSQSConfig);
            CreateQueueRequest createQueueRequest =
                new CreateQueueRequest();
            var orderJson = new JavaScriptSerializer().Serialize(order);
            string payload = orderJson;

            createQueueRequest.QueueName = "PizzaOrderQueue";
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = MyQueueUrl,
                MessageBody = payload
            };

            SendMessageResponse sendMessageResponse =
                amazonSQSClient.SendMessage(sendMessageRequest);
        }

        public List<Order> RetrieveOrders(int? messagesToRetreive = null, CancellationToken? token = null)
        {
            var orders = new List<Order>();

            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            var count = 0;
            var amazonSQSClient = new AmazonSQSClient(amazonSQSConfig);
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest { QueueUrl = MyQueueUrl };
            while (true)
            {
                if (token != null && ((CancellationToken)token).IsCancellationRequested || (messagesToRetreive != null && count >= messagesToRetreive))
                {
                    break;
                }

                ReceiveMessageResponse receiveMessageResponse =
                    amazonSQSClient.ReceiveMessage(receiveMessageRequest);

                var result = receiveMessageResponse;
                if (result.Messages.Count != 0)
                {
                    for (int i = 0; i < result.Messages.Count; i++)
                    {
                        var order = new JavaScriptSerializer().Deserialize<Order>(result.Messages[i].Body);
                        orders.Add(order);

                        count++;
                        amazonSQSClient.DeleteMessage(MyQueueUrl, result.Messages[i].ReceiptHandle);

                        if (messagesToRetreive != null && count >= messagesToRetreive)
                        {

                            break;
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            return orders;
        }


       
    }
}
