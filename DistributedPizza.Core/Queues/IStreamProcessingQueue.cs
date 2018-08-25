using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;

namespace DistributedPizza.Core.Queues
{
    public interface IStreamProcessingQueue
    {
        void QueueOrder(Order order);
        List<Order> RetrieveOrders(int? messagesToRetreive = null);
    }
}
